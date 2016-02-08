using SmartBot.Plugins.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using SmartBot.Database;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;


namespace SmartBot.Plugins
{
    public static class Extension
    {
        public static void AddOrUpdate<TKey, TValue>(
            this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }
    }
   
    [Serializable]
    public class smPluginDataContainer : PluginDataContainer
    {
        [ItemsSource(typeof(DeckType)) ] //XCeed reference
        public DeckType TestOpponentDeck {get;set;} 
        
        public int AverageDeckCostHigherThan { get; set; }
        public bool AutoUpdateV3 { get; set; }
        public bool AutoUpdateTracker { get; set; }
        [ItemsSource(typeof(DeckType))] //XCeed reference
        public DeckType TestYourDeck { get; set; }

        public string LSmartMulliganV3 { get; private set; }
        public string LSmartTracker { get; private set; }

        public smPluginDataContainer()
        {
            Name = "SmartTracker";
            TestOpponentDeck = DeckType.Unknown;
            TestYourDeck = DeckType.Unknown;
            AutoUpdateV3 = false;
            AutoUpdateTracker = false;
            LSmartMulliganV3 = "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV2/MulliganProfiles/SmartMulliganV3.cs";
            LSmartTracker = "";

        }
    }

    public class SMTracker : Plugin
    {
         #region cards

