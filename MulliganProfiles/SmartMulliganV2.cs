
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartBot.Database;
using SmartBot.Mulligan;
using SmartBot.Plugins.API;


namespace SmartBotUI.SmartMulliganV2
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
    public class bMulliganProfile : MulliganProfile
    {
        /******************************************/
        /**************EDIT THIS LINE ONLY*********/
        /******************************************/
        private const bool TrackMulligan = true;
        private const bool OldMysteriousChallenger = false;
        /*If you chose not to be tracked, I won't be
         *             able to fix mulligan errors*/
        /******************************************/

        /***********************************************************/
        /***********DO NOT EDIT ANYTHING BELOW THIS LINE************/
        /***********************************************************/
        private const int ControlConst = 4;//don't touch me
        private const int TempoConst = 3;//don't touch me
        private const double Face = 0.9;//don't touch me

        #region allcards

        //=====DRUID=====
        private const string Claw = "CS2_005";
        private const string HealingTouch = "CS2_007";
        private const string Moonfire = "CS2_008";
        private const string MarkoftheWild = "CS2_009";
        private const string SavageRoar = "CS2_011";
        private const string Swipe = "CS2_012";
        private const string WildGrowth = "CS2_013";
        private const string Shapeshift = "CS2_017";
        private const string IronbarkProtector = "CS2_232";
        private const string Innervate = "EX1_169";
        private const string Starfire = "EX1_173";
        private const string Wrath = "EX1_154";
        private const string MarkofNature = "EX1_155";
        private const string SouloftheForest = "EX1_158";
        private const string PoweroftheWild = "EX1_160";
        private const string Naturalize = "EX1_161";
        private const string Nourish = "EX1_164";
        private const string DruidoftheClaw = "EX1_165";
        private const string KeeperoftheGrove = "EX1_166";
        private const string AncientofWar = "EX1_178";
        private const string Bite = "EX1_570";
        private const string ForceofNature = "EX1_571";
        private const string Cenarius = "EX1_573";
        private const string Savagery = "EX1_578";
        private const string Treant = "EX1_tk9";
        private const string Starfall = "NEW1_007";
        private const string AncientofLore = "NEW1_008";
        private const string PoisonSeeds = "FP1_019";
        private const string AnodizedRoboCub = "GVG_030";
        private const string Recycle = "GVG_031";
        private const string GroveTender = "GVG_032";
        private const string TreeofLife = "GVG_033";
        private const string MechBearCat = "GVG_034";
        private const string Malorne = "GVG_035";
        private const string DarkWispers = "GVG_041";
        private const string DruidoftheFang = "GVG_080";
        private const string VolcanicLumberer = "BRM_009";
        private const string DruidoftheFlame = "BRM_010";
        private const string LivingRoots = "AT_037";
        private const string DarnassusAspirant = "AT_038";
        private const string SavageCombatant = "AT_039";
        private const string Wildwalker = "AT_040";
        private const string KnightoftheWild = "AT_041";
        private const string DruidoftheSaber = "AT_042";
        private const string AstralCommunion = "AT_043";
        private const string Mulch = "AT_044";
        private const string Aviana = "AT_045";
        //=====ROGUE=====
        private const string Backstab = "CS2_072";
        private const string DeadlyPoison = "CS2_074";
        private const string SinisterStrike = "CS2_075";
        private const string Assassinate = "CS2_076";
        private const string Sprint = "CS2_077";
        private const string AssassinsBlade = "CS2_080";
        private const string WickedKnife = "CS2_082";
        private const string FanofKnives = "EX1_129";
        private const string Shiv = "EX1_278";
        private const string Sap = "EX1_581";
        private const string ValeeraSanguinar = "HERO_03";
        private const string Vanish = "NEW1_004";
        private const string ColdBlood = "CS2_073";
        private const string BladeFlurry = "CS2_233";
        private const string Eviscerate = "EX1_124";
        private const string Betrayal = "EX1_126";
        private const string Conceal = "EX1_128";
        private const string DefiasRingleader = "EX1_131";
        private const string PerditionsBlade = "EX1_133";
        private const string SI7Agent = "EX1_134";
        private const string Headcrack = "EX1_137";
        private const string Shadowstep = "EX1_144";
        private const string Preparation = "EX1_145";
        private const string PatientAssassin = "EX1_522";
        private const string EdwinVanCleef = "EX1_613";
        private const string Kidnapper = "NEW1_005";
        private const string MasterofDisguise = "NEW1_014";
        private const string AnubarAmbusher = "FP1_026";
        private const string TinkersSharpswordOil = "GVG_022";
        private const string GoblinAutoBarber = "GVG_023";
        private const string CogmastersWrench = "GVG_024";
        private const string OneeyedCheat = "GVG_025";
        private const string IronSensei = "GVG_027";
        private const string TradePrinceGallywix = "GVG_028";
        private const string Sabotage = "GVG_047";
        private const string OgreNinja = "GVG_088";
        private const string GangUp = "BRM_007";
        private const string DarkIronSkulker = "BRM_008";
        private const string ShadoPanRider = "AT_028";
        private const string Buccaneer = "AT_029";
        private const string UndercityValiant = "AT_030";
        private const string Cutpurse = "AT_031";
        private const string ShadyDealer = "AT_032";
        private const string Burgle = "AT_033";
        private const string PoisonedBlade = "AT_034";
        private const string BeneaththeGrounds = "AT_035";
        private const string Anubarak = "AT_036";
        //=====SHAMAN=====
        private const string FrostShock = "CS2_037";
        private const string Windfury = "CS2_039";
        private const string AncestralHealing = "CS2_041";
        private const string FireElemental = "CS2_042";
        private const string RockbiterWeapon = "CS2_045";
        private const string Bloodlust = "CS2_046";
        private const string TotemicCall = "CS2_049";
        private const string SearingTotem = "CS2_050";
        private const string StoneclawTotem = "CS2_051";
        private const string WrathofAirTotem = "CS2_052";
        private const string TotemicMight = "EX1_244";
        private const string Hex = "EX1_246";
        private const string FlametongueTotem = "EX1_565";
        private const string Windspeaker = "EX1_587";
        private const string Thrall = "HERO_02";
        private const string HealingTotem = "NEW1_009";
        private const string AncestralSpirit = "CS2_038";
        private const string FarSight = "CS2_053";
        private const string LightningBolt = "EX1_238";
        private const string LavaBurst = "EX1_241";
        private const string DustDevil = "EX1_243";
        private const string EarthShock = "EX1_245";
        private const string StormforgedAxe = "EX1_247";
        private const string FeralSpirit = "EX1_248";
        private const string EarthElemental = "EX1_250";
        private const string ForkedLightning = "EX1_251";
        private const string UnboundElemental = "EX1_258";
        private const string LightningStorm = "EX1_259";
        private const string Doomhammer = "EX1_567";
        private const string ManaTideTotem = "EX1_575";
        private const string SpiritWolf = "EX1_tk11";
        private const string AlAkirtheWindlord = "NEW1_010";
        private const string Reincarnate = "FP1_025";
        private const string AncestorsCall = "GVG_029";
        private const string Powermace = "GVG_036";
        private const string WhirlingZapomatic = "GVG_037";
        private const string Crackle = "GVG_038";
        private const string VitalityTotem = "GVG_039";
        private const string SiltfinSpiritwalker = "GVG_040";
        private const string Neptulon = "GVG_042";
        private const string DunemaulShaman = "GVG_066";
        private const string LavaShock = "BRM_011";
        private const string FireguardDestroyer = "BRM_012";
        private const string TuskarrTotemic = "AT_046";
        private const string DraeneiTotemcarver = "AT_047";
        private const string HealingWave = "AT_048";
        private const string ThunderBluffValiant = "AT_049";
        private const string ChargedHammer = "AT_050";
        private const string ElementalDestruction = "AT_051";
        private const string TotemGolem = "AT_052";
        private const string AncestralKnowledge = "AT_053";
        private const string TheMistcaller = "AT_054";
        //=====Mage=====
        private const string Polymorph = "CS2_022";
        private const string ArcaneIntellect = "CS2_023";
        private const string Frostbolt = "CS2_024";
        private const string ArcaneExplosion = "CS2_025";
        private const string FrostNova = "CS2_026";
        private const string MirrorImage = "CS2_027";
        private const string Fireball = "CS2_029";
        private const string Flamestrike = "CS2_032";
        private const string WaterElemental = "CS2_033";
        private const string Fireblast = "CS2_034";
        private const string ArcaneMissiles = "EX1_277";
        private const string JainaProudmoore = "HERO_08";
        private const string Blizzard = "CS2_028";
        private const string IceLance = "CS2_031";
        private const string EtherealArcanist = "EX1_274";
        private const string ConeofCold = "EX1_275";
        private const string Pyroblast = "EX1_279";
        private const string Counterspell = "EX1_287";
        private const string IceBarrier = "EX1_289";
        private const string MirrorEntity = "EX1_294";
        private const string IceBlock = "EX1_295";
        private const string ArchmageAntonidas = "EX1_559";
        private const string Vaporize = "EX1_594";
        private const string SorcerersApprentice = "EX1_608";
        private const string KirinTorMage = "EX1_612";
        private const string ManaWyrm = "NEW1_012";
        private const string Spellbender = "tt_010";
        private const string Duplicate = "FP1_018";
        private const string Flamecannon = "GVG_001";
        private const string Snowchugger = "GVG_002";
        private const string UnstablePortal = "GVG_003";
        private const string GoblinBlastmage = "GVG_004";
        private const string EchoofMedivh = "GVG_005";
        private const string FlameLeviathan = "GVG_007";
        private const string WeeSpellstopper = "GVG_122";
        private const string SootSpewer = "GVG_123";
        private const string Flamewaker = "BRM_002";
        private const string DragonsBreath = "BRM_003";
        private const string FlameLance = "AT_001";
        private const string Effigy = "AT_002";
        private const string FallenHero = "AT_003";
        private const string ArcaneBlast = "AT_004";
        private const string PolymorphBoar = "AT_005";
        private const string DalaranAspirant = "AT_006";
        private const string Spellslinger = "AT_007";
        private const string ColdarraDrake = "AT_008";
        private const string Rhonin = "AT_009";
        //=====HUNTER=====
        private const string HuntersMark = "CS2_084";
        private const string StarvingBuzzard = "CS2_237";
        private const string SteadyShot = "DS1h_292";
        private const string Houndmaster = "DS1_070";
        private const string TimberWolf = "DS1_175";
        private const string TundraRhino = "DS1_178";
        private const string MultiShot = "DS1_183";
        private const string Tracking = "DS1_184";
        private const string ArcaneShot = "DS1_185";
        private const string KillCommand = "EX1_539";
        private const string Rexxar = "HERO_05";
        private const string AnimalCompanion = "NEW1_031";
        private const string Misha = "NEW1_032";
        private const string Leokk = "NEW1_033";
        private const string Huffer = "NEW1_034";
        private const string GladiatorsLongbow = "DS1_188";
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
        private const string Snipe = "EX1_609";
        private const string ExplosiveTrap = "EX1_610";
        private const string FreezingTrap = "EX1_611";
        private const string DeadlyShot = "EX1_617";
        private const string Webspinner = "FP1_011";
        private const string CallPet = "GVG_017";
        private const string FeignDeath = "GVG_026";
        private const string Glaivezooka = "GVG_043";
        private const string KingofBeasts = "GVG_046";
        private const string MetaltoothLeaper = "GVG_048";
        private const string Gahzrilla = "GVG_049";
        private const string CobraShot = "GVG_073";
        private const string SteamwheedleSniper = "GVG_087";
        private const string QuickShot = "BRM_013";
        private const string CoreRager = "BRM_014";
        private const string RamWrangler = "AT_010";
        private const string Powershot = "AT_056";
        private const string Stablemaster = "AT_057";
        private const string KingsElekk = "AT_058";
        private const string BraveArcher = "AT_059";
        private const string BearTrap = "AT_060";
        private const string LockandLoad = "AT_061";
        private const string BallofSpiders = "AT_062";
        private const string Acidmaw = "AT_063";
        //=====PRIEST=====
        private const string LesserHeal = "CS1h_001";
        private const string HolyNova = "CS1_112";
        private const string MindControl = "CS1_113";
        private const string HolySmite = "CS1_130";
        private const string MindVision = "CS2_003";
        private const string PowerWordShield = "CS2_004";
        private const string ShadowWordPain = "CS2_234";
        private const string NorthshireCleric = "CS2_235";
        private const string DivineSpirit = "CS2_236";
        private const string MindBlast = "DS1_233";
        private const string ShadowWordDeath = "EX1_622";
        private const string AnduinWrynn = "HERO_09";
        private const string InnerFire = "CS1_129";
        private const string CabalShadowPriest = "EX1_091";
        private const string Silence = "EX1_332";
        private const string ShadowMadness = "EX1_334";
        private const string Lightspawn = "EX1_335";
        private const string Thoughtsteal = "EX1_339";
        private const string Lightwell = "EX1_341";
        private const string Mindgames = "EX1_345";
        private const string ProphetVelen = "EX1_350";
        private const string AuchenaiSoulpriest = "EX1_591";
        private const string CircleofHealing = "EX1_621";
        private const string TempleEnforcer = "EX1_623";
        private const string HolyFire = "EX1_624";
        private const string Shadowform = "EX1_625";
        private const string MassDispel = "EX1_626";
        private const string DarkCultist = "FP1_023";
        private const string Lightbomb = "GVG_008";
        private const string Shadowbomber = "GVG_009";
        private const string VelensChosen = "GVG_010";
        private const string Shrinkmeister = "GVG_011";
        private const string LightoftheNaaru = "GVG_012";
        private const string Voljin = "GVG_014";
        private const string Shadowboxer = "GVG_072";
        private const string UpgradedRepairBot = "GVG_083";
        private const string TwilightWhelp = "BRM_004";
        private const string Resurrect = "BRM_017";
        private const string HolyChampion = "AT_011";
        private const string SpawnofShadows = "AT_012";
        private const string PowerWordGlory = "AT_013";
        private const string Shadowfiend = "AT_014";
        private const string Convert = "AT_015";
        private const string Confuse = "AT_016";
        private const string ConfessorPaletress = "AT_018";
        private const string FlashHeal = "AT_055";
        private const string WyrmrestAgent = "AT_116";
        //=====WARRIOR=====
        private const string ArmorUp = "CS2_102";
        private const string Charge = "CS2_103";
        private const string HeroicStrike = "CS2_105";
        private const string FieryWarAxe = "CS2_106";
        private const string Execute = "CS2_108";
        private const string ArcaniteReaper = "CS2_112";
        private const string Cleave = "CS2_114";
        private const string WarsongCommander = "EX1_084";
        private const string Whirlwind = "EX1_400";
        private const string ShieldBlock = "EX1_606";
        private const string GarroshHellscream = "HERO_01";
        private const string KorkronElite = "NEW1_011";
        private const string Rampage = "CS2_104";
        private const string Slam = "EX1_391";
        private const string BattleRage = "EX1_392";
        private const string ArathiWeaponsmith = "EX1_398";
        private const string Armorsmith = "EX1_402";
        private const string Brawl = "EX1_407";
        private const string MortalStrike = "EX1_408";
        private const string Upgrade = "EX1_409";
        private const string ShieldSlam = "EX1_410";
        private const string Gorehowl = "EX1_411";
        private const string GrommashHellscream = "EX1_414";
        private const string CruelTaskmaster = "EX1_603";
        private const string FrothingBerserker = "EX1_604";
        private const string InnerRage = "EX1_607";
        private const string CommandingShout = "NEW1_036";
        private const string DeathsBite = "FP1_021";
        private const string BouncingBlade = "GVG_050";
        private const string Warbot = "GVG_051";
        private const string Crush = "GVG_052";
        private const string Shieldmaiden = "GVG_053";
        private const string OgreWarmaul = "GVG_054";
        private const string ScrewjankClunker = "GVG_055";
        private const string IronJuggernaut = "GVG_056";
        private const string SiegeEngine = "GVG_086";
        private const string Revenge = "BRM_015";
        private const string AxeFlinger = "BRM_016";
        private const string Bash = "AT_064";
        private const string KingsDefender = "AT_065";
        private const string OrgrimmarAspirant = "AT_066";
        private const string MagnataurAlpha = "AT_067";
        private const string Bolster = "AT_068";
        private const string SparringPartner = "AT_069";
        private const string AlexstraszasChampion = "AT_071";
        private const string VarianWrynn = "AT_072";
        private const string SeaReaver = "AT_130";
        //=====WARLOCK=====
        private const string LifeTap = "CS2_056";
        private const string ShadowBolt = "CS2_057";
        private const string DrainLife = "CS2_061";
        private const string Hellfire = "CS2_062";
        private const string Corruption = "CS2_063";
        private const string DreadInfernal = "CS2_064";
        private const string Voidwalker = "CS2_065";
        private const string MortalCoil = "EX1_302";
        private const string Succubus = "EX1_306";
        private const string Soulfire = "EX1_308";
        private const string Guldan = "HERO_07";
        private const string SacrificialPact = "NEW1_003";
        private const string BloodImp = "CS2_059";
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
        private const string BloodFury = "EX1_323w";
        private const string Demonfire = "EX1_596";
        private const string INFERNO = "EX1_tk33";
        private const string Infernal = "EX1_tk34";
        private const string Voidcaller = "FP1_022";
        private const string Darkbomb = "GVG_015";
        private const string MistressofPain = "GVG_018";
        private const string Demonheart = "GVG_019";
        private const string FelCannon = "GVG_020";
        private const string MalGanis = "GVG_021";
        private const string Implosion = "GVG_045";
        private const string AnimaGolem = "GVG_077";
        private const string FloatingWatcher = "GVG_100";
        private const string Demonwrath = "BRM_005";
        private const string ImpGangBoss = "BRM_006";
        private const string Dreadsteed = "AT_019";
        private const string FearsomeDoomguard = "AT_020";
        private const string TinyKnightofEvil = "AT_021";
        private const string FistofJaraxxus = "AT_022";
        private const string VoidCrusher = "AT_023";
        private const string Demonfuse = "AT_024";
        private const string DarkBargain = "AT_025";
        private const string Wrathguard = "AT_026";
        private const string WilfredFizzlebang = "AT_027";
        //=====PALADIN=====
        private const string BlessingofMight = "CS2_087";
        private const string GuardianofKings = "CS2_088";
        private const string HolyLight = "CS2_089";
        private const string LightsJustice = "CS2_091";
        private const string BlessingofKings = "CS2_092";
        private const string Consecration = "CS2_093";
        private const string HammerofWrath = "CS2_094";
        private const string TruesilverChampion = "CS2_097";
        private const string Reinforce = "CS2_101";
        private const string Humility = "EX1_360";
        private const string HandofProtection = "EX1_371";
        private const string UtherLightbringer = "HERO_04";
        private const string NobleSacrifice = "EX1_130";
        private const string EyeforanEye = "EX1_132";
        private const string Redemption = "EX1_136";
        private const string DivineFavor = "EX1_349";
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
        private const string Equality = "EX1_619";
        private const string Avenge = "FP1_020";
        private const string SealofLight = "GVG_057";
        private const string ShieldedMinibot = "GVG_058";
        private const string Coghammer = "GVG_059";
        private const string Quartermaster = "GVG_060";
        private const string MusterforBattle = "GVG_061";
        private const string CobaltGuardian = "GVG_062";
        private const string BolvarFordragon = "GVG_063";
        private const string ScarletPurifier = "GVG_101";
        private const string SolemnVigil = "BRM_001";
        private const string DragonConsort = "BRM_018";
        private const string CompetitiveSpirit = "AT_073";
        private const string SealofChampions = "AT_074";
        private const string WarhorseTrainer = "AT_075";
        private const string MurlocKnight = "AT_076";
        private const string ArgentLance = "AT_077";
        private const string EntertheColiseum = "AT_078";
        private const string MysteriousChallenger = "AT_079";
        private const string EadricthePure = "AT_081";
        private const string TuskarrJouster = "AT_104";
        //=====NEUTRAL=====
        private const string GoldshireFootman = "CS1_042";
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
        private const string Boar = "CS2_boar";
        private const string Sheep = "CS2_tk1";
        private const string DarkscaleHealer = "DS1_055";
        private const string VoodooDoctor = "EX1_011";
        private const string NoviceEngineer = "EX1_015";
        private const string ShatteredSunCleric = "EX1_019";
        private const string DragonlingMechanic = "EX1_025";
        private const string AcidicSwampOoze = "EX1_066";
        private const string GurubashiBerserker = "EX1_399";
        private const string MurlocTidehunter = "EX1_506";
        private const string GrimscaleOracle = "EX1_508";
        private const string DalaranMage = "EX1_582";
        private const string Nightblade = "EX1_593";
        private const string AvataroftheCoin = "GAME_002";
        private const string Frog = "hexfrog";
        private const string Skeleton = "skele11";
        private const string FenCreeper = "CS1_069";
        private const string EarthenRingFarseer = "CS2_117";
        private const string SouthseaDeckhand = "CS2_146";
        private const string SilverHandKnight = "CS2_151";
        private const string Squire = "CS2_152";
        private const string RavenholdtAssassin = "CS2_161";
        private const string YoungDragonhawk = "CS2_169";
        private const string InjuredBlademaster = "CS2_181";
        private const string AbusiveSergeant = "CS2_188";
        private const string IronbeakOwl = "CS2_203";
        private const string SpitefulSmith = "CS2_221";
        private const string VentureCoMercenary = "CS2_227";
        private const string Wisp = "CS2_231";
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
        private const string DireWolfAlpha = "EX1_162";
        private const string EmperorCobra = "EX1_170";
        private const string BaronGeddon = "EX1_249";
        private const string FrostElemental = "EX1_283";
        private const string AzureDrake = "EX1_284";
        private const string RagnarostheFirelord = "EX1_298";
        private const string TaurenWarrior = "EX1_390";
        private const string AmaniBerserker = "EX1_393";
        private const string MogushanWarden = "EX1_396";
        private const string Shieldbearer = "EX1_405";
        private const string RagingWorgen = "EX1_412";
        private const string MurlocWarleader = "EX1_507";
        private const string MurlocTidecaller = "EX1_509";
        private const string HarvestGolem = "EX1_556";
        private const string NatPagle = "EX1_557";
        private const string HarrisonJones = "EX1_558";
        private const string Nozdormu = "EX1_560";
        private const string Alexstrasza = "EX1_561";
        private const string Onyxia = "EX1_562";
        private const string Malygos = "EX1_563";
        private const string FacelessManipulator = "EX1_564";
        private const string Ysera = "EX1_572";
        private const string TheBeast = "EX1_577";
        private const string PriestessofElune = "EX1_583";
        private const string AncientMage = "EX1_584";
        private const string SeaGiant = "EX1_586";
        private const string BloodKnight = "EX1_590";
        private const string CultMaster = "EX1_595";
        private const string ImpMaster = "EX1_597";
        private const string Imp = "EX1_598";
        private const string IllidanStormrage = "EX1_614";
        private const string ManaWraith = "EX1_616";
        private const string MoltenGiant = "EX1_620";
        private const string Squirrel = "EX1_tk28";
        private const string Devilsaur = "EX1_tk29";
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
        private const string MasterSwordsmith = "NEW1_037";
        private const string Gruul = "NEW1_038";
        private const string Hogger = "NEW1_040";
        private const string StampedingKodo = "NEW1_041";
        private const string DamagedGolem = "skele21";
        private const string FlesheatingGhoul = "tt_004";
        private const string OpentheGates = "BRMC_83";
        private const string DragonkinSpellcaster = "BRMC_84";
        private const string Lucifron = "BRMC_85";
        private const string Atramedes = "BRMC_86";
        private const string MoiraBronzebeard = "BRMC_87";
        private const string DrakonidSlayer = "BRMC_88";
        private const string WhirlingAsh = "BRMC_89";
        private const string LivingLava = "BRMC_90";
        private const string SonoftheFlame = "BRMC_91";
        private const string CorenDirebrew = "BRMC_92";
        private const string OmnotronDefenseSystem = "BRMC_93";
        private const string Sulfuras = "BRMC_94";
        private const string Golemagg = "BRMC_95";
        private const string HighJusticeGrimstone = "BRMC_96";
        private const string Vaelastrasz = "BRMC_97";
        private const string Razorgore = "BRMC_98";
        private const string Garr = "BRMC_99";
        private const string OldMurkEye = "EX1_062";
        private const string CaptainsParrot = "NEW1_016";
        private const string GelbinMekkatorque = "EX1_112";
        private const string ZombieChow = "FP1_001";
        private const string HauntedCreeper = "FP1_002";
        private const string EchoingOoze = "FP1_003";
        private const string MadScientist = "FP1_004";
        private const string ShadeofNaxxramas = "FP1_005";
        private const string Deathcharger = "FP1_006";
        private const string NerubianEgg = "FP1_007";
        private const string SpectralKnight = "FP1_008";
        private const string Deathlord = "FP1_009";
        private const string Maexxna = "FP1_010";
        private const string SludgeBelcher = "FP1_012";
        private const string KelThuzad = "FP1_013";
        private const string Stalagg = "FP1_014";
        private const string Feugen = "FP1_015";
        private const string WailingSoul = "FP1_016";
        private const string NerubarWeblord = "FP1_017";
        private const string UnstableGhoul = "FP1_024";
        private const string StoneskinGargoyle = "FP1_027";
        private const string Undertaker = "FP1_028";
        private const string DancingSwords = "FP1_029";
        private const string Loatheb = "FP1_030";
        private const string BaronRivendare = "FP1_031";
        private const string Mechwarper = "GVG_006";
        private const string Cogmaster = "GVG_013";
        private const string FelReaver = "GVG_016";
        private const string SpiderTank = "GVG_044";
        private const string Puddlestomper = "GVG_064";
        private const string OgreBrute = "GVG_065";
        private const string StonesplinterTrogg = "GVG_067";
        private const string BurlyRockjawTrogg = "GVG_068";
        private const string AntiqueHealbot = "GVG_069";
        private const string SaltyDog = "GVG_070";
        private const string LostTallstrider = "GVG_071";
        private const string KezanMystic = "GVG_074";
        private const string ShipsCannon = "GVG_075";
        private const string ExplosiveSheep = "GVG_076";
        private const string MechanicalYeti = "GVG_078";
        private const string ForceTankMAX = "GVG_079";
        private const string GilblinStalker = "GVG_081";
        private const string ClockworkGnome = "GVG_082";
        private const string FlyingMachine = "GVG_084";
        private const string AnnoyoTron = "GVG_085";
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
        private const string Flameheart = "BRMA_01";
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
        private const string TwilightGuardian = "AT_017";
        private const string SkycapnKragg = "AT_070";
        private const string GarrisonCommander = "AT_080";
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
        private const string EydisDarkbane = "AT_131";
        private const string JusticarTrueheart = "AT_132";
        private const string GadgetzanJouster = "AT_133";

        #endregion
        #region LoE Cards
        private const string ForgottenTorch = "LOE_002";
        private const string EtherealConjurer = "LOE_003";
        private const string MuseumCurator = "LOE_006";
        private const string CurseofRafaam = "LOE_007";
        private const string EyeofHakkar = "LOE_008";
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
        private const string Entomb = "LOE_104";
        private const string ExplorerHat = "LOE_105";
        private const string EerieStatue = "LOE_107";
        private const string AncientShade = "LOE_110";
        private const string ExcavatedEvil = "LOE_111";
        private const string EveryfinisAwesome = "LOE_113";
        private const string RavenIdol = "LOE_115";
        private const string ReliquarySeeker = "LOE_116";
        #endregion

        private static bool _hasCoin;
        public static int Allowed1Drops = 1;
        public static int Allowed2Drops = _hasCoin ? 3 : 2;
        public static int Allowed3Drops = _hasCoin ? 2 : 1;
        public static int Allowed4Drops = 1;
        //Number of drops in choices
        public static int Num1Drops { get; private set; }
        public static int Num2Drops { get; private set; }
        public static int Num3Drops { get; private set; }
        public static int Num4Drops { get; private set; }
        //Number of drops in deck
        public static int Num1DropsDeck { get; private set; }
        public static int Num2DropsDeck { get; private set; }
        public static int Num3DropsDeck { get; private set; }
        public static int Num4DropsDeck { get; private set; }
        //Number of Races in a deck
        public static int NumPirates { get; private set; }
        public static int NumTotems { get; private set; }
        public static int NumMurlocs { get; private set; }
        public static int NumDemons { get; private set; }
        public static int NumBeasts { get; private set; }
        public static int NumDragons { get; private set; }
        public static int NumMechs { get; private set; }
        //[Not used as of now]
        public static int NumLegendaryCards { get; private set; }
        public static int NumEpicCards { get; private set; }
        public static int NumRareCards { get; private set; }
        public static int NumCommonCards { get; private set; }
        public static int NumFreeCards { get; private set; }
        //TODO Ancor
        public static double AverageCost { get; set; }
        public static double EarlyCardsWight { get; set; }
        public static int NumMinions { get; set; }
        public static int NumWeapons { get; set; }
        public static int NumSpells { get; set; }
        //has in choices
        private static bool _has1Drop;
        private static bool _has2Drop;
        private static bool _has3Drop;
        private static bool _has4Drop;
        private static bool _hasWeapon;

        public static List<string> CurrentDeck;
        private const string Coin = "GAME_005";

        private readonly Dictionary<string, bool> _whiteList; // CardName, KeepDouble
        private readonly List<Card.Cards> _cardsToKeep;
        private static List<Card.Cards> _ctk; //static cards to keep
        public static readonly string MAIN_DIR = AppDomain.CurrentDomain.BaseDirectory + "\\MulliganProfiles\\";

        private static Card.CClass _oc; //opponent class
        private static Card.CClass _ownC; //own class
        private static bool _aggro; //aggro or passive opponent
        private static bool _wc; //weapon class

        public bMulliganProfile()
        {
            _whiteList = new Dictionary<string, bool>();
            _cardsToKeep = new List<Card.Cards>();
        }

        public List<Card.Cards> HandleMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {

            DefaultIni(opponentClass, ownClass);

            //CurrentDeck = new List<string> { "CS2_046", "CS2_046", "EX1_587", "EX1_565", "EX1_565", "CS2_042", "CS2_042", "EX1_005", "CS2_045", "CS2_045", "CS2_196", "CS2_196", "CS2_147", "CS2_147", "CS2_226", "CS2_226", "EX1_025", "EX1_025", "EX1_019", "EX1_019", "CS2_171", "CS2_171", "CS2_222", "CS2_222", "EX1_246", "EX1_246", "EX1_506", "EX1_506", "CS2_122", "CS2_122" };
            CurrentDeck = Bot.CurrentDeck().Cards.ToList();
            _hasCoin = choices.Count > 3;
            var myInfo = GetDeckInfo(ownClass);
            //TODO quickjump
            //myInfo.DeckStyle = Style.Tempo;
            //myInfo.DeckType = DeckType.TempoMage;

            CheckDirectory("MulliganArchives", "SmartMulligan_debug");
            var supported = true;
            //myInfo.DeckStyle = GetStyle();
            ArchiveDeck(myInfo.DeckType);
            switch (myInfo.DeckType)
            {
                case DeckType.Unknown:
                    myInfo.DeckStyle = GetStyle();
                    HandleMinions(choices, _whiteList, opponentClass, ownClass, 0, null, null, myInfo.DeckStyle, myInfo.DeckType);
                    HandleWeapons(choices, ownClass, _whiteList);
                    HandleSpells(choices, _whiteList);
                    break;
                case DeckType.PatronWarrior:
                    HandlePatronMulligan(choices, opponentClass);
                    break;
                case DeckType.ControlWarrior:
                    HandleControlWarrior(choices, opponentClass);
                    break;
                case DeckType.SecretPaladin:
                    if (OldMysteriousChallenger)
                    {
                        Bot.Log("[SmartMulligan] You are running old and greedy MC logic, to change to improved version change line 29 to false");
                        Bot.Log("[SmartMulligan] See forum for more details regarding their differences");
                        HandleSecretPaladin(choices, opponentClass);
                    }
                    else
                    {
                        Bot.Log("[SmartMulligan] You are running improved MC logic. If for some reason you wish to return back to previous logic, open this mulligan with notepad and change line 29 to true");
                        Bot.Log("[SmartMulligan] See forum for more details regarding their differences");
                        HandleSecretPaladin(choices, myInfo.DeckStyle);
                    }
                    
                    break;
                case DeckType.MidRangeDruid:
                    HandleDruid(choices, opponentClass, myInfo.DeckStyle);
                    break;
                case DeckType.TokenDruid:
                    HandleDruid(choices, opponentClass, myInfo.DeckStyle);
                    break;
                case DeckType.DemonHandlock:
                    HandleHandlock(choices, opponentClass);
                    break;
                case DeckType.DragonHandlock:
                    HandleDragons(choices, opponentClass, DragonDeck.DragonWarlock);
                    break;
                case DeckType.TempoMage:
                    HandleMage(choices, myInfo);
                    break;
                case DeckType.DragonPriest:
                    HandleDragons(choices, opponentClass, DragonDeck.DragonPriest);
                    break;
                case DeckType.FreezeMage:
                    HandleFreezeMage(choices, opponentClass);
                    break;
                case DeckType.MidRangePaladin:
                    HandlePaladin(choices, opponentClass, myInfo.DeckStyle);
                    break;
                case DeckType.ControlPriest:
                    supported = false;
                    break;
                case DeckType.DemonZooWarlock:
                    HandleZoo(choices, _oc, myInfo);
                    break;
                case DeckType.MidRangeHunter:
                    HandleHunter(choices, opponentClass, myInfo.DeckStyle);
                    break;
                case DeckType.MechMage:
                    HandleMechMage(choices, opponentClass);
                    break;
                case DeckType.Handlock:
                    HandleHandlock(choices, opponentClass);
                    break;
                case DeckType.Zoolock:
                    HandleZoo(choices, _oc, myInfo);
                    break;
                    //test
                case DeckType.HybridHunter:
                    HandleHunter(choices, opponentClass, myInfo.DeckStyle);
                    break;
                case DeckType.EchoMage:
                    HandleEchoMage(choices, opponentClass);
                    break;
                case DeckType.AggroPaladin:
                    HandlePaladin(choices, opponentClass, myInfo.DeckStyle);
                    break;
                case DeckType.DragonWarrior:
                    HandleDragons(choices, opponentClass, DragonDeck.DragonWarrior);
                    break;
                case DeckType.FaceHunter:
                    HandleHunter(choices, opponentClass, myInfo.DeckStyle);
                    break;
                case DeckType.OilRogue:
                    HandleOilRogues(choices, opponentClass);
                    break;
                case DeckType.MechShaman:
                    supported = false;
                    break;
                case DeckType.DragonShaman:
                    supported = false;
                    break;
                case DeckType.TotemShaman:
                    HandleShaman(choices, opponentClass, myInfo.DeckType);
                    break;
                case DeckType.MalygosShaman:
                    supported = false;
                    break;
                case DeckType.ControlShaman:
                    supported = false;
                    break;
                case DeckType.MechWarrior:
                    HandleMechWarrior(choices, opponentClass);
                    break;
                case DeckType.RampDruid:
                    HandleDruid(choices, opponentClass, myInfo.DeckStyle);
                    break;
                case DeckType.PirateRogue:
                    supported = false;
                    break;
                case DeckType.BurstRogue:
                    HandleBurstRogue(choices, opponentClass);
                    break;
                case DeckType.AggroDruid:
                    HandleDruid(choices, opponentClass, myInfo.DeckStyle);
                    break;
                case DeckType.ComboPriest:
                    supported = false;
                    break;
                case DeckType.Arena:
                    HandleMinions(choices, _whiteList, opponentClass, ownClass, 0);
                    HandleWeapons(choices, ownClass, _whiteList);
                    HandleSpells(choices, _whiteList);
                    break;
                case DeckType.FaceWarrior:
                    HandleFaceWarrior(choices, opponentClass);
                    break;
                case DeckType.FatigueWarrior:
                    HandleControlWarrior(choices, opponentClass, myInfo.DeckType);
                    break;
                case DeckType.RelinquaryZoo:
                    HandleZoo(choices, _oc, myInfo);
                    break;
                case DeckType.FaceShaman:
                    supported = false;
                    break;
                case DeckType.RaptorRogue:
                    //supported = false;
                    HandleRaptorRogue(choices, _oc, myInfo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (myInfo.DeckType == DeckType.Unknown)
            {
                Bot.Log(
                    string.Format(
                        "[SmartMulligan] Current deck is {0}.  " +
                        "However, my analysis shows that you are running {1} deck, so mulligan will treat it as normal {2} {3} deck",
                        myInfo.DeckType, myInfo.DeckStyle, myInfo.DeckStyle, _ownC.ToString().ToLower()));
            }
            else
            {
                if (!supported)
                {
                    Bot.Log(
                        string.Format(
                            "[SmartMulligan] I haven't extensively tested: {0}.",
                            myInfo.DeckType));
                    Bot.Log(
                        string.Format(
                            "[SmartMulligan] But this mulligan will try to adjust properly with {0} Mulligan logic.",
                            DeckType.Arena));
                    Bot.Log(
                        string.Format(
                            "[SmartMulligan] Mulligan will treat it as {0} style deck ", myInfo.DeckStyle));
                    SetDefaultsForStyle(myInfo.DeckStyle);
                    HandleMinions(choices, _whiteList, opponentClass, ownClass, 0, null, null, myInfo.DeckStyle);
                    HandleWeapons(choices, ownClass, _whiteList);
                    HandleSpells(choices, _whiteList);
                }
                else
                {
                    Bot.Log(string.Format("[SmartMulligan] Recognized {0} deck." +
                                          " If that is not true, please report it to Arthur", myInfo.DeckType));
                }
            }
           
            foreach (var s in from s in choices
                              let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString())
                              where _whiteList.ContainsKey(s.ToString())
                              where !keptOneAlready | _whiteList[s.ToString()]
                              select s)
                _cardsToKeep.Add(s);

            _ctk = _cardsToKeep;
            if (TrackMulligan) DisplayMulligans(choices, opponentClass, ownClass);

            return _cardsToKeep;
        }

        private void HandleMage(List<Card.Cards> choices, DeckData myInfo)
        {
            //nmc is a never mulligan card list. Those cards are always kept with at least 1 copy
            List<string> nmc = new List<string>{ArcaneBlast, ManaWyrm, Frostbolt, UnstablePortal, MadScientist, SorcerersApprentice};
            var hasRem = choices.Any(c => CardTemplate.LoadFromId(c).Type == Card.CType.SPELL && nmc.Any(q=> q.ToString() == c.ToString()));
            var hasMin = choices.Any(c => CardTemplate.LoadFromId(c).Type == Card.CType.MINION && nmc.Any(q => q.ToString() == c.ToString()));
            var oneDropPrio = choices.Any(c => CardTemplate.LoadFromId(c).Id.ToString() == ClockworkGnome) &&
                              choices.Any(c => CardTemplate.LoadFromId(c).Id.ToString() == ManaWyrm);
            foreach (var q in from q in nmc let card = CardTemplate.LoadFromId(q) where card.Type == Card.CType.SPELL select q)
                _whiteList.AddOrUpdate(q, q == UnstablePortal && _hasCoin);
           
            foreach (var q in from q in nmc let card = CardTemplate.LoadFromId(q) where card.Type == Card.CType.MINION select q)
                _whiteList.AddOrUpdate(q, false);
            
            foreach (var q in choices)
            {
                CardTemplate card = CardTemplate.LoadFromId(q);
                if(card.Type == Card.CType.MINION && card.Cost == 1)
                    _whiteList.AddOrUpdate(q.ToString(), _hasCoin);
                if(card.Type == Card.CType.MINION && card.Cost == 2)
                    _whiteList.AddOrUpdate(q.ToString(), _hasCoin && card.HasDeathrattle);
                if(card.Type == Card.CType.MINION && card.Class == Card.CClass.MAGE && card.Cost == 3)
                    _whiteList.AddOrUpdate(!hasMin ? q.ToString(): "", false);
            }
            if (oneDropPrio)
                _whiteList.Remove(ClockworkGnome);
            _whiteList.AddOrUpdate(_oc == Card.CClass.PRIEST && _hasCoin ? Fireball: "", false); //against dragons
            _whiteList.AddOrUpdate(_oc == Card.CClass.WARRIOR || _oc == Card.CClass.PRIEST && !hasRem ? ArcaneMissiles : "", false);
            _whiteList.AddOrUpdate(_aggro || _oc == Card.CClass.WARRIOR|| !hasRem ? ArcaneMissiles : "", false);
            _whiteList.AddOrUpdate(_oc != Card.CClass.ROGUE && _oc != Card.CClass.SHAMAN && _oc != Card.CClass.PALADIN? Flamecannon : "", false);
            _whiteList.AddOrUpdate(_oc == Card.CClass.PALADIN ||_oc == Card.CClass.HUNTER ? ArcaneExplosion : "", false);
        }

        private void HandleRaptorRogue(List<Card.Cards> choices, Card.CClass oc, DeckData myInfo)
        {
            SetDefaultsForStyle(myInfo.DeckStyle);
            List<string> activators = new List<string> { UnearthedRaptor, AbusiveSergeant };
            List<string> needActivation = new List<string> { NerubianEgg };
            HandleMinions(choices, _whiteList, oc, _ownC, 0, activators, needActivation);
            _whiteList.AddOrUpdate(_hasCoin ? UnearthedRaptor : "", false);
        }

        private void HandleZoo(List<Card.Cards> choices, Card.CClass oc, DeckData info)
        {
            SetDefaultsForStyle(info.DeckStyle);
            List<string> activators = new List<string> { PowerOverwhelming, VoidTerror, AbusiveSergeant, DefenderofArgus };
            List<string> needActivation = new List<string> { NerubianEgg };
            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == NerubianEgg) ? PowerOverwhelming : "", false);
            HandleMinions(choices, _whiteList, oc, _ownC, 0, activators, needActivation);
            _whiteList.AddOrUpdate(_has2Drop && _hasCoin && ChoicesIntersectList(choices, needActivation) ? DefenderofArgus : "", false);
        }

        //Necessary Preparations
        private static void DefaultIni(Card.CClass opponentClass, Card.CClass ownClass)
        {
            try
            {
                //TODO resharper ancor
                //Not used, but has potential in future expansion that might utilize quality
                _oc = opponentClass;
                _ownC = ownClass;
                _aggro = _oc == Card.CClass.PALADIN || _oc == Card.CClass.DRUID || _oc == Card.CClass.HUNTER;
                _wc = _oc == Card.CClass.WARRIOR || _oc == Card.CClass.PALADIN || _oc == Card.CClass.HUNTER ||
                      _oc == Card.CClass.ROGUE;
                NumMechs = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Race == Card.CRace.MECH);
                NumDragons = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Race == Card.CRace.DRAGON);
                NumBeasts = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Race == Card.CRace.BEAST);
                NumDemons = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Race == Card.CRace.DEMON);
                NumMurlocs = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Race == Card.CRace.MURLOC);
                NumPirates = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Race == Card.CRace.PIRATE);
                NumTotems = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Race == Card.CRace.TOTEM);
                Num1DropsDeck = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Cost == 1);
                Num2DropsDeck = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Cost == 2);
                Num3DropsDeck = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Cost == 3);
                Num4DropsDeck = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Cost == 4);
                EarlyCardsWight = (double)(Num1DropsDeck + Num2DropsDeck + Num3DropsDeck) / 30;
                NumFreeCards = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Quality == Card.CQuality.Free);
                NumCommonCards = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Quality == Card.CQuality.Common);
                NumRareCards = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Quality == Card.CQuality.Rare);
                NumEpicCards = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Quality == Card.CQuality.Epic);
                NumLegendaryCards = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Quality == Card.CQuality.Legendary);
                NumSpells = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Type == Card.CType.SPELL);
                NumWeapons = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Type == Card.CType.WEAPON);
                NumMinions = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).Type == Card.CType.MINION);
                AverageCost = CurrentDeck.Average(c => CardTemplate.LoadFromId(c).Cost);
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        //In case something wrong happens, it will save files for debug purposes
        private static void ArchiveDeck(DeckType deckType)
        {
            using (
               var file =
                   new StreamWriter(MAIN_DIR + "MulliganArchives/SmartMulligan_debug/" + deckType + ".txt", true)
               )
            {
                foreach (var q in CurrentDeck)
                    file.Write("\"{0}\",", q);
                file.WriteLine(" ");
                foreach (var q in CurrentDeck)
                    file.Write("{0};", q);
                file.WriteLine(" ");
                foreach (var q in CurrentDeck)
                    file.WriteLine("{0} ", CardTemplate.LoadFromId(q).Name);

                file.WriteLine(" ");
                file.WriteLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- ");
            }
        }

        //such lazy work
        private void HandleOilRogues(List<Card.Cards> choices, Card.CClass opponentClass)
        {
            var vMage = new List<string> { BladeFlurry, Backstab, EarthenRingFarseer, Preparation, Eviscerate, SI7Agent, DeadlyPoison }; //mage
            foreach (var q in vMage)
                _whiteList.AddOrUpdate(q, false);
            var vDruid = new List<string> { VioletTeacher, AzureDrake, EarthenRingFarseer, Sap, DeadlyPoison, Eviscerate, SI7Agent }; //druid
            foreach (var q in vDruid)
                _whiteList.AddOrUpdate(q, false);
            var vPaladin = new List<string> { FanofKnives, VioletTeacher, Backstab, SI7Agent, EarthenRingFarseer, DeadlyPoison, BladeFlurry, HarrisonJones }; //paladin
            foreach (var q in vPaladin)
                _whiteList.AddOrUpdate(q, false);
            var vShaman = new List<string> { BladeFlurry, Backstab, SI7Agent, VioletTeacher, DeadlyPoison, AzureDrake, FanofKnives }; //shaman
            foreach (var q in vShaman)
                _whiteList.AddOrUpdate(q, false);
            var vWarlock = new List<string> { BladeFlurry, Backstab, EarthenRingFarseer, Preparation, Eviscerate, SI7Agent, DeadlyPoison }; //warlock
            foreach (var q in vWarlock)
                _whiteList.AddOrUpdate(q, false);
            var vPriest = new List<string> { AzureDrake, VioletTeacher, Sprint, DeadlyPoison, Eviscerate, EarthenRingFarseer, SI7Agent, Sap }; //priest
            foreach (var q in vPriest)
                _whiteList.AddOrUpdate(q, false);
            var vWarrior = new List<string> { VioletTeacher, Sprint, AzureDrake, EarthenRingFarseer, SI7Agent, HarrisonJones }; //warrior
            foreach (var q in vWarrior)
                _whiteList.AddOrUpdate(q, false);
            var vHunter = new List<string> { Backstab, EarthenRingFarseer, SI7Agent, Preparation, Eviscerate, FanofKnives }; //hunter
            foreach (var q in vHunter)
                _whiteList.AddOrUpdate(q, false);
            var vRogue = new List<string> { VioletTeacher, AzureDrake, EarthenRingFarseer, DeadlyPoison, Eviscerate, Sprint, Preparation }; //rogue
            foreach (var q in vRogue)
                _whiteList.AddOrUpdate(q, false);
        }
        //Not to confuse with mech/aggro warrior
        private void HandleFaceWarrior(List<Card.Cards> choices, Card.CClass opponentClass)
        {
            _whiteList.AddOrUpdate(FieryWarAxe, _hasCoin);
            _whiteList.AddOrUpdate(_hasCoin ? DeathsBite : "", false);
            foreach (var q in from q in choices let temp = CardTemplate.LoadFromId(q) where temp.Cost == 1 select q)
            {
                _has1Drop = true;
                _whiteList.AddOrUpdate(q.ToString(), false);
            }
            if (opponentClass == Card.CClass.HUNTER)
                _whiteList.AddOrUpdate(IronbeakOwl, false);
        }
        //Obvious... I hope
        private void HandleControlWarrior(List<Card.Cards> choices, Card.CClass opponentClass, DeckType deckType = DeckType.ControlWarrior)
        {
            if (_aggro && deckType == DeckType.FatigueWarrior)
            {
                _whiteList.AddOrUpdate(Slam, false);
                _whiteList.AddOrUpdate(Deathlord, false);
            }
            foreach (var q in choices)
            {
                var card = CardTemplate.LoadFromId(q);
                if (card.Cost < 5 && card.Type == Card.CType.WEAPON)
                {
                    _hasWeapon = true;
                    _whiteList.AddOrUpdate(q.ToString(), false);
                    switch (card.Cost)
                    {
                        case 2:
                            _has2Drop = true;
                            break;
                        case 4:
                            _has4Drop = true;
                            break;
                    }
                }
                if (card.Cost != 2 || card.Type != Card.CType.MINION) continue;
                _has2Drop = true;
                _whiteList.AddOrUpdate(q.ToString(), false);
            }
            if (_hasCoin && (_has2Drop || _has4Drop))
            {
                _whiteList.AddOrUpdate(ShieldBlock, false);
                _whiteList.AddOrUpdate(ShieldSlam, false);
            }
            _whiteList.AddOrUpdate(_aggro ? Whirlwind : "", false);
            if (opponentClass == Card.CClass.WARLOCK)
            {
                _whiteList.AddOrUpdate(BigGameHunter, false);
                _whiteList.AddOrUpdate(ShieldSlam, false);
                _whiteList.AddOrUpdate(Execute, false);
                _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == Execute) ? CruelTaskmaster : "", false);
            }
            if (_wc)
                _whiteList.AddOrUpdate(_has2Drop || _has4Drop || _hasCoin ? HarrisonJones : "", false);


        }
        //Midrage
        private void HandlePaladin(List<Card.Cards> choices, Card.CClass opponentClass, Style dStyle)
        {
            var gotEarly = false;
            var heavyControl = opponentClass == Card.CClass.WARLOCK || opponentClass == Card.CClass.PRIEST || opponentClass == Card.CClass.SHAMAN || opponentClass == Card.CClass.WARRIOR || opponentClass == Card.CClass.ROGUE;

            if (dStyle == Style.Aggro)
            {
                foreach (var q in choices)
                {
                    var temp = CardTemplate.LoadFromId(q);
                    if (temp.Cost == 1 && choices.Count(e => e.ToString() == temp.Id.ToString()) == 2)
                        _whiteList.AddOrUpdate(q.ToString(), false);
                    if (q.ToString() == ArgentSquire || q.ToString() == LeperGnome)
                    {
                        _whiteList.AddOrUpdate(ArgentSquire, _aggro);
                        _whiteList.AddOrUpdate(LeperGnome, _aggro);

                        gotEarly = true;
                        _has1Drop = true;
                    }
                    if (q.ToString() == ShieldedMinibot || q.ToString() == KnifeJuggler)
                    {
                        _has2Drop = true;
                        _whiteList.AddOrUpdate(ShieldedMinibot, _hasCoin);
                        _whiteList.AddOrUpdate(KnifeJuggler, false);
                    }
                }

                _whiteList.AddOrUpdate(gotEarly ? BlessingofMight : "", false);
                _whiteList.AddOrUpdate(gotEarly ? AbusiveSergeant : "", false);
                _whiteList.AddOrUpdate(_has2Drop ? MusterforBattle : "", false);
                _whiteList.AddOrUpdate(_has2Drop ? KingMukla : "", false);
                _whiteList.AddOrUpdate(_aggro ? IronbeakOwl : "", false);
                _whiteList.AddOrUpdate(_aggro && _hasCoin ? TruesilverChampion : "", false);
                _whiteList.AddOrUpdate(heavyControl ? DivineFavor : "", false);
                return;
            }
            _whiteList.AddOrUpdate(ZombieChow, _aggro);
            foreach (var q in choices)
            {
                if (q.ToString() == ZombieChow)
                {
                    _whiteList.AddOrUpdate(ZombieChow, _aggro);
                    _has1Drop = true;
                }
                if (q.ToString() == ShieldedMinibot || q.ToString() == KnifeJuggler)
                {
                    _has2Drop = true;
                    _whiteList.AddOrUpdate(ShieldedMinibot, _hasCoin);
                    _whiteList.AddOrUpdate(KnifeJuggler, false);
                }
            }
            _whiteList.AddOrUpdate(_has2Drop || _aggro ? MusterforBattle : "", false);
            _whiteList.AddOrUpdate(heavyControl ? PilotedShredder : "", false);
            _whiteList.AddOrUpdate(heavyControl && _hasCoin ? TruesilverChampion : "", false);
            _whiteList.AddOrUpdate(opponentClass == Card.CClass.DRUID || opponentClass == Card.CClass.WARLOCK ? AldorPeacekeeper : "", false);
            _whiteList.AddOrUpdate(opponentClass == Card.CClass.HUNTER || opponentClass == Card.CClass.PALADIN ? Consecration : "", false);
        }
        // [4 in 1] Includes: Dragon Priest, Warlock (Not Malygos), Paladin, Warrior 
        private void HandleDragons(List<Card.Cards> choices, Card.CClass opponentClass, DragonDeck type)
        {
            var needActivator = choices.Any(c => c.ToString() == WyrmrestAgent || c.ToString() == TwilightWhelp);
            var exception = new List<string> { IronbeakOwl, Shrinkmeister, SunfuryProtector, AncientWatcher };
            var hasExcept = choices.Select(q => CardTemplate.LoadFromId(q).Id.ToString())
                               .ToList()
                               .Intersect(exception)
                               .ToList()
                               .Count > 0;
            var activateMe = false;
            foreach (var q in choices)
            {
                var card = CardTemplate.LoadFromId(q);
                if (card.Type == Card.CType.SPELL) continue;
                if (card.Cost == 1)
                {
                    _has1Drop = true;
                    _whiteList.AddOrUpdate(q.ToString(), false);
                }
                if (card.Cost == 2)
                {
                    _has2Drop = true;
                    _whiteList.AddOrUpdate(!hasExcept ? q.ToString() : "", _hasCoin && card.Type != Card.CType.WEAPON);
                    _whiteList.AddOrUpdate(_aggro ? IronbeakOwl : "", false);
                    _whiteList.AddOrUpdate(_aggro ? AncientWatcher : "", false);
                }
                if (!_has2Drop) continue;
                _has3Drop = true;
                activateMe = true;
                _whiteList.AddOrUpdate(BlackwingTechnician, false);
            }

            switch (type)
            {
                case DragonDeck.DragonPaladin:
                    _whiteList.AddOrUpdate(activateMe && choices.Any(c => c.ToString() == TwilightGuardian) ? TwilightGuardian : choices.Any(c => c.ToString() == DragonConsort) ? DragonConsort : "", false);
                    _whiteList.AddOrUpdate(MusterforBattle, false);
                    _whiteList.AddOrUpdate(opponentClass == Card.CClass.DRUID ? AldorPeacekeeper : "", false);
                    _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == LightsJustice) ? LightsJustice : choices.Any(c => c.ToString() == Coghammer) ? Coghammer : choices.Any(c => c.ToString() == SwordofJustice) ? SwordofJustice : TruesilverChampion, false);

                    break;
                case DragonDeck.DragonPriest:
                    _whiteList.AddOrUpdate(_aggro ? ShadowWordPain : "", false);
                    _whiteList.AddOrUpdate(!_aggro && (_has1Drop || _has2Drop) ? PowerWordShield : "", false);
                    _whiteList.AddOrUpdate(_has2Drop || (_has1Drop && _hasCoin) ? VelensChosen : "", false);
                    if (needActivator)
                    {
                        Bot.Log("[SmartMulligan] I will try to get a dragon to go along with whelp/agent");
                        _whiteList.AddOrUpdate(TwilightWhelp, true);
                        _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == TwilightGuardian) ? TwilightGuardian : choices.Any(c => c.ToString() == DragonkinSorcerer) ? DragonkinSorcerer : choices.Any(c => c.ToString() == AzureDrake) ? AzureDrake : choices.Any(c => c.ToString() == Chillmaw) ? Chillmaw : Ysera, false);
                    }
                    break;
                case DragonDeck.DragonWarlock:
                    if (!_aggro)
                    {
                        List<string> coreControl = new List<string> { TwilightDrake, MountainGiant };
                        bool gotCore =
                            choices.Select(q => CardTemplate.LoadFromId(q).Id.ToString())
                                .ToList()
                                .Intersect(coreControl)
                                .ToList()
                                .Count > 0;
                        _whiteList.AddOrUpdate(!gotCore ? TwilightGuardian : "", false);
                        foreach (var q in from q in coreControl let card = CardTemplate.LoadFromId(q) select q)
                            _whiteList.AddOrUpdate(q, true);
                        _whiteList.AddOrUpdate(Hellfire, false);
                        _whiteList.AddOrUpdate(Shadowflame, false);
                        break;
                    }
                    List<string> coreAggro = new List<string> { MortalCoil, Darkbomb, Hellfire, Shadowflame };
                    foreach (var q in coreAggro)
                        _whiteList.AddOrUpdate(q, false);

                    break;
                case DragonDeck.DragonShaman:
                    break;
                case DragonDeck.DragonMage:
                    break;
                case DragonDeck.DragonWarrior:
                    _whiteList.AddOrUpdate(!_has2Drop ? Bash : "", false);
                    _whiteList.AddOrUpdate(DeathsBite, false);
                    _whiteList.AddOrUpdate(_aggro && _has3Drop ? TwilightGuardian : "", false);
                    _whiteList.AddOrUpdate(!_aggro ? TwilightGuardian : "", false);
                    break;
                case DragonDeck.DragonRogue:
                    break;
                case DragonDeck.DragonDruid:
                    break;
                case DragonDeck.DragonHunter:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }
        //Control and totem shaman
        private void HandleShaman(List<Card.Cards> choices, Card.CClass opponentClass, DeckType dType)
        {
            var control = false;
            switch (opponentClass)
            {
                case Card.CClass.SHAMAN:
                    control = true;
                    break;
                case Card.CClass.PRIEST:
                    control = true;
                    break;
                case Card.CClass.MAGE:
                    //control = true;
                    break;
                case Card.CClass.PALADIN:
                    break;
                case Card.CClass.WARRIOR:
                    control = true;
                    break;
                case Card.CClass.WARLOCK:
                    {
                        _whiteList.AddOrUpdate(EarthShock, false); //earth shock because I assume it's a handlock
                        control = true;
                    }
                    break;
                case Card.CClass.HUNTER:
                    break;
                case Card.CClass.ROGUE:
                    control = true;
                    break;
                case Card.CClass.DRUID: // I assume that druids on the ladder are aggro
                    //control = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("opponentClass", opponentClass, null);
            }
            foreach (var c in choices)
            {
                var temp = CardTemplate.LoadFromId(c.ToString());

                if (temp.Race == Card.CRace.TOTEM && temp.Cost == 2 && temp.Atk != 0)
                {
                    _has2Drop = true;
                    _whiteList.AddOrUpdate(c.ToString(), _hasCoin);
                }


                if (temp.Type == Card.CType.MINION && temp.Quality != Card.CQuality.Epic)
                {
                    if (temp.Cost == 1)
                    {
                        _has1Drop = true;
                        _whiteList.AddOrUpdate(c.ToString(), !control);
                    }
                    if (temp.Cost == 2 && temp.Atk > 0)
                    {
                        _has2Drop = true;
                        _whiteList.AddOrUpdate(c.ToString(), false);
                    }
                    if (!control && temp.Cost == 3 && temp.Atk > 1)
                    {
                        _has3Drop = true;
                        _whiteList.AddOrUpdate(c.ToString(), _hasCoin);
                    }
                }
                var curve = _has2Drop || _has3Drop && _hasCoin;
                if (curve && temp.Cost == 4 && temp.Atk > 3)
                {
                    _whiteList.AddOrUpdate(c.ToString(), false);
                }
            }
            foreach (var c in choices)
            {
                var temp = CardTemplate.LoadFromId(c.ToString());
                if (temp.Type != Card.CType.SPELL || c.ToString() == HealingWave || c.ToString() == Hex) continue;
                if (control && temp.Cost == 1) _whiteList.AddOrUpdate(c.ToString(), false);
                if (!control && temp.Cost == 3)
                    _whiteList.AddOrUpdate(c.ToString(), false);
            }
        }
        //Midrange, Hybrid and Face
        private void HandleHunter(List<Card.Cards> choices, Card.CClass opponentClass, Style dStyle)
        {
            var hasCoin = choices.Count > 3;
            _has2Drop = choices.Any(c => c.ToString() == HauntedCreeper) || choices.Any(c => c.ToString() == MadScientist) || choices.Any(c => c.ToString() == KnifeJuggler); // Kings Elek is not a legitimate 2 drop in my eyes
            var allowHunterMark = (choices.Any(c => c.ToString() == Webspinner) || (choices.Any(c => c.ToString() == HauntedCreeper)));
            if (dStyle == Style.Face || dStyle == Style.Aggro)
            {
                foreach (var q in from q in choices let w = CardTemplate.LoadFromId(q) where w.Cost == 1 select q)
                {
                    _has1Drop = true;
                    _whiteList.AddOrUpdate(q.ToString(), true);
                }
                foreach (var q in from q in choices let w = CardTemplate.LoadFromId(q) where w.Cost == 2 && !w.IsSecret && w.Id.ToString() != IronbeakOwl select q)
                {
                    _has2Drop = true;
                    _whiteList.AddOrUpdate(q.ToString(), true);
                }
                foreach (var q in (from q in choices let w = CardTemplate.LoadFromId(q) where w.Cost == 3 && w.Health > 1 select q).Where(q => _has1Drop || _has2Drop))
                {
                    _has3Drop = true;
                    _whiteList.AddOrUpdate(q.ToString(), false);
                }
                _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MadScientist) ? EaglehornBow : choices.Any(c => c.ToString() == Glaivezooka) ? Glaivezooka : EaglehornBow, false);
                if (dStyle == Style.Face) return;
            }

            _whiteList.AddOrUpdate(Webspinner, false);
            _whiteList.AddOrUpdate(KnifeJuggler, false);
            _whiteList.AddOrUpdate(HauntedCreeper, false);
            _whiteList.AddOrUpdate(MadScientist, hasCoin); //keeps 2 scientists on coin
            _whiteList.AddOrUpdate(LeperGnome, false);
            _whiteList.AddOrUpdate(AbusiveSergeant, false);
            _whiteList.AddOrUpdate(WorgenInfiltrator, false);
            _whiteList.AddOrUpdate(ArgentSquire, false);
            _whiteList.AddOrUpdate(LanceCarrier, false);

            _whiteList.AddOrUpdate(!_has2Drop ? KingsElekk : ArgentHorserider, false);
            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == KnifeJuggler) && _hasCoin ? SnakeTrap : "", false);

            switch (opponentClass)
            {
                case Card.CClass.DRUID:
                    {
                        if (allowHunterMark)
                            _whiteList.AddOrUpdate(HuntersMark, false);
                        if (_has2Drop)
                        {
                            _whiteList.AddOrUpdate(EaglehornBow, false);
                            _whiteList.AddOrUpdate(AnimalCompanion, false);
                        }
                        if (_has2Drop && hasCoin)
                            _whiteList.AddOrUpdate(PilotedShredder, false);

                        _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MadScientist) ? EaglehornBow : choices.Any(c => c.ToString() == Glaivezooka) ? Glaivezooka : EaglehornBow, false);


                        break;
                    }
                case Card.CClass.HUNTER:
                    {
                        if (_has2Drop)
                        {
                            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MadScientist) ? EaglehornBow : choices.Any(c => c.ToString() == Glaivezooka) ? Glaivezooka : EaglehornBow, false);
                        }
                        _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MadScientist) ? EaglehornBow : choices.Any(c => c.ToString() == Glaivezooka) ? Glaivezooka : EaglehornBow, false);
                        if (_has2Drop || hasCoin)
                            _whiteList.AddOrUpdate(AnimalCompanion, false);
                        _whiteList.AddOrUpdate(UnleashtheHounds, false);
                        break;
                    }
                case Card.CClass.MAGE:
                    {
                        if (_has2Drop)
                        {
                            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MadScientist) ? EaglehornBow : choices.Any(c => c.ToString() == Glaivezooka) ? Glaivezooka : EaglehornBow, false);

                            _whiteList.AddOrUpdate(QuickShot, false);
                            _whiteList.AddOrUpdate(IronbeakOwl, false);
                            _whiteList.AddOrUpdate(AnimalCompanion, false);
                        }
                        if (hasCoin)
                            _whiteList.AddOrUpdate(AnimalCompanion, false);
                        _whiteList.AddOrUpdate(BearTrap, false); //It is a justifiable secret against tempo mages
                        break;
                    }
                case Card.CClass.PALADIN:
                    {
                        if (_has2Drop)
                        {
                            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MadScientist) ? EaglehornBow : choices.Any(c => c.ToString() == Glaivezooka) ? Glaivezooka : EaglehornBow, false);
                        }
                        _whiteList.AddOrUpdate(Flare, false);
                        _whiteList.AddOrUpdate(UnleashtheHounds, false);
                        if (hasCoin && _has2Drop)
                            _whiteList.AddOrUpdate(AnimalCompanion, false);

                        break;
                    }
                case Card.CClass.PRIEST:
                    {
                        _whiteList.AddOrUpdate(KingsElekk, false);
                        _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MadScientist) ? EaglehornBow : choices.Any(c => c.ToString() == Glaivezooka) ? Glaivezooka : EaglehornBow, false);
                        if (hasCoin)
                            _whiteList.AddOrUpdate(PilotedShredder, false);
                        if (_has2Drop)
                        {
                            _whiteList.AddOrUpdate(AnimalCompanion, false);
                            _whiteList.AddOrUpdate(QuickShot, false);
                        }
                        break;
                    }
                case Card.CClass.ROGUE:
                    {
                        _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MadScientist) ? EaglehornBow : choices.Any(c => c.ToString() == Glaivezooka) ? Glaivezooka : EaglehornBow, false);
                        if (_has2Drop)
                            _whiteList.AddOrUpdate(AnimalCompanion, false);
                        break;
                    }
                case Card.CClass.SHAMAN:
                    {
                        if (allowHunterMark)
                            _whiteList.AddOrUpdate(HuntersMark, false);
                        _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MadScientist) ? EaglehornBow : choices.Any(c => c.ToString() == Glaivezooka) ? Glaivezooka : EaglehornBow, false);
                        break;
                    }
                case Card.CClass.WARLOCK:
                    {
                        _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MadScientist) ? EaglehornBow : choices.Any(c => c.ToString() == Glaivezooka) ? Glaivezooka : EaglehornBow, false);
                        if (hasCoin || _has2Drop)
                            _whiteList.AddOrUpdate(AnimalCompanion, false);
                        if (_has2Drop)
                            _whiteList.AddOrUpdate(QuickShot, false);

                        break;
                    }
                case Card.CClass.WARRIOR:
                    {
                        _whiteList.AddOrUpdate(IronbeakOwl, false);
                        _whiteList.AddOrUpdate(Wolfrider, false);
                        _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MadScientist) ? EaglehornBow : choices.Any(c => c.ToString() == Glaivezooka) ? Glaivezooka : EaglehornBow, false);
                        if (_has2Drop || hasCoin)
                            _whiteList.AddOrUpdate(AnimalCompanion, false);
                        if (_has2Drop && hasCoin)
                            _whiteList.AddOrUpdate(AnimalCompanion, true);
                        break;
                    }
            }
        }
        //Ramp, Midrange, FelFace
        private void HandleDruid(List<Card.Cards> choices, Card.CClass opponentClass, Style dStyle)
        {
            if (dStyle == Style.Face)
            {
                foreach (var c in choices)
                {
                    var temp = CardTemplate.LoadFromId(c.ToString());
                    if (temp.Type != Card.CType.MINION) continue;
                    switch (temp.Cost)
                    {
                        case 1:
                            _has1Drop = true;
                            _whiteList.AddOrUpdate(c.ToString(), false);
                            break;
                        case 2:
                            _has2Drop = true;
                            _whiteList.AddOrUpdate(c.ToString(), false);
                            break;
                        case 3:
                            _has3Drop = true;
                            _whiteList.AddOrUpdate(c.ToString(), false);
                            break;
                    }
                }
                foreach (var c in from c in choices let temp = CardTemplate.LoadFromId(c.ToString()) where !_has2Drop && temp.Cost == 2 && temp.Type == Card.CType.SPELL select c)
                    _whiteList.AddOrUpdate(c.ToString(), false);
            }
            var hasInnervate = choices.Any(g => g.ToString() == Innervate); //If there is an innervate
            var doubleInnervate = choices.Count(g => g.ToString() == Innervate) == 2; //If there are 2 innervates
            var hasWildGrowth = choices.Any(g => g.ToString() == WildGrowth); //If the is a wild growth
            var hasAspirant = choices.Any(g => g.ToString() == DarnassusAspirant); //If there is an Aspirant
            var earlyGame = hasWildGrowth || hasAspirant;

            /*Always whitelists at least 1 innervate*/
            _whiteList.AddOrUpdate(Innervate, true);
            _whiteList.AddOrUpdate(WildGrowth, false);
            _whiteList.AddOrUpdate(DarnassusAspirant, true);
            _whiteList.AddOrUpdate(ZombieChow, _hasCoin);
            _whiteList.AddOrUpdate(SpiderTank, hasInnervate);
            _whiteList.AddOrUpdate(FjolaLightbane, false);
            _whiteList.AddOrUpdate(EydisDarkbane, false);
            _whiteList.AddOrUpdate(FlameJuggler, false);
            _whiteList.AddOrUpdate(DruidoftheSaber, false);

            var control = false;
            /********************************************/
            _whiteList.AddOrUpdate(doubleInnervate && _hasCoin ? DrBoom : choices.Any(c => c.ToString() == AncientofLore) && doubleInnervate && _hasCoin ? AncientofLore : "", false);
            switch (opponentClass)
            {
                case Card.CClass.DRUID:
                    {
                        control = true;
                        break;
                    }

                case Card.CClass.MAGE:
                    {
                        _whiteList.AddOrUpdate(Wrath, false);
                        break;
                    }
                case Card.CClass.PALADIN:
                    {
                        if (earlyGame) _whiteList.AddOrUpdate(Swipe, false);
                        break;
                    }
                case Card.CClass.PRIEST:
                    {
                        if (earlyGame)
                            _whiteList.AddOrUpdate(Wrath, false);
                        if (_hasCoin)
                            _whiteList.AddOrUpdate(KeeperoftheGrove, true);
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
                        if (earlyGame) _whiteList.AddOrUpdate(KeeperoftheGrove, false);
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
            if (_wc && _hasCoin)
                _whiteList.AddOrUpdate(HarrisonJones, false);

            if (_aggro)
            {
                if (hasInnervate)
                    _whiteList.AddOrUpdate(ShadeofNaxxramas, false);
                if (_hasCoin && hasInnervate)
                    _whiteList.AddOrUpdate(PilotedShredder, false);

                _whiteList.AddOrUpdate(LivingRoots, true);
                _whiteList.AddOrUpdate(Wrath, false);
                _whiteList.AddOrUpdate(KeeperoftheGrove, false);
            }

            else if (control)
            {
                _whiteList.AddOrUpdate(LivingRoots, false);
                _whiteList.AddOrUpdate(ShadeofNaxxramas, false);

                if (hasWildGrowth || hasAspirant || _hasCoin)
                {
                    if (choices.Any(c => c.ToString() == PilotedShredder))
                        _has4Drop = true;
                    _whiteList.AddOrUpdate(PilotedShredder, false);
                }

                if (_hasCoin && doubleInnervate) //I refuse to add Dr Boom and Ancient of Lore to this
                {
                    if (choices.Any(c => c.ToString() == PilotedShredder))
                        _has4Drop = true;
                    _whiteList.AddOrUpdate(PilotedShredder, true);
                }


                if (!_hasCoin || !hasInnervate || _has4Drop) return;
                _whiteList.AddOrUpdate(AzureDrake, false);
                _whiteList.AddOrUpdate(DruidoftheClaw, false);
            }
        }
        //Obvious... I hope
        private void HandleHandlock(List<Card.Cards> choices, Card.CClass opponentClass)
        {
            var strongHand = choices.Any(c => c.ToString() == MountainGiant && c.ToString() == TwilightDrake);
            var goodHand = choices.Any(c => c.ToString() == MountainGiant || c.ToString() == TwilightDrake);
            var hasVoidCaller = choices.Any(c => c.ToString() == Voidcaller);

            //Check if there are no good drops in the opening hand
            var terribleHand = choices.All(c => c.ToString() != MountainGiant) && choices.All(c => c.ToString() != TwilightDrake) && choices.All(c => c.ToString() != AncientWatcher) && choices.All(c => c.ToString() != SunfuryProtector) && choices.All(c => c.ToString() != Voidcaller);
            _whiteList.AddOrUpdate(terribleHand ? MoltenGiant : "", true);


            switch (opponentClass)
            {
                case Card.CClass.DRUID:
                    {
                        _whiteList.AddOrUpdate(MountainGiant, true);
                        _whiteList.AddOrUpdate(Hellfire, false);
                        _whiteList.AddOrUpdate(AncientWatcher, false);
                        _whiteList.AddOrUpdate(DefenderofArgus, false);
                        _whiteList.AddOrUpdate(Voidcaller, false);
                        _whiteList.AddOrUpdate(TwilightDrake, _hasCoin);

                        if (hasVoidCaller)
                            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MalGanis) ? MalGanis : choices.Any(c => c.ToString() == LordJaraxxus) ? LordJaraxxus : choices.Any(c => c.ToString() == Doomguard) ? Doomguard : choices.Any(c => c.ToString() == DreadInfernal) ? DreadInfernal : "", false);
                        break;
                    }
                case Card.CClass.HUNTER:
                    {
                        _whiteList.AddOrUpdate(ZombieChow, true);
                        _whiteList.AddOrUpdate(MortalCoil, false);
                        _whiteList.AddOrUpdate(MoltenGiant, true);
                        _whiteList.AddOrUpdate(SunfuryProtector, false);
                        _whiteList.AddOrUpdate(AcidicSwampOoze, true);
                        _whiteList.AddOrUpdate(AncientWatcher, false);
                        _whiteList.AddOrUpdate(Darkbomb, false);

                        break;
                    }
                case Card.CClass.MAGE:
                    {
                        _whiteList.AddOrUpdate(MortalCoil, false);
                        _whiteList.AddOrUpdate(IronbeakOwl, false);
                        _whiteList.AddOrUpdate(TwilightDrake, true);
                        _whiteList.AddOrUpdate(Darkbomb, false);
                        _whiteList.AddOrUpdate(Hellfire, false);
                        _whiteList.AddOrUpdate(SunfuryProtector, false);
                        _whiteList.AddOrUpdate(Voidcaller, false);
                        if (hasVoidCaller)
                            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MalGanis) ? MalGanis : choices.Any(c => c.ToString() == LordJaraxxus) ? LordJaraxxus : choices.Any(c => c.ToString() == Doomguard) ? Doomguard : choices.Any(c => c.ToString() == DreadInfernal) ? DreadInfernal : "", false);
                        _whiteList.AddOrUpdate(AncientWatcher, false);


                        break;
                    }
                case Card.CClass.PALADIN:
                    {
                        _whiteList.AddOrUpdate(Voidcaller, false);
                        _whiteList.AddOrUpdate(ZombieChow, true);
                        _whiteList.AddOrUpdate(MortalCoil, false);
                        _whiteList.AddOrUpdate(TwilightDrake, true);
                        _whiteList.AddOrUpdate(Hellfire, false);

                        _whiteList.AddOrUpdate(MoltenGiant, true);
                        _whiteList.AddOrUpdate(MountainGiant, false);
                        _whiteList.AddOrUpdate(SunfuryProtector, false);
                        if (hasVoidCaller)
                            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MalGanis) ? MalGanis : choices.Any(c => c.ToString() == LordJaraxxus) ? LordJaraxxus : choices.Any(c => c.ToString() == Doomguard) ? Doomguard : choices.Any(c => c.ToString() == DreadInfernal) ? DreadInfernal : "", false);

                        _whiteList.AddOrUpdate(AncientWatcher, false);
                        break;
                    }
                case Card.CClass.PRIEST:
                    {
                        _whiteList.AddOrUpdate(Voidcaller, false);
                        _whiteList.AddOrUpdate(MountainGiant, true);
                        _whiteList.AddOrUpdate(TwilightDrake, true);
                        _whiteList.AddOrUpdate(Darkbomb, false);
                        _whiteList.AddOrUpdate(IronbeakOwl, false);
                        if (_hasCoin && strongHand)
                            _whiteList.AddOrUpdate(EmperorThaurissan, false);
                        if (hasVoidCaller)
                            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MalGanis) ? MalGanis : choices.Any(c => c.ToString() == LordJaraxxus) ? LordJaraxxus : choices.Any(c => c.ToString() == Doomguard) ? Doomguard : choices.Any(c => c.ToString() == DreadInfernal) ? DreadInfernal : "", false);
                        break;
                    }
                case Card.CClass.ROGUE:
                    {
                        _whiteList.AddOrUpdate(Voidcaller, false);
                        _whiteList.AddOrUpdate(MountainGiant, true);
                        _whiteList.AddOrUpdate(TwilightDrake, true);
                        _whiteList.AddOrUpdate(Darkbomb, false);
                        if (hasVoidCaller)
                            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MalGanis) ? MalGanis : choices.Any(c => c.ToString() == LordJaraxxus) ? LordJaraxxus : choices.Any(c => c.ToString() == Doomguard) ? Doomguard : choices.Any(c => c.ToString() == DreadInfernal) ? DreadInfernal : "", false);
                        break;
                    }
                case Card.CClass.SHAMAN:
                    {
                        _whiteList.AddOrUpdate(Voidcaller, false);
                        _whiteList.AddOrUpdate(ZombieChow, true);
                        _whiteList.AddOrUpdate(AncientWatcher, true);
                        _whiteList.AddOrUpdate(Darkbomb, false);
                        _whiteList.AddOrUpdate(Hellfire, false);
                        _whiteList.AddOrUpdate(SunfuryProtector, false);
                        _whiteList.AddOrUpdate(MoltenGiant, true);


                        if (hasVoidCaller)
                            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MalGanis) ? MalGanis : choices.Any(c => c.ToString() == LordJaraxxus) ? LordJaraxxus : choices.Any(c => c.ToString() == Doomguard) ? Doomguard : choices.Any(c => c.ToString() == DreadInfernal) ? DreadInfernal : "", false);
                        break;
                    }
                case Card.CClass.WARLOCK:
                    {
                        _whiteList.AddOrUpdate(TwilightDrake, true);
                        _whiteList.AddOrUpdate(Darkbomb, false);
                        _whiteList.AddOrUpdate(Hellfire, false);
                        _whiteList.AddOrUpdate(SunfuryProtector, false);
                        _whiteList.AddOrUpdate(BigGameHunter, false);
                        _whiteList.AddOrUpdate(MoltenGiant, true);
                        if ((choices.Any(c => c.ToString() == MortalCoil) && choices.Any(c => c.ToString() == IronbeakOwl)))
                        {
                            _whiteList.AddOrUpdate(IronbeakOwl, false);
                            _whiteList.AddOrUpdate(MortalCoil, false);
                        }

                        _whiteList.AddOrUpdate(AncientWatcher, false);

                        break;
                    }
                case Card.CClass.WARRIOR:
                    {
                        _whiteList.AddOrUpdate(Voidcaller, false);
                        _whiteList.AddOrUpdate(AncientWatcher, true);
                        _whiteList.AddOrUpdate(Darkbomb, false);
                        if (goodHand)
                            _whiteList.AddOrUpdate(AcidicSwampOoze, false);
                        _whiteList.AddOrUpdate(TwilightDrake, true);
                        _whiteList.AddOrUpdate(MountainGiant, true);
                        _whiteList.AddOrUpdate(IronbeakOwl, false);
                        if (_hasCoin && strongHand)
                            _whiteList.AddOrUpdate(EmperorThaurissan, false);
                        if (hasVoidCaller)
                            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MalGanis) ? MalGanis : choices.Any(c => c.ToString() == LordJaraxxus) ? LordJaraxxus : choices.Any(c => c.ToString() == Doomguard) ? Doomguard : choices.Any(c => c.ToString() == DreadInfernal) ? DreadInfernal : "", false);

                        break;
                    }
            }
        }
        //Personal prefernse, will not accept recomendations :roto2cafe:
        private void HandleFreezeMage(List<Card.Cards> choices, Card.CClass opponentClass)
        {
            if (_aggro)
            {
                _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == Doomsayer) ? Doomsayer : Frostbolt, false);
                _whiteList.AddOrUpdate(ForgottenTorch, false);
                foreach (var c in choices)
                {
                    var card = CardTemplate.LoadFromId(c);
                    if (card.Quality == Card.CQuality.Legendary) continue;
                    _whiteList.AddOrUpdate(card.Cost == 2 ? c.ToString() : "", false);
                }
            }
            else
            {
                foreach (var c in choices)
                {
                    var card = CardTemplate.LoadFromId(c);
                    _whiteList.AddOrUpdate(card.Cost == 2 ? c.ToString() : "", false);
                }
                _whiteList.AddOrUpdate(opponentClass == Card.CClass.DRUID ? Frostbolt : "", false);
                _whiteList.AddOrUpdate(ArcaneIntellect, false);
                _whiteList.AddOrUpdate(AcolyteofPain, false);
            }
        }
        //See above
        private void HandleEchoMage(List<Card.Cards> choices, Card.CClass opponentClass)
        {
            var heavyControlGroup = opponentClass == Card.CClass.PRIEST || opponentClass == Card.CClass.WARRIOR;
            var doom = opponentClass != Card.CClass.MAGE || opponentClass != Card.CClass.PRIEST;
            _whiteList.AddOrUpdate(MadScientist, _hasCoin);
            _whiteList.AddOrUpdate(ArcaneIntellect, false);
            if (_aggro)
            {
                _whiteList.AddOrUpdate(ExplosiveSheep, false);
                _whiteList.AddOrUpdate(SunfuryProtector, false);
            }
            _whiteList.AddOrUpdate(heavyControlGroup ? EmperorThaurissan : "", false);
            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == EmperorThaurissan) ? Duplicate : "", false);
            _whiteList.AddOrUpdate(doom ? Doomsayer : "", false);
            foreach (var q in choices)
            {
                var card = CardTemplate.LoadFromId(q);
                if (card.Type == Card.CType.SPELL || card.HasBattlecry || card.Atk > 2 || card.HasDeathrattle) continue;
                _whiteList.AddOrUpdate(card.Cost == 2 ? q.ToString() : "", false);
            }
        }
        //Not to be confused with face
        private void HandleMechWarrior(IEnumerable<Card.Cards> cards, Card.CClass opponentClass)
        {
            foreach (var c in cards)
            {
                var temp = CardTemplate.LoadFromId(c.ToString());
                if (!_hasWeapon && temp.Type == Card.CType.WEAPON && temp.Cost == 2)
                {
                    _has2Drop = true;
                    _whiteList.AddOrUpdate(c.ToString(), false);
                    _hasWeapon = true;
                }
                if (temp.Race == Card.CRace.MECH)
                {
                    if (temp.Cost == 1)
                    {
                        _has1Drop = true;
                        _whiteList.AddOrUpdate(c.ToString(), false);
                    }
                    if (temp.Cost == 2)
                    {
                        _has2Drop = true;
                        _whiteList.AddOrUpdate(c.ToString(), false);
                    }
                    if (temp.Cost == 3)
                    {
                        _has3Drop = true;
                        _whiteList.AddOrUpdate(c.ToString(), false);
                    }
                }
                if (temp.Type != Card.CType.WEAPON && temp.Cost <= 3 && temp.Type != Card.CType.SPELL && temp.Race != Card.CRace.MECH)
                {
                    _whiteList.AddOrUpdate(c.ToString(), false);
                }

                if (_aggro) continue;
                if (temp.Type != Card.CType.WEAPON && _has2Drop && temp.Cost == 4 && temp.Race == Card.CRace.MECH && temp.Atk > 3)
                    _whiteList.AddOrUpdate(c.ToString(), false);
            }
        }

        private void HandleSecretPaladin(List<Card.Cards> choices, Style style)
        {
            var has2Drop = (choices.Any(c => CardTemplate.LoadFromId(c).Cost == 2 && CardTemplate.LoadFromId(c).Type == Card.CType.MINION));
            foreach (var q in choices)
            {
                var card = CardTemplate.LoadFromId(q);
                if (card.Cost == 1 && card.Type == Card.CType.MINION)
                    _whiteList.AddOrUpdate(q.ToString(), card.Divineshield || _hasCoin || card.Health == 3);
                if (card.Cost == 2 && card.Type == Card.CType.MINION)
                    _whiteList.AddOrUpdate(q.ToString(), card.Divineshield && card.Atk == 2);
                if (card.Cost == 3 && card.Type == Card.CType.SPELL && card.Id.ToString() != DivineFavor)
                    _whiteList.AddOrUpdate(q.ToString(), false);
            }
            if (_aggro)
            {
                _whiteList.AddOrUpdate(_oc == Card.CClass.DRUID ? AldorPeacekeeper : "", false);
                _whiteList.AddOrUpdate(Consecration, false);
            }
            else
            {
                _whiteList.Remove(IronbeakOwl);
                _whiteList.AddOrUpdate(_hasCoin ? MysteriousChallenger : "", false);
                _whiteList.AddOrUpdate(_hasCoin ? TruesilverChampion : "", false);
                _whiteList.AddOrUpdate(PilotedShredder, _oc == Card.CClass.WARRIOR && _hasCoin);
            }
            _whiteList.AddOrUpdate(MusterforBattle, false);
            if ((choices.Any(c => c.ToString() == Coghammer) && has2Drop && (_oc != Card.CClass.WARRIOR)))
                _whiteList.AddOrUpdate(Coghammer, false);
            else if ((_oc == Card.CClass.WARRIOR) || (_oc == Card.CClass.PRIEST) || (_oc == Card.CClass.ROGUE) || (_oc == Card.CClass.DRUID))
                _whiteList.AddOrUpdate(TruesilverChampion, false);

        }
        //Mysterious Challenger 3.4
        private void HandleSecretPaladin(List<Card.Cards> choices, Card.CClass opponentClass)
        {
            var hasCoin = choices.Count > 3;
            var has2Drop = (choices.Any(c => c.ToString() == ShieldedMinibot) || choices.Any(c => c.ToString() == MadScientist) || choices.Any(c => c.ToString() == KnifeJuggler));
            var lazyFlag = false;
            if (CurrentDeck.Any(c => c == TirionFordring || c == DrBoom))
            {
                var comSpirit = choices.Any(c => c.ToString() == CompetitiveSpirit) && choices.Any(c => c.ToString() == MusterforBattle) && _hasCoin;
                _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == ShieldedMinibot) ? Redemption : "", false);
                if (opponentClass != Card.CClass.ROGUE || opponentClass != Card.CClass.DRUID)
                    _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == KnifeJuggler) ? NobleSacrifice : "", false);
                _whiteList.AddOrUpdate(comSpirit ? CompetitiveSpirit : "", false);
                _whiteList.AddOrUpdate(MusterforBattle, false);

            }

            #region Default Mulligan

            _whiteList.AddOrUpdate(AbusiveSergeant, false);
            _whiteList.AddOrUpdate(ArgentSquire, true);
            _whiteList.AddOrUpdate(Coin, true); // Would be nice to keep double
            if (opponentClass != Card.CClass.WARRIOR)
                _whiteList.AddOrUpdate(HauntedCreeper, false);
            _whiteList.AddOrUpdate(KnifeJuggler, false);
            _whiteList.AddOrUpdate(LeperGnome, true);
            _whiteList.AddOrUpdate(MadScientist, true);
            _whiteList.AddOrUpdate(Secretkeeper, false);
            _whiteList.AddOrUpdate(ShieldedMinibot, true);
            _whiteList.AddOrUpdate(ZombieChow, true);
            _whiteList.AddOrUpdate(has2Drop ? HarvestGolem : "", false);
            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == KnifeJuggler) ? MusterforBattle : "", false);

            #endregion Default Mulligan

            _whiteList.AddOrUpdate(hasCoin ? MysteriousChallenger : "", false);
            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == KnifeJuggler) ? NobleSacrifice : "", false);
            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == ShieldedMinibot) ? Redemption : "", false);
            switch (opponentClass)
            {
                case Card.CClass.DRUID:
                    {
                        lazyFlag = hasCoin;
                        _whiteList.AddOrUpdate(MusterforBattle, false);
                        _whiteList.AddOrUpdate(hasCoin || has2Drop ? AldorPeacekeeper : "", false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);

                        break;
                    }
                case Card.CClass.HUNTER:
                    {
                        _whiteList.AddOrUpdate(has2Drop ? Coghammer : "", false);
                        _whiteList.AddOrUpdate(AnnoyoTron, false);
                        _whiteList.AddOrUpdate(hasCoin ? Consecration : "", false);
                        break;
                    }
                case Card.CClass.MAGE:
                    {
                        lazyFlag = hasCoin;
                        _whiteList.AddOrUpdate(MusterforBattle, false);

                        break;
                    }
                case Card.CClass.PALADIN:
                    {
                        _whiteList.AddOrUpdate(hasCoin || has2Drop ? BloodKnight : "", false);
                        _whiteList.AddOrUpdate(hasCoin || has2Drop ? IronbeakOwl : "", false);
                        _whiteList.AddOrUpdate(AnnoyoTron, false);
                        _whiteList.AddOrUpdate(Consecration, false);
                        _whiteList.AddOrUpdate(Consecration, false);
                        _whiteList.AddOrUpdate(MusterforBattle, false);
                        _whiteList.AddOrUpdate(hasCoin ? MysteriousChallenger : "", false);
                        break;
                    }
                case Card.CClass.PRIEST:
                    {

                        _whiteList.AddOrUpdate(PilotedShredder, false);

                        break;
                    }
                case Card.CClass.ROGUE:
                    {
                        if (hasCoin)
                            lazyFlag = true;
                        _whiteList.AddOrUpdate(MusterforBattle, false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        break;
                    }
                case Card.CClass.SHAMAN:
                    {
                        _whiteList.AddOrUpdate(MusterforBattle, false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        break;
                    }
                case Card.CClass.WARLOCK:
                    {
                        _whiteList.AddOrUpdate(Consecration, false);
                        _whiteList.AddOrUpdate(MusterforBattle, false);
                        break;
                    }
                case Card.CClass.WARRIOR:
                    {
                        _whiteList.AddOrUpdate(AnnoyoTron, false);
                        _whiteList.AddOrUpdate(MusterforBattle, false);
                        _whiteList.AddOrUpdate(PilotedShredder, hasCoin);

                        break;
                    }
            }
            if ((choices.Any(c => c.ToString() == Coghammer) && has2Drop && (opponentClass != Card.CClass.WARRIOR)))
                _whiteList.AddOrUpdate(Coghammer, false);
            else if ((opponentClass == Card.CClass.WARRIOR) || (opponentClass == Card.CClass.PRIEST) || (opponentClass == Card.CClass.ROGUE) || (opponentClass == Card.CClass.DRUID))
                _whiteList.AddOrUpdate(TruesilverChampion, false);

            if ((lazyFlag) && (choices.Any(c => c.ToString() == CompetitiveSpirit) && choices.Any(c => c.ToString() == MusterforBattle)))
            {
                _whiteList.AddOrUpdate(MusterforBattle, false);
                _whiteList.AddOrUpdate(CompetitiveSpirit, false);
            }

            if (!hasCoin) return;
            _whiteList.AddOrUpdate(AnnoyoTron, false);

        }
        //Obvious, I hope
        private void HandleMechMage(List<Card.Cards> choices, Card.CClass opponentClass)
        {
            CardTemplate oneManaMinion = null;
            foreach (var c in choices)
            {
                var temp = CardTemplate.LoadFromId(c.ToString());
                if (temp.Race == Card.CRace.MECH)
                {
                    if (temp.Cost == 1)
                        oneManaMinion = CardTemplate.LoadFromId(c.ToString());
                    if (temp.Cost == 2)
                    {
                        _has2Drop = true;
                        _whiteList.AddOrUpdate(c.ToString(), temp.Atk > 1);
                    }
                }
                if (temp.Race != Card.CRace.MECH && temp.Quality != Card.CQuality.Epic && temp.Type != Card.CType.SPELL) //minions handler
                {
                    if (temp.Cost == 1 && temp.Race != Card.CRace.MECH)
                    {
                        _has1Drop = true;
                        _whiteList.AddOrUpdate(c.ToString(), false);
                    }
                    if (temp.Cost == 2)
                    {
                        _has2Drop = true;
                        _whiteList.AddOrUpdate(c.ToString(), true);
                    }
                }
                if (opponentClass == Card.CClass.PALADIN && temp.Cost == 3 && temp.Quality == Card.CQuality.Epic)
                    _whiteList.AddOrUpdate(c.ToString(), false);
                if (temp.Type == Card.CType.SPELL && temp.Cost == 2)
                    _whiteList.AddOrUpdate(c.ToString(), false);
                if (_aggro)
                {
                    if (temp.Type == Card.CType.SPELL && temp.Cost == 1)
                        _whiteList.AddOrUpdate(c.ToString(), false);
                    continue;
                }
                if (temp.Cost == 4 && temp.Race == Card.CRace.MECH && temp.Atk > 3)
                    _whiteList.AddOrUpdate(c.ToString(), false);
            }
            foreach (var c in from c in choices let temp = CardTemplate.LoadFromId(c.ToString()) where _has2Drop && temp.Cost == 3 && temp.Type == Card.CType.MINION select c)
            {
                _has3Drop = true;
                _whiteList.AddOrUpdate(c.ToString(), _hasCoin);
            }
            var curve = _has2Drop && _has3Drop;
            if (oneManaMinion != null && (!_has1Drop && oneManaMinion.Cost == 1))
                _whiteList.AddOrUpdate("GVG_082", false);
            if (!_has3Drop && !_hasCoin) return;
            if (_has3Drop && _hasCoin && choices.Any(c => c.ToString() == "GVG_016"))
                _whiteList.AddOrUpdate("GVG_016", false);
            if (choices.Any(c => c.ToString() == "GVG_004") && _has3Drop || curve)
                _whiteList.AddOrUpdate("GVG_004", false);
        }
        //Basically aggro rogue
        private void HandleBurstRogue(List<Card.Cards> choices, Card.CClass opponentClass)
        {
            List<string> alwaysTwo = new List<string> { LeperGnome, SouthseaDeckhand, LootHoarder };
            foreach (var q in alwaysTwo)
                _whiteList.AddOrUpdate(q, true);
            List<string> atLeastOne = new List<string> { DefiasRingleader, SI7Agent, ArgentHorserider };
            foreach (var q in atLeastOne)
                _whiteList.AddOrUpdate(q, false);
            switch (opponentClass)
            {
                case Card.CClass.SHAMAN:
                    {
                        _whiteList.AddOrUpdate(DefiasRingleader, true);
                        _whiteList.AddOrUpdate(DeadlyPoison, false);
                        _whiteList.AddOrUpdate(IronbeakOwl, false);
                        break;
                    }
                case Card.CClass.PRIEST:
                    {
                        _whiteList.AddOrUpdate(DefiasRingleader, true);
                        _whiteList.AddOrUpdate(DeadlyPoison, false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        break;
                    }
                case Card.CClass.MAGE:

                    //
                    {
                        _whiteList.AddOrUpdate(DeadlyPoison, false);
                        _whiteList.AddOrUpdate(IronbeakOwl, false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        break;
                    }
                case Card.CClass.PALADIN:
                    {
                        _whiteList.AddOrUpdate(DeadlyPoison, false);
                        _whiteList.AddOrUpdate(IronbeakOwl, false);
                        _whiteList.AddOrUpdate(BladeFlurry, false);
                        break;
                    }
                case Card.CClass.WARRIOR:
                    {
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        _whiteList.AddOrUpdate(DeadlyPoison, false);
                        break;
                    }
                case Card.CClass.WARLOCK:
                    {
                        _whiteList.AddOrUpdate(DefiasRingleader, true);
                        _whiteList.AddOrUpdate(DeadlyPoison, false);
                        _whiteList.AddOrUpdate(IronbeakOwl, true);
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        break;
                    }
                case Card.CClass.HUNTER:
                    {
                        _whiteList.AddOrUpdate(IronbeakOwl, false);
                        _whiteList.AddOrUpdate(BladeFlurry, false);
                        break;
                    }
                case Card.CClass.ROGUE:
                    {
                        _whiteList.AddOrUpdate(DefiasRingleader, true);
                        _whiteList.AddOrUpdate(DeadlyPoison, false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        _whiteList.AddOrUpdate(_hasCoin ? AssassinsBlade : "", false);
                        _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == ArgentHorserider) ? ColdBlood : "", false);
                        break;
                    }
                case Card.CClass.DRUID:
                    {
                        _whiteList.AddOrUpdate(IronbeakOwl, false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        break;
                    }
            }
        }
        //Warrior specific. For now
        private void HandlePatronMulligan(List<Card.Cards> choices, Card.CClass opponentClass)
        {
            var aggro = (opponentClass == Card.CClass.DRUID || opponentClass == Card.CClass.HUNTER || opponentClass == Card.CClass.SHAMAN || opponentClass == Card.CClass.PALADIN);
            _hasWeapon = choices.Any(c => CardTemplate.LoadFromId(c).Type == Card.CType.WEAPON);

            _whiteList.AddOrUpdate(_hasWeapon ? DreadCorsair : "", false);
            if (aggro)
            {
                List<string> oneOf = new List<string> { FieryWarAxe, UnstableGhoul, Whirlwind, FrothingBerserker };
                foreach (var q in oneOf)
                    _whiteList.AddOrUpdate(q, false);
                _whiteList.AddOrUpdate(!_hasWeapon ? Slam : "", false);
                _whiteList.AddOrUpdate(_hasWeapon ? DreadCorsair : "", false);
                _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == DreadCorsair) ? DeathsBite : "", false);
            }
            else
            {
                List<string> oneOfControl = new List<string> { FieryWarAxe, DeathsBite, GnomishInventor, Slam };
                foreach (var q in oneOfControl)
                    _whiteList.AddOrUpdate(q, false);
                if (opponentClass == Card.CClass.PRIEST || opponentClass == Card.CClass.WARRIOR)
                    _whiteList.AddOrUpdate(EmperorThaurissan, false);
            }
        }
        //Archive stuff
        private static void CheckDirectory(string subdir, string subdir2 = "")
        {
            if (Directory.Exists(MAIN_DIR + "/" + subdir + "/" + subdir2))
                return;
            Directory.CreateDirectory(MAIN_DIR + "/" + subdir + "/" + subdir2);
        }
       
        //TODO ARENA ANCOR

        private static void HandleSpells(List<Card.Cards> choices, Dictionary<string, bool> whiteList)
        {
            var OneManaSpell = false;
            var TwoManaSpell = false;
            var ThreeManaSpell = false;

            var allowedSpells = new List<string>
            {
                Frostbolt, Flamecannon, UnstablePortal, ArcaneMissiles, MirrorImage, ForgottenTorch, //Mage
                RockbiterWeapon, FeralSpirit, //Shaman
                HolySmite, VelensChosen, Thoughtsteal, PowerWordShield, //Priest
                NobleSacrifice, Avenge, SealofChampions, MusterforBattle, //Paladin
                Bash, Slam, ShieldBlock, //Warrior
                MortalCoil, Darkbomb, Implosion, CurseofRafaam, //Warlock
                Tracking, AnimalCompanion, UnleashtheHounds, //Hunter
                DeadlyPoison, Burgle, BeneaththeGrounds, Backstab, //Rogue
                Innervate, WildGrowth, LivingRoots, PoweroftheWild, RavenIdol //Druid
            };
            var badSecrets = new List<string>
            {
                Snipe, Misdirection, Spellbender, Counterspell, Vaporize, EyeforanEye, Redemption, Repentance, CompetitiveSpirit, SacredTrial, DartTrap
            };
            foreach (var c in (from c in choices let spells = CardTemplate.LoadFromId(c.ToString()) where spells.Type == Card.CType.SPELL && allowedSpells.Contains(c.ToString()) where !spells.IsSecret && spells.Cost == 0 select c))
                whiteList.AddOrUpdate(c.ToString(), false);

            foreach (var c in (from c in choices let spells = CardTemplate.LoadFromId(c.ToString()) where spells.Type == Card.CType.SPELL && allowedSpells.Contains(c.ToString()) where !spells.IsSecret && spells.Cost == 1 && !_has1Drop select c))
            {
                OneManaSpell = true;
                whiteList.AddOrUpdate(c.ToString(), false);
            }
            foreach (var c in (from c in choices let spells = CardTemplate.LoadFromId(c.ToString()) where spells.Type == Card.CType.SPELL && allowedSpells.Contains(c.ToString()) where !spells.IsSecret && spells.Cost == 2 && !_has1Drop select c))
            {
                TwoManaSpell = true;
                whiteList.AddOrUpdate(c.ToString(), false);
            }
            foreach (var c in (from c in choices let spells = CardTemplate.LoadFromId(c.ToString()) where spells.Type == Card.CType.SPELL && allowedSpells.Contains(c.ToString()) where !spells.IsSecret && spells.Cost == 3 && !_has1Drop select c).TakeWhile(c => !TwoManaSpell))
            {
                ThreeManaSpell = true;
                whiteList.AddOrUpdate(c.ToString(), false);
                break;
            }
            foreach (var q in choices)
            {
                var spells = CardTemplate.LoadFromId(q.ToString());

                if (spells.Type == Card.CType.SPELL && allowedSpells.Contains(q.ToString()))
                    if (badSecrets.Contains(q.ToString())) continue;
                if (spells.Cost == 1 && spells.IsSecret && !_has1Drop)
                {
                    whiteList.AddOrUpdate(q.ToString(), false);
                }
                if (spells.Cost == 2 && spells.IsSecret && !_has2Drop && choices.All(w => w.ToString() != MadScientist))
                    // toss away any secrets if I have mad scientist
                    whiteList.AddOrUpdate(q.ToString(), false);
                if (spells.Cost == 3 && spells.IsSecret && !_has3Drop && _hasCoin && choices.All(w => w.ToString() != MadScientist)) //toss away any secret if I have mad scientist
                    whiteList.AddOrUpdate(q.ToString(), false);
            }

        }

        private static void HandleWeapons(List<Card.Cards> choices, Card.CClass ownClass, Dictionary<string, bool> whiteList)
        {
            switch (ownClass)
            {
                case Card.CClass.SHAMAN:
                    {
                        whiteList.AddOrUpdate(choices.Any(c => c.ToString() == StormforgedAxe) ? StormforgedAxe : choices.Any(c => c.ToString() == Powermace) ? Powermace : "", false);
                        break;
                    }
                case Card.CClass.PALADIN:
                    {
                        whiteList.AddOrUpdate(choices.Any(c => c.ToString() == LightsJustice) ? LightsJustice : choices.Any(c => c.ToString() == Coghammer) ? Coghammer : choices.Any(c => c.ToString() == SwordofJustice) ? SwordofJustice : "", false);
                        break;
                    }
                case Card.CClass.WARRIOR:
                    {
                        whiteList.AddOrUpdate(choices.Any(c => c.ToString() == FieryWarAxe) ? FieryWarAxe : choices.Any(c => c.ToString() == KingsDefender) ? KingsDefender : choices.Any(c => c.ToString() == DeathsBite) ? DeathsBite : OgreWarmaul, false);
                        whiteList.AddOrUpdate(_has2Drop || _hasCoin ? DeathsBite : "", false);
                        break;
                    }
                case Card.CClass.HUNTER:
                    {
                        whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MadScientist) ? EaglehornBow : choices.Any(c => c.ToString() == Glaivezooka) ? Glaivezooka : EaglehornBow, false);
                        break;
                    }
                case Card.CClass.ROGUE:
                    {
                        whiteList.AddOrUpdate(choices.Any(c => c.ToString() == PerditionsBlade) ? PerditionsBlade : CogmastersWrench, false);
                        break;
                    }
            }
            //return StormforgedAxe;
        }

        private static void HandleMinions(List<Card.Cards> choices, IDictionary<string, bool> whiteList, Card.CClass opponentClass, Card.CClass ownClass, int valueMod = 0, List<string> activators = null, List<string> needActivation = null, Style deckStyle = Style.Tempo, DeckType type = DeckType.Unknown)
        {
            if (valueMod > 6)
                valueMod = 0;
            Dictionary<string, int> mychoices = new Dictionary<string, int>();
            Dictionary<string, int> mychoices2 = new Dictionary<string, int>();
            Dictionary<string, int> mychoices3 = new Dictionary<string, int>();
            Dictionary<string, int> mychoices4 = new Dictionary<string, int>();
            whiteList.AddOrUpdate(BloodImp, false);
            bool allowFourDrop = false;
            _hasWeapon =
                choices.Any(
                    c =>
                        CardTemplate.LoadFromId(c.ToString()).Type == Card.CType.WEAPON &&
                        CardTemplate.LoadFromId(c.ToString()).Cost <= 4);

            foreach (var c in choices)
            {
                var minion = CardTemplate.LoadFromId(c.ToString());
                var modifier = 0;
                switch (minion.Cost)
                {
                    case 1:
                        {
                            if (minion.Id.ToString() == LeperGnome)
                                modifier += 5;
                            if (activators != null && ChoicesIntersectList(choices, needActivation))
                                if (activators.Any(e => e.ToString() == minion.Id.ToString()))
                                    modifier += 5;

                            switch (_ownC)
                            {
                                case Card.CClass.SHAMAN:
                                    break;
                                case Card.CClass.PRIEST:
                                    break;
                                case Card.CClass.MAGE:
                                    break;
                                case Card.CClass.PALADIN:
                                    break;
                                case Card.CClass.WARRIOR:
                                    break;
                                case Card.CClass.WARLOCK:
                                    modifier += minion.Id.ToString() == ReliquarySeeker ? -10 : 10;
                                    break;
                                case Card.CClass.HUNTER:
                                    break;
                                case Card.CClass.ROGUE:
                                    break;
                                case Card.CClass.DRUID:
                                    break;
                                case Card.CClass.NONE:
                                    break;

                            }
                            mychoices.AddOrUpdate(minion.Id.ToString(), GetPriority(minion, opponentClass, ownClass, modifier));
                            break;
                        }
                    case 2:
                        {
                            switch (minion.Class)
                            {
                                case Card.CClass.SHAMAN:
                                    if (minion.Race != Card.CRace.TOTEM)
                                        modifier += 2;
                                    break;
                                case Card.CClass.PRIEST:
                                    if (minion.Id.ToString() == WyrmrestAgent && NumDragons < 3)
                                        modifier -= 2;
                                    else modifier += 2;
                                    break;
                                case Card.CClass.MAGE:
                                    break;
                                case Card.CClass.PALADIN:
                                    break;
                                case Card.CClass.WARRIOR:
                                    break;
                                case Card.CClass.WARLOCK:
                                    break;
                                case Card.CClass.HUNTER:
                                    break;
                                case Card.CClass.ROGUE:
                                    if (HasCombo(minion))
                                        modifier += 1;
                                    break;
                                case Card.CClass.DRUID:
                                    break;
                                case Card.CClass.NONE:
                                    if (minion.Id.ToString() == NerubianEgg && ChoicesIntersectList(choices, activators))
                                        modifier += 9;
                                    if (minion.Id.ToString() == IronbeakOwl && _aggro)
                                        modifier += 4;
                                    if (type == DeckType.Unknown)
                                        modifier += 4;
                                    break;
                            }

                            mychoices2.AddOrUpdate(minion.Id.ToString(), GetPriority(minion, opponentClass, ownClass, modifier));
                            break;
                        }
                    case 3:
                        {
                            if (type == DeckType.Unknown)
                                modifier += 3;
                            if (!IsArena() && _aggro)
                                modifier -= 8;
                            mychoices3.AddOrUpdate(minion.Id.ToString(), GetPriority(minion, opponentClass, ownClass, modifier));
                            break;
                        }
                    case 4:
                        {
                            if (BadFourDrop(minion))
                                modifier -= 20;
                            if (_has1Drop || _has2Drop)
                            {
                                allowFourDrop = true;
                                modifier += 2;
                            }
                            if (_has2Drop && _has3Drop)
                            {
                                allowFourDrop = true;
                                modifier += 2;
                            }
                            if (_hasWeapon)
                                allowFourDrop = true;
                            mychoices4.AddOrUpdate(minion.Id.ToString(), GetPriority(minion, opponentClass, ownClass, modifier));
                            break;
                        }
                    case 5:
                        whiteList.AddOrUpdate(_oc == Card.CClass.PALADIN ? DarkIronSkulker : "", false);
                        break;

                }
            }
            Num1Drops = 0;
            var sortedDict = (mychoices.OrderByDescending(entry => entry.Value)).Take(4).ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (var c in sortedDict)
            {
                if (c.Value < 1) continue;
                if (c.Value <= 1 && _hasCoin) continue;
                whiteList.AddOrUpdate(c.Key, c.Value > (6 - valueMod) && _hasCoin);
                Num1Drops++;
                _has1Drop = true;
                if (Num1Drops >= Allowed1Drops) break;
            }
            var sortedDict2 = (mychoices2.OrderByDescending(entry => entry.Value)).Take(4).ToDictionary(pair => pair.Key, pair => pair.Value);
            Num2Drops = 0;
            foreach (var c in sortedDict2.Where(c => c.Value >= (2 - valueMod)))
            {
                if (Num2Drops == Allowed2Drops) break;
                Num2Drops++;
                _has2Drop = true;
                whiteList.AddOrUpdate(c.Key, c.Value > 4);
            }
            var sortedDict3 = (mychoices3.OrderByDescending(entry => entry.Value)).Take(4) //at most 2 3 drops
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            Num3Drops = 0;
            foreach (var c in sortedDict3)
            {
                if (Num2Drops == 0 && type != DeckType.Unknown) break;
                if (deckStyle == Style.Aggro || deckStyle == Style.Face && !_has2Drop) break;
                if (Num3Drops == Allowed3Drops) break;
                if (c.Value < 1) continue;
                Num3Drops++;
                _has3Drop = true;
                whiteList.AddOrUpdate(c.Key, false);
            }
            var sortedDict4 = (mychoices4.OrderByDescending(entry => entry.Value)).Take(Allowed4Drops)
                //at most 2 3 drops
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (var c in sortedDict4)
            {
                //if (Num2Drops == 0) break;
                if (Num4Drops == Allowed4Drops) break;
                if (c.Value < 1 || !allowFourDrop) continue;
                Num4Drops++;
                _has4Drop = true;
                whiteList.AddOrUpdate(c.Key, false);
            }
            //GetFourDrops(choices, badMinions, whiteList, aggro);
        }

        private static bool IsArena()
        {
            return Bot.CurrentMode() == Bot.Mode.ArenaAuto || Bot.CurrentMode() == Bot.Mode.Arena;
        }

        private static bool ChoicesIntersectList(List<Card.Cards> choices, List<string> activators)
        {
            if (activators == null || activators.All(string.IsNullOrWhiteSpace))
                return false;
            return choices.Select(q => CardTemplate.LoadFromId(q).Id.ToString()).ToList().Intersect(activators).ToList().Count > 0;
        }

        private static bool BadFourDrop(CardTemplate minion)
        {
            List<string> ommits = new List<string>
            {
                MaidenoftheLake, BaronRivendare, OldMurkEye, Dreadsteed, EnhanceoMechano, MagnataurAlpha, MiniMage, SiltfinSpiritwalker, WeeSpellstopper, AncientMage, CoreRager, DefenderofArgus, DraeneiTotemcarver, EtherealArcanist, Jeeves, MasterofDisguise, ScrewjankClunker, WailingSoul, CultMaster, DragonlingMechanic, KeeperofUldaman, KorkronElite, Lightspawn, MogushanWarden, SilvermoonGuardian, StormwindKnight, SummoningPortal, TournamentMedic, Wildwalker
            };
            return ommits.Any(c => c.ToString() == minion.Id.ToString());
        }

        private static bool HasCombo(CardTemplate minion)
        {
            List<string> comboList = new List<string> { GoblinAutoBarber, UndercityValiant, EdwinVanCleef, SI7Agent, DefiasRingleader };
            return comboList.Any(c => c.ToString() == minion.Id.ToString());
        }

        private static int GetPriority(CardTemplate c, Card.CClass opponentClass, Card.CClass ownClass, int modifier = 0)
        {
            if (c.Type == Card.CType.WEAPON || c.Type == Card.CType.SPELL || c.Type == Card.CType.HERO || c.Type == Card.CType.HEROPOWER || c.IsSecret)
                return -50;
            if (c.Cost > 4)
                return 0;
            var value = 0;
            /*
             * 
             */
            var myDeck = CurrentDeck;
            var numMechs = myDeck.Count(q => CardTemplate.LoadFromId(q).Race == Card.CRace.MECH);
            var containsWarper = myDeck.Any(q => q.ToString() == Mechwarper);
            var numDrag = myDeck.Count(q => CardTemplate.LoadFromId(q).Race == Card.CRace.DRAGON);
            var numSecret = myDeck.Count(q => CardTemplate.LoadFromId(q).IsSecret && CardTemplate.LoadFromId(q).Cost == 1);
            if (c.Cost == 6 && numSecret > 4)
                value += 20;


            var vanila = c.Health == 3 && c.Atk == 2;
            var vanila3 = c.Health == 4 && c.Atk == 3;

            switch (c.Cost)
            {
                case 1:
                    {
                        if (opponentClass == Card.CClass.MAGE || opponentClass == Card.CClass.ROGUE || opponentClass == Card.CClass.DRUID && c.Health == 1 && !c.Divineshield)
                            value--;
                        if (c.Name.Contains("Cogmaster") && numMechs > 3)
                            value++;
                        if (c.Race == Card.CRace.BEAST && ownClass == Card.CClass.HUNTER)
                            value++;
                        if (c.Name.Contains("Injured") && ownClass != Card.CClass.PRIEST)
                            value -= 7;
                        if (c.Name.Contains("secret") && numSecret > 2)
                            value++;
                        if (c.Name.Contains("Voodoo"))
                            value -= 2;
                        if (c.Overload > 0)
                            value -= 4;
                        if (c.Health == 3)
                            value++;
                        if (c.Race == Card.CRace.DEMON && c.Atk == 3)
                            value += 4;
                        if (c.Health == c.Atk && c.HasBattlecry)
                            value += 2;
                        if (c.Quality == Card.CQuality.Epic)
                            value -= 2;
                        if (c.Quality == Card.CQuality.Rare && c.Atk > c.Health)
                            value--;
                        if (c.Atk > c.Health && c.Class == Card.CClass.ROGUE)
                            value += 2;
                        if (c.Stealth && c.Atk == 2)
                            value++;
                        if (c.Divineshield)
                            value++;
                        if (c.HasDeathrattle)
                            value++;
                        if (c.Race == Card.CRace.MURLOC)
                            value--;
                        if (c.Class == Card.CClass.NONE && c.Atk == 2 || c.HasBattlecry)
                            value++;
                        if (c.Taunt && c.Class != Card.CClass.WARLOCK)
                            value -= 2;
                        if (c.Class == Card.CClass.HUNTER && !c.HasDeathrattle)
                            value--;
                        if (c.Class == Card.CClass.HUNTER && c.HasDeathrattle)
                            value++;
                        break;
                    }
                case 2:
                    {
                        value--;
                        if ((c.Health == 1 && !c.HasDeathrattle) || (c.Atk > c.Health + 1) || (c.Health > c.Atk + 1) || (c.Atk == 1 && c.Health == 1))
                        {
                            value -= 2;
                            break;
                        }

                        if (c.Charge)
                            value--;
                        if (c.Taunt)
                            value--;
                        value++;

                        if (c.Name.Equals("Sunfury Protector") && numMechs > 3)
                            value -= 4;
                        if (c.Name.Equals("Mechwarper") && numMechs > 3)
                            value += 2;
                        if (c.Stealth)
                            value++;
                        if (c.Race == Card.CRace.DEMON && c.HasBattlecry)
                            value -= 5;
                        value++;
                        if (vanila && opponentClass == Card.CClass.PALADIN)
                            value++;


                        break;
                    }
                case 3:
                    {
                        value += 2;
                        if (opponentClass == Card.CClass.PALADIN && c.Health == 2)
                            value--;
                        if (c.Charge && c.Divineshield)
                            value++;
                        if (c.Quality == Card.CQuality.Legendary)
                            value++;
                        if (c.Race == Card.CRace.DEMON)
                            value++;
                        if (c.Quality == Card.CQuality.Epic && opponentClass != Card.CClass.PALADIN)
                            value -= 7;
                        if (c.Health == 1 && !c.Divineshield && !c.HasDeathrattle)
                            value -= 5;
                        if (vanila3)
                            value += 2;
                        if (c.Health == c.Cost && c.Atk == c.Cost && c.Quality == Card.CQuality.Rare && !c.Inspire)
                            value -= 7;
                        if (c.Health < c.Cost && c.Atk < c.Cost)
                            value -= 5;
                        if (c.Health == 2 && c.Atk == 2 && c.Class == Card.CClass.DRUID)
                            value += 5;
                        if (c.Class == Card.CClass.ROGUE && c.Race != Card.CRace.MECH)
                            value += 5;
                        if (c.HasBattlecry)
                            value++;
                        if (c.HasDeathrattle)
                            value++;
                        break;
                    }
                case 4:
                    {
                        if (numDrag > 2 && c.Quality == Card.CQuality.Epic)
                            value += 5;
                        if (c.Health == 5 && c.Atk == 4)
                            value += 2;
                        if (c.Health < c.Cost && !c.HasDeathrattle)
                            value -= 8;

                        value++;
                        break;
                    }
            }
            if (c.Health >= c.Atk + 1)
            {
                if (c.HasBattlecry) value++;
                if (c.HasDeathrattle) value++;
                if (c.Enrage) value++;
            }
            if (c.Cantattack || c.Atk == 0)
                value -= 5;
            if (c.Health > c.Cost && c.Atk > c.Cost)
                value += 2;
            if (c.Divineshield) value += 2;
            if (c.Divineshield && c.Health == c.Cost && c.Atk == c.Cost)
                value += 2;
            if (c.Stealth) value++;
            value += c.Health - 1 + c.Atk - 1;


            return value + modifier;
        }

        private void DisplayMulligans(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            using (var file = new StreamWriter(MAIN_DIR + "MulliganArchives\\" + ownClass + "_" + Bot.CurrentMode() + ".txt", true))
            {
                file.WriteLine("/////////////////////////////////////////////////");
                //file.WriteLine("CURRENT MODE IS: "+Bot.CurrentMode());
                file.WriteLine("Your Card options against " + opponentClass + " as a " + ownClass + " were:");
                foreach (var q in choices)
                    file.WriteLine(CardTemplate.LoadFromId(q.ToString()).Cost + " mana card: " + CardTemplate.LoadFromId(q.ToString()).Name);
                file.WriteLine("");
                file.WriteLine("Mulligan pick the following cards to keep ");
                foreach (var q in _ctk)
                    file.WriteLine(CardTemplate.LoadFromId(q.ToString()).Cost + " mana card: " + CardTemplate.LoadFromId(q.ToString()).Name);
                file.WriteLine("|");
                file.WriteLine("=====COMMENT SECTION[Agree? No? Why?]:");
                file.WriteLine("=>");
                //file.WriteLine("========================================");
                file.WriteLine("Your average deck mana cost is " + CurrentDeck.Average(c => CardTemplate.LoadFromId(c).Cost));
                file.WriteLine("--------------------------------------");
                file.WriteLine("----Your alternative options in your deck were: ");
                var allOneDrops = (from q in CurrentDeck select CardTemplate.LoadFromId(q) into qq where qq.Cost == 1 && !qq.IsSecret select qq).ToList();
                var allTwoDrops = (from q in CurrentDeck select CardTemplate.LoadFromId(q) into qq where qq.Cost == 2 && !qq.IsSecret select qq).ToList();
                var allThreeDrops = (from q in CurrentDeck select CardTemplate.LoadFromId(q) into qq where qq.Cost == 3 && !qq.IsSecret select qq).ToList();
                var allFourDrops = (from q in CurrentDeck select CardTemplate.LoadFromId(q) into qq where qq.Cost == 4 && !qq.IsSecret select qq).ToList();
                file.WriteLine("\n------------One Drops:--");
                file.WriteLine(" ");
                var count = 0;
                foreach (var c in allOneDrops)
                {
                    if (count == 3)
                    {
                        file.WriteLine(" ");
                        count = 0;
                    }
                    file.Write("\"{0}[{1}/{2}]\", ", c.Name, c.Atk, c.Health);
                    count++;
                }
                count = 0;
                file.WriteLine(" ");
                file.WriteLine("\n------------Two Drops:--");

                foreach (var c in allTwoDrops)
                {
                    if (count == 3)
                    {
                        file.WriteLine(" ");
                        count = 0;
                    }
                    file.Write("\"{0}[{1}/{2}]\", ", c.Name, c.Atk, c.Health);
                    count++;
                }
                count = 0;
                file.WriteLine(" ");
                file.WriteLine("\n------------Three Drops:--");

                foreach (var c in allThreeDrops)
                {
                    if (count == 3)
                    {
                        file.WriteLine(" ");
                        count = 0;
                    }
                    file.Write("\"{0}[{1}/{2}]\", ", c.Name, c.Atk, c.Health);
                    count++;
                }
                count = 0;
                file.WriteLine(" ");
                file.WriteLine("------------Four Drops:--");

                foreach (var c in allFourDrops)
                {
                    if (count == 3)
                    {
                        file.WriteLine(" ");
                        count = 0;
                    }
                    file.Write("\"{0}[{1}/{2}]\", ", c.Name, c.Atk, c.Health);
                    count++;
                }
            }
        }



        //Half of this project



        private DeckData GetDeckInfo(Card.CClass ownClass)
        {
            var info = new DeckData { Cards = CurrentDeck };
            if (Bot.CurrentMode() == Bot.Mode.Arena || Bot.CurrentMode() == Bot.Mode.ArenaAuto)
            {
                info.DeckStyle = GetStyle();
                info.DeckType = DeckType.Arena;
                SetDefaultsForStyle(info.DeckStyle);
                return info;
            }
            switch (ownClass)
            {
                case Card.CClass.SHAMAN:
                    var faceShaman = new List<string> { RockbiterWeapon, LeperGnome, TunnelTrogg, LavaBurst, LightningBolt };
                    if (CoreComparison(CurrentDeck.Intersect(faceShaman).ToList(), faceShaman, 1, DeckType.FaceShaman))
                    {
                        info.DeckStyle = Style.Face;
                        info.DeckType = DeckType.FaceShaman;
                        return info;
                    }
                    var mechShaman = new List<string> { Crackle, Powermace, Mechwarper, MechanicalYeti, PilotedShredder }; //1
                    if (CoreComparison(CurrentDeck.Intersect(mechShaman).ToList(), mechShaman, 1, DeckType.MechShaman))
                    {

                        info.DeckStyle = Style.Aggro;
                        info.DeckType = DeckType.MechShaman;
                        return info;
                    }
                    var dragonShaman = new List<string> { BlackwingTechnician, BlackwingCorruptor, AzureDrake, TwilightGuardian }; //2
                    if (CoreComparison(CurrentDeck.Intersect(dragonShaman).ToList(), dragonShaman, 1, DeckType.DragonShaman) || NumDragons > 8)
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.DragonShaman;
                        return info;
                    }
                    var totemShaman = new List<string> { TotemGolem, ThunderBluffValiant, ManaTideTotem }; //1
                    if (CoreComparison(CurrentDeck.Intersect(totemShaman).ToList(), totemShaman, 1, DeckType.TotemShaman))
                    {
                        info.DeckStyle = Style.Tempo;
                        info.DeckType = DeckType.TotemShaman;
                        return info;
                    }
                    var malygosShaman = new List<string> { AncestorsCall, Malygos }; //0
                    if (CoreComparison(CurrentDeck.Intersect(malygosShaman).ToList(), malygosShaman, 1, DeckType.MalygosShaman))
                    {
                        info.DeckStyle = Style.Combo;
                        info.DeckType = DeckType.MalygosShaman;
                        return info;
                    }
                    var controlShaman = new List<string> { DrBoom, SludgeBelcher, Hex, FeralSpirit, LightningStorm, EarthShock, Loatheb, SylvanasWindrunner }; //2
                    if (CoreComparison(CurrentDeck.Intersect(controlShaman).ToList(), controlShaman, 2, DeckType.ControlShaman))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.ControlShaman;
                        return info;
                    }
                    break;
                case Card.CClass.PRIEST:
                    var comboPriest = new List<string> { InnerFire, DivineSpirit, MindBlast, ProphetVelen }; //1
                    if (CoreComparison(CurrentDeck.Intersect(comboPriest).ToList(), comboPriest, 2, DeckType.ComboPriest))
                    {
                        info.DeckStyle = Style.Combo;
                        info.DeckType = DeckType.ComboPriest;
                        return info;
                    }
                    var dragonPriest = new List<string> { TwilightGuardian, WyrmrestAgent, TwilightWhelp }; //1
                    if (CoreComparison(CurrentDeck.Intersect(dragonPriest).ToList(), dragonPriest, 1, DeckType.DragonPriest))
                    {
                        info.DeckStyle = Style.Tempo;
                        info.DeckType = DeckType.DragonPriest;
                        return info;
                    }
                    var controlPriest = new List<string> { PowerWordShield, CircleofHealing, NorthshireCleric, DarkCultist, AuchenaiSoulpriest, HolyNova, Lightbomb, Entomb, CabalShadowPriest }; //1
                    if (CoreComparison(CurrentDeck.Intersect(controlPriest).ToList(), controlPriest, 2, DeckType.ControlPriest))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.ControlPriest;
                        return info;
                    }
                    break;
                case Card.CClass.MAGE:
                    var tempoMage = new List<string> { AzureDrake, Flamewaker, Frostbolt, Fireball, MadScientist, Flamecannon, MadScientist, SorcerersApprentice, ManaWyrm, UnstablePortal }; //1
                    if (CoreComparison(CurrentDeck.Intersect(tempoMage).ToList(), tempoMage, 2, DeckType.TempoMage))
                    {
                        info.DeckStyle = Style.Tempo;
                        info.DeckType = DeckType.TempoMage;
                        return info;
                    }

                    var freezeMage = new List<string> { IceBlock, IceBarrier, Alexstrasza, Frostbolt, IceLance, EmperorThaurissan, Fireball, ArchmageAntonidas, BloodmageThalnos, ArcaneIntellect }; //1
                    if (CoreComparison(CurrentDeck.Intersect(freezeMage).ToList(), freezeMage, 0, DeckType.FreezeMage))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.FreezeMage;
                        return info;
                    }
                    var mechMage = new List<string> { ClockworkGnome, Mechwarper, SpiderTank, GoblinBlastmage, Fireball, Frostbolt, TinkertownTechnician }; //1
                    if (CoreComparison(CurrentDeck.Intersect(mechMage).ToList(), mechMage, 1, DeckType.MechMage))
                    {
                        info.DeckStyle = Style.Face;
                        info.DeckType = DeckType.MechMage;
                        return info;
                    }
                    var echoMage = new List<string> { EchoofMedivh, IceBlock, Duplicate }; //1
                    if (CoreComparison(CurrentDeck.Intersect(echoMage).ToList(), echoMage, 1, DeckType.EchoMage, EchoofMedivh))
                    {

                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.EchoMage;
                        return info;
                    }
                    break;
                case Card.CClass.PALADIN:
                    var secretPaladin = new List<string> { MysteriousChallenger, Avenge, NobleSacrifice, CompetitiveSpirit, Redemption }; //0
                    if (CoreComparison(CurrentDeck.Intersect(secretPaladin).ToList(), secretPaladin, 1, DeckType.SecretPaladin, MysteriousChallenger))
                    {
                        info.DeckStyle = Style.Aggro;
                        info.DeckType = DeckType.SecretPaladin;
                        return info;
                    }
                    var midRangePaladin = new List<string> { LayonHands, AldorPeacekeeper, TirionFordring, ZombieChow, BigGameHunter, JusticarTrueheart, SylvanasWindrunner, Quartermaster, MusterforBattle, Equality, Consecration }; //2
                    if (CoreComparison(CurrentDeck.Intersect(midRangePaladin).ToList(), midRangePaladin, 2, DeckType.MidRangePaladin))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.MidRangePaladin;
                        return info;
                    }

                    var aggroPaladin = new List<string> { BlessingofMight, BlessingofKings, TruesilverChampion, KnifeJuggler, ArcaneGolem, LeeroyJenkins, WorgenInfiltrator, LeperGnome, AbusiveSergeant, ArgentSquire, ShieldedMinibot, DivineFavor }; //2
                    if (CoreComparison(CurrentDeck.Intersect(aggroPaladin).ToList(), aggroPaladin, 2, DeckType.AggroPaladin))
                    {
                        info.DeckStyle = Style.Face;
                        info.DeckType = DeckType.AggroPaladin;
                        return info;
                    }
                    break;
                case Card.CClass.WARRIOR:
                    var corePatron = new List<string> { GrimPatron, Whirlwind, DeathsBite, FieryWarAxe, EmperorThaurissan, AcolyteofPain, FrothingBerserker }; //0
                    if (CoreComparison(CurrentDeck.Intersect(corePatron).ToList(), corePatron, 1, DeckType.PatronWarrior))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.PatronWarrior;
                        return info;
                    }
                    var controlWarrior = new List<string> { Execute, ShieldSlam, Alexstrasza, JusticarTrueheart, Shieldmaiden, Brawl, DeathsBite, AcolyteofPain, DrBoom }; //1   
                    if (CoreComparison(CurrentDeck.Intersect(controlWarrior).ToList(), controlWarrior, 2, DeckType.ControlWarrior))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.ControlWarrior;
                        return info;
                    }
                    var fatigueWarrior = new List<string> { JusticarTrueheart, Deathlord, RenoJackson, BouncingBlade, SludgeBelcher, ShieldSlam, Execute, Brawl }; //1
                    if (CoreComparison(CurrentDeck.Intersect(fatigueWarrior).ToList(), fatigueWarrior, 1, DeckType.FatigueWarrior))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.FatigueWarrior;
                        return info;
                    }
                    var faceWarrior = new List<string> { FieryWarAxe, DeathsBite, LeperGnome, ArcaniteReaper }; //0
                    if (CoreComparison(CurrentDeck.Intersect(faceWarrior).ToList(), faceWarrior, 1, DeckType.FaceWarrior))
                    {
                        info.DeckStyle = Style.Face;
                        info.DeckType = DeckType.FaceWarrior;
                        return info;
                    }
                    var dragonWarrior = new List<string> { BlackwingTechnician, BlackwingCorruptor, Nefarian, TwilightGuardian, Alexstrasza, AlexstraszasChampion }; //1
                    if (CoreComparison(CurrentDeck.Intersect(dragonWarrior).ToList(), dragonWarrior, 1, DeckType.DragonWarrior))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.DragonWarrior;
                        return info;
                    }
                    var mechWarrior = new List<string> { ClockworkGnome, Mechwarper, PilotedShredder, SpiderTank, FieryWarAxe, DeathsBite, ArcaniteReaper }; //1
                    if (CoreComparison(CurrentDeck.Intersect(mechWarrior).ToList(), mechWarrior, 1, DeckType.MechWarrior))
                    {
                        info.DeckStyle = Style.Aggro;
                        info.DeckType = DeckType.MechWarrior;
                        return info;
                    }
                    break;
                case Card.CClass.WARLOCK:
                    var demonHandlock = new List<string> { Voidcaller, MoltenGiant, MountainGiant, MalGanis, TwilightDrake }; //1
                    if (CoreComparison(CurrentDeck.Intersect(demonHandlock).ToList(), demonHandlock, 1, DeckType.DemonHandlock))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.DemonHandlock;
                        return info;
                    }
                    var dragonHandlock = new List<string> { TwilightGuardian, TwilightDrake, AzureDrake, Malygos, BlackwingCorruptor }; //1
                    if (CoreComparison(CurrentDeck.Intersect(dragonHandlock).ToList(), dragonHandlock, 1, DeckType.DragonHandlock))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.DragonHandlock;
                        return info;
                    }
                    var demonZooWarlock = new List<string> { FlameImp, Implosion, ImpGangBoss, Voidcaller, MalGanis, PowerOverwhelming }; //1
                    if (CoreComparison(CurrentDeck.Intersect(demonZooWarlock).ToList(), demonZooWarlock, 1, DeckType.DemonZooWarlock))
                    {
                        info.DeckStyle = Style.Aggro;
                        info.DeckType = DeckType.DemonZooWarlock;
                        return info;
                    }
                    var handlock = new List<string> { TwilightDrake, MoltenGiant, MoltenGiant, MountainGiant, Shadowflame, MortalCoil, LordJaraxxus, Hellfire, AntiqueHealbot }; //1
                    if (CoreComparison(CurrentDeck.Intersect(handlock).ToList(), handlock, 1, DeckType.Handlock))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.Handlock;
                        return info;
                    }
                    var zoolock = new List<string> { FlameImp, Voidcaller, PowerOverwhelming, Doomguard }; //1
                    var relinquary = new List<string> { ReliquarySeeker, Voidcaller, DarkPeddler, Implosion }; //1
                    if (CoreComparison(CurrentDeck.Intersect(relinquary).ToList(), relinquary, 1, DeckType.RelinquaryZoo, ReliquarySeeker))
                    {
                        info.DeckStyle = Style.Aggro;
                        info.DeckType = DeckType.RelinquaryZoo;
                        return info;
                    }
                    if (CoreComparison(CurrentDeck.Intersect(zoolock).ToList(), zoolock, 1, DeckType.Zoolock))
                    {
                        info.DeckStyle = Style.Aggro;
                        info.DeckType = DeckType.Zoolock;
                        return info;
                    }

                    break;
                case Card.CClass.HUNTER:
                    List<string> midRangeHunter = new List<string> { Webspinner, KillCommand, MadScientist, FreezingTrap, SavannahHighmane }; //1
                    List<string> hybridHunter = new List<string> { LeperGnome, PilotedShredder, ArcaneGolem, SavannahHighmane }; //1
                    List<string> faceHunter = new List<string> { AbusiveSergeant, UnleashtheHounds, KnifeJuggler, ArcaneGolem, Wolfrider }; //1

                    if (CoreComparison(CurrentDeck.Intersect(midRangeHunter).ToList(), midRangeHunter, 1, DeckType.MidRangeHunter, SavannahHighmane))
                    {
                        if (CoreComparison(CurrentDeck.Intersect(hybridHunter).ToList(), hybridHunter, 1, DeckType.HybridHunter))
                        {
                            info.DeckStyle = Style.Aggro;
                            info.DeckType = DeckType.HybridHunter;
                            return info;
                        }
                        info.DeckStyle = Style.Aggro;
                        info.DeckType = DeckType.MidRangeHunter;
                        return info;
                    }

                    if (CoreComparison(CurrentDeck.Intersect(faceHunter).ToList(), faceHunter, 1, DeckType.FaceHunter) || EarlyCardsWight >= Face)
                    {
                        info.DeckStyle = Style.Face;
                        info.DeckType = DeckType.FaceHunter;
                        return info;
                    }

                    break;
                case Card.CClass.ROGUE:
                    var raptorRogue = new List<string> { UnearthedRaptor, NerubianEgg };
                    if (CoreComparison(CurrentDeck.Intersect(raptorRogue).ToList(), raptorRogue, 1, DeckType.RaptorRogue, UnearthedRaptor))
                    {
                        info.DeckStyle = Style.Tempo;
                        info.DeckType = DeckType.RaptorRogue;
                        return info;
                    }
                    var oilRogue = new List<string> { TinkersSharpswordOil, BloodmageThalnos, Eviscerate, Sap, Sprint, DeadlyPoison }; //1
                    if (CoreComparison(CurrentDeck.Intersect(oilRogue).ToList(), oilRogue, 1, DeckType.OilRogue))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.OilRogue;
                        return info;
                    }
                    var pirateRogue = new List<string> { ShipsCannon, SouthseaCaptain, SouthseaDeckhand, SkycapnKragg, DreadCorsair, BloodsailRaider, Buccaneer }; //1
                    if (CoreComparison(CurrentDeck.Intersect(pirateRogue).ToList(), pirateRogue, 1, DeckType.PirateRogue))
                    {
                        info.DeckStyle = Style.Aggro;
                        info.DeckType = DeckType.PirateRogue;
                        return info;
                    }
                    var burstRogue = new List<string> { LeperGnome, SouthseaDeckhand, ArcaneGolem, ArgentHorserider, PilotedShredder, DefiasRingleader, DeadlyPoison, ColdBlood, AssassinsBlade }; //1
                    if (CoreComparison(CurrentDeck.Intersect(burstRogue).ToList(), burstRogue, 1, DeckType.BurstRogue))
                    {
                        info.DeckStyle = Style.Face;
                        info.DeckType = DeckType.BurstRogue;
                        return info;
                    }
                    break;
                case Card.CClass.DRUID:
                    var midRangeDruid = new List<string> { ForceofNature, SavageRoar, AncientofLore }; //0
                    if (CoreComparison(CurrentDeck.Intersect(midRangeDruid).ToList(), midRangeDruid, 0, DeckType.MidRangeDruid))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.MidRangeDruid;
                        return info;
                    }
                    var tokenDruid = new List<string> { LivingRoots, VioletTeacher, PoweroftheWild }; //0
                    if (CoreComparison(CurrentDeck.Intersect(tokenDruid).ToList(), tokenDruid, 0, DeckType.TokenDruid))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.TokenDruid;
                        return info;
                    }
                    var rampDruid = new List<string> { Innervate, AncientofWar, AncientofLore, KeeperoftheGrove, WildGrowth, Cenarius, ShadeofNaxxramas, Swipe, BigGameHunter };
                    if (CoreComparison(CurrentDeck.Intersect(rampDruid).ToList(), rampDruid, 3, DeckType.RampDruid))
                    {
                        info.DeckStyle = Style.Control;
                        info.DeckType = DeckType.RampDruid;
                        return info;
                    }
                    var aggroDruid = new List<string> { LeperGnome, AbusiveSergeant, PoweroftheWild, SavageRoar, LivingRoots }; //1
                    if (CoreComparison(CurrentDeck.Intersect(aggroDruid).ToList(), aggroDruid, 2, DeckType.AggroDruid))
                    {
                        info.DeckStyle = Style.Face;
                        info.DeckType = DeckType.AggroDruid;
                        return info;
                    }
                    break;
            }
            info.DeckStyle = Style.Unknown;
            info.DeckType = DeckType.Unknown;
            return info;
        }

        private void SetDefaultsForStyle(Style ds)
        {
            switch (ds)
            {
                case Style.Aggro:
                    Allowed1Drops = _hasCoin ? 4 : 3;
                    Allowed2Drops = _hasCoin ? 3 : 2; //allow 3 on coin, 1 wihtout
                    Allowed3Drops = _hasCoin ? 2 : 1; //allows 2 on coin, 1 without
                    Allowed4Drops = 1;
                    break;
                case Style.Control:
                    Allowed1Drops = 1;
                    Allowed2Drops = _hasCoin ? 3 : 2; //allow 3 on coin, 1 wihtout
                    Allowed3Drops = _hasCoin ? 2 : 1; //allows 2 on coin, 1 without
                    Allowed4Drops = 2;
                    break;
                case Style.Tempo:
                    Allowed1Drops = 1;
                    Allowed2Drops = _hasCoin ? 3 : 2; //allow 3 on coin, 1 wihtout
                    Allowed3Drops = _hasCoin ? 2 : 1; //allows 2 on coin, 1 without
                    Allowed4Drops = 1;
                    break;
                case Style.Face:
                    Allowed1Drops = _hasCoin ? 4 : 3;
                    Allowed2Drops = _hasCoin ? 3 : 2; //allow 3 on coin, 1 wihtout
                    Allowed3Drops = _hasCoin ? 1 : 0; //allows 2 on coin, 1 without
                    Allowed4Drops = 0;
                    break;
                case Style.Midrange:
                    break;
                case Style.Combo:
                    break;
                default:
                    Allowed1Drops = 1;
                    Allowed2Drops = _hasCoin ? 3 : 2; //allow 3 on coin, 1 wihtout
                    Allowed3Drops = _hasCoin ? 2 : 1; //allows 2 on coin, 1 without
                    Allowed4Drops = 1;
                    break;
            }
        }

        private Style GetStyle()
        {
            Style res = AverageCost >= ControlConst
                ? Style.Control
                : (AverageCost < ControlConst) && (AverageCost >= TempoConst)
                    ? Style.Tempo
                    : (AverageCost < 3) ? Style.Aggro : Style.Tempo;
            Bot.Log(string.Format("Average Cost {0}", AverageCost));
            SetDefaultsForStyle(res);
            //Bot.Log(string.Format("Num1 {0}, Num2{1} Num3{2} Weight{3} ", Num1DropsDeck, Num2DropsDeck, Num3DropsDeck, EarlyCardsWight));
            return EarlyCardsWight >= Face ? Style.Face : res;
        }
        /// <summary>
        /// Method copares your intersected with core list with the core. If it passes an accepted tolerance leven and 
        /// posseses a required card, it passes. 
        /// [Note] Reason I pass whole lists is to easily debug them in this method if needed. Ideally I would just pass List.Count 
        /// </summary>
        /// <param name="intersectedWithCoreList">List of core cards that intersect your deck</param>
        /// <param name="core">List of core cards used in comparison</param>
        /// <param name="acceptableError">Number of cards you are allowd to be off</param>
        /// <param name="type">Deck type, used for debugging only</param>
        /// <param name="required">Require card even if it satisfied the tolerance level</param>
        /// <returns>True, if your deck is similar to the core within the accepted tolerance level</returns>
        private static bool CoreComparison(List<string> intersectedWithCoreList, List<string> core, int acceptableError, DeckType type, string required = "")
        {
            bool flag = true;
            if (required != "")
                flag = intersectedWithCoreList.Any(c => c.ToString() == required.ToString());

            if (intersectedWithCoreList.Count <= core.Count && intersectedWithCoreList.Count >= core.Count - acceptableError)
                Bot.Log(string.Format("[SmartMulligan] Deck passed accepted tolerance level for {0}", type));
            //else Bot.Log(string.Format("[SmartMulligan] Deck failed accepted tolerance level for {0}", type));
            return flag && intersectedWithCoreList.Count <= core.Count && intersectedWithCoreList.Count >= core.Count - acceptableError;
        }


    }

    public class DeckData
    {
        public Style DeckStyle { get; set; }
        public DeckType DeckType { get; set; }
        public List<string> Cards { get; set; }
    }

    public enum DeckType
    {
        Unknown,
        Arena,
        ControlWarrior,
        FatigueWarrior,
        DragonWarrior,
        PatronWarrior,
        MechWarrior,
        FaceWarrior, //Missing Fatigue tweaks
        SecretPaladin,
        MidRangePaladin,
        AggroPaladin, //Complete
        RampDruid,
        AggroDruid,
        MidRangeDruid,
        TokenDruid, //Complete
        Handlock,
        Zoolock,
        RelinquaryZoo,
        DemonHandlock,
        DemonZooWarlock,
        DragonHandlock, //Missing ZooLock, DemonZoo
        TempoMage,
        FreezeMage,
        MechMage,
        EchoMage, //Missing Tempo
        DragonPriest,
        ControlPriest,
        ComboPriest, //Missing Combo
        MidRangeHunter,
        HybridHunter,
        FaceHunter, //Complete
        OilRogue,
        PirateRogue,
        BurstRogue, //Complete
        FaceShaman,
        MechShaman,
        DragonShaman,
        TotemShaman,
        MalygosShaman,
        ControlShaman, //Missing Dragon Shaman, MalyShaman
        RaptorRogue
    }

    public enum DragonDeck
    {
        Unknown,
        DragonPaladin,
        DragonPriest,
        DragonWarlock,
        DragonShaman,
        DragonMage,
        DragonWarrior,
        DragonRogue,
        DragonDruid,
        DragonHunter
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