namespace DynamicDocsWPF.Model.Base_Classes
{
    public class Tag
    {
        public Tag Parent { get; set; }

        public Tag(Tag parent)
        {
            Parent = parent;
        }
    }
}