using CardGame.Server.Services;
using System.ComponentModel.DataAnnotations;

namespace CardGame.Server.Models
{
    public class ResponseGetGameState
    {
        [Required]
        public IEnumerable<CardItem?> AllCards { get; set; }
        public string SignedInUser { get; set; }
        public bool IsActiveGame { get; set; }
    }
}
