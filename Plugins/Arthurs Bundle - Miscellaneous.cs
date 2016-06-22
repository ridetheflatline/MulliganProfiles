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
        [DisplayName("Transfer SM and ST History")]
        public bool STTransfer { get; set; }
        [DisplayName("Bundle Language")]
        public Localization locale { get; private set; }
        [DisplayName("Auto Concede vs Arthur")]
        public bool CVA { get; set; }
        public ArthursBundleMiscellaneous()
        {
            Name = "Arthurs Bundle - Miscellaneous";
            Enabled = true;
            sLegend = false;
            STTransfer = true;
            sLegendRank = 1000;
            AC = false;
            CVA = false;
            ACDetails = "It's more of a todo list than a working feature.\nDon't expect it to be updated anytime soon ";
            locale = Localization.English;

        }

    }

    public class Miscellaneous : Plugin
    {
        public override void OnTurnEnd()
        {
            if (((ArthursBundleMiscellaneous)DataContainer).CVA)
            {
                 if (Bot.GetCurrentOpponentId() == -4057046260809111675) Bot.Concede();
            }
            base.OnTurnBegin();
        }


        public override void OnStarted()
        {
            bool complication = false;
            try
            {
                if (((ArthursBundleMiscellaneous)DataContainer).STTransfer && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Plugins\\SmartTracker.cs"))
                {
                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\SmartTracker\\"))
                    {
                        Bot.Log("[MISC] Nothing to Transfer");
                        return;
                    }
                    Bot.Log("TIME TO MOVE SOME FILES AROUND CUZ WHY NOT");
                    String directoryName = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker";
                    DirectoryInfo dirInfo = new DirectoryInfo(directoryName);
                    if (!dirInfo.Exists)
                        Directory.CreateDirectory(directoryName);

                    List<String> AllHistoryFiles = Directory
                                       .GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\SmartTracker", "*.*", SearchOption.AllDirectories).ToList();


                    foreach (var q in dirInfo.GetFiles())
                    {
                        if (q.Name == "ShaKey") continue;
                        q.Delete();
                    }
                    foreach (string file in AllHistoryFiles)
                    {

                        FileInfo mFile = new FileInfo(file);
                        // to remove name collusion
                        if (!new FileInfo(dirInfo + "\\" + mFile.Name).Exists)
                            mFile.MoveTo(dirInfo + "\\" + mFile.Name);
                        else mFile.Replace(dirInfo + "\\" + mFile.Name, dirInfo + "\\" + mFile.Name + ".bc");
                        Bot.Log(string.Format("[MISC] Succesfully moved {0} to {1}", mFile.Name, file));
                    }
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\Plugins\\SmartTracker.cs");
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Plugins\\SmartTracker.settings"))
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\Plugins\\SmartTracker.settings");
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\MulliganProfiles\\SmartMulligan.cs"))
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\MulliganProfiles\\SmartMulligan.cs");
                    ((ArthursBundleMiscellaneous)DataContainer).STTransfer = false;
                    System.Windows.Forms.MessageBox.Show("\t[IMPORTANT Arthurs' Bundle Post Data Transfer message]\n\n"
                        + "If you are seeing this box, that means update went smoothly and your old history safely transfered."

                        + "\nNow you need to navigate to Miscellaneous plugin and disable 'Transfer SM and ST'");
                    Bot.ReloadPlugins();
                    Bot.RefreshMulliganProfiles();
                    Bot.RefreshProfiles();
                }
                ((ArthursBundleMiscellaneous)DataContainer).STTransfer = false;
                base.OnStarted();
            }
            catch (Exception e)
            {
                Bot.Log("Could not transfer files :(" + e.Message);
                complication = true;
            }
            if (complication) return;
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\SmartTracker\\"))
                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\SmartTracker\\", true);
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\SmartMulligan\\"))
                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\SmartMulligan\\", true);


        }
        private bool IsFileLocked(string filename)
        {
            bool Locked = false;
            try
            {
                FileStream fs =
                    File.Open(filename, FileMode.OpenOrCreate,
                    FileAccess.ReadWrite, FileShare.None);
                fs.Close();
            }
            catch (IOException ex)
            {
                Bot.Log("Hello Masterwai " + ex.Message);
                Locked = true;
            }
            return Locked;
        }
        public override void OnGameEnd()
        {
            if (Bot.CurrentMode() != Bot.Mode.RankedWild || Bot.CurrentMode() != Bot.Mode.RankedStandard) return;
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
    public enum Localization
    {
        English, Russian
    }
}
