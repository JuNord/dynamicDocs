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
    }
}