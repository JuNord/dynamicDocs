namespace DynamicDocsWPF.Model.Base_Classes
{
    public abstract class NamedTag : Tag
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        protected NamedTag(Tag parent) : base(parent)
        {
        }
    }
}