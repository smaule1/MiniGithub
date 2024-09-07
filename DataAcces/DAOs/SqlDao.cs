using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAOs
{
    public class SqlDao
    {
        private string _connectionString;

        private static SqlDao _instance;

        private SqlDao()
        {
            _connectionString = " Data Source=FABIO\\SQLEXPRESS;Initial Catalog=ReseñasContenidoDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
        }

        public static SqlDao GetInstance()
        {
            
                if (_instance == null)
                {
                    _instance = new SqlDao();
                }
                return _instance;
            
        }

        public void ExecuteProcedure(SqlOperation sqlOperation)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sqlOperation.ProcedureName, conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    foreach (var param in sqlOperation.Parameters)
                    {
                        command.Parameters.Add(param);
                    }

                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public int ExecuteProcedureReturnId(SqlOperation sqlOperation)
        {
            int returnValue = 0;
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sqlOperation.ProcedureName, conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    foreach (var param in sqlOperation.Parameters)
                    {
                        if (param.Direction == ParameterDirection.Output)
                        {
                            var sqlParam = command.Parameters.Add(param.ParameterName, param.SqlDbType);
                            sqlParam.Value = param.Value;
                            sqlParam.Direction = param.Direction;
                            sqlParam.Size = -1;
                        }
                        else
                        {
                            command.Parameters.Add(param);
                        }

                    }

                    conn.Open();
                    command.ExecuteNonQuery();

                    // Recupera el valor de retorno si existe
                    foreach (SqlParameter param in command.Parameters)
                    {
                        if (param.Direction == ParameterDirection.ReturnValue)
                        {
                            returnValue = Convert.ToInt32(param.Value);
                            break;
                        }
                    }
                }
            }

            return returnValue;
        }


        public List<Dictionary<string,object>> ExecuteQueryProcedure(SqlOperation SqlOp)
        {
            var lstResult = new List<Dictionary<string, object>>();

            using (var conn = new SqlConnection(_connectionString)) { 
                using(var command = new SqlCommand(SqlOp.ProcedureName, conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    foreach (var param in SqlOp.Parameters)
                    {
                        command.Parameters.Add(param);
                    }
                    conn.Open();

                    var reader = command.ExecuteReader();

                    if(reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int index = 0; index < reader.FieldCount; index++)
                            {
                               var key = reader.GetName(index);
                               var value = reader.GetValue(index);
                               row[key] = value;
                            }
                            lstResult.Add(row);
                        }
                    }
                }

            }
            return lstResult;
        }


        public List<Dictionary<string, object>> ExecuteQuery(SqlCmd cmd)
        {
            var lstResult = new List<Dictionary<string, object>>();

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(cmd.CommandText, conn)
                {
                    CommandType = CommandType.Text
                })
                {
                    foreach (var param in cmd.Parameters)
                    {
                        command.Parameters.Add(param);
                    }
                    conn.Open();

                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int index = 0; index < reader.FieldCount; index++)
                            {
                                var key = reader.GetName(index);
                                var value = reader.GetValue(index);
                                row[key] = value;
                            }
                            lstResult.Add(row);
                        }
                    }
                }

            }
            return lstResult;
        }



    }

    
}
