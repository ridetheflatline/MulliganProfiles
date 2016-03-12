using SmartBot.Plugins.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using SmartBot.Database;

namespace SmartBot.Plugins
{
    public static class Extension
    {
        public static void AddOrUpdate<TKey, TValue>(
            this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }
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
        public static int QualityCount(this IList<Card.Cards> list, Card.CQuality qQuality)
        {
            return list.Count(cards => CardTemplate.LoadFromId(cards).Quality == qQuality);
        }
        public static bool IsRenoDeck (this IList<Card.Cards> list, int treshhold=10)
        {
            bool renoCheck = list.Count == list.Distinct().Count();
            return (renoCheck && list.Count >= treshhold) || list.Contains(Cards.RenoJackson);
        }
    }

    [Serializable]
    public class SmartTracker : PluginDataContainer
    {

        /// <summary>
        /// This variable is to add extra two option to SmartTracker that will allow you
        /// to use Mulligan Tester by Botfanatic
        /// </summary>
        private const bool MulliganTesterDebug = false;

        [DisplayName("[0] AU SmartMulligan")]
        public bool AutoUpdateV3 { get; private set; }
        [DisplayName("[1] SM version")]
        public double Mversion { get; private set; }
        [DisplayName("[2] AU Tracker")]
        public bool AutoUpdateTracker { get; set; }
        [DisplayName("[3] ST version")]
        public double Tversion { get; private set; }

        
        
        //[DisplayName("Random Intro Messages")]
        //public bool RandomMovieQuotes { get; private set; }
        [DisplayName("[4] ID Mode")]
        public IdentityMode Mode { get; set; }

        [DisplayName("[4] Manual -f Deck")]
        public DeckType ForcedDeckType { get; set; }
        [Browsable(MulliganTesterDebug ? true : false)]
        [DisplayName("Mulligan Tester: you")]
        public DeckType MulliganTesterYourDeck { get; set; }
        [Browsable(MulliganTesterDebug ? true : false)]
        [DisplayName("Mulligan Tester: enemy")]
        public DeckType MulliganTEsterEnemyDeck { get; set; }

        [DisplayName("Display Prediction")]
        public bool PredictionDisplay { get; set; }
        [DisplayName("Glossary")]
        public string Dictionary { get; private set; }

        [Browsable(false)]
        public string LSmartMulliganV3 { get; private set; }
        [Browsable(false)]
        public string LSmartTracker { get; private set; }

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

        public SmartTracker()
        {
            Name = "SmartTracker";
            ForcedDeckType = DeckType.Unknown;
            MulliganTEsterEnemyDeck = DeckType.Unknown;
            MulliganTesterYourDeck = DeckType.Unknown;
            AutoUpdateV3 = false;
            AutoUpdateTracker = false;
            AutoFriendlyDeckType = DeckType.Unknown;
            EnemyDeckTypeGuess = DeckType.Unknown;
            LSmartMulliganV3 = "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/MulliganProfiles/SmartMulliganV3/version.txt";
            LSmartTracker = "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/Plugins/SmartTracker/tracker.version";

        }

        public void ReloadDictionary()
        {
            Dictionary = "AU:\t\tAuto Update\nSM:\t\tSmart Mulligan\nST:\t\tSmart Tracker" +
                         "\nID Mode:\tTells Tracker your prefered way of identifying 'your' deck" +
                         "\nManual -f\tTell tracker the deck you are playing if you chose Manual ID mode" +
                         "\nDisplay Prediction:\tShows on the top left corner what tracker assumes your opponent is";
            AutoUpdateV3 = false;

        }
        public void VersionCheck()
        {
            try
            {
                using (
                    StreamReader Tversionl =
                        new StreamReader(AppDomain.CurrentDomain.BaseDirectory +
                                         "Plugins\\SmartTracker\\tracker.version"))
                using (
                    StreamReader Mversionl =
                        new StreamReader(AppDomain.CurrentDomain.BaseDirectory +
                                         "MulliganProfiles\\SmartMulliganV3\\version.txt"))

                {
                    Tversion = double.Parse(Tversionl.ReadLine());
                    Mversion = double.Parse(Mversionl.ReadLine());
                }
            }
            catch (Exception e)
            {
                Bot.Log("[Version Update Failed]" + e.Message);
            }
        }
    }

    public class SmTracker : Plugin
    {
        public bool identified = false;
        private DeckData informationData;
        private readonly string MulliganDir = AppDomain.CurrentDomain.BaseDirectory + "MulliganProfiles\\";
        private readonly string MulliganInformation = AppDomain.CurrentDomain.BaseDirectory + "MulliganProfiles\\SmartMulliganV3\\";
        private readonly string TrackerDir = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\";
        private readonly string TrackerVersion = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\SmartTracker\\";
        private int _screenWidth;
        private int _screenHeight;
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
        public override void OnTick()
        {
            GUI.ClearUI();
            if(((SmartTracker)DataContainer).PredictionDisplay)
            GUI.AddElement(new GuiElementText("Prediction: " + ((SmartTracker)DataContainer).EnemyDeckTypeGuess
                 + "|" + ((SmartTracker)DataContainer).EnemyDeckStyleGuess, (_screenWidth) / 64, PercToPixHeight(40), 155,
                30,
                16, 255, 215, 0));
            if (!_started) return;
            
            if (Bot.CurrentScene() == Bot.Scene.GAMEPLAY)
            {
                IdentifyMyStuff();
                CheckHistory();
            }
            if (Bot.CurrentScene() != Bot.Scene.GAMEPLAY)
            {
                identified = false;
            }
        }

        public override void OnGameEnd()
        {
            base.OnGameEnd();
            ((SmartTracker)DataContainer).EnemyDeckTypeGuess = DeckType.Unknown;
            Log("[SmartTracker_debug] Resetting Guess");
        }

        public override void OnGameBegin()
        {
            IdentifyMyStuff();
            CheckHistory();
        }

