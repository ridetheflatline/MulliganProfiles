using System;
using System.Collections.Generic;
using System.Linq;
using SmartBot.Plugins.API;
using SmartBotProfiles;

/* Explanation on profiles :
 * 
 * All the values defined in profiles are percentage modifiers, it means that it will affect base profile's default values.
 * 
 * Modifiers values can be set within the range (-1000 - 1000)  (negative modifier has the opposite effect)
 * You can specify targets for the non-global modifiers, these target specific modifers will be added on top of global modifier + modifier for the card (without target)
 * 
 * parameters.GlobalSpellsModifier ---> Modifier applied to all spells no matter what they are. The higher is the modifier, the less likely the AI will be to play the spell
 * parameters.GlobalMinionsModifier ---> Modifier applied to all minions no matter what they are. The higher is the modifier, the less likely the AI will be to play the minion
 * 
 * parameters.GlobalAggroModifier ---> Modifier applied to enemy's health value, the higher it is, the more aggressive will be the AI
 * parameters.GlobalDefenseModifier ---> Modifier applied to friendly's health value, the higher it is, the more hp conservative will be the AI
 * 
 * parameters.SpellsModifiers ---> You can set individual modifiers to each spells there, those are ADDED to the GLOBAL modifiers !!
 * parameters.MinionsModifiers ---> You can set individual modifiers to each minions there, those are ADDED to the GLOBAL modifiers !!
 * 
 * parameters.GlobalDrawModifier ---> Modifier applied to card draw value
 * parameters.GlobalWeaponsModifier ---> Modifier applied to the value of weapons attacks
 * 
 */

namespace TrackerExampleProfile
{
    public static class Extension
    {
        public static bool ContainsAll<T1>(this IList<T1> list, params T1[] items)
        {
            return !items.Except(list).Any();
        }
        //In the big picture, this is crappy\lazy coding, but it makes the code really readable
        public static bool IsOneOf(this Style s, params Style[] list)
        {
            return list.Any(ls => ls == s);
        }
        public static bool Is(this Style s, Style reference)
        {
            return s == reference;
        }
        public static bool IsAggresive(this Style s)
        {
            return s.IsOneOf(Style.Face, Style.Aggro);
        }
        public static bool IsOneOf(this DeckType s, params DeckType[] list)
        {
            return list.Any(ls => ls == s);
        }
        public static bool Is(this DeckType s, DeckType reference)
        {
            return s == reference;
        }
    }
    [Serializable]
    public class ProfileTrackerTemplate : Profile
    {
        //Cards definitions
        private const Card.Cards SteadyShot = Card.Cards.DS1h_292;
        private const Card.Cards Shapeshift = Card.Cards.CS2_017;
        private const Card.Cards LifeTap = Card.Cards.CS2_056;
        private const Card.Cards Fireblast = Card.Cards.CS2_034;
        private const Card.Cards Reinforce = Card.Cards.CS2_101;
        private const Card.Cards ArmorUp = Card.Cards.CS2_102;
        private const Card.Cards LesserHeal = Card.Cards.CS1h_001;
        private const Card.Cards DaggerMastery = Card.Cards.CS2_083b;

        public DeckType debugEnemyDeckType = DeckType.Basic;
        public DeckType debugMyDeckType = DeckType.Basic;
        public Style debugEnemyStyle = Style.Tempo;
        public Style debugMyStyle = Style.Tempo;
        public Style debugArenaStyle = Style.Tempo;

        private readonly Dictionary<Card.Cards, int> _heroPowersPriorityTable = new Dictionary<Card.Cards, int>
        {
            {SteadyShot, 8},
            {Shapeshift, 7},
            {LifeTap, 6},
            {Fireblast, 5},
            {Reinforce, 4},
            {ArmorUp, 3},
            {LesserHeal, 2},
            {DaggerMastery, 1}
        };
        public TrackerValues st;
        //Dictionary<DeckType, List<Card.Cards> SingleAoeClears = new Dictionary<DeckType, List<Card.Cards>>();
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns>ProfileParameters.</returns>
        public ProfileParameters GetParameters(Board board)
        {

            //setup parameters with default as baseprofile
            var parameters = new ProfileParameters(BaseProfile.Default);
           
            
            try
            {
                st = new TrackerValues();
            }
            catch (Exception tracker)
            {
                Bot.Log("[ProfileExample] Tracker couldn't generate one of the values " + tracker.Message);
                //Debug Mode in order EnemyDeckType
                Bot.Log("Using Debug Values");
                st = new TrackerValues(
                     debugEnemyDeckType, //Enemy DeckType
                     debugMyDeckType,    //Your DeckType
                     debugEnemyStyle,    //Enemy Style
                     debugMyStyle,       //Your Style
                     debugArenaStyle     //Your ArenaStyle
                    );
            }

            if (st.EnemyStyle.IsOneOf(Style.Control, Style.Combo, Style.Tempo))
            {
                //do something
            }
            if (st.EnemyStyle.IsAggresive())
            {
                //Do Something
            }
            if (st.EnemyDeckType.Is(DeckType.Basic))
            {
                Bot.Log("===========Opponent is Basic");

            }
            



            return parameters;
        }



        public Card.Cards SirFinleyChoice(List<Card.Cards> choices)
        {
            var filteredTable = _heroPowersPriorityTable.Where(x => choices.Contains(x.Key)).ToList();
            return filteredTable.First(x => x.Value == filteredTable.Max(y => y.Value)).Key;
        }
    }
    /// <summary>
    /// ========================================DO NOT EDIT ANYTING BELOW UNLESS YOU KNOW WHAT YOU ARE DOING==============================================
    /// </summary>
    public class TrackerValues
    {
        public DeckType EnemyDeckType { get; set; }
        public Style EnemyStyle { get; set; }
        public DeckType MyDeckType { get; set; }
        public Style MyStyle { get; set; }
        public bool Mystery { get; set; }
        public Style ArenaStyle { get; set; }
        public TrackerValues()
        {
            Dictionary<string, object> values =
                 Bot.GetPlugins().Find(plugin => plugin.DataContainer.Name == "SmartTracker").GetProperties();
            EnemyDeckType = (DeckType)values["EnemyDeckTypeGuess"];
            MyDeckType = (DeckType)values["AutoFriendlyDeckType"];
            EnemyStyle = (Style)values["EnemyDeckStyleGuess"];
            MyStyle = (Style)values["AutoFriendlyStyle"];
            ArenaStyle = (Style)values["ArenaStyle"];
            Mystery = (bool)values["MysteryBoolean"];
        }
        public TrackerValues(DeckType edt, DeckType mdt, Style es, Style ms, Style ar)
        {
            EnemyDeckType = edt;
            MyDeckType = mdt;
            EnemyStyle = es;
            MyStyle = ms;
            ArenaStyle = ar;
            Mystery = false;
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