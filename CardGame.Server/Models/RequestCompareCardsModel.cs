using CardGame.Server.Services;
using System.ComponentModel.DataAnnotations;

namespace CardGame.Server.Models
{
    public class RequestCompareCardsModel
    {
        [Required]
        public CardItem CardA { get; set; }

        [Required]
        public CardItem CardB { get; set; }

    }
}
