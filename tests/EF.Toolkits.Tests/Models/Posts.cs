namespace EF.Toolkits.Tests.Models
{
    /// <summary>
    /// Комментарий для базого типа в наследовании TPH.
    /// </summary>
    public class Post
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }
    }

    /// <summary>
    /// Наследник А.
    /// </summary>
    public class PostA : Post
    {
        /// <summary>
        /// Тексь А.
        /// </summary>
        public string TextA { get; set; }
    }

    /// <summary>
    /// Наследник Б.
    /// </summary>
    public class PostB : Post
    {
        /// <summary>
        /// Тексь Б.
        /// </summary>
        public string TextB { get; set; }
    }
}