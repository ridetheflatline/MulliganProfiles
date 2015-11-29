using System;
using System.Collections.Generic;
using SmartBot.Plugins.API;

namespace SmartBot.Mulligan
{
    [Serializable]
    public class DefaultMulliganProfile : MulliganProfile
    {
       
        public List<Card.Cards> HandleMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            var CardsToKeep = new List<Card.Cards>();
            var HasCoin = choices.Count == 4;
            CardsToKeep.Add(Card.Cards.EX1_029);
            return CardsToKeep;
        }
    }
}
