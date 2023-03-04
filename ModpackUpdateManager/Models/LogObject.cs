namespace ModpackUpdateManager.Models
{
    public class LogObject
    {
        public LogObject(string name, string reason)
        {
            Name = name;
            Reason = reason;
        }

        public string Name { get; set; }
        public string Reason { get; set; }

    }
}
