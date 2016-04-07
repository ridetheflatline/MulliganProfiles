using System;
using SmartBot.Plugins.API;
using SmartBotProfiles;
using System.Collections.Generic;

namespace SmartBotProfiles
{
    [Serializable]
    public class MidrangeRush : Profile
    {
        public ProfileParameters GetParameters(Board board)
        {
           return new ProfileParameters(BaseProfile.Rush);
        }

 	public Card.Cards SirFinleyChoice(List<Card.Cards> choices)
	{
		return choices[0];
	}
    }
}