using EF.Toolkits.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Toolkits.CustomSql.Abstractions;

namespace EF.Toolkits.Tests.Sql
{
    public class TriggersGenerator : CustomSqlGenerator
    {
        public TriggersGenerator(DbContext dbContext, ModelBuilder modelBuilder) : base(dbContext, modelBuilder)
        {
        }

        public string GenerateTriggersScript()
        {
            var animal = GetTableName<Animal>();

            var species = GetColumnName<Animal>(x => x.Species);
            var animalType = GetColumnName<Animal>(x => x.AnimalType);

            return
                $"IF NEW.{species} IS NOT NULL AND NEW.{species} IS DISTINCT FROM OLD.{species} THEN\n" +
                $"    RAISE EXCEPTION 'Нельзя менять породу';\n" +
                $"END IF;\n" +
                $"IF NEW.{species} IS NOT NULL THEN\n" +
                $"    UPDATE {animal} SET {animalType} = NEW.{animalType}\n" +
                $"    WHERE {species} = NEW.{species};\n" +
                $"END IF;";
        }
    }
}