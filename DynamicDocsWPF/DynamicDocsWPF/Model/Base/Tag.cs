namespace DynamicDocsWPF.Model.Base
{
    public abstract class Tag
    {
        public Tag Parent { get; }

        protected Tag(Tag parent)
        {
            Parent = parent;
        }
    }
}