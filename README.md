## Install environment for the project.
- Used Visual studio 2022
- Used Node.js 20.11.1

## Speical logic & How I developed the test project.

###	Backend Part, ASP.NET C#

1)	Routes
#### /get-game-state
-Get Game Status
-Generate Cards(20 items) and return cards to client
#### /compare-cards
- Validate if requested data of cards are validate.
-	Validate if the Activated user has requested.
- Compare 2 cards and determine the result
#### /register-player-name
- Register player name on system
-	Validate the duplicated user name
-	Keep the session of user data
-	Try catch of the exceed users(two players must be)
#### /check-game-state
- Check game’s available or not
#### /update-cards
- Server Side Event API, it’s for updating real-time
#### … More routes…	

3)	Specialist in the game
-	A user can refresh the web browser while he’s playing the game but the current game state will be kept.
-	Game data is used by Singleton software architecture model so real-time data will be kept in system memory as the global data.
-	By using Server Side Event, the game state will be automatically updated in the real-time.
4)	Used Swagger documentation for the API
Note:
-	SSE for updating in real-time in the backend

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

###	Frontend Part, Angular 17
-	I have used Angular material components for UI development.
-	Server Side Event(SSE) is used for updating in real-time.


### Result
![alt text](https://github.com/starsoftdev/card-game-asp-angular/blob/master/1.png?raw=true)
![alt text](https://github.com/starsoftdev/card-game-asp-angular/blob/master/2.png?raw=true)

