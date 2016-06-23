using System;
using System.Collections.Generic;
using System.IO;
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



        /// <summary>
        /// Adds or updates only 1 card
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="map"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }
        /// <summary>
        /// Adds every card to a whitelist
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="map"></param>
        /// <param name="value"></param>
        /// <param name="keys"></param>
        public static void AddAll<TKey, TValue>(this IDictionary<TKey, TValue> map, TValue value, params TKey[] keys)
        {
            foreach (TKey key in keys)
                map.AddOrUpdate(key, value);
        }
        public static bool IsShitfest(this Bot.Mode mode)
        {
            return mode == Bot.Mode.Arena || mode == Bot.Mode.ArenaAuto;
        }
        /// <summary>
        /// Removes unwanted cards from whitelist
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="map"></param>
        /// <param name="keys"></param>
        public static void BlackListAll<TKey, TValue>(this IDictionary<TKey, TValue> map, params TKey[] keys)
        {
            foreach (TKey key in keys.Where(map.ContainsKey))
                map.Remove(key);
        }
        /// <summary>
        /// Tells if any card in a list is present in another list (more than 1 card)
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="map"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static bool HasAny<TKey>(this IList<TKey> map, params TKey[] entry)
        {
            return map.Intersect(entry).Any();
        }


        /// <summary>
        /// Has all
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="list"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool HasAll<T1>(this IList<T1> list, params T1[] items)
        {
            return !items.Except(list).Any();
        }
        /// <summary>
        /// Checks if a race is present in a list (usually choices)
        /// </summary>
        /// <param name="list"></param>
        /// <param name="race"></param>
        /// <returns></returns>
        public static bool HasRace(this IList<Card.Cards> list, Card.CRace race)
        {
            return list.Any(q => CardTemplate.LoadFromId(q).Race == race);
        }
        /// <summary>
        /// I am lazy, so I just check Card Template with an extension 
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static bool IsMinion(this Card.Cards card)
        {
            return CardTemplate.LoadFromId(card).Type == Card.CType.MINION;
        }
        public static CardTemplate Template(this Card.Cards card)
        {
            return CardTemplate.LoadFromId(card);
        }
        /// <summary>
        /// Same as above, lazyness
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static bool SecretClass(this Card.CClass card)
        {
            return card == Card.CClass.MAGE || card == Card.CClass.HUNTER || card == Card.CClass.PALADIN;
        }
        /// <summary>
        /// Same as above, lazyness, that is why
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static bool WeaponClass(this Card.CClass card)
        {
            return card == Card.CClass.ROGUE || card == Card.CClass.WARRIOR || card == Card.CClass.HUNTER || card == Card.CClass.PALADIN;
        }
        /// <summary>
        /// Should I repeat myself?
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static bool IsSpell(this Card.Cards card)
        {
            return CardTemplate.LoadFromId(card).Type == Card.CType.SPELL;
        }
        public static bool IsCthusWhatever(this DeckType dt)
        {
            return dt.IsOneOf(DeckType.CThunDruid, DeckType.CThunHunter, DeckType.CThunLock,
                DeckType.CThunPaladin, DeckType.CThunMage,
                DeckType.CThunRogue, DeckType.CThunShaman, DeckType.CThunWarrior);
        }
        /// <summary>
        /// Just gonna leave it blank
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static bool IsWeapon(this Card.Cards card)
        {
            return CardTemplate.LoadFromId(card).Type == Card.CType.MINION;
        }
        public static bool HasRamp(this List<Card.Cards> list)
        {
            return list.HasAny(Cards.Innervate, Cards.WildGrowth);
        }
        public static int Cost(this Card.Cards card)
        {
            return CardTemplate.LoadFromId(card).Cost;
        }
        /// <summary>
        /// Counts how many cards you have for a particular turn for that list
        /// Intended to be used with a full deck of 30 cards, otherwise it's kind of useless
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="turn"></param>
        /// <returns></returns>
        public static int TurnPlayCount(this IList<Card.Cards> deck, int turn)
        {
            return deck.Count(cards => CardTemplate.LoadFromId(cards).Cost == turn);
        }
        public static object Fetch(this List<Plugin> list, string name, string data)
        {
            return list.Find(c => c.DataContainer.Name == name).GetProperties()[data];
        }
        /// <summary>
        /// Not to be confused as end conditions. Checks if you have a turn X with a minimum priority of minPriority
        /// </summary>
        /// <param name="list"></param>
        /// <param name="turn"></param>
        /// <param name="minPriority"></param>
        /// <returns></returns>
        public static bool HasTurn(this IList<Card.Cards> list, int turn, int minPriority)
        {
            return list.Any(q => CardTemplate.LoadFromId(q).Cost == turn && (CardTable.ContainsKey(q) && CardTable[q] >= minPriority));
        }
        /// <summary>
        /// Returns predetermined priority value for a card because I am lazy
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public static int Priority(this Card.Cards id, int modifier = 0)
        {
            Report(string.Format("Entered Priority for {0}:{1}", id.Template().Name,id.IsMinion() ? (CardTable[id] + modifier) : 0 ));
            return id.IsMinion() ? (CardTable[id] + modifier) : 0;
        }
        public static bool IsStandard(this Bot.Mode mode)
        {
            return mode == Bot.Mode.RankedStandard || mode == Bot.Mode.UnrankedStandard;
        }
        public static bool Range(this int cost, int min, int max)
        {
            return cost >= min && cost <= max;
        }
        public static bool IsOneOf(this Card.CClass id, params Card.CClass[] list)
        {
            return list.Any(q => q == id);
        }
        public static bool Is(this Card.CClass id, Card.CClass reference)
        {
            return id == reference;
        }
        public static bool Is(this DeckType type, DeckType reference)
        {
            return type == reference;
        }
        public static bool IsOneOf(this DeckType id, params DeckType[] list)
        {
            return list.Any(q => q == id);
        }
        public static void AddInOrder<TKey, TValue>(this IDictionary<TKey, TValue> map, int max, IList<TKey> list, TValue value, params TKey[] keys)
        {
            int endpoint = 0;
            foreach (var q in keys.TakeWhile(q => endpoint < max).Where(list.Contains))
            {
                map[q] = value;
                endpoint++;
            }
        }

        #region CardTable
        public static Dictionary<Card.Cards, int> CardTable = new Dictionary<Card.Cards, int>
        {

            {Card.Cards.CS2_231, 0}, //[1/1]Wisp [0 mana] [NONE card]
            {Card.Cards.LOEA10_3, 0}, //[1/1]Murloc Tinyfin [0 mana] [NONE card]
            {Card.Cards.GVG_093, 0}, //[0/2]Target Dummy [0 mana] [NONE card]
            {Card.Cards.CS1_042, 0}, //[1/2]Goldshire Footman [1 mana] [NONE card]
            {Card.Cards.CS2_065, 3}, //[1/3]Voidwalker [1 mana] [WARLOCK card]
            {Card.Cards.CS2_168, 2}, //[2/1]Murloc Raider [1 mana] [NONE card]
            {Card.Cards.CS2_171, 2}, //[1/1]Stonetusk Boar [1 mana] [NONE card]
            {Card.Cards.CS2_189, 3}, //[1/1]Elven Archer [1 mana] [NONE card]
            {Card.Cards.CS2_235, 4}, //[1/3]Northshire Cleric [1 mana] [PRIEST card]
            {Card.Cards.DS1_175, 0}, //[1/1]Timber Wolf [1 mana] [HUNTER card]
            {Card.Cards.EX1_011, 1}, //[2/1]Voodoo Doctor [1 mana] [NONE card]
            {Card.Cards.EX1_508, 0}, //[1/1]Grimscale Oracle [1 mana] [NONE card]
            {Card.Cards.CS2_059, 1}, //[0/1]Blood Imp [1 mana] [WARLOCK card]
            {Card.Cards.CS2_146, 1}, //[2/1]Southsea Deckhand [1 mana] [NONE card]
            {Card.Cards.CS2_169, 0}, //[1/1]Young Dragonhawk [1 mana] [NONE card]
            {Card.Cards.CS2_188, 4}, //[2/1]Abusive Sergeant [1 mana] [NONE card]
            {Card.Cards.EX1_001, 0}, //[1/2]Lightwarden [1 mana] [NONE card]
            {Card.Cards.EX1_004, 1}, //[2/1]Young Priestess [1 mana] [NONE card]
            {Card.Cards.EX1_008, 3}, //[1/1]Argent Squire [1 mana] [NONE card]
            {Card.Cards.EX1_009, 0}, //[1/1]Angry Chicken [1 mana] [NONE card]
            {Card.Cards.EX1_010, 4}, //[2/1]Worgen Infiltrator [1 mana] [NONE card]
            {Card.Cards.EX1_029, 2}, //[2/1]Leper Gnome [1 mana] [NONE card]
            {Card.Cards.EX1_080, 2}, //[1/2]Secretkeeper [1 mana] [NONE card]
            {Card.Cards.EX1_243, -5}, //[3/1]Dust Devil [1 mana] [SHAMAN card]
            {Card.Cards.EX1_319, 5}, //[3/2]Flame Imp [1 mana] [WARLOCK card]
            {Card.Cards.EX1_405, 0}, //[0/4]Shieldbearer [1 mana] [NONE card]
            {Card.Cards.EX1_509, 0}, //[1/2]Murloc Tidecaller [1 mana] [NONE card]
            {Card.Cards.NEW1_012, 4}, //[1/3]Mana Wyrm [1 mana] [MAGE card]
            {Card.Cards.NEW1_017, 1}, //[1/2]Hungry Crab [1 mana] [NONE card]
            {Card.Cards.NEW1_025, 1}, //[1/2]Bloodsail Corsair [1 mana] [NONE card]
            {Card.Cards.LOE_010, 2}, //[2/1]Pit Snake [1 mana] [ROGUE card]
            {Card.Cards.LOE_018, 5 }, //[1/3]Tunnel Trogg [1 mana] [SHAMAN card]
            {Card.Cards.LOE_076, 4}, //[1/3] Sir Finley Mrrgglton [1 mana] [NONE card]
            {Card.Cards.LOE_116, -1}, //[1/1]Reliquary Seeker [1 mana] [WARLOCK card]
            {Card.Cards.FP1_001, 10}, //[2/3]Zombie Chow [1 mana] [NONE card]
            {Card.Cards.FP1_011, 3}, //[1/1]Webspinner [1 mana] [HUNTER card]
            {Card.Cards.FP1_028, 0}, //[1/2]Undertaker [1 mana] [NONE card]
            {Card.Cards.GVG_009, 0}, //[2/1]Shadowbomber [1 mana] [PRIEST card]
            {Card.Cards.GVG_013, 2}, //[1/2]Cogmaster [1 mana] [NONE card]
            {Card.Cards.GVG_051, 1}, //[1/3]Warbot [1 mana] [WARRIOR card]
            {Card.Cards.GVG_082, 3}, //[2/1]Clockwork Gnome [1 mana] [NONE card]
            {Card.Cards.BRM_004, 2}, //[2/1]Twilight Whelp [1 mana] [PRIEST card]
            {Card.Cards.BRM_022, 0}, //[0/2]Dragon Egg [1 mana] [NONE card]
            {Card.Cards.AT_029, 8}, //[2/1]Buccaneer [1 mana] [ROGUE card]
            {Card.Cards.AT_059, -1}, //[2/1]Brave Archer [1 mana] [HUNTER card]
            {Card.Cards.AT_082, 2}, //[1/2]Lowly Squire [1 mana] [NONE card]
            {Card.Cards.AT_097, 1}, //[2/1]Tournament Attendee [1 mana] [NONE card]
            {Card.Cards.AT_105, 1}, //[2/4]Injured Kvaldir [1 mana] [NONE card]
            {Card.Cards.AT_133, 6}, //[1/2]Gadgetzan Jouster [1 mana] [NONE card]
            {Card.Cards.CS2_120, 3}, //[2/3]River Crocolisk [2 mana] [NONE card]
            {Card.Cards.CS2_121, 2}, //[2/2]Frostwolf Grunt [2 mana] [NONE card]
            {Card.Cards.CS2_142, 1}, //[2/2]Kobold Geomancer [2 mana] [NONE card]
            {Card.Cards.CS2_172, 3}, //[3/2]Bloodfen Raptor [2 mana] [NONE card]
            {Card.Cards.CS2_173, 0}, //[2/1]Bluegill Warrior [2 mana] [NONE card]
            {Card.Cards.EX1_015, 3}, //[1/1]Novice Engineer [2 mana] [NONE card]
            {Card.Cards.EX1_066, 3}, //[3/2]Acidic Swamp Ooze [2 mana] [NONE card]
            {Card.Cards.EX1_306, 1}, //[4/3]Succubus [2 mana] [WARLOCK card]
            {Card.Cards.EX1_506, 2}, //[2/1]Murloc Tidehunter [2 mana] [NONE card]
            {Card.Cards.EX1_565, 4}, //[0/3]Flametongue Totem [2 mana] [SHAMAN card]
            {Card.Cards.CS2_203, 0}, //[2/1]Ironbeak Owl [2 mana] [NONE card]
            {Card.Cards.EX1_012, 2}, //[1/1]Bloodmage Thalnos [2 mana] [NONE card]
            {Card.Cards.EX1_045, 2}, //[4/5]Ancient Watcher [2 mana] [NONE card]
            {Card.Cards.EX1_049, 3}, //[3/2]Youthful Brewmaster [2 mana] [NONE card]
            {Card.Cards.EX1_055, 2}, //[1/3]Mana Addict [2 mana] [NONE card]
            {Card.Cards.EX1_058, 3}, //[2/3]Sunfury Protector [2 mana] [NONE card]
            {Card.Cards.EX1_059, 2}, //[2/2]Crazed Alchemist [2 mana] [NONE card]
            {Card.Cards.EX1_076, 1}, //[2/2]Pint-Sized Summoner [2 mana] [NONE card]
            {Card.Cards.EX1_082, 3}, //[3/2]Mad Bomber [2 mana] [NONE card]
            {Card.Cards.EX1_096, 4}, //[2/1]Loot Hoarder [2 mana] [NONE card]
            {Card.Cards.EX1_100, 0}, //[0/4]Lorewalker Cho [2 mana] [NONE card]
            {Card.Cards.EX1_131, 2}, //[2/2]Defias Ringleader [2 mana] [ROGUE card]
            {Card.Cards.EX1_162, 2}, //[2/2]Dire Wolf Alpha [2 mana] [NONE card]
            {Card.Cards.EX1_341, 2}, //[0/5]Lightwell [2 mana] [PRIEST card]
            {Card.Cards.EX1_362, 0}, //[2/2]Argent Protector [2 mana] [PALADIN card]
            {Card.Cards.EX1_393, 3}, //[2/3]Amani Berserker [2 mana] [NONE card]
            {Card.Cards.EX1_402, 2}, //[1/4]Armorsmith [2 mana] [WARRIOR card]
            {Card.Cards.EX1_522, 1}, //[1/1]Patient Assassin [2 mana] [ROGUE card]
            {Card.Cards.EX1_531, 2}, //[2/2]Scavenging Hyena [2 mana] [HUNTER card]
            {Card.Cards.EX1_557, 3}, //[0/4]Nat Pagle [2 mana] [NONE card]
            {Card.Cards.EX1_603, 2}, //[2/2]Cruel Taskmaster [2 mana] [WARRIOR card]
            {Card.Cards.EX1_608, 3}, //[3/2]Sorcerer's Apprentice [2 mana] [MAGE card]
            {Card.Cards.EX1_616, 2}, //[2/2]Mana Wraith [2 mana] [NONE card]
            {Card.Cards.NEW1_018, 3}, //[2/3]Bloodsail Raider [2 mana] [NONE card]
            {Card.Cards.NEW1_019, 4}, //[3/2]Knife Juggler [2 mana] [NONE card]
            {Card.Cards.NEW1_020, 3},//[3/2]Wild Pyromancer [2 mana] [NONE card]
            {Card.Cards.NEW1_021, 1}, //[0/7]Doomsayer [2 mana] [NONE card]
            {Card.Cards.NEW1_023, 3}, //[3/2]Faerie Dragon [2 mana] [NONE card]
            {Card.Cards.NEW1_029, 4}, //[4/4]Millhouse Manastorm [2 mana] [NONE card]
            {Card.Cards.NEW1_037, 2}, //[1/3]Master Swordsmith [2 mana] [NONE card]
            {Card.Cards.LOE_006, 2}, //[1/2]Museum Curator [2 mana] [PRIEST card]
            {Card.Cards.LOE_023, 4}, //[2/2]Dark Peddler [2 mana] [WARLOCK card]
            {Card.Cards.LOE_029, 4}, //[1/1]Jeweled Scarab [2 mana] [NONE card]
            {Card.Cards.LOE_046, 4}, //[3/2]Huge Toad [2 mana] [NONE card]
            {Card.Cards.NEW1_016, 1}, //[1/1]Captain's Parrot [2 mana] [NONE card]
            {Card.Cards.FP1_002, 4}, //[1/2]Haunted Creeper [2 mana] [NONE card]
            {Card.Cards.FP1_003, 3}, //[1/2]Echoing Ooze [2 mana] [NONE card]
            {Card.Cards.FP1_004, 4}, //[2/2]Mad Scientist [2 mana] [NONE card]
            {Card.Cards.FP1_007,0}, //[0/2]Nerubian Egg [2 mana] [NONE card]
            {Card.Cards.FP1_017, 2}, //[1/4]Nerub'ar Weblord [2 mana] [NONE card]
            {Card.Cards.FP1_024, 2}, //[1/3]Unstable Ghoul [2 mana] [NONE card]
            {Card.Cards.GVG_002, 4}, //[2/3]Snowchugger [2 mana] [MAGE card]
            {Card.Cards.GVG_006, 5}, //[2/3]Mechwarper [2 mana] [NONE card]
            {Card.Cards.GVG_011, 3}, //[3/2]Shrinkmeister [2 mana] [PRIEST card]
            {Card.Cards.GVG_018, 2}, //[1/4]Mistress of Pain [2 mana] [WARLOCK card]
            {Card.Cards.GVG_023, 5}, //[3/2]Goblin Auto-Barber [2 mana] [ROGUE card]
            {Card.Cards.GVG_025, 2}, //[4/1]One-eyed Cheat [2 mana] [ROGUE card]
            {Card.Cards.GVG_030, 3}, //[2/2]Anodized Robo Cub [2 mana] [DRUID card]
            {Card.Cards.GVG_037, 3}, //[3/2]Whirling Zap-o-matic [2 mana] [SHAMAN card]
            {Card.Cards.GVG_039, 0}, //[0/3]Vitality Totem [2 mana] [SHAMAN card]
            {Card.Cards.GVG_058, 5}, //[2/2]Shielded Minibot [2 mana] [PALADIN card]
            {Card.Cards.GVG_064, 3}, //[3/2]Puddlestomper [2 mana] [NONE card]
            {Card.Cards.GVG_067, 3}, //[2/3]Stonesplinter Trogg [2 mana] [NONE card]
            {Card.Cards.GVG_072, 4}, //[2/3]Shadowboxer [2 mana] [PRIEST card]
            {Card.Cards.GVG_075, 3}, //[2/3]Ship's Cannon [2 mana] [NONE card]
            {Card.Cards.GVG_076, 1}, //[1/1]Explosive Sheep [2 mana] [NONE card]
            {Card.Cards.GVG_081, 3}, //[2/3]Gilblin Stalker [2 mana] [NONE card]
            {Card.Cards.GVG_085, 2}, //[1/2]Annoy-o-Tron [2 mana] [NONE card]
            {Card.Cards.GVG_087, 3}, //[2/3]Steamwheedle Sniper [2 mana] [HUNTER card]
            {Card.Cards.GVG_103, 2}, //[1/2]Micro Machine [2 mana] [NONE card]
            {Card.Cards.GVG_108, 3}, //[3/2]Recombobulator [2 mana] [NONE card]
            {Card.Cards.AT_003, 3}, //[3/2]Fallen Hero [2 mana] [MAGE card]
            {Card.Cards.AT_021, 3}, //[3/2]Tiny Knight of Evil [2 mana] [WARLOCK card]
            {Card.Cards.AT_026, 2}, //[4/3]Wrathguard [2 mana] [WARLOCK card]
            {Card.Cards.AT_030, 4}, //[3/2]Undercity Valiant [2 mana] [ROGUE card]
            {Card.Cards.AT_031, 1}, //[2/2]Cutpurse [2 mana] [ROGUE card]
            {Card.Cards.AT_038, 10}, //[2/3]Darnassus Aspirant [2 mana] [DRUID card]
            {Card.Cards.AT_042, 2}, //[2/1]Druid of the Saber [2 mana] [DRUID card]
            {Card.Cards.AT_052, 8}, //[3/4]Totem Golem [2 mana] [SHAMAN card]
            {Card.Cards.AT_058, 3}, //[3/2]King's Elekk [2 mana] [HUNTER card]
            {Card.Cards.AT_069, 3}, //[3/2]Sparring Partner [2 mana] [WARRIOR card]
            {Card.Cards.AT_071, 3}, //[2/3]Alexstrasza's Champion [2 mana] [WARRIOR card]
            {Card.Cards.AT_080, 3}, //[2/3]Garrison Commander [2 mana] [NONE card]
            {Card.Cards.AT_084, 2}, //[1/2]Lance Carrier [2 mana] [NONE card]
            {Card.Cards.AT_089, 4}, //[3/2]Boneguard Lieutenant [2 mana] [NONE card]
            {Card.Cards.AT_094, 3}, //[2/3]Flame Juggler [2 mana] [NONE card]
            {Card.Cards.AT_109, 2}, //[2/4]Argent Watchman [2 mana] [NONE card]
            {Card.Cards.AT_116, 1}, //[1/4]Wyrmrest Agent [2 mana] [PRIEST card]
            {Card.Cards.CS2_118, -1}, //[5/1]Magma Rager [3 mana] [NONE card]
            {Card.Cards.CS2_122, 1}, //[2/2]Raid Leader [3 mana] [NONE card]
            {Card.Cards.CS2_124, -1}, //[3/1]Wolfrider [3 mana] [NONE card]
            {Card.Cards.CS2_125, 2}, //[3/3]Ironfur Grizzly [3 mana] [NONE card]
            {Card.Cards.CS2_127, 0}, //[1/4]Silverback Patriarch [3 mana] [NONE card]
            {Card.Cards.CS2_141, 2}, //[2/2]Ironforge Rifleman [3 mana] [NONE card]
            {Card.Cards.CS2_196, 3}, //[2/3]Razorfen Hunter [3 mana] [NONE card]
            {Card.Cards.EX1_019, 3}, //[3/2]Shattered Sun Cleric [3 mana] [NONE card]
            {Card.Cards.EX1_084, 1}, //[2/3]Warsong Commander [3 mana] [WARRIOR card]
            {Card.Cards.EX1_582, 1}, //[1/4]Dalaran Mage [3 mana] [NONE card]
            {Card.Cards.CS2_117, 3}, //[3/3]Earthen Ring Farseer [3 mana] [NONE card]
            {Card.Cards.CS2_181, 5}, //[4/7]Injured Blademaster [3 mana] [NONE card]
            {Card.Cards.EX1_005, 0}, //[4/2]Big Game Hunter [3 mana] [NONE card]
            {Card.Cards.EX1_006, 0}, //[0/3]Alarm-o-Bot [3 mana] [NONE card]
            {Card.Cards.EX1_007, 2}, //[1/3]Acolyte of Pain [3 mana] [NONE card]
            {Card.Cards.EX1_014, 5}, //[5/5]King Mukla [3 mana] [NONE card]
            {Card.Cards.EX1_017, 4}, //[4/2]Jungle Panther [3 mana] [NONE card]
            {Card.Cards.EX1_020, 4}, //[3/1]Scarlet Crusader [3 mana] [NONE card]
            {Card.Cards.EX1_021, 1}, //[2/3]Thrallmar Farseer [3 mana] [NONE card]
            {Card.Cards.EX1_044,2}, //[2/2]Questing Adventurer [3 mana] [NONE card]
            {Card.Cards.EX1_050, 1}, //[2/2]Coldlight Oracle [3 mana] [NONE card]
            {Card.Cards.EX1_083, 3}, //[3/3]Tinkmaster Overspark [3 mana] [NONE card]
            {Card.Cards.EX1_085, 0}, //[3/3]Mind Control Tech [3 mana] [NONE card]
            {Card.Cards.EX1_089, 0}, //[4/2]Arcane Golem [3 mana] [NONE card]
            {Card.Cards.EX1_102, 2}, //[1/4]Demolisher [3 mana] [NONE card]
            {Card.Cards.EX1_103, 2}, //[2/3]Coldlight Seer [3 mana] [NONE card]
            {Card.Cards.EX1_134, 4}, //[3/3]SI:7 Agent [3 mana] [ROGUE card]
            {Card.Cards.EX1_170, 2}, //[2/3]Emperor Cobra [3 mana] [NONE card]
            {Card.Cards.EX1_258, 2}, //[2/4]Unbound Elemental [3 mana] [SHAMAN card]
            {Card.Cards.EX1_301, 4}, //[3/5]Felguard [3 mana] [WARLOCK card]
            {Card.Cards.EX1_304, 2}, //[3/3]Void Terror [3 mana] [WARLOCK card]
            {Card.Cards.EX1_382, 3}, //[3/3]Aldor Peacekeeper [3 mana] [PALADIN card]
            {Card.Cards.EX1_390, 1}, //[2/3]Tauren Warrior [3 mana] [NONE card]
            {Card.Cards.EX1_412, 2}, //[3/3]Raging Worgen [3 mana] [NONE card]
            {Card.Cards.EX1_507, 2}, //[3/3]Murloc Warleader [3 mana] [NONE card]
            {Card.Cards.EX1_556, 6}, //[2/3]Harvest Golem [3 mana] [NONE card]
            {Card.Cards.EX1_575, 0}, //[0/3]Mana Tide Totem [3 mana] [SHAMAN card]
            {Card.Cards.EX1_590, 3}, //[3/3]Blood Knight [3 mana] [NONE card]
            {Card.Cards.EX1_597, 4}, //[1/5]Imp Master [3 mana] [NONE card]
            {Card.Cards.EX1_604, 3}, //[2/4]Frothing Berserker [3 mana] [WARRIOR card]
            {Card.Cards.EX1_612, 4}, //[4/3]Kirin Tor Mage [3 mana] [MAGE card]
            {Card.Cards.EX1_613, 0}, //[2/2]Edwin VanCleef [3 mana] [ROGUE card]
            {Card.Cards.NEW1_027, 1}, //[3/3]Southsea Captain [3 mana] [NONE card]
            {Card.Cards.tt_004, 2}, //[2/3]Flesheating Ghoul [3 mana] [NONE card]
            {Card.Cards.LOE_019, 4}, //[3/4]Unearthed Raptor [3 mana] [ROGUE card]
            {Card.Cards.LOE_020, 1}, //[2/4]Desert Camel [3 mana] [HUNTER card]
            {Card.Cards.LOE_022, 4}, //[3/4]Fierce Monkey [3 mana] [WARRIOR card]
            {Card.Cards.LOE_050, 4}, //[3/2]Mounted Raptor [3 mana] [DRUID card]
            {Card.Cards.LOE_077, 2}, //[2/4]Brann Bronzebeard [3 mana] [NONE card]
            {Card.Cards.FP1_005, 0}, //[2/2]Shade of Naxxramas [3 mana] [NONE card]
            {Card.Cards.FP1_009, 4}, //[2/8]Deathlord [3 mana] [NONE card]
            {Card.Cards.FP1_023, 5}, //[3/4]Dark Cultist [3 mana] [PRIEST card]
            {Card.Cards.FP1_027, 2}, //[1/4]Stoneskin Gargoyle [3 mana] [NONE card]
            {Card.Cards.FP1_029, 5}, //[4/4]Dancing Swords [3 mana] [NONE card]
            {Card.Cards.GVG_027, 3}, //[2/2]Iron Sensei [3 mana] [ROGUE card]
            {Card.Cards.GVG_032, 2}, //[2/4]Grove Tender [3 mana] [DRUID card]
            {Card.Cards.GVG_044, 5}, //[3/4]Spider Tank [3 mana] [NONE card]
            {Card.Cards.GVG_048, 4}, //[3/3]Metaltooth Leaper [3 mana] [HUNTER card]
            {Card.Cards.GVG_065, 5}, //[4/4]Ogre Brute [3 mana] [NONE card]
            {Card.Cards.GVG_084, 0}, //[1/4]Flying Machine [3 mana] [NONE card]
            {Card.Cards.GVG_089, 0}, //[2/4]Illuminator [3 mana] [NONE card]
            {Card.Cards.GVG_092, 2}, //[3/2]Gnomish Experimenter [3 mana] [NONE card]
            {Card.Cards.GVG_095, 0}, //[2/4]Goblin Sapper [3 mana] [NONE card]
            {Card.Cards.GVG_097, 2}, //[2/3]Lil' Exorcist [3 mana] [NONE card]
            {Card.Cards.GVG_098, 0}, //[1/4]Gnomeregan Infantry [3 mana] [NONE card]
            {Card.Cards.GVG_101, 3}, //[4/3]Scarlet Purifier [3 mana] [PALADIN card]
            {Card.Cards.GVG_102, 2}, //[3/3]Tinkertown Technician [3 mana] [NONE card]
            {Card.Cards.GVG_104, 1}, //[2/3]Hobgoblin [3 mana] [NONE card]
            {Card.Cards.GVG_123, 4}, //[3/3]Soot Spewer [3 mana] [MAGE card]
            {Card.Cards.BRM_002, 3}, //[2/4]Flamewaker [3 mana] [MAGE card]
            {Card.Cards.BRM_006, 5}, //[2/4]Imp Gang Boss [3 mana] [WARLOCK card]
            {Card.Cards.BRM_010, 3}, //[2/2]Druid of the Flame [3 mana] [DRUID card]
            {Card.Cards.BRM_033, 3}, //[2/4]Blackwing Technician [3 mana] [NONE card]
            {Card.Cards.AT_007, 4}, //[3/4]Spellslinger [3 mana] [MAGE card]
            {Card.Cards.AT_014, 2}, //[3/3]Shadowfiend [3 mana] [PRIEST card]
            {Card.Cards.AT_032, 2}, //[4/3]Shady Dealer [3 mana] [ROGUE card]
            {Card.Cards.AT_046, 3}, //[3/2]Tuskarr Totemic [3 mana] [SHAMAN card]
            {Card.Cards.AT_057, 2}, //[4/2]Stablemaster [3 mana] [HUNTER card]
            {Card.Cards.AT_063t, 2}, //[4/2]Dreadscale [3 mana] [HUNTER card]
            {Card.Cards.AT_066, 1}, //[3/3]Orgrimmar Aspirant [3 mana] [WARRIOR card]
            {Card.Cards.AT_075, 0}, //[2/4]Warhorse Trainer [3 mana] [PALADIN card]
            {Card.Cards.AT_083, 4}, //[3/3]Dragonhawk Rider [3 mana] [NONE card]
            {Card.Cards.AT_086, 3}, //[4/3]Saboteur [3 mana] [NONE card]
            {Card.Cards.AT_087, 3}, //[2/1]Argent Horserider [3 mana] [NONE card]
            {Card.Cards.AT_092, 0}, //[5/2]Ice Rager [3 mana] [NONE card]
            {Card.Cards.AT_095, 5}, //[2/2]Silent Knight [3 mana] [NONE card]
            {Card.Cards.AT_100, 6}, //[3/3]Silver Hand Regent [3 mana] [NONE card]
            {Card.Cards.AT_106, 2}, //[4/3]Light's Champion [3 mana] [NONE card]
            {Card.Cards.AT_110, 4}, //[2/5]Coliseum Manager [3 mana] [NONE card]
            {Card.Cards.AT_115, 1}, //[2/2]Fencing Coach [3 mana] [NONE card]
            {Card.Cards.AT_117, 0}, //[4/2]Master of Ceremonies [3 mana] [NONE card]
            {Card.Cards.AT_129, 4}, //[3/4]Fjola Lightbane [3 mana] [NONE card]
            {Card.Cards.AT_131, 4}, //[3/4]Eydis Darkbane [3 mana] [NONE card]
            {Card.Cards.CS2_033, 6}, //[3/6]Water Elemental [4 mana] [MAGE card]
            {Card.Cards.CS2_119, 0}, //[2/7]Oasis Snapjaw [4 mana] [NONE card]
            {Card.Cards.CS2_131, 0}, //[2/5]Stormwind Knight [4 mana] [NONE card]
            {Card.Cards.CS2_147, 0}, //[2/4]Gnomish Inventor [4 mana] [NONE card]
            {Card.Cards.CS2_179, 4}, //[3/5]Sen'jin Shieldmasta [4 mana] [NONE card]
            {Card.Cards.CS2_182, 5}, //[4/5]Chillwind Yeti [4 mana] [NONE card]
            {Card.Cards.CS2_197, 3}, //[4/4]Ogre Magi [4 mana] [NONE card]
            {Card.Cards.DS1_070, 0}, //[4/3]Houndmaster [4 mana] [HUNTER card]
            {Card.Cards.EX1_025, 0}, //[2/4]Dragonling Mechanic [4 mana] [NONE card]
            {Card.Cards.EX1_587, 0}, //[3/3]Windspeaker [4 mana] [SHAMAN card]
            {Card.Cards.NEW1_011, 0}, //[4/3]Kor'kron Elite [4 mana] [WARRIOR card]
            {Card.Cards.EX1_023, 0}, //[3/3]Silvermoon Guardian [4 mana] [NONE card]
            {Card.Cards.EX1_043, 1}, //[4/1]Twilight Drake [4 mana] [NONE card]
            {Card.Cards.EX1_046, 5}, //[4/4]Dark Iron Dwarf [4 mana] [NONE card]
            {Card.Cards.EX1_048, 0}, //[4/3]Spellbreaker [4 mana] [NONE card]
            {Card.Cards.EX1_057, 0}, //[5/4]Ancient Brewmaster [4 mana] [NONE card]
            {Card.Cards.EX1_093, 0}, //[2/3]Defender of Argus [4 mana] [NONE card]
            {Card.Cards.EX1_166, 0}, //[2/4]Keeper of the Grove [4 mana] [DRUID card]
            {Card.Cards.EX1_274, 0}, //[3/3]Ethereal Arcanist [4 mana] [MAGE card]
            {Card.Cards.EX1_313, 0}, //[5/6]Pit Lord [4 mana] [WARLOCK card]
            {Card.Cards.EX1_315, 0}, //[0/4]Summoning Portal [4 mana] [WARLOCK card]
            {Card.Cards.EX1_335, 0}, //[0/5]Lightspawn [4 mana] [PRIEST card]
            {Card.Cards.EX1_396, 0}, //[1/7]Mogu'shan Warden [4 mana] [NONE card]
            {Card.Cards.EX1_398, 0}, //[3/3]Arathi Weaponsmith [4 mana] [WARRIOR card]
            {Card.Cards.EX1_584, 0}, //[2/5]Ancient Mage [4 mana] [NONE card]
            {Card.Cards.EX1_591, 1}, //[3/5]Auchenai Soulpriest [4 mana] [PRIEST card]
            {Card.Cards.EX1_595, 0}, //[4/2]Cult Master [4 mana] [NONE card]
            {Card.Cards.NEW1_014, 0}, //[4/4]Master of Disguise [4 mana] [ROGUE card]
            {Card.Cards.NEW1_022, 0}, //[3/3]Dread Corsair [4 mana] [NONE card]
            {Card.Cards.NEW1_026, 0}, //[3/5]Violet Teacher [4 mana] [NONE card]
            {Card.Cards.LOE_012, 1}, //[5/4]Tomb Pillager [4 mana] [ROGUE card]
            {Card.Cards.LOE_016, 0}, //[2/6]Rumbling Elemental [4 mana] [SHAMAN card]
            {Card.Cards.LOE_017, 0}, //[3/4]Keeper of Uldaman [4 mana] [PALADIN card]
            {Card.Cards.LOE_039, 0}, //[3/4]Gorillabot A-3 [4 mana] [NONE card]
            {Card.Cards.LOE_047, 0}, //[3/3]Tomb Spider [4 mana] [NONE card]
            {Card.Cards.LOE_051, 0}, //[4/4]Jungle Moonkin [4 mana] [DRUID card]
            {Card.Cards.LOE_079, 0}, //[3/5]Elise Starseeker [4 mana] [NONE card]
            {Card.Cards.LOE_107, 0}, //[7/7]Eerie Statue [4 mana] [NONE card]
            {Card.Cards.LOE_110, 0}, //[7/4]Ancient Shade [4 mana] [NONE card]
            {Card.Cards.EX1_062, 0}, //[2/4]Old Murk-Eye [4 mana] [NONE card]
            {Card.Cards.FP1_016, 0}, //[3/5]Wailing Soul [4 mana] [NONE card]
            {Card.Cards.FP1_022, 4}, //[3/4]Voidcaller [4 mana] [WARLOCK card]
            {Card.Cards.FP1_026, 3}, //[5/5]Anub'ar Ambusher [4 mana] [ROGUE card]
            {Card.Cards.FP1_031, 0}, //[1/7]Baron Rivendare [4 mana] [NONE card]
            {Card.Cards.GVG_004, 3}, //[5/4]Goblin Blastmage [4 mana] [MAGE card]
            {Card.Cards.GVG_020, 1}, //[3/5]Fel Cannon [4 mana] [WARLOCK card]
            {Card.Cards.GVG_040, 0}, //[2/5]Siltfin Spiritwalker [4 mana] [SHAMAN card]
            {Card.Cards.GVG_055, 0}, //[2/5]Screwjank Clunker [4 mana] [WARRIOR card]
            {Card.Cards.GVG_066, 0}, //[5/4]Dunemaul Shaman [4 mana] [SHAMAN card]
            {Card.Cards.GVG_068, 0}, //[3/5]Burly Rockjaw Trogg [4 mana] [NONE card]
            {Card.Cards.GVG_071, 0}, //[5/4]Lost Tallstrider [4 mana] [NONE card]
            {Card.Cards.GVG_074, 0}, //[4/3]Kezan Mystic [4 mana] [NONE card]
            {Card.Cards.GVG_078, 5}, //[4/5]Mechanical Yeti [4 mana] [NONE card]
            {Card.Cards.GVG_091, 4}, //[2/5]Arcane Nullifier X-21 [4 mana] [NONE card]
            {Card.Cards.GVG_094, 0}, //[1/4]Jeeves [4 mana] [NONE card]
            {Card.Cards.GVG_096, 8}, //[4/3]Piloted Shredder [4 mana] [NONE card]
            {Card.Cards.GVG_107, 0}, //[3/2]Enhance-o Mechano [4 mana] [NONE card]
            {Card.Cards.GVG_109, 0}, //[4/1]Mini-Mage [4 mana] [NONE card]
            {Card.Cards.GVG_122, 0}, //[2/5]Wee Spellstopper [4 mana] [MAGE card]
            {Card.Cards.BRM_012, 0}, //[3/6]Fireguard Destroyer [4 mana] [SHAMAN card]
            {Card.Cards.BRM_014, 0}, //[4/4]Core Rager [4 mana] [HUNTER card]
            {Card.Cards.BRM_016, 0}, //[2/5]Axe Flinger [4 mana] [WARRIOR card]
            {Card.Cards.BRM_020, 3}, //[3/5]Dragonkin Sorcerer [4 mana] [NONE card]
            {Card.Cards.BRM_026, 2}, //[5/6]Hungry Dragon [4 mana] [NONE card]
            {Card.Cards.AT_006, 3}, //[3/5]Dalaran Aspirant [4 mana] [MAGE card]
            {Card.Cards.AT_011, 5}, //[3/5]Holy Champion [4 mana] [PRIEST card]
            {Card.Cards.AT_012, 3}, //[5/4]Spawn of Shadows [4 mana] [PRIEST card]
            {Card.Cards.AT_017, 1}, //[2/6]Twilight Guardian [4 mana] [NONE card]
            {Card.Cards.AT_019, 0}, //[1/1]Dreadsteed [4 mana] [WARLOCK card]
            {Card.Cards.AT_039, 6}, //[5/4]Savage Combatant [4 mana] [DRUID card]
            {Card.Cards.AT_040, 0}, //[4/4]Wildwalker [4 mana] [DRUID card]
            {Card.Cards.AT_047, 0}, //[4/4]Draenei Totemcarver [4 mana] [SHAMAN card]
            {Card.Cards.AT_067, 0}, //[5/3]Magnataur Alpha [4 mana] [WARRIOR card]
            {Card.Cards.AT_076, 0}, //[3/4]Murloc Knight [4 mana] [PALADIN card]
            {Card.Cards.AT_085, 0}, //[2/6]Maiden of the Lake [4 mana] [NONE card]
            {Card.Cards.AT_091, 0}, //[1/8]Tournament Medic [4 mana] [NONE card]
            {Card.Cards.AT_093, 0}, //[2/6]Frigid Snobold [4 mana] [NONE card]
            {Card.Cards.AT_108, 0}, //[5/3]Armored Warhorse [4 mana] [NONE card]
            {Card.Cards.AT_111, 0}, //[3/5]Refreshment Vendor [4 mana] [NONE card]
            {Card.Cards.AT_114, 4}, //[5/4]Evil Heckler [4 mana] [NONE card]
            {Card.Cards.AT_121, 0}, //[4/4]Crowd Favorite [4 mana] [NONE card]
            {Card.Cards.AT_122, 0}, //[4/4]Gormok the Impaler [4 mana] [NONE card]
            /*Whisper*/
            {Cards.ForbiddenAncient, 0 },
            {Cards.PossessedVillager, 3},
            {Cards.FieryBat, 3 },
            {Cards.SelflessHero, 1 },
            {Cards.VilefinInquisitor, 4 },
            {Cards.NZothsFirstMate, 3 },
            {Cards.ShifterZerus, 1 },
            {Cards.TentacleofNZoth, 2 },
            {Cards.ZealousInitiate, 1 },
            {Cards.CultSorcerer, 3 },
            {Cards.UndercityHuckster, 2 },
            {Cards.EternalSentinel, 2 },
            {Cards.DarkshireLibrarian, 1 },
            {Cards.BilefinTidehunter, 2 },
            {Cards.Duskboar, 0 },
            {Cards.NattheDarkfisher, -1 },
            {Cards.BeckonerofEvil, 5},
            {Cards.TwilightGeomancer, 2 },
            {Cards.TwistedWorgen, 3 },
            {Cards.AddledGrizzly, 0 },
            {Cards.TwilightFlamecaller, 2 },
            {Cards.StewardofDarkshire, 2 },
            {Cards.DarkshireCouncilman, 3 },
            {Cards.BloodsailCultist, 3 },
            {Cards.RavagingGhoul, 2 },
            {Cards.AmgamRager, 2 },
            {Cards.DiscipleofCThun, 3 },
            {Cards.SilithidSwarmer, 2 },
            {Cards.SpawnofNZoth, 1 },
            {Cards.SquirmingTentacle, 4 },
            {Cards.TwilightElder, 5 },
            {Cards.FandralStaghelm, 3 },
            {Cards.KlaxxiAmberWeaver, 2 },
            {Cards.MireKeeper, 1 },
            {Cards.InfestedWolf, 1 },
            {Cards.DementedFrostcaller, 2 },
            {Cards.HoodedAcolyte, 1 },
            {Cards.ShiftingShade, 0 },
            {Cards.SouthseaSquidface, 1 },
            {Cards.XarilPoisonedMind, 0 },
            {Cards.FlamewreathedFaceless, 4 },
            {Cards.MasterofEvolution, 4 },
            {Cards.BloodhoofBrave, 2 },
            {Cards.AberrantBerserker, 0 },
            {Cards.BlackwaterPirate, 0 },
            {Cards.CThunsChosen, 0 },
            {Cards.CyclopianHorror, 1 },
            {Cards.EaterofSecrets, 0 },
            {Cards.EvolvedKobold, 0 },
            {Cards.FacelessShambler, 0 },
            {Cards.InfestedTauren, 4 },
            {Cards.MidnightDrake, 0 },
            {Cards.PollutedHoarder, 0 },
            {Cards.TwilightSummoner, 0 },
         };
        public static void Report(string msg)
        {
            using (StreamWriter log = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\DebugLog.txt", true))
            {
                log.WriteLine("[{0}] {1}", DateTime.Now, msg);
            }
        }
        #endregion
        /// <summary>
        /// This is a useless method
        /// </summary>
        /// <param name="deck"></param>
        /// <returns></returns>
        public static Tuple<int, int, int> NumOfTypes(this IList<Card.Cards> deck)
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


        public static bool Aggresive(this Style st)
        {
            return st == Style.Face || st == Style.Aggro || st == Style.Tempo;
        }

        public static DeckType FindHimInHistory(this long id, Card.CClass op, int n = 50)
        {
            Prediction prediction = new Prediction();

            List<string> history = File.ReadLines(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\MatchHistory.txt").Reverse().Take(n).ToList();
            Report("FHF: 1.0");
            List<DeckType> allDeckTypes = (from q in history select q.Split(new[] { "||" }, StringSplitOptions.None) into information let enemyId = long.Parse(information[2]) where id == enemyId select (DeckType)Enum.Parse(typeof(DeckType), information[8])).ToList();
            Report("History Search generated all deck types from history");
            if (allDeckTypes.Count >= 1)
            {
                Report("Going through all deck types in history");
                foreach (var q in allDeckTypes)
                {
                    try
                    {
                        Report(string.Format("{0}:{1}", q.ToString(), DeckClass[q]));
                    }
                    catch (Exception)
                    {

                        continue;
                    }
                } 
                Report("==================>" +allDeckTypes.First());
                Report(op.ToString());
                Report((DeckClass[allDeckTypes.First()] == op).ToString());
                DeckType eDeckType = DeckType.Unknown;//                   allDeckTypes.First(q => DeckClass[q] == op);
                foreach(var q in allDeckTypes)
                {
                    
                    if (q == DeckType.Basic) continue;
                    if (DeckClass[q] == op)
                    {
                        eDeckType = q;
                        break;
                    }
                    
                }
                Report("FHF: 2.1");
                
                try
                {
                    eDeckType = prediction.GetMostFacedDeckType(op);
                }catch(Exception)
                {
                    eDeckType = GetDefault(op);
                    if (!Bot.CurrentMode().IsShitfest())
                    Bot.Log("[ABTracker] You have faced this opponent before, but he was playing a different class, so going to guees he is "+eDeckType);
                    return eDeckType;
                }
                Report("FHF: 2.2");
                if (!Bot.CurrentMode().IsShitfest())
                    Bot.Log(string.Format("[ABTracker] You have faced this opponent before, his last played deck with {0} was {1}", op.ToString().ToLower(), eDeckType));
                Report("Mulligan Stage 2 Completed without accident");
                return eDeckType;
            }
            Report("History: Never seen the guy territory");
            foreach (var q in history)
            {
                try
                {
                    var info = q.Split(new[] { "||" }, StringSplitOptions.None);

                    if ((Card.CClass)Enum.Parse(typeof(Card.CClass), info[7]) != op) continue;
                    if (info[3] != Bot.CurrentMode().ToString()) continue;

                    prediction.Update(op, (DeckType)Enum.Parse(typeof(DeckType), info[8]));
                }
                catch (Exception e)
                {
                    Bot.Log("[URGENT] Please locate DebugLog.txt in Logs/ABTracker/ folder and show it to Arthur");

                    Report("history check error: "+e.Message);
                    continue;
                }
            }
             Report("FHF: 4.0");
            DeckType unknownPrediction;
            try
            {
                unknownPrediction = prediction.GetMostFacedDeckType(op);
            }
            catch (Exception notfound)
            {
                Report("History didn't yield to any worthwhile results");
                unknownPrediction = GetDefault(op);
            }
             Report("FHF: 5.0");
            if (!Bot.CurrentMode().IsShitfest())
                Bot.Log(string.Format("[Arthurs' Bundle: Mulligan] You have not faced this opponent in the past {0} games. From your history, you mostly face {1} decks, so that is what we will go with.", n, unknownPrediction));
                            Report("FHF: 6.0");
            Report("Mulligan Stage 2 Completed without accident");
            return unknownPrediction;
        }

        private static DeckType GetDefault(Card.CClass op)
        {
            switch (op)
            {
                case Card.CClass.SHAMAN:
                return DeckType.FaceShaman;
                case Card.CClass.PRIEST:
                return DeckType.ControlPriest; ;
                case Card.CClass.MAGE:
                return DeckType.TempoMage; ;
                case Card.CClass.PALADIN:
                return DeckType.MidRangePaladin; ;
                case Card.CClass.WARRIOR:
                return DeckType.ControlWarrior; ;
                case Card.CClass.WARLOCK:
                return DeckType.Zoolock; ;
                case Card.CClass.HUNTER:
                return DeckType.MidRangeHunter; ;
                case Card.CClass.ROGUE:
                return DeckType.MiracleRogue; ;
                case Card.CClass.DRUID:
                return DeckType.CThunDruid; ;
                case Card.CClass.NONE:
                return DeckType.Unknown; ;
                case Card.CClass.JARAXXUS:
                return DeckType.Unknown; ;

            }
            return DeckType.Unknown;
        }

        #region deckclass

        public static Dictionary<DeckType, Card.CClass> DeckClass = new Dictionary<DeckType, Card.CClass>
        {
            { DeckType.Unknown, Card.CClass.NONE}, {DeckType.Arena, Card.CClass.NONE}, /*Warrior*/
            { DeckType.TempoWarrior, Card.CClass.WARRIOR },
            { DeckType.ControlWarrior, Card.CClass.WARRIOR},
            { DeckType.FatigueWarrior, Card.CClass.WARRIOR},
            { DeckType.DragonWarrior, Card.CClass.WARRIOR},
            { DeckType.PatronWarrior, Card.CClass.WARRIOR},
            { DeckType.WorgenOTKWarrior, Card.CClass.WARRIOR},
            { DeckType.MechWarrior, Card.CClass.WARRIOR},
            { DeckType.FaceWarrior, Card.CClass.WARRIOR},
            { DeckType.RenoWarrior, Card.CClass.WARRIOR}, /*Paladin*/
            { DeckType.SecretPaladin, Card.CClass.PALADIN},
            { DeckType.MidRangePaladin, Card.CClass.PALADIN},
            { DeckType.DragonPaladin, Card.CClass.PALADIN},
            { DeckType.AggroPaladin, Card.CClass.PALADIN},
            { DeckType.AnyfinMurglMurgl, Card.CClass.PALADIN},
            { DeckType.RenoPaladin, Card.CClass.PALADIN}, /*Druid*/
            { DeckType.RampDruid, Card.CClass.DRUID},
            { DeckType.AggroDruid, Card.CClass.DRUID},
            { DeckType.DragonDruid, Card.CClass.DRUID},
            { DeckType.MidRangeDruid, Card.CClass.DRUID},
            { DeckType.TokenDruid, Card.CClass.DRUID},
            { DeckType.SilenceDruid, Card.CClass.DRUID},
            { DeckType.MechDruid, Card.CClass.DRUID},
            { DeckType.AstralDruid, Card.CClass.DRUID},
            { DeckType.MillDruid, Card.CClass.DRUID},
            { DeckType.BeastDruid, Card.CClass.DRUID},
            { DeckType.RenoDruid, Card.CClass.DRUID}, /*Warlock*/
            { DeckType.Handlock, Card.CClass.WARLOCK},
            { DeckType.RenoLock, Card.CClass.WARLOCK},
            { DeckType.Zoolock, Card.CClass.WARLOCK}, //Same handler as flood zoo and reliquary
            { DeckType.DemonHandlock, Card.CClass.WARLOCK},
            { DeckType.DragonHandlock, Card.CClass.WARLOCK},
            { DeckType.MalyLock, Card.CClass.WARLOCK},
            { DeckType.ControlWarlock, Card.CClass.WARLOCK}, /*Mage*/
            { DeckType.TempoMage, Card.CClass.MAGE},
            { DeckType.FreezeMage, Card.CClass.MAGE},
            { DeckType.FaceFreezeMage, Card.CClass.MAGE},
            { DeckType.DragonMage, Card.CClass.MAGE},
            { DeckType.MechMage, Card.CClass.MAGE},
            { DeckType.EchoMage, Card.CClass.MAGE},
            { DeckType.RenoMage, Card.CClass.MAGE}, /*Priest*/
            { DeckType.DragonPriest, Card.CClass.PRIEST},
            { DeckType.ControlPriest, Card.CClass.PRIEST},
            { DeckType.ComboPriest, Card.CClass.PRIEST},
            { DeckType.MechPriest, Card.CClass.PRIEST}, /*Hunter*/
            { DeckType.MidRangeHunter, Card.CClass.HUNTER},
            { DeckType.HybridHunter, Card.CClass.HUNTER},
            { DeckType.FaceHunter, Card.CClass.HUNTER},
            { DeckType.CamelHunter, Card.CClass.HUNTER},
            { DeckType.DragonHunter, Card.CClass.HUNTER},
            { DeckType.RenoHunter, Card.CClass.HUNTER}, /*Rogue*/
            { DeckType.OilRogue, Card.CClass.ROGUE},
            { DeckType.PirateRogue, Card.CClass.ROGUE},
            { DeckType.FaceRogue, Card.CClass.ROGUE},
            { DeckType.MalyRogue, Card.CClass.ROGUE},
            { DeckType.RaptorRogue, Card.CClass.ROGUE},
            { DeckType.MiracleRogue, Card.CClass.ROGUE},
            { DeckType.RenoRogue, Card.CClass.ROGUE},
            { DeckType.MechRogue, Card.CClass.ROGUE},
            { DeckType.MillRogue, Card.CClass.ROGUE}, /*Cance... I mean Shaman*/
            { DeckType.MidrangeShaman, Card.CClass.SHAMAN},
            { DeckType.FaceShaman, Card.CClass.SHAMAN},
            { DeckType.MechShaman, Card.CClass.SHAMAN},
            { DeckType.DragonShaman, Card.CClass.SHAMAN},
            { DeckType.MalygosShaman, Card.CClass.SHAMAN},
            { DeckType.ControlShaman, Card.CClass.SHAMAN},
            { DeckType.RenoShaman, Card.CClass.SHAMAN},
            { DeckType.BattleryShaman, Card.CClass.SHAMAN}, /*Poor Kids*/
        };

        #endregion
    }

    #endregion

    public class GameContainer

    {
        private const string Mode = "Mode";
        private const string TrackerMyType = "AutoFriendlyDeckType";
        private const string TrackerForceMyType = "ForcedDeckType";
        private const string TrackerMyStyle = "AutoFriendlyStyle";
        private const string TrackerEnemyType = "EnemyDeckTypeGuess";
        private const string TrackerEnemyStyle = "EnemyDeckStyleGuess";
        private const string MulliganTesterMyDeck = "MulliganTesterYourDeck";
        private const string MulliganTesterEnemyDeck = "MulliganTEsterEnemyDeck";
        private const string EnumsCount = "SynchEnums";
        private const string nGames = "AnalyzeGames";

        public DeckType MyDeckType { get; set; }
        public Style MyStyle { get; set; }
        public List<Card.Cards> MyDeck { get; set; }
        public DeckType EneDeckType { get; set; }
        public Style EnemyStyle { get; set; }


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
        public Tuple<List<Card.Cards>, List<Card.Cards>, List<Card.Cards>, List<Card.Cards>, List<Card.Cards>, List<Card.Cards>> AllDropsTuple { get; set; }


        //needed
        public GameContainer(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            
            var value = Enum.GetValues(typeof(DeckType)).Cast<DeckType>().ToList();
            foreach (var q in value)
            {
                if (q == DeckType.Count) continue;
                if (!DeckStyles.ContainsKey(q))
                {
                    Bot.Log("[URGENT!!!] MISSING " + q + " IN STYLES, IT MIGHT CRASH THE BOT WHEN ENCOUNTERED");
                }
            }
            try
            {
                Bot.GetPlugins().Find(p => p.DataContainer.Name == "Arthurs Bundle - Tracker").TryToWriteProperty("Enemy", opponentClass);
            }
            catch (Exception e)
            {
               Bot.Log("[URGENT] Please locate DebugLog.txt in Logs/ABTracker/ folder and show it to Arthur");
                Report("Writing Enemy to tracker error "+ e.Message);

            }
            Choices = choices;
            OpponentClass = opponentClass;
            OwnClass = ownClass;
            Coin = choices.Count > 3;

            #region Arthurs Bundle - Tracker

            Plugin tracker = Bot.GetPlugins().Find(plugin => plugin.DataContainer.Name == "Arthurs Bundle - Tracker");

            if (!tracker.DataContainer.Enabled)
            {
                Bot.Log("[Arthurs' Bundle: Mulligan] This mulligan relies on having Tracker active at all times in order to function. Please enable Tracker, or chose different mulligan");
                Bot.StopBot();
            }
            Dictionary<string, object> properties = tracker.GetProperties();
            //PrintDebug(properties);
            if ((int)DeckType.Count != (int)properties[EnumsCount])
            {
                Bot.Log(string.Format("[URGENT!!!!] Arthur, your enums are out of synch: {0} vs {1}", (int)DeckType.Count, (int)properties[EnumsCount]));
            }
            Report("Tracker Properties table was successfully generated");
            MyDeckType = properties[Mode].ToString() == "Manual" ? (DeckType)properties[TrackerForceMyType] //if Manual
                : (DeckType)properties[TrackerMyType]; //if Auto

            if (properties[Mode].ToString() == "Manual")
                Bot.Log("[AB - Mulligan] Dear friends, I notice that you are forcing deck recognition." + " I do not make failsafe checks on whether or not you are using a proper deck, " + "so if you decide to force Camel Hunter while playing FaceFreeze mage. It will let you. I hope you know what you are doing.");
            Bot.Log(string.Format("[You chose {0} Detection] {1} [Default: AutoDetection] {2}", properties[Mode], MyDeckType, properties[TrackerMyType]));

            #endregion
            Report("Deck Recognition via tracker was successful");
            MyStyle = (Style)properties[TrackerMyStyle];
            Report("Deck Style recognition via tracker was successful");
            EneDeckType = Bot.GetCurrentOpponentId().FindHimInHistory(opponentClass, (int)Bot.GetPlugins().Fetch("Arthurs Bundle - History", "GTA"));
            Report("Number of games fetched from History Extension was successful");
            EnemyStyle = DeckStyles[EneDeckType];
            Report("Enemy Style was successfully found");
            try
            {
                Bot.GetPlugins().Find(p => p.DataContainer.Name == "Arthurs Bundle - Tracker").TryToWriteProperty("EnemyDeckTypeGuess", EneDeckType);
                Bot.GetPlugins().Find(p => p.DataContainer.Name == "Arthurs Bundle - Tracker").TryToWriteProperty("EnemyDeckStyleGuess", EnemyStyle);
            }
            catch (Exception e)
            {
                Bot.Log("[URGENT] Please locate DebugLog.txt in Logs/ABTracker/ folder and show it to Arthur");

                Report("Something horrible happend during communication with Tracker :" + e.Message);
                
            }
            Report("Communicated enemy deck and style to tracker successfully");
            ZeroDrops = Choices.Where(card => CardTemplate.LoadFromId(card).Cost == 0).ToList();
            OneDrops = Choices.Where(card => CardTemplate.LoadFromId(card).Cost == 1).ToList();
            TwoDrops = Choices.Where(card => CardTemplate.LoadFromId(card).Cost == 2).ToList();
            ThreeDrops = Choices.Where(card => CardTemplate.LoadFromId(card).Cost == 3).ToList();
            FourDrops = Choices.Where(card => CardTemplate.LoadFromId(card).Cost == 4).ToList();
            FivePlusDrops = Choices.Where(card => CardTemplate.LoadFromId(card).Cost > 4).ToList();
            HasTurnOne = false;
            HasTurnTwo = false;
            HasTurnThree = false;
            Report("==========================");
            Report("Parties of Interest " + MyDeck + MyStyle + EneDeckType + EnemyStyle);
            Report("==========================");
        }
        public static void Report(string msg)
        {
            using (StreamWriter log = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\DebugLog.txt", true))
            {
                log.WriteLine("[{0}] {1}", DateTime.Now, msg);
            }
        }
       

        /// <summary>
        /// Mulligan Tester
        /// </summary>
        public string MtDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\MulliganProfiles\\AB - Mulligan\\mt.txt";
        public string GetMTValues()
        {
            return File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\MulliganProfiles\\AB - Mulligan\\mt.txt");
        }
        public GameContainer(bool t, List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            Report("Entered Mulligan Tester container as " + GetMTValues());
            Choices = choices;
            OpponentClass = opponentClass;
            OwnClass = ownClass;
            Coin = choices.Count > 3;
            Report("Created Choices, OC, OC, Coin");
            #region Arthurs Bundle - Tracker
            using (StreamReader mt = new StreamReader(MtDirectory))
            {
                string[] info = mt.ReadLine().Split(':');
                if (Bot.CurrentMode().IsShitfest())
                {
                    MyDeckType = DeckType.Arena;
                    MyStyle = Style.Tempo;
                    EneDeckType = DeckType.Arena;
                    EnemyStyle = Style.Tempo;
                }
                else
                {
                    MyDeckType = (DeckType)Enum.Parse(typeof(DeckType), info[0]);
                    MyStyle = (Style)Enum.Parse(typeof(Style), info[1]);
                    EneDeckType = (DeckType)Enum.Parse(typeof(DeckType), info[2]);
                    EnemyStyle = (Style)Enum.Parse(typeof(Style), info[3]);
                }
            }
            Report("Mulligan Tester decks chosen");
            #endregion
            foreach (var q in choices)
                Report("" + q);
            Report("Split of choices successful");
            ZeroDrops = Choices.Where(card => CardTemplate.LoadFromId(card).Cost == 0).ToList();
            OneDrops = Choices.Where(card => CardTemplate.LoadFromId(card).Cost == 1).ToList();
            TwoDrops = Choices.Where(card => CardTemplate.LoadFromId(card).Cost == 2).ToList();
            ThreeDrops = Choices.Where(card => CardTemplate.LoadFromId(card).Cost == 3).ToList();
            FourDrops = Choices.Where(card => CardTemplate.LoadFromId(card).Cost == 4).ToList();
            FivePlusDrops = Choices.Where(card => CardTemplate.LoadFromId(card).Cost > 4).ToList();
            HasTurnOne = false;
            HasTurnTwo = false;
            HasTurnThree = false;
            Report("Finished setup");
        }

        public override string ToString()
        {
            return string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}", Choices.Aggregate("", (current, q) => current + ("Cards." + CardTemplate.LoadFromId(q).Name.Replace(" ", "") + ", ")), OpponentClass, OwnClass, Coin, MyDeckType, MyStyle, EneDeckType, EnemyStyle, ZeroDrops.Aggregate("", (current, q) => current + ("Cards." + CardTemplate.LoadFromId(q).Name.Replace(" ", "") + ", ")), OneDrops.Aggregate("", (current, q) => current + ("Cards." + CardTemplate.LoadFromId(q).Name.Replace(" ", "") + ", ")), TwoDrops.Aggregate("", (current, q) => current + ("Cards." + CardTemplate.LoadFromId(q).Name.Replace(" ", "") + ", ")));
        }

        public readonly Dictionary<DeckType, Style> DeckStyles = new Dictionary<DeckType, Style>
        {
            {DeckType.Custom, Style.Unknown},
            {DeckType.Unknown, Style.Unknown},
            {DeckType.Arena, Style.Control},
            /*Warrior*/
            {DeckType.ControlWarrior, Style.Control},
            {DeckType.FatigueWarrior, Style.Fatigue},
            {DeckType.DragonWarrior, Style.Control},
            {DeckType.PatronWarrior, Style.Tempo},
            {DeckType.WorgenOTKWarrior, Style.Combo},
            {DeckType.MechWarrior, Style.Aggro},
            {DeckType.FaceWarrior, Style.Face},
            {DeckType.RenoWarrior, Style.Control },
            {DeckType.CThunWarrior, Style.Control },
            {DeckType.TempoWarrior, Style.Tempo },

            /*Paladin*/
            {DeckType.SecretPaladin, Style.Tempo},
            {DeckType.MidRangePaladin, Style.Control},
            {DeckType.DragonPaladin, Style.Control},
            {DeckType.AggroPaladin, Style.Aggro},
            {DeckType.AnyfinMurglMurgl, Style.Combo},
            {DeckType.RenoPaladin, Style.Control},
            {DeckType.CThunPaladin, Style.Combo },
            /*Druid*/
            {DeckType.RampDruid, Style.Control},
            {DeckType.AggroDruid, Style.Aggro},
            {DeckType.DragonDruid, Style.Control},
            {DeckType.MidRangeDruid, Style.Tempo},
            {DeckType.TokenDruid, Style.Tempo},
            {DeckType.SilenceDruid, Style.Control},
            {DeckType.MechDruid, Style.Aggro},
            {DeckType.AstralDruid, Style.Control},
            {DeckType.MillDruid, Style.Fatigue},
            {DeckType.BeastDruid, Style.Tempo},
            {DeckType.RenoDruid, Style.Control},
            {DeckType.CThunDruid, Style.Combo },
            /*Warlock*/
            {DeckType.Handlock, Style.Control},
            {DeckType.RenoLock, Style.Control},
            {DeckType.Zoolock, Style.Aggro}, //Same handler as flood zoo and reliquary
            {DeckType.DemonHandlock, Style.Control},
            {DeckType.DragonHandlock, Style.Control},
            {DeckType.MalyLock, Style.Combo},
            {DeckType.ControlWarlock, Style.Control},
            {DeckType.CThunLock, Style.Combo },
            /*Mage*/
            {DeckType.TempoMage, Style.Tempo},
            {DeckType.FreezeMage, Style.Control},
            {DeckType.FaceFreezeMage, Style.Aggro},
            {DeckType.DragonMage, Style.Control},
            {DeckType.MechMage, Style.Aggro},
            {DeckType.EchoMage, Style.Control},
            {DeckType.CThunMage, Style.Combo},
            {DeckType.RenoMage, Style.Control},
            /*Priest*/
            {DeckType.DragonPriest, Style.Tempo},
            {DeckType.ControlPriest, Style.Control},
            {DeckType.ComboPriest, Style.Combo},
            {DeckType.MechPriest, Style.Aggro},
            /*Hunter*/
            {DeckType.MidRangeHunter, Style.Tempo},
            {DeckType.HybridHunter, Style.Aggro},
            {DeckType.FaceHunter, Style.Face},
            {DeckType.CamelHunter, Style.Control},
            {DeckType.DragonHunter, Style.Control},
            {DeckType.RenoHunter, Style.Control},
            {DeckType.CThunHunter, Style.Combo },
            /*Rogue*/
            {DeckType.OilRogue, Style.Combo},
            {DeckType.PirateRogue, Style.Aggro},
            {DeckType.FaceRogue, Style.Face},
            {DeckType.MalyRogue, Style.Combo},
            {DeckType.RaptorRogue, Style.Tempo},
            {DeckType.MiracleRogue, Style.Combo},
            {DeckType.RenoRogue, Style.Control},
            {DeckType.MechRogue, Style.Tempo},
            {DeckType.MillRogue, Style.Fatigue},
            {DeckType.CThunRogue, Style.Combo },
            /*Cance... I mean Shaman*/
            {DeckType.FaceShaman, Style.Face},
            {DeckType.MidrangeShaman, Style.Tempo},
            {DeckType.MechShaman, Style.Aggro},
            {DeckType.DragonShaman, Style.Control},
            {DeckType.MalygosShaman, Style.Combo},
            {DeckType.ControlShaman, Style.Control},
            {DeckType.RenoShaman, Style.Combo},
            {DeckType.BattleryShaman, Style.Control},
            {DeckType.CThunShaman, Style.Combo },

            /*Poor Kids*/
            {DeckType.Basic, Style.Tempo}
        };

        public GameContainer()
        {
        }
    }

    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class Mulligan : MulliganProfile
    {
        public Card.Cards Nothing = Card.Cards.GAME_005;
        public Card.CClass Mage = Card.CClass.MAGE;
        public Card.CClass Priest = Card.CClass.PRIEST;
        public Card.CClass Shaman = Card.CClass.SHAMAN;
        public Card.CClass Warlock = Card.CClass.WARLOCK;
        public Card.CClass Warrior = Card.CClass.WARRIOR;
        public Card.CClass Paladin = Card.CClass.PALADIN;
        public Card.CClass Rogue = Card.CClass.ROGUE;
        public Card.CClass Hunter = Card.CClass.HUNTER;
        public Card.CClass Druid = Card.CClass.DRUID;

        #region variables

        public static List<string> CurrentDeck = new List<string>();
        private readonly Dictionary<Card.Cards, bool> _whiteList; // CardName, KeepDouble
        private readonly List<Card.Cards> _cardsToKeep;

        #endregion

        public Mulligan()
        {
            _whiteList = new Dictionary<Card.Cards, bool>
            {
                {Card.Cards.GAME_005, true}, //coin
                {Cards.Innervate, true}, //always keeps 2 innervates (set false for 1)
                {Cards.WildGrowth, false}, //only keeps 1 wild growth
                {Cards.FieryWarAxe, false }
            };
            _cardsToKeep = new List<Card.Cards>();
        }


        public List<Card.Cards> HandleMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            ClearReport();
            Report("Mulligan Stage Entered");

            GameContainer mtgc = new GameContainer(true, choices, opponentClass, ownClass);
            //Report("Mulligan Tester Container created");
            try
            {
                GameContainer gc = new GameContainer(choices, opponentClass, ownClass);
                Report("Normal gc created");
                Mulliganaccordingly(gc);
                Report("Finished Mulliganing accordingly");
                mtgc = gc;
                
            }
            catch (NotImplementedException e)
            {
                Report(string.Format("[Arthurs' Bundle: Mulligan] Current deck is not implemented: {0}", e.TargetSite));
                Core(mtgc);
            }
            catch (Exception q)
            {
                Bot.Log("[Possibly Harmless] Please locate DebugLog.txt in Logs/ABTracker/ folder and show it to Arthur to varify few things");

                Report("Deck Implementation " +q.Message + " " + q.TargetSite);
                Report("[Critical Event] Using Mulligan Tester values to preserve the bot flow");
                Mulliganaccordingly(mtgc);
            }

            foreach (var s in from s in choices let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString()) where _whiteList.ContainsKey(s) where !keptOneAlready | _whiteList[s] select s)
                _cardsToKeep.Add(s);
            MulliganLog(choices, _cardsToKeep, mtgc);
            return _cardsToKeep;
        }

        private void MulliganLog(List<Card.Cards> choices, List<Card.Cards> cardsToKeep, GameContainer gc)
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\");
            using (StreamWriter ml = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\MulliganHistory.txt", true))
            {
                Bot.Log("=========================================");
                Bot.Log(string.Format("[{0}||{1} vs {2}:{3}]", Bot.CurrentMode(), gc.MyDeckType, gc.OpponentClass, gc.EneDeckType));
                Bot.Log(string.Format("Given: " + choices.Aggregate("", (current, q) => current + (" " + CardTemplate.LoadFromId(q).Name.Replace(" ", "") + ", "))));
                Bot.Log(string.Format("Kept: " + cardsToKeep.Aggregate("", (current, q) => current + (" " + CardTemplate.LoadFromId(q).Name.Replace(" ", "") + ", "))));
                ml.WriteLine("=========================================");
                ml.WriteLine("[{0}||{1} vs {2}:{3}]", Bot.CurrentMode(), gc.MyDeckType, gc.OpponentClass, gc.EneDeckType);
                ml.WriteLine("Given: " + choices.Aggregate("", (current, q) => current + (" " + CardTemplate.LoadFromId(q).Name.Replace(" ", "") + ", ")));
                ml.WriteLine("Kept: " + cardsToKeep.Aggregate("", (current, q) => current + (" " + CardTemplate.LoadFromId(q).Name.Replace(" ", "") + ", ")));
            }
        }

        public void Mulliganaccordingly(GameContainer gc)
        {

            switch (gc.MyDeckType)
            {
                case DeckType.Custom:
                HandleCustomDeck(gc);
                break;
                case DeckType.Unknown:
                Core(gc);
                break;
                case DeckType.Arena:
                Core(gc);
                break;
                case DeckType.ControlWarrior:
                HandleControlWarrior(gc);
                break;
                case DeckType.FatigueWarrior:
                HandleFatigueWarrior(gc);
                break;
                case DeckType.DragonWarrior:
                HandleDragonWarrior(gc);
                break;
                case DeckType.PatronWarrior:
                HandlePatronWarrior(gc);
                break;
                case DeckType.WorgenOTKWarrior:
                HandleWorgenOTKWarrior(gc);
                break;
                case DeckType.MechWarrior:
                HandleMechWarrior(gc);
                break;
                case DeckType.FaceWarrior:
                HandleFaceWarrior(gc);
                break;
                case DeckType.RenoWarrior:
                HandleRenoWarrior(gc);
                break;
                case DeckType.SecretPaladin:
                HandleSecretPaladin(gc);
                break;
                case DeckType.MidRangePaladin:
                HandleMidRangePaladin(gc);
                break;
                case DeckType.DragonPaladin:
                HandleDragonPaladin(gc);
                break;
                case DeckType.AggroPaladin:
                HandleAggroPaladin(gc);
                break;
                case DeckType.AnyfinMurglMurgl:
                HandleAnyfinMurglMurgl(gc);
                break;
                case DeckType.RenoPaladin:
                HandleRenoPaladin(gc);
                break;
                case DeckType.RampDruid:
                HandleRampDruid(gc);
                break;
                case DeckType.AggroDruid:
                HandleAggroDruid(gc);
                break;
                case DeckType.DragonDruid:
                HandleDragonDruid(gc);
                break;
                case DeckType.MidRangeDruid:
                HandleMidRangeDruid(gc);
                break;
                case DeckType.TokenDruid:
                HandleTokenDruid(gc);
                break;
                case DeckType.SilenceDruid:
                HandleSilenceDruid(gc);
                break;
                case DeckType.MechDruid:
                HandleMechDruid(gc);
                break;
                case DeckType.AstralDruid:
                HandleAstralDruid(gc);
                break;
                case DeckType.MillDruid:
                HandleMillDruid(gc);
                break;
                case DeckType.BeastDruid:
                HandleBeastDruid(gc);
                break;
                case DeckType.RenoDruid:
                HandleRenoDruid(gc);
                break;
                case DeckType.Handlock:
                HandleHandlock(gc);
                break;
                case DeckType.RenoLock:
                HandleRenoLock(gc);
                break;
                case DeckType.Zoolock:
                HandleZoolock(gc);
                break;
                case DeckType.DemonHandlock:
                HandleDemonHandlock(gc);
                break;
                case DeckType.DragonHandlock:
                HandleDragonHandlock(gc);
                break;
                case DeckType.MalyLock:
                HandleMalyLock(gc);
                break;
                case DeckType.ControlWarlock:
                HandleControlWarlock(gc);
                break;
                case DeckType.TempoMage:
                HandleTempoMage(gc);
                break;
                case DeckType.FreezeMage:
                HandleFreezeMage(gc);
                break;
                case DeckType.FaceFreezeMage:
                HandleFaceFreezeMage(gc);
                break;
                case DeckType.DragonMage:
                HandleDragonMage(gc);
                break;
                case DeckType.MechMage:
                HandleMechMage(gc);
                break;
                case DeckType.EchoMage:
                HandleEchoMage(gc);
                break;
                case DeckType.RenoMage:
                HandleRenoMage(gc);
                break;
                case DeckType.DragonPriest:
                HandleDragonPriest(gc);
                break;
                case DeckType.ControlPriest:
                HandleControlPriest(gc);
                break;
                case DeckType.ComboPriest:
                HandleComboPriest(gc);
                break;
                case DeckType.MechPriest:
                HandleMechPriest(gc);
                break;
                case DeckType.MidRangeHunter:
                HandleMidrangeHunter(gc);
                break;
                case DeckType.HybridHunter:
                HandleHybridHunter(gc);
                break;
                case DeckType.FaceHunter:
                HandleFaceHunter(gc);
                break;
                case DeckType.CamelHunter:
                HandleCamelHunter(gc);
                break;
                case DeckType.RenoHunter:
                HandleRenoHunter(gc);
                break;
                case DeckType.DragonHunter:
                HandleDragonHunter(gc);
                break;
                case DeckType.OilRogue:
                HandleOilRogue(gc);
                break;
                case DeckType.PirateRogue:
                HandlePirateRogue(gc);
                break;
                case DeckType.FaceRogue:
                HandleFaceRogue(gc);
                break;
                case DeckType.MalyRogue:
                HandleMalyRogue(gc);
                break;
                case DeckType.RaptorRogue:
                HandleRaptorRogue(gc);
                break;
                case DeckType.MiracleRogue:
                HandleMiracleRogue(gc);
                break;
                case DeckType.MechRogue:
                HandleMechRogue(gc);
                break;
                case DeckType.RenoRogue:
                HandleRenoRogue(gc);
                break;
                case DeckType.MillRogue:
                HandleMillRogue(gc);
                break;
                case DeckType.FaceShaman:
                HandleFaceShaman(gc);
                break;
                case DeckType.MechShaman:
                HandleMechShaman(gc);
                break;
                case DeckType.DragonShaman:
                HandleDragonShaman(gc);
                break;
                case DeckType.MalygosShaman:
                HandleMalyShaman(gc);
                break;
                case DeckType.ControlShaman:
                HandleControlShaman(gc);
                break;
                case DeckType.MidrangeShaman:
                HandleTotemShaman(gc);
                break;
                case DeckType.BattleryShaman:
                HandleBattlecryShaman(gc);
                break;
                case DeckType.RenoShaman:
                HandleRenoShaman(gc);
                break;
                case DeckType.Basic:
                Core(gc);
                break;
                case DeckType.CThunWarrior:
                HandleCThunDecks(gc);
                break;
                case DeckType.CThunPaladin:
                HandleCThunDecks(gc);
                break;
                case DeckType.CThunDruid:
                HandleCThunDecks(gc);
                break;
                case DeckType.CThunLock:
                HandleCThunDecks(gc);
                break;
                case DeckType.CThunMage:
                HandleCThunDecks(gc);
                break;
                case DeckType.CThunHunter:
                HandleCThunDecks(gc);
                break;
                case DeckType.CThunRogue:
                HandleCThunDecks(gc);
                break;
                case DeckType.CThunShaman:
                HandleCThunDecks(gc);
                break;
                case DeckType.TempoWarrior:
                HandleTempoWarrior(gc);
                break;
                default:
                throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleTempoWarrior(GameContainer gc)
        {
            Core(gc);

            if (gc.EneDeckType.IsOneOf(DeckType.BeastDruid, DeckType.MidRangeDruid, DeckType.RampDruid))
            {
                _whiteList.AddAll(false, Cards.BloodToIchor, Cards.FierceMonkey, Cards.RavagingGhoul, Cards.FrothingBerserker,
                    gc.Coin || gc.HasTurnTwo ? Cards.KorkronElite : Nothing);
            }
            if (gc.OpponentClass.Is(Hunter))
            {
                _whiteList.AddInOrder(1, gc.Choices, false, Cards.FierceMonkey, Cards.FrothingBerserker);
                _whiteList.AddAll(false, Cards.Armorsmith, Cards.RavagingGhoul);
                TurnChecker(gc);
            }
            if (gc.OpponentClass.Is(Mage))
            {
                _whiteList.AddAll(false, Cards.BloodToIchor, Cards.Armorsmith, Cards.FierceMonkey, Cards.RavagingGhoul, Cards.FrothingBerserker);
                TurnChecker(gc);
                if (gc.EneDeckType.Is(DeckType.FreezeMage))
                {
                    _whiteList.AddAll(false, Cards.BattleRage, Cards.BloodhoofBrave);
                }
            }
            if (gc.OpponentClass.Is(Rogue))
            {
                _whiteList.AddAll(false, Cards.BloodToIchor, Cards.FierceMonkey, Cards.FrothingBerserker, Cards.RavagingGhoul);
                TurnChecker(gc);
                if (gc.HasTurnThree) _whiteList.AddOrUpdate(Cards.BloodhoofBrave, false);
            }
            if (gc.OpponentClass.Is(Paladin))
            {
                _whiteList.AddAll(false, Cards.BloodToIchor, Cards.FierceMonkey, Cards.FrothingBerserker, Cards.RavagingGhoul);
                TurnChecker(gc);
            }
            if (gc.OpponentClass.Is(Priest))
            {
                _whiteList.AddAll(false, Cards.BloodToIchor, Cards.BattleRage, Cards.FierceMonkey, Cards.FrothingBerserker, Cards.RavagingGhoul);
                TurnChecker(gc);
            }
            if (gc.OpponentClass.Is(Shaman))
            {
                _whiteList.AddAll(false, Cards.BloodToIchor, Cards.BattleRage, Cards.FierceMonkey, Cards.FrothingBerserker, Cards.RavagingGhoul);
                if (gc.EnemyStyle.Aggresive()) _whiteList.AddOrUpdate(Cards.Whirlwind, false);
                TurnChecker(gc);
            }
            if (gc.OpponentClass.Is(Warlock))
            {
                if (gc.EnemyStyle.Aggresive())
                {
                    _whiteList.AddInOrder(1, gc.Choices, false, Cards.RavagingGhoul, Cards.Whirlwind);
                    _whiteList.AddOrUpdate(Cards.Armorsmith, false);
                }
                _whiteList.AddAll(false, Cards.BloodToIchor, Cards.FierceMonkey, Cards.RavagingGhoul);
            }
            if (gc.OpponentClass.Is(Warrior))
            {
                _whiteList.AddAll(false, Cards.BloodToIchor, Cards.Armorsmith, Cards.FierceMonkey, Cards.RavagingGhoul, Cards.FrothingBerserker, Cards.KorkronElite);
                TurnChecker(gc);
            }

        }

        private void TurnChecker(GameContainer gc)
        {
            foreach (var q in gc.Choices.Intersect(_whiteList.Keys))
            {
                switch (q.Cost())
                {
                    case 1:
                    gc.HasTurnOne = true;
                    break;
                    case 2:
                    gc.HasTurnTwo = true;
                    break;
                    case 3:
                    gc.HasTurnThree = true;
                    break;
                }
            }
        }
        /// <summary>
        ///DO NOT REMOVE CUSTOM REGION BREAKS 
        ///ALL CHANGES MUST BE BETWEEN CUSTOM REGIONS
        /// </summary>
        /// <param name="gc"></param>

        private void HandleCustomDeck(GameContainer gc)
        {
            #region Custom

            #endregion Custom
        }

        private void HandleNZothPaladin(GameContainer gc)
        {
            Core(gc);
            _whiteList.AddAll(false, Cards.Doomsayer);
            if (gc.EneDeckType.IsCthusWhatever()) _whiteList.AddAll(false, Cards.TruesilverChampion, Cards.AcolyteofPain, Cards.TwilightSummoner);
            if (gc.EnemyStyle.Aggresive())
                _whiteList.AddInOrder(3, gc.Choices, false, Cards.WildPyromancer, Cards.Equality, Cards.AcolyteofPain, Cards.HarvestGolem, Cards.ArgentLance);

            if (gc.OpponentClass.WeaponClass()) _whiteList.AddOrUpdate(Cards.AcidicSwampOoze, false);
            if (gc.EneDeckType.IsOneOf(TempoMage, Zoolock, RenoLock)) _whiteList.AddOrUpdate(Cards.TruesilverChampion, false);
            if (gc.EnemyStyle.Aggresive()) _whiteList.AddOrUpdate(Cards.Consecration, false);
            if (gc.OpponentClass.IsOneOf(Shaman, Druid)) _whiteList.AddOrUpdate(Cards.AldorPeacekeeper, false);
        }

        private void HandleCThunDecks(GameContainer gc)
        {
            Report("Entered CThun VIP club");
            HandleSpellsAndWeapons(gc);
            Report("successfully parsed weapons and spells");
            foreach (var q in gc.OneDrops.Where(c => c.IsMinion()))
            {
                gc.HasTurnOne = true;
                _whiteList.AddOrUpdate(q, false);
            }
            foreach (var q in gc.TwoDrops.Where(c => c.IsMinion()))
            {
                gc.HasTurnTwo = true;
                _whiteList.AddOrUpdate(q, q.Priority() > 3);
            }
            foreach (var q in gc.ThreeDrops.Where(c => c.IsMinion()))
            {
                gc.HasTurnThree = true;
                _whiteList.AddOrUpdate(q, false);
            }
            _whiteList.AddOrUpdate(gc.HasTurnTwo ? Cards.DiscipleofCThun : Nothing, false);
            _whiteList.AddOrUpdate(gc.Choices.HasRamp() ? Cards.MireKeeper : Nothing, false);
            _whiteList.AddOrUpdate(gc.HasTurnTwo && gc.HasTurnThree ? Cards.CThun : Nothing, false);
            if (gc.Choices.Contains(Cards.CThun) && gc.HasTurnOne && gc.HasTurnThree)
            {
                Bot.Log("[ABTracker] You have both, 2 and 3 drops, so we are keeping C'Thun");
            }
        }

        private void HandleFatigueWarrior(GameContainer gc)
        {
            HandleControlWarrior(gc);
            if (gc.EnemyStyle.Aggresive()) _whiteList.AddOrUpdate(Cards.Deathlord, false);

        }

        private void HandleDragonWarrior(GameContainer gc)
        {
            try
            {
                HandleDragonLogicCore(gc);
            }
            catch (Exception e)
            {
                                Bot.Log("[URGENT] Please locate DebugLog.txt in Logs/ABTracker/ folder and show it to Arthur");

                Core(gc);
            }
        }

        private void HandlePatronWarrior(GameContainer gc)
        {
            var aggro = gc.EnemyStyle.Aggresive();
            bool hasWeapon = gc.Choices.Any(cards => cards.IsWeapon());

            _whiteList.AddOrUpdate(hasWeapon ? Cards.DreadCorsair : Nothing, false);
            if (aggro)
            {
                _whiteList.AddAll(false, Cards.FieryWarAxe, Cards.UnstableGhoul, Cards.Whirlwind, Cards.FrothingBerserker);
                _whiteList.AddOrUpdate(!hasWeapon ? Cards.Slam : Nothing, false);
                _whiteList.AddOrUpdate(hasWeapon ? Cards.DreadCorsair : Nothing, false);
                _whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.DreadCorsair) ? Cards.DeathsBite : Nothing, false);
            }
            else
            {
                _whiteList.AddAll(false, Cards.FieryWarAxe, Cards.DeathsBite, Cards.GnomishInventor, Cards.Slam);
                if (gc.OpponentClass.IsOneOf(Priest, Warrior))
                    _whiteList.AddOrUpdate(Cards.EmperorThaurissan, false);
            }
        }

        private void HandleWorgenOTKWarrior(GameContainer gc)
        {
            Core(gc);
        }

        private void HandleMechWarrior(GameContainer gc)
        {
            HandleMechsCoreLogic(gc);
            if (!gc.EnemyStyle.Aggresive() && gc.HasTurnTwo && gc.Coin)
            {
                _whiteList.AddOrUpdate(Cards.PilotedShredder, false);
            }
        }

        private void HandleDragonLogicCore(GameContainer gc)
        {
            foreach (var q in gc.Choices)
            {
                if (q.IsSpell()) continue;
                if (q.Cost() == 1)
                {
                    gc.HasTurnOne = true;
                    _whiteList.AddOrUpdate(q, false);
                }
                if (q.Cost() == 2)
                {
                    gc.HasTurnTwo = true;
                    _whiteList.AddOrUpdate(q, gc.Coin && !q.IsWeapon());
                }
                if (!gc.HasTurnTwo) continue;
                _whiteList.AddOrUpdate(Cards.BlackwingTechnician, false);
            }
        }

        private void HandleMechsCoreLogic(GameContainer gc)
        {
            foreach (var c in gc.Choices)
            {
                if (c.IsWeapon() && c.Cost() < 3)
                {
                    gc.HasTurnTwo = true;
                    _whiteList.AddOrUpdate(c, false);
                }
                if (!c.IsMinion()) continue;
                switch (c.Cost())
                {
                    case 1:
                    {
                        _whiteList.AddOrUpdate(c, false);
                        gc.HasTurnOne = true;
                        break;
                    }
                    case 2:
                    {
                        _whiteList.AddOrUpdate(c, false);
                        gc.HasTurnTwo = true;
                        break;
                    }
                    case 3:
                    {
                        if (gc.EnemyStyle.Aggresive())
                            break;
                        _whiteList.AddOrUpdate(c, false);
                        gc.HasTurnOne = true;
                        break;
                    }
                }
            }
        }

        private void HandleFaceWarrior(GameContainer gc)
        {
            Core(gc);
            _whiteList.AddOrUpdate(Cards.FieryWarAxe, true);
            foreach (var q in gc.Choices.Where(card => card.Cost() == 1 && card.IsMinion()))
            {
                _whiteList.AddOrUpdate(q, q.Priority() > 3 && gc.Coin);
            }
        }

        private void HandleRenoWarrior(GameContainer gc)
        {
            Core(gc);
            if (gc.EnemyStyle.Aggresive()) _whiteList.AddInOrder(3, gc.Choices, false, Cards.FierceMonkey, Cards.SenjinShieldmasta, Cards.Revenge, Cards.Slam, Cards.SludgeBelcher);
            if (!gc.EnemyStyle.Aggresive()) _whiteList.AddInOrder(3, gc.Choices, false, Cards.AcolyteofPain, Cards.FieryWarAxe, Cards.DeathsBite, Cards.PilotedShredder, Cards.Slam);
            if (gc.OpponentClass.IsOneOf(Paladin, Warlock) && Bot.CurrentMode().IsStandard()) _whiteList.AddOrUpdate(Cards.Revenge, false);
        }

        private void HandleTauntWarrior(GameContainer gc)
        {
            Core(gc);
        }

        private void HandleMidRangePaladin(GameContainer gc)
        {
            HandleNZothPaladin(gc);
        }

        private void HandleDragonPaladin(GameContainer gc)
        {
            HandleDragonLogicCore(gc);
        }

        private void HandleAggroPaladin(GameContainer gc)
        {
            Core(gc);
            if (gc.OpponentClass.Is(Druid)) _whiteList.AddOrUpdate(Cards.KeeperofUldaman, false);
            if (gc.OpponentClass.Is(Shaman)) _whiteList.AddOrUpdate(Cards.AbusiveSergeant, false);
            if (gc.OpponentClass.Is(Mage) && !gc.EnemyStyle.Aggresive()) _whiteList.AddOrUpdate(Cards.DivineFavor, false);
            if (gc.OpponentClass.Is(Warrior)) _whiteList.AddAll(gc.Coin, Cards.ArgentSquire, Cards.BilefinTidehunter);


        }

        private void HandleAnyfinMurglMurgl(GameContainer gc)
        {
            HandleSpellsAndWeapons(gc);
            foreach (var card in gc.Choices.Where(choice => CardTemplate.LoadFromId(choice).Cost <= 4 && CardTemplate.LoadFromId(choice).Type == Card.CType.MINION && CardTemplate.LoadFromId(choice).Race != Card.CRace.MURLOC))
            {
                switch (card.Cost())
                {
                    case 1:
                    gc.HasTurnOne = true;
                    _whiteList.AddOrUpdate(card, gc.Coin || card.Priority() >= 4);
                    break;
                    case 2:
                    if (!gc.Choices.HasTurn(1, 2) && !gc.Coin) continue;
                    if (gc.Coin) _whiteList.AddOrUpdate(card, false);
                    gc.HasTurnTwo = true;
                    _whiteList.AddOrUpdate(card, card.Priority(card == Cards.WildPyromancer ? -2 : 0) > 4 && gc.Coin && CardTemplate.LoadFromId(card).Overload == 0);
                    break;
                    case 3:
                    if (!gc.Choices.HasTurn(1, 2) || !gc.Choices.HasTurn(2, 2)) continue;
                    gc.HasTurnThree = true;
                    _whiteList.AddOrUpdate(card, false);
                    break;
                }
                //_whiteList.AddOrUpdate(card.ToString(), _hasCoin && GetPriority(card.ToString()) > 4);
            }
        }

        private void HandleRenoPaladin(GameContainer gc)
        {
            Core(gc);

        }

        private void HandleRampDruid(GameContainer gc)
        {
            Core(gc);
            bool gotone = false;
            foreach (var q in gc.Choices.Where(c => c.IsMinion() && c.Cost().Range(4, 5)))
            {
                if (!gc.Choices.HasRamp() || gotone) break;
                _whiteList.AddOrUpdate(q, false);
                gotone = true;

            }
        }

        private void HandleAggroDruid(GameContainer gc)
        {
            foreach (var c in gc.Choices)
            {
                if (!c.IsMinion()) continue;
                switch (c.Cost())
                {
                    case 1:
                    gc.HasTurnOne = true;
                    _whiteList.AddOrUpdate(c, false);
                    break;
                    case 2:
                    gc.HasTurnTwo = true;
                    _whiteList.AddOrUpdate(c, false);
                    break;
                    case 3:
                    gc.HasTurnThree = true;
                    _whiteList.AddOrUpdate(c, false);
                    break;
                }
            }
            /*Always whitelists at least 1 innervate*/
            _whiteList.AddAll(false, Cards.WildGrowth, Cards.FjolaLightbane, Cards.EydisDarkbane, Cards.FlameJuggler, Cards.DruidoftheSaber);
        }

        private void HandleDragonDruid(GameContainer gc)
        {
            Core(gc);
            HandleDragonLogicCore(gc);
        }

        private void HandleMidRangeDruid(GameContainer gc)
        {
            bool Innervate = gc.Choices.HasAny(Cards.Innervate);
            bool dInnervate = gc.Choices.Count(c => c == Cards.Innervate) == 2;
            bool wildGrowth = gc.Choices.HasAny(Cards.WildGrowth);
            Core(gc);
            if (Innervate) _whiteList.AddAll(false, Cards.DruidoftheFlame, Cards.SavageCombatant);
            if (gc.EnemyStyle.Aggresive()) _whiteList.AddAll(false, Cards.Wrath, Cards.LivingRoots);
        }

        private void HandleTokenDruid(GameContainer gc)
        {
            Core(gc);
        }

        private void HandleSilenceDruid(GameContainer gc)
        {
            HandleMidRangeDruid(gc);
            _whiteList.AddOrUpdate(Cards.AncientWatcher, false);
        }

        private void HandleMechDruid(GameContainer gc)
        {
            HandleMechsCoreLogic(gc);
            if (!gc.EnemyStyle.Aggresive() && gc.HasTurnTwo && gc.Coin)
            {
                _whiteList.AddOrUpdate(Cards.PilotedShredder, false);
            }
        }

        private void HandleAstralDruid(GameContainer gc)
        {
            Core(gc);
            _whiteList.AddAll(true, Cards.Innervate, Cards.WildGrowth, Cards.LootHoarder, Cards.AstralCommunion);
        }

        private void HandleMillDruid(GameContainer gc)
        {
            Core(gc);
        }

        private void HandleBeastDruid(GameContainer gc)
        {
            HandleMidRangeDruid(gc);
        }

        private void HandleRenoDruid(GameContainer gc)
        {
            HandleMidRangeDruid(gc);
        }

        private void HandleHandlock(GameContainer gc)
        {
            HandleRenoLock(gc);

        }

        private void HandleRenoLock(GameContainer gc)
        {
            var vc = gc.Choices.HasAny(Cards.Voidcaller);
            Core(gc);
            _whiteList.Remove(Cards.AncientWatcher);
            _whiteList.AddAll(false, Cards.MountainGiant, Cards.TwilightDrake, Cards.DarkPeddler, gc.Coin ? Cards.ImpGangBoss : Nothing);
            if (gc.EnemyStyle.Aggresive())
                _whiteList.AddAll(false, Cards.Hellfire, Cards.Shadowflame, Cards.EarthenRingFarseer, Cards.MoltenGiant, Cards.MindControlTech, gc.Coin ? Cards.RenoJackson : Nothing, gc.Coin || gc.Choices.HasAny(Cards.AncientWatcher) ? Cards.DefenderofArgus : Nothing, gc.Choices.HasAny(Cards.SunfuryProtector, Cards.DefenderofArgus) ? Cards.AncientWatcher : Nothing);
            else
            {
                _whiteList.AddAll(false, Cards.EmperorThaurissan, Cards.PilotedShredder, gc.OpponentClass.Is(Shaman) ? Cards.Hellfire : Nothing);
            }
            HandleSpellsAndWeapons(gc);
            _whiteList.AddOrUpdate(Cards.Voidcaller, false);
            _whiteList.Remove(!gc.EnemyStyle.Aggresive() ? Cards.SunfuryProtector : Nothing);
            _whiteList.AddOrUpdate(gc.OpponentClass.IsOneOf(Mage, Hunter) ? Cards.KezanMystic : Nothing, false);
        }

        private void HandleRenoComboLock(GameContainer gc)
        {
            HandleRenoLock(gc);
        }

        private void HandleZoolock(GameContainer gc)
        {
            //List<Card.Cards> activators = new List<Card.Cards> {Cards.PowerOverwhelming, Cards.VoidTerror, Cards.AbusiveSergeant, Cards.DefenderofArgus};
            Report("Entered Zoo mulligan");
            List<Card.Cards> needActivation = new List<Card.Cards> { Cards.NerubianEgg };
            _whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.NerubianEgg) ? Cards.PowerOverwhelming : Nothing, false);
            Core(gc);
            _whiteList.AddOrUpdate(gc.HasTurnTwo && gc.Coin && gc.Choices.Intersect(needActivation).Any() ? Cards.DefenderofArgus : Nothing, false);
        }

        private void HandleDemonHandlock(GameContainer gc)
        {
            HandleHandlock(gc);
            bool voidcaller = gc.Choices.HasAny(Cards.Voidcaller);
            if (voidcaller) _whiteList.AddInOrder(1, gc.Choices, false, Cards.MalGanis, Cards.LordJaraxxus, Cards.Doomguard, Cards.FearsomeDoomguard, Cards.DreadInfernal);
        }

        private void HandleDemonZooWarlock(GameContainer gc)
        {
            HandleZoolock(gc);
        }

        private void HandleDragonHandlock(GameContainer gc)
        {
            Core(gc);
            HandleDragonLogicCore(gc);
        }

        private void HandleMalyLock(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandleControlWarlock(GameContainer gc)
        {
            HandleRenoLock(gc);
            _whiteList.AddOrUpdate(Cards.Doomsayer, false);
        }

        private void HandleTempoMage(GameContainer gc)
        {
            Core(gc);
            if (gc.EnemyStyle.Aggresive()) _whiteList.AddAll(false, Cards.Flamewaker, Cards.ArcaneMissiles, Cards.ArcaneBlast);
            else _whiteList.AddAll(false, Cards.ArcaneBlast, Cards.Frostbolt);

        }

        private void HandleFreezeMage(GameContainer gc)
        {
            Core(gc);
            if (gc.EnemyStyle.Aggresive()) _whiteList.AddAll(false, Cards.Doomsayer, Cards.ForgottenTorch, Cards.Frostbolt);
            _whiteList.AddAll(false, Cards.ArcaneIntellect, Cards.AcolyteofPain);
        }

        private void HandleFaceFreezeMage(GameContainer gc)
        {
            HandleFreezeMage(gc);
        }

        private void HandleDragonMage(GameContainer gc)
        {
            Core(gc);
            HandleDragonLogicCore(gc);
        }

        private void HandleMechMage(GameContainer gc)
        {
            HandleMechsCoreLogic(gc);
            if (!gc.EnemyStyle.Aggresive() && gc.HasTurnTwo && gc.Coin)
            {
                _whiteList.AddOrUpdate(Cards.PilotedShredder, false);
            }
        }

        private void HandleEchoMage(GameContainer gc)
        {
            HandleFreezeMage(gc);
            if (gc.EnemyStyle.Aggresive()) _whiteList.AddAll(false, Cards.ExplosiveSheep, Cards.SunfuryProtector);
            if (gc.Choices.HasAny(Cards.EmperorThaurissan, Cards.Duplicate) && gc.EnemyStyle == Style.Control) _whiteList.AddAll(false, Cards.EmperorThaurissan, Cards.Duplicate);
            if (gc.OpponentClass.IsOneOf(Priest, Mage) && !gc.EnemyStyle.Aggresive()) _whiteList.Remove(Cards.Doomsayer);

        }

        private void HandleFatigueMage(GameContainer gc)
        {
            HandleEchoMage(gc);
        }

        private void HandleRenoMage(GameContainer gc)
        {
            Core(gc);

        }

        private void HandleDragonPriest(GameContainer gc)
        {
            Core(gc);
            HandleDragonLogicCore(gc);
        }

        private void HandleControlPriest(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandleComboPriest(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandleMechPriest(GameContainer gc)
        {
            HandleMechsCoreLogic(gc);
            if (!gc.EnemyStyle.Aggresive() && gc.HasTurnTwo && gc.Coin)
            {
                _whiteList.AddOrUpdate(Cards.PilotedShredder, false);
            }
        }

        private void HandleShadowPriest(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandleMidrangeHunter(GameContainer gc)
        {
            _whiteList.AddOrUpdate(Cards.Doomsayer, false);
            foreach (var q in gc.Choices.Where(c => CardTemplate.LoadFromId(c).Cost <= 4 && CardTemplate.LoadFromId(c).Type == Card.CType.MINION))
            {
                switch (CardTemplate.LoadFromId(q).Cost)
                {
                    case 1:
                    _whiteList.AddOrUpdate(q.Priority() > 1 ? q : Nothing, false);
                    break;
                    case 2:
                    _whiteList.AddOrUpdate(q.Priority() > 1 ? q : Nothing, q.Priority() >= 3 && gc.Coin);
                    break;
                    case 3:
                    gc.HasTurnThree = true;
                    _whiteList.AddOrUpdate(q.Priority() > 1 ? q : Nothing, false);
                    break;
                    case 4:
                    _whiteList.AddOrUpdate(q.Priority() > 6 && gc.Coin ? q : Nothing, false);
                    break;
                }
            }
            foreach (var q in gc.Choices.Where(c => CardTemplate.LoadFromId(c).Cost <= 4 && !CardTemplate.LoadFromId(c).IsSecret && CardTemplate.LoadFromId(c).Type == Card.CType.SPELL))
            {
                switch (CardTemplate.LoadFromId(q).Cost)
                {
                    case 1:
                    _whiteList.AddOrUpdate(q, false);
                    break;
                    case 2:
                    _whiteList.AddOrUpdate(q, false);
                    break;
                    case 3:
                    gc.HasTurnThree = true;
                    _whiteList.AddOrUpdate(gc.EnemyStyle.Aggresive() ? Cards.UnleashtheHounds : Nothing, false);
                    _whiteList.AddOrUpdate(gc.Coin ? Cards.AnimalCompanion : Nothing, gc.Coin);
                    break;
                    case 4:
                    _whiteList.AddOrUpdate(q, false);
                    break;
                }
            }
            _whiteList.AddOrUpdate(gc.HasTurnTwo && gc.Choices.HasAny(Cards.AnimalCompanion) && gc.Coin && gc.OpponentClass.Is(Warrior) ? Cards.SavannahHighmane : Nothing, false);
            //_whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.HauntedCreeper, Cards.Webspinner) ? Cards.HuntersMark : Nothing, false);
            _whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.KnifeJuggler) && gc.Coin ? Cards.SnakeTrap : Nothing, false);
            _whiteList.AddOrUpdate(!gc.HasTurnTwo && !gc.HasTurnOne && !gc.Coin ? Cards.FreezingTrap : Nothing, false);
        }

        private void HandleHybridHunter(GameContainer gc)
        {
            Core(gc);
        }

        private void HandleFaceHunter(GameContainer gc)
        {
            _whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.MadScientist) ? Cards.EaglehornBow : gc.Choices.HasAny(Cards.Glaivezooka) ? Cards.Glaivezooka : Cards.EaglehornBow, false);

            var allowHunterMark = (gc.Choices.HasAny(Cards.Webspinner, Cards.HauntedCreeper));
            foreach (var q in from q in gc.Choices where q.Cost() == 1 select q)
            {
                gc.HasTurnOne = true;
                _whiteList.AddOrUpdate(q, true);
            }
            foreach (var q in from q in gc.Choices where q.Cost() == 1 && !q.IsSpell() select q)
            {
                gc.HasTurnTwo = true;
                _whiteList.AddOrUpdate(q, true);
            }
            foreach (var q in
                (from q in gc.Choices let w = CardTemplate.LoadFromId(q) where w.Cost == 3 && w.Health > 1 select q).Where(q => gc.HasTurnOne || gc.HasTurnTwo))
            {
                gc.HasTurnThree = true;
                _whiteList.AddOrUpdate(q, false);
            }
        }

        private void HandleHatHunter(GameContainer gc)
        {
            HandleMidrangeHunter(gc);
        }

        private void HandleCamelHunter(GameContainer gc)
        {
            HandleMidrangeHunter(gc);
            if (_whiteList.ContainsKey(Cards.InjuredKvaldir)) _whiteList.Remove(Cards.InjuredKvaldir);
        }

        private void HandleRenoHunter(GameContainer gc)
        {
            Core(gc);
            HandleMidrangeHunter(gc);
        }

        private void HandleDragonHunter(GameContainer gc)
        {
            Core(gc);
            HandleDragonLogicCore(gc);
        }

        private void HandleOilRogue(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandlePirateRogue(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandleFaceRogue(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandleMalyRogue(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandleRaptorRogue(GameContainer gc)
        {
            foreach (var q in gc.Choices.Where(c => (gc.EnemyStyle.Aggresive() ? c.Cost() <= 2 : c.Cost() <= 3) && !c.IsSpell() && CardTemplate.LoadFromId(c).Quality != Card.CQuality.Legendary))
                _whiteList.AddOrUpdate(q, q.Cost() < 3);

            var has2 = gc.TwoDrops.Any();
            var has3 = gc.ThreeDrops.Any();
            var hasDeathRattle = gc.Choices.Any(q => CardTemplate.LoadFromId(q).HasDeathrattle && CardTemplate.LoadFromId(q).Cost <= 2 && CardTemplate.LoadFromId(q).Quality != Card.CQuality.Legendary);
            _whiteList.AddOrUpdate(hasDeathRattle ? Cards.UnearthedRaptor : Nothing, hasDeathRattle && gc.Coin);
            _whiteList.AddOrUpdate(has2 && has3 ? Cards.PilotedShredder : Nothing, false);
            if (gc.EnemyStyle.Aggresive()) _whiteList.AddOrUpdate(Cards.Backstab, false);
            _whiteList.AddOrUpdate(gc.Coin ? Cards.Sap : Nothing, false);
            _whiteList.AddOrUpdate(gc.Coin ? Cards.PilotedShredder : Nothing, false);
            _whiteList.AddOrUpdate(Cards.NerubianEgg, gc.Coin);
            _whiteList.AddOrUpdate(gc.OpponentClass.Is(Paladin) ? Cards.FanofKnives : Nothing, false);
        }

        private void HandleFatigueRogue(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandleMiracleRogue(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandleMechRogue(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandleRenoRogue(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandleMillRogue(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandleFaceShaman(GameContainer gc)
        {
            foreach (var q in gc.OneDrops.Where(card => card.IsMinion()))
            {
                gc.HasTurnOne = true;
                _whiteList.AddOrUpdate(q, gc.Coin || q.Priority() >= 4);
            }
            foreach (var w in gc.OneDrops.Where(card => card.IsMinion()).Where(w => gc.Choices.HasTurn(1, 2) || gc.Coin))
            {
                if (gc.Coin) _whiteList.AddOrUpdate(w, false);
                gc.HasTurnTwo = true;
                _whiteList.AddOrUpdate(w, w.Priority() > 4 && gc.Coin && CardTemplate.LoadFromId(w).Overload == 0);
            }
            foreach (var w in
                gc.OneDrops.Where(card => card.IsMinion()).Where(w => gc.Choices.HasTurn(1, 2) && gc.Choices.HasTurn(2, 2)))
            {
                gc.HasTurnThree = true;
                _whiteList.AddOrUpdate(w, false);
            }

            if (gc.OpponentClass.IsOneOf(Rogue, Warrior))
                _whiteList.AddOrUpdate(gc.HasTurnOne && gc.HasTurnTwo && gc.HasTurnThree ? Cards.Doomhammer : Nothing, false);
            if (gc.OpponentClass.IsOneOf(Mage, Druid))
                _whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.LightningBolt) ? Cards.LightningBolt : gc.HasTurnOne && gc.Coin ? Cards.RockbiterWeapon : Nothing, false);
        }


        private void HandleMechShaman(GameContainer gc)
        {
            HandleMechsCoreLogic(gc);
            if (!gc.EnemyStyle.Aggresive() && gc.HasTurnTwo && gc.Coin)
            {
                _whiteList.AddOrUpdate(Cards.PilotedShredder, false);
            }
        }

        private void HandleDragonShaman(GameContainer gc)
        {
            Report("Enetered DShaman");
            Core(gc);
            Report("Handled Core");
            HandleDragonLogicCore(gc);
            Report("Handled Dragon Core");
        }

        private void HandleTotemShaman(GameContainer gc)
        {
            Core(gc);
            if (gc.OpponentClass.WeaponClass() && gc.Coin) _whiteList.AddOrUpdate(Cards.HarrisonJones, false);
            //Hex is kept vs high innervate threats and flamewaker here.
            if (gc.OpponentClass.IsOneOf(Druid, Mage)) _whiteList.AddOrUpdate(Cards.Hex, false);
            //Cleric and Mana wyrm might get out of hand, so we try to kill them asap with biter.
            if (gc.OpponentClass.IsOneOf(Priest, Mage)) _whiteList.AddOrUpdate(Cards.RockbiterWeapon, false);
            if (gc.OpponentClass.Is(Warrior) && gc.HasTurnOne) _whiteList.AddOrUpdate(Cards.FlametongueTotem, false);
            if (gc.OpponentClass.Is(Warlock) && gc.EnemyStyle.Aggresive()) _whiteList.AddOrUpdate(Cards.LightningStorm, false);
            if (gc.HasTurnOne && gc.HasTurnTwo && gc.Coin) _whiteList.AddOrUpdate(Cards.FlamewreathedFaceless, false);

        }

        private void HandleMalyShaman(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandleControlShaman(GameContainer gc)
        {
            throw new NotImplementedException();
        }

        private void HandleBattlecryShaman(GameContainer gc)
        {
            Core(gc);
            HandleTotemShaman(gc);
        }

        private void HandleRenoShaman(GameContainer gc)
        {
            Core(gc);
        }

        private void HandleControlWarrior(GameContainer gc)
        {
            bool hasWeapon = false;


            _whiteList.AddAll(false, Cards.FieryWarAxe, Cards.DeathsBite);
            foreach (var q in gc.Choices)
            {
                if (q.Cost() < 5 && q.IsWeapon())
                {
                    hasWeapon = true;
                    _whiteList.AddOrUpdate(q, false);
                    switch (q.Cost())
                    {
                        case 2:
                        gc.HasTurnTwo = true;
                        break;
                        case 4:
                        gc.HasTurnThree = true;
                        break;
                    }
                }
                if (q.Cost() != 2 || q.IsMinion()) continue;
                gc.HasTurnTwo = true;
                _whiteList.AddOrUpdate(q, false);
            }
            _whiteList.AddOrUpdate(Cards.FierceMonkey, hasWeapon && gc.Coin && gc.EnemyStyle.Aggresive());
            if (gc.Coin && (gc.HasTurnTwo || gc.Choices.HasAny(Cards.FieryWarAxe, Cards.DeathsBite)))
            {
                _whiteList.AddOrUpdate(Cards.ShieldBlock, false);
                _whiteList.AddOrUpdate(Cards.ShieldSlam, false);
            }
            _whiteList.AddOrUpdate(gc.EnemyStyle.Aggresive() ? Cards.Whirlwind : Nothing, false);
            if (gc.OpponentClass.Is(Card.CClass.WARLOCK))
            {
                _whiteList.AddOrUpdate(Cards.ShieldSlam, false);
                _whiteList.AddOrUpdate(Cards.Execute, false);
                _whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.Execute) ? Cards.CruelTaskmaster : Nothing, false);
            }
            if (gc.OpponentClass.WeaponClass())
                _whiteList.AddOrUpdate(gc.HasTurnTwo || gc.Coin ? Cards.HarrisonJones : Nothing, false);
        }


        private void Core(GameContainer gc)
        {
            try
            {
                string[] fKeep = ((string)Bot.GetPlugins().Find(p => p.DataContainer.Name == "Arthurs Bundle - Mulligan Core").GetProperties()["fck"]).Split('~');
                if (fKeep.Length > 1)
                {
                    foreach (var q in fKeep.Where(c => CardTemplate.LoadFromId((Card.Cards)Enum.Parse(typeof(Card.Cards), c)).IsCollectible))
                    {
                        if (q == "" || q == null) continue;
                        _whiteList.AddOrUpdate((Card.Cards)Enum.Parse(typeof(Card.Cards), q), false);
                        if (gc.Choices.Contains((Card.Cards)Enum.Parse(typeof(Card.Cards), q)))
                            Bot.Log(string.Format("[AB - Mulligan] Keeping {0} because it's on forceful whitelist", CardTemplate.LoadFromId(q).Name));
                    }
                }
            }catch(Exception)
            {
                //Bot.Log("[AB - Mulligan] Was unable to forcefully keep cards " + formaterro.Message);
            }
            bool noChange = (bool)Bot.GetPlugins().Find(c => c.DataContainer.Name == "Arthurs Bundle - Mulligan Core").GetProperties()["NoChange"];
            bool strict = (string) Bot.GetPlugins().Fetch("Arthurs Bundle - Mulligan Core", "mode").ToString() == "StrictCurve" 
                && !noChange;
            MulliganCoreData data;
            try
            {
                if (noChange)
                    data = new MulliganCoreData(2, 1, 3, 2, 2, 1, 1, 1, false, true);
                else data = new MulliganCoreData();

                #region minion handler

            }
            catch (Exception e)
            {
                Bot.Log("[FAILED] " + e.Message);
                data = new MulliganCoreData(2, 1, 3, 2, 2, 1, 1, 1, false, true);
            }
            try
            {

                int allowed1Drops = gc.Coin ? data.Max1DropsCoin : data.Max1Drops;
                int allowed2Drops = gc.Coin ? data.Max2DropsCoin : data.Max2Drops;
                int allowed3Drops = 0;
                int allowed4Drops = 0;

                foreach (var q in gc.OneDrops.Where(q => q.IsMinion() && q.Priority() >= 2).OrderByDescending(q => q.Priority()).Take(allowed1Drops))
                {
                    gc.HasTurnOne = true;
                    _whiteList.AddOrUpdate(q, gc.Coin && q.Priority() > 5);
                }
                if (strict && !gc.HasTurnOne && !gc.Coin)
                {
                    HandleSpellsAndWeapons(gc);
                    return;
                };
                foreach (var q in gc.TwoDrops.Where(q => q.IsMinion() && q.Priority() >= 2).OrderByDescending(q => q.Priority()).Take(allowed2Drops))
                {
                    gc.HasTurnTwo = true;
                    _whiteList.AddOrUpdate(q, q.Priority() > 5);
                }
                if (strict && !gc.HasTurnTwo)
                {
                    HandleSpellsAndWeapons(gc);
                    return;
                };
                allowed3Drops = gc.Coin && (gc.HasTurnOne || gc.HasTurnTwo)
                    ? data.Max3DropsCoin
                    : (gc.HasTurnOne || gc.HasTurnTwo) ? data.Max3Drops : 0;
                if (!data.NoChange && data.control)
                    allowed3Drops = gc.Coin
                        ? data.Max3DropsCoin
                        : data.Max3Drops;

                if (strict && !gc.HasTurnThree)
                {
                    HandleSpellsAndWeapons(gc);
                    return;
                };
                foreach (var q in gc.ThreeDrops.Where(q => q.IsMinion() && q.Priority() >= 1).OrderByDescending(q => q.Priority()).Take(allowed3Drops))
                {
                    _whiteList.AddOrUpdate(q, false);
                    gc.HasTurnThree = true;
                }
                if (strict && !gc.HasTurnThree)
                {
                    HandleSpellsAndWeapons(gc);
                    return;
                };
                allowed4Drops = gc.Coin && gc.HasTurnThree
                    ? data.Max4DropsCoin
                    : gc.HasTurnThree ? data.Max4Drops : 0;
                if (!data.NoChange && data.control)
                    allowed4Drops = gc.Coin
                        ? data.Max4DropsCoin
                        : data.Max4Drops;
                foreach (var q in gc.FourDrops.Where(q => q.IsMinion() && q.Priority() >= 4).OrderByDescending(q => q.Priority()).Take(allowed4Drops))
                {
                    _whiteList.AddOrUpdate(q, gc.Coin);
                }
            }
            catch (Exception e)
            {
                Bot.Log("[URGENT] Please locate DebugLog.txt in Logs/ABTracker/ folder and show it to Arthur");
                Report("---------- Core Handler encountered an error ----------");
                Report(e.Message);
                Report(string.Format("{0}:{1}", e.HelpLink, e.TargetSite));
                Report(e.Message);
            }
            
            #endregion

            HandleSpellsAndWeapons(gc);
        }

        private void HandleSpellsAndWeapons(GameContainer gc)
        {
            List<Card.Cards> ThreatHandler = new List<Card.Cards>
            {
              //Mage
              Cards.Polymorph, Cards.PolymorphBoar,
              //Paladin
              Cards.Equality, Cards.AldorPeacekeeper, Cards.Humility, Cards.KeeperofUldaman,
              //Rogue
              Cards.Sap,
              //Priest
              Cards.ShadowWordDeath,
              //Warrior
              Cards.Execute,
              //Shaman
              Cards.Hex,
              //Hunter
              Cards.DeadlyShot,Cards.HuntersMark,
              //Druid
              Cards.Mulch,
              //Warlock
            };
            if (Bot.CurrentMode().IsShitfest() && gc.OpponentClass.Is(Shaman) && ThreatHandler.Intersect(gc.Choices).Any())
                _whiteList.AddOrUpdate(ThreatHandler.Intersect(gc.Choices).First(), false);
            #region spell/weapon handler

            bool hasGood1 = gc.Choices.HasTurn(1, 3);
            bool hasGood2 = gc.Choices.HasTurn(2, 3);
            bool hasGood1Or2 = gc.Choices.HasTurn(1, 3) || gc.Choices.HasTurn(2, 3);
            bool hasGood1And2 = gc.Choices.HasTurn(1, 3) && gc.Choices.HasTurn(2, 3);
            switch (gc.OwnClass)
            {
                case Card.CClass.SHAMAN:
                _whiteList.AddInOrder(1, gc.Choices, false, Cards.StormforgedAxe, Cards.Powermace);
                _whiteList.AddOrUpdate(Cards.RockbiterWeapon, false); // [1 Cost]
                _whiteList.AddOrUpdate(!hasGood2 && gc.Coin ? Cards.FarSight : Card.Cards.GAME_005, false); // [3 Cost]
                _whiteList.AddOrUpdate(Cards.FeralSpirit, false); // [3 Cost]
                break;
                case Card.CClass.PRIEST:
                _whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.InjuredBlademaster) ? Cards.LightoftheNaaru : Card.Cards.GAME_005, false); // [1 Cost]
                _whiteList.AddOrUpdate(hasGood1Or2 ? Cards.HolySmite : Card.Cards.GAME_005, false); // [1 Cost]
                _whiteList.AddOrUpdate(!gc.Coin ? Cards.MindVision : Card.Cards.GAME_005, false); // [1 Cost]
                _whiteList.AddOrUpdate(Cards.PowerWordShield, false); // [1 Cost] 
                _whiteList.AddOrUpdate(Cards.ShadowWordPain, false); // [2 Cost]
                _whiteList.AddOrUpdate(Cards.Shadowform, false); // [3 Cost]
                _whiteList.AddOrUpdate((hasGood1Or2 && gc.Coin) || hasGood2 ? Cards.VelensChosen : Card.Cards.GAME_005, false); // [3 Cost]
                break;
                case Card.CClass.MAGE:
                _whiteList.AddOrUpdate(hasGood1 ? Cards.Frostbolt : Card.Cards.GAME_005, false); // [2 Cost]
                _whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.ManaWyrm) && gc.Coin ? Cards.MirrorImage : Card.Cards.GAME_005, false); // [1 Cost]
                _whiteList.AddOrUpdate(hasGood1 ? Cards.ArcaneMissiles : Card.Cards.GAME_005, false); // [1 Cost]
                _whiteList.AddOrUpdate(!hasGood1And2 ? Cards.MirrorEntity : Card.Cards.GAME_005, false); // [3 Cost]
                _whiteList.AddOrUpdate(!hasGood1And2 || hasGood2 ? Cards.ForgottenTorch : Card.Cards.GAME_005, false); // [3 Cost]
                _whiteList.AddOrUpdate(hasGood1 || gc.Coin ? Cards.Flamecannon : Card.Cards.GAME_005, false); // [2 Cost]
                _whiteList.AddOrUpdate(Cards.UnstablePortal, gc.Coin); // [2 Cost]
                _whiteList.AddOrUpdate(hasGood1Or2 ? Cards.ArcaneBlast : Card.Cards.GAME_005, false); // [1 Cost]
                break;
                case Card.CClass.PALADIN:
                _whiteList.AddInOrder(1, gc.Choices, false, Cards.LightsJustice, Cards.RallyingBlade, Cards.Coghammer, Cards.SwordofJustice);
                _whiteList.AddOrUpdate(gc.Coin ? Cards.DivineStrength : Nothing, false);
                _whiteList.AddOrUpdate(hasGood2 ? Cards.NobleSacrifice : Nothing, false); // [1 Cost]
                _whiteList.AddAll(false, Cards.Avenge, Cards.MusterforBattle);

                break;
                case Card.CClass.WARRIOR:
                _whiteList.AddOrUpdate(gc.OpponentClass == Card.CClass.PALADIN ? Cards.Whirlwind : Card.Cards.GAME_005, false); // [1 Cost]
                _whiteList.AddOrUpdate(hasGood2 && gc.Choices.HasAny(Cards.ShieldSlam) ? Cards.ShieldBlock : Card.Cards.GAME_005, false); // [3 Cost]
                _whiteList.AddAll(false, Cards.BloodToIchor, Cards.Slam); // [2 Cost]
                _whiteList.AddOrUpdate(!hasGood1 ? Cards.Upgrade : Card.Cards.GAME_005, false); // [1 Cost]
                _whiteList.AddOrUpdate(hasGood2 && gc.Choices.HasAny(Cards.ShieldBlock) ? Cards.ShieldSlam : Card.Cards.GAME_005, false); // [1 Cost]
                _whiteList.AddOrUpdate(hasGood1Or2 && !gc.Choices.HasTurn(3, 3) ? Cards.Bash : Card.Cards.GAME_005, false); // [3 Cost]
                break;
                case Card.CClass.WARLOCK:
                _whiteList.AddAll(false, Cards.RenounceDarkness, Cards.MortalCoil); // [1 Cost]
                _whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.NerubianEgg) ? Cards.PowerOverwhelming : Card.Cards.GAME_005, false); // [1 Cost]
                _whiteList.AddOrUpdate(!hasGood2 ? Cards.CurseofRafaam : Card.Cards.GAME_005, false); // [2 Cost]
                _whiteList.AddOrUpdate(hasGood1Or2 || gc.Coin ? Cards.Darkbomb : Card.Cards.GAME_005, false); // [2 Cost]

                break;
                case Card.CClass.HUNTER:
                _whiteList.AddInOrder(1, gc.Choices, false, Cards.Glaivezooka, Cards.EaglehornBow);

                _whiteList.AddOrUpdate(!hasGood1Or2 ? Cards.Tracking : Card.Cards.GAME_005, false); // [1 Cost]
                _whiteList.AddOrUpdate(Cards.AnimalCompanion, gc.Coin); // [3 Cost]
                _whiteList.AddOrUpdate(gc.OpponentClass == Card.CClass.PALADIN ? Cards.UnleashtheHounds : Card.Cards.GAME_005, false); // [3 Cost]
                _whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.KnifeJuggler) ? Cards.SnakeTrap : Card.Cards.GAME_005, false); // [2 Cost]
                _whiteList.AddOrUpdate(!hasGood1Or2 ? Cards.FreezingTrap : Card.Cards.GAME_005, false); // [2 Cost]
                _whiteList.AddOrUpdate(hasGood1Or2 && gc.Coin ? Cards.QuickShot : Card.Cards.GAME_005, false); // [2 Cost]
                _whiteList.AddOrUpdate(hasGood1And2 ? Cards.Powershot : Card.Cards.GAME_005, false); // [3 Cost]
                _whiteList.AddOrUpdate(gc.Coin ? Cards.BearTrap : Card.Cards.GAME_005, false); // [2 Cost]
                break;
                case Card.CClass.ROGUE:
                _whiteList.AddInOrder(1, gc.Choices, false, Cards.PerditionsBlade, Cards.CogmastersWrench);
                _whiteList.AddOrUpdate(gc.Coin || !gc.HasTurnOne ? Cards.JourneyBelow : Nothing, false);
                _whiteList.AddOrUpdate(Cards.Backstab, false); // [0 Cost]
                _whiteList.AddOrUpdate(Cards.DeadlyPoison, false); // [1 Cost]
                _whiteList.AddOrUpdate(gc.OpponentClass == Card.CClass.PALADIN ? Cards.FanofKnives : Card.Cards.GAME_005, false); // [3 Cost]
                _whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.Burgle) || gc.Choices.HasAny(Cards.BeneaththeGrounds) ? Cards.Preparation : Card.Cards.GAME_005, false); // [0 Cost]
                _whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.Preparation) ? Cards.Burgle : Card.Cards.GAME_005, false); // [3 Cost]
                _whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.Preparation) ? Cards.BeneaththeGrounds : Card.Cards.GAME_005, false); // [3 Cost]

                break;
                case Card.CClass.DRUID:
                _whiteList.AddOrUpdate(hasGood1 ? Cards.MarkoftheWild : Card.Cards.GAME_005, false); // [2 Cost]
                _whiteList.AddAll(false, Cards.Wrath, Cards.PoweroftheWild);
                _whiteList.AddOrUpdate(Cards.LivingRoots, gc.Coin); // [1 Cost]
                break;
            }
            foreach (var card in from card in gc.Choices let cardQ = CardTemplate.LoadFromId(card) where _whiteList.ContainsKey(card) && cardQ.IsSecret && gc.Choices.HasAny(Cards.MadScientist) select card)
            {
                _whiteList.Remove(card);
            }

            #endregion
        }

        /// <summary>
        /// Secret Palaidn mulligan logic
        /// ported from V2
        /// </summary>
        /// <param name="gc">game container</param>
        private void HandleSecretPaladin(GameContainer gc)
        {
            var has2Drop = gc.Choices.Any(c => c.Cost() == 2 && c.IsMinion());
            bool vAggro = gc.EnemyStyle.Aggresive();
            _whiteList.AddOrUpdate(!gc.Choices.HasTurn(1, 0) && gc.Coin && gc.Choices.HasAny(Cards.MusterforBattle) ? Cards.Avenge : Nothing, false);
            _whiteList.AddOrUpdate(gc.OpponentClass.Is(Shaman) && gc.Choices.HasAny(Cards.ShieldedMinibot) ? Cards.Redemption : Nothing, false);

            if (gc.Choices.HasAll(Cards.NobleSacrifice, Cards.Avenge, Cards.Secretkeeper) && gc.OpponentClass.Is(Shaman))
                _whiteList.AddAll(false, Cards.NobleSacrifice, Cards.Avenge, Cards.Secretkeeper);
            _whiteList.AddOrUpdate(gc.OpponentClass.Is(Shaman) && gc.Coin ? Cards.HarrisonJones : Nothing, false);
            _whiteList.AddOrUpdate(gc.Choices.HasAny(Cards.HauntedCreeper, Cards.NerubianEgg) && gc.Coin ? Cards.KeeperofUldaman : Nothing, false);

            foreach (var q in gc.Choices)
            {
                var card = CardTemplate.LoadFromId(q);
                if (q.Cost() == 1 && q.IsMinion())
                    _whiteList.AddOrUpdate(q, card.Divineshield || card.Health == 3);
                if (q.Cost() == 2 && q.IsMinion())
                    _whiteList.AddOrUpdate(q, card.Divineshield && card.Atk == 2);
                if (q.Cost() == 3 && q.IsSpell() && q != Cards.DivineFavor)
                    _whiteList.AddOrUpdate(q, false);
            }
            if (vAggro)
            {
                _whiteList.AddOrUpdate(gc.OpponentClass.Is(Druid) ? Cards.AldorPeacekeeper : Nothing, false);
                _whiteList.AddOrUpdate(Cards.Consecration, false);
                _whiteList.AddOrUpdate(gc.Coin ? Cards.PilotedShredder : Nothing, false);
            }
            else
            {
                _whiteList.Remove(Cards.IronbeakOwl);
                _whiteList.AddOrUpdate(gc.Coin ? Cards.MysteriousChallenger : Nothing, false);
                _whiteList.AddOrUpdate(gc.Coin ? Cards.TruesilverChampion : Nothing, false);
                _whiteList.AddOrUpdate(Cards.PilotedShredder, gc.OpponentClass.Is(Warrior) && gc.Coin);
            }
            _whiteList.AddOrUpdate(Cards.MusterforBattle, false);
            if (gc.Choices.HasAny(Cards.Coghammer) && has2Drop && !gc.OpponentClass.Is(Warrior))
                _whiteList.AddOrUpdate(Cards.Coghammer, false);
            else if (gc.OpponentClass.IsOneOf(Warrior, Priest, Rogue, Druid))
                _whiteList.AddOrUpdate(Cards.TruesilverChampion, false);
        }


        /// <summary>
        /// 
        /// </summary>
        private void ClearReport()
        {
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\DebugLog.txt", string.Empty);
        }

        public static void Report(string msg)
        {
            using (StreamWriter log = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ABTracker\\DebugLog.txt", true))
            {
                log.WriteLine("[{0}] {1}", DateTime.Now, msg);
            }
        }

        private DeckType unknown = DeckType.Unknown;
        private DeckType Arena = DeckType.Arena;
        /*Warrior*/
        private DeckType ControlWarrior = DeckType.ControlWarrior;
        private DeckType FatigueWarrior = DeckType.FatigueWarrior;
        private DeckType DragonWarrior = DeckType.DragonWarrior;
        private DeckType PatronWarrior = DeckType.PatronWarrior;
        private DeckType WorgenOTKWarrior = DeckType.WorgenOTKWarrior;
        private DeckType MechWarrior = DeckType.MechWarrior;
        private DeckType FaceWarrior = DeckType.FaceWarrior;
        private DeckType RenoWarrior = DeckType.RenoWarrior;
        /*Paladin*/
        private DeckType SecretPaladin = DeckType.SecretPaladin;
        private DeckType MidRangePaladin = DeckType.MidRangePaladin;
        private DeckType DragonPaladin = DeckType.DragonPaladin;
        private DeckType AggroPaladin = DeckType.AggroPaladin;
        private DeckType AnyfinMurglMurgl = DeckType.AnyfinMurglMurgl;
        private DeckType RenoPaladin = DeckType.RenoPaladin;
        /*Druid*/
        private DeckType RampDruid = DeckType.RampDruid;
        private DeckType AggroDruid = DeckType.AggroDruid;
        private DeckType DragonDruid = DeckType.DragonDruid;
        private DeckType MidRangeDruid = DeckType.MidRangeDruid;
        private DeckType TokenDruid = DeckType.TokenDruid;
        private DeckType SilenceDruid = DeckType.SilenceDruid;
        private DeckType MechDruid = DeckType.MechDruid;
        private DeckType AstralDruid = DeckType.AstralDruid;
        private DeckType MillDruid = DeckType.MillDruid;
        private DeckType BeastDruid = DeckType.BeastDruid;
        private DeckType RenoDruid = DeckType.RenoDruid;
        /*Warlock*/
        private DeckType Handlock = DeckType.Handlock;
        private DeckType RenoLock = DeckType.RenoLock;

        private DeckType Zoolock = DeckType.Zoolock;

        private DeckType DemonHandlock = DeckType.DemonHandlock;

        private DeckType DragonHandlock = DeckType.DragonHandlock;
        private DeckType MalyLock = DeckType.MalyLock;
        private DeckType ControlWarlock = DeckType.ControlWarlock;
        /*Mage*/
        private DeckType TempoMage = DeckType.TempoMage;
        private DeckType FreezeMage = DeckType.FreezeMage;
        private DeckType FaceFreezeMage = DeckType.FaceFreezeMage;
        private DeckType DragonMage = DeckType.DragonMage;
        private DeckType MechMage = DeckType.MechMage;
        private DeckType EchoMage = DeckType.EchoMage;
        private DeckType RenoMage = DeckType.RenoMage;
        /*Priest*/
        private DeckType DragonPriest = DeckType.DragonPriest;
        private DeckType ControlPriest = DeckType.ControlPriest;
        private DeckType ComboPriest = DeckType.ComboPriest;
        private DeckType MechPriest = DeckType.MechPriest;

        /*Huntard*/
        private DeckType MidRangeHunter = DeckType.MidRangeHunter;
        private DeckType HybridHunter = DeckType.HybridHunter;
        private DeckType FaceHunter = DeckType.FaceHunter;
        private DeckType CamelHunter = DeckType.CamelHunter;
        private DeckType RenoHunter = DeckType.RenoHunter;
        private DeckType DragonHunter = DeckType.DragonHunter;
        /*Rogue*/
        private DeckType OilRogue = DeckType.OilRogue;
        private DeckType PirateRogue = DeckType.PirateRogue;
        private DeckType FaceRogue = DeckType.FaceRogue;
        private DeckType MalyRogue = DeckType.MalyRogue;
        private DeckType RaptorRogue = DeckType.RaptorRogue;
        private DeckType MiracleRogue = DeckType.MiracleRogue;
        private DeckType MechRogue = DeckType.MechRogue;
        private DeckType RenoRogue = DeckType.RenoRogue;
        private DeckType MillRogue = DeckType.MillRogue;
        /*Chaman*/
        private DeckType FaceShaman = DeckType.FaceShaman;
        private DeckType MechShaman = DeckType.MechShaman;
        private DeckType DragonShaman = DeckType.DragonShaman;
        private DeckType MidrangeShaman = DeckType.MidrangeShaman;
        private DeckType MalygosShaman = DeckType.MalygosShaman;
        private DeckType ControlShaman = DeckType.ControlShaman;
        private DeckType BattleryShaman = DeckType.BattleryShaman;
        private DeckType RenoShaman = DeckType.RenoShaman;

        private DeckType Basic = DeckType.Basic;
    }

    #region enums
    public class Prediction
    {
        public static Dictionary<Card.CClass,
            Dictionary<DeckType, int>> prediction = new Dictionary<Card.CClass, Dictionary<DeckType, int>>();
        public void Update(Card.CClass cl, DeckType dt)
        {
            Dictionary<DeckType, int> temp = new Dictionary<DeckType, int>();
            if (prediction.TryGetValue(cl, out temp))
            {
                if (temp.ContainsKey(dt))
                    temp[dt]++;
                else temp[dt] = 0;
                prediction[cl] = temp;
            }
            else prediction[cl] = new Dictionary<DeckType, int> { { dt, 0 } };
        }

        public DeckType GetMostFacedDeckType(Card.CClass cl)
        {
            return prediction[cl].Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }

    }
    public class MulliganCoreData
    {

        public int Max1Drops { get; set; }
        public int Max1DropsCoin { get; set; }
        public int Max2Drops { get; set; }
        public int Max2DropsCoin { get; set; }
        public int Max3Drops { get; set; }
        public int Max3DropsCoin { get; set; }
        public int Max4Drops { get; set; }
        public int Max4DropsCoin { get; set; }
        public bool control { get; set; }
        public bool NoChange { get; set; }



        public MulliganCoreData()
        {
            Dictionary<string, object> data = Bot.GetPlugins().Find(c => c.DataContainer.Name == "Arthurs Bundle - Mulligan Core").GetProperties();
            Max1Drops = (int)data["Max1Drops"];
            Max1DropsCoin = (int)data["Max1DropsCoin"];
            Max2Drops = (int)data["Max2Drops"];
            Max2DropsCoin = (int)data["Max2DropsCoin"];
            Max3Drops = (int)data["Max3Drops"];
            Max3DropsCoin = (int)data["Max3DropsCoin"];
            Max4Drops = (int)data["Max4Drops"];
            Max4DropsCoin = (int)data["Max4DropsCoin"];
            control = (bool)data["control"];
            NoChange = (bool)data["NoChange"];
        }
        public MulliganCoreData(int v1, int v2, int v3, int v4, int v5, int v6, int v7, int v8, bool v9, bool v10)
        {
            Max1Drops = v1;
            Max1DropsCoin = v2;
            Max2Drops = v3;
            Max2DropsCoin = v4;
            Max3Drops = v5;
            Max3DropsCoin = v6;
            Max4Drops = v7;
            Max4DropsCoin = v8;
            control = v9;
            NoChange = v10;
        }

        public override string ToString()
        {
            return string.Format("Container " + Max1Drops + Max1DropsCoin + Max2Drops + Max2DropsCoin + Max3Drops + Max3DropsCoin + Max4Drops + Max4DropsCoin + NoChange + control);
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

    #endregion
}
