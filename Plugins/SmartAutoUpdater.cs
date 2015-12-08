using SmartBot.Plugins.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;



namespace SmartBot.Plugins
{
    [Serializable]
    public class AuContainer : PluginDataContainer
    {
        public bool CheckAfterEachGame { get; set; }
        public string GithubRawLink { get; private set; }
        public string GithubRawLink2 { get; private set; }
        public string GithubRawLink3 { get; set; }
        public string GithubRawLink4 { get; set; }
        public string GithubRawLink5 { get; set; }
        public string GithubRawLink6 { get; set; }
        public string GithubRawLink7 { get; set; }
        public string GithubRawLink8 { get; set; }
        public string GithubRawLink9 { get; set; }

        public AuContainer()
        {
            Name = "SmartAutoUpdater";
            Enabled = true;
            GithubRawLink = "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV2/MulliganProfiles/SmartMulliganV2.cs";
            GithubRawLink2 = "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV2/Plugins/SmartAutoUpdater.cs";
        }
    }

    public class BPlugin : Plugin
    {
        private static string _githubRawLink1;
        private static string _githubRawLink2;
        private static string _githubRawLink3;
        private static string _githubRawLink4;
        private static string _githubRawLink5;
        private static string _githubRawLink6;
        private static string _githubRawLink7;
        private static string _githubRawLink8;
        private static string _githubRawLink9;
        private static bool _frequentChecks;


        public static readonly string MainDir = AppDomain.CurrentDomain.BaseDirectory + "MulliganProfiles\\";
        public static readonly string MainDirPlugin = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\";
        private List<string> _gitCollection;
        public static string Templink;

        public override void OnStarted()
        {
            if (!DataContainer.Enabled)
                return;
            Initialize();
            {
                Bot.Log("[SmartAutoUpdater] History of updates can be found here: " + AppDomain.CurrentDomain.BaseDirectory + "\\SmartAutoUpdaterLog\\SmartAU_Log.txt");
                Bot.Log("========================================");
                LookForUpdates(_gitCollection);
                Bot.Log("========================================");
            }
        }

        private void Initialize()
        {
            if (!DataContainer.Enabled)
                return;
            _frequentChecks = ((AuContainer)DataContainer).CheckAfterEachGame;
            _githubRawLink1 = ((AuContainer)DataContainer).GithubRawLink;
            _githubRawLink2 = ((AuContainer)DataContainer).GithubRawLink2;
            _githubRawLink3 = ((AuContainer)DataContainer).GithubRawLink3;
            _githubRawLink4 = ((AuContainer)DataContainer).GithubRawLink4;
            _githubRawLink5 = ((AuContainer)DataContainer).GithubRawLink5;
            _githubRawLink6 = ((AuContainer)DataContainer).GithubRawLink6;
            _githubRawLink7 = ((AuContainer)DataContainer).GithubRawLink7;
            _githubRawLink8 = ((AuContainer)DataContainer).GithubRawLink8;
            _githubRawLink9 = ((AuContainer)DataContainer).GithubRawLink9;

            _gitCollection = new List<string> { _githubRawLink1, _githubRawLink2, _githubRawLink3, _githubRawLink4, _githubRawLink5, _githubRawLink6, _githubRawLink7, _githubRawLink8, _githubRawLink9 };
            
        }
        public override void OnGameEnd()
        {
            if (!_frequentChecks) return;
            Bot.Log(string.Format("[SmartAutoUpdater] FrequentChecks are enabled. Checking for plugin and mulligan updates"));
            Bot.Log("You may view update log at: " +AppDomain.CurrentDomain.BaseDirectory + "\\SmartAutoUpdaterLog\\SmartAU_Log.txt");
            Bot.Log("===================================");
            Initialize();
            LookForUpdates(_gitCollection);
            Bot.Log("===================================");
        }

