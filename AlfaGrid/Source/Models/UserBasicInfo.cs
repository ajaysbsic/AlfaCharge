namespace AlfaGrid.Source.Models
{
    public class UserBasicInfo
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int RoleID { get; set; }
        public string RoleText { get; set; }
    }

    public enum RoleDetails
    {
        User = 1,
        Manager,
        Admin,
    }
}