using SmartBot.Plugins.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using SmartBot.Plugins;


namespace SmartBot.Plugins
{
    [Serializable]
    public class auContainer : PluginDataContainer
    {
        public string GitLink1 { get; private set; }
        public string GitLink2 { get; set; }
        public string GitLink3 { get; set; }

        public string GitLink4 { get; set; }
        public string GitLink5 { get; set; }
        public string GitLink6 { get; set; }
        public string GitLink7 { get; set; }
        public string GitLink8 { get; set; }
        public string GitLink9 { get; set; }

        public auContainer()
        {
            Name = "AutoUpdateMulligan";
            Enabled = true;
            GitLink1 = "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV2/MulliganProfiles/SmartMulliganV2.cs";
        }
    }

    public class BPlugin : Plugin
    {
        private static string _gitLink1;
        private static string _gitLink2;
        private static string _gitLink3;
        private static string _gitLink4;
        private static string _gitLink5;
        private static string _gitLink6;
        private static string _gitLink7;
        private static string _gitLink8;
        private static string _gitLink9;


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
        }

        private void Initialize()
        {
            if (!DataContainer.Enabled)
                return;
            _gitLink1 = ((auContainer)DataContainer).GitLink1;
            _gitLink2 = ((auContainer)DataContainer).GitLink2;
            _gitLink3 = ((auContainer)DataContainer).GitLink3;
            _gitLink4 = ((auContainer)DataContainer).GitLink4;
            _gitLink5 = ((auContainer)DataContainer).GitLink5;
            _gitLink6 = ((auContainer)DataContainer).GitLink6;
            _gitLink7 = ((auContainer)DataContainer).GitLink7;
            _gitLink8 = ((auContainer)DataContainer).GitLink8;
            _gitLink9 = ((auContainer)DataContainer).GitLink9;

            _gitCollection = new List<string> { _gitLink1, _gitLink2, _gitLink3, _gitLink4, _gitLink5, _gitLink6, _gitLink7, _gitLink8, _gitLink9};
        }

        public override void OnGameEnd()
        {
            try
            {
                LookForUpdates(_gitCollection);
            }
            catch (Exception ex)
            {
                Bot.Log("Error while looking for updates: " +ex.Message);
            }
        }

        private void LookForUpdates(List<string> listOfGitLinks)
        {

            foreach (string gitLink in listOfGitLinks.Where(gitLink => gitLink != null))
            {
                try
                {
                    int index = gitLink.LastIndexOf('/') + 1;
                    _request = WebRequest.Create(gitLink);
                    _respons = _request.GetResponse();
                    _responsText = new StreamReader(_respons.GetResponseStream());
                    _strResponse = _responsText.ReadToEnd();
                    string savedCopy = _strResponse;
                    Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                    _strResponse = rgx.Replace(_strResponse, "");

                    _curMull = new StreamReader(MAIN_DIR + gitLink.Substring(index));
                    _strCurMulligan = _curMull.ReadToEnd();
                    _strCurMulligan = rgx.Replace(_strCurMulligan, "");

                    Bot.Log(_strCurMulligan.Equals(_strResponse)
                        ? string.Format("[AutoUpdater] {0} is up to date", gitLink.Substring(index))
                        : string.Format("[AutoUpdater] newer version for {0} is available ", gitLink.Substring(index)));
                    if (!_strCurMulligan.Equals(_strResponse))
                    {
                        _curMull.Dispose();
                        _responsText.Dispose();
                        _respons.Dispose();
                        Reset();
                        Update(savedCopy, gitLink.Substring(index));
                        try
                        {
                            Bot.RefreshMulliganProfiles();
                        }
                        catch (ArgumentException e)
                        {
                            Bot.Log("Error while refreshing mulligans " +e.Message);
                        }
                        return;
                    }
                    _curMull.Dispose();
                    _responsText.Dispose();
                    _respons.Dispose();
                    Reset();
                }
                catch (NullReferenceException ex)
                {
                    Bot.Log(string.Format("{0} is caused by {1}" ,ex.Message, ex.Source));
                }
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