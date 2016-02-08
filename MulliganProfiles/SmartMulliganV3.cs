using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartBot.Mulligan;
using SmartBot.Plugins.API;



// ReSharper disable once CheckNamespace

namespace SmartBotUI.SmartMulliganV3
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



        public static List<string> CurrentDeck = new List<string>();


        private readonly Dictionary<string, bool> _whiteList; // CardName, KeepDouble
        private readonly List<Card.Cards> _cardsToKeep;

        #endregion

        public bMulliganProfile()
        {
            _whiteList = new Dictionary<string, bool> { { "GAME_005", true }, { Innervate, true }, { WildGrowth, false } };
            _cardsToKeep = new List<Card.Cards>();
        }


        public List<Card.Cards> HandleMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            CurrentDeck = Bot.CurrentDeck().Cards;
            Bot.Log("[SMV3] You are not suppose to run this, go back to v2 untill I make an appropriate forum thread about it");
            //HandleMinions(choices, _whiteList, myInfo);
            foreach (var s in from s in choices
                              let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString())
                              where _whiteList.ContainsKey(s.ToString())
                              where !keptOneAlready | _whiteList[s.ToString()]
                              select s)
                _cardsToKeep.Add(s);

            return _cardsToKeep;
        }

    }

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
}
