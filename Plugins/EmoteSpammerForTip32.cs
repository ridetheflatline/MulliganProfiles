using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartBot.Plugins;
using SmartBot.Plugins.API;


namespace RopeToVictory
{
    [Serializable]
    public class EmoteSpammerForTip32 : PluginDataContainer
    {
        //Init vars
        public EmoteSpammerForTip32()
        {
            Name = "RopeToVictory";
        }

    }

    public class EmoteSpammerForTip32 : Plugin
    {
        public override async void OnTurnBegin()
        {
            Bot.SuspendBot();
            await Task.Delay(20000);
            Bot.ResumeBot();
        }
        public override void OnTick()
        {
            Bot.Log(string.Format("[TiP32] Spamming {0}", Bot.EmoteType.Greetings));
            Bot.SendEmote(Bot.EmoteType.Greetings);
        }

    }
}