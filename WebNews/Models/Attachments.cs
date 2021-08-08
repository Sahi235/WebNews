namespace WebNews.Models
{
    public class Attachments
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EmailId { get; set; }
        public string AttachmentUrl { get; set; }
        public virtual Email Email { get; set; }
    }
}