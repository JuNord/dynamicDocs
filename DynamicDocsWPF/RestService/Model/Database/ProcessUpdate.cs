namespace RestService.Model.Database
{
    public class ProcessUpdate
    {
        public ProcessUpdate()
        {
        }

        public ProcessUpdate(int id, bool declined)
        {
            ID = id;
            Declined = declined;
        }

        public int ID { get; set; }
        public bool Declined { get; set; }
    }
}