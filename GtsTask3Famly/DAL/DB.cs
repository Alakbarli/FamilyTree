using GtsTask3Famly.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GtsTask3Famly.DAL
{
    public class DB : IdentityDbContext<User>
    {
        public DB(DbContextOptions<DB> options) : base(options)
        {

        }
        public DbSet<Person> People { get; set; }
        public DbSet<RelRole> RelRoles { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<Family> Families { get; set; }
        public DbSet<FamilyToUser> FamilyToUsers { get; set; }
        public DbSet<UserToPerson> UserToPeople { get; set; }
        public DbSet<PersonToken> PersonTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity(typeof(Gender)).HasData(
                 new Gender { Id = 1, Name = "Male" },
                 new Gender { Id = 2, Name = "Female" }
       );
            builder.Entity(typeof(RelRole)).HasData(
                new RelRole { Id = 1, GenderId = 1, Name = "Father" },
                new RelRole { Id = 2, GenderId = 2, Name = "Mother" },
                new RelRole { Id = 3, GenderId = 1, Name = "Grandpa" },
                new RelRole { Id = 4, GenderId = 2, Name = "Grandma" },
                new RelRole { Id = 5, GenderId = 1, Name = "Brother" },
                new RelRole { Id = 6, GenderId = 2, Name = "Sister" },
                new RelRole { Id = 7, GenderId = 1, Name = "Grandson" },
                new RelRole { Id = 8, GenderId = 2, Name = "Granddaughter" },
                new RelRole { Id = 9, GenderId = 1, Name = "Son" },
                new RelRole { Id = 10, GenderId = 2, Name = "Daughter" },
                new RelRole { Id = 11, GenderId = 1, Name = "Husband" },
                new RelRole { Id = 12, GenderId = 2, Name = "Wife" },
                new RelRole { Id = 14, GenderId = null, Name = "norole" },
                new RelRole { Id = 15, GenderId = null, Name = "norole" }
                );
        }
    }
}