        private const string GoldshireFootman = "CS1_042";
        private const string HolyNova = "CS1_112";
        private const string MindControl = "CS1_113";
        private const string HolySmite = "CS1_130";
        private const string MindVision = "CS2_003";
        private const string PowerWordShield = "CS2_004";
        private const string Claw = "CS2_005";
        private const string HealingTouch = "CS2_007";
        private const string Moonfire = "CS2_008";
        private const string MarkoftheWild = "CS2_009";
        private const string SavageRoar = "CS2_011";
        private const string Swipe = "CS2_012";
        private const string WildGrowth = "CS2_013";
        private const string Polymorph = "CS2_022";
        private const string ArcaneIntellect = "CS2_023";
        private const string Frostbolt = "CS2_024";
        private const string ArcaneExplosion = "CS2_025";
        private const string FrostNova = "CS2_026";
        private const string MirrorImage = "CS2_027";
        private const string Fireball = "CS2_029";
        private const string Flamestrike = "CS2_032";
        private const string WaterElemental = "CS2_033";
        private const string FrostShock = "CS2_037";
        private const string Windfury = "CS2_039";
        private const string AncestralHealing = "CS2_041";
        private const string FireElemental = "CS2_042";
        private const string RockbiterWeapon = "CS2_045";
        private const string Bloodlust = "CS2_046";
        private const string ShadowBolt = "CS2_057";
        private const string DrainLife = "CS2_061";
        private const string Hellfire = "CS2_062";
        private const string Corruption = "CS2_063";
        private const string DreadInfernal = "CS2_064";
        private const string Voidwalker = "CS2_065";
        private const string Backstab = "CS2_072";
        private const string DeadlyPoison = "CS2_074";
        private const string SinisterStrike = "CS2_075";
        private const string Assassinate = "CS2_076";
        private const string Sprint = "CS2_077";
        private const string AssassinsBlade = "CS2_080";
        private const string HuntersMark = "CS2_084";
        private const string BlessingofMight = "CS2_087";
        private const string GuardianofKings = "CS2_088";
        private const string HolyLight = "CS2_089";
        private const string LightsJustice = "CS2_091";
        private const string BlessingofKings = "CS2_092";
        private const string Consecration = "CS2_093";
        private const string HammerofWrath = "CS2_094";
        private const string TruesilverChampion = "CS2_097";
        private const string Charge = "CS2_103";
        private const string HeroicStrike = "CS2_105";
        private const string FieryWarAxe = "CS2_106";
        private const string Execute = "CS2_108";
        private const string ArcaniteReaper = "CS2_112";
        private const string Cleave = "CS2_114";
        private const string MagmaRager = "CS2_118";
        private const string OasisSnapjaw = "CS2_119";
        private const string RiverCrocolisk = "CS2_120";
        private const string FrostwolfGrunt = "CS2_121";
        private const string RaidLeader = "CS2_122";
        private const string Wolfrider = "CS2_124";
        private const string IronfurGrizzly = "CS2_125";
        private const string SilverbackPatriarch = "CS2_127";
        private const string StormwindKnight = "CS2_131";
        private const string IronforgeRifleman = "CS2_141";
        private const string KoboldGeomancer = "CS2_142";
        private const string GnomishInventor = "CS2_147";
        private const string StormpikeCommando = "CS2_150";
        private const string Archmage = "CS2_155";
        private const string LordoftheArena = "CS2_162";
        private const string MurlocRaider = "CS2_168";
        private const string StonetuskBoar = "CS2_171";
        private const string BloodfenRaptor = "CS2_172";
        private const string BluegillWarrior = "CS2_173";
        private const string SenjinShieldmasta = "CS2_179";
        private const string ChillwindYeti = "CS2_182";
        private const string WarGolem = "CS2_186";
        private const string BootyBayBodyguard = "CS2_187";
        private const string ElvenArcher = "CS2_189";
        private const string RazorfenHunter = "CS2_196";
        private const string OgreMagi = "CS2_197";
        private const string BoulderfistOgre = "CS2_200";
        private const string CoreHound = "CS2_201";
        private const string RecklessRocketeer = "CS2_213";
        private const string StormwindChampion = "CS2_222";
        private const string FrostwolfWarlord = "CS2_226";
        private const string IronbarkProtector = "CS2_232";
        private const string ShadowWordPain = "CS2_234";
        private const string NorthshireCleric = "CS2_235";
        private const string DivineSpirit = "CS2_236";
        private const string StarvingBuzzard = "CS2_237";
        private const string DarkscaleHealer = "DS1_055";
        private const string Houndmaster = "DS1_070";
        private const string TimberWolf = "DS1_175";
        private const string TundraRhino = "DS1_178";
        private const string MultiShot = "DS1_183";
        private const string Tracking = "DS1_184";
        private const string ArcaneShot = "DS1_185";
        private const string MindBlast = "DS1_233";
        private const string VoodooDoctor = "EX1_011";
        private const string NoviceEngineer = "EX1_015";
        private const string ShatteredSunCleric = "EX1_019";
        private const string DragonlingMechanic = "EX1_025";
        private const string AcidicSwampOoze = "EX1_066";
        private const string WarsongCommander = "EX1_084";
        private const string FanofKnives = "EX1_129";
        private const string Innervate = "EX1_169";
        private const string Starfire = "EX1_173";
        private const string TotemicMight = "EX1_244";
        private const string Hex = "EX1_246";
        private const string ArcaneMissiles = "EX1_277";
        private const string Shiv = "EX1_278";
        private const string MortalCoil = "EX1_302";
        private const string Succubus = "EX1_306";
        private const string Soulfire = "EX1_308";
        private const string Humility = "EX1_360";
        private const string HandofProtection = "EX1_371";
        private const string GurubashiBerserker = "EX1_399";
        private const string Whirlwind = "EX1_400";
        private const string MurlocTidehunter = "EX1_506";
        private const string GrimscaleOracle = "EX1_508";
        private const string KillCommand = "EX1_539";
        private const string FlametongueTotem = "EX1_565";
        private const string Sap = "EX1_581";
        private const string DalaranMage = "EX1_582";
        private const string Windspeaker = "EX1_587";
        private const string Nightblade = "EX1_593";
        private const string ShieldBlock = "EX1_606";
        private const string ShadowWordDeath = "EX1_622";
        private const string SacrificialPact = "NEW1_003";
        private const string Vanish = "NEW1_004";
        private const string KorkronElite = "NEW1_011";
        private const string AnimalCompanion = "NEW1_031";
        private const string FenCreeper = "CS1_069";
        private const string InnerFire = "CS1_129";
        private const string Blizzard = "CS2_028";
        private const string IceLance = "CS2_031";
        private const string AncestralSpirit = "CS2_038";
        private const string FarSight = "CS2_053";
        private const string BloodImp = "CS2_059";
        private const string ColdBlood = "CS2_073";
        private const string Rampage = "CS2_104";
        private const string EarthenRingFarseer = "CS2_117";
        private const string SouthseaDeckhand = "CS2_146";
        private const string SilverHandKnight = "CS2_151";
        private const string RavenholdtAssassin = "CS2_161";
        private const string YoungDragonhawk = "CS2_169";
        private const string InjuredBlademaster = "CS2_181";
        private const string AbusiveSergeant = "CS2_188";
        private const string IronbeakOwl = "CS2_203";
        private const string SpitefulSmith = "CS2_221";
        private const string VentureCoMercenary = "CS2_227";
        private const string Wisp = "CS2_231";
        private const string BladeFlurry = "CS2_233";
        private const string GladiatorsLongbow = "DS1_188";
        private const string Lightwarden = "EX1_001";
        private const string TheBlackKnight = "EX1_002";
        private const string YoungPriestess = "EX1_004";
        private const string BigGameHunter = "EX1_005";
        private const string AlarmoBot = "EX1_006";
        private const string AcolyteofPain = "EX1_007";
        private const string ArgentSquire = "EX1_008";
        private const string AngryChicken = "EX1_009";
        private const string WorgenInfiltrator = "EX1_010";
        private const string BloodmageThalnos = "EX1_012";
        private const string KingMukla = "EX1_014";
        private const string SylvanasWindrunner = "EX1_016";
        private const string JunglePanther = "EX1_017";
        private const string ScarletCrusader = "EX1_020";
        private const string ThrallmarFarseer = "EX1_021";
        private const string SilvermoonGuardian = "EX1_023";
        private const string StranglethornTiger = "EX1_028";
        private const string LeperGnome = "EX1_029";
        private const string Sunwalker = "EX1_032";
        private const string WindfuryHarpy = "EX1_033";
        private const string TwilightDrake = "EX1_043";
        private const string QuestingAdventurer = "EX1_044";
        private const string AncientWatcher = "EX1_045";
        private const string DarkIronDwarf = "EX1_046";
        private const string Spellbreaker = "EX1_048";
        private const string YouthfulBrewmaster = "EX1_049";
        private const string ColdlightOracle = "EX1_050";
        private const string ManaAddict = "EX1_055";
        private const string AncientBrewmaster = "EX1_057";
        private const string SunfuryProtector = "EX1_058";
        private const string CrazedAlchemist = "EX1_059";
        private const string ArgentCommander = "EX1_067";
        private const string PintSizedSummoner = "EX1_076";
        private const string Secretkeeper = "EX1_080";
        private const string MadBomber = "EX1_082";
        private const string TinkmasterOverspark = "EX1_083";
        private const string MindControlTech = "EX1_085";
        private const string ArcaneGolem = "EX1_089";
        private const string CabalShadowPriest = "EX1_091";
        private const string DefenderofArgus = "EX1_093";
        private const string GadgetzanAuctioneer = "EX1_095";
        private const string LootHoarder = "EX1_096";
        private const string Abomination = "EX1_097";
        private const string LorewalkerCho = "EX1_100";
        private const string Demolisher = "EX1_102";
        private const string ColdlightSeer = "EX1_103";
        private const string MountainGiant = "EX1_105";
        private const string CairneBloodhoof = "EX1_110";
        private const string LeeroyJenkins = "EX1_116";
        private const string Eviscerate = "EX1_124";
        private const string Betrayal = "EX1_126";
        private const string Conceal = "EX1_128";
        private const string NobleSacrifice = "EX1_130";
        private const string DefiasRingleader = "EX1_131";
        private const string EyeforanEye = "EX1_132";
        private const string PerditionsBlade = "EX1_133";
        private const string SI7Agent = "EX1_134";
        private const string Redemption = "EX1_136";
        private const string Headcrack = "EX1_137";
        private const string Shadowstep = "EX1_144";
        private const string Preparation = "EX1_145";
        private const string Wrath = "EX1_154";
        private const string MarkofNature = "EX1_155";
        private const string SouloftheForest = "EX1_158";
        private const string PoweroftheWild = "EX1_160";
        private const string Naturalize = "EX1_161";
        private const string DireWolfAlpha = "EX1_162";
        private const string Nourish = "EX1_164";
        private const string DruidoftheClaw = "EX1_165";
        private const string KeeperoftheGrove = "EX1_166";
        private const string EmperorCobra = "EX1_170";
        private const string AncientofWar = "EX1_178";
        private const string LightningBolt = "EX1_238";
        private const string LavaBurst = "EX1_241";
        private const string DustDevil = "EX1_243";
        private const string EarthShock = "EX1_245";
        private const string StormforgedAxe = "EX1_247";
        private const string FeralSpirit = "EX1_248";
        private const string BaronGeddon = "EX1_249";
        private const string EarthElemental = "EX1_250";
        private const string ForkedLightning = "EX1_251";
        private const string UnboundElemental = "EX1_258";
        private const string LightningStorm = "EX1_259";
        private const string EtherealArcanist = "EX1_274";
        private const string ConeofCold = "EX1_275";
        private const string Pyroblast = "EX1_279";
        private const string FrostElemental = "EX1_283";
        private const string AzureDrake = "EX1_284";
        private const string Counterspell = "EX1_287";
        private const string IceBarrier = "EX1_289";
        private const string MirrorEntity = "EX1_294";
        private const string IceBlock = "EX1_295";
        private const string RagnarostheFirelord = "EX1_298";
        private const string Felguard = "EX1_301";
        private const string Shadowflame = "EX1_303";
        private const string VoidTerror = "EX1_304";
        private const string SiphonSoul = "EX1_309";
        private const string Doomguard = "EX1_310";
        private const string TwistingNether = "EX1_312";
        private const string PitLord = "EX1_313";
        private const string SummoningPortal = "EX1_315";
        private const string PowerOverwhelming = "EX1_316";
        private const string SenseDemons = "EX1_317";
        private const string FlameImp = "EX1_319";
        private const string BaneofDoom = "EX1_320";
        private const string LordJaraxxus = "EX1_323";
        private const string Silence = "EX1_332";
        private const string ShadowMadness = "EX1_334";
        private const string Lightspawn = "EX1_335";
        private const string Thoughtsteal = "EX1_339";
        private const string Lightwell = "EX1_341";
        private const string Mindgames = "EX1_345";
        private const string DivineFavor = "EX1_349";
        private const string ProphetVelen = "EX1_350";
        private const string LayonHands = "EX1_354";
        private const string BlessedChampion = "EX1_355";
        private const string ArgentProtector = "EX1_362";
        private const string BlessingofWisdom = "EX1_363";
        private const string HolyWrath = "EX1_365";
        private const string SwordofJustice = "EX1_366";
        private const string Repentance = "EX1_379";
        private const string AldorPeacekeeper = "EX1_382";
        private const string TirionFordring = "EX1_383";
        private const string AvengingWrath = "EX1_384";
        private const string TaurenWarrior = "EX1_390";
        private const string Slam = "EX1_391";
        private const string BattleRage = "EX1_392";
        private const string AmaniBerserker = "EX1_393";
        private const string MogushanWarden = "EX1_396";
        private const string ArathiWeaponsmith = "EX1_398";
        private const string Armorsmith = "EX1_402";
        private const string Shieldbearer = "EX1_405";
        private const string Brawl = "EX1_407";
        private const string MortalStrike = "EX1_408";
        private const string Upgrade = "EX1_409";
        private const string ShieldSlam = "EX1_410";
        private const string Gorehowl = "EX1_411";
        private const string RagingWorgen = "EX1_412";
        private const string GrommashHellscream = "EX1_414";
        private const string MurlocWarleader = "EX1_507";
        private const string MurlocTidecaller = "EX1_509";
        private const string PatientAssassin = "EX1_522";
        private const string ScavengingHyena = "EX1_531";
        private const string Misdirection = "EX1_533";
        private const string SavannahHighmane = "EX1_534";
        private const string EaglehornBow = "EX1_536";
        private const string ExplosiveShot = "EX1_537";
        private const string UnleashtheHounds = "EX1_538";
        private const string KingKrush = "EX1_543";
        private const string Flare = "EX1_544";
        private const string BestialWrath = "EX1_549";
        private const string SnakeTrap = "EX1_554";
        private const string HarvestGolem = "EX1_556";
        private const string NatPagle = "EX1_557";
        private const string HarrisonJones = "EX1_558";
        private const string ArchmageAntonidas = "EX1_559";
        private const string Nozdormu = "EX1_560";
        private const string Alexstrasza = "EX1_561";
        private const string Onyxia = "EX1_562";
        private const string Malygos = "EX1_563";
        private const string FacelessManipulator = "EX1_564";
        private const string Doomhammer = "EX1_567";
        private const string Bite = "EX1_570";
        private const string ForceofNature = "EX1_571";
        private const string Ysera = "EX1_572";
        private const string Cenarius = "EX1_573";
        private const string ManaTideTotem = "EX1_575";
        private const string TheBeast = "EX1_577";
        private const string Savagery = "EX1_578";
        private const string PriestessofElune = "EX1_583";
        private const string AncientMage = "EX1_584";
        private const string SeaGiant = "EX1_586";
        private const string BloodKnight = "EX1_590";
        private const string AuchenaiSoulpriest = "EX1_591";
        private const string Vaporize = "EX1_594";
        private const string CultMaster = "EX1_595";
        private const string Demonfire = "EX1_596";
        private const string ImpMaster = "EX1_597";
        private const string CruelTaskmaster = "EX1_603";
        private const string FrothingBerserker = "EX1_604";
        private const string InnerRage = "EX1_607";
        private const string SorcerersApprentice = "EX1_608";
        private const string Snipe = "EX1_609";
        private const string ExplosiveTrap = "EX1_610";
        private const string FreezingTrap = "EX1_611";
        private const string KirinTorMage = "EX1_612";
        private const string EdwinVanCleef = "EX1_613";
        private const string IllidanStormrage = "EX1_614";
        private const string ManaWraith = "EX1_616";
        private const string DeadlyShot = "EX1_617";
        private const string Equality = "EX1_619";
        private const string MoltenGiant = "EX1_620";
        private const string CircleofHealing = "EX1_621";
        private const string TempleEnforcer = "EX1_623";
        private const string HolyFire = "EX1_624";
        private const string Shadowform = "EX1_625";
        private const string MassDispel = "EX1_626";
        private const string Kidnapper = "NEW1_005";
        private const string Starfall = "NEW1_007";
        private const string AncientofLore = "NEW1_008";
        private const string AlAkirtheWindlord = "NEW1_010";
        private const string ManaWyrm = "NEW1_012";
        private const string MasterofDisguise = "NEW1_014";
        private const string HungryCrab = "NEW1_017";
        private const string BloodsailRaider = "NEW1_018";
        private const string KnifeJuggler = "NEW1_019";
        private const string WildPyromancer = "NEW1_020";
        private const string Doomsayer = "NEW1_021";
        private const string DreadCorsair = "NEW1_022";
        private const string FaerieDragon = "NEW1_023";
        private const string CaptainGreenskin = "NEW1_024";
        private const string BloodsailCorsair = "NEW1_025";
        private const string VioletTeacher = "NEW1_026";
        private const string SouthseaCaptain = "NEW1_027";
        private const string MillhouseManastorm = "NEW1_029";
        private const string Deathwing = "NEW1_030";
        private const string CommandingShout = "NEW1_036";
        private const string MasterSwordsmith = "NEW1_037";
        private const string Gruul = "NEW1_038";
        private const string Hogger = "NEW1_040";
        private const string StampedingKodo = "NEW1_041";
        private const string FlesheatingGhoul = "tt_004";
        private const string Spellbender = "tt_010";
        private const string MurlocTinyfin = "LOEA10_3";
        private const string ForgottenTorch = "LOE_002";
        private const string EtherealConjurer = "LOE_003";
        private const string MuseumCurator = "LOE_006";
        private const string CurseofRafaam = "LOE_007";
        private const string ObsidianDestroyer = "LOE_009";
        private const string PitSnake = "LOE_010";
        private const string RenoJackson = "LOE_011";
        private const string TombPillager = "LOE_012";
        private const string RumblingElemental = "LOE_016";
        private const string KeeperofUldaman = "LOE_017";
        private const string TunnelTrogg = "LOE_018";
        private const string UnearthedRaptor = "LOE_019";
        private const string DesertCamel = "LOE_020";
        private const string DartTrap = "LOE_021";
        private const string FierceMonkey = "LOE_022";
        private const string DarkPeddler = "LOE_023";
        private const string AnyfinCanHappen = "LOE_026";
        private const string SacredTrial = "LOE_027";
        private const string JeweledScarab = "LOE_029";
        private const string NagaSeaWitch = "LOE_038";
        private const string GorillabotA3 = "LOE_039";
        private const string HugeToad = "LOE_046";
        private const string TombSpider = "LOE_047";
        private const string MountedRaptor = "LOE_050";
        private const string JungleMoonkin = "LOE_051";
        private const string DjinniofZephyrs = "LOE_053";
        private const string AnubisathSentinel = "LOE_061";
        private const string FossilizedDevilsaur = "LOE_073";
        private const string SirFinleyMrrgglton = "LOE_076";
        private const string BrannBronzebeard = "LOE_077";
        private const string EliseStarseeker = "LOE_079";
        private const string SummoningStone = "LOE_086";
        private const string WobblingRunts = "LOE_089";
        private const string ArchThiefRafaam = "LOE_092";
        private const string Entomb = "LOE_104";
        private const string ExplorersHat = "LOE_105";
        private const string EerieStatue = "LOE_107";
        private const string AncientShade = "LOE_110";
        private const string ExcavatedEvil = "LOE_111";
        private const string EveryfinisAwesome = "LOE_113";
        private const string RavenIdol = "LOE_115";
        private const string ReliquarySeeker = "LOE_116";
        private const string CursedBlade = "LOE_118";
        private const string OldMurkEye = "EX1_062";
        private const string CaptainsParrot = "NEW1_016";
        private const string GelbinMekkatorque = "EX1_112";
        private const string EliteTaurenChieftain = "PRO_001";
        private const string ZombieChow = "FP1_001";
        private const string HauntedCreeper = "FP1_002";
        private const string EchoingOoze = "FP1_003";
        private const string MadScientist = "FP1_004";
        private const string ShadeofNaxxramas = "FP1_005";
        private const string NerubianEgg = "FP1_007";
        private const string SpectralKnight = "FP1_008";
        private const string Deathlord = "FP1_009";
        private const string Maexxna = "FP1_010";
        private const string Webspinner = "FP1_011";
        private const string SludgeBelcher = "FP1_012";
        private const string KelThuzad = "FP1_013";
        private const string Stalagg = "FP1_014";
        private const string Feugen = "FP1_015";
        private const string WailingSoul = "FP1_016";
        private const string NerubarWeblord = "FP1_017";
        private const string Duplicate = "FP1_018";
        private const string PoisonSeeds = "FP1_019";
        private const string Avenge = "FP1_020";
        private const string DeathsBite = "FP1_021";
        private const string Voidcaller = "FP1_022";
        private const string DarkCultist = "FP1_023";
        private const string UnstableGhoul = "FP1_024";
        private const string Reincarnate = "FP1_025";
        private const string AnubarAmbusher = "FP1_026";
        private const string StoneskinGargoyle = "FP1_027";
        private const string Undertaker = "FP1_028";
        private const string DancingSwords = "FP1_029";
        private const string Loatheb = "FP1_030";
        private const string BaronRivendare = "FP1_031";
        private const string Flamecannon = "GVG_001";
        private const string Snowchugger = "GVG_002";
        private const string UnstablePortal = "GVG_003";
        private const string GoblinBlastmage = "GVG_004";
        private const string EchoofMedivh = "GVG_005";
        private const string Mechwarper = "GVG_006";
        private const string FlameLeviathan = "GVG_007";
        private const string Lightbomb = "GVG_008";
        private const string Shadowbomber = "GVG_009";
        private const string VelensChosen = "GVG_010";
        private const string Shrinkmeister = "GVG_011";
        private const string LightoftheNaaru = "GVG_012";
        private const string Cogmaster = "GVG_013";
        private const string Voljin = "GVG_014";
        private const string Darkbomb = "GVG_015";
        private const string FelReaver = "GVG_016";
        private const string CallPet = "GVG_017";
        private const string MistressofPain = "GVG_018";
        private const string Demonheart = "GVG_019";
        private const string FelCannon = "GVG_020";
        private const string MalGanis = "GVG_021";
        private const string TinkersSharpswordOil = "GVG_022";
        private const string GoblinAutoBarber = "GVG_023";
        private const string CogmastersWrench = "GVG_024";
        private const string OneeyedCheat = "GVG_025";
        private const string FeignDeath = "GVG_026";
        private const string IronSensei = "GVG_027";
        private const string TradePrinceGallywix = "GVG_028";
        private const string AncestorsCall = "GVG_029";
        private const string AnodizedRoboCub = "GVG_030";
        private const string Recycle = "GVG_031";
        private const string GroveTender = "GVG_032";
        private const string TreeofLife = "GVG_033";
        private const string MechBearCat = "GVG_034";
        private const string Malorne = "GVG_035";
        private const string Powermace = "GVG_036";
        private const string WhirlingZapomatic = "GVG_037";
        private const string Crackle = "GVG_038";
        private const string VitalityTotem = "GVG_039";
        private const string SiltfinSpiritwalker = "GVG_040";
        private const string DarkWispers = "GVG_041";
        private const string Neptulon = "GVG_042";
        private const string Glaivezooka = "GVG_043";
        private const string SpiderTank = "GVG_044";
        private const string Implosion = "GVG_045";
        private const string KingofBeasts = "GVG_046";
        private const string Sabotage = "GVG_047";
        private const string MetaltoothLeaper = "GVG_048";
        private const string Gahzrilla = "GVG_049";
        private const string BouncingBlade = "GVG_050";
        private const string Warbot = "GVG_051";
        private const string Crush = "GVG_052";
        private const string Shieldmaiden = "GVG_053";
        private const string OgreWarmaul = "GVG_054";
        private const string ScrewjankClunker = "GVG_055";
        private const string IronJuggernaut = "GVG_056";
        private const string SealofLight = "GVG_057";
        private const string ShieldedMinibot = "GVG_058";
        private const string Coghammer = "GVG_059";
        private const string Quartermaster = "GVG_060";
        private const string MusterforBattle = "GVG_061";
        private const string CobaltGuardian = "GVG_062";
        private const string BolvarFordragon = "GVG_063";
        private const string Puddlestomper = "GVG_064";
        private const string OgreBrute = "GVG_065";
        private const string DunemaulShaman = "GVG_066";
        private const string StonesplinterTrogg = "GVG_067";
        private const string BurlyRockjawTrogg = "GVG_068";
        private const string AntiqueHealbot = "GVG_069";
        private const string SaltyDog = "GVG_070";
        private const string LostTallstrider = "GVG_071";
        private const string Shadowboxer = "GVG_072";
        private const string CobraShot = "GVG_073";
        private const string KezanMystic = "GVG_074";
        private const string ShipsCannon = "GVG_075";
        private const string ExplosiveSheep = "GVG_076";
        private const string AnimaGolem = "GVG_077";
        private const string MechanicalYeti = "GVG_078";
        private const string ForceTankMAX = "GVG_079";
        private const string DruidoftheFang = "GVG_080";
        private const string GilblinStalker = "GVG_081";
        private const string ClockworkGnome = "GVG_082";
        private const string UpgradedRepairBot = "GVG_083";
        private const string FlyingMachine = "GVG_084";
        private const string AnnoyoTron = "GVG_085";
        private const string SiegeEngine = "GVG_086";
        private const string SteamwheedleSniper = "GVG_087";
        private const string OgreNinja = "GVG_088";
        private const string Illuminator = "GVG_089";
        private const string MadderBomber = "GVG_090";
        private const string ArcaneNullifierX21 = "GVG_091";
        private const string GnomishExperimenter = "GVG_092";
        private const string TargetDummy = "GVG_093";
        private const string Jeeves = "GVG_094";
        private const string GoblinSapper = "GVG_095";
        private const string PilotedShredder = "GVG_096";
        private const string LilExorcist = "GVG_097";
        private const string GnomereganInfantry = "GVG_098";
        private const string BombLobber = "GVG_099";
        private const string FloatingWatcher = "GVG_100";
        private const string ScarletPurifier = "GVG_101";
        private const string TinkertownTechnician = "GVG_102";
        private const string MicroMachine = "GVG_103";
        private const string Hobgoblin = "GVG_104";
        private const string PilotedSkyGolem = "GVG_105";
        private const string Junkbot = "GVG_106";
        private const string EnhanceoMechano = "GVG_107";
        private const string Recombobulator = "GVG_108";
        private const string MiniMage = "GVG_109";
        private const string DrBoom = "GVG_110";
        private const string MimironsHead = "GVG_111";
        private const string MogortheOgre = "GVG_112";
        private const string FoeReaper4000 = "GVG_113";
        private const string SneedsOldShredder = "GVG_114";
        private const string Toshley = "GVG_115";
        private const string MekgineerThermaplugg = "GVG_116";
        private const string Gazlowe = "GVG_117";
        private const string TroggzortheEarthinator = "GVG_118";
        private const string Blingtron3000 = "GVG_119";
        private const string HemetNesingwary = "GVG_120";
        private const string ClockworkGiant = "GVG_121";
        private const string WeeSpellstopper = "GVG_122";
        private const string SootSpewer = "GVG_123";
        private const string SolemnVigil = "BRM_001";
        private const string Flamewaker = "BRM_002";
        private const string DragonsBreath = "BRM_003";
        private const string TwilightWhelp = "BRM_004";
        private const string Demonwrath = "BRM_005";
        private const string ImpGangBoss = "BRM_006";
        private const string GangUp = "BRM_007";
        private const string DarkIronSkulker = "BRM_008";
        private const string VolcanicLumberer = "BRM_009";
        private const string DruidoftheFlame = "BRM_010";
        private const string LavaShock = "BRM_011";
        private const string FireguardDestroyer = "BRM_012";
        private const string QuickShot = "BRM_013";
        private const string CoreRager = "BRM_014";
        private const string Revenge = "BRM_015";
        private const string AxeFlinger = "BRM_016";
        private const string Resurrect = "BRM_017";
        private const string DragonConsort = "BRM_018";
        private const string GrimPatron = "BRM_019";
        private const string DragonkinSorcerer = "BRM_020";
        private const string DragonEgg = "BRM_022";
        private const string DrakonidCrusher = "BRM_024";
        private const string VolcanicDrake = "BRM_025";
        private const string HungryDragon = "BRM_026";
        private const string MajordomoExecutus = "BRM_027";
        private const string EmperorThaurissan = "BRM_028";
        private const string RendBlackhand = "BRM_029";
        private const string Nefarian = "BRM_030";
        private const string Chromaggus = "BRM_031";
        private const string BlackwingTechnician = "BRM_033";
        private const string BlackwingCorruptor = "BRM_034";
        private const string FlameLance = "AT_001";
        private const string Effigy = "AT_002";
        private const string FallenHero = "AT_003";
        private const string ArcaneBlast = "AT_004";
        private const string PolymorphBoar = "AT_005";
        private const string DalaranAspirant = "AT_006";
        private const string Spellslinger = "AT_007";
        private const string ColdarraDrake = "AT_008";
        private const string Rhonin = "AT_009";
        private const string RamWrangler = "AT_010";
        private const string HolyChampion = "AT_011";
        private const string SpawnofShadows = "AT_012";
        private const string PowerWordGlory = "AT_013";
        private const string Shadowfiend = "AT_014";
        private const string Convert = "AT_015";
        private const string Confuse = "AT_016";
        private const string TwilightGuardian = "AT_017";
        private const string ConfessorPaletress = "AT_018";
        private const string Dreadsteed = "AT_019";
        private const string FearsomeDoomguard = "AT_020";
        private const string TinyKnightofEvil = "AT_021";
        private const string FistofJaraxxus = "AT_022";
        private const string VoidCrusher = "AT_023";
        private const string Demonfuse = "AT_024";
        private const string DarkBargain = "AT_025";
        private const string Wrathguard = "AT_026";
        private const string WilfredFizzlebang = "AT_027";
        private const string ShadoPanRider = "AT_028";
        private const string Buccaneer = "AT_029";
        private const string UndercityValiant = "AT_030";
        private const string Cutpurse = "AT_031";
        private const string ShadyDealer = "AT_032";
        private const string Burgle = "AT_033";
        private const string PoisonedBlade = "AT_034";
        private const string BeneaththeGrounds = "AT_035";
        private const string Anubarak = "AT_036";
        private const string LivingRoots = "AT_037";
        private const string DarnassusAspirant = "AT_038";
        private const string SavageCombatant = "AT_039";
        private const string Wildwalker = "AT_040";
        private const string KnightoftheWild = "AT_041";
        private const string DruidoftheSaber = "AT_042";
        private const string AstralCommunion = "AT_043";
        private const string Mulch = "AT_044";
        private const string Aviana = "AT_045";
        private const string TuskarrTotemic = "AT_046";
        private const string DraeneiTotemcarver = "AT_047";
        private const string HealingWave = "AT_048";
        private const string ThunderBluffValiant = "AT_049";
        private const string ChargedHammer = "AT_050";
        private const string ElementalDestruction = "AT_051";
        private const string TotemGolem = "AT_052";
        private const string AncestralKnowledge = "AT_053";
        private const string TheMistcaller = "AT_054";
        private const string FlashHeal = "AT_055";
        private const string Powershot = "AT_056";
        private const string Stablemaster = "AT_057";
        private const string KingsElekk = "AT_058";
        private const string BraveArcher = "AT_059";
        private const string BearTrap = "AT_060";
        private const string LockandLoad = "AT_061";
        private const string BallofSpiders = "AT_062";
        private const string Acidmaw = "AT_063";
        private const string Dreadscale = "AT_063t";
        private const string Bash = "AT_064";
        private const string KingsDefender = "AT_065";
        private const string OrgrimmarAspirant = "AT_066";
        private const string MagnataurAlpha = "AT_067";
        private const string Bolster = "AT_068";
        private const string SparringPartner = "AT_069";
        private const string SkycapnKragg = "AT_070";
        private const string AlexstraszasChampion = "AT_071";
        private const string VarianWrynn = "AT_072";
        private const string CompetitiveSpirit = "AT_073";
        private const string SealofChampions = "AT_074";
        private const string WarhorseTrainer = "AT_075";
        private const string MurlocKnight = "AT_076";
        private const string ArgentLance = "AT_077";
        private const string EntertheColiseum = "AT_078";
        private const string MysteriousChallenger = "AT_079";
        private const string GarrisonCommander = "AT_080";
        private const string EadricthePure = "AT_081";
        private const string LowlySquire = "AT_082";
        private const string DragonhawkRider = "AT_083";
        private const string LanceCarrier = "AT_084";
        private const string MaidenoftheLake = "AT_085";
        private const string Saboteur = "AT_086";
        private const string ArgentHorserider = "AT_087";
        private const string MogorsChampion = "AT_088";
        private const string BoneguardLieutenant = "AT_089";
        private const string MuklasChampion = "AT_090";
        private const string TournamentMedic = "AT_091";
        private const string IceRager = "AT_092";
        private const string FrigidSnobold = "AT_093";
        private const string FlameJuggler = "AT_094";
        private const string SilentKnight = "AT_095";
        private const string ClockworkKnight = "AT_096";
        private const string TournamentAttendee = "AT_097";
        private const string SideshowSpelleater = "AT_098";
        private const string Kodorider = "AT_099";
        private const string SilverHandRegent = "AT_100";
        private const string PitFighter = "AT_101";
        private const string CapturedJormungar = "AT_102";
        private const string NorthSeaKraken = "AT_103";
        private const string TuskarrJouster = "AT_104";
        private const string InjuredKvaldir = "AT_105";
        private const string LightsChampion = "AT_106";
        private const string ArmoredWarhorse = "AT_108";
        private const string ArgentWatchman = "AT_109";
        private const string ColiseumManager = "AT_110";
        private const string RefreshmentVendor = "AT_111";
        private const string MasterJouster = "AT_112";
        private const string Recruiter = "AT_113";
        private const string EvilHeckler = "AT_114";
        private const string FencingCoach = "AT_115";
        private const string WyrmrestAgent = "AT_116";
        private const string MasterofCeremonies = "AT_117";
        private const string GrandCrusader = "AT_118";
        private const string KvaldirRaider = "AT_119";
        private const string FrostGiant = "AT_120";
        private const string CrowdFavorite = "AT_121";
        private const string GormoktheImpaler = "AT_122";
        private const string Chillmaw = "AT_123";
        private const string BolfRamshield = "AT_124";
        private const string Icehowl = "AT_125";
        private const string NexusChampionSaraad = "AT_127";
        private const string TheSkeletonKnight = "AT_128";
        private const string FjolaLightbane = "AT_129";
        private const string SeaReaver = "AT_130";
        private const string EydisDarkbane = "AT_131";
        private const string JusticarTrueheart = "AT_132";
        private const string GadgetzanJouster = "AT_133";