        private void LookForUpdates(List<string> list)
        {
            var plugin = false;
            try
            {
                foreach (var link in list.Where(link => !string.IsNullOrEmpty(link)))
                {
                    Templink = link;
                    int index = link.LastIndexOf('/') + 1;
                    HttpWebRequest request = WebRequest.Create(link) as HttpWebRequest;
                    if (request == null)
                    {
                        Bot.Log(string.Format("[SmartAutoUpdater]Could not get data from gitlink {0}", link));
                        continue;
                    }
                    //using (StreamWriter debugFile = new StreamWriter(MAIN_DIR + "Debug" + link.Substring(index), false)) //debug writer
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        string githubReaderContent = reader.ReadToEnd();
                        string copy = githubReaderContent;
                        string cleanFolderFile;
                        var cleanGit = new string(githubReaderContent.Where(Char.IsLetter).ToArray());
                        if (githubReaderContent.Contains("PluginDataContainer"))
                        {
                            using (
                                StreamReader pluginStreamReader =
                                    new StreamReader(MainDirPlugin + link.Substring(index)))
                            {
                                plugin = true;
                                string fileInFolder = pluginStreamReader.ReadToEnd();
                                cleanFolderFile = new string(fileInFolder.Where(Char.IsLetter).ToArray());
                            }
                        }
                        else
                        {
                            using (StreamReader mulliStreamReader = new StreamReader(MainDir + link.Substring(index)))
                            {
                                string fileInFolder = mulliStreamReader.ReadToEnd();
                                cleanFolderFile = new string(fileInFolder.Where(Char.IsLetter).ToArray());
                            }
                        }

                        if (!string.Equals(cleanGit, cleanFolderFile))
                        {
                            Bot.Log(string.Format("[SmartAutoUpdater] Outdated {0} {1}",
                                plugin ? "pluign detected" : "mulligan detected", link.Substring(index)));
                            Update(copy, link.Substring(index));
                            if (plugin)
                                Bot.Log("You will need to reload your plugins in the plugin tab for updte to take effect");
                            else Bot.RefreshMulliganProfiles();

                        }
                        else
                        {
                            Bot.Log(string.Format("[SmartAutoUpdater] {0} {1} is up to date", link.Substring(index),
                                plugin ? "pluign" : "mulligan file"));
                        }
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                Bot.Log("[SmartAutoUpdater] Report this to Arthur:" + ex.Message);
            }
            catch (FileNotFoundException nofile)
            {
                Bot.Log(string.Format("Could not find file {0}. Downloading it", nofile.FileName));
                HttpWebRequest request = WebRequest.Create(Templink) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string githubReaderContent = reader.ReadToEnd();
                    string copy = githubReaderContent;
                    Update(copy, Templink.Substring(Templink.LastIndexOf('/') + 1));
                }
            }
            catch (FormatException formatException)
            {
                Bot.Log("[SmartAutoUpdater] Report this to Arthur: " + formatException.Message);
            }
            catch (DirectoryNotFoundException eDirectoryNotFoundException)
            {
                Bot.Log("[SmartAutoUpdater] Report this to Arthur:" + eDirectoryNotFoundException.Message);
            }
        }
        
        private void Update(string fileStr, string fileName)
        {
            CheckDirectory("SmartAutoUpdaterLog");
            using (var file = new StreamWriter(fileStr.Contains("PluginDataContainer") ? MainDirPlugin + fileName : MainDir + fileName, false))
            using (var updateLog = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\SmartAutoUpdaterLog\\SmartAU_Log.txt", true))
            {
                file.WriteLine(fileStr);
                Bot.Log(string.Format("[SmartAutoUpdater] Completed update for {0} {1}", fileName, fileStr.Contains("PluginDataContainer") ? "plugin" : ""));
                updateLog.WriteLine("====================================================");
                updateLog.WriteLine("{0} has been updated at {1}", fileName, DateTime.Now );
                updateLog.WriteLine("Consult corresponding forum thread for more details");
                updateLog.WriteLine("====================================================");
            }
        }
        private static void CheckDirectory(string subdir, string subdir2 = "")
        {
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/" + subdir + "/" + subdir2))
                return;
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/" + subdir + "/" + subdir2);
        }
    }
}
