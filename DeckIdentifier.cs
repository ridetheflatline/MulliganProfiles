using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartBot.Database;
using SmartBot.Mulligan;
using SmartBot.Plugins.API;
public class OpponentDeckData
{
    public Style DeckStyle { get; set; }
    public DeckType DeckType { get; set; }
    public List<string> Cards { get; set; }
    public long Identification { get; set; }

    private DeckData GetDeckInfo(Card.CClass ownClass, List<string> CurrentDeck)
    {
        var info = new DeckData { Cards = CurrentDeck };

        Dictionary<Dictionary<DeckType, Style>, int> DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>();
        Dictionary<DeckType, Style> BestDeck = new Dictionary<DeckType, Style>();
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
                        {new Dictionary<DeckType, Style> {{DeckType.TotemShaman, Style.Tempo}}, CurrentDeck.Intersect(totemShaman).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.FaceShaman, Style.Face}}, CurrentDeck.Intersect(faceShaman).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.MechShaman, Style.Aggro}}, CurrentDeck.Intersect(mechShaman).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.DragonShaman, Style.Control}}, CurrentDeck.Intersect(dragonShaman).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.MalygosShaman, Style.Combo}}, CurrentDeck.Intersect(malygosShaman).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.ControlShaman, Style.Control}}, CurrentDeck.Intersect(controlShaman).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}}, CurrentDeck.Intersect(basicShaman).ToList().Count},
                    };
                BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                info.DeckType = BestDeck.Keys.First();
                info.DeckStyle = BestDeck.Values.First();
                Bot.Log("===========================");
                foreach (var q in DeckDictionary)
                    Bot.Log(string.Format("For {0} your opponent deck scored {1}", q.Key.First().Key, q.Value));
                Bot.Log("===========================");
                break;

            #endregion

            #region priest

            case Card.CClass.PRIEST:
                List<string> comboPriest = new List<string> { CircleofHealing, CircleofHealing, InnerFire, InnerFire, Alexstrasza, AcolyteofPain, AcolyteofPain, PowerWordShield, PowerWordShield, Silence, Silence, ShadowWordDeath, DivineSpirit, DivineSpirit, NorthshireCleric, NorthshireCleric, HarrisonJones, StormwindKnight, AuchenaiSoulpriest, HolyNova, Loatheb, Deathlord, Deathlord, VelensChosen, GnomereganInfantry, Lightbomb, Lightbomb, EmperorThaurissan, };
                List<string> dragonPriest = new List<string> { CabalShadowPriest, CabalShadowPriest, AzureDrake, PowerWordShield, PowerWordShield, Ysera, ShadowWordDeath, ShadowWordDeath, NorthshireCleric, HarrisonJones, HolyNova, SludgeBelcher, VelensChosen, VelensChosen, DrBoom, Shrinkmeister, Shrinkmeister, Voljin, Lightbomb, Lightbomb, BlackwingTechnician, BlackwingTechnician, TwilightWhelp, TwilightWhelp, TwilightGuardian, TwilightGuardian, Chillmaw, WyrmrestAgent, WyrmrestAgent, };
                List<string> controlPriest = new List<string> { LightoftheNaaru, LightoftheNaaru, PowerWordShield, PowerWordShield, NorthshireCleric, NorthshireCleric, MuseumCurator, MuseumCurator, Shrinkmeister, ShadowWordDeath, AuchenaiSoulpriest, AuchenaiSoulpriest, HolyNova, Entomb, Entomb, Lightbomb, Lightbomb, CabalShadowPriest, WildPyromancer, WildPyromancer, Deathlord, Deathlord, InjuredBlademaster, InjuredBlademaster, SludgeBelcher, SludgeBelcher, JusticarTrueheart, Ysera, };
                List<string> mechPriest = new List<string> { PowerWordShield, PowerWordShield, NorthshireCleric, NorthshireCleric, HolyNova, HolyNova, DarkCultist, DarkCultist, VelensChosen, VelensChosen, DrBoom, SpiderTank, UpgradedRepairBot, UpgradedRepairBot, Mechwarper, Mechwarper, PilotedShredder, PilotedShredder, ClockworkGnome, ClockworkGnome, Shadowboxer, Shadowboxer, Voljin, SpawnofShadows, GorillabotA3, Entomb, MuseumCurator, MuseumCurator, };
                List<string> basicPriest = new List<string> { ChillwindYeti, BoulderfistOgre, AcidicSwampOoze, GnomishInventor, StormwindChampion, ShadowWordPain, SenjinShieldmasta, MindControl, HolySmite, PowerWordShield, ShatteredSunCleric, NoviceEngineer, ShadowWordDeath, BloodfenRaptor, NorthshireCleric, HolyNova, };
                DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {new Dictionary<DeckType, Style> {{DeckType.ComboPriest, Style.Combo}}, CurrentDeck.Intersect(comboPriest).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.DragonPriest, Style.Tempo}}, CurrentDeck.Intersect(dragonPriest).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.ControlPriest, Style.Control}}, CurrentDeck.Intersect(controlPriest).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.MechPriest, Style.Aggro}}, CurrentDeck.Intersect(mechPriest).ToList().Count},
                         {new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}}, CurrentDeck.Intersect(basicPriest).ToList().Count},
                    };
                BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

                info.DeckType = BestDeck.Keys.First();
                info.DeckStyle = BestDeck.Values.First();
                Bot.Log("===========================");
                foreach (var q in DeckDictionary)
                    Bot.Log(string.Format("For {0} your opponent deck scored {1}", q.Key.First().Key, q.Value));
                Bot.Log("===========================");
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
                Bot.Log("===========================");
                foreach (var q in DeckDictionary)
                    Bot.Log(string.Format("For {0} your opponent deck scored {1}", q.Key.First().Key, q.Value));
                Bot.Log("===========================");
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
                Bot.Log("===========================");
                foreach (var q in DeckDictionary)
                    Bot.Log(string.Format("For {0} your opponent deck scored {1}", q.Key.First().Key, q.Value));
                Bot.Log("===========================");
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
                Bot.Log("===========================");
                foreach (var q in DeckDictionary)
                    Bot.Log(string.Format("For {0} your opponent deck scored {1}", q.Key.First().Key, q.Value));
                Bot.Log("===========================");
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
                List<string> relinquary = new List<string> { PowerOverwhelming, PowerOverwhelming, Voidwalker, Voidwalker, IronbeakOwl, DefenderofArgus, DefenderofArgus, AbusiveSergeant, AbusiveSergeant, SeaGiant, SeaGiant, ZombieChow, NerubianEgg, NerubianEgg, Loatheb, EchoingOoze, EchoingOoze, HauntedCreeper, HauntedCreeper, DrBoom, Implosion, Implosion, ImpGangBoss, ImpGangBoss, GormoktheImpaler, DarkPeddler, DarkPeddler, ReliquarySeeker, ReliquarySeeker, };
                List<string> basicdeck = new List<string>{ChillwindYeti, ShatteredSunCleric, SenjinShieldmasta, ZombieChow, HauntedCreeper, SludgeBelcher, KelThuzad, Loatheb, EmperorThaurissan, ImpGangBoss, DarkPeddler, JeweledScarab, ArchThiefRafaam, ShadowBolt, Hellfire, DreadInfernal, MortalCoil,  
};
                DeckDictionary = new Dictionary<Dictionary<DeckType, Style>, int>
                    {
                        {new Dictionary<DeckType, Style> {{DeckType.RenoLock, Style.Control}}, CurrentDeck.Intersect(renolock).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.DemonHandlock, Style.Control}}, CurrentDeck.Intersect(demonHandlock).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.Zoolock, Style.Aggro}}, CurrentDeck.Intersect(zoolock).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.DragonHandlock, Style.Control}}, CurrentDeck.Intersect(dragonHandlock).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.DemonZooWarlock, Style.Aggro}}, CurrentDeck.Intersect(demonZooWarlock).ToList().Count}, {new Dictionary<DeckType, Style> {{DeckType.Handlock, Style.Control}}, CurrentDeck.Intersect(handlock).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.RelinquaryZoo, Style.Aggro}}, CurrentDeck.Intersect(relinquary).ToList().Count},
                        {new Dictionary<DeckType, Style> {{DeckType.Basic, Style.Control}}, CurrentDeck.Intersect(basicdeck).ToList().Count},
                    };
                BestDeck = DeckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                info.DeckType = BestDeck.Keys.First();
                info.DeckStyle = BestDeck.Values.First();
                Bot.Log("===========================");
                foreach (var q in DeckDictionary)
                    Bot.Log(string.Format("For {0} your opponent deck scored {1}", q.Key.First().Key, q.Value));
                Bot.Log("===========================");
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
                Bot.Log("===========================");
                foreach (var q in DeckDictionary)
                    Bot.Log(string.Format("For {0} your opponent deck scored {1}", q.Key.First().Key, q.Value));
                Bot.Log("===========================");
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
                Bot.Log("===========================");
                foreach (var q in DeckDictionary)
                    Bot.Log(string.Format("For {0} your opponent deck scored {1}", q.Key.First().Key, q.Value));
                Bot.Log("===========================");
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
                Bot.Log("===========================");
                foreach (var q in DeckDictionary)
                    Bot.Log(string.Format("For {0} your opponent deck scored {1}", q.Key.First().Key, q.Value));
                Bot.Log("===========================");
                break;

            #endregion
        }
        Bot.Log(string.Format("[Smart Mulligan Tracker] Your previous opponent deck is closest to ({0})", info.DeckType));
        return info;
    }

    private DeckData SetUnknown(DeckData info, Dictionary<Dictionary<DeckType, Style>, int> deckDictionary)
    {
        info.DeckType = DeckType.Unknown;
        info.DeckStyle = Style.Unknown;
        return info;
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
    AggroPaladin,
    AnyfinMurglMurgl, //Complete
    RampDruid,
    AggroDruid,
    MidRangeDruid,
    TokenDruid, //Complete
    Handlock,
    RenoLock,
    Zoolock,
    RelinquaryZoo,
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
