namespace RestService.Model.Database
{
    public class ArchivedProcess
    {
        public int Id { get; set; }
        public string ProcessTemplate_ID { get; set; }
        public string Owner_ID { get; set; }
        public int CurrentStep { get; set; }
        public bool Declined { get; set; }
    }
}