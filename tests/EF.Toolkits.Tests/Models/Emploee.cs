namespace EF.Toolkits.Tests.Models
{
    /// <summary>
    /// Сотрудник.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Комментаий: Имя.
        /// </summary>
        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string MiddleName { get; set; }

        /// <summary>
        /// Компания.
        /// </summary>
        public EmployeeCompany Company { get; set; }

        public int Age { get; set; }
    }

    /// <summary>
    /// Комменатрий для owned.
    /// </summary>
    public class EmployeeCompany
    {
        /// <summary>
        /// Имя компании.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Должность в компании.
        /// </summary>
        public string Position { get; set; }
    }
}