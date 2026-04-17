using System;
using System.Linq;
using EF.Toolkits.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Toolkits.CustomSql;
using Toolkits.CustomSql.Constants;
using Toolkits.CustomSql.Helpers;
using Toolkits.EntityFrameworkCore;
using Xunit;

namespace EF.Toolkits.Tests.Model
{
    /// <summary>
    /// Тесты проверяют хранение произвольного SQL в аннотациях модели
    /// и генерацию операций миграции через CustomSqlMigrationOperationModifier.
    /// Реальное подключение к БД не требуется — только построение модели.
    /// </summary>
    public class CustomSqlAnnotationTests
    {
        // SQL для тестового представления сводки заказов
        internal const string SqlUp = "CREATE VIEW orders_summary AS SELECT id, number, total_amount FROM \"Orders\";";

        internal const string SqlDown = "DROP VIEW IF EXISTS orders_summary;";

        internal const string SqlName = "orders_summary";

        private static IModel GetModel(DbContext ctx)
        {
#if NET6_0_OR_GREATER
            return ctx.GetService<IDesignTimeModel>().Model;
#else
            return ctx.Model;
#endif
        }

        private static IRelationalModel GetRelationalModel(DbContext ctx) => GetModel(ctx).GetRelationalModel();

        private static DbContextOptions<T> BuildOptions<T>() where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            builder
                .UseNpgsql("ef_toolkits_customsql_testt")
                .UseCustomSql();

            return builder.Options;
        }

        // --- Хранение аннотаций ---

        [Fact]
        public void AddCustomSql_Should_StoreSqlUpAnnotation_InModel()
        {
            // Arrange
            using var context = new CustomSqlContext(BuildOptions<CustomSqlContext>());

            // Act
            var annotations = GetModel(context).GetAnnotations().ToList();

            // Assert
            Assert.Contains(annotations, a => a.Name == $"{CustomSqlConstants.SqlUp}{SqlName}");
        }

        [Fact]
        public void AddCustomSql_Should_StoreSqlDownAnnotation()
        {
            // Arrange
            using var ctx = new CustomSqlContext(BuildOptions<CustomSqlContext>());

            // Act
            var annotations = GetModel(ctx).GetAnnotations().ToList();

            // Assert
            Assert.Contains(annotations, a => a.Name == $"{CustomSqlConstants.SqlDown}{SqlName}");
        }

        [Fact]
        public void AddCustomSql_Should_StoreSqlUpAnnotation_WithCorrectScript()
        {
            // Arrange
            using var ctx = new CustomSqlContext(BuildOptions<CustomSqlContext>());

            // Act
            var annotation = GetModel(ctx).GetAnnotations()
                .SingleOrDefault(a => a.Name == $"{CustomSqlConstants.SqlUp}{SqlName}");

            // Assert
            Assert.NotNull(annotation);
            Assert.Equal(SqlUp, annotation.Value?.ToString());
        }

        [Fact]
        public void AddCustomSql_Should_StoreSqlDownAnnotation_WithCorrectScript()
        {
            // Arrange
            using var ctx = new CustomSqlContext(BuildOptions<CustomSqlContext>());

            // Act
            var annotation = GetModel(ctx).GetAnnotations()
                .SingleOrDefault(a => a.Name == $"{CustomSqlConstants.SqlDown}{SqlName}");

            // Assert
            Assert.NotNull(annotation);
            Assert.Equal(SqlDown, annotation.Value?.ToString());
        }

        // --- RelationalModelHelper.GetCustomAnnotations ---

        [Fact]
        public void GetCustomAnnotations_Should_ReturnRegisteredAnnotation()
        {
            // Arrange
            using var ctx = new CustomSqlContext(BuildOptions<CustomSqlContext>());

            // Act
            var relModel = GetRelationalModel(ctx);
            var annotations = RelationalModelHelper.GetCustomAnnotations(relModel);

            // Assert
            Assert.Contains(annotations, a => a.Name == SqlName);
        }

        [Fact]
        public void GetCustomAnnotations_Should_ReturnAnnotation_WithCorrectSqlUp()
        {
            // Arrange
            using var ctx = new CustomSqlContext(BuildOptions<CustomSqlContext>());

            // Act
            var relModel = GetRelationalModel(ctx);
            var annotation = RelationalModelHelper.GetCustomAnnotations(relModel)
                .Single(a => a.Name == SqlName);

            // Assert
            Assert.Equal(SqlUp, annotation.SqlUp);
        }

        [Fact]
        public void GetCustomAnnotations_Should_ReturnAnnotation_WithCorrectSqlDown()
        {
            // Arrange
            using var ctx = new CustomSqlContext(BuildOptions<CustomSqlContext>());

            // Act
            var relModel = GetRelationalModel(ctx);
            var annotation = RelationalModelHelper.GetCustomAnnotations(relModel)
                .Single(a => a.Name == SqlName);

            // Assert
            Assert.Equal(SqlDown, annotation.SqlDown);
        }

