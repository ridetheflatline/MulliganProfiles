using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SmartBot.Database;
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

        public static int TurnPlayCount(this IList<Card.Cards> deck, int turn)
        {
            return deck.Count(cards => CardTemplate.LoadFromId(cards).Cost == turn);
        }
        /// <summary>
        /// Item1 = number of minions
        /// Item2 = number of spells
        /// Item3 = number of weapons
        /// </summary>
        /// <param name="deck"></param>
        /// <returns></returns>
        public static Tuple<int,int,int> NumOfTypes(this IList<Card.Cards> deck)
        {
            return new Tuple<int, int, int>
            (
                deck.Count(cards => CardTemplate.LoadFromId(cards).Type == Card.CType.MINION),
                deck.Count(cards => CardTemplate.LoadFromId(cards).Type == Card.CType.SPELL),
                deck.Count(cards => CardTemplate.LoadFromId(cards).Type == Card.CType.WEAPON)
            );
        }
        public static Tuple<List<Card.Cards>, List<Card.Cards>, List<Card.Cards>, List<Card.Cards>, List<Card.Cards>> SplitByTurnList(this IList<Card.Cards> list)
        {
            return new Tuple<List<Card.Cards>, List<Card.Cards>, List<Card.Cards>, List<Card.Cards>, List<Card.Cards>>
                (
                    list.Where(card => CardTemplate.LoadFromId(card).Cost == 0).ToList(),
                    list.Where(card => CardTemplate.LoadFromId(card).Cost == 1).ToList(),
                    list.Where(card => CardTemplate.LoadFromId(card).Cost == 2).ToList(),
                    list.Where(card => CardTemplate.LoadFromId(card).Cost == 3).ToList(),
                    list.Where(card => CardTemplate.LoadFromId(card).Cost == 4).ToList()
                );
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

        private const string TrackerMyType = "AutoRecognizeDeckType";
        private const string TrackerMyStyle = "";
        private const string TrackerEnemyType = "";
        private const string TrackerEnemyStyle = "";
        public List<Card.Cards> Choices;
        public Card.CClass OpponentClass;
        public Card.CClass OwnClass;

        public GameContainer(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            this.Choices = choices;
            this.OpponentClass = opponentClass;
            this.OwnClass = ownClass;
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
                    ? (DeckType) properties["ForceDeckType"] //if Manual
                    : (DeckType) properties[TrackerMyType];  //if Auto
            
            if (properties["mode"].ToString() == "Manual")
                Bot.Log("[SmartMulliganV3] Dear friends, I notice that you are forcing deck recognition." +
                        " I do not make failsafe checks on whether or not you are using a proper deck, " +
                        "so if you decide to force Camel Hunter while playing FaceFreeze mage. It will let you. I hope you know what you are doing.");
                Bot.Log(string.Format("[You chose {0} Detection] {1} [Default: AutoDetection] {2}", properties["mode"], MyDeckType, properties["AutoRecognizeDeckType"]));

            MyStyle = (Style) properties[TrackerMyStyle];
            EneDeckType = (DeckType) properties[TrackerEnemyType];
            EnemyStyle = (Style) properties[TrackerEnemyStyle];
        }

        public GameContainer()
        {
           
        }
    }
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class SmartMulliganV3 : MulliganProfile
    {

      

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

            GameContainer newGame = new GameContainer(choices, opponentClass, ownClass);
            switch(newGame.MyDeckType)
            {
                case DeckType.Unknown:
                    break;
                case DeckType.Arena:
                    break;
                case DeckType.ControlWarrior:
                    break;
                case DeckType.FatigueWarrior:
                    break;
                case DeckType.DragonWarrior:
                    break;
                case DeckType.PatronWarrior:
                    break;
                case DeckType.WorgenOTKWarrior:
                    break;
                case DeckType.MechWarrior:
                    break;
                case DeckType.FaceWarrior:
                    break;
                case DeckType.SecretPaladin:
                    HandleSecretPaladin(newGame);
                    break;
                case DeckType.MidRangePaladin:
                    break;
                case DeckType.DragonPaladin:
                    break;
                case DeckType.AggroPaladin:
                    break;
                case DeckType.AnyfinMurglMurgl:
                    break;
                case DeckType.RampDruid:
                    break;
                case DeckType.AggroDruid:
                    break;
                case DeckType.DragonDruid:
                    break;
                case DeckType.MidRangeDruid:
                    break;
                case DeckType.TokenDruid:
                    break;
                case DeckType.SilenceDruid:
                    break;
                case DeckType.MechDruid:
                    break;
                case DeckType.AstralDruid:
                    break;
                case DeckType.Handlock:
                    break;
                case DeckType.RenoLock:
                    break;
                case DeckType.RenoComboLock:
                    break;
                case DeckType.Zoolock:
                    break;
                case DeckType.RelinquaryZoo:
                    break;
                case DeckType.DemonHandlock:
                    break;
                case DeckType.DemonZooWarlock:
                    break;
                case DeckType.DragonHandlock:
                    break;
                case DeckType.MalyLock:
                    break;
                case DeckType.TempoMage:
                    break;
                case DeckType.FreezeMage:
                    break;
                case DeckType.FaceFreezeMage:
                    break;
                case DeckType.DragonMage:
                    break;
                case DeckType.MechMage:
                    break;
                case DeckType.EchoMage:
                    break;
                case DeckType.FatigueMage:
                    break;
                case DeckType.DragonPriest:
                    break;
                case DeckType.ControlPriest:
                    break;
                case DeckType.ComboPriest:
                    break;
                case DeckType.MechPriest:
                    break;
                case DeckType.ShadowPriest:
                    break;
                case DeckType.MidRangeHunter:
                    break;
                case DeckType.HybridHunter:
                    break;
                case DeckType.FaceHunter:
                    break;
                case DeckType.HatHunter:
                    break;
                case DeckType.CamelHunter:
                    break;
                case DeckType.OilRogue:
                    break;
                case DeckType.PirateRogue:
                    break;
                case DeckType.FaceRogue:
                    break;
                case DeckType.MalyRogue:
                    break;
                case DeckType.RaptorRogue:
                    break;
                case DeckType.FatigueRogue:
                    break;
                case DeckType.MiracleRogue:
                    break;
                case DeckType.FaceShaman:
                    break;
                case DeckType.MechShaman:
                    break;
                case DeckType.DragonShaman:
                    break;
                case DeckType.TotemShaman:
                    break;
                case DeckType.MalygosShaman:
                    break;
                case DeckType.ControlShaman:
                    break;
                case DeckType.BloodlustShaman:
                    break;
                case DeckType.BattleryShaman:
                    break;
                case DeckType.Basic:
                    break;
               
            }


            //HandleMinions(choices, _whiteList, myInfo);
            foreach (var s in from s in choices let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString()) where _whiteList.ContainsKey(s) where !keptOneAlready | _whiteList[s] select s)
                _cardsToKeep.Add(s);

            return _cardsToKeep;
        }

        private void HandleSecretPaladin(GameContainer newGame)
        {
            //1 = minion || 2 = spells || 3 = weapons
            var information = newGame.Choices.NumOfTypes();
            switch (newGame.OpponentClass)
            {
                
            }
            
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
        RenoWarrior,
        TauntWarrior,
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
        MillDruid,
        BeastDruid,
        RenoDruid,
        RenoPaladin,
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

    #endregion
}

