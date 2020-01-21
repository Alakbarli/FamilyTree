﻿// <auto-generated />
using System;
using GtsTask3Famly.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GtsTask3Famly.Migrations
{
    [DbContext(typeof(DB))]
    [Migration("20200121081550_AddedPerssonToken")]
    partial class AddedPerssonToken
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GtsTask3Famly.Models.Family", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Logo");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Families");
                });

            modelBuilder.Entity("GtsTask3Famly.Models.FamilyToUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FamilyId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("FamilyId");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("FamilyToUsers");
                });

            modelBuilder.Entity("GtsTask3Famly.Models.Gender", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Genders");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Male"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Female"
                        });
                });

            modelBuilder.Entity("GtsTask3Famly.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Age");

                    b.Property<DateTime>("Birthdate");

                    b.Property<int>("FamilyId");

                    b.Property<string>("Firstname")
                        .IsRequired();

                    b.Property<int>("GenderId");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Photo");

                    b.HasKey("Id");

                    b.HasIndex("FamilyId");

                    b.HasIndex("GenderId");

                    b.ToTable("People");
                });

            modelBuilder.Entity("GtsTask3Famly.Models.PersonToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<DateTime>("Date");

                    b.Property<int>("PersonId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("PersonTokens");
                });

            modelBuilder.Entity("GtsTask3Famly.Models.RelRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("GenderId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("GenderId");

                    b.ToTable("RelRoles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            GenderId = 1,
                            Name = "Father"
                        },
                        new
                        {
                            Id = 2,
                            GenderId = 2,
                            Name = "Mother"
                        },
                        new
                        {
                            Id = 3,
                            GenderId = 1,
                            Name = "Grandpa"
                        },
                        new
                        {
                            Id = 4,
                            GenderId = 2,
                            Name = "Grandma"
                        },
                        new
                        {
                            Id = 5,
                            GenderId = 1,
                            Name = "Brother"
                        },
                        new
                        {
                            Id = 6,
                            GenderId = 2,
                            Name = "Sister"
                        },
                        new
                        {
                            Id = 7,
                            GenderId = 1,
                            Name = "Grandson"
                        },
                        new
                        {
                            Id = 8,
                            GenderId = 2,
                            Name = "Granddaughter"
                        },
                        new
                        {
                            Id = 9,
                            GenderId = 1,
                            Name = "Son"
                        },
                        new
                        {
                            Id = 10,
                            GenderId = 2,
                            Name = "Daughter"
                        },
                        new
                        {
                            Id = 11,
                            GenderId = 1,
                            Name = "Husband"
                        },
                        new
                        {
                            Id = 12,
                            GenderId = 2,
                            Name = "Wife"
                        },
                        new
                        {
                            Id = 14,
                            Name = "norole"
                        },
                        new
                        {
                            Id = 15,
                            Name = "norole"
                        });
                });

            modelBuilder.Entity("GtsTask3Famly.Models.Relationship", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FamilyId");

                    b.Property<bool>("IsMain");

                    b.Property<int?>("PersonId");

                    b.Property<int?>("RelRoleId");

                    b.Property<int>("RelatedUserId");

                    b.HasKey("Id");

                    b.HasIndex("FamilyId");

                    b.HasIndex("PersonId");

                    b.HasIndex("RelRoleId");

                    b.ToTable("Relationships");
                });

            modelBuilder.Entity("GtsTask3Famly.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("Avatar");

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("Firstname")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("GenderId");

                    b.Property<string>("Lastname")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("GenderId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("GtsTask3Famly.Models.UserToPerson", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("PersonId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("PersonId")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("UserToPeople");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("GtsTask3Famly.Models.FamilyToUser", b =>
                {
                    b.HasOne("GtsTask3Famly.Models.Family", "Family")
                        .WithMany("FamilyToUsers")
                        .HasForeignKey("FamilyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GtsTask3Famly.Models.User", "User")
                        .WithOne("FamilyToUser")
                        .HasForeignKey("GtsTask3Famly.Models.FamilyToUser", "UserId");
                });

            modelBuilder.Entity("GtsTask3Famly.Models.Person", b =>
                {
                    b.HasOne("GtsTask3Famly.Models.Family", "Family")
                        .WithMany("FamilyMembers")
                        .HasForeignKey("FamilyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GtsTask3Famly.Models.Gender", "Gender")
                        .WithMany("People")
                        .HasForeignKey("GenderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GtsTask3Famly.Models.RelRole", b =>
                {
                    b.HasOne("GtsTask3Famly.Models.Gender", "Gender")
                        .WithMany("Roles")
                        .HasForeignKey("GenderId");
                });

            modelBuilder.Entity("GtsTask3Famly.Models.Relationship", b =>
                {
                    b.HasOne("GtsTask3Famly.Models.Family", "Family")
                        .WithMany("Users")
                        .HasForeignKey("FamilyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GtsTask3Famly.Models.Person", "Person")
                        .WithMany("Relationships")
                        .HasForeignKey("PersonId");

                    b.HasOne("GtsTask3Famly.Models.RelRole", "Role")
                        .WithMany("Relationships")
                        .HasForeignKey("RelRoleId");
                });

            modelBuilder.Entity("GtsTask3Famly.Models.User", b =>
                {
                    b.HasOne("GtsTask3Famly.Models.Gender", "Gender")
                        .WithMany("Users")
                        .HasForeignKey("GenderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GtsTask3Famly.Models.UserToPerson", b =>
                {
                    b.HasOne("GtsTask3Famly.Models.Person", "Person")
                        .WithOne("UserToPerson")
                        .HasForeignKey("GtsTask3Famly.Models.UserToPerson", "PersonId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GtsTask3Famly.Models.User", "User")
                        .WithOne("UserToPerson")
                        .HasForeignKey("GtsTask3Famly.Models.UserToPerson", "UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("GtsTask3Famly.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("GtsTask3Famly.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GtsTask3Famly.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("GtsTask3Famly.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
