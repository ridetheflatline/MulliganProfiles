using System;
using System.Collections.Generic;
using System.Linq;
using SmartBot.Database;
using SmartBot.Mulligan;
using SmartBot.Plugins;
using SmartBot.Plugins.API;

//Version ~3.01



namespace MulliganProfiles
{
    #region Extension class
    public static class Extension
    {
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }

        public static void AddAll<TKey, TValue>(this IDictionary<TKey, TValue> map, TValue value, params TKey[] keys)
        {
            foreach(TKey key in keys)
                map.AddOrUpdate(key, value);
        }

        public static void BlackListAll<TKey, TValue>(this IDictionary<TKey, TValue> map, params TKey[] keys)
        {
            foreach (TKey key in keys.Where(map.ContainsKey))
                map.Remove(key);
        }

        public static bool IsMinion(this Card.Cards card)
        {
            return CardTemplate.LoadFromId(card).Type == Card.CType.MINION;
        }

        public static bool IsSpell(this Card.Cards card)
        {
            return CardTemplate.LoadFromId(card).Type == Card.CType.SPELL;
        }

        public static bool IsWeapon(this Card.Cards card)
        {
            return CardTemplate.LoadFromId(card).Type == Card.CType.MINION;
        }
        public static int TurnPlayCount(this IList<Card.Cards> deck, int turn)
        {
            return deck.Count(cards => CardTemplate.LoadFromId(cards).Cost == turn);
        }
        /// <summary>
        /// This is a useless method
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
        /// <summary>
        /// this is a very useless method that I realized was useless only after I used it... once, like ever... It's sad, please don't code like this
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Tuple<List<Card.Cards>, List<Card.Cards>, List<Card.Cards>, List<Card.Cards>, List<Card.Cards>, List<Card.Cards>> SplitByTurnList(this IList<Card.Cards> list)
        {
            return new Tuple<List<Card.Cards>, List<Card.Cards>, List<Card.Cards>, List<Card.Cards>, List<Card.Cards>, List<Card.Cards>>
                (
                    list.Where(card => CardTemplate.LoadFromId(card).Cost == 0).ToList(), //all 0 drops
                    list.Where(card => CardTemplate.LoadFromId(card).Cost == 1).ToList(), //all 1 drops
                    list.Where(card => CardTemplate.LoadFromId(card).Cost == 2).ToList(), //all 2 drops
                    list.Where(card => CardTemplate.LoadFromId(card).Cost == 3).ToList(), //all 3 drops
                    list.Where(card => CardTemplate.LoadFromId(card).Cost == 4).ToList(), //all 4 drops
                    list.Where(card => CardTemplate.LoadFromId(card).Cost > 4).ToList()   //all 5+ drops
                );
        }

    }
    #endregion

    public class GameContainer 

    {

        public DeckType MyDeckType { get; set; }
        public Style MyStyle { get; set; }
        public List<Card.Cards> MyDeck { get; set; }
        public DeckType EneDeckType { get; set; }
        public Style EnemyStyle { get; set; }
        
        private const string Mode = "Mode";
        private const string TrackerMyType = "AutoFriendlyDeckType";
        private const string TrackerForceMyType = "ForcedDeckType";
        private const string TrackerMyStyle = "AutoFriendlyStyle";
        private const string TrackerEnemyType = "EnemyDeckTypeGuess";
        private const string TrackerEnemyStyle = "EnemyDeckStyleGuess";
        private const string MulliganTesterMyDeck = "MulliganTesterYourDeck";
        private const string MulliganTesterEnemyDeck = "MulliganTEsterEnemyDeck";
        private const string EnumsCount = "SynchEnums";
        public List<Card.Cards> Choices { get; set; }
        public Card.CClass OpponentClass { get; set; }
        public Card.CClass OwnClass { get; set; }

        /// <summary>
        /// All of the below drops originate from choices
        /// </summary>

        public List<Card.Cards> ZeroDrops { get; set; }
        public List<Card.Cards> OneDrops { get; set; }
        public List<Card.Cards> TwoDrops { get; set; }
        public List<Card.Cards> ThreeDrops { get; set; }
        public List<Card.Cards> FourDrops { get; set; }
        public List<Card.Cards> FivePlusDrops { get; set; }

        public bool HasTurnOne { get; set; }
        public bool HasTurnTwo { get; set; }
        public bool HasTurnThree { get; set; }
        
        public bool Coin { get; set; }
        public Tuple<List<Card.Cards>,
            List<Card.Cards>, List<Card.Cards>,
            List<Card.Cards>, List<Card.Cards>,
            List<Card.Cards>> AllDropsTuple { get; set; }




        public GameContainer(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            
            Choices = choices;
            OpponentClass = opponentClass;
            OwnClass = ownClass;
            Coin = choices.Count > 3;
            #region SmartTracker
            Plugin tracker = Bot.GetPlugins().Find(plugin => plugin.DataContainer.Name == "SmartTracker");

            if (!tracker.DataContainer.Enabled)
            {
                Bot.Log("[SmartMulliganV3] This mulligan relies on having SmartTracker active at all times in order to function. Please enable SmartTracker, or chose different mulligan");
                Bot.StopBot();
            }
            Dictionary<string, object> properties = tracker.GetProperties();
            //PrintDebug(properties);
            if (Enum.GetNames(typeof (DeckType)).Length != (int) properties[EnumsCount])
            {
                Bot.Log("[URGENT!!!!] Arthur, your enums are out of synch");
            }
            MyDeckType = properties[Mode].ToString() == "Manual" 
                    ? (DeckType) properties[TrackerForceMyType] //if Manual
                    : (DeckType) properties[TrackerMyType];  //if Auto
            
            if (properties[Mode].ToString() == "Manual")
                Bot.Log("[SmartMulliganV3] Dear friends, I notice that you are forcing deck recognition." +
                        " I do not make failsafe checks on whether or not you are using a proper deck, " +
                        "so if you decide to force Camel Hunter while playing FaceFreeze mage. It will let you. I hope you know what you are doing.");
                Bot.Log(string.Format("[You chose {0} Detection] {1} [Default: AutoDetection] {2}", properties[Mode], MyDeckType, properties[TrackerMyType]));
            #endregion 
            MyStyle = (Style) properties[TrackerMyStyle];
            EneDeckType = (DeckType) properties[TrackerEnemyType];
            EnemyStyle = (Style) properties[TrackerEnemyStyle];
            AllDropsTuple = choices.SplitByTurnList();
            ZeroDrops = AllDropsTuple.Item1;
            OneDrops = AllDropsTuple.Item2;
            TwoDrops = AllDropsTuple.Item3;
            ThreeDrops = AllDropsTuple.Item4;
            FourDrops = AllDropsTuple.Item5;
            FivePlusDrops = AllDropsTuple.Item6;
            HasTurnOne = false;
            HasTurnTwo = false;
            HasTurnThree = false;

        }

        private void PrintDebug(Dictionary<string, object> properties)
        {
        Bot.Log(string.Format("==============" +
                              "\nMy Type: {0}" +
                              "\nMy Forced Type: {1}" +
                              "\nMy Style: {2}" +
                              "\nEnemy Type: {3}" +
                              "\nEnemy Style: {4}" +
                              "\nMulligan Tester <me>: {5}" +
                              "\nMulligan Tester <him>: {6}" +
                              "\nEnum Count: {7}" +
                              "\t", properties[TrackerMyType], 
                              properties[TrackerForceMyType], 
                              properties[TrackerMyStyle], 
                              properties[TrackerEnemyType], 
                              properties[TrackerEnemyStyle],
                              properties[MulliganTesterMyDeck],
                              properties[MulliganTesterEnemyDeck],
                              properties[EnumsCount]) 
                              );
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
                { Card.Cards.GAME_005, true },  //coin
                { Cards.Innervate, true },      //always keeps 2 innervates (set false for 1)
                { Cards.WildGrowth, false }     //only keeps 1 wild growth
            };

            _cardsToKeep = new List<Card.Cards>();
        }


        public List<Card.Cards> HandleMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
           GameContainer gameContainer = new GameContainer(choices, opponentClass, ownClass);
            
            switch (gameContainer.MyDeckType)
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
                case DeckType.RenoWarrior:
                    break;
                case DeckType.TauntWarrior:
                    break;
                case DeckType.SecretPaladin:
                    HandleSecretPaladin(gameContainer);
                    break;
                case DeckType.MidRangePaladin:
                    break;
                case DeckType.DragonPaladin:
                    break;
                case DeckType.AggroPaladin:
                    break;
                case DeckType.AnyfinMurglMurgl:
                    break;
                case DeckType.RenoPaladin:
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
                case DeckType.MillDruid:
                    break;
                case DeckType.BeastDruid:
                    break;
                case DeckType.RenoDruid:
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
                case DeckType.ControlWarlock:
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
                case DeckType.RenoMage:
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
                case DeckType.RenoHunter:
                    break;
                case DeckType.DragonHunter:
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
                case DeckType.MechRogue:
                    break;
                case DeckType.RenoRogue:
                    break;
                case DeckType.MillRogue:
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
                case DeckType.RenoShaman:
                    break;
                case DeckType.Basic:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            
            //HandleMinions(choices, _whiteList, myInfo);
            foreach (var s in from s in choices let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString()) where _whiteList.ContainsKey(s) where !keptOneAlready | _whiteList[s] select s)
                _cardsToKeep.Add(s);

            return _cardsToKeep;
        }

        private void HandleSecretPaladin(GameContainer gameContainer)
        {
            Bot.Log("[Kappa] Entered Secret Paladin");
            foreach (var q in gameContainer.OneDrops.Where(card => card.IsMinion()))
            {
                _whiteList.AddOrUpdate(q, false);
                gameContainer.HasTurnOne = true;
            }
            foreach (var q in gameContainer.TwoDrops.Where(card => card.IsMinion()))
            {
                _whiteList.AddOrUpdate(q, false);
                gameContainer.HasTurnTwo = true;
            }
            foreach (var q in gameContainer.ThreeDrops.Where(card => card.IsMinion()))
            {
                if (q.IsSpell())
                    _whiteList.AddOrUpdate(q, false);
                if (!gameContainer.HasTurnTwo) continue; //continue instead of break in case our allowed 3 drop spell is later in choices
                _whiteList.AddOrUpdate(q, false);
                gameContainer.HasTurnThree = true;
            }
            foreach (var q in gameContainer.FourDrops.Where(card => card.IsMinion()))
                _whiteList.AddOrUpdate(q, false);
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

    #endregion
}

