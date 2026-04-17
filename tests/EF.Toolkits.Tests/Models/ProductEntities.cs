using Toolkits.AutoComments.Attributes;

namespace EF.Toolkits.Tests.Models
{
    /// <summary>
    /// Товар в каталоге магазина.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Идентификатор товара.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование товара.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///        Цена товара в рублях.       
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Статус товара в каталоге.
        /// </summary>
        [AutoCommentEnumDescription]
        public ProductStatus Status { get; set; }

        /// <summary>
        /// Категория товара.
        /// </summary>
        public ProductCategory Category { get; set; }
    }

    /// <summary>
    /// Статус товара.
    /// </summary>
    public enum ProductStatus
    {
        /// <summary>
        /// Активный, доступен для заказа.
        /// </summary>
        Active = 0,

        /// <summary>
        /// Неактивный, скрыт из каталога.
        /// </summary>
        Inactive = 1,

        /// <summary>
        /// В архиве, снят с продажи.
        /// </summary>
        Archived = 2,
    }

    /// <summary>
    /// Категория товара.
    /// </summary>
    public enum ProductCategory
    {
        /// <summary>
        /// Одежда.
        /// </summary>
        Clothing,

        /// <summary>
        /// Книги.
        /// </summary>
        Books,

        /// <summary>
        /// Игрушки.
        /// </summary>
        Toys,
    }

    /// <summary>
    /// Представление каталога товаров (без ключа).
    /// </summary>
    public class ProductCatalogView
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }

    /// <summary>
    /// Склад хранения товаров.
    /// </summary>
    public class Warehouse
    {
        /// <summary>
        /// Идентификатор склада.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Адрес склада.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Вместимость склада в единицах.
        /// </summary>
        public int Capacity { get; set; }
    }
}