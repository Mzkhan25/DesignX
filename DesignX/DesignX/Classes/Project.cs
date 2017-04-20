namespace DesignX.Classes
{
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Url { get; set; }
        public int UpVote { get; set; }
        public int DownVote { get; set; }
        public bool Deleted { get; set; }
    }
}