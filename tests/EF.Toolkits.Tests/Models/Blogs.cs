namespace EF.Toolkits.Tests.Models
{
    /// <summary>
    /// Комментарий для базого абстрактоного типа в наследовании TPC.
    /// </summary>
    public abstract class BlogBase
    {
        public int Id { get; set; }
    }
    
    /// <summary>
    /// Наследник в TPC.
    /// </summary>
    public class BlogA : BlogBase
    {
        public string Name { get; set; }
    }
}