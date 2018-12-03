namespace RestService.Model.Database
{
    public class ProcessUpdate
    {
        public ProcessUpdate()
        {
        }

        public ProcessUpdate(int id, bool declined, bool locks)
        {
            ID = id;
            Declined = declined;
            Locks = locks;
        }

        public int ID { get; set; }
        public bool Declined { get; set; }
        public bool Locks { get; set; }
    }
}