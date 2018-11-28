using DynamicDocsWPF.Model.Base_Classes;

namespace DynamicDocsWPF.Model.ProcessElement
{
    public class ValidationTag : Tag
    {
        public ValidationTag(Tag parent, bool locks ) :base(parent)
        {
            Locks = locks;
        }

        public bool Locks { get;}
    }
}