using SmartBot.Plugins.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;
using SmartBot.Plugins;


namespace SmartBot.Plugins
{
    [Serializable]
    public class AuContainer : PluginDataContainer
    {
        public bool CheckAfterEachGame { get; set; }
        public string MulliganRawGitHub1 { get; private set; }
        public string MulliganRawGitHub2 { get; set; }
        public string MulliganRawGitHub3 { get; set; }
        public string MulliganRawGitHub4 { get; set; }
        public string MulliganRawGitHub5 { get; set; }
        public string MulliganRawGitHub6 { get; set; }
        public string MulliganRawGitHub7 { get; set; }
        public string MulliganRawGitHub8 { get; set; }
        public string MulliganRawGitHub9 { get; set; }

        public AuContainer()
        {
            Name = "AutoUpdateMulligan";
            Enabled = true;
            MulliganRawGitHub1 = "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV2/MulliganProfiles/SmartMulliganV2.cs";
            
        }
    }

    public class BPlugin : Plugin
    {
        private static string _mulliganRawGitHub1;
        private static string _mulliganRawGitHub2;
        private static string _mulliganRawGitHub3;
        private static string _mulliganRawGitHub4;
        private static string _mulliganRawGitHub5;
        private static string _mulliganRawGitHub6;
        private static string _mulliganRawGitHub7;
        private static string _mulliganRawGitHub8;
        private static string _mulliganRawGitHub9;
        private static bool _frequentChecks;

        private WebRequest _request;
        private WebResponse _respons;
        private StreamReader _responsText;
        private string _strResponse;

        private StreamReader _curMull;
        private string _strCurMulligan;
        public static readonly string MAIN_DIR = AppDomain.CurrentDomain.BaseDirectory + "\\MulliganProfiles\\";
        private List<string> _gitCollection;


        private void Reset()
        {
            _curMull.Dispose();
            _responsText.Dispose();
            _respons.Dispose();
            _request = null;
            _respons = null;
            _responsText = null;
            _strResponse = "";
            _curMull = null;
            _strCurMulligan = "";
        }
        //Constructor
        public override void OnPluginCreated()
        {
        }

        public override void OnStarted()
        {
            if (!DataContainer.Enabled)
                return;
            Initialize();
            {
                Bot.Log("================================");
                LookForUpdates(_gitCollection);
                Bot.Log("================================");
            }
        }

        private void Initialize()
        {
            if (!DataContainer.Enabled)
                return;
            _frequentChecks = ((AuContainer) DataContainer).CheckAfterEachGame;
            _mulliganRawGitHub1 = ((AuContainer)DataContainer).MulliganRawGitHub1;
            _mulliganRawGitHub2 = ((AuContainer)DataContainer).MulliganRawGitHub2;
            _mulliganRawGitHub3 = ((AuContainer)DataContainer).MulliganRawGitHub3;
            _mulliganRawGitHub4 = ((AuContainer)DataContainer).MulliganRawGitHub4;
            _mulliganRawGitHub5 = ((AuContainer)DataContainer).MulliganRawGitHub5;
            _mulliganRawGitHub6 = ((AuContainer)DataContainer).MulliganRawGitHub6;
            _mulliganRawGitHub7 = ((AuContainer)DataContainer).MulliganRawGitHub7;
            _mulliganRawGitHub8 = ((AuContainer)DataContainer).MulliganRawGitHub8;
            _mulliganRawGitHub9 = ((AuContainer)DataContainer).MulliganRawGitHub9;

            _gitCollection = new List<string> { _mulliganRawGitHub1, _mulliganRawGitHub2, _mulliganRawGitHub3, _mulliganRawGitHub4, _mulliganRawGitHub5, _mulliganRawGitHub6, _mulliganRawGitHub7, _mulliganRawGitHub8, _mulliganRawGitHub9 };
        }
        public override void OnGameEnd()
        {
            if (!_frequentChecks) return;
            Bot.Log(string.Format("[AutoUpdater] FrequentChecks are enabled. Checking for mulligan updates"));
            Bot.Log("===================================");
            Initialize();
            LookForUpdates(_gitCollection);
            Bot.Log("===================================");
        }

        private void LookForUpdates(List<string> listOfMulliganRawGitHubs)
        {
            try
            {
                foreach (string mulliganRawGitHub in listOfMulliganRawGitHubs.Where(MulliganRawGitHub => MulliganRawGitHub != null))
                {
                    int index = mulliganRawGitHub.LastIndexOf('/') + 1;
                    _request = WebRequest.Create(mulliganRawGitHub);
                    _respons = _request.GetResponse();
                    _responsText = new StreamReader(_respons.GetResponseStream());
                    _strResponse = _responsText.ReadToEnd();
                    string savedCopy = _strResponse;
                    Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                    _strResponse = rgx.Replace(_strResponse, "");
                    try
                    {
                        _curMull = new StreamReader(MAIN_DIR + mulliganRawGitHub.Substring(index));
                    }
                    catch (FileNotFoundException nofilException)
                    {
                        Bot.Log(string.Format("[AutoUpdater] {0} . Downloading it now", nofilException.Message));
                        Update(savedCopy, mulliganRawGitHub.Substring(index));
                        Reset();
                        Bot.RefreshMulliganProfiles();
                        return;
                    }
                    _strCurMulligan = _curMull.ReadToEnd();
                    _strCurMulligan = rgx.Replace(_strCurMulligan, "");

                    Bot.Log(_strCurMulligan.Equals(_strResponse)
                        ? string.Format("[AutoUpdater] {0} is up to date", mulliganRawGitHub.Substring(index))
                        : string.Format("[AutoUpdater] newer version for {0} is available. Update in process ", mulliganRawGitHub.Substring(index)));
                    if (!_strCurMulligan.Equals(_strResponse))
                    {
                        Reset();
                        Update(savedCopy, mulliganRawGitHub.Substring(index));
                        try
                        {
                            Bot.RefreshMulliganProfiles();
                        }
                        catch (ArgumentException e)
                        {
                            Bot.Log("[AutoUpdater] Error while refreshing mulligans " + e.Message);
                        }
                        return;
                    }
                    Reset();
                }

            }
            catch (NullReferenceException ex)
            {
                Bot.Log(string.Format("[AutoUpdater] Unexpected error, shouldn't hurt you, but if you keep seeing this, report it to Arthur. Source {0}", ex.Source));
            }

        }
        private void Update(string updatedMulligan, string fileName)
        {
            try
            {
                Bot.Log("===================================");
                //using (var file = new StreamWriter(MAIN_DIR + "DebugStuff/" + fileName, false))
                using (var file = new StreamWriter(MAIN_DIR + fileName, false))
                {
                    file.WriteLine(updatedMulligan);
                    Bot.Log(string.Format("[AutoUpdater] Updated {0}", fileName));
                }
            }
            catch
                (IOException ioException)
            {
                Bot.Log(string.Format("{0}. Source of your problem is {1}", ioException.Message, ioException.Source));
            }
            Bot.Log("===================================");
        }
    }
}