        #endregion
        public bool alreadyIdentified = false;
        public static readonly string MainDir = AppDomain.CurrentDomain.BaseDirectory + "MulliganProfiles\\SmartMulliganV3\\";
        public Dictionary<string,string> AllOpponentsDictionary = new Dictionary<string, string>(); 
        public override void OnTick()
        {
            Bot.Log("[SM Tracker] YOU ARE NOT SUPPOSE TO USE THIS YET");
            if (Bot.CurrentScene() != Bot.Scene.GAMEPLAY) alreadyIdentified = false;
            if (Bot.CurrentBoard == null || alreadyIdentified ) return;
            DeckData informationData = GetDeckInfo(Bot.CurrentBoard.FriendClass, Bot.CurrentDeck().Cards);
            CheckDirectory(MainDir);
            using (StreamWriter readMe = new StreamWriter(MainDir + "our_deck.v3", false))
            {
                readMe.WriteLine("{0}|{1}|{2}|{3}", informationData.DeckType, informationData.DeckStyle, string.Join(";",informationData.DeckList), "Hi");
            }
            Bot.Log(string.Format("Succesfully Identified deck\n\n{0}|{1}|{2}|{3}\n\n", informationData.DeckType, informationData.DeckStyle, string.Join(";", informationData.DeckList), "Hi"));
            alreadyIdentified = true;
        }

