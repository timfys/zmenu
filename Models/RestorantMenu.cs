namespace Menu4Tech.Models
{
    public class RestorantMenu
    {
        public string Logo { get; set; } = string.Empty;

        public IList<Category> Categories { get; set; }
    }
}
