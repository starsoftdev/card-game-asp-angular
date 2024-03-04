using System.Collections;
using System.Collections.Generic;

namespace CardGame.Server.Services;

public interface ICardGameService
{
    void GenerateAllCards();
    bool CompareCards(string SignedInUser, CardItem CardA, CardItem CardB);
    void RegisterPlayerName(string PlayerName, out bool Result, out bool IsAdmin, out bool IsDuplicated);
    List<CardItem?> GetAllCards(string SignedInUser);
    void ResetGame(string SignedInUser);
    bool CheckActivatedUser(string SignedInUser);
}
public class CardItem
{
    public required string FileName { get; set; }
    public required int Value { get; set; }
}


public class CardGameService : ICardGameService
{
    private List<CardItem?> GL_AllCards = [];
    private bool IsStartedGame = false;
    private List<string> Users = [];
    private string ActivatedUser = "";    

    public void ShuffleList<T>(List<T> list)
    {
        Random random = new Random();
        int n = list.Count;
        for (int i = 0; i < n; i++)
        {
            // Randomly pick an index from i to n - 1
            int randomIndex = i + random.Next(n - i);

            // Swap list[i] with the element at randomIndex
            T temp = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = temp;
        }
    }
    public void GenerateAllCards()
    {
        int i = 1;
        List<CardItem?> allCards = [];
        for (; i <= 10; i++)
        {
            CardItem CardA = new CardItem
            {
                FileName = $"{i}_1",
                Value = i
            };
            CardItem CardB = new CardItem
            {
                FileName = $"{i}_2",
                Value = i
            };
            allCards.Add(CardA);
            allCards.Add(CardB);
        }

        ShuffleList(allCards);

        GL_AllCards = allCards;
    }

    public bool CompareCards(string SignedInUser, CardItem CardA, CardItem CardB)
    {
        if (SignedInUser == null || ActivatedUser == "" || Users.Count != 2)
        {
            throw new Exception("Game players must be 2");
        }
        if (SignedInUser != ActivatedUser)
        {
            throw new Exception("You are not activated user, please wait until the activated user has ended");
        }
        // Check valid card
        bool isExistCard = GL_AllCards.Any(Card => Card?.Value == CardA.Value);
        if (isExistCard == false)
        {
            throw new Exception("Card is already removed or invalid, please refresh your web browser.");
        }

        bool Result = false;

        if (CardA.Value == CardB.Value)
        {
            int i = 0;
            int iMax = GL_AllCards.Count();

            for (; i < iMax; i ++)
            {
                if (GL_AllCards[i]?.Value == CardA?.Value)
                {
                    GL_AllCards[i] = null;
                }
            }
            Result = true;
        }

        // Switch User
        if (Result == false)
        {
            ActivatedUser = Users.Where(u => u != ActivatedUser).ToArray()[0];
        }

        return Result;
    }

    public void RegisterPlayerName(string PlayerName, out bool Result, out bool IsAdmin, out bool IsDuplicated)
    {
        int allUsersCount = Users.Count();
        IsDuplicated = false;
        if (allUsersCount == 2)
        {
            Result = false;
            IsAdmin = false;
            return;
        }

        if (allUsersCount == 0)
        {
            IsAdmin = true;
            GenerateAllCards();
        }
        else
        {
            IsAdmin = false;
            IsDuplicated = Users.Any(u => PlayerName == u);
            if (IsDuplicated == true)
            {
                Result = false;
                return;
            }
        }

        Users.Add(PlayerName);

        Result = true;
    }

    public List<CardItem?> GetAllCards(string SignedInUser)
    {
        if (SignedInUser != null)
        {
            if (ActivatedUser == "")
            {
                ActivatedUser = SignedInUser;
            }
        } else
        {
            return [];
        }
        
        return GL_AllCards;
    }

    public void ResetGame(string SignedInUser)
    {
        ActivatedUser = SignedInUser;
        GenerateAllCards();
    }

    public bool CheckActivatedUser(string SignedInUser)
    {
        return SignedInUser != null && ActivatedUser == SignedInUser;
    }
}
