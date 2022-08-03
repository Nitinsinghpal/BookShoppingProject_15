using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject_15.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mook.DapperCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject.DataAccess.Repository
{
    
    public class SP_Call : ISP_Call
    {
        private readonly ApplicationDbContext _context;
        private static string ConnectionString = "";
        public SP_Call(ApplicationDbContext context)
        {
            _context = context;
            ConnectionString = _context.Database.GetDbConnection().ConnectionString;
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public void Execute<T>(string procedureName, DynamicParameters param = null)
        {
            using(SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                sqlCon.Execute(procedureName, param, commandType:CommandType.StoredProcedure);
            }
        }

        public IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null)
        {
            using(SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                return sqlCon.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);
            }

        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                var result = SqlMapper.QueryMultiple(sqlCon, procedureName, param, commandType: CommandType.StoredProcedure);

                var item1 = result.Read<T1>();
                var item2 = result.Read<T2>();

                if (item1 != null && item2 != null)
                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1,item2);

            }
            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());


        }

        public T OneRecord<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                var value = sqlCon.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);

                return value.FirstOrDefault();

            }

        }

        public T Single<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();

                return sqlCon.ExecuteScalar<T>(procedureName,param,commandType:CommandType.StoredProcedure);

            }
        }
    }
}