        [Fact]
        public void GetCustomAnnotations_Should_ReturnEmpty_WhenModelIsNull()
        {
            // Arrange & Act
            var result = RelationalModelHelper.GetCustomAnnotations(null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetCustomAnnotations_Should_ReturnEmpty_WhenNoAnnotationsRegistered()
        {
            // Arrange
            using var ctx = new EmptyCustomSqlContext(BuildOptions<EmptyCustomSqlContext>());

            // Act
            var relModel = GetRelationalModel(ctx);
            var annotations = RelationalModelHelper.GetCustomAnnotations(relModel);

            // Assert
            Assert.Empty(annotations);
        }

        // --- CustomSqlMigrationOperationModifier ---

        [Fact]
        public void ModifyOperations_Should_ProduceCreateOperation_WhenAnnotationIsNew()
        {
            // Arrange
            using var emptyCtx = new EmptyCustomSqlContext(BuildOptions<EmptyCustomSqlContext>());
            using var sqlCtx = new CustomSqlContext(BuildOptions<CustomSqlContext>());

            var source = GetRelationalModel(emptyCtx);
            var target = GetRelationalModel(sqlCtx);

            // Act
            var modifier = new CustomSqlMigrationOperationModifier();
            var ops = modifier.ModifyOperations(Array.Empty<MigrationOperation>(), source, target);

            // Assert
            Assert.Contains(ops, o => o is SqlOperation s && s.Sql == SqlUp);
        }

        [Fact]
        public void ModifyOperations_Should_ProduceDeleteOperation_WhenAnnotationRemoved()
        {
            // Arrange
            using var sqlCtx = new CustomSqlContext(BuildOptions<CustomSqlContext>());
            using var emptyCtx = new EmptyCustomSqlContext(BuildOptions<EmptyCustomSqlContext>());

            var source = GetRelationalModel(sqlCtx);
            var target = GetRelationalModel(emptyCtx);

            // Act
            var modifier = new CustomSqlMigrationOperationModifier();
            var ops = modifier.ModifyOperations(Array.Empty<MigrationOperation>(), source, target);

            // Assert — должна появиться операция удаления представления
            Assert.Contains(ops, o => o is SqlOperation s && s.Sql == SqlDown);
        }

        [Fact]
        public void ModifyOperations_Should_ProduceNoOperations_WhenAnnotationUnchanged()
        {
            // Arrange
            using var ctx1 = new CustomSqlContext(BuildOptions<CustomSqlContext>());
            using var ctx2 = new CustomSqlContext(BuildOptions<CustomSqlContext>());

            var source = GetRelationalModel(ctx1);
            var target = GetRelationalModel(ctx2);

            // Act
            var modifier = new CustomSqlMigrationOperationModifier();
            var ops = modifier.ModifyOperations(Array.Empty<MigrationOperation>(), source, target);

            // Assert
            Assert.Empty(ops);
        }

        [Fact]
        public void ModifyOperations_Should_ProduceDeleteThenCreate_WhenSqlUpChanged()
        {
            // Arrange
            using var original = new CustomSqlContext(BuildOptions<CustomSqlContext>());
            using var changed = new WithChangedSqlContext(WithChangedSqlContext.BuildOptions());

            var source = GetRelationalModel(original);
            var target = GetRelationalModel(changed);

            // Act
            var modifier = new CustomSqlMigrationOperationModifier();
            var ops = modifier.ModifyOperations(Array.Empty<MigrationOperation>(), source, target)
                .OfType<SqlOperation>().ToList();

            // Assert
            Assert.Contains(ops, o => o.Sql == SqlDown);
            Assert.Contains(ops, o => o.Sql == WithChangedSqlContext.NewSqlUp);

            var deleteIdx = ops.FindIndex(o => o.Sql == SqlDown);
            var createIdx = ops.FindIndex(o => o.Sql == WithChangedSqlContext.NewSqlUp);
            Assert.True(deleteIdx < createIdx, "Удаление должно идти до создания");
        }

        [Fact]
        public void ModifyOperations_Should_PlacePassthroughOps_BeforeCreateOperations()
        {
            // Arrange — passthrough-операция (например, ALTER TABLE идёт между удалением и созданием)
            using var emptyCtx = new EmptyCustomSqlContext(BuildOptions<EmptyCustomSqlContext>());
            using var sqlCtx = new CustomSqlContext(BuildOptions<CustomSqlContext>());

            var source = GetRelationalModel(emptyCtx);
            var target = GetRelationalModel(sqlCtx);
            var passthroughOp = new SqlOperation
            {
                Sql = "SELECT 1;"
            };

            // Act
            var modifier = new CustomSqlMigrationOperationModifier();
            var ops = modifier.ModifyOperations(new[]
            {
                passthroughOp
            }, source, target).ToList();

            // Assert — passthrough-операции идут до CREATE-операций
            var passthroughIdx = ops.IndexOf(passthroughOp);
            var createIdx = ops.FindIndex(o => o is SqlOperation s && s.Sql == SqlUp);
            Assert.True(passthroughIdx >= 0);
            Assert.True(passthroughIdx < createIdx, "Passthrough-операции должны идти до CREATE-операций");
        }
    }

    internal sealed class EmptyCustomSqlContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public EmptyCustomSqlContext(DbContextOptions<EmptyCustomSqlContext> options) : base(options)
        {
        }
    }

    internal sealed class CustomSqlContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public CustomSqlContext(DbContextOptions<CustomSqlContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddCustomSql(
                CustomSqlAnnotationTests.SqlName,
                CustomSqlAnnotationTests.SqlUp,
                CustomSqlAnnotationTests.SqlDown);
        }
    }

    internal sealed class WithChangedSqlContext : DbContext
    {
        // Изменённый SQL — добавили version в SELECT
        public const string NewSqlUp =
            "CREATE VIEW orders_summary AS SELECT id, number, total_amount, 'v2' AS version FROM \"Orders\";";

        public DbSet<Order> Orders { get; set; }

        public static DbContextOptions<WithChangedSqlContext> BuildOptions()
        {
            var b = new DbContextOptionsBuilder<WithChangedSqlContext>();
            b.UseNpgsql("Host=localhost;Database=ef_toolkits_customsql_test").UseCustomSql();

            return b.Options;
        }

        public WithChangedSqlContext(DbContextOptions<WithChangedSqlContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddCustomSql(
                CustomSqlAnnotationTests.SqlName,
                NewSqlUp,
                CustomSqlAnnotationTests.SqlDown);
        }
    }
}