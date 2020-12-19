using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelss;
using Modelss.MModels;
using System.Reflection;
namespace DataAccess_Layer
{
    
    public class DALconfig
    {
        public  readonly string connectionString;

        public FindCreteria criteria;
        
        public string StoredFunction;

        public DALconfig(string connectionString,int num)
        {
            StoredFunction = "GetInfo";
            this.connectionString = connectionString;
            criteria = new FindCreteria
            {
                count = 10,
                start = num
            };
        }

    }

    public static class SqlCommandExtensions
    {
        public static IEnumerable<TEntity> ReadAll<TEntity>(this SqlCommand command) 
            where TEntity : new()
        {
             var result=ReadAllIncaps<TEntity>(command);
             return result;
        }

        private static IEnumerable<TEntity> ReadAllIncaps<TEntity>(SqlCommand command) 
            where TEntity : new()
        {
            var reader = command.ExecuteReader();
            if (!reader.HasRows)
            {
                reader.Close();
                return Enumerable.Empty<TEntity>();

            }

            var entities = reader.ParseFromReaderIncaps<TEntity>();
            reader.Close();
            
            return entities;
        }

        private static IEnumerable<TEntity> ParseFromReaderIncaps<TEntity>(this SqlDataReader reader)
            where TEntity : new()
        {
            var entityType = typeof(TEntity);
            var entityProps = entityType.GetProperties();
            var entities=new List<TEntity>();

            while (reader.Read())
            {
               var entity=new TEntity();
               foreach (var entityPropInfo in entityProps)
               {
                   var value = reader[entityPropInfo.Name];
                   if (value is DBNull)
                   {
                       value = null;
                   }

                   entityPropInfo.SetValue(entity,value);
               }
               entities.Add(entity);
            }

            return entities;
        }
    }
}
