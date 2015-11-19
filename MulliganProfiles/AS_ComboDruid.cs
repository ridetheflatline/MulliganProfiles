using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartBot.Database;
using SmartBot.Mulligan;
using SmartBot.Plugins.API;


namespace SmartBotUI.Mulligan.ComboDruid
{
    internal static class Extension
    {
        public static void AddOrUpdate<TKey, TValue>(
     this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }
    }

    [Serializable]
    public class bMulliganProfile : MulliganProfile
    {
        #region Data

        private const bool GnimshDruid = true;
        /*Behavioral logic*/
        /************************************/
        private readonly bool _facingMechMages = false;       //Set it to true if you are facing mech mages.
        private readonly bool _excessiveRamp = false;         //Keeps both, wild growth and darnasus Aspirant
        private readonly bool _twoInnervatesPlease = true;   //Will keep both innervates instead of 1 
        private readonly bool _boomAndLore = true;           //Coin innnervate Innervate Ka-Boooooooooooom.
        private readonly bool FaceDruid = true;
        //          -Please note that twoInnervatesPlease needs to be set to true

        //WIP list. They won't work, I can't justify them in combo druid just yet

        private bool _bgh = false;                   //WIP, Keeps Big Game hunter on a good curve against Paladins

        //TODO: Add more based on suggestions. 


        /********************************************
                            Don't ask
                                        __
                         _..-''--'----_.
                       ,''.-''| .---/ _`-._
                     ,' \ \  ;| | ,/ / `-._`-.
                   ,' ,',\ \( | |// /,-._  / /
                   ;.`. `,\ \`| |/ / |   )/ /
                  / /`_`.\_\ \| /_.-.'-''/ /
                 / /_|_:.`. \ |;'`..')  / /
                 `-._`-._`.`.;`.\  ,'  / /
                     `-._`.`/    ,'-._/ /
                       : `-/     \`-.._/
                       |  :      ;._ (
                       :  |      \  ` \
                        \         \   |
                         :        :   ;
                         |           /
                         ;         ,'
                        /         /
                       /         /
                                / Hearthstone
         
        
        /*****************DECK LIST******************/
        private const string AncientOfLore = "NEW1_008";
        private const string AncientOfWar = "EX1_178";
        private const string AzureDrake = "EX1_284";
        private const string DarnasussAspirant = "AT_038";
        private const string DrBoom = "GVG_110";
        private const string DruidOfTheClaw = "EX1_165";
        private const string DruidOfTheSaber = "AT_042";
        private const string EmperorThaurissan = "BRM_028";
        private const string EydisDarkbane = "AT_131";
        private const string FjolaLightbane = "AT_129";
        private const string FlameJuggler = "AT_094";
        private const string ForceOfNature = "EX1_571";
        private const string HarrisonJones = "EX1_558";
        private const string Innervate = "EX1_169";
        private const string KeeperOfGroove = "EX1_166";
        private const string LivingRoots = "AT_037";
        private const string MarkOfTheWild = "CS2_009";
        private const string PilotedShredder = "GVG_096";
        private const string PowerOfTheWild = "EX1_160";
        private const string SavageCombatant = "AT_039";
        private const string SavageRoar = "CS2_011";
        private const string ShadeOfNaxxramas = "FP1_005";
        private const string SpiderTank = "GVG_044"; //In place of sisters
        private const string Swipe = "CS2_012";
        private const string WildGrowth = "CS2_013";
        private const string Wrath = "EX1_154";
        /********************************************/
        #endregion

        private readonly Dictionary<string, bool> _whiteList; // CardName, KeepDouble
        private readonly Dictionary<string, int> _ramp;
        private readonly List<Card.Cards> _cardsToKeep;



        public bMulliganProfile()
            : base()
        {
            _whiteList = new Dictionary<string, bool>();
            _ramp = new Dictionary<string, int>();// wild growth or Darnassus Aspirant
            _cardsToKeep = new List<Card.Cards>();
        }


