using System;
using System.Collections.Generic;
using System.Linq;
using SmartBot.Discover;
using SmartBot.Plugins.API;

namespace Discover
{
    internal class DiscoverTemplate : DiscoverPickHandler
    {
        private const Card.Cards DarkPeddler = Card.Cards.LOE_023;
        private const Card.Cards EtherealConjurer = Card.Cards.LOE_003;
        private const Card.Cards GorillabotA3 = Card.Cards.LOE_039;
        private const Card.Cards JeweledScarab = Card.Cards.LOE_029;
        private const Card.Cards MuseumCurator = Card.Cards.LOE_006;
        private const Card.Cards RavenIdol = Card.Cards.LOE_115;
        private const Card.Cards TombSpider = Card.Cards.LOE_047;
        private const Card.Cards ArchThiefRafaam = Card.Cards.LOE_092;
        private const Card.Cards SirFinleyMrrgglton = Card.Cards.LOE_076;

        private const Card.Cards SteadyShot = Card.Cards.DS1h_292;
        private const Card.Cards Shapeshift = Card.Cards.CS2_017;
        private const Card.Cards LifeTap = Card.Cards.CS2_056;
        private const Card.Cards Fireblast = Card.Cards.CS2_034;
        private const Card.Cards Reinforce = Card.Cards.CS2_101;
        private const Card.Cards ArmorUp = Card.Cards.CS2_102;
        private const Card.Cards LesserHeal = Card.Cards.CS1h_001;
        private const Card.Cards DaggerMastery = Card.Cards.CS2_083b;

        public readonly Dictionary<Card.Cards, int> _heroPowersPriorityTable = new Dictionary<Card.Cards, int>
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
            BoardState BoardCondition = GetBoardCondition(board);
            bool Even = BoardCondition == BoardState.Even;
            bool Losing = BoardCondition == BoardState.Losing;
            bool Winning = BoardCondition == BoardState.Winning;

            switch (originCard)
            {
                case DarkPeddler:
                    return DarkPeddlerHandler(choices, board, BoardCondition);
                    
                case EtherealConjurer:
                    return EtherealConjurerHandler(choices, board, BoardCondition);
                    
                case GorillabotA3:
                    return GorillabotA3Handler(choices, board, BoardCondition);
                    
                case JeweledScarab:
                    return JeweledScarabHandler(choices, board, BoardCondition);
                    
                case MuseumCurator:
                    return MuseumCuratorHandler(choices, board, BoardCondition);
                    
                case RavenIdol:
                    return choices[0];
                    
                case TombSpider:
                    return TombSpiderHandler(choices, board, BoardCondition);
                    
                case ArchThiefRafaam:
                    //TODO: This should be easy, opponent without aoe? mummies, winning? lantern, opponent health below 5 and no board on both sides Clock? 
                    return Winning ? choices[0] : Losing ? choices[1] : board.HeroEnemy.CurrentHealth < 5 ? choices[2]: choices[0]; 
                    
                case SirFinleyMrrgglton:
                    //TODO: Take into account mode and deck you are playing. 
                    //TODO: Arena <Fireblast > Dagger > Tokens > else> || and so on. 
                    List<KeyValuePair<Card.Cards, int>> filteredTable = _heroPowersPriorityTable.Where(x => choices.Contains(x.Key)).ToList();
                    return filteredTable.First(x => x.Value == filteredTable.Max(y => y.Value)).Key;
                    

            }
            return choices[0];
        }
        //TODO: ==================================================DARK PEDDLER=================================================
        public const Card.Cards Corruption = Card.Cards.CS2_063; 			 //[1 Mana] [0/0] Corruption ||| [WARLOCK]
        public const Card.Cards Voidwalker = Card.Cards.CS2_065; 			 //[1 Mana] [1/3] Voidwalker ||| [WARLOCK]
        public const Card.Cards MortalCoil = Card.Cards.EX1_302; 			 //[1 Mana] [0/0] Mortal Coil ||| [WARLOCK]
        public const Card.Cards Soulfire = Card.Cards.EX1_308; 			 //[1 Mana] [0/0] Soulfire ||| [WARLOCK]
        public const Card.Cards BloodImp = Card.Cards.CS2_059; 			 //[1 Mana] [0/1] Blood Imp ||| [WARLOCK]
        public const Card.Cards PowerOverwhelming = Card.Cards.EX1_316; 			 //[1 Mana] [0/0] Power Overwhelming ||| [WARLOCK]
        public const Card.Cards FlameImp = Card.Cards.EX1_319; 			 //[1 Mana] [3/2] Flame Imp ||| [WARLOCK]
        public const Card.Cards ReliquarySeeker = Card.Cards.LOE_116; 			 //[1 Mana] [1/1] Reliquary Seeker ||| [WARLOCK]
        public const Card.Cards GoldshireFootman = Card.Cards.CS1_042; 			 //[1 Mana] [1/2] Goldshire Footman ||| [NONE]
        public const Card.Cards MurlocRaider = Card.Cards.CS2_168; 			 //[1 Mana] [2/1] Murloc Raider ||| [NONE]
        public const Card.Cards StonetuskBoar = Card.Cards.CS2_171; 			 //[1 Mana] [1/1] Stonetusk Boar ||| [NONE]
        public const Card.Cards ElvenArcher = Card.Cards.CS2_189; 			 //[1 Mana] [1/1] Elven Archer ||| [NONE]
        public const Card.Cards VoodooDoctor = Card.Cards.EX1_011; 			 //[1 Mana] [2/1] Voodoo Doctor ||| [NONE]
        public const Card.Cards GrimscaleOracle = Card.Cards.EX1_508; 			 //[1 Mana] [1/1] Grimscale Oracle ||| [NONE]
        public const Card.Cards SouthseaDeckhand = Card.Cards.CS2_146; 			 //[1 Mana] [2/1] Southsea Deckhand ||| [NONE]
        public const Card.Cards YoungDragonhawk = Card.Cards.CS2_169; 			 //[1 Mana] [1/1] Young Dragonhawk ||| [NONE]
        public const Card.Cards AbusiveSergeant = Card.Cards.CS2_188; 			 //[1 Mana] [2/1] Abusive Sergeant ||| [NONE]
        public const Card.Cards Lightwarden = Card.Cards.EX1_001; 			 //[1 Mana] [1/2] Lightwarden ||| [NONE]
        public const Card.Cards YoungPriestess = Card.Cards.EX1_004; 			 //[1 Mana] [2/1] Young Priestess ||| [NONE]
        public const Card.Cards ArgentSquire = Card.Cards.EX1_008; 			 //[1 Mana] [1/1] Argent Squire ||| [NONE]
        public const Card.Cards AngryChicken = Card.Cards.EX1_009; 			 //[1 Mana] [1/1] Angry Chicken ||| [NONE]
        public const Card.Cards WorgenInfiltrator = Card.Cards.EX1_010; 			 //[1 Mana] [2/1] Worgen Infiltrator ||| [NONE]
        //public const Card.Cards LeperGnome = Card.Cards.EX1_029; 			 //[1 Mana] [2/1] Leper Gnome ||| [NONE]
        public const Card.Cards Secretkeeper = Card.Cards.EX1_080; 			 //[1 Mana] [1/2] Secretkeeper ||| [NONE]
        public const Card.Cards Shieldbearer = Card.Cards.EX1_405; 			 //[1 Mana] [0/4] Shieldbearer ||| [NONE]
        public const Card.Cards MurlocTidecaller = Card.Cards.EX1_509; 			 //[1 Mana] [1/2] Murloc Tidecaller ||| [NONE]
        public const Card.Cards HungryCrab = Card.Cards.NEW1_017; 			 //[1 Mana] [1/2] Hungry Crab ||| [NONE]
        public const Card.Cards BloodsailCorsair = Card.Cards.NEW1_025; 			 //[1 Mana] [1/2] Bloodsail Corsair ||| [NONE]
        //public const Card.Cards SirFinleyMrrgglton = Card.Cards.LOE_076; 			 //[1 Mana] [1/3] Sir Finley Mrrgglton ||| [NONE]
        //public const Card.Cards ZombieChow = Card.Cards.FP1_001; 			 //[1 Mana] [2/3] Zombie Chow ||| [NONE]
        public const Card.Cards Undertaker = Card.Cards.FP1_028; 			 //[1 Mana] [1/2] Undertaker ||| [NONE]
        public const Card.Cards Cogmaster = Card.Cards.GVG_013; 			 //[1 Mana] [1/2] Cogmaster ||| [NONE]
        //public const Card.Cards ClockworkGnome = Card.Cards.GVG_082; 			 //[1 Mana] [2/1] Clockwork Gnome ||| [NONE]
        public const Card.Cards DragonEgg = Card.Cards.BRM_022; 			 //[1 Mana] [0/2] Dragon Egg ||| [NONE]
        public const Card.Cards LowlySquire = Card.Cards.AT_082; 			 //[1 Mana] [1/2] Lowly Squire ||| [NONE]
        public const Card.Cards TournamentAttendee = Card.Cards.AT_097; 			 //[1 Mana] [2/1] Tournament Attendee ||| [NONE]
        public const Card.Cards InjuredKvaldir = Card.Cards.AT_105; 			 //[1 Mana] [2/4] Injured Kvaldir ||| [NONE]
        public const Card.Cards GadgetzanJouster = Card.Cards.AT_133; 			 //[1 Mana] [1/2] Gadgetzan Jouster ||| [NONE]

