using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toolkits.CustomSql.Models;

namespace Toolkits.CustomSql
{
    public static class AddCustomSqlExtensions
    {
        public static void AddCustomSql(this ModelBuilder modelBuilder, string name, string sqlUp, string sqlDown)
        {
            var sqlUpModel = new SqlUpModel(name, sqlUp);
            var sqlDownModel = new SqlDownModel(name, sqlDown);

            modelBuilder.HasAnnotation(sqlUpModel.Name, sqlUpModel.Sql);
            modelBuilder.HasAnnotation(sqlDownModel.Name, sqlDownModel.Sql);
        }

        public static void AddCustomSql<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string sqlUp, string sqlDown)
            where TEntity : class
        {
            var sqlUpModel = new SqlUpModel(name, sqlUp);
            var sqlDownModel = new SqlDownModel(name, sqlDown);

            entityTypeBuilder.HasAnnotation(sqlUpModel.Name, sqlUpModel.Sql);
            entityTypeBuilder.HasAnnotation(sqlDownModel.Name, sqlDownModel.Sql);
        }

        public static void AddCustomSql(this IConventionEntityTypeBuilder entityTypeBuilder, string name, string sqlUp, string sqlDown)
        {
            var sqlUpModel = new SqlUpModel(name, sqlUp);
            var sqlDownModel = new SqlDownModel(name, sqlDown);

            entityTypeBuilder.HasAnnotation(sqlUpModel.Name, sqlUpModel.Sql);
            entityTypeBuilder.HasAnnotation(sqlDownModel.Name, sqlDownModel.Sql);
        }
    }
}