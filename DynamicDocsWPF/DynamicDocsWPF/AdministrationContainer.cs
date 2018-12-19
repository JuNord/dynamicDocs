namespace DynamicDocsWPF
{
    public class AdministrationContainer
    {
        public string Email { get; set; }
        public int PermissionLevel { get; set; }
        public string Role { get; set; }

        public override bool Equals(object obj)
        {
            return IsEqual(obj as AdministrationContainer);
        }

        protected bool IsEqual(AdministrationContainer other)
        {
            if(other != null)
                return string.Equals(Email, other.Email) && PermissionLevel == other.PermissionLevel && string.Equals(Role, other.Role);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ PermissionLevel;
                hashCode = (hashCode * 397) ^ (Role != null ? Role.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}