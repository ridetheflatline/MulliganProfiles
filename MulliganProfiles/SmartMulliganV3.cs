using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartBot.Mulligan;
using SmartBot.Plugins;
using SmartBot.Plugins.API;

//Version ~3.01



namespace MulliganProfiles
{
    public static class Extension
    {
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }

        public static bool Exist<TObject>(this IList<TObject> deck, TObject race)
        {
            return deck.Contains(race);
        }
    }
    
    public class GameContainer 

    {

        public DeckType MyDeckType { get; set; }
        public Style MyStyle;
        public List<Card.Cards> MyDeck;
        public DeckType EneDeckType;
        public Style EnemyStyle;
        public List<Card.Cards> EnemyDeck;
        public Dictionary<Dictionary<DeckType, Style>, List<Card.Cards>> EnemyHistory;
        public long UniqueUlong;

        public GameContainer()
        {
            Plugin tracker = Bot.GetPlugins().Find(plugin => plugin.DataContainer.Name == "SmartTracker");
            if (!tracker.DataContainer.Enabled)
            {
                Bot.Log("[SmartMulliganV3] This mulligan relies on having SmartTracker active at all times in order to function. Please enable SmartTracker, or chose different mulligan");
                Bot.StopBot();
            }
            Dictionary<string, object> properties = tracker.GetProperties();
            if (Enum.GetNames(typeof (DeckType)).Length != (int) properties["synchEnums"])
            {
                Bot.Log("[URGENT!!!!] Arthur, your enums are out of synch");
            }
            MyDeckType = properties["mode"].ToString() == "Manual" 
                    ? (DeckType) properties["ForceDeckType"]
                    : (DeckType)properties["AutoRecognizeDeckType"]; //if Manual
            
            if (properties["mode"].ToString() == "Manual")
                Bot.Log("[SmartMulliganV3] Dear friends, I notice that you are forcing deck recognition." +
                        " I do not make failsafe checks on whether or not you are using a proper deck, " +
                        "so if you decide to force Camel Hunter while playing FaceFreeze mage. It will let you. I hope you know what you are doing.");
                Bot.Log(string.Format("[You chose {0} Detection] {1} [Default: AutoDetection] {2}", properties["mode"], MyDeckType, properties["AutoRecognizeDeckType"]));

            MyStyle = DeckStyles[MyDeckType];
        }
        public readonly Dictionary<DeckType, Style> DeckStyles = new Dictionary<DeckType, Style>
        {

            {DeckType.Unknown, Style.Unknown},
            {DeckType.Arena, Style.Control},
            /*Warrior*/
            {DeckType.ControlWarrior, Style.Control},
            {DeckType.FatigueWarrior, Style.Control},
            {DeckType.DragonWarrior, Style.Control},
            {DeckType.PatronWarrior, Style.Tempo},
            {DeckType.WorgenOTKWarrior, Style.Combo},
            {DeckType.MechWarrior, Style.Aggro},
            {DeckType.FaceWarrior, Style.Face},
            /*Paladin*/
            {DeckType.SecretPaladin, Style.Tempo},
            {DeckType.MidRangePaladin, Style.Control},
            {DeckType.DragonPaladin, Style.Control},
            {DeckType.AggroPaladin, Style.Aggro},
            {DeckType.AnyfinMurglMurgl, Style.Combo},
            /*Druid*/
            {DeckType.RampDruid, Style.Control},
            {DeckType.AggroDruid, Style.Aggro},
            {DeckType.DragonDruid, Style.Control},
            {DeckType.MidRangeDruid, Style.Combo},
            {DeckType.TokenDruid, Style.Tempo},
            {DeckType.SilenceDruid,Style.Control},
            {DeckType.MechDruid,Style.Combo},
            {DeckType.AstralDruid,Style.Control},
            /*Warlock*/
            {DeckType.Handlock, Style.Control},
            {DeckType.RenoLock, Style.Control},
            {DeckType.Zoolock, Style.Tempo},//Same handler as flood zoo and reliquary
            {DeckType.DemonHandlock, Style.Control},
            {DeckType.DemonZooWarlock, Style.Tempo},
            {DeckType.DragonHandlock, Style.Control},
            {DeckType.MalyLock, Style.Combo},
            {DeckType.RenoComboLock, Style.Combo},
            /*Mage*/
            {DeckType.TempoMage, Style.Tempo},
            {DeckType.FreezeMage, Style.Control},
            {DeckType.FaceFreezeMage, Style.Aggro},
            {DeckType.DragonMage, Style.Control},
            {DeckType.MechMage, Style.Aggro},
            {DeckType.EchoMage, Style.Control},
            {DeckType.FatigueMage, Style.Control},
            /*Priest*/
            {DeckType.DragonPriest, Style.Tempo},
            {DeckType.ControlPriest, Style.Control},
            {DeckType.ComboPriest, Style.Combo},
            {DeckType.MechPriest, Style.Aggro},
            {DeckType.ShadowPriest, Style.Combo},
            /*Hunter*/
            {DeckType.MidRangeHunter, Style.Tempo},
            {DeckType.HybridHunter, Style.Aggro},
            {DeckType.FaceHunter, Style.Face},
            {DeckType.HatHunter, Style.Control},
            {DeckType.CamelHunter, Style.Control },
            /*Rogue*/
            {DeckType.OilRogue, Style.Combo},
            {DeckType.PirateRogue, Style.Aggro},
            {DeckType.FaceRogue, Style.Face},
            {DeckType.MalyRogue, Style.Combo},
            {DeckType.RaptorRogue, Style.Tempo},
            {DeckType.FatigueRogue, Style.Combo},
            {DeckType.MiracleRogue, Style.Combo},
            /*Cance... I mean Shaman*/
            {DeckType.FaceShaman, Style.Face},
            {DeckType.MechShaman, Style.Aggro},
            {DeckType.DragonShaman, Style.Control},
            {DeckType.TotemShaman, Style.Tempo},
            {DeckType.MalygosShaman, Style.Combo},
            {DeckType.ControlShaman, Style.Control},
            {DeckType.BloodlustShaman, Style.Combo},
            {DeckType.BattleryShaman, Style.Control }, //TODO: me
            /*Poor Kids*/
            {DeckType.Basic, Style.Tempo}
        };
    }
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class SmartMulliganV3 : MulliganProfile
    {

        private readonly string MulliganDir = AppDomain.CurrentDomain.BaseDirectory + "MulliganProfiles\\";
        private readonly string MulliganInformation = AppDomain.CurrentDomain.BaseDirectory + "MulliganProfiles\\SmartMulliganV3\\";
        private readonly string TrackerDir = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\";
        private readonly string TrackerVersion = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\SmartTracker\\";



        #region variables



        public static List<string> CurrentDeck = new List<string>();


        private readonly Dictionary<Card.Cards, bool> _whiteList; // CardName, KeepDouble
        private readonly List<Card.Cards> _cardsToKeep;

        #endregion

        public SmartMulliganV3()
        {

            _whiteList = new Dictionary<Card.Cards, bool>
            {
                { Card.Cards.GAME_005, true },//coin
                { Cards.Innervate, true },
                { Cards.WildGrowth, false }
            };

            _cardsToKeep = new List<Card.Cards>();
        }


        public List<Card.Cards> HandleMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {

            GameContainer newGame = new GameContainer();
            _whiteList.AddOrUpdate(Cards.AcolyteofPain, false);
            //Bot.Log(string.Format("My deck is {0} style {1} ", newGame.MyDeckType, newGame.MyStyle));
            
            


            //HandleMinions(choices, _whiteList, myInfo);
            foreach (var s in from s in choices
                                  let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString())
                                  where _whiteList.ContainsKey(s)
                                  where !keptOneAlready | _whiteList[s]
                                  select s)
                    _cardsToKeep.Add(s);

            return _cardsToKeep;
        }

        private void ParseOpponentInformation(GameContainer newGame)
        {
            try
            {
                using (StreamReader opponentReader = new StreamReader(MulliganInformation + "OpponentDeckInfo.txt"))

                {
                    string line;
                    while ((line = opponentReader.ReadLine()) != null)
                    {
                        string[] check = line.Split(':');
                        if (check[1] != Bot.GetCurrentOpponentId().ToString()) continue;
                        newGame.UniqueUlong = Bot.GetCurrentOpponentId();
                        var dictionaryList = new Dictionary<DeckType, Style>
                        {
                            {
                                (DeckType) Enum.Parse(typeof (DeckType), check[2]),
                                (Style) Enum.Parse(typeof (Style), check[3])
                            }
                        };
                        newGame.EnemyHistory.AddOrUpdate(dictionaryList, ParseOpponentList(check));
                    }
                    if (newGame.UniqueUlong != 200 || newGame.UniqueUlong != 0)
                    {
                        Bot.Log("[Tracker] You have played this opponents before, he played");
                        foreach (var q in newGame.EnemyHistory)
                        {
                            Bot.Log(string.Format("{0}---{1}", q.Value, q.Key));
                        }
                        Bot.Log(
                            string.Format("[Tracker] SmartMulligan is making an assumption that your opponent is {0}",
                                newGame.EnemyHistory.First().Key));

                        newGame.EneDeckType = newGame.EnemyHistory.First().Key.First().Key;
                        newGame.EnemyStyle = newGame.EnemyHistory.First().Key.First().Value;
                        newGame.EnemyDeck = newGame.EnemyHistory.First().Value;
                    }
                    else
                    {
                        Bot.Log("[Tracker] This is the first time you are facing this opponent");
                    }

                }
            }
            catch (Exception e)
            {
                Bot.Log(e.Message);
            }
            /*foreach(var q in newGame.EnemyDeck)
                Bot.Log(q.ToString());*/
        }

      
        /// <summary>
        /// TODO: DO ME PLEASE YOU LAZY FUCK
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        private List<Card.Cards> ParseOpponentList(string[] check)
        {
           return new List<Card.Cards>();
        }

        public int CountCards(Card.Cards card)
        {
            return CurrentDeck.Count(c => c == card.ToString());
        }
       


    }
    #region enums

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
        /*Paladin*/
        SecretPaladin,
        MidRangePaladin,
        DragonPaladin,
        AggroPaladin,
        AnyfinMurglMurgl,
        /*Druid*/
        RampDruid,
        AggroDruid,
        DragonDruid,
        MidRangeDruid,
        TokenDruid,
        SilenceDruid,
        MechDruid,
        AstralDruid,
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
        /*Mage*/
        TempoMage,
        FreezeMage,
        FaceFreezeMage,
        DragonMage,
        MechMage,
        EchoMage,
        FatigueMage,
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
        /**/
        OilRogue,
        PirateRogue,
        FaceRogue,
        MalyRogue,
        RaptorRogue,
        FatigueRogue,
        MiracleRogue,
        /**/
        FaceShaman,
        MechShaman,
        DragonShaman,
        TotemShaman,
        MalygosShaman,
        ControlShaman,
        BloodlustShaman,
        BattleryShaman,

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
        Tempo
    }
    #endregion
}

