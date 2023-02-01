using Bogus;
using Bogus.DataSets;
using CsvHelper;
using CsvHelper.Configuration;
using HtlDamage.Application.Dto;
using HtlDamage.Application.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
            using (var fs = File.Open(Path.Combine("Data", "User.CSV"), FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                var usersCsv = csv.GetRecords<UserDto>();
                var users = usersCsv
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
            using (var fs = File.Open(Path.Combine("Data", "Room.CSV"), FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                var roomsCsv = csv.GetRecords<RoomDto>();
                var rooms = roomsCsv
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
