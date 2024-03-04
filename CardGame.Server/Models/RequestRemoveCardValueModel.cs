using CardGame.Server.Services;
using System.ComponentModel.DataAnnotations;

namespace CardGame.Server.Models
{
    public class RequestRemoveCardValueModel
    {
        [Required]
        public int CardValue { get; set; }
    }
}
