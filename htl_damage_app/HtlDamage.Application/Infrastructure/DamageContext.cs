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
            var fileNameUsers = Path.Combine("Data", "User.CSV");
            using (var fs = File.Open(fileNameUsers, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<UserCsv>();
                var users = csv.GetRecords<User>();
                Users.AddRange(users);
                SaveChanges();
            }

            // Room Categories
            var fileNameRooms = Path.Combine("Data", "Room.CSV");
            using (var fs = File.Open(fileNameRooms, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();

                var categories = new List<RoomCategory>();
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
            using (var fs = File.Open(fileNameRooms, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<RoomCsv>();
                var rooms = csv.GetRecords<Room>();
                
                // TODO: Add rooms into DbSet Rooms
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
