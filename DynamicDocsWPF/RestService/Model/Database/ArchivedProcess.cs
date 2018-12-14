namespace RestService.Model.Database
{
    public class ArchivedProcess
    {
        public int Id { get; set; }
        public string ProcessTemplateId { get; set; }
        public string OwnerId { get; set; }
        public int CurrentStep { get; set; }
        public bool Declined { get; set; }
    }
}