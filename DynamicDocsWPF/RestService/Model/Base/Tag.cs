namespace RestService.Model.Base
{
    public abstract class Tag
    {
        protected Tag(Tag parent)
        {
            Parent = parent;
        }

        public Tag Parent { get; }
    }
}