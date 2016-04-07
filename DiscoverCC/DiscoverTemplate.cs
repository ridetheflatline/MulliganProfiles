using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartBot.Database;
using SmartBot.Discover;
using SmartBot.Plugins.API;

namespace Discover
{
    public static class Extension
    {
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }

        public static bool HasBghTarget(this Board board)
        {
            return board.MinionEnemy.Any(card => card.Template.Atk > 7);
        }
        public static bool CanActivateReliquary(this Board board)
        {
            return board.MinionFriend.Count >=5;
        }
        public static bool HasRaceInPlay(this Board board, Card.CRace crace)
        {
            return board.MinionFriend.Any(card => card.Template.Race == crace);
        }
        public static bool HasRaceInHand(this Board board, Card.CRace crace)
        {
            return board.MinionFriend.Any(card => card.Template.Race == crace);
        }

        //public static bool HasTurnXPlay(this Board board, int turn)
        //{
        //    return board.
        //}
    }
    public class DiscoverTemplate : DiscoverPickHandler
    {
        
        private const Card.Cards SteadyShot = Card.Cards.DS1h_292;
        private const Card.Cards Shapeshift = Card.Cards.CS2_017;
        private const Card.Cards LifeTap = Card.Cards.CS2_056;
        private const Card.Cards Fireblast = Card.Cards.CS2_034;
        private const Card.Cards Reinforce = Card.Cards.CS2_101;
        private const Card.Cards ArmorUp = Card.Cards.CS2_102;
        private const Card.Cards LesserHeal = Card.Cards.CS1h_001;
        private const Card.Cards DaggerMastery = Card.Cards.CS2_083b;

        public readonly Dictionary<Card.Cards, int> HeroPowersPriorityTable = new Dictionary<Card.Cards, int>
        {
            {SteadyShot, 1},
            {Shapeshift, 4},
            {LifeTap, 5},
            {Fireblast, 8},
            {Reinforce, 6},
            {ArmorUp, 2},
            {LesserHeal, 3},
            {DaggerMastery, 7}
        };

        public Card.Cards HandlePickDecision(Card.Cards originCard, List<Card.Cards> choices, Board board)
        {
            
            if (board.HasRaceInPlay(Card.CRace.MECH))
            return choices[0];
            BoardState BoardCondition = GetBoardCondition(board);
            bool Even = BoardCondition == BoardState.Even;
            bool Losing = BoardCondition == BoardState.Losing;
            bool Winning = BoardCondition == BoardState.Winning;
            Card.Cards returnCard = choices[0];
            switch (originCard)//will override return card from being left choice
            {
                case Cards.DarkPeddler:
                    returnCard = DarkPeddlerHandler(choices, board, BoardCondition);
                    break;

                case Cards.EtherealConjurer:
                    returnCard = EtherealConjurerHandler(choices, board, BoardCondition);
                    //return EtherealConjurerHandler(true, choices, board, BoardCondition);
                    break;

                case Cards.GorillabotA3:
                    returnCard = GorillabotA3Handler(choices, board, BoardCondition);
                    break;

                case Cards.JeweledScarab:
                    returnCard = JeweledScarabHandler(choices, board, BoardCondition);
                    break;

                case Cards.MuseumCurator:
                    returnCard = MuseumCuratorHandler(choices, board, BoardCondition);
                    break;

                case Cards.RavenIdol:
                    returnCard = choices[0];
                    break;

                case Cards.TombSpider:
                    returnCard = TombSpiderHandler(choices, board, BoardCondition);
                    break;

                case Cards.ArchThiefRafaam:
                    //TODO: This should be easy, opponent without aoe? mummies, winning? lantern, opponent health below 5 and no board on both sides Clock? 
                    returnCard = Winning ? choices[0] : Losing ? choices[1] : board.HeroEnemy.CurrentHealth < 5 ? choices[2] : choices[0];
                    break;

                case Cards.SirFinleyMrrgglton:
                    //TODO: Take into account mode and deck you are playing. 
                    //TODO: Arena <Fireblast > Dagger > Tokens > else> || and so on. 
                    List<KeyValuePair<Card.Cards, int>> filteredTable = HeroPowersPriorityTable.Where(x => choices.Contains(x.Key)).ToList();
                    returnCard = filteredTable.First(x => x.Value == filteredTable.Max(y => y.Value)).Key;
                    break;
            }
            using (
                StreamWriter log = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\DiscoverCC\\DiscoverLog.txt", true)
                )
            {
                log.WriteLine("[Discover:{0}] Your choices were\n", CardTemplate.LoadFromId(originCard).Name);
                foreach (var template in choices.Select(CardTemplate.LoadFromId))
                {
                    log.Write("{0} || ", template.Name);
                }
                log.WriteLine("\n[Best Choice according to Arthur's template] {0}",
                CardTemplate.LoadFromId(returnCard).Name);
                log.WriteLine("==============================");
                log.WriteLine(Bot.CurrentBoard.ToSeed());
                log.WriteLine("\n==============================");
            }
            return returnCard;
        }
        //TODO: ==================================================DARK PEDDLER=================================================
        //Corruption = Card.Cards.CS2_063; 			         //[1 Mana] [0/0] Corruption                ||| [WARLOCK]
        //Voidwalker = Card.Cards.CS2_065; 			         //[1 Mana] [1/3] Voidwalker                ||| [WARLOCK]
        //MortalCoil = Card.Cards.EX1_302; 			         //[1 Mana] [0/0] Mortal Coil               ||| [WARLOCK]
        //Soulfire = Card.Cards.EX1_308; 			         //[1 Mana] [0/0] Soulfire                  ||| [WARLOCK]
        //BloodImp = Card.Cards.CS2_059; 			         //[1 Mana] [0/1] Blood Imp                 ||| [WARLOCK]
        //PowerOverwhelming = Card.Cards.EX1_316; 			 //[1 Mana] [0/0] Power Overwhelming        ||| [WARLOCK]
        //FlameImp = Card.Cards.EX1_319; 			         //[1 Mana] [3/2] Flame Imp                 ||| [WARLOCK]
        //ReliquarySeeker = Card.Cards.LOE_116; 			 //[1 Mana] [1/1] Reliquary Seeker          ||| [WARLOCK]
        //GoldshireFootman = Card.Cards.CS1_042; 			 //[1 Mana] [1/2] Goldshire Footman         ||| [NONE]
        //MurlocRaider = Card.Cards.CS2_168; 			     //[1 Mana] [2/1] Murloc Raider             ||| [NONE]
        //StonetuskBoar = Card.Cards.CS2_171; 			     //[1 Mana] [1/1] Stonetusk Boar            ||| [NONE]
        //ElvenArcher = Card.Cards.CS2_189; 		    	 //[1 Mana] [1/1] Elven Archer              ||| [NONE]
        //VoodooDoctor = Card.Cards.EX1_011; 			     //[1 Mana] [2/1] Voodoo Doctor             ||| [NONE]
        //GrimscaleOracle = Card.Cards.EX1_508; 			 //[1 Mana] [1/1] Grimscale Oracle          ||| [NONE]
        //SouthseaDeckhand = Card.Cards.CS2_146; 			 //[1 Mana] [2/1] Southsea Deckhand         ||| [NONE]
        //YoungDragonhawk = Card.Cards.CS2_169; 			 //[1 Mana] [1/1] Young Dragonhawk          ||| [NONE]
        //AbusiveSergeant = Card.Cards.CS2_188; 			 //[1 Mana] [2/1] Abusive Sergeant          ||| [NONE]
        //Lightwarden = Card.Cards.EX1_001; 			     //[1 Mana] [1/2] Lightwarden               ||| [NONE]
        //YoungPriestess = Card.Cards.EX1_004; 			     //[1 Mana] [2/1] Young Priestess           ||| [NONE]
        //ArgentSquire = Card.Cards.EX1_008; 			     //[1 Mana] [1/1] Argent Squire             ||| [NONE]
        //AngryChicken = Card.Cards.EX1_009; 			     //[1 Mana] [1/1] Angry Chicken             ||| [NONE]
        //WorgenInfiltrator = Card.Cards.EX1_010; 			 //[1 Mana] [2/1] Worgen Infiltrator        ||| [NONE]
        ////LeperGnome = Card.Cards.EX1_029; 			     //[1 Mana] [2/1] Leper Gnome               ||| [NONE]
        //Secretkeeper = Card.Cards.EX1_080; 		    	 //[1 Mana] [1/2] Secretkeeper              ||| [NONE]
        //Shieldbearer = Card.Cards.EX1_405; 			     //[1 Mana] [0/4] Shieldbearer              ||| [NONE]
        //MurlocTidecaller = Card.Cards.EX1_509; 			 //[1 Mana] [1/2] Murloc Tidecaller         ||| [NONE]
        //HungryCrab = Card.Cards.NEW1_017; 			     //[1 Mana] [1/2] Hungry Crab               ||| [NONE]
        //BloodsailCorsair = Card.Cards.NEW1_025; 			 //[1 Mana] [1/2] Bloodsail Corsair         ||| [NONE]
        ////SirFinleyMrrgglton = Card.Cards.LOE_076; 		 //[1 Mana] [1/3] Sir Finley Mrrgglton      ||| [NONE]
        ////ZombieChow = Card.Cards.FP1_001; 			     //[1 Mana] [2/3] Zombie Chow               ||| [NONE]
        //Undertaker = Card.Cards.FP1_028; 			         //[1 Mana] [1/2] Undertaker                ||| [NONE]
        //Cogmaster = Card.Cards.GVG_013; 	        		 //[1 Mana] [1/2] Cogmaster                 ||| [NONE]
        ////ClockworkGnome = Card.Cards.GVG_082; 			 //[1 Mana] [2/1] Clockwork Gnome           ||| [NONE]
        //DragonEgg = Card.Cards.BRM_022; 	        		 //[1 Mana] [0/2] Dragon Egg                ||| [NONE]
        //LowlySquire = Card.Cards.AT_082; 		        	 //[1 Mana] [1/2] Lowly Squire              ||| [NONE]
        //TournamentAttendee = Card.Cards.AT_097; 			 //[1 Mana] [2/1] Tournament Attendee       ||| [NONE]
        //InjuredKvaldir = Card.Cards.AT_105; 		    	 //[1 Mana] [2/4] Injured Kvaldir           ||| [NONE]
        //GadgetzanJouster = Card.Cards.AT_133; 			 //[1 Mana] [1/2] Gadgetzan Jouster         ||| [NONE]

        private Card.Cards DarkPeddlerHandler(List<Card.Cards> choices, Board board, BoardState boardCondition)
        {
            bool opWeaponClass = board.IsWeaponClass(board.EnemyClass);
            bool rel = board.MinionFriend.Count == 6;
            var BoardLethals = CalculateNeededDamageForLethal(board);
            int SpellLethal = BoardLethals.Item2;
            int BoardLethal = BoardLethals.Item1;
            bool aggro = board.EnemyClass == Card.CClass.PALADIN || board.EnemyClass == Card.CClass.SHAMAN;

            Dictionary<Card.Cards, int> darkPeddlerDictionary = new Dictionary<Card.Cards, int>
            {
                //TODO: all cards should have a value based on board condition via special method. 
                {Cards.Corruption, 50}, //[1 Mana] [0/0]
                {Cards.Voidwalker, aggro ? 50 : 30}, //[1 Mana] [1/3]
                {Cards.MortalCoil, aggro ? 55 : 35}, //[1 Mana] [0/0]
                {Cards.Soulfire, SpellLethal <= 4 ? 10000 :  5}, //[1 Mana] [0/0]
                {Cards.BloodImp, 0}, //[1 Mana] [0/1]
                {Cards.PowerOverwhelming, 75}, //[1 Mana] [0/0]
                {Cards.FlameImp, 70}, //[1 Mana] [3/2]
                {Cards.ReliquarySeeker, rel ? 90 : 5}, //[1 Mana] [1/1]
                {Cards.GoldshireFootman, aggro ? 50: 20}, //[1 Mana] [1/2]
                {Cards.MurlocRaider, 0}, //[1 Mana] [2/1]
                {Cards.StonetuskBoar, 0}, //[1 Mana] [1/1]
                {Cards.ElvenArcher, 15}, //[1 Mana] [1/1]
                {Cards.VoodooDoctor, 15}, //[1 Mana] [2/1]
                {Cards.GrimscaleOracle, 0}, //[1 Mana] [1/1]
                {Cards.SouthseaDeckhand, 0}, //[1 Mana] [2/1]
                {Cards.YoungDragonhawk, 0}, //[1 Mana] [1/1]
                {Cards.AbusiveSergeant, 30}, //[1 Mana] [2/1]
                {Cards.Lightwarden, 0}, //[1 Mana] [1/2]
                {Cards.YoungPriestess, 0}, //[1 Mana] [2/1]
                {Cards.ArgentSquire, 45}, //[1 Mana] [1/1]
                {Cards.AngryChicken, 0}, //[1 Mana] [1/1]
                {Cards.WorgenInfiltrator, 20}, //[1 Mana] [2/1]
                {Cards.LeperGnome, 50}, //[1 Mana] [2/1]
                {Cards.Secretkeeper, 0}, //[1 Mana] [1/2]
                {Cards.Shieldbearer, 0}, //[1 Mana] [0/4]
                {Cards.MurlocTidecaller, 0}, //[1 Mana] [1/2]
                {Cards.HungryCrab, 0}, //[1 Mana] [1/2]
                {Cards.BloodsailCorsair, opWeaponClass ? 15 : 0}, //[1 Mana] [1/2]
                {Cards.SirFinleyMrrgglton, -1}, //[1 Mana] [1/3]
                {Cards.ZombieChow, 30}, //[1 Mana] [2/3]
                {Cards.Undertaker, 10}, //[1 Mana] [1/2]
                {Cards.Cogmaster, 5}, //[1 Mana] [1/2]
                {Cards.ClockworkGnome, 15}, //[1 Mana] [2/1]
                {Cards.DragonEgg, 10}, //[1 Mana] [0/2]
                {Cards.LowlySquire, 10}, //[1 Mana] [1/2]
                {Cards.TournamentAttendee, 0}, //[1 Mana] [2/1]
                {Cards.InjuredKvaldir, 0}, //[1 Mana] [2/4]
                {Cards.GadgetzanJouster, 40}, //[1 Mana] [1/2]


            };
            List<KeyValuePair<Card.Cards, int>> filteredTable = darkPeddlerDictionary.Where(x => choices.Contains(x.Key)).ToList();
            return filteredTable.First(x => x.Value == filteredTable.Max(y => y.Value)).Key;

        }
        /// <summary>
        /// int 1: Damage with taunt
        /// int 2: Damage with no taunt (enemy hero health)
        /// TODO: Does not force every minion and spell to attack face. Needs to be addressed
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        private Tuple<int, int> CalculateNeededDamageForLethal(Board board)
        {

            int withTaunt = board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor
                   - board.MinionFriend.Where(card => card.CanAttack).Sum(q => q.CurrentAtk) //our total damage on board
                   + board.MinionEnemy.Where(card => card.IsTaunt).Sum(q => q.CurrentHealth) //enemy total taun health
                                                                                             //TODO: subtract burn in hand
                   ;
            int enemyHealth = board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor;
            return new Tuple<int, int>(withTaunt, enemyHealth);
        }


        //TODO: ==================================================ETHEREAL CONJURER=================================================
        //TODO: ===================================================ALL MAGE SPELLS=================================================

        //Polymorph = Card.Cards.CS2_022; 			     //[4 Mana] [0/0] Polymorph             ||| [MAGE]
        //ArcaneIntellect = Card.Cards.CS2_023; 		 //[3 Mana] [0/0] Arcane Intellect      ||| [MAGE]
        //Frostbolt = Card.Cards.CS2_024; 			     //[2 Mana] [0/0] Frostbolt             ||| [MAGE]
        //ArcaneExplosion = Card.Cards.CS2_025; 		 //[2 Mana] [0/0] Arcane Explosion      ||| [MAGE]
        //FrostNova = Card.Cards.CS2_026; 			     //[3 Mana] [0/0] Frost Nova            ||| [MAGE]
        //MirrorImage = Card.Cards.CS2_027; 			 //[1 Mana] [0/0] Mirror Image          ||| [MAGE]
        //Fireball = Card.Cards.CS2_029; 			     //[4 Mana] [0/0] Fireball              ||| [MAGE]
        //Flamestrike = Card.Cards.CS2_032; 			 //[7 Mana] [0/0] Flamestrike           ||| [MAGE]
        //ArcaneMissiles = Card.Cards.EX1_277; 			 //[1 Mana] [0/0] Arcane Missiles       ||| [MAGE]
        //Blizzard = Card.Cards.CS2_028; 			     //[6 Mana] [0/0] Blizzard              ||| [MAGE]
        //IceLance = Card.Cards.CS2_031; 			     //[1 Mana] [0/0] Ice Lance             ||| [MAGE]
        //ConeofCold = Card.Cards.EX1_275; 			     //[4 Mana] [0/0] Cone of Cold          ||| [MAGE]
        //Pyroblast = Card.Cards.EX1_279; 			     //[10 Mana] [0/0] Pyroblast            ||| [MAGE]
        //Counterspell = Card.Cards.EX1_287; 			 //[3 Mana] [0/0] Counterspell          ||| [MAGE]
        //IceBarrier = Card.Cards.EX1_289; 			     //[3 Mana] [0/0] Ice Barrier           ||| [MAGE]
        //MirrorEntity = Card.Cards.EX1_294; 			 //[3 Mana] [0/0] Mirror Entity         ||| [MAGE]
        //IceBlock = Card.Cards.EX1_295; 			     //[3 Mana] [0/0] Ice Block             ||| [MAGE]
        //Vaporize = Card.Cards.EX1_594; 			     //[3 Mana] [0/0] Vaporize              ||| [MAGE]
        //Spellbender = Card.Cards.tt_010; 			     //[3 Mana] [0/0] Spellbender           ||| [MAGE]
        //ForgottenTorch = Card.Cards.LOE_002; 			 //[3 Mana] [0/0] Forgotten Torch       ||| [MAGE]
        //Duplicate = Card.Cards.FP1_018; 			     //[3 Mana] [0/0] Duplicate             ||| [MAGE]
        //Flamecannon = Card.Cards.GVG_001; 			 //[2 Mana] [0/0] Flamecannon           ||| [MAGE]
        //UnstablePortal = Card.Cards.GVG_003; 			 //[2 Mana] [0/0] Unstable Portal       ||| [MAGE]
        //EchoofMedivh = Card.Cards.GVG_005; 			 //[4 Mana] [0/0] Echo of Medivh        ||| [MAGE]
        //DragonsBreath = Card.Cards.BRM_003; 			 //[5 Mana] [0/0] Dragon's Breath       ||| [MAGE]
        //FlameLance = Card.Cards.AT_001; 		    	 //[5 Mana] [0/0] Flame Lance           ||| [MAGE]
        //Effigy = Card.Cards.AT_002; 			         //[3 Mana] [0/0] Effigy                ||| [MAGE]
        //ArcaneBlast = Card.Cards.AT_004; 			     //[1 Mana] [0/0] Arcane Blast          ||| [MAGE]
        //PolymorphBoar = Card.Cards.AT_005; 			 //[3 Mana] [0/0] Polymorph: Boar       ||| [MAGE]


        private Card.Cards EtherealConjurerHandler(List<Card.Cards> choices, Board board, BoardState boardCondition)
        {

            Dictionary<Card.Cards, int> etherealConjurerDictionary = new Dictionary<Card.Cards, int>
            {
                {Cards.Polymorph, 0},            //[4 Mana] [0/0] Polymorph             ||| [MAGE]
                {Cards.ArcaneIntellect, 0},      //[3 Mana] [0/0] Arcane Intellect      ||| [MAGE]
                {Cards.Frostbolt, 0},            //[2 Mana] [0/0] Frostbolt             ||| [MAGE]
                {Cards.ArcaneExplosion, 0},      //[2 Mana] [0/0] Arcane Explosion      ||| [MAGE]
                {Cards.FrostNova, 0},            //[3 Mana] [0/0] Frost Nova            ||| [MAGE]
                {Cards.MirrorImage, 0},          //[1 Mana] [0/0] Mirror Image          ||| [MAGE]
                {Cards.Fireball, 0},             //[4 Mana] [0/0] Fireball              ||| [MAGE]
                {Cards.Flamestrike, 0},          //[7 Mana] [0/0] Flamestrike           ||| [MAGE]
                {Cards.ArcaneMissiles, 0},       //[1 Mana] [0/0] Arcane Missiles       ||| [MAGE]
                {Cards.Blizzard, 0},             //[6 Mana] [0/0] Blizzard              ||| [MAGE]
                {Cards.IceLance, 0},             //[1 Mana] [0/0] Ice Lance             ||| [MAGE]
                {Cards.ConeofCold, 0},           //[4 Mana] [0/0] Cone of Cold          ||| [MAGE]
                {Cards.Pyroblast, 0},            //[10 Mana] [0/0] Pyroblast            ||| [MAGE]
                {Cards.Counterspell, 0},         //[3 Mana] [0/0] Counterspell          ||| [MAGE]
                {Cards.IceBarrier, 0},           //[3 Mana] [0/0] Ice Barrier           ||| [MAGE]
                {Cards.MirrorEntity, 0},         //[3 Mana] [0/0] Mirror Entity         ||| [MAGE]
                {Cards.IceBlock, 0},             //[3 Mana] [0/0] Ice Block             ||| [MAGE]
                {Cards.Vaporize, 0},             //[3 Mana] [0/0] Vaporize              ||| [MAGE]
                {Cards.Spellbender, 0},          //[3 Mana] [0/0] Spellbender           ||| [MAGE]
                {Cards.ForgottenTorch, 0},       //[3 Mana] [0/0] Forgotten Torch       ||| [MAGE]
                {Cards.Duplicate, 0},            //[3 Mana] [0/0] Duplicate             ||| [MAGE]
                {Cards.Flamecannon, 0},          //[2 Mana] [0/0] Flamecannon           ||| [MAGE]
                {Cards.UnstablePortal, 0},       //[2 Mana] [0/0] Unstable Portal       ||| [MAGE]
                {Cards.EchoofMedivh, 0},         //[4 Mana] [0/0] Echo of Medivh        ||| [MAGE]
                {Cards.DragonsBreath, 0},        //[5 Mana] [0/0] Dragon's Breath       ||| [MAGE]
                {Cards.FlameLance, 0},           //[5 Mana] [0/0] Flame Lance           ||| [MAGE]
                {Cards.Effigy, 0},               //[3 Mana] [0/0] Effigy                ||| [MAGE]
                {Cards.ArcaneBlast, 0},          //[1 Mana] [0/0] Arcane Blast          ||| [MAGE]
                {Cards.PolymorphBoar, 0},        //[3 Mana] [0/0] Polymorph: Boar       ||| [MAGE]
            };

            List<KeyValuePair<Card.Cards, int>> filteredTable = etherealConjurerDictionary.Where(x => choices.Contains(x.Key)).ToList();
            return filteredTable.First(x => x.Value == filteredTable.Max(y => y.Value)).Key;
        }
        /// <summary>
        /// Assumes you are mage when this event triggers
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="board"></param>
        /// <param name="boardCondition"></param>
        /// <returns></returns>
        private Card.Cards EtherealConjurerHandler(bool value, List<Card.Cards> choices, Board board, BoardState boardCondition)
        {
            Dictionary<Card.Cards, int> burnCardsDamageTable = new Dictionary<Card.Cards, int>
            {
                {Cards.Pyroblast, 10},
                {Cards.Frostbolt, 3},
                {Cards.Fireball, 6},
                {Cards.ForgottenTorch, 3},
                {Cards.IceLance, board.HeroEnemy.IsFrozen ? 4 : 0}
            };

            List<Card.Cards> burnSpells = new List<Card.Cards> { Cards.Frostbolt, Cards.Fireball, Cards.ForgottenTorch, Cards.IceLance };
            List<Card.Cards> boardWipe = new List<Card.Cards> { Cards.Blizzard, Cards.Flamestrike };
            switch (boardCondition)
            {
                case BoardState.Even:
                    //TODO: implement logic
                    break;
                case BoardState.Winning:
                    if (choices.Intersect(burnSpells).Any())
                    {
                        List<KeyValuePair<Card.Cards, int>> filteredBurnsByDamage = burnCardsDamageTable.Where(x => choices.Contains(x.Key)).ToList();
                        return filteredBurnsByDamage.First(x => x.Value == filteredBurnsByDamage.Max(y => y.Value)).Key;
                    }
                    break;
                case BoardState.Losing:
                    //TODO: check if it will actually save you
                    if (choices.Intersect(boardWipe).Any())
                        return choices.Intersect(boardWipe).First(); //Will pick first one

                    break;
                default:
                    throw new ArgumentOutOfRangeException("boardCondition", boardCondition, null);
            }
            return choices[0];
        }

        //TODO: ==================================================TOMB SPIDER=================================================
        //TODO: ===================================================ALL BEASTS=================================================
        //FierceMonkey = Card.Cards.LOE_022; 			 //[3 Mana] [3/4] Fierce Monkey             ||| [WARRIOR]
        //StarvingBuzzard = Card.Cards.CS2_237;          //[5 Mana] [3/2] Starving Buzzard          ||| [HUNTER]
        //TimberWolf = Card.Cards.DS1_175;               //[1 Mana] [1/1] Timber Wolf               ||| [HUNTER]
        //TundraRhino = Card.Cards.DS1_178;              //[5 Mana] [2/5] Tundra Rhino              ||| [HUNTER]
        //ScavengingHyena = Card.Cards.EX1_531;          //[2 Mana] [2/2] Scavenging Hyena          ||| [HUNTER]
        //SavannahHighmane = Card.Cards.EX1_534;         //[6 Mana] [6/5] Savannah Highmane         ||| [HUNTER]
        //KingKrush = Card.Cards.EX1_543;                //[9 Mana] [8/8] King Krush                ||| [HUNTER]
        ////DesertCamel = Card.Cards.LOE_020; 			 //[3 Mana] [2/4] Desert Camel              ||| [HUNTER]
        //Webspinner = Card.Cards.FP1_011;               //[1 Mana] [1/1] Webspinner                ||| [HUNTER]
        //KingofBeasts = Card.Cards.GVG_046;             //[5 Mana] [2/6] King of Beasts            ||| [HUNTER]
        //Gahzrilla = Card.Cards.GVG_049;                //[7 Mana] [6/9] Gahz'rilla                ||| [HUNTER]
        //CoreRager = Card.Cards.BRM_014;                //[4 Mana] [4/4] Core Rager                ||| [HUNTER]
        //KingsElekk = Card.Cards.AT_058;                //[2 Mana] [3/2] King's Elekk              ||| [HUNTER]
        //Acidmaw = Card.Cards.AT_063;                   //[7 Mana] [4/2] Acidmaw                   ||| [HUNTER]
        //Dreadscale = Card.Cards.AT_063t; 			     //[3 Mana] [4/2] Dreadscale                ||| [HUNTER]
        //PitSnake = Card.Cards.LOE_010;                 //[1 Mana] [2/1] Pit Snake                 ||| [ROGUE] imposter
        //MountedRaptor = Card.Cards.LOE_050; 			 //[3 Mana] [3/2] Mounted Raptor            ||| [DRUID]
        //JungleMoonkin = Card.Cards.LOE_051;            //[4 Mana] [4/4] Jungle Moonkin            ||| [DRUID]
        //Malorne = Card.Cards.GVG_035;                  //[7 Mana] [9/7] Malorne                   ||| [DRUID]
        //SavageCombatant = Card.Cards.AT_039;           //[4 Mana] [5/4] Savage Combatant          ||| [DRUID]
        //OasisSnapjaw = Card.Cards.CS2_119;             //[4 Mana] [2/7] Oasis Snapjaw             ||| [NONE]
        //RiverCrocolisk = Card.Cards.CS2_120;           //[2 Mana] [2/3] River Crocolisk           ||| [NONE]
        //IronfurGrizzly = Card.Cards.CS2_125; 			 //[3 Mana] [3/3] Ironfur Grizzly           ||| [NONE]
        //SilverbackPatriarch = Card.Cards.CS2_127; 	 //[3 Mana] [1/4] Silverback Patriarch      ||| [NONE]
        //StonetuskBoar = Card.Cards.CS2_171; 			 //[1 Mana] [1/1] Stonetusk Boar            ||| [NONE]
        //BloodfenRaptor = Card.Cards.CS2_172;           //[2 Mana] [3/2] Bloodfen Raptor           ||| [NONE]
        //CoreHound = Card.Cards.CS2_201;                //[7 Mana] [9/5] Core Hound                ||| [NONE]
        //YoungDragonhawk = Card.Cards.CS2_169; 		 //[1 Mana] [1/1] Young Dragonhawk          ||| [NONE]
        //IronbeakOwl = Card.Cards.CS2_203;              //[2 Mana] [2/1] Ironbeak Owl              ||| [NONE]
        //AngryChicken = Card.Cards.EX1_009; 			 //[1 Mana] [1/1] Angry Chicken             ||| [NONE]
        //KingMukla = Card.Cards.EX1_014; 	    		 //[3 Mana] [5/5] King Mukla                ||| [NONE]
        //JunglePanther = Card.Cards.EX1_017; 			 //[3 Mana] [4/2] Jungle Panther            ||| [NONE]
        //StranglethornTiger = Card.Cards.EX1_028;       //[5 Mana] [5/5] Stranglethorn Tiger       ||| [NONE]
        //DireWolfAlpha = Card.Cards.EX1_162;            //[2 Mana] [2/2] Dire Wolf Alpha           ||| [NONE]
        //EmperorCobra = Card.Cards.EX1_170; 			 //[3 Mana] [2/3] Emperor Cobra             ||| [NONE]
        //TheBeast = Card.Cards.EX1_577; 		    	 //[6 Mana] [9/7] The Beast                 ||| [NONE]
        //HungryCrab = Card.Cards.NEW1_017; 			 //[1 Mana] [1/2] Hungry Crab               ||| [NONE]
        //StampedingKodo = Card.Cards.NEW1_041;          //[5 Mana] [3/5] Stampeding Kodo           ||| [NONE]
        //JeweledScarab = Card.Cards.LOE_029; 			 //[2 Mana] [1/1] Jeweled Scarab            ||| [NONE]
        //HugeToad = Card.Cards.LOE_046; 		    	 //[2 Mana] [3/2] Huge Toad                 ||| [NONE]
        //TombSpider = Card.Cards.LOE_047; 		    	 //[4 Mana] [3/3] Tomb Spider               ||| [NONE]
        //CaptainsParrot = Card.Cards.NEW1_016;          //[2 Mana] [1/1] Captain's Parrot          ||| [NONE]
        //HauntedCreeper = Card.Cards.FP1_002; 			 //[2 Mana] [1/2] Haunted Creeper           ||| [NONE]
        //Maexxna = Card.Cards.FP1_010;                  //[6 Mana] [2/8] Maexxna                   ||| [NONE]
        //LostTallstrider = Card.Cards.GVG_071;          //[4 Mana] [5/4] Lost Tallstrider          ||| [NONE]
        //MuklasChampion = Card.Cards.AT_090;            //[5 Mana] [4/3] Mukla's Champion          ||| [NONE]
        //CapturedJormungar = Card.Cards.AT_102;         //[7 Mana] [5/9] Captured Jormungar        ||| [NONE]
        //ArmoredWarhorse = Card.Cards.AT_108;           //[4 Mana] [5/3] Armored Warhorse          ||| [NONE]

        private Card.Cards TombSpiderHandler(List<Card.Cards> choices, Board board, BoardState boardCondition)
        {
            Dictionary<Card.Cards, int> tombSpiderDictionary = new Dictionary<Card.Cards, int>
            {
                {Cards.FierceMonkey, 0},
                {Cards.StarvingBuzzard, 0},
                {Cards.TimberWolf, 0},
                {Cards.TundraRhino, 0},
                {Cards.ScavengingHyena, 0},
                {Cards.SavannahHighmane, 0},
                {Cards.KingKrush, 0},
                {Cards.DesertCamel, 0},
                {Cards.Webspinner, 0},
                {Cards.KingofBeasts, 0},
                {Cards.Gahzrilla, 0},
                {Cards.CoreRager, 0},
                {Cards.KingsElekk, 0},
                {Cards.Acidmaw, 0},
                {Cards.Dreadscale, 0},
                {Cards.PitSnake, 0},
                {Cards.MountedRaptor, 0},
                {Cards.JungleMoonkin, 0},
                {Cards.Malorne, 0},
                {Cards.SavageCombatant, 0},
                {Cards.OasisSnapjaw, 0},
                {Cards.RiverCrocolisk, 0},
                {Cards.IronfurGrizzly, 0},
                {Cards.SilverbackPatriarch, 0},
                {Cards.StonetuskBoar, 0},
                {Cards.BloodfenRaptor, 0},
                {Cards.CoreHound, 0},
                {Cards.YoungDragonhawk, 0},
                {Cards.IronbeakOwl, 0},
                {Cards.AngryChicken, 0},
                {Cards.KingMukla, 0},
                {Cards.JunglePanther, 0},
                {Cards.StranglethornTiger, 0},
                {Cards.DireWolfAlpha, 0},
                {Cards.EmperorCobra, 0},
                {Cards.TheBeast, 0},
                {Cards.HungryCrab, 0},
                {Cards.StampedingKodo, 0},
                {Cards.JeweledScarab, 0},
                {Cards.HugeToad, 0},
                {Cards.TombSpider, 0},
                {Cards.CaptainsParrot, 0},
                {Cards.HauntedCreeper, 0},
                {Cards.Maexxna, 0},
                {Cards.LostTallstrider, 0},
                {Cards.MuklasChampion, 0},
                {Cards.CapturedJormungar, 0},
                {Cards.ArmoredWarhorse, 0},
            };
            List<KeyValuePair<Card.Cards, int>> filteredTable = tombSpiderDictionary.Where(x => choices.Contains(x.Key)).ToList();
            return filteredTable.First(x => x.Value == filteredTable.Max(y => y.Value)).Key;
        }

        //TODO: ==================================================JEWELED SCARAB=================================================
        //Hex = Card.Cards.EX1_246;                 //[3 Mana] [0/0] Hex                        ||| [SHAMAN]
        //FarSight = Card.Cards.CS2_053;            //[3 Mana] [0/0] Far Sight                  ||| [SHAMAN]
        //LavaBurst = Card.Cards.EX1_241;           //[3 Mana] [0/0] Lava Burst                 ||| [SHAMAN]
        //FeralSpirit = Card.Cards.EX1_248;         //[3 Mana] [0/0] Feral Spirit               ||| [SHAMAN]
        //UnboundElemental = Card.Cards.EX1_258;    //[3 Mana] [2/4] Unbound Elemental          ||| [SHAMAN]
        //LightningStorm = Card.Cards.EX1_259;      //[3 Mana] [0/0] Lightning Storm            ||| [SHAMAN]
        //ManaTideTotem = Card.Cards.EX1_575;       //[3 Mana] [0/3] Mana Tide Totem            ||| [SHAMAN]
        //Powermace = Card.Cards.GVG_036;           //[3 Mana] [3/2] Powermace                  ||| [SHAMAN]
        //TuskarrTotemic = Card.Cards.AT_046;       //[3 Mana] [3/2] Tuskarr Totemic            ||| [SHAMAN]
        //HealingWave = Card.Cards.AT_048;          //[3 Mana] [0/0] Healing Wave               ||| [SHAMAN]
        //ElementalDestruction = Card.Cards.AT_051; //[3 Mana] [0/0] Elemental Destruction      ||| [SHAMAN]
        //ShadowWordDeath = Card.Cards.EX1_622;     //[3 Mana] [0/0] Shadow Word: Death         ||| [PRIEST]
        //Thoughtsteal = Card.Cards.EX1_339;        //[3 Mana] [0/0] Thoughtsteal               ||| [PRIEST]
        //Shadowform = Card.Cards.EX1_625;          //[3 Mana] [0/0] Shadowform                 ||| [PRIEST]
        //DarkCultist = Card.Cards.FP1_023; 		//[3 Mana] [3/4] Dark Cultist               ||| [PRIEST]
        //VelensChosen = Card.Cards.GVG_010;        //[3 Mana] [0/0] Velen's Chosen             ||| [PRIEST]
        //Shadowfiend = Card.Cards.AT_014;          //[3 Mana] [3/3] Shadowfiend                ||| [PRIEST]
        //ArcaneIntellect = Card.Cards.CS2_023; 	//[3 Mana] [0/0] Arcane Intellect           ||| [MAGE]
        //FrostNova = Card.Cards.CS2_026; 			//[3 Mana] [0/0] Frost Nova                 ||| [MAGE]
        //Counterspell = Card.Cards.EX1_287; 		//[3 Mana] [0/0] Counterspell               ||| [MAGE]
        //IceBarrier = Card.Cards.EX1_289; 			//[3 Mana] [0/0] Ice Barrier                ||| [MAGE]
        //MirrorEntity = Card.Cards.EX1_294; 		//[3 Mana] [0/0] Mirror Entity              ||| [MAGE]
        //IceBlock = Card.Cards.EX1_295; 			//[3 Mana] [0/0] Ice Block                  ||| [MAGE]
        //Vaporize = Card.Cards.EX1_594; 			//[3 Mana] [0/0] Vaporize                   ||| [MAGE]
        //KirinTorMage = Card.Cards.EX1_612; 		//[3 Mana] [4/3] Kirin Tor Mage             ||| [MAGE]
        //Spellbender = Card.Cards.tt_010; 			//[3 Mana] [0/0] Spellbender                ||| [MAGE]
        //ForgottenTorch = Card.Cards.LOE_002; 		//[3 Mana] [0/0] Forgotten Torch            ||| [MAGE]
        //Duplicate = Card.Cards.FP1_018; 			//[3 Mana] [0/0] Duplicate                  ||| [MAGE]
        //SootSpewer = Card.Cards.GVG_123;          //[3 Mana] [3/3] Soot Spewer                ||| [MAGE]
        //Flamewaker = Card.Cards.BRM_002;          //[3 Mana] [2/4] Flamewaker                 ||| [MAGE]
        //Effigy = Card.Cards.AT_002; 			    //[3 Mana] [0/0] Effigy                     ||| [MAGE]
        //PolymorphBoar = Card.Cards.AT_005; 		//[3 Mana] [0/0] Polymorph: Boar            ||| [MAGE]
        //Spellslinger = Card.Cards.AT_007;         //[3 Mana] [3/4] Spellslinger               ||| [MAGE]
        //DivineFavor = Card.Cards.EX1_349;         //[3 Mana] [0/0] Divine Favor               ||| [PALADIN]
        //SwordofJustice = Card.Cards.EX1_366;      //[3 Mana] [1/5] Sword of Justice           ||| [PALADIN]
        //AldorPeacekeeper = Card.Cards.EX1_382;    //[3 Mana] [3/3] Aldor Peacekeeper          ||| [PALADIN]
        //Coghammer = Card.Cards.GVG_059;           //[3 Mana] [2/3] Coghammer                  ||| [PALADIN]
        //MusterforBattle = Card.Cards.GVG_061;     //[3 Mana] [0/0] Muster for Battle          ||| [PALADIN]
        //ScarletPurifier = Card.Cards.GVG_101;     //[3 Mana] [4/3] Scarlet Purifier           ||| [PALADIN]
        //SealofChampions = Card.Cards.AT_074;      //[3 Mana] [0/0] Seal of Champions          ||| [PALADIN]
        //WarhorseTrainer = Card.Cards.AT_075;      //[3 Mana] [2/4] Warhorse Trainer           ||| [PALADIN]
        //Charge = Card.Cards.CS2_103;              //[3 Mana] [0/0] Charge                     ||| [WARRIOR]
        //WarsongCommander = Card.Cards.EX1_084;    //[3 Mana] [2/3] Warsong Commander          ||| [WARRIOR]
        //ShieldBlock = Card.Cards.EX1_606;         //[3 Mana] [0/0] Shield Block               ||| [WARRIOR]
        //FrothingBerserker = Card.Cards.EX1_604;   //[3 Mana] [2/4] Frothing Berserker         ||| [WARRIOR]
        //FierceMonkey = Card.Cards.LOE_022;        //[3 Mana] [3/4] Fierce Monkey              ||| [WARRIOR]
        //BouncingBlade = Card.Cards.GVG_050;       //[3 Mana] [0/0] Bouncing Blade             ||| [WARRIOR]
        //OgreWarmaul = Card.Cards.GVG_054;         //[3 Mana] [4/2] Ogre Warmaul               ||| [WARRIOR]
        //Bash = Card.Cards.AT_064;                 //[3 Mana] [0/0] Bash                       ||| [WARRIOR]
        //KingsDefender = Card.Cards.AT_065;        //[3 Mana] [3/2] King's Defender            ||| [WARRIOR]
        //OrgrimmarAspirant = Card.Cards.AT_066;    //[3 Mana] [3/3] Orgrimmar Aspirant         ||| [WARRIOR]
        //ShadowBolt = Card.Cards.CS2_057;          //[3 Mana] [0/0] Shadow Bolt                ||| [WARLOCK]
        //DrainLife = Card.Cards.CS2_061;           //[3 Mana] [0/0] Drain Life                 ||| [WARLOCK]
        //Felguard = Card.Cards.EX1_301;            //[3 Mana] [3/5] Felguard                   ||| [WARLOCK]
        //VoidTerror = Card.Cards.EX1_304;          //[3 Mana] [3/3] Void Terror                ||| [WARLOCK]
        //SenseDemons = Card.Cards.EX1_317;         //[3 Mana] [0/0] Sense Demons               ||| [WARLOCK]
        //Demonwrath = Card.Cards.BRM_005;          //[3 Mana] [0/0] Demonwrath                 ||| [WARLOCK]
        //ImpGangBoss = Card.Cards.BRM_006;         //[3 Mana] [2/4] Imp Gang Boss              ||| [WARLOCK]
        //KillCommand = Card.Cards.EX1_539;         //[3 Mana] [0/0] Kill Command               ||| [HUNTER]
        //AnimalCompanion = Card.Cards.NEW1_031;    //[3 Mana] [0/0] Animal Companion           ||| [HUNTER]
        //EaglehornBow = Card.Cards.EX1_536;        //[3 Mana] [3/2] Eaglehorn Bow              ||| [HUNTER]
        //UnleashtheHounds = Card.Cards.EX1_538;    //[3 Mana] [0/0] Unleash the Hounds         ||| [HUNTER]
        //DeadlyShot = Card.Cards.EX1_617;          //[3 Mana] [0/0] Deadly Shot                ||| [HUNTER]
        //DesertCamel = Card.Cards.LOE_020;         //[3 Mana] [2/4] Desert Camel               ||| [HUNTER]
        //MetaltoothLeaper = Card.Cards.GVG_048;    //[3 Mana] [3/3] Metaltooth Leaper          ||| [HUNTER]
        //Powershot = Card.Cards.AT_056;            //[3 Mana] [0/0] Powershot                  ||| [HUNTER]
        //Stablemaster = Card.Cards.AT_057;         //[3 Mana] [4/2] Stablemaster               ||| [HUNTER]
        //Dreadscale = Card.Cards.AT_063t;          //[3 Mana] [4/2] Dreadscale                 ||| [HUNTER]
        //FanofKnives = Card.Cards.EX1_129;         //[3 Mana] [0/0] Fan of Knives              ||| [ROGUE]
        //PerditionsBlade = Card.Cards.EX1_133;     //[3 Mana] [2/2] Perdition's Blade          ||| [ROGUE]
        //SI7Agent = Card.Cards.EX1_134;            //[3 Mana] [3/3] SI:7 Agent                 ||| [ROGUE]
        //Headcrack = Card.Cards.EX1_137;           //[3 Mana] [0/0] Headcrack                  ||| [ROGUE]
        //EdwinVanCleef = Card.Cards.EX1_613;       //[3 Mana] [2/2] Edwin VanCleef             ||| [ROGUE]
        //UnearthedRaptor = Card.Cards.LOE_019;     //[3 Mana] [3/4] Unearthed Raptor           ||| [ROGUE]
        //CogmastersWrench = Card.Cards.GVG_024;    //[3 Mana] [1/3] Cogmaster's Wrench         ||| [ROGUE]
        //IronSensei = Card.Cards.GVG_027;          //[3 Mana] [2/2] Iron Sensei                ||| [ROGUE]
        //ShadyDealer = Card.Cards.AT_032;          //[3 Mana] [4/3] Shady Dealer               ||| [ROGUE]
        //Burgle = Card.Cards.AT_033;               //[3 Mana] [0/0] Burgle                     ||| [ROGUE]
        //BeneaththeGrounds = Card.Cards.AT_035;    //[3 Mana] [0/0] Beneath the Grounds        ||| [ROGUE]
        //HealingTouch = Card.Cards.CS2_007;        //[3 Mana] [0/0] Healing Touch              ||| [DRUID]
        //SavageRoar = Card.Cards.CS2_011;          //[3 Mana] [0/0] Savage Roar                ||| [DRUID]
        //MarkofNature = Card.Cards.EX1_155;        //[3 Mana] [0/0] Mark of Nature             ||| [DRUID]
        //MountedRaptor = Card.Cards.LOE_050;       //[3 Mana] [3/2] Mounted Raptor             ||| [DRUID]
        //GroveTender = Card.Cards.GVG_032;         //[3 Mana] [2/4] Grove Tender               ||| [DRUID]
        //DruidoftheFlame = Card.Cards.BRM_010;     //[3 Mana] [2/2] Druid of the Flame         ||| [DRUID]
        //Mulch = Card.Cards.AT_044;                //[3 Mana] [0/0] Mulch                      ||| [DRUID]
        //MagmaRager = Card.Cards.CS2_118;          //[3 Mana] [5/1] Magma Rager                ||| [NONE]
        //RaidLeader = Card.Cards.CS2_122;          //[3 Mana] [2/2] Raid Leader                ||| [NONE]
        //Wolfrider = Card.Cards.CS2_124;           //[3 Mana] [3/1] Wolfrider                  ||| [NONE]
        //IronfurGrizzly = Card.Cards.CS2_125;      //[3 Mana] [3/3] Ironfur Grizzly            ||| [NONE]
        //SilverbackPatriarch = Card.Cards.CS2_127; //[3 Mana] [1/4] Silverback Patriarch       ||| [NONE]
        //IronforgeRifleman = Card.Cards.CS2_141;   //[3 Mana] [2/2] Ironforge Rifleman         ||| [NONE]
        //RazorfenHunter = Card.Cards.CS2_196;      //[3 Mana] [2/3] Razorfen Hunter            ||| [NONE]
        //ShatteredSunCleric = Card.Cards.EX1_019;  //[3 Mana] [3/2] Shattered Sun Cleric       ||| [NONE]
        //DalaranMage = Card.Cards.EX1_582;         //[3 Mana] [1/4] Dalaran Mage               ||| [NONE]
        //EarthenRingFarseer = Card.Cards.CS2_117;  //[3 Mana] [3/3] Earthen Ring Farseer       ||| [NONE]
        //InjuredBlademaster = Card.Cards.CS2_181;  //[3 Mana] [4/7] Injured Blademaster        ||| [NONE]
        //BigGameHunter = Card.Cards.EX1_005;       //[3 Mana] [4/2] Big Game Hunter            ||| [NONE]
        //AlarmoBot = Card.Cards.EX1_006;           //[3 Mana] [0/3] Alarm-o-Bot                ||| [NONE]
        //AcolyteofPain = Card.Cards.EX1_007;       //[3 Mana] [1/3] Acolyte of Pain            ||| [NONE]
        //KingMukla = Card.Cards.EX1_014;           //[3 Mana] [5/5] King Mukla                 ||| [NONE]
        //JunglePanther = Card.Cards.EX1_017;       //[3 Mana] [4/2] Jungle Panther             ||| [NONE]
        //ScarletCrusader = Card.Cards.EX1_020;     //[3 Mana] [3/1] Scarlet Crusader           ||| [NONE]
        //ThrallmarFarseer = Card.Cards.EX1_021;    //[3 Mana] [2/3] Thrallmar Farseer          ||| [NONE]
        //QuestingAdventurer = Card.Cards.EX1_044;  //[3 Mana] [2/2] Questing Adventurer        ||| [NONE]
        //ColdlightOracle = Card.Cards.EX1_050;     //[3 Mana] [2/2] Coldlight Oracle           ||| [NONE]
        //TinkmasterOverspark = Card.Cards.EX1_083; //[3 Mana] [3/3] Tinkmaster Overspark       ||| [NONE]
        //MindControlTech = Card.Cards.EX1_085;     //[3 Mana] [3/3] Mind Control Tech          ||| [NONE]
        //ArcaneGolem = Card.Cards.EX1_089;         //[3 Mana] [4/2] Arcane Golem               ||| [NONE]
        //Demolisher = Card.Cards.EX1_102;          //[3 Mana] [1/4] Demolisher                 ||| [NONE]
        //ColdlightSeer = Card.Cards.EX1_103;       //[3 Mana] [2/3] Coldlight Seer             ||| [NONE]
        //EmperorCobra = Card.Cards.EX1_170;        //[3 Mana] [2/3] Emperor Cobra              ||| [NONE]
        //TaurenWarrior = Card.Cards.EX1_390;       //[3 Mana] [2/3] Tauren Warrior             ||| [NONE]
        //RagingWorgen = Card.Cards.EX1_412;        //[3 Mana] [3/3] Raging Worgen              ||| [NONE]
        //MurlocWarleader = Card.Cards.EX1_507;     //[3 Mana] [3/3] Murloc Warleader           ||| [NONE]
        //HarvestGolem = Card.Cards.EX1_556; 		//[3 Mana] [2/3] Harvest Golem              ||| [NONE]
        //BloodKnight = Card.Cards.EX1_590;         //[3 Mana] [3/3] Blood Knight               ||| [NONE]
        //ImpMaster = Card.Cards.EX1_597;           //[3 Mana] [1/5] Imp Master                 ||| [NONE]
        //SouthseaCaptain = Card.Cards.NEW1_027;    //[3 Mana] [3/3] Southsea Captain           ||| [NONE]
        //FlesheatingGhoul = Card.Cards.tt_004;     //[3 Mana] [2/3] Flesheating Ghoul          ||| [NONE]
        //BrannBronzebeard = Card.Cards.LOE_077;    //[3 Mana] [2/4] Brann Bronzebeard          ||| [NONE]
        //ShadeofNaxxramas = Card.Cards.FP1_005;    //[3 Mana] [2/2] Shade of Naxxramas         ||| [NONE]
        //Deathlord = Card.Cards.FP1_009; 			//[3 Mana] [2/8] Deathlord                  ||| [NONE]
        //StoneskinGargoyle = Card.Cards.FP1_027;   //[3 Mana] [1/4] Stoneskin Gargoyle         ||| [NONE]
        //DancingSwords = Card.Cards.FP1_029; 		//[3 Mana] [4/4] Dancing Swords             ||| [NONE]
        //SpiderTank = Card.Cards.GVG_044;          //[3 Mana] [3/4] Spider Tank                ||| [NONE]
        //OgreBrute = Card.Cards.GVG_065;           //[3 Mana] [4/4] Ogre Brute                 ||| [NONE]
        //FlyingMachine = Card.Cards.GVG_084;       //[3 Mana] [1/4] Flying Machine             ||| [NONE]
        //Illuminator = Card.Cards.GVG_089;         //[3 Mana] [2/4] Illuminator                ||| [NONE]
        //GnomishExperimenter = Card.Cards.GVG_092; //[3 Mana] [3/2] Gnomish Experimenter       ||| [NONE]
        //GoblinSapper = Card.Cards.GVG_095;        //[3 Mana] [2/4] Goblin Sapper              ||| [NONE]
        //LilExorcist = Card.Cards.GVG_097;         //[3 Mana] [2/3] Lil' Exorcist              ||| [NONE]
        //GnomereganInfantry = Card.Cards.GVG_098;  //[3 Mana] [1/4] Gnomeregan Infantry        ||| [NONE]
        //TinkertownTechnician = Card.Cards.GVG_102;//[3 Mana] [3/3] Tinkertown Technician      ||| [NONE]
        //Hobgoblin = Card.Cards.GVG_104;           //[3 Mana] [2/3] Hobgoblin                  ||| [NONE]
        //BlackwingTechnician = Card.Cards.BRM_033; //[3 Mana] [2/4] Blackwing Technician       ||| [NONE]
        //DragonhawkRider = Card.Cards.AT_083;      //[3 Mana] [3/3] Dragonhawk Rider           ||| [NONE]
        //Saboteur = Card.Cards.AT_086;             //[3 Mana] [4/3] Saboteur                   ||| [NONE]
        //ArgentHorserider = Card.Cards.AT_087;     //[3 Mana] [2/1] Argent Horserider          ||| [NONE]
        //IceRager = Card.Cards.AT_092;             //[3 Mana] [5/2] Ice Rager                  ||| [NONE]
        //SilentKnight = Card.Cards.AT_095;         //[3 Mana] [2/2] Silent Knight              ||| [NONE]
        //SilverHandRegent = Card.Cards.AT_100;     //[3 Mana] [3/3] Silver Hand Regent         ||| [NONE]
        //LightsChampion = Card.Cards.AT_106;       //[3 Mana] [4/3] Light's Champion           ||| [NONE]
        //ColiseumManager = Card.Cards.AT_110;      //[3 Mana] [2/5] Coliseum Manager           ||| [NONE]
        //FencingCoach = Card.Cards.AT_115;         //[3 Mana] [2/2] Fencing Coach              ||| [NONE]
        //MasterofCeremonies = Card.Cards.AT_117;   //[3 Mana] [4/2] Master of Ceremonies       ||| [NONE]
        //FjolaLightbane = Card.Cards.AT_129;       //[3 Mana] [3/4] Fjola Lightbane            ||| [NONE]
        //EydisDarkbane = Card.Cards.AT_131;        //[3 Mana] [3/4] Eydis Darkbane             ||| [NONE]

        private Card.Cards JeweledScarabHandler(List<Card.Cards> choices, Board board, BoardState boardCondition)
        {
            Dictionary<Card.Cards, int> jeweledScarabDictionary = new Dictionary<Card.Cards, int>
            {
                {Cards.Hex, 0},
                {Cards.FarSight, 0},
                {Cards.LavaBurst, 0},
                {Cards.FeralSpirit, 0},
                {Cards.UnboundElemental, 0},
                {Cards.LightningStorm, 0},
                {Cards.ManaTideTotem, 0},
                {Cards.Powermace, 0},
                {Cards.TuskarrTotemic, 0},
                {Cards.HealingWave, 0},
                {Cards.ElementalDestruction, 0},
                {Cards.ShadowWordDeath, 0},
                {Cards.Thoughtsteal, 0},
                {Cards.Shadowform, 0},
                {Cards.DarkCultist, 0},
                {Cards.VelensChosen, 0},
                {Cards.Shadowfiend, 0},
                {Cards.ArcaneIntellect, 0},
                {Cards.FrostNova, 0},
                {Cards.Counterspell, 0},
                {Cards.IceBarrier, 0},
                {Cards.MirrorEntity, 0},
                {Cards.IceBlock, 0},
                {Cards.Vaporize, 0},
                {Cards.KirinTorMage, 0},
                {Cards.Spellbender, 0},
                {Cards.ForgottenTorch, 0},
                {Cards.Duplicate, 0},
                {Cards.SootSpewer, 0},
                {Cards.Flamewaker, 0},
                {Cards.Effigy, 0},
                {Cards.PolymorphBoar, 0},
                {Cards.Spellslinger, 0},
                {Cards.DivineFavor, 0},
                {Cards.SwordofJustice, 0},
                {Cards.AldorPeacekeeper, 0},
                {Cards.Coghammer, 0},
                {Cards.MusterforBattle, 0},
                {Cards.ScarletPurifier, 0},
                {Cards.SealofChampions, 0},
                {Cards.WarhorseTrainer, 0},
                {Cards.Charge, 0},
                {Cards.WarsongCommander, 0},
                {Cards.ShieldBlock, 0},
                {Cards.FrothingBerserker, 0},
                {Cards.FierceMonkey, 0},
                {Cards.BouncingBlade, 0},
                {Cards.OgreWarmaul, 0},
                {Cards.Bash, 0},
                {Cards.KingsDefender, 0},
                {Cards.OrgrimmarAspirant, 0},
                {Cards.ShadowBolt, 0},
                {Cards.DrainLife, 0},
                {Cards.Felguard, 0},
                {Cards.VoidTerror, 0},
                {Cards.SenseDemons, 0},
                {Cards.Demonwrath, 0},
                {Cards.ImpGangBoss, 0},
                {Cards.KillCommand, 0},
                {Cards.AnimalCompanion, 0},
                {Cards.EaglehornBow, 0},
                {Cards.UnleashtheHounds, 0},
                {Cards.DeadlyShot, 0},
                {Cards.DesertCamel, 0},
                {Cards.MetaltoothLeaper, 0},
                {Cards.Powershot, 0},
                {Cards.Stablemaster, 0},
                {Cards.Dreadscale, 0},
                {Cards.FanofKnives, 0},
                {Cards.PerditionsBlade, 0},
                {Cards.SI7Agent, 0},
                {Cards.Headcrack, 0},
                {Cards.EdwinVanCleef, 0},
                {Cards.UnearthedRaptor, 0},
                {Cards.CogmastersWrench, 0},
                {Cards.IronSensei, 0},
                {Cards.ShadyDealer, 0},
                {Cards.Burgle, 0},
                {Cards.BeneaththeGrounds, 0},
                {Cards.HealingTouch, 0},
                {Cards.SavageRoar, 0},
                {Cards.MarkofNature, 0},
                {Cards.MountedRaptor, 0},
                {Cards.GroveTender, 0},
                {Cards.DruidoftheFlame, 0},
                {Cards.Mulch, 0},
                {Cards.MagmaRager, 0},
                {Cards.RaidLeader, 0},
                {Cards.Wolfrider, 0},
                {Cards.IronfurGrizzly, 0},
                {Cards.SilverbackPatriarch, 0},
                {Cards.IronforgeRifleman, 0},
                {Cards.RazorfenHunter, 0},
                {Cards.ShatteredSunCleric, 0},
                {Cards.DalaranMage, 0},
                {Cards.EarthenRingFarseer, 0},
                {Cards.InjuredBlademaster, 0},
                {Cards.BigGameHunter, board.HasBghTarget() ? 100 : 0},
                {Cards.AlarmoBot, 0},
                {Cards.AcolyteofPain, 0},
                {Cards.KingMukla, 0},
                {Cards.JunglePanther, 0},
                {Cards.ScarletCrusader, 0},
                {Cards.ThrallmarFarseer, 0},
                {Cards.QuestingAdventurer, 0},
                {Cards.ColdlightOracle, 0},
                {Cards.TinkmasterOverspark, 0},
                {Cards.MindControlTech, 0},
                {Cards.ArcaneGolem, 0},
                {Cards.Demolisher, 0},
                {Cards.ColdlightSeer, 0},
                {Cards.EmperorCobra, 0},
                {Cards.TaurenWarrior, 0},
                {Cards.RagingWorgen, 0},
                {Cards.MurlocWarleader, 0},
                {Cards.HarvestGolem, 0},
                {Cards.BloodKnight, 0},
                {Cards.ImpMaster, 0},
                {Cards.SouthseaCaptain, 0},
                {Cards.FlesheatingGhoul, 0},
                {Cards.BrannBronzebeard, 0},
                {Cards.ShadeofNaxxramas, 0},
                {Cards.Deathlord, 0},
                {Cards.StoneskinGargoyle, 0},
                {Cards.DancingSwords, 0},
                {Cards.SpiderTank, 0},
                {Cards.OgreBrute, 0},
                {Cards.FlyingMachine, 0},
                {Cards.Illuminator, 0},
                {Cards.GnomishExperimenter, 0},
                {Cards.GoblinSapper, 0},
                {Cards.LilExorcist, 0},
                {Cards.GnomereganInfantry, 0},
                {Cards.TinkertownTechnician, 0},
                {Cards.Hobgoblin, 0},
                {Cards.BlackwingTechnician, 0},
                {Cards.DragonhawkRider, 0},
                {Cards.Saboteur, 0},
                {Cards.ArgentHorserider, 0},
                {Cards.IceRager, 0},
                {Cards.SilentKnight, 0},
                {Cards.SilverHandRegent, 0},
                {Cards.LightsChampion, 0},
                {Cards.ColiseumManager, 0},
                {Cards.FencingCoach, 0},
                {Cards.MasterofCeremonies, 0},
                {Cards.FjolaLightbane, 0},
                {Cards.EydisDarkbane, 0},
            };
            var damageTuple = CalculateNeededDamageForLethal(board);
            int withTaunt = damageTuple.Item1;
            int withoutTaunt = damageTuple.Item2;
            if (board.Hand.Any(c => c.Template.IsSecret))
                jeweledScarabDictionary.AddOrUpdate(Cards.KirinTorMage, 100);
            //This would look like crap with ternary operator
            if (board.MinionFriend.Any(minion => minion.Template.Race == Card.CRace.BEAST)
                && withTaunt <= 5 //has a beast and needs 5 damage for lethal
                || withoutTaunt <= 3) //or needs to deal 3 damage to kill
            {
                jeweledScarabDictionary.AddOrUpdate(Cards.KillCommand, 100);
            }
            List<KeyValuePair<Card.Cards, int>> filteredTable = jeweledScarabDictionary.Where(x => choices.Contains(x.Key)).ToList();
            return filteredTable.First(x => x.Value == filteredTable.Max(y => y.Value)).Key;
        }

        //WhirlingZapomatic = Card.Cards.GVG_037;       //[2 Mana] [3/2] Whirling Zap-o-matic       ||| [SHAMAN]
        //Shadowboxer = Card.Cards.GVG_072;             //[2 Mana] [2/3] Shadowboxer                ||| [PRIEST]
        //UpgradedRepairBot = Card.Cards.GVG_083;       //[5 Mana] [5/5] Upgraded Repair Bot        ||| [PRIEST]
        //Snowchugger = Card.Cards.GVG_002;             //[2 Mana] [2/3] Snowchugger                ||| [MAGE]
        //FlameLeviathan = Card.Cards.GVG_007;          //[7 Mana] [7/7] Flame Leviathan            ||| [MAGE]
        //SootSpewer = Card.Cards.GVG_123; 			    //[3 Mana] [3/3] Soot Spewer                ||| [MAGE]
        //ShieldedMinibot = Card.Cards.GVG_058;         //[2 Mana] [2/2] Shielded Minibot           ||| [PALADIN]
        //CobaltGuardian = Card.Cards.GVG_062;          //[5 Mana] [6/3] Cobalt Guardian            ||| [PALADIN]
        //Warbot = Card.Cards.GVG_051;                  //[1 Mana] [1/3] Warbot                     ||| [WARRIOR]
        //ScrewjankClunker = Card.Cards.GVG_055;        //[4 Mana] [2/5] Screwjank Clunker          ||| [WARRIOR]
        //IronJuggernaut = Card.Cards.GVG_056;          //[6 Mana] [6/5] Iron Juggernaut            ||| [WARRIOR]
        //SiegeEngine = Card.Cards.GVG_086;             //[5 Mana] [5/5] Siege Engine               ||| [WARRIOR]
        //FelCannon = Card.Cards.GVG_020;               //[4 Mana] [3/5] Fel Cannon                 ||| [WARLOCK]
        //AnimaGolem = Card.Cards.GVG_077;              //[6 Mana] [9/9] Anima Golem                ||| [WARLOCK]
        //MetaltoothLeaper = Card.Cards.GVG_048; 		//[3 Mana] [3/3] Metaltooth Leaper          ||| [HUNTER]
        //GoblinAutoBarber = Card.Cards.GVG_023;        //[2 Mana] [3/2] Goblin Auto-Barber         ||| [ROGUE]
        //IronSensei = Card.Cards.GVG_027; 			    //[3 Mana] [2/2] Iron Sensei                ||| [ROGUE]
        //AnodizedRoboCub = Card.Cards.GVG_030;         //[2 Mana] [2/2] Anodized Robo Cub          ||| [DRUID]
        //MechBearCat = Card.Cards.GVG_034;             //[6 Mana] [7/6] Mech-Bear-Cat              ||| [DRUID]
        //AlarmoBot = Card.Cards.EX1_006; 			    //[3 Mana] [0/3] Alarm-o-Bot                ||| [NONE]
        //Demolisher = Card.Cards.EX1_102; 			    //[3 Mana] [1/4] Demolisher                 ||| [NONE]
        //HarvestGolem = Card.Cards.EX1_556; 			//[3 Mana] [2/3] Harvest Golem              ||| [NONE]
        //GorillabotA3 = Card.Cards.LOE_039; 			//[4 Mana] [3/4] Gorillabot A-3             ||| [NONE]
        //Mechwarper = Card.Cards.GVG_006;              //[2 Mana] [2/3] Mechwarper                 ||| [NONE]
        //FelReaver = Card.Cards.GVG_016;               //[5 Mana] [8/8] Fel Reaver                 ||| [NONE]
        //SpiderTank = Card.Cards.GVG_044; 			    //[3 Mana] [3/4] Spider Tank                ||| [NONE]
        //AntiqueHealbot = Card.Cards.GVG_069;          //[5 Mana] [3/3] Antique Healbot            ||| [NONE]
        //ExplosiveSheep = Card.Cards.GVG_076; 			//[2 Mana] [1/1] Explosive Sheep            ||| [NONE]
        //MechanicalYeti = Card.Cards.GVG_078; 			//[4 Mana] [4/5] Mechanical Yeti            ||| [NONE]
        //ForceTankMAX = Card.Cards.GVG_079;            //[8 Mana] [7/7] Force-Tank MAX             ||| [NONE]
        //ClockworkGnome = Card.Cards.GVG_082; 			//[1 Mana] [2/1] Clockwork Gnome            ||| [NONE]
        //FlyingMachine = Card.Cards.GVG_084; 			//[3 Mana] [1/4] Flying Machine             ||| [NONE]
        //AnnoyoTron = Card.Cards.GVG_085;              //[2 Mana] [1/2] Annoy-o-Tron               ||| [NONE]
        //ArcaneNullifierX21 = Card.Cards.GVG_091;      //[4 Mana] [2/5] Arcane Nullifier X-21      ||| [NONE]
        //TargetDummy = Card.Cards.GVG_093;             //[0 Mana] [0/2] Target Dummy               ||| [NONE]
        //Jeeves = Card.Cards.GVG_094;                  //[4 Mana] [1/4] Jeeves                     ||| [NONE]
        //PilotedShredder = Card.Cards.GVG_096; 		//[4 Mana] [4/3] Piloted Shredder           ||| [NONE]
        //MicroMachine = Card.Cards.GVG_103;            //[2 Mana] [1/2] Micro Machine              ||| [NONE]
        //PilotedSkyGolem = Card.Cards.GVG_105; 		//[6 Mana] [6/4] Piloted Sky Golem          ||| [NONE]
        //Junkbot = Card.Cards.GVG_106;                 //[5 Mana] [1/5] Junkbot                    ||| [NONE]
        //EnhanceoMechano = Card.Cards.GVG_107;         //[4 Mana] [3/2] Enhance-o Mechano          ||| [NONE]
        //MimironsHead = Card.Cards.GVG_111;            //[5 Mana] [4/5] Mimiron's Head             ||| [NONE]
        //FoeReaper4000 = Card.Cards.GVG_113;           //[8 Mana] [6/9] Foe Reaper 4000            ||| [NONE]
        //SneedsOldShredder = Card.Cards.GVG_114; 		//[8 Mana] [5/7] Sneed's Old Shredder       ||| [NONE]
        //MekgineerThermaplugg = Card.Cards.GVG_116;    //[9 Mana] [9/7] Mekgineer Thermaplugg      ||| [NONE]
        //Blingtron3000 = Card.Cards.GVG_119;           //[5 Mana] [3/4] Blingtron 3000             ||| [NONE]
        //ClockworkGiant = Card.Cards.GVG_121;          //[12 Mana] [8/8] Clockwork Giant           ||| [NONE]
        //ClockworkKnight = Card.Cards.AT_096;          //[5 Mana] [5/5] Clockwork Knight           ||| [NONE]

        private Card.Cards GorillabotA3Handler(List<Card.Cards> choices, Board board, BoardState boardCondition)
        {
            var early = board.MaxMana <= 3;
            var mid = board.MaxMana > 3 && board.MaxMana <=7;
            var late = board.MaxMana > 3;
            Dictionary<Card.Cards, int> gorilaBotList = new Dictionary<Card.Cards, int>
            {
                {Cards.WhirlingZapomatic, 0},
                {Cards.Shadowboxer, 0},
                {Cards.UpgradedRepairBot, 0},
                {Cards.Snowchugger, 30},
                {Cards.FlameLeviathan, 50},
                {Cards.SootSpewer, 0},
                {Cards.ShieldedMinibot, 30},
                {Cards.CobaltGuardian, 0},
                {Cards.Warbot, 0},
                {Cards.ScrewjankClunker, 0},
                {Cards.IronJuggernaut, 50},
                {Cards.SiegeEngine, 10},
                {Cards.FelCannon, 0},
                {Cards.AnimaGolem, 0},
                {Cards.MetaltoothLeaper, 0},
                {Cards.GoblinAutoBarber, 0},
                {Cards.IronSensei, 0},
                {Cards.AnodizedRoboCub, 0},
                {Cards.MechBearCat, 0},
                {Cards.AlarmoBot, 0},
                {Cards.Demolisher, 0},
                {Cards.HarvestGolem, 0},
                {Cards.GorillabotA3, 0},
                {Cards.Mechwarper, 0},
                {Cards.FelReaver, 0},
                {Cards.SpiderTank, 0},
                {Cards.AntiqueHealbot, 0},
                {Cards.ExplosiveSheep, 0},
                {Cards.MechanicalYeti, 0},
                {Cards.ForceTankMAX, 0},
                {Cards.ClockworkGnome, 0},
                {Cards.FlyingMachine, 0},
                {Cards.AnnoyoTron, 0},
                {Cards.ArcaneNullifierX21, 0},
                {Cards.TargetDummy, 0},
                {Cards.Jeeves, 0},
                {Cards.PilotedShredder, 0},
                {Cards.MicroMachine, 0},
                {Cards.PilotedSkyGolem, 0},
                {Cards.Junkbot, 0},
                {Cards.EnhanceoMechano, 0},
                {Cards.MimironsHead, 0},
                {Cards.FoeReaper4000, 0},
                {Cards.SneedsOldShredder, 0},
                {Cards.MekgineerThermaplugg, 0},
                {Cards.Blingtron3000, 0},
                {Cards.ClockworkGiant, 0},
                {Cards.ClockworkKnight, 0},
            };
            foreach (var q in choices)
            {
                
            }
            List<KeyValuePair<Card.Cards, int>> filteredTable = gorilaBotList.Where(x => choices.Contains(x.Key)).ToList();
            return filteredTable.First(x => x.Value == filteredTable.Max(y => y.Value)).Key;
        }

        //======================================================MUSEUM CURATOR=======================================================
        //DarkCultist = Card.Cards.FP1_023;             //[3 Mana] [3/4] Dark Cultist               ||| [PRIEST]
        //BloodmageThalnos = Card.Cards.EX1_012;        //[2 Mana] [1/1] Bloodmage Thalnos          ||| [NONE]
        //SylvanasWindrunner = Card.Cards.EX1_016;      //[6 Mana] [5/5] Sylvanas Windrunner        ||| [NONE]
        //LeperGnome = Card.Cards.EX1_029;              //[1 Mana] [2/1] Leper Gnome                ||| [NONE]
        //LootHoarder = Card.Cards.EX1_096;             //[2 Mana] [2/1] Loot Hoarder               ||| [NONE]
        //Abomination = Card.Cards.EX1_097;             //[5 Mana] [4/4] Abomination                ||| [NONE]
        //CairneBloodhoof = Card.Cards.EX1_110;         //[6 Mana] [4/5] Cairne Bloodhoof           ||| [NONE]
        //HarvestGolem = Card.Cards.EX1_556;            //[3 Mana] [2/3] Harvest Golem              ||| [NONE]
        //TheBeast = Card.Cards.EX1_577;                //[6 Mana] [9/7] The Beast                  ||| [NONE]
        //HugeToad = Card.Cards.LOE_046;                //[2 Mana] [3/2] Huge Toad                  ||| [NONE]
        //AnubisathSentinel = Card.Cards.LOE_061;       //[5 Mana] [4/4] Anubisath Sentinel         ||| [NONE]
        //WobblingRunts = Card.Cards.LOE_089;           //[6 Mana] [2/6] Wobbling Runts             ||| [NONE]
        //ZombieChow = Card.Cards.FP1_001;              //[1 Mana] [2/3] Zombie Chow                ||| [NONE]
        //HauntedCreeper = Card.Cards.FP1_002;          //[2 Mana] [1/2] Haunted Creeper            ||| [NONE]
        //MadScientist = Card.Cards.FP1_004;            //[2 Mana] [2/2] Mad Scientist              ||| [NONE]
        //NerubianEgg = Card.Cards.FP1_007;             //[2 Mana] [0/2] Nerubian Egg               ||| [NONE]
        //Deathlord = Card.Cards.FP1_009;               //[3 Mana] [2/8] Deathlord                  ||| [NONE]
        //SludgeBelcher = Card.Cards.FP1_012;           //[5 Mana] [3/5] Sludge Belcher             ||| [NONE]
        //Stalagg = Card.Cards.FP1_014;                 //[5 Mana] [7/4] Stalagg                    ||| [NONE]
        //Feugen = Card.Cards.FP1_015;                  //[5 Mana] [4/7] Feugen                     ||| [NONE]
        //UnstableGhoul = Card.Cards.FP1_024;           //[2 Mana] [1/3] Unstable Ghoul             ||| [NONE]
        //DancingSwords = Card.Cards.FP1_029;           //[3 Mana] [4/4] Dancing Swords             ||| [NONE]
        //ExplosiveSheep = Card.Cards.GVG_076;          //[2 Mana] [1/1] Explosive Sheep            ||| [NONE]
        //MechanicalYeti = Card.Cards.GVG_078;          //[4 Mana] [4/5] Mechanical Yeti            ||| [NONE]
        //ClockworkGnome = Card.Cards.GVG_082;          //[1 Mana] [2/1] Clockwork Gnome            ||| [NONE]
        //PilotedShredder = Card.Cards.GVG_096;         //[4 Mana] [4/3] Piloted Shredder           ||| [NONE]
        //PilotedSkyGolem = Card.Cards.GVG_105;         //[6 Mana] [6/4] Piloted Sky Golem          ||| [NONE]
        //SneedsOldShredder = Card.Cards.GVG_114;       //[8 Mana] [5/7] Sneed's Old Shredder       ||| [NONE]
        //Toshley = Card.Cards.GVG_115;                 //[6 Mana] [5/7] Toshley                    ||| [NONE]
        //MajordomoExecutus = Card.Cards.BRM_027;       //[9 Mana] [9/7] Majordomo Executus         ||| [NONE]
        //Chillmaw = Card.Cards.AT_123;                 //[7 Mana] [6/6] Chillmaw                   ||| [NONE]
        //TheSkeletonKnight = Card.Cards.AT_128;        //[6 Mana] [7/4] The Skeleton Knight        ||| [NONE] 

        private Card.Cards MuseumCuratorHandler(List<Card.Cards> choices, Board board, BoardState boardCondition)
        {
            Dictionary<Card.Cards, int> museumCuratorDictionary = new Dictionary<Card.Cards, int>
            {
                {Cards.DarkCultist, 0},
                {Cards.BloodmageThalnos, 0},
                {Cards.SylvanasWindrunner, 0},
                {Cards.LeperGnome, 0},
                {Cards.LootHoarder, 0},
                {Cards.Abomination, 0},
                {Cards.CairneBloodhoof, 0},
                {Cards.HarvestGolem, 0},
                {Cards.TheBeast, 0},
                {Cards.HugeToad, 0},
                {Cards.AnubisathSentinel, 0},
                {Cards.WobblingRunts, 0},
                {Cards.ZombieChow, 0},
                {Cards.HauntedCreeper, 0},
                {Cards.MadScientist, 0},
                {Cards.NerubianEgg, 0},
                {Cards.Deathlord, 0},
                {Cards.SludgeBelcher, 0},
                {Cards.Stalagg, 0},
                {Cards.Feugen, 0},
                {Cards.UnstableGhoul, 0},
                {Cards.DancingSwords, 0},
                {Cards.ExplosiveSheep, 0},
                {Cards.MechanicalYeti, 0},
                {Cards.ClockworkGnome, 0},
                {Cards.PilotedShredder, 0},
                {Cards.PilotedSkyGolem, 0},
                {Cards.SneedsOldShredder, 0},
                {Cards.Toshley, 0},
                {Cards.MajordomoExecutus, 0},
                {Cards.Chillmaw, 0},
                {Cards.TheSkeletonKnight, 0},
            };
            List<KeyValuePair<Card.Cards, int>> filteredTable = museumCuratorDictionary.Where(x => choices.Contains(x.Key)).ToList();
            return filteredTable.First(x => x.Value == filteredTable.Max(y => y.Value)).Key;
        }

        private BoardState GetBoardCondition(Board board)
        {
            /* TODO: Take into account lethal, card advantage, card draw, fatigue, actually look at what minions enemy has on board vs yours. Work on this.
             * This board state conditions will not always be correct. 
             */
            if (board.TurnCount < 4)
                return BoardState.Even;
            if ((board.EnemyCardCount > board.Hand.Count + 3) && (board.MinionEnemy.Count > board.MinionFriend.Count))
                return BoardState.Losing;
            if ((board.EnemyCardCount + 3 < board.Hand.Count) && (board.MinionEnemy.Count < board.MinionFriend.Count))
                return BoardState.Winning;
            return BoardState.Even;
        }
    }

    public enum BoardState
    {
        Even,
        Winning,
        Losing
    }
}