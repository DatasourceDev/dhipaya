using Dapper;
using Dhipaya.DTO;
using Dhipaya.Extensions;
using Dhipaya.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dhipaya.ModelsDapper
{
   public interface IReportRepository
   {
      Task<IEnumerable<CustomerPoint>> ListCustomerPoint(ModelReportBaseDTO model);
      Task<IEnumerable<CustomerPoint>> ListCustomerPointAll(ModelReportBaseDTO model);

      Task<IEnumerable<Redeem>> ListRedeem(ModelReportBaseDTO model);
      Task<IEnumerable<Redeem>> ListRedeemAll(ModelReportBaseDTO model);

      Task<IEnumerable<CustomerClassChange>> ListClassChange(ModelReportBaseDTO model);
      Task<IEnumerable<Customer>> ListRedeemRank(ModelReportBaseDTO model);
      Task<IEnumerable<Customer>> ListInviteRank(ModelReportBaseDTO model);
   }

   public class ReportRepository : IReportRepository
   {
      private readonly IConfiguration _config;

      public ReportRepository(IConfiguration config)
      {
         _config = config;
      }

      public IDbConnection Connection
      {
         get
         {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
         }
      }

      public async Task<IEnumerable<CustomerPoint>> ListCustomerPoint(ModelReportBaseDTO model)
      {
         using (IDbConnection conn = Connection)
         {
            if (model.pmax > 0)
            {
               var sQuery = new StringBuilder();
               sQuery.Append(" DECLARE @Page int");
               sQuery.Append(" SET @Page = " + model.pno);
               sQuery.Append(" DECLARE @Amount int");
               sQuery.Append(" SET @Amount = " + model.pmax);

               sQuery.Append(" SELECT p.*, c.*, u.* FROM(");
               sQuery.Append(" SELECT p.*, ROW_NUMBER() OVER(");
               sQuery.Append(" order by p.Create_On desc");
               sQuery.Append(" ) AS rownumber FROM CustomerPoints p");
               sQuery.Append(" INNER JOIN Customers c on c.ID = p.CustomerID");
               sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID");
               sQuery.Append(" LEFT JOIN Products pd on pd.ProductID = p.ProductID");
               sQuery.Append(" WHERE 1 =1 ");


               if (model.search_product_id.HasValue)
                  sQuery.Append(" AND pd.ProductID = " + model.search_product_id);

              

               if (model.search_trantype.HasValue)
                  sQuery.Append(" AND p.TransacionTypeID = " + model.search_trantype);


               if (model.customerClassID.HasValue)
                  sQuery.Append(" AND c.CustomerClassID = " + model.customerClassID);


               if (!string.IsNullOrEmpty(model.search_sdate))
               {
                  var date = DateUtil.ToDate(model.search_sdate);
                  var datetimester = DateUtil.ToInternalDate(date);
                  sQuery.Append(" AND p.Create_On >= convert(datetime, '" + datetimester + "', 101)");
               }
               if (!string.IsNullOrEmpty(model.search_edate))
               {
                  var date = DateUtil.ToDate(model.search_edate).Value.AddDays(1);
                  var datetimester = DateUtil.ToInternalDate(date);
                  sQuery.Append(" AND p.Create_On <= convert(datetime, '" + datetimester + "', 101)");
               }

               if (!string.IsNullOrEmpty(model.search_text))
               {
                  var text = model.search_text.Replace(" ", "").ToLower().Trim();
                  sQuery.Append(" and (");
                  sQuery.Append("  REPLACE(c.NameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.SurNameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.Email,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(u.UserName,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.MoblieNo,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.IDCard,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.NameEn,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.SurNameEn,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.RefCode,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.FriendCode,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.NameTh + c.SurNameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" )");
               }
               if (model.pmax > 0)
               {
                  sQuery.Append(" ) as p");
                  sQuery.Append(" INNER JOIN Customers c on c.ID = p.CustomerID");
                  sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID");
                  sQuery.Append(" LEFT JOIN Products pd on pd.ProductID = p.ProductID");
                  sQuery.Append(" WHERE rownumber >= (@Page)* @Amount-(@Amount - 1) AND rownumber <= (@Page) * @Amount");
               }


               var sQueryCnt = sQuery.ToString().Replace("p.*, c.*, u.*", " COUNT(DISTINCT p.ID) ");
               sQueryCnt = sQueryCnt.Replace("WHERE rownumber >= (@Page)* @Amount-(@Amount - 1) AND rownumber <= (@Page) * @Amount", "");
               conn.Open();
               model.totalrow = conn.Query<int>(sQueryCnt.ToString()).FirstOrDefault();
               sQuery.Append(" order by p.Create_On desc");

               var result = await conn.QueryAsync<CustomerPoint, Customer, User, CustomerPoint>(
                                   sQuery.ToString(), (point, customer, user) =>
                                   {
                                      point.Customer = customer;
                                      point.Customer.User = user;
                                      return point;
                                   });
               conn.Close();

               return result;
            }
            return null;
         }
      }

      public async Task<IEnumerable<CustomerPoint>> ListCustomerPointAll(ModelReportBaseDTO model)
      {
         using (IDbConnection conn = Connection)
         {

            var sQuery = new StringBuilder();

            sQuery.Append(" SELECT p.*, c.*, u.* ");
            sQuery.Append(" FROM CustomerPoints p");
            sQuery.Append(" INNER JOIN Customers c on c.ID = p.CustomerID");
            sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID");
            sQuery.Append(" LEFT JOIN Products pd on pd.ProductID = p.ProductID");
            sQuery.Append(" WHERE 1 =1 ");


            if (model.search_product_id.HasValue)
               sQuery.Append(" AND pd.ProductID = " + model.search_product_id);

            if (model.search_trantype.HasValue)
               sQuery.Append(" AND p.TransacionTypeID = " + model.search_trantype);


            if (model.customerClassID.HasValue)
               sQuery.Append(" AND c.CustomerClassID = " + model.customerClassID);


            if (!string.IsNullOrEmpty(model.search_sdate))
            {
               var date = DateUtil.ToDate(model.search_sdate);
               var datetimester = DateUtil.ToInternalDate(date);
               sQuery.Append(" AND p.Create_On >= convert(datetime, '" + datetimester + "', 101)");
            }
            if (!string.IsNullOrEmpty(model.search_edate))
            {
               var date = DateUtil.ToDate(model.search_edate).Value.AddDays(1);
               var datetimester = DateUtil.ToInternalDate(date);
               sQuery.Append(" AND p.Create_On <= convert(datetime, '" + datetimester + "', 101)");
            }

            if (!string.IsNullOrEmpty(model.search_text))
            {
               var text = model.search_text.Replace(" ", "").ToLower().Trim();
               sQuery.Append(" and (");
               sQuery.Append("  REPLACE(c.NameTh,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.SurNameTh,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.Email,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(u.UserName,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.MoblieNo,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.IDCard,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.NameEn,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.SurNameEn,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.RefCode,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.FriendCode,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.NameTh + c.SurNameTh,' ','') like N'%" + text + "%'");
               sQuery.Append(" )");
            }

            sQuery.Append(" order by p.Create_On desc");

            var result = await conn.QueryAsync<CustomerPoint, Customer, User, CustomerPoint>(
                                sQuery.ToString(), (point, customer, user) =>
                                {
                                   point.Customer = customer;
                                   point.Customer.User = user;
                                   return point;
                                });
            conn.Close();

            return result;

         }
      }

      public async Task<IEnumerable<Redeem>> ListRedeem(ModelReportBaseDTO model)
      {
         using (IDbConnection conn = Connection)
         {
            if (model.pmax > 0)
            {
               var sQuery = new StringBuilder();
               sQuery.Append(" DECLARE @Page int");
               sQuery.Append(" SET @Page = " + model.pno);
               sQuery.Append(" DECLARE @Amount int");
               sQuery.Append(" SET @Amount = " + model.pmax);

               sQuery.Append(" SELECT r.*, c.* FROM(");
               sQuery.Append(" SELECT r.*, ROW_NUMBER() OVER(");
               sQuery.Append(" order by r.RedeemDate desc");
               sQuery.Append(" ) AS rownumber FROM Redeems r");
               sQuery.Append(" INNER JOIN Customers c on c.ID = r.CustomerID");
               sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID");
               sQuery.Append(" INNER JOIN Privileges p on r.PrivilegeID = p.PrivilegeID ");
               sQuery.Append(" INNER JOIN MerchantCategories mc ON p.CategoryID = mc.CategoryID");
               sQuery.Append(" WHERE 1 =1 ");

               if (model.search_category_id.HasValue)
                  sQuery.Append(" AND mc.CategoryID = " + model.search_category_id);

               if (model.search_privilege.HasValue)
                  sQuery.Append(" AND r.PrivilegeID = " + model.search_privilege);

               if (model.search_redeemtype.HasValue)
                  sQuery.Append(" AND r.RedeemType = " + (int) model.search_redeemtype);

               if (model.customerClassID.HasValue)
                  sQuery.Append(" AND c.CustomerClassID = " + model.customerClassID);


               if (!string.IsNullOrEmpty(model.search_sdate))
               {
                  var date = DateUtil.ToDate(model.search_sdate);
                  var datetimester = DateUtil.ToInternalDate(date);
                  sQuery.Append(" AND r.RedeemDate >= convert(datetime, '" + datetimester + "', 101)");
               }
               if (!string.IsNullOrEmpty(model.search_edate))
               {
                  var date = DateUtil.ToDate(model.search_edate).Value.AddDays(1);
                  var datetimester = DateUtil.ToInternalDate(date);
                  sQuery.Append(" AND r.RedeemDate <= convert(datetime, '" + datetimester + "', 101)");
               }

               if (!string.IsNullOrEmpty(model.search_text))
               {
                  var text = model.search_text.Replace(" ", "").ToLower().Trim();
                  sQuery.Append(" and (");
                  sQuery.Append("  REPLACE(c.NameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.SurNameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.Email,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(u.UserName,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.MoblieNo,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.IDCard,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.NameEn,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.SurNameEn,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.RefCode,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.FriendCode,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.NameTh + c.SurNameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(r.RedeemCode,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(r.Address,' ','') like N'%" + text + "%'");
                  sQuery.Append(" )");
               }
               if (!string.IsNullOrEmpty(model.search_equal_text))
               {
                  var text = model.search_equal_text;
                  sQuery.Append(" and (");
                  sQuery.Append(" c.NameTh = N'" + text + "'");
                  sQuery.Append(" OR  c.SurNameTh = N'" + text + "'");
                  sQuery.Append(" OR  c.Email = N'" + text + "'");
                  sQuery.Append(" OR  u.UserName = N'" + text + "'");
                  sQuery.Append(" OR  c.MoblieNo = N'" + text + "'");
                  sQuery.Append(" OR  c.IDCard = N'" + text + "'");
                  sQuery.Append(" OR  c.NameEn = N'" + text + "'");
                  sQuery.Append(" OR  c.SurNameEn = N'" + text + "'");
                  sQuery.Append(" OR  c.RefCode = N'" + text + "'");
                  sQuery.Append(" OR  c.FriendCode = N'" + text + "'");
                  sQuery.Append(" OR  c.NameTh + c.SurNameTh = N'" + text + "'");
                  sQuery.Append(" OR  r.RedeemCode = N'" + text + "'");
                  sQuery.Append(" OR  r.Address = N'" + text + "'");
                  sQuery.Append(" )");
               }

               if (model.pmax > 0)
               {
                  sQuery.Append(" ) as r");
                  sQuery.Append(" INNER JOIN Customers c on c.ID = r.CustomerID");
                  sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID");
                  sQuery.Append(" INNER JOIN Privileges p on r.PrivilegeID = p.PrivilegeID ");
                  sQuery.Append(" INNER JOIN MerchantCategories mc ON p.CategoryID = mc.CategoryID");
                  sQuery.Append(" WHERE rownumber >= (@Page)* @Amount-(@Amount - 1) AND rownumber <= (@Page) * @Amount");
               }


               var sQueryCnt = sQuery.ToString().Replace("r.*, c.*", " COUNT(DISTINCT r.RedeemID) ");
               sQueryCnt = sQueryCnt.Replace("WHERE rownumber >= (@Page)* @Amount-(@Amount - 1) AND rownumber <= (@Page) * @Amount", "");
               conn.Open();
               model.totalrow = conn.Query<int>(sQueryCnt.ToString()).FirstOrDefault();
               sQuery.Append(" order by r.RedeemDate desc");

               var result = await conn.QueryAsync<Redeem, Customer, Redeem>(
                                   sQuery.ToString(), (point, customer) =>
                                   {
                                      point.Customer = customer;
                                      return point;
                                   });
               conn.Close();

               return result;
            }
            return null;
         }
      }

      public async Task<IEnumerable<Redeem>> ListRedeemAll(ModelReportBaseDTO model)
      {
         using (IDbConnection conn = Connection)
         {

            var sQuery = new StringBuilder();

            sQuery.Append(" SELECT r.*, c.* ");
            sQuery.Append(" FROM Redeems r");
            sQuery.Append(" INNER JOIN Customers c on c.ID = r.CustomerID");
            sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID");
            sQuery.Append(" WHERE 1 =1 ");


            if (model.search_privilege.HasValue)
               sQuery.Append(" AND r.PrivilegeID = " + model.search_privilege);

            if (model.search_redeemtype.HasValue)
               sQuery.Append(" AND r.RedeemType = " + (int)model.search_redeemtype);

            if (model.customerClassID.HasValue)
               sQuery.Append(" AND c.CustomerClassID = " + model.customerClassID);


            if (!string.IsNullOrEmpty(model.search_sdate))
            {
               var date = DateUtil.ToDate(model.search_sdate);
               var datetimester = DateUtil.ToInternalDate(date);
               sQuery.Append(" AND r.RedeemDate >= convert(datetime, '" + datetimester + "', 101)");
            }
            if (!string.IsNullOrEmpty(model.search_edate))
            {
               var date = DateUtil.ToDate(model.search_edate).Value.AddDays(1);
               var datetimester = DateUtil.ToInternalDate(date);
               sQuery.Append(" AND r.RedeemDate <= convert(datetime, '" + datetimester + "', 101)");
            }

            if (!string.IsNullOrEmpty(model.search_text))
            {
               var text = model.search_text.Replace(" ", "").ToLower().Trim();
               sQuery.Append(" and (");
               sQuery.Append("  REPLACE(c.NameTh,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.SurNameTh,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.Email,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(u.UserName,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.MoblieNo,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.IDCard,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.NameEn,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.SurNameEn,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.RefCode,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.FriendCode,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(c.NameTh + c.SurNameTh,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(r.RedeemCode,' ','') like N'%" + text + "%'");
               sQuery.Append(" OR  REPLACE(r.Address,' ','') like N'%" + text + "%'");
               sQuery.Append(" )");
            }
            if (!string.IsNullOrEmpty(model.search_equal_text))
            {
               var text = model.search_equal_text;
               sQuery.Append(" and (");
               sQuery.Append(" c.NameTh = N'" + text + "'");
               sQuery.Append(" OR  c.SurNameTh = N'" + text + "'");
               sQuery.Append(" OR  c.Email = N'" + text + "'");
               sQuery.Append(" OR  u.UserName = N'" + text + "'");
               sQuery.Append(" OR  c.MoblieNo = N'" + text + "'");
               sQuery.Append(" OR  c.IDCard = N'" + text + "'");
               sQuery.Append(" OR  c.NameEn = N'" + text + "'");
               sQuery.Append(" OR  c.SurNameEn = N'" + text + "'");
               sQuery.Append(" OR  c.RefCode = N'" + text + "'");
               sQuery.Append(" OR  c.FriendCode = N'" + text + "'");
               sQuery.Append(" OR  c.NameTh + c.SurNameTh = N'" + text + "'");
               sQuery.Append(" OR  r.RedeemCode = N'" + text + "'");
               sQuery.Append(" OR  r.Address = N'" + text + "'");
               sQuery.Append(" )");
            }
            conn.Open();

            sQuery.Append(" order by r.RedeemDate desc");

            var result = await conn.QueryAsync<Redeem, Customer, Redeem>(
                                sQuery.ToString(), (point, customer) =>
                                {
                                   point.Customer = customer;
                                   return point;
                                });
            conn.Close();

            return result;

         }
      }

      public async Task<IEnumerable<CustomerClassChange>> ListClassChange(ModelReportBaseDTO model)
      {
         using (IDbConnection conn = Connection)
         {
            if (model.pmax > 0)
            {
               var sQuery = new StringBuilder();
               sQuery.Append(" DECLARE @Page int");
               sQuery.Append(" SET @Page = " + model.pno);
               sQuery.Append(" DECLARE @Amount int");
               sQuery.Append(" SET @Amount = " + model.pmax);

               sQuery.Append(" SELECT cc.*, c.*, u.* FROM(");
               sQuery.Append(" SELECT cc.*, ROW_NUMBER() OVER(");
               sQuery.Append(" order by cc.Create_On desc");
               sQuery.Append(" ) AS rownumber FROM CustomerClassChanges cc");
               sQuery.Append(" INNER JOIN Customers c on c.ID = cc.CustomerID");
               sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID");
               sQuery.Append(" WHERE 1 =1 ");


               if (model.customerClassID.HasValue)
                  sQuery.Append(" AND cc.FromID = " + model.customerClassID);

               if (model.customerClassID2.HasValue)
                  sQuery.Append(" AND cc.ToID = " + model.customerClassID2);


               if (!string.IsNullOrEmpty(model.search_sdate))
               {
                  var date = DateUtil.ToDate(model.search_sdate);
                  var datetimester = DateUtil.ToInternalDate(date);
                  sQuery.Append(" AND cc.Create_On >= convert(datetime, '" + datetimester + "', 101)");
               }
               if (!string.IsNullOrEmpty(model.search_edate))
               {
                  var date = DateUtil.ToDate(model.search_edate).Value.AddDays(1);
                  var datetimester = DateUtil.ToInternalDate(date);
                  sQuery.Append(" AND cc.Create_On <= convert(datetime, '" + datetimester + "', 101)");
               }

               if (!string.IsNullOrEmpty(model.search_text))
               {
                  var text = model.search_text.Replace(" ", "").ToLower().Trim();
                  sQuery.Append(" and (");
                  sQuery.Append("  REPLACE(c.NameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.SurNameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.Email,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(u.UserName,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.MoblieNo,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.IDCard,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.NameEn,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.SurNameEn,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.RefCode,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.FriendCode,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.NameTh + c.SurNameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" )");
               }
               if (model.pmax > 0)
               {
                  sQuery.Append(" ) as cc");
                  sQuery.Append(" INNER JOIN Customers c on c.ID = cc.CustomerID");
                  sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID");
                  sQuery.Append(" WHERE rownumber >= (@Page)* @Amount-(@Amount - 1) AND rownumber <= (@Page) * @Amount");
               }


               var sQueryCnt = sQuery.ToString().Replace("cc.*, c.*, u.*", " COUNT(DISTINCT cc.ID) ");
               sQueryCnt = sQueryCnt.Replace("WHERE rownumber >= (@Page)* @Amount-(@Amount - 1) AND rownumber <= (@Page) * @Amount", "");
               conn.Open();
               model.totalrow = conn.Query<int>(sQueryCnt.ToString()).FirstOrDefault();
               sQuery.Append(" order by cc.Create_On desc");

               var result = await conn.QueryAsync<CustomerClassChange, Customer,User, CustomerClassChange>(
                                   sQuery.ToString(), (chage, customer,user) =>
                                   {
                                      chage.Customer = customer;
                                      chage.Customer.User = user;
                                      return chage;
                                   });
               conn.Close();

               return result;
            }
            return null;
         }
      }

      public async Task<IEnumerable<Customer>> ListRedeemRank(ModelReportBaseDTO model)
      {
         using (IDbConnection conn = Connection)
         {
            if (model.pmax > 0)
            {
               var sQuery = new StringBuilder();
               sQuery.Append(" DECLARE @Page int");
               sQuery.Append(" SET @Page = " + model.pno);
               sQuery.Append(" DECLARE @Amount int");
               sQuery.Append(" SET @Amount = " + model.pmax);

               sQuery.Append(" SELECT c.*, u.*, cl.* FROM(");
               sQuery.Append(" SELECT c.*,");
               sQuery.Append(" (Select Count(*) from Redeems r where  r.CustomerID = c.ID   ) Redeemed,");
               sQuery.Append(" ROW_NUMBER() OVER(");
               sQuery.Append(" order by (Select Count(*) from Redeems r where  r.CustomerID = c.ID ) desc");
               sQuery.Append(" ) AS rownumber FROM Customers c");
               sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID ");
               sQuery.Append(" WHERE 1 =1 and  (Select Count(*) from Redeems r where  r.CustomerID = c.ID   ) > 0");
               if (model.customerClassID.HasValue)
                  sQuery.Append(" AND c.CustomerClassID = " + model.customerClassID);

               if (model.search_user_type.HasValue)
                  sQuery.Append(" AND c.UserLevel = " + (int)model.search_user_type);

               if (model.customer_chanal.HasValue)
                  sQuery.Append(" AND c.Channel = " + (int)model.customer_chanal);

              
               //if (!string.IsNullOrEmpty(model.search_sdate))
               //{
               //   var date = DateUtil.ToDate(model.search_sdate);
               //   var datetimester = DateUtil.ToInternalDate(date);
               //   sQuery.Append(" AND c.Create_On >= convert(datetime, '" + datetimester + "', 101)");
               //}
               //if (!string.IsNullOrEmpty(model.search_edate))
               //{
               //   var date = DateUtil.ToDate(model.search_edate).Value.AddDays(1);
               //   var datetimester = DateUtil.ToInternalDate(date);
               //   sQuery.Append(" AND c.Create_On <= convert(datetime, '" + datetimester + "', 101)");
               //}

               if (!string.IsNullOrEmpty(model.search_text))
               {
                  var text = model.search_text.Replace(" ", "").ToLower().Trim();
                  sQuery.Append(" and (");
                  sQuery.Append("  REPLACE(c.NameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.SurNameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.Email,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(u.UserName,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.MoblieNo,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.IDCard,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.NameEn,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.SurNameEn,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.RefCode,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.FriendCode,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.NameTh + c.SurNameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" )");
               }

               sQuery.Append(" ) as c");
               sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID ");
               sQuery.Append(" INNER JOIN CustomerClasses cl on cl.ID = c.CustomerClassID ");
               sQuery.Append(" WHERE rownumber >= (@Page)* @Amount-(@Amount - 1) AND rownumber <= (@Page) * @Amount");

               conn.Open();

               var sQueryCnt = sQuery.ToString().Replace("c.*, u.*, cl.*", " COUNT(DISTINCT c.ID) ");
               sQueryCnt = sQueryCnt.Replace("WHERE rownumber >= (@Page)* @Amount-(@Amount - 1) AND rownumber <= (@Page) * @Amount", "");
               model.totalrow = conn.Query<int>(sQueryCnt.ToString()).FirstOrDefault();
               sQuery.Append(" order by c.Redeemed desc");
               var result = await conn.QueryAsync<Customer, User, CustomerClass, Customer>(
                                   sQuery.ToString(), (customer, user, cClass) =>
                                   {
                                      customer.User = user;
                                      customer.CustomerClass = cClass;
                                      return customer;
                                   });
               conn.Close();


               return result;
            }
            return null;
         }
      }

      public async Task<IEnumerable<Customer>> ListInviteRank(ModelReportBaseDTO model)
      {
         using (IDbConnection conn = Connection)
         {
            if (model.pmax > 0)
            {
               var sQuery = new StringBuilder();
               sQuery.Append(" DECLARE @Page int");
               sQuery.Append(" SET @Page = " + model.pno);
               sQuery.Append(" DECLARE @Amount int");
               sQuery.Append(" SET @Amount = " + model.pmax);

               sQuery.Append(" SELECT c.*, u.* FROM(");
               sQuery.Append(" SELECT c.*,");
               sQuery.Append(" (Select Count(*) from CustomerPoints r  where r.TransacionTypeID  = 5 and c.ID = r.CustomerID   ) Friends,");
               sQuery.Append(" ROW_NUMBER() OVER(");
               sQuery.Append(" order by (Select Count(*) from CustomerPoints r  where r.TransacionTypeID  = 5 and c.ID = r.CustomerID ) desc");
               sQuery.Append(" ) AS rownumber FROM Customers c");
               sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID ");
               sQuery.Append(" WHERE 1 =1 and  (Select Count(*) from CustomerPoints r  where r.TransacionTypeID  = 5 and c.ID = r.CustomerID   ) > 0");

               if (model.customerClassID.HasValue)
                  sQuery.Append(" AND c.CustomerClassID = " + model.customerClassID);

               if (model.search_user_type.HasValue)
                  sQuery.Append(" AND c.UserLevel = " + (int)model.search_user_type);

               if (model.customer_chanal.HasValue)
                  sQuery.Append(" AND c.Channel = " + (int)model.customer_chanal);
           

               if (!string.IsNullOrEmpty(model.search_text))
               {
                  var text = model.search_text.Replace(" ", "").ToLower().Trim();
                  sQuery.Append(" and (");
                  sQuery.Append("  REPLACE(c.NameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.SurNameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.Email,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(u.UserName,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.MoblieNo,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.IDCard,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.NameEn,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.SurNameEn,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.RefCode,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.FriendCode,' ','') like N'%" + text + "%'");
                  sQuery.Append(" OR  REPLACE(c.NameTh + c.SurNameTh,' ','') like N'%" + text + "%'");
                  sQuery.Append(" )");
               }

               sQuery.Append(" ) as c");
               sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID ");
               sQuery.Append(" WHERE rownumber >= (@Page)* @Amount-(@Amount - 1) AND rownumber <= (@Page) * @Amount");

               conn.Open();

               var sQueryCnt = sQuery.ToString().Replace("c.*, u.*", " COUNT(DISTINCT c.ID) ");
               sQueryCnt = sQueryCnt.Replace("WHERE rownumber >= (@Page)* @Amount-(@Amount - 1) AND rownumber <= (@Page) * @Amount", "");
               model.totalrow = conn.Query<int>(sQueryCnt.ToString()).FirstOrDefault();
               sQuery.Append(" order by c.Friends desc");
               var result = await conn.QueryAsync<Customer, User, Customer>(
                                    sQuery.ToString(), (customer, user) =>
                                    {
                                       customer.User = user;
                                       return customer;
                                    });
               conn.Close();
               return result;
            }
            return null;
         }
      }
   }
}
