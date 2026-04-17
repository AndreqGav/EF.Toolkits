namespace EF.Toolkits.Tests.Models
{
    /// <summary>
    /// Заказ покупателя.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Идентификатор заказа.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Номер заказа.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Итоговая сумма заказа в рублях.
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
