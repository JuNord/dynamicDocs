namespace WebServerWPF.RestDots
{
    public abstract class RequestPostDocTemplate
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public bool ForceOverWrite { get; set; }
    }
}