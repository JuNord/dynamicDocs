namespace RestService
{
    public class ProcessInstance
    {
        public int CurrentProcess_ID { get; set; }
        public string ProcessTemplate_ID { get; set; }
        public int Owner_id { get; set; }
        public int CurrentStep { get; set; }
        public bool Declined { get; set; }
    }
}