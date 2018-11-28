namespace RestService.Model.Database
{
    public class RunningProcess
    {
        public int Id { get; set; }
        public string Template_ID { get; set; }
        public string Owner_ID { get; set; }
        public int CurrentStep { get; set; }
        public bool Declined { get; set; }
        public bool Archived { get; set; }
    }
}