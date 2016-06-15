using SmartBot.Plugins.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;


namespace SmartBot.Plugins
{
    [Serializable]
    public class ArthursBundleMiscellaneous : PluginDataContainer
    {

        [DisplayName("Stop when legend")]
        public bool sLegend { get; set; }
        [DisplayName("Stop at rank X legend")]
        public int sLegendRank { get; set; }
        [DisplayName("Auto-Concede\nUnwinable games")]
        public bool AC { get; private set; }
        [DisplayName("Auto Concede:")]
        public string ACDetails { get; private set; }
        [DisplayName("Transfer SmartTracker History")]
        public bool STTransfer { get; set; }
        public ArthursBundleMiscellaneous()
        {
            Name = "Arthurs Bundle - Miscellaneous";
            sLegend = false;
            sLegendRank = 1000;
            AC = false;
            ACDetails = "It's more of a todo list than a working feature.\nDon't expect it to be updated anytime soon ";

        }

    }

    public class Miscellaneous : Plugin
    {
        public override void OnStarted()
        {
            if (((ArthursBundleMiscellaneous)DataContainer).STTransfer && Bot.GetPlugins().Exists(c => c.DataContainer.Name == "SmartTracker"))
            {
                String directoryName = AppDomain.CurrentDomain.BaseDirectory+"\\Plugins\\ABTracker";
                DirectoryInfo dirInfo = new DirectoryInfo(directoryName);
                if (!dirInfo.Exists)
                    Directory.CreateDirectory(directoryName);

                List<String> AllHistoryFiles = Directory
                                   .GetFiles(AppDomain.CurrentDomain.BaseDirectory+"\\Plugins\\SmartTracker", "*.*", SearchOption.AllDirectories).ToList();

                foreach (string file in AllHistoryFiles)
                {
                    FileInfo mFile = new FileInfo(file);
                    // to remove name collusion
                    if (!new FileInfo(dirInfo + "\\" + mFile.Name).Exists)
                        mFile.MoveTo(dirInfo + "\\" + mFile.Name);
                }
            }
            base.OnStarted();
        }
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
