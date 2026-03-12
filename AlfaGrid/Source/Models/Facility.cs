namespace AlfaGrid.Source.Models
{
    public class Facility
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool IsIconFont { get; set; } // True if Icon contains Material Icons unicode, false if it's an image path

        public Facility(string name, string icon, bool isIconFont = true)
        {
            Name = name;
            Icon = icon;
            IsIconFont = isIconFont;
        }
    }
}
