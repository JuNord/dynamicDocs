namespace RestService.Model.Base
{
    public abstract class NamedTag : Tag
    {
        protected NamedTag(Tag parent, string name, string description) : base(parent)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}