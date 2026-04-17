using EF.Toolkits.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Toolkits.CustomSql.Helpers;
using Toolkits.EntityFrameworkCore;
using Toolkits.Triggers;
using Toolkits.Triggers.Enums;
using Xunit;

namespace EF.Toolkits.Tests.Model
{
    /// <summary>
    /// Тесты проверяют, что конвенция триггеров правильно преобразует
    /// аннотации TriggerObject в SQL-аннотации CustomSql.
    /// Реальное подключение к БД не требуется — только построение модели.
    /// </summary>
    public class TriggerModelTests
    {
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
                .UseNpgsql("ef_toolkits_triggers_test")
                .UseCustomSql(o => o.UseTriggers());

            return builder.Options;
        }

        // --- BeforeInsert: проверка сгенерированного SQL ---

        [Fact]
        public void BeforeInsert_Should_GenerateSqlUp_WithCreateFunction()
        {
            // Arrange
            using var ctx = new SingleTriggerContext(BuildOptions<SingleTriggerContext>());

            // Act
            var sqlUp = RelationalModelHelper.GetCustomAnnotations(GetRelationalModel(ctx))[0].SqlUp;

            // Assert
            Assert.Contains("CREATE FUNCTION", sqlUp);
        }

        [Fact]
        public void BeforeInsert_Should_GenerateSqlUp_WithCreateTrigger()
        {
            // Arrange
            using var ctx = new SingleTriggerContext(BuildOptions<SingleTriggerContext>());

            // Act
            var sqlUp = RelationalModelHelper.GetCustomAnnotations(GetRelationalModel(ctx))[0].SqlUp;

            // Assert — сам триггер создаётся через CREATE TRIGGER
            Assert.Contains("CREATE TRIGGER", sqlUp);
        }

        [Fact]
        public void BeforeInsert_Should_GenerateSqlUp_WithBeforeInsertKeywords()
        {
            // Arrange
            using var ctx = new SingleTriggerContext(BuildOptions<SingleTriggerContext>());

            // Act
            var sqlUp = RelationalModelHelper.GetCustomAnnotations(GetRelationalModel(ctx))[0].SqlUp;

            // Assert
            Assert.Contains("BEFORE INSERT", sqlUp);
        }

        [Fact]
        public void BeforeInsert_Should_GenerateSqlUp_WithTriggerName()
        {
            // Arrange
            using var ctx = new SingleTriggerContext(BuildOptions<SingleTriggerContext>());

            // Act
            var sqlUp = RelationalModelHelper.GetCustomAnnotations(GetRelationalModel(ctx))[0].SqlUp;

            // Assert
            Assert.Contains(SingleTriggerContext.TriggerName, sqlUp);
        }

        [Fact]
        public void BeforeInsert_Should_GenerateSqlUp_WithTriggerBody()
        {
            // Arrange
            using var ctx = new SingleTriggerContext(BuildOptions<SingleTriggerContext>());

            // Act
            var sqlUp = RelationalModelHelper.GetCustomAnnotations(GetRelationalModel(ctx))[0].SqlUp;

            // Assert
            Assert.Contains(SingleTriggerContext.TriggerBody, sqlUp);
        }

        [Fact]
        public void BeforeInsert_Should_GenerateSqlDown_WithDropFunction()
        {
            // Arrange
            using var ctx = new SingleTriggerContext(BuildOptions<SingleTriggerContext>());

            // Act
            var sqlDown = RelationalModelHelper.GetCustomAnnotations(GetRelationalModel(ctx))[0].SqlDown;

            // Assert
            Assert.Contains("DROP FUNCTION", sqlDown);
        }

        [Fact]
        public void BeforeInsert_Should_GenerateSqlDown_WithTriggerNameAndCascade()
        {
            // Arrange
            using var ctx = new SingleTriggerContext(BuildOptions<SingleTriggerContext>());

            // Act
            var sqlDown = RelationalModelHelper.GetCustomAnnotations(GetRelationalModel(ctx))[0].SqlDown;

            // Assert
            Assert.Contains(SingleTriggerContext.TriggerName, sqlDown);
            Assert.Contains("CASCADE", sqlDown);
        }

        // --- Несколько триггеров ---

        [Fact]
        public void MultipleTriggers_Should_ProduceMultipleAnnotations()
        {
            // Arrange
            using var ctx = new TwoTriggersContext(BuildOptions<TwoTriggersContext>());

            // Act
            var annotations = RelationalModelHelper.GetCustomAnnotations(GetRelationalModel(ctx));

            // Assert
            Assert.Equal(2, annotations.Count);
        }

        [Fact]
        public void AfterUpdate_Should_GenerateSqlUp_WithAfterUpdateKeywords()
        {
            // Arrange
            using var ctx = new TwoTriggersContext(BuildOptions<TwoTriggersContext>());

            // Act
            var annotations = RelationalModelHelper.GetCustomAnnotations(GetRelationalModel(ctx));

            // Assert
            Assert.Contains("AFTER UPDATE", annotations[1].SqlUp);
        }

        // --- CONSTRAINT TRIGGER ---

        [Fact]
        public void ConstraintTrigger_Should_GenerateSqlUp_WithConstraintTriggerKeyword()
        {
            // Arrange
            using var ctx = new ConstraintTriggerContext(BuildOptions<ConstraintTriggerContext>());

            // Act
            var sqlUp = RelationalModelHelper.GetCustomAnnotations(GetRelationalModel(ctx))[0].SqlUp;

            // Assert
            Assert.Contains("CONSTRAINT TRIGGER", sqlUp);
        }

        [Fact]
        public void ConstraintTrigger_Should_GenerateSqlUp_WithNotDeferrable_WhenNotDeferrable()
        {
            // Arrange
            using var ctx = new ConstraintTriggerContext(BuildOptions<ConstraintTriggerContext>());

            // Act
            var sqlUp = RelationalModelHelper.GetCustomAnnotations(GetRelationalModel(ctx))[0].SqlUp;

            // Assert
            Assert.Contains("NOT DEFERRABLE", sqlUp);
        }

        // --- Ключи аннотаций в сырой модели ---

        [Fact]
        public void TriggerConvention_Should_StoreSqlUpAnnotation_OnEntityType()
        {
            // Arrange
            using var ctx = new SingleTriggerContext(BuildOptions<SingleTriggerContext>());

            // Act — конвенция преобразует TriggerObject в SqlUp/SqlDown аннотации
            var annotations = RelationalModelHelper.GetCustomAnnotations(GetRelationalModel(ctx));

            // Assert — аннотация SqlUp создана
            Assert.NotEmpty(annotations);
            Assert.NotNull(annotations[0].SqlUp);
        }

        [Fact]
        public void TriggerConvention_Should_StoreSqlDownAnnotation_OnEntityType()
        {
            // Arrange
            using var ctx = new SingleTriggerContext(BuildOptions<SingleTriggerContext>());

            // Act
            var annotations = RelationalModelHelper.GetCustomAnnotations(GetRelationalModel(ctx));

            // Assert — аннотация SqlDown создана
            Assert.NotEmpty(annotations);
            Assert.NotNull(annotations[0].SqlDown);
        }
    }

    internal sealed class SingleTriggerContext : DbContext
    {
        public const string TriggerName = "invoice_set_defaults";

        public const string TriggerBody = "NEW.is_paid = false;";

        public DbSet<Invoice> Invoices { get; set; }

        public SingleTriggerContext(DbContextOptions<SingleTriggerContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>(entity => { entity.BeforeInsert(TriggerName, TriggerBody); });
        }
    }

    internal sealed class TwoTriggersContext : DbContext
    {
        public DbSet<Invoice> Invoices { get; set; }

        public TwoTriggersContext(DbContextOptions<TwoTriggersContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>(entity =>
            {
                // Первый триггер — BEFORE INSERT (установка значений по умолчанию)
                entity.BeforeInsert("invoice_on_insert", "NEW.is_paid = false;");

                // Второй триггер — AFTER UPDATE (логирование изменений)
                entity.AfterUpdate("invoice_on_update", "PERFORM log_invoice_change(NEW.id);");
            });
        }
    }

    internal sealed class ConstraintTriggerContext : DbContext
    {
        public DbSet<Invoice> Invoices { get; set; }

        public ConstraintTriggerContext(DbContextOptions<ConstraintTriggerContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>(entity =>
            {
                // CONSTRAINT TRIGGER — проверяет целостность данных после вставки
                entity.AfterInsert(
                    "invoice_constraint_check",
                    "PERFORM verify_invoice_integrity(NEW.id);",
                    TriggerTypeEnum.ConstraintNotDeferrable);
            });
        }
    }
}