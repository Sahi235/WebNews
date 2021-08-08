namespace WebNews.Areas.Panel.ViewModels.Users
{
    public class UserRoleVM
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool InRole { get; set; }
        public bool WasInRole { get; set; }
    }
}