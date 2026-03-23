using EF.Toolkits.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Toolkits.CustomSql.Abstractions;

namespace EF.Toolkits.Tests.Sql
{
    public class ViewSqlGenerator : CustomSqlGenerator
    {
        public ViewSqlGenerator(DbContext dbContext, ModelBuilder modelBuilder) : base(dbContext, modelBuilder)
        {
        }

        public string Create()
        {
            var animal = GetTableName<Animal>();
            var id = GetColumnName<Animal>(e => e.AnimalType);
            var type = GetColumnName<Animal>(e => e.AnimalType);

            return
                $"CREATE OR REPLACE VIEW public.animal_cats\n" +
                $"AS SELECT {id}, {type} FROM {animal}";
        }
        
        public string Drop()
        {
            return "DROP VIEW IF EXISTS public.animal_cats";
        }
    }
}