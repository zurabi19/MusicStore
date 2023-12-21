using Microsoft.Build.Framework;

namespace MusicStore__1_.Models
{
    public class EditRoleModelView
    {
        public EditRoleModelView()
        {
            Users = new List<string>();
        }
        public string RoleId { get; set; }
        
        [Required]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}
