namespace WebServerWPF.Model
{
    public class ArchivedProcess
    {
        public int CurrentProcess_ID { get; set; }
        public string ProcessTemplate_ID { get; set; }
        public int Owner_ID { get; set; }
        public int CurrentStep { get; set; }
        public bool Declined { get; set; }
    }
}