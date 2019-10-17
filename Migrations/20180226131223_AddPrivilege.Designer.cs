﻿// <auto-generated />
using Dhipaya.DAL;
using Dhipaya.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Dhipaya.Migrations
{
    [DbContext(typeof(ChFrontContext))]
    [Migration("20180226131223_AddPrivilege")]
    partial class AddPrivilege
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Dhipaya.Models.Aumphur", b =>
                {
                    b.Property<int>("AumphurID");

                    b.Property<string>("AumphurCode");

                    b.Property<string>("AumphurName");

                    b.Property<int>("GeographyID");

                    b.Property<int>("ProvinceID");

                    b.HasKey("AumphurID");

                    b.HasIndex("GeographyID");

                    b.HasIndex("ProvinceID");

                    b.ToTable("Aumphurs");
                });

            modelBuilder.Entity("Dhipaya.Models.Customer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CUR_Address");

                    b.Property<int?>("CUR_Aumper");

                    b.Property<string>("CUR_Building");

                    b.Property<int?>("CUR_Province");

                    b.Property<int?>("CUR_Tumbon");

                    b.Property<string>("CUR_ZipCode");

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<string>("Customer_No");

                    b.Property<string>("Customer_Type");

                    b.Property<DateTime?>("DOB");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("IDC_Address");

                    b.Property<int?>("IDC_Aumper");

                    b.Property<string>("IDC_Building");

                    b.Property<int?>("IDC_Province");

                    b.Property<int?>("IDC_Tumbon");

                    b.Property<string>("IDC_ZipCode");

                    b.Property<string>("IDCard")
                        .IsRequired();

                    b.Property<bool>("IsDhiMember");

                    b.Property<string>("LineID");

                    b.Property<string>("MoblieNo")
                        .IsRequired();

                    b.Property<string>("NameEn")
                        .IsRequired();

                    b.Property<string>("NameTh")
                        .IsRequired();

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<string>("SurNameEn")
                        .IsRequired();

                    b.Property<string>("SurNameTh")
                        .IsRequired();

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.Property<int?>("UserID");

                    b.Property<int>("UserLevel");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Dhipaya.Models.CustomerPrivilege", b =>
                {
                    b.Property<int>("CustomerPrivilegeID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Chanel");

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<decimal?>("CreditPoint");

                    b.Property<int?>("CustomerID");

                    b.Property<int?>("PrivilegeID");

                    b.Property<DateTime?>("SubmitDate");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.HasKey("CustomerPrivilegeID");

                    b.ToTable("CustomerPrivileges");
                });

            modelBuilder.Entity("Dhipaya.Models.Geography", b =>
                {
                    b.Property<int>("GeographyID");

                    b.Property<string>("GeographyName");

                    b.HasKey("GeographyID");

                    b.ToTable("Geographys");
                });

            modelBuilder.Entity("Dhipaya.Models.Merchant", b =>
                {
                    b.Property<int>("MerchantID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<string>("MerchantName");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.HasKey("MerchantID");

                    b.ToTable("Merchants");
                });

            modelBuilder.Entity("Dhipaya.Models.Privilege", b =>
                {
                    b.Property<int>("PrivilegeID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<decimal?>("CreditPoint");

                    b.Property<DateTime?>("EndDate");

                    b.Property<string>("ImgUrl");

                    b.Property<string>("LogoUrl");

                    b.Property<int?>("MerchantID");

                    b.Property<string>("PrivilegeCondition");

                    b.Property<string>("PrivilegeDesc");

                    b.Property<string>("PrivilegeName");

                    b.Property<DateTime?>("StartDate");

                    b.Property<int>("Status");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.Property<string>("Youtube");

                    b.HasKey("PrivilegeID");

                    b.HasIndex("MerchantID");

                    b.ToTable("Privileges");
                });

            modelBuilder.Entity("Dhipaya.Models.Province", b =>
                {
                    b.Property<int>("ProvinceID");

                    b.Property<int>("GeographyID");

                    b.Property<string>("ProvinceCode");

                    b.Property<string>("ProvinceName");

                    b.HasKey("ProvinceID");

                    b.HasIndex("GeographyID");

                    b.ToTable("Provinces");
                });

            modelBuilder.Entity("Dhipaya.Models.Tumbon", b =>
                {
                    b.Property<int>("TumbonID");

                    b.Property<int>("AumphurID");

                    b.Property<int>("GeographyID");

                    b.Property<int>("ProvinceID");

                    b.Property<string>("TumbonCode");

                    b.Property<string>("TumbonName");

                    b.HasKey("TumbonID");

                    b.HasIndex("AumphurID");

                    b.HasIndex("GeographyID");

                    b.HasIndex("ProvinceID");

                    b.ToTable("Tumbons");
                });

            modelBuilder.Entity("Dhipaya.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("PhoneNumber");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.Property<int>("UserRoleID");

                    b.HasKey("ID");

                    b.HasIndex("UserRoleID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Dhipaya.Models.UserRole", b =>
                {
                    b.Property<int>("UserRoleID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("RoleName");

                    b.HasKey("UserRoleID");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Dhipaya.Models.Aumphur", b =>
                {
                    b.HasOne("Dhipaya.Models.Geography", "Geography")
                        .WithMany("Aumphurs")
                        .HasForeignKey("GeographyID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Dhipaya.Models.Province", "Province")
                        .WithMany("Aumphurs")
                        .HasForeignKey("ProvinceID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Dhipaya.Models.Customer", b =>
                {
                    b.HasOne("Dhipaya.Models.User", "User")
                        .WithMany("Customers")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Dhipaya.Models.Privilege", b =>
                {
                    b.HasOne("Dhipaya.Models.Merchant", "Merchant")
                        .WithMany("Privileges")
                        .HasForeignKey("MerchantID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Dhipaya.Models.Province", b =>
                {
                    b.HasOne("Dhipaya.Models.Geography", "Geography")
                        .WithMany("Provinces")
                        .HasForeignKey("GeographyID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Dhipaya.Models.Tumbon", b =>
                {
                    b.HasOne("Dhipaya.Models.Aumphur", "Aumphur")
                        .WithMany("Tumbons")
                        .HasForeignKey("AumphurID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Dhipaya.Models.Geography", "Geography")
                        .WithMany("Tumbons")
                        .HasForeignKey("GeographyID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Dhipaya.Models.Province", "Province")
                        .WithMany("Tumbons")
                        .HasForeignKey("ProvinceID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Dhipaya.Models.User", b =>
                {
                    b.HasOne("Dhipaya.Models.UserRole", "UserRole")
                        .WithMany("Users")
                        .HasForeignKey("UserRoleID")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
