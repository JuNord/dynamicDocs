using System;

namespace RestService.Model.Database
{
    public class ProcessInstance
    {
        public int Id { get; set; }
        public string TemplateId { get; set; }
        public string OwnerId { get; set; }
        public int CurrentStep { get; set; }
        public bool Declined { get; set; }
        public bool Archived { get; set; }
        public bool Locked { get; set; }
        public string Created { get; set; }
        public string Changed { get; set; }
        public string Subject { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ProcessInstance inst)
            {
                return Equals(inst);
            }

            return false;
        }

        private bool Equals(ProcessInstance other)
        {
            return Id == other.Id && string.Equals(TemplateId, other.TemplateId) && string.Equals(OwnerId, other.OwnerId) && CurrentStep == other.CurrentStep && Declined == other.Declined && Archived == other.Archived && Locked == other.Locked && string.Equals(Created, other.Created) && string.Equals(Changed, other.Changed) && string.Equals(Subject, other.Subject);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (TemplateId != null ? TemplateId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (OwnerId != null ? OwnerId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CurrentStep;
                hashCode = (hashCode * 397) ^ Declined.GetHashCode();
                hashCode = (hashCode * 397) ^ Archived.GetHashCode();
                hashCode = (hashCode * 397) ^ Locked.GetHashCode();
                hashCode = (hashCode * 397) ^ (Created != null ? Created.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Changed != null ? Changed.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Subject != null ? Subject.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}