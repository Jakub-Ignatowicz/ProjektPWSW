using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string Username { get; set; } = String.Empty;
        public string PasswordHash { get; set; } = String.Empty;

        public ICollection<Tracker> Trackers { get; } = new HashSet<Tracker>();
    }
}
