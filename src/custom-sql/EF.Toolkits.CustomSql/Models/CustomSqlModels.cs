using EF.Toolkits.CustomSql.Constants;

namespace EF.Toolkits.CustomSql.Models
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
        public SqlUpModel(string name, string sql) : base($"{CustomSqlConstants.SqlUp}_{name}", sql)
        {
        }
    }

    public class SqlDownModel : SqlAnnotationModel
    {
        public SqlDownModel(string name, string sql) : base($"{CustomSqlConstants.SqlDown}_{name}", sql)
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