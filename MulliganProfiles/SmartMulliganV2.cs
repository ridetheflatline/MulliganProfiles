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
    // ReSharper disable once InconsistentNaming
    public class bMulliganProfile : MulliganProfile
    {
        private double version = 3.01;
        /******************************************/
        /**************EDIT THIS LINE ONLY*********/
        /******************************************/
        private const bool TrackMulligan = true;
        private static bool IntroMessage = true;
        private const DeckType DebugDeckType = DeckType.Zoolock;
        private const Style DebugStyle = Style.Aggro;//no need to set up style for non arena decks
        /*If you chose not to be tracked, I won't be
         *             able to fix mulligan errors*/
        /******************************************/
        private const bool ArthursReasonToDrink = false; // I am not responsible for nuclear explosions in your archive folder if you chose to enable it
        /***********************************************************/
        /***********DO NOT EDIT ANYTHING BELOW THIS LINE************/
        /***********************************************************/
        private const int ControlConst = 4; //don't touch me
        private const int TempoConst = 3; //don't touch me
        private const double Face = 0.9; //don't touch me
        public static readonly string MainDir = AppDomain.CurrentDomain.BaseDirectory + "\\MulliganProfiles\\";
        private const string Archive = "MulliganArchives";
        private static readonly string StringForArthur = "needed to add new decks";

        #region cards

        private readonly Dictionary<string, string> CardsDictionary = new Dictionary<string, string>
        {
            {"Goldshire Footman", "CS1_042"},
            {"Holy Nova", "CS1_112"},
            {"Mind Control", "CS1_113"},
            {"Holy Smite", "CS1_130"},
            {"Mind Vision", "CS2_003"},
            {"Power Word: Shield", "CS2_004"},
            {"Claw", "CS2_005"},
            {"Healing Touch", "CS2_007"},
            {"Moonfire", "CS2_008"},
            {"Mark of the Wild", "CS2_009"},
            {"Savage Roar", "CS2_011"},
            {"Swipe", "CS2_012"},
            {"Wild Growth", "CS2_013"},
            {"Polymorph", "CS2_022"},
            {"Arcane Intellect", "CS2_023"},
            {"Frostbolt", "CS2_024"},
            {"Arcane Explosion", "CS2_025"},
            {"Frost Nova", "CS2_026"},
            {"Mirror Image", "CS2_027"},
            {"Fireball", "CS2_029"},
            {"Flamestrike", "CS2_032"},
            {"Water Elemental", "CS2_033"},
            {"Frost Shock", "CS2_037"},
            {"Windfury", "CS2_039"},
            {"Ancestral Healing", "CS2_041"},
            {"Fire Elemental", "CS2_042"},
            {"Rockbiter Weapon", "CS2_045"},
            {"Bloodlust", "CS2_046"},
            {"Shadow Bolt", "CS2_057"},
            {"Drain Life", "CS2_061"},
            {"Hellfire", "CS2_062"},
            {"Corruption", "CS2_063"},
            {"Dread Infernal", "CS2_064"},
            {"Voidwalker", "CS2_065"},
            {"Backstab", "CS2_072"},
            {"Deadly Poison", "CS2_074"},
            {"Sinister Strike", "CS2_075"},
            {"Assassinate", "CS2_076"},
            {"Sprint", "CS2_077"},
            {"Assassin's Blade", "CS2_080"},
            {"Hunter's Mark", "CS2_084"},
            {"Blessing of Might", "CS2_087"},
            {"Guardian of Kings", "CS2_088"},
            {"Holy Light", "CS2_089"},
            {"Light's Justice", "CS2_091"},
            {"Blessing of Kings", "CS2_092"},
            {"Consecration", "CS2_093"},
            {"Hammer of Wrath", "CS2_094"},
            {"Truesilver Champion", "CS2_097"},
            {"Charge", "CS2_103"},
            {"Heroic Strike", "CS2_105"},
            {"Fiery War Axe", "CS2_106"},
            {"Execute", "CS2_108"},
            {"Arcanite Reaper", "CS2_112"},
            {"Cleave", "CS2_114"},
            {"Magma Rager", "CS2_118"},
            {"Oasis Snapjaw", "CS2_119"},
            {"River Crocolisk", "CS2_120"},
            {"Frostwolf Grunt", "CS2_121"},
            {"Raid Leader", "CS2_122"},
            {"Wolfrider", "CS2_124"},
            {"Ironfur Grizzly", "CS2_125"},
            {"Silverback Patriarch", "CS2_127"},
            {"Stormwind Knight", "CS2_131"},
            {"Ironforge Rifleman", "CS2_141"},
            {"Kobold Geomancer", "CS2_142"},
            {"Gnomish Inventor", "CS2_147"},
            {"Stormpike Commando", "CS2_150"},
            {"Archmage", "CS2_155"},
            {"Lord of the Arena", "CS2_162"},
            {"Murloc Raider", "CS2_168"},
            {"Stonetusk Boar", "CS2_171"},
            {"Bloodfen Raptor", "CS2_172"},
            {"Bluegill Warrior", "CS2_173"},
            {"Sen'jin Shieldmasta", "CS2_179"},
            {"Chillwind Yeti", "CS2_182"},
            {"War Golem", "CS2_186"},
            {"Booty Bay Bodyguard", "CS2_187"},
            {"Elven Archer", "CS2_189"},
            {"Razorfen Hunter", "CS2_196"},
            {"Ogre Magi", "CS2_197"},
            {"Boulderfist Ogre", "CS2_200"},
            {"Core Hound", "CS2_201"},
            {"Reckless Rocketeer", "CS2_213"},
            {"Stormwind Champion", "CS2_222"},
            {"Frostwolf Warlord", "CS2_226"},
            {"Ironbark Protector", "CS2_232"},
            {"Shadow Word: Pain", "CS2_234"},
            {"Northshire Cleric", "CS2_235"},
            {"Divine Spirit", "CS2_236"},
            {"Starving Buzzard", "CS2_237"},
            {"Darkscale Healer", "DS1_055"},
            {"Houndmaster", "DS1_070"},
            {"Timber Wolf", "DS1_175"},
            {"Tundra Rhino", "DS1_178"},
            {"Multi-Shot", "DS1_183"},
            {"Tracking", "DS1_184"},
            {"Arcane Shot", "DS1_185"},
            {"Mind Blast", "DS1_233"},
            {"Voodoo Doctor", "EX1_011"},
            {"Novice Engineer", "EX1_015"},
            {"Shattered Sun Cleric", "EX1_019"},
            {"Dragonling Mechanic", "EX1_025"},
            {"Acidic Swamp Ooze", "EX1_066"},
            {"Warsong Commander", "EX1_084"},
            {"Fan of Knives", "EX1_129"},
            {"Innervate", "EX1_169"},
            {"Starfire", "EX1_173"},
            {"Totemic Might", "EX1_244"},
            {"Hex", "EX1_246"},
            {"Arcane Missiles", "EX1_277"},
            {"Shiv", "EX1_278"},
            {"Mortal Coil", "EX1_302"},
            {"Succubus", "EX1_306"},
            {"Soulfire", "EX1_308"},
            {"Humility", "EX1_360"},
            {"Hand of Protection", "EX1_371"},
            {"Gurubashi Berserker", "EX1_399"},
            {"Whirlwind", "EX1_400"},
            {"Murloc Tidehunter", "EX1_506"},
            {"Grimscale Oracle", "EX1_508"},
            {"Kill Command", "EX1_539"},
            {"Flametongue Totem", "EX1_565"},
            {"Sap", "EX1_581"},
            {"Dalaran Mage", "EX1_582"},
            {"Windspeaker", "EX1_587"},
            {"Nightblade", "EX1_593"},
            {"Shield Block", "EX1_606"},
            {"Shadow Word: Death", "EX1_622"},
            {"Garrosh Hellscream", "HERO_01"},
            {"Thrall", "HERO_02"},
            {"Valeera Sanguinar", "HERO_03"},
            {"Uther Lightbringer", "HERO_04"},
            {"Rexxar", "HERO_05"},
            {"Malfurion Stormrage", "HERO_06"},
            {"Gul'dan", "HERO_07"},
            {"Jaina Proudmoore", "HERO_08"},
            {"Anduin Wrynn", "HERO_09"},
            {"Sacrificial Pact", "NEW1_003"},
            {"Vanish", "NEW1_004"},
            {"Kor'kron Elite", "NEW1_011"},
            {"Animal Companion", "NEW1_031"},
            {"Fen Creeper", "CS1_069"},
            {"Inner Fire", "CS1_129"},
            {"Blizzard", "CS2_028"},
            {"Ice Lance", "CS2_031"},
            {"Ancestral Spirit", "CS2_038"},
            {"Far Sight", "CS2_053"},
            {"Blood Imp", "CS2_059"},
            {"Cold Blood", "CS2_073"},
            {"Rampage", "CS2_104"},
            {"Earthen Ring Farseer", "CS2_117"},
            {"Southsea Deckhand", "CS2_146"},
            {"Silver Hand Knight", "CS2_151"},
            {"Ravenholdt Assassin", "CS2_161"},
            {"Young Dragonhawk", "CS2_169"},
            {"Injured Blademaster", "CS2_181"},
            {"Abusive Sergeant", "CS2_188"},
            {"Ironbeak Owl", "CS2_203"},
            {"Spiteful Smith", "CS2_221"},
            {"Venture Co. Mercenary", "CS2_227"},
            {"Wisp", "CS2_231"},
            {"Blade Flurry", "CS2_233"},
            {"Gladiator's Longbow", "DS1_188"},
            {"Lightwarden", "EX1_001"},
            {"The Black Knight", "EX1_002"},
            {"Young Priestess", "EX1_004"},
            {"Big Game Hunter", "EX1_005"},
            {"Alarm-o-Bot", "EX1_006"},
            {"Acolyte of Pain", "EX1_007"},
            {"Argent Squire", "EX1_008"},
            {"Angry Chicken", "EX1_009"},
            {"Worgen Infiltrator", "EX1_010"},
            {"Bloodmage Thalnos", "EX1_012"},
            {"King Mukla", "EX1_014"},
            {"Sylvanas Windrunner", "EX1_016"},
            {"Jungle Panther", "EX1_017"},
            {"Scarlet Crusader", "EX1_020"},
            {"Thrallmar Farseer", "EX1_021"},
            {"Silvermoon Guardian", "EX1_023"},
            {"Stranglethorn Tiger", "EX1_028"},
            {"Leper Gnome", "EX1_029"},
            {"Sunwalker", "EX1_032"},
            {"Windfury Harpy", "EX1_033"},
            {"Twilight Drake", "EX1_043"},
            {"Questing Adventurer", "EX1_044"},
            {"Ancient Watcher", "EX1_045"},
            {"Dark Iron Dwarf", "EX1_046"},
            {"Spellbreaker", "EX1_048"},
            {"Youthful Brewmaster", "EX1_049"},
            {"Coldlight Oracle", "EX1_050"},
            {"Mana Addict", "EX1_055"},
            {"Ancient Brewmaster", "EX1_057"},
            {"Sunfury Protector", "EX1_058"},
            {"Crazed Alchemist", "EX1_059"},
            {"Argent Commander", "EX1_067"},
            {"Pint-Sized Summoner", "EX1_076"},
            {"Secretkeeper", "EX1_080"},
            {"Mad Bomber", "EX1_082"},
            {"Tinkmaster Overspark", "EX1_083"},
            {"Mind Control Tech", "EX1_085"},
            {"Arcane Golem", "EX1_089"},
            {"Cabal Shadow Priest", "EX1_091"},
            {"Defender of Argus", "EX1_093"},
            {"Gadgetzan Auctioneer", "EX1_095"},
            {"Loot Hoarder", "EX1_096"},
            {"Abomination", "EX1_097"},
            {"Lorewalker Cho", "EX1_100"},
            {"Demolisher", "EX1_102"},
            {"Coldlight Seer", "EX1_103"},
            {"Mountain Giant", "EX1_105"},
            {"Cairne Bloodhoof", "EX1_110"},
            {"Leeroy Jenkins", "EX1_116"},
            {"Eviscerate", "EX1_124"},
            {"Betrayal", "EX1_126"},
            {"Conceal", "EX1_128"},
            {"Noble Sacrifice", "EX1_130"},
            {"Defias Ringleader", "EX1_131"},
            {"Eye for an Eye", "EX1_132"},
            {"Perdition's Blade", "EX1_133"},
            {"SI:7 Agent", "EX1_134"},
            {"Redemption", "EX1_136"},
            {"Headcrack", "EX1_137"},
            {"Shadowstep", "EX1_144"},
            {"Preparation", "EX1_145"},
            {"Wrath", "EX1_154"},
            {"Mark of Nature", "EX1_155"},
            {"Soul of the Forest", "EX1_158"},
            {"Power of the Wild", "EX1_160"},
            {"Naturalize", "EX1_161"},
            {"Dire Wolf Alpha", "EX1_162"},
            {"Nourish", "EX1_164"},
            {"Druid of the Claw", "EX1_165"},
            {"Keeper of the Grove", "EX1_166"},
            {"Emperor Cobra", "EX1_170"},
            {"Ancient of War", "EX1_178"},
            {"Lightning Bolt", "EX1_238"},
            {"Lava Burst", "EX1_241"},
            {"Dust Devil", "EX1_243"},
            {"Earth Shock", "EX1_245"},
            {"Stormforged Axe", "EX1_247"},
            {"Feral Spirit", "EX1_248"},
            {"Baron Geddon", "EX1_249"},
            {"Earth Elemental", "EX1_250"},
            {"Forked Lightning", "EX1_251"},
            {"Unbound Elemental", "EX1_258"},
            {"Lightning Storm", "EX1_259"},
            {"Ethereal Arcanist", "EX1_274"},
            {"Cone of Cold", "EX1_275"},
            {"Pyroblast", "EX1_279"},
            {"Frost Elemental", "EX1_283"},
            {"Azure Drake", "EX1_284"},
            {"Counterspell", "EX1_287"},
            {"Ice Barrier", "EX1_289"},
            {"Mirror Entity", "EX1_294"},
            {"Ice Block", "EX1_295"},
            {"Ragnaros the Firelord", "EX1_298"},
            {"Felguard", "EX1_301"},
            {"Shadowflame", "EX1_303"},
            {"Void Terror", "EX1_304"},
            {"Siphon Soul", "EX1_309"},
            {"Doomguard", "EX1_310"},
            {"Twisting Nether", "EX1_312"},
            {"Pit Lord", "EX1_313"},
            {"Summoning Portal", "EX1_315"},
            {"Power Overwhelming", "EX1_316"},
            {"Sense Demons", "EX1_317"},
            {"Flame Imp", "EX1_319"},
            {"Bane of Doom", "EX1_320"},
            {"Lord Jaraxxus", "EX1_323"},
            {"Silence", "EX1_332"},
            {"Shadow Madness", "EX1_334"},
            {"Lightspawn", "EX1_335"},
            {"Thoughtsteal", "EX1_339"},
            {"Lightwell", "EX1_341"},
            {"Mindgames", "EX1_345"},
            {"Divine Favor", "EX1_349"},
            {"Prophet Velen", "EX1_350"},
            {"Lay on Hands", "EX1_354"},
            {"Blessed Champion", "EX1_355"},
            {"Argent Protector", "EX1_362"},
            {"Blessing of Wisdom", "EX1_363"},
            {"Holy Wrath", "EX1_365"},
            {"Sword of Justice", "EX1_366"},
            {"Repentance", "EX1_379"},
            {"Aldor Peacekeeper", "EX1_382"},
            {"Tirion Fordring", "EX1_383"},
            {"Avenging Wrath", "EX1_384"},
            {"Tauren Warrior", "EX1_390"},
            {"Slam", "EX1_391"},
            {"Battle Rage", "EX1_392"},
            {"Amani Berserker", "EX1_393"},
            {"Mogu'shan Warden", "EX1_396"},
            {"Arathi Weaponsmith", "EX1_398"},
            {"Armorsmith", "EX1_402"},
            {"Shieldbearer", "EX1_405"},
            {"Brawl", "EX1_407"},
            {"Mortal Strike", "EX1_408"},
            {"Upgrade!", "EX1_409"},
            {"Shield Slam", "EX1_410"},
            {"Gorehowl", "EX1_411"},
            {"Raging Worgen", "EX1_412"},
            {"Grommash Hellscream", "EX1_414"},
            {"Murloc Warleader", "EX1_507"},
            {"Murloc Tidecaller", "EX1_509"},
            {"Patient Assassin", "EX1_522"},
            {"Scavenging Hyena", "EX1_531"},
            {"Misdirection", "EX1_533"},
            {"Savannah Highmane", "EX1_534"},
            {"Eaglehorn Bow", "EX1_536"},
            {"Explosive Shot", "EX1_537"},
            {"Unleash the Hounds", "EX1_538"},
            {"King Krush", "EX1_543"},
            {"Flare", "EX1_544"},
            {"Bestial Wrath", "EX1_549"},
            {"Snake Trap", "EX1_554"},
            {"Harvest Golem", "EX1_556"},
            {"Nat Pagle", "EX1_557"},
            {"Harrison Jones", "EX1_558"},
            {"Archmage Antonidas", "EX1_559"},
            {"Nozdormu", "EX1_560"},
            {"Alexstrasza", "EX1_561"},
            {"Onyxia", "EX1_562"},
            {"Malygos", "EX1_563"},
            {"Faceless Manipulator", "EX1_564"},
            {"Doomhammer", "EX1_567"},
            {"Bite", "EX1_570"},
            {"Force of Nature", "EX1_571"},
            {"Ysera", "EX1_572"},
            {"Cenarius", "EX1_573"},
            {"Mana Tide Totem", "EX1_575"},
            {"The Beast", "EX1_577"},
            {"Savagery", "EX1_578"},
            {"Priestess of Elune", "EX1_583"},
            {"Ancient Mage", "EX1_584"},
            {"Sea Giant", "EX1_586"},
            {"Blood Knight", "EX1_590"},
            {"Auchenai Soulpriest", "EX1_591"},
            {"Vaporize", "EX1_594"},
            {"Cult Master", "EX1_595"},
            {"Demonfire", "EX1_596"},
            {"Imp Master", "EX1_597"},
            {"Cruel Taskmaster", "EX1_603"},
            {"Frothing Berserker", "EX1_604"},
            {"Inner Rage", "EX1_607"},
            {"Sorcerer's Apprentice", "EX1_608"},
            {"Snipe", "EX1_609"},
            {"Explosive Trap", "EX1_610"},
            {"Freezing Trap", "EX1_611"},
            {"Kirin Tor Mage", "EX1_612"},
            {"Edwin VanCleef", "EX1_613"},
            {"Illidan Stormrage", "EX1_614"},
            {"Mana Wraith", "EX1_616"},
            {"Deadly Shot", "EX1_617"},
            {"Equality", "EX1_619"},
            {"Molten Giant", "EX1_620"},
            {"Circle of Healing", "EX1_621"},
            {"Temple Enforcer", "EX1_623"},
            {"Holy Fire", "EX1_624"},
            {"Shadowform", "EX1_625"},
            {"Mass Dispel", "EX1_626"},
            {"Kidnapper", "NEW1_005"},
            {"Starfall", "NEW1_007"},
            {"Ancient of Lore", "NEW1_008"},
            {"Al'Akir the Windlord", "NEW1_010"},
            {"Mana Wyrm", "NEW1_012"},
            {"Master of Disguise", "NEW1_014"},
            {"Hungry Crab", "NEW1_017"},
            {"Bloodsail Raider", "NEW1_018"},
            {"Knife Juggler", "NEW1_019"},
            {"Wild Pyromancer", "NEW1_020"},
            {"Doomsayer", "NEW1_021"},
            {"Dread Corsair", "NEW1_022"},
            {"Faerie Dragon", "NEW1_023"},
            {"Captain Greenskin", "NEW1_024"},
            {"Bloodsail Corsair", "NEW1_025"},
            {"Violet Teacher", "NEW1_026"},
            {"Southsea Captain", "NEW1_027"},
            {"Millhouse Manastorm", "NEW1_029"},
            {"Deathwing", "NEW1_030"},
            {"Commanding Shout", "NEW1_036"},
            {"Master Swordsmith", "NEW1_037"},
            {"Gruul", "NEW1_038"},
            {"Hogger", "NEW1_040"},
            {"Stampeding Kodo", "NEW1_041"},
            {"Flesheating Ghoul", "tt_004"},
            {"Spellbender", "tt_010"},
            {"Magni Bronzebeard", "HERO_01a"},
            {"Alleria Windrunner", "HERO_05a"},
            {"Medivh", "HERO_08a"},
            {"Murloc Tinyfin", "LOEA10_3"},
            {"Forgotten Torch", "LOE_002"},
            {"Ethereal Conjurer", "LOE_003"},
            {"Museum Curator", "LOE_006"},
            {"Curse of Rafaam", "LOE_007"},
            {"Obsidian Destroyer", "LOE_009"},
            {"Pit Snake", "LOE_010"},
            {"Reno Jackson", "LOE_011"},
            {"Tomb Pillager", "LOE_012"},
            {"Rumbling Elemental", "LOE_016"},
            {"Keeper of Uldaman", "LOE_017"},
            {"Tunnel Trogg", "LOE_018"},
            {"Unearthed Raptor", "LOE_019"},
            {"Desert Camel", "LOE_020"},
            {"Dart Trap", "LOE_021"},
            {"Fierce Monkey", "LOE_022"},
            {"Dark Peddler", "LOE_023"},
            {"Anyfin Can Happen", "LOE_026"},
            {"Sacred Trial", "LOE_027"},
            {"Jeweled Scarab", "LOE_029"},
            {"Naga Sea Witch", "LOE_038"},
            {"Gorillabot A-3", "LOE_039"},
            {"Huge Toad", "LOE_046"},
            {"Tomb Spider", "LOE_047"},
            {"Mounted Raptor", "LOE_050"},
            {"Jungle Moonkin", "LOE_051"},
            {"Djinni of Zephyrs", "LOE_053"},
            {"Anubisath Sentinel", "LOE_061"},
            {"Fossilized Devilsaur", "LOE_073"},
            {"Sir Finley Mrrgglton", "LOE_076"},
            {"Brann Bronzebeard", "LOE_077"},
            {"Elise Starseeker", "LOE_079"},
            {"Summoning Stone", "LOE_086"},
            {"Wobbling Runts", "LOE_089"},
            {"Arch-Thief Rafaam", "LOE_092"},
            {"Entomb", "LOE_104"},
            {"Explorer's Hat", "LOE_105"},
            {"Eerie Statue", "LOE_107"},
            {"Ancient Shade", "LOE_110"},
            {"Excavated Evil", "LOE_111"},
            {"Everyfin is Awesome", "LOE_113"},
            {"Raven Idol", "LOE_115"},
            {"Reliquary Seeker", "LOE_116"},
            {"Cursed Blade", "LOE_118"},
            {"Animated Armor", "LOE_119"},
            {"Old Murk-Eye", "EX1_062"},
            {"Captain's Parrot", "NEW1_016"},
            {"Gelbin Mekkatorque", "EX1_112"},
            {"Elite Tauren Chieftain", "PRO_001"},
            {"Zombie Chow", "FP1_001"},
            {"Haunted Creeper", "FP1_002"},
            {"Echoing Ooze", "FP1_003"},
            {"Mad Scientist", "FP1_004"},
            {"Shade of Naxxramas", "FP1_005"},
            {"Nerubian Egg", "FP1_007"},
            {"Spectral Knight", "FP1_008"},
            {"Deathlord", "FP1_009"},
            {"Maexxna", "FP1_010"},
            {"Webspinner", "FP1_011"},
            {"Sludge Belcher", "FP1_012"},
            {"Kel'Thuzad", "FP1_013"},
            {"Stalagg", "FP1_014"},
            {"Feugen", "FP1_015"},
            {"Wailing Soul", "FP1_016"},
            {"Nerub'ar Weblord", "FP1_017"},
            {"Duplicate", "FP1_018"},
            {"Poison Seeds", "FP1_019"},
            {"Avenge", "FP1_020"},
            {"Death's Bite", "FP1_021"},
            {"Voidcaller", "FP1_022"},
            {"Dark Cultist", "FP1_023"},
            {"Unstable Ghoul", "FP1_024"},
            {"Reincarnate", "FP1_025"},
            {"Anub'ar Ambusher", "FP1_026"},
            {"Stoneskin Gargoyle", "FP1_027"},
            {"Undertaker", "FP1_028"},
            {"Dancing Swords", "FP1_029"},
            {"Loatheb", "FP1_030"},
            {"Baron Rivendare", "FP1_031"},
            {"Flamecannon", "GVG_001"},
            {"Snowchugger", "GVG_002"},
            {"Unstable Portal", "GVG_003"},
            {"Goblin Blastmage", "GVG_004"},
            {"Echo of Medivh", "GVG_005"},
            {"Mechwarper", "GVG_006"},
            {"Flame Leviathan", "GVG_007"},
            {"Lightbomb", "GVG_008"},
            {"Shadowbomber", "GVG_009"},
            {"Velen's Chosen", "GVG_010"},
            {"Shrinkmeister", "GVG_011"},
            {"Light of the Naaru", "GVG_012"},
            {"Cogmaster", "GVG_013"},
            {"Vol'jin", "GVG_014"},
            {"Darkbomb", "GVG_015"},
            {"Fel Reaver", "GVG_016"},
            {"Call Pet", "GVG_017"},
            {"Mistress of Pain", "GVG_018"},
            {"Demonheart", "GVG_019"},
            {"Fel Cannon", "GVG_020"},
            {"Mal'Ganis", "GVG_021"},
            {"Tinker's Sharpsword Oil", "GVG_022"},
            {"Goblin Auto-Barber", "GVG_023"},
            {"Cogmaster's Wrench", "GVG_024"},
            {"One-eyed Cheat", "GVG_025"},
            {"Feign Death", "GVG_026"},
            {"Iron Sensei", "GVG_027"},
            {"Trade Prince Gallywix", "GVG_028"},
            {"Ancestor's Call", "GVG_029"},
            {"Anodized Robo Cub", "GVG_030"},
            {"Recycle", "GVG_031"},
            {"Grove Tender", "GVG_032"},
            {"Tree of Life", "GVG_033"},
            {"Mech-Bear-Cat", "GVG_034"},
            {"Malorne", "GVG_035"},
            {"Powermace", "GVG_036"},
            {"Whirling Zap-o-matic", "GVG_037"},
            {"Crackle", "GVG_038"},
            {"Vitality Totem", "GVG_039"},
            {"Siltfin Spiritwalker", "GVG_040"},
            {"Dark Wispers", "GVG_041"},
            {"Neptulon", "GVG_042"},
            {"Glaivezooka", "GVG_043"},
            {"Spider Tank", "GVG_044"},
            {"Imp-losion", "GVG_045"},
            {"King of Beasts", "GVG_046"},
            {"Sabotage", "GVG_047"},
            {"Metaltooth Leaper", "GVG_048"},
            {"Gahz'rilla", "GVG_049"},
            {"Bouncing Blade", "GVG_050"},
            {"Warbot", "GVG_051"},
            {"Crush", "GVG_052"},
            {"Shieldmaiden", "GVG_053"},
            {"Ogre Warmaul", "GVG_054"},
            {"Screwjank Clunker", "GVG_055"},
            {"Iron Juggernaut", "GVG_056"},
            {"Seal of Light", "GVG_057"},
            {"Shielded Minibot", "GVG_058"},
            {"Coghammer", "GVG_059"},
            {"Quartermaster", "GVG_060"},
            {"Muster for Battle", "GVG_061"},
            {"Cobalt Guardian", "GVG_062"},
            {"Bolvar Fordragon", "GVG_063"},
            {"Puddlestomper", "GVG_064"},
            {"Ogre Brute", "GVG_065"},
            {"Dunemaul Shaman", "GVG_066"},
            {"Stonesplinter Trogg", "GVG_067"},
            {"Burly Rockjaw Trogg", "GVG_068"},
            {"Antique Healbot", "GVG_069"},
            {"Salty Dog", "GVG_070"},
            {"Lost Tallstrider", "GVG_071"},
            {"Shadowboxer", "GVG_072"},
            {"Cobra Shot", "GVG_073"},
            {"Kezan Mystic", "GVG_074"},
            {"Ship's Cannon", "GVG_075"},
            {"Explosive Sheep", "GVG_076"},
            {"Anima Golem", "GVG_077"},
            {"Mechanical Yeti", "GVG_078"},
            {"Force-Tank MAX", "GVG_079"},
            {"Druid of the Fang", "GVG_080"},
            {"Gilblin Stalker", "GVG_081"},
            {"Clockwork Gnome", "GVG_082"},
            {"Upgraded Repair Bot", "GVG_083"},
            {"Flying Machine", "GVG_084"},
            {"Annoy-o-Tron", "GVG_085"},
            {"Siege Engine", "GVG_086"},
            {"Steamwheedle Sniper", "GVG_087"},
            {"Ogre Ninja", "GVG_088"},
            {"Illuminator", "GVG_089"},
            {"Madder Bomber", "GVG_090"},
            {"Arcane Nullifier X-21", "GVG_091"},
            {"Gnomish Experimenter", "GVG_092"},
            {"Target Dummy", "GVG_093"},
            {"Jeeves", "GVG_094"},
            {"Goblin Sapper", "GVG_095"},
            {"Piloted Shredder", "GVG_096"},
            {"Lil' Exorcist", "GVG_097"},
            {"Gnomeregan Infantry", "GVG_098"},
            {"Bomb Lobber", "GVG_099"},
            {"Floating Watcher", "GVG_100"},
            {"Scarlet Purifier", "GVG_101"},
            {"Tinkertown Technician", "GVG_102"},
            {"Micro Machine", "GVG_103"},
            {"Hobgoblin", "GVG_104"},
            {"Piloted Sky Golem", "GVG_105"},
            {"Junkbot", "GVG_106"},
            {"Enhance-o Mechano", "GVG_107"},
            {"Recombobulator", "GVG_108"},
            {"Mini-Mage", "GVG_109"},
            {"Dr. Boom", "GVG_110"},
            {"Mimiron's Head", "GVG_111"},
            {"Mogor the Ogre", "GVG_112"},
            {"Foe Reaper 4000", "GVG_113"},
            {"Sneed's Old Shredder", "GVG_114"},
            {"Toshley", "GVG_115"},
            {"Mekgineer Thermaplugg", "GVG_116"},
            {"Gazlowe", "GVG_117"},
            {"Troggzor the Earthinator", "GVG_118"},
            {"Blingtron 3000", "GVG_119"},
            {"Hemet Nesingwary", "GVG_120"},
            {"Clockwork Giant", "GVG_121"},
            {"Wee Spellstopper", "GVG_122"},
            {"Soot Spewer", "GVG_123"},
            {"Solemn Vigil", "BRM_001"},
            {"Flamewaker", "BRM_002"},
            {"Dragon's Breath", "BRM_003"},
            {"Twilight Whelp", "BRM_004"},
            {"Demonwrath", "BRM_005"},
            {"Imp Gang Boss", "BRM_006"},
            {"Gang Up", "BRM_007"},
            {"Dark Iron Skulker", "BRM_008"},
            {"Volcanic Lumberer", "BRM_009"},
            {"Druid of the Flame", "BRM_010"},
            {"Lava Shock", "BRM_011"},
            {"Fireguard Destroyer", "BRM_012"},
            {"Quick Shot", "BRM_013"},
            {"Core Rager", "BRM_014"},
            {"Revenge", "BRM_015"},
            {"Axe Flinger", "BRM_016"},
            {"Resurrect", "BRM_017"},
            {"Dragon Consort", "BRM_018"},
            {"Grim Patron", "BRM_019"},
            {"Dragonkin Sorcerer", "BRM_020"},
            {"Dragon Egg", "BRM_022"},
            {"Drakonid Crusher", "BRM_024"},
            {"Volcanic Drake", "BRM_025"},
            {"Hungry Dragon", "BRM_026"},
            {"Majordomo Executus", "BRM_027"},
            {"Emperor Thaurissan", "BRM_028"},
            {"Rend Blackhand", "BRM_029"},
            {"Nefarian", "BRM_030"},
            {"Chromaggus", "BRM_031"},
            {"Blackwing Technician", "BRM_033"},
            {"Blackwing Corruptor", "BRM_034"},
            {"Flame Lance", "AT_001"},
            {"Effigy", "AT_002"},
            {"Fallen Hero", "AT_003"},
            {"Arcane Blast", "AT_004"},
            {"Polymorph: Boar", "AT_005"},
            {"Dalaran Aspirant", "AT_006"},
            {"Spellslinger", "AT_007"},
            {"Coldarra Drake", "AT_008"},
            {"Rhonin", "AT_009"},
            {"Ram Wrangler", "AT_010"},
            {"Holy Champion", "AT_011"},
            {"Spawn of Shadows", "AT_012"},
            {"Power Word: Glory", "AT_013"},
            {"Shadowfiend", "AT_014"},
            {"Convert", "AT_015"},
            {"Confuse", "AT_016"},
            {"Twilight Guardian", "AT_017"},
            {"Confessor Paletress", "AT_018"},
            {"Dreadsteed", "AT_019"},
            {"Fearsome Doomguard", "AT_020"},
            {"Tiny Knight of Evil", "AT_021"},
            {"Fist of Jaraxxus", "AT_022"},
            {"Void Crusher", "AT_023"},
            {"Demonfuse", "AT_024"},
            {"Dark Bargain", "AT_025"},
            {"Wrathguard", "AT_026"},
            {"Wilfred Fizzlebang", "AT_027"},
            {"Shado-Pan Rider", "AT_028"},
            {"Buccaneer", "AT_029"},
            {"Undercity Valiant", "AT_030"},
            {"Cutpurse", "AT_031"},
            {"Shady Dealer", "AT_032"},
            {"Burgle", "AT_033"},
            {"Poisoned Blade", "AT_034"},
            {"Beneath the Grounds", "AT_035"},
            {"Anub'arak", "AT_036"},
            {"Living Roots", "AT_037"},
            {"Darnassus Aspirant", "AT_038"},
            {"Savage Combatant", "AT_039"},
            {"Wildwalker", "AT_040"},
            {"Knight of the Wild", "AT_041"},
            {"Druid of the Saber", "AT_042"},
            {"Astral Communion", "AT_043"},
            {"Mulch", "AT_044"},
            {"Aviana", "AT_045"},
            {"Tuskarr Totemic", "AT_046"},
            {"Draenei Totemcarver", "AT_047"},
            {"Healing Wave", "AT_048"},
            {"Thunder Bluff Valiant", "AT_049"},
            {"Charged Hammer", "AT_050"},
            {"Elemental Destruction", "AT_051"},
            {"Totem Golem", "AT_052"},
            {"Ancestral Knowledge", "AT_053"},
            {"The Mistcaller", "AT_054"},
            {"Flash Heal", "AT_055"},
            {"Powershot", "AT_056"},
            {"Stablemaster", "AT_057"},
            {"King's Elekk", "AT_058"},
            {"Brave Archer", "AT_059"},
            {"Bear Trap", "AT_060"},
            {"Lock and Load", "AT_061"},
            {"Ball of Spiders", "AT_062"},
            {"Acidmaw", "AT_063"},
            {"Dreadscale", "AT_063t"},
            {"Bash", "AT_064"},
            {"King's Defender", "AT_065"},
            {"Orgrimmar Aspirant", "AT_066"},
            {"Magnataur Alpha", "AT_067"},
            {"Bolster", "AT_068"},
            {"Sparring Partner", "AT_069"},
            {"Skycap'n Kragg", "AT_070"},
            {"Alexstrasza's Champion", "AT_071"},
            {"Varian Wrynn", "AT_072"},
            {"Competitive Spirit", "AT_073"},
            {"Seal of Champions", "AT_074"},
            {"Warhorse Trainer", "AT_075"},
            {"Murloc Knight", "AT_076"},
            {"Argent Lance", "AT_077"},
            {"Enter the Coliseum", "AT_078"},
            {"Mysterious Challenger", "AT_079"},
            {"Garrison Commander", "AT_080"},
            {"Eadric the Pure", "AT_081"},
            {"Lowly Squire", "AT_082"},
            {"Dragonhawk Rider", "AT_083"},
            {"Lance Carrier", "AT_084"},
            {"Maiden of the Lake", "AT_085"},
            {"Saboteur", "AT_086"},
            {"Argent Horserider", "AT_087"},
            {"Mogor's Champion", "AT_088"},
            {"Boneguard Lieutenant", "AT_089"},
            {"Mukla's Champion", "AT_090"},
            {"Tournament Medic", "AT_091"},
            {"Ice Rager", "AT_092"},
            {"Frigid Snobold", "AT_093"},
            {"Flame Juggler", "AT_094"},
            {"Silent Knight", "AT_095"},
            {"Clockwork Knight", "AT_096"},
            {"Tournament Attendee", "AT_097"},
            {"Sideshow Spelleater", "AT_098"},
            {"Kodorider", "AT_099"},
            {"Silver Hand Regent", "AT_100"},
            {"Pit Fighter", "AT_101"},
            {"Captured Jormungar", "AT_102"},
            {"North Sea Kraken", "AT_103"},
            {"Tuskarr Jouster", "AT_104"},
            {"Injured Kvaldir", "AT_105"},
            {"Light's Champion", "AT_106"},
            {"Armored Warhorse", "AT_108"},
            {"Argent Watchman", "AT_109"},
            {"Coliseum Manager", "AT_110"},
            {"Refreshment Vendor", "AT_111"},
            {"Master Jouster", "AT_112"},
            {"Recruiter", "AT_113"},
            {"Evil Heckler", "AT_114"},
            {"Fencing Coach", "AT_115"},
            {"Wyrmrest Agent", "AT_116"},
            {"Master of Ceremonies", "AT_117"},
            {"Grand Crusader", "AT_118"},
            {"Kvaldir Raider", "AT_119"},
            {"Frost Giant", "AT_120"},
            {"Crowd Favorite", "AT_121"},
            {"Gormok the Impaler", "AT_122"},
            {"Chillmaw", "AT_123"},
            {"Bolf Ramshield", "AT_124"},
            {"Icehowl", "AT_125"},
            {"Nexus-Champion Saraad", "AT_127"},
            {"The Skeleton Knight", "AT_128"},
            {"Fjola Lightbane", "AT_129"},
            {"Sea Reaver", "AT_130"},
            {"Eydis Darkbane", "AT_131"},
            {"Justicar Trueheart", "AT_132"},
            {"Gadgetzan Jouster", "AT_133"},

        };
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

        #region variables

        private const int UnknownTreshhold = 7;
        private static List<Card.Cards> _ch;
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
        public static double AverageCost { get; set; }
        public static double EarlyCardsWight { get; set; }
        public static int NumMinions { get; set; }
        public static int NumWeapons { get; set; }
        public static int NumSpells { get; set; }
        public static int NumSecrets { get; set; }

        //has in choices
        private static bool _has1Drop;
        private static bool _has2Drop;
        private static bool _has3Drop;
        private static bool _has4Drop;
        private static bool _hasWeapon;

        public static List<string> CurrentDeck = new List<string>();
        private const string Coin = "GAME_005";

        private readonly Dictionary<string, bool> _whiteList; // CardName, KeepDouble
        private readonly List<Card.Cards> _cardsToKeep;
        private static List<Card.Cards> _ctk; //static cards to keep

        private static Card.CClass _oc; //opponent class
        private static Card.CClass _ownC; //own class
        private static bool _secretClass;
        private static bool _secretClassEnemy;
        private static bool _aggro; //aggro or passive opponent
        private static bool _wc; //weapon class

        #endregion

        public bMulliganProfile()
        {
            _whiteList = new Dictionary<string, bool> { { Coin, true }, { Innervate, true }, { WildGrowth, false } };
            _cardsToKeep = new List<Card.Cards>();
        }


        public List<Card.Cards> HandleMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            #region intro
            _ch = choices;
            _hasCoin = choices.Count > 3;
            bool debuggerFlag = false;
            
            #endregion


            try
            {

                CurrentDeck = Bot.CurrentDeck().Cards.ToList();
                using (StreamWriter LastPlayedDeck = new StreamWriter(MainDir + "LastPlayedDeck.txt", false))
                {
                    foreach (var q in CurrentDeck.Distinct().ToList())
                        LastPlayedDeck.WriteLine("{0} {1}", CurrentDeck.Count(c => c == q), CardTemplate.LoadFromId(q).Name);
                }
            }

            catch (Exception ex)
            {
                debuggerFlag = true;
                using (var fu = new StreamWriter(MainDir + "sm.v3"))
                {
                    fu.WriteLine("");
                }
                using (StreamReader deckReader = new StreamReader(MainDir + "LastPlayedDeck.txt"))
                {
                    string line;
                    while ((line = deckReader.ReadLine()) != null)
                    {
                        if (line == string.Empty) break;
                        CurrentDeck.Add(CardsDictionary[line.Substring(2)]);
                        if (line.Substring(0, 1) == "2")
                            CurrentDeck.Add(CardsDictionary[line.Substring(2)]);
                    }

                }
            }
            DefaultIni(opponentClass, ownClass);

            var myInfo = GetDeckInfo(ownClass);
            var opInfo = GetDeckInfo();
            if (debuggerFlag) //change those if you want to use mulligan tester. 
            {
                myInfo.DeckStyle = DebugStyle;
                myInfo.DeckType = DebugDeckType;
            }
            DefinePriorities(myInfo);
            ModifySpecialPriorities();
            var supported = true;
            ArchiveDeck(myInfo.DeckType);
            switch (myInfo.DeckType)
            {
                case DeckType.Unknown:
                    myInfo.DeckStyle = GetStyle();
                    HandleMinions(choices, _whiteList, myInfo);
                    HandleWeapons(choices, _whiteList);
                    HandleSpells(choices, _whiteList);
                    break;
                case DeckType.PatronWarrior:
                    HandlePatronMulligan(choices);
                    break;
                case DeckType.ControlWarrior:
                    HandleControlWarrior(choices);
                    break;
                case DeckType.SecretPaladin:
                    HandleSecretPaladin(choices, myInfo.DeckStyle);
                    break;
                case DeckType.MidRangeDruid:
                    HandleDruid(choices, myInfo);
                    break;
                case DeckType.TokenDruid:
                    HandleDruid(choices, myInfo);
                    break;
                case DeckType.DemonHandlock:
                    HandleHandlock(choices, myInfo);
                    break;
                case DeckType.DragonHandlock:
                    HandleDragons(choices, DragonDeck.DragonWarlock);
                    break;
                case DeckType.TempoMage:
                    HandleMage(choices);
                    break;
                case DeckType.DragonPriest:
                    HandleDragons(choices, DragonDeck.DragonPriest);
                    break;
                case DeckType.FreezeMage:
                    HandleFreezeMage(choices);
                    break;
                case DeckType.MidRangePaladin:
                    HandlePaladin(choices, myInfo.DeckStyle);
                    break;
                case DeckType.ControlPriest:
                    HandleControlPriest(myInfo);
                    break;
                case DeckType.DemonZooWarlock:
                    HandleZoo(choices, myInfo);
                    break;
                case DeckType.MidRangeHunter:
                    HandleHunter(choices, myInfo.DeckStyle);
                    break;
                case DeckType.MechMage:
                    HandleMechMage(choices);
                    break;
                case DeckType.Handlock:
                    HandleHandlock(choices, myInfo);
                    break;
                case DeckType.Zoolock:
                    HandleZoo(choices, myInfo);
                    break;
                case DeckType.HybridHunter:
                    HandleHunter(choices, myInfo.DeckStyle);
                    break;
                case DeckType.FatigueMage:
                    HandleEchoMage(choices);
                    break;
                case DeckType.AggroPaladin:
                    HandlePaladin(choices, myInfo.DeckStyle);
                    break;
                case DeckType.DragonWarrior:
                    HandleDragons(choices, DragonDeck.DragonWarrior);
                    break;
                case DeckType.FaceHunter:
                    HandleHunter(choices, myInfo.DeckStyle);
                    break;
                case DeckType.OilRogue:
                    HandleOilRogues(choices);
                    break;
                case DeckType.MechShaman:
                    supported = false;
                    break;
                case DeckType.DragonShaman:
                    supported = false;
                    break;
                case DeckType.TotemShaman:
                    HandleShaman(choices, myInfo.DeckType);
                    break;
                case DeckType.MalygosShaman:
                    supported = false;
                    break;
                case DeckType.ControlShaman:
                    supported = false;
                    break;
                case DeckType.MechWarrior:
                    HandleMechWarrior(choices);
                    break;
                case DeckType.RampDruid:
                    HandleDruid(choices, myInfo);
                    break;
                case DeckType.PirateRogue:
                    supported = false;
                    break;
                case DeckType.BurstRogue:
                    HandleBurstRogue(choices);
                    break;
                case DeckType.AggroDruid:
                    HandleDruid(choices, myInfo);
                    break;
                case DeckType.ComboPriest:
                    supported = false;
                    break;
                case DeckType.Arena:
                    SetDefaultsForStyle(myInfo.DeckStyle);
                    HandleMinions(choices, _whiteList, myInfo);
                    HandleWeapons(choices, _whiteList);
                    HandleSpells(choices, _whiteList);
                    break;
                case DeckType.FaceWarrior:
                    HandleFaceWarrior(choices);
                    break;
                case DeckType.FatigueWarrior:
                    HandleControlWarrior(choices, myInfo.DeckType);
                    break;

                case DeckType.FaceShaman:
                    HandleFaceShaman(choices, myInfo);
                    break;
                case DeckType.RaptorRogue:
                    HandleRaptorRogue(choices, myInfo);
                    break;
                case DeckType.BloodlustShaman:
                    supported = false;
                    break;
                case DeckType.RenoLock:
                    HandleRenoLock(myInfo);
                    break;
                case DeckType.AnyfinMurglMurgl:
                    HandleMurglMurgl(choices, myInfo);
                    break;
                case DeckType.MechPriest:
                    supported = false;
                    break;
                case DeckType.Basic:
                    SetDefaultsForStyle(myInfo.DeckStyle);
                    HandleMinions(choices, _whiteList, myInfo);
                    HandleWeapons(choices, _whiteList);
                    HandleSpells(choices, _whiteList);
                    break;
                default:
                    SetDefaultsForStyle(myInfo.DeckStyle);
                    HandleMinions(choices, _whiteList, myInfo);
                    HandleWeapons(choices, _whiteList);
                    HandleSpells(choices, _whiteList);
                    break;
            }

            switch (myInfo.DeckType)
            {
                case DeckType.Unknown:
                    Bot.Log(string.Format("[SmartMulligan] Current deck is {0}.  " + "However, my analysis shows that you are running {1} deck, so mulligan will treat it as normal {2} {3} deck", myInfo.DeckType, myInfo.DeckStyle, myInfo.DeckStyle, _ownC.ToString().ToLower()));
                    break;
                case DeckType.AnyfinMurglMurgl:
                    Bot.Log(string.Format("[SmartMulligan] Rwlrwl! {0} lgrlg. Raaauuorgrlgelgshmurglefurgleauugburlge auuurlr Arthur", myInfo.DeckType));
                    break;
                case DeckType.FaceShaman:
                    Bot.Log(string.Format("[SmartMulligan] Is that {0}? Shame on you, please go away. ", myInfo.DeckType));
                    break;
                default:
                    if (!supported)
                    {
                        Bot.Log(string.Format("[SmartMulligan] I haven't coded {0} mulligan logic.", myInfo.DeckType));
                        Bot.Log(string.Format("[SmartMulligan] But this mulligan will try to adjust properly with {0} Mulligan logic.", DeckType.Arena));
                        Bot.Log(string.Format("[SmartMulligan] Mulligan will treat it as {0} style deck. But please remember that It's most likely wrong ", myInfo.DeckStyle));
                        SetDefaultsForStyle(myInfo.DeckStyle);
                        HandleMinions(choices, _whiteList, myInfo);
                        HandleWeapons(choices, _whiteList);
                        HandleSpells(choices, _whiteList);
                    }

                    else
                        Bot.Log(string.Format("[SmartMulligan] Recognized {0} deck. If that is not true, please report it in SmartMulligan report thread", myInfo.DeckType));
                    break;
            }

            foreach (var s in from s in choices let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString()) where _whiteList.ContainsKey(s.ToString()) where !keptOneAlready | _whiteList[s.ToString()] select s)
                _cardsToKeep.Add(s);

            _ctk = _cardsToKeep;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (TrackMulligan) DisplayMulligans(choices, myInfo);

            return _cardsToKeep;
        }

        private List<string> ParseOpponentList(string[] check)
        {
            return new List<string>();
        }

        private const Card.CClass Mage = Card.CClass.MAGE;
        private const Card.CClass Paladin = Card.CClass.MAGE;
        private const Card.CClass Priest = Card.CClass.MAGE;
        private const Card.CClass Waclock = Card.CClass.MAGE;
        private const Card.CClass Hunter = Card.CClass.MAGE;
        private const Card.CClass Rogue = Card.CClass.MAGE;
        private const Card.CClass Warrior = Card.CClass.MAGE;
        private const Card.CClass Shaman = Card.CClass.MAGE;
        private const Card.CClass Druid = Card.CClass.MAGE;


        private void HandleControlPriest(DeckData myInfo)
        {
            HandleMinions(_ch, _whiteList, myInfo);
            HandleSpells(_ch, _whiteList);
            _whiteList.AddOrUpdate(_hasCoin && Against(Mage, Druid) ? Thoughtsteal : "", false);
            if (_aggro && ChoiceAnd(AuchenaiSoulpriest, CircleofHealing))
            {
                WhiteListAList(new List<string> { AuchenaiSoulpriest, CircleofHealing });
            }

        }


        private OpponentDeckData GetDeckInfo()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Plugins\\SmartTracker.cs"))
            {
                Bot.Log("[SmartMulligan] [SMTracker.cs] not found. Loading basic assumption" +
                "\nAggro Classes: Paladin, Warlock, Shaman, Hunter" +
                "\nControl classes: Priest, Warrior, Mage, Rogue");//Looks like shit in the bot log
                return null;
            }
            using (StreamReader HistoryReader = new StreamReader(MainDir + "OpponentDeckInfo.txt"))
            {
                string line;
                var opponentInfo = new OpponentDeckData { Identification = 200, DeckPreferencesDictionary = new Dictionary<DeckType, List<string>>() };
                while ((line = HistoryReader.ReadLine()) != null)
                {
                    string[] check = line.Split(':');
                    if (check[1] != Bot.GetCurrentOpponentId().ToString()) continue;
                    opponentInfo.Identification = Bot.GetCurrentOpponentId();
                    opponentInfo.DeckPreferencesDictionary.AddOrUpdate(
                        (DeckType)Enum.Parse(typeof(DeckType), check[2]),//Just an enum parser. 
                        ParseOpponentList(check));
                }
                if (opponentInfo.Identification != 200)
                {
                    Bot.Log("[Tracker] You have played this opponents before, he played");
                    foreach (var q in opponentInfo.DeckPreferencesDictionary)
                    {
                        Bot.Log(string.Format("{0}---{1}", q.Value, q.Key));
                    }
                    Bot.Log(string.Format(
                        "[Tracker] SmartMulligan is making an assumption that your opponent is {0}", opponentInfo.DeckPreferencesDictionary.First().Key));
                }
                else
                {
                    Bot.Log("[Tracker] This is the first time you are facing this opponent");
                }
                return opponentInfo;
            }
        }

        private void HandleMurglMurgl(List<Card.Cards> choices, DeckData myInfo)
        {
            PreFiveDrops.AddOrUpdate(WildPyromancer, 0);
            HandleSpells(choices, _whiteList);
            HandleWeapons(choices, _whiteList);
            foreach (var card in choices.Where(choice => CardTemplate.LoadFromId(choice).Cost <= 4 && CardTemplate.LoadFromId(choice).Type == Card.CType.MINION && CardTemplate.LoadFromId(choice).Race != Card.CRace.MURLOC))
            {
                switch (CardTemplate.LoadFromId(card).Cost)
                {
                    case 1:
                        _has1Drop = true;
                        _whiteList.AddOrUpdate(card.ToString(), _hasCoin || GetPriority(card.ToString()) >= 4);
                        break;
                    case 2:
                        if (!HasGoodDrop(1, 2) && !_hasCoin) continue;
                        if (_hasCoin) _whiteList.AddOrUpdate(card.ToString(), false);
                        _has2Drop = true;
                        _whiteList.AddOrUpdate(card.ToString(), GetPriority(card.ToString()) > 4 && _hasCoin && CardTemplate.LoadFromId(card).Overload == 0);
                        Num2Drops++;
                        break;
                    case 3:
                        if (!HasGoodDrop(1, 2) || !HasGoodDrop(2, 2)) continue;
                        _has3Drop = true;
                        _whiteList.AddOrUpdate(card.ToString(), false);
                        break;
                }
                //_whiteList.AddOrUpdate(card.ToString(), _hasCoin && GetPriority(card.ToString()) > 4);
            }
        }

        private void HandleFaceShaman(List<Card.Cards> choices, DeckData myInfo)
        {
            foreach (var card in choices.Where(choice => CardTemplate.LoadFromId(choice).Cost <= 3 && CardTemplate.LoadFromId(choice).Type == Card.CType.MINION))
            {
                switch (CardTemplate.LoadFromId(card).Cost)
                {
                    case 1:
                        _has1Drop = true;
                        _whiteList.AddOrUpdate(card.ToString(), _hasCoin || GetPriority(card.ToString()) >= 4);
                        break;
                    case 2:
                        if (!HasGoodDrop(1, 2) && !_hasCoin) continue;
                        if (_hasCoin) _whiteList.AddOrUpdate(card.ToString(), false);
                        _has2Drop = true;
                        _whiteList.AddOrUpdate(card.ToString(), GetPriority(card.ToString()) > 4 && _hasCoin && CardTemplate.LoadFromId(card).Overload == 0);
                        Num2Drops++;
                        break;
                    case 3:
                        if (!HasGoodDrop(1, 2) || !HasGoodDrop(2, 2)) continue;
                        _has3Drop = true;
                        _whiteList.AddOrUpdate(card.ToString(), false);
                        break;
                }
                //_whiteList.AddOrUpdate(card.ToString(), _hasCoin && GetPriority(card.ToString()) > 4);
            }
            if (_oc == Card.CClass.ROGUE || _oc == Card.CClass.WARRIOR)
                _whiteList.AddOrUpdate(_has1Drop && _has2Drop && _has3Drop ? Doomhammer : "", false);
            if (_oc == Card.CClass.MAGE || _oc == Card.CClass.DRUID)
                _whiteList.AddOrUpdate(ChoicesHasCard(LightningBolt) ? LightningBolt : HasGoodDrop(1, 2) && _hasCoin ? RockbiterWeapon : "", false);
        }

        private void DefinePriorities(DeckData data)
        {
            PreFiveDrops = new Dictionary<string, int>
                #region allpre5drops

            {
                {"CS2_231", 0}, //[1/1]Wisp [0 mana] [NONE card]
                {"LOEA10_3", 0}, //[1/1]Murloc Tinyfin [0 mana] [NONE card]
                {"GVG_093", 0}, //[0/2]Target Dummy [0 mana] [NONE card]
                {"CS1_042", 0}, //[1/2]Goldshire Footman [1 mana] [NONE card]
                {"CS2_065", 2}, //[1/3]Voidwalker [1 mana] [WARLOCK card]
                {"CS2_168", Num1DropsDeck < 3 ? 3 : 1}, //[2/1]Murloc Raider [1 mana] [NONE card]
                {"CS2_171", 2}, //[1/1]Stonetusk Boar [1 mana] [NONE card]
                {"CS2_189", 3}, //[1/1]Elven Archer [1 mana] [NONE card]
                {"CS2_235", 4}, //[1/3]Northshire Cleric [1 mana] [PRIEST card]
                {"DS1_175", 0}, //[1/1]Timber Wolf [1 mana] [HUNTER card]
                {"EX1_011", 1}, //[2/1]Voodoo Doctor [1 mana] [NONE card]
                {"EX1_508", 0}, //[1/1]Grimscale Oracle [1 mana] [NONE card]
                {"CS2_059", _hasCoin && _has1Drop ? 5 : 0}, //[0/1]Blood Imp [1 mana] [WARLOCK card]
                {"CS2_146", 1}, //[2/1]Southsea Deckhand [1 mana] [NONE card]
                {"CS2_169", 0}, //[1/1]Young Dragonhawk [1 mana] [NONE card]
                {"CS2_188", 4}, //[2/1]Abusive Sergeant [1 mana] [NONE card]
                {"EX1_001", 0}, //[1/2]Lightwarden [1 mana] [NONE card]
                {"EX1_004", 1}, //[2/1]Young Priestess [1 mana] [NONE card]
                {"EX1_008", 3}, //[1/1]Argent Squire [1 mana] [NONE card]
                {"EX1_009", 0}, //[1/1]Angry Chicken [1 mana] [NONE card]
                {"EX1_010", 4}, //[2/1]Worgen Infiltrator [1 mana] [NONE card]
                {"EX1_029", 3}, //[2/1]Leper Gnome [1 mana] [NONE card]
                {"EX1_080", _secretClass ? 4 : 0}, //[1/2]Secretkeeper [1 mana] [NONE card]
                {"EX1_243", -5}, //[3/1]Dust Devil [1 mana] [SHAMAN card]
                {"EX1_319", 5}, //[3/2]Flame Imp [1 mana] [WARLOCK card]
                {"EX1_405", 0}, //[0/4]Shieldbearer [1 mana] [NONE card]
                {"EX1_509", 0}, //[1/2]Murloc Tidecaller [1 mana] [NONE card]
                {"NEW1_012", 4}, //[1/3]Mana Wyrm [1 mana] [MAGE card]
                {"NEW1_017", NumMurlocs > 3 ? 2 : 0}, //[1/2]Hungry Crab [1 mana] [NONE card]
                {"NEW1_025", _wc ? 2 : 0}, //[1/2]Bloodsail Corsair [1 mana] [NONE card]
                {"LOE_010", 2}, //[2/1]Pit Snake [1 mana] [ROGUE card]
                {"LOE_018", 3}, //[1/3]Tunnel Trogg [1 mana] [SHAMAN card]
                {"LOE_076", _ownC == Card.CClass.SHAMAN ? 10 : 4}, //[1/3] Sir Finley Mrrgglton [1 mana] [NONE card]
                {"LOE_116", -1}, //[1/1]Reliquary Seeker [1 mana] [WARLOCK card]
                {"FP1_001", 10}, //[2/3]Zombie Chow [1 mana] [NONE card]
                {"FP1_011", 3}, //[1/1]Webspinner [1 mana] [HUNTER card]
                {"FP1_028", 0}, //[1/2]Undertaker [1 mana] [NONE card]
                {"GVG_009", 0}, //[2/1]Shadowbomber [1 mana] [PRIEST card]
                {"GVG_013", NumMechs > 4 || ChoicesHasRace(Card.CRace.MECH) ? 4 : 0}, //[1/2]Cogmaster [1 mana] [NONE card]
                {"GVG_051", 1}, //[1/3]Warbot [1 mana] [WARRIOR card]
                {"GVG_082", 3}, //[2/1]Clockwork Gnome [1 mana] [NONE card]
                {"BRM_004", NumDragons > 5 || ChoicesHasRace(Card.CRace.DRAGON, TwilightWhelp) ? 4 : 1}, //[2/1]Twilight Whelp [1 mana] [PRIEST card]
                {"BRM_022", 0}, //[0/2]Dragon Egg [1 mana] [NONE card]
                {"AT_029", 8}, //[2/1]Buccaneer [1 mana] [ROGUE card]
                {"AT_059", -1}, //[2/1]Brave Archer [1 mana] [HUNTER card]
                {"AT_082", 2}, //[1/2]Lowly Squire [1 mana] [NONE card]
                {"AT_097", 1}, //[2/1]Tournament Attendee [1 mana] [NONE card]
                {"AT_105", _ownC == Card.CClass.PRIEST ? 4 : 1}, //[2/4]Injured Kvaldir [1 mana] [NONE card]
                {"AT_133", 6}, //[1/2]Gadgetzan Jouster [1 mana] [NONE card]
                {"CS2_120", 3}, //[2/3]River Crocolisk [2 mana] [NONE card]
                {"CS2_121", 2}, //[2/2]Frostwolf Grunt [2 mana] [NONE card]
                {"CS2_142", _ownC == Card.CClass.ROGUE ? 3 : 1}, //[2/2]Kobold Geomancer [2 mana] [NONE card]
                {"CS2_172", 3}, //[3/2]Bloodfen Raptor [2 mana] [NONE card]
                {"CS2_173", 0}, //[2/1]Bluegill Warrior [2 mana] [NONE card]
                {
                    "EX1_015", data != null && data.DeckStyle == Style.Control && Num2DropsDeck < 3 ? 4 : 1
                }, //[1/1]Novice Engineer [2 mana] [NONE card]
                {"EX1_066", _wc ? 4 : 3}, //[3/2]Acidic Swamp Ooze [2 mana] [NONE card]
                {"EX1_306", 1}, //[4/3]Succubus [2 mana] [WARLOCK card]
                {"EX1_506", 2}, //[2/1]Murloc Tidehunter [2 mana] [NONE card]
                {"EX1_565", 4}, //[0/3]Flametongue Totem [2 mana] [SHAMAN card]
                {"CS2_203", 0}, //[2/1]Ironbeak Owl [2 mana] [NONE card]
                {"EX1_012", 2}, //[1/1]Bloodmage Thalnos [2 mana] [NONE card]
                {"EX1_045", 2}, //[4/5]Ancient Watcher [2 mana] [NONE card]
                {"EX1_049", 3}, //[3/2]Youthful Brewmaster [2 mana] [NONE card]
                {"EX1_055", 2}, //[1/3]Mana Addict [2 mana] [NONE card]
                {"EX1_058", 3}, //[2/3]Sunfury Protector [2 mana] [NONE card]
                {"EX1_059", 2}, //[2/2]Crazed Alchemist [2 mana] [NONE card]
                {"EX1_076", 1}, //[2/2]Pint-Sized Summoner [2 mana] [NONE card]
                {"EX1_082", 3}, //[3/2]Mad Bomber [2 mana] [NONE card]
                {"EX1_096", 4}, //[2/1]Loot Hoarder [2 mana] [NONE card]
                {"EX1_100", 0}, //[0/4]Lorewalker Cho [2 mana] [NONE card]
                {"EX1_131", _hasCoin ? 4 : 2}, //[2/2]Defias Ringleader [2 mana] [ROGUE card]
                {"EX1_162", 2}, //[2/2]Dire Wolf Alpha [2 mana] [NONE card]
                {"EX1_341", 2}, //[0/5]Lightwell [2 mana] [PRIEST card]
                {"EX1_362", 0}, //[2/2]Argent Protector [2 mana] [PALADIN card]
                {"EX1_393", 3}, //[2/3]Amani Berserker [2 mana] [NONE card]
                {"EX1_402", 2}, //[1/4]Armorsmith [2 mana] [WARRIOR card]
                {"EX1_522", 1}, //[1/1]Patient Assassin [2 mana] [ROGUE card]
                {"EX1_531", NumBeasts > 4 ? 3 : 2}, //[2/2]Scavenging Hyena [2 mana] [HUNTER card]
                {"EX1_557", 3}, //[0/4]Nat Pagle [2 mana] [NONE card]
                {"EX1_603", 2}, //[2/2]Cruel Taskmaster [2 mana] [WARRIOR card]
                {"EX1_608", 3}, //[3/2]Sorcerer's Apprentice [2 mana] [MAGE card]
                {"EX1_616", 2}, //[2/2]Mana Wraith [2 mana] [NONE card]
                {"NEW1_018", 3}, //[2/3]Bloodsail Raider [2 mana] [NONE card]
                {"NEW1_019", 6}, //[3/2]Knife Juggler [2 mana] [NONE card]
                {"NEW1_020", _ownC == Card.CClass.PRIEST ? 4 : 3}, //[3/2]Wild Pyromancer [2 mana] [NONE card]
                {"NEW1_021", 1}, //[0/7]Doomsayer [2 mana] [NONE card]
                {"NEW1_023", 3}, //[3/2]Faerie Dragon [2 mana] [NONE card]
                {"NEW1_029", 4}, //[4/4]Millhouse Manastorm [2 mana] [NONE card]
                {"NEW1_037", 2}, //[1/3]Master Swordsmith [2 mana] [NONE card]
                {"LOE_006", 2}, //[1/2]Museum Curator [2 mana] [PRIEST card]
                {"LOE_023", 4}, //[2/2]Dark Peddler [2 mana] [WARLOCK card]
                {"LOE_029", 4}, //[1/1]Jeweled Scarab [2 mana] [NONE card]
                {"LOE_046", 4}, //[3/2]Huge Toad [2 mana] [NONE card]
                {"NEW1_016", NumPirates > 4 ? 4 : 2}, //[1/1]Captain's Parrot [2 mana] [NONE card]
                {"FP1_002", 4}, //[1/2]Haunted Creeper [2 mana] [NONE card]
                {"FP1_003", 3}, //[1/2]Echoing Ooze [2 mana] [NONE card]
                {"FP1_004", _secretClass && NumSecrets > 0 ? 8 : 3}, //[2/2]Mad Scientist [2 mana] [NONE card]
                {"FP1_007", ChoiceOr(AbusiveSergeant, DireWolfAlpha, PowerOverwhelming) ? 5 : 0}, //[0/2]Nerubian Egg [2 mana] [NONE card]
                {"FP1_017", 2}, //[1/4]Nerub'ar Weblord [2 mana] [NONE card]
                {"FP1_024", 2}, //[1/3]Unstable Ghoul [2 mana] [NONE card]
                {"GVG_002", 4}, //[2/3]Snowchugger [2 mana] [MAGE card]
                {"GVG_006", 5}, //[2/3]Mechwarper [2 mana] [NONE card]
                {"GVG_011", 3}, //[3/2]Shrinkmeister [2 mana] [PRIEST card]
                {"GVG_018", 2}, //[1/4]Mistress of Pain [2 mana] [WARLOCK card]
                {"GVG_023", 5}, //[3/2]Goblin Auto-Barber [2 mana] [ROGUE card]
                {"GVG_025", 2}, //[4/1]One-eyed Cheat [2 mana] [ROGUE card]
                {"GVG_030", 3}, //[2/2]Anodized Robo Cub [2 mana] [DRUID card]
                {"GVG_037", _hasCoin ? 5 : 3}, //[3/2]Whirling Zap-o-matic [2 mana] [SHAMAN card]
                {"GVG_039", 0}, //[0/3]Vitality Totem [2 mana] [SHAMAN card]
                {"GVG_058", 5}, //[2/2]Shielded Minibot [2 mana] [PALADIN card]
                {"GVG_064", 3}, //[3/2]Puddlestomper [2 mana] [NONE card]
                {"GVG_067", 3}, //[2/3]Stonesplinter Trogg [2 mana] [NONE card]
                {"GVG_072", 4}, //[2/3]Shadowboxer [2 mana] [PRIEST card]
                {"GVG_075", ChoicesHasRace(Card.CRace.PIRATE) ? 5 : 3}, //[2/3]Ship's Cannon [2 mana] [NONE card]
                {"GVG_076", _ownC == Card.CClass.MAGE && _oc == Card.CClass.PALADIN ? 5 : 1}, //[1/1]Explosive Sheep [2 mana] [NONE card]
                {"GVG_081", 3}, //[2/3]Gilblin Stalker [2 mana] [NONE card]
                {"GVG_085", 2}, //[1/2]Annoy-o-Tron [2 mana] [NONE card]
                {"GVG_087", 3}, //[2/3]Steamwheedle Sniper [2 mana] [HUNTER card]
                {"GVG_103", 2}, //[1/2]Micro Machine [2 mana] [NONE card]
                {"GVG_108", 3}, //[3/2]Recombobulator [2 mana] [NONE card]
                {"AT_003", 3}, //[3/2]Fallen Hero [2 mana] [MAGE card]
                {"AT_021", 3}, //[3/2]Tiny Knight of Evil [2 mana] [WARLOCK card]
                {"AT_026", 2}, //[4/3]Wrathguard [2 mana] [WARLOCK card]
                {"AT_030", 4}, //[3/2]Undercity Valiant [2 mana] [ROGUE card]
                {"AT_031", 1}, //[2/2]Cutpurse [2 mana] [ROGUE card]
                {"AT_038", 10}, //[2/3]Darnassus Aspirant [2 mana] [DRUID card]
                {"AT_042", 2}, //[2/1]Druid of the Saber [2 mana] [DRUID card]
                {"AT_052", 5}, //[3/4]Totem Golem [2 mana] [SHAMAN card]
                {"AT_058", 3}, //[3/2]King's Elekk [2 mana] [HUNTER card]
                {"AT_069", 3}, //[3/2]Sparring Partner [2 mana] [WARRIOR card]
                {"AT_071", ChoicesHasRace(Card.CRace.DRAGON) ? 5 : 3}, //[2/3]Alexstrasza's Champion [2 mana] [WARRIOR card]
                {"AT_080", 3}, //[2/3]Garrison Commander [2 mana] [NONE card]
                {"AT_084", 2}, //[1/2]Lance Carrier [2 mana] [NONE card]
                {"AT_089", 4}, //[3/2]Boneguard Lieutenant [2 mana] [NONE card]
                {"AT_094", 3}, //[2/3]Flame Juggler [2 mana] [NONE card]
                {"AT_109", 2}, //[2/4]Argent Watchman [2 mana] [NONE card]
                {"AT_116", ChoicesHasRace(Card.CRace.DRAGON) ? 8 : 1}, //[1/4]Wyrmrest Agent [2 mana] [PRIEST card]
                {"CS2_118", -1}, //[5/1]Magma Rager [3 mana] [NONE card]
                {"CS2_122", 1}, //[2/2]Raid Leader [3 mana] [NONE card]
                {"CS2_124", -1}, //[3/1]Wolfrider [3 mana] [NONE card]
                {"CS2_125", 2}, //[3/3]Ironfur Grizzly [3 mana] [NONE card]
                {"CS2_127", 0}, //[1/4]Silverback Patriarch [3 mana] [NONE card]
                {"CS2_141", 2}, //[2/2]Ironforge Rifleman [3 mana] [NONE card]
                {"CS2_196", 3}, //[2/3]Razorfen Hunter [3 mana] [NONE card]
                {"EX1_019", 3}, //[3/2]Shattered Sun Cleric [3 mana] [NONE card]
                {"EX1_084", 1}, //[2/3]Warsong Commander [3 mana] [WARRIOR card]
                {"EX1_582", 1}, //[1/4]Dalaran Mage [3 mana] [NONE card]
                {"CS2_117", 3}, //[3/3]Earthen Ring Farseer [3 mana] [NONE card]
                {"CS2_181", 5}, //[4/7]Injured Blademaster [3 mana] [NONE card]
                {"EX1_005", 0}, //[4/2]Big Game Hunter [3 mana] [NONE card]
                {"EX1_006", 0}, //[0/3]Alarm-o-Bot [3 mana] [NONE card]
                {"EX1_007", _ownC == Card.CClass.MAGE && data != null && data.DeckStyle == Style.Control ? 5 : 2}, //[1/3]Acolyte of Pain [3 mana] [NONE card]
                {"EX1_014", 5}, //[5/5]King Mukla [3 mana] [NONE card]
                {"EX1_017", 4}, //[4/2]Jungle Panther [3 mana] [NONE card]
                {"EX1_020", 4}, //[3/1]Scarlet Crusader [3 mana] [NONE card]
                {"EX1_021", 1}, //[2/3]Thrallmar Farseer [3 mana] [NONE card]
                {"EX1_044", _hasCoin ? 3 : 2}, //[2/2]Questing Adventurer [3 mana] [NONE card]
                {"EX1_050", 1}, //[2/2]Coldlight Oracle [3 mana] [NONE card]
                {"EX1_083", 3}, //[3/3]Tinkmaster Overspark [3 mana] [NONE card]
                {"EX1_085", 0}, //[3/3]Mind Control Tech [3 mana] [NONE card]
                {"EX1_089", 0}, //[4/2]Arcane Golem [3 mana] [NONE card]
                {"EX1_102", 2}, //[1/4]Demolisher [3 mana] [NONE card]
                {"EX1_103", 2}, //[2/3]Coldlight Seer [3 mana] [NONE card]
                {"EX1_134", _hasCoin ? 7 : 4}, //[3/3]SI:7 Agent [3 mana] [ROGUE card]
                {"EX1_170", 2}, //[2/3]Emperor Cobra [3 mana] [NONE card]
                {"EX1_258", 2}, //[2/4]Unbound Elemental [3 mana] [SHAMAN card]
                {"EX1_301", 4}, //[3/5]Felguard [3 mana] [WARLOCK card]
                {"EX1_304", ChoicesHasCard(NerubianEgg) ? 7 : 2}, //[3/3]Void Terror [3 mana] [WARLOCK card]
                {"EX1_382", 3}, //[3/3]Aldor Peacekeeper [3 mana] [PALADIN card]
                {"EX1_390", 1}, //[2/3]Tauren Warrior [3 mana] [NONE card]
                {"EX1_412", 2}, //[3/3]Raging Worgen [3 mana] [NONE card]
                {"EX1_507", ChoicesHasRace(Card.CRace.MURLOC) ? 4 : 2}, //[3/3]Murloc Warleader [3 mana] [NONE card]
                {"EX1_556", 6}, //[2/3]Harvest Golem [3 mana] [NONE card]
                {"EX1_575", 0}, //[0/3]Mana Tide Totem [3 mana] [SHAMAN card]
                {"EX1_590", 3}, //[3/3]Blood Knight [3 mana] [NONE card]
                {"EX1_597", 4}, //[1/5]Imp Master [3 mana] [NONE card]
                {"EX1_604", 3}, //[2/4]Frothing Berserker [3 mana] [WARRIOR card]
                {"EX1_612", NumSecrets > 0 ? 4 : 2}, //[4/3]Kirin Tor Mage [3 mana] [MAGE card]
                {"EX1_613", 0}, //[2/2]Edwin VanCleef [3 mana] [ROGUE card]
                {"NEW1_027", 1}, //[3/3]Southsea Captain [3 mana] [NONE card]
                {"tt_004", 2}, //[2/3]Flesheating Ghoul [3 mana] [NONE card]
                {"LOE_019", ChoiceHasDeathRattle(1, 3) ? 7 : 4}, //[3/4]Unearthed Raptor [3 mana] [ROGUE card]
                {"LOE_020", Num1DropsDeck >= 1 ? 4 : 1}, //[2/4]Desert Camel [3 mana] [HUNTER card]
                {"LOE_022", 4}, //[3/4]Fierce Monkey [3 mana] [WARRIOR card]
                {"LOE_050", 4}, //[3/2]Mounted Raptor [3 mana] [DRUID card]
                {"LOE_077", 2}, //[2/4]Brann Bronzebeard [3 mana] [NONE card]
                {"FP1_005", 0}, //[2/2]Shade of Naxxramas [3 mana] [NONE card]
                {"FP1_009", 4}, //[2/8]Deathlord [3 mana] [NONE card]
                {"FP1_023", 5}, //[3/4]Dark Cultist [3 mana] [PRIEST card]
                {"FP1_027", 2}, //[1/4]Stoneskin Gargoyle [3 mana] [NONE card]
                {"FP1_029", 5}, //[4/4]Dancing Swords [3 mana] [NONE card]
                {"GVG_027", ChoicesHasRace(Card.CRace.MECH) ? 7 : 3}, //[2/2]Iron Sensei [3 mana] [ROGUE card]
                {"GVG_032", 2}, //[2/4]Grove Tender [3 mana] [DRUID card]
                {"GVG_044", 5}, //[3/4]Spider Tank [3 mana] [NONE card]
                {"GVG_048", 4}, //[3/3]Metaltooth Leaper [3 mana] [HUNTER card]
                {"GVG_065", 5}, //[4/4]Ogre Brute [3 mana] [NONE card]
                {"GVG_084", 0}, //[1/4]Flying Machine [3 mana] [NONE card]
                {"GVG_089", 0}, //[2/4]Illuminator [3 mana] [NONE card]
                {"GVG_092", 2}, //[3/2]Gnomish Experimenter [3 mana] [NONE card]
                {"GVG_095", 0}, //[2/4]Goblin Sapper [3 mana] [NONE card]
                {"GVG_097", 2}, //[2/3]Lil' Exorcist [3 mana] [NONE card]
                {"GVG_098", 0}, //[1/4]Gnomeregan Infantry [3 mana] [NONE card]
                {"GVG_101", 3}, //[4/3]Scarlet Purifier [3 mana] [PALADIN card]
                {"GVG_102", ChoicesHasRace(Card.CRace.MECH) ? 5 : 2}, //[3/3]Tinkertown Technician [3 mana] [NONE card]
                {"GVG_104", 1}, //[2/3]Hobgoblin [3 mana] [NONE card]
                {"GVG_123", 4}, //[3/3]Soot Spewer [3 mana] [MAGE card]
                {"BRM_002", 3}, //[2/4]Flamewaker [3 mana] [MAGE card]
                {"BRM_006", 5}, //[2/4]Imp Gang Boss [3 mana] [WARLOCK card]
                {"BRM_010", 3}, //[2/2]Druid of the Flame [3 mana] [DRUID card]
                {"BRM_033", ChoicesHasRace(Card.CRace.DRAGON) || NumDragons > 5 ? 6 : 3}, //[2/4]Blackwing Technician [3 mana] [NONE card]
                {"AT_007", 4}, //[3/4]Spellslinger [3 mana] [MAGE card]
                {"AT_014", 2}, //[3/3]Shadowfiend [3 mana] [PRIEST card]
                {"AT_032", ChoicesHasRace(Card.CRace.PIRATE) ? 5 : 2}, //[4/3]Shady Dealer [3 mana] [ROGUE card]
                {"AT_046", _hasCoin ? 4 : 3}, //[3/2]Tuskarr Totemic [3 mana] [SHAMAN card]
                {"AT_057", 2}, //[4/2]Stablemaster [3 mana] [HUNTER card]
                {"AT_063t", 2}, //[4/2]Dreadscale [3 mana] [HUNTER card]
                {"AT_066", 1}, //[3/3]Orgrimmar Aspirant [3 mana] [WARRIOR card]
                {"AT_075", 0}, //[2/4]Warhorse Trainer [3 mana] [PALADIN card]
                {"AT_083", 4}, //[3/3]Dragonhawk Rider [3 mana] [NONE card]
                {"AT_086", 3}, //[4/3]Saboteur [3 mana] [NONE card]
                {"AT_087", 3}, //[2/1]Argent Horserider [3 mana] [NONE card]
                {"AT_092", 0}, //[5/2]Ice Rager [3 mana] [NONE card]
                {"AT_095", 5}, //[2/2]Silent Knight [3 mana] [NONE card]
                {"AT_100", 6}, //[3/3]Silver Hand Regent [3 mana] [NONE card]
                {"AT_106", 2}, //[4/3]Light's Champion [3 mana] [NONE card]
                {"AT_110", 4}, //[2/5]Coliseum Manager [3 mana] [NONE card]
                {"AT_115", 1}, //[2/2]Fencing Coach [3 mana] [NONE card]
                {"AT_117", 0}, //[4/2]Master of Ceremonies [3 mana] [NONE card]
                {"AT_129", 4}, //[3/4]Fjola Lightbane [3 mana] [NONE card]
                {"AT_131", 4}, //[3/4]Eydis Darkbane [3 mana] [NONE card]
                {"CS2_033", 6}, //[3/6]Water Elemental [4 mana] [MAGE card]
                {"CS2_119", 0}, //[2/7]Oasis Snapjaw [4 mana] [NONE card]
                {"CS2_131", 0}, //[2/5]Stormwind Knight [4 mana] [NONE card]
                {"CS2_147", 0}, //[2/4]Gnomish Inventor [4 mana] [NONE card]
                {"CS2_179", 4}, //[3/5]Sen'jin Shieldmasta [4 mana] [NONE card]
                {"CS2_182", 5}, //[4/5]Chillwind Yeti [4 mana] [NONE card]
                {"CS2_197", _ownC == Card.CClass.ROGUE ? 5 : 3}, //[4/4]Ogre Magi [4 mana] [NONE card]
                {"DS1_070", ChoicesHasRace(Card.CRace.BEAST) ? 5 : 0}, //[4/3]Houndmaster [4 mana] [HUNTER card]
                {"EX1_025", 0}, //[2/4]Dragonling Mechanic [4 mana] [NONE card]
                {"EX1_587", 0}, //[3/3]Windspeaker [4 mana] [SHAMAN card]
                {"NEW1_011", 0}, //[4/3]Kor'kron Elite [4 mana] [WARRIOR card]
                {"EX1_023", 0}, //[3/3]Silvermoon Guardian [4 mana] [NONE card]
                {"EX1_043", (data != null && data.DeckStyle == Style.Control) || _hasCoin ? 10 : 0}, //[4/1]Twilight Drake [4 mana] [NONE card]
                {"EX1_046", 5}, //[4/4]Dark Iron Dwarf [4 mana] [NONE card]
                {"EX1_048", 0}, //[4/3]Spellbreaker [4 mana] [NONE card]
                {"EX1_057", 0}, //[5/4]Ancient Brewmaster [4 mana] [NONE card]
                {"EX1_093", 0}, //[2/3]Defender of Argus [4 mana] [NONE card]
                {"EX1_166", 0}, //[2/4]Keeper of the Grove [4 mana] [DRUID card]
                {"EX1_274", 0}, //[3/3]Ethereal Arcanist [4 mana] [MAGE card]
                {"EX1_313", 0}, //[5/6]Pit Lord [4 mana] [WARLOCK card]
                {"EX1_315", 0}, //[0/4]Summoning Portal [4 mana] [WARLOCK card]
                {"EX1_335", 0}, //[0/5]Lightspawn [4 mana] [PRIEST card]
                {"EX1_396", 0}, //[1/7]Mogu'shan Warden [4 mana] [NONE card]
                {"EX1_398", 0}, //[3/3]Arathi Weaponsmith [4 mana] [WARRIOR card]
                {"EX1_584", 0}, //[2/5]Ancient Mage [4 mana] [NONE card]
                {"EX1_591", ChoicesHasCard(CircleofHealing) && (data != null && data.DeckStyle == Style.Control) ? 6 : 0}, //[3/5]Auchenai Soulpriest [4 mana] [PRIEST card]
                {"EX1_595", 0}, //[4/2]Cult Master [4 mana] [NONE card]
                {"NEW1_014", 0}, //[4/4]Master of Disguise [4 mana] [ROGUE card]
                {"NEW1_022", _hasWeapon ? 5 : 0}, //[3/3]Dread Corsair [4 mana] [NONE card]
                {"NEW1_026", 0}, //[3/5]Violet Teacher [4 mana] [NONE card]
                {"LOE_012", _hasCoin ? 5 : 0}, //[5/4]Tomb Pillager [4 mana] [ROGUE card]
                {"LOE_016", 0}, //[2/6]Rumbling Elemental [4 mana] [SHAMAN card]
                {"LOE_017", 0}, //[3/4]Keeper of Uldaman [4 mana] [PALADIN card]
                {"LOE_039", 0}, //[3/4]Gorillabot A-3 [4 mana] [NONE card]
                {"LOE_047", 0}, //[3/3]Tomb Spider [4 mana] [NONE card]
                {"LOE_051", 0}, //[4/4]Jungle Moonkin [4 mana] [DRUID card]
                {"LOE_079", 0}, //[3/5]Elise Starseeker [4 mana] [NONE card]
                {"LOE_107", 0}, //[7/7]Eerie Statue [4 mana] [NONE card]
                {"LOE_110", 0}, //[7/4]Ancient Shade [4 mana] [NONE card]
                {"EX1_062", 0}, //[2/4]Old Murk-Eye [4 mana] [NONE card]
                {"FP1_016", 0}, //[3/5]Wailing Soul [4 mana] [NONE card]
                {"FP1_022", ChoicesHasRace(Card.CRace.DEMON) ? 10 : 4}, //[3/4]Voidcaller [4 mana] [WARLOCK card]
                {"FP1_026", 3}, //[5/5]Anub'ar Ambusher [4 mana] [ROGUE card]
                {"FP1_031", 0}, //[1/7]Baron Rivendare [4 mana] [NONE card]
                {"GVG_004", ChoicesHasRace(Card.CRace.MECH) || (NumMechs > 4 && _hasCoin) ? 8 : 3}, //[5/4]Goblin Blastmage [4 mana] [MAGE card]
                {"GVG_020", ChoicesHasRace(Card.CRace.MECH) ? 5 : 0}, //[3/5]Fel Cannon [4 mana] [WARLOCK card]
                {"GVG_040", 0}, //[2/5]Siltfin Spiritwalker [4 mana] [SHAMAN card]
                {"GVG_055", 0}, //[2/5]Screwjank Clunker [4 mana] [WARRIOR card]
                {"GVG_066", 0}, //[5/4]Dunemaul Shaman [4 mana] [SHAMAN card]
                {"GVG_068", 0}, //[3/5]Burly Rockjaw Trogg [4 mana] [NONE card]
                {"GVG_071", 0}, //[5/4]Lost Tallstrider [4 mana] [NONE card]
                {"GVG_074", _secretClassEnemy ? 5 : 0}, //[4/3]Kezan Mystic [4 mana] [NONE card]
                {"GVG_078", 5}, //[4/5]Mechanical Yeti [4 mana] [NONE card]
                {"GVG_091", 4}, //[2/5]Arcane Nullifier X-21 [4 mana] [NONE card]
                {"GVG_094", 0}, //[1/4]Jeeves [4 mana] [NONE card]
                {"GVG_096", 8}, //[4/3]Piloted Shredder [4 mana] [NONE card]
                {"GVG_107", 0}, //[3/2]Enhance-o Mechano [4 mana] [NONE card]
                {"GVG_109", 0}, //[4/1]Mini-Mage [4 mana] [NONE card]
                {"GVG_122", 0}, //[2/5]Wee Spellstopper [4 mana] [MAGE card]
                {"BRM_012", 0}, //[3/6]Fireguard Destroyer [4 mana] [SHAMAN card]
                {"BRM_014", 0}, //[4/4]Core Rager [4 mana] [HUNTER card]
                {"BRM_016", 0}, //[2/5]Axe Flinger [4 mana] [WARRIOR card]
                {"BRM_020", 3}, //[3/5]Dragonkin Sorcerer [4 mana] [NONE card]
                {"BRM_026", 2}, //[5/6]Hungry Dragon [4 mana] [NONE card]
                {"AT_006", 3}, //[3/5]Dalaran Aspirant [4 mana] [MAGE card]
                {"AT_011", 5}, //[3/5]Holy Champion [4 mana] [PRIEST card]
                {"AT_012", 3}, //[5/4]Spawn of Shadows [4 mana] [PRIEST card]
                {"AT_017", ChoicesHasRace(Card.CRace.DRAGON) || NumDragons > 4 ? 8 : 0}, //[2/6]Twilight Guardian [4 mana] [NONE card]
                {"AT_019", 0}, //[1/1]Dreadsteed [4 mana] [WARLOCK card]
                {"AT_039", HasRamp() ? 10 : 6}, //[5/4]Savage Combatant [4 mana] [DRUID card]
                {"AT_040", 0}, //[4/4]Wildwalker [4 mana] [DRUID card]
                {"AT_047", 0}, //[4/4]Draenei Totemcarver [4 mana] [SHAMAN card]
                {"AT_067", 0}, //[5/3]Magnataur Alpha [4 mana] [WARRIOR card]
                {"AT_076", 0}, //[3/4]Murloc Knight [4 mana] [PALADIN card]
                {"AT_085", 0}, //[2/6]Maiden of the Lake [4 mana] [NONE card]
                {"AT_091", 0}, //[1/8]Tournament Medic [4 mana] [NONE card]
                {"AT_093", 0}, //[2/6]Frigid Snobold [4 mana] [NONE card]
                {"AT_108", 0}, //[5/3]Armored Warhorse [4 mana] [NONE card]
                {"AT_111", 0}, //[3/5]Refreshment Vendor [4 mana] [NONE card]
                {"AT_114", 4}, //[5/4]Evil Heckler [4 mana] [NONE card]
                {"AT_121", 0}, //[4/4]Crowd Favorite [4 mana] [NONE card]
                {"AT_122", 0}, //[4/4]Gormok the Impaler [4 mana] [NONE card]
            };

            #endregion
        }


        private static void ModifySpecialPriorities()
        {
            PreFiveDrops.AddOrUpdate(PintSizedSummoner, HasGoodDrop(2, 3) ? 1 : 2);
            PreFiveDrops.AddOrUpdate(ArgentProtector, HasGoodDrop(1, 5) ? 4 : 0);
            PreFiveDrops.AddOrUpdate(AlarmoBot, ChoicesHasCard(Innervate) && !HasGoodDrop(3, 3) && !HasGoodDrop(4, 3) ? 4 : 0);
            PreFiveDrops.AddOrUpdate(KingMukla, HasGoodDrop(1, 4) && HasGoodDrop(2, 4) ? 10 : 5);
            PreFiveDrops.AddOrUpdate(UnboundElemental, HasGoodDrop(2, 4) ? 4 : 2);
            PreFiveDrops.AddOrUpdate(ShadeofNaxxramas, HasGoodDrop(2, 3) || (ChoicesHasCard(WildGrowth) && _hasCoin) ? 4 : 0);
            PreFiveDrops.AddOrUpdate(VioletTeacher, _hasCoin && NumSpells > 1 && HasGoodDrop(2, 3) && HasGoodDrop(3, 3) ? 5 : 0);
            PreFiveDrops.AddOrUpdate(TombSpider, _hasCoin || HasGoodDrop(2, 5) ? 5 : 0);
        }

        //Necessary Preparations
        private static void DefaultIni(Card.CClass opponentClass, Card.CClass ownClass)
        {
            try
            {
                //TODO resharper ancor
                _oc = opponentClass;
                _ownC = ownClass;
                _secretClass = _ownC == Card.CClass.PALADIN || _ownC == Card.CClass.MAGE || _ownC == Card.CClass.HUNTER;
                _secretClassEnemy = _oc == Card.CClass.PALADIN || _oc == Card.CClass.MAGE || _oc == Card.CClass.HUNTER;
                _aggro = _oc == Card.CClass.PALADIN || _oc == Card.CClass.HUNTER || _oc == Card.CClass.SHAMAN;
                _wc = _oc == Card.CClass.WARRIOR || _oc == Card.CClass.PALADIN || _oc == Card.CClass.HUNTER || _oc == Card.CClass.ROGUE;
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
                NumSecrets = CurrentDeck.Count(q => CardTemplate.LoadFromId(q).IsSecret);
                AverageCost = CurrentDeck.Average(c => CardTemplate.LoadFromId(c).Cost);
            }
            catch (Exception e)
            {
                Bot.Log(string.Format("[SmartMulligan] Thank you for using SmartMulligan. If this message exists, then unicorns invaded your world and I need to be informed about them"));
                Bot.Log(string.Format("[SmartMulligan] If your average deck cost shows 0, that means murlocs took over and you need dto sub in hungry crab, or use alternate injection for better AI. PS: Your average deck cost is {0} ", AverageCost));
            }
        }

        //In case something wrong happens, it will save files for debug purposes
        private static void ArchiveDeck(DeckType deckType)
        {
            CheckDirectory(string.Format("{0}\\{1}\\{2}\\", MainDir, Archive, StringForArthur));
            Dictionary<string, int> myDeck = new Dictionary<string, int>();
            using (var file = new StreamWriter(string.Format("{0}\\{1}\\{2}\\{3}.txt", MainDir, Archive, StringForArthur, "deck request file"), true))
            {
                file.WriteLine("SMV2 recognized {0}, but it should be [WRITE WHAT IT SHOULD BE]", deckType);
                file.WriteLine(" ");
                foreach (var q in CurrentDeck.Distinct().ToList())
                    file.Write("{0}, ", CardTemplate.LoadFromId(q).Name.Replace(" ", ""));
                file.WriteLine(" ");
                foreach (var q in CurrentDeck.Distinct())
                {
                    myDeck.AddOrUpdate(q, CurrentDeck.Count(c => c == q));
                }
                foreach (var q in myDeck)
                {
                    file.WriteLine("{0} {1}", q.Value, CardTemplate.LoadFromId(q.Key).Name);
                }

                file.WriteLine("========================================================================================================================");
            }

        }

        #region archetype mulligan handlers

        private void HandleRenoLock(DeckData myInfo)
        {
            var vc = ChoicesHasCard(Voidcaller);
            HandleMinions(_ch, _whiteList, myInfo);
            _whiteList.Remove(AncientWatcher);
            WhiteListAList(new List<string> { MountainGiant, TwilightDrake, DarkPeddler, _hasCoin ? ImpGangBoss : "" });
            WhiteListAList(_aggro ? new List<string>
            {
                Hellfire, Shadowflame, EarthenRingFarseer, MoltenGiant, MindControlTech, _hasCoin ? RenoJackson : "", _hasCoin || ChoicesHasCard(AncientWatcher) ? DefenderofArgus : "", ChoiceOr(new List<string> {SunfuryProtector, DefenderofArgus}) ? AncientWatcher : ""
            } : new List<string>
            {
                EmperorThaurissan, PilotedShredder, _oc == Card.CClass.WARLOCK ? BigGameHunter : _oc == Card.CClass.SHAMAN ? Hellfire : _oc == Card.CClass.MAGE ? IronbeakOwl : ""
            });
            HandleSpells(_ch, _whiteList);
            _whiteList.AddOrUpdate(Voidcaller, false);
            _whiteList.Remove(!_aggro ? SunfuryProtector : "");
            _whiteList.AddOrUpdate(_oc == Card.CClass.MAGE || _oc == Card.CClass.HUNTER ? KezanMystic : "", false);
            if (_oc == Card.CClass.PRIEST || _oc == Card.CClass.WARRIOR)
                _whiteList.AddOrUpdate(vc ? GetDemonDescending(3, LordJaraxxus) : "", false);
            else _whiteList.AddOrUpdate(vc ? GetDemonDescending(3) : "", false);
        }

        private void HandleMage(List<Card.Cards> choices)
        {
            //nmc is a never mulligan card list. Those cards are always kept with at least 1 copy
            List<string> nmc = new List<string>
            {
                ArcaneBlast, ManaWyrm, Frostbolt, UnstablePortal, MadScientist, SorcerersApprentice
            };
            var hasRem = choices.Any(c => CardTemplate.LoadFromId(c).Type == Card.CType.SPELL && nmc.Any(q => q.ToString() == c.ToString()));
            var hasMin = choices.Any(c => CardTemplate.LoadFromId(c).Type == Card.CType.MINION && nmc.Any(q => q.ToString() == c.ToString()));
            var oneDropPrio = choices.Any(c => CardTemplate.LoadFromId(c).Id.ToString() == ClockworkGnome) && choices.Any(c => CardTemplate.LoadFromId(c).Id.ToString() == ManaWyrm);
            foreach (var q in
                from q in nmc let card = CardTemplate.LoadFromId(q) where card.Type == Card.CType.SPELL select q)
                _whiteList.AddOrUpdate(q, q == UnstablePortal && _hasCoin);

            foreach (var q in
                from q in nmc let card = CardTemplate.LoadFromId(q) where card.Type == Card.CType.MINION select q)
                _whiteList.AddOrUpdate(q, false);

            foreach (var q in choices)
            {
                CardTemplate card = CardTemplate.LoadFromId(q);
                if (card.Type == Card.CType.MINION && card.Cost == 1)
                    _whiteList.AddOrUpdate(q.ToString(), _hasCoin);
                if (card.Type == Card.CType.MINION && card.Cost == 2)
                    _whiteList.AddOrUpdate(q.ToString(), _hasCoin && card.HasDeathrattle);
                if (card.Type == Card.CType.MINION && card.Class == Card.CClass.MAGE && card.Cost == 3)
                    _whiteList.AddOrUpdate(!hasMin ? q.ToString() : "", false);
            }
            if (oneDropPrio)
                _whiteList.Remove(ClockworkGnome);
            _whiteList.AddOrUpdate(_oc == Card.CClass.PRIEST && _hasCoin ? Fireball : "", false); //against dragons
            _whiteList.AddOrUpdate(_oc == Card.CClass.WARRIOR || _oc == Card.CClass.PRIEST && !hasRem ? ArcaneMissiles : "", false);
            _whiteList.AddOrUpdate(_aggro || _oc == Card.CClass.WARRIOR || !hasRem ? ArcaneMissiles : "", false);
            _whiteList.AddOrUpdate(_oc != Card.CClass.ROGUE && _oc != Card.CClass.SHAMAN && _oc != Card.CClass.PALADIN ? Flamecannon : "", false);
            _whiteList.AddOrUpdate(_oc == Card.CClass.PALADIN || _oc == Card.CClass.HUNTER ? ArcaneExplosion : "", false);
        }

        private void HandleRaptorRogue(List<Card.Cards> choices, DeckData myInfo)
        {
            foreach (var q in choices.Where(c => (_aggro ? CardTemplate.LoadFromId(c).Cost <= 2 : CardTemplate.LoadFromId(c).Cost <= 3) && CardTemplate.LoadFromId(c).Type != Card.CType.SPELL && CardTemplate.LoadFromId(c).Quality != Card.CQuality.Legendary))
                _whiteList.AddOrUpdate(q.ToString(), CardTemplate.LoadFromId(q).Cost < 3);

            var has2 = choices.Any(q => CardTemplate.LoadFromId(q).Cost == 2);
            var has3 = choices.Any(q => CardTemplate.LoadFromId(q).Cost == 3);
            var hasDeathRattle = choices.Any(q => CardTemplate.LoadFromId(q).HasDeathrattle && CardTemplate.LoadFromId(q).Cost <= 2 && CardTemplate.LoadFromId(q).Quality != Card.CQuality.Legendary);
            _whiteList.AddOrUpdate(hasDeathRattle ? UnearthedRaptor : "", hasDeathRattle && _hasCoin);
            _whiteList.AddOrUpdate(has2 && has3 ? PilotedShredder : "", false);
            if (_aggro) _whiteList.AddOrUpdate(Backstab, false);
            _whiteList.AddOrUpdate(_hasCoin ? Sap : "", false);
            _whiteList.AddOrUpdate(_hasCoin ? PilotedShredder : "", false);
            _whiteList.AddOrUpdate(NerubianEgg, _hasCoin);
            _whiteList.AddOrUpdate(_oc == Card.CClass.PALADIN ? FanofKnives : "", false);
        }

        private void HandleZoo(List<Card.Cards> choices, DeckData info)
        {
            SetDefaultsForStyle(info.DeckStyle);
            List<string> activators = new List<string> { PowerOverwhelming, VoidTerror, AbusiveSergeant, DefenderofArgus };
            List<string> needActivation = new List<string> { NerubianEgg };
            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == NerubianEgg) ? PowerOverwhelming : "", false);
            HandleMinions(choices, _whiteList, info);
            _whiteList.AddOrUpdate(_has2Drop && _hasCoin && ChoicesIntersectList(needActivation) ? DefenderofArgus : "", false);
        }

        // ReSharper disable once UnusedParameter.Local
        private void HandleOilRogues(List<Card.Cards> choices)
        {
            var vMage = new List<string>
            {
                BladeFlurry, Backstab, EarthenRingFarseer, Preparation, Eviscerate, SI7Agent, DeadlyPoison
            }; //mage
            foreach (var q in vMage)
                _whiteList.AddOrUpdate(q, false);
            var vDruid = new List<string>
            {
                VioletTeacher, AzureDrake, EarthenRingFarseer, Sap, DeadlyPoison, Eviscerate, SI7Agent
            }; //druid
            foreach (var q in vDruid)
                _whiteList.AddOrUpdate(q, false);
            var vPaladin = new List<string>
            {
                FanofKnives, VioletTeacher, Backstab, SI7Agent, EarthenRingFarseer, DeadlyPoison, BladeFlurry, HarrisonJones
            }; //paladin
            foreach (var q in vPaladin)
                _whiteList.AddOrUpdate(q, false);
            var vShaman = new List<string>
            {
                BladeFlurry, Backstab, SI7Agent, VioletTeacher, DeadlyPoison, AzureDrake, FanofKnives
            }; //shaman
            foreach (var q in vShaman)
                _whiteList.AddOrUpdate(q, false);
            var vWarlock = new List<string>
            {
                BladeFlurry, Backstab, EarthenRingFarseer, Preparation, Eviscerate, SI7Agent, DeadlyPoison
            }; //warlock
            foreach (var q in vWarlock)
                _whiteList.AddOrUpdate(q, false);
            var vPriest = new List<string>
            {
                AzureDrake, VioletTeacher, Sprint, DeadlyPoison, Eviscerate, EarthenRingFarseer, SI7Agent, Sap
            }; //priest
            foreach (var q in vPriest)
                _whiteList.AddOrUpdate(q, false);
            var vWarrior = new List<string>
            {
                VioletTeacher, Sprint, AzureDrake, EarthenRingFarseer, SI7Agent, HarrisonJones
            }; //warrior
            foreach (var q in vWarrior)
                _whiteList.AddOrUpdate(q, false);
            var vHunter = new List<string>
            {
                Backstab, EarthenRingFarseer, SI7Agent, Preparation, Eviscerate, FanofKnives
            }; //hunter
            foreach (var q in vHunter)
                _whiteList.AddOrUpdate(q, false);
            var vRogue = new List<string>
            {
                VioletTeacher, AzureDrake, EarthenRingFarseer, DeadlyPoison, Eviscerate, Sprint, Preparation
            }; //rogue
            foreach (var q in vRogue)
                _whiteList.AddOrUpdate(q, false);
        }

        //Not to confuse with mech/aggro warrior
        private void HandleFaceWarrior(List<Card.Cards> choices)
        {
            _whiteList.AddOrUpdate(FieryWarAxe, _hasCoin);
            _whiteList.AddOrUpdate(_hasCoin ? DeathsBite : "", false);
            foreach (var q in from q in choices let temp = CardTemplate.LoadFromId(q) where temp.Cost == 1 select q)
            {
                _has1Drop = true;
                _whiteList.AddOrUpdate(q.ToString(), false);
            }
            if (_oc == Card.CClass.HUNTER)
                _whiteList.AddOrUpdate(IronbeakOwl, false);
        }

        //Obvious... I hope
        private void HandleControlWarrior(List<Card.Cards> choices, DeckType deckType = DeckType.ControlWarrior)
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
            _whiteList.AddOrUpdate(FierceMonkey, _hasWeapon && _hasCoin && _aggro);
            if (_hasCoin && (_has2Drop || _has4Drop))
            {
                _whiteList.AddOrUpdate(ShieldBlock, false);
                _whiteList.AddOrUpdate(ShieldSlam, false);
            }
            _whiteList.AddOrUpdate(_aggro ? Whirlwind : "", false);
            if (_oc == Card.CClass.WARLOCK)
            {
                _whiteList.AddOrUpdate(BigGameHunter, false);
                _whiteList.AddOrUpdate(ShieldSlam, false);
                _whiteList.AddOrUpdate(Execute, false);
                _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == Execute) ? CruelTaskmaster : "", false);
            }
            if (_wc)
                _whiteList.AddOrUpdate(_has2Drop || _has4Drop || _hasCoin ? HarrisonJones : "", false);
        }

        private void HandlePaladin(List<Card.Cards> choices, Style dStyle)
        {
            var gotEarly = false;
            var heavyControl = _oc == Card.CClass.WARLOCK || _oc == Card.CClass.PRIEST || _oc == Card.CClass.SHAMAN || _oc == Card.CClass.WARRIOR || _oc == Card.CClass.ROGUE;

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
            _whiteList.AddOrUpdate(_oc == Card.CClass.DRUID || _oc == Card.CClass.WARLOCK ? AldorPeacekeeper : "", false);
            _whiteList.AddOrUpdate(_oc == Card.CClass.HUNTER || _oc == Card.CClass.PALADIN ? Consecration : "", false);
        }

        // [4 in 1] Includes: Dragon Priest, Warlock (Not Malygos), Paladin, Warrior 
        private void HandleDragons(List<Card.Cards> choices, DragonDeck type)
        {
            var needActivator = choices.Any(c => c.ToString() == WyrmrestAgent || c.ToString() == TwilightWhelp);
            var exception = new List<string> { IronbeakOwl, Shrinkmeister, SunfuryProtector, AncientWatcher };
            var hasExcept = choices.Select(q => CardTemplate.LoadFromId(q).Id.ToString()).ToList().Intersect(exception).ToList().Count > 0;
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
                    _whiteList.AddOrUpdate(_oc == Card.CClass.DRUID ? AldorPeacekeeper : "", false);
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
                        bool gotCore = choices.Select(q => CardTemplate.LoadFromId(q).Id.ToString()).ToList().Intersect(coreControl).ToList().Count > 0;
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
        private void HandleShaman(List<Card.Cards> choices, DeckType dType)
        {
            var control = false;
            switch (_oc)
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
                case Card.CClass.DRUID:
                    break;
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
        private void HandleHunter(List<Card.Cards> choices, Style dStyle)
        {
            HandleWeapons(choices, _whiteList);
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
                foreach (var q in
                    (from q in choices let w = CardTemplate.LoadFromId(q) where w.Cost == 3 && w.Health > 1 select q).Where(q => _has1Drop || _has2Drop))
                {
                    _has3Drop = true;
                    _whiteList.AddOrUpdate(q.ToString(), false);
                }
                _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == MadScientist) ? EaglehornBow : choices.Any(c => c.ToString() == Glaivezooka) ? Glaivezooka : EaglehornBow, false);
                if (dStyle == Style.Face) return;
            }
            PreFiveDrops.AddOrUpdate(InjuredKvaldir, -1);
            PreFiveDrops.AddOrUpdate(IronbeakOwl, -1);
            foreach (var q in choices.Where(c => CardTemplate.LoadFromId(c).Cost <= 4 && CardTemplate.LoadFromId(c).Type == Card.CType.MINION))
            {
                switch (CardTemplate.LoadFromId(q).Cost)
                {
                    case 1:
                        _whiteList.AddOrUpdate(PreFiveDrops[q.ToString()] > 1 ? q.ToString() : "", false);
                        break;
                    case 2:
                        _whiteList.AddOrUpdate(_aggro ? IronbeakOwl : "", false);
                        _whiteList.AddOrUpdate(PreFiveDrops[q.ToString()] > 1 ? q.ToString() : "", PreFiveDrops[q.ToString()] >= 3 && _hasCoin);
                        break;
                    case 3:
                        _has3Drop = true;
                        _whiteList.AddOrUpdate(PreFiveDrops[q.ToString()] > 1 ? q.ToString() : "", false);
                        break;
                    case 4:
                        _whiteList.AddOrUpdate(PreFiveDrops[q.ToString()] > 6 && _hasCoin ? q.ToString() : "", false);
                        break;
                }
            }
            foreach (var q in choices.Where(c => CardTemplate.LoadFromId(c).Cost <= 4 && !CardTemplate.LoadFromId(c).IsSecret && CardTemplate.LoadFromId(c).Type == Card.CType.SPELL))
            {
                switch (CardTemplate.LoadFromId(q).Cost)
                {
                    case 1:
                        _whiteList.AddOrUpdate(q.ToString(), false);
                        break;
                    case 2:
                        _whiteList.AddOrUpdate(q.ToString(), false);
                        break;
                    case 3:
                        _has3Drop = true;
                        _whiteList.AddOrUpdate(_aggro ? UnleashtheHounds : "", false);
                        _whiteList.AddOrUpdate(_hasCoin || HasGoodDrop(2, 2) ? AnimalCompanion : "", _hasCoin);
                        break;
                    case 4:
                        _whiteList.AddOrUpdate(q.ToString(), false);
                        break;
                }
            }
            _whiteList.AddOrUpdate(HasGoodDrop(2, 2) && ChoicesHasCard(AnimalCompanion) && _hasCoin && _oc == Card.CClass.WARRIOR ? SavannahHighmane : "", false);
            _whiteList.AddOrUpdate(ChoiceOr(HauntedCreeper, Webspinner) ? HuntersMark : "", false);
            _whiteList.AddOrUpdate(ChoicesHasCard(KnifeJuggler) && _hasCoin ? SnakeTrap : "", false);
            _whiteList.AddOrUpdate(!HasGoodDrop(2, 2) && !HasGoodDrop(1, 2) && !_hasCoin ? FreezingTrap : "", false);
        }

        //Ramp, Midrange, FelFace
        private void HandleDruid(List<Card.Cards> choices, DeckData data)
        {
            if (data.DeckType == DeckType.TokenDruid)
            {
                foreach (var q in choices.Where(c => CardTemplate.LoadFromId(c).Cost <= 3))
                {
                    var card = CardTemplate.LoadFromId(q);
                    if (card.Type == Card.CType.SPELL && card.Cost >= 3) continue;
                    _whiteList.AddOrUpdate(q.ToString(), (card.Type == Card.CType.SPELL && card.Cost < 1) || (card.Cost == 2 && _hasCoin && card.Type != Card.CType.SPELL));
                }

                _whiteList.AddOrUpdate(Jeeves, false);
            }
            if (data.DeckStyle == Style.Face)
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
            switch (_oc)
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
        private void HandleHandlock(List<Card.Cards> choices, DeckData myInfo)
        {
            var strongHand = choices.Any(c => c.ToString() == MountainGiant && c.ToString() == TwilightDrake);
            var goodHand = choices.Any(c => c.ToString() == MountainGiant || c.ToString() == TwilightDrake);
            var hasVoidCaller = choices.Any(c => c.ToString() == Voidcaller);

            //Check if there are no good drops in the opening hand
            var terribleHand = choices.All(c => c.ToString() != MountainGiant) && choices.All(c => c.ToString() != TwilightDrake) && choices.All(c => c.ToString() != AncientWatcher) && choices.All(c => c.ToString() != SunfuryProtector) && choices.All(c => c.ToString() != Voidcaller);
            _whiteList.AddOrUpdate(terribleHand ? MoltenGiant : "", true);


            switch (_oc)
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
            if (myInfo.DeckType == DeckType.RenoLock)
                HandleMinions(choices, _whiteList, myInfo);
        }

        //Personal prefernse, will not accept recomendations :roto2cafe:
        private void HandleFreezeMage(List<Card.Cards> choices)
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
                _whiteList.AddOrUpdate(_oc == Card.CClass.DRUID ? Frostbolt : "", false);
                _whiteList.AddOrUpdate(ArcaneIntellect, false);
                _whiteList.AddOrUpdate(AcolyteofPain, false);
            }
        }

        //See above
        private void HandleEchoMage(List<Card.Cards> choices)
        {
            var heavyControlGroup = _oc == Card.CClass.PRIEST || _oc == Card.CClass.WARRIOR;
            var doom = _oc != Card.CClass.MAGE || _oc != Card.CClass.PRIEST;
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
        private void HandleMechWarrior(List<Card.Cards> cards)
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
            _whiteList.AddOrUpdate(!HasGoodDrop(1, 0) && _hasCoin && ChoicesHasCard(MusterforBattle) ? Avenge : "", false);
            _whiteList.AddOrUpdate(_oc == Card.CClass.SHAMAN && ChoicesHasCard(ShieldedMinibot) ? Redemption : "", false);
            if (ChoiceAnd(NobleSacrifice, Avenge, Secretkeeper) && _oc == Card.CClass.SHAMAN)
                WhiteListAList(new List<string> { NobleSacrifice, Avenge, Secretkeeper });
            _whiteList.AddOrUpdate(_oc == Card.CClass.SHAMAN && _hasCoin ? HarrisonJones : "", false);
            _whiteList.AddOrUpdate(ChoiceOr(HauntedCreeper, NerubianEgg) && _hasCoin ? KeeperofUldaman : "", false);
            foreach (var q in choices)
            {
                var card = CardTemplate.LoadFromId(q);
                if (card.Cost == 1 && card.Type == Card.CType.MINION)
                    _whiteList.AddOrUpdate(q.ToString(), card.Divineshield || card.Health == 3);
                if (card.Cost == 2 && card.Type == Card.CType.MINION)
                    _whiteList.AddOrUpdate(q.ToString(), card.Divineshield && card.Atk == 2);
                if (card.Cost == 3 && card.Type == Card.CType.SPELL && card.Id.ToString() != DivineFavor)
                    _whiteList.AddOrUpdate(q.ToString(), false);
            }
            if (_aggro)
            {
                _whiteList.AddOrUpdate(_oc == Card.CClass.DRUID ? AldorPeacekeeper : "", false);
                _whiteList.AddOrUpdate(Consecration, false);
                _whiteList.AddOrUpdate(_hasCoin ? PilotedShredder : "", false);
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

        //Obvious, I hope
        private void HandleMechMage(List<Card.Cards> choices)
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
                if (temp.Race != Card.CRace.MECH && temp.Quality != Card.CQuality.Epic && temp.Type != Card.CType.SPELL)
                //minions handler
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
                if (_oc == Card.CClass.PALADIN && temp.Cost == 3 && temp.Quality == Card.CQuality.Epic)
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
        private void HandleBurstRogue(List<Card.Cards> choices)
        {
            List<string> alwaysTwo = new List<string> { LeperGnome, SouthseaDeckhand, LootHoarder };
            foreach (var q in alwaysTwo)
                _whiteList.AddOrUpdate(q, true);
            List<string> atLeastOne = new List<string> { DefiasRingleader, SI7Agent, ArgentHorserider };
            foreach (var q in atLeastOne)
                _whiteList.AddOrUpdate(q, false);
            switch (_oc)
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
        private void HandlePatronMulligan(List<Card.Cards> choices)
        {
            var aggro = (_oc == Card.CClass.DRUID || _oc == Card.CClass.HUNTER || _oc == Card.CClass.SHAMAN || _oc == Card.CClass.PALADIN);
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
                if (_oc == Card.CClass.PRIEST || _oc == Card.CClass.WARRIOR)
                    _whiteList.AddOrUpdate(EmperorThaurissan, false);
            }
        }

        #endregion

        //Archive stuff
        private static void CheckDirectory(string subdir)
        {
            if (Directory.Exists(subdir))
                return;
            Directory.CreateDirectory(subdir);
        }

        private static void HandleSpells(List<Card.Cards> choices, Dictionary<string, bool> whiteList)
        {
            HandleSpells(whiteList);
        }


        private static void HandleSpells(Dictionary<string, bool> whiteList)
        {
            bool hasGood1 = HasGoodDrop(1, 3);
            bool hasGood2 = HasGoodDrop(2, 3);
            bool hasGood1Or2 = HasGoodDrop(1, 3) || HasGoodDrop(2, 3);
            bool hasGood1And2 = HasGoodDrop(1, 3) && HasGoodDrop(2, 3);
            switch (_ownC)
            {
                case Card.CClass.SHAMAN:
                    whiteList.AddOrUpdate(RockbiterWeapon, false); // [1 Cost]
                    whiteList.AddOrUpdate(!hasGood2 && _hasCoin ? FarSight : "", false); // [3 Cost]
                    whiteList.AddOrUpdate(FeralSpirit, false); // [3 Cost]
                    break;
                case Card.CClass.PRIEST:
                    whiteList.AddOrUpdate(ChoicesHasCard(InjuredBlademaster) ? LightoftheNaaru : "", false); // [1 Cost]
                    whiteList.AddOrUpdate(hasGood1Or2 ? HolySmite : "", false); // [1 Cost]
                    whiteList.AddOrUpdate(!_hasCoin ? MindVision : "", false); // [1 Cost]
                    whiteList.AddOrUpdate(PowerWordShield, false); // [1 Cost] 
                    whiteList.AddOrUpdate(ShadowWordPain, false); // [2 Cost]
                    whiteList.AddOrUpdate(Shadowform, false); // [3 Cost]
                    whiteList.AddOrUpdate((hasGood1Or2 && _hasCoin) || hasGood2 ? VelensChosen : "", false); // [3 Cost]
                    break;
                case Card.CClass.MAGE:
                    whiteList.AddOrUpdate(hasGood1 ? Frostbolt : "", false); // [2 Cost]
                    whiteList.AddOrUpdate(ChoicesHasCard(ManaWyrm) && _hasCoin ? MirrorImage : "", false); // [1 Cost]
                    whiteList.AddOrUpdate(hasGood1 ? ArcaneMissiles : "", false); // [1 Cost]
                    whiteList.AddOrUpdate(!hasGood1And2 ? MirrorEntity : "", false); // [3 Cost]
                    whiteList.AddOrUpdate(!hasGood1And2 || hasGood2 ? ForgottenTorch : "", false); // [3 Cost]
                    whiteList.AddOrUpdate(hasGood1 || _hasCoin ? Flamecannon : "", false); // [2 Cost]
                    whiteList.AddOrUpdate(UnstablePortal, _hasCoin); // [2 Cost]
                    whiteList.AddOrUpdate(hasGood1Or2 ? ArcaneBlast : "", false); // [1 Cost]
                    break;
                case Card.CClass.PALADIN:
                    whiteList.AddOrUpdate(hasGood2 ? NobleSacrifice : "", false); // [1 Cost]
                    whiteList.AddOrUpdate(Avenge, false); // [1 Cost]
                    whiteList.AddOrUpdate(MusterforBattle, false); // [3 Cost]
                    break;
                case Card.CClass.WARRIOR:
                    whiteList.AddOrUpdate(Against(Paladin) ? Whirlwind : "", false); // [1 Cost]
                    whiteList.AddOrUpdate(hasGood2 && ChoicesHasCard(ShieldSlam) ? ShieldBlock : "", false); // [3 Cost]
                    whiteList.AddOrUpdate(Slam, false); // [2 Cost]
                    whiteList.AddOrUpdate(!hasGood1 ? Upgrade : "", false); // [1 Cost]
                    whiteList.AddOrUpdate(hasGood2 && ChoicesHasCard(ShieldBlock) ? ShieldSlam : "", false); // [1 Cost]
                    whiteList.AddOrUpdate(hasGood1Or2 && !HasGoodDrop(3, 3) ? Bash : "", false); // [3 Cost]
                    break;
                case Card.CClass.WARLOCK:
                    whiteList.AddOrUpdate(MortalCoil, false); // [1 Cost]
                    whiteList.AddOrUpdate(ChoicesHasCard(NerubianEgg) ? PowerOverwhelming : "", false); // [1 Cost]
                    whiteList.AddOrUpdate(NumDemons > 5 && _hasCoin && !hasGood2 ? SenseDemons : "", false); // [3 Cost]
                    whiteList.AddOrUpdate(!hasGood2 ? CurseofRafaam : "", false); // [2 Cost]
                    whiteList.AddOrUpdate(hasGood1Or2 || _hasCoin ? Darkbomb : "", false); // [2 Cost]
                    break;
                case Card.CClass.HUNTER:
                    whiteList.AddOrUpdate(!hasGood1Or2 ? Tracking : "", false); // [1 Cost]
                    whiteList.AddOrUpdate(AnimalCompanion, _hasCoin); // [3 Cost]
                    whiteList.AddOrUpdate(Against(Paladin) ? UnleashtheHounds : "", false); // [3 Cost]
                    whiteList.AddOrUpdate(ChoicesHasCard(KnifeJuggler) ? SnakeTrap : "", false); // [2 Cost]
                    whiteList.AddOrUpdate(!hasGood1Or2 ? FreezingTrap : "", false); // [2 Cost]
                    whiteList.AddOrUpdate(NumBeasts > 5 ? CallPet : "", false); // [2 Cost]
                    whiteList.AddOrUpdate(hasGood1Or2 && _hasCoin ? QuickShot : "", false); // [2 Cost]
                    whiteList.AddOrUpdate(hasGood1And2 ? Powershot : "", false); // [3 Cost]
                    whiteList.AddOrUpdate(_hasCoin ? BearTrap : "", false); // [2 Cost]
                    break;
                case Card.CClass.ROGUE:
                    whiteList.AddOrUpdate(Backstab, false); // [0 Cost]
                    whiteList.AddOrUpdate(DeadlyPoison, false); // [1 Cost]
                    whiteList.AddOrUpdate(Against(Paladin) ? FanofKnives : "", false); // [3 Cost]
                    whiteList.AddOrUpdate(ChoicesHasCard(Burgle) || ChoicesHasCard(BeneaththeGrounds) ? Preparation : "", false); // [0 Cost]
                    whiteList.AddOrUpdate(ChoicesHasCard(Preparation) ? Burgle : "", false); // [3 Cost]
                    whiteList.AddOrUpdate(ChoicesHasCard(Preparation) ? BeneaththeGrounds : "", false); // [3 Cost]

                    break;
                case Card.CClass.DRUID:
                    whiteList.AddOrUpdate(hasGood1 ? MarkoftheWild : "", false); // [2 Cost]
                    whiteList.AddOrUpdate(Wrath, false); // [2 Cost]
                    whiteList.AddOrUpdate(PoweroftheWild, false); // [2 Cost]
                    whiteList.AddOrUpdate(LivingRoots, _hasCoin); // [1 Cost]
                    break;
            }
            foreach (var card in _ch)
            {
                CardTemplate cardQ = CardTemplate.LoadFromId(card);
                if (whiteList.ContainsKey(card.ToString()) && cardQ.IsSecret && ChoicesHasCard(MadScientist))
                    whiteList.Remove(card.ToString());
            }
        }

        private static bool Against(Card.CClass ocCClass, Card.CClass ocCClass2, Card.CClass ocCClass3 = Card.CClass.NONE, Card.CClass ocCClass4 = Card.CClass.NONE, Card.CClass ocCClass5 = Card.CClass.NONE)
        {
            if (ocCClass3 == Card.CClass.NONE)
                return _oc == ocCClass || _oc == ocCClass2;
            if (ocCClass4 == Card.CClass.NONE)
                return _oc == ocCClass || _oc == ocCClass2 || _oc == ocCClass3;
            if (ocCClass5 == Card.CClass.NONE)
                return _oc == ocCClass || _oc == ocCClass2;
            return _oc == ocCClass || _oc == ocCClass2;
        }

        private static bool Against(Card.CClass opponentCClass)
        {
            return _oc == opponentCClass;
        }

        private static void HandleWeapons(List<Card.Cards> choices, Dictionary<string, bool> whiteList)
        {
            switch (_ownC)
            {
                case Card.CClass.SHAMAN:
                    {
                        whiteList.AddOrUpdate(choices.Any(c => c.ToString() == StormforgedAxe) ? StormforgedAxe : choices.Any(c => c.ToString() == Powermace) ? Powermace : "", false);
                        break;
                    }
                case Card.CClass.PALADIN:
                    {
                        whiteList.AddOrUpdate(choices.Any(c => c.ToString() == LightsJustice) && HasGoodDropCount(2, 6) < 2 ? LightsJustice : choices.Any(c => c.ToString() == Coghammer) ? Coghammer : choices.Any(c => c.ToString() == SwordofJustice) ? SwordofJustice : "", false);
                        whiteList.AddOrUpdate(_hasCoin && !ChoiceOr(LightsJustice, Coghammer, SwordofJustice) ? TruesilverChampion : "", false);
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

        private static void HandleMinions(List<Card.Cards> choices, Dictionary<string, bool> whiteList, DeckData dataContainer)
        {
            _has1Drop = false;
            Num1Drops = 0;
            _has2Drop = false;
            Num2Drops = 0;
            _has3Drop = false;
            Num3Drops = 0;
            _has3Drop = false;
            Num4Drops = 0;
            bool deepSearchOne = CurrentDeck.Count(c => GetPriority(c) >= 3) > 1;
            bool deepSearchTwo = CurrentDeck.Count(c => GetPriority(c) >= 3) > 1;

            List<string> ones = choices.Where(card => CardTemplate.LoadFromId(card).Cost == 1).Select(q => GetPriority(q.ToString()) > 0 ? q.ToString() : "").OrderByDescending(c => GetPriority(c)).ToList();
            List<string> two = choices.Where(card => CardTemplate.LoadFromId(card).Cost == 2).Select(q => GetPriority(q.ToString()) > 0 ? q.ToString() : "").ToList().OrderByDescending(c => GetPriority(c)).ToList();
            List<string> three = choices.Where(card => CardTemplate.LoadFromId(card).Cost == 3).Select(q => GetPriority(q.ToString()) > 0 ? q.ToString() : "").ToList().OrderByDescending(c => GetPriority(c)).ToList();
            List<string> four = choices.Where(card => CardTemplate.LoadFromId(card).Cost == 4).Select(q => GetPriority(q.ToString()) > 0 ? q.ToString() : "").ToList().OrderByDescending(c => GetPriority(c)).ToList();
            foreach (var q in ones)
            {
                var priority = GetPriority(q);
                if ((priority <= 1 && deepSearchOne && !HasGoodDrop(2, 3)) || (Num1Drops == Allowed1Drops)) continue;
                Num1Drops++;

                _has1Drop = true;
                whiteList.AddOrUpdate(q, _hasCoin && priority > 5);
            }
            foreach (var q in from q in two let priority = GetPriority(q) where (priority > 1 || !deepSearchTwo) && (Num2Drops != Allowed2Drops) select q)
            {
                _has2Drop = true;
                Num2Drops++;
                whiteList.AddOrUpdate(q, false);
            }
            foreach (var q in from q in three let priority = GetPriority(q) where (priority > 1) && (Num3Drops != Allowed3Drops) select q)
            {
                whiteList.AddOrUpdate((_has1Drop && _hasCoin) || (_has2Drop) ? q : "", false);
                if (!(_has1Drop && _hasCoin) || !_has2Drop)
                    continue;
                _has3Drop = true;
                Num3Drops++;
            }
            foreach (var q in from q in four let priority = GetPriority(q) where (priority > 3) && (Num4Drops != Allowed4Drops) select q)
            {
                whiteList.AddOrUpdate((_has1Drop && _has2Drop) || _has3Drop ? q : "", _hasCoin);
                if (!(_has1Drop && _has2Drop) || !_has3Drop)
                    continue;
                _has4Drop = true;
                Num4Drops++;
            }
        }

        public static Dictionary<string, int> PreFiveDrops { get; set; }

        private static int GetPriority(string id, int modifier = 0)
        {
            var card = CardTemplate.LoadFromId(id);
            var vanila23 = card.Health == card.Cost + 1 && card.Atk == card.Cost;
            var vanila32 = card.Health == card.Cost && card.Atk == card.Cost + 1;
            if (!PreFiveDrops.ContainsKey(id)) return 0;
            var extra = 0;
            if (CardTemplate.LoadFromId(id).Race == Card.CRace.MECH && ChoicesHasCard(Mechwarper) && CardTemplate.LoadFromId(id).Cost <= 4)
                extra = _hasCoin ? extra + 4 : extra + 2;
            if (ChoicesHasCard(TwilightWhelp) || ChoicesHasCard(WyrmrestAgent))
                extra = CardTemplate.LoadFromId(id).Race == Card.CRace.DRAGON ? extra + 4 : extra + 0;
            if (vanila23 || vanila32)
                extra += 1;
            extra = _oc == Card.CClass.PALADIN ? extra + 1 : extra;

            return PreFiveDrops[id] + modifier + extra;
        }

        private static void DisplayMulligans(List<Card.Cards> choices, DeckData dataContainer)
        {
            List<string> choisesList = choices.Select(c => c.ToString()).ToList(); //.OrderBy(q=>CardTemplate.LoadFromId(q).Cost).ToList();
            List<string> cardsKept = _ctk.Select(c => c.ToString()).ToList(); //.OrderBy(q => CardTemplate.LoadFromId(q).Cost).ToList();
            if (choisesList.Count != cardsKept.Count)
            {
                while (choisesList.Count != cardsKept.Count)
                    cardsKept.Add("");
            }
            CheckDirectory(string.Format("{0}\\{1}\\", MainDir, Archive));
            using (var file = new StreamWriter(string.Format("{0}\\{1}\\[{2}] {3}.txt", MainDir, Archive, Bot.CurrentMode(), dataContainer.DeckType), true))
            {
                file.WriteLine("==================START COPY=======================");
                //file.WriteLine("CURRENT MODE IS: "+Bot.CurrentMode());
                file.WriteLine("[{2}]You were {0} vs {1}", _ownC.ToString().ToLower(), _oc.ToString().ToLower(), _hasCoin ? "Coin" : "No Coin");
                file.WriteLine("[Kept]\t\t[Offered]\t[Card Name]");
                int value = _hasCoin ? 4 : 3;
                var printed = new List<string> { };
                for (int i = 0; i < value; i++)
                {
                    file.WriteLine("{0}{5} {1}{3}//{2} {4}", cardsKept.Any(c => c.ToString() == choisesList.ElementAt(i) && (!printed.Contains(choisesList.ElementAt(i)) && cardsKept.Count(w => w.ToString() == choisesList.ElementAt(i)) == 1) || (!printed.Contains(choisesList.ElementAt(i)) && cardsKept.Count(w => w.ToString() == choisesList.ElementAt(i)) == 2)) ? choisesList.ElementAt(i) : " \t", choisesList.ElementAt(i), //rrr
                        CardTemplate.LoadFromId(choisesList.ElementAt(i)).Name, choisesList.ElementAt(i).Length == 6 ? "\t\t" : "\t", GetPriority(choisesList.ElementAt(i)), choisesList.ElementAt(i).Length == 8 ? "\t" : "\t\t");
                    if (cardsKept.Any(c => c.ToString() == choisesList.ElementAt(i)))
                        printed.Add(choisesList.ElementAt(i));
                }
                file.WriteLine(" ");
                file.Write("{0}|{1}~~", choices.Count, _ctk.Count);
                file.Write("{0}*{1}*{2}*{3}", _has1Drop, _has2Drop, _has3Drop, _has4Drop);
                file.Write("~~");
                foreach (var q in _ctk)
                    file.Write("{0}*", q);
                file.WriteLine(" ");
                foreach (var q in CurrentDeck)
                    file.Write("\"{0}\",", q);
                file.WriteLine("");
                file.WriteLine("{19}|{0}~A{1}*C{2}*D{3}~A{4}*C{5}*D{6}~A{7}*C{8}*D{9}~A{10}*C{11}*D{12}~!~{13}*{14}*{15}*{16}||{17}||{18}", _hasCoin, Allowed1Drops, Num1Drops, Num1DropsDeck, Allowed2Drops, Num2Drops, Num2DropsDeck, Allowed3Drops, Num3Drops, Num3DropsDeck, Allowed4Drops, Num4Drops, Num4DropsDeck, _hasWeapon, NumWeapons, NumSecrets, NumSpells, AverageCost, EarlyCardsWight, dataContainer.DeckType);

                if (!IsArena() && !ArthursReasonToDrink)
                {
                    file.WriteLine("==================END COPY=======================");
                    return;
                }
                file.WriteLine("\n\t\t\t[ArenaValues]");
                foreach (var q in CurrentDeck.Where(c => CardTemplate.LoadFromId(c).Cost <= 4 && CardTemplate.LoadFromId(c).Type == Card.CType.MINION).OrderByDescending(c => CardTemplate.LoadFromId(c).Cost).ThenBy(c => PreFiveDrops.ContainsKey(c.ToString()) ? PreFiveDrops[c.ToString()] : 0))
                    file.WriteLine("[{0} mana] {1} had priority: {2}", CardTemplate.LoadFromId(q).Cost, CardTemplate.LoadFromId(q).Name, PreFiveDrops.ContainsKey(q) ? GetPriority(q) : 0); //
                file.WriteLine("==================END COPY=======================");
            }
        }

        private static bool IsArena()
        {
            return Bot.CurrentMode() == Bot.Mode.Arena || Bot.CurrentMode() == Bot.Mode.ArenaAuto;
        }

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
            Dictionary<Dictionary<DeckType, Style>, int> DeckDictionary = null;
            Dictionary<DeckType, Style> BestDeck = null;
            switch (ownClass)
            {
                #region shaman

                case Card.CClass.SHAMAN:
                    var totemShaman = new List<string> { TunnelTrogg, TunnelTrogg, FlametongueTotem, FlametongueTotem, TotemGolem, TotemGolem, LightningStorm, LightningStorm, Hex, Hex, TuskarrTotemic, TuskarrTotemic, FireguardDestroyer, FireguardDestroyer, FireElemental, FireElemental, ThunderBluffValiant, ThunderBluffValiant, Neptulon, DrBoom, DefenderofArgus, PilotedShredder, PilotedShredder, SylvanasWindrunner, JeweledScarab, JeweledScarab, AzureDrake, AzureDrake, };
                    var faceShaman = new List<string> { UnboundElemental, UnboundElemental, EarthShock, StormforgedAxe, Doomhammer, Doomhammer, FeralSpirit, FeralSpirit, RockbiterWeapon, RockbiterWeapon, LeperGnome, LeperGnome, AbusiveSergeant, LavaBurst, LavaBurst, Crackle, Crackle, LavaShock, LavaShock, TotemGolem, TotemGolem, ArgentHorserider, ArgentHorserider, AncestralKnowledge, AncestralKnowledge, SirFinleyMrrgglton, TunnelTrogg, TunnelTrogg, };
                    var mechShaman = new List<string> { Hex, HarvestGolem, HarvestGolem, FlametongueTotem, FlametongueTotem, RockbiterWeapon, RockbiterWeapon, FireElemental, FireElemental, Loatheb, Cogmaster, Cogmaster, AnnoyoTron, AnnoyoTron, DrBoom, SpiderTank, SpiderTank, Mechwarper, Mechwarper, PilotedShredder, PilotedShredder, BombLobber, BombLobber, WhirlingZapomatic, WhirlingZapomatic, Crackle, Crackle, Powermace, Powermace, };
                    var dragonShaman = new List<string> { FeralSpirit, Hex, Hex, AzureDrake, AzureDrake, Deathwing, Ysera, FireElemental, FireElemental, LightningStorm, LightningStorm, BlackwingTechnician, BlackwingTechnician, LavaShock, BlackwingCorruptor, TotemGolem, TotemGolem, AncestralKnowledge, HealingWave, HealingWave, TheMistcaller, TwilightGuardian, TwilightGuardian, Chillmaw, JeweledScarab, JeweledScarab, BrannBronzebeard, TunnelTrogg, TunnelTrogg, };
                    var malygosShaman = new List<string> { EarthShock, EarthShock, FarSight, FarSight, StormforgedAxe, FeralSpirit, FeralSpirit, FrostShock, FrostShock, Malygos, GnomishInventor, GnomishInventor, Crackle, Crackle, Hex, Hex, LavaBurst, LavaBurst, ManaTideTotem, ManaTideTotem, LightningStorm, LightningStorm, AncestorsCall, AncestorsCall, AntiqueHealbot, AntiqueHealbot, Alexstrasza, AzureDrake, };
                    var controlShaman = new List<string> { EarthShock, Hex, Hex, AzureDrake, AzureDrake, Doomsayer, Doomsayer, Ysera, FireElemental, FireElemental, LightningStorm, LightningStorm, Loatheb, SludgeBelcher, SludgeBelcher, DrBoom, Neptulon, LavaShock, LavaShock, VolcanicDrake, VolcanicDrake, HealingWave, HealingWave, ElementalDestruction, ElementalDestruction, TwilightGuardian, TwilightGuardian, JeweledScarab, JeweledScarab, };
                    var basicShaman = new List<string> { BoulderfistOgre, AcidicSwampOoze, GnomishInventor, Bloodlust, Hex, SenjinShieldmasta, FlametongueTotem, ShatteredSunCleric, RockbiterWeapon, TunnelTrogg, RumblingElemental, FireElemental, SirFinleyMrrgglton, JeweledScarab, BrannBronzebeard, ArchThiefRafaam, FrostwolfWarlord };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {new Dictionary<DeckType, Style> {{DeckType.TotemShaman, Style.Tempo}}, CurrentDeck.Intersect(totemShaman).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.FaceShaman, Style.Face}}, CurrentDeck.Intersect(faceShaman).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.MechShaman, Style.Aggro}}, CurrentDeck.Intersect(mechShaman).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.DragonShaman, Style.Control}}, CurrentDeck.Intersect(dragonShaman).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.MalygosShaman, Style.Combo}}, CurrentDeck.Intersect(malygosShaman).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.ControlShaman, Style.Control}}, CurrentDeck.Intersect(controlShaman).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}}, CurrentDeck.Intersect(basicShaman).ToList().Count},
                    };
                    if (!CurrentDeck.Contains(Malygos))
                        DeckDictionary.AddOrUpdate(new Dictionary<DeckType, Style> { { DeckType.MalygosShaman, Style.Combo } }, 0);
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                #endregion

                #region priest

                case Card.CClass.PRIEST:
                    List<string> comboPriest = new List<string> { CircleofHealing, CircleofHealing, InnerFire, InnerFire, Alexstrasza, AcolyteofPain, AcolyteofPain, PowerWordShield, PowerWordShield, Silence, Silence, ShadowWordDeath, DivineSpirit, DivineSpirit, NorthshireCleric, NorthshireCleric, HarrisonJones, StormwindKnight, AuchenaiSoulpriest, HolyNova, Loatheb, Deathlord, Deathlord, VelensChosen, GnomereganInfantry, Lightbomb, Lightbomb, EmperorThaurissan, };
                    List<string> controlPriest = new List<string> { LightoftheNaaru, LightoftheNaaru, PowerWordShield, PowerWordShield, NorthshireCleric, NorthshireCleric, MuseumCurator, MuseumCurator, Shrinkmeister, ShadowWordDeath, AuchenaiSoulpriest, AuchenaiSoulpriest, HolyNova, Entomb, Entomb, Lightbomb, Lightbomb, CabalShadowPriest, WildPyromancer, WildPyromancer, Deathlord, Deathlord, InjuredBlademaster, InjuredBlademaster, SludgeBelcher, SludgeBelcher, JusticarTrueheart, Ysera, };
                    List<string> mechPriest = new List<string> { PowerWordShield, PowerWordShield, NorthshireCleric, NorthshireCleric, HolyNova, HolyNova, DarkCultist, DarkCultist, VelensChosen, VelensChosen, DrBoom, SpiderTank, UpgradedRepairBot, UpgradedRepairBot, Mechwarper, Mechwarper, PilotedShredder, PilotedShredder, ClockworkGnome, ClockworkGnome, Shadowboxer, Shadowboxer, Voljin, SpawnofShadows, GorillabotA3, Entomb, MuseumCurator, MuseumCurator, };
                    List<string> basicPriest = new List<string> { ChillwindYeti, BoulderfistOgre, AcidicSwampOoze, GnomishInventor, StormwindChampion, ShadowWordPain, SenjinShieldmasta, MindControl, HolySmite, PowerWordShield, ShatteredSunCleric, NoviceEngineer, ShadowWordDeath, BloodfenRaptor, NorthshireCleric, HolyNova, };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {new Dictionary<DeckType, Style> {{DeckType.ComboPriest, Style.Combo}}, CurrentDeck.Intersect(comboPriest).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.ControlPriest, Style.Control}}, CurrentDeck.Intersect(controlPriest).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.MechPriest, Style.Aggro}}, CurrentDeck.Intersect(mechPriest).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}}, CurrentDeck.Intersect(basicPriest).ToList().Count},
                    };
                    if (NumDragons > 3 && CurrentDeck.Contains(WyrmrestAgent))
                    {
                        List<string> dragonPriestv2 = new List<string> { CabalShadowPriest, AzureDrake, PowerWordShield, Ysera, ShadowWordDeath, NorthshireCleric, HolyNova, DarkCultist, VelensChosen, Lightbomb, BlackwingTechnician, BlackwingCorruptor, Chromaggus, TwilightWhelp, TwilightGuardian, WyrmrestAgent, BrannBronzebeard, Entomb, MuseumCurator };
                        List<string> dragonPriest = new List<string> { CabalShadowPriest, CabalShadowPriest, AzureDrake, PowerWordShield, PowerWordShield, Ysera, ShadowWordDeath, ShadowWordDeath, NorthshireCleric, HarrisonJones, HolyNova, SludgeBelcher, VelensChosen, VelensChosen, DrBoom, Shrinkmeister, Shrinkmeister, Voljin, Lightbomb, Lightbomb, BlackwingTechnician, BlackwingTechnician, TwilightWhelp, TwilightWhelp, TwilightGuardian, TwilightGuardian, Chillmaw, WyrmrestAgent, WyrmrestAgent, };
                        DeckDictionary.AddOrUpdate(new Dictionary<DeckType, Style> { { DeckType.DragonPriest, Style.Tempo } }, CurrentDeck.Intersect(dragonPriest).ToList().Count);
                        DeckDictionary.AddOrUpdate(new Dictionary<DeckType, Style> { { DeckType.DragonPriest, Style.Tempo } }, CurrentDeck.Intersect(dragonPriestv2).ToList().Count);
                    }
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();


                    break;

                #endregion

                #region mage

                case Card.CClass.MAGE:
                    List<string> tempoMage = new List<string> { MirrorImage, Frostbolt, Frostbolt, ArchmageAntonidas, ManaWyrm, ManaWyrm, AzureDrake, AzureDrake, ArcaneIntellect, ArcaneIntellect, Fireball, Fireball, Counterspell, MirrorEntity, ArcaneMissiles, ArcaneMissiles, MadScientist, MadScientist, UnstablePortal, UnstablePortal, DrBoom, PilotedShredder, PilotedShredder, Flamecannon, ClockworkGnome, Flamewaker, Flamewaker, ArcaneBlast, };
                    List<string> freezeMage = new List<string> { Flamestrike, FrostNova, FrostNova, Frostbolt, Frostbolt, IceLance, IceLance, ArchmageAntonidas, Blizzard, Blizzard, Alexstrasza, LootHoarder, AcolyteofPain, AcolyteofPain, Doomsayer, Doomsayer, ArcaneIntellect, ArcaneIntellect, Pyroblast, Fireball, Fireball, BloodmageThalnos, IceBarrier, IceBarrier, MadScientist, MadScientist, AntiqueHealbot, EmperorThaurissan, };
                    List<string> mechMage = new List<string> { ArchmageAntonidas, ManaWyrm, ManaWyrm, Fireball, Fireball, UnstablePortal, UnstablePortal, Cogmaster, Cogmaster, AnnoyoTron, AnnoyoTron, DrBoom, SpiderTank, SpiderTank, Mechwarper, Mechwarper, PilotedShredder, PilotedShredder, GoblinBlastmage, GoblinBlastmage, ClockworkGnome, TinkertownTechnician, Snowchugger, Snowchugger, ClockworkKnight, ClockworkKnight, GorillabotA3, GorillabotA3, };
                    List<string> echoMage = new List<string> { IceBlock, IceBlock, Flamestrike, Flamestrike, BigGameHunter, MoltenGiant, MoltenGiant, Frostbolt, Frostbolt, ArchmageAntonidas, Blizzard, Blizzard, Alexstrasza, SunfuryProtector, SunfuryProtector, AcolyteofPain, AcolyteofPain, ArcaneIntellect, ArcaneIntellect, Polymorph, MadScientist, MadScientist, ExplosiveSheep, ExplosiveSheep, AntiqueHealbot, EchoofMedivh, EchoofMedivh, EmperorThaurissan, };
                    List<string> basicMage = new List<string> { ChillwindYeti, Flamestrike, RazorfenHunter, BoulderfistOgre, AcidicSwampOoze, Frostbolt, GnomishInventor, WaterElemental, StormwindChampion, SenjinShieldmasta, ShatteredSunCleric, ArcaneIntellect, Fireball, BloodfenRaptor, ArcaneMissiles, Polymorph, GurubashiBerserker, };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {new Dictionary<DeckType, Style> {{DeckType.TempoMage, Style.Tempo}}, CurrentDeck.Intersect(tempoMage).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.FreezeMage, Style.Control}}, CurrentDeck.Intersect(freezeMage).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.MechMage, Style.Aggro}}, CurrentDeck.Intersect(mechMage).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.FatigueMage, Style.Control}}, CurrentDeck.Intersect(echoMage).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}}, CurrentDeck.Intersect(basicMage).ToList().Count},
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                #endregion

                #region paladin

                case Card.CClass.PALADIN:
                    List<string> secretPaladin = new List<string> { BlessingofKings, BigGameHunter, NobleSacrifice, NobleSacrifice, Consecration, TruesilverChampion, TirionFordring, Secretkeeper, Secretkeeper, IronbeakOwl, HarrisonJones, Redemption, Avenge, Avenge, SludgeBelcher, HauntedCreeper, DrBoom, PilotedShredder, PilotedShredder, MusterforBattle, MusterforBattle, Coghammer, ShieldedMinibot, ShieldedMinibot, CompetitiveSpirit, MysteriousChallenger, MysteriousChallenger, KeeperofUldaman, };
                    List<string> midRangePaladin = new List<string> { BigGameHunter, Consecration, Consecration, TruesilverChampion, TruesilverChampion, TirionFordring, KnifeJuggler, KnifeJuggler, IronbeakOwl, ZombieChow, ZombieChow, Loatheb, SludgeBelcher, SludgeBelcher, DrBoom, PilotedShredder, PilotedShredder, MusterforBattle, MusterforBattle, AntiqueHealbot, Coghammer, ShieldedMinibot, ShieldedMinibot, Quartermaster, Quartermaster, JusticarTrueheart, KeeperofUldaman, KeeperofUldaman, };
                    List<string> aggroPaladin = new List<string> { ArcaneGolem, SouthseaDeckhand, SouthseaDeckhand, Consecration, TruesilverChampion, TruesilverChampion, HammerofWrath, BlessingofMight, BlessingofMight, KnifeJuggler, KnifeJuggler, ArgentSquire, ArgentSquire, IronbeakOwl, LeperGnome, LeperGnome, AbusiveSergeant, AbusiveSergeant, DivineFavor, DivineFavor, LeeroyJenkins, HauntedCreeper, MusterforBattle, MusterforBattle, Coghammer, ShieldedMinibot, ShieldedMinibot, SealofChampions, };
                    List<string> anyfin = new List<string> { AldorPeacekeeper, CultMaster, OldMurkEye, MurlocWarleader, Consecration, BluegillWarrior, TruesilverChampion, KnifeJuggler, LayonHands, GrimscaleOracle, ZombieChow, SludgeBelcher, DrBoom, PilotedShredder, MusterforBattle, AntiqueHealbot, Coghammer, ShieldedMinibot, SolemnVigil, AnyfinCanHappen, KeeperofUldaman };
                    List<string> basicPaladin = new List<string> { BlessingofKings, ChillwindYeti, RazorfenHunter, BoulderfistOgre, AcidicSwampOoze, Consecration, GuardianofKings, TruesilverChampion, StormwindChampion, SenjinShieldmasta, HammerofWrath, MurlocTidehunter, ShatteredSunCleric, RiverCrocolisk, BloodfenRaptor, FrostwolfWarlord, };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {new Dictionary<DeckType, Style> {{DeckType.SecretPaladin, Style.Tempo}}, CurrentDeck.Intersect(secretPaladin).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.MidRangePaladin, Style.Control}}, CurrentDeck.Intersect(midRangePaladin).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.AggroPaladin, Style.Face}}, CurrentDeck.Intersect(aggroPaladin).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.AnyfinMurglMurgl, Style.Combo}}, CurrentDeck.Intersect(anyfin).ToList().Count},
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                #endregion

                #region warrior

                case Card.CClass.WARRIOR:
                    List<string> corePatron = new List<string> { KorkronElite, KorkronElite, Whirlwind, Whirlwind, Slam, Slam, Execute, Execute, DreadCorsair, InnerRage, InnerRage, AcolyteofPain, AcolyteofPain, FieryWarAxe, FieryWarAxe, GrommashHellscream, Armorsmith, Armorsmith, BattleRage, BattleRage, DeathsBite, DeathsBite, UnstableGhoul, UnstableGhoul, DrBoom, GrimPatron, GrimPatron, SirFinleyMrrgglton, };
                    List<string> controlWarrior = new List<string> { BigGameHunter, Slam, Slam, Execute, Execute, Brawl, Brawl, CruelTaskmaster, AcolyteofPain, AcolyteofPain, ShieldBlock, ShieldBlock, BaronGeddon, FieryWarAxe, FieryWarAxe, Armorsmith, Armorsmith, DeathsBite, DeathsBite, SludgeBelcher, SludgeBelcher, Shieldmaiden, Shieldmaiden, JusticarTrueheart, Bash, EliseStarseeker, FierceMonkey, FierceMonkey, };
                    List<string> fatigueWarrior = new List<string> { BigGameHunter, BigGameHunter, AcidicSwampOoze, ColdlightOracle, ColdlightOracle, Gorehowl, Execute, Execute, Brawl, Brawl, CruelTaskmaster, CruelTaskmaster, ShieldBlock, FieryWarAxe, Armorsmith, DeathsBite, DeathsBite, SludgeBelcher, SludgeBelcher, Deathlord, Deathlord, Shieldmaiden, Shieldmaiden, AntiqueHealbot, AntiqueHealbot, IronJuggernaut, JusticarTrueheart, BrannBronzebeard, };
                    List<string> faceWarrior = new List<string> { ArcaneGolem, ArcaneGolem, SouthseaDeckhand, SouthseaDeckhand, KorkronElite, KorkronElite, Wolfrider, DreadCorsair, DreadCorsair, MortalStrike, MortalStrike, IronbeakOwl, LeperGnome, LeperGnome, FieryWarAxe, FieryWarAxe, BloodsailRaider, BloodsailRaider, Upgrade, Upgrade, DeathsBite, DeathsBite, ArgentHorserider, ArgentHorserider, Bash, Bash, SirFinleyMrrgglton, CursedBlade, };
                    List<string> dragonWarrior = new List<string> { Execute, Execute, AzureDrake, AzureDrake, Alexstrasza, CruelTaskmaster, CruelTaskmaster, TwilightDrake, TwilightDrake, FieryWarAxe, FieryWarAxe, DeathsBite, DeathsBite, Loatheb, DrBoom, Shieldmaiden, Shieldmaiden, IronJuggernaut, BlackwingTechnician, BlackwingTechnician, BlackwingCorruptor, BlackwingCorruptor, Nefarian, JusticarTrueheart, AlexstraszasChampion, TwilightGuardian, TwilightGuardian, BrannBronzebeard, };
                    List<string> mechWarrior = new List<string> { KorkronElite, ArcaniteReaper, MortalStrike, MortalStrike, HarvestGolem, HarvestGolem, IronbeakOwl, FieryWarAxe, FieryWarAxe, DeathsBite, DeathsBite, Cogmaster, AnnoyoTron, AnnoyoTron, SpiderTank, SpiderTank, Mechwarper, Mechwarper, PilotedShredder, PilotedShredder, ScrewjankClunker, ScrewjankClunker, Warbot, Warbot, FelReaver, FelReaver, SirFinleyMrrgglton, GorillabotA3, };
                    List<string> basicWarrior = new List<string> { ChillwindYeti, RazorfenHunter, BoulderfistOgre, AcidicSwampOoze, Cleave, KorkronElite, ArcaniteReaper, Execute, GnomishInventor, StormwindChampion, SenjinShieldmasta, ShatteredSunCleric, ShieldBlock, RiverCrocolisk, BloodfenRaptor, FieryWarAxe };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {new Dictionary<DeckType, Style> {{DeckType.PatronWarrior, Style.Tempo}}, CurrentDeck.Intersect(corePatron).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.ControlWarrior, Style.Control}}, CurrentDeck.Intersect(controlWarrior).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.FatigueWarrior, Style.Control}}, CurrentDeck.Intersect(fatigueWarrior).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.FaceWarrior, Style.Face}}, CurrentDeck.Intersect(faceWarrior).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.DragonWarrior, Style.Control}}, CurrentDeck.Intersect(dragonWarrior).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.MechWarrior, Style.Aggro}}, CurrentDeck.Intersect(mechWarrior).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Aggro}}, CurrentDeck.Intersect(basicWarrior).ToList().Count},
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                #endregion

                #region warlock

                case Card.CClass.WARLOCK:
                    List<string> renolock = new List<string> { MortalCoil, Darkbomb, DarkPeddler, Demonwrath, ImpGangBoss, Hellfire, Implosion, Shadowflame, SiphonSoul, LordJaraxxus, AbusiveSergeant, ZombieChow, IronbeakOwl, SunfuryProtector, BigGameHunter, BrannBronzebeard, EarthenRingFarseer, DefenderofArgus, PilotedShredder, TwilightDrake, AntiqueHealbot, Feugen, Loatheb, SludgeBelcher, Stalagg, EmperorThaurissan, RenoJackson, DrBoom, MoltenGiant, };
                    List<string> demonHandlock = new List<string> { MortalCoil, MortalCoil, MoltenGiant, MoltenGiant, AncientWatcher, AncientWatcher, MountainGiant, MountainGiant, TwilightDrake, TwilightDrake, SunfuryProtector, SunfuryProtector, LordJaraxxus, IronbeakOwl, DefenderofArgus, SiphonSoul, Shadowflame, Shadowflame, ZombieChow, ZombieChow, Voidcaller, Voidcaller, Loatheb, SludgeBelcher, SludgeBelcher, DrBoom, AntiqueHealbot, MalGanis, Darkbomb, };
                    List<string> zoolock = new List<string> { FlameImp, FlameImp, PowerOverwhelming, PowerOverwhelming, DireWolfAlpha, DireWolfAlpha, Voidwalker, Voidwalker, KnifeJuggler, KnifeJuggler, IronbeakOwl, Doomguard, Doomguard, DefenderofArgus, DefenderofArgus, AbusiveSergeant, AbusiveSergeant, Voidcaller, Voidcaller, NerubianEgg, NerubianEgg, Loatheb, HauntedCreeper, HauntedCreeper, DrBoom, Implosion, Implosion, ImpGangBoss, ImpGangBoss, };
                    List<string> dragonHandlock = new List<string> { MortalCoil, MortalCoil, Darkbomb, Darkbomb, AncientWatcher, AncientWatcher, IronbeakOwl, SunfuryProtector, SunfuryProtector, BigGameHunter, Hellfire, Hellfire, Shadowflame, DefenderofArgus, TwilightDrake, TwilightDrake, TwilightGuardian, TwilightGuardian, AntiqueHealbot, BlackwingCorruptor, BlackwingCorruptor, EmperorThaurissan, Chillmaw, DrBoom, Alexstrasza, MountainGiant, MountainGiant, MoltenGiant, MoltenGiant, };
                    List<string> demonZooWarlock = new List<string> { PowerOverwhelming, PowerOverwhelming, Voidwalker, Voidwalker, KnifeJuggler, KnifeJuggler, IronbeakOwl, Doomguard, Doomguard, DefenderofArgus, DefenderofArgus, AbusiveSergeant, AbusiveSergeant, SeaGiant, BaneofDoom, Voidcaller, Voidcaller, NerubianEgg, NerubianEgg, HauntedCreeper, HauntedCreeper, DrBoom, MalGanis, Implosion, Implosion, ImpGangBoss, ImpGangBoss, DarkPeddler, DarkPeddler, };
                    List<string> handlock = new List<string> { BigGameHunter, MoltenGiant, MoltenGiant, Hellfire, Hellfire, AncientWatcher, MountainGiant, MountainGiant, TwilightDrake, TwilightDrake, SunfuryProtector, SunfuryProtector, LordJaraxxus, IronbeakOwl, IronbeakOwl, DefenderofArgus, Shadowflame, Loatheb, SludgeBelcher, SludgeBelcher, DrBoom, AntiqueHealbot, AntiqueHealbot, Darkbomb, Darkbomb, EmperorThaurissan, BrannBronzebeard, DarkPeddler, };
                    List<string> basicdeck = new List<string>{ChillwindYeti, ShatteredSunCleric, SenjinShieldmasta, ZombieChow, HauntedCreeper, SludgeBelcher, KelThuzad, Loatheb, EmperorThaurissan, ImpGangBoss, DarkPeddler, JeweledScarab, ArchThiefRafaam, ShadowBolt, Hellfire, DreadInfernal, MortalCoil,
};
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {new Dictionary<DeckType, Style> {{DeckType.RenoLock, Style.Control}}, CurrentDeck.Intersect(renolock).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.DemonHandlock, Style.Control}}, CurrentDeck.Intersect(demonHandlock).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.Zoolock, Style.Aggro}}, CurrentDeck.Intersect(zoolock).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.DragonHandlock, Style.Control}}, CurrentDeck.Intersect(dragonHandlock).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.DemonZooWarlock, Style.Aggro}}, CurrentDeck.Intersect(demonZooWarlock).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.Handlock, Style.Control}}, CurrentDeck.Intersect(handlock).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}}, CurrentDeck.Intersect(basicdeck).ToList().Count},
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                #endregion

                #region hunter

                case Card.CClass.HUNTER:
                    List<string> midRangeHunter = new List<string> { HuntersMark, HuntersMark, Webspinner, BearTrap, FreezingTrap, SnakeTrap, EaglehornBow, EaglehornBow, AnimalCompanion, AnimalCompanion, KillCommand, KillCommand, UnleashtheHounds, UnleashtheHounds, Houndmaster, Houndmaster, RamWrangler, HauntedCreeper, JeweledScarab, KnifeJuggler, KnifeJuggler, MadScientist, MadScientist, TombSpider, TombSpider, SludgeBelcher, SludgeBelcher, DrBoom, };
                    List<string> hybridHunter = new List<string> { FreezingTrap, UnleashtheHounds, UnleashtheHounds, ExplosiveTrap, EaglehornBow, KnifeJuggler, KnifeJuggler, KillCommand, KillCommand, IronbeakOwl, LeperGnome, LeperGnome, AbusiveSergeant, AbusiveSergeant, AnimalCompanion, AnimalCompanion, Loatheb, MadScientist, MadScientist, HauntedCreeper, HauntedCreeper, PilotedShredder, PilotedShredder, Glaivezooka, Glaivezooka, QuickShot, ArgentHorserider, ArgentHorserider, };
                    List<string> faceHunter = new List<string> { Glaivezooka, Glaivezooka, ExplosiveTrap, QuickShot, QuickShot, EaglehornBow, AnimalCompanion, AnimalCompanion, UnleashtheHounds, UnleashtheHounds, KillCommand, KillCommand, DartTrap, DesertCamel, AbusiveSergeant, AbusiveSergeant, LeperGnome, LeperGnome, SouthseaDeckhand, HauntedCreeper, IronbeakOwl, IronbeakOwl, KnifeJuggler, KnifeJuggler, MadScientist, MadScientist, ArcaneGolem, ArcaneGolem, WorgenInfiltrator, };
                    List<string> basicHunter = new List<string> { RazorfenHunter, TimberWolf, StarvingBuzzard, TundraRhino, ArcaneShot, Wolfrider, Houndmaster, BluegillWarrior, MultiShot, ShatteredSunCleric, KillCommand, RiverCrocolisk, RecklessRocketeer, BloodfenRaptor, AnimalCompanion };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {new Dictionary<DeckType, Style> {{DeckType.MidRangeHunter, Style.Tempo}}, CurrentDeck.Intersect(midRangeHunter).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.HybridHunter, Style.Aggro}}, CurrentDeck.Intersect(hybridHunter).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.FaceHunter, Style.Face}}, CurrentDeck.Intersect(faceHunter).ToList().Count},
                         {new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}}, CurrentDeck.Intersect(basicHunter).ToList().Count},
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;
                #endregion

                #region rogue

                case Card.CClass.ROGUE:
                    List<string> raptorRogue = new List<string> { ColdBlood, ColdBlood, FanofKnives, FanofKnives, Eviscerate, Eviscerate, Sap, LootHoarder, LootHoarder, Backstab, Backstab, UnearthedRaptor, UnearthedRaptor, AbusiveSergeant, AbusiveSergeant, LeperGnome, LeperGnome, NerubianEgg, NerubianEgg, DefenderofArgus, DefenderofArgus, PilotedShredder, PilotedShredder, Loatheb, SludgeBelcher, SludgeBelcher, DrBoom, HauntedCreeper, HauntedCreeper, };
                    List<string> pirateRogue = new List<string> { Sprint, Sprint, SouthseaDeckhand, SouthseaDeckhand, BladeFlurry, BladeFlurry, DreadCorsair, DreadCorsair, AzureDrake, AzureDrake, SI7Agent, SI7Agent, Preparation, Preparation, Eviscerate, Eviscerate, Sap, AssassinsBlade, Backstab, Backstab, BloodsailRaider, BloodsailRaider, ShipsCannon, ShipsCannon, TinkersSharpswordOil, TinkersSharpswordOil, Buccaneer, Buccaneer, };
                    List<string> oilRogue = new List<string> { DeadlyPoison, DeadlyPoison, Sprint, Sprint, SouthseaDeckhand, BladeFlurry, BladeFlurry, AzureDrake, AzureDrake, SI7Agent, SI7Agent, Preparation, Preparation, FanofKnives, FanofKnives, Eviscerate, Eviscerate, Sap, Sap, Backstab, Backstab, BloodmageThalnos, PilotedShredder, PilotedShredder, AntiqueHealbot, TinkersSharpswordOil, TinkersSharpswordOil, TombPillager, TombPillager, };
                    List<string> burstRogue = new List<string> { ColdlightOracle, ColdBlood, ColdBlood, ArcaneGolem, ArcaneGolem, SouthseaDeckhand, BladeFlurry, BladeFlurry, SI7Agent, SI7Agent, Eviscerate, Eviscerate, Sap, DefiasRingleader, AssassinsBlade, ArgentSquire, ArgentSquire, IronbeakOwl, IronbeakOwl, LeperGnome, LeperGnome, Loatheb, PilotedShredder, PilotedShredder, GoblinAutoBarber, GoblinAutoBarber, TinkersSharpswordOil, TinkersSharpswordOil, };
                    List<string> basicRogue = new List<string> { ChillwindYeti, RazorfenHunter, BoulderfistOgre, AcidicSwampOoze, DeadlyPoison, Sprint, GnomishInventor, StormwindChampion, SenjinShieldmasta, FanofKnives, AssassinsBlade, ShatteredSunCleric, NoviceEngineer, Backstab, Assassinate, BloodfenRaptor, };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {new Dictionary<DeckType, Style> {{DeckType.RaptorRogue, Style.Tempo}}, CurrentDeck.Intersect(raptorRogue).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.PirateRogue, Style.Aggro}}, CurrentDeck.Intersect(pirateRogue).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.OilRogue, Style.Combo}}, CurrentDeck.Intersect(oilRogue).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.BurstRogue, Style.Face}}, CurrentDeck.Intersect(burstRogue).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}}, CurrentDeck.Intersect(basicRogue).ToList().Count},
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                #endregion

                #region druid

                case Card.CClass.DRUID:
                    List<string> midRangeDruid = new List<string> { BigGameHunter, ForceofNature, ForceofNature, AzureDrake, AzureDrake, WildGrowth, WildGrowth, SavageRoar, SavageRoar, KeeperoftheGrove, KeeperoftheGrove, Innervate, Innervate, DruidoftheClaw, DruidoftheClaw, Swipe, Swipe, Wrath, Wrath, ShadeofNaxxramas, ShadeofNaxxramas, Loatheb, DrBoom, PilotedShredder, PilotedShredder, EmperorThaurissan, LivingRoots, LivingRoots, };
                    List<string> tokenDruid = new List<string> { SouloftheForest, SouloftheForest, SavageRoar, SavageRoar, KeeperoftheGrove, MarkoftheWild, MarkoftheWild, DefenderofArgus, DefenderofArgus, Innervate, Innervate, AbusiveSergeant, AbusiveSergeant, LivingRoots, LivingRoots, MountedRaptor, MountedRaptor, DragonEgg, DragonEgg, NerubianEgg, NerubianEgg, EchoingOoze, EchoingOoze, Jeeves, Jeeves, HauntedCreeper, HauntedCreeper, SirFinleyMrrgglton, };
                    List<string> rampDruid = new List<string> { BigGameHunter, AncientofWar, AncientofWar, WildGrowth, WildGrowth, KeeperoftheGrove, KeeperoftheGrove, RagnarostheFirelord, Innervate, Innervate, DruidoftheClaw, DruidoftheClaw, Swipe, Swipe, Wrath, Wrath, ZombieChow, ZombieChow, SludgeBelcher, SludgeBelcher, DrBoom, EmperorThaurissan, DruidoftheFlame, DruidoftheFlame, DarnassusAspirant, DarnassusAspirant, MasterJouster, MasterJouster, };
                    List<string> aggroDruid = new List<string> { ForceofNature, ForceofNature, SavageRoar, SavageRoar, KnifeJuggler, KnifeJuggler, KeeperoftheGrove, KeeperoftheGrove, LeperGnome, Innervate, Innervate, DruidoftheClaw, DruidoftheClaw, Swipe, Swipe, Loatheb, DrBoom, PilotedShredder, PilotedShredder, FelReaver, FelReaver, ArgentHorserider, DarnassusAspirant, DarnassusAspirant, DruidoftheSaber, DruidoftheSaber, LivingRoots, LivingRoots, SirFinleyMrrgglton, };
                    List<string> basicDruid = new List<string> { ChillwindYeti, RazorfenHunter, BoulderfistOgre, AcidicSwampOoze, IronbarkProtector, GnomishInventor, WildGrowth, SenjinShieldmasta, ShatteredSunCleric, NoviceEngineer, MarkoftheWild, Claw, Innervate, Swipe, Starfire };
                    DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {new Dictionary<DeckType, Style> {{DeckType.MidRangeDruid, Style.Combo}}, CurrentDeck.Intersect(midRangeDruid).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.TokenDruid, Style.Aggro}}, CurrentDeck.Intersect(tokenDruid).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.RampDruid, Style.Control}}, CurrentDeck.Intersect(rampDruid).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.AggroDruid, Style.Face}}, CurrentDeck.Intersect(aggroDruid).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}}, CurrentDeck.Intersect(basicDruid).ToList().Count},
                    };
                    BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                    info.DeckType = BestDeck.Keys.First();
                    info.DeckStyle = BestDeck.Values.First();

                    break;

                    #endregion
            }
            if (DeckDictionary != null && DeckDictionary[BestDeck] < UnknownTreshhold)
            {
                Bot.Log(string.Format("[SmartMulligan] Current deck didn't pass the threshold ({0}) of any known decks in SmartMulligan database.", UnknownTreshhold));
                return SetUnknown(info, DeckDictionary);
            }
            return info;
        }

        private DeckData SetUnknown(DeckData info, Dictionary<Dictionary<DeckType, Style>, int> deckDictionary)
        {
            info.DeckType = DeckType.Unknown;
            info.DeckStyle = GetStyle();
            return info;
        }

        private static void SetDefaultsForStyle(Style ds)
        {
            switch (ds)
            {
                case Style.Aggro:
                    Allowed1Drops = _hasCoin ? 4 : 3;
                    Allowed2Drops = _hasCoin ? 3 : 2;
                    Allowed3Drops = _hasCoin ? 2 : 1;
                    Allowed4Drops = _hasCoin ? 1 : 0;
                    break;
                case Style.Control:
                    Allowed1Drops = 1;
                    Allowed2Drops = _hasCoin ? 3 : 2;
                    Allowed3Drops = _hasCoin ? 2 : 1;
                    Allowed4Drops = _hasCoin ? 2 : 1;
                    break;
                case Style.Tempo:
                    Allowed1Drops = 1;
                    Allowed2Drops = _hasCoin ? 3 : 2;
                    Allowed3Drops = _hasCoin ? 2 : 1;
                    Allowed4Drops = 1;
                    break;
                case Style.Face:
                    Allowed1Drops = _hasCoin ? 4 : 3;
                    Allowed2Drops = _hasCoin ? 3 : 2;
                    Allowed3Drops = _hasCoin ? 1 : 0;
                    Allowed4Drops = 0;
                    break;
                default: //all other styles
                    Allowed1Drops = 1;
                    Allowed2Drops = _hasCoin ? 3 : 2;
                    Allowed3Drops = _hasCoin ? 2 : 1;
                    Allowed4Drops = 1;
                    break;
            }
        }

        private Style GetStyle()
        {
            Style res = AverageCost >= ControlConst ? Style.Control : (AverageCost < ControlConst) && (AverageCost >= TempoConst) ? Style.Tempo : (AverageCost < 3) ? Style.Aggro : Style.Tempo;
            //Bot.Log(string.Format("Average Cost {0}", AverageCost));
            SetDefaultsForStyle(res);
            //Bot.Log(string.Format("Num1 {0}, Num2{1} Num3{2} Weight{3} ", Num1DropsDeck, Num2DropsDeck, Num3DropsDeck, EarlyCardsWight));
            return EarlyCardsWight >= Face ? Style.Face : res;
        }

        //TODO: HELPER STUFF
        //=====================================================================================
        public static bool ChoiceAnd(List<string> listMinionId)
        {
            return !listMinionId.Except(_ch.Select(c => c.ToString()).ToList()).Any();
        }

        private static bool ChoiceOr(string s1, string s2, string s3 = "", string s4 = "")
        {
            return ChoiceOr(new List<string> { s1, s2, s3, s4 });
        }

        private static bool ChoiceOr(List<string> listMinionId)
        {
            return listMinionId.Count <= 4 && listMinionId.Intersect(_ch.Select(c => c.ToString()).ToList()).Any();
        }

        private static bool ChoicesHasCard(string minionId)
        {
            return _ch.Any(c => c.ToString() == minionId);
        }

        private static bool ChoicesHasRace(Card.CRace race, string excludedMinion = "")
        {
            return _ch.Any(c => CardTemplate.LoadFromId(c).Race == race && CardTemplate.LoadFromId(c).Id.ToString() != excludedMinion);
        }

        public static bool ChoiceAnd(string str1, string str2, string str3 = "", string str4 = "")
        {
            return str3 != "" ? ChoiceAnd(new List<string> { str1, str2, str3 }) : ChoiceAnd(str4 != "" ? new List<string> { str1, str2, str3, str4 } : new List<string> { str1, str2 });
        }

        private void WhiteListAList(List<string> list, bool twoCopies = false)
        {
            foreach (var q in list)
                _whiteList.AddOrUpdate(q, twoCopies);
        }
        private void WhiteListAList(Dictionary<string, bool> list)
        {
            foreach (var q in list)
                _whiteList.AddOrUpdate(q.Key, q.Value);
        }

        private string GetDemonAscending(int minCost, string exceptId = "")
        {
            return _ch.Select(c => c.ToString()).Where(q => CardTemplate.LoadFromId(q).Cost >= minCost && CardTemplate.LoadFromId(q).Race == Card.CRace.DEMON && CardTemplate.LoadFromId(q).Id.ToString() != exceptId).OrderBy(q => CardTemplate.LoadFromId(q).Cost).Take(1).First();
        }

        private string GetDemonDescending(int minCost, string exceptId = "")
        {
            return _ch.Select(c => c.ToString()).Where(q => CardTemplate.LoadFromId(q).Cost >= minCost && CardTemplate.LoadFromId(q).Race == Card.CRace.DEMON && CardTemplate.LoadFromId(q).Id.ToString() != exceptId).OrderByDescending(q => CardTemplate.LoadFromId(q).Cost).Take(1).First();
        }

        private static bool HasRamp()
        {
            return _ch.Any(c => c.ToString() == Innervate || c.ToString() == DarnassusAspirant || c.ToString() == WildGrowth);
        }

        private static bool ChoiceHasDeathRattle(int lowerBound, int upperBound)
        {
            return _ch.Any(c => CardTemplate.LoadFromId(c).HasDeathrattle && CardTemplate.LoadFromId(c).Cost >= lowerBound && CardTemplate.LoadFromId(c).Cost <= upperBound);
        }

        private static bool HasGoodDrop(int cost, int treshhold)
        {
            return _ch.Any(q => CardTemplate.LoadFromId(q).Cost == cost && (PreFiveDrops.ContainsKey(q.ToString()) && PreFiveDrops[q.ToString()] >= treshhold));
        }

        private static int HasGoodDropCount(int cost, int treshhold)
        {
            return _ch.Count(q => CardTemplate.LoadFromId(q).Cost == cost && (GetPriority(q.ToString()) >= treshhold));
        }

        private static bool ChoicesIntersectList(List<string> list)
        {
            if (list == null || list.All(string.IsNullOrWhiteSpace))
                return false;
            return _ch.Select(q => CardTemplate.LoadFromId(q).Id.ToString()).ToList().Intersect(list).ToList().Count == list.Count;
        }


        //=====================================================================================
    }

    /// <summary>
    /// TODO: WIP. Will require Plugin
    /// </summary>
    public class OpponentDeckData
    {
        public Dictionary<DeckType, List<string>> DeckPreferencesDictionary { get; set; }
        public bool Aggro { get; set; }
        public long Identification { get; set; }
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
        AggroPaladin,
        AnyfinMurglMurgl, //Complete
        RampDruid,
        AggroDruid,
        MidRangeDruid,
        TokenDruid, //Complete
        Handlock,
        RenoLock,
        Zoolock,
        DemonHandlock,
        DemonZooWarlock,
        DragonHandlock, //Missing ZooLock, DemonZoo
        TempoMage,
        FreezeMage,
        MechMage,
        FatigueMage, //Missing Tempo
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
        BloodlustShaman,
        RaptorRogue,
        MechPriest,
        Basic
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