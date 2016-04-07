using System;
using SmartBot.Plugins.API;
using SmartBotProfiles;
using System.Collections.Generic;

namespace SmartBotProfiles
{
    [Serializable]
    public class MidrangeDefault : Profile
    {
        public ProfileParameters GetParameters(Board board)
        {
            return new ProfileParameters(BaseProfile.Default);
        }

        public Card.Cards SirFinleyChoice(List<Card.Cards> choices)
	{
		return choices[0];
	}

    }
}