        //Logic is the following. 
        //Priority goes to 
        //                 Darnassus Aspirant against aggro
        //                 Wild Growth against control
        //
        //  bl stands for boom and lore. If called with true, it will allow dr boom and ancient of lore ramping 
        public void Ramp(List<Card.Cards> choices, int wg = 1, int da = 2, bool bl = false) //Wild Growth and Darnasuss Aspirant
        {
            var coin = choices.Count > 3;

            if (_excessiveRamp)
            {
                _whiteList.AddOrUpdate(WildGrowth, false);
                _whiteList.AddOrUpdate(DarnasussAspirant, true);
                return;
            }

            if (bl && (choices.Any(c => c.ToString() == DrBoom)))
                _whiteList.AddOrUpdate(DrBoom, false);
            else if (bl && (choices.Any(c => c.ToString() == AncientOfLore)))
                _whiteList.AddOrUpdate(AncientOfLore, false);

            if (FaceDruid)
            {
                var onedrop = false;
                var twodrop = false;
                var threedrop = false;

                foreach (var c in choices)
                {
                    var temp = CardTemplate.LoadFromId(c.ToString());
                    if (temp.Cost == 1 && temp.Type == SmartBot.Plugins.API.Card.CType.MINION)
                    {
                        onedrop = true;
                        _whiteList.AddOrUpdate(c.ToString(), false);
                    }
                    if (temp.Cost == 2 && temp.Type == SmartBot.Plugins.API.Card.CType.MINION)
                    {
                        twodrop = true;
                        _whiteList.AddOrUpdate(c.ToString(), false);
                    }
                    if (temp.Cost == 3 && temp.Type == SmartBot.Plugins.API.Card.CType.MINION)
                    {
                        threedrop = true;
                        _whiteList.AddOrUpdate(c.ToString(), false);
                    }
                }
                foreach (var c in choices)
                {
                    var temp = CardTemplate.LoadFromId(c.ToString());
                    if (!twodrop && temp.Cost == 2 && temp.Type == SmartBot.Plugins.API.Card.CType.SPELL)
                        _whiteList.AddOrUpdate(c.ToString(), false);
                }
            }

            /**************************************************************************/
            /*Ramping priority finder**************************************************/
            /**************************************************************************/
            _ramp.AddOrUpdate(WildGrowth, wg);
            _ramp.AddOrUpdate(DarnasussAspirant, da);
            var i = 0;
            while (true)
            {

                if (choices.Any(c => c.ToString() == _ramp.FirstOrDefault(x => x.Value == i).Key))
                {
                    _whiteList.AddOrUpdate(_ramp.FirstOrDefault(x => x.Value == i).Key, false);
                    break;
                }
                i++;
                if (i > 2) //moved it from while condition
                    break; //Doesn't bark anymore
            }
            /**************************************************************************/
            if (coin && da == 1)
                _whiteList.AddOrUpdate(DarnasussAspirant, true);
        }

        public List<Card.Cards> HandleMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            /*Conditions based on mulligan openers*/
            var hasCoin = choices.Count > 3;
            var hasInnervate = choices.Any(g => g.ToString() == Innervate); //If there is an innervate
            var doubleInnervate = choices.Count(g => g.ToString() == Innervate) == 2; //If there are 2 innervates
            var hasWildGrowth = choices.Any(g => g.ToString() == WildGrowth); //If the is a wild growth
            var hasAspirant = choices.Any(g => g.ToString() == DarnasussAspirant); //If there is an Aspirant
            var weaponClass = opponentClass == Card.CClass.WARRIOR || opponentClass == Card.CClass.PALADIN;
            var earlyGame = hasWildGrowth || hasAspirant;

            /*Always whitelists at least 1 innervate*/
            _whiteList.AddOrUpdate(Innervate, _twoInnervatesPlease);


            _whiteList.AddOrUpdate(SpiderTank, hasInnervate);
            _whiteList.AddOrUpdate(FjolaLightbane, false);
            _whiteList.AddOrUpdate(EydisDarkbane, false);
            _whiteList.AddOrUpdate(FlameJuggler, false);
            _whiteList.AddOrUpdate(DruidOfTheSaber, false);
            /*************************************************/

            /*Since mulligan is straightforward 
             against classes I am grouping most of them
             * Aggro:
             *      Hunter
             *      Paladin
             *      [Optional] Mech Mage
             *      
             * Control/midrange
             *      All else
             * 
             ********************************************/
            var control = false;
            var aggro = false;
            /********************************************/