        private void CheckHistory()
        {
            if (Bot.CurrentBoard == null) return;
            Bot.Log("[SmartTracker] Looing up your opponent");
            bool foundCurr = false;
            using (
                StreamReader historyReader =
                    new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\SmartTracker\\MatchHistory.txt"))
            {
                if(historyReader.ReadLine().Contains("something"))
                    foundCurr = true;
            }
            //DEFAULTS
            if (foundCurr) return;
            switch (Bot.CurrentBoard.EnemyClass)
            {
                case Card.CClass.SHAMAN:
                    ((SmartTracker)DataContainer).EnemyDeckTypeGuess = DeckType.FaceShaman;
                    ((SmartTracker)DataContainer).EnemyDeckStyleGuess = DeckStyles[((SmartTracker)DataContainer).EnemyDeckTypeGuess];
                    break;
                case Card.CClass.PRIEST:
                    ((SmartTracker)DataContainer).EnemyDeckTypeGuess = DeckType.ControlPriest;
                    ((SmartTracker)DataContainer).EnemyDeckStyleGuess = DeckStyles[((SmartTracker)DataContainer).EnemyDeckTypeGuess];
                    break;
                case Card.CClass.MAGE:
                    ((SmartTracker)DataContainer).EnemyDeckTypeGuess = DeckType.FreezeMage;
                    ((SmartTracker)DataContainer).EnemyDeckStyleGuess = DeckStyles[((SmartTracker)DataContainer).EnemyDeckTypeGuess];
                    break;
                case Card.CClass.PALADIN:
                    ((SmartTracker)DataContainer).EnemyDeckTypeGuess = DeckType.SecretPaladin;
                    ((SmartTracker)DataContainer).EnemyDeckStyleGuess = DeckStyles[((SmartTracker)DataContainer).EnemyDeckTypeGuess];
                    break;
                case Card.CClass.WARRIOR:
                    ((SmartTracker)DataContainer).EnemyDeckTypeGuess = DeckType.ControlWarrior;
                    ((SmartTracker)DataContainer).EnemyDeckStyleGuess = DeckStyles[((SmartTracker)DataContainer).EnemyDeckTypeGuess];
                    break;
                case Card.CClass.WARLOCK:
                    ((SmartTracker)DataContainer).EnemyDeckTypeGuess = DeckType.Zoolock;
                    ((SmartTracker)DataContainer).EnemyDeckStyleGuess = DeckStyles[((SmartTracker)DataContainer).EnemyDeckTypeGuess];
                    break;
                case Card.CClass.HUNTER:
                    ((SmartTracker)DataContainer).EnemyDeckTypeGuess = DeckType.MidRangeHunter;
                    ((SmartTracker)DataContainer).EnemyDeckStyleGuess = DeckStyles[((SmartTracker)DataContainer).EnemyDeckTypeGuess];
                    break;
                case Card.CClass.ROGUE:
                    ((SmartTracker)DataContainer).EnemyDeckTypeGuess = DeckType.OilRogue;
                    ((SmartTracker)DataContainer).EnemyDeckStyleGuess = DeckStyles[((SmartTracker)DataContainer).EnemyDeckTypeGuess];
                    break;
                case Card.CClass.DRUID:
                    ((SmartTracker)DataContainer).EnemyDeckTypeGuess = DeckType.MidRangeDruid;
                    ((SmartTracker)DataContainer).EnemyDeckStyleGuess = DeckStyles[((SmartTracker)DataContainer).EnemyDeckTypeGuess];
                    break;
               
            }
            //((SmartTracker) DataContainer).EnemyDeckTypeGuess = DeckType.Unknown;
        }

        public void IdentifyMyStuff()
        {
            if (Bot.CurrentBoard == null || identified) return;

            informationData = GetDeckInfo(Bot.CurrentDeck().Class, Bot.CurrentDeck().Cards);
            ((SmartTracker) DataContainer).AutoFriendlyDeckType = informationData.DeckType;
            ((SmartTracker) DataContainer).AutoFriendlyStyle = DeckStyles[informationData.DeckType];
            if (((SmartTracker) DataContainer).Mode == IdentityMode.Manual)
            {
                DeckType tempType = informationData.DeckType;
                informationData.DeckType = ((SmartTracker) DataContainer).ForcedDeckType;
                informationData.DeckStyle = DeckStyles[((SmartTracker) DataContainer).ForcedDeckType];
                Bot.Log(string.Format("[SmartTracker] You are forcing SmartMulliganV3 to treat your deck as {0}, {1}," + "\n\t\t[Debug] Tracker would have recognized it as {2}, {3}", informationData.DeckType, informationData.DeckStyle, tempType, DeckStyles[tempType]));
            }

            if (((SmartTracker) DataContainer).Mode == IdentityMode.Auto)
                Bot.Log(string.Format("Succesfully Identified deck\n{0}|{1}|", informationData.DeckType, informationData.DeckStyle));
            identified = true;
        }


        public override void OnPluginCreated()
        {
            CheckFiles();
            ((SmartTracker) DataContainer).VersionCheck();
            ((SmartTracker) DataContainer).ReloadDictionary();
            ((SmartTracker) DataContainer).SynchEnums = Enum.GetNames(typeof (DeckType)).Length;
            CheckDirectory(MulliganInformation);
            CheckDirectory(TrackerVersion);
            CheckDirectory(AppDomain.CurrentDomain.BaseDirectory + "Logs\\SmartTracker\\");
            
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
        }

        private static bool _started = false;

        public override void OnStarted()
        {
            _started = true;
            if (Bot.CurrentScene() == Bot.Scene.GAMEPLAY)
            {
                Bot.Log((Bot.CurrentBoard == null) + " " + identified);
                IdentifyMyStuff();
            }

            if (((SmartTracker) DataContainer).AutoUpdateV3)
            {
                CheckUpdatesMulligan(((SmartTracker) DataContainer).LSmartMulliganV3);
                ((SmartTracker) DataContainer).VersionCheck();
            }
            if (((SmartTracker) DataContainer).AutoUpdateTracker)
            {
                CheckUpdatesTracker(((SmartTracker) DataContainer).LSmartTracker);
                ((SmartTracker) DataContainer).VersionCheck();
            }
        }


        private void CheckUpdatesTracker(string lSmartTracker)
        {
            HttpWebRequest request = WebRequest.Create(lSmartTracker) as HttpWebRequest;
            if (request == null)
            {
                Bot.Log(string.Format("[SmartAutoUpdater] Could not get data from gitlink {0}", lSmartTracker));
                return;
            }
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (StreamReader str = new StreamReader(response.GetResponseStream()))
            using (StreamReader localVersion = new StreamReader(TrackerVersion + "tracker.version"))

            {
                string strline = str.ReadLine();
                double remoteVer = double.Parse(strline);
                double localVer = double.Parse(localVersion.ReadLine());

                if (localVer == remoteVer) Bot.Log("[SmartTracker] SmartTracker is up to date");
                if (localVer > remoteVer)
                {
                    Bot.Log(string.Format("[SmartTracker] Local Version: {0} Remote Version {1}", localVer, remoteVer));
                    Bot.Log("[SmartTracker] Arthur, you are an idiot. Push new update");
                }
                if (localVer < remoteVer)
                {
                    localVersion.Close();
                    UpdateTracker(lSmartTracker, remoteVer, localVer);
                }
            }
        }

        private void UpdateTracker(string lSmartTracker, double remoteVer, double localVer)
        {
            Bot.Log(string.Format("[SmartTracker] Local Version: {0} Remote Version {1}\n\t\tUpdating...", localVer, remoteVer));
            HttpWebRequest trackeRequest = WebRequest.Create("https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/Plugins/SmartTracker.cs") as HttpWebRequest;
            if (trackeRequest == null)
            {
                Bot.Log(string.Format("[SmartAutoUpdater] Could not get data from gitlink {0}", lSmartTracker));
                return;
            }
            using (HttpWebResponse mulResponse = trackeRequest.GetResponse() as HttpWebResponse)
            using (StreamReader trFile = new StreamReader(mulResponse.GetResponseStream()))
            using (StreamWriter updateLocalCopy = new StreamWriter(TrackerDir + "SmartTracker.cs"))
            {
                string tempfile = trFile.ReadToEnd();
                //Bot.Log("[IGOT HERE]");
                updateLocalCopy.WriteLine(tempfile);
                Bot.RefreshMulliganProfiles();
                Bot.Log("[SmartTracker] SmartTracker is now fully updated");
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
                if (localVer == remoteVer) Bot.Log("[SmartTracker] SmartMulliganV3 is up to date");
                if (localVer > remoteVer)
                {
                    Bot.Log(string.Format("[SmartTracker] Local Version: {0} Remote Version {1}", localVer, remoteVer));
                    Bot.Log("[SmartTracker] Arthur, you are an idiot. Push new update");
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
            Bot.Log(string.Format("[SmartTracker] Local Version: {0} Remote Version {1}\n\t\tUpdating...", localVer, remoteVer));
            HttpWebRequest MulliganRequest = WebRequest.Create("https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/MulliganProfiles/SmartMulliganV3.cs") as HttpWebRequest;
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
                Bot.Log("[SmartTracker] SmartMulligan is now fully updated");
                UpdateVersion(remoteVer);
            }
        }


        private void UpdateVersion(double remoteVer, bool value = false)
        {
            using (StreamWriter localVersion = new StreamWriter(value ? TrackerVersion + "tracker.version" : MulliganInformation + "version.txt", false))
            {
                localVersion.WriteLine(remoteVer);
            }
        }


        public override void OnStopped()
        {
            identified = false;
            _started = false;
        }

        public override void OnTurnBegin()
        {
            base.OnTurnBegin();
            CheckOpponentDeck();
        }

        public void CheckOpponentDeck()
        {
            List<Card.Cards> graveyard = Bot.CurrentBoard.EnemyGraveyard.ToList();
            List<Card> board = Bot.CurrentBoard.MinionEnemy.ToList();
            List<string> opponentDeck = new List<string> {};
            opponentDeck.AddRange(graveyard.Select(q => q.ToString()));
            opponentDeck.AddRange(board.Select(q => q.Template.Id.ToString()));
            DeckData opponentInfo = GetDeckInfo(Bot.CurrentBoard.EnemyClass, opponentDeck);
            if (((SmartTracker) DataContainer).EnemyDeckTypeGuess == opponentInfo.DeckType)
            {
                Bot.Log("[SmartTracker_debug] Your opponent is playing " + opponentInfo.DeckType + ":" + opponentInfo.DeckStyle);
                return;
            }
            Log(string.Format("[SmartTracker_debug] New signature detected in your opponent decks {0} => {1}", ((SmartTracker) DataContainer).EnemyDeckTypeGuess, opponentInfo.DeckType));
            ((SmartTracker) DataContainer).EnemyDeckTypeGuess = opponentInfo.DeckType;
            ((SmartTracker) DataContainer).EnemyDeckStyleGuess = DeckStyles[opponentInfo.DeckType];
        }

        public void CheckOpponentDeck(string res)
        {
            List<Card.Cards> graveyard = Bot.CurrentBoard.EnemyGraveyard.ToList();
            List<Card> board = Bot.CurrentBoard.MinionEnemy.ToList();
            List<string> opponentDeck = new List<string> {};
            opponentDeck.AddRange(graveyard.Select(q => q.ToString()));
            opponentDeck.AddRange(board.Select(q => q.Template.Id.ToString()));
            var opDeck = opponentDeck.Where(card => CardTemplate.LoadFromId(card).IsCollectible);
            string str = opDeck.Aggregate("", (current, q) => current + ("Cards." + CardTemplate.LoadFromId(q).Name.Replace(" ", "") + ", "));
            using (StreamWriter opponentDeckInfo = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\SmartTracker\\MatchHistory.txt", true))
            {
                DeckData opponentInfo = GetDeckInfo(Bot.CurrentBoard.EnemyClass, opponentDeck, Bot.CurrentBoard.SecretEnemyCount);
                opponentDeckInfo.WriteLine("{0}||{1}||{2}||{3}||{4}||{5}", DateTime.UtcNow, res, Bot.GetCurrentOpponentId(), opponentInfo.DeckType, opponentInfo.DeckStyle, str);
                Bot.Log(string.Format("[Tracker] Succesfully recorded your opponent: {0}", opponentInfo.DeckType));
            }
        }

        public override void OnDefeat()
        {
            try
            {
                CheckOpponentDeck("lost");
            }
            catch (Exception e)
            {
                Bot.Log("Something happened that wasn't intended" + e.Message);
            }
        }

        public override void OnVictory()
        {
            try
            {
                CheckOpponentDeck("won");
            }
            catch (Exception e)
            {
                Bot.Log("Something happened that wasn't intended" + e.Message);
            }
        }

        public readonly Dictionary<DeckType, Style> DeckStyles = new Dictionary<DeckType, Style>
        {
            {DeckType.Unknown, Style.Unknown}, {DeckType.Arena, Style.Control}, /*Warrior*/
            {DeckType.ControlWarrior, Style.Control}, {DeckType.FatigueWarrior, Style.Fatigue}, {DeckType.DragonWarrior, Style.Control}, {DeckType.PatronWarrior, Style.Tempo}, {DeckType.WorgenOTKWarrior, Style.Combo}, {DeckType.MechWarrior, Style.Aggro}, {DeckType.FaceWarrior, Style.Face}, /*Paladin*/
            {DeckType.SecretPaladin, Style.Tempo}, {DeckType.MidRangePaladin, Style.Control}, {DeckType.DragonPaladin, Style.Control}, {DeckType.AggroPaladin, Style.Aggro}, {DeckType.AnyfinMurglMurgl, Style.Combo}, {DeckType.RenoPaladin, Style.Control}, /*Druid*/
            {DeckType.RampDruid, Style.Control}, {DeckType.AggroDruid, Style.Aggro}, {DeckType.DragonDruid, Style.Control}, {DeckType.MidRangeDruid, Style.Combo}, {DeckType.TokenDruid, Style.Tempo}, {DeckType.SilenceDruid, Style.Control}, {DeckType.MechDruid, Style.Aggro}, {DeckType.AstralDruid, Style.Control}, {DeckType.MillDruid, Style.Fatigue}, {DeckType.BeastDruid, Style.Tempo}, {DeckType.RenoDruid, Style.Control}, /*Warlock*/
            {DeckType.Handlock, Style.Control}, {DeckType.RenoLock, Style.Control}, {DeckType.Zoolock, Style.Tempo}, //Same handler as flood zoo and reliquary
            {DeckType.DemonHandlock, Style.Control}, {DeckType.DemonZooWarlock, Style.Tempo}, {DeckType.DragonHandlock, Style.Control}, {DeckType.MalyLock, Style.Combo}, {DeckType.RenoComboLock, Style.Combo}, {DeckType.ControlWarlock, Style.Control}, /*Mage*/
            {DeckType.TempoMage, Style.Tempo}, {DeckType.FreezeMage, Style.Control}, {DeckType.FaceFreezeMage, Style.Aggro}, {DeckType.DragonMage, Style.Control}, {DeckType.MechMage, Style.Aggro}, {DeckType.EchoMage, Style.Control}, {DeckType.FatigueMage, Style.Fatigue}, {DeckType.RenoMage, Style.Control}, /*Priest*/
            {DeckType.DragonPriest, Style.Tempo}, {DeckType.ControlPriest, Style.Control}, {DeckType.ComboPriest, Style.Combo}, {DeckType.MechPriest, Style.Aggro}, {DeckType.ShadowPriest, Style.Combo}, /*Hunter*/
            {DeckType.MidRangeHunter, Style.Tempo}, {DeckType.HybridHunter, Style.Aggro}, {DeckType.FaceHunter, Style.Face}, {DeckType.HatHunter, Style.Control}, {DeckType.CamelHunter, Style.Control}, {DeckType.DragonHunter, Style.Control}, {DeckType.RenoHunter, Style.Control}, /*Rogue*/
            {DeckType.OilRogue, Style.Combo}, {DeckType.PirateRogue, Style.Aggro}, {DeckType.FaceRogue, Style.Face}, {DeckType.MalyRogue, Style.Combo}, {DeckType.RaptorRogue, Style.Tempo}, {DeckType.FatigueRogue, Style.Combo}, {DeckType.MiracleRogue, Style.Combo}, {DeckType.RenoRogue, Style.Control}, {DeckType.MechRogue, Style.Tempo}, {DeckType.MillRogue, Style.Fatigue}, /*Cance... I mean Shaman*/
            {DeckType.FaceShaman, Style.Face}, {DeckType.MechShaman, Style.Aggro}, {DeckType.DragonShaman, Style.Control}, {DeckType.TotemShaman, Style.Tempo}, {DeckType.MalygosShaman, Style.Combo}, {DeckType.ControlShaman, Style.Control}, {DeckType.BloodlustShaman, Style.Combo}, {DeckType.RenoShaman, Style.Combo}, {DeckType.BattleryShaman, Style.Control}, /*Poor Kids*/
            {DeckType.Basic, Style.Tempo}
        };

        public DeckData GetDeckInfo(Card.CClass ownClass, List<string> curDeck, int activeSecrets = 0)
        {
            List<Card.Cards> CurrentDeck = curDeck.Select(q => (Card.Cards) Enum.Parse(typeof (Card.Cards), q)).ToList().Where(card => CardTemplate.LoadFromId(card).IsCollectible).ToList();
            var info = new DeckData {DeckList = CurrentDeck, DeckType = DeckType.Unknown, DeckStyle = DeckStyles[DeckType.Unknown]};
            if (CurrentDeck.Count == 0) return info;
            string str = CurrentDeck.Aggregate("", (current, q) => current + ("Cards." + CardTemplate.LoadFromId(q).Name.Replace(" ", "") + ", "));
            Log("[SmartTracker_debug] " + str);

            Dictionary<DeckType, int> deckDictionary = new Dictionary<DeckType, int>();

            switch (ownClass)
            {
                    #region shaman

                case Card.CClass.SHAMAN:
                    if (CurrentDeck.IsRenoDeck())
                    {
                        List<Card.Cards> renoShaman = new List<Card.Cards> {Cards.LightningBolt, Cards.SylvanasWindrunner, Cards.EarthShock, Cards.FarSight, Cards.StormforgedAxe, Cards.Doomhammer, Cards.FeralSpirit, Cards.Hex, Cards.AzureDrake, Cards.AlAkirtheWindlord, Cards.RockbiterWeapon, Cards.RagnarostheFirelord, Cards.BloodmageThalnos, Cards.DefenderofArgus, Cards.ManaTideTotem, Cards.FireElemental, Cards.LightningStorm, Cards.Crackle, Cards.LavaShock, Cards.EmperorThaurissan, Cards.TotemGolem, Cards.TuskarrTotemic, Cards.AncestralKnowledge, Cards.HealingWave, Cards.JeweledScarab, Cards.SirFinleyMrrgglton, Cards.TombSpider, Cards.RenoJackson, Cards.TunnelTrogg, Cards.ArchThiefRafaam,};
                        deckDictionary.AddOrUpdate(DeckType.RenoShaman, CurrentDeck.Intersect(renoShaman).Count());
                    }
                    List<Card.Cards> FaceShaman = new List<Card.Cards> {Cards.LightningBolt, Cards.LightningBolt, Cards.UnboundElemental, Cards.UnboundElemental, Cards.EarthShock, Cards.StormforgedAxe, Cards.Doomhammer, Cards.Doomhammer, Cards.FeralSpirit, Cards.FeralSpirit, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.LeperGnome, Cards.LeperGnome, Cards.AbusiveSergeant, Cards.LavaBurst, Cards.LavaBurst, Cards.Crackle, Cards.Crackle, Cards.LavaShock, Cards.LavaShock, Cards.TotemGolem, Cards.TotemGolem, Cards.ArgentHorserider, Cards.ArgentHorserider, Cards.AncestralKnowledge, Cards.AncestralKnowledge, Cards.SirFinleyMrrgglton, Cards.TunnelTrogg, Cards.TunnelTrogg,};
                    if (CurrentDeck.RaceCount(Card.CRace.MECH) > 2)
                    {
                        List<Card.Cards> MechShaman = new List<Card.Cards> {Cards.LightningBolt, Cards.LightningBolt, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.TunnelTrogg, Cards.TunnelTrogg, Cards.Crackle, Cards.Crackle, Cards.TotemGolem, Cards.TotemGolem, Cards.WhirlingZapomatic, Cards.WhirlingZapomatic, Cards.Powermace, Cards.Powermace, Cards.LavaBurst, Cards.LavaBurst, Cards.UnboundElemental, Cards.UnboundElemental, Cards.Doomhammer, Cards.Doomhammer, Cards.Cogmaster, Cards.Cogmaster, Cards.LeperGnome, Cards.SirFinleyMrrgglton, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.Mechwarper, Cards.Mechwarper, Cards.SpiderTank, Cards.SpiderTank,};
                        deckDictionary.AddOrUpdate(DeckType.MechShaman, CurrentDeck.Intersect(MechShaman).Count());
                    }
                    List<Card.Cards> TotemShaman = new List<Card.Cards> {Cards.EarthShock, Cards.Bloodlust, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.AlAkirtheWindlord, Cards.FlametongueTotem, Cards.FlametongueTotem, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.DefenderofArgus, Cards.ManaTideTotem, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.LightningStorm, Cards.ZombieChow, Cards.ZombieChow, Cards.TotemGolem, Cards.TotemGolem, Cards.TuskarrTotemic, Cards.TuskarrTotemic, Cards.ThunderBluffValiant, Cards.ThunderBluffValiant, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.HauntedCreeper, Cards.HauntedCreeper,};
                    if (CurrentDeck.Contains(Cards.Malygos))
                    {
                        List<Card.Cards> MalygosShaman = new List<Card.Cards> {Cards.LightningBolt, Cards.LightningBolt, Cards.EarthShock, Cards.EarthShock, Cards.FarSight, Cards.FarSight, Cards.StormforgedAxe, Cards.FeralSpirit, Cards.FeralSpirit, Cards.FrostShock, Cards.FrostShock, Cards.Malygos, Cards.GnomishInventor, Cards.GnomishInventor, Cards.Crackle, Cards.Crackle, Cards.Hex, Cards.Hex, Cards.LavaBurst, Cards.LavaBurst, Cards.ManaTideTotem, Cards.ManaTideTotem, Cards.LightningStorm, Cards.LightningStorm, Cards.AncestorsCall, Cards.AncestorsCall, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.Alexstrasza, Cards.AzureDrake,};
                        deckDictionary.AddOrUpdate(DeckType.MalygosShaman, CurrentDeck.Intersect(MalygosShaman).Count());
                    }

                    List<Card.Cards> BasicChaman = new List<Card.Cards> {Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.FlametongueTotem, Cards.FlametongueTotem, Cards.Hex, Cards.Hex, Cards.Bloodlust, Cards.FireElemental, Cards.FireElemental, Cards.AcidicSwampOoze, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.MurlocTidehunter, Cards.RazorfenHunter, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.GnomishInventor, Cards.SenjinShieldmasta, Cards.SenjinShieldmasta, Cards.FrostwolfWarlord, Cards.FrostwolfWarlord, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion, Cards.StormwindChampion,};
                    deckDictionary.AddOrUpdate(DeckType.FaceShaman, CurrentDeck.Intersect(FaceShaman).Count());
                    if (CurrentDeck.ContainsSome(Cards.Malygos, Cards.TwilightGuardian, Cards.AzureDrake))
                    {
                        List<Card.Cards> DragonShaman = new List<Card.Cards> {Cards.EarthShock, Cards.FeralSpirit, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.Deathwing, Cards.Ysera, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.LightningStorm, Cards.BlackwingTechnician, Cards.BlackwingTechnician, Cards.LavaShock, Cards.BlackwingCorruptor, Cards.TotemGolem, Cards.TotemGolem, Cards.AncestralKnowledge, Cards.HealingWave, Cards.HealingWave, Cards.TheMistcaller, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.Chillmaw, Cards.JeweledScarab, Cards.JeweledScarab, Cards.BrannBronzebeard, Cards.TunnelTrogg, Cards.TunnelTrogg,};
                        deckDictionary.AddOrUpdate(DeckType.DragonShaman, CurrentDeck.Intersect(DragonShaman).Count());
                    }
                    deckDictionary.AddOrUpdate(DeckType.TotemShaman, CurrentDeck.Intersect(TotemShaman).Count());
                    if (CurrentDeck.ContainsSome(Cards.Doomsayer, Cards.BigGameHunter, Cards.ElementalDestruction, Cards.HealingWave, Cards.LightningStorm, Cards.JeweledScarab))
                    {
                        List<Card.Cards> ControlShaman = new List<Card.Cards> {Cards.BigGameHunter, Cards.FeralSpirit, Cards.FeralSpirit, Cards.Bloodlust, Cards.Bloodlust, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.FlametongueTotem, Cards.FlametongueTotem, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.DefenderofArgus, Cards.DefenderofArgus, Cards.AbusiveSergeant, Cards.ManaTideTotem, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.LightningStorm, Cards.ZombieChow, Cards.ZombieChow, Cards.NerubianEgg, Cards.NerubianEgg, Cards.Loatheb, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.BrannBronzebeard,};
                        List<Card.Cards> ControlShaman2 = new List<Card.Cards> {Cards.BigGameHunter, Cards.EarthShock, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.Doomsayer, Cards.Doomsayer, Cards.Ysera, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.LightningStorm, Cards.Loatheb, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.DrBoom, Cards.Neptulon, Cards.LavaShock, Cards.LavaShock, Cards.VolcanicDrake, Cards.VolcanicDrake, Cards.HealingWave, Cards.HealingWave, Cards.ElementalDestruction, Cards.ElementalDestruction, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.JeweledScarab, Cards.JeweledScarab,};
                        deckDictionary.AddOrUpdate(DeckType.ControlShaman, CurrentDeck.Intersect(ControlShaman).Count());
                        deckDictionary.AddOrUpdate(DeckType.ControlShaman, CurrentDeck.Intersect(ControlShaman2).Count());
                    }

                    if (CurrentDeck.Contains(Cards.Bloodlust))
                    {
                        List<Card.Cards> BloodlustShaman = new List<Card.Cards> {Cards.BigGameHunter, Cards.AcidicSwampOoze, Cards.EarthShock, Cards.FeralSpirit, Cards.FeralSpirit, Cards.Bloodlust, Cards.Bloodlust, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.FlametongueTotem, Cards.FlametongueTotem, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.DefenderofArgus, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.LightningStorm, Cards.ZombieChow, Cards.ZombieChow, Cards.NerubianEgg, Cards.NerubianEgg, Cards.Loatheb, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.PilotedShredder, Cards.TuskarrTotemic,};
                        deckDictionary.AddOrUpdate(DeckType.BloodlustShaman, CurrentDeck.Intersect(BloodlustShaman).Count());
                    }
                    if (CurrentDeck.Contains(Cards.RumblingElemental))
                    {
                        List<Card.Cards> battlecryShaman = new List<Card.Cards> {Cards.LightningBolt, Cards.LightningBolt, Cards.StormforgedAxe, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.DefenderofArgus, Cards.DefenderofArgus, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.ZombieChow, Cards.ZombieChow, Cards.Loatheb, Cards.DrBoom, Cards.TotemGolem, Cards.TotemGolem, Cards.TuskarrTotemic, Cards.TuskarrTotemic, Cards.JusticarTrueheart, Cards.JeweledScarab, Cards.JeweledScarab, Cards.BrannBronzebeard, Cards.RumblingElemental, Cards.RumblingElemental, Cards.TunnelTrogg, Cards.TunnelTrogg,};
                        deckDictionary.AddOrUpdate(DeckType.BloodlustShaman, CurrentDeck.Intersect(battlecryShaman).Count());
                    }

                    deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(BasicChaman).Count());

                    break;

                    #endregion

                    #region priest

                case Card.CClass.PRIEST:
                    if (CurrentDeck.ContainsSome(Cards.WyrmrestAgent, Cards.TwilightWhelp, Cards.TwilightGuardian, Cards.BlackwingCorruptor, Cards.BlackwingTechnician))
                    {
                        info.DeckType = DeckType.DragonPriest;
                        info.DeckStyle = DeckStyles[DeckType.DragonPriest];
                        return info;
                        //List<Card.Cards> dragonPriest = new List<Card.Cards> { Cards.CabalShadowPriest, Cards.CabalShadowPriest, Cards.AzureDrake, Cards.AzureDrake, Cards.PowerWordShield, Cards.PowerWordShield, Cards.Ysera, Cards.ShadowWordDeath, Cards.ShadowWordDeath, Cards.NorthshireCleric, Cards.NorthshireCleric, Cards.HolyNova, Cards.HolyNova, Cards.VelensChosen, Cards.Shrinkmeister, Cards.Shrinkmeister, Cards.Lightbomb, Cards.BlackwingTechnician, Cards.BlackwingTechnician, Cards.RendBlackhand, Cards.BlackwingCorruptor, Cards.BlackwingCorruptor, Cards.TwilightWhelp, Cards.TwilightWhelp, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.Chillmaw, Cards.WyrmrestAgent, Cards.WyrmrestAgent, Cards.BrannBronzebeard, };
                        //deckDictionary.AddOrUpdate(DeckType.DragonPriest, CurrentDeck.Intersect(dragonPriest).Count());
                    }
                    List<Card.Cards> ContrlPriest = new List<Card.Cards> {Cards.WildPyromancer, Cards.WildPyromancer, Cards.CircleofHealing, Cards.CircleofHealing, Cards.Thoughtsteal, Cards.CabalShadowPriest, Cards.CabalShadowPriest, Cards.InjuredBlademaster, Cards.InjuredBlademaster, Cards.PowerWordShield, Cards.PowerWordShield, Cards.ShadowWordDeath, Cards.NorthshireCleric, Cards.NorthshireCleric, Cards.AuchenaiSoulpriest, Cards.AuchenaiSoulpriest, Cards.HolyNova, Cards.ZombieChow, Cards.ZombieChow, Cards.Deathlord, Cards.Deathlord, Cards.LightoftheNaaru, Cards.LightoftheNaaru, Cards.Lightbomb, Cards.Lightbomb, Cards.JusticarTrueheart, Cards.EliseStarseeker, Cards.Entomb, Cards.Entomb, Cards.MuseumCurator,};
                    deckDictionary.AddOrUpdate(DeckType.ControlPriest, CurrentDeck.Intersect(ContrlPriest).Count());
                    if (CurrentDeck.ContainsSome(Cards.InnerFire, Cards.ProphetVelen))
                    {
                        List<Card.Cards> ComboPriest = new List<Card.Cards> {Cards.WildPyromancer, Cards.WildPyromancer, Cards.ProphetVelen, Cards.Malygos, Cards.AzureDrake, Cards.ShadowWordPain, Cards.LootHoarder, Cards.LootHoarder, Cards.HolySmite, Cards.HolySmite, Cards.MindBlast, Cards.MindBlast, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.PowerWordShield, Cards.PowerWordShield, Cards.HolyFire, Cards.HolyFire, Cards.BloodmageThalnos, Cards.ShadowWordDeath, Cards.NorthshireCleric, Cards.NorthshireCleric, Cards.HarrisonJones, Cards.HolyNova, Cards.HolyNova, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.VelensChosen, Cards.VelensChosen, Cards.EmperorThaurissan,};
                        deckDictionary.AddOrUpdate(DeckType.ComboPriest, CurrentDeck.Intersect(ComboPriest).Count());
                    }
                    if (CurrentDeck.RaceCount(Card.CRace.MECH) > 2)
                    {
                        List<Card.Cards> MechPriest = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.CabalShadowPriest, Cards.CabalShadowPriest, Cards.ShadowWordPain, Cards.PowerWordShield, Cards.PowerWordShield, Cards.ShadowMadness, Cards.CairneBloodhoof, Cards.NorthshireCleric, Cards.NorthshireCleric, Cards.HolyNova, Cards.Shrinkmeister, Cards.Shrinkmeister, Cards.VelensChosen, Cards.VelensChosen, Cards.DarkCultist, Cards.DarkCultist, Cards.UpgradedRepairBot, Cards.UpgradedRepairBot, Cards.Voljin, Cards.Mechwarper, Cards.Mechwarper, Cards.SpiderTank, Cards.SpiderTank, Cards.MechanicalYeti, Cards.MechanicalYeti, Cards.PilotedShredder, Cards.PilotedShredder, Cards.Loatheb, Cards.TroggzortheEarthinator,};
                        deckDictionary.AddOrUpdate(DeckType.MechPriest, CurrentDeck.Intersect(MechPriest).Count());
                    }

                    if (CurrentDeck.Contains(Cards.Shadowform))
                    {
                        List<Card.Cards> ShadowPriest = new List<Card.Cards> {Cards.WildPyromancer, Cards.WildPyromancer, Cards.Thoughtsteal, Cards.CabalShadowPriest, Cards.CabalShadowPriest, Cards.ProphetVelen, Cards.Alexstrasza, Cards.SenjinShieldmasta, Cards.SenjinShieldmasta, Cards.HolySmite, Cards.MindBlast, Cards.MindBlast, Cards.Shadowform, Cards.Shadowform, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.PowerWordShield, Cards.PowerWordShield, Cards.ShadowMadness, Cards.HolyFire, Cards.ShadowWordDeath, Cards.HolyNova, Cards.ZombieChow, Cards.Deathlord, Cards.Deathlord, Cards.Voljin, Cards.Lightbomb, Cards.Lightbomb, Cards.EmperorThaurissan, Cards.Entomb,};
                        deckDictionary.AddOrUpdate(DeckType.ShadowPriest, CurrentDeck.Intersect(ShadowPriest).Count());
                    }
                    List<Card.Cards> BasicPriest = new List<Card.Cards> {Cards.HolySmite, Cards.HolySmite, Cards.PowerWordShield, Cards.PowerWordShield, Cards.NorthshireCleric, Cards.NorthshireCleric, Cards.DivineSpirit, Cards.ShadowWordPain, Cards.ShadowWordPain, Cards.ShadowWordDeath, Cards.HolyNova, Cards.HolyNova, Cards.MindControl, Cards.VoodooDoctor, Cards.AcidicSwampOoze, Cards.RiverCrocolisk, Cards.RiverCrocolisk, Cards.IronfurGrizzly, Cards.IronfurGrizzly, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.DarkscaleHealer, Cards.DarkscaleHealer, Cards.GurubashiBerserker, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion,};
                    deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(BasicPriest).Count());

                    break;

                    #endregion

                    #region mage

                case Card.CClass.MAGE:
                    if (CurrentDeck.IsRenoDeck())
                    {
                        List<Card.Cards> renoMage = new List<Card.Cards> {Cards.IceBlock, Cards.SylvanasWindrunner, Cards.Flamestrike, Cards.FrostNova, Cards.BigGameHunter, Cards.MoltenGiant, Cards.Frostbolt, Cards.YouthfulBrewmaster, Cards.Blizzard, Cards.SunfuryProtector, Cards.AcolyteofPain, Cards.Doomsayer, Cards.ArcaneIntellect, Cards.IronbeakOwl, Cards.Fireball, Cards.EarthenRingFarseer, Cards.Polymorph, Cards.ZombieChow, Cards.Duplicate, Cards.Loatheb, Cards.SludgeBelcher, Cards.Deathlord, Cards.ExplosiveSheep, Cards.DrBoom, Cards.AntiqueHealbot, Cards.EchoofMedivh, Cards.EmperorThaurissan, Cards.ForgottenTorch, Cards.RenoJackson, Cards.EtherealConjurer,};
                        deckDictionary.AddOrUpdate(DeckType.RenoMage, CurrentDeck.Intersect(renoMage).Count());
                    }
                    List<Card.Cards> tempoMage = new List<Card.Cards> {Cards.SorcerersApprentice, Cards.SorcerersApprentice, Cards.MirrorImage, Cards.MirrorImage, Cards.Frostbolt, Cards.Frostbolt, Cards.ArchmageAntonidas, Cards.ManaWyrm, Cards.ManaWyrm, Cards.WaterElemental, Cards.WaterElemental, Cards.MindControlTech, Cards.ArcaneIntellect, Cards.ArcaneIntellect, Cards.Fireball, Cards.Fireball, Cards.Counterspell, Cards.MirrorEntity, Cards.ArcaneMissiles, Cards.ArcaneMissiles, Cards.MadScientist, Cards.MadScientist, Cards.UnstablePortal, Cards.UnstablePortal, Cards.Flamecannon, Cards.Flamecannon, Cards.Flamewaker, Cards.Flamewaker, Cards.EtherealConjurer, Cards.EtherealConjurer,};
                    List<Card.Cards> basicMage = new List<Card.Cards> {Cards.Frostbolt, Cards.Frostbolt, Cards.ArcaneIntellect, Cards.ArcaneIntellect, Cards.Fireball, Cards.Fireball, Cards.Polymorph, Cards.Polymorph, Cards.WaterElemental, Cards.WaterElemental, Cards.Flamestrike, Cards.Flamestrike, Cards.AcidicSwampOoze, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.KoboldGeomancer, Cards.RazorfenHunter, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.SenjinShieldmasta, Cards.GurubashiBerserker, Cards.Archmage, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion,};
                    deckDictionary.AddOrUpdate(DeckType.TempoMage, CurrentDeck.Intersect(tempoMage).Count());
                    if (CurrentDeck.ContainsAtLeast(2, Cards.IceBarrier, Cards.IceBlock, Cards.AcolyteofPain, Cards.LootHoarder, Cards.NoviceEngineer, Cards.MadScientist))
                    {
                        List<Card.Cards> freeze = new List<Card.Cards> {Cards.IceBlock, Cards.IceBlock, Cards.Flamestrike, Cards.FrostNova, Cards.FrostNova, Cards.Frostbolt, Cards.Frostbolt, Cards.IceLance, Cards.IceLance, Cards.ArchmageAntonidas, Cards.Blizzard, Cards.Blizzard, Cards.Alexstrasza, Cards.LootHoarder, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.Doomsayer, Cards.Doomsayer, Cards.ArcaneIntellect, Cards.ArcaneIntellect, Cards.Pyroblast, Cards.Fireball, Cards.Fireball, Cards.BloodmageThalnos, Cards.IceBarrier, Cards.IceBarrier, Cards.MadScientist, Cards.MadScientist, Cards.AntiqueHealbot, Cards.EmperorThaurissan,};
                        List<Card.Cards> faceFreeze = new List<Card.Cards> {Cards.SorcerersApprentice, Cards.SorcerersApprentice, Cards.IceBlock, Cards.IceBlock, Cards.MirrorImage, Cards.FrostNova, Cards.FrostNova, Cards.Frostbolt, Cards.Frostbolt, Cards.IceLance, Cards.IceLance, Cards.ManaWyrm, Cards.ManaWyrm, Cards.AzureDrake, Cards.AzureDrake, Cards.LootHoarder, Cards.LootHoarder, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.ArcaneIntellect, Cards.ArcaneIntellect, Cards.Fireball, Cards.Fireball, Cards.BloodmageThalnos, Cards.MadScientist, Cards.MadScientist, Cards.PilotedShredder, Cards.PilotedShredder, Cards.ForgottenTorch, Cards.ForgottenTorch,};
                        deckDictionary.AddOrUpdate(DeckType.FreezeMage, CurrentDeck.Intersect(freeze).Count());
                        deckDictionary.AddOrUpdate(DeckType.FaceFreezeMage, CurrentDeck.Intersect(faceFreeze).Count());
                    }
                    if (CurrentDeck.RaceCount(Card.CRace.DRAGON) > 2)
                    {
                        List<Card.Cards> dragonMage = new List<Card.Cards> {Cards.Frostbolt, Cards.Frostbolt, Cards.Duplicate, Cards.IceBarrier, Cards.IceBlock, Cards.Polymorph, Cards.Polymorph, Cards.Flamestrike, Cards.Flamestrike, Cards.ZombieChow, Cards.MadScientist, Cards.MadScientist, Cards.BigGameHunter, Cards.BlackwingTechnician, Cards.BlackwingTechnician, Cards.TwilightDrake, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.AzureDrake, Cards.AzureDrake, Cards.BlackwingCorruptor, Cards.BlackwingCorruptor, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.EmperorThaurissan, Cards.DrBoom, Cards.Alexstrasza, Cards.Ysera,};
                        deckDictionary.AddOrUpdate(DeckType.DragonMage, CurrentDeck.Intersect(dragonMage).Count());
                    }
                    if (CurrentDeck.RaceCount(Card.CRace.MECH) > 3 || CurrentDeck.ContainsSome(Cards.ClockworkGnome, Cards.Mechwarper, Cards.SpiderTank, Cards.TinkertownTechnician, Cards.Snowchugger))
                    {
                        List<Card.Cards> mechMage = new List<Card.Cards> {Cards.Frostbolt, Cards.Frostbolt, Cards.ArchmageAntonidas, Cards.ManaWyrm, Cards.ManaWyrm, Cards.Fireball, Cards.Fireball, Cards.UnstablePortal, Cards.UnstablePortal, Cards.Cogmaster, Cards.Cogmaster, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.DrBoom, Cards.SpiderTank, Cards.SpiderTank, Cards.Mechwarper, Cards.Mechwarper, Cards.PilotedShredder, Cards.PilotedShredder, Cards.GoblinBlastmage, Cards.GoblinBlastmage, Cards.ClockworkGnome, Cards.TinkertownTechnician, Cards.Snowchugger, Cards.Snowchugger, Cards.ClockworkKnight, Cards.ClockworkKnight, Cards.GorillabotA3, Cards.GorillabotA3,};
                        deckDictionary.AddOrUpdate(DeckType.MechMage, CurrentDeck.Intersect(mechMage).Count());
                    }
                    if (CurrentDeck.ContainsSome(Cards.Duplicate, Cards.EmperorThaurissan, Cards.EtherealConjurer))
                    {
                        List<Card.Cards> grinderMage = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.Flamestrike, Cards.Flamestrike, Cards.BigGameHunter, Cards.Frostbolt, Cards.Frostbolt, Cards.MindControlTech, Cards.MirrorEntity, Cards.Polymorph, Cards.ZombieChow, Cards.ZombieChow, Cards.Duplicate, Cards.MadScientist, Cards.MadScientist, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.ExplosiveSheep, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.EchoofMedivh, Cards.EmperorThaurissan, Cards.RefreshmentVendor, Cards.JeweledScarab, Cards.JeweledScarab, Cards.BrannBronzebeard, Cards.EtherealConjurer, Cards.EtherealConjurer,};
                        deckDictionary.AddOrUpdate(DeckType.FatigueMage, CurrentDeck.Intersect(grinderMage).Count());
                    }
                    deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(basicMage).Count());
                    break;

