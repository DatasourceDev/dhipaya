using Dapper;
using Dhipaya.DTO;
using Dhipaya.Extensions;
using Dhipaya.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dhipaya.ModelsDapper
{
   public interface ICustomerRepository
   {
      Task<Customer> GetByID(int id);
      Task<int> GetPoint(int id);
      Task<IEnumerable<Customer>> List(ModelReportBaseDTO model);
      Task<IEnumerable<Customer>> ListAll(ModelReportBaseDTO model);
   }

   public class CustomerRepository : ICustomerRepository
   {
      private readonly IConfiguration _config;
      public ILogger _logger;

      public CustomerRepository(IConfiguration config, ILogger<CustomerRepository> logger)
      {
         this._logger = logger;
         _config = config;
      }

      public IDbConnection Connection
      {
         get
         {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
         }
      }
      public async Task<Customer> GetByID(int id)
      {
         using (IDbConnection conn = Connection)
         {
            string sQuery = "SELECT * FROM Customers WHERE ID = @ID";
            conn.Open();
            var result = await conn.QueryAsync<Customer>(sQuery, new { ID = id });
            return result.FirstOrDefault();
         }
      }

      public async Task<IEnumerable<Customer>> List(ModelReportBaseDTO model)
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
               sQuery.Append(" (Select Sum( p.Point ) from CustomerPoints p where p.CustomerID = c.ID) Point, ");
               sQuery.Append(" (Select  Sum(  r.Point) from Redeems r where  r.CustomerID = c.ID) RedeemPoint,");
               sQuery.Append(" ROW_NUMBER() OVER(");
               if (!string.IsNullOrEmpty(model.orderby))
                  sQuery.Append(" order by c." + model.orderby);
               else
                  sQuery.Append(" order by c.NameTh, c.SurNameTh");
               sQuery.Append(" ) AS rownumber FROM Customers c");
               sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID ");
               sQuery.Append(" WHERE 1 =1 ");
               if (model.customerClassID.HasValue)
                  sQuery.Append(" AND c.CustomerClassID = " + model.customerClassID);

               if (model.search_user_type.HasValue)
                  sQuery.Append(" AND c.UserLevel = " + (int)model.search_user_type);

               if (model.customer_chanal.HasValue)
                  sQuery.Append(" AND c.Channel = " + (int)model.customer_chanal);

               if (model.search_birthday.HasValue)
               {
                  sQuery.Append(" AND Day(c.DOB) = " + model.search_birthday);
               }
               if (model.search_birthmonth.HasValue)
               {
                  sQuery.Append(" AND Month(c.DOB) = " + model.search_birthmonth);
               }
               if (model.search_birthyear.HasValue)
               {
                  sQuery.Append(" AND Year(c.DOB) = " + model.search_birthyear);
               }
               if (!string.IsNullOrEmpty(model.search_sdate))
               {
                  var date = DateUtil.ToDate(model.search_sdate);
                  var datetimester = DateUtil.ToInternalDate(date);
                  sQuery.Append(" AND c.Create_On >= convert(datetime, '" + datetimester + "', 101)");
               }
               if (!string.IsNullOrEmpty(model.search_edate))
               {
                  var date = DateUtil.ToDate(model.search_edate).Value.AddDays(1);
                  var datetimester = DateUtil.ToInternalDate(date);
                  sQuery.Append(" AND c.Create_On <= convert(datetime, '" + datetimester + "', 101)");
               }
               if (model.dup == 1)
               {
                  sQuery.Append(" AND c.Idcard in (select IDCard from CustomerDups)");
                  sQuery.Append(" AND c.Idcard is not null and c.idcard <> ''");
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

               sQuery.Append(" ) as c");
               sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID ");
               sQuery.Append(" INNER JOIN CustomerClasses cl on cl.ID = c.CustomerClassID ");
               sQuery.Append(" WHERE rownumber >= (@Page)* @Amount-(@Amount - 1) AND rownumber <= (@Page) * @Amount");

               conn.Open();

               var sQueryCnt = sQuery.ToString().Replace("c.*, u.*, cl.*", " COUNT(DISTINCT c.ID) ");
               sQueryCnt = sQueryCnt.Replace("WHERE rownumber >= (@Page)* @Amount-(@Amount - 1) AND rownumber <= (@Page) * @Amount", "");
               model.totalrow = conn.Query<int>(sQueryCnt.ToString()).FirstOrDefault();


               if (!string.IsNullOrEmpty(model.orderby))
                  sQuery.Append(" order by c." + model.orderby);
               else
                  sQuery.Append(" order by c.NameTh, c.SurNameTh");
               var result = await conn.QueryAsync<Customer, User, CustomerClass, Customer>(
                                   sQuery.ToString(), (customer, user, cClass) =>
                                   {
                                      //customer.Point = GetPoint(customer.ID);
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
      public async Task<IEnumerable<Customer>> ListAll(ModelReportBaseDTO model)
      {
         using (IDbConnection conn = Connection)
         {

            var sQuery = new StringBuilder();

            sQuery.Append(" SELECT c.*, u.*, cl.* FROM Customers c");
            sQuery.Append(" INNER JOIN Users u on u.ID = c.UserID ");
            sQuery.Append(" INNER JOIN CustomerClasses cl on cl.ID = c.CustomerClassID ");
            sQuery.Append(" WHERE 1 =1 ");
            if (model.customerClassID.HasValue)
               sQuery.Append(" AND c.CustomerClassID = " + model.customerClassID);

            if (model.search_user_type.HasValue)
               sQuery.Append(" AND c.UserLevel = " + (int)model.search_user_type);

            if (model.customer_chanal.HasValue)
               sQuery.Append(" AND c.Channel = " + (int)model.customer_chanal);

            if (model.search_birthday.HasValue)
            {
               sQuery.Append(" AND Day(c.DOB) = " + model.search_birthday);
            }
            if (model.search_birthmonth.HasValue)
            {
               sQuery.Append(" AND Month(c.DOB) = " + model.search_birthmonth);
            }
            if (model.search_birthyear.HasValue)
            {
               sQuery.Append(" AND Year(c.DOB) = " + model.search_birthyear);
            }
            if (!string.IsNullOrEmpty(model.search_sdate))
            {
               var date = DateUtil.ToDate(model.search_sdate);
               var datetimester = DateUtil.ToInternalDate(date);
               sQuery.Append(" AND c.Create_On >= convert(datetime, '" + datetimester + "', 101)");
            }
            if (!string.IsNullOrEmpty(model.search_edate))
            {
               var date = DateUtil.ToDate(model.search_edate).Value.AddDays(1);
               var datetimester = DateUtil.ToInternalDate(date);
               sQuery.Append(" AND c.Create_On <= convert(datetime, '" + datetimester + "', 101)");
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
            conn.Open();

            if (!string.IsNullOrEmpty(model.orderby))
               sQuery.Append(" order by c." + model.orderby);
            else
               sQuery.Append(" order by c.NameTh, c.SurNameTh");
            var result = await conn.QueryAsync<Customer, User, CustomerClass, Customer>(
                                sQuery.ToString(), (customer, user, cClass) =>
                                {
                                   //customer.Point = GetPoint(customer.ID);
                                   customer.User = user;
                                   customer.CustomerClass = cClass;
                                   return customer;
                                });
            conn.Close();


            return result;

         }
      }
      public async Task<int> GetPoint(int id)
      {
         using (IDbConnection conn = Connection)
         {
            conn.Open();

            var sQuery = new StringBuilder();
            sQuery.Append(" select(case when p.Point is null then 0 else p.Point end + case when r.Point is null then 0 else p.Point end) as Point");
            sQuery.Append(" from Customers c");
            sQuery.Append(" left join CustomerPoints p on p.CustomerID = c.ID");
            sQuery.Append(" left join Redeems r on r.CustomerID = c.ID");
            sQuery.Append(" where c.ID = @ID");
            var result = await conn.QueryAsync<int>(sQuery.ToString(), new { ID = id });
            conn.Close();
            return result.FirstOrDefault();
         }
      }
   }
}
