using Toolkits.AutoComments.Attributes;

namespace EF.Toolkits.Tests.Models
{
    /// <summary>
    /// Сущность "Живтоное".
    /// </summary>
    public class Animal
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Кличка.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Порода.
        /// </summary>
        public string Species { get; set; }

        /// <summary>
        /// Тип.
        /// </summary>
        [AutoCommentsEnumValues]
        public AnimalType AnimalType { get; set; }
    }

    /// <summary>
    /// Тип живтоного.
    /// </summary>
    public enum AnimalType
    {
        /// <summary>
        /// Собакен.
        /// </summary>
        Dog,

        /// <summary>
        /// Кошька.
        /// </summary>
        Cat,

        /// <summary>
        /// Рыбка.
        /// </summary>
        Fish,

        /// <summary>
        /// Игрушка?
        /// </summary>
        Toy
    }
}