                    #endregion

                    #region paladin

                case Card.CClass.PALADIN:
                    if (activeSecrets > 0)
                        return new DeckData {DeckList = CurrentDeck, DeckType = DeckType.SecretPaladin, DeckStyle = DeckStyles[DeckType.SecretPaladin]};
                    if (CurrentDeck.IsRenoDeck(10))
                    {
                        List<Card.Cards> renoPaladin = new List<Card.Cards> {Cards.AldorPeacekeeper, Cards.BlessingofKings, Cards.SylvanasWindrunner, Cards.BigGameHunter, Cards.Humility, Cards.Consecration, Cards.TruesilverChampion, Cards.MindControlTech, Cards.Equality, Cards.StampedingKodo, Cards.TirionFordring, Cards.KnifeJuggler, Cards.IronbeakOwl, Cards.RagnarostheFirelord, Cards.LayonHands, Cards.HarrisonJones, Cards.ZombieChow, Cards.SludgeBelcher, Cards.DrBoom, Cards.PilotedShredder, Cards.MusterforBattle, Cards.AntiqueHealbot, Cards.Coghammer, Cards.ShieldedMinibot, Cards.Quartermaster, Cards.EadricthePure, Cards.TuskarrJouster, Cards.MurlocKnight, Cards.RenoJackson, Cards.KeeperofUldaman,};
                        List<Card.Cards> renoPaladin2 = new List<Card.Cards> {Cards.AldorPeacekeeper, Cards.BlessingofKings, Cards.SylvanasWindrunner, Cards.BigGameHunter, Cards.BlessingofWisdom, Cards.Humility, Cards.ArgentProtector, Cards.Consecration, Cards.GuardianofKings, Cards.TruesilverChampion, Cards.MindControlTech, Cards.Equality, Cards.StampedingKodo, Cards.TirionFordring, Cards.IronbeakOwl, Cards.LayonHands, Cards.DefenderofArgus, Cards.DrBoom, Cards.BombLobber, Cards.MusterforBattle, Cards.Coghammer, Cards.ShieldedMinibot, Cards.Quartermaster, Cards.JusticarTrueheart, Cards.GadgetzanJouster, Cards.TuskarrJouster, Cards.MurlocKnight, Cards.JeweledScarab, Cards.BrannBronzebeard, Cards.RenoJackson,};
                        deckDictionary.AddOrUpdate(DeckType.RenoPaladin, CurrentDeck.Intersect(renoPaladin).Count());
                        deckDictionary.AddOrUpdate(DeckType.RenoPaladin, CurrentDeck.Intersect(renoPaladin2).Count());
                    }
                    if (CurrentDeck.ContainsSome(Cards.AnyfinCanHappen, Cards.BluegillWarrior, Cards.MurlocWarleader, Cards.OldMurkEye, Cards.Doomsayer))
                    {
                        List<Card.Cards> anyfinPaladin = new List<Card.Cards> {Cards.AldorPeacekeeper, Cards.AldorPeacekeeper, Cards.CultMaster, Cards.OldMurkEye, Cards.MurlocWarleader, Cards.MurlocWarleader, Cards.Consecration, Cards.Consecration, Cards.BluegillWarrior, Cards.BluegillWarrior, Cards.TruesilverChampion, Cards.KnifeJuggler, Cards.LayonHands, Cards.GrimscaleOracle, Cards.GrimscaleOracle, Cards.ZombieChow, Cards.SludgeBelcher, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.MusterforBattle, Cards.MusterforBattle, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.Coghammer, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.SolemnVigil, Cards.AnyfinCanHappen, Cards.KeeperofUldaman,};
                        deckDictionary.AddOrUpdate(DeckType.AnyfinMurglMurgl, CurrentDeck.Intersect(anyfinPaladin).Count());
                    }
                    List<Card.Cards> basicPaladin = new List<Card.Cards> {Cards.TruesilverChampion, Cards.TruesilverChampion, Cards.BlessingofKings, Cards.BlessingofKings, Cards.Consecration, Cards.Consecration, Cards.HammerofWrath, Cards.HammerofWrath, Cards.GuardianofKings, Cards.GuardianofKings, Cards.AcidicSwampOoze, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.MurlocTidehunter, Cards.MurlocTidehunter, Cards.RiverCrocolisk, Cards.RiverCrocolisk, Cards.RazorfenHunter, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.FrostwolfWarlord, Cards.FrostwolfWarlord, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion, Cards.StormwindChampion,};
                    if (CurrentDeck.ContainsSome(Cards.Secretkeeper, Cards.Avenge, Cards.NobleSacrifice, Cards.Redemption, Cards.MysteriousChallenger, Cards.KnifeJuggler, Cards.ShieldedMinibot, Cards.MusterforBattle))
                    {
                        List<Card.Cards> secretPaladin = new List<Card.Cards> {Cards.BlessingofKings, Cards.NobleSacrifice, Cards.NobleSacrifice, Cards.TruesilverChampion, Cards.TirionFordring, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.Secretkeeper, Cards.Secretkeeper, Cards.DivineFavor, Cards.Repentance, Cards.Redemption, Cards.Avenge, Cards.Avenge, Cards.Loatheb, Cards.SludgeBelcher, Cards.HauntedCreeper, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.MusterforBattle, Cards.MusterforBattle, Cards.Coghammer, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.CompetitiveSpirit, Cards.MysteriousChallenger, Cards.MysteriousChallenger, Cards.KeeperofUldaman, Cards.KeeperofUldaman,};
                        deckDictionary.AddOrUpdate(DeckType.SecretPaladin, CurrentDeck.Intersect(secretPaladin).Count());
                    }
                    if (CurrentDeck.ContainsSome(Cards.ShieldedMinibot, Cards.PilotedShredder, Cards.AntiqueHealbot, Cards.Quartermaster, Cards.LayonHands, Cards.Doomsayer))
                    {
                        List<Card.Cards> midrangePaladin = new List<Card.Cards> {Cards.AldorPeacekeeper, Cards.AldorPeacekeeper, Cards.BigGameHunter, Cards.Consecration, Cards.Consecration, Cards.TruesilverChampion, Cards.TruesilverChampion, Cards.Equality, Cards.TirionFordring, Cards.KnifeJuggler, Cards.LayonHands, Cards.DefenderofArgus, Cards.ZombieChow, Cards.ZombieChow, Cards.Loatheb, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.MusterforBattle, Cards.MusterforBattle, Cards.Coghammer, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.Quartermaster, Cards.Quartermaster, Cards.JusticarTrueheart, Cards.MurlocKnight, Cards.KeeperofUldaman,};
                        deckDictionary.AddOrUpdate(DeckType.MidRangePaladin, CurrentDeck.Intersect(midrangePaladin).Count());
                    }
                    if (CurrentDeck.ContainsSome(Cards.TwilightGuardian, Cards.BlackwingTechnician, Cards.DragonConsort, Cards.BlackwingCorruptor) || CurrentDeck.RaceCount(Card.CRace.DRAGON) > 3)
                    {
                        List<Card.Cards> dragonPaladin = new List<Card.Cards> {Cards.AldorPeacekeeper, Cards.AldorPeacekeeper, Cards.BigGameHunter, Cards.Consecration, Cards.Consecration, Cards.TruesilverChampion, Cards.TruesilverChampion, Cards.Alexstrasza, Cards.Equality, Cards.Ysera, Cards.IronbeakOwl, Cards.ZombieChow, Cards.ZombieChow, Cards.SludgeBelcher, Cards.MusterforBattle, Cards.MusterforBattle, Cards.AntiqueHealbot, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.HungryDragon, Cards.HungryDragon, Cards.BlackwingTechnician, Cards.BlackwingTechnician, Cards.BlackwingCorruptor, Cards.VolcanicDrake, Cards.Chromaggus, Cards.DragonConsort, Cards.DragonConsort, Cards.SolemnVigil, Cards.EmperorThaurissan,};
                        deckDictionary.AddOrUpdate(DeckType.DragonPaladin, CurrentDeck.Intersect(dragonPaladin).Count());
                    }
                    if (CurrentDeck.ContainsAtLeast(2, Cards.AbusiveSergeant, Cards.LeperGnome, Cards.LeeroyJenkins, Cards.DivineFavor, Cards.BlessingofMight, Cards.ArcaneGolem))
                    {
                        List<Card.Cards> aggroPaladin = new List<Card.Cards> {Cards.BlessingofMight, Cards.BlessingofMight, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.Coghammer, Cards.DivineFavor, Cards.DivineFavor, Cards.MusterforBattle, Cards.MusterforBattle, Cards.TruesilverChampion, Cards.BlessingofKings, Cards.BlessingofKings, Cards.KeeperofUldaman, Cards.KeeperofUldaman, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.ArgentSquire, Cards.ArgentSquire, Cards.LeperGnome, Cards.LeperGnome, Cards.SirFinleyMrrgglton, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.IronbeakOwl, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.ArcaneGolem, Cards.PilotedShredder, Cards.LeeroyJenkins, Cards.Loatheb,};
                        deckDictionary.AddOrUpdate(DeckType.AggroPaladin, CurrentDeck.Intersect(aggroPaladin).Count());
                    }
                    deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(basicPaladin).Count());
                    break;

