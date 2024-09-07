using Microsoft.Data.SqlClient;
using System.Data;

namespace DataAccess.DAOs
{
    public class SqlBase
    {

        public List<SqlParameter> Parameters { get; set; }


        public SqlBase()
        {
            Parameters = new List<SqlParameter>();
        }

        /*
         * Metodos utilitarios para agregar parametros
         *         */

        public void AddVarCharParameter(string parameterName, string value)
        {
            Parameters.Add(new SqlParameter(parameterName, value));
        }

        public void AddCharParameter(string parameterName, char value)
        {
            Parameters.Add(new SqlParameter(parameterName, value));
        }

        public void AddIntParameter(string parameterName, int value)
        {
            Parameters.Add(new SqlParameter(parameterName, value));
        }

        public void AddIntParameterDirection(string parameterName, ParameterDirection direction)
        {
            Parameters.Add(new SqlParameter { ParameterName = parameterName, Direction = direction });
        }

        public void AddIntParameter(string parameterName, int? value)
        {
            if (value == null)
                Parameters.Add(new SqlParameter(parameterName, DBNull.Value));
            else
                Parameters.Add(new SqlParameter(parameterName, value));
        }

        public void AddDateTimeParameter(string parameterName, DateTime value)
        {
            Parameters.Add(new SqlParameter(parameterName, value));
        }

        public void AddBitParameter(string parameterName, bool value)
        {
            Parameters.Add(new SqlParameter(parameterName, value));
        }

        public void AddDoubleParameter(string paramName, double paramValue)
        {
            Parameters.Add(new SqlParameter(paramName, paramValue));
        }
        public void AddTimeParameter(string parameterName, TimeSpan value)
        {
            Parameters.Add(new SqlParameter(parameterName, value));
        }

        public void AddDecimalParameter(string parameterName, decimal? value)
        {
            if (value == null)
                Parameters.Add(new SqlParameter(parameterName, DBNull.Value));
            else
                Parameters.Add(new SqlParameter(parameterName, value));
        }

        public void AddDecimalParameter(string v1, object v2)
        {
            throw new NotImplementedException();
        }

        public void AddBinaryParameter(string parameterName, byte[] value)
        {
            Parameters.Add(new SqlParameter(parameterName, value));
        }
    }


}
