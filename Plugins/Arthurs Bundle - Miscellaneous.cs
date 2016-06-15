using SmartBot.Plugins.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;


namespace SmartBot.Plugins
{
    [Serializable]
    public class ArthursBundleMiscellaneous : PluginDataContainer
    {


        public bool sLegend { get; set; }
        public int sLegendRank { get; set; }

        public ArthursBundleMiscellaneous()
        {
            Name = "Arthurs Bundle - Miscellaneous";
            sLegend = false;
            sLegendRank = 1000;

        }

    }

    public class Miscellaneous : Plugin
    {
        public override void OnGameEnd()
        {
            if (((ArthursBundleMiscellaneous)DataContainer).sLegend && Bot.GetPlayerDatas().GetRank() == 0)
            {
                Bot.Log("[AB - Miscellaneous] You are Legend, go pat yourself on the back.");
                Bot.StopBot();
            }
            if (((ArthursBundleMiscellaneous)DataContainer).sLegendRank <= Bot.GetPlayerDatas().GetLegendIndex())
            {
                Bot.Log("[AB - Miscellaneous] You reached your desired legend rank. Go pat yourself on the back");
            }
            base.OnGameEnd();
        }

    }
}
