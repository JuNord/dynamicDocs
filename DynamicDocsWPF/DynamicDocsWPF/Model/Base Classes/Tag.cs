namespace DynamicDocsWPF.Model.Base_Classes
{
    public abstract class Tag
    {
        public Tag Parent { get; }

        public Tag(Tag parent)
        {
            Parent = parent;
        }
    }
}