namespace UWB.Models
{
    public class AnchorData
    {
        public string A { get; set; } = null!;
        public float R { get; set; }
    }

    public class TagData
    {
        public string Id { get;set; } = null!;
        public AnchorData[] Links { get; set; }
    }
}
