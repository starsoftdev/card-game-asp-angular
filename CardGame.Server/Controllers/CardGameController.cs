using CardGame.Server.Services;
using Microsoft.AspNetCore.Mvc;
using CardGame.Server.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

namespace CardGame.Server.Controllers
{
    [ApiController]
    [Route("")]
    public class CardGameController : ControllerBase
    {
        private readonly ICardGameService _cardGameService;
        private readonly CardGameGlobalService _cardGameGlobalService;

        private readonly ILogger<CardGameController> _logger;
                
        public CardGameController(ILogger<CardGameController> logger, ICardGameService cardGameService, CardGameGlobalService cardGameGlobalService)
        {
            _logger = logger;
            _cardGameService = cardGameService;
            _cardGameGlobalService = cardGameGlobalService;
        }

        [HttpGet]
        [Route("get-game-state")]
        public IActionResult GetAllCards()
        {
            string SignedInUser = HttpContext.Session.GetString("SignedInUser");
            IEnumerable<CardItem?> AllCards = _cardGameService.GetAllCards(SignedInUser);
            bool IsActiveGame = _cardGameService.CheckActivatedUser(SignedInUser);
            if (AllCards.Count() == 0)
            {
                HttpContext.Session.Remove("SignedInUser");
            }
            ResponseGetGameState res = new ResponseGetGameState
            {
                AllCards = AllCards,
                SignedInUser = SignedInUser != null ? SignedInUser : "",
                IsActiveGame = IsActiveGame
            };
            return Ok(res);
        }

        [HttpPost]
        [Route("compare-cards")]
        public IActionResult PostComareCards([FromBody] RequestCompareCardsModel CardData)
        {
            string SignedInUser = HttpContext.Session.GetString("SignedInUser");
            try
            {
                bool Result = _cardGameService.CompareCards(SignedInUser, CardData.CardA, CardData.CardB);

                if (Result == true)
                {
                    _cardGameGlobalService.ShouldRemoveCards.Add(CardData.CardA.Value);

                    int oldScore = 0;
                    _cardGameGlobalService.Score.TryGetValue(SignedInUser, out oldScore);
                    _cardGameGlobalService.Score[SignedInUser] = oldScore + 1;
                }
                return Ok(Result);
            } catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("register-player-name")]
        public IActionResult PostRegisterPlayerName([FromBody] RegisterPlayerNameModel RequestPlayerName)
        {
            bool Result;
            bool IsAdmin;
            bool IsDuplicated;
            _cardGameService.RegisterPlayerName(RequestPlayerName.PlayerName, out Result, out IsAdmin, out IsDuplicated);

            var respose = new { Result, IsAdmin, IsDuplicated };

            if (Result == true)
            {
                HttpContext.Session.SetString("SignedInUser", RequestPlayerName.PlayerName);
                _cardGameGlobalService.Score.Add(RequestPlayerName.PlayerName, 0);
            }

            return Ok(respose);
        }
        
        [HttpGet]
        [Route("check-game-state")]
        public IActionResult GetCheckGameState()
        {
            string SignedInUser = HttpContext.Session.GetString("SignedInUser");
            return Ok(SignedInUser != null);
        }
        
        [HttpGet]
        [Route("reset-game-state")]
        public bool GetResetGameState()
        {
            string SignedInUser = HttpContext.Session.GetString("SignedInUser");
            _cardGameService.ResetGame(SignedInUser);

            List<string> keys = _cardGameGlobalService.Score.Keys.ToList<string>();
            keys.ForEach(k =>
            {
                _cardGameGlobalService.Score[k] = 0;
            });
            _cardGameGlobalService.ResetGame = true;   
            return true;
        }
        
        [HttpPost]
        [Route("remove-eliminate-card")]
        public bool PostRemoveEliminateCard([FromBody] RequestRemoveCardValueModel requestRemoveCardValueModel)
        {
            string SignedInUser = HttpContext.Session.GetString("SignedInUser");
            _cardGameGlobalService.RemoveEliminateCard(SignedInUser, requestRemoveCardValueModel.CardValue);

            return true;
        }

        [HttpGet]
        [Route("remove-reset-game-status")]
        public bool GetRemoveResetGame()
        {
            string SignedInUser = HttpContext.Session.GetString("SignedInUser");
            if (SignedInUser != null)
            {
                _cardGameGlobalService.RemoveResetGame(SignedInUser);
            }            

            return true;
        }

        [HttpGet]
        [Route("update-cards")]
        public async Task<IActionResult> GetUpdateCardsWithSSE()
        {
            string SignedInUser = HttpContext.Session.GetString("SignedInUser");

            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            while (true)
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
                int unixTimestamp = (int)dateTimeOffset.ToUnixTimeSeconds();

                int maxCount = _cardGameGlobalService.ShouldRemoveCards.Count();
                int i = 0;
                for (; i < maxCount; i ++)
                {
                    try
                    {
                        int CardValue = 0;
                        CardValue = _cardGameGlobalService.ShouldRemoveCards[i];
                        //lock (_cardGameGlobalService.ShouldRemoveCards)
                        //{
                        //    FileName = _cardGameGlobalService.ShouldRemoveCards[i];
                        //    //_cardGameGlobalService.ShouldRemoveCards.RemoveAt(i);
                        //}
                        var sendData = new { 
                            type = "removeCard",
                            cardValue = CardValue
                        };
                        await Response.WriteAsync($"id: {i + 1 + unixTimestamp}\n");
                        await Response.WriteAsync($"event: message\n");
                        await Response.WriteAsync($"data: {JsonConvert.SerializeObject(sendData)}\n\n");
                        await Response.Body.FlushAsync();

                    } catch(Exception ex)
                    {
                    }
                }

                // When Reset Game
                if (_cardGameGlobalService.ResetGame)
                {
                    var sendData = new
                    {
                        type = "resetGame"
                    };
                    await Response.WriteAsync($"id: {unixTimestamp}\n");
                    await Response.WriteAsync($"event: message\n");
                    await Response.WriteAsync($"data: {JsonConvert.SerializeObject(sendData)}\n\n");
                    await Response.Body.FlushAsync();
                }
                if (_cardGameGlobalService.Score.Count == 2)
                {
                    bool IsActiveGame = _cardGameService.CheckActivatedUser(SignedInUser);
                    var sendData = new
                    {
                        type = "setScore",
                        score = _cardGameGlobalService.Score,
                        isActiveGame = IsActiveGame
                    };
                    string JSONSendData = JsonConvert.SerializeObject(sendData);
                    await Response.WriteAsync($"id: {unixTimestamp}\n");
                    await Response.WriteAsync($"event: message\n");
                    await Response.WriteAsync($"data: {JSONSendData}\n\n");
                    await Response.Body.FlushAsync();
                }

                await Task.Delay(1000);
            }
        }
    }
}
