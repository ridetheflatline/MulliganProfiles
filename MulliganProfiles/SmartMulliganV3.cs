using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartBot.Database;
using SmartBot.Mulligan;
using SmartBot.Plugins.API;
//Version ~3.01


// ReSharper disable once CheckNamespace

namespace SmartMulliganV3
{
    public static class Extension
    {
        public static void AddOrUpdate<TKey, TValue>(
            this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }
    }

    public class Game
    {
        public DeckType MyDeckType;
        public Style MyStyle;
        public List<Card.Cards> MyDeck; 
        public DeckType EneDeckType;
        public Style EnemyStyle;
        public List<Card.Cards> EnemyDeck;
        public Dictionary<Dictionary<DeckType, Style>, List<Card.Cards>> EnemyHistory;  
        public long UniqueUlong;


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
            try
            {
                using (StreamReader deckTypeReader = new StreamReader(MulliganInformation + "our_deck.v3"))
                
                {

                    string str = deckTypeReader.ReadLine();
                    string[] info = str.Split('~');
                    //Bot.Log("BITCH ASS " +info[0]);
                    Game newGame = new Game
                    {
                        MyDeckType = (DeckType)Enum.Parse(typeof(DeckType), info[0]),
                        MyStyle = (Style)Enum.Parse(typeof(Style), info[1]),
                        MyDeck = Bot.CurrentDeck().Cards.Select(q => (Card.Cards)Enum.Parse(typeof(Card.Cards), q)).ToList(),
                        UniqueUlong = 200,

                    };
                    ParseOpponentInformation(newGame);
                    //==============Opponent stuff begins here

                   
                   
                }
            }
            catch (Exception e)
            {
                Bot.Log("You need to install SmartTracker " +e.Message);
            }
            //HandleMinions(choices, _whiteList, myInfo);
            foreach (var s in from s in choices
                                  let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString())
                                  where _whiteList.ContainsKey(s)
                                  where !keptOneAlready | _whiteList[s]
                                  select s)
                    _cardsToKeep.Add(s);

            return _cardsToKeep;
        }

        private void ParseOpponentInformation(Game newGame)
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
                        { (DeckType)Enum.Parse(typeof(DeckType), check[2]), (Style)Enum.Parse(typeof(Style), check[3]) }
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
                    Bot.Log(string.Format("[Tracker] SmartMulligan is making an assumption that your opponent is {0}", newGame.EnemyHistory.First().Key));
                  
                    newGame.EneDeckType = newGame.EnemyHistory.First().Key.First().Key;
                    newGame.EnemyStyle = newGame.EnemyHistory.First().Key.First().Value;
                    newGame.EnemyDeck = newGame.EnemyHistory.First().Value;
                }
                else
                {
                    Bot.Log("[Tracker] This is the first time you are facing this opponent");
                }
                
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
        /*Warlock*/
        Handlock,
        RenoLock,
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
        DragonHunter,
        FaceHunter,
        HatHunter,
        /**/
        OilRogue,
        PirateRogue,
        FaceRogue,
        MalyRogue,
        DragonRogue,
        RaptorRogue,
        FatigueRogue,
        /**/
        FaceShaman,
        MechShaman,
        DragonShaman,
        TotemShaman,
        MalygosShaman,
        ControlShaman,
        BloodlustShaman,
        Basic
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
