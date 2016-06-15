﻿using SmartBot.Plugins.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using SmartBot.Database;


namespace SmartBot.Plugins
{
    public static class Extension
    {
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }

        public static void AddOrUpdateDeck<TKey>(this IDictionary<TKey, int> map, TKey key)
        {
            if (map.ContainsKey(key))
                map[key]++;
            else map[key] = 0;
        }
        public static object Fetch(this List<Plugin> list, string name, string data)
        {
            return list.Find(c => c.DataContainer.Name == name).GetProperties()[data];
        }  

        //kappa
        public static bool ContainsAll<T1>(this IList<T1> list, params T1[] items)
        {
            return !items.Except(list).Any();
        }
        public static bool ContainsSome<T1>(this IList<T1> list, params T1[] items)
        {
            return list.Intersect(items).Any();
        }
        public static bool ContainsAtLeast<T1>(this IList<T1> list, int minReq, params T1[] items)
        {
            return list.Intersect(items).Count() >= minReq;
        }
        public static int RaceCount(this IList<Card.Cards> list, Card.CRace wCrace)
        {
            return list.Count(cards => CardTemplate.LoadFromId(cards).Race == wCrace);
        }
        public static bool IsArena(this Bot.Mode mode)
        {
            return mode == Bot.Mode.Arena || mode == Bot.Mode.ArenaAuto;
        } 
        public static int QualityCount(this IList<Card.Cards> list, Card.CQuality qQuality)
        {
            return list.Count(cards => CardTemplate.LoadFromId(cards).Quality == qQuality);
        }
        public static bool IsRenoDeck(this IList<Card.Cards> list, int treshhold = 10)
        {
            bool renoCheck = list.Count == list.Distinct().Count();
            return (renoCheck && list.Count >= treshhold) || list.Contains(Cards.RenoJackson);
        }
        public static bool IsCthun(this IList<Card.Cards> list)
        {
            return list.ContainsSome(Cards.KlaxxiAmberWeaver, Cards.DarkArakkoa,  Cards.HoodedAcolyte, Cards.TwilightDarkmender
                , Cards.BladeofCThun, Cards.UsherofSouls, Cards.AncientShieldbearer, Cards.TwilightGeomancer, Cards.DiscipleofCThun, Cards.TwilightElder
                , Cards.CThunsChosen, Cards.CrazedWorshipper, Cards.SkeramCultist, Cards.TwinEmperorVeklor, Cards.Doomcaller);
        }
        public static bool IsBasic<T1>(this IList<T1> list, IList<T1> list2)
        {
            return list.Intersect(list2).Count() < 3;
        }
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
            where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }
    }

    [Serializable]
    public class ABTracker : PluginDataContainer
    {
        /// <summary>
        /// Русские пользователи должны поменять 'false' на 'true' 
        /// для локализации трекера
        /// </summary>
        private const bool Russian = false;
        /// <summary>
        /// This variable is to add extra two option to ABTracker that will allow you
        /// to use Mulligan Tester by Botfanatic
        /// </summary>
        private const bool DebugTesting = true;
        [Browsable(false)]
        public Style ArenaStyle { get; set; }
        [DisplayName("[A]---------------------")]
        public string sectionA { get; private set;}

        [DisplayName(Russian ? "[A] Донат Трекеру" : "[A] Donation link")]
        public string donation { get; set; }
        [DisplayName("[B]---------------------")]
        public string sectionB { get; private set; }
        [DisplayName(Russian ? "[B] Авто-Обновление" : "[B] Auto Update")]
        public bool AutoUpdate { get; set; }
        [DisplayName(Russian ? "[B] Режим Обновления" : "[B] Update Mode")]
        public Update UpdateMode { get; set; }
        [DisplayName(Russian ? "[B] Описание":"[B] Description")]
        public string UpdateGlossary { get; private set; }
        [Browsable(false)]
        public double Mversion { get; private set; }
        [Browsable(false)]
        public double Tversion { get; private set; }
        [DisplayName(Russian ? "[B] Версии" : "[B] Version")]
        public string Versions { get; private set; }
        [DisplayName("[C]---------------------")]
        public string sectionC { get; private set; }

        [DisplayName(Russian ? "[C] Определение Колоды" : "[C] ID Mode")]
        public IdentityMode Mode { get; set; }
        [DisplayName(Russian ? "[C] Ваша Колода" : "[C] Manual -f Deck")]
        public DeckType ForcedDeckType { get; set; }
        [Browsable(DebugTesting ? true : false)]
        [DisplayName("Mulligan Tester: you")]
        public DeckType MulliganTesterYourDeck { get; set; }
        [Browsable(DebugTesting ? true : false)]
        [DisplayName("Mulligan Tester: enemy")]
        public DeckType MulliganTEsterEnemyDeck { get; set; }

        [DisplayName(Russian ? "[C] Тренер" : "[C] Coach")]
        public bool PredictionDisplay { get; set; }

        [DisplayName(Russian ? "Словарь" : "Glossary")]
        public string Dictionary { get; private set; }

        [Browsable(false)]
        public string LSmartMulligan { get; private set; }
        [Browsable(false)]
        public string LABTracker { get; private set; }

        [Browsable(false)]
        public DeckType AutoFriendlyDeckType { get; set; }
        [Browsable(false)]
        public Style AutoFriendlyStyle { get; set; }
        [Browsable(false)]
        public DeckType EnemyDeckTypeGuess { get; set; }
        [Browsable(false)]
        public Style EnemyDeckStyleGuess { get; set; }
        [Browsable(false)]
        public int SynchEnums { get; set; }
        [DisplayName("Hall of Fame")]
        public string HallOfFame { get; private set; }
        
        [DisplayName("[E]---------------------")]

        public string sectionE { get; private set; }
        [DisplayName("[E] Show Glossary Button ")]
        public bool GlossaryButton { get; set; }
        [DisplayName("[E] Stop after Legend")]
        public bool StopLegend { get; set; }
        [DisplayName("[E] Stop at Legend Rank")]
        public int StopLegendX { get; set; }
        [DisplayName("[E] Predict enemy from turn")]
        public int ProfilePredictionTurn { get; set; }

        [Browsable(false)]
        public int CurrentTurn { get; set; }
        [Browsable(false)]
        public Card.CClass Enemy { get; set; }

       
        public ABTracker()
        {
            Name = "Arthurs Bundle - Tracker";
            donation = "http://bit.ly/ABTrackerDonation";
            ForcedDeckType = DeckType.Unknown;
            MulliganTEsterEnemyDeck = DeckType.Unknown;
            MulliganTesterYourDeck = DeckType.Arena;
            AutoUpdate = true;
            AutoFriendlyDeckType = DeckType.Unknown;
            EnemyDeckTypeGuess = DeckType.Unknown;
            Enemy = Card.CClass.JARAXXUS;
            LSmartMulligan = "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/MulliganProfiles/SmartMulliganV3/version.txt";
            LABTracker = "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/Plugins/ABTracker/tracker.version";

        }
        public void RefreshMenu()
        {
            donation = "http://bit.ly/ABTrackerDonation";
        }
        public void ReloadDictionary()

        {
            RefreshMenu();
            Dictionary = "Detailed Summary: winrate vs decks + classes\nMinimal: winrate against decks\n"
                +"Card Breakdown is currently unavailable\nMystery Button: Enable at your own risk";
            UpdateGlossary = "Hard Update: fully overwrtites mulligan file. [Recomended for casual botters]"
                +"\nSoft Update: Will keep your customly defined mulligans [Recomended for advanced users]";
            sectionA = "[DONATION SECTION]";
            sectionB = "[AUTO UPDATE SECTION]";
            sectionC = "[IDENTIFICATION SECTION]";
            sectionE = "[EXTRA FEATURES]";
            if (Russian)
            {
                Dictionary = "[placeholder]";
            }
        }
        public bool Ru()
        {
            return Russian;
        }


        public void VersionCheck()
        {
            try
            {
                using (StreamReader Tversionl = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "Plugins\\ABTracker\\tracker.version"))
                using (StreamReader Mversionl = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "MulliganProfiles\\SmartMulliganV3\\version.txt"))

                {
                    NumberFormatInfo format = new NumberFormatInfo
                    {
                        NumberGroupSeparator = ",",
                        NumberDecimalSeparator = "."
                    };
                    Tversion = double.Parse(Tversionl.ReadLine(), format);
                    Mversion = double.Parse(Mversionl.ReadLine(), format);
                    //Bot.Log(string.Format("[Debug] saving {0} and {1}", Tversion, Mversion));
                    Versions = string.Format("ABTracker: {0}\nSmartMulligan: {1}", Tversion, Mversion);
                    HallOfFame = "[SmartMulligan]" +
                                 "\nTruci, Wirmate, Botfanatic, TheBeast792, Sylvanas2077, Masterwai" +
                                 "\n[ABTracker]" +
                                 "\nWirmate";
                }
            }

            catch (Exception e)
            {
                Bot.Log("[Version Update Failed]" + e.Message);
            }
            RefreshMenu();
        }
    }
    public enum Plugins
    {
        History,
        MulliganCore,
    }
    public class SmTracker : Plugin
    {
        public bool identified = false;
        public bool pregameEnemyIdentified = false;
        private DeckData informationData;
        private readonly string MulliganDir = AppDomain.CurrentDomain.BaseDirectory + "MulliganProfiles\\";
        private readonly string MulliganInformation = AppDomain.CurrentDomain.BaseDirectory + "MulliganProfiles\\SmartMulliganV3\\";
        private readonly string TrackerDir = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\";
        private readonly string TrackerVersion = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\ABTracker\\";
        private int _screenWidth;
        private int _screenHeight;
        public bool Russian;
        public readonly Dictionary<Plugins, string> plugins = new Dictionary<Plugins, string>
        {
            {Plugins.History, "Arthurs Bundle - History" }, {Plugins.MulliganCore, "Arthurs Bundle - Mulligan Core" }
        };
        private int PercToPixWidth(int percent)
        {
            return (int)((_screenWidth / 100.0f) * percent);
        }

        private int PercToPixHeight(int percent)
        {
            return (int)((_screenHeight / 100.0f) * percent);
        }

        private float RgbToSrgb(int rgb)
        {
            return (rgb / (255.0f));
        }

        public Card.CClass Enemy;
        public Board PregameBoard;
        public static readonly List<Bot.Mode> AllowedModes = new List<Bot.Mode>
        {
            Bot.Mode.UnrankedStandard, Bot.Mode.UnrankedWild, Bot.Mode.RankedStandard, Bot.Mode.RankedWild, Bot.Mode.Practice, Bot.Mode.None
        };
        public static bool talkedWithMulligan = false;
        #region GameEvents()
        public override void OnTick()
        {
            if (!_started || !_supported) return;
            if (((ABTracker)DataContainer).PredictionDisplay && ((ABTracker)DataContainer).EnemyDeckTypeGuess != DeckType.Unknown && !talkedWithMulligan)
            {
                GUI.ClearUI();
                GUI.AddElement(
                    new GuiElementText(
                        "Prediction: " + ((ABTracker)DataContainer).EnemyDeckTypeGuess
                        , (_screenWidth) / 64, PercToPixHeight(40), 155, 30, 16, 255, 215, 0));
                talkedWithMulligan = true;
            }
            try
            {
                if (Bot.CurrentScene() == Bot.Scene.GAMEPLAY) return;
                ((ABTracker)DataContainer).EnemyDeckTypeGuess = DeckType.Unknown;
                ((ABTracker)DataContainer).EnemyDeckStyleGuess = Style.Unknown;
                identified = false;
                pregameEnemyIdentified = false;
                talkedWithMulligan = false;

            }
            catch (Exception e)
            {
                Bot.Log("[ABTracker] Encountered error: " + e.Message);
                identified = true;
                pregameEnemyIdentified = true;
                talkedWithMulligan = true;
            }
        }

        public override void OnGameEnd()
        {
            ((ABTracker)DataContainer).EnemyDeckTypeGuess = DeckType.Unknown;
            ((ABTracker)DataContainer).EnemyDeckStyleGuess = Style.Unknown;
            if (((ABTracker)DataContainer).StopLegend && Bot.GetPlayerDatas().GetRank() == 0)
            {
                Bot.Log("[ABTracker] You are Legend, bot will now stop. ");
                Bot.StopBot();
            }
            if(((ABTracker)DataContainer).StopLegendX <= Bot.GetPlayerDatas().GetLegendIndex())
            base.OnGameEnd();
        }

        public override void OnGameBegin()
        {
            ((ABTracker)DataContainer).CurrentTurn = 0;
            Bot.Log("-----------Game Begun-------------");
            IdentifyMyStuff();
        }
        #endregion
       
        public void IdentifyMyStuff()
        {
            if (identified || !_supported) return;

            informationData = GetDeckInfo(Bot.CurrentDeck().Class, Bot.CurrentDeck().Cards);
            ((ABTracker)DataContainer).AutoFriendlyDeckType = informationData.DeckType;
            ((ABTracker)DataContainer).AutoFriendlyStyle = DeckStyles[informationData.DeckType];
            if (((ABTracker)DataContainer).Mode == IdentityMode.Manual)
            {
                DeckType tempType = informationData.DeckType;
                informationData.DeckType = ((ABTracker)DataContainer).ForcedDeckType;
                informationData.DeckStyle = DeckStyles[((ABTracker)DataContainer).ForcedDeckType];
                Bot.Log(string.Format("[ABTracker] You are forcing SmartMulligan to treat your deck as {0}, {1}," + "\n\t\t[Debug] Tracker would have recognized it as {2}, {3}", informationData.DeckType, informationData.DeckStyle, tempType, DeckStyles[tempType]));
            }

            identified = true;
        }


        public override void OnPluginCreated()
        {
            CheckDirectory(MulliganInformation);
            CheckDirectory(TrackerVersion);
            CheckDirectory(AppDomain.CurrentDomain.BaseDirectory + "Logs\\ABTracker\\");
            CheckFiles();
            ((ABTracker)DataContainer).VersionCheck();
            ((ABTracker)DataContainer).ReloadDictionary();
            ((ABTracker)DataContainer).SynchEnums = Enum.GetNames(typeof(DeckType)).Length;
        }

        private void CheckFiles()
        {
            if (!File.Exists(TrackerVersion + "tracker.version"))
            {
                string createText = "0.001" + Environment.NewLine;
                File.WriteAllText(TrackerVersion + "tracker.version", createText);
            }
            if (!File.Exists(MulliganInformation + "version.txt"))
            {
                string createText = "0.001" + Environment.NewLine;
                File.WriteAllText(MulliganInformation + "version.txt", createText);
            }
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\MatchHistory.txt"))
            {
                File.Create(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\MatchHistory.txt");
            }
        }

        private static bool _started = false;
        private static bool _supported = false;
        public override void OnStarted()
        {
            //System.Windows.Forms.MessageBox.Show("Hello Darkness my old friend");
            Russian = ((ABTracker)DataContainer).Ru();
            SetupMulliganTester();
            CreateCardReport(Bot.CurrentDeck().Cards);
            if (!AllowedModes.Contains(Bot.CurrentMode()))
            {
                Bot.Log(string.Format("[ABTracker] You are playing {0} mode." +
                                      " Tracker has no use for information gathered here," +
                                      " defaulting your mulligan to Arena." +
                                      " Forced Deck Type is ignored", Bot.CurrentMode().ToString()));
                ((ABTracker)DataContainer).AutoFriendlyDeckType = DeckType.Arena;
                ((ABTracker)DataContainer).AutoFriendlyStyle = Style.Tempo;
                _supported = false;
                ((ABTracker)DataContainer).Enabled = true;
                return;
            }
            _supported = true;
            _started = true;
            
            IdentifyMyStuff();
            Bot.Log("--------------This is the deck that Tracker Picked up -------------------" +
            "\n" + Bot.CurrentDeck().Cards.Aggregate("", (current, q) => current +
              ("Cards." + CardTemplate.LoadFromId(q).Name.Replace(" ", "") + ", ")));
            Bot.Log("---------------If it's wrong, just start the bot again------------------------");
            if (((ABTracker)DataContainer).Mode == IdentityMode.Auto)
                Bot.Log(string.Format("[ABTracker] Succesfully Identified your deck as: [{0}:{1}]", informationData.DeckType, informationData.DeckStyle));
            GUI.ClearUI();
            if (((ABTracker)DataContainer).PredictionDisplay)
                GUI.AddElement(
                    new GuiElementText(
                        "Prediction: " + ((ABTracker)DataContainer).EnemyDeckTypeGuess + "|" +
                        ((ABTracker)DataContainer).EnemyDeckStyleGuess
                        , (_screenWidth) / 64, PercToPixHeight(40), 155, 30, 16, 255, 215, 0));
            if (((ABTracker)DataContainer).AutoUpdate)
            {
                CheckUpdatesMulligan(((ABTracker)DataContainer).LSmartMulligan);
                ((ABTracker)DataContainer).VersionCheck();

                CheckUpdatesTracker(((ABTracker)DataContainer).LABTracker);
                ((ABTracker)DataContainer).VersionCheck();
                               
            }
            

        }
        private Dictionary<Card.Cards, Statistics> CardStats = new Dictionary<Card.Cards, Statistics>(); 
        private void CreateCardReport(List<string> cards)
        {
            foreach (var q in cards.Distinct())
            {
                //CardStats.AddOrUpdate((Card.Cards) Enum.Parse(typeof (Card.Cards), q), new Statistics(((ABTracker)DataContainer).AutoFriendlyDeckType, ));
            }
        }

        private void SetupMulliganTester()
        {
            using (
                StreamWriter mt =
                    new StreamWriter(AppDomain.CurrentDomain.BaseDirectory +
                                     "\\MulliganProfiles\\SmartMulliganV3\\mt.txt"))
            {
                mt.WriteLine("{0}:{1}:{2}:{3}",
                    ((ABTracker)DataContainer).MulliganTesterYourDeck,
                    DeckStyles[((ABTracker)DataContainer).MulliganTesterYourDeck],
                    ((ABTracker)DataContainer).MulliganTEsterEnemyDeck,
                    DeckStyles[((ABTracker)DataContainer).MulliganTEsterEnemyDeck]);
            }
        }

        

        #region autoupdate
        private void CheckUpdatesTracker(string lABTracker)
        {
            HttpWebRequest request = WebRequest.Create(lABTracker) as HttpWebRequest;
            if (request == null)
            {
                Bot.Log(string.Format("[SmartAutoUpdater] Could not get data from gitlink {0}", lABTracker));
                return;
            }
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (StreamReader str = new StreamReader(response.GetResponseStream()))
            using (StreamReader localVersion = new StreamReader(TrackerVersion + "tracker.version"))

            {
                string[] remote_version = str.ReadLine().Split('.');
                string[] local__version = localVersion.ReadLine().Split('.');
                int remote_major = int.Parse(remote_version[0]);
                int remote_minor = int.Parse(remote_version[1]);
                int local__major = int.Parse(local__version[0]);
                int local__minor = int.Parse(local__version[1]);
                string r_version = remote_major + "." + remote_minor;
                string l_version = local__major + "." + remote_minor;
                if (remote_major == local__major && remote_minor == local__minor)
                {
                    Bot.Log("[ABTracker] ABTracker is up to date");
                    return;
                }
                if (remote_major < local__major ||
                    (remote_major == local__major && remote_minor < local__minor))
                {
                    Bot.Log(string.Format("[ABTracker] Remote Version: {0}.{1} Local Version {2}.{3}",
                        remote_major, remote_minor, local__major, local__minor));
                    Bot.Log("[ABTracker] Arthur, you are an idiot. Push new update");
                    return;
                }
                localVersion.Close();
                UpdateTracker(lABTracker, r_version, l_version);
            }
        }

        private void UpdateTracker(string lABTracker, string remoteVer, string localVer)
        {
            Report(string.Format("Updater is using {0} remote version and {1} local version [origin:tracker]", remoteVer, localVer));
            Bot.Log(string.Format("[ABTracker] Local Version: {0} Remote Version {1}\n\t\tUpdating...", localVer, remoteVer));
            HttpWebRequest trackeRequest = WebRequest.Create("https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/Plugins/ArthursBundleTracker.cs") as HttpWebRequest;     //new
            if (trackeRequest == null)
            {
                Bot.Log(string.Format("[SmartAutoUpdater] Could not get data from gitlink {0}", lABTracker));
                return;
            }
            using (HttpWebResponse mulResponse = trackeRequest.GetResponse() as HttpWebResponse)
            using (StreamReader trFile = new StreamReader(mulResponse.GetResponseStream()))
            using (StreamWriter updateLocalCopy = new StreamWriter(TrackerDir + "ArthursBundleTracker.cs"))
            {
                string tempfile = trFile.ReadToEnd();
                updateLocalCopy.WriteLine(tempfile);
                Bot.ReloadPlugins();
                Bot.Log("[ABTracker] ABTracker is now fully updated");       //
                UpdateVersion(remoteVer, true);
            }
        }

        private void CheckUpdatesMulligan(string lSmartMulligan)
        {
            HttpWebRequest request = WebRequest.Create(lSmartMulligan) as HttpWebRequest;
            if (request == null)
            {
                Bot.Log(string.Format("[SmartAutoUpdater] Could not get data from gitlink {0}", lSmartMulligan));
                return;
            }
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (StreamReader str = new StreamReader(response.GetResponseStream()))
            using (StreamReader localVersion = new StreamReader(MulliganInformation + "version.txt"))

            {
                string[] remote_version = str.ReadLine().Split('.');
                string[] local__version = localVersion.ReadLine().Split('.');
                int remote_major = int.Parse(remote_version[0]);
                int remote_minor = int.Parse(remote_version[1]);
                int local__major = int.Parse(local__version[0]);
                int local__minor = int.Parse(local__version[1]);
                string r_version = remote_major + "." + remote_minor;
                string l_version = local__major + "." + remote_minor;
                if (remote_major == local__major && remote_minor == local__minor)
                {
                    Bot.Log("[SmartMulligan] SmartMulligan is up to date");
                    return;
                }
                if (remote_major < local__major ||
                    (remote_major == local__major && remote_minor < local__minor))
                {
                    Bot.Log(string.Format("[SmartMulligan] Local Version: {0}.{1} Remote Version {2}.{3}",
                        remote_major, remote_minor, local__major, local__minor));
                    Bot.Log("[SmartMulligan] Arthur, you are an idiot. Push new update");
                    return;
                }
                localVersion.Close();
                UpdateMulligan(lSmartMulligan, r_version, l_version);


            }
        }

        private void UpdateMulligan(string lSmartMulligan, string remoteVer, string localVer)
        {
            Report(string.Format(" Updater is using {0} remote version and {1} local version [origin:uMulligan]", remoteVer, localVer));
            Bot.Log(string.Format("[ABTracker] Local Version: {0} Remote Version {1}\n\t\tUpdating...", localVer, remoteVer));
            HttpWebRequest MulliganRequest = WebRequest.Create("https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/MulliganProfiles/MulliganBundle.cs") as HttpWebRequest;
            if (MulliganRequest == null)
            {
                Bot.Log(string.Format("[SmartAutoUpdater] Could not get data from gitlink {0}", lSmartMulligan));
                return;
            }

            string myCustom = "";
            try
            {
                string myLocalFile = File.ReadAllText(MulliganDir + "SmartMulligan.cs");
                myCustom = myLocalFile.Substring(myLocalFile.IndexOf("#region Custom") + "#region Custom".Length, myLocalFile.LastIndexOf("#endregion Custom")
                    - (myLocalFile.IndexOf("#region Custom") + "#region Custom".Length));
                Bot.Log(myCustom);
                Bot.Log("==============================");
            }catch(FileNotFoundException)
            {
                Bot.Log("[ABTracker] This is your first time running tracker, downloading SmartMulligan");
            }
            using (HttpWebResponse mulResponse = MulliganRequest.GetResponse() as HttpWebResponse)
            using (StreamReader mulFile = new StreamReader(mulResponse.GetResponseStream()))
            using (StreamWriter updateLocalCopy = new StreamWriter(MulliganDir + "SmartMulligan.cs"))
            {

                string tempfile = mulFile.ReadToEnd();
                string low = tempfile.Substring(0, tempfile.IndexOf("#region Custom")+14);
                string high = tempfile.Substring(tempfile.IndexOf("#endregion Custom"));
                string final = low + myCustom + high;
                if (((ABTracker)DataContainer).UpdateMode == Update.Soft)
                    updateLocalCopy.WriteLine(final);
                else updateLocalCopy.WriteLine(tempfile);
                Bot.RefreshMulliganProfiles();
                Bot.Log("[ABTracker] SmartMulligan is now fully updated");
                UpdateVersion(remoteVer);
            }

        }


        private void UpdateVersion(string remoteVer, bool value = false)
        {
            using (StreamWriter localVersion = new StreamWriter(value ? TrackerVersion + "tracker.version" : MulliganInformation + "version.txt", false))
            {
                localVersion.WriteLine(remoteVer);
            }
        }
        #endregion

        public override void OnStopped()
        {
            GUI.ClearUI();
            ((ABTracker)DataContainer).Enemy = Card.CClass.JARAXXUS;
            identified = false;
            _started = false;
            pregameEnemyIdentified = false;
            talkedWithMulligan = false;
             if (!_supported)
                ((ABTracker)DataContainer).Enabled = true;
        }

        public static int Turn;
        // Dictionary<int, Func<string, bool>> Mystery = new Dictionary<int, Func<TResult>>();
        public override void OnTurnBegin()
        {
            base.OnTurnBegin();
            if (Bot.CurrentMode() == Bot.Mode.Arena || Bot.CurrentMode() == Bot.Mode.ArenaAuto)
            {
                ((ABTracker)DataContainer).ArenaStyle = GetDeckInfo(Bot.CurrentBoard.FriendClass, Bot.CurrentDeck().Cards).DeckStyle;
                Bot.Log(string.Format("[ST_Debug] Your arena deck mostly resembles {0} type deck", ((ABTracker)DataContainer).ArenaStyle));
            }
            ((ABTracker)DataContainer).CurrentTurn += 1;
            if (!_supported) return;
            Turn = ((ABTracker)DataContainer).CurrentTurn;
            ShowPrediction();
            Bot.Log("========"  +Bot.CurrentBoard.TurnCount);
            if (Bot.CurrentBoard.TurnCount < ((ABTracker)DataContainer).ProfilePredictionTurn) return;
            try
            {
                CheckOpponentDeck();
            }
            catch (Exception e)
            {
                Bot.Log(string.Format("{0} || {1} || {2}", e.Message, e.TargetSite, e.InnerException));
            }
            ShowPrediction();
            

        }
        public void ShowPrediction()
        {
            GUI.ClearUI();
            if (((ABTracker)DataContainer).PredictionDisplay)
                GUI.AddElement(
                    new GuiElementText(
                        "Prediction: " + ((ABTracker)DataContainer).EnemyDeckTypeGuess + "|" +
                        ((ABTracker)DataContainer).EnemyDeckStyleGuess
                        , (_screenWidth) / 64, PercToPixHeight(40), 155, 30, 16, 255, 215, 0));
        }
        public override void OnFriendRequestReceived(FriendRequest request)
        {

            Bot.Log(string.Format("[ABTracker] FRIEND REQUEST: {0} {1} {2}", request.GetId(), request.GetPlayerName(), request.ToString()));
        }
        public void CheckOpponentDeck()
        {
            List<Card.Cards> graveyard = Bot.CurrentBoard.EnemyGraveyard.ToList();
            List<Card> board = Bot.CurrentBoard.MinionEnemy.ToList();
            List<string> opponentDeck = new List<string> { };
            opponentDeck.AddRange(graveyard.Where(card => CardTemplate.LoadFromId(card).IsCollectible).Select(q => q.ToString()));
            opponentDeck.AddRange(board.Where(card => card.Template.IsCollectible).Select(q => q.Template.Id.ToString()));
            if (opponentDeck.Count == 0 || opponentDeck == null) return;
            DeckData opponentInfo = GetDeckInfo(Bot.CurrentBoard.EnemyClass, opponentDeck);
            if (((ABTracker)DataContainer).EnemyDeckTypeGuess == opponentInfo.DeckType)
            {
                Bot.Log("[ABTracker_debug] Your opponent is playing " + opponentInfo.DeckType + ":" + opponentInfo.DeckStyle);
                return;
            }
            Log(string.Format("[ABTracker_debug] New signature detected in your opponent decks {0} => {1}", ((ABTracker)DataContainer).EnemyDeckTypeGuess, opponentInfo.DeckType));
            ((ABTracker)DataContainer).EnemyDeckTypeGuess = opponentInfo.DeckType;
            ((ABTracker)DataContainer).EnemyDeckStyleGuess = DeckStyles[opponentInfo.DeckType];
        }

        public void CheckOpponentDeck(string res)
        {
            List<Card.Cards> graveyard = Bot.CurrentBoard.EnemyGraveyard.ToList();
            List<Card> board = Bot.CurrentBoard.MinionEnemy.ToList();
            List<string> opponentDeck = new List<string> { };
            opponentDeck.AddRange(graveyard.Where(card => CardTemplate.LoadFromId(card).IsCollectible).Select(q => q.ToString()));
            opponentDeck.AddRange(board.Where(card => card.Template.IsCollectible).Select(q => q.Template.Id.ToString()));
            if (opponentDeck.Count == 0) return;
            string str = opponentDeck.Aggregate("", (current, q) => current + "," + q);
            bool rgt = (bool) Bot.GetPlugins().Find(c => c.DataContainer.Name == plugins[Plugins.History]).GetProperties()["RGT"];
            using (StreamWriter opponentDeckInfo = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\MatchHistory.txt", true))
            {
                DeckData opponentInfo = GetDeckInfo(Bot.CurrentBoard.EnemyClass, opponentDeck, Bot.CurrentBoard.SecretEnemyCount);
                opponentDeckInfo.WriteLine("{0}||{1}||{2}||{3}||{4}||{5}||{6}||{7}||{8}||{9}||{10}||{11}",  rgt
                   ? DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) : "some time ago",
                    res,
                    Bot.GetCurrentOpponentId(),
                    Bot.CurrentMode(),
                    Bot.CurrentBoard.FriendClass, ((ABTracker)DataContainer).AutoFriendlyDeckType, ((ABTracker)DataContainer).AutoFriendlyStyle,
                    Bot.CurrentBoard.EnemyClass, opponentInfo.DeckType, opponentInfo.DeckStyle,
                    str, Bot.CurrentDeck().Cards.Aggregate("", (current, q) => current + "," + q)
                    );
                Bot.Log(string.Format("[Tracker] Succesfully recorded your opponent: {0}", opponentInfo.DeckType));

            }
        }
        /*
         * Played =  graveyard+board+secrets+weapons
         * Drawn = played + hand
         */
        public void RecordCards(string res)
        {
           
            List<Card.Cards> graveyard = Bot.CurrentBoard.FriendGraveyard.ToList();
            List<Card> board = Bot.CurrentBoard.MinionFriend.ToList();
            List<Card.Cards> secrets = Bot.CurrentBoard.Secret.ToList();
            Card weapon = Bot.CurrentBoard.WeaponFriend;
            Bot.Log("=============Finished Played cards");
            List<Card> hand = Bot.CurrentBoard.Hand.ToList();
            Bot.Log("=============Finished Drawn cards");

            List<string> played = new List<string> { };
            played.AddRange(graveyard.Where(card => CardTemplate.LoadFromId(card).IsCollectible).Select(q => q.ToString()));
            played.AddRange(board.Where(card => card.Template.IsCollectible).Select(q => q.Template.Id.ToString()));
            played.AddRange(secrets.Where(card => CardTemplate.LoadFromId(card).IsCollectible).Select(q => q.ToString()));
            if(Bot.CurrentBoard.HasWeapon())
                played.Add(weapon.Template.Id.ToString());
            Bot.Log("=============Created Played");

            List<string> drawn = new List<string>();
            drawn.AddRange(hand.Where(card => card.Template.IsCollectible).Select(q => q.Template.Id.ToString()));
            Bot.Log("=============Created Drawn");

            if (played.Count == 0) return;
            if (drawn.Count == 0) return;
            string splayed = played.Aggregate("", (current, q) => current + "," + q);
            string sdrawn = drawn.Aggregate("", (current, q) => current + "," + q);
            using (StreamWriter DeckPerformanceHistory = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\DeckPerformanceHistory.txt", true))
            {
                DeckPerformanceHistory.WriteLine("{0}~{1}~{2}~{3}~{4}~{5}", res,Bot.CurrentMode(), Bot.CurrentBoard.EnemyClass, Bot.CurrentBoard.FriendClass, sdrawn, splayed);


            }
        }
        public override void OnDefeat()
        {
            Bot.Log("========================================" + Bot.CurrentMode());

            //if (!_supported) return;


            try
            {
                CheckOpponentDeck("lost");
                RecordCards("lost");
            }
            catch (Exception e)
            {
                Bot.Log("Something happened that wasn't intended" + e.Message);
            }
            Bot.Log("[ST] Adden recent opponent to data set.");

        }

        public override void OnVictory()
        {
            Bot.Log("========================================" + Bot.CurrentMode());
            //if (!_supported) return;

            try
            {
                CheckOpponentDeck("won");
                RecordCards("won");
            }
            catch (Exception e)
            {
                Bot.Log("Something happened that wasn't intended" + e.Message);
            }

            Bot.Log("[ST] Adden recent opponent to data set.");
        }
        #region deckIdentifier
        public readonly Dictionary<DeckType, Style> DeckStyles = new Dictionary<DeckType, Style>
        {
            {DeckType.Custom, Style.Unknown},
            {DeckType.Unknown, Style.Unknown},
            {DeckType.Arena, Style.Control},
            /*Warrior*/
            {DeckType.ControlWarrior, Style.Control},
            {DeckType.FatigueWarrior, Style.Fatigue},
            {DeckType.DragonWarrior, Style.Control},
            {DeckType.PatronWarrior, Style.Tempo},
            {DeckType.WorgenOTKWarrior, Style.Combo},
            {DeckType.MechWarrior, Style.Aggro},
            {DeckType.FaceWarrior, Style.Face},
            {DeckType.RenoWarrior, Style.Control },
            {DeckType.CThunWarrior, Style.Control },
            {DeckType.TempoWarrior, Style.Tempo },

            /*Paladin*/
            {DeckType.SecretPaladin, Style.Tempo},
            {DeckType.MidRangePaladin, Style.Control},
            {DeckType.DragonPaladin, Style.Control},
            {DeckType.AggroPaladin, Style.Aggro},
            {DeckType.AnyfinMurglMurgl, Style.Combo},
            {DeckType.RenoPaladin, Style.Control},
            {DeckType.CThunPaladin, Style.Combo },
            /*Druid*/
            {DeckType.RampDruid, Style.Control},
            {DeckType.AggroDruid, Style.Aggro},
            {DeckType.DragonDruid, Style.Control},
            {DeckType.MidRangeDruid, Style.Tempo},
            {DeckType.TokenDruid, Style.Tempo},
            {DeckType.SilenceDruid, Style.Control},
            {DeckType.MechDruid, Style.Aggro},
            {DeckType.AstralDruid, Style.Control},
            {DeckType.MillDruid, Style.Fatigue},
            {DeckType.BeastDruid, Style.Tempo},
            {DeckType.RenoDruid, Style.Control},
            {DeckType.CThunDruid, Style.Combo },
            /*Warlock*/
            {DeckType.Handlock, Style.Control},
            {DeckType.RenoLock, Style.Control},
            {DeckType.Zoolock, Style.Aggro}, //Same handler as flood zoo and reliquary
            {DeckType.DemonHandlock, Style.Control},
            {DeckType.DragonHandlock, Style.Control},
            {DeckType.MalyLock, Style.Combo},
            {DeckType.ControlWarlock, Style.Control},
            {DeckType.CThunLock, Style.Combo },
            /*Mage*/
            {DeckType.TempoMage, Style.Tempo},
            {DeckType.FreezeMage, Style.Control},
            {DeckType.FaceFreezeMage, Style.Aggro},
            {DeckType.DragonMage, Style.Control},
            {DeckType.MechMage, Style.Aggro},
            {DeckType.EchoMage, Style.Control},
            {DeckType.CThunMage, Style.Combo},
            {DeckType.RenoMage, Style.Control},
            /*Priest*/
            {DeckType.DragonPriest, Style.Tempo},
            {DeckType.ControlPriest, Style.Control},
            {DeckType.ComboPriest, Style.Combo},
            {DeckType.MechPriest, Style.Aggro},
            /*Hunter*/
            {DeckType.MidRangeHunter, Style.Tempo},
            {DeckType.HybridHunter, Style.Aggro},
            {DeckType.FaceHunter, Style.Face},
            {DeckType.CamelHunter, Style.Control},
            {DeckType.DragonHunter, Style.Control},
            {DeckType.RenoHunter, Style.Control},
            {DeckType.CThunHunter, Style.Combo },
            /*Rogue*/
            {DeckType.OilRogue, Style.Combo},
            {DeckType.PirateRogue, Style.Aggro},
            {DeckType.FaceRogue, Style.Face},
            {DeckType.MalyRogue, Style.Combo},
            {DeckType.RaptorRogue, Style.Tempo},
            {DeckType.MiracleRogue, Style.Combo},
            {DeckType.RenoRogue, Style.Control},
            {DeckType.MechRogue, Style.Tempo},
            {DeckType.MillRogue, Style.Fatigue},
            {DeckType.CThunRogue, Style.Combo },
            /*Cance... I mean Shaman*/
            {DeckType.FaceShaman, Style.Face},
            {DeckType.MidrangeShaman, Style.Tempo},
            {DeckType.MechShaman, Style.Aggro},
            {DeckType.DragonShaman, Style.Control},
            {DeckType.MalygosShaman, Style.Combo},
            {DeckType.ControlShaman, Style.Control},
            {DeckType.RenoShaman, Style.Combo},
            {DeckType.BattleryShaman, Style.Control},
            {DeckType.CThunShaman, Style.Combo },

            /*Poor Kids*/
            {DeckType.Basic, Style.Tempo}
        };


        #region reference decks
        //Reference Decks
        private readonly List<Card.Cards> renoShaman = new List<Card.Cards> { Cards.LightningBolt, Cards.SylvanasWindrunner, Cards.EarthShock, Cards.FarSight, Cards.StormforgedAxe, Cards.Doomhammer, Cards.FeralSpirit, Cards.Hex, Cards.AzureDrake, Cards.AlAkirtheWindlord, Cards.RockbiterWeapon, Cards.RagnarostheFirelord, Cards.BloodmageThalnos, Cards.DefenderofArgus, Cards.ManaTideTotem, Cards.FireElemental, Cards.LightningStorm, Cards.Crackle, Cards.LavaShock, Cards.EmperorThaurissan, Cards.TotemGolem, Cards.TuskarrTotemic, Cards.AncestralKnowledge, Cards.HealingWave, Cards.JeweledScarab, Cards.SirFinleyMrrgglton, Cards.TombSpider, Cards.RenoJackson, Cards.TunnelTrogg, Cards.ArchThiefRafaam, };
        private readonly List<Card.Cards> FaceShaman = new List<Card.Cards> { Cards.LightningBolt, Cards.LightningBolt, Cards.UnboundElemental, Cards.UnboundElemental, Cards.EarthShock, Cards.StormforgedAxe, Cards.Doomhammer, Cards.Doomhammer, Cards.FeralSpirit, Cards.FeralSpirit, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.LeperGnome, Cards.LeperGnome, Cards.AbusiveSergeant, Cards.LavaBurst, Cards.LavaBurst, Cards.Crackle, Cards.Crackle, Cards.LavaShock, Cards.LavaShock, Cards.TotemGolem, Cards.TotemGolem, Cards.ArgentHorserider, Cards.ArgentHorserider, Cards.AncestralKnowledge, Cards.AncestralKnowledge, Cards.SirFinleyMrrgglton, Cards.TunnelTrogg, Cards.TunnelTrogg, };
        private readonly List<Card.Cards> MechShaman = new List<Card.Cards> { Cards.LightningBolt, Cards.LightningBolt, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.TunnelTrogg, Cards.TunnelTrogg, Cards.Crackle, Cards.Crackle, Cards.TotemGolem, Cards.TotemGolem, Cards.WhirlingZapomatic, Cards.WhirlingZapomatic, Cards.Powermace, Cards.Powermace, Cards.LavaBurst, Cards.LavaBurst, Cards.UnboundElemental, Cards.UnboundElemental, Cards.Doomhammer, Cards.Doomhammer, Cards.Cogmaster, Cards.Cogmaster, Cards.LeperGnome, Cards.SirFinleyMrrgglton, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.Mechwarper, Cards.Mechwarper, Cards.SpiderTank, Cards.SpiderTank, };
        private readonly List<Card.Cards> TotemShaman = new List<Card.Cards> { Cards.EarthShock, Cards.Bloodlust, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.AlAkirtheWindlord, Cards.FlametongueTotem, Cards.FlametongueTotem, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.DefenderofArgus, Cards.ManaTideTotem, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.LightningStorm, Cards.ZombieChow, Cards.ZombieChow, Cards.TotemGolem, Cards.TotemGolem, Cards.TuskarrTotemic, Cards.TuskarrTotemic, Cards.ThunderBluffValiant, Cards.ThunderBluffValiant, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.HauntedCreeper, Cards.HauntedCreeper, };
        private readonly List<Card.Cards> TotemShaman2 = new List<Card.Cards>{Cards.LightningBolt,Cards.Doomhammer,Cards.FeralSpirit,Cards.FeralSpirit,Cards.Bloodlust,Cards.Hex,Cards.Hex,Cards.FlametongueTotem,Cards.FlametongueTotem,Cards.ArgentSquire,Cards.ArgentSquire,Cards.RockbiterWeapon,Cards.RockbiterWeapon,Cards.ManaTideTotem,Cards.ManaTideTotem,Cards.LightningStorm,Cards.LightningStorm,Cards.TotemGolem,Cards.TotemGolem,Cards.TuskarrTotemic,Cards.TuskarrTotemic,Cards.ThunderBluffValiant,Cards.ThunderBluffValiant,Cards.FlameJuggler,Cards.FlameJuggler,Cards.TunnelTrogg,Cards.TunnelTrogg,Cards.ThingfromBelow,Cards.ThingfromBelow,Cards.PrimalFusion,};
        private readonly List<Card.Cards> TotemShaman3 = new List<Card.Cards>{Cards.LightningBolt,Cards.Doomhammer,Cards.FeralSpirit,Cards.Bloodlust,Cards.Hex,Cards.Hex,Cards.FlametongueTotem,Cards.FlametongueTotem,Cards.ArgentSquire,Cards.RockbiterWeapon,Cards.RockbiterWeapon,Cards.HarrisonJones,Cards.ManaTideTotem,Cards.ManaTideTotem,Cards.LightningStorm,Cards.LightningStorm,Cards.TotemGolem,Cards.TotemGolem,Cards.TuskarrTotemic,Cards.TuskarrTotemic,Cards.ThunderBluffValiant,Cards.ThunderBluffValiant,Cards.FlameJuggler,Cards.FlameJuggler,Cards.TunnelTrogg,Cards.TunnelTrogg,Cards.ThingfromBelow,Cards.ThingfromBelow,Cards.FlamewreathedFaceless,Cards.PrimalFusion,};
        private readonly List<Card.Cards> TotemShaman4 = new List<Card.Cards>{Cards.Doomhammer,Cards.FeralSpirit,Cards.FeralSpirit,Cards.Bloodlust,Cards.Hex,Cards.Hex,Cards.FlametongueTotem,Cards.FlametongueTotem,Cards.ArgentSquire,Cards.ArgentSquire,Cards.RockbiterWeapon,Cards.RockbiterWeapon,Cards.ManaTideTotem,Cards.ManaTideTotem,Cards.LightningStorm,Cards.TotemGolem,Cards.TotemGolem,Cards.TuskarrTotemic,Cards.TuskarrTotemic,Cards.ThunderBluffValiant,Cards.ThunderBluffValiant,Cards.FlameJuggler,Cards.FlameJuggler,Cards.TunnelTrogg,Cards.TunnelTrogg,Cards.ThingfromBelow,Cards.ThingfromBelow,Cards.MasterofEvolution,Cards.PrimalFusion,Cards.PrimalFusion,};
        private readonly List<Card.Cards> MalygosShaman = new List<Card.Cards> { Cards.LightningBolt, Cards.LightningBolt, Cards.EarthShock, Cards.EarthShock, Cards.FarSight, Cards.FarSight, Cards.StormforgedAxe, Cards.FeralSpirit, Cards.FeralSpirit, Cards.FrostShock, Cards.FrostShock, Cards.Malygos, Cards.GnomishInventor, Cards.GnomishInventor, Cards.Crackle, Cards.Crackle, Cards.Hex, Cards.Hex, Cards.LavaBurst, Cards.LavaBurst, Cards.ManaTideTotem, Cards.ManaTideTotem, Cards.LightningStorm, Cards.LightningStorm, Cards.AncestorsCall, Cards.AncestorsCall, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.Alexstrasza, Cards.AzureDrake, };
        private readonly List<Card.Cards> BasicChaman = new List<Card.Cards> { Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.FlametongueTotem, Cards.FlametongueTotem, Cards.Hex, Cards.Hex, Cards.Bloodlust, Cards.FireElemental, Cards.FireElemental, Cards.AcidicSwampOoze, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.MurlocTidehunter, Cards.RazorfenHunter, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.GnomishInventor, Cards.SenjinShieldmasta, Cards.SenjinShieldmasta, Cards.FrostwolfWarlord, Cards.FrostwolfWarlord, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion, Cards.StormwindChampion, };
        private readonly List<Card.Cards> DragonShaman = new List<Card.Cards> { Cards.EarthShock, Cards.FeralSpirit, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.Deathwing, Cards.Ysera, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.LightningStorm, Cards.BlackwingTechnician, Cards.BlackwingTechnician, Cards.LavaShock, Cards.BlackwingCorruptor, Cards.TotemGolem, Cards.TotemGolem, Cards.AncestralKnowledge, Cards.HealingWave, Cards.HealingWave, Cards.TheMistcaller, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.Chillmaw, Cards.JeweledScarab, Cards.JeweledScarab, Cards.BrannBronzebeard, Cards.TunnelTrogg, Cards.TunnelTrogg, };
        private readonly List<Card.Cards> ControlShaman = new List<Card.Cards> { Cards.BigGameHunter, Cards.FeralSpirit, Cards.FeralSpirit, Cards.Bloodlust, Cards.Bloodlust, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.FlametongueTotem, Cards.FlametongueTotem, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.DefenderofArgus, Cards.DefenderofArgus, Cards.AbusiveSergeant, Cards.ManaTideTotem, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.LightningStorm, Cards.ZombieChow, Cards.ZombieChow, Cards.NerubianEgg, Cards.NerubianEgg, Cards.Loatheb, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.BrannBronzebeard, };
        private readonly List<Card.Cards> ControlShaman2 = new List<Card.Cards> { Cards.BigGameHunter, Cards.EarthShock, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.Doomsayer, Cards.Doomsayer, Cards.Ysera, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.LightningStorm, Cards.Loatheb, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.DrBoom, Cards.Neptulon, Cards.LavaShock, Cards.LavaShock, Cards.VolcanicDrake, Cards.VolcanicDrake, Cards.HealingWave, Cards.HealingWave, Cards.ElementalDestruction, Cards.ElementalDestruction, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.JeweledScarab, Cards.JeweledScarab, };
        private readonly List<Card.Cards> BloodlustShaman = new List<Card.Cards> { Cards.BigGameHunter, Cards.AcidicSwampOoze, Cards.EarthShock, Cards.FeralSpirit, Cards.FeralSpirit, Cards.Bloodlust, Cards.Bloodlust, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.FlametongueTotem, Cards.FlametongueTotem, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.DefenderofArgus, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.LightningStorm, Cards.ZombieChow, Cards.ZombieChow, Cards.NerubianEgg, Cards.NerubianEgg, Cards.Loatheb, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.PilotedShredder, Cards.TuskarrTotemic, };
        private readonly List<Card.Cards> battlecryShaman = new List<Card.Cards> { Cards.LightningBolt, Cards.LightningBolt, Cards.StormforgedAxe, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.DefenderofArgus, Cards.DefenderofArgus, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.ZombieChow, Cards.ZombieChow, Cards.Loatheb, Cards.DrBoom, Cards.TotemGolem, Cards.TotemGolem, Cards.TuskarrTotemic, Cards.TuskarrTotemic, Cards.JusticarTrueheart, Cards.JeweledScarab, Cards.JeweledScarab, Cards.BrannBronzebeard, Cards.RumblingElemental, Cards.RumblingElemental, Cards.TunnelTrogg, Cards.TunnelTrogg, };
        private readonly List<Card.Cards> ControlShaman3 = new List<Card.Cards>{Cards.LightningBolt,Cards.LightningBolt,Cards.SylvanasWindrunner,Cards.Hex,Cards.Hex,Cards.RockbiterWeapon,Cards.Ysera,Cards.CairneBloodhoof,Cards.BloodmageThalnos,Cards.ManaTideTotem,Cards.ManaTideTotem,Cards.LightningStorm,Cards.LightningStorm,Cards.LavaShock,Cards.LavaShock,Cards.EmperorThaurissan,Cards.AncestralKnowledge,Cards.HealingWave,Cards.HealingWave,Cards.ElementalDestruction,Cards.ElementalDestruction,Cards.Chillmaw,Cards.EliseStarseeker,Cards.HallazealtheAscended,Cards.NZoththeCorruptor,Cards.ThingfromBelow,Cards.ThingfromBelow,Cards.YoggSaronHopesEnd,Cards.Stormcrack,Cards.Stormcrack,};

        private readonly List<Card.Cards> ContrlPriest = new List<Card.Cards> { Cards.WildPyromancer, Cards.WildPyromancer, Cards.CircleofHealing, Cards.CircleofHealing, Cards.Thoughtsteal, Cards.CabalShadowPriest, Cards.CabalShadowPriest, Cards.InjuredBlademaster, Cards.InjuredBlademaster, Cards.PowerWordShield, Cards.PowerWordShield, Cards.ShadowWordDeath, Cards.NorthshireCleric, Cards.NorthshireCleric, Cards.AuchenaiSoulpriest, Cards.AuchenaiSoulpriest, Cards.HolyNova, Cards.ZombieChow, Cards.ZombieChow, Cards.Deathlord, Cards.Deathlord, Cards.LightoftheNaaru, Cards.LightoftheNaaru, Cards.Lightbomb, Cards.Lightbomb, Cards.JusticarTrueheart, Cards.EliseStarseeker, Cards.Entomb, Cards.Entomb, Cards.MuseumCurator, };
        private readonly List<Card.Cards> ComboPriest = new List<Card.Cards> { Cards.WildPyromancer, Cards.WildPyromancer, Cards.ProphetVelen, Cards.Malygos, Cards.AzureDrake, Cards.ShadowWordPain, Cards.LootHoarder, Cards.LootHoarder, Cards.HolySmite, Cards.HolySmite, Cards.MindBlast, Cards.MindBlast, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.PowerWordShield, Cards.PowerWordShield, Cards.HolyFire, Cards.HolyFire, Cards.BloodmageThalnos, Cards.ShadowWordDeath, Cards.NorthshireCleric, Cards.NorthshireCleric, Cards.HarrisonJones, Cards.HolyNova, Cards.HolyNova, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.VelensChosen, Cards.VelensChosen, Cards.EmperorThaurissan, };
        private readonly List<Card.Cards> MechPriest = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.CabalShadowPriest, Cards.CabalShadowPriest, Cards.ShadowWordPain, Cards.PowerWordShield, Cards.PowerWordShield, Cards.ShadowMadness, Cards.CairneBloodhoof, Cards.NorthshireCleric, Cards.NorthshireCleric, Cards.HolyNova, Cards.Shrinkmeister, Cards.Shrinkmeister, Cards.VelensChosen, Cards.VelensChosen, Cards.DarkCultist, Cards.DarkCultist, Cards.UpgradedRepairBot, Cards.UpgradedRepairBot, Cards.Voljin, Cards.Mechwarper, Cards.Mechwarper, Cards.SpiderTank, Cards.SpiderTank, Cards.MechanicalYeti, Cards.MechanicalYeti, Cards.PilotedShredder, Cards.PilotedShredder, Cards.Loatheb, Cards.TroggzortheEarthinator, };
        private readonly List<Card.Cards> ShadowPriest = new List<Card.Cards> { Cards.WildPyromancer, Cards.WildPyromancer, Cards.Thoughtsteal, Cards.CabalShadowPriest, Cards.CabalShadowPriest, Cards.ProphetVelen, Cards.Alexstrasza, Cards.SenjinShieldmasta, Cards.SenjinShieldmasta, Cards.HolySmite, Cards.MindBlast, Cards.MindBlast, Cards.Shadowform, Cards.Shadowform, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.PowerWordShield, Cards.PowerWordShield, Cards.ShadowMadness, Cards.HolyFire, Cards.ShadowWordDeath, Cards.HolyNova, Cards.ZombieChow, Cards.Deathlord, Cards.Deathlord, Cards.Voljin, Cards.Lightbomb, Cards.Lightbomb, Cards.EmperorThaurissan, Cards.Entomb, };
        private readonly List<Card.Cards> BasicPriest = new List<Card.Cards> { Cards.HolySmite, Cards.HolySmite, Cards.PowerWordShield, Cards.PowerWordShield, Cards.NorthshireCleric, Cards.NorthshireCleric, Cards.DivineSpirit, Cards.ShadowWordPain, Cards.ShadowWordPain, Cards.ShadowWordDeath, Cards.HolyNova, Cards.HolyNova, Cards.MindControl, Cards.VoodooDoctor, Cards.AcidicSwampOoze, Cards.RiverCrocolisk, Cards.RiverCrocolisk, Cards.IronfurGrizzly, Cards.IronfurGrizzly, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.DarkscaleHealer, Cards.DarkscaleHealer, Cards.GurubashiBerserker, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion, };

        private readonly List<Card.Cards> renoMage = new List<Card.Cards> { Cards.IceBlock, Cards.SylvanasWindrunner, Cards.Flamestrike, Cards.FrostNova, Cards.BigGameHunter, Cards.MoltenGiant, Cards.Frostbolt, Cards.YouthfulBrewmaster, Cards.Blizzard, Cards.SunfuryProtector, Cards.AcolyteofPain, Cards.Doomsayer, Cards.ArcaneIntellect, Cards.IronbeakOwl, Cards.Fireball, Cards.EarthenRingFarseer, Cards.Polymorph, Cards.ZombieChow, Cards.Duplicate, Cards.Loatheb, Cards.SludgeBelcher, Cards.Deathlord, Cards.ExplosiveSheep, Cards.DrBoom, Cards.AntiqueHealbot, Cards.EchoofMedivh, Cards.EmperorThaurissan, Cards.ForgottenTorch, Cards.RenoJackson, Cards.EtherealConjurer, };
        private readonly List<Card.Cards> tempoMage = new List<Card.Cards> { Cards.SorcerersApprentice, Cards.SorcerersApprentice, Cards.MirrorImage, Cards.MirrorImage, Cards.Frostbolt, Cards.Frostbolt, Cards.ArchmageAntonidas, Cards.ManaWyrm, Cards.ManaWyrm, Cards.WaterElemental, Cards.WaterElemental, Cards.MindControlTech, Cards.ArcaneIntellect, Cards.ArcaneIntellect, Cards.Fireball, Cards.Fireball, Cards.Counterspell, Cards.MirrorEntity, Cards.ArcaneMissiles, Cards.ArcaneMissiles, Cards.MadScientist, Cards.MadScientist, Cards.UnstablePortal, Cards.UnstablePortal, Cards.Flamecannon, Cards.Flamecannon, Cards.Flamewaker, Cards.Flamewaker, Cards.EtherealConjurer, Cards.EtherealConjurer, };
        private readonly List<Card.Cards> basicMage = new List<Card.Cards> { Cards.Frostbolt, Cards.Frostbolt, Cards.ArcaneIntellect, Cards.ArcaneIntellect, Cards.Fireball, Cards.Fireball, Cards.Polymorph, Cards.Polymorph, Cards.WaterElemental, Cards.WaterElemental, Cards.Flamestrike, Cards.Flamestrike, Cards.AcidicSwampOoze, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.KoboldGeomancer, Cards.RazorfenHunter, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.SenjinShieldmasta, Cards.GurubashiBerserker, Cards.Archmage, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion, };
        private readonly List<Card.Cards> freeze = new List<Card.Cards> { Cards.IceBlock, Cards.IceBlock, Cards.Flamestrike, Cards.FrostNova, Cards.FrostNova, Cards.Frostbolt, Cards.Frostbolt, Cards.IceLance, Cards.IceLance, Cards.ArchmageAntonidas, Cards.Blizzard, Cards.Blizzard, Cards.Alexstrasza, Cards.LootHoarder, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.Doomsayer, Cards.Doomsayer, Cards.ArcaneIntellect, Cards.ArcaneIntellect, Cards.Pyroblast, Cards.Fireball, Cards.Fireball, Cards.BloodmageThalnos, Cards.IceBarrier, Cards.IceBarrier, Cards.MadScientist, Cards.MadScientist, Cards.AntiqueHealbot, Cards.EmperorThaurissan, };
        private readonly List<Card.Cards> faceFreeze = new List<Card.Cards> { Cards.SorcerersApprentice, Cards.SorcerersApprentice, Cards.IceBlock, Cards.IceBlock, Cards.MirrorImage, Cards.FrostNova, Cards.FrostNova, Cards.Frostbolt, Cards.Frostbolt, Cards.IceLance, Cards.IceLance, Cards.ManaWyrm, Cards.ManaWyrm, Cards.AzureDrake, Cards.AzureDrake, Cards.LootHoarder, Cards.LootHoarder, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.ArcaneIntellect, Cards.ArcaneIntellect, Cards.Fireball, Cards.Fireball, Cards.BloodmageThalnos, Cards.MadScientist, Cards.MadScientist, Cards.PilotedShredder, Cards.PilotedShredder, Cards.ForgottenTorch, Cards.ForgottenTorch, };
        private readonly List<Card.Cards> dragonMage = new List<Card.Cards> { Cards.Frostbolt, Cards.Frostbolt, Cards.Duplicate, Cards.IceBarrier, Cards.IceBlock, Cards.Polymorph, Cards.Polymorph, Cards.Flamestrike, Cards.Flamestrike, Cards.ZombieChow, Cards.MadScientist, Cards.MadScientist, Cards.BigGameHunter, Cards.BlackwingTechnician, Cards.BlackwingTechnician, Cards.TwilightDrake, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.AzureDrake, Cards.AzureDrake, Cards.BlackwingCorruptor, Cards.BlackwingCorruptor, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.EmperorThaurissan, Cards.DrBoom, Cards.Alexstrasza, Cards.Ysera, };
        private readonly List<Card.Cards> mechMage = new List<Card.Cards> { Cards.Frostbolt, Cards.Frostbolt, Cards.ArchmageAntonidas, Cards.ManaWyrm, Cards.ManaWyrm, Cards.Fireball, Cards.Fireball, Cards.UnstablePortal, Cards.UnstablePortal, Cards.Cogmaster, Cards.Cogmaster, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.DrBoom, Cards.SpiderTank, Cards.SpiderTank, Cards.Mechwarper, Cards.Mechwarper, Cards.PilotedShredder, Cards.PilotedShredder, Cards.GoblinBlastmage, Cards.GoblinBlastmage, Cards.ClockworkGnome, Cards.TinkertownTechnician, Cards.Snowchugger, Cards.Snowchugger, Cards.ClockworkKnight, Cards.ClockworkKnight, Cards.GorillabotA3, Cards.GorillabotA3, };
        private readonly List<Card.Cards> grinderMage = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.Flamestrike, Cards.Flamestrike, Cards.BigGameHunter, Cards.Frostbolt, Cards.Frostbolt, Cards.MindControlTech, Cards.MirrorEntity, Cards.Polymorph, Cards.ZombieChow, Cards.ZombieChow, Cards.Duplicate, Cards.MadScientist, Cards.MadScientist, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.ExplosiveSheep, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.EchoofMedivh, Cards.EmperorThaurissan, Cards.RefreshmentVendor, Cards.JeweledScarab, Cards.JeweledScarab, Cards.BrannBronzebeard, Cards.EtherealConjurer, Cards.EtherealConjurer, };

        private readonly List<Card.Cards> renoPaladin = new List<Card.Cards> { Cards.AldorPeacekeeper, Cards.BlessingofKings, Cards.SylvanasWindrunner, Cards.BigGameHunter, Cards.Humility, Cards.Consecration, Cards.TruesilverChampion, Cards.MindControlTech, Cards.Equality, Cards.StampedingKodo, Cards.TirionFordring, Cards.KnifeJuggler, Cards.IronbeakOwl, Cards.RagnarostheFirelord, Cards.LayonHands, Cards.HarrisonJones, Cards.ZombieChow, Cards.SludgeBelcher, Cards.DrBoom, Cards.PilotedShredder, Cards.MusterforBattle, Cards.AntiqueHealbot, Cards.Coghammer, Cards.ShieldedMinibot, Cards.Quartermaster, Cards.EadricthePure, Cards.TuskarrJouster, Cards.MurlocKnight, Cards.RenoJackson, Cards.KeeperofUldaman, };
        private readonly List<Card.Cards> renoPaladin2 = new List<Card.Cards> { Cards.AldorPeacekeeper, Cards.BlessingofKings, Cards.SylvanasWindrunner, Cards.BigGameHunter, Cards.BlessingofWisdom, Cards.Humility, Cards.ArgentProtector, Cards.Consecration, Cards.GuardianofKings, Cards.TruesilverChampion, Cards.MindControlTech, Cards.Equality, Cards.StampedingKodo, Cards.TirionFordring, Cards.IronbeakOwl, Cards.LayonHands, Cards.DefenderofArgus, Cards.DrBoom, Cards.BombLobber, Cards.MusterforBattle, Cards.Coghammer, Cards.ShieldedMinibot, Cards.Quartermaster, Cards.JusticarTrueheart, Cards.GadgetzanJouster, Cards.TuskarrJouster, Cards.MurlocKnight, Cards.JeweledScarab, Cards.BrannBronzebeard, Cards.RenoJackson, };
        private readonly List<Card.Cards> anyfinPaladin = new List<Card.Cards> { Cards.AldorPeacekeeper, Cards.AldorPeacekeeper, Cards.CultMaster, Cards.OldMurkEye, Cards.MurlocWarleader, Cards.MurlocWarleader, Cards.Consecration, Cards.Consecration, Cards.BluegillWarrior, Cards.BluegillWarrior, Cards.TruesilverChampion, Cards.KnifeJuggler, Cards.LayonHands, Cards.GrimscaleOracle, Cards.GrimscaleOracle, Cards.ZombieChow, Cards.SludgeBelcher, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.MusterforBattle, Cards.MusterforBattle, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.Coghammer, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.SolemnVigil, Cards.AnyfinCanHappen, Cards.KeeperofUldaman, };
        private readonly List<Card.Cards> basicPaladin = new List<Card.Cards> { Cards.TruesilverChampion, Cards.TruesilverChampion, Cards.BlessingofKings, Cards.BlessingofKings, Cards.Consecration, Cards.Consecration, Cards.HammerofWrath, Cards.HammerofWrath, Cards.GuardianofKings, Cards.GuardianofKings, Cards.AcidicSwampOoze, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.MurlocTidehunter, Cards.MurlocTidehunter, Cards.RiverCrocolisk, Cards.RiverCrocolisk, Cards.RazorfenHunter, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.FrostwolfWarlord, Cards.FrostwolfWarlord, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion, Cards.StormwindChampion, };
        private readonly List<Card.Cards> secretPaladin = new List<Card.Cards> { Cards.BlessingofKings, Cards.NobleSacrifice, Cards.NobleSacrifice, Cards.TruesilverChampion, Cards.TirionFordring, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.Secretkeeper, Cards.Secretkeeper, Cards.DivineFavor, Cards.Repentance, Cards.Redemption, Cards.Avenge, Cards.Avenge, Cards.Loatheb, Cards.SludgeBelcher, Cards.HauntedCreeper, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.MusterforBattle, Cards.MusterforBattle, Cards.Coghammer, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.CompetitiveSpirit, Cards.MysteriousChallenger, Cards.MysteriousChallenger, Cards.KeeperofUldaman, Cards.KeeperofUldaman, };
        private readonly List<Card.Cards> midrangePaladin = new List<Card.Cards> { Cards.AldorPeacekeeper, Cards.AldorPeacekeeper, Cards.BigGameHunter, Cards.Consecration, Cards.Consecration, Cards.TruesilverChampion, Cards.TruesilverChampion, Cards.Equality, Cards.TirionFordring, Cards.KnifeJuggler, Cards.LayonHands, Cards.DefenderofArgus, Cards.ZombieChow, Cards.ZombieChow, Cards.Loatheb, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.MusterforBattle, Cards.MusterforBattle, Cards.Coghammer, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.Quartermaster, Cards.Quartermaster, Cards.JusticarTrueheart, Cards.MurlocKnight, Cards.KeeperofUldaman, };
        private readonly List<Card.Cards> dragonPaladin = new List<Card.Cards> { Cards.AldorPeacekeeper, Cards.AldorPeacekeeper, Cards.BigGameHunter, Cards.Consecration, Cards.Consecration, Cards.TruesilverChampion, Cards.TruesilverChampion, Cards.Alexstrasza, Cards.Equality, Cards.Ysera, Cards.IronbeakOwl, Cards.ZombieChow, Cards.ZombieChow, Cards.SludgeBelcher, Cards.MusterforBattle, Cards.MusterforBattle, Cards.AntiqueHealbot, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.HungryDragon, Cards.HungryDragon, Cards.BlackwingTechnician, Cards.BlackwingTechnician, Cards.BlackwingCorruptor, Cards.VolcanicDrake, Cards.Chromaggus, Cards.DragonConsort, Cards.DragonConsort, Cards.SolemnVigil, Cards.EmperorThaurissan, };
        private readonly List<Card.Cards> aggroPaladin = new List<Card.Cards> { Cards.BlessingofMight, Cards.BlessingofMight, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.Coghammer, Cards.DivineFavor, Cards.DivineFavor, Cards.MusterforBattle, Cards.MusterforBattle, Cards.TruesilverChampion, Cards.BlessingofKings, Cards.BlessingofKings, Cards.KeeperofUldaman, Cards.KeeperofUldaman, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.ArgentSquire, Cards.ArgentSquire, Cards.LeperGnome, Cards.LeperGnome, Cards.SirFinleyMrrgglton, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.IronbeakOwl, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.ArcaneGolem, Cards.PilotedShredder, Cards.LeeroyJenkins, Cards.Loatheb, };
        private readonly List<Card.Cards> nZothPaladin = new List<Card.Cards> { Cards.ForbiddenHealing, Cards.ForbiddenHealing, Cards.Humility, Cards.Equality, Cards.Equality, Cards.AldorPeacekeeper, Cards.AldorPeacekeeper, Cards.TruesilverChampion, Cards.TruesilverChampion, Cards.Consecration, Cards.Consecration, Cards.KeeperofUldaman, Cards.SolemnVigil, Cards.SolemnVigil, Cards.LayonHands, Cards.RagnarosLightlord, Cards.TirionFordring, Cards.AcidicSwampOoze, Cards.Doomsayer, Cards.Doomsayer, Cards.WildPyromancer, Cards.WildPyromancer, Cards.InfestedTauren, Cards.TwilightSummoner, Cards.CorruptedHealbot, Cards.CorruptedHealbot, Cards.StampedingKodo, Cards.CairneBloodhoof, Cards.SylvanasWindrunner, Cards.NZoththeCorruptor, };

        private readonly List<Card.Cards> controlWarriorCards = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.ShieldSlam, Cards.ShieldSlam, Cards.BigGameHunter, Cards.Slam, Cards.Slam, Cards.Execute, Cards.Execute, Cards.Brawl, Cards.Brawl, Cards.Deathwing, Cards.ShieldBlock, Cards.ShieldBlock, Cards.BaronGeddon, Cards.HarrisonJones, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.Armorsmith, Cards.DeathsBite, Cards.DeathsBite, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.Shieldmaiden, Cards.Shieldmaiden, Cards.Revenge, Cards.Revenge, Cards.JusticarTrueheart, Cards.Bash, Cards.JeweledScarab, Cards.JeweledScarab, };
        private readonly List<Card.Cards> renoWarrior = new List<Card.Cards> { Cards.ShieldSlam, Cards.BigGameHunter, Cards.Gorehowl, Cards.Slam, Cards.Execute, Cards.Brawl, Cards.CruelTaskmaster, Cards.Deathwing, Cards.Ysera, Cards.BaronGeddon, Cards.HarrisonJones, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.Armorsmith, Cards.Revenge, Cards.Bash, Cards.BouncingBlade, Cards.DeathsBite, Cards.Shieldmaiden, Cards.Crush, Cards.PilotedShredder, Cards.Loatheb, Cards.SludgeBelcher, Cards.EmperorThaurissan, Cards.JusticarTrueheart, Cards.RenoJackson, Cards.DrBoom, Cards.BrannBronzebeard, Cards.Deathlord, Cards.IronJuggernaut, };
        private readonly List<Card.Cards> renoWarrior2 = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.ShieldSlam, Cards.BigGameHunter, Cards.Gorehowl, Cards.Slam, Cards.Execute, Cards.Brawl, Cards.SenjinShieldmasta, Cards.CruelTaskmaster, Cards.AcolyteofPain, Cards.ShieldBlock, Cards.Ysera, Cards.IronbeakOwl, Cards.BaronGeddon, Cards.HarrisonJones, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.Armorsmith, Cards.ZombieChow, Cards.DeathsBite, Cards.SludgeBelcher, Cards.DrBoom, Cards.Shieldmaiden, Cards.Revenge, Cards.JusticarTrueheart, Cards.Bash, Cards.EliseStarseeker, Cards.JeweledScarab, Cards.RenoJackson, Cards.FierceMonkey, };
        private readonly List<Card.Cards> tauntWarrior = new List<Card.Cards> { Cards.BigGameHunter, Cards.Slam, Cards.Slam, Cards.Execute, Cards.Execute, Cards.Brawl, Cards.TheBlackKnight, Cards.AcolyteofPain, Cards.DefenderofArgus, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.Armorsmith, Cards.Armorsmith, Cards.DeathsBite, Cards.DeathsBite, Cards.KelThuzad, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.ArcaneNullifierX21, Cards.ArcaneNullifierX21, Cards.Kodorider, Cards.Bolster, Cards.VarianWrynn, Cards.SparringPartner, Cards.SparringPartner, Cards.EliseStarseeker, Cards.ObsidianDestroyer, Cards.ObsidianDestroyer, Cards.FierceMonkey, Cards.FierceMonkey, };
        private readonly List<Card.Cards> fatigueWarrior = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.ShieldSlam, Cards.ShieldSlam, Cards.BigGameHunter, Cards.Gorehowl, Cards.Slam, Cards.Slam, Cards.Execute, Cards.Execute, Cards.Brawl, Cards.Brawl, Cards.BaronGeddon, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.Revenge, Cards.Bash, Cards.Bash, Cards.BouncingBlade, Cards.DeathsBite, Cards.DeathsBite, Cards.Shieldmaiden, Cards.Shieldmaiden, Cards.Deathlord, Cards.Deathlord, Cards.JusticarTrueheart, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.UnstableGhoul, Cards.RenoJackson, };
        private readonly List<Card.Cards> faceWar = new List<Card.Cards> { Cards.HeroicStrike, Cards.HeroicStrike, Cards.ArcaneGolem, Cards.ArcaneGolem, Cards.SouthseaDeckhand, Cards.SouthseaDeckhand, Cards.KorkronElite, Cards.KorkronElite, Cards.Wolfrider, Cards.DreadCorsair, Cards.DreadCorsair, Cards.MortalStrike, Cards.MortalStrike, Cards.IronbeakOwl, Cards.LeperGnome, Cards.LeperGnome, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.BloodsailRaider, Cards.BloodsailRaider, Cards.Upgrade, Cards.Upgrade, Cards.DeathsBite, Cards.DeathsBite, Cards.ArgentHorserider, Cards.ArgentHorserider, Cards.Bash, Cards.Bash, Cards.SirFinleyMrrgglton, Cards.CursedBlade, };
        private readonly List<Card.Cards> dragonWarrior = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.ShieldSlam, Cards.ShieldSlam, Cards.BigGameHunter, Cards.Execute, Cards.Execute, Cards.Brawl, Cards.Alexstrasza, Cards.Deathwing, Cards.Ysera, Cards.BaronGeddon, Cards.HarrisonJones, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.DeathsBite, Cards.DeathsBite, Cards.Shieldmaiden, Cards.EmperorThaurissan, Cards.Nefarian, Cards.Revenge, Cards.Revenge, Cards.BlackwingCorruptor, Cards.BlackwingCorruptor, Cards.JusticarTrueheart, Cards.Chillmaw, Cards.Bash, Cards.Bash, Cards.TwilightGuardian, Cards.TwilightGuardian, };
        private readonly List<Card.Cards> patronWarrior = new List<Card.Cards> { Cards.FrothingBerserker, Cards.KorkronElite, Cards.Whirlwind, Cards.Whirlwind, Cards.Slam, Cards.Slam, Cards.Execute, Cards.Execute, Cards.DreadCorsair, Cards.DreadCorsair, Cards.InnerRage, Cards.InnerRage, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.Armorsmith, Cards.Armorsmith, Cards.BattleRage, Cards.BattleRage, Cards.DeathsBite, Cards.DeathsBite, Cards.Loatheb, Cards.SludgeBelcher, Cards.UnstableGhoul, Cards.DrBoom, Cards.GrimPatron, Cards.GrimPatron, Cards.ArchThiefRafaam, };
        private readonly List<Card.Cards> worgen = new List<Card.Cards> { Cards.ShieldSlam, Cards.RagingWorgen, Cards.RagingWorgen, Cards.Whirlwind, Cards.Execute, Cards.Execute, Cards.GnomishInventor, Cards.GnomishInventor, Cards.Brawl, Cards.Brawl, Cards.CruelTaskmaster, Cards.CruelTaskmaster, Cards.InnerRage, Cards.InnerRage, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.NoviceEngineer, Cards.NoviceEngineer, Cards.Rampage, Cards.Rampage, Cards.ShieldBlock, Cards.ShieldBlock, Cards.IronbeakOwl, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.Charge, Cards.Charge, Cards.DeathsBite, Cards.DeathsBite, Cards.AntiqueHealbot, };
        private readonly List<Card.Cards> mechWar = new List<Card.Cards> { Cards.HeroicStrike, Cards.HeroicStrike, Cards.KorkronElite, Cards.KorkronElite, Cards.ArcaniteReaper, Cards.ArcaniteReaper, Cards.MortalStrike, Cards.MortalStrike, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.DeathsBite, Cards.DeathsBite, Cards.Cogmaster, Cards.Cogmaster, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.SpiderTank, Cards.SpiderTank, Cards.Mechwarper, Cards.Mechwarper, Cards.PilotedShredder, Cards.PilotedShredder, Cards.TinkertownTechnician, Cards.ScrewjankClunker, Cards.ScrewjankClunker, Cards.Warbot, Cards.Warbot, Cards.FelReaver, Cards.FelReaver, Cards.ClockworkKnight, };
        private readonly List<Card.Cards> basicWarrior = new List<Card.Cards> { Cards.Execute, Cards.Execute, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.Cleave, Cards.ShieldBlock, Cards.KorkronElite, Cards.KorkronElite, Cards.ArcaniteReaper, Cards.ArcaniteReaper, Cards.ElvenArcher, Cards.AcidicSwampOoze, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.RiverCrocolisk, Cards.RazorfenHunter, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.GnomishInventor, Cards.SenjinShieldmasta, Cards.SenjinShieldmasta, Cards.StormpikeCommando, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion, };
        private readonly List<Card.Cards> tempoWarrior = new List<Card.Cards>{Cards.FrothingBerserker,Cards.FrothingBerserker,Cards.KorkronElite,Cards.KorkronElite,Cards.Execute,Cards.Execute,Cards.AzureDrake,Cards.AzureDrake,Cards.CruelTaskmaster,Cards.CruelTaskmaster,Cards.AcolyteofPain,Cards.ArgentCommander,Cards.ArgentCommander,Cards.RagnarostheFirelord,Cards.ArathiWeaponsmith,Cards.HarrisonJones,Cards.FieryWarAxe,Cards.FieryWarAxe,Cards.GrommashHellscream,Cards.Armorsmith,Cards.Armorsmith,Cards.VarianWrynn,Cards.FierceMonkey,Cards.RavagingGhoul,Cards.RavagingGhoul,Cards.BloodhoofBrave,Cards.BloodhoofBrave,Cards.Malkorok,Cards.BloodToIchor,Cards.BloodToIchor,};

        private readonly List<Card.Cards> controlWarlock = new List<Card.Cards> { Cards.MortalCoil, Cards.MortalCoil, Cards.BigGameHunter, Cards.Hellfire, Cards.Hellfire, Cards.Demonfire, Cards.LordJaraxxus, Cards.Ysera, Cards.IronbeakOwl, Cards.DefenderofArgus, Cards.DefenderofArgus, Cards.EarthenRingFarseer, Cards.SiphonSoul, Cards.BaneofDoom, Cards.Loatheb, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.PilotedShredder, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.Demonheart, Cards.MistressofPain, Cards.Darkbomb, Cards.Darkbomb, Cards.Implosion, Cards.ImpGangBoss, Cards.ImpGangBoss, Cards.EmperorThaurissan, Cards.EliseStarseeker, Cards.JeweledScarab, };
        private readonly List<Card.Cards> handlock = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.MortalCoil, Cards.MortalCoil, Cards.BigGameHunter, Cards.MoltenGiant, Cards.MoltenGiant, Cards.Hellfire, Cards.AncientWatcher, Cards.AncientWatcher, Cards.MountainGiant, Cards.MountainGiant, Cards.TwilightDrake, Cards.TwilightDrake, Cards.SunfuryProtector, Cards.SunfuryProtector, Cards.LordJaraxxus, Cards.IronbeakOwl, Cards.IronbeakOwl, Cards.DefenderofArgus, Cards.EarthenRingFarseer, Cards.Shadowflame, Cards.ZombieChow, Cards.ZombieChow, Cards.SludgeBelcher, Cards.DrBoom, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.Darkbomb, Cards.Darkbomb, Cards.BrannBronzebeard, };
        private readonly List<Card.Cards> renoLock = new List<Card.Cards> { Cards.MortalCoil, Cards.BigGameHunter, Cards.AcidicSwampOoze, Cards.MoltenGiant, Cards.Hellfire, Cards.AncientWatcher, Cards.MountainGiant, Cards.TwilightDrake, Cards.MindControlTech, Cards.SunfuryProtector, Cards.LordJaraxxus, Cards.IronbeakOwl, Cards.RagnarostheFirelord, Cards.DefenderofArgus, Cards.SiphonSoul, Cards.AbusiveSergeant, Cards.Shadowflame, Cards.Voidcaller, Cards.SludgeBelcher, Cards.DrBoom, Cards.PilotedShredder, Cards.AntiqueHealbot, Cards.MalGanis, Cards.Darkbomb, Cards.Implosion, Cards.ImpGangBoss, Cards.EmperorThaurissan, Cards.Demonwrath, Cards.RefreshmentVendor, Cards.RenoJackson, };
        private readonly List<Card.Cards> RenoCombo = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.MortalCoil, Cards.BigGameHunter, Cards.AcidicSwampOoze, Cards.Hellfire, Cards.PowerOverwhelming, Cards.TwilightDrake, Cards.TwistingNether, Cards.FacelessManipulator, Cards.LordJaraxxus, Cards.IronbeakOwl, Cards.DefenderofArgus, Cards.EarthenRingFarseer, Cards.SiphonSoul, Cards.AbusiveSergeant, Cards.Shadowflame, Cards.LeeroyJenkins, Cards.ZombieChow, Cards.Loatheb, Cards.SludgeBelcher, Cards.DrBoom, Cards.AntiqueHealbot, Cards.Darkbomb, Cards.ImpGangBoss, Cards.EmperorThaurissan, Cards.Demonwrath, Cards.RefreshmentVendor, Cards.BrannBronzebeard, Cards.RenoJackson, Cards.DarkPeddler, };
        private readonly List<Card.Cards> zoolock = new List<Card.Cards> { Cards.BigGameHunter, Cards.AcidicSwampOoze, Cards.FlameImp, Cards.FlameImp, Cards.DarkIronDwarf, Cards.PowerOverwhelming, Cards.PowerOverwhelming, Cards.DireWolfAlpha, Cards.Voidwalker, Cards.Voidwalker, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.IronbeakOwl, Cards.Doomguard, Cards.Doomguard, Cards.DarkPeddler, Cards.DarkPeddler, Cards.ImpGangBoss, Cards.ImpGangBoss, Cards.Implosion, Cards.Implosion, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.NerubianEgg, Cards.NerubianEgg, Cards.BrannBronzebeard, Cards.DefenderofArgus, Cards.DefenderofArgus, };
        private readonly List<Card.Cards> demonzoolock = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.BigGameHunter, Cards.FlameImp, Cards.VoidTerror, Cards.PowerOverwhelming, Cards.PowerOverwhelming, Cards.DireWolfAlpha, Cards.Voidwalker, Cards.Voidwalker, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.IronbeakOwl, Cards.Doomguard, Cards.Doomguard, Cards.DefenderofArgus, Cards.DefenderofArgus, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.Voidcaller, Cards.Voidcaller, Cards.NerubianEgg, Cards.NerubianEgg, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.MalGanis, Cards.Implosion, Cards.Implosion, Cards.ImpGangBoss, Cards.ImpGangBoss, };
        private readonly List<Card.Cards> demonHandLock = new List<Card.Cards> { Cards.DreadInfernal, Cards.MortalCoil, Cards.MortalCoil, Cards.MoltenGiant, Cards.MoltenGiant, Cards.AncientWatcher, Cards.AncientWatcher, Cards.MountainGiant, Cards.MountainGiant, Cards.TwilightDrake, Cards.TwilightDrake, Cards.SunfuryProtector, Cards.SunfuryProtector, Cards.LordJaraxxus, Cards.IronbeakOwl, Cards.DefenderofArgus, Cards.SiphonSoul, Cards.Shadowflame, Cards.Shadowflame, Cards.ZombieChow, Cards.ZombieChow, Cards.Voidcaller, Cards.Voidcaller, Cards.Loatheb, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.DrBoom, Cards.AntiqueHealbot, Cards.MalGanis, Cards.Darkbomb, };
        private readonly List<Card.Cards> dragonHandlock = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.MortalCoil, Cards.MortalCoil, Cards.Darkbomb, Cards.Darkbomb, Cards.AncientWatcher, Cards.AncientWatcher, Cards.IronbeakOwl, Cards.SunfuryProtector, Cards.SunfuryProtector, Cards.BigGameHunter, Cards.Hellfire, Cards.Hellfire, Cards.Shadowflame, Cards.DefenderofArgus, Cards.TwilightDrake, Cards.TwilightDrake, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.AntiqueHealbot, Cards.BlackwingCorruptor, Cards.BlackwingCorruptor, Cards.EmperorThaurissan, Cards.Chillmaw, Cards.DrBoom, Cards.Alexstrasza, Cards.MountainGiant, Cards.MountainGiant, Cards.MoltenGiant, Cards.MoltenGiant, };
        private readonly List<Card.Cards> malyLock = new List<Card.Cards> { Cards.MortalCoil, Cards.MortalCoil, Cards.BigGameHunter, Cards.BigGameHunter, Cards.Hellfire, Cards.Hellfire, Cards.Malygos, Cards.AzureDrake, Cards.TwilightDrake, Cards.TwilightDrake, Cards.IronbeakOwl, Cards.Soulfire, Cards.EarthenRingFarseer, Cards.AbusiveSergeant, Cards.ZombieChow, Cards.ZombieChow, Cards.Loatheb, Cards.SludgeBelcher, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.Darkbomb, Cards.Darkbomb, Cards.BlackwingCorruptor, Cards.BlackwingCorruptor, Cards.EmperorThaurissan, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.BrannBronzebeard, Cards.DarkPeddler, Cards.DarkPeddler, };

        private readonly List<Card.Cards> dragonHunter = new List<Card.Cards> { Cards.HuntersMark, Cards.HuntersMark, Cards.WildPyromancer, Cards.WildPyromancer, Cards.ArcaneShot, Cards.ArcaneShot, Cards.GladiatorsLongbow, Cards.AzureDrake, Cards.AzureDrake, Cards.Alexstrasza, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.Ysera, Cards.Gahzrilla, Cards.BlackwingTechnician, Cards.BlackwingTechnician, Cards.RendBlackhand, Cards.BlackwingCorruptor, Cards.BlackwingCorruptor, Cards.DrakonidCrusher, Cards.DrakonidCrusher, Cards.Chromaggus, Cards.QuickShot, Cards.QuickShot, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.Chillmaw, Cards.KingsElekk, Cards.KingsElekk, Cards.Dreadscale, };
        private readonly List<Card.Cards> renoHunter = new List<Card.Cards> { Cards.SavannahHighmane, Cards.HuntersMark, Cards.BigGameHunter, Cards.FreezingTrap, Cards.Tracking, Cards.Houndmaster, Cards.UnleashtheHounds, Cards.ExplosiveTrap, Cards.EaglehornBow, Cards.KnifeJuggler, Cards.KillCommand, Cards.Ysera, Cards.IronbeakOwl, Cards.AnimalCompanion, Cards.Webspinner, Cards.Loatheb, Cards.MadScientist, Cards.MadScientist, Cards.SludgeBelcher, Cards.HauntedCreeper, Cards.DrBoom, Cards.PilotedShredder, Cards.QuickShot, Cards.ArgentHorserider, Cards.BearTrap, Cards.KingsElekk, Cards.Powershot, Cards.JeweledScarab, Cards.SirFinleyMrrgglton, Cards.RenoJackson, };
        private readonly List<Card.Cards> renoHunter2 = new List<Card.Cards> { Cards.HuntersMark, Cards.SavannahHighmane, Cards.BigGameHunter, Cards.Houndmaster, Cards.UnleashtheHounds, Cards.ExplosiveTrap, Cards.EaglehornBow, Cards.Doomsayer, Cards.IronbeakOwl, Cards.DefenderofArgus, Cards.EarthenRingFarseer, Cards.AnimalCompanion, Cards.HarrisonJones, Cards.MadScientist, Cards.SludgeBelcher, Cards.Deathlord, Cards.HauntedCreeper, Cards.DrBoom, Cards.AntiqueHealbot, Cards.QuickShot, Cards.JusticarTrueheart, Cards.BearTrap, Cards.KingsElekk, Cards.Powershot, Cards.EliseStarseeker, Cards.JeweledScarab, Cards.BrannBronzebeard, Cards.SirFinleyMrrgglton, Cards.RenoJackson, Cards.ArchThiefRafaam, };
        private readonly List<Card.Cards> injuredCamel = new List<Card.Cards> { Cards.SavannahHighmane, Cards.SavannahHighmane, Cards.HuntersMark, Cards.HuntersMark, Cards.CultMaster, Cards.CultMaster, Cards.Houndmaster, Cards.Houndmaster, Cards.UnleashtheHounds, Cards.UnleashtheHounds, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.AnimalCompanion, Cards.AnimalCompanion, Cards.Webspinner, Cards.Webspinner, Cards.Loatheb, Cards.SludgeBelcher, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.PilotedShredder, Cards.Glaivezooka, Cards.Glaivezooka, Cards.FlameJuggler, Cards.FlameJuggler, Cards.InjuredKvaldir, Cards.InjuredKvaldir, Cards.DesertCamel, Cards.DesertCamel, };
        private readonly List<Card.Cards> hybridHunter = new List<Card.Cards> { Cards.SavannahHighmane, Cards.SavannahHighmane, Cards.FreezingTrap, Cards.UnleashtheHounds, Cards.UnleashtheHounds, Cards.ExplosiveTrap, Cards.EaglehornBow, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.KillCommand, Cards.KillCommand, Cards.IronbeakOwl, Cards.LeperGnome, Cards.LeperGnome, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.AnimalCompanion, Cards.AnimalCompanion, Cards.Loatheb, Cards.MadScientist, Cards.MadScientist, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.PilotedShredder, Cards.PilotedShredder, Cards.Glaivezooka, Cards.Glaivezooka, Cards.QuickShot, Cards.ArgentHorserider, Cards.ArgentHorserider, };
        private readonly List<Card.Cards> hatHunter = new List<Card.Cards> { Cards.HuntersMark, Cards.HuntersMark, Cards.SylvanasWindrunner, Cards.UnleashtheHounds, Cards.UnleashtheHounds, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.AnimalCompanion, Cards.AnimalCompanion, Cards.Flare, Cards.ZombieChow, Cards.NerubianEgg, Cards.NerubianEgg, Cards.Webspinner, Cards.Webspinner, Cards.Loatheb, Cards.HauntedCreeper, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.Glaivezooka, Cards.Glaivezooka, Cards.QuickShot, Cards.QuickShot, Cards.BallofSpiders, Cards.BearTrap, Cards.ExplorersHat, Cards.ExplorersHat, Cards.JeweledScarab, Cards.JeweledScarab, };
        private readonly List<Card.Cards> midRangeHunter = new List<Card.Cards> { Cards.SavannahHighmane, Cards.SavannahHighmane, Cards.HuntersMark, Cards.FreezingTrap, Cards.FreezingTrap, Cards.Houndmaster, Cards.Houndmaster, Cards.UnleashtheHounds, Cards.StranglethornTiger, Cards.EaglehornBow, Cards.EaglehornBow, Cards.KillCommand, Cards.KillCommand, Cards.IronbeakOwl, Cards.AnimalCompanion, Cards.AnimalCompanion, Cards.Webspinner, Cards.Webspinner, Cards.Loatheb, Cards.MadScientist, Cards.MadScientist, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.Glaivezooka, Cards.QuickShot, Cards.KingsElekk, Cards.KingsElekk, };
        private readonly List<Card.Cards> midRangeHunter2 = new List<Card.Cards>{Cards.SavannahHighmane,Cards.SavannahHighmane,Cards.HuntersMark,Cards.FreezingTrap,Cards.FreezingTrap,Cards.Houndmaster,Cards.Houndmaster,Cards.DeadlyShot,Cards.UnleashtheHounds,Cards.UnleashtheHounds,Cards.EaglehornBow,Cards.EaglehornBow,Cards.StampedingKodo,Cards.KillCommand,Cards.KillCommand,Cards.AnimalCompanion,Cards.AnimalCompanion,Cards.QuickShot,Cards.QuickShot,Cards.RamWrangler,Cards.KingsElekk,Cards.KingsElekk,Cards.HugeToad,Cards.HugeToad,Cards.CalloftheWild,Cards.CalloftheWild,Cards.FieryBat,Cards.FieryBat,Cards.InfestedWolf,Cards.CarrionGrub,};
        private readonly List<Card.Cards> midRangeHunter3 = new List<Card.Cards>{Cards.SavannahHighmane,Cards.SavannahHighmane,Cards.HuntersMark,Cards.SylvanasWindrunner,Cards.FreezingTrap,Cards.Houndmaster,Cards.Houndmaster,Cards.UnleashtheHounds,Cards.UnleashtheHounds,Cards.EaglehornBow,Cards.EaglehornBow,Cards.StampedingKodo,Cards.KillCommand,Cards.KillCommand,Cards.RagnarostheFirelord,Cards.AnimalCompanion,Cards.AnimalCompanion,Cards.HarrisonJones,Cards.QuickShot,Cards.QuickShot,Cards.KingsElekk,Cards.HugeToad,Cards.HugeToad,Cards.CalloftheWild,Cards.CalloftheWild,Cards.PrincessHuhuran,Cards.FieryBat,Cards.FieryBat,Cards.InfestedWolf,Cards.InfestedWolf,};
        private readonly List<Card.Cards> midRangeHunter4 = new List<Card.Cards>{Cards.SavannahHighmane,Cards.SavannahHighmane,Cards.HuntersMark,Cards.SylvanasWindrunner,Cards.FreezingTrap,Cards.Houndmaster,Cards.Houndmaster,Cards.UnleashtheHounds,Cards.UnleashtheHounds,Cards.EaglehornBow,Cards.EaglehornBow,Cards.StampedingKodo,Cards.KillCommand,Cards.KillCommand,Cards.RagnarostheFirelord,Cards.AnimalCompanion,Cards.AnimalCompanion,Cards.HarrisonJones,Cards.QuickShot,Cards.QuickShot,Cards.KingsElekk,Cards.HugeToad,Cards.HugeToad,Cards.CalloftheWild,Cards.CalloftheWild,Cards.PrincessHuhuran,Cards.FieryBat,Cards.FieryBat,Cards.InfestedWolf,Cards.InfestedWolf,};
        private readonly List<Card.Cards> midRangeHunter5 = new List<Card.Cards>{Cards.SavannahHighmane,Cards.SavannahHighmane,Cards.FreezingTrap,Cards.TundraRhino,Cards.Houndmaster,Cards.Houndmaster,Cards.DeadlyShot,Cards.DeadlyShot,Cards.UnleashtheHounds,Cards.EaglehornBow,Cards.EaglehornBow,Cards.StampedingKodo,Cards.StampedingKodo,Cards.Doomsayer,Cards.Doomsayer,Cards.KillCommand,Cards.KillCommand,Cards.AnimalCompanion,Cards.AnimalCompanion,Cards.QuickShot,Cards.QuickShot,Cards.KingsElekk,Cards.KingsElekk,Cards.HugeToad,Cards.HugeToad,Cards.CalloftheWild,Cards.CalloftheWild,Cards.InfestedWolf,Cards.InfestedWolf,Cards.CarrionGrub,};
        private readonly List<Card.Cards> faceHunter = new List<Card.Cards> { Cards.ArcaneGolem, Cards.WorgenInfiltrator, Cards.WorgenInfiltrator, Cards.UnleashtheHounds, Cards.UnleashtheHounds, Cards.ExplosiveTrap, Cards.EaglehornBow, Cards.EaglehornBow, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.KillCommand, Cards.KillCommand, Cards.IronbeakOwl, Cards.LeperGnome, Cards.LeperGnome, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.AnimalCompanion, Cards.AnimalCompanion, Cards.LeeroyJenkins, Cards.MadScientist, Cards.MadScientist, Cards.Glaivezooka, Cards.QuickShot, Cards.QuickShot, Cards.FlameJuggler, Cards.FlameJuggler, Cards.ArgentHorserider, Cards.ArgentHorserider, Cards.BearTrap, };
        private readonly List<Card.Cards> basicHunter = new List<Card.Cards> { Cards.HuntersMark, Cards.HuntersMark, Cards.ArcaneShot, Cards.ArcaneShot, Cards.Tracking, Cards.AnimalCompanion, Cards.AnimalCompanion, Cards.KillCommand, Cards.KillCommand, Cards.MultiShot, Cards.MultiShot, Cards.Houndmaster, Cards.Houndmaster, Cards.StonetuskBoar, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.RiverCrocolisk, Cards.RiverCrocolisk, Cards.IronfurGrizzly, Cards.IronfurGrizzly, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.OasisSnapjaw, Cards.FrostwolfWarlord, Cards.FrostwolfWarlord, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion, };

        private readonly List<Card.Cards> renoRogue = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.BigGameHunter, Cards.DeadlyPoison, Cards.Sprint, Cards.Betrayal, Cards.BladeFlurry, Cards.AzureDrake, Cards.SI7Agent, Cards.SenjinShieldmasta, Cards.Preparation, Cards.FanofKnives, Cards.Eviscerate, Cards.Sap, Cards.Backstab, Cards.VioletTeacher, Cards.BloodmageThalnos, Cards.EarthenRingFarseer, Cards.Assassinate, Cards.ZombieChow, Cards.SludgeBelcher, Cards.DrBoom, Cards.SneedsOldShredder, Cards.PilotedShredder, Cards.GoblinAutoBarber, Cards.TinkersSharpswordOil, Cards.DarkIronSkulker, Cards.EmperorThaurissan, Cards.Burgle, Cards.JeweledScarab, Cards.RenoJackson, };
        private readonly List<Card.Cards> renoRogue2 = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.BigGameHunter, Cards.DeadlyPoison, Cards.Sprint, Cards.GadgetzanAuctioneer, Cards.BladeFlurry, Cards.YouthfulBrewmaster, Cards.AzureDrake, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.FanofKnives, Cards.Eviscerate, Cards.Sap, Cards.Backstab, Cards.Backstab, Cards.BloodmageThalnos, Cards.EarthenRingFarseer, Cards.ZombieChow, Cards.SludgeBelcher, Cards.DrBoom, Cards.PilotedShredder, Cards.AntiqueHealbot, Cards.Sabotage, Cards.TinkersSharpswordOil, Cards.Burgle, Cards.Anubarak, Cards.RefreshmentVendor, Cards.UnearthedRaptor, Cards.RenoJackson, };
        private readonly List<Card.Cards> millRogue = new List<Card.Cards> { Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.ColdlightOracle, Cards.ColdlightOracle, Cards.Sprint, Cards.BladeFlurry, Cards.YouthfulBrewmaster, Cards.SI7Agent, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.FanofKnives, Cards.FanofKnives, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.Sap, Cards.Backstab, Cards.Backstab, Cards.Shadowstep, Cards.Shadowstep, Cards.Vanish, Cards.Vanish, Cards.GangUp, Cards.GangUp, Cards.BrannBronzebeard, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.Deathlord, Cards.Deathlord, };
        private readonly List<Card.Cards> mechRogue = new List<Card.Cards> { Cards.ColdBlood, Cards.ColdBlood, Cards.BladeFlurry, Cards.FanofKnives, Cards.Eviscerate, Cards.Eviscerate, Cards.HarvestGolem, Cards.HarvestGolem, Cards.EnhanceoMechano, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.DrBoom, Cards.Mechwarper, Cards.Mechwarper, Cards.PilotedShredder, Cards.PilotedShredder, Cards.ClockworkGnome, Cards.ClockworkGnome, Cards.TinkertownTechnician, Cards.TinkertownTechnician, Cards.GoblinAutoBarber, Cards.GoblinAutoBarber, Cards.IronSensei, Cards.IronSensei, Cards.TinkersSharpswordOil, Cards.ClockworkKnight, Cards.ClockworkKnight, Cards.EydisDarkbane, Cards.GorillabotA3, Cards.GorillabotA3, };
        private readonly List<Card.Cards> miracleRogue = new List<Card.Cards> { Cards.EdwinVanCleef, Cards.DeadlyPoison, Cards.ColdBlood, Cards.ColdBlood, Cards.GadgetzanAuctioneer, Cards.GadgetzanAuctioneer, Cards.Shiv, Cards.Shiv, Cards.BladeFlurry, Cards.AzureDrake, Cards.AzureDrake, Cards.Conceal, Cards.SI7Agent, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.FanofKnives, Cards.FanofKnives, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.Sap, Cards.Backstab, Cards.Backstab, Cards.BloodmageThalnos, Cards.Shadowstep, Cards.Shadowstep, Cards.EarthenRingFarseer, Cards.EarthenRingFarseer, Cards.LeeroyJenkins, };
        private readonly List<Card.Cards> oilRogue = new List<Card.Cards> { Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.Sprint, Cards.Sprint, Cards.SouthseaDeckhand, Cards.BladeFlurry, Cards.BladeFlurry, Cards.AzureDrake, Cards.AzureDrake, Cards.SI7Agent, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.FanofKnives, Cards.FanofKnives, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.Sap, Cards.Backstab, Cards.Backstab, Cards.VioletTeacher, Cards.BloodmageThalnos, Cards.EarthenRingFarseer, Cards.PilotedShredder, Cards.PilotedShredder, Cards.AntiqueHealbot, Cards.Sabotage, Cards.TinkersSharpswordOil, Cards.TinkersSharpswordOil, };
        private readonly List<Card.Cards> pirateRogue = new List<Card.Cards> { Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.Sprint, Cards.Sprint, Cards.SouthseaDeckhand, Cards.SouthseaDeckhand, Cards.BladeFlurry, Cards.BladeFlurry, Cards.DreadCorsair, Cards.DreadCorsair, Cards.AzureDrake, Cards.AzureDrake, Cards.SI7Agent, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.AssassinsBlade, Cards.Backstab, Cards.Backstab, Cards.BloodsailRaider, Cards.BloodsailRaider, Cards.ShipsCannon, Cards.ShipsCannon, Cards.TinkersSharpswordOil, Cards.TinkersSharpswordOil, Cards.Buccaneer, Cards.Buccaneer, };
        private readonly List<Card.Cards> malyRogue = new List<Card.Cards> { Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.GadgetzanAuctioneer, Cards.GadgetzanAuctioneer, Cards.Shiv, Cards.Shiv, Cards.Malygos, Cards.BladeFlurry, Cards.BladeFlurry, Cards.AzureDrake, Cards.AzureDrake, Cards.SI7Agent, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.FanofKnives, Cards.FanofKnives, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.Sap, Cards.Backstab, Cards.Backstab, Cards.TombPillager, Cards.TombPillager, Cards.BloodmageThalnos, Cards.VioletTeacher, Cards.EmperorThaurissan, Cards.AntiqueHealbot, Cards.AntiqueHealbot, };
        private readonly List<Card.Cards> fatigueRogue = new List<Card.Cards> { Cards.BigGameHunter, Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.ColdlightOracle, Cards.ColdlightOracle, Cards.BladeFlurry, Cards.BladeFlurry, Cards.SI7Agent, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.FanofKnives, Cards.FanofKnives, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.Sap, Cards.Backstab, Cards.Backstab, Cards.BloodmageThalnos, Cards.Shadowstep, Cards.Shadowstep, Cards.Vanish, Cards.Vanish, Cards.Deathlord, Cards.Deathlord, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.TinkersSharpswordOil, Cards.GangUp, };
        private readonly List<Card.Cards> raptorRogue = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.ColdBlood, Cards.ColdBlood, Cards.FanofKnives, Cards.FanofKnives, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.LootHoarder, Cards.LootHoarder, Cards.Backstab, Cards.Backstab, Cards.UnearthedRaptor, Cards.UnearthedRaptor, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.LeperGnome, Cards.LeperGnome, Cards.NerubianEgg, Cards.NerubianEgg, Cards.DefenderofArgus, Cards.DefenderofArgus, Cards.PilotedShredder, Cards.PilotedShredder, Cards.Loatheb, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.DrBoom, Cards.HauntedCreeper, Cards.HauntedCreeper, };
        private readonly List<Card.Cards> faceRogue = new List<Card.Cards> { Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.ColdlightOracle, Cards.ColdlightOracle, Cards.ColdBlood, Cards.ColdBlood, Cards.ArcaneGolem, Cards.ArcaneGolem, Cards.SouthseaDeckhand, Cards.SouthseaDeckhand, Cards.Wolfrider, Cards.Wolfrider, Cards.BladeFlurry, Cards.BladeFlurry, Cards.SI7Agent, Cards.SI7Agent, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.Sap, Cards.LeperGnome, Cards.LeperGnome, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.TinkersSharpswordOil, Cards.TinkersSharpswordOil, Cards.ArgentHorserider, Cards.ArgentHorserider, Cards.Buccaneer, Cards.Buccaneer, };
        private readonly List<Card.Cards> basic = new List<Card.Cards> { Cards.Backstab, Cards.Backstab, Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.Sap, Cards.Shiv, Cards.Shiv, Cards.FanofKnives, Cards.FanofKnives, Cards.AssassinsBlade, Cards.AssassinsBlade, Cards.Assassinate, Cards.Assassinate, Cards.Sprint, Cards.AcidicSwampOoze, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.KoboldGeomancer, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.GnomishInventor, Cards.SenjinShieldmasta, Cards.SenjinShieldmasta, Cards.BoulderfistOgre, Cards.BoulderfistOgre, };

        private readonly List<Card.Cards> tokenDruid = new List<Card.Cards> { Cards.PoweroftheWild, Cards.PoweroftheWild, Cards.SouloftheForest, Cards.SouloftheForest, Cards.SavageRoar, Cards.SavageRoar, Cards.KeeperoftheGrove, Cards.MarkoftheWild, Cards.MarkoftheWild, Cards.DefenderofArgus, Cards.DefenderofArgus, Cards.Innervate, Cards.Innervate, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.LivingRoots, Cards.LivingRoots, Cards.MountedRaptor, Cards.MountedRaptor, Cards.DragonEgg, Cards.DragonEgg, Cards.NerubianEgg, Cards.NerubianEgg, Cards.EchoingOoze, Cards.EchoingOoze, Cards.Jeeves, Cards.Jeeves, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.SirFinleyMrrgglton, };
        private readonly List<Card.Cards> tokenDruid2 = new List<Card.Cards>{Cards.Nourish,Cards.Nourish,Cards.PoweroftheWild,Cards.PoweroftheWild,Cards.WildGrowth,Cards.WildGrowth,Cards.SouloftheForest,Cards.SavageRoar,Cards.VioletTeacher,Cards.VioletTeacher,Cards.BloodmageThalnos,Cards.Innervate,Cards.Innervate,Cards.Cenarius,Cards.Swipe,Cards.Swipe,Cards.Wrath,Cards.Wrath,Cards.LivingRoots,Cards.LivingRoots,Cards.Mulch,Cards.RavenIdol,Cards.RavenIdol,Cards.MireKeeper,Cards.MireKeeper,Cards.YoggSaronHopesEnd,Cards.WispsoftheOldGods,Cards.WispsoftheOldGods,Cards.FandralStaghelm,Cards.FeralRage,};
        private readonly List<Card.Cards> basicDruid = new List<Card.Cards> { Cards.Innervate, Cards.Innervate, Cards.Claw, Cards.Claw, Cards.MarkoftheWild, Cards.MarkoftheWild, Cards.WildGrowth, Cards.WildGrowth, Cards.Swipe, Cards.Swipe, Cards.Starfire, Cards.Starfire, Cards.IronbarkProtector, Cards.IronbarkProtector, Cards.AcidicSwampOoze, Cards.RiverCrocolisk, Cards.RiverCrocolisk, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.GnomishInventor, Cards.SenjinShieldmasta, Cards.SenjinShieldmasta, Cards.DarkscaleHealer, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion, Cards.StormwindChampion, };
        private readonly List<Card.Cards> renoDruid = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.AncientofLore, Cards.BigGameHunter, Cards.ForceofNature, Cards.AncientofWar, Cards.AzureDrake, Cards.WildGrowth, Cards.SenjinShieldmasta, Cards.SavageRoar, Cards.MindControlTech, Cards.KeeperoftheGrove, Cards.Innervate, Cards.DruidoftheClaw, Cards.HarrisonJones, Cards.Cenarius, Cards.Swipe, Cards.Wrath, Cards.ZombieChow, Cards.ShadeofNaxxramas, Cards.Loatheb, Cards.SludgeBelcher, Cards.DrBoom, Cards.PilotedShredder, Cards.SavageCombatant, Cards.DarnassusAspirant, Cards.LivingRoots, Cards.JeweledScarab, Cards.RenoJackson, Cards.RavenIdol, Cards.MountedRaptor, };
        private readonly List<Card.Cards> renoDruid2 = new List<Card.Cards> { Cards.AncientofLore, Cards.BigGameHunter, Cards.ForceofNature, Cards.AncientofWar, Cards.AzureDrake, Cards.WildGrowth, Cards.SavageRoar, Cards.TheBlackKnight, Cards.KnifeJuggler, Cards.KeeperoftheGrove, Cards.BloodmageThalnos, Cards.Innervate, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.ZombieChow, Cards.RavenIdol, Cards.DarnassusAspirant, Cards.MountedRaptor, Cards.SavageCombatant, Cards.HauntedCreeper, Cards.PilotedShredder, Cards.Loatheb, Cards.SludgeBelcher, Cards.EmperorThaurissan, Cards.RenoJackson, Cards.DrBoom, Cards.LivingRoots, Cards.ShadeofNaxxramas, };
        private readonly List<Card.Cards> beastDruid = new List<Card.Cards> { Cards.AncientofLore, Cards.AncientofLore, Cards.ForceofNature, Cards.SavageRoar, Cards.SavageRoar, Cards.IronbeakOwl, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.Wrath, Cards.ShadeofNaxxramas, Cards.Loatheb, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.DruidoftheFang, Cards.SavageCombatant, Cards.SavageCombatant, Cards.KnightoftheWild, Cards.Wildwalker, Cards.JeweledScarab, Cards.JeweledScarab, Cards.TombSpider, Cards.RavenIdol, Cards.MountedRaptor, Cards.MountedRaptor, };
        private readonly List<Card.Cards> beastDruid2 = new List<Card.Cards>{Cards.PoweroftheWild,Cards.PoweroftheWild,Cards.AzureDrake,Cards.AzureDrake,Cards.SavageRoar,Cards.SavageRoar,Cards.VioletTeacher,Cards.VioletTeacher,Cards.Innervate,Cards.Innervate,Cards.DruidoftheClaw,Cards.DruidoftheClaw,Cards.Swipe,Cards.Swipe,Cards.LeeroyJenkins,Cards.DruidoftheFlame,Cards.DruidoftheFlame,Cards.SavageCombatant,Cards.SavageCombatant,Cards.DarnassusAspirant,Cards.DarnassusAspirant,Cards.DruidoftheSaber,Cards.DruidoftheSaber,Cards.LivingRoots,Cards.LivingRoots,Cards.SirFinleyMrrgglton,Cards.MountedRaptor,Cards.MountedRaptor,Cards.MarkofYShaarj,Cards.MarkofYShaarj,};
        private readonly List<Card.Cards> astralDruid = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.AncientofLore, Cards.AncientofLore, Cards.Nourish, Cards.Nourish, Cards.AncientofWar, Cards.AncientofWar, Cards.WildGrowth, Cards.WildGrowth, Cards.Alexstrasza, Cards.Ysera, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.KelThuzad, Cards.DrBoom, Cards.TreeofLife, Cards.GroveTender, Cards.GroveTender, Cards.Nefarian, Cards.Chromaggus, Cards.EmperorThaurissan, Cards.FrostGiant, Cards.FrostGiant, Cards.Aviana, Cards.AstralCommunion, Cards.AstralCommunion, };
        private readonly List<Card.Cards> mechDruid = new List<Card.Cards> { Cards.AncientofLore, Cards.AncientofLore, Cards.PoweroftheWild, Cards.ForceofNature, Cards.ForceofNature, Cards.SavageRoar, Cards.SavageRoar, Cards.KeeperoftheGrove, Cards.KeeperoftheGrove, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.Wrath, Cards.Starfire, Cards.HauntedCreeper, Cards.Cogmaster, Cards.Cogmaster, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.DrBoom, Cards.SpiderTank, Cards.SpiderTank, Cards.Mechwarper, Cards.Mechwarper, Cards.PilotedShredder, Cards.PilotedShredder, Cards.TinkertownTechnician, };
        private readonly List<Card.Cards> millDruid = new List<Card.Cards> { Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.ColdlightOracle, Cards.ColdlightOracle, Cards.BladeFlurry, Cards.SI7Agent, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.KingMukla, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.Sap, Cards.Backstab, Cards.Backstab, Cards.Shadowstep, Cards.Shadowstep, Cards.Vanish, Cards.Vanish, Cards.Deathlord, Cards.Deathlord, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.GangUp, Cards.GangUp, Cards.RefreshmentVendor, Cards.RefreshmentVendor, Cards.BeneaththeGrounds, Cards.BrannBronzebeard, };
        private readonly List<Card.Cards> rampDruid = new List<Card.Cards> { Cards.AncientofLore, Cards.AncientofLore, Cards.BigGameHunter, Cards.ForceofNature, Cards.ForceofNature, Cards.AncientofWar, Cards.AzureDrake, Cards.AzureDrake, Cards.WildGrowth, Cards.WildGrowth, Cards.SavageRoar, Cards.SavageRoar, Cards.KeeperoftheGrove, Cards.KeeperoftheGrove, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.Wrath, Cards.ShadeofNaxxramas, Cards.ShadeofNaxxramas, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.EmperorThaurissan, Cards.DarnassusAspirant, Cards.DarnassusAspirant, Cards.LivingRoots, };
        private readonly List<Card.Cards> rampDruid2 = new List<Card.Cards> { Cards.AncientofLore, Cards.AncientofLore, Cards.BigGameHunter, Cards.AncientofWar, Cards.AncientofWar, Cards.WildGrowth, Cards.WildGrowth, Cards.KeeperoftheGrove, Cards.KeeperoftheGrove, Cards.RagnarostheFirelord, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.Wrath, Cards.ZombieChow, Cards.ZombieChow, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.DrBoom, Cards.EmperorThaurissan, Cards.DruidoftheFlame, Cards.DruidoftheFlame, Cards.DarnassusAspirant, Cards.DarnassusAspirant, Cards.MasterJouster, Cards.MasterJouster, };
        private readonly List<Card.Cards> rampDruid3 = new List<Card.Cards> { Cards.AncientofLore, Cards.AncientofLore, Cards.BigGameHunter, Cards.Sunwalker, Cards.AncientofWar, Cards.AncientofWar, Cards.AzureDrake, Cards.WildGrowth, Cards.WildGrowth, Cards.SenjinShieldmasta, Cards.SenjinShieldmasta, Cards.MindControlTech, Cards.KeeperoftheGrove, Cards.KeeperoftheGrove, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.Wrath, Cards.ZombieChow, Cards.KelThuzad, Cards.Loatheb, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.Deathlord, Cards.DrBoom, Cards.EmperorThaurissan, };
        private readonly List<Card.Cards> rampDruidLC = new List<Card.Cards> { Cards.Innervate, Cards.Innervate, Cards.LivingRoots, Cards.LivingRoots, Cards.WildGrowth, Cards.WildGrowth, Cards.Wrath, Cards.Wrath, Cards.DarnassusAspirant, Cards.DarnassusAspirant, Cards.Mulch, Cards.Swipe, Cards.Swipe, Cards.FandralStaghelm, Cards.MireKeeper, Cards.MireKeeper, Cards.Nourish, Cards.DruidoftheClaw, Cards.DruidoftheClaw, Cards.AncientofWar, Cards.AncientofWar, Cards.Spellbreaker, Cards.AzureDrake, Cards.AzureDrake, Cards.NagaSeaWitch, Cards.EmperorThaurissan, Cards.SylvanasWindrunner, Cards.RagnarostheFirelord, Cards.Ysera, Cards.YShaarjRageUnbound, };
        private readonly List<Card.Cards> rampDruidKol = new List<Card.Cards> { Cards.Innervate, Cards.Innervate, Cards.LivingRoots, Cards.LivingRoots, Cards.RavenIdol, Cards.WildGrowth, Cards.WildGrowth, Cards.Wrath, Cards.Wrath, Cards.Mulch, Cards.Mulch, Cards.Swipe, Cards.Swipe, Cards.FandralStaghelm, Cards.MireKeeper, Cards.MireKeeper, Cards.Nourish, Cards.Nourish, Cards.DruidoftheClaw, Cards.DruidoftheClaw, Cards.DarkArakkoa, Cards.DarkArakkoa, Cards.AncientofWar, Cards.AncientofWar, Cards.Cenarius, Cards.Spellbreaker, Cards.AzureDrake, Cards.AzureDrake, Cards.SylvanasWindrunner, Cards.RagnarostheFirelord, };
        private readonly List<Card.Cards> aggroDruid = new List<Card.Cards> { Cards.ForceofNature, Cards.ForceofNature, Cards.SavageRoar, Cards.SavageRoar, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.KeeperoftheGrove, Cards.KeeperoftheGrove, Cards.LeperGnome, Cards.LeperGnome, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.ShadeofNaxxramas, Cards.ShadeofNaxxramas, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.FelReaver, Cards.FelReaver, Cards.SavageCombatant, Cards.DarnassusAspirant, Cards.DarnassusAspirant, Cards.DruidoftheSaber, Cards.DruidoftheSaber, Cards.LivingRoots, Cards.LivingRoots, };
        private readonly List<Card.Cards> midRangeDruid = new List<Card.Cards> { Cards.AncientofLore, Cards.AncientofLore, Cards.BigGameHunter, Cards.ForceofNature, Cards.ForceofNature, Cards.AzureDrake, Cards.AzureDrake, Cards.WildGrowth, Cards.WildGrowth, Cards.SavageRoar, Cards.SavageRoar, Cards.KeeperoftheGrove, Cards.KeeperoftheGrove, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.Wrath, Cards.ShadeofNaxxramas, Cards.Loatheb, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.EmperorThaurissan, Cards.DarnassusAspirant, Cards.LivingRoots, Cards.LivingRoots, };
        private readonly List<Card.Cards> silenceDruid = new List<Card.Cards> { Cards.AncientWatcher, Cards.AncientWatcher, Cards.ForceofNature, Cards.ForceofNature, Cards.AzureDrake, Cards.AzureDrake, Cards.SavageRoar, Cards.SavageRoar, Cards.KeeperoftheGrove, Cards.KeeperoftheGrove, Cards.IronbeakOwl, Cards.IronbeakOwl, Cards.Innervate, Cards.Innervate, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.Wrath, Cards.Deathlord, Cards.Deathlord, Cards.WailingSoul, Cards.WailingSoul, Cards.DrBoom, Cards.FelReaver, Cards.FelReaver, Cards.DarnassusAspirant, Cards.DarnassusAspirant, Cards.Mulch, Cards.EerieStatue, Cards.EerieStatue, };
        private readonly List<Card.Cards> cthunDruid = new List<Card.Cards> { Cards.Innervate, Cards.LivingRoots, Cards.LivingRoots, Cards.WildGrowth, Cards.Wrath, Cards.BeckonerofEvil, Cards.DarnassusAspirant, Cards.FeralRage, Cards.DiscipleofCThun, Cards.DiscipleofCThun, Cards.TwilightElder, Cards.Swipe, Cards.CThunsChosen, Cards.KlaxxiAmberWeaver, Cards.CrazedWorshipper, Cards.DarkArakkoa, Cards.TwinEmperorVeklor, Cards.Doomcaller, Cards.CThun };
        #endregion 
        public DeckData GetDeckInfo(Card.CClass cClass, List<string> curDeck, int activeSecrets = 0)
        {
            List<Card.Cards> CurrentDeck = curDeck.Select(q => (Card.Cards)Enum.Parse(typeof(Card.Cards), q)).ToList().Where(card => CardTemplate.LoadFromId(card).IsCollectible).ToList();
            var info = new DeckData { DeckList = CurrentDeck, DeckType = DeckType.Unknown, DeckStyle = DeckStyles[DeckType.Unknown] };
            if (CurrentDeck.Count == 0) return info;
            string str = CurrentDeck.Aggregate("", (current, q) => current + ("Cards." + CardTemplate.LoadFromId(q).Name.Replace(" ", "") + ", "));
            Log("[ABTracker_debug] " + str);
           
            Dictionary<DeckType, int> deckDictionary = new Dictionary<DeckType, int>();

            switch (cClass)
            {
                #region shaman

                case Card.CClass.SHAMAN:
                if (CurrentDeck.IsCthun())
                {
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.CThunShaman, DeckStyle = DeckStyles[DeckType.CThunShaman] };
                }
                int[] TotemShamans = new int[6]
                {
                        CurrentDeck.Intersect(TotemShaman).Count(),
                        CurrentDeck.Intersect(TotemShaman2).Count(),
                        CurrentDeck.Intersect(TotemShaman3).Count(),
                        CurrentDeck.Intersect(TotemShaman4).Count(),
                        CurrentDeck.Intersect(BloodlustShaman).Count(),
                        CurrentDeck.Intersect(BloodlustShaman).Count(),
                                             
                    };
                    
                    deckDictionary.AddOrUpdate(DeckType.MidrangeShaman, TotemShamans.Max());
                if (CurrentDeck.IsRenoDeck())
                {
                    deckDictionary.AddOrUpdate(DeckType.RenoShaman, CurrentDeck.Intersect(renoShaman).Count());
                }
                if (CurrentDeck.RaceCount(Card.CRace.MECH) > 2)
                {
                    deckDictionary.AddOrUpdate(DeckType.MechShaman, CurrentDeck.Intersect(MechShaman).Count());
                }
                if (CurrentDeck.Contains(Cards.Malygos))
                {
                    deckDictionary.AddOrUpdate(DeckType.MalygosShaman, CurrentDeck.Intersect(MalygosShaman).Count());
                }

                deckDictionary.AddOrUpdate(DeckType.FaceShaman, CurrentDeck.Intersect(FaceShaman).Count());
                if (CurrentDeck.ContainsSome(Cards.Malygos, Cards.TwilightGuardian))
                {
                    deckDictionary.AddOrUpdate(DeckType.DragonShaman, CurrentDeck.Intersect(DragonShaman).Count());
                }
                if (CurrentDeck.ContainsSome(Cards.Doomsayer, Cards.BigGameHunter, Cards.ElementalDestruction, Cards.HealingWave, Cards.LightningStorm, Cards.JeweledScarab))
                {
                    int[] chmn = new int[3]
                    {
                        CurrentDeck.Intersect(ControlShaman).Count(),
                        CurrentDeck.Intersect(ControlShaman2).Count(),
                        CurrentDeck.Intersect(ControlShaman3).Count(),
                    };
                    deckDictionary.AddOrUpdate(DeckType.ControlShaman,chmn.Max());
                    
                }

                

                deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(BasicChaman).Count());

                break;

                #endregion

                #region priest

                case Card.CClass.PRIEST:
                if (CurrentDeck.IsCthun())
                {
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.ControlPriest, DeckStyle = DeckStyles[DeckType.ControlPriest] };
                }
                if (Turn < 3 && CurrentDeck.Count == 0)
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.ControlPriest, DeckStyle = DeckStyles[DeckType.ControlPriest] };
                if (CurrentDeck.ContainsSome(Cards.WyrmrestAgent, Cards.TwilightWhelp, Cards.TwilightGuardian, Cards.BlackwingCorruptor, Cards.BlackwingTechnician))
                {
                    if (!CurrentDeck.IsRenoDeck())
                    {
                        info.DeckType = DeckType.DragonPriest;
                        info.DeckStyle = DeckStyles[DeckType.DragonPriest];
                        return info;
                    }
                }
                deckDictionary.AddOrUpdate(DeckType.ControlPriest, CurrentDeck.Intersect(ContrlPriest).Count());
                if (CurrentDeck.ContainsSome(Cards.InnerFire, Cards.ProphetVelen))
                {
                    deckDictionary.AddOrUpdate(DeckType.ComboPriest, CurrentDeck.Intersect(ComboPriest).Count());
                }
                if (CurrentDeck.RaceCount(Card.CRace.MECH) > 2)
                {
                    deckDictionary.AddOrUpdate(DeckType.MechPriest, CurrentDeck.Intersect(MechPriest).Count());
                }

               
                deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(BasicPriest).Count());

                break;

                #endregion

                #region mage

                case Card.CClass.MAGE:
                if (CurrentDeck.IsCthun())
                {
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.CThunMage, DeckStyle = DeckStyles[DeckType.CThunMage] };
                }
                if (Turn < 3 && CurrentDeck.Count == 0)
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.FreezeMage, DeckStyle = DeckStyles[DeckType.FreezeMage] };
                if (CurrentDeck.IsRenoDeck())
                {
                    deckDictionary.AddOrUpdate(DeckType.RenoMage, CurrentDeck.Intersect(renoMage).Count());
                }
                deckDictionary.AddOrUpdate(DeckType.TempoMage, CurrentDeck.Intersect(tempoMage).Count());
                if (CurrentDeck.ContainsAtLeast(2, Cards.IceBarrier, Cards.IceBlock, Cards.AcolyteofPain, Cards.LootHoarder, Cards.NoviceEngineer, Cards.MadScientist))
                {
                    deckDictionary.AddOrUpdate(DeckType.FreezeMage, CurrentDeck.Intersect(freeze).Count());
                    deckDictionary.AddOrUpdate(DeckType.FaceFreezeMage, CurrentDeck.Intersect(faceFreeze).Count());
                }
                if (CurrentDeck.RaceCount(Card.CRace.DRAGON) > 2)
                {
                    deckDictionary.AddOrUpdate(DeckType.DragonMage, CurrentDeck.Intersect(dragonMage).Count());
                }
                if (CurrentDeck.RaceCount(Card.CRace.MECH) > 3 || CurrentDeck.ContainsSome(Cards.Cogmaster, Cards.ClockworkGnome, Cards.Mechwarper, Cards.SpiderTank, Cards.TinkertownTechnician, Cards.Snowchugger))
                {
                    deckDictionary.AddOrUpdate(DeckType.MechMage, CurrentDeck.Intersect(mechMage).Count());
                }
                
                deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(basicMage).Count());
                break;

                #endregion

                #region paladin

                case Card.CClass.PALADIN:
                if (CurrentDeck.IsCthun())
                {
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.CThunPaladin, DeckStyle = DeckStyles[DeckType.CThunPaladin] };
                }
                if (activeSecrets > 0)
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.SecretPaladin, DeckStyle = DeckStyles[DeckType.SecretPaladin] };
                if (CurrentDeck.IsRenoDeck(10))
                {
                    int[] reno = new int[2]
                    {
                       CurrentDeck.Intersect(renoPaladin).Count(),
                       CurrentDeck.Intersect(renoPaladin2).Count()
                    };
                    deckDictionary.AddOrUpdate(DeckType.RenoPaladin, reno.Max());
                    
                }
                if (CurrentDeck.ContainsSome(Cards.AnyfinCanHappen, Cards.BluegillWarrior, Cards.MurlocWarleader, Cards.OldMurkEye, Cards.Doomsayer))
                {
                    deckDictionary.AddOrUpdate(DeckType.AnyfinMurglMurgl, CurrentDeck.Intersect(anyfinPaladin).Count());
                }
                if (CurrentDeck.ContainsSome(Cards.Secretkeeper, Cards.Avenge, Cards.NobleSacrifice, Cards.Redemption, Cards.MysteriousChallenger, Cards.KnifeJuggler, Cards.ShieldedMinibot, Cards.MusterforBattle))
                {
                    deckDictionary.AddOrUpdate(DeckType.SecretPaladin, CurrentDeck.Intersect(secretPaladin).Count());
                }
                if (CurrentDeck.ContainsSome(Cards.ShieldedMinibot, Cards.PilotedShredder, Cards.AntiqueHealbot, Cards.Quartermaster, Cards.LayonHands, Cards.Doomsayer))
                {
                    int[] midrange = new int[2]
                    {
                      CurrentDeck.Intersect(nZothPaladin).Count(),
                      CurrentDeck.Intersect(midrangePaladin).Count(),
                    };
                    deckDictionary.AddOrUpdate(DeckType.MidRangePaladin, CurrentDeck.Intersect(midrangePaladin).Count());
                    
                }
                if (CurrentDeck.ContainsSome(Cards.TwilightGuardian, Cards.BlackwingTechnician, Cards.DragonConsort, Cards.BlackwingCorruptor) || CurrentDeck.RaceCount(Card.CRace.DRAGON) > 3)
                {
                    deckDictionary.AddOrUpdate(DeckType.DragonPaladin, CurrentDeck.Intersect(dragonPaladin).Count());
                }
                if (CurrentDeck.ContainsAtLeast(2, Cards.AbusiveSergeant, Cards.LeperGnome, Cards.LeeroyJenkins, Cards.DivineFavor, Cards.BlessingofMight, Cards.ArcaneGolem))
                {
                    deckDictionary.AddOrUpdate(DeckType.AggroPaladin, CurrentDeck.Intersect(aggroPaladin).Count());
                }
                
                deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(basicPaladin).Count());
                break;

                #endregion

                #region warrior

                case Card.CClass.WARRIOR:
                deckDictionary.AddOrUpdate(DeckType.TempoWarrior, CurrentDeck.Intersect(tempoWarrior).Count());
                if (CurrentDeck.IsCthun())
                {
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.CThunWarrior, DeckStyle = DeckStyles[DeckType.CThunWarrior] };
                }
                if (Turn < 3 && CurrentDeck.Count == 0)
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.ControlWarrior, DeckStyle = DeckStyles[DeckType.ControlWarrior] };
                if (CurrentDeck.IsRenoDeck())
                {
                    deckDictionary.AddOrUpdate(DeckType.RenoWarrior, CurrentDeck.Intersect(renoWarrior).Count());
                    deckDictionary.AddOrUpdate(DeckType.RenoWarrior, CurrentDeck.Intersect(renoWarrior2).Count());
                }
                
                if (CurrentDeck.Contains(Cards.Deathlord))
                {
                    deckDictionary.AddOrUpdate(DeckType.FatigueWarrior, CurrentDeck.Intersect(fatigueWarrior).Count());
                }
                deckDictionary.AddOrUpdate(DeckType.ControlWarrior, CurrentDeck.Intersect(controlWarriorCards).Count());
                if (CurrentDeck.RaceCount(Card.CRace.DRAGON) > 2 || CurrentDeck.ContainsSome(Cards.AlexstraszasChampion, Cards.TwilightGuardian, Cards.BlackwingCorruptor, Cards.BlackwingTechnician))
                {
                    deckDictionary.AddOrUpdate(DeckType.DragonWarrior, CurrentDeck.Intersect(dragonWarrior).Count());
                }
                if (CurrentDeck.ContainsSome(Cards.GrimPatron, Cards.FrothingBerserker))
                {
                    deckDictionary.AddOrUpdate(DeckType.PatronWarrior, CurrentDeck.Intersect(patronWarrior).Count());
                }
                if (CurrentDeck.ContainsAll(Cards.RagingWorgen, Cards.Charge))
                {
                    deckDictionary.AddOrUpdate(DeckType.WorgenOTKWarrior, CurrentDeck.Intersect(worgen).Count());
                }
                if (CurrentDeck.RaceCount(Card.CRace.MECH) > 2)
                {
                    deckDictionary.AddOrUpdate(DeckType.MechWarrior, CurrentDeck.Intersect(mechWar).Count());
                }
                deckDictionary.AddOrUpdate(DeckType.FaceWarrior, CurrentDeck.Intersect(faceWar).Count());
                deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(basicWarrior).Count());

                break;

                #endregion

                #region warlock
                 
                case Card.CClass.WARLOCK:
                if (CurrentDeck.IsCthun())
                {
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.CThunLock, DeckStyle = DeckStyles[DeckType.CThunLock] };
                }
                if (Turn < 3 && CurrentDeck.Count == 0)
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.RenoLock, DeckStyle = DeckStyles[DeckType.RenoLock] };
                if (CurrentDeck.ContainsAtLeast(2, Cards.JeweledScarab, Cards.AntiqueHealbot, Cards.Ysera, Cards.EmperorThaurissan, Cards.EliseStarseeker, Cards.Demonfire, Cards.Hellfire, Cards.SiphonSoul))
                {
                    deckDictionary.AddOrUpdate(DeckType.ControlWarlock, CurrentDeck.Intersect(controlWarlock).Count());
                }
                if (CurrentDeck.ContainsSome(Cards.MountainGiant, Cards.TwilightDrake, Cards.AncientWatcher, Cards.SunfuryProtector))
                {
                    deckDictionary.AddOrUpdate(DeckType.Handlock, CurrentDeck.Intersect(handlock).Count());
                }
                if (CurrentDeck.IsRenoDeck())
                {
                    var reno = new int[2]
                    {
                      CurrentDeck.Intersect(renoLock).Count(),
                      CurrentDeck.Intersect(RenoCombo).Count(),
                    };
                    deckDictionary.AddOrUpdate(DeckType.RenoLock, reno.Max());
                    
                }
                if (CurrentDeck.ContainsAtLeast(2, Cards.Voidcaller, Cards.Doomguard, Cards.MalGanis, Cards.ImpGangBoss))
                {

                    if (CurrentDeck.ContainsSome(Cards.TwilightDrake, Cards.MountainGiant, Cards.MoltenGiant))
                    {
                        deckDictionary.AddOrUpdate(DeckType.DemonHandlock, CurrentDeck.Intersect(demonHandLock).Count());
                    }
                }
                var zoo = new int[]
                {
                    CurrentDeck.Intersect(demonzoolock).Count(),
                    CurrentDeck.Intersect(zoolock).Count()
                };
                deckDictionary.AddOrUpdate(DeckType.Zoolock, zoo.Max());
                if (CurrentDeck.ContainsAll(Cards.TwilightGuardian, Cards.MountainGiant))
                {
                    deckDictionary.AddOrUpdate(DeckType.DragonHandlock, CurrentDeck.Intersect(dragonHandlock).Count());
                }
                if (CurrentDeck.Contains(Cards.Malygos))
                {
                    deckDictionary.AddOrUpdate(DeckType.MalyLock, CurrentDeck.Intersect(malyLock).Count());
                }
                break;

                #endregion

                #region hunter

                case Card.CClass.HUNTER:
                if (CurrentDeck.IsCthun())
                {
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.CThunHunter, DeckStyle = DeckStyles[DeckType.CThunHunter] };
                }
                if (CurrentDeck.RaceCount(Card.CRace.DRAGON) > 3 || CurrentDeck.ContainsAtLeast(2, Cards.TwilightGuardian, Cards.BlackwingTechnician, Cards.BlackwingCorruptor, Cards.DrakonidCrusher, Cards.RendBlackhand))
                {
                    deckDictionary.AddOrUpdate(DeckType.DragonHunter, CurrentDeck.Intersect(dragonHunter).Count());
                }
                int[] mrh = new int[5] {CurrentDeck.Intersect(midRangeHunter).Count(),
                        CurrentDeck.Intersect(midRangeHunter2).Count(),
                        CurrentDeck.Intersect(midRangeHunter3).Count(),
                        CurrentDeck.Intersect(midRangeHunter4).Count(),
                        CurrentDeck.Intersect(midRangeHunter5).Count()
                    };
                if (CurrentDeck.IsRenoDeck())
                {
                    deckDictionary.AddOrUpdate(DeckType.RenoHunter, CurrentDeck.Intersect(renoHunter).Count());
                    deckDictionary.AddOrUpdate(DeckType.RenoHunter, CurrentDeck.Intersect(renoHunter2).Count());
                }
                if (CurrentDeck.ContainsAll(Cards.DesertCamel, Cards.InjuredKvaldir) || CurrentDeck.ContainsAtLeast(2, Cards.FlameJuggler, Cards.Glaivezooka, Cards.CultMaster))
                {
                    deckDictionary.AddOrUpdate(DeckType.CamelHunter, CurrentDeck.Intersect(injuredCamel).Count());
                }
                if (CurrentDeck.ContainsAtLeast(2, Cards.SavannahHighmane, Cards.LeperGnome, Cards.AbusiveSergeant, Cards.ArcaneGolem))
                {
                    deckDictionary.AddOrUpdate(DeckType.HybridHunter, CurrentDeck.Intersect(hybridHunter).Count());
                }
                
                if (CurrentDeck.ContainsAtLeast(2, Cards.ExplosiveTrap, Cards.AbusiveSergeant, Cards.LeperGnome, Cards.KnifeJuggler))
                {
                    deckDictionary.AddOrUpdate(DeckType.FaceHunter, CurrentDeck.Intersect(faceHunter).Count());
                }
                deckDictionary.AddOrUpdate(DeckType.MidRangeHunter, mrh.Max());
                deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(basicHunter).Count());

                break;

                #endregion

                #region rogue

                case Card.CClass.ROGUE:
                if (CurrentDeck.IsCthun())
                {
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.CThunRogue, DeckStyle = DeckStyles[DeckType.CThunRogue] };
                }
                deckDictionary.AddOrUpdate(DeckType.OilRogue, 3);
                if (CurrentDeck.IsRenoDeck(10))
                {
                    List<int> renos = new List<int> { CurrentDeck.Intersect(renoRogue).Count(), CurrentDeck.Intersect(renoRogue2).Count() };
                    deckDictionary.AddOrUpdate(DeckType.RenoRogue, renos.Max());
                    //deckDictionary.AddOrUpdate(DeckType.RenoRogue, CurrentDeck.Intersect(renoRogue2).Count());
                }
                if (CurrentDeck.ContainsSome(Cards.ColdlightOracle, Cards.GangUp))
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.MillRogue, DeckStyle = DeckStyles[DeckType.MillRogue] };
                if (CurrentDeck.ContainsSome(Cards.Deathlord, Cards.Shadowstep, Cards.YouthfulBrewmaster, Cards.Vanish))
                {
                    deckDictionary.AddOrUpdate(DeckType.MillRogue, CurrentDeck.Intersect(millRogue).Count());
                }
                if (CurrentDeck.RaceCount(Card.CRace.MECH) >= 4 || CurrentDeck.ContainsAtLeast(2, Cards.ClockworkGnome, Cards.Mechwarper, Cards.GoblinAutoBarber, Cards.AnnoyoTron, Cards.TinkertownTechnician))
                {
                    deckDictionary.AddOrUpdate(DeckType.MechRogue, CurrentDeck.Intersect(mechRogue).Count());
                }
                if (CurrentDeck.ContainsAtLeast(2, Cards.GadgetzanAuctioneer, Cards.Conceal, Cards.EdwinVanCleef, Cards.AzureDrake, Cards.SI7Agent, Cards.Preparation))
                {
                    deckDictionary.AddOrUpdate(DeckType.MiracleRogue, CurrentDeck.Intersect(miracleRogue).Count());
                }
                if (CurrentDeck.ContainsSome(Cards.TinkersSharpswordOil, Cards.DeadlyPoison, Cards.VioletTeacher, Cards.EdwinVanCleef, Cards.Backstab, Cards.BladeFlurry, Cards.FanofKnives))
                {
                    deckDictionary.AddOrUpdate(DeckType.OilRogue, CurrentDeck.Intersect(oilRogue).Count());
                }

                if (CurrentDeck.Contains(Cards.ShipsCannon))
                {
                    deckDictionary.AddOrUpdate(DeckType.PirateRogue, CurrentDeck.Intersect(pirateRogue).Count());
                }
                if (CurrentDeck.Contains(Cards.EmperorThaurissan))
                {
                    deckDictionary.AddOrUpdate(DeckType.MalyRogue, CurrentDeck.Intersect(malyRogue).Count());
                }
               
                if (CurrentDeck.Contains(Cards.UnearthedRaptor))
                {
                    deckDictionary.AddOrUpdate(DeckType.RaptorRogue, CurrentDeck.Intersect(raptorRogue).Count());
                }
                deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(basic).Count());
                deckDictionary.AddOrUpdate(DeckType.FaceRogue, CurrentDeck.Intersect(faceRogue).Count());
                break;

                #endregion

                #region druid

                case Card.CClass.DRUID:

                if (CurrentDeck.IsCthun())
                {
                    return new DeckData { DeckList = CurrentDeck, DeckType = DeckType.CThunDruid, DeckStyle = DeckStyles[DeckType.CThunDruid] };
                }
                if (CurrentDeck.IsRenoDeck(10)) //at least 1/3 of a deck must be distict
                {
                    deckDictionary.AddOrUpdate(DeckType.RenoDruid, CurrentDeck.Intersect(renoDruid).Count());
                    deckDictionary.AddOrUpdate(DeckType.RenoDruid, CurrentDeck.Intersect(renoDruid2).Count());
                }
                if (CurrentDeck.RaceCount(Card.CRace.BEAST) > 3 || CurrentDeck.ContainsAtLeast(2, Cards.HauntedCreeper, Cards.MountedRaptor, Cards.Wildwalker, Cards.DruidoftheFang))
                {
                    int[] beastdru = new int[2]
                    {
                        CurrentDeck.Intersect(beastDruid).Count(),
                        CurrentDeck.Intersect(beastDruid2).Count(),

                    };
                    deckDictionary.AddOrUpdate(DeckType.BeastDruid, beastdru.Max());
                }
                if (CurrentDeck.QualityCount(Card.CQuality.Legendary) > 4 || CurrentDeck.Contains(Cards.AstralCommunion))
                {
                    deckDictionary.AddOrUpdate(DeckType.AstralDruid, CurrentDeck.Intersect(astralDruid).Count());
                }

                deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(basicDruid).Count());
                if (CurrentDeck.RaceCount(Card.CRace.MECH) > 1 || CurrentDeck.ContainsSome(Cards.Cogmaster, Cards.AnnoyoTron, Cards.Mechwarper, Cards.ClockworkGnome))
                {
                    deckDictionary.AddOrUpdate(DeckType.MechDruid, CurrentDeck.Intersect(mechDruid).Count());
                }
                if (CurrentDeck.ContainsSome(Cards.ColdlightOracle, Cards.Naturalize, Cards.GroveTender, Cards.TreeofLife, Cards.YouthfulBrewmaster))
                {
                    deckDictionary.AddOrUpdate(DeckType.MillDruid, CurrentDeck.Intersect(millDruid).Count());
                }

                if (CurrentDeck.ContainsAtLeast(2, Cards.RagnarostheFirelord, Cards.AncientofWar, Cards.Cenarius, Cards.SylvanasWindrunner, Cards.TheBlackKnight, Cards.SludgeBelcher, Cards.ZombieChow, Cards.SenjinShieldmasta))
                {
                    int[] ramps = new int[5] {CurrentDeck.Intersect(rampDruid).Count(),
                        CurrentDeck.Intersect(rampDruid2).Count(),
                        CurrentDeck.Intersect(rampDruid3).Count(),
                        CurrentDeck.Intersect(rampDruidLC).Count(),
                        CurrentDeck.Intersect(rampDruidKol).Count()
                    };

                    deckDictionary.AddOrUpdate(DeckType.RampDruid, ramps.Max());

                }
                if (CurrentDeck.ContainsSome(Cards.LeperGnome, Cards.MountedRaptor, Cards.FelReaver, Cards.DruidoftheSaber, Cards.KnifeJuggler))
                {
                    deckDictionary.AddOrUpdate(DeckType.AggroDruid, CurrentDeck.Intersect(aggroDruid).Count());
                }
                if (CurrentDeck.ContainsAll(Cards.ForceofNature, Cards.SavageRoar) || CurrentDeck.ContainsSome(Cards.DarnassusAspirant, Cards.ShadeofNaxxramas, Cards.WildGrowth, Cards.Innervate, Cards.Wrath))
                {
                    deckDictionary.AddOrUpdate(DeckType.MidRangeDruid, CurrentDeck.Intersect(midRangeDruid).Count());
                }
                int[] tkndru = new int[2]
                {
                    CurrentDeck.Intersect(tokenDruid).Count(),
                    CurrentDeck.Intersect(tokenDruid2).Count(),
                };
                deckDictionary.AddOrUpdate(DeckType.TokenDruid, tkndru.Max()); //EGG 
                if (CurrentDeck.ContainsSome(Cards.AncientWatcher, Cards.EerieStatue, Cards.WailingSoul))
                {
                    deckDictionary.AddOrUpdate(DeckType.SilenceDruid, CurrentDeck.Intersect(silenceDruid).Count());
                }

                break;

                #endregion
            }
            var bestDeck = deckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            if (!Bot.CurrentMode().IsArena() && CurrentDeck.Count > 10 && deckDictionary[bestDeck] < 5)
            {
                Bot.Log("[Tracker] It appears that your opponent is playing something random");
            }
            try
            {
                info.DeckType = bestDeck;
                info.DeckStyle = DeckStyles[bestDeck];
                //Bot.Log(String.Format("{0}|||{1}", info.DeckType, info.DeckStyle));
            }
            catch (Exception e)
            {
                Bot.Log(String.Format("\n\n[WARNING] Arthur forgot to add PlayStyle for {0}. Feel free to go and make fun of him\n Error: {1} \n\n", bestDeck, e.Message));
            }
            //Bot.Log(info.DeckType +"||"+ info.DeckStyle);
            return info;
        }
        #endregion
        private void Log(string str, int location = 0)
        {
            string file = location != 0 ? "MatchHistory.txt" : "MidgameIdentificationLog.txt";
            if (IsFileLocked(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\" + file)) return;
            using (
                StreamWriter logfile =
                    new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\" + file, true))
            {
                logfile.WriteLine("[" + DateTime.Now + "] " + str);
            }
        }
        private static void Report(string str)
        {
            using (StreamWriter log = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\TrackerAULog.txt", true))
            {
                log.WriteLine("[{0}] {1}", DateTime.Now, str);
            }
        }

        private static void CheckDirectory(string subdir)
        {
            if (Directory.Exists(subdir))
                return;
            Directory.CreateDirectory(subdir);
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
    }


    public class DeckData
    {
        public DeckType DeckType { get; set; }
        public Style DeckStyle { get; set; }
        public List<Card.Cards> DeckList { get; set; }
    }

    public class CardStatistic
    {
        public Card.Cards MyCard { get; set; }
        public DeckType MyDeckType { get; set; }
        public int Played { get; set; }
        public int Drawn { get; set; }
       

        public CardStatistic(Card.Cards card, DeckType dt )
        {
            MyCard = card;
            MyDeckType = dt;
            Played = 0;
            Drawn = 0;
        }

        

    }
   

    public enum DeckType
    {
        Custom,
        Unknown,
        Arena,
        /*Warrior*/
        ControlWarrior,
        FatigueWarrior,
        DragonWarrior,
        PatronWarrior,
        WorgenOTKWarrior,
        MechWarrior,
        FaceWarrior,
        RenoWarrior,
        CThunWarrior,
        TempoWarrior,
        /*Paladin*/
        SecretPaladin,
        MidRangePaladin,
        DragonPaladin,
        AggroPaladin,
        AnyfinMurglMurgl,
        RenoPaladin,
        CThunPaladin,
        
        /*Druid*/
        RampDruid,
        AggroDruid,
        DragonDruid,
        MidRangeDruid,
        TokenDruid,
        SilenceDruid,
        MechDruid,
        AstralDruid,
        MillDruid,
        BeastDruid,
        RenoDruid,
        CThunDruid,
        /*Warlock*/
        Handlock,
        RenoLock,
        Zoolock,
        DemonHandlock,
        DragonHandlock,
        MalyLock,
        ControlWarlock,
        CThunLock,
        /*Mage*/
        TempoMage,
        FreezeMage,
        FaceFreezeMage,
        DragonMage,
        MechMage,
        EchoMage,
        RenoMage,
        CThunMage,
        /*Priest*/
        DragonPriest,
        ControlPriest,
        ComboPriest,
        MechPriest,
        
        /*Huntard*/
        MidRangeHunter,
        HybridHunter,
        FaceHunter,
        CamelHunter,
        RenoHunter,
        DragonHunter,
        CThunHunter,
        /*Rogue*/
        OilRogue,
        PirateRogue,
        FaceRogue,
        MalyRogue,
        RaptorRogue,
        MiracleRogue,
        MechRogue,
        RenoRogue,
        MillRogue,
        CThunRogue,
        /*Chaman*/
        FaceShaman,
        MechShaman,
        DragonShaman,
        MidrangeShaman,
        MalygosShaman,
        ControlShaman,
        
        BattleryShaman,
        RenoShaman,
        CThunShaman,

        Basic,
    }

    public enum Style
    {
        Unknown,
        Face,
        Aggro,
        Control,
        Midrange,
        Combo,
        Tempo,
        Fatigue,
    }

    public enum IdentityMode
    {
        Auto,
        Manual
    }
        
    public enum Locale
    {
        English, Russian
    }
    public enum Update
    {
        Hard, Soft
    }

}