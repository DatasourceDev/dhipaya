using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dhipaya.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace Dhipaya.DAL
{
   public class ChFrontContext : DbContext
   {
      // Master Data
      public DbSet<AccountCode> AccountCodes { get; set; }
      public DbSet<TerminateCustomer> TerminateCustomers { get; set; }
      public DbSet<TerminateRedeem> TerminateRedeems { get; set; }
      public DbSet<TerminateCustomerPoint> TerminateCustomerPoints { get; set; }
      public DbSet<TerminateMobilePoint> TerminateMobilePoints { get; set; }
      public DbSet<TerminateCustomerClassChange> TerminateCustomerClassChanges { get; set; }
      public DbSet<TerminatePointAdjust> TerminatePointAdjusts { get; set; }
      public DbSet<TerminateUser> TerminateUsers { get; set; }

      public DbSet<MerchantCategory> MerchantCategories { get; set; }
      public DbSet<Merchant> Merchants { get; set; }
      public DbSet<Privilege> Privileges { get; set; }
      public DbSet<PrivilegeCode> PrivilegeCodes { get; set; }

      public DbSet<Province> Provinces { get; set; }
      public DbSet<Aumphur> Aumphurs { get; set; }
      public DbSet<Tumbon> Tumbons { get; set; }
      public DbSet<PrivilegeImage> PrivilegeImages { get; set; }
      public DbSet<Redeem> Redeems { get; set; }
      public DbSet<PrivilegeImpts> PrivilegeImpts { get; set; }
      public DbSet<PrivilegeCustomerClass> PrivilegeCustomerClasses { get; set; }
      public DbSet<CustomerPrefix> CustomerPrefixs { get; set; }
      public DbSet<ShareholderNoSendMailImpt> ShareholderNoSendMailImpts { get; set; }
      public DbSet<PointTransacionType> PointTransacionTypes { get; set; }
      public DbSet<Product> Products { get; set; }
      public DbSet<Contact> Contacts { get; set; }
      public DbSet<ContactBlock> ContactBlocks { get; set; }
      public DbSet<Banner> Banners { get; set; }
      public DbSet<NewsActivityGroup> NewsActivityGroups { get; set; }
      public DbSet<NewsActivity> NewsActivities { get; set; }
      public DbSet<NewsActivityImage> NewsActivityImages { get; set; }
      public DbSet<Question> Questions { get; set; }
      public DbSet<AboutUs> AboutUss { get; set; }
      public DbSet<QuestionGroup> QuestionGroups { get; set; }
      public DbSet<PointCondition> PointConditions { get; set; }
      public DbSet<PointLimit> PointLimits { get; set; }
      public DbSet<PointConditionProduct> PointConditionProducts { get; set; }
      public DbSet<PointConditionCustomerClass> PointConditionCustomerClasses { get; set; }
      public DbSet<PointConditionTier> PointConditionTiers { get; set; }
      public DbSet<PointAdjust> PointAdjusts { get; set; }
      public DbSet<Gallery> Galleries { get; set; }

      public DbSet<LogEmail> LogEmails { get; set; }
      public DbSet<MobilePoint> MobilePoints { get; set; }
      public DbSet<CustomerPoint> CustomerPoints { get; set; }
      public DbSet<Subscriber> Subscribers { get; set; }

      public DbSet<UserRole> UserRoles { get; set; }
      public DbSet<User> Users { get; set; }
      public DbSet<Page> Pages { get; set; }
      public DbSet<PageRole> PageRoles { get; set; }
      public DbSet<Customer> Customers { get; set; }
      public DbSet<CustomerClass> CustomerClasses { get; set; }
      public DbSet<CustomerClassChange> CustomerClassChanges { get; set; }
      
      public DbSet<CustomerImpt> CustomerImpts { get; set; }
      public DbSet<ShareholderImpt> ShareholderImpts { get; set; }
      public DbSet<Configuration> Configurations { get; set; }

      public ChFrontContext(DbContextOptions options) : base(options) { }

      public ChFrontContext() { }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
         {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
         }

         modelBuilder.Entity<Customer>()
             .Property(b => b.Status)
             .HasDefaultValue(UserStatusType.Active);
         modelBuilder.Entity<User>()
             .Property(b => b.Status)
             .HasDefaultValue(UserStatusType.Active);
         modelBuilder.Entity<Customer>()
            .Property(b => b.PrefixEn)
            .HasDefaultValue(1);
         modelBuilder.Entity<Customer>()
          .Property(b => b.PrefixTh)
          .HasDefaultValue(1);
         base.OnModelCreating(modelBuilder);
      }
   }
}
