namespace DynamicDocsWPF.Model.Base_Classes
{
    public abstract class NamedTag : Tag
    {
        public string Name { get; set; }
        public string Description { get; set; }
        

        protected NamedTag(Tag parent) : base(parent)
        {
        }
    }
}