        private Card.Cards DarkPeddlerHandler(List<Card.Cards> choices, Board board, BoardState boardCondition)
        {
            bool opWeaponClass = board.IsWeaponClass(board.EnemyClass);
            bool rel = board.MinionFriend.Count == 6;
            int needForLethal = CalculateNeededDamageForLethal(board);
            bool aggro = board.EnemyClass == Card.CClass.PALADIN || board.EnemyClass == Card.CClass.SHAMAN;
            
            Dictionary<Card.Cards, int> oneDropPriorityTable = new Dictionary<Card.Cards, int>
            {

                //TODO: all cards should have a value based on board condition via special method. 
                {Corruption, 50}, //[1 Mana] [0/0]
                {Voidwalker, aggro ? 50 : 30}, //[1 Mana] [1/3]
                {MortalCoil, aggro ? 55 : 35}, //[1 Mana] [0/0]
                {Soulfire, needForLethal <= 4 ? 10000 :  5}, //[1 Mana] [0/0]
                {BloodImp, 0}, //[1 Mana] [0/1]
                {PowerOverwhelming, 75}, //[1 Mana] [0/0]
                {FlameImp, 70}, //[1 Mana] [3/2]
                {ReliquarySeeker, rel ? 90 : 5}, //[1 Mana] [1/1]
                {GoldshireFootman, aggro ? 50: 20}, //[1 Mana] [1/2]
                {MurlocRaider, 0}, //[1 Mana] [2/1]
                {StonetuskBoar, 0}, //[1 Mana] [1/1]
                {ElvenArcher, 15}, //[1 Mana] [1/1]
                {VoodooDoctor, 15}, //[1 Mana] [2/1]
                {GrimscaleOracle, 0}, //[1 Mana] [1/1]
                {SouthseaDeckhand, 0}, //[1 Mana] [2/1]
                {YoungDragonhawk, 0}, //[1 Mana] [1/1]
                {AbusiveSergeant, 30}, //[1 Mana] [2/1]
                {Lightwarden, 0}, //[1 Mana] [1/2]
                {YoungPriestess, 0}, //[1 Mana] [2/1]
                {ArgentSquire, 45}, //[1 Mana] [1/1]
                {AngryChicken, 0}, //[1 Mana] [1/1]
                {WorgenInfiltrator, 20}, //[1 Mana] [2/1]
                {LeperGnome, 50}, //[1 Mana] [2/1]
                {Secretkeeper, 0}, //[1 Mana] [1/2]
                {Shieldbearer, 0}, //[1 Mana] [0/4]
                {MurlocTidecaller, 0}, //[1 Mana] [1/2]
                {HungryCrab, 0}, //[1 Mana] [1/2]
                {BloodsailCorsair, opWeaponClass ? 15 : 0}, //[1 Mana] [1/2]
                {SirFinleyMrrgglton, -1}, //[1 Mana] [1/3]
                {ZombieChow, 30}, //[1 Mana] [2/3]
                {Undertaker, 10}, //[1 Mana] [1/2]
                {Cogmaster, 5}, //[1 Mana] [1/2]
                {ClockworkGnome, 15}, //[1 Mana] [2/1]
                {DragonEgg, 10}, //[1 Mana] [0/2]
                {LowlySquire, 10}, //[1 Mana] [1/2]
                {TournamentAttendee, 0}, //[1 Mana] [2/1]
                {InjuredKvaldir, 0}, //[1 Mana] [2/4]
                {GadgetzanJouster, 40}, //[1 Mana] [1/2]


            };
            List<KeyValuePair<Card.Cards, int>> filteredTable = oneDropPriorityTable.Where(x => choices.Contains(x.Key)).ToList();
            return filteredTable.First(x => x.Value == filteredTable.Max(y => y.Value)).Key;
                    
        }

        private int CalculateNeededDamageForLethal(Board board)
        {
            return board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor
                   - board.MinionFriend.Where(card => card.CanAttack).Sum(q => q.CurrentAtk) //our total damage on board
                   + board.MinionEnemy.Where(card => card.IsTaunt).Sum(q => q.CurrentHealth) //enemy total taun health
                   //TODO: subtract burn in hand
                   ;

        }

        /// <summary>
        /// TODO: calculate if you are missing 4 damage to lethal
        /// </summary>
        /// <returns></returns>
        private bool Lethal()
        {
            return true;
        }

        //TODO: ==================================================ETHEREAL CONJURER=================================================
        //TODO: ===================================================ALL MAGE SPELLS=================================================
        
        public const Card.Cards Polymorph = Card.Cards.CS2_022; 			 //[4 Mana] [0/0] Polymorph ||| [MAGE]
        public const Card.Cards ArcaneIntellect = Card.Cards.CS2_023; 			 //[3 Mana] [0/0] Arcane Intellect ||| [MAGE]
        public const Card.Cards Frostbolt = Card.Cards.CS2_024; 			 //[2 Mana] [0/0] Frostbolt ||| [MAGE]
        public const Card.Cards ArcaneExplosion = Card.Cards.CS2_025; 			 //[2 Mana] [0/0] Arcane Explosion ||| [MAGE]
        public const Card.Cards FrostNova = Card.Cards.CS2_026; 			 //[3 Mana] [0/0] Frost Nova ||| [MAGE]
        public const Card.Cards MirrorImage = Card.Cards.CS2_027; 			 //[1 Mana] [0/0] Mirror Image ||| [MAGE]
        public const Card.Cards Fireball = Card.Cards.CS2_029; 			 //[4 Mana] [0/0] Fireball ||| [MAGE]
        public const Card.Cards Flamestrike = Card.Cards.CS2_032; 			 //[7 Mana] [0/0] Flamestrike ||| [MAGE]
        public const Card.Cards ArcaneMissiles = Card.Cards.EX1_277; 			 //[1 Mana] [0/0] Arcane Missiles ||| [MAGE]
        public const Card.Cards Blizzard = Card.Cards.CS2_028; 			 //[6 Mana] [0/0] Blizzard ||| [MAGE]
        public const Card.Cards IceLance = Card.Cards.CS2_031; 			 //[1 Mana] [0/0] Ice Lance ||| [MAGE]
        public const Card.Cards ConeofCold = Card.Cards.EX1_275; 			 //[4 Mana] [0/0] Cone of Cold ||| [MAGE]
        public const Card.Cards Pyroblast = Card.Cards.EX1_279; 			 //[10 Mana] [0/0] Pyroblast ||| [MAGE]
        public const Card.Cards Counterspell = Card.Cards.EX1_287; 			 //[3 Mana] [0/0] Counterspell ||| [MAGE]
        public const Card.Cards IceBarrier = Card.Cards.EX1_289; 			 //[3 Mana] [0/0] Ice Barrier ||| [MAGE]
        public const Card.Cards MirrorEntity = Card.Cards.EX1_294; 			 //[3 Mana] [0/0] Mirror Entity ||| [MAGE]
        public const Card.Cards IceBlock = Card.Cards.EX1_295; 			 //[3 Mana] [0/0] Ice Block ||| [MAGE]
        public const Card.Cards Vaporize = Card.Cards.EX1_594; 			 //[3 Mana] [0/0] Vaporize ||| [MAGE]
        public const Card.Cards Spellbender = Card.Cards.tt_010; 			 //[3 Mana] [0/0] Spellbender ||| [MAGE]
        public const Card.Cards ForgottenTorch = Card.Cards.LOE_002; 			 //[3 Mana] [0/0] Forgotten Torch ||| [MAGE]
        public const Card.Cards Duplicate = Card.Cards.FP1_018; 			 //[3 Mana] [0/0] Duplicate ||| [MAGE]
        public const Card.Cards Flamecannon = Card.Cards.GVG_001; 			 //[2 Mana] [0/0] Flamecannon ||| [MAGE]
        public const Card.Cards UnstablePortal = Card.Cards.GVG_003; 			 //[2 Mana] [0/0] Unstable Portal ||| [MAGE]
        public const Card.Cards EchoofMedivh = Card.Cards.GVG_005; 			 //[4 Mana] [0/0] Echo of Medivh ||| [MAGE]
        public const Card.Cards DragonsBreath = Card.Cards.BRM_003; 			 //[5 Mana] [0/0] Dragon's Breath ||| [MAGE]
        public const Card.Cards FlameLance = Card.Cards.AT_001; 			 //[5 Mana] [0/0] Flame Lance ||| [MAGE]
        public const Card.Cards Effigy = Card.Cards.AT_002; 			 //[3 Mana] [0/0] Effigy ||| [MAGE]
        public const Card.Cards ArcaneBlast = Card.Cards.AT_004; 			 //[1 Mana] [0/0] Arcane Blast ||| [MAGE]
        public const Card.Cards PolymorphBoar = Card.Cards.AT_005; 			 //[3 Mana] [0/0] Polymorph: Boar ||| [MAGE]
        /// <summary>
        /// Assumes you are mage when this event triggers
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="board"></param>
        /// <param name="boardCondition"></param>
        /// <returns></returns>
        private Card.Cards EtherealConjurerHandler(List<Card.Cards> choices, Board board, BoardState boardCondition)
        {
            Dictionary<Card.Cards, int> burnCardsDamageTable = new Dictionary<Card.Cards, int>
            {
                {Pyroblast, 10},
                {Frostbolt, 3},
                {Fireball, 6},
                {ForgottenTorch, 3},
                {IceLance, board.HeroEnemy.IsFrozen ? 4 : 0}
            };

            List<Card.Cards> burnSpells = new List<Card.Cards>{Frostbolt, Fireball, ForgottenTorch, IceLance};
            List<Card.Cards> boardWipe = new List<Card.Cards>{Blizzard, Flamestrike};
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
        //public const Card.Cards FierceMonkey = Card.Cards.LOE_022; 			 //[3 Mana] [3/4] Fierce Monkey ||| [WARRIOR]
        public const Card.Cards StarvingBuzzard = Card.Cards.CS2_237; //[5 Mana] [3/2] Starving Buzzard ||| [HUNTER]
        public const Card.Cards TimberWolf = Card.Cards.DS1_175; //[1 Mana] [1/1] Timber Wolf ||| [HUNTER]
        public const Card.Cards TundraRhino = Card.Cards.DS1_178; //[5 Mana] [2/5] Tundra Rhino ||| [HUNTER]
        public const Card.Cards ScavengingHyena = Card.Cards.EX1_531; //[2 Mana] [2/2] Scavenging Hyena ||| [HUNTER]
        public const Card.Cards SavannahHighmane = Card.Cards.EX1_534; //[6 Mana] [6/5] Savannah Highmane ||| [HUNTER]
        public const Card.Cards KingKrush = Card.Cards.EX1_543; //[9 Mana] [8/8] King Krush ||| [HUNTER]
        //public const Card.Cards DesertCamel = Card.Cards.LOE_020; 			 //[3 Mana] [2/4] Desert Camel ||| [HUNTER]
        public const Card.Cards Webspinner = Card.Cards.FP1_011; //[1 Mana] [1/1] Webspinner ||| [HUNTER]
        public const Card.Cards KingofBeasts = Card.Cards.GVG_046; //[5 Mana] [2/6] King of Beasts ||| [HUNTER]
        public const Card.Cards Gahzrilla = Card.Cards.GVG_049; //[7 Mana] [6/9] Gahz'rilla ||| [HUNTER]
        public const Card.Cards CoreRager = Card.Cards.BRM_014; //[4 Mana] [4/4] Core Rager ||| [HUNTER]
        public const Card.Cards KingsElekk = Card.Cards.AT_058; //[2 Mana] [3/2] King's Elekk ||| [HUNTER]
        public const Card.Cards Acidmaw = Card.Cards.AT_063; //[7 Mana] [4/2] Acidmaw ||| [HUNTER]
        //public const Card.Cards Dreadscale = Card.Cards.AT_063t; 			 //[3 Mana] [4/2] Dreadscale ||| [HUNTER]
        public const Card.Cards PitSnake = Card.Cards.LOE_010; //[1 Mana] [2/1] Pit Snake ||| [ROGUE]
        //public const Card.Cards MountedRaptor = Card.Cards.LOE_050; 			 //[3 Mana] [3/2] Mounted Raptor ||| [DRUID]
        public const Card.Cards JungleMoonkin = Card.Cards.LOE_051; //[4 Mana] [4/4] Jungle Moonkin ||| [DRUID]
        public const Card.Cards Malorne = Card.Cards.GVG_035; //[7 Mana] [9/7] Malorne ||| [DRUID]
        public const Card.Cards SavageCombatant = Card.Cards.AT_039; //[4 Mana] [5/4] Savage Combatant ||| [DRUID]
        public const Card.Cards OasisSnapjaw = Card.Cards.CS2_119; //[4 Mana] [2/7] Oasis Snapjaw ||| [NONE]
        public const Card.Cards RiverCrocolisk = Card.Cards.CS2_120; //[2 Mana] [2/3] River Crocolisk ||| [NONE]
        //public const Card.Cards IronfurGrizzly = Card.Cards.CS2_125; 			 //[3 Mana] [3/3] Ironfur Grizzly ||| [NONE]
        //public const Card.Cards SilverbackPatriarch = Card.Cards.CS2_127; 			 //[3 Mana] [1/4] Silverback Patriarch ||| [NONE]
        // public const Card.Cards StonetuskBoar = Card.Cards.CS2_171; 			 //[1 Mana] [1/1] Stonetusk Boar ||| [NONE]
        public const Card.Cards BloodfenRaptor = Card.Cards.CS2_172; //[2 Mana] [3/2] Bloodfen Raptor ||| [NONE]
        public const Card.Cards CoreHound = Card.Cards.CS2_201; //[7 Mana] [9/5] Core Hound ||| [NONE]
        //public const Card.Cards YoungDragonhawk = Card.Cards.CS2_169; 			 //[1 Mana] [1/1] Young Dragonhawk ||| [NONE]
        public const Card.Cards IronbeakOwl = Card.Cards.CS2_203; //[2 Mana] [2/1] Ironbeak Owl ||| [NONE]
        //public const Card.Cards AngryChicken = Card.Cards.EX1_009; 			 //[1 Mana] [1/1] Angry Chicken ||| [NONE]
        //public const Card.Cards KingMukla = Card.Cards.EX1_014; 			 //[3 Mana] [5/5] King Mukla ||| [NONE]
        //public const Card.Cards JunglePanther = Card.Cards.EX1_017; 			 //[3 Mana] [4/2] Jungle Panther ||| [NONE]
        public const Card.Cards StranglethornTiger = Card.Cards.EX1_028; //[5 Mana] [5/5] Stranglethorn Tiger ||| [NONE]
        public const Card.Cards DireWolfAlpha = Card.Cards.EX1_162; //[2 Mana] [2/2] Dire Wolf Alpha ||| [NONE]
        //public const Card.Cards EmperorCobra = Card.Cards.EX1_170; 			 //[3 Mana] [2/3] Emperor Cobra ||| [NONE]
        //public const Card.Cards TheBeast = Card.Cards.EX1_577; 			 //[6 Mana] [9/7] The Beast ||| [NONE]
        //public const Card.Cards HungryCrab = Card.Cards.NEW1_017; 			 //[1 Mana] [1/2] Hungry Crab ||| [NONE]
        public const Card.Cards StampedingKodo = Card.Cards.NEW1_041; //[5 Mana] [3/5] Stampeding Kodo ||| [NONE]
        //public const Card.Cards JeweledScarab = Card.Cards.LOE_029; 			 //[2 Mana] [1/1] Jeweled Scarab ||| [NONE]
        //public const Card.Cards HugeToad = Card.Cards.LOE_046; 			 //[2 Mana] [3/2] Huge Toad ||| [NONE]
        //public const Card.Cards TombSpider = Card.Cards.LOE_047; 			 //[4 Mana] [3/3] Tomb Spider ||| [NONE]
        public const Card.Cards CaptainsParrot = Card.Cards.NEW1_016; //[2 Mana] [1/1] Captain's Parrot ||| [NONE]
        //public const Card.Cards HauntedCreeper = Card.Cards.FP1_002; 			 //[2 Mana] [1/2] Haunted Creeper ||| [NONE]
        public const Card.Cards Maexxna = Card.Cards.FP1_010; //[6 Mana] [2/8] Maexxna ||| [NONE]
        public const Card.Cards LostTallstrider = Card.Cards.GVG_071; //[4 Mana] [5/4] Lost Tallstrider ||| [NONE]
        public const Card.Cards MuklasChampion = Card.Cards.AT_090; //[5 Mana] [4/3] Mukla's Champion ||| [NONE]
        public const Card.Cards CapturedJormungar = Card.Cards.AT_102; //[7 Mana] [5/9] Captured Jormungar ||| [NONE]
        public const Card.Cards ArmoredWarhorse = Card.Cards.AT_108; //[4 Mana] [5/3] Armored Warhorse ||| [NONE]

        private Card.Cards TombSpiderHandler(List<Card.Cards> choices, Board board, BoardState boardCondition)
        {
            return choices[0];
        }

        //TODO: ==================================================JEWELED SCARAB=================================================
        public const Card.Cards Hex = Card.Cards.EX1_246; //[3 Mana] [0/0] Hex ||| [SHAMAN]
        public const Card.Cards FarSight = Card.Cards.CS2_053; //[3 Mana] [0/0] Far Sight ||| [SHAMAN]
        public const Card.Cards LavaBurst = Card.Cards.EX1_241; //[3 Mana] [0/0] Lava Burst ||| [SHAMAN]
        public const Card.Cards FeralSpirit = Card.Cards.EX1_248; //[3 Mana] [0/0] Feral Spirit ||| [SHAMAN]
        public const Card.Cards UnboundElemental = Card.Cards.EX1_258; //[3 Mana] [2/4] Unbound Elemental ||| [SHAMAN]
        public const Card.Cards LightningStorm = Card.Cards.EX1_259; //[3 Mana] [0/0] Lightning Storm ||| [SHAMAN]
        public const Card.Cards ManaTideTotem = Card.Cards.EX1_575; //[3 Mana] [0/3] Mana Tide Totem ||| [SHAMAN]
        public const Card.Cards Powermace = Card.Cards.GVG_036; //[3 Mana] [3/2] Powermace ||| [SHAMAN]
        public const Card.Cards TuskarrTotemic = Card.Cards.AT_046; //[3 Mana] [3/2] Tuskarr Totemic ||| [SHAMAN]
        public const Card.Cards HealingWave = Card.Cards.AT_048; //[3 Mana] [0/0] Healing Wave ||| [SHAMAN]
        public const Card.Cards ElementalDestruction = Card.Cards.AT_051; //[3 Mana] [0/0] Elemental Destruction ||| [SHAMAN]
        public const Card.Cards ShadowWordDeath = Card.Cards.EX1_622; //[3 Mana] [0/0] Shadow Word: Death ||| [PRIEST]
        public const Card.Cards Thoughtsteal = Card.Cards.EX1_339; //[3 Mana] [0/0] Thoughtsteal ||| [PRIEST]
        public const Card.Cards Shadowform = Card.Cards.EX1_625; //[3 Mana] [0/0] Shadowform ||| [PRIEST]
        //public const Card.Cards DarkCultist = Card.Cards.FP1_023; 			 //[3 Mana] [3/4] Dark Cultist ||| [PRIEST]
        public const Card.Cards VelensChosen = Card.Cards.GVG_010; //[3 Mana] [0/0] Velen's Chosen ||| [PRIEST]
        public const Card.Cards Shadowfiend = Card.Cards.AT_014; //[3 Mana] [3/3] Shadowfiend ||| [PRIEST]
        //public const Card.Cards ArcaneIntellect = Card.Cards.CS2_023; 			 //[3 Mana] [0/0] Arcane Intellect ||| [MAGE]
        //public const Card.Cards FrostNova = Card.Cards.CS2_026; 			 //[3 Mana] [0/0] Frost Nova ||| [MAGE]
        //public const Card.Cards Counterspell = Card.Cards.EX1_287; 			 //[3 Mana] [0/0] Counterspell ||| [MAGE]
        //public const Card.Cards IceBarrier = Card.Cards.EX1_289; 			 //[3 Mana] [0/0] Ice Barrier ||| [MAGE]
        //public const Card.Cards MirrorEntity = Card.Cards.EX1_294; 			 //[3 Mana] [0/0] Mirror Entity ||| [MAGE]
        //public const Card.Cards IceBlock = Card.Cards.EX1_295; 			 //[3 Mana] [0/0] Ice Block ||| [MAGE]
        //public const Card.Cards Vaporize = Card.Cards.EX1_594; 			 //[3 Mana] [0/0] Vaporize ||| [MAGE]
        //public const Card.Cards KirinTorMage = Card.Cards.EX1_612; 			 //[3 Mana] [4/3] Kirin Tor Mage ||| [MAGE]
        //public const Card.Cards Spellbender = Card.Cards.tt_010; 			 //[3 Mana] [0/0] Spellbender ||| [MAGE]
        //public const Card.Cards ForgottenTorch = Card.Cards.LOE_002; 			 //[3 Mana] [0/0] Forgotten Torch ||| [MAGE]
        //public const Card.Cards Duplicate = Card.Cards.FP1_018; 			 //[3 Mana] [0/0] Duplicate ||| [MAGE]
        public const Card.Cards SootSpewer = Card.Cards.GVG_123; //[3 Mana] [3/3] Soot Spewer ||| [MAGE]
        public const Card.Cards Flamewaker = Card.Cards.BRM_002; //[3 Mana] [2/4] Flamewaker ||| [MAGE]
        //public const Card.Cards Effigy = Card.Cards.AT_002; 			 //[3 Mana] [0/0] Effigy ||| [MAGE]
        //public const Card.Cards PolymorphBoar = Card.Cards.AT_005; 			 //[3 Mana] [0/0] Polymorph: Boar ||| [MAGE]
        public const Card.Cards Spellslinger = Card.Cards.AT_007; //[3 Mana] [3/4] Spellslinger ||| [MAGE]
        public const Card.Cards DivineFavor = Card.Cards.EX1_349; //[3 Mana] [0/0] Divine Favor ||| [PALADIN]
        public const Card.Cards SwordofJustice = Card.Cards.EX1_366; //[3 Mana] [1/5] Sword of Justice ||| [PALADIN]
        public const Card.Cards AldorPeacekeeper = Card.Cards.EX1_382; //[3 Mana] [3/3] Aldor Peacekeeper ||| [PALADIN]
        public const Card.Cards Coghammer = Card.Cards.GVG_059; //[3 Mana] [2/3] Coghammer ||| [PALADIN]
        public const Card.Cards MusterforBattle = Card.Cards.GVG_061; //[3 Mana] [0/0] Muster for Battle ||| [PALADIN]
        public const Card.Cards ScarletPurifier = Card.Cards.GVG_101; //[3 Mana] [4/3] Scarlet Purifier ||| [PALADIN]
        public const Card.Cards SealofChampions = Card.Cards.AT_074; //[3 Mana] [0/0] Seal of Champions ||| [PALADIN]
        public const Card.Cards WarhorseTrainer = Card.Cards.AT_075; //[3 Mana] [2/4] Warhorse Trainer ||| [PALADIN]
        public const Card.Cards Charge = Card.Cards.CS2_103; //[3 Mana] [0/0] Charge ||| [WARRIOR]
        public const Card.Cards WarsongCommander = Card.Cards.EX1_084; //[3 Mana] [2/3] Warsong Commander ||| [WARRIOR]
        public const Card.Cards ShieldBlock = Card.Cards.EX1_606; //[3 Mana] [0/0] Shield Block ||| [WARRIOR]
        public const Card.Cards FrothingBerserker = Card.Cards.EX1_604; //[3 Mana] [2/4] Frothing Berserker ||| [WARRIOR]
        public const Card.Cards FierceMonkey = Card.Cards.LOE_022; //[3 Mana] [3/4] Fierce Monkey ||| [WARRIOR]
        public const Card.Cards BouncingBlade = Card.Cards.GVG_050; //[3 Mana] [0/0] Bouncing Blade ||| [WARRIOR]
        public const Card.Cards OgreWarmaul = Card.Cards.GVG_054; //[3 Mana] [4/2] Ogre Warmaul ||| [WARRIOR]
        public const Card.Cards Bash = Card.Cards.AT_064; //[3 Mana] [0/0] Bash ||| [WARRIOR]
        public const Card.Cards KingsDefender = Card.Cards.AT_065; //[3 Mana] [3/2] King's Defender ||| [WARRIOR]
        public const Card.Cards OrgrimmarAspirant = Card.Cards.AT_066; //[3 Mana] [3/3] Orgrimmar Aspirant ||| [WARRIOR]
        public const Card.Cards ShadowBolt = Card.Cards.CS2_057; //[3 Mana] [0/0] Shadow Bolt ||| [WARLOCK]
        public const Card.Cards DrainLife = Card.Cards.CS2_061; //[3 Mana] [0/0] Drain Life ||| [WARLOCK]
        public const Card.Cards Felguard = Card.Cards.EX1_301; //[3 Mana] [3/5] Felguard ||| [WARLOCK]
        public const Card.Cards VoidTerror = Card.Cards.EX1_304; //[3 Mana] [3/3] Void Terror ||| [WARLOCK]
        public const Card.Cards SenseDemons = Card.Cards.EX1_317; //[3 Mana] [0/0] Sense Demons ||| [WARLOCK]
        public const Card.Cards Demonwrath = Card.Cards.BRM_005; //[3 Mana] [0/0] Demonwrath ||| [WARLOCK]
        public const Card.Cards ImpGangBoss = Card.Cards.BRM_006; //[3 Mana] [2/4] Imp Gang Boss ||| [WARLOCK]
        public const Card.Cards KillCommand = Card.Cards.EX1_539; //[3 Mana] [0/0] Kill Command ||| [HUNTER]
        public const Card.Cards AnimalCompanion = Card.Cards.NEW1_031; //[3 Mana] [0/0] Animal Companion ||| [HUNTER]
        public const Card.Cards EaglehornBow = Card.Cards.EX1_536; //[3 Mana] [3/2] Eaglehorn Bow ||| [HUNTER]
        public const Card.Cards UnleashtheHounds = Card.Cards.EX1_538; //[3 Mana] [0/0] Unleash the Hounds ||| [HUNTER]
        public const Card.Cards DeadlyShot = Card.Cards.EX1_617; //[3 Mana] [0/0] Deadly Shot ||| [HUNTER]
        public const Card.Cards DesertCamel = Card.Cards.LOE_020; //[3 Mana] [2/4] Desert Camel ||| [HUNTER]
        public const Card.Cards MetaltoothLeaper = Card.Cards.GVG_048; //[3 Mana] [3/3] Metaltooth Leaper ||| [HUNTER]
        public const Card.Cards Powershot = Card.Cards.AT_056; //[3 Mana] [0/0] Powershot ||| [HUNTER]
        public const Card.Cards Stablemaster = Card.Cards.AT_057; //[3 Mana] [4/2] Stablemaster ||| [HUNTER]
        public const Card.Cards Dreadscale = Card.Cards.AT_063t; //[3 Mana] [4/2] Dreadscale ||| [HUNTER]
        public const Card.Cards FanofKnives = Card.Cards.EX1_129; //[3 Mana] [0/0] Fan of Knives ||| [ROGUE]
        public const Card.Cards PerditionsBlade = Card.Cards.EX1_133; //[3 Mana] [2/2] Perdition's Blade ||| [ROGUE]
        public const Card.Cards SI7Agent = Card.Cards.EX1_134; //[3 Mana] [3/3] SI:7 Agent ||| [ROGUE]
        public const Card.Cards Headcrack = Card.Cards.EX1_137; //[3 Mana] [0/0] Headcrack ||| [ROGUE]
        public const Card.Cards EdwinVanCleef = Card.Cards.EX1_613; //[3 Mana] [2/2] Edwin VanCleef ||| [ROGUE]
        public const Card.Cards UnearthedRaptor = Card.Cards.LOE_019; //[3 Mana] [3/4] Unearthed Raptor ||| [ROGUE]
        public const Card.Cards CogmastersWrench = Card.Cards.GVG_024; //[3 Mana] [1/3] Cogmaster's Wrench ||| [ROGUE]
        public const Card.Cards IronSensei = Card.Cards.GVG_027; //[3 Mana] [2/2] Iron Sensei ||| [ROGUE]
        public const Card.Cards ShadyDealer = Card.Cards.AT_032; //[3 Mana] [4/3] Shady Dealer ||| [ROGUE]
        public const Card.Cards Burgle = Card.Cards.AT_033; //[3 Mana] [0/0] Burgle ||| [ROGUE]
        public const Card.Cards BeneaththeGrounds = Card.Cards.AT_035; //[3 Mana] [0/0] Beneath the Grounds ||| [ROGUE]
        public const Card.Cards HealingTouch = Card.Cards.CS2_007; //[3 Mana] [0/0] Healing Touch ||| [DRUID]
        public const Card.Cards SavageRoar = Card.Cards.CS2_011; //[3 Mana] [0/0] Savage Roar ||| [DRUID]
        public const Card.Cards MarkofNature = Card.Cards.EX1_155; //[3 Mana] [0/0] Mark of Nature ||| [DRUID]
        public const Card.Cards MountedRaptor = Card.Cards.LOE_050; //[3 Mana] [3/2] Mounted Raptor ||| [DRUID]
        public const Card.Cards GroveTender = Card.Cards.GVG_032; //[3 Mana] [2/4] Grove Tender ||| [DRUID]
        public const Card.Cards DruidoftheFlame = Card.Cards.BRM_010; //[3 Mana] [2/2] Druid of the Flame ||| [DRUID]
        public const Card.Cards Mulch = Card.Cards.AT_044; //[3 Mana] [0/0] Mulch ||| [DRUID]
        public const Card.Cards MagmaRager = Card.Cards.CS2_118; //[3 Mana] [5/1] Magma Rager ||| [NONE]
        public const Card.Cards RaidLeader = Card.Cards.CS2_122; //[3 Mana] [2/2] Raid Leader ||| [NONE]
        public const Card.Cards Wolfrider = Card.Cards.CS2_124; //[3 Mana] [3/1] Wolfrider ||| [NONE]
        public const Card.Cards IronfurGrizzly = Card.Cards.CS2_125; //[3 Mana] [3/3] Ironfur Grizzly ||| [NONE]
        public const Card.Cards SilverbackPatriarch = Card.Cards.CS2_127; //[3 Mana] [1/4] Silverback Patriarch ||| [NONE]
        public const Card.Cards IronforgeRifleman = Card.Cards.CS2_141; //[3 Mana] [2/2] Ironforge Rifleman ||| [NONE]
        public const Card.Cards RazorfenHunter = Card.Cards.CS2_196; //[3 Mana] [2/3] Razorfen Hunter ||| [NONE]
        public const Card.Cards ShatteredSunCleric = Card.Cards.EX1_019; //[3 Mana] [3/2] Shattered Sun Cleric ||| [NONE]
        public const Card.Cards DalaranMage = Card.Cards.EX1_582; //[3 Mana] [1/4] Dalaran Mage ||| [NONE]
        public const Card.Cards EarthenRingFarseer = Card.Cards.CS2_117; //[3 Mana] [3/3] Earthen Ring Farseer ||| [NONE]
        public const Card.Cards InjuredBlademaster = Card.Cards.CS2_181; //[3 Mana] [4/7] Injured Blademaster ||| [NONE]
        public const Card.Cards BigGameHunter = Card.Cards.EX1_005; //[3 Mana] [4/2] Big Game Hunter ||| [NONE]
        public const Card.Cards AlarmoBot = Card.Cards.EX1_006; //[3 Mana] [0/3] Alarm-o-Bot ||| [NONE]
        public const Card.Cards AcolyteofPain = Card.Cards.EX1_007; //[3 Mana] [1/3] Acolyte of Pain ||| [NONE]
        public const Card.Cards KingMukla = Card.Cards.EX1_014; //[3 Mana] [5/5] King Mukla ||| [NONE]
        public const Card.Cards JunglePanther = Card.Cards.EX1_017; //[3 Mana] [4/2] Jungle Panther ||| [NONE]
        public const Card.Cards ScarletCrusader = Card.Cards.EX1_020; //[3 Mana] [3/1] Scarlet Crusader ||| [NONE]
        public const Card.Cards ThrallmarFarseer = Card.Cards.EX1_021; //[3 Mana] [2/3] Thrallmar Farseer ||| [NONE]
        public const Card.Cards QuestingAdventurer = Card.Cards.EX1_044; //[3 Mana] [2/2] Questing Adventurer ||| [NONE]
        public const Card.Cards ColdlightOracle = Card.Cards.EX1_050; //[3 Mana] [2/2] Coldlight Oracle ||| [NONE]
        public const Card.Cards TinkmasterOverspark = Card.Cards.EX1_083; //[3 Mana] [3/3] Tinkmaster Overspark ||| [NONE]
        public const Card.Cards MindControlTech = Card.Cards.EX1_085; //[3 Mana] [3/3] Mind Control Tech ||| [NONE]
        public const Card.Cards ArcaneGolem = Card.Cards.EX1_089; //[3 Mana] [4/2] Arcane Golem ||| [NONE]
        public const Card.Cards Demolisher = Card.Cards.EX1_102; //[3 Mana] [1/4] Demolisher ||| [NONE]
        public const Card.Cards ColdlightSeer = Card.Cards.EX1_103; //[3 Mana] [2/3] Coldlight Seer ||| [NONE]
        public const Card.Cards EmperorCobra = Card.Cards.EX1_170; //[3 Mana] [2/3] Emperor Cobra ||| [NONE]
        public const Card.Cards TaurenWarrior = Card.Cards.EX1_390; //[3 Mana] [2/3] Tauren Warrior ||| [NONE]
        public const Card.Cards RagingWorgen = Card.Cards.EX1_412; //[3 Mana] [3/3] Raging Worgen ||| [NONE]
        public const Card.Cards MurlocWarleader = Card.Cards.EX1_507; //[3 Mana] [3/3] Murloc Warleader ||| [NONE]
        //public const Card.Cards HarvestGolem = Card.Cards.EX1_556; 			 //[3 Mana] [2/3] Harvest Golem ||| [NONE]
        public const Card.Cards BloodKnight = Card.Cards.EX1_590; //[3 Mana] [3/3] Blood Knight ||| [NONE]
        public const Card.Cards ImpMaster = Card.Cards.EX1_597; //[3 Mana] [1/5] Imp Master ||| [NONE]
        public const Card.Cards SouthseaCaptain = Card.Cards.NEW1_027; //[3 Mana] [3/3] Southsea Captain ||| [NONE]
        public const Card.Cards FlesheatingGhoul = Card.Cards.tt_004; //[3 Mana] [2/3] Flesheating Ghoul ||| [NONE]
        public const Card.Cards BrannBronzebeard = Card.Cards.LOE_077; //[3 Mana] [2/4] Brann Bronzebeard ||| [NONE]
        public const Card.Cards ShadeofNaxxramas = Card.Cards.FP1_005; //[3 Mana] [2/2] Shade of Naxxramas ||| [NONE]
        //public const Card.Cards Deathlord = Card.Cards.FP1_009; 			 //[3 Mana] [2/8] Deathlord ||| [NONE]
        public const Card.Cards StoneskinGargoyle = Card.Cards.FP1_027; //[3 Mana] [1/4] Stoneskin Gargoyle ||| [NONE]
        //public const Card.Cards DancingSwords = Card.Cards.FP1_029; 			 //[3 Mana] [4/4] Dancing Swords ||| [NONE]
        public const Card.Cards SpiderTank = Card.Cards.GVG_044; //[3 Mana] [3/4] Spider Tank ||| [NONE]
        public const Card.Cards OgreBrute = Card.Cards.GVG_065; //[3 Mana] [4/4] Ogre Brute ||| [NONE]
        public const Card.Cards FlyingMachine = Card.Cards.GVG_084; //[3 Mana] [1/4] Flying Machine ||| [NONE]
        public const Card.Cards Illuminator = Card.Cards.GVG_089; //[3 Mana] [2/4] Illuminator ||| [NONE]
        public const Card.Cards GnomishExperimenter = Card.Cards.GVG_092; //[3 Mana] [3/2] Gnomish Experimenter ||| [NONE]
        public const Card.Cards GoblinSapper = Card.Cards.GVG_095; //[3 Mana] [2/4] Goblin Sapper ||| [NONE]
        public const Card.Cards LilExorcist = Card.Cards.GVG_097; //[3 Mana] [2/3] Lil' Exorcist ||| [NONE]
        public const Card.Cards GnomereganInfantry = Card.Cards.GVG_098; //[3 Mana] [1/4] Gnomeregan Infantry ||| [NONE]
        public const Card.Cards TinkertownTechnician = Card.Cards.GVG_102; //[3 Mana] [3/3] Tinkertown Technician ||| [NONE]
        public const Card.Cards Hobgoblin = Card.Cards.GVG_104; //[3 Mana] [2/3] Hobgoblin ||| [NONE]
        public const Card.Cards BlackwingTechnician = Card.Cards.BRM_033; //[3 Mana] [2/4] Blackwing Technician ||| [NONE]
        public const Card.Cards DragonhawkRider = Card.Cards.AT_083; //[3 Mana] [3/3] Dragonhawk Rider ||| [NONE]
        public const Card.Cards Saboteur = Card.Cards.AT_086; //[3 Mana] [4/3] Saboteur ||| [NONE]
        public const Card.Cards ArgentHorserider = Card.Cards.AT_087; //[3 Mana] [2/1] Argent Horserider ||| [NONE]
        public const Card.Cards IceRager = Card.Cards.AT_092; //[3 Mana] [5/2] Ice Rager ||| [NONE]
        public const Card.Cards SilentKnight = Card.Cards.AT_095; //[3 Mana] [2/2] Silent Knight ||| [NONE]
        public const Card.Cards SilverHandRegent = Card.Cards.AT_100; //[3 Mana] [3/3] Silver Hand Regent ||| [NONE]
        public const Card.Cards LightsChampion = Card.Cards.AT_106; //[3 Mana] [4/3] Light's Champion ||| [NONE]
        public const Card.Cards ColiseumManager = Card.Cards.AT_110; //[3 Mana] [2/5] Coliseum Manager ||| [NONE]
        public const Card.Cards FencingCoach = Card.Cards.AT_115; //[3 Mana] [2/2] Fencing Coach ||| [NONE]
        public const Card.Cards MasterofCeremonies = Card.Cards.AT_117; //[3 Mana] [4/2] Master of Ceremonies ||| [NONE]
        public const Card.Cards FjolaLightbane = Card.Cards.AT_129; //[3 Mana] [3/4] Fjola Lightbane ||| [NONE]
        public const Card.Cards EydisDarkbane = Card.Cards.AT_131; //[3 Mana] [3/4] Eydis Darkbane ||| [NONE]

        private Card.Cards JeweledScarabHandler(List<Card.Cards> choices, Board board, BoardState boardCondition)
        {
            return choices[0];
        }

        public const Card.Cards WhirlingZapomatic = Card.Cards.GVG_037; //[2 Mana] [3/2] Whirling Zap-o-matic ||| [SHAMAN]
        public const Card.Cards Shadowboxer = Card.Cards.GVG_072; //[2 Mana] [2/3] Shadowboxer ||| [PRIEST]
        public const Card.Cards UpgradedRepairBot = Card.Cards.GVG_083; //[5 Mana] [5/5] Upgraded Repair Bot ||| [PRIEST]
        public const Card.Cards Snowchugger = Card.Cards.GVG_002; //[2 Mana] [2/3] Snowchugger ||| [MAGE]
        public const Card.Cards FlameLeviathan = Card.Cards.GVG_007; //[7 Mana] [7/7] Flame Leviathan ||| [MAGE]
        //public const Card.Cards SootSpewer = Card.Cards.GVG_123; 			 //[3 Mana] [3/3] Soot Spewer ||| [MAGE]
        public const Card.Cards ShieldedMinibot = Card.Cards.GVG_058; //[2 Mana] [2/2] Shielded Minibot ||| [PALADIN]
        public const Card.Cards CobaltGuardian = Card.Cards.GVG_062; //[5 Mana] [6/3] Cobalt Guardian ||| [PALADIN]
        public const Card.Cards Warbot = Card.Cards.GVG_051; //[1 Mana] [1/3] Warbot ||| [WARRIOR]
        public const Card.Cards ScrewjankClunker = Card.Cards.GVG_055; //[4 Mana] [2/5] Screwjank Clunker ||| [WARRIOR]
        public const Card.Cards IronJuggernaut = Card.Cards.GVG_056; //[6 Mana] [6/5] Iron Juggernaut ||| [WARRIOR]
        public const Card.Cards SiegeEngine = Card.Cards.GVG_086; //[5 Mana] [5/5] Siege Engine ||| [WARRIOR]
        public const Card.Cards FelCannon = Card.Cards.GVG_020; //[4 Mana] [3/5] Fel Cannon ||| [WARLOCK]
        public const Card.Cards AnimaGolem = Card.Cards.GVG_077; //[6 Mana] [9/9] Anima Golem ||| [WARLOCK]
        //public const Card.Cards MetaltoothLeaper = Card.Cards.GVG_048; 			 //[3 Mana] [3/3] Metaltooth Leaper ||| [HUNTER]
        public const Card.Cards GoblinAutoBarber = Card.Cards.GVG_023; //[2 Mana] [3/2] Goblin Auto-Barber ||| [ROGUE]
        //public const Card.Cards IronSensei = Card.Cards.GVG_027; 			 //[3 Mana] [2/2] Iron Sensei ||| [ROGUE]
        public const Card.Cards AnodizedRoboCub = Card.Cards.GVG_030; //[2 Mana] [2/2] Anodized Robo Cub ||| [DRUID]
        public const Card.Cards MechBearCat = Card.Cards.GVG_034; //[6 Mana] [7/6] Mech-Bear-Cat ||| [DRUID]
        //public const Card.Cards AlarmoBot = Card.Cards.EX1_006; 			 //[3 Mana] [0/3] Alarm-o-Bot ||| [NONE]
        //public const Card.Cards Demolisher = Card.Cards.EX1_102; 			 //[3 Mana] [1/4] Demolisher ||| [NONE]
        //public const Card.Cards HarvestGolem = Card.Cards.EX1_556; 			 //[3 Mana] [2/3] Harvest Golem ||| [NONE]
        //public const Card.Cards GorillabotA3 = Card.Cards.LOE_039; 			 //[4 Mana] [3/4] Gorillabot A-3 ||| [NONE]
        public const Card.Cards Mechwarper = Card.Cards.GVG_006; //[2 Mana] [2/3] Mechwarper ||| [NONE]
        public const Card.Cards FelReaver = Card.Cards.GVG_016; //[5 Mana] [8/8] Fel Reaver ||| [NONE]
        //public const Card.Cards SpiderTank = Card.Cards.GVG_044; 			 //[3 Mana] [3/4] Spider Tank ||| [NONE]
        public const Card.Cards AntiqueHealbot = Card.Cards.GVG_069; //[5 Mana] [3/3] Antique Healbot ||| [NONE]
        //public const Card.Cards ExplosiveSheep = Card.Cards.GVG_076; 			 //[2 Mana] [1/1] Explosive Sheep ||| [NONE]
        //public const Card.Cards MechanicalYeti = Card.Cards.GVG_078; 			 //[4 Mana] [4/5] Mechanical Yeti ||| [NONE]
        public const Card.Cards ForceTankMAX = Card.Cards.GVG_079; //[8 Mana] [7/7] Force-Tank MAX ||| [NONE]
        //public const Card.Cards ClockworkGnome = Card.Cards.GVG_082; 			 //[1 Mana] [2/1] Clockwork Gnome ||| [NONE]
        //public const Card.Cards FlyingMachine = Card.Cards.GVG_084; 			 //[3 Mana] [1/4] Flying Machine ||| [NONE]
        public const Card.Cards AnnoyoTron = Card.Cards.GVG_085; //[2 Mana] [1/2] Annoy-o-Tron ||| [NONE]
        public const Card.Cards ArcaneNullifierX21 = Card.Cards.GVG_091; //[4 Mana] [2/5] Arcane Nullifier X-21 ||| [NONE]
        public const Card.Cards TargetDummy = Card.Cards.GVG_093; //[0 Mana] [0/2] Target Dummy ||| [NONE]
        public const Card.Cards Jeeves = Card.Cards.GVG_094; //[4 Mana] [1/4] Jeeves ||| [NONE]
        //public const Card.Cards PilotedShredder = Card.Cards.GVG_096; 			 //[4 Mana] [4/3] Piloted Shredder ||| [NONE]
        public const Card.Cards MicroMachine = Card.Cards.GVG_103; //[2 Mana] [1/2] Micro Machine ||| [NONE]
        //public const Card.Cards PilotedSkyGolem = Card.Cards.GVG_105; 			 //[6 Mana] [6/4] Piloted Sky Golem ||| [NONE]
        public const Card.Cards Junkbot = Card.Cards.GVG_106; //[5 Mana] [1/5] Junkbot ||| [NONE]
        public const Card.Cards EnhanceoMechano = Card.Cards.GVG_107; //[4 Mana] [3/2] Enhance-o Mechano ||| [NONE]
        public const Card.Cards MimironsHead = Card.Cards.GVG_111; //[5 Mana] [4/5] Mimiron's Head ||| [NONE]
        public const Card.Cards FoeReaper4000 = Card.Cards.GVG_113; //[8 Mana] [6/9] Foe Reaper 4000 ||| [NONE]
        //public const Card.Cards SneedsOldShredder = Card.Cards.GVG_114; 			 //[8 Mana] [5/7] Sneed's Old Shredder ||| [NONE]
        public const Card.Cards MekgineerThermaplugg = Card.Cards.GVG_116; //[9 Mana] [9/7] Mekgineer Thermaplugg ||| [NONE]
        public const Card.Cards Blingtron3000 = Card.Cards.GVG_119; //[5 Mana] [3/4] Blingtron 3000 ||| [NONE]
        public const Card.Cards ClockworkGiant = Card.Cards.GVG_121; //[12 Mana] [8/8] Clockwork Giant ||| [NONE]
        public const Card.Cards ClockworkKnight = Card.Cards.AT_096; //[5 Mana] [5/5] Clockwork Knight ||| [NONE]

        private Card.Cards GorillabotA3Handler(List<Card.Cards> choices, Board board, BoardState boardCondition)
        {
            return choices[0];
        }

        //======================================================MUSEUM CURATOR=======================================================
        public const Card.Cards DarkCultist = Card.Cards.FP1_023; //[3 Mana] [3/4] Dark Cultist ||| [PRIEST]
        public const Card.Cards BloodmageThalnos = Card.Cards.EX1_012; //[2 Mana] [1/1] Bloodmage Thalnos ||| [NONE]
        public const Card.Cards SylvanasWindrunner = Card.Cards.EX1_016; //[6 Mana] [5/5] Sylvanas Windrunner ||| [NONE]
        public const Card.Cards LeperGnome = Card.Cards.EX1_029; //[1 Mana] [2/1] Leper Gnome ||| [NONE]
        public const Card.Cards LootHoarder = Card.Cards.EX1_096; //[2 Mana] [2/1] Loot Hoarder ||| [NONE]
        public const Card.Cards Abomination = Card.Cards.EX1_097; //[5 Mana] [4/4] Abomination ||| [NONE]
        public const Card.Cards CairneBloodhoof = Card.Cards.EX1_110; //[6 Mana] [4/5] Cairne Bloodhoof ||| [NONE]
        public const Card.Cards HarvestGolem = Card.Cards.EX1_556; //[3 Mana] [2/3] Harvest Golem ||| [NONE]
        public const Card.Cards TheBeast = Card.Cards.EX1_577; //[6 Mana] [9/7] The Beast ||| [NONE]
        public const Card.Cards HugeToad = Card.Cards.LOE_046; //[2 Mana] [3/2] Huge Toad ||| [NONE]
        public const Card.Cards AnubisathSentinel = Card.Cards.LOE_061; //[5 Mana] [4/4] Anubisath Sentinel ||| [NONE]
        public const Card.Cards WobblingRunts = Card.Cards.LOE_089; //[6 Mana] [2/6] Wobbling Runts ||| [NONE]
        public const Card.Cards ZombieChow = Card.Cards.FP1_001; //[1 Mana] [2/3] Zombie Chow ||| [NONE]
        public const Card.Cards HauntedCreeper = Card.Cards.FP1_002; //[2 Mana] [1/2] Haunted Creeper ||| [NONE]
        public const Card.Cards MadScientist = Card.Cards.FP1_004; //[2 Mana] [2/2] Mad Scientist ||| [NONE]
        public const Card.Cards NerubianEgg = Card.Cards.FP1_007; //[2 Mana] [0/2] Nerubian Egg ||| [NONE]
        public const Card.Cards Deathlord = Card.Cards.FP1_009; //[3 Mana] [2/8] Deathlord ||| [NONE]
        public const Card.Cards SludgeBelcher = Card.Cards.FP1_012; //[5 Mana] [3/5] Sludge Belcher ||| [NONE]
        public const Card.Cards Stalagg = Card.Cards.FP1_014; //[5 Mana] [7/4] Stalagg ||| [NONE]
        public const Card.Cards Feugen = Card.Cards.FP1_015; //[5 Mana] [4/7] Feugen ||| [NONE]
        public const Card.Cards UnstableGhoul = Card.Cards.FP1_024; //[2 Mana] [1/3] Unstable Ghoul ||| [NONE]
        public const Card.Cards DancingSwords = Card.Cards.FP1_029; //[3 Mana] [4/4] Dancing Swords ||| [NONE]
        public const Card.Cards ExplosiveSheep = Card.Cards.GVG_076; //[2 Mana] [1/1] Explosive Sheep ||| [NONE]
        public const Card.Cards MechanicalYeti = Card.Cards.GVG_078; //[4 Mana] [4/5] Mechanical Yeti ||| [NONE]
        public const Card.Cards ClockworkGnome = Card.Cards.GVG_082; //[1 Mana] [2/1] Clockwork Gnome ||| [NONE]
        public const Card.Cards PilotedShredder = Card.Cards.GVG_096; //[4 Mana] [4/3] Piloted Shredder ||| [NONE]
        public const Card.Cards PilotedSkyGolem = Card.Cards.GVG_105; //[6 Mana] [6/4] Piloted Sky Golem ||| [NONE]
        public const Card.Cards SneedsOldShredder = Card.Cards.GVG_114; //[8 Mana] [5/7] Sneed's Old Shredder ||| [NONE]
        public const Card.Cards Toshley = Card.Cards.GVG_115; //[6 Mana] [5/7] Toshley ||| [NONE]
        public const Card.Cards MajordomoExecutus = Card.Cards.BRM_027; //[9 Mana] [9/7] Majordomo Executus ||| [NONE]
        public const Card.Cards Chillmaw = Card.Cards.AT_123; //[7 Mana] [6/6] Chillmaw ||| [NONE]
        public const Card.Cards TheSkeletonKnight = Card.Cards.AT_128; //[6 Mana] [7/4] The Skeleton Knight ||| [NONE] 

        private Card.Cards MuseumCuratorHandler(List<Card.Cards> choices, Board board, BoardState boardCondition)
        {
            return choices[0];
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