using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Data
{
    public class OrionTekDbContext : DbContext
    {
        private HttpContext httpContext;

        public OrionTekDbContext(DbContextOptions options, IHttpContextAccessor _httpContextAccessor) : base(options)
        {
            httpContext = _httpContextAccessor.HttpContext;
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

            var genericEntityType = typeof(GenericEntity);
            var entityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(e => genericEntityType.IsAssignableFrom(e.ClrType));

            foreach (var entityType in entityTypes)
            {
                var entityTypeName = entityType.ClrType.Name;
                var createdByRelationship = modelBuilder.Entity(entityType.ClrType)
                    .HasOne(typeof(User), "CreatedBy")
                    .WithMany()
                    .HasForeignKey("CreatedById");

                var updatedByRelationship = modelBuilder.Entity(entityType.ClrType)
                    .HasOne(typeof(User), "ModifiedBy")
                    .WithMany()
                    .HasForeignKey("ModifiedById");

                createdByRelationship.OnDelete(DeleteBehavior.Restrict);
                updatedByRelationship.OnDelete(DeleteBehavior.Restrict);
            }

            base.OnModelCreating(modelBuilder);
        }

        private int? GetUserId()
        {
            var token = httpContext.Request.Headers["Authorization"].ToString();
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            int? userId = null;

            try
            {
                userId = int.Parse((tokenHandler.ReadToken(token) as JwtSecurityToken)?.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value);
            }
            catch
            {

            }
            return userId;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var auditableEntitySet = ChangeTracker.Entries<GenericEntity>();
            if (!auditableEntitySet.Any())
            {
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }

            var id = GetUserId();

            foreach (var auditableEntity in auditableEntitySet.Where(c => c.State == EntityState.Added || c.State == EntityState.Modified || c.State == EntityState.Deleted).ToList())
            {
                auditableEntity.Entity.ModifiedAt = DateTime.Now;
                if (auditableEntity.State == EntityState.Added)
                {
                    auditableEntity.Entity.CreatedById = id;
                }
                auditableEntity.Entity.ModifiedById = id;
            }

            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            var auditableEntitySet = ChangeTracker.Entries<GenericEntity>();
            if (!auditableEntitySet.Any())
            {
                return base.SaveChanges();
            }

            var id = GetUserId();

            foreach (var auditableEntity in auditableEntitySet.Where(c => c.State == EntityState.Added || c.State == EntityState.Modified || c.State == EntityState.Deleted).ToList())
            {
                auditableEntity.Entity.ModifiedAt = DateTime.Now;
                if (auditableEntity.State == EntityState.Added)
                {
                    auditableEntity.Entity.CreatedById = id;
                }
                auditableEntity.Entity.ModifiedById = id;
            }

            return base.SaveChanges();
        }
    }
}