        public override void OnStarted()
        {
            using (StreamWriter debugStreamWriter = new StreamWriter(MainDir + "debug_decks.v3", false))
            {
                debugStreamWriter.WriteLine("{0}|{1}", ((smPluginDataContainer)DataContainer).TestYourDeck, ((smPluginDataContainer)DataContainer).TestOpponentDeck);
            }
            if (((smPluginDataContainer) DataContainer).AutoUpdateV3)
            {
                
            }
            if (((smPluginDataContainer) DataContainer).AutoUpdateTracker)
            {

            }
        }

        public override void OnStopped()
        {
            alreadyIdentified = false;
        }

        public void CheckOpponentDeck(string res)
        {
            List<Card.Cards> graveyard = Bot.CurrentBoard.EnemyGraveyard.ToList();
            List<Card> board = Bot.CurrentBoard.MinionEnemy.ToList();
            List<string> opponentDeck = new List<string> { };
            opponentDeck.AddRange(graveyard.Select(q => q.ToString()));
            opponentDeck.AddRange(board.Select(q => q.Template.Id.ToString()));
            using (StreamWriter opponentDeckInfo = new StreamWriter(MainDir + "OpponentDeckInfo.txt", true))
            {
                DeckData opponentInfo = GetDeckInfo(Bot.CurrentBoard.EnemyClass, opponentDeck);
                opponentDeckInfo.WriteLine("{0}:{1}:{2}:{3}:{4}",res ,  Bot.GetCurrentOpponentId(), opponentInfo.DeckType, opponentInfo.DeckStyle,  string.Join(",", opponentDeck.Where(c=> CardTemplate.LoadFromId(c).IsCollectible).ToArray()));
                Bot.Log("[Tracker] Succesfully recorder your opponent shit");
            }
        }
        public override void OnDefeat()
        {
            CheckOpponentDeck("lost");
            alreadyIdentified = false;
        }

        public override void OnVictory()
        {
            CheckOpponentDeck("won");
            alreadyIdentified = false;

        }

