namespace AlfaGrid.Source.Models
{
    public class CountryInfo
    {
        public string Name { get; set; } = "Saudi Arabia";
        public string Iso2 { get; set; } = "SA";
        public string DialCode { get; set; } = "+966";
        public string FlagImage { get; set; } = "";
        public override string ToString() => $"{Name} ({DialCode})";
    }
}