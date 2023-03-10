using Bogus;
using Bogus.DataSets;
using CsvHelper;
using CsvHelper.Configuration;
using Docker.DotNet.Models;
using HtlDamage.Application.Dto;
using HtlDamage.Application.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
        public DbSet<DamageCategory> DamageCategories => Set<DamageCategory>();
        public DbSet<DamageRecipient> DamageRecipients => Set<DamageRecipient>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<RoomCategory> RoomCategories => Set<RoomCategory>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Lesson> Lessons => Set<Lesson>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DamageReport>().HasKey(d => new { d.DamageId, d.UserId });
            modelBuilder.Entity<DamageRecipient>().HasIndex(d => new { d.DamageCategoryId, d.Email }).IsUnique();

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

        /// <summary>
        /// Initialize the database with some values (holidays, ...).
        /// Unlike Seed, this method is also called in production.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void Initialize()
        {

        }

        public async Task Seed()
        {
            Randomizer.Seed = new Random(187);
            var faker = new Faker();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Encoding = Encoding.UTF8,
                Delimiter = ";",
                MissingFieldFound = null
            };

            // Users
            var users = new List<User>();
            using (var reader = new StreamReader(Path.Combine("Data", "User.CSV"), Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                var usersCsv = csv.GetRecords<UserDto>();
                users = usersCsv
                    .Select(r =>
                    {
                        return string.IsNullOrEmpty(r.SchoolClass) switch
                        {
                            false => new Student(
                                firstName: r.FirstName,
                                lastName: r.LastName,
                                userName: r.UserName,
                                schoolClass: r.SchoolClass)
                            { Guid = faker.Random.Guid() } as User,
                            _ => new Teacher(
                                firstName: r.FirstName,
                                lastName: r.LastName,
                                userName: r.UserName)
                            { Guid = faker.Random.Guid() } as User
                        };
                    })
                    .OrderBy(u => u.UserName)
                    .ToList();

                Users.AddRange(users);
                await SaveChangesAsync();
            }

            // Rooms
            var rooms = new List<Room>();
            var roomCategories = new List<RoomCategory>();
            using (var reader = new StreamReader(Path.Combine("Data", "Room.CSV"), Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                var roomsCsv = csv.GetRecords<RoomDto>();
                rooms = roomsCsv
                    .Select(r =>
                    {
                        var category = roomCategories.FirstOrDefault(c => c.Name == r.RoomCategory);
                        if (category is null)
                        {
                            category = new RoomCategory(name: r.RoomCategory) { Guid = faker.Random.Guid() };
                            roomCategories.Add(category);
                        }

                        return new Room(
                                roomCategory: category,
                                floor: r.Floor,
                                building: r.Building,
                                roomNumber: r.RoomNumber)
                        { Guid = faker.Random.Guid() };
                    })
                    .OrderBy(r => r.RoomNumber)
                    .ToList();

                RoomCategories.AddRange(roomCategories);
                await SaveChangesAsync();

                Rooms.AddRange(rooms);
                await SaveChangesAsync();
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

            var teachers = new string[] { "SZ", "KRB", "NAI", "MIP", "ZUM" };
            var lessons = new Faker<Lesson>("de").CustomInstantiator(f =>
            {
                // Random date
                var randomDate = f.Date.Between(start: new DateTime(2022, 9, 1), end: new DateTime(2023, 6, 1));
                while (randomDate.DayOfWeek == DayOfWeek.Saturday || randomDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    randomDate = f.Date.Between(start: new DateTime(2022, 9, 1), end: new DateTime(2023, 6, 1));
                }

                // date
                var date = randomDate.Date.AddSeconds(f.Random.Int(8 * 3600, 17 * 3600));

                return new Lesson(
                    date: date,
                    lessonNumber: f.Random.Int(1, 10),
                    schoolClass: f.Random.ListItem(schoolClasses),
                    teacherId: f.Random.ListItem(teachers),
                    room: f.Random.ListItem(rooms))
                { Guid = f.Random.Guid() };
            })
                .Generate(30)
                .OrderBy(l => l.Date)
                .ToList();

            Lessons.AddRange(lessons);
            await SaveChangesAsync();

            // DamageCategories 
            var damageCategories = new DamageCategory[] {
                new DamageCategory(name: "Verschmutzung") { Guid = faker.Random.Guid() },
                new DamageCategory(name: "Reparatur") { Guid = faker.Random.Guid() },
                new DamageCategory(name: "Sicherheitsgefährdend") { Guid = faker.Random.Guid() }
            };

            DamageCategories.AddRange(damageCategories);
            await SaveChangesAsync();

            // DamageRecipients
            var emails = new string[] { "maz22374@spengergasse.at", "zhe22045@spengergasse.at", "rad22669@spengergasse.at" };

            var damageRecipients = new Faker<DamageRecipient>("de").CustomInstantiator(f =>
            {
                return new DamageRecipient(
                    email: f.Random.ListItem(emails),
                    damageCategory: f.Random.ListItem(damageCategories))
                { Guid = f.Random.Guid() };
            })
                .Generate(10)
                .GroupBy(d => new { d.Email, d.DamageCategoryId }).Select(g => g.First())
                .ToList();

            DamageRecipients.AddRange(damageRecipients);
            await SaveChangesAsync();

            // Damages
            var damages = new Faker<Damage>("de").CustomInstantiator(f =>
            {
                // Random date
                var randomDate = f.Date.Between(start: new DateTime(2022, 9, 1), end: new DateTime(2023, 6, 1));
                while (randomDate.DayOfWeek == DayOfWeek.Saturday || randomDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    randomDate = f.Date.Between(start: new DateTime(2022, 9, 1), end: new DateTime(2023, 6, 1));
                }

                // Date created
                var created = randomDate.Date.AddSeconds(f.Random.Int(8 * 3600, 17 * 3600));
                // Date lastSeen
                var lastSeen = created.AddDays(f.Random.Int(1, 3)).Date.AddSeconds(f.Random.Int(8 * 3600, 17 * 3600));

                var damage = new Damage(
                    name: f.Lorem.Sentence(f.Random.Int(3, 10)),
                    imageUrl: f.Image.PicsumUrl(),
                    room: f.Random.ListItem(rooms),
                    created: created,
                    lastSeen: lastSeen,
                    lesson: f.Random.ListItem(lessons),
                    damageCategory: f.Random.ListItem(damageCategories))
                { Guid = f.Random.Guid() };

                damage.DamageReports.Add(new DamageReport(damage, f.Random.ListItem(users), created));
                damage.DamageReports.Add(new DamageReport(damage, f.Random.ListItem(users), lastSeen));
                return damage;
            })
                .Generate(50)
                .OrderBy(e => e.Name)
                .ToList();

            Damages.AddRange(damages);
            await SaveChangesAsync();
        }

        public async Task CreateDatabase(bool isDevelopment)
        {
            if (isDevelopment) { Database.EnsureDeleted(); }
            // EnsureCreated only creates the model if the database does not exist or it has no
            // tables. Returns true if the schema was created.  Returns false if there are
            // existing tables in the database. This avoids initializing multiple times.
            if (Database.EnsureCreated()) { Initialize(); }
            if (isDevelopment) await Seed();
        }
    }
}
