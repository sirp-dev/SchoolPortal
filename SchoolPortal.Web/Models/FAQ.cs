namespace SchoolPortal.Web.Models
{
    public class FAQ
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int SortOrder { get; set; }
        public bool Show { get; set; }
    }
}
