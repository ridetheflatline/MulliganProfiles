using SmartBot.Plugins.API;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;




namespace SmartBot.Plugins
{


    [Serializable]
    public class qCheckSmartUpdates : PluginDataContainer
    {

        /// <summary>
        /// This variable is to add extra two option to CheckSmartUpdates that will allow you
        /// to use Mulligan Tester by Botfanatic
        /// </summary>

        [DisplayName("Update Button")]
        public bool update { get; set; }



        [Browsable(false)]
        public string LSmartMulliganV3 { get; private set; }

        [Browsable(false)]
        public string LCheckSmartUpdates { get; private set; }



        public qCheckSmartUpdates()
        {
            Name = "CheckSmartUpdates";

            LSmartMulliganV3 =
                "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/MulliganProfiles/SmartMulliganV3/version.txt";
            LCheckSmartUpdates =
                "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/Plugins/CheckSmartUpdates/tracker.version";

        }
    }

    public class CheckSmartUpdates : Plugin
    {

        private readonly string MulliganDir = AppDomain.CurrentDomain.BaseDirectory + "MulliganProfiles\\";

        private readonly string MulliganInformation = AppDomain.CurrentDomain.BaseDirectory +
                                                      "MulliganProfiles\\SmartMulliganV3\\";

        private readonly string TrackerDir = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\";
        private readonly string TrackerVersion = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\CheckSmartUpdates\\";

        public override void OnTick()
        {

        }

       
        public override void OnStarted()
        {


            if (((qCheckSmartUpdates) DataContainer).update)
            {
                CheckUpdatesMulligan(((qCheckSmartUpdates) DataContainer).LSmartMulliganV3);
                CheckUpdatesTracker(((qCheckSmartUpdates) DataContainer).LCheckSmartUpdates);
            }


        }


        private void CheckUpdatesTracker(string lCheckSmartUpdates)
        {
            HttpWebRequest request = WebRequest.Create(lCheckSmartUpdates) as HttpWebRequest;
            if (request == null)
            {
                Bot.Log(string.Format("[SmartAutoUpdater] Could not get data from gitlink {0}", lCheckSmartUpdates));
                return;
            }
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (StreamReader str = new StreamReader(response.GetResponseStream()))
            using (StreamReader localVersion = new StreamReader(TrackerVersion + "tracker.version"))

            {
                string strline = str.ReadLine();
                double remoteVer = double.Parse(strline);
                double localVer = double.Parse(localVersion.ReadLine());

                if (localVer == remoteVer) Bot.Log("[CheckSmartUpdates] CheckSmartUpdates is up to date");
                if (localVer > remoteVer)
                {
                    Bot.Log(string.Format("[CheckSmartUpdates] Local Version: {0} Remote Version {1}", localVer,
                        remoteVer));
                    Bot.Log("[CheckSmartUpdates] Arthur, you are an idiot. Push new update");
                }
                if (localVer < remoteVer)
                {
                    localVersion.Close();
                    UpdateTracker(lCheckSmartUpdates, remoteVer, localVer);
                }

            }
        }

        private void UpdateTracker(string lCheckSmartUpdates, double remoteVer, double localVer)
        {
            Bot.Log(string.Format("[CheckSmartUpdates] Local Version: {0} Remote Version {1}\n\t\tUpdating...", localVer,
                remoteVer));
            HttpWebRequest trackeRequest = WebRequest
                .Create(
                    "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/Plugins/CheckSmartUpdates.cs")
                as HttpWebRequest;
            if (trackeRequest == null)
            {
                Bot.Log(string.Format("[SmartAutoUpdater] Could not get data from gitlink {0}", lCheckSmartUpdates));
                return;
            }
            using (HttpWebResponse mulResponse = trackeRequest.GetResponse() as HttpWebResponse)
            using (StreamReader trFile = new StreamReader(mulResponse.GetResponseStream()))
            using (StreamWriter updateLocalCopy = new StreamWriter(TrackerDir + "CheckSmartUpdates.cs"))
            {
                string tempfile = trFile.ReadToEnd();
                Bot.Log("[IGOT HERE]");
                updateLocalCopy.WriteLine(tempfile);
                Bot.RefreshMulliganProfiles();
                Bot.Log("[CheckSmartUpdates] CheckSmartUpdates is now fully updated");
                UpdateVersion(remoteVer, true);
            }
        }

        private void CheckUpdatesMulligan(string lSmartMulliganV3)
        {
            HttpWebRequest request = WebRequest.Create(lSmartMulliganV3) as HttpWebRequest;
            if (request == null)
            {
                Bot.Log(string.Format("[SmartAutoUpdater] Could not get data from gitlink {0}", lSmartMulliganV3));
                return;
            }
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (StreamReader str = new StreamReader(response.GetResponseStream()))
            using (StreamReader localVersion = new StreamReader(MulliganInformation + "version.txt"))

            {
                double remoteVer = double.Parse(str.ReadLine());
                double localVer = double.Parse(localVersion.ReadLine());
                Bot.Log(remoteVer.ToString(CultureInfo.InvariantCulture));
                if (localVer == remoteVer) Bot.Log("[CheckSmartUpdates] SmartMulliganV3 is up to date");
                if (localVer > remoteVer)
                {
                    Bot.Log(string.Format("[CheckSmartUpdates] Local Version: {0} Remote Version {1}", localVer,
                        remoteVer));
                    Bot.Log("[CheckSmartUpdates] Arthur, you are an idiot. Push new update");
                }
                if (localVer < remoteVer)
                {
                    localVersion.Close();
                    UpdateMulligan(lSmartMulliganV3, remoteVer, localVer);
                }

            }
        }

        private void UpdateMulligan(string lSmartMulliganV3, double remoteVer, double localVer)
        {
            Bot.Log(string.Format("[CheckSmartUpdates] Local Version: {0} Remote Version {1}\n\t\tUpdating...", localVer,
                remoteVer));
            HttpWebRequest MulliganRequest = WebRequest
                .Create(
                    "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/MulliganProfiles/SmartMulliganV3.cs")
                as HttpWebRequest;
            if (MulliganRequest == null)
            {
                Bot.Log(string.Format("[SmartAutoUpdater] Could not get data from gitlink {0}", lSmartMulliganV3));
                return;
            }
            using (HttpWebResponse mulResponse = MulliganRequest.GetResponse() as HttpWebResponse)
            using (StreamReader mulFile = new StreamReader(mulResponse.GetResponseStream()))
            using (StreamWriter updateLocalCopy = new StreamWriter(MulliganDir + "SmartMulliganV3.cs"))
            {
                string tempfile = mulFile.ReadToEnd();
                //Bot.Log("");
                updateLocalCopy.WriteLine(tempfile);
                Bot.RefreshMulliganProfiles();
                Bot.Log("[CheckSmartUpdates] SmartMulligan is now fully updated");
                UpdateVersion(remoteVer);
            }

        }


        private void UpdateVersion(double remoteVer, bool value = false)
        {

            using (
                StreamWriter localVersion =
                    new StreamWriter(value ? TrackerVersion + "tracker.version" : MulliganInformation + "version.txt",
                        false))
            {
                localVersion.WriteLine(remoteVer);
            }

        }


    }
}
