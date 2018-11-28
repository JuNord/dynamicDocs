namespace WebServerWPF.Model
{
    public class ProcessUpdate
    {
        public int ID { get; set; }
        public bool Declined { get; set; }

        public ProcessUpdate(){}

        public ProcessUpdate(int id, bool declined)
        {
            ID = id;
            Declined = declined;
        }
    }
}