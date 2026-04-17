using EF.Toolkits.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Toolkits.EntityFrameworkCore;
using Xunit;

namespace EF.Toolkits.Tests.Model
{
    /// <summary>
    /// Тесты проверяют, что конвенция автокомментариев корректно устанавливает
    /// комментарии на таблицы и колонки на основе XML-документации.
    /// Реальное подключение к БД не требуется — только построение модели.
    /// </summary>
    public class AutoCommentsModelTests
    {
        private static IModel GetModel(DbContext ctx)
        {
#if NET6_0_OR_GREATER
            return ctx.GetService<IDesignTimeModel>().Model;
#else
            return ctx.Model;
#endif
        }

        private static DbContextOptions<AutoCommentsTestDbContext> BuildOptions(bool globalEnumDescriptions = false)
        {
            var builder = new DbContextOptionsBuilder<AutoCommentsTestDbContext>();
            builder
                .UseNpgsql("Host=localhost;Database=ef_toolkits_autocomments_test")
                .UseAutoComments(options =>
                {
                    var o = options.FromXmlFiles("Comments.xml");

                    if (globalEnumDescriptions)
                    {
                        o.AddEnumDescriptions();
                    }
                });

            return builder.Options;
        }

        [Fact]
        public void AutoComments_Should_SetTableComment()
        {
            // Arrange
            var options = BuildOptions();

            // Act
            using var ctx = new AutoCommentsTestDbContext(options);
            var comment = GetModel(ctx).FindEntityType(typeof(Product))!.GetComment();

            // Assert
            Assert.NotNull(comment);
            Assert.Equal("Товар в каталоге магазина.", comment);
        }

        [Fact]
        public void AutoComments_Should_SetColumnComment()
        {
            // Arrange
            var options = BuildOptions();

            // Act
            using var ctx = new AutoCommentsTestDbContext(options);
            var commentId = GetModel(ctx)
                .FindEntityType(typeof(Product))!
                .FindProperty(nameof(Product.Id))!
                .GetComment();

            var commentTitle = GetModel(ctx)
                .FindEntityType(typeof(Product))!
                .FindProperty(nameof(Product.Title))!
                .GetComment();

            // Assert
            Assert.NotNull(commentId);
            Assert.Equal("Идентификатор товара.", commentId);
            Assert.NotNull(commentTitle);
            Assert.Equal("Наименование товара.", commentTitle);
        }
        
        [Fact]
        public void AutoComments_Should_SetTrimmedColumnComment()
        {
            // Arrange
            var options = BuildOptions();

            // Act
            using var ctx = new AutoCommentsTestDbContext(options);

            var comment = GetModel(ctx)
                .FindEntityType(typeof(Product))!
                .FindProperty(nameof(Product.Price))!
                .GetComment();

            // Assert
            Assert.NotNull(comment);
            Assert.Equal("Цена товара в рублях.", comment);
        }

        [Fact]
        public void AutoComments_Should_AppendEnumValues_ToIntColumn_WhenHasEnumDescriptionsAttribute()
        {
            // Arrange
            var options = BuildOptions();

            // Act
            using var ctx = new AutoCommentsTestDbContext(options);
            var comment = GetModel(ctx)
                .FindEntityType(typeof(Product))!
                .FindProperty(nameof(Product.Status))!
                .GetComment();

            // Assert
            Assert.NotNull(comment);
            Assert.StartsWith("Статус товара в каталоге.\n", comment);
            Assert.Contains("0 - Активный, доступен для заказа.", comment);
            Assert.Contains("1 - Неактивный, скрыт из каталога.", comment);
            Assert.Contains("2 - В архиве, снят с продажи.", comment);
        }

        [Fact]
        public void AutoComments_ShouldNot_AppendEnumValues_WhenHasNoEnumDescriptionsAttribute()
        {
            // Arrange
            var options = BuildOptions();

            // Act
            using var ctx = new AutoCommentsTestDbContext(options);
            var comment = GetModel(ctx)
                .FindEntityType(typeof(Product))!
                .FindProperty(nameof(Product.Category))!
                .GetComment();

            // Assert
            Assert.NotNull(comment);
            Assert.Equal("Категория товара.", comment);
        }

        [Fact]
        public void AutoComments_Should_AppendEnumValues_ToStringColumn_WhenGlobalEnableEnumDescriptions()
        {
            // Arrange
            var options = BuildOptions(globalEnumDescriptions: true);

            // Act
            using var ctx = new AutoCommentsTestDbContext(options);
            var comment = GetModel(ctx)
                .FindEntityType(typeof(Product))!
                .FindProperty(nameof(Product.Category))!
                .GetComment();

            // Assert
            Assert.NotNull(comment);
            Assert.StartsWith("Категория товара.\n", comment);
            Assert.Contains("Clothing - Одежда.", comment);
            Assert.Contains("Books - Книги.", comment);
            Assert.Contains("Toys - Игрушки.", comment);
        }

        [Fact]
        public void AutoComments_Should_NotOverwrite_ManualTableComment()
        {
            // Arrange — у Warehouse задан ручной комментарий в OnModelCreating
            var options = BuildOptions();

            // Act
            using var ctx = new AutoCommentsTestDbContext(options);
            var comment = GetModel(ctx).FindEntityType(typeof(Warehouse))!.GetComment();

            // Assert
            Assert.Equal("Склад хранения (ручной комментарий)", comment);
        }

        [Fact]
        public void AutoComments_Should_NotOverwrite_ManualPropertyComment()
        {
            // Arrange
            var options = BuildOptions();

            // Act
            using var ctx = new AutoCommentsTestDbContext(options);
            var comment = GetModel(ctx)
                .FindEntityType(typeof(Warehouse))!
                .FindProperty(nameof(Warehouse.Address))!
                .GetComment();

            // Assert
            Assert.Equal("Адрес (ручной комментарий)", comment);
        }

        [Fact]
        public void AutoComments_Should_SkipView_AndNotSetComment()
        {
            // Arrange
            var options = BuildOptions();

            // Act
            using var ctx = new AutoCommentsTestDbContext(options);
            var comment = GetModel(ctx).FindEntityType(typeof(ProductCatalogView))!.GetComment();

            // Assert
            Assert.Null(comment);
        }
    }

    internal sealed class AutoCommentsTestDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<ProductCatalogView> ProductCatalogViews { get; set; }

        public AutoCommentsTestDbContext(DbContextOptions<AutoCommentsTestDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Представление — конвенция автокомментариев должна его пропускать
            modelBuilder.Entity<ProductCatalogView>(builder =>
            {
                builder.HasNoKey();
                builder.ToView("Product");
            });

            modelBuilder.Entity<Product>(builder => { builder.Property(e => e.Category).HasConversion<string>(); });

            // Ручной комментарий — конвенция не должна его перезаписывать
            modelBuilder.Entity<Warehouse>(builder =>
            {
#if NET7_0_OR_GREATER
                builder.ToTable("Warehouses", t => t.HasComment("Склад хранения (ручной комментарий)"));
#else
                builder.HasComment("Склад хранения (ручной комментарий)");
                builder.ToTable("Warehouses");
#endif
                builder.Property(w => w.Address).HasComment("Адрес (ручной комментарий)");
            });
        }
    }
}