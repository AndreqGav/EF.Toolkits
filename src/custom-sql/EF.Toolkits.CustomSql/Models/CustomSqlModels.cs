using Toolkits.CustomSql.Constants;

namespace Toolkits.CustomSql.Models
{
    public class SqlAnnotationModel
    {
        public string Name { get; }

        public string Sql { get; }

        public SqlAnnotationModel(string name, string sql)
        {
            Name = name;
            Sql = sql;
        }
    }

    public class SqlUpModel : SqlAnnotationModel
    {
        public SqlUpModel(string name, string sql) : base($"{CustomSqlConstants.SqlUp}{name}", sql)
        {
        }
    }

    public class SqlDownModel : SqlAnnotationModel
    {
        public SqlDownModel(string name, string sql) : base($"{CustomSqlConstants.SqlDown}{name}", sql)
        {
        }
    }

    public class CustomSqlModels
    {
        public string Name { get; }

        public string SqlUp { get; }

        public string SqlDown { get; }

        public CustomSqlModels(string name, string sqlUp, string sqlDown)
        {
            Name = name;
            SqlUp = sqlUp;
            SqlDown = sqlDown;
        }
    }
}