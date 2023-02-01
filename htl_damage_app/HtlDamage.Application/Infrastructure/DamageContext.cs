using Bogus;
using Bogus.DataSets;
using CsvHelper;
using CsvHelper.Configuration;
using HtlDamage.Application.Dto;
using HtlDamage.Application.Model;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace HtlDamage.Application.Infrastructure
{
    public class DamageContext : DbContext
    {
        public DamageContext(DbContextOptions<DamageContext> opt) : base(opt) { }

        public DbSet<Damage> Damages => Set<Damage>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RoomCategory> RoomCategories => Set<RoomCategory>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Lesson> Lessons => Set<Lesson>();

        public void Seed()
        {
            Randomizer.Seed = new Random(187);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Encoding = Encoding.UTF8,
                Delimiter = ";",
                MissingFieldFound = null
            };

            // Users
            var users = new List<User>();
            using (var fs = File.Open(Path.Combine("Data", "User.CSV"), FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                var usersCsv = csv.GetRecords<UserDto>();
                users = usersCsv
                    .Select(r =>
                    {
                        return new User(
                                  firstName: r.FirstName,
                                  lastName: r.LastName,
                                  userName: r.UserName,
                                  schoolClass: r.SchoolClass);
                    })
                    .ToList();

                Users.AddRange(users);
                SaveChanges();
            }

            // Room Categories
            var categories = new List<RoomCategory>();
            using (var fs = File.Open(Path.Combine("Data", "Room.CSV"), FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();

                var uniqueCategories = new HashSet<string>();
                while (csv.Read())
                {
                    var category = csv.GetField("RoomCategory");
                    if (!string.IsNullOrEmpty(category) && uniqueCategories.Add(category))
                    {
                        categories.Add(new RoomCategory(name: category));
                    }
                }

                RoomCategories.AddRange(categories);
                SaveChanges();
            }

            // Rooms
            var rooms = new List<Room>();
            using (var fs = File.Open(Path.Combine("Data", "Room.CSV"), FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                var roomsCsv = csv.GetRecords<RoomDto>();
                rooms = roomsCsv
                    .Where(r => categories.Any(c => c.Name == r.RoomCategory))
                    .Select(r =>
                    {
                        var category = categories.First(c => c.Name == r.RoomCategory);

                        return new Room(
                                roomCategory: category,
                                floor: r.Floor,
                                building: r.Building,
                                roomNumber: r.RoomNumber);
                    })
                    .ToList();

                Rooms.AddRange(rooms);
                SaveChanges();
            }

            // Lessons
            var teacherId = new List<string>();
            using (var fs = File.Open(Path.Combine("Data", "User.CSV"), FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                var usersCsv = csv.GetRecords<UserDto>();
                teacherId = usersCsv
                    .Where(r => String.IsNullOrEmpty(r.SchoolClass))
                    .Select(r =>
                    {
                        return r.UserName;
                    })
                    .ToList();
            }

            string[] schoolClasses = new string[]
            {
                "3AHIF",
                "3BHIF",
                "3CHIF",
                "2AHIF",
                "2BHIF",
                "2CHIF",
                "1AHIF",
                "1BHIF",
                "1CHIF"
            };

            var lessons = new Faker<Lesson>("de").CustomInstantiator(f =>
            {
                var randomDate = f.Date.Between(start: DateTime.MinValue, end: DateTime.MaxValue);
                while (randomDate.DayOfWeek == DayOfWeek.Saturday || randomDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    randomDate = f.Date.Between(start: DateTime.MinValue, end: DateTime.MaxValue);
                }

                return new Lesson(
                    date: randomDate.Date.AddSeconds(f.Random.Int(8 * 3600, 17 * 3600)),
                    lessonNumber: f.Random.Int(1, 10),
                    schoolClass: f.Random.ListItem(schoolClasses),
                    teacherId: f.Random.ListItem(teacherId)
                );
            })
                .Generate(30)
                .OrderBy(e => e.Date)
                .ToList();

            Lessons.AddRange(lessons);
            SaveChanges();

            // Damages
            var damages = new Faker<Damage>("de").CustomInstantiator(f =>
            {
                var randomDate = f.Date.Between(start: DateTime.MinValue, end: DateTime.MaxValue);
                while (randomDate.DayOfWeek == DayOfWeek.Saturday || randomDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    randomDate = f.Date.Between(start: DateTime.MinValue, end: DateTime.MaxValue);
                }

                var damage = new Damage(
                    name: f.Lorem.Sentence(f.Random.Int(3, 10)),
                    room: f.Random.ListItem(rooms),
                    created: randomDate,
                    lastSeen: randomDate,
                    lesson: f.Random.ListItem(lessons));

                damage.Users.Add(f.Random.ListItem(users));

                return damage;
            })
                .Generate(50)
                .OrderBy(e => e.Name)
                .ToList();

            Damages.AddRange(damages);
            SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var key in entityType.GetForeignKeys())
                    key.DeleteBehavior = DeleteBehavior.Restrict;

                foreach (var prop in entityType.GetDeclaredProperties())
                {
                    if (prop.Name == "Guid")
                    {
                        modelBuilder.Entity(entityType.ClrType).HasAlternateKey("Guid");
                        prop.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                    }

                    if (prop.ClrType == typeof(string) && prop.GetMaxLength() is null) prop.SetMaxLength(255);
                    if (prop.ClrType == typeof(DateTime)) prop.SetPrecision(3);
                    if (prop.ClrType == typeof(DateTime?)) prop.SetPrecision(3);
                }
            }
        }
    }
}
