namespace Extractor.Models
{
    class EventDependency
    {
        public string Type { get; set; }

        public int? Governor { get; set; }

        public int? Dependent { get; set; }
    }
}
