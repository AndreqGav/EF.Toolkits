namespace EF.Toolkits.Tests.Models
{
    /// <summary>
    /// Счёт на оплату.
    /// </summary>
    public class Invoice
    {
        /// <summary>
        /// Идентификатор счёта.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Сумма счёта в рублях.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Статус оплаты.
        /// </summary>
        public bool IsPaid { get; set; }
    }
}