            switch (opponentClass)
            {
                case Card.CClass.DRUID:
                    {
                        control = true;
                        break;
                    }
                case Card.CClass.HUNTER:
                    {
                        aggro = true;
                        break;
                    }
                case Card.CClass.MAGE:
                    {
                        _whiteList.AddOrUpdate(Wrath, false);
                        if (_facingMechMages)
                            aggro = true;
                        else
                            control = true;
                        break;
                    }
                case Card.CClass.PALADIN:
                    {
                        if (earlyGame) _whiteList.AddOrUpdate(Swipe, false);
                        aggro = true;
                        break;
                    }
                case Card.CClass.PRIEST:
                    {
                        if (earlyGame)
                            _whiteList.AddOrUpdate(Wrath, false);
                        if (hasCoin)
                            _whiteList.AddOrUpdate(KeeperOfGroove, true);
                        control = true;
                        break;
                    }
                case Card.CClass.ROGUE:
                    {
                        control = true;
                        break;
                    }
                case Card.CClass.SHAMAN:
                    {
                        control = true;
                        break;
                    }
                case Card.CClass.WARLOCK:
                    {
                        if (earlyGame) _whiteList.AddOrUpdate(KeeperOfGroove, false);
                        control = true;
                        break;
                    }
                case Card.CClass.WARRIOR:
                    {
                        _whiteList.AddOrUpdate(Wrath, false);
                        control = true;
                        break;
                    }
            }
            /*
            * Cheat Notes
            * ================================================
            * Going first: 
            *          Innervate x1
            *          3 mana: Shade
            *    [t2]  4 mana: Piloted, Keeper
            *          
            *          Innervate x2
            *          5 mana: Azure, Druid of the Claw
            *    
            * 
            * On Coin: innervate x1
            *          Shade, Piloted, Keeper
            *    [t2]  Azure, Druid of the Claw
            *          
            *          Innervate x2
            *    [t2]  Ancient of Lore, Dr. Balance
            *    ==============================================
            */

            if (weaponClass && hasCoin)
                _whiteList.AddOrUpdate(HarrisonJones, false);

            if (aggro)
            {
                if (hasInnervate)
                    _whiteList.AddOrUpdate(ShadeOfNaxxramas, false);
                if (hasCoin && hasInnervate)
                    _whiteList.AddOrUpdate(PilotedShredder, false);

                _whiteList.AddOrUpdate(LivingRoots, true);
                _whiteList.AddOrUpdate(Wrath, false);
                _whiteList.AddOrUpdate(KeeperOfGroove, false);
                Ramp(choices, 2, 1); // Darnasus Aspirant over Wild Growth
            }

            else if (control)
            {
                _whiteList.AddOrUpdate(LivingRoots, false);
                _whiteList.AddOrUpdate(ShadeOfNaxxramas, false);

                if (hasWildGrowth || hasAspirant || hasCoin)
                    _whiteList.AddOrUpdate(PilotedShredder, false);

                if (hasCoin && doubleInnervate) //I refuse to add Dr Boom and Ancient of Lore to this
                    _whiteList.AddOrUpdate(PilotedShredder, true); //coin innervate shredder t1, innervate shredder t2 #thedream


                if (hasCoin && hasInnervate)
                {
                    _whiteList.AddOrUpdate(AzureDrake, false);
                    _whiteList.AddOrUpdate(DruidOfTheClaw, false);
                }
                if (hasCoin && doubleInnervate && _boomAndLore)
                    Ramp(choices, 1, 2, true);
                else
                    Ramp(choices);
            }

            /*Line below is the same as:
             * ------------------------------------------------
              foreach (Card s in Choices)
            {
                bool keptOneAlready = false;

                if (_cardsToKeep.Any(c => c.ToString() == s.ToString()))
                {
                    keptOneAlready = true;
                }
                if (_whiteList.ContainsKey(s.ToString()))
                {
                    if (!keptOneAlready | _whiteList[s.ToString()])
                    {
                        _cardsToKeep.Add(s);
                    }
                }
            }
             -------------------------------------------------*/
            foreach (var s in from s in choices let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString()) where _whiteList.ContainsKey(s.ToString()) where !keptOneAlready | _whiteList[s.ToString()] select s)
                _cardsToKeep.Add(s);


            return _cardsToKeep;
        }
    }
}