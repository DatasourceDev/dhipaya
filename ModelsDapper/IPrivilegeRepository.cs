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
   public interface IPrivilegeRepository
   {
      Task<IEnumerable<PrivilegeCode>> ListCode(ModelReportBaseDTO model);
   }

   public class PrivilegeRepository : IPrivilegeRepository
   {
      private readonly IConfiguration _config;
      public ILogger _logger;

      public PrivilegeRepository(IConfiguration config, ILogger<CustomerRepository> logger)
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

      public async Task<IEnumerable<PrivilegeCode>> ListCode(ModelReportBaseDTO model)
      {
         using (IDbConnection conn = Connection)
         {
            if (model.pmax == 0)
               model.pmax = 25;
            var sQuery = new StringBuilder();
            sQuery.Append(" DECLARE @Page int");
            sQuery.Append(" SET @Page = " + model.pno);
            sQuery.Append(" DECLARE @Amount int");
            sQuery.Append(" SET @Amount = " + model.pmax);

            sQuery.Append(" SELECT c.*  ");
            sQuery.Append(" FROM(");
            sQuery.Append(" SELECT c.*,");
            sQuery.Append(" ROW_NUMBER() OVER(ORDER BY c.ID desc) AS rownumber");
            sQuery.Append(" FROM PrivilegeCodes c");
            sQuery.Append(" WHERE 1 =1 ");

            if (model.search_privilege.HasValue)
               sQuery.Append(" AND c.PrivilegeID = " + model.search_privilege);

            if (!string.IsNullOrEmpty(model.search_code))
            {
               var text = model.search_code.Replace(" ", "").ToLower().Trim();
               sQuery.Append(" AND  REPLACE(c.Code,' ','') like N'%" + text + "%'");
            }
            sQuery.Append(" ) as c");
            sQuery.Append(" WHERE rownumber >= (@Page)* @Amount-(@Amount - 1) AND rownumber <= (@Page) * @Amount");

            conn.Open();

            var sQueryCnt = sQuery.ToString().Replace("c.*  ", " COUNT(DISTINCT c.ID) ");
            sQueryCnt = sQueryCnt.Replace("WHERE rownumber >= (@Page)* @Amount-(@Amount - 1) AND rownumber <= (@Page) * @Amount", "");
            model.totalrow = conn.Query<int>(sQueryCnt.ToString()).FirstOrDefault();

            sQuery.Append(" order by c.ID desc");

            var result = await conn.QueryAsync<PrivilegeCode>(sQuery.ToString());
            conn.Close();


            return result;
         }
      }
   }
}
