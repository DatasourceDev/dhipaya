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
    [Migration("20181019104212_AlterConditionDay2")]
    partial class AlterConditionDay2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Dhipaya.Models.AboutUs", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<string>("Description");

                    b.Property<string>("Title");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.Property<string>("Url");

                    b.HasKey("ID");

                    b.ToTable("AboutUss");
                });

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

            modelBuilder.Entity("Dhipaya.Models.Banner", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<DateTime?>("EndDate");

                    b.Property<int>("Index");

                    b.Property<string>("MobileUrl");

                    b.Property<DateTime?>("StartDate");

                    b.Property<int>("Status");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.Property<string>("Url");

                    b.HasKey("ID");

                    b.ToTable("Banners");
                });

            modelBuilder.Entity("Dhipaya.Models.Contact", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContactNo")
                        .IsRequired();

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Information")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("Dhipaya.Models.Customer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BCryptPwd");

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

                    b.Property<int>("Channel");

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<string>("CustomerNo");

                    b.Property<int>("CustomerType")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<DateTime?>("DOB");

                    b.Property<bool>("DoSendReisterEmail");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FacebookFlag");

                    b.Property<string>("FacebookID");

                    b.Property<string>("FriendCode");

                    b.Property<string>("IDC_Address");

                    b.Property<int?>("IDC_Aumper");

                    b.Property<string>("IDC_Building");

                    b.Property<int?>("IDC_Province");

                    b.Property<int?>("IDC_Tumbon");

                    b.Property<string>("IDC_ZipCode");

                    b.Property<string>("IDCard");

                    b.Property<bool>("IIAIgnoreSyned");

                    b.Property<bool>("IIASyned");

                    b.Property<bool>("IsDhiMember");

                    b.Property<string>("LineID");

                    b.Property<string>("MoblieNo");

                    b.Property<string>("NameEn");

                    b.Property<string>("NameTh")
                        .IsRequired();

                    b.Property<int?>("PrefixEn")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<int?>("PrefixTh")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<string>("PromotionCode");

                    b.Property<string>("RefCode");

                    b.Property<bool>("SentReisterEmail");

                    b.Property<string>("SentReisterMsg");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<bool?>("Success");

                    b.Property<string>("SurNameEn");

                    b.Property<string>("SurNameTh")
                        .IsRequired();

                    b.Property<bool>("Syned");

                    b.Property<string>("TelNo");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.Property<int?>("UserID");

                    b.Property<int>("UserLevel");

                    b.Property<string>("WorkTelNo");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Dhipaya.Models.CustomerImpt", b =>
                {
                    b.Property<int>("No");

                    b.Property<string>("Aumper");

                    b.Property<string>("Building");

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<string>("CustomerNo");

                    b.Property<string>("CustomerType");

                    b.Property<DateTime?>("DOB");

                    b.Property<string>("Email");

                    b.Property<string>("FacebookFlag");

                    b.Property<string>("FacebookID");

                    b.Property<string>("FriendCode");

                    b.Property<string>("IDCard");

                    b.Property<string>("IsDhiMember");

                    b.Property<string>("Lane");

                    b.Property<string>("LineID");

                    b.Property<string>("MoblieNo");

                    b.Property<string>("NameTh");

                    b.Property<string>("Password");

                    b.Property<string>("PrefixTh");

                    b.Property<string>("Province");

                    b.Property<string>("RefCode");

                    b.Property<string>("Road");

                    b.Property<string>("Status");

                    b.Property<string>("SurNameTh");

                    b.Property<string>("Tumbon");

                    b.Property<string>("UserLevel");

                    b.Property<string>("Username");

                    b.Property<string>("VillageName");

                    b.Property<string>("VillageNo");

                    b.Property<string>("ZipCode");

                    b.HasKey("No");

                    b.ToTable("CustomerImpts");
                });

            modelBuilder.Entity("Dhipaya.Models.CustomerPoint", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<int>("CustomerChanal");

                    b.Property<int>("CustomerID");

                    b.Property<string>("Name");

                    b.Property<decimal>("Point");

                    b.Property<int?>("ProductID");

                    b.Property<int>("TransacionTypeID");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.HasKey("ID");

                    b.HasIndex("CustomerID");

                    b.ToTable("CustomerPoints");
                });

            modelBuilder.Entity("Dhipaya.Models.CustomerPrefix", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("NameEng");

                    b.HasKey("ID");

                    b.ToTable("CustomerPrefixs");
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

                    b.Property<int?>("ProvinceID");

                    b.Property<int>("Status");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.Property<string>("Url");

                    b.Property<string>("Youtube");

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

                    b.Property<string>("Description");

                    b.Property<string>("Logo");

                    b.Property<string>("RedWord");

                    b.Property<int>("Status");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.Property<string>("Url");

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

            modelBuilder.Entity("Dhipaya.Models.NewsActivity", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<string>("Description");

                    b.Property<DateTime?>("EndDate");

                    b.Property<string>("ImgUrl");

                    b.Property<int>("Index");

                    b.Property<DateTime?>("StartDate");

                    b.Property<int>("Status");

                    b.Property<string>("Title");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.Property<string>("VideoUrl");

                    b.HasKey("ID");

                    b.ToTable("NewsActivities");
                });

            modelBuilder.Entity("Dhipaya.Models.PointCondition", b =>
                {
                    b.Property<int>("ConditionID")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal?>("CalPoint");

                    b.Property<decimal?>("CalPointPurchaseAmt");

                    b.Property<string>("ConditionCode");

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<string>("Description");

                    b.Property<DateTime?>("EndDate");

                    b.Property<bool>("Gold");

                    b.Property<bool>("IsAllDay");

                    b.Property<bool>("IsForBirthday");

                    b.Property<bool>("IsFri");

                    b.Property<bool>("IsMon");

                    b.Property<bool>("IsSat");

                    b.Property<bool>("IsSun");

                    b.Property<bool>("IsThu");

                    b.Property<bool>("IsTue");

                    b.Property<bool>("IsWed");

                    b.Property<int?>("LimitedDay");

                    b.Property<int?>("LimitedMonth");

                    b.Property<int?>("LimitedOnce");

                    b.Property<int?>("LimitedWeek");

                    b.Property<string>("Name");

                    b.Property<decimal?>("Percent");

                    b.Property<int>("Period");

                    b.Property<decimal?>("Point");

                    b.Property<int>("PointType");

                    b.Property<bool>("Silver");

                    b.Property<DateTime?>("StartDate");

                    b.Property<int>("Status");

                    b.Property<int>("TransacionTypeID");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.HasKey("ConditionID");

                    b.HasIndex("TransacionTypeID");

                    b.ToTable("PointConditions");
                });

            modelBuilder.Entity("Dhipaya.Models.PointConditionProduct", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ConditionID");

                    b.Property<int>("ProductID");

                    b.HasKey("ID");

                    b.HasIndex("ConditionID");

                    b.ToTable("PointConditionProducts");
                });

            modelBuilder.Entity("Dhipaya.Models.PointConditionTier", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ConditionID");

                    b.Property<decimal?>("TierPercent");

                    b.Property<decimal?>("TierPoint");

                    b.Property<decimal?>("TierPurchaseAmtFrom");

                    b.Property<decimal?>("TierPurchaseAmtTo");

                    b.HasKey("ID");

                    b.HasIndex("ConditionID");

                    b.ToTable("PointConditionTier");
                });

            modelBuilder.Entity("Dhipaya.Models.PointTransacionType", b =>
                {
                    b.Property<int>("TransacionTypeID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<bool>("Locked");

                    b.Property<string>("Name");

                    b.Property<string>("TransacionTypeCode");

                    b.HasKey("TransacionTypeID");

                    b.ToTable("PointTransacionTypes");
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

                    b.Property<int>("LimitType");

                    b.Property<int?>("LimitedDay");

                    b.Property<int?>("LimitedMonth");

                    b.Property<int?>("LimitedWeek");

                    b.Property<string>("LogoUrl");

                    b.Property<int?>("MerchantID");

                    b.Property<int>("Period");

                    b.Property<string>("PrivilegeCondition");

                    b.Property<string>("PrivilegeDesc");

                    b.Property<string>("PrivilegeName");

                    b.Property<int?>("PrivilegeTypeID");

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

                    b.Property<string>("Youtube");

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

            modelBuilder.Entity("Dhipaya.Models.Product", b =>
                {
                    b.Property<int>("ProductID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<string>("Description");

                    b.Property<string>("ProductCode");

                    b.Property<string>("ProductName");

                    b.Property<int>("Status");

                    b.Property<int>("TransacionTypeID");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.HasKey("ProductID");

                    b.ToTable("Products");
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

            modelBuilder.Entity("Dhipaya.Models.Question", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<string>("Description");

                    b.Property<DateTime?>("EndDate");

                    b.Property<int>("Index");

                    b.Property<int?>("QuestionGroupID");

                    b.Property<DateTime?>("StartDate");

                    b.Property<int>("Status");

                    b.Property<string>("Title");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.HasKey("ID");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("Dhipaya.Models.QuestionGroup", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<string>("Description");

                    b.Property<int>("Index");

                    b.Property<string>("Name");

                    b.Property<int>("Status");

                    b.Property<string>("Update_By");

                    b.Property<DateTime?>("Update_On");

                    b.HasKey("ID");

                    b.ToTable("QuestionGroups");
                });

            modelBuilder.Entity("Dhipaya.Models.Redeem", b =>
                {
                    b.Property<int>("RedeemID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CustomerID");

                    b.Property<decimal>("Point");

                    b.Property<int>("PrivilegeID");

                    b.Property<string>("RedeemCode");

                    b.Property<DateTime?>("RedeemDate");

                    b.HasKey("RedeemID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("PrivilegeID");

                    b.ToTable("Redeems");
                });

            modelBuilder.Entity("Dhipaya.Models.ShareholderImpt", b =>
                {
                    b.Property<string>("AccountID");

                    b.Property<string>("Aumper");

                    b.Property<string>("Building");

                    b.Property<string>("Create_By");

                    b.Property<DateTime?>("Create_On");

                    b.Property<string>("Email");

                    b.Property<string>("HTelNo");

                    b.Property<string>("IDCard");

                    b.Property<string>("Lane");

                    b.Property<string>("MoblieNo");

                    b.Property<string>("NameTh");

                    b.Property<string>("PrefixTh");

                    b.Property<string>("Province");

                    b.Property<string>("Road");

                    b.Property<string>("SurNameTh");

                    b.Property<string>("Tumbon");

                    b.Property<string>("WTelNo");

                    b.Property<string>("ZipCode");

                    b.HasKey("AccountID");

                    b.ToTable("ShareholderImpts");
                });

            modelBuilder.Entity("Dhipaya.Models.ShareholderNoSendMailImpt", b =>
                {
                    b.Property<string>("AccountID");

                    b.Property<string>("Name");

                    b.HasKey("AccountID");

                    b.ToTable("ShareholderNoSendMailImpts");
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

                    b.Property<string>("Password");

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

            modelBuilder.Entity("Dhipaya.Models.CustomerPoint", b =>
                {
                    b.HasOne("Dhipaya.Models.Customer")
                        .WithMany("CustomerPoints")
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Dhipaya.Models.Merchant", b =>
                {
                    b.HasOne("Dhipaya.Models.MerchantCategory", "MerchantCategories")
                        .WithMany("Merchants")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Dhipaya.Models.PointCondition", b =>
                {
                    b.HasOne("Dhipaya.Models.PointTransacionType", "PointTransacionType")
                        .WithMany()
                        .HasForeignKey("TransacionTypeID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Dhipaya.Models.PointConditionProduct", b =>
                {
                    b.HasOne("Dhipaya.Models.PointCondition")
                        .WithMany("PointConditionProducts")
                        .HasForeignKey("ConditionID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Dhipaya.Models.PointConditionTier", b =>
                {
                    b.HasOne("Dhipaya.Models.PointCondition")
                        .WithMany("PointConditionTiers")
                        .HasForeignKey("ConditionID")
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
