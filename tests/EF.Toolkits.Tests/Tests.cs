using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EF.Toolkits.Tests
{
    public class Tests
    {
        [Fact]
        public void Migrations_Should_Include_CustomSql_Triggers_And_Comments()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            DbContextConfigurator.Configure(optionsBuilder);

            using var context = new AppDbContext(optionsBuilder.Options);

            context.Database.EnsureDeleted();

            // Создаем миграции в памяти
            var migrator = context.GetService<IMigrator>();

            var sqlScript = migrator.GenerateScript(
                fromMigration: null,
                toMigration: null,
                options: MigrationsSqlGenerationOptions.Default);

            // Act & Assert
        }

        [Fact]
        public void CustomMigrationsModelDiffer_Should_Detect_Annotation_Changes()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            DbContextConfigurator.Configure(optionsBuilder);

            using var context = new AppDbContext(optionsBuilder.Options);

            var services = context.GetInfrastructure();

            var differ = services.GetService<IMigrationsModelDiffer>();

            var sourceModel = CreateEmptyRelationalModel(context);

#if NET6_0_OR_GREATER
            var targetModel = context.GetService<IDesignTimeModel>().Model.GetRelationalModel();
#else
            var targetModel = context.Model.GetRelationalModel();
#endif
            var operations = differ.GetDifferences(sourceModel, targetModel);

            // Act

            // Assert
        }

        private IRelationalModel CreateEmptyRelationalModel(AppDbContext context)
        {
            var modelBuilder = new ModelBuilder();
            var services = context.GetInfrastructure();

#if NET6_0_OR_GREATER
            var finalizeModel = modelBuilder.FinalizeModel();

            var modelRuntimeInitializer = services.GetService<IModelRuntimeInitializer>();
            finalizeModel = modelRuntimeInitializer.Initialize(finalizeModel);

            return finalizeModel.GetRelationalModel();
#else

            var migrationsAssembly = context.GetService<IMigrationsAssembly>();

            var dependencies = context.GetService<ProviderConventionSetBuilderDependencies>();
            var relationalDependencies = context.GetService<RelationalConventionSetBuilderDependencies>();

            if (migrationsAssembly.ModelSnapshot != null)
            {
                var typeMappingConvention = new TypeMappingConvention(dependencies);
                typeMappingConvention.ProcessModelFinalizing(
                    ((IConventionModel)migrationsAssembly.ModelSnapshot.Model).Builder, null);
            }

            var relationalModelConvention = new RelationalModelConvention(dependencies, relationalDependencies);
            var sourceModel = relationalModelConvention.ProcessModelFinalized(modelBuilder.Model);

            return sourceModel.GetRelationalModel();
#endif
        }
    }
}