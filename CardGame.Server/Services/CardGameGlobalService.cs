using System.Collections.Generic;
using System.Linq;

namespace CardGame.Server.Services
{
    public class CardGameGlobalService
    {
        public List<int> ShouldRemoveCards { get; set; } = [];
        public bool ResetGame { get; set; } = false;
        public Dictionary<string, int> Score = new Dictionary<string, int>();

        private List<int> _tmpShouldRemoveByAllHTTPRequests = [];
        private Dictionary<string, bool> _tmpResetGame = new Dictionary<string, bool>();

        public void RemoveEliminateCard(string SignedInUser, int CardValue)
        {
            if (_tmpShouldRemoveByAllHTTPRequests.Any(x => x == CardValue) == true)
            {
                _tmpShouldRemoveByAllHTTPRequests.Remove(CardValue);
                ShouldRemoveCards.Remove(CardValue);
            } else
            {
                _tmpShouldRemoveByAllHTTPRequests.Add(CardValue);
            }
        }

        public void RemoveResetGame(string SignedInUser)
        {
            bool containKey = _tmpResetGame.ContainsKey(SignedInUser);
            bool oldValue = false;
            _tmpResetGame.TryGetValue(SignedInUser, out oldValue);
            if (oldValue == false)
            {
                if (containKey == false)
                {
                    _tmpResetGame.Add(SignedInUser, true);
                } 
                else
                {
                    _tmpResetGame[SignedInUser] = true;
                }
            }

            var filteredDictionary = _tmpResetGame.Where(pair => pair.Value == true).ToList();
            
            if (filteredDictionary.Count() == 2)
            {
                _tmpResetGame.Clear();
                ResetGame = false;
            }
        }
    }
}