                    #endregion

                    #region warrior

                case Card.CClass.WARRIOR:
                    List<Card.Cards> controlWarriorCards = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.ShieldSlam, Cards.ShieldSlam, Cards.BigGameHunter, Cards.Slam, Cards.Slam, Cards.Execute, Cards.Execute, Cards.Brawl, Cards.Brawl, Cards.Deathwing, Cards.ShieldBlock, Cards.ShieldBlock, Cards.BaronGeddon, Cards.HarrisonJones, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.Armorsmith, Cards.DeathsBite, Cards.DeathsBite, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.Shieldmaiden, Cards.Shieldmaiden, Cards.Revenge, Cards.Revenge, Cards.JusticarTrueheart, Cards.Bash, Cards.JeweledScarab, Cards.JeweledScarab,};

                    if (CurrentDeck.IsRenoDeck())
                    {
                        List<Card.Cards> renoWarrior = new List<Card.Cards> {Cards.ShieldSlam, Cards.BigGameHunter, Cards.Gorehowl, Cards.Slam, Cards.Execute, Cards.Brawl, Cards.CruelTaskmaster, Cards.Deathwing, Cards.Ysera, Cards.BaronGeddon, Cards.HarrisonJones, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.Armorsmith, Cards.Revenge, Cards.Bash, Cards.BouncingBlade, Cards.DeathsBite, Cards.Shieldmaiden, Cards.Crush, Cards.PilotedShredder, Cards.Loatheb, Cards.SludgeBelcher, Cards.EmperorThaurissan, Cards.JusticarTrueheart, Cards.RenoJackson, Cards.DrBoom, Cards.BrannBronzebeard, Cards.Deathlord, Cards.IronJuggernaut,};
                        List<Card.Cards> renoWarrior2 = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.ShieldSlam, Cards.BigGameHunter, Cards.Gorehowl, Cards.Slam, Cards.Execute, Cards.Brawl, Cards.SenjinShieldmasta, Cards.CruelTaskmaster, Cards.AcolyteofPain, Cards.ShieldBlock, Cards.Ysera, Cards.IronbeakOwl, Cards.BaronGeddon, Cards.HarrisonJones, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.Armorsmith, Cards.ZombieChow, Cards.DeathsBite, Cards.SludgeBelcher, Cards.DrBoom, Cards.Shieldmaiden, Cards.Revenge, Cards.JusticarTrueheart, Cards.Bash, Cards.EliseStarseeker, Cards.JeweledScarab, Cards.RenoJackson, Cards.FierceMonkey,};
                        deckDictionary.AddOrUpdate(DeckType.RenoWarrior, CurrentDeck.Intersect(renoWarrior).Count());
                        deckDictionary.AddOrUpdate(DeckType.RenoWarrior, CurrentDeck.Intersect(renoWarrior2).Count());
                    }
                    if (CurrentDeck.ContainsAtLeast(2, Cards.Bolster, Cards.SparringPartner, Cards.FierceMonkey, Cards.ObsidianDestroyer, Cards.ArcaneNullifierX21))
                    {
                        List<Card.Cards> tauntWarrior = new List<Card.Cards> {Cards.BigGameHunter, Cards.Slam, Cards.Slam, Cards.Execute, Cards.Execute, Cards.Brawl, Cards.TheBlackKnight, Cards.AcolyteofPain, Cards.DefenderofArgus, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.Armorsmith, Cards.Armorsmith, Cards.DeathsBite, Cards.DeathsBite, Cards.KelThuzad, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.ArcaneNullifierX21, Cards.ArcaneNullifierX21, Cards.Kodorider, Cards.Bolster, Cards.VarianWrynn, Cards.SparringPartner, Cards.SparringPartner, Cards.EliseStarseeker, Cards.ObsidianDestroyer, Cards.ObsidianDestroyer, Cards.FierceMonkey, Cards.FierceMonkey,};
                        deckDictionary.AddOrUpdate(DeckType.TauntWarrior, CurrentDeck.Intersect(tauntWarrior).Count());
                    }
                    if (CurrentDeck.Contains(Cards.Deathlord))
                    {
                        List<Card.Cards> fatigueWarrior = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.ShieldSlam, Cards.ShieldSlam, Cards.BigGameHunter, Cards.Gorehowl, Cards.Slam, Cards.Slam, Cards.Execute, Cards.Execute, Cards.Brawl, Cards.Brawl, Cards.BaronGeddon, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.Revenge, Cards.Bash, Cards.Bash, Cards.BouncingBlade, Cards.DeathsBite, Cards.DeathsBite, Cards.Shieldmaiden, Cards.Shieldmaiden, Cards.Deathlord, Cards.Deathlord, Cards.JusticarTrueheart, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.UnstableGhoul, Cards.RenoJackson,};
                        deckDictionary.AddOrUpdate(DeckType.FatigueWarrior, CurrentDeck.Intersect(fatigueWarrior).Count());
                    }
                    List<Card.Cards> faceWar = new List<Card.Cards> {Cards.HeroicStrike, Cards.HeroicStrike, Cards.ArcaneGolem, Cards.ArcaneGolem, Cards.SouthseaDeckhand, Cards.SouthseaDeckhand, Cards.KorkronElite, Cards.KorkronElite, Cards.Wolfrider, Cards.DreadCorsair, Cards.DreadCorsair, Cards.MortalStrike, Cards.MortalStrike, Cards.IronbeakOwl, Cards.LeperGnome, Cards.LeperGnome, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.BloodsailRaider, Cards.BloodsailRaider, Cards.Upgrade, Cards.Upgrade, Cards.DeathsBite, Cards.DeathsBite, Cards.ArgentHorserider, Cards.ArgentHorserider, Cards.Bash, Cards.Bash, Cards.SirFinleyMrrgglton, Cards.CursedBlade,};
                    deckDictionary.AddOrUpdate(DeckType.ControlWarrior, CurrentDeck.Intersect(controlWarriorCards).Count());
                    if (CurrentDeck.RaceCount(Card.CRace.DRAGON) > 2 || CurrentDeck.ContainsSome(Cards.AlexstraszasChampion, Cards.TwilightGuardian, Cards.BlackwingCorruptor, Cards.BlackwingTechnician))
                    {
                        List<Card.Cards> dragonWarrior = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.ShieldSlam, Cards.ShieldSlam, Cards.BigGameHunter, Cards.Execute, Cards.Execute, Cards.Brawl, Cards.Alexstrasza, Cards.Deathwing, Cards.Ysera, Cards.BaronGeddon, Cards.HarrisonJones, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.DeathsBite, Cards.DeathsBite, Cards.Shieldmaiden, Cards.EmperorThaurissan, Cards.Nefarian, Cards.Revenge, Cards.Revenge, Cards.BlackwingCorruptor, Cards.BlackwingCorruptor, Cards.JusticarTrueheart, Cards.Chillmaw, Cards.Bash, Cards.Bash, Cards.TwilightGuardian, Cards.TwilightGuardian,};
                        deckDictionary.AddOrUpdate(DeckType.DragonWarrior, CurrentDeck.Intersect(dragonWarrior).Count());
                    }
                    if (CurrentDeck.ContainsSome(Cards.GrimPatron, Cards.FrothingBerserker))
                    {
                        List<Card.Cards> patronWarrior = new List<Card.Cards> {Cards.FrothingBerserker, Cards.KorkronElite, Cards.Whirlwind, Cards.Whirlwind, Cards.Slam, Cards.Slam, Cards.Execute, Cards.Execute, Cards.DreadCorsair, Cards.DreadCorsair, Cards.InnerRage, Cards.InnerRage, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.Armorsmith, Cards.Armorsmith, Cards.BattleRage, Cards.BattleRage, Cards.DeathsBite, Cards.DeathsBite, Cards.Loatheb, Cards.SludgeBelcher, Cards.UnstableGhoul, Cards.DrBoom, Cards.GrimPatron, Cards.GrimPatron, Cards.ArchThiefRafaam,};
                        deckDictionary.AddOrUpdate(DeckType.PatronWarrior, CurrentDeck.Intersect(patronWarrior).Count());
                    }
                    if (CurrentDeck.ContainsAll(Cards.RagingWorgen, Cards.Charge))
                    {
                        List<Card.Cards> worgen = new List<Card.Cards> {Cards.ShieldSlam, Cards.RagingWorgen, Cards.RagingWorgen, Cards.Whirlwind, Cards.Execute, Cards.Execute, Cards.GnomishInventor, Cards.GnomishInventor, Cards.Brawl, Cards.Brawl, Cards.CruelTaskmaster, Cards.CruelTaskmaster, Cards.InnerRage, Cards.InnerRage, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.NoviceEngineer, Cards.NoviceEngineer, Cards.Rampage, Cards.Rampage, Cards.ShieldBlock, Cards.ShieldBlock, Cards.IronbeakOwl, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.Charge, Cards.Charge, Cards.DeathsBite, Cards.DeathsBite, Cards.AntiqueHealbot,};
                        deckDictionary.AddOrUpdate(DeckType.WorgenOTKWarrior, CurrentDeck.Intersect(worgen).Count());
                    }
                    if (CurrentDeck.RaceCount(Card.CRace.MECH) > 2)
                    {
                        List<Card.Cards> mechWar = new List<Card.Cards> {Cards.HeroicStrike, Cards.HeroicStrike, Cards.KorkronElite, Cards.KorkronElite, Cards.ArcaniteReaper, Cards.ArcaniteReaper, Cards.MortalStrike, Cards.MortalStrike, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.DeathsBite, Cards.DeathsBite, Cards.Cogmaster, Cards.Cogmaster, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.SpiderTank, Cards.SpiderTank, Cards.Mechwarper, Cards.Mechwarper, Cards.PilotedShredder, Cards.PilotedShredder, Cards.TinkertownTechnician, Cards.ScrewjankClunker, Cards.ScrewjankClunker, Cards.Warbot, Cards.Warbot, Cards.FelReaver, Cards.FelReaver, Cards.ClockworkKnight,};
                        deckDictionary.AddOrUpdate(DeckType.MechWarrior, CurrentDeck.Intersect(mechWar).Count());
                    }
                    deckDictionary.AddOrUpdate(DeckType.FaceWarrior, CurrentDeck.Intersect(faceWar).Count());
                    List<Card.Cards> basicWarrior = new List<Card.Cards> {Cards.Execute, Cards.Execute, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.Cleave, Cards.ShieldBlock, Cards.KorkronElite, Cards.KorkronElite, Cards.ArcaniteReaper, Cards.ArcaniteReaper, Cards.ElvenArcher, Cards.AcidicSwampOoze, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.RiverCrocolisk, Cards.RazorfenHunter, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.GnomishInventor, Cards.SenjinShieldmasta, Cards.SenjinShieldmasta, Cards.StormpikeCommando, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion,};
                    deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(basicWarrior).Count());

                    break;

                    #endregion

                    #region warlock

                case Card.CClass.WARLOCK:

                    if (CurrentDeck.ContainsAtLeast(2, Cards.JeweledScarab, Cards.AntiqueHealbot, Cards.Ysera, Cards.EmperorThaurissan, Cards.EliseStarseeker, Cards.Demonfire, Cards.Hellfire, Cards.SiphonSoul))
                    {
                        List<Card.Cards> controlWarlock = new List<Card.Cards> {Cards.MortalCoil, Cards.MortalCoil, Cards.BigGameHunter, Cards.Hellfire, Cards.Hellfire, Cards.Demonfire, Cards.LordJaraxxus, Cards.Ysera, Cards.IronbeakOwl, Cards.DefenderofArgus, Cards.DefenderofArgus, Cards.EarthenRingFarseer, Cards.SiphonSoul, Cards.BaneofDoom, Cards.Loatheb, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.PilotedShredder, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.Demonheart, Cards.MistressofPain, Cards.Darkbomb, Cards.Darkbomb, Cards.Implosion, Cards.ImpGangBoss, Cards.ImpGangBoss, Cards.EmperorThaurissan, Cards.EliseStarseeker, Cards.JeweledScarab,};
                        deckDictionary.AddOrUpdate(DeckType.ControlWarlock, CurrentDeck.Intersect(controlWarlock).Count());
                    }
                    if (CurrentDeck.ContainsSome(Cards.MountainGiant, Cards.TwilightDrake, Cards.AncientWatcher, Cards.SunfuryProtector))
                    {
                        List<Card.Cards> handlock = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.MortalCoil, Cards.MortalCoil, Cards.BigGameHunter, Cards.MoltenGiant, Cards.MoltenGiant, Cards.Hellfire, Cards.AncientWatcher, Cards.AncientWatcher, Cards.MountainGiant, Cards.MountainGiant, Cards.TwilightDrake, Cards.TwilightDrake, Cards.SunfuryProtector, Cards.SunfuryProtector, Cards.LordJaraxxus, Cards.IronbeakOwl, Cards.IronbeakOwl, Cards.DefenderofArgus, Cards.EarthenRingFarseer, Cards.Shadowflame, Cards.ZombieChow, Cards.ZombieChow, Cards.SludgeBelcher, Cards.DrBoom, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.Darkbomb, Cards.Darkbomb, Cards.BrannBronzebeard,};
                        deckDictionary.AddOrUpdate(DeckType.Handlock, CurrentDeck.Intersect(handlock).Count());
                    }
                    if (CurrentDeck.IsRenoDeck())
                    {
                        List<Card.Cards> renoLock = new List<Card.Cards> {Cards.MortalCoil, Cards.BigGameHunter, Cards.AcidicSwampOoze, Cards.MoltenGiant, Cards.Hellfire, Cards.AncientWatcher, Cards.MountainGiant, Cards.TwilightDrake, Cards.MindControlTech, Cards.SunfuryProtector, Cards.LordJaraxxus, Cards.IronbeakOwl, Cards.RagnarostheFirelord, Cards.DefenderofArgus, Cards.SiphonSoul, Cards.AbusiveSergeant, Cards.Shadowflame, Cards.Voidcaller, Cards.SludgeBelcher, Cards.DrBoom, Cards.PilotedShredder, Cards.AntiqueHealbot, Cards.MalGanis, Cards.Darkbomb, Cards.Implosion, Cards.ImpGangBoss, Cards.EmperorThaurissan, Cards.Demonwrath, Cards.RefreshmentVendor, Cards.RenoJackson,};
                        deckDictionary.AddOrUpdate(DeckType.RenoLock, CurrentDeck.Intersect(renoLock).Count());
                        if (CurrentDeck.ContainsAll(Cards.ArcaneGolem, Cards.FacelessManipulator))
                        {
                            List<Card.Cards> RenoCombo = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.MortalCoil, Cards.BigGameHunter, Cards.AcidicSwampOoze, Cards.Hellfire, Cards.PowerOverwhelming, Cards.TwilightDrake, Cards.TwistingNether, Cards.FacelessManipulator, Cards.LordJaraxxus, Cards.IronbeakOwl, Cards.DefenderofArgus, Cards.EarthenRingFarseer, Cards.SiphonSoul, Cards.AbusiveSergeant, Cards.Shadowflame, Cards.LeeroyJenkins, Cards.ZombieChow, Cards.Loatheb, Cards.SludgeBelcher, Cards.DrBoom, Cards.AntiqueHealbot, Cards.Darkbomb, Cards.ImpGangBoss, Cards.EmperorThaurissan, Cards.Demonwrath, Cards.RefreshmentVendor, Cards.BrannBronzebeard, Cards.RenoJackson, Cards.DarkPeddler,};
                            deckDictionary.AddOrUpdate(DeckType.RenoComboLock, CurrentDeck.Intersect(RenoCombo).Count() + 1); //addomg some extra weight
                        }
                    }
                    List<Card.Cards> zoolock = new List<Card.Cards> {Cards.BigGameHunter, Cards.AcidicSwampOoze, Cards.FlameImp, Cards.FlameImp, Cards.DarkIronDwarf, Cards.PowerOverwhelming, Cards.PowerOverwhelming, Cards.DireWolfAlpha, Cards.Voidwalker, Cards.Voidwalker, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.IronbeakOwl, Cards.Doomguard, Cards.Doomguard, Cards.DarkPeddler, Cards.DarkPeddler, Cards.ImpGangBoss, Cards.ImpGangBoss, Cards.Implosion, Cards.Implosion, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.NerubianEgg, Cards.NerubianEgg, Cards.BrannBronzebeard, Cards.DefenderofArgus, Cards.DefenderofArgus,};
                    if (CurrentDeck.ContainsAtLeast(2, Cards.Voidcaller, Cards.Doomguard, Cards.MalGanis, Cards.ImpGangBoss))
                    {
                        List<Card.Cards> demonzoolock = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.BigGameHunter, Cards.FlameImp, Cards.VoidTerror, Cards.PowerOverwhelming, Cards.PowerOverwhelming, Cards.DireWolfAlpha, Cards.Voidwalker, Cards.Voidwalker, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.IronbeakOwl, Cards.Doomguard, Cards.Doomguard, Cards.DefenderofArgus, Cards.DefenderofArgus, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.Voidcaller, Cards.Voidcaller, Cards.NerubianEgg, Cards.NerubianEgg, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.MalGanis, Cards.Implosion, Cards.Implosion, Cards.ImpGangBoss, Cards.ImpGangBoss,};
                        deckDictionary.AddOrUpdate(DeckType.DemonZooWarlock, CurrentDeck.Intersect(demonzoolock).Count());

                        if (CurrentDeck.ContainsSome(Cards.TwilightDrake, Cards.MountainGiant, Cards.MoltenGiant))
                        {
                            List<Card.Cards> demonHandLock = new List<Card.Cards> {Cards.DreadInfernal, Cards.MortalCoil, Cards.MortalCoil, Cards.MoltenGiant, Cards.MoltenGiant, Cards.AncientWatcher, Cards.AncientWatcher, Cards.MountainGiant, Cards.MountainGiant, Cards.TwilightDrake, Cards.TwilightDrake, Cards.SunfuryProtector, Cards.SunfuryProtector, Cards.LordJaraxxus, Cards.IronbeakOwl, Cards.DefenderofArgus, Cards.SiphonSoul, Cards.Shadowflame, Cards.Shadowflame, Cards.ZombieChow, Cards.ZombieChow, Cards.Voidcaller, Cards.Voidcaller, Cards.Loatheb, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.DrBoom, Cards.AntiqueHealbot, Cards.MalGanis, Cards.Darkbomb,};
                            deckDictionary.AddOrUpdate(DeckType.DemonHandlock, CurrentDeck.Intersect(demonHandLock).Count());
                        }
                    }
                    deckDictionary.AddOrUpdate(DeckType.Zoolock, CurrentDeck.Intersect(zoolock).Count());
                    if (CurrentDeck.ContainsAll(Cards.TwilightGuardian, Cards.MountainGiant))
                    {
                        List<Card.Cards> dragonHandlock = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.MortalCoil, Cards.MortalCoil, Cards.Darkbomb, Cards.Darkbomb, Cards.AncientWatcher, Cards.AncientWatcher, Cards.IronbeakOwl, Cards.SunfuryProtector, Cards.SunfuryProtector, Cards.BigGameHunter, Cards.Hellfire, Cards.Hellfire, Cards.Shadowflame, Cards.DefenderofArgus, Cards.TwilightDrake, Cards.TwilightDrake, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.AntiqueHealbot, Cards.BlackwingCorruptor, Cards.BlackwingCorruptor, Cards.EmperorThaurissan, Cards.Chillmaw, Cards.DrBoom, Cards.Alexstrasza, Cards.MountainGiant, Cards.MountainGiant, Cards.MoltenGiant, Cards.MoltenGiant,};
                        deckDictionary.AddOrUpdate(DeckType.DragonHandlock, CurrentDeck.Intersect(dragonHandlock).Count());
                    }
                    if (CurrentDeck.Contains(Cards.Malygos))
                    {
                        List<Card.Cards> malyLock = new List<Card.Cards> {Cards.MortalCoil, Cards.MortalCoil, Cards.BigGameHunter, Cards.BigGameHunter, Cards.Hellfire, Cards.Hellfire, Cards.Malygos, Cards.AzureDrake, Cards.TwilightDrake, Cards.TwilightDrake, Cards.IronbeakOwl, Cards.Soulfire, Cards.EarthenRingFarseer, Cards.AbusiveSergeant, Cards.ZombieChow, Cards.ZombieChow, Cards.Loatheb, Cards.SludgeBelcher, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.Darkbomb, Cards.Darkbomb, Cards.BlackwingCorruptor, Cards.BlackwingCorruptor, Cards.EmperorThaurissan, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.BrannBronzebeard, Cards.DarkPeddler, Cards.DarkPeddler,};
                        deckDictionary.AddOrUpdate(DeckType.MalyLock, CurrentDeck.Intersect(malyLock).Count());
                    }
                    break;

                    #endregion

                    #region hunter

                case Card.CClass.HUNTER:

                    if (CurrentDeck.RaceCount(Card.CRace.DRAGON) > 3 || CurrentDeck.ContainsAtLeast(2, Cards.TwilightGuardian, Cards.BlackwingTechnician, Cards.BlackwingCorruptor, Cards.DrakonidCrusher, Cards.RendBlackhand))
                    {
                        List<Card.Cards> dragonHunter = new List<Card.Cards> {Cards.HuntersMark, Cards.HuntersMark, Cards.WildPyromancer, Cards.WildPyromancer, Cards.ArcaneShot, Cards.ArcaneShot, Cards.GladiatorsLongbow, Cards.AzureDrake, Cards.AzureDrake, Cards.Alexstrasza, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.Ysera, Cards.Gahzrilla, Cards.BlackwingTechnician, Cards.BlackwingTechnician, Cards.RendBlackhand, Cards.BlackwingCorruptor, Cards.BlackwingCorruptor, Cards.DrakonidCrusher, Cards.DrakonidCrusher, Cards.Chromaggus, Cards.QuickShot, Cards.QuickShot, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.Chillmaw, Cards.KingsElekk, Cards.KingsElekk, Cards.Dreadscale,};
                        deckDictionary.AddOrUpdate(DeckType.DragonHunter, CurrentDeck.Intersect(dragonHunter).Count());
                    }
                    if (CurrentDeck.IsRenoDeck())
                    {
                        List<Card.Cards> renoHunter = new List<Card.Cards> {Cards.SavannahHighmane, Cards.HuntersMark, Cards.BigGameHunter, Cards.FreezingTrap, Cards.Tracking, Cards.Houndmaster, Cards.UnleashtheHounds, Cards.ExplosiveTrap, Cards.EaglehornBow, Cards.KnifeJuggler, Cards.KillCommand, Cards.Ysera, Cards.IronbeakOwl, Cards.AnimalCompanion, Cards.Webspinner, Cards.Loatheb, Cards.MadScientist, Cards.MadScientist, Cards.SludgeBelcher, Cards.HauntedCreeper, Cards.DrBoom, Cards.PilotedShredder, Cards.QuickShot, Cards.ArgentHorserider, Cards.BearTrap, Cards.KingsElekk, Cards.Powershot, Cards.JeweledScarab, Cards.SirFinleyMrrgglton, Cards.RenoJackson,};
                        List<Card.Cards> renoHunter2 = new List<Card.Cards> {Cards.HuntersMark, Cards.SavannahHighmane, Cards.BigGameHunter, Cards.Houndmaster, Cards.UnleashtheHounds, Cards.ExplosiveTrap, Cards.EaglehornBow, Cards.Doomsayer, Cards.IronbeakOwl, Cards.DefenderofArgus, Cards.EarthenRingFarseer, Cards.AnimalCompanion, Cards.HarrisonJones, Cards.MadScientist, Cards.SludgeBelcher, Cards.Deathlord, Cards.HauntedCreeper, Cards.DrBoom, Cards.AntiqueHealbot, Cards.QuickShot, Cards.JusticarTrueheart, Cards.BearTrap, Cards.KingsElekk, Cards.Powershot, Cards.EliseStarseeker, Cards.JeweledScarab, Cards.BrannBronzebeard, Cards.SirFinleyMrrgglton, Cards.RenoJackson, Cards.ArchThiefRafaam,};
                        deckDictionary.AddOrUpdate(DeckType.RenoHunter, CurrentDeck.Intersect(renoHunter).Count());
                        deckDictionary.AddOrUpdate(DeckType.RenoHunter, CurrentDeck.Intersect(renoHunter2).Count());
                    }
                    if (CurrentDeck.ContainsAll(Cards.DesertCamel, Cards.InjuredKvaldir) || CurrentDeck.ContainsAtLeast(2, Cards.FlameJuggler, Cards.Glaivezooka, Cards.CultMaster))
                    {
                        List<Card.Cards> injuredCamel = new List<Card.Cards> {Cards.SavannahHighmane, Cards.SavannahHighmane, Cards.HuntersMark, Cards.HuntersMark, Cards.CultMaster, Cards.CultMaster, Cards.Houndmaster, Cards.Houndmaster, Cards.UnleashtheHounds, Cards.UnleashtheHounds, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.AnimalCompanion, Cards.AnimalCompanion, Cards.Webspinner, Cards.Webspinner, Cards.Loatheb, Cards.SludgeBelcher, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.PilotedShredder, Cards.Glaivezooka, Cards.Glaivezooka, Cards.FlameJuggler, Cards.FlameJuggler, Cards.InjuredKvaldir, Cards.InjuredKvaldir, Cards.DesertCamel, Cards.DesertCamel,};
                        deckDictionary.AddOrUpdate(DeckType.CamelHunter, CurrentDeck.Intersect(injuredCamel).Count());
                    }
                    if (CurrentDeck.ContainsAtLeast(2, Cards.SavannahHighmane, Cards.LeperGnome, Cards.AbusiveSergeant, Cards.ArcaneGolem))
                    {
                        List<Card.Cards> hybridHunter = new List<Card.Cards> {Cards.SavannahHighmane, Cards.SavannahHighmane, Cards.FreezingTrap, Cards.UnleashtheHounds, Cards.UnleashtheHounds, Cards.ExplosiveTrap, Cards.EaglehornBow, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.KillCommand, Cards.KillCommand, Cards.IronbeakOwl, Cards.LeperGnome, Cards.LeperGnome, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.AnimalCompanion, Cards.AnimalCompanion, Cards.Loatheb, Cards.MadScientist, Cards.MadScientist, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.PilotedShredder, Cards.PilotedShredder, Cards.Glaivezooka, Cards.Glaivezooka, Cards.QuickShot, Cards.ArgentHorserider, Cards.ArgentHorserider,};
                        deckDictionary.AddOrUpdate(DeckType.HybridHunter, CurrentDeck.Intersect(hybridHunter).Count());
                    }
                    if (CurrentDeck.Contains(Cards.ExplorersHat))
                    {
                        List<Card.Cards> hatHunter = new List<Card.Cards> {Cards.HuntersMark, Cards.HuntersMark, Cards.SylvanasWindrunner, Cards.UnleashtheHounds, Cards.UnleashtheHounds, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.AnimalCompanion, Cards.AnimalCompanion, Cards.Flare, Cards.ZombieChow, Cards.NerubianEgg, Cards.NerubianEgg, Cards.Webspinner, Cards.Webspinner, Cards.Loatheb, Cards.HauntedCreeper, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.Glaivezooka, Cards.Glaivezooka, Cards.QuickShot, Cards.QuickShot, Cards.BallofSpiders, Cards.BearTrap, Cards.ExplorersHat, Cards.ExplorersHat, Cards.JeweledScarab, Cards.JeweledScarab,};
                        deckDictionary.AddOrUpdate(DeckType.HatHunter, CurrentDeck.Intersect(hatHunter).Count());
                    }
                    List<Card.Cards> midRangeHunter = new List<Card.Cards> {Cards.SavannahHighmane, Cards.SavannahHighmane, Cards.HuntersMark, Cards.FreezingTrap, Cards.FreezingTrap, Cards.Houndmaster, Cards.Houndmaster, Cards.UnleashtheHounds, Cards.StranglethornTiger, Cards.EaglehornBow, Cards.EaglehornBow, Cards.KillCommand, Cards.KillCommand, Cards.IronbeakOwl, Cards.AnimalCompanion, Cards.AnimalCompanion, Cards.Webspinner, Cards.Webspinner, Cards.Loatheb, Cards.MadScientist, Cards.MadScientist, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.Glaivezooka, Cards.QuickShot, Cards.KingsElekk, Cards.KingsElekk,};
                    if (CurrentDeck.ContainsAtLeast(2, Cards.ExplosiveTrap, Cards.AbusiveSergeant, Cards.LeperGnome, Cards.KnifeJuggler))
                    {
                        List<Card.Cards> faceHunter = new List<Card.Cards> {Cards.ArcaneGolem, Cards.WorgenInfiltrator, Cards.WorgenInfiltrator, Cards.UnleashtheHounds, Cards.UnleashtheHounds, Cards.ExplosiveTrap, Cards.EaglehornBow, Cards.EaglehornBow, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.KillCommand, Cards.KillCommand, Cards.IronbeakOwl, Cards.LeperGnome, Cards.LeperGnome, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.AnimalCompanion, Cards.AnimalCompanion, Cards.LeeroyJenkins, Cards.MadScientist, Cards.MadScientist, Cards.Glaivezooka, Cards.QuickShot, Cards.QuickShot, Cards.FlameJuggler, Cards.FlameJuggler, Cards.ArgentHorserider, Cards.ArgentHorserider, Cards.BearTrap,};
                        deckDictionary.AddOrUpdate(DeckType.FaceHunter, CurrentDeck.Intersect(faceHunter).Count());
                    }
                    List<Card.Cards> basicHunter = new List<Card.Cards> {Cards.HuntersMark, Cards.HuntersMark, Cards.ArcaneShot, Cards.ArcaneShot, Cards.Tracking, Cards.AnimalCompanion, Cards.AnimalCompanion, Cards.KillCommand, Cards.KillCommand, Cards.MultiShot, Cards.MultiShot, Cards.Houndmaster, Cards.Houndmaster, Cards.StonetuskBoar, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.RiverCrocolisk, Cards.RiverCrocolisk, Cards.IronfurGrizzly, Cards.IronfurGrizzly, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.OasisSnapjaw, Cards.FrostwolfWarlord, Cards.FrostwolfWarlord, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion,};
                    deckDictionary.AddOrUpdate(DeckType.MidRangeHunter, CurrentDeck.Intersect(midRangeHunter).Count());
                    deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(basicHunter).Count());

                    break;

                    #endregion

                    #region rogue

                case Card.CClass.ROGUE:

                    deckDictionary.AddOrUpdate(DeckType.OilRogue, 3);
                    if (CurrentDeck.IsRenoDeck(10))
                    {
                        List<Card.Cards> renoRogue = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.BigGameHunter, Cards.DeadlyPoison, Cards.Sprint, Cards.Betrayal, Cards.BladeFlurry, Cards.AzureDrake, Cards.SI7Agent, Cards.SenjinShieldmasta, Cards.Preparation, Cards.FanofKnives, Cards.Eviscerate, Cards.Sap, Cards.Backstab, Cards.VioletTeacher, Cards.BloodmageThalnos, Cards.EarthenRingFarseer, Cards.Assassinate, Cards.ZombieChow, Cards.SludgeBelcher, Cards.DrBoom, Cards.SneedsOldShredder, Cards.PilotedShredder, Cards.GoblinAutoBarber, Cards.TinkersSharpswordOil, Cards.DarkIronSkulker, Cards.EmperorThaurissan, Cards.Burgle, Cards.JeweledScarab, Cards.RenoJackson,};
                        List<Card.Cards> renoRogue2 = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.BigGameHunter, Cards.DeadlyPoison, Cards.Sprint, Cards.GadgetzanAuctioneer, Cards.BladeFlurry, Cards.YouthfulBrewmaster, Cards.AzureDrake, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.FanofKnives, Cards.Eviscerate, Cards.Sap, Cards.Backstab, Cards.Backstab, Cards.BloodmageThalnos, Cards.EarthenRingFarseer, Cards.ZombieChow, Cards.SludgeBelcher, Cards.DrBoom, Cards.PilotedShredder, Cards.AntiqueHealbot, Cards.Sabotage, Cards.TinkersSharpswordOil, Cards.Burgle, Cards.Anubarak, Cards.RefreshmentVendor, Cards.UnearthedRaptor, Cards.RenoJackson,};
                        deckDictionary.AddOrUpdate(DeckType.RenoRogue, CurrentDeck.Intersect(renoRogue).Count());
                        deckDictionary.AddOrUpdate(DeckType.RenoRogue, CurrentDeck.Intersect(renoRogue2).Count());
                    }
                    if (CurrentDeck.ContainsSome(Cards.ColdlightOracle, Cards.GangUp))
                        return new DeckData {DeckList = CurrentDeck, DeckType = DeckType.MillRogue, DeckStyle = DeckStyles[DeckType.MillRogue]};
                    if (CurrentDeck.ContainsSome(Cards.Deathlord, Cards.Shadowstep, Cards.YouthfulBrewmaster, Cards.Vanish))
                    {
                        List<Card.Cards> millRogue = new List<Card.Cards> {Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.ColdlightOracle, Cards.ColdlightOracle, Cards.Sprint, Cards.BladeFlurry, Cards.YouthfulBrewmaster, Cards.SI7Agent, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.FanofKnives, Cards.FanofKnives, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.Sap, Cards.Backstab, Cards.Backstab, Cards.Shadowstep, Cards.Shadowstep, Cards.Vanish, Cards.Vanish, Cards.GangUp, Cards.GangUp, Cards.BrannBronzebeard, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.Deathlord, Cards.Deathlord,};
                        deckDictionary.AddOrUpdate(DeckType.MillRogue, CurrentDeck.Intersect(millRogue).Count());
                    }
                    if (CurrentDeck.RaceCount(Card.CRace.MECH) >= 4 || CurrentDeck.ContainsAtLeast(2, Cards.ClockworkGnome, Cards.Mechwarper, Cards.GoblinAutoBarber, Cards.AnnoyoTron, Cards.TinkertownTechnician))
                    {
                        List<Card.Cards> mechRogue = new List<Card.Cards> {Cards.ColdBlood, Cards.ColdBlood, Cards.BladeFlurry, Cards.FanofKnives, Cards.Eviscerate, Cards.Eviscerate, Cards.HarvestGolem, Cards.HarvestGolem, Cards.EnhanceoMechano, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.DrBoom, Cards.Mechwarper, Cards.Mechwarper, Cards.PilotedShredder, Cards.PilotedShredder, Cards.ClockworkGnome, Cards.ClockworkGnome, Cards.TinkertownTechnician, Cards.TinkertownTechnician, Cards.GoblinAutoBarber, Cards.GoblinAutoBarber, Cards.IronSensei, Cards.IronSensei, Cards.TinkersSharpswordOil, Cards.ClockworkKnight, Cards.ClockworkKnight, Cards.EydisDarkbane, Cards.GorillabotA3, Cards.GorillabotA3,};
                        deckDictionary.AddOrUpdate(DeckType.MechRogue, CurrentDeck.Intersect(mechRogue).Count());
                    }
                    if (CurrentDeck.Contains(Cards.GadgetzanAuctioneer))
                    {
                        List<Card.Cards> miracleRogue = new List<Card.Cards> {Cards.EdwinVanCleef, Cards.DeadlyPoison, Cards.ColdBlood, Cards.ColdBlood, Cards.GadgetzanAuctioneer, Cards.GadgetzanAuctioneer, Cards.Shiv, Cards.Shiv, Cards.BladeFlurry, Cards.AzureDrake, Cards.AzureDrake, Cards.Conceal, Cards.SI7Agent, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.FanofKnives, Cards.FanofKnives, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.Sap, Cards.Backstab, Cards.Backstab, Cards.BloodmageThalnos, Cards.Shadowstep, Cards.Shadowstep, Cards.EarthenRingFarseer, Cards.EarthenRingFarseer, Cards.LeeroyJenkins,};
                        deckDictionary.AddOrUpdate(DeckType.MiracleRogue, CurrentDeck.Intersect(miracleRogue).Count());
                    }
                    if (CurrentDeck.ContainsSome(Cards.TinkersSharpswordOil, Cards.DeadlyPoison, Cards.VioletTeacher, Cards.EdwinVanCleef, Cards.Backstab, Cards.BladeFlurry, Cards.FanofKnives))
                    {
                        List<Card.Cards> oilRogue = new List<Card.Cards> {Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.Sprint, Cards.Sprint, Cards.SouthseaDeckhand, Cards.BladeFlurry, Cards.BladeFlurry, Cards.AzureDrake, Cards.AzureDrake, Cards.SI7Agent, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.FanofKnives, Cards.FanofKnives, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.Sap, Cards.Backstab, Cards.Backstab, Cards.VioletTeacher, Cards.BloodmageThalnos, Cards.EarthenRingFarseer, Cards.PilotedShredder, Cards.PilotedShredder, Cards.AntiqueHealbot, Cards.Sabotage, Cards.TinkersSharpswordOil, Cards.TinkersSharpswordOil,};
                        deckDictionary.AddOrUpdate(DeckType.OilRogue, CurrentDeck.Intersect(oilRogue).Count());
                    }

                    if (CurrentDeck.Contains(Cards.ShipsCannon))
                    {
                        List<Card.Cards> pirateRogue = new List<Card.Cards> {Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.Sprint, Cards.Sprint, Cards.SouthseaDeckhand, Cards.SouthseaDeckhand, Cards.BladeFlurry, Cards.BladeFlurry, Cards.DreadCorsair, Cards.DreadCorsair, Cards.AzureDrake, Cards.AzureDrake, Cards.SI7Agent, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.AssassinsBlade, Cards.Backstab, Cards.Backstab, Cards.BloodsailRaider, Cards.BloodsailRaider, Cards.ShipsCannon, Cards.ShipsCannon, Cards.TinkersSharpswordOil, Cards.TinkersSharpswordOil, Cards.Buccaneer, Cards.Buccaneer,};
                        deckDictionary.AddOrUpdate(DeckType.PirateRogue, CurrentDeck.Intersect(pirateRogue).Count());
                    }
                    if (CurrentDeck.Contains(Cards.Malygos))
                    {
                        List<Card.Cards> malyRogue = new List<Card.Cards> {Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.GadgetzanAuctioneer, Cards.GadgetzanAuctioneer, Cards.Shiv, Cards.Shiv, Cards.Malygos, Cards.BladeFlurry, Cards.BladeFlurry, Cards.AzureDrake, Cards.AzureDrake, Cards.SI7Agent, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.FanofKnives, Cards.FanofKnives, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.Sap, Cards.Backstab, Cards.Backstab, Cards.TombPillager, Cards.TombPillager, Cards.BloodmageThalnos, Cards.VioletTeacher, Cards.EmperorThaurissan, Cards.AntiqueHealbot, Cards.AntiqueHealbot,};
                        deckDictionary.AddOrUpdate(DeckType.MalyRogue, CurrentDeck.Intersect(malyRogue).Count());
                    }
                    if (CurrentDeck.Contains(Cards.ColdlightOracle))
                    {
                        List<Card.Cards> fatigueRogue = new List<Card.Cards> {Cards.BigGameHunter, Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.ColdlightOracle, Cards.ColdlightOracle, Cards.BladeFlurry, Cards.BladeFlurry, Cards.SI7Agent, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.FanofKnives, Cards.FanofKnives, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.Sap, Cards.Backstab, Cards.Backstab, Cards.BloodmageThalnos, Cards.Shadowstep, Cards.Shadowstep, Cards.Vanish, Cards.Vanish, Cards.Deathlord, Cards.Deathlord, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.TinkersSharpswordOil, Cards.GangUp,};
                        deckDictionary.AddOrUpdate(DeckType.FatigueRogue, CurrentDeck.Intersect(fatigueRogue).Count());
                    }
                    if (CurrentDeck.Contains(Cards.UnearthedRaptor))
                    {
                        List<Card.Cards> raptorRogue = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.ColdBlood, Cards.ColdBlood, Cards.FanofKnives, Cards.FanofKnives, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.LootHoarder, Cards.LootHoarder, Cards.Backstab, Cards.Backstab, Cards.UnearthedRaptor, Cards.UnearthedRaptor, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.LeperGnome, Cards.LeperGnome, Cards.NerubianEgg, Cards.NerubianEgg, Cards.DefenderofArgus, Cards.DefenderofArgus, Cards.PilotedShredder, Cards.PilotedShredder, Cards.Loatheb, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.DrBoom, Cards.HauntedCreeper, Cards.HauntedCreeper,};
                        deckDictionary.AddOrUpdate(DeckType.RaptorRogue, CurrentDeck.Intersect(raptorRogue).Count());
                    }
                    List<Card.Cards> faceRogue = new List<Card.Cards> {Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.ColdlightOracle, Cards.ColdlightOracle, Cards.ColdBlood, Cards.ColdBlood, Cards.ArcaneGolem, Cards.ArcaneGolem, Cards.SouthseaDeckhand, Cards.SouthseaDeckhand, Cards.Wolfrider, Cards.Wolfrider, Cards.BladeFlurry, Cards.BladeFlurry, Cards.SI7Agent, Cards.SI7Agent, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.Sap, Cards.LeperGnome, Cards.LeperGnome, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.TinkersSharpswordOil, Cards.TinkersSharpswordOil, Cards.ArgentHorserider, Cards.ArgentHorserider, Cards.Buccaneer, Cards.Buccaneer,};
                    List<Card.Cards> basic = new List<Card.Cards> {Cards.Backstab, Cards.Backstab, Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.Sap, Cards.Shiv, Cards.Shiv, Cards.FanofKnives, Cards.FanofKnives, Cards.AssassinsBlade, Cards.AssassinsBlade, Cards.Assassinate, Cards.Assassinate, Cards.Sprint, Cards.AcidicSwampOoze, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.KoboldGeomancer, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.GnomishInventor, Cards.SenjinShieldmasta, Cards.SenjinShieldmasta, Cards.BoulderfistOgre, Cards.BoulderfistOgre,};
                    deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(basic).Count());
                    deckDictionary.AddOrUpdate(DeckType.FaceRogue, CurrentDeck.Intersect(faceRogue).Count());
                    break;

                    #endregion

                    #region druid

                case Card.CClass.DRUID:
                    List<Card.Cards> tokenDruid = new List<Card.Cards> {Cards.PoweroftheWild, Cards.PoweroftheWild, Cards.SouloftheForest, Cards.SouloftheForest, Cards.SavageRoar, Cards.SavageRoar, Cards.KeeperoftheGrove, Cards.MarkoftheWild, Cards.MarkoftheWild, Cards.DefenderofArgus, Cards.DefenderofArgus, Cards.Innervate, Cards.Innervate, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.LivingRoots, Cards.LivingRoots, Cards.MountedRaptor, Cards.MountedRaptor, Cards.DragonEgg, Cards.DragonEgg, Cards.NerubianEgg, Cards.NerubianEgg, Cards.EchoingOoze, Cards.EchoingOoze, Cards.Jeeves, Cards.Jeeves, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.SirFinleyMrrgglton,};
                    List<Card.Cards> basicDruid = new List<Card.Cards> {Cards.Innervate, Cards.Innervate, Cards.Claw, Cards.Claw, Cards.MarkoftheWild, Cards.MarkoftheWild, Cards.WildGrowth, Cards.WildGrowth, Cards.Swipe, Cards.Swipe, Cards.Starfire, Cards.Starfire, Cards.IronbarkProtector, Cards.IronbarkProtector, Cards.AcidicSwampOoze, Cards.RiverCrocolisk, Cards.RiverCrocolisk, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.GnomishInventor, Cards.SenjinShieldmasta, Cards.SenjinShieldmasta, Cards.DarkscaleHealer, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion, Cards.StormwindChampion,};

                    if (CurrentDeck.IsRenoDeck(10)) //at least 1/3 of a deck must be distict
                    {
                        List<Card.Cards> renoDruid = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.AncientofLore, Cards.BigGameHunter, Cards.ForceofNature, Cards.AncientofWar, Cards.AzureDrake, Cards.WildGrowth, Cards.SenjinShieldmasta, Cards.SavageRoar, Cards.MindControlTech, Cards.KeeperoftheGrove, Cards.Innervate, Cards.DruidoftheClaw, Cards.HarrisonJones, Cards.Cenarius, Cards.Swipe, Cards.Wrath, Cards.ZombieChow, Cards.ShadeofNaxxramas, Cards.Loatheb, Cards.SludgeBelcher, Cards.DrBoom, Cards.PilotedShredder, Cards.SavageCombatant, Cards.DarnassusAspirant, Cards.LivingRoots, Cards.JeweledScarab, Cards.RenoJackson, Cards.RavenIdol, Cards.MountedRaptor,};
                        List<Card.Cards> renoDruid2 = new List<Card.Cards> {Cards.AncientofLore, Cards.BigGameHunter, Cards.ForceofNature, Cards.AncientofWar, Cards.AzureDrake, Cards.WildGrowth, Cards.SavageRoar, Cards.TheBlackKnight, Cards.KnifeJuggler, Cards.KeeperoftheGrove, Cards.BloodmageThalnos, Cards.Innervate, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.ZombieChow, Cards.RavenIdol, Cards.DarnassusAspirant, Cards.MountedRaptor, Cards.SavageCombatant, Cards.HauntedCreeper, Cards.PilotedShredder, Cards.Loatheb, Cards.SludgeBelcher, Cards.EmperorThaurissan, Cards.RenoJackson, Cards.DrBoom, Cards.LivingRoots, Cards.ShadeofNaxxramas,};
                        deckDictionary.AddOrUpdate(DeckType.RenoDruid, CurrentDeck.Intersect(renoDruid).Count());
                        deckDictionary.AddOrUpdate(DeckType.RenoDruid, CurrentDeck.Intersect(renoDruid2).Count());
                    }
                    if (CurrentDeck.RaceCount(Card.CRace.BEAST) > 3 || CurrentDeck.ContainsAtLeast(2, Cards.HauntedCreeper, Cards.MountedRaptor, Cards.Wildwalker, Cards.DruidoftheFang))
                    {
                        List<Card.Cards> beastDruid = new List<Card.Cards> {Cards.AncientofLore, Cards.AncientofLore, Cards.ForceofNature, Cards.SavageRoar, Cards.SavageRoar, Cards.IronbeakOwl, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.Wrath, Cards.ShadeofNaxxramas, Cards.Loatheb, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.DruidoftheFang, Cards.SavageCombatant, Cards.SavageCombatant, Cards.KnightoftheWild, Cards.Wildwalker, Cards.JeweledScarab, Cards.JeweledScarab, Cards.TombSpider, Cards.RavenIdol, Cards.MountedRaptor, Cards.MountedRaptor,};
                        deckDictionary.AddOrUpdate(DeckType.BeastDruid, CurrentDeck.Intersect(beastDruid).Count());
                    }
                    if (CurrentDeck.QualityCount(Card.CQuality.Legendary) > 4 || CurrentDeck.Contains(Cards.AstralCommunion))
                    {
                        List<Card.Cards> astralDruid = new List<Card.Cards> {Cards.SylvanasWindrunner, Cards.AncientofLore, Cards.AncientofLore, Cards.Nourish, Cards.Nourish, Cards.AncientofWar, Cards.AncientofWar, Cards.WildGrowth, Cards.WildGrowth, Cards.Alexstrasza, Cards.Ysera, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.KelThuzad, Cards.DrBoom, Cards.TreeofLife, Cards.GroveTender, Cards.GroveTender, Cards.Nefarian, Cards.Chromaggus, Cards.EmperorThaurissan, Cards.FrostGiant, Cards.FrostGiant, Cards.Aviana, Cards.AstralCommunion, Cards.AstralCommunion,};
                        deckDictionary.AddOrUpdate(DeckType.AstralDruid, CurrentDeck.Intersect(astralDruid).Count());
                    }

                    deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(basicDruid).Count());
                    if (CurrentDeck.RaceCount(Card.CRace.MECH) > 1 || CurrentDeck.ContainsSome(Cards.Cogmaster, Cards.AnnoyoTron, Cards.Mechwarper, Cards.ClockworkGnome))
                    {
                        List<Card.Cards> mechDruid = new List<Card.Cards> {Cards.AncientofLore, Cards.AncientofLore, Cards.PoweroftheWild, Cards.ForceofNature, Cards.ForceofNature, Cards.SavageRoar, Cards.SavageRoar, Cards.KeeperoftheGrove, Cards.KeeperoftheGrove, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.Wrath, Cards.Starfire, Cards.HauntedCreeper, Cards.Cogmaster, Cards.Cogmaster, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.DrBoom, Cards.SpiderTank, Cards.SpiderTank, Cards.Mechwarper, Cards.Mechwarper, Cards.PilotedShredder, Cards.PilotedShredder, Cards.TinkertownTechnician,};
                        deckDictionary.AddOrUpdate(DeckType.MechDruid, CurrentDeck.Intersect(mechDruid).Count());
                    }
                    if (CurrentDeck.ContainsSome(Cards.ColdlightOracle, Cards.Naturalize, Cards.GroveTender, Cards.TreeofLife, Cards.YouthfulBrewmaster))
                    {
                        List<Card.Cards> millDruid = new List<Card.Cards> {Cards.DeadlyPoison, Cards.DeadlyPoison, Cards.ColdlightOracle, Cards.ColdlightOracle, Cards.BladeFlurry, Cards.SI7Agent, Cards.SI7Agent, Cards.Preparation, Cards.Preparation, Cards.KingMukla, Cards.Eviscerate, Cards.Eviscerate, Cards.Sap, Cards.Sap, Cards.Backstab, Cards.Backstab, Cards.Shadowstep, Cards.Shadowstep, Cards.Vanish, Cards.Vanish, Cards.Deathlord, Cards.Deathlord, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.GangUp, Cards.GangUp, Cards.RefreshmentVendor, Cards.RefreshmentVendor, Cards.BeneaththeGrounds, Cards.BrannBronzebeard,};
                        deckDictionary.AddOrUpdate(DeckType.MillDruid, CurrentDeck.Intersect(millDruid).Count());
                    }

                    if (CurrentDeck.ContainsAtLeast(2, Cards.RagnarostheFirelord, Cards.AncientofWar, Cards.Cenarius, Cards.SylvanasWindrunner, Cards.TheBlackKnight, Cards.SludgeBelcher, Cards.ZombieChow, Cards.SenjinShieldmasta))
                    {
                        List<Card.Cards> rampDruid = new List<Card.Cards> {Cards.AncientofLore, Cards.AncientofLore, Cards.BigGameHunter, Cards.ForceofNature, Cards.ForceofNature, Cards.AncientofWar, Cards.AzureDrake, Cards.AzureDrake, Cards.WildGrowth, Cards.WildGrowth, Cards.SavageRoar, Cards.SavageRoar, Cards.KeeperoftheGrove, Cards.KeeperoftheGrove, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.Wrath, Cards.ShadeofNaxxramas, Cards.ShadeofNaxxramas, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.EmperorThaurissan, Cards.DarnassusAspirant, Cards.DarnassusAspirant, Cards.LivingRoots,};
                        List<Card.Cards> rampDruid2 = new List<Card.Cards> {Cards.AncientofLore, Cards.AncientofLore, Cards.BigGameHunter, Cards.AncientofWar, Cards.AncientofWar, Cards.WildGrowth, Cards.WildGrowth, Cards.KeeperoftheGrove, Cards.KeeperoftheGrove, Cards.RagnarostheFirelord, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.Wrath, Cards.ZombieChow, Cards.ZombieChow, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.DrBoom, Cards.EmperorThaurissan, Cards.DruidoftheFlame, Cards.DruidoftheFlame, Cards.DarnassusAspirant, Cards.DarnassusAspirant, Cards.MasterJouster, Cards.MasterJouster,};
                        List<Card.Cards> rampDruid3 = new List<Card.Cards> {Cards.AncientofLore, Cards.AncientofLore, Cards.BigGameHunter, Cards.Sunwalker, Cards.AncientofWar, Cards.AncientofWar, Cards.AzureDrake, Cards.WildGrowth, Cards.WildGrowth, Cards.SenjinShieldmasta, Cards.SenjinShieldmasta, Cards.MindControlTech, Cards.KeeperoftheGrove, Cards.KeeperoftheGrove, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.Wrath, Cards.ZombieChow, Cards.KelThuzad, Cards.Loatheb, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.Deathlord, Cards.DrBoom, Cards.EmperorThaurissan,};
                        deckDictionary.AddOrUpdate(DeckType.RampDruid, CurrentDeck.Intersect(rampDruid).Count());
                        deckDictionary.AddOrUpdate(DeckType.RampDruid, CurrentDeck.Intersect(rampDruid2).Count());
                        deckDictionary.AddOrUpdate(DeckType.RampDruid, CurrentDeck.Intersect(rampDruid3).Count());
                    }
                    if (CurrentDeck.ContainsSome(Cards.LeperGnome, Cards.MountedRaptor, Cards.FelReaver, Cards.DruidoftheSaber, Cards.KnifeJuggler))
                    {
                        List<Card.Cards> aggroDruid = new List<Card.Cards> {Cards.ForceofNature, Cards.ForceofNature, Cards.SavageRoar, Cards.SavageRoar, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.KeeperoftheGrove, Cards.KeeperoftheGrove, Cards.LeperGnome, Cards.LeperGnome, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.ShadeofNaxxramas, Cards.ShadeofNaxxramas, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.FelReaver, Cards.FelReaver, Cards.SavageCombatant, Cards.DarnassusAspirant, Cards.DarnassusAspirant, Cards.DruidoftheSaber, Cards.DruidoftheSaber, Cards.LivingRoots, Cards.LivingRoots,};
                        deckDictionary.AddOrUpdate(DeckType.AggroDruid, CurrentDeck.Intersect(aggroDruid).Count());
                    }
                    if (CurrentDeck.ContainsAll(Cards.ForceofNature, Cards.SavageRoar) || CurrentDeck.ContainsSome(Cards.DarnassusAspirant, Cards.ShadeofNaxxramas, Cards.WildGrowth, Cards.Innervate, Cards.Wrath))
                    {
                        List<Card.Cards> midRangeDruid = new List<Card.Cards> {Cards.AncientofLore, Cards.AncientofLore, Cards.BigGameHunter, Cards.ForceofNature, Cards.ForceofNature, Cards.AzureDrake, Cards.AzureDrake, Cards.WildGrowth, Cards.WildGrowth, Cards.SavageRoar, Cards.SavageRoar, Cards.KeeperoftheGrove, Cards.KeeperoftheGrove, Cards.Innervate, Cards.Innervate, Cards.DruidoftheClaw, Cards.DruidoftheClaw, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.Wrath, Cards.ShadeofNaxxramas, Cards.Loatheb, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.EmperorThaurissan, Cards.DarnassusAspirant, Cards.LivingRoots, Cards.LivingRoots,};
                        deckDictionary.AddOrUpdate(DeckType.MidRangeDruid, CurrentDeck.Intersect(midRangeDruid).Count());
                    }
                    deckDictionary.AddOrUpdate(DeckType.TokenDruid, CurrentDeck.Intersect(tokenDruid).Count()); //EGG 
                    if (CurrentDeck.ContainsSome(Cards.AncientWatcher, Cards.EerieStatue, Cards.WailingSoul))
                    {
                        List<Card.Cards> silenceDruid = new List<Card.Cards> {Cards.AncientWatcher, Cards.AncientWatcher, Cards.ForceofNature, Cards.ForceofNature, Cards.AzureDrake, Cards.AzureDrake, Cards.SavageRoar, Cards.SavageRoar, Cards.KeeperoftheGrove, Cards.KeeperoftheGrove, Cards.IronbeakOwl, Cards.IronbeakOwl, Cards.Innervate, Cards.Innervate, Cards.Swipe, Cards.Swipe, Cards.Wrath, Cards.Wrath, Cards.Deathlord, Cards.Deathlord, Cards.WailingSoul, Cards.WailingSoul, Cards.DrBoom, Cards.FelReaver, Cards.FelReaver, Cards.DarnassusAspirant, Cards.DarnassusAspirant, Cards.Mulch, Cards.EerieStatue, Cards.EerieStatue,};
                        deckDictionary.AddOrUpdate(DeckType.SilenceDruid, CurrentDeck.Intersect(silenceDruid).Count());
                    }

                    break;

                    #endregion
            }
            var bestDeck = deckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
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

        private void Log(string str, int location = 0)
        {
            string file = location != 0 ? "MatchHistory.txt" : "MidgameIdentificationLog.txt";
            using (StreamWriter logfile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\SmartTracker\\" + file, true))
            {
                logfile.WriteLine(str);
            }
        }


        private static void CheckDirectory(string subdir)
        {
            if (Directory.Exists(subdir))
                return;
            Directory.CreateDirectory(subdir);
        }
    }

    public class DeckData
    {
        public DeckType DeckType { get; set; }
        public Style DeckStyle { get; set; }
        public List<Card.Cards> DeckList { get; set; }
    }


    public enum DeckType
    {
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
        TauntWarrior,
        /*Paladin*/
        SecretPaladin,
        MidRangePaladin,
        DragonPaladin,
        AggroPaladin,
        AnyfinMurglMurgl,
        RenoPaladin,
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
        /*Warlock*/
        Handlock,
        RenoLock,
        RenoComboLock,
        Zoolock,
        RelinquaryZoo,
        DemonHandlock,
        DemonZooWarlock,
        DragonHandlock,
        MalyLock,
        ControlWarlock,
        /*Mage*/
        TempoMage,
        FreezeMage,
        FaceFreezeMage,
        DragonMage,
        MechMage,
        EchoMage,
        FatigueMage,
        RenoMage,
        /*Priest*/
        DragonPriest,
        ControlPriest,
        ComboPriest,
        MechPriest,
        ShadowPriest,
        /*Huntard*/
        MidRangeHunter,
        HybridHunter,
        FaceHunter,
        HatHunter,
        CamelHunter,
        RenoHunter,
        DragonHunter,
        /*Rogue*/
        OilRogue,
        PirateRogue,
        FaceRogue,
        MalyRogue,
        RaptorRogue,
        FatigueRogue,
        MiracleRogue,
        MechRogue,
        RenoRogue,
        MillRogue,
        /*Chaman*/
        FaceShaman,
        MechShaman,
        DragonShaman,
        TotemShaman,
        MalygosShaman,
        ControlShaman,
        BloodlustShaman,
        BattleryShaman,
        RenoShaman,

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
}
