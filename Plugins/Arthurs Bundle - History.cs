using SmartBot.Plugins.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Bundle;
using SmartBot.Database;
using SmartBot.Plugins;

namespace SmartBot.Plugins
{
    public static class Extension
    {
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }

        public static bool IsArena(this Bot.Mode mode)
        {
            return mode == Bot.Mode.Arena || mode == Bot.Mode.ArenaAuto;
        }
    }
    [Serializable]
    public class ABHistory : PluginDataContainer
    {
        [DisplayName("[--] First Time User")]
        public bool ftu { get; set; }
        [DisplayName("[A] Games to Analyze")]
        public int GTA { get; set; }
        [DisplayName("[B] Record Game Time")]
        public bool RGT { get; set; }
        [DisplayName("[A] Print Summary on start")]
        public bool print { get; set; }
        [DisplayName("[A] OnStart Summary Format")]
        public Details details { get; set; }           
        [DisplayName("[CSV] ")]
        public string message { get; private set; }
        [DisplayName("[CSV] Druid")]
        public bool Druid { get; set; }
        [DisplayName("[CSV] Hunter")]
        public bool Hunter { get; set; }
        [DisplayName("[CSV] Mage")]
        public bool Mage { get; set; }
        [DisplayName("[CSV] Paladin")]
        public bool Paladin { get; set; }
        [DisplayName("[CSV] Priest")]
        public bool Priest { get; set; }
        [DisplayName("[CSV] Rogue")]
        public bool Rogue { get; set; }
        [DisplayName("[CSV] Shaman")]
        public bool Shaman { get; set; }
        [DisplayName("[CSV] Warlock")]
        public bool Warlock { get; set; }
        [DisplayName("[CSV] Warrior")]
        public bool Warrior { get; set; }
        [Browsable(false)]
        [DisplayName("[CSV] z vs Make DeckType Spreadsheet")]
        public bool CVSDeckTypes { get; set; }
        [Browsable(false)]
        [DisplayName("[CSV] z vs DeckType")]
        public DeckType CVSDeckType { get; set; }

        
        
        public ABHistory()
        {
            Name = "Arthurs Bundle - History";
            message = "Chose classes against which you want spreadsheets created";
            GTA = 50;
            RGT = true;
            print = true;
            CVSDeckType = DeckType.ControlWarrior;
            CVSDeckTypes = false;
            details = Details.Classes;
            Enabled = true;
        }
    }
    public class HistoryDisplay : Plugin
    {
        private BundleData bd = null;
        public override void OnPluginCreated()
        {
            base.OnPluginCreated();
        }
        public override void OnStarted()
        {
            base.OnStarted();
            
            bd = new BundleData(Bot.Mode.RankedStandard);
            
            var data = DataContainer as ABHistory;
            if (data.ftu)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("All Card Breakdowns are stored in\n\n"
                      + "{0}\n\nFormat is Comma Seperated Values which can be opened with your Microsoft Excel or similar tools.\nPS: No, I will not print them to logs because they are huge. "
                      , AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\CardBreakdown\\"));
                data.ftu = false;
            }
            if (data.print)
            {
                Bot.Log(string.Format("Overall Winrate for the past {0} gamse is {1}", data.GTA, bd.MatchHistory.GetTotalWinrate()));
                if (((ABHistory)DataContainer).details == Details.Classes)
                    Bot.Log(bd.MatchHistory.ClassPerformance());
                else if (((ABHistory)DataContainer).details == Details.DeckTypes)
                    Bot.Log(bd.MatchHistory.DeckTypePerformance());
                else
                {
                    Bot.Log(bd.MatchHistory.ClassPerformance());
                    Bot.Log(bd.MatchHistory.DeckTypePerformance());
                };
            }
            bd.CardStat.ToCSV(); //default is always generated
            Dictionary<Card.CClass, bool> GenerateClassStats = new Dictionary<Card.CClass, bool>
            {
                {Card.CClass.SHAMAN, data.Shaman },
                {Card.CClass.DRUID, data.Druid },
                {Card.CClass.MAGE, data.Mage },

                {Card.CClass.WARLOCK, data.Warlock },
                {Card.CClass.WARRIOR, data.Warrior },
                {Card.CClass.ROGUE, data.Rogue },

                {Card.CClass.PRIEST, data.Priest },
                {Card.CClass.HUNTER, data.Hunter },
                {Card.CClass.PALADIN, data.Paladin },
            };
            foreach(var q in GenerateClassStats.Where(c=> c.Value == true))
            {
                Bot.Log("[Bundle History] Generated CSV card performance against " + q.Key);
                bd.MatchHistory.PerformanceAgainstClass[q.Key].ToCSV(q.Key.ToString());
            }
           
            

            

            
        }
    }
    public enum Details
    {
        Classes,
        DeckTypes,
        Both,
    }
    public enum DetailsVsClasses
    {
        All,
        Shaman, Warrior, Rogue, Mage, Druid, Hunter, Warlock, Priest, Paladin
    }
}
namespace Bundle
{
    public static class BundleExtension
    {
        public static Dictionary<string, object> GetData(this List<Plugin> list, string pName)
        {
            return list.Find(c => c.DataContainer.Name == pName).GetProperties();
        }
        public static bool IsArena(this Bot.Mode mode)
        {
            return mode == Bot.Mode.Arena || mode == Bot.Mode.ArenaAuto;
        }
        public static bool IsRushDeck(this Style st)
        {
            return st == Style.Aggro || st == Style.Face;
        }
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }
        public static List<Card.Cards> ToCardList(this List<string> list)
        {
            return list.Select(card => (Card.Cards)Enum.Parse(typeof(Card.Cards), card)).ToList();
        }
        public static String Name(this Card.Cards card)
        {
            return CardTemplate.LoadFromId(card).Name.ToString();
        }

    }
    [Serializable]
    public class BundleData
    {
        
        public Tracker TrackerData { get; private set; }
        public SirFinley SirFinleyPriority { get; private set; }
        public History MatchHistory { get; private set; }
        public CardStat CardStat { get; set; }
        public BundleData(DeckType param)
        {
            TrackerData = new Tracker();
            SirFinleyPriority = new SirFinley();
            MatchHistory = new History();
            CardStat = new CardStat(param);


        }
        public BundleData(Card.CClass param)
        {
            TrackerData = new Tracker();
            SirFinleyPriority = new SirFinley();
            MatchHistory = new History();
            CardStat = new CardStat(param);


        }
        public BundleData(Bot.Mode mode)
        {
            TrackerData = new Tracker();
            SirFinleyPriority = new SirFinley();
            MatchHistory = new History(mode);
            CardStat = new CardStat(mode);
        }
        public BundleData()
        {
            TrackerData = new Tracker();

            SirFinleyPriority = new SirFinley();
            MatchHistory = new History();
            CardStat = new CardStat();


        }
    }
    [Serializable]
    public class Tracker
    {
        /// <summary>
        /// Selecter tracker update mode {Soft:keeps custom mulligan, Hard:ignores any custom changes}
        /// </summary>
        public Update TrackerUpdateMode { get; private set; }
        /// <summary>
        /// Manually selected friendly DeckType
        /// </summary>
        public DeckType ManualDeckType { get; private set; }
        /// <summary>
        /// Manually selected friendly Style
        /// </summary>
        public Style ManualStyle { get; private set; }
        /// <summary>
        /// Automatically detected Deck Archetype by tracker based on currently selected deck
        /// </summary>
        public DeckType AutomaticFriendlyDeckType { get; private set; }
        /// <summary>
        /// Automatically deteceted friendly Style according to tracker.
        /// </summary>
        public Style AutomaticFriendlyStyle { get; private set; }
        /// <summary>
        /// Automatically detected enemy Deck Archetype according to tracker
        /// </summary>
        public DeckType DetectedEnemyDeckType { get; private set; }
        /// <summary>
        /// Automatically detected enemy Style according to tracker 
        /// </summary>
        public Style DetectedEnemyStyle { get; private set; }
        /// <summary>
        /// Total number of defined Deck Archetypes
        /// </summary>
        public int TotalNumberOfPossibleDecks { get; private set; }
        /// <summary>
        /// Returns style of our arena deck
        /// </summary>
        public Style ArenaStyle { get; private set; }
        public Tracker()
        {
            var data = Bot.GetPlugins().GetData("Arthurs Bundle - Tracker");
            TrackerUpdateMode = (Update)data["UpdateMode"];
            ManualDeckType = (DeckType)data["ForcedDeckType"];
            ManualStyle = (Style)data["ForcedStyle"];
            AutomaticFriendlyDeckType = (DeckType)data["AutoFriendlyDeckType"];
            AutomaticFriendlyStyle = (Style)data["AutoFriendlyStyle"];
            DetectedEnemyDeckType = (DeckType)data["EnemyDeckTypeGuess"];
            DetectedEnemyStyle = (Style)data["EnemyDeckStyleGuess"];
            TotalNumberOfPossibleDecks = (int)data["SynchEnums"];
            ArenaStyle = (Style)data["ArenaStyle"];

        }
        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}",
                TrackerUpdateMode, ManualDeckType, ManualStyle, AutomaticFriendlyDeckType,
               AutomaticFriendlyStyle, DetectedEnemyDeckType, DetectedEnemyStyle, TotalNumberOfPossibleDecks);
        }
    }


    [Serializable]
    public class SirFinley
    {
        public int DruidHP { get; set; }
        public int HunterHP { get; set; }
        public int MageHP { get; set; }
        public int PaladinHP { get; set; }
        public int PriestHP { get; set; }
        public int RogueHP { get; set; }
        public int ShamanHP { get; set; }
        public int WarlockHP { get; set; }
        public int WarriorHP { get; set; }
        public Dictionary<Card.Cards, int> HeroPowerTable { get; set; }
        private const Card.Cards SteadyShot = Card.Cards.DS1h_292;
        private const Card.Cards Shapeshift = Card.Cards.CS2_017;
        private const Card.Cards LifeTap = Card.Cards.CS2_056;
        private const Card.Cards Fireblast = Card.Cards.CS2_034;
        private const Card.Cards Reinforce = Card.Cards.CS2_101;
        private const Card.Cards ArmorUp = Card.Cards.CS2_102;
        private const Card.Cards LesserHeal = Card.Cards.CS1h_001;
        private const Card.Cards DaggerMastery = Card.Cards.CS2_083b;
        private const Card.Cards TotemicCall = Card.Cards.CS2_049;
        public SirFinley()
        {
            var data = Bot.GetPlugins().GetData("Arthurs Bundle - Sir Mrrgglton");
            DruidHP = (int)data["DruidHP"];
            HunterHP = (int)data["HunterHP"];
            MageHP = (int)data["MageHP"];
            PaladinHP = (int)data["PaladinHP"];
            PriestHP = (int)data["PriestHP"];
            RogueHP = (int)data["RogueHP"];
            ShamanHP = (int)data["ShamanHP"];
            WarlockHP = (int)data["WarlockHP"];
            WarriorHP = (int)data["WarriorHP"];
            HeroPowerTable = new Dictionary<Card.Cards, int>
            {
               {Shapeshift, DruidHP },
               {SteadyShot, HunterHP },
               {Fireblast, MageHP },

               {Reinforce, PaladinHP },
               {LesserHeal, PriestHP },
               {DaggerMastery, RogueHP },

               {TotemicCall, ShamanHP },
               {LifeTap, WarlockHP },
               {ArmorUp, WarriorHP },
            };
        }

    }
    public class CardStat : Tracker
    {
        public int GamesToCheck { get; set; }
        public List<Card.Cards> CardDeck { get; set; }
        public Dictionary<Card.Cards, double> AverageCardWinRate { get; set; }
        public Dictionary<Card.Cards, int> AverageCardPlayed { get; private set; }
        public Dictionary<Card.Cards, int> AverageCardDrawn { get; private set; }
        public Dictionary<Card.Cards, int> CardVictories { get; private set; }
        public Dictionary<Card.Cards, int> CardDefeats { get; private set; }
        public int TotalGames { get; set; }
        private Bot.Mode mode { get; set; }
        private DeckType myDeck { get; set; }
        private double TotalWinrate { get; set; }
        private int TotalWins { get; set; }
        private int TotalLosses { get; set; }
        private double AverageCardsDrawn { get; set; }

        public CardStat(DeckType eDeckType, Bot.Mode mode = Bot.Mode.RankedStandard)
        {
            this.mode = mode;
            myDeck = AutomaticFriendlyDeckType;
            Report("ENTERED CONSTRUCTOR");
            Ini();

            SeeHistory(eDeckType, mode);
            Report("FINISHED HISTORY");

        }
        public CardStat(Card.CClass vsEnemy, Bot.Mode mode = Bot.Mode.RankedStandard)
        {
            this.mode = mode;
            myDeck = AutomaticFriendlyDeckType;
            Report("ENTERED CONSTRUCTOR");
            Ini();
            SeeHistory(vsEnemy, mode);
            Report("FINISHED HISTORY");

        }
        public CardStat(Bot.Mode mode = Bot.Mode.RankedStandard)
        {
            this.mode = mode;
            myDeck = AutomaticFriendlyDeckType;
            Report("ENTERED CONSTRUCTOR");
            Ini();
            SeeHistory(true, mode);
            Report("FINISHED HISTORY");

        }

        /// <summary>
        /// 
        /// info[0] = result
        /// info[1] = Mode
        /// info[2] = FriendlyClass
        /// info[3] = FriendlyDeckType
        /// info[4] = EnemyClass
        /// info[5] = EnemyDeckType
        /// info[6] = Cards drawn
        /// info[7] = Cards played
        /// 
        /// </summary>
        /// <param name="edt"></param>
        private void SeeHistory(object parameter, Bot.Mode mode = Bot.Mode.RankedStandard)
        {
            int lineCount = File.ReadLines(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\CardPerformance.txt").Count();
            GamesToCheck = lineCount < GamesToCheck ? lineCount : GamesToCheck;
            List<string> matches = File.ReadLines(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\CardPerformance.txt")
                .Reverse().Take(GamesToCheck).ToList();
            Report("Match List created");
            List<int> dCards = new List<int>();
            foreach (var q in matches)
            {
                string[] line = q.Split('~');
                Report("Split was fine");
                if (parameter is DeckType && line[5] != parameter.ToString())
                {
                    Report("no match " + parameter.ToString() + ";" + line[5]);
                    continue;
                }
                //check vs particular deck type

                else if (parameter is Card.CClass && line[4] != parameter.ToString())
                {
                    Report("no match " + parameter.ToString() + ";" + line[4]);
                    continue;
                }//check vs particular class 
                Report("DeckType and/or class match");
                if (line[1] != mode.ToString())
                {
                    Report("Mode didn't match " + line[1] + ";" + mode);
                    continue;
                }
                Report("DeckType Check or Class check was fine");
                if ((DeckType)Enum.Parse(typeof(DeckType), line[3]) != AutomaticFriendlyDeckType) continue; //check if it's my current deck
                Report("MyDeck check was fine");
                Report(line[6]);
                var drawn = line[6].Split(',').Skip(1).ToList().ToCardList(); //get all drawn Card.Cards
                Report("Drawn Created");
                if (drawn.Count < 1) continue;

                var played = line[7].Split(',').Skip(1).ToList().ToCardList(); //get all played Card.Cards
                Report("Played created");
                if (played.Count < 1) continue;
                Report("Pre card calculations set up");
                if (line[0] == "won") TotalWins++;
                else TotalLosses++;
                foreach (var card in CardDeck.Distinct())
                {
                    if (played.Contains(card)) AverageCardPlayed[card]++;
                    if (drawn.Contains(card)) AverageCardDrawn[card]++;
                    if (line[0] == "won" && (drawn.Contains(card) || played.Contains(card))) CardVictories[card]++;
                    else if (drawn.Contains(card) || played.Contains(card)) CardDefeats[card]++;
                }
                dCards.Add(drawn.Count);
                TotalGames++;

            }
            AverageCardsDrawn = (double) dCards.Sum()/TotalGames;
            TotalWinrate = ((double)TotalWins / (TotalWins + TotalLosses))*100;
            foreach (var card in CardDeck.Distinct())
            {
                double wr = (double)CardVictories[card] / (CardVictories[card] + CardDefeats[card]);
                AverageCardWinRate[card] = wr * 100;
            }
        }
        private void Ini()
        {
            TotalGames = 0;
            TotalWins = 0;
            TotalLosses = 0;
            TotalWinrate = 0.00;
            AverageCardWinRate = new Dictionary<Card.Cards, double>();
            AverageCardDrawn = new Dictionary<Card.Cards, int>();
            AverageCardPlayed = new Dictionary<Card.Cards, int>();
            CardDefeats = new Dictionary<Card.Cards, int>();
            CardVictories = new Dictionary<Card.Cards, int>();
            Report("INI ENTERED");
            GamesToCheck = (int)Bot.GetPlugins().GetData("Arthurs Bundle - History")["GTA"];
            Report("CamesToCheck accessed");
            CardDeck = Bot.CurrentDeck().Cards.ToCardList();
            Report("DeckSelected");
            foreach (var q in CardDeck)
            {
                Report(q.Name());
                try
                {
                    AverageCardWinRate.AddOrUpdate(q, 0.00);
                    AverageCardPlayed.AddOrUpdate(q, 0);
                    AverageCardDrawn.AddOrUpdate(q, 0);
                    CardVictories.AddOrUpdate(q, 0);
                    CardDefeats.AddOrUpdate(q, 0);
                }
                catch (Exception e)
                {
                    Report(e.Message);
                    continue;
                }
            }
            Report("INI IS FINE");
        }

        public Double CardWinrate(Card.Cards card)
        {
            return AverageCardWinRate[card];
        }
        public string DeckCardBreakdown(bool orderByDescending)
        {
            string returnStr = "===================" + GamesToCheck + "\nWinRate\tWon\tLost\tName";
            foreach (var q in orderByDescending
                ? CardDeck.OrderByDescending(c => AverageCardWinRate[c])
                : CardDeck.OrderBy(c => AverageCardWinRate[c]))
                returnStr += string.Format("\n[{0}%]\t{1}\t{2}\t{3}", AverageCardWinRate[q].ToString("#0.##"), CardVictories[q], CardDefeats[q], q.Name());
            return returnStr;
        }
        private static void CheckDirectory(string subdir)
        {
            if (Directory.Exists(subdir))
                return;
            Directory.CreateDirectory(subdir);
        }
        public void ToCSV(string param = "All")
        {

            CheckDirectory(AppDomain.CurrentDomain.BaseDirectory + "Logs\\ABTracker\\CardBreakdowns\\");
                
            using (StreamWriter log = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + string.Format("\\Logs\\ABTracker\\CardBreakdowns\\Vs{0}CardPerformanceChart.csv", param), false))
            {
                
                log.WriteLine("Mode,{0}", mode, AutomaticFriendlyDeckType, TotalGames);
                log.WriteLine("Deck,{0}", AutomaticFriendlyDeckType);
                log.WriteLine("Total Games,{0}", TotalGames);
                log.WriteLine("Winrate, {0}%,Against,{1}", TotalWinrate, param);
                log.WriteLine("Cards Drawn/Game,{0}\n", AverageCardsDrawn);
                log.WriteLine("Card Name, Played, Drawn, Victories, Defeats, Winrate, Deviation, Unplayed, Winrate with Unplayed");
                foreach (var q in CardDeck.Distinct())
                    log.WriteLine("{0},{1},{2},{3},{4},{5}%,{6},{7},{8}%", q.Name(), AverageCardPlayed[q], AverageCardDrawn[q], CardVictories[q],
                        CardDefeats[q], AverageCardWinRate[q].ToString("#0.00"), (AverageCardWinRate[q] - TotalWinrate).ToString("#0.##"),  TotalGames-AverageCardPlayed[q],
                        (((double)CardVictories[q]/(CardDefeats[q] +CardVictories[q] + (TotalGames-AverageCardPlayed[q])))*100).ToString("#0.##"));
            }

        }
        public static void Report(string msg)
        {
            return;
            //using (StreamWriter log = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\DebugProfileLog.txt", true))
            //{
            //    log.WriteLine("[{0}] {1}", DateTime.Now, msg);
            //}
        }

    }
    /// <summary>
    /// Only applicatble to Bot.CurrentDeck();
    /// </summary>
    [Serializable]
    public class History 
    {
        public int GamesToCheck { get; private set; }
        public List<Card.Cards> Deck { get; private set; }
        public Dictionary<Card.CClass, double> ClassWinRate { get; private set; }
        public Dictionary<Card.CClass, int> ClassVictories { get; set; }
        public Dictionary<Card.CClass, int> ClassDefeats { get; set; }

        public Dictionary<DeckType, double> DeckTypeWinRate { get; private set; }
        public Dictionary<DeckType, int> DeckTypeVictories { get; set; }
        public Dictionary<DeckType, int> DeckTypeDefeats { get; set; }
        public Dictionary<Card.CClass, CardStat> PerformanceAgainstClass { get; set; }
        public Dictionary<DeckType, CardStat> PerformanceAgainstDeckType { get; set; }
        public Dictionary<Bot.Mode, CardStat> PerformanceInMode { get; set; }
        public static double TotalWinrate { get; set; }


        public History(Bot.Mode cmode = Bot.Mode.RankedStandard)
        {
            GamesToCheck = (int)Bot.GetPlugins().GetData("Arthurs Bundle - History")["GTA"];
            Deck = Bot.CurrentDeck().Cards.Select(card => (Card.Cards)Enum.Parse(typeof(Card.Cards), card)).ToList();
            ClassWinRate = ParseClassWinrate(Deck, cmode);
            DeckTypeWinRate = ParseDeckTypeWinRate(Deck, cmode);
            //TODO: DeckType Performances
            PerformanceInMode = new Dictionary<Bot.Mode, CardStat>
            {
                { Bot.Mode.RankedStandard, new CardStat(Bot.Mode.RankedStandard)},
                { Bot.Mode.RankedWild, new CardStat(Bot.Mode.RankedWild)},
                //TODO: add rest of modes
            };
            PerformanceAgainstClass = new Dictionary<Card.CClass, CardStat>
            {
                {Card.CClass.MAGE, new CardStat(Card.CClass.MAGE, cmode)},
                {Card.CClass.DRUID, new CardStat(Card.CClass.DRUID, cmode)}, 
                {Card.CClass.SHAMAN, new CardStat(Card.CClass.SHAMAN, cmode)},

                {Card.CClass.WARRIOR, new CardStat(Card.CClass.WARRIOR, cmode)},
                {Card.CClass.WARLOCK, new CardStat(Card.CClass.WARLOCK, cmode)},
                {Card.CClass.ROGUE, new CardStat(Card.CClass.ROGUE, cmode)},

                {Card.CClass.PRIEST, new CardStat(Card.CClass.PRIEST, cmode)},
                {Card.CClass.PALADIN, new CardStat(Card.CClass.PALADIN, cmode)},
                {Card.CClass.HUNTER, new CardStat(Card.CClass.HUNTER, cmode)},
            };
            TotalWinrate = GetTotalWinrate();

        }
        public double GetTotalWinrate()
        {
            
            double vic = 0.00;
            double los = 0.00;
            foreach(var q in ClassVictories.Keys.ToList())
            {
                vic += ClassVictories[q];
            }
            foreach(var q in ClassDefeats.Keys.ToList())
            {
                los += ClassDefeats[q];
            }
            return (double) vic/((double) vic+(double) los);
        }
        /// <summary>
        /// COMPLETE
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="gta"></param>
        /// <returns></returns>
        private Dictionary<DeckType, double> ParseDeckTypeWinRate(List<Card.Cards> deck, Bot.Mode cmode)
        {

            List<DeckType> values = Enum.GetValues(typeof(DeckType)).Cast<DeckType>().ToList();
            Dictionary<DeckType, double> result = new Dictionary<DeckType, double>();
            DeckTypeDefeats = new Dictionary<DeckType, int>();
            DeckTypeVictories = new Dictionary<DeckType, int>();
           
            foreach (var q in values)
            {
                result[q] = 0.00;
                DeckTypeVictories[q] = 0;
                DeckTypeDefeats[q] = 0;
            }
            int lineCount = File.ReadLines(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\MatchHistory.txt").Count(c => c.Contains(cmode.ToString()));
            GamesToCheck = lineCount < GamesToCheck ? lineCount : GamesToCheck;
            List<string> matches = File.ReadLines(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\MatchHistory.txt")
                .Reverse().Where(c => c.Contains(cmode.ToString())).Take(GamesToCheck).ToList();
            foreach (var q in matches)
            {
                var information = q.Split(new[] { "||" }, StringSplitOptions.None);
                if (information[1] == "won") DeckTypeVictories[(DeckType)Enum.Parse(typeof(DeckType), information[8])]++;
                else DeckTypeDefeats[(DeckType)Enum.Parse(typeof(DeckType), information[8])]++;
            }
            foreach (var decktype in result.Keys.ToList())
            {
                double wr = (double)DeckTypeVictories[decktype] / (DeckTypeVictories[decktype] + DeckTypeDefeats[decktype]);
                result[decktype] = wr * 100;
            }
            return result;
        }
        /// <summary>
        /// COMPLETE
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="gta"></param>
        /// <returns></returns>
        private Dictionary<Card.CClass, double> ParseClassWinrate(List<Card.Cards> deck, Bot.Mode cmode)
        {
            Dictionary<Card.CClass, double> result = new Dictionary<Card.CClass, double>
            {
                {Card.CClass.DRUID,0.00},{Card.CClass.HUNTER,0.00},{Card.CClass.MAGE,0.00},
                {Card.CClass.PALADIN,0.00},{Card.CClass.PRIEST,0.00},{Card.CClass.ROGUE,0.00},
                {Card.CClass.SHAMAN,0.00},{Card.CClass.WARLOCK,0.00},{Card.CClass.WARRIOR,0.00}
            };
            int lineCount = File.ReadLines(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\MatchHistory.txt").
                Count(c => c.Contains(cmode.ToString()));
            GamesToCheck = lineCount < GamesToCheck ? lineCount : GamesToCheck;
            List<string> matches = File.ReadLines(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\MatchHistory.txt")
                .Reverse().Where(c => c.Contains(cmode.ToString())).Take(GamesToCheck)
                .ToList();
            ClassVictories = new Dictionary<Card.CClass, int>
            {
                {Card.CClass.DRUID,0},{Card.CClass.HUNTER,0},{Card.CClass.MAGE,0},
                {Card.CClass.PALADIN,0},{Card.CClass.PRIEST,0},{Card.CClass.ROGUE,0},
                {Card.CClass.SHAMAN,0},{Card.CClass.WARLOCK,0},{Card.CClass.WARRIOR,0}
            };
            ClassDefeats = new Dictionary<Card.CClass, int>
            {
                {Card.CClass.DRUID,0},{Card.CClass.HUNTER,0},{Card.CClass.MAGE,0},
                {Card.CClass.PALADIN,0},{Card.CClass.PRIEST,0},{Card.CClass.ROGUE,0},
                {Card.CClass.SHAMAN,0},{Card.CClass.WARLOCK,0},{Card.CClass.WARRIOR,0}
            };

            foreach (var q in matches)
            {
                var information = q.Split(new[] { "||" }, StringSplitOptions.None);
                if (information[1] == "won") ClassVictories[(Card.CClass)Enum.Parse(typeof(Card.CClass), information[7])]++;
                else ClassDefeats[(Card.CClass)Enum.Parse(typeof(Card.CClass), information[7])]++;
            }
            foreach (var Cclass in result.Keys.ToList())
            {
                double wr = (double)ClassVictories[Cclass] / (ClassVictories[Cclass] + ClassDefeats[Cclass]);
                result[Cclass] = wr * 100;
            }


            return result;
        }
        public string ClassPerformance()
        {
            string ret = "\n===================================\nWin Rate\t\tPlayed\tWon\tLost\tvs Class";
            foreach (var q in ClassWinRate.Keys.Where(wr => ClassWinRate[wr].ToString() != "NaN").ToList().OrderByDescending(c=> ClassVictories[c]+ ClassDefeats[c]))
                ret += string.Format("\n{0}%\t\t{1}\t{2}\t{3}\t{4}", ClassWinRate[q].ToString("#0.##"), ClassVictories[q]+ClassDefeats[q], ClassVictories[q], ClassDefeats[q], q);
            ret += "\n===================================\n";    
                
            return ret;
        }
        public string DeckTypePerformance()
        {
            string ret = "\n===================================\nWin Rate\t\tPlayed\tWon\tLost\tvs DeckType";
            foreach (var q in DeckTypeWinRate.Keys.Where(wr => DeckTypeWinRate[wr].ToString() != "NaN").OrderByDescending(ene => DeckTypeVictories[ene] + DeckTypeDefeats[ene]).ToList())
            {
               
                ret += string.Format("\n{0}%\t\t{1}\t{2}\t{3}\t{4}", DeckTypeWinRate[q].ToString("#0.##"), DeckTypeVictories[q] + DeckTypeDefeats[q],
                    DeckTypeVictories[q], DeckTypeDefeats[q], q);
                
            }
            ret += "\n===================================\n";    
            return ret;
            
        }    

    }
    public enum Update
    {
        Hard, Soft
    }
    public enum DeckType
    {
        [Browsable(false)]
        Custom,
        [Browsable(false)]
        Unknown,
        [Browsable(false)]
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
        Count,
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
}

