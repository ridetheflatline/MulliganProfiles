using SmartBot.Plugins.API;
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

        
        public static bool IsArena(this Bot.Mode mode)
        {
            return mode == Bot.Mode.Arena || mode == Bot.Mode.ArenaAuto;  
        }
    }
    [Serializable]
    public class Arthurs_Bundle___History : PluginDataContainer
    {
        [DisplayName("Games to Analyze")]
        public int GTA { get; set; }
        [DisplayName("Record Game Time")]
        public bool RGT { get; set; }
        [DisplayName("Print Summary on start")]
        public bool print { get; set; }
        [DisplayName("Detailed Summary")]
        public Details details { get; set; }

        public Arthurs_Bundle___History()
        {
            Name = "Arthurs Bundle - History";
            GTA = 50;
            RGT = true;
            print = true;
            details = Details.Minimal;
        }
    }
    public class HistoryDisplay : Plugin
    {
        public override void OnStarted()
        {
            CheckHistory();
            base.OnStarted();
        }
        public override void OnStopped()
        {
            try
            {
                Stats.First().Value.Reset();

            }
            catch (Exception)
            {
                //ignored
            }
            Stats.Clear();

            base.OnStopped();
        }
        public static Dictionary<long, List<DeckType>> EnemyHistory = new Dictionary<long, List<DeckType>>();
        private void CheckHistory()
        {
            int numGames = ((Arthurs_Bundle___History)DataContainer).GTA;
            int lineCount = File.ReadLines(
                AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\MatchHistory.txt").Count(c => c.Contains(Bot.CurrentMode().ToString()));
            numGames = lineCount < numGames ? lineCount : ((Arthurs_Bundle___History)DataContainer).GTA;
            if (!Bot.CurrentMode().IsArena())
                Bot.Log(string.Format("[ABTracker] Finished parsing {0} games in {1} mode", lineCount, Bot.CurrentMode()));
            List<string> text = File.ReadLines(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\MatchHistory.txt")
                .Reverse().Where(c => c.Contains(Bot.CurrentMode().ToString())).Take(numGames)
                .ToList();

            if (((Arthurs_Bundle___History)DataContainer).details == Details.Detailed)
            {
                foreach (var q in text)
                {
                    var information = q.Split(new[] { "||" }, StringSplitOptions.None);
                    DeckType dt = (DeckType)Enum.Parse(typeof(DeckType), information[8]);
                    if (information[5].ToString().Contains("Cards")) continue;
                    Card.CClass enemy = (Card.CClass)Enum.Parse(typeof(Card.CClass), information[7]);
                    if (Stats.ContainsKey(dt))
                    {
                        Stats[dt].UpdateData(dt, enemy, information[1]);
                    }
                    else
                    {
                        Stats.AddOrUpdate(dt, new Statistics(dt, enemy, information[1]));
                    }
                }
            }
            else
            {
                foreach (var q in text)
                {
                    var information = q.Split(new[] { "||" }, StringSplitOptions.None);
                    DeckType dt = (DeckType)Enum.Parse(typeof(DeckType), information[8]);
                    if (information[5].ToString().Contains("Cards")) continue;
                    if (Stats.ContainsKey(dt))
                    {
                        Stats[dt].UpdateData(dt, information[1]);
                    }
                    else
                    {
                        Stats.AddOrUpdate(dt, new Statistics(dt, information[1]));
                    }
                }
            }
            if (!((Arthurs_Bundle___History)DataContainer).print) return;
            try
            {
                PrintHistory();

            }
            catch (InvalidOperationException exception)
            {
                Bot.Log("[ABTracker] Your History is empty. If that is not true, blame Masterwai ||" + exception.Message);

            }

        }

        public Dictionary<DeckType, Statistics> Stats = new Dictionary<DeckType, Statistics>();
        public void PrintHistory()
        {
            Bot.Log("========================================");
            Bot.Log(string.Format("\tYour Stats for the past {0} games", ((Arthurs_Bundle___History)DataContainer).GTA));
            Bot.Log("");
            Bot.Log("Won\tPlayed\tWinrate\t\tEnemy Deck");
            Bot.Log("========================================");
            foreach (var q in Stats.OrderByDescending(c => c.Value.Played))
            {
                Bot.Log(q.Value.ToString());
            }
            Bot.Log("========================================");

            Bot.Log(string.Format("Cumulative winrate: {0}%", Stats.First().Value.ShowTotalWinrate().ToString("#0.###")));
            if (((Arthurs_Bundle___History)DataContainer).details == Details.Detailed)
            {
                Stats.First().Value.ClassSummary();
            }
            Bot.Log("========================================");
        }  //a

    }
    public class Statistics
    {
        public int Won { get; set; }
        public int Played { get; set; }
        public double Winrate { get; set; }
        public DeckType Type { get; set; }
        public static int cWon = 0;
        public static int cPlayed = 0;
        private static Dictionary<Card.CClass, int> VsCClassWins = new Dictionary<Card.CClass, int>
        {
            { Card.CClass.ROGUE, 0},  { Card.CClass.SHAMAN, 0}, { Card.CClass.PALADIN, 0}, { Card.CClass.MAGE, 0}, { Card.CClass.WARRIOR, 0}, { Card.CClass.WARLOCK, 0}, { Card.CClass.HUNTER, 0}, { Card.CClass.PRIEST, 0}, { Card.CClass.DRUID, 0},
        };
        private static Dictionary<Card.CClass, int> VsCClassPlayed = new Dictionary<Card.CClass, int>
        {
            { Card.CClass.ROGUE, 0},  { Card.CClass.SHAMAN, 0}, { Card.CClass.PALADIN, 0}, { Card.CClass.MAGE, 0}, { Card.CClass.WARRIOR, 0}, { Card.CClass.WARLOCK, 0}, { Card.CClass.HUNTER, 0}, { Card.CClass.PRIEST, 0}, { Card.CClass.DRUID, 0},
        };
        private List<Card.CClass> Classes = new List<Card.CClass>
        {
            Card.CClass.ROGUE,      Card.CClass.SHAMAN, Card.CClass.PALADIN, Card.CClass.MAGE, Card.CClass.WARRIOR, Card.CClass.WARLOCK, Card.CClass.HUNTER, Card.CClass.DRUID, Card.CClass.PRIEST
        };
        public Statistics(DeckType dt, string str)
        {
            Type = dt;
            Won = str == "won" ? 1 : 0;
            cWon += str == "won" ? 1 : 0;
            Played = 1;
            cPlayed++;
            Winrate = 0.00;
        }
        public Statistics(DeckType dt, Card.CClass enemy, string str)
        {
            Type = dt;
            Won = str == "won" ? 1 : 0;
            cWon += str == "won" ? 1 : 0;
            VsCClassWins[enemy] += str == "won" ? 1 : 0;
            VsCClassPlayed[enemy]++;
            Played = 1;
            cPlayed++;
            Winrate = 0.00;

        }
        public void UpdateData(DeckType dt, string str)
        {
            Played++;
            cPlayed++;
            if (str == "won")
            {
                Won++;
                cWon++;
            }

        }
        public void UpdateData(DeckType dt, Card.CClass enemy, string str)
        {
            Played++;
            cPlayed++;
            VsCClassPlayed[enemy]++;
            if (str == "won")
            {
                Won++;
                cWon++;
                VsCClassWins[enemy]++;
            }

        }

        public double ShowTotalWinrate()
        {
            return cWon / (double)cPlayed * 100;
        }

        public void Reset()
        {
            foreach (var q in Classes)
            {
                VsCClassPlayed[q] = 0;
                VsCClassWins[q] = 0;
                cWon = 0;
                cPlayed = 0;
            }
        }

        public void ClassSummary()
        {

            foreach (var q in VsCClassPlayed)
            {
                Bot.Log(string.Format("{0}\t\t{1}%", q, (((double)VsCClassWins[q.Key] / (double)q.Value) * 100).ToString("#0.##")));
            }
        }
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}%\t\t{3}", Won, Played, (((double)Won / (double)Played) * 100).ToString("#0.##"), Type);
        }
    }
    public enum Details
    {
        Minimal, Detailed
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
}

