using CardGame.Server.Services;
using System.ComponentModel.DataAnnotations;

namespace CardGame.Server.Models
{
    public class RegisterPlayerNameModel
    {
        [Required]
        public string PlayerName { get; set; }
    }
}
