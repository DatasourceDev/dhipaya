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
    [Migration("20180722165647_AlterPrivilege2")]
    partial class AlterPrivilege2
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

                    b.ToTable("Aumphurs");
                });

            modelBuilder.Entity("Dhipaya.Models.Customer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CUR_Address");

                    b.Property<int?>("CUR_Aumper");

                    b.Property<string>("CUR_Building");

                    b.Property<string>("CUR_Lane");

                    b.Property<string>("CUR_LaneEn");

                    b.Property<int?>("CUR_Province");

                    b.Property<string>("CUR_Road");

                    b.Property<string>("CUR_RoadEn");

                    b.Property<int?>("CUR_Tumbon");

                    b.Property<string>("CUR_VillageName");

                    b.Property<string>("CUR_VillageNameEn");

                    b.Property<string>("CUR_VillageNo");

                    b.Property<string>("CUR_ZipCode");

                    b.Property<string>("Channel");

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<string>("CustomerNo");

                    b.Property<int>("CustomerType")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<DateTime?>("DOB");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FacebookFlag");

                    b.Property<string>("FriendCode");

                    b.Property<string>("IDC_Address");

                    b.Property<int?>("IDC_Aumper");

                    b.Property<string>("IDC_Building");

                    b.Property<int?>("IDC_Province");

                    b.Property<int?>("IDC_Tumbon");

                    b.Property<string>("IDC_ZipCode");

                    b.Property<string>("IDCard");

                    b.Property<bool>("IsDhiMember");

                    b.Property<string>("LineID");

                    b.Property<string>("MoblieNo");

                    b.Property<string>("NameEn");

                    b.Property<string>("NameTh")
                        .IsRequired();

                    b.Property<int>("PrefixEn")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<int>("PrefixTh")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("PromotionCode");

                    b.Property<string>("RefCode");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<bool?>("Success");

                    b.Property<string>("SurNameEn");

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

            modelBuilder.Entity("Dhipaya.Models.LogEmail", b =>
                {
                    b.Property<int>("LogID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("Msg");

                    b.Property<DateTime?>("SentDateTime");

                    b.HasKey("LogID");

                    b.ToTable("LogEmails");
                });

            modelBuilder.Entity("Dhipaya.Models.Merchant", b =>
                {
                    b.Property<int>("MerchantID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CategoryID");

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<string>("MerchantName");

                    b.Property<int>("Status");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.Property<string>("Url");

                    b.HasKey("MerchantID");

                    b.HasIndex("CategoryID");

                    b.ToTable("Merchants");
                });

            modelBuilder.Entity("Dhipaya.Models.MerchantCategory", b =>
                {
                    b.Property<int>("CategoryID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CategoryName");

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<int>("Status");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.HasKey("CategoryID");

                    b.ToTable("MerchantCategories");
                });

            modelBuilder.Entity("Dhipaya.Models.MobilePoint", b =>
                {
                    b.Property<int>("MobilePointID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Channel")
                        .IsRequired();

                    b.Property<int>("CustomerID");

                    b.Property<string>("Package")
                        .IsRequired();

                    b.Property<decimal>("Point");

                    b.Property<string>("Product")
                        .IsRequired();

                    b.Property<string>("Source")
                        .IsRequired();

                    b.HasKey("MobilePointID");

                    b.ToTable("MobilePoints");
                });

            modelBuilder.Entity("Dhipaya.Models.Privilege", b =>
                {
                    b.Property<int>("PrivilegeID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Allowable_Outlet");

                    b.Property<int?>("CategoryID");

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<decimal?>("CreditPoint");

                    b.Property<DateTime?>("EndDate");

                    b.Property<bool>("Gold");

                    b.Property<bool>("GoldLady");

                    b.Property<string>("ImgUrl");

                    b.Property<string>("LogoUrl");

                    b.Property<int?>("MaxUsage");

                    b.Property<int?>("MaxUsagePerPerson");

                    b.Property<int?>("MerchantID");

                    b.Property<string>("PrivilegeCondition");

                    b.Property<string>("PrivilegeDesc");

                    b.Property<string>("PrivilegeName");

                    b.Property<int?>("PrivilegeTypeID");

                    b.Property<string>("RenewMaxUsage");

                    b.Property<string>("RenewMaxUsagePerPerson");

                    b.Property<bool>("Silver");

                    b.Property<DateTime?>("StartDate");

                    b.Property<int>("Status");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.Property<string>("Youtube");

                    b.HasKey("PrivilegeID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("MerchantID");

                    b.HasIndex("PrivilegeTypeID");

                    b.ToTable("Privileges");
                });

            modelBuilder.Entity("Dhipaya.Models.PrivilegeImage", b =>
                {
                    b.Property<int>("PrivilegeImageID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Main");

                    b.Property<int>("PrivilegeID");

                    b.Property<string>("Url");

                    b.HasKey("PrivilegeImageID");

                    b.HasIndex("PrivilegeID");

                    b.ToTable("PrivilegeImages");
                });

            modelBuilder.Entity("Dhipaya.Models.PrivilegeImpts", b =>
                {
                    b.Property<int>("No");

                    b.Property<string>("Condition");

                    b.Property<string>("Gold");

                    b.Property<string>("Limit");

                    b.Property<string>("LimitPerPerson");

                    b.Property<string>("LimitPerPersonPeriod");

                    b.Property<string>("LimitPeriod");

                    b.Property<string>("MerchantName");

                    b.Property<string>("Outlets");

                    b.Property<DateTime?>("PeriodFrom");

                    b.Property<DateTime?>("PeriodTo");

                    b.Property<string>("PrivilegeName");

                    b.Property<string>("PrivilegeType");

                    b.Property<string>("ProvinceName");

                    b.Property<string>("Silver");

                    b.HasKey("No");

                    b.ToTable("PrivilegeImpts");
                });

            modelBuilder.Entity("Dhipaya.Models.PrivilegeMemberLevel", b =>
                {
                    b.Property<int>("PrivilegeMemberLevelID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MemberLevel");

                    b.Property<decimal?>("Percent");

                    b.Property<int?>("PrivilegeID");

                    b.Property<int>("Status");

                    b.HasKey("PrivilegeMemberLevelID");

                    b.HasIndex("PrivilegeID");

                    b.ToTable("PrivilegeMemberLevels");
                });

            modelBuilder.Entity("Dhipaya.Models.PrivilegeType", b =>
                {
                    b.Property<int>("PrivilegeTypeID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PrivilegeTypeDesc");

                    b.Property<string>("PrivilegeTypeName");

                    b.Property<int>("Status");

                    b.HasKey("PrivilegeTypeID");

                    b.ToTable("PrivilegeTypes");
                });

            modelBuilder.Entity("Dhipaya.Models.Province", b =>
                {
                    b.Property<int>("ProvinceID");

                    b.Property<int>("GeographyID");

                    b.Property<string>("ProvinceCode");

                    b.Property<string>("ProvinceName");

                    b.HasKey("ProvinceID");

                    b.ToTable("Provinces");
                });

            modelBuilder.Entity("Dhipaya.Models.Redeem", b =>
                {
                    b.Property<int>("RedeemID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CustomerID");

                    b.Property<DateTime?>("EndDate");

                    b.Property<decimal>("Point");

                    b.Property<int>("PrivilegeID");

                    b.Property<string>("RedeemCode");

                    b.Property<DateTime?>("StartDate");

                    b.HasKey("RedeemID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("PrivilegeID");

                    b.ToTable("Redeems");
                });

            modelBuilder.Entity("Dhipaya.Models.Tumbon", b =>
                {
                    b.Property<int>("TumbonID");

                    b.Property<int>("AumphurID");

                    b.Property<int>("GeographyID");

                    b.Property<string>("PostalCode");

                    b.Property<int>("ProvinceID");

                    b.Property<string>("TumbonCode");

                    b.Property<string>("TumbonName");

                    b.HasKey("TumbonID");

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

            modelBuilder.Entity("Dhipaya.Models.Customer", b =>
                {
                    b.HasOne("Dhipaya.Models.User", "User")
                        .WithMany("Customers")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Dhipaya.Models.Merchant", b =>
                {
                    b.HasOne("Dhipaya.Models.MerchantCategory", "MerchantCategories")
                        .WithMany("Merchants")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Dhipaya.Models.Privilege", b =>
                {
                    b.HasOne("Dhipaya.Models.MerchantCategory", "MerchantCategory")
                        .WithMany()
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Dhipaya.Models.Merchant", "Merchant")
                        .WithMany("Privileges")
                        .HasForeignKey("MerchantID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Dhipaya.Models.PrivilegeType", "PrivilegeType")
                        .WithMany("Privileges")
                        .HasForeignKey("PrivilegeTypeID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Dhipaya.Models.PrivilegeImage", b =>
                {
                    b.HasOne("Dhipaya.Models.Privilege", "Privilege")
                        .WithMany("PrivilegeImages")
                        .HasForeignKey("PrivilegeID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Dhipaya.Models.PrivilegeMemberLevel", b =>
                {
                    b.HasOne("Dhipaya.Models.Privilege", "Privilege")
                        .WithMany("PrivilegeMemberLevels")
                        .HasForeignKey("PrivilegeID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Dhipaya.Models.Redeem", b =>
                {
                    b.HasOne("Dhipaya.Models.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Dhipaya.Models.Privilege", "Privilege")
                        .WithMany()
                        .HasForeignKey("PrivilegeID")
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