        public DeckData GetDeckInfo(Card.CClass ownClass, List<string> CurrentDeck)
        {
            var info = new DeckData { DeckList = CurrentDeck };

            Dictionary<Dictionary<DeckType, Style>, int> DeckDictionary =
                new Dictionary<Dictionary<DeckType, Style>, int>();
            Dictionary<DeckType, Style> BestDeck = new Dictionary<DeckType, Style>();
            switch (ownClass)
            {
                #region shaman

                case Card.CClass.SHAMAN:
                    var totemShaman = new List<string>
                    {
                        TunnelTrogg,
                        TunnelTrogg,
                        FlametongueTotem,
                        FlametongueTotem,
                        TotemGolem,
                        TotemGolem,
                        LightningStorm,
                        LightningStorm,
                        Hex,
                        Hex,
                        TuskarrTotemic,
                        TuskarrTotemic,
                        FireguardDestroyer,
                        FireguardDestroyer,
                        FireElemental,
                        FireElemental,
                        ThunderBluffValiant,
                        ThunderBluffValiant,
                        Neptulon,
                        DrBoom,
                        DefenderofArgus,
                        PilotedShredder,
                        PilotedShredder,
                        SylvanasWindrunner,
                        JeweledScarab,
                        JeweledScarab,
                        AzureDrake,
                        AzureDrake,
                    };
                    var d1 = new Dictionary<DeckType, Style> { { DeckType.TotemShaman, Style.Tempo } };
                    var faceShaman = new List<string>
                    {
                        UnboundElemental,
                        UnboundElemental,
                        EarthShock,
                        StormforgedAxe,
                        Doomhammer,
                        Doomhammer,
                        FeralSpirit,
                        FeralSpirit,
                        RockbiterWeapon,
                        RockbiterWeapon,
                        LeperGnome,
                        LeperGnome,
                        AbusiveSergeant,
                        LavaBurst,
                        LavaBurst,
                        Crackle,
                        Crackle,
                        LavaShock,
                        LavaShock,
                        TotemGolem,
                        TotemGolem,
                        ArgentHorserider,
                        ArgentHorserider,
                        AncestralKnowledge,
                        AncestralKnowledge,
                        SirFinleyMrrgglton,
                        TunnelTrogg,
                        TunnelTrogg,
                    };
                    var d2 = new Dictionary<DeckType, Style> { { DeckType.FaceShaman, Style.Face } };
                    var mechShaman = new List<string>
                    {
                        Hex,
                        HarvestGolem,
                        HarvestGolem,
                        FlametongueTotem,
                        FlametongueTotem,
                        RockbiterWeapon,
                        RockbiterWeapon,
                        FireElemental,
                        FireElemental,
                        Loatheb,
                        Cogmaster,
                        Cogmaster,
                        AnnoyoTron,
                        AnnoyoTron,
                        DrBoom,
                        SpiderTank,
                        SpiderTank,
                        Mechwarper,
                        Mechwarper,
                        PilotedShredder,
                        PilotedShredder,
                        BombLobber,
                        BombLobber,
                        WhirlingZapomatic,
                        WhirlingZapomatic,
                        Crackle,
                        Crackle,
                        Powermace,
                        Powermace,
                    };
                    var d3 = new Dictionary<DeckType, Style> { { DeckType.MechShaman, Style.Aggro } };
                    var dragonShaman = new List<string>
                    {
                        FeralSpirit,
                        Hex,
                        Hex,
                        AzureDrake,
                        AzureDrake,
                        Deathwing,
                        Ysera,
                        FireElemental,
                        FireElemental,
                        LightningStorm,
                        LightningStorm,
                        BlackwingTechnician,
                        BlackwingTechnician,
                        LavaShock,
                        BlackwingCorruptor,
                        TotemGolem,
                        TotemGolem,
                        AncestralKnowledge,
                        HealingWave,
                        HealingWave,
                        TheMistcaller,
                        TwilightGuardian,
                        TwilightGuardian,
                        Chillmaw,
                        JeweledScarab,
                        JeweledScarab,
                        BrannBronzebeard,
                        TunnelTrogg,
                        TunnelTrogg,
                    };
                    var d4 = new Dictionary<DeckType, Style> { { DeckType.MechShaman, Style.Aggro } };
                    var malygosShaman = new List<string>
                    {
                        EarthShock,
                        EarthShock,
                        FarSight,
                        FarSight,
                        StormforgedAxe,
                        FeralSpirit,
                        FeralSpirit,
                        FrostShock,
                        FrostShock,
                        Malygos,
                        GnomishInventor,
                        GnomishInventor,
                        Crackle,
                        Crackle,
                        Hex,
                        Hex,
                        LavaBurst,
                        LavaBurst,
                        ManaTideTotem,
                        ManaTideTotem,
                        LightningStorm,
                        LightningStorm,
                        AncestorsCall,
                        AncestorsCall,
                        AntiqueHealbot,
                        AntiqueHealbot,
                        Alexstrasza,
                        AzureDrake,
                    };
                    var controlShaman = new List<string>
                    {
                        EarthShock,
                        Hex,
                        Hex,
                        AzureDrake,
                        AzureDrake,
                        Doomsayer,
                        Doomsayer,
                        Ysera,
                        FireElemental,
                        FireElemental,
                        LightningStorm,
                        LightningStorm,
                        Loatheb,
                        SludgeBelcher,
                        SludgeBelcher,
                        DrBoom,
                        Neptulon,
                        LavaShock,
                        LavaShock,
                        VolcanicDrake,
                        VolcanicDrake,
                        HealingWave,
                        HealingWave,
                        ElementalDestruction,
                        ElementalDestruction,
                        TwilightGuardian,
                        TwilightGuardian,
                        JeweledScarab,
                        JeweledScarab,
                    };
                    var basicShaman = new List<string>
                    {
                        BoulderfistOgre,
                        AcidicSwampOoze,
                        GnomishInventor,
                        Bloodlust,
                        Hex,
                        SenjinShieldmasta,
                        FlametongueTotem,
                        ShatteredSunCleric,
                        RockbiterWeapon,
                        TunnelTrogg,
                        RumblingElemental,
                        FireElemental,
                        SirFinleyMrrgglton,
                        JeweledScarab,
                        BrannBronzebeard,
                        ArchThiefRafaam,
                        FrostwolfWarlord
                    };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {
                            new Dictionary<DeckType, Style> {{DeckType.TotemShaman, Style.Tempo}},
                            CurrentDeck.Intersect(totemShaman).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.FaceShaman, Style.Face}},
                            CurrentDeck.Intersect(faceShaman).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.MechShaman, Style.Aggro}},
                            CurrentDeck.Intersect(mechShaman).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.DragonShaman, Style.Control}},
                            CurrentDeck.Intersect(dragonShaman).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.MalygosShaman, Style.Combo}},
                            CurrentDeck.Intersect(malygosShaman).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.ControlShaman, Style.Control}},
                            CurrentDeck.Intersect(controlShaman).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}},
                            CurrentDeck.Intersect(basicShaman).ToList().Count
                        },
                    };
                    //if (DeckHas(CurrentDeck, Malygos))
                    // DeckDictionary.Remove(d1);
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                #endregion

                #region priest

                case Card.CClass.PRIEST:
                    List<string> comboPriest = new List<string>
                    {
                        CircleofHealing,
                        CircleofHealing,
                        InnerFire,
                        InnerFire,
                        Alexstrasza,
                        AcolyteofPain,
                        AcolyteofPain,
                        PowerWordShield,
                        PowerWordShield,
                        Silence,
                        Silence,
                        ShadowWordDeath,
                        DivineSpirit,
                        DivineSpirit,
                        NorthshireCleric,
                        NorthshireCleric,
                        HarrisonJones,
                        StormwindKnight,
                        AuchenaiSoulpriest,
                        HolyNova,
                        Loatheb,
                        Deathlord,
                        Deathlord,
                        VelensChosen,
                        GnomereganInfantry,
                        Lightbomb,
                        Lightbomb,
                        EmperorThaurissan,
                    };
                    List<string> dragonPriest = new List<string>
                    {
                        CabalShadowPriest,
                        CabalShadowPriest,
                        AzureDrake,
                        PowerWordShield,
                        PowerWordShield,
                        Ysera,
                        ShadowWordDeath,
                        ShadowWordDeath,
                        NorthshireCleric,
                        HarrisonJones,
                        HolyNova,
                        SludgeBelcher,
                        VelensChosen,
                        VelensChosen,
                        DrBoom,
                        Shrinkmeister,
                        Shrinkmeister,
                        Voljin,
                        Lightbomb,
                        Lightbomb,
                        BlackwingTechnician,
                        BlackwingTechnician,
                        TwilightWhelp,
                        TwilightWhelp,
                        TwilightGuardian,
                        TwilightGuardian,
                        Chillmaw,
                        WyrmrestAgent,
                        WyrmrestAgent,
                    };
                    List<string> controlPriest = new List<string>
                    {
                        LightoftheNaaru,
                        LightoftheNaaru,
                        PowerWordShield,
                        PowerWordShield,
                        NorthshireCleric,
                        NorthshireCleric,
                        MuseumCurator,
                        MuseumCurator,
                        Shrinkmeister,
                        ShadowWordDeath,
                        AuchenaiSoulpriest,
                        AuchenaiSoulpriest,
                        HolyNova,
                        Entomb,
                        Entomb,
                        Lightbomb,
                        Lightbomb,
                        CabalShadowPriest,
                        WildPyromancer,
                        WildPyromancer,
                        Deathlord,
                        Deathlord,
                        InjuredBlademaster,
                        InjuredBlademaster,
                        SludgeBelcher,
                        SludgeBelcher,
                        JusticarTrueheart,
                        Ysera,
                    };
                    List<string> mechPriest = new List<string>
                    {
                        PowerWordShield,
                        PowerWordShield,
                        NorthshireCleric,
                        NorthshireCleric,
                        HolyNova,
                        HolyNova,
                        DarkCultist,
                        DarkCultist,
                        VelensChosen,
                        VelensChosen,
                        DrBoom,
                        SpiderTank,
                        UpgradedRepairBot,
                        UpgradedRepairBot,
                        Mechwarper,
                        Mechwarper,
                        PilotedShredder,
                        PilotedShredder,
                        ClockworkGnome,
                        ClockworkGnome,
                        Shadowboxer,
                        Shadowboxer,
                        Voljin,
                        SpawnofShadows,
                        GorillabotA3,
                        Entomb,
                        MuseumCurator,
                        MuseumCurator,
                    };
                    List<string> basicPriest = new List<string>
                    {
                        ChillwindYeti,
                        BoulderfistOgre,
                        AcidicSwampOoze,
                        GnomishInventor,
                        StormwindChampion,
                        ShadowWordPain,
                        SenjinShieldmasta,
                        MindControl,
                        HolySmite,
                        PowerWordShield,
                        ShatteredSunCleric,
                        NoviceEngineer,
                        ShadowWordDeath,
                        BloodfenRaptor,
                        NorthshireCleric,
                        HolyNova,
                    };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {
                            new Dictionary<DeckType, Style> {{DeckType.ComboPriest, Style.Combo}},
                            CurrentDeck.Intersect(comboPriest).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.DragonPriest, Style.Tempo}},
                            CurrentDeck.Intersect(dragonPriest).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.ControlPriest, Style.Control}},
                            CurrentDeck.Intersect(controlPriest).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.MechPriest, Style.Aggro}},
                            CurrentDeck.Intersect(mechPriest).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}},
                            CurrentDeck.Intersect(basicPriest).ToList().Count
                        },
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                #endregion

                #region mage

                case Card.CClass.MAGE:
                    List<string> tempoMage = new List<string>
                    {
                        MirrorImage,
                        Frostbolt,
                        Frostbolt,
                        ArchmageAntonidas,
                        ManaWyrm,
                        ManaWyrm,
                        AzureDrake,
                        AzureDrake,
                        ArcaneIntellect,
                        ArcaneIntellect,
                        Fireball,
                        Fireball,
                        Counterspell,
                        MirrorEntity,
                        ArcaneMissiles,
                        ArcaneMissiles,
                        MadScientist,
                        MadScientist,
                        UnstablePortal,
                        UnstablePortal,
                        DrBoom,
                        PilotedShredder,
                        PilotedShredder,
                        Flamecannon,
                        ClockworkGnome,
                        Flamewaker,
                        Flamewaker,
                        ArcaneBlast,
                    };
                    List<string> freezeMage = new List<string>
                    {
                        Flamestrike,
                        FrostNova,
                        FrostNova,
                        Frostbolt,
                        Frostbolt,
                        IceLance,
                        IceLance,
                        ArchmageAntonidas,
                        Blizzard,
                        Blizzard,
                        Alexstrasza,
                        LootHoarder,
                        AcolyteofPain,
                        AcolyteofPain,
                        Doomsayer,
                        Doomsayer,
                        ArcaneIntellect,
                        ArcaneIntellect,
                        Pyroblast,
                        Fireball,
                        Fireball,
                        BloodmageThalnos,
                        IceBarrier,
                        IceBarrier,
                        MadScientist,
                        MadScientist,
                        AntiqueHealbot,
                        EmperorThaurissan,
                    };
                    List<string> mechMage = new List<string>
                    {
                        ArchmageAntonidas,
                        ManaWyrm,
                        ManaWyrm,
                        Fireball,
                        Fireball,
                        UnstablePortal,
                        UnstablePortal,
                        Cogmaster,
                        Cogmaster,
                        AnnoyoTron,
                        AnnoyoTron,
                        DrBoom,
                        SpiderTank,
                        SpiderTank,
                        Mechwarper,
                        Mechwarper,
                        PilotedShredder,
                        PilotedShredder,
                        GoblinBlastmage,
                        GoblinBlastmage,
                        ClockworkGnome,
                        TinkertownTechnician,
                        Snowchugger,
                        Snowchugger,
                        ClockworkKnight,
                        ClockworkKnight,
                        GorillabotA3,
                        GorillabotA3,
                    };
                    List<string> echoMage = new List<string>
                    {
                        IceBlock,
                        IceBlock,
                        Flamestrike,
                        Flamestrike,
                        BigGameHunter,
                        MoltenGiant,
                        MoltenGiant,
                        Frostbolt,
                        Frostbolt,
                        ArchmageAntonidas,
                        Blizzard,
                        Blizzard,
                        Alexstrasza,
                        SunfuryProtector,
                        SunfuryProtector,
                        AcolyteofPain,
                        AcolyteofPain,
                        ArcaneIntellect,
                        ArcaneIntellect,
                        Polymorph,
                        MadScientist,
                        MadScientist,
                        ExplosiveSheep,
                        ExplosiveSheep,
                        AntiqueHealbot,
                        EchoofMedivh,
                        EchoofMedivh,
                        EmperorThaurissan,
                    };
                    List<string> basicMage = new List<string>
                    {
                        ChillwindYeti,
                        Flamestrike,
                        RazorfenHunter,
                        BoulderfistOgre,
                        AcidicSwampOoze,
                        Frostbolt,
                        GnomishInventor,
                        WaterElemental,
                        StormwindChampion,
                        SenjinShieldmasta,
                        ShatteredSunCleric,
                        ArcaneIntellect,
                        Fireball,
                        BloodfenRaptor,
                        ArcaneMissiles,
                        Polymorph,
                        GurubashiBerserker,
                    };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {
                            new Dictionary<DeckType, Style> {{DeckType.TempoMage, Style.Tempo}},
                            CurrentDeck.Intersect(tempoMage).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.FreezeMage, Style.Control}},
                            CurrentDeck.Intersect(freezeMage).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.MechMage, Style.Aggro}},
                            CurrentDeck.Intersect(mechMage).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.FatigueMage, Style.Control}},
                            CurrentDeck.Intersect(echoMage).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}},
                            CurrentDeck.Intersect(basicMage).ToList().Count
                        },
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                #endregion

                #region paladin

                case Card.CClass.PALADIN:
                    List<string> secretPaladin = new List<string>
                    {
                        BlessingofKings,
                        BigGameHunter,
                        NobleSacrifice,
                        NobleSacrifice,
                        Consecration,
                        TruesilverChampion,
                        TirionFordring,
                        Secretkeeper,
                        Secretkeeper,
                        IronbeakOwl,
                        HarrisonJones,
                        Redemption,
                        Avenge,
                        Avenge,
                        SludgeBelcher,
                        HauntedCreeper,
                        DrBoom,
                        PilotedShredder,
                        PilotedShredder,
                        MusterforBattle,
                        MusterforBattle,
                        Coghammer,
                        ShieldedMinibot,
                        ShieldedMinibot,
                        CompetitiveSpirit,
                        MysteriousChallenger,
                        MysteriousChallenger,
                        KeeperofUldaman,
                    };
                    List<string> midRangePaladin = new List<string>
                    {
                        BigGameHunter,
                        Consecration,
                        Consecration,
                        TruesilverChampion,
                        TruesilverChampion,
                        TirionFordring,
                        KnifeJuggler,
                        KnifeJuggler,
                        IronbeakOwl,
                        ZombieChow,
                        ZombieChow,
                        Loatheb,
                        SludgeBelcher,
                        SludgeBelcher,
                        DrBoom,
                        PilotedShredder,
                        PilotedShredder,
                        MusterforBattle,
                        MusterforBattle,
                        AntiqueHealbot,
                        Coghammer,
                        ShieldedMinibot,
                        ShieldedMinibot,
                        Quartermaster,
                        Quartermaster,
                        JusticarTrueheart,
                        KeeperofUldaman,
                        KeeperofUldaman,
                    };
                    List<string> aggroPaladin = new List<string>
                    {
                        ArcaneGolem,
                        SouthseaDeckhand,
                        SouthseaDeckhand,
                        Consecration,
                        TruesilverChampion,
                        TruesilverChampion,
                        HammerofWrath,
                        BlessingofMight,
                        BlessingofMight,
                        KnifeJuggler,
                        KnifeJuggler,
                        ArgentSquire,
                        ArgentSquire,
                        IronbeakOwl,
                        LeperGnome,
                        LeperGnome,
                        AbusiveSergeant,
                        AbusiveSergeant,
                        DivineFavor,
                        DivineFavor,
                        LeeroyJenkins,
                        HauntedCreeper,
                        MusterforBattle,
                        MusterforBattle,
                        Coghammer,
                        ShieldedMinibot,
                        ShieldedMinibot,
                        SealofChampions,
                    };
                    List<string> anyfin = new List<string>
                    {
                        AldorPeacekeeper,
                        CultMaster,
                        OldMurkEye,
                        MurlocWarleader,
                        Consecration,
                        BluegillWarrior,
                        TruesilverChampion,
                        KnifeJuggler,
                        LayonHands,
                        GrimscaleOracle,
                        ZombieChow,
                        SludgeBelcher,
                        DrBoom,
                        PilotedShredder,
                        MusterforBattle,
                        AntiqueHealbot,
                        Coghammer,
                        ShieldedMinibot,
                        SolemnVigil,
                        AnyfinCanHappen,
                        KeeperofUldaman
                    };
                    List<string> basicPaladin = new List<string>
                    {
                        BlessingofKings,
                        ChillwindYeti,
                        RazorfenHunter,
                        BoulderfistOgre,
                        AcidicSwampOoze,
                        Consecration,
                        GuardianofKings,
                        TruesilverChampion,
                        StormwindChampion,
                        SenjinShieldmasta,
                        HammerofWrath,
                        MurlocTidehunter,
                        ShatteredSunCleric,
                        RiverCrocolisk,
                        BloodfenRaptor,
                        FrostwolfWarlord,
                    };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {
                            new Dictionary<DeckType, Style> {{DeckType.SecretPaladin, Style.Tempo}},
                            CurrentDeck.Intersect(secretPaladin).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.MidRangePaladin, Style.Control}},
                            CurrentDeck.Intersect(midRangePaladin).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.AggroPaladin, Style.Face}},
                            CurrentDeck.Intersect(aggroPaladin).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.AnyfinMurglMurgl, Style.Combo}},
                            CurrentDeck.Intersect(anyfin).ToList().Count
                        },
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                #endregion

                #region warrior

                case Card.CClass.WARRIOR:
                    List<string> corePatron = new List<string>
                    {
                        KorkronElite,
                        KorkronElite,
                        Whirlwind,
                        Whirlwind,
                        Slam,
                        Slam,
                        Execute,
                        Execute,
                        DreadCorsair,
                        InnerRage,
                        InnerRage,
                        AcolyteofPain,
                        AcolyteofPain,
                        FieryWarAxe,
                        FieryWarAxe,
                        GrommashHellscream,
                        Armorsmith,
                        Armorsmith,
                        BattleRage,
                        BattleRage,
                        DeathsBite,
                        DeathsBite,
                        UnstableGhoul,
                        UnstableGhoul,
                        DrBoom,
                        GrimPatron,
                        GrimPatron,
                        SirFinleyMrrgglton,
                    };
                    List<string> controlWarrior = new List<string>
                    {
                        BigGameHunter,
                        Slam,
                        Slam,
                        Execute,
                        Execute,
                        Brawl,
                        Brawl,
                        CruelTaskmaster,
                        AcolyteofPain,
                        AcolyteofPain,
                        ShieldBlock,
                        ShieldBlock,
                        BaronGeddon,
                        FieryWarAxe,
                        FieryWarAxe,
                        Armorsmith,
                        Armorsmith,
                        DeathsBite,
                        DeathsBite,
                        SludgeBelcher,
                        SludgeBelcher,
                        Shieldmaiden,
                        Shieldmaiden,
                        JusticarTrueheart,
                        Bash,
                        EliseStarseeker,
                        FierceMonkey,
                        FierceMonkey,
                    };
                    List<string> fatigueWarrior = new List<string>
                    {
                        BigGameHunter,
                        BigGameHunter,
                        AcidicSwampOoze,
                        ColdlightOracle,
                        ColdlightOracle,
                        Gorehowl,
                        Execute,
                        Execute,
                        Brawl,
                        Brawl,
                        CruelTaskmaster,
                        CruelTaskmaster,
                        ShieldBlock,
                        FieryWarAxe,
                        Armorsmith,
                        DeathsBite,
                        DeathsBite,
                        SludgeBelcher,
                        SludgeBelcher,
                        Deathlord,
                        Deathlord,
                        Shieldmaiden,
                        Shieldmaiden,
                        AntiqueHealbot,
                        AntiqueHealbot,
                        IronJuggernaut,
                        JusticarTrueheart,
                        BrannBronzebeard,
                    };
                    List<string> faceWarrior = new List<string>
                    {
                        ArcaneGolem,
                        ArcaneGolem,
                        SouthseaDeckhand,
                        SouthseaDeckhand,
                        KorkronElite,
                        KorkronElite,
                        Wolfrider,
                        DreadCorsair,
                        DreadCorsair,
                        MortalStrike,
                        MortalStrike,
                        IronbeakOwl,
                        LeperGnome,
                        LeperGnome,
                        FieryWarAxe,
                        FieryWarAxe,
                        BloodsailRaider,
                        BloodsailRaider,
                        Upgrade,
                        Upgrade,
                        DeathsBite,
                        DeathsBite,
                        ArgentHorserider,
                        ArgentHorserider,
                        Bash,
                        Bash,
                        SirFinleyMrrgglton,
                        CursedBlade,
                    };
                    List<string> dragonWarrior = new List<string>
                    {
                        Execute,
                        Execute,
                        AzureDrake,
                        AzureDrake,
                        Alexstrasza,
                        CruelTaskmaster,
                        CruelTaskmaster,
                        TwilightDrake,
                        TwilightDrake,
                        FieryWarAxe,
                        FieryWarAxe,
                        DeathsBite,
                        DeathsBite,
                        Loatheb,
                        DrBoom,
                        Shieldmaiden,
                        Shieldmaiden,
                        IronJuggernaut,
                        BlackwingTechnician,
                        BlackwingTechnician,
                        BlackwingCorruptor,
                        BlackwingCorruptor,
                        Nefarian,
                        JusticarTrueheart,
                        AlexstraszasChampion,
                        TwilightGuardian,
                        TwilightGuardian,
                        BrannBronzebeard,
                    };
                    List<string> mechWarrior = new List<string>
                    {
                        KorkronElite,
                        ArcaniteReaper,
                        MortalStrike,
                        MortalStrike,
                        HarvestGolem,
                        HarvestGolem,
                        IronbeakOwl,
                        FieryWarAxe,
                        FieryWarAxe,
                        DeathsBite,
                        DeathsBite,
                        Cogmaster,
                        AnnoyoTron,
                        AnnoyoTron,
                        SpiderTank,
                        SpiderTank,
                        Mechwarper,
                        Mechwarper,
                        PilotedShredder,
                        PilotedShredder,
                        ScrewjankClunker,
                        ScrewjankClunker,
                        Warbot,
                        Warbot,
                        FelReaver,
                        FelReaver,
                        SirFinleyMrrgglton,
                        GorillabotA3,
                    };
                    List<string> basicWarrior = new List<string>
                    {
                        ChillwindYeti,
                        RazorfenHunter,
                        BoulderfistOgre,
                        AcidicSwampOoze,
                        Cleave,
                        KorkronElite,
                        ArcaniteReaper,
                        Execute,
                        GnomishInventor,
                        StormwindChampion,
                        SenjinShieldmasta,
                        ShatteredSunCleric,
                        ShieldBlock,
                        RiverCrocolisk,
                        BloodfenRaptor,
                        FieryWarAxe
                    };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {
                            new Dictionary<DeckType, Style> {{DeckType.PatronWarrior, Style.Tempo}},
                            CurrentDeck.Intersect(corePatron).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.ControlWarrior, Style.Control}},
                            CurrentDeck.Intersect(controlWarrior).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.FatigueWarrior, Style.Control}},
                            CurrentDeck.Intersect(fatigueWarrior).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.FaceWarrior, Style.Face}},
                            CurrentDeck.Intersect(faceWarrior).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.DragonWarrior, Style.Control}},
                            CurrentDeck.Intersect(dragonWarrior).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.MechWarrior, Style.Aggro}},
                            CurrentDeck.Intersect(mechWarrior).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Aggro}},
                            CurrentDeck.Intersect(basicWarrior).ToList().Count
                        },
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                #endregion

                #region warlock

                case Card.CClass.WARLOCK:
                    List<string> renolock = new List<string>
                    {
                        MortalCoil,
                        Darkbomb,
                        DarkPeddler,
                        Demonwrath,
                        ImpGangBoss,
                        Hellfire,
                        Implosion,
                        Shadowflame,
                        SiphonSoul,
                        LordJaraxxus,
                        AbusiveSergeant,
                        ZombieChow,
                        IronbeakOwl,
                        SunfuryProtector,
                        BigGameHunter,
                        BrannBronzebeard,
                        EarthenRingFarseer,
                        DefenderofArgus,
                        PilotedShredder,
                        TwilightDrake,
                        AntiqueHealbot,
                        Feugen,
                        Loatheb,
                        SludgeBelcher,
                        Stalagg,
                        EmperorThaurissan,
                        RenoJackson,
                        DrBoom,
                        MoltenGiant,
                    };
                    List<string> demonHandlock = new List<string>
                    {
                        MortalCoil,
                        MortalCoil,
                        MoltenGiant,
                        MoltenGiant,
                        AncientWatcher,
                        AncientWatcher,
                        MountainGiant,
                        MountainGiant,
                        TwilightDrake,
                        TwilightDrake,
                        SunfuryProtector,
                        SunfuryProtector,
                        LordJaraxxus,
                        IronbeakOwl,
                        DefenderofArgus,
                        SiphonSoul,
                        Shadowflame,
                        Shadowflame,
                        ZombieChow,
                        ZombieChow,
                        Voidcaller,
                        Voidcaller,
                        Loatheb,
                        SludgeBelcher,
                        SludgeBelcher,
                        DrBoom,
                        AntiqueHealbot,
                        MalGanis,
                        Darkbomb,
                    };
                    List<string> zoolock = new List<string>
                    {
                        FlameImp,
                        FlameImp,
                        PowerOverwhelming,
                        PowerOverwhelming,
                        DireWolfAlpha,
                        DireWolfAlpha,
                        Voidwalker,
                        Voidwalker,
                        KnifeJuggler,
                        KnifeJuggler,
                        IronbeakOwl,
                        Doomguard,
                        Doomguard,
                        DefenderofArgus,
                        DefenderofArgus,
                        AbusiveSergeant,
                        AbusiveSergeant,
                        Voidcaller,
                        Voidcaller,
                        NerubianEgg,
                        NerubianEgg,
                        Loatheb,
                        HauntedCreeper,
                        HauntedCreeper,
                        DrBoom,
                        Implosion,
                        Implosion,
                        ImpGangBoss,
                        ImpGangBoss,
                    };
                    List<string> dragonHandlock = new List<string>
                    {
                        MortalCoil,
                        MortalCoil,
                        Darkbomb,
                        Darkbomb,
                        AncientWatcher,
                        AncientWatcher,
                        IronbeakOwl,
                        SunfuryProtector,
                        SunfuryProtector,
                        BigGameHunter,
                        Hellfire,
                        Hellfire,
                        Shadowflame,
                        DefenderofArgus,
                        TwilightDrake,
                        TwilightDrake,
                        TwilightGuardian,
                        TwilightGuardian,
                        AntiqueHealbot,
                        BlackwingCorruptor,
                        BlackwingCorruptor,
                        EmperorThaurissan,
                        Chillmaw,
                        DrBoom,
                        Alexstrasza,
                        MountainGiant,
                        MountainGiant,
                        MoltenGiant,
                        MoltenGiant,
                    };
                    List<string> demonZooWarlock = new List<string>
                    {
                        PowerOverwhelming,
                        PowerOverwhelming,
                        Voidwalker,
                        Voidwalker,
                        KnifeJuggler,
                        KnifeJuggler,
                        IronbeakOwl,
                        Doomguard,
                        Doomguard,
                        DefenderofArgus,
                        DefenderofArgus,
                        AbusiveSergeant,
                        AbusiveSergeant,
                        SeaGiant,
                        BaneofDoom,
                        Voidcaller,
                        Voidcaller,
                        NerubianEgg,
                        NerubianEgg,
                        HauntedCreeper,
                        HauntedCreeper,
                        DrBoom,
                        MalGanis,
                        Implosion,
                        Implosion,
                        ImpGangBoss,
                        ImpGangBoss,
                        DarkPeddler,
                        DarkPeddler,
                    };
                    List<string> handlock = new List<string>
                    {
                        BigGameHunter,
                        MoltenGiant,
                        MoltenGiant,
                        Hellfire,
                        Hellfire,
                        AncientWatcher,
                        MountainGiant,
                        MountainGiant,
                        TwilightDrake,
                        TwilightDrake,
                        SunfuryProtector,
                        SunfuryProtector,
                        LordJaraxxus,
                        IronbeakOwl,
                        IronbeakOwl,
                        DefenderofArgus,
                        Shadowflame,
                        Loatheb,
                        SludgeBelcher,
                        SludgeBelcher,
                        DrBoom,
                        AntiqueHealbot,
                        AntiqueHealbot,
                        Darkbomb,
                        Darkbomb,
                        EmperorThaurissan,
                        BrannBronzebeard,
                        DarkPeddler,
                    };
                    List<string> relinquary = new List<string>
                    {
                        PowerOverwhelming,
                        PowerOverwhelming,
                        Voidwalker,
                        Voidwalker,
                        IronbeakOwl,
                        DefenderofArgus,
                        DefenderofArgus,
                        AbusiveSergeant,
                        AbusiveSergeant,
                        SeaGiant,
                        SeaGiant,
                        ZombieChow,
                        NerubianEgg,
                        NerubianEgg,
                        Loatheb,
                        EchoingOoze,
                        EchoingOoze,
                        HauntedCreeper,
                        HauntedCreeper,
                        DrBoom,
                        Implosion,
                        Implosion,
                        ImpGangBoss,
                        ImpGangBoss,
                        GormoktheImpaler,
                        DarkPeddler,
                        DarkPeddler,
                        ReliquarySeeker,
                        ReliquarySeeker,
                    };
                    List<string> basicdeck = new List<string>
                    {
                        ChillwindYeti,
                        ShatteredSunCleric,
                        SenjinShieldmasta,
                        ZombieChow,
                        HauntedCreeper,
                        SludgeBelcher,
                        KelThuzad,
                        Loatheb,
                        EmperorThaurissan,
                        ImpGangBoss,
                        DarkPeddler,
                        JeweledScarab,
                        ArchThiefRafaam,
                        ShadowBolt,
                        Hellfire,
                        DreadInfernal,
                        MortalCoil,
                    };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {
                            new Dictionary<DeckType, Style> {{DeckType.RenoLock, Style.Control}},
                            CurrentDeck.Intersect(renolock).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.DemonHandlock, Style.Control}},
                            CurrentDeck.Intersect(demonHandlock).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.Zoolock, Style.Aggro}},
                            CurrentDeck.Intersect(zoolock).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.DragonHandlock, Style.Control}},
                            CurrentDeck.Intersect(dragonHandlock).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.DemonZooWarlock, Style.Aggro}},
                            CurrentDeck.Intersect(demonZooWarlock).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.Handlock, Style.Control}},
                            CurrentDeck.Intersect(handlock).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.RelinquaryZoo, Style.Aggro}},
                            CurrentDeck.Intersect(relinquary).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}},
                            CurrentDeck.Intersect(basicdeck).ToList().Count
                        },
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                #endregion

                #region hunter

                case Card.CClass.HUNTER:
                    List<string> midRangeHunter = new List<string>
                    {
                        HuntersMark,
                        HuntersMark,
                        Webspinner,
                        BearTrap,
                        FreezingTrap,
                        SnakeTrap,
                        EaglehornBow,
                        EaglehornBow,
                        AnimalCompanion,
                        AnimalCompanion,
                        KillCommand,
                        KillCommand,
                        UnleashtheHounds,
                        UnleashtheHounds,
                        Houndmaster,
                        Houndmaster,
                        RamWrangler,
                        HauntedCreeper,
                        JeweledScarab,
                        KnifeJuggler,
                        KnifeJuggler,
                        MadScientist,
                        MadScientist,
                        TombSpider,
                        TombSpider,
                        SludgeBelcher,
                        SludgeBelcher,
                        DrBoom,
                    };
                    List<string> hybridHunter = new List<string>
                    {
                        FreezingTrap,
                        UnleashtheHounds,
                        UnleashtheHounds,
                        ExplosiveTrap,
                        EaglehornBow,
                        KnifeJuggler,
                        KnifeJuggler,
                        KillCommand,
                        KillCommand,
                        IronbeakOwl,
                        LeperGnome,
                        LeperGnome,
                        AbusiveSergeant,
                        AbusiveSergeant,
                        AnimalCompanion,
                        AnimalCompanion,
                        Loatheb,
                        MadScientist,
                        MadScientist,
                        HauntedCreeper,
                        HauntedCreeper,
                        PilotedShredder,
                        PilotedShredder,
                        Glaivezooka,
                        Glaivezooka,
                        QuickShot,
                        ArgentHorserider,
                        ArgentHorserider,
                    };
                    List<string> faceHunter = new List<string>
                    {
                        Glaivezooka,
                        Glaivezooka,
                        ExplosiveTrap,
                        QuickShot,
                        QuickShot,
                        EaglehornBow,
                        AnimalCompanion,
                        AnimalCompanion,
                        UnleashtheHounds,
                        UnleashtheHounds,
                        KillCommand,
                        KillCommand,
                        DartTrap,
                        DesertCamel,
                        AbusiveSergeant,
                        AbusiveSergeant,
                        LeperGnome,
                        LeperGnome,
                        SouthseaDeckhand,
                        HauntedCreeper,
                        IronbeakOwl,
                        IronbeakOwl,
                        KnifeJuggler,
                        KnifeJuggler,
                        MadScientist,
                        MadScientist,
                        ArcaneGolem,
                        ArcaneGolem,
                        WorgenInfiltrator,
                    };
                    List<string> basicHunter = new List<string>
                    {
                        RazorfenHunter,
                        TimberWolf,
                        StarvingBuzzard,
                        TundraRhino,
                        ArcaneShot,
                        Wolfrider,
                        Houndmaster,
                        BluegillWarrior,
                        MultiShot,
                        ShatteredSunCleric,
                        KillCommand,
                        RiverCrocolisk,
                        RecklessRocketeer,
                        BloodfenRaptor,
                        AnimalCompanion
                    };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {
                            new Dictionary<DeckType, Style> {{DeckType.MidRangeHunter, Style.Tempo}},
                            CurrentDeck.Intersect(midRangeHunter).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.HybridHunter, Style.Aggro}},
                            CurrentDeck.Intersect(hybridHunter).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.FaceHunter, Style.Face}},
                            CurrentDeck.Intersect(faceHunter).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}},
                            CurrentDeck.Intersect(basicHunter).ToList().Count
                        },
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                #endregion

                #region rogue

                case Card.CClass.ROGUE:
                    List<string> raptorRogue = new List<string>
                    {
                        ColdBlood,
                        ColdBlood,
                        FanofKnives,
                        FanofKnives,
                        Eviscerate,
                        Eviscerate,
                        Sap,
                        LootHoarder,
                        LootHoarder,
                        Backstab,
                        Backstab,
                        UnearthedRaptor,
                        UnearthedRaptor,
                        AbusiveSergeant,
                        AbusiveSergeant,
                        LeperGnome,
                        LeperGnome,
                        NerubianEgg,
                        NerubianEgg,
                        DefenderofArgus,
                        DefenderofArgus,
                        PilotedShredder,
                        PilotedShredder,
                        Loatheb,
                        SludgeBelcher,
                        SludgeBelcher,
                        DrBoom,
                        HauntedCreeper,
                        HauntedCreeper,
                    };
                    List<string> pirateRogue = new List<string>
                    {
                        Sprint,
                        Sprint,
                        SouthseaDeckhand,
                        SouthseaDeckhand,
                        BladeFlurry,
                        BladeFlurry,
                        DreadCorsair,
                        DreadCorsair,
                        AzureDrake,
                        AzureDrake,
                        SI7Agent,
                        SI7Agent,
                        Preparation,
                        Preparation,
                        Eviscerate,
                        Eviscerate,
                        Sap,
                        AssassinsBlade,
                        Backstab,
                        Backstab,
                        BloodsailRaider,
                        BloodsailRaider,
                        ShipsCannon,
                        ShipsCannon,
                        TinkersSharpswordOil,
                        TinkersSharpswordOil,
                        Buccaneer,
                        Buccaneer,
                    };
                    List<string> oilRogue = new List<string>
                    {
                        DeadlyPoison,
                        DeadlyPoison,
                        Sprint,
                        Sprint,
                        SouthseaDeckhand,
                        BladeFlurry,
                        BladeFlurry,
                        AzureDrake,
                        AzureDrake,
                        SI7Agent,
                        SI7Agent,
                        Preparation,
                        Preparation,
                        FanofKnives,
                        FanofKnives,
                        Eviscerate,
                        Eviscerate,
                        Sap,
                        Sap,
                        Backstab,
                        Backstab,
                        BloodmageThalnos,
                        PilotedShredder,
                        PilotedShredder,
                        AntiqueHealbot,
                        TinkersSharpswordOil,
                        TinkersSharpswordOil,
                        TombPillager,
                        TombPillager,
                    };
                    List<string> burstRogue = new List<string>
                    {
                        ColdlightOracle,
                        ColdBlood,
                        ColdBlood,
                        ArcaneGolem,
                        ArcaneGolem,
                        SouthseaDeckhand,
                        BladeFlurry,
                        BladeFlurry,
                        SI7Agent,
                        SI7Agent,
                        Eviscerate,
                        Eviscerate,
                        Sap,
                        DefiasRingleader,
                        AssassinsBlade,
                        ArgentSquire,
                        ArgentSquire,
                        IronbeakOwl,
                        IronbeakOwl,
                        LeperGnome,
                        LeperGnome,
                        Loatheb,
                        PilotedShredder,
                        PilotedShredder,
                        GoblinAutoBarber,
                        GoblinAutoBarber,
                        TinkersSharpswordOil,
                        TinkersSharpswordOil,
                    };
                    List<string> basicRogue = new List<string>
                    {
                        ChillwindYeti,
                        RazorfenHunter,
                        BoulderfistOgre,
                        AcidicSwampOoze,
                        DeadlyPoison,
                        Sprint,
                        GnomishInventor,
                        StormwindChampion,
                        SenjinShieldmasta,
                        FanofKnives,
                        AssassinsBlade,
                        ShatteredSunCleric,
                        NoviceEngineer,
                        Backstab,
                        Assassinate,
                        BloodfenRaptor,
                    };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {
                            new Dictionary<DeckType, Style> {{DeckType.RaptorRogue, Style.Tempo}},
                            CurrentDeck.Intersect(raptorRogue).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.PirateRogue, Style.Aggro}},
                            CurrentDeck.Intersect(pirateRogue).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.OilRogue, Style.Combo}},
                            CurrentDeck.Intersect(oilRogue).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.FaceRogue, Style.Face}},
                            CurrentDeck.Intersect(burstRogue).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}},
                            CurrentDeck.Intersect(basicRogue).ToList().Count
                        },
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                #endregion

                #region druid

                case Card.CClass.DRUID:
                    List<string> midRangeDruid = new List<string>
                    {
                        BigGameHunter,
                        ForceofNature,
                        ForceofNature,
                        AzureDrake,
                        AzureDrake,
                        WildGrowth,
                        WildGrowth,
                        SavageRoar,
                        SavageRoar,
                        KeeperoftheGrove,
                        KeeperoftheGrove,
                        Innervate,
                        Innervate,
                        DruidoftheClaw,
                        DruidoftheClaw,
                        Swipe,
                        Swipe,
                        Wrath,
                        Wrath,
                        ShadeofNaxxramas,
                        ShadeofNaxxramas,
                        Loatheb,
                        DrBoom,
                        PilotedShredder,
                        PilotedShredder,
                        EmperorThaurissan,
                        LivingRoots,
                        LivingRoots,
                    };
                    List<string> tokenDruid = new List<string>
                    {
                        SouloftheForest,
                        SouloftheForest,
                        SavageRoar,
                        SavageRoar,
                        KeeperoftheGrove,
                        MarkoftheWild,
                        MarkoftheWild,
                        DefenderofArgus,
                        DefenderofArgus,
                        Innervate,
                        Innervate,
                        AbusiveSergeant,
                        AbusiveSergeant,
                        LivingRoots,
                        LivingRoots,
                        MountedRaptor,
                        MountedRaptor,
                        DragonEgg,
                        DragonEgg,
                        NerubianEgg,
                        NerubianEgg,
                        EchoingOoze,
                        EchoingOoze,
                        Jeeves,
                        Jeeves,
                        HauntedCreeper,
                        HauntedCreeper,
                        SirFinleyMrrgglton,
                    };
                    List<string> rampDruid = new List<string>
                    {
                        BigGameHunter,
                        AncientofWar,
                        AncientofWar,
                        WildGrowth,
                        WildGrowth,
                        KeeperoftheGrove,
                        KeeperoftheGrove,
                        RagnarostheFirelord,
                        Innervate,
                        Innervate,
                        DruidoftheClaw,
                        DruidoftheClaw,
                        Swipe,
                        Swipe,
                        Wrath,
                        Wrath,
                        ZombieChow,
                        ZombieChow,
                        SludgeBelcher,
                        SludgeBelcher,
                        DrBoom,
                        EmperorThaurissan,
                        DruidoftheFlame,
                        DruidoftheFlame,
                        DarnassusAspirant,
                        DarnassusAspirant,
                        MasterJouster,
                        MasterJouster,
                    };
                    List<string> aggroDruid = new List<string>
                    {
                        ForceofNature,
                        ForceofNature,
                        SavageRoar,
                        SavageRoar,
                        KnifeJuggler,
                        KnifeJuggler,
                        KeeperoftheGrove,
                        KeeperoftheGrove,
                        LeperGnome,
                        Innervate,
                        Innervate,
                        DruidoftheClaw,
                        DruidoftheClaw,
                        Swipe,
                        Swipe,
                        Loatheb,
                        DrBoom,
                        PilotedShredder,
                        PilotedShredder,
                        FelReaver,
                        FelReaver,
                        ArgentHorserider,
                        DarnassusAspirant,
                        DarnassusAspirant,
                        DruidoftheSaber,
                        DruidoftheSaber,
                        LivingRoots,
                        LivingRoots,
                        SirFinleyMrrgglton,
                    };
                    List<string> basicDruid = new List<string>
                    {
                        ChillwindYeti,
                        RazorfenHunter,
                        BoulderfistOgre,
                        AcidicSwampOoze,
                        IronbarkProtector,
                        GnomishInventor,
                        WildGrowth,
                        SenjinShieldmasta,
                        ShatteredSunCleric,
                        NoviceEngineer,
                        MarkoftheWild,
                        Claw,
                        Innervate,
                        Swipe,
                        Starfire
                    };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {
                            new Dictionary<DeckType, Style> {{DeckType.MidRangeDruid, Style.Combo}},
                            CurrentDeck.Intersect(midRangeDruid).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.TokenDruid, Style.Aggro}},
                            CurrentDeck.Intersect(tokenDruid).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.RampDruid, Style.Control}},
                            CurrentDeck.Intersect(rampDruid).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.AggroDruid, Style.Face}},
                            CurrentDeck.Intersect(aggroDruid).ToList().Count
                        },
                        {
                            new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}},
                            CurrentDeck.Intersect(basicDruid).ToList().Count
                        },
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                    #endregion
            }
            
            return info;
        }

        private DeckData SetUnknown(DeckData info, Dictionary<Dictionary<DeckType, Style>, int> deckDictionary)
        {
            info.DeckType = DeckType.Unknown;
            info.DeckStyle = Style.Unknown;
            return info;
        }
        private static void CheckDirectory(string subdir)
        {
            if (Directory.Exists(subdir))
                return;
            Directory.CreateDirectory(subdir);
        }

    }

    public class DeckData
    {
        public DeckType DeckType { get; set; }
        public Style DeckStyle { get; set; }
        public List<string> DeckList { get; set; }
        
    }
    

    public enum DeckType
    {
        [Browsable(false)]
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
        [Description("KiblerMage")]
        DragonMage,
        MechMage,
        EchoMage,
        FatigueMage, 
        /*Priest*/
        DragonPriest,
        ControlPriest,
        [Description("Call me Firebat")]
        ComboPriest, 
        MechPriest,
        ShadowPriest,
        /*Huntard*/
        MidRangeHunter,
        HybridHunter,
        FaceHunter, 
        HatHunter,
        /**/
        OilRogue,
        PirateRogue,
        FaceRogue,
        MalyRogue,
        RaptorRogue,
        [Description("Those who hate fun")]
        FatigueRogue,
        /**/
        FaceShaman,
        MechShaman,
        DragonShaman,
        TotemShaman,
        MalygosShaman,
        ControlShaman, 
        BloodlustShaman,
        [Browsable(false)]
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
   
}