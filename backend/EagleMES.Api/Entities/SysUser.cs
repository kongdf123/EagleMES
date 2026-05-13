using System.ComponentModel.DataAnnotations.Schema;

namespace EagleMES.Api.Entities
{
    [Table("Users")]
    public class SysUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
