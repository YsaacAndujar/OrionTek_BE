using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class OrionTekDbContext : DbContext
    {
        public OrionTekDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PasswordRecoveryCode> PasswordRecoveryCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Direction>()
                .HasOne(direction => direction.Client)
                .WithMany(client => client.Directions)
                .HasForeignKey(direction => direction.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<PasswordRecoveryCode>()
                .HasOne(pr => pr.User)
                .WithMany(user => user.PasswordRecoveryCodes)
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
