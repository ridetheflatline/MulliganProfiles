using SmartBot.Plugins.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using SmartBot.Database;



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
    public class SmartTracker : PluginDataContainer
    {

        /// <summary>
        /// This variable is to add extra two option to SmartTracker that will allow you
        /// to use Mulligan Tester by Botfanatic
        /// </summary>
        private const bool MulliganTesterDebug = false;


        [DisplayName("[0] Update Mulligan")]
        public bool AutoUpdateV3 { get; set; }
        [DisplayName("[0] Update Tracker")]
        public bool AutoUpdateTracker { get; set; }
        //[DisplayName("Random Intro Messages")]
        //public bool RandomMovieQuotes { get; private set; }
        [DisplayName("[S] B# Public Key")]
        public string BitCoin { get; private set; }
        [DisplayName("[1] Identifier Mode")]
        public IdentityMode mode { get; set; }

        [DisplayName("[1] Your Deck")]
        public DeckType ForceDeckType { get; set; }
        [Browsable(MulliganTesterDebug ? true : false)]
        [DisplayName("[test] Mulligan Tester: you")]
        public DeckType MT_YourDeck { get; set; }
        [Browsable(MulliganTesterDebug ? true : false)]
        [DisplayName("[test] Mulligan Tester: enemy")]
        public DeckType MT_OpponentDeck { get; set; }

        [DisplayName("Donatuib Button")]
        public bool donate { get; set; }

       

        [Browsable(false)]
        public string LSmartMulliganV3 { get; private set; }
        [Browsable(false)]
        public string LSmartTracker { get; private set; }



        public SmartTracker()
        {
            Name = "SmartTracker";
            ForceDeckType = DeckType.Unknown;
            MT_OpponentDeck = DeckType.Unknown;
            MT_YourDeck = DeckType.Unknown;
            AutoUpdateV3 = false;
            AutoUpdateTracker = false;
            BitCoin = "16ujoN2p8JaxBvkWFxPRcy1x8roG1XMeBM"; 
            LSmartMulliganV3 = "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/MulliganProfiles/SmartMulliganV3/version.txt";
            LSmartTracker = "https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/Plugins/SmartTracker/tracker.version";

        }
    }

    public class SMTracker : Plugin
    {
        public bool identified = false;
        private DeckData informationData;
        private readonly string MulliganDir = AppDomain.CurrentDomain.BaseDirectory + "MulliganProfiles\\";
        private readonly string MulliganInformation = AppDomain.CurrentDomain.BaseDirectory + "MulliganProfiles\\SmartMulliganV3\\";
        private readonly string TrackerDir = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\";
        private readonly string TrackerVersion = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\SmartTracker\\";

        public override void OnTick()
        {
            switch (Bot.CurrentScene())
            {
                case Bot.Scene.GAMEPLAY:
                    if (Bot.CurrentBoard == null || identified) break;
                   
                    informationData = GetDeckInfo(Bot.CurrentBoard.FriendClass, Bot.CurrentDeck().Cards);
                    using (StreamWriter readMe = new StreamWriter(MulliganInformation + "our_deck.v3", false))
                    {
                        readMe.WriteLine("{0}~{1}~{2}~{3}", 
                            ((SmartTracker)DataContainer).mode,
                            ((SmartTracker)DataContainer).mode == IdentityMode.Auto ? informationData.DeckType : ((SmartTracker)DataContainer).ForceDeckType,
                            informationData.DeckStyle, string.Join(";", informationData.DeckList));
                    }
                    if (((SmartTracker)DataContainer).mode == IdentityMode.Manual
                        && informationData.DeckType == ((SmartTracker)DataContainer).ForceDeckType)
                        Bot.Log("[Tracker] Automatic identification yields to the same identification as your forcefully inserted deck.");
                    Bot.Log(string.Format("Succesfully Identified deck\n{0}|{1}|{2}|", informationData.DeckType, informationData.DeckStyle, string.Join(";", informationData.DeckList)));
                    identified = true;
                    break;

                case Bot.Scene.INVALID:
                    identified = false;
                    break;
                case Bot.Scene.STARTUP:
                    identified = false;
                    break;
                case Bot.Scene.LOGIN:
                    identified = false;
                    break;
                case Bot.Scene.HUB:
                    identified = false;
                    break;
                case Bot.Scene.COLLECTIONMANAGER:
                    identified = false;
                    break;
                case Bot.Scene.PACKOPENING:
                    identified = false;
                    break;
                case Bot.Scene.TOURNAMENT:
                    identified = false;
                    break;
                case Bot.Scene.FRIENDLY:
                    identified = false;
                    break;
                case Bot.Scene.FATAL_ERROR:
                    identified = false;
                    break;
                case Bot.Scene.DRAFT:
                    identified = false;
                    break;
                case Bot.Scene.CREDITS:
                    identified = false;
                    break;
                case Bot.Scene.RESET:
                    identified = false;
                    break;
                case Bot.Scene.ADVENTURE:
                    identified = false;
                    break;
                case Bot.Scene.TAVERN_BRAWL:
                    identified = false;
                    break;
                default:
                    identified = false;
                    break;

            }
        }

        public override void OnPluginCreated()
        {
            try
            {
                CheckDirectory(MulliganInformation);
            }
            catch (Exception mulliganException)
            {
                Bot.Log("Error Updating Mulligan" + mulliganException.Message + " " +mulliganException.TargetSite);
            }
            try
            {
                CheckDirectory(TrackerVersion);
            }
            catch (Exception trackerException)
            {
                Bot.Log("Error Updating Tracker" + trackerException.Message + " " + trackerException.TargetSite);
            }
        }

        public override void OnStarted()
        {

            using (StreamWriter debugStreamWriter = new StreamWriter(MulliganInformation + "debug_decks.v3", false))
            {
                debugStreamWriter.WriteLine("{0}|{1}", ((SmartTracker)DataContainer).MT_YourDeck, ((SmartTracker)DataContainer).MT_OpponentDeck);
            }
            if (((SmartTracker)DataContainer).AutoUpdateV3)
            {
                CheckUpdatesMulligan(((SmartTracker)DataContainer).LSmartMulliganV3);
            }
            if (((SmartTracker)DataContainer).AutoUpdateTracker)
            {
                CheckUpdatesTracker(((SmartTracker)DataContainer).LSmartTracker);
            }
            

        }

       
        private void CheckUpdatesTracker(string lSmartTracker)
        {
            HttpWebRequest request = WebRequest.Create(lSmartTracker) as HttpWebRequest;
            if (request == null)
            {
                Bot.Log(string.Format("[SmartAutoUpdater] Could not get data from gitlink {0}", lSmartTracker));
                return;
            }
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (StreamReader str = new StreamReader(response.GetResponseStream()))
            using (StreamReader localVersion = new StreamReader(TrackerVersion + "tracker.version"))

            {
                string strline = str.ReadLine();
                double remoteVer = double.Parse(strline);
                double localVer = double.Parse(localVersion.ReadLine());
               
                if (localVer == remoteVer) Bot.Log("[SmartTracker] SmartTracker is up to date");
                if (localVer > remoteVer)
                {
                    Bot.Log(string.Format("[SmartTracker] Local Version: {0} Remote Version {1}", localVer, remoteVer));
                    Bot.Log("[SmartTracker] Arthur, you are an idiot. Push new update");
                }
                if (localVer < remoteVer)
                {
                    localVersion.Close();
                    UpdateTracker(lSmartTracker, remoteVer, localVer);
                }

            }
        }

        private void UpdateTracker(string lSmartTracker, double remoteVer, double localVer)
        {
            Bot.Log(string.Format("[SmartTracker] Local Version: {0} Remote Version {1}\n\t\tUpdating...", localVer, remoteVer));
            HttpWebRequest trackeRequest = WebRequest
                .Create("https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/Plugins/SmartTracker.cs")
                as HttpWebRequest;
            if (trackeRequest == null)
            {
                Bot.Log(string.Format("[SmartAutoUpdater] Could not get data from gitlink {0}", lSmartTracker));
                return;
            }
            using (HttpWebResponse mulResponse = trackeRequest.GetResponse() as HttpWebResponse)
            using (StreamReader trFile = new StreamReader(mulResponse.GetResponseStream()))
            using (StreamWriter updateLocalCopy = new StreamWriter(TrackerDir + "SmartTracker.cs"))
            {
                string tempfile = trFile.ReadToEnd();
                Bot.Log("[IGOT HERE]");
                updateLocalCopy.WriteLine(tempfile);
                Bot.RefreshMulliganProfiles();
                Bot.Log("[SmartTracker] SmartTracker is now fully updated");
                UpdateVersion(remoteVer, true);
            }
        }

        private void CheckUpdatesMulligan(string lSmartMulliganV3)
        {
            HttpWebRequest request = WebRequest.Create(lSmartMulliganV3) as HttpWebRequest;
            if (request == null)
            {
                Bot.Log(string.Format("[SmartAutoUpdater] Could not get data from gitlink {0}", lSmartMulliganV3));
                return;
            }
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (StreamReader str = new StreamReader(response.GetResponseStream()))
            using (StreamReader localVersion = new StreamReader(MulliganInformation + "version.txt"))

            {
                double remoteVer = double.Parse(str.ReadLine());
                double localVer = double.Parse(localVersion.ReadLine());
                Bot.Log(remoteVer.ToString(CultureInfo.InvariantCulture));
                if (localVer == remoteVer) Bot.Log("[SmartTracker] SmartMulliganV3 is up to date");
                if (localVer > remoteVer)
                {
                    Bot.Log(string.Format("[SmartTracker] Local Version: {0} Remote Version {1}", localVer, remoteVer));
                    Bot.Log("[SmartTracker] Arthur, you are an idiot. Push new update");
                }
                if (localVer < remoteVer)
                {
                    localVersion.Close();
                    UpdateMulligan(lSmartMulliganV3 ,remoteVer, localVer);
                }

            }
        }

        private void UpdateMulligan(string lSmartMulliganV3, double remoteVer, double localVer)
        {
            Bot.Log(string.Format("[SmartTracker] Local Version: {0} Remote Version {1}\n\t\tUpdating...", localVer, remoteVer));
            HttpWebRequest MulliganRequest = WebRequest
                .Create("https://raw.githubusercontent.com/ArthurFairchild/MulliganProfiles/SmartMulliganV3/MulliganProfiles/SmartMulliganV3.cs")
                as HttpWebRequest;
            if (MulliganRequest == null)
            {
                Bot.Log(string.Format("[SmartAutoUpdater] Could not get data from gitlink {0}", lSmartMulliganV3));
                return;
            }
            using (HttpWebResponse mulResponse = MulliganRequest.GetResponse() as HttpWebResponse)
            using (StreamReader mulFile = new StreamReader(mulResponse.GetResponseStream()))
            using (StreamWriter updateLocalCopy = new StreamWriter(MulliganDir + "SmartMulliganV3.cs"))
            {
                string tempfile = mulFile.ReadToEnd();
                //Bot.Log("");
                updateLocalCopy.WriteLine(tempfile);
                Bot.RefreshMulliganProfiles();
                Bot.Log("[SmartTracker] SmartMulligan is now fully updated");
                UpdateVersion(remoteVer);
            }

        }
    
    

        private void UpdateVersion(double remoteVer, bool value = false)
        {
            
            using (StreamWriter localVersion = new StreamWriter(value ? TrackerVersion +"tracker.version" : MulliganInformation + "version.txt", false))
            {
                localVersion.WriteLine(remoteVer);
            }
            
        }


        public override void OnStopped()
        {
            identified = false;

        }

        public void CheckOpponentDeck(string res)
        {
            List<Card.Cards> graveyard = Bot.CurrentBoard.EnemyGraveyard.ToList();
            List<Card> board = Bot.CurrentBoard.MinionEnemy.ToList();
            List<string> opponentDeck = new List<string> { };
            opponentDeck.AddRange(graveyard.Select(q => q.ToString()));
            opponentDeck.AddRange(board.Select(q => q.Template.Id.ToString()));
            using (StreamWriter opponentDeckInfo = new StreamWriter(MulliganInformation + "OpponentDeckInfo.txt", true))
            {
                DeckData opponentInfo = GetDeckInfo(Bot.CurrentBoard.EnemyClass, opponentDeck);
                opponentDeckInfo.WriteLine("{0}:{1}:{2}:{3}:{4}", res, Bot.GetCurrentOpponentId(), opponentInfo.DeckType, opponentInfo.DeckStyle, string.Join(",", opponentDeck.Where(c => CardTemplate.LoadFromId(c).IsCollectible).ToArray()));
                Bot.Log(string.Format("[Tracker] Succesfully recorder your opponent: {0}", opponentInfo.DeckType));
            }
        }

        public override void OnDefeat()
        {
            CheckOpponentDeck("lost");
        }

        public override void OnVictory()
        {
            CheckOpponentDeck("won");
        }

        public readonly Dictionary<DeckType, Style> DeckStyles = new Dictionary<DeckType, Style>
        {
            
            {DeckType.Unknown, Style.Unknown},
            {DeckType.Arena, Style.Control},

            {DeckType.ControlWarrior, Style.Control},
            {DeckType.FatigueWarrior, Style.Control},
            {DeckType.DragonWarrior, Style.Control},
            {DeckType.PatronWarrior, Style.Tempo},
            {DeckType.WorgenOTKWarrior, Style.Combo},
            {DeckType.MechWarrior, Style.Aggro},
            {DeckType.FaceWarrior, Style.Face},

            {DeckType.SecretPaladin, Style.Tempo},
            {DeckType.MidRangePaladin, Style.Control},
            {DeckType.DragonPaladin, Style.Control},
            {DeckType.AggroPaladin, Style.Aggro},
            {DeckType.AnyfinMurglMurgl, Style.Combo},

            {DeckType.RampDruid, Style.Control},
            {DeckType.AggroDruid, Style.Aggro},
            {DeckType.DragonDruid, Style.Control},
            {DeckType.MidRangeDruid, Style.Combo},
            {DeckType.TokenDruid, Style.Tempo},

            {DeckType.Handlock, Style.Control},
            {DeckType.RenoLock, Style.Control},
            {DeckType.Zoolock, Style.Tempo},
            {DeckType.DemonHandlock, Style.Control},
            {DeckType.DemonZooWarlock, Style.Tempo},
            {DeckType.DragonHandlock, Style.Control},
            {DeckType.MalyLock, Style.Control},

            {DeckType.TempoMage, Style.Tempo},
            {DeckType.FreezeMage, Style.Control},
            {DeckType.FaceFreezeMage, Style.Aggro},
            {DeckType.DragonMage, Style.Control},
            {DeckType.MechMage, Style.Aggro},
            {DeckType.EchoMage, Style.Control},
            {DeckType.FatigueMage, Style.Control},

            {DeckType.DragonPriest, Style.Tempo},
            {DeckType.ControlPriest, Style.Control},

            {DeckType.ComboPriest, Style.Combo},
            {DeckType.MechPriest, Style.Aggro},
            {DeckType.ShadowPriest, Style.Combo},

            {DeckType.MidRangeHunter, Style.Tempo},
            {DeckType.HybridHunter, Style.Aggro},
            {DeckType.FaceHunter, Style.Face},
            {DeckType.HatHunter, Style.Control},

            {DeckType.OilRogue, Style.Combo},
            {DeckType.PirateRogue, Style.Aggro},
            {DeckType.FaceRogue, Style.Face},
            {DeckType.MalyRogue, Style.Combo},
            {DeckType.RaptorRogue, Style.Tempo},
            {DeckType.FatigueRogue, Style.Combo},

            {DeckType.FaceShaman, Style.Face},
            {DeckType.MechShaman, Style.Aggro},
            {DeckType.DragonShaman, Style.Control},
            {DeckType.TotemShaman, Style.Tempo},
            {DeckType.MalygosShaman, Style.Combo},
            {DeckType.ControlShaman, Style.Control},
            {DeckType.BloodlustShaman, Style.Combo},

            {DeckType.Basic, Style.Control}
        };
        public DeckData GetDeckInfo(Card.CClass ownClass, List<string> curDeck)
        {
            Bot.Log("I AM HERE PHAGGOT I AM HERE PHAGGOT I AM HERE PHAGGOT I AM HERE PHAGGOT");
            List<Card.Cards> CurrentDeck = curDeck.Select(q => (Card.Cards) Enum.Parse(typeof (Card.Cards), q)).ToList();
            var info = new DeckData { DeckList = CurrentDeck };

            Dictionary<DeckType, int> deckDictionary = new Dictionary<DeckType, int>();

            switch (ownClass)
            {
                #region shaman

                case Card.CClass.SHAMAN:
                    List<Card.Cards> FaceShaman = new List<Card.Cards> { Cards.LightningBolt, Cards.LightningBolt, Cards.UnboundElemental, Cards.UnboundElemental, Cards.EarthShock, Cards.StormforgedAxe, Cards.Doomhammer, Cards.Doomhammer, Cards.FeralSpirit, Cards.FeralSpirit, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.LeperGnome, Cards.LeperGnome, Cards.AbusiveSergeant, Cards.LavaBurst, Cards.LavaBurst, Cards.Crackle, Cards.Crackle, Cards.LavaShock, Cards.LavaShock, Cards.TotemGolem, Cards.TotemGolem, Cards.ArgentHorserider, Cards.ArgentHorserider, Cards.AncestralKnowledge, Cards.AncestralKnowledge, Cards.SirFinleyMrrgglton, Cards.TunnelTrogg, Cards.TunnelTrogg, };
                    List<Card.Cards> MechShaman = new List<Card.Cards> { Cards.LightningBolt, Cards.LightningBolt, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.TunnelTrogg, Cards.TunnelTrogg, Cards.Crackle, Cards.Crackle, Cards.TotemGolem, Cards.TotemGolem, Cards.WhirlingZapomatic, Cards.WhirlingZapomatic, Cards.Powermace, Cards.Powermace, Cards.LavaBurst, Cards.LavaBurst, Cards.UnboundElemental, Cards.UnboundElemental, Cards.Doomhammer, Cards.Doomhammer, Cards.Cogmaster, Cards.Cogmaster, Cards.LeperGnome, Cards.SirFinleyMrrgglton, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.Mechwarper, Cards.Mechwarper, Cards.SpiderTank, Cards.SpiderTank, };
                    List<Card.Cards> DragonShaman = new List<Card.Cards> { Cards.EarthShock, Cards.FeralSpirit, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.Deathwing, Cards.Ysera, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.LightningStorm, Cards.BlackwingTechnician, Cards.BlackwingTechnician, Cards.LavaShock, Cards.BlackwingCorruptor, Cards.TotemGolem, Cards.TotemGolem, Cards.AncestralKnowledge, Cards.HealingWave, Cards.HealingWave, Cards.TheMistcaller, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.Chillmaw, Cards.JeweledScarab, Cards.JeweledScarab, Cards.BrannBronzebeard, Cards.TunnelTrogg, Cards.TunnelTrogg, };
                    List<Card.Cards> TotemShaman = new List<Card.Cards> { Cards.EarthShock, Cards.Bloodlust, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.AlAkirtheWindlord, Cards.FlametongueTotem, Cards.FlametongueTotem, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.DefenderofArgus, Cards.ManaTideTotem, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.LightningStorm, Cards.ZombieChow, Cards.ZombieChow, Cards.TotemGolem, Cards.TotemGolem, Cards.TuskarrTotemic, Cards.TuskarrTotemic, Cards.ThunderBluffValiant, Cards.ThunderBluffValiant, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.HauntedCreeper, Cards.HauntedCreeper, };

                    if(CurrentDeck.Contains(Cards.Malygos))
                     {
                        List<Card.Cards> MalygosShaman = new List<Card.Cards> { Cards.LightningBolt, Cards.LightningBolt, Cards.EarthShock, Cards.EarthShock, Cards.FarSight, Cards.FarSight, Cards.StormforgedAxe, Cards.FeralSpirit, Cards.FeralSpirit, Cards.FrostShock, Cards.FrostShock, Cards.Malygos, Cards.GnomishInventor, Cards.GnomishInventor, Cards.Crackle, Cards.Crackle, Cards.Hex, Cards.Hex, Cards.LavaBurst, Cards.LavaBurst, Cards.ManaTideTotem, Cards.ManaTideTotem, Cards.LightningStorm, Cards.LightningStorm, Cards.AncestorsCall, Cards.AncestorsCall, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.Alexstrasza, Cards.AzureDrake, };
                        deckDictionary.AddOrUpdate(DeckType.MalygosShaman, CurrentDeck.Intersect(MalygosShaman).Count());
                    }

                    List<Card.Cards> ControlShaman = new List<Card.Cards> { Cards.BigGameHunter, Cards.FeralSpirit, Cards.FeralSpirit, Cards.Bloodlust, Cards.Bloodlust, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.FlametongueTotem, Cards.FlametongueTotem, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.DefenderofArgus, Cards.DefenderofArgus, Cards.AbusiveSergeant, Cards.ManaTideTotem, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.LightningStorm, Cards.ZombieChow, Cards.ZombieChow, Cards.NerubianEgg, Cards.NerubianEgg, Cards.Loatheb, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.BrannBronzebeard, };
                    List<Card.Cards> BloodlustShaman = new List<Card.Cards> { Cards.BigGameHunter, Cards.AcidicSwampOoze, Cards.EarthShock, Cards.FeralSpirit, Cards.FeralSpirit, Cards.Bloodlust, Cards.Bloodlust, Cards.Hex, Cards.Hex, Cards.AzureDrake, Cards.AzureDrake, Cards.FlametongueTotem, Cards.FlametongueTotem, Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.DefenderofArgus, Cards.FireElemental, Cards.FireElemental, Cards.LightningStorm, Cards.LightningStorm, Cards.ZombieChow, Cards.ZombieChow, Cards.NerubianEgg, Cards.NerubianEgg, Cards.Loatheb, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.PilotedShredder, Cards.TuskarrTotemic, };
                    List<Card.Cards> BasicChaman = new List<Card.Cards> { Cards.RockbiterWeapon, Cards.RockbiterWeapon, Cards.FlametongueTotem, Cards.FlametongueTotem, Cards.Hex, Cards.Hex, Cards.Bloodlust, Cards.FireElemental, Cards.FireElemental, Cards.AcidicSwampOoze, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.MurlocTidehunter, Cards.RazorfenHunter, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.GnomishInventor, Cards.SenjinShieldmasta, Cards.SenjinShieldmasta, Cards.FrostwolfWarlord, Cards.FrostwolfWarlord, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion, Cards.StormwindChampion, };
                    deckDictionary.AddOrUpdate(DeckType.FaceShaman, CurrentDeck.Intersect(FaceShaman).Count());
                    deckDictionary.AddOrUpdate(DeckType.MechShaman, CurrentDeck.Intersect(MechShaman).Count());
                    deckDictionary.AddOrUpdate(DeckType.DragonShaman, CurrentDeck.Intersect(DragonShaman).Count());
                    deckDictionary.AddOrUpdate(DeckType.TotemShaman, CurrentDeck.Intersect(TotemShaman).Count());
                    
                    deckDictionary.AddOrUpdate(DeckType.ControlShaman, CurrentDeck.Intersect(ControlShaman).Count());
                    deckDictionary.AddOrUpdate(DeckType.BloodlustShaman, CurrentDeck.Intersect(BloodlustShaman).Count());
                    deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(BasicChaman).Count());
                   
                    break;

                #endregion

                #region priest

                case Card.CClass.PRIEST:
                    if (CurrentDeck.Contains(Cards.WyrmrestAgent))
                    {
                        List<Card.Cards> dragonPriest = new List<Card.Cards> { Cards.CabalShadowPriest, Cards.CabalShadowPriest, Cards.AzureDrake, Cards.AzureDrake, Cards.PowerWordShield, Cards.PowerWordShield, Cards.Ysera, Cards.ShadowWordDeath, Cards.ShadowWordDeath, Cards.NorthshireCleric, Cards.NorthshireCleric, Cards.HolyNova, Cards.HolyNova, Cards.VelensChosen, Cards.Shrinkmeister, Cards.Shrinkmeister, Cards.Lightbomb, Cards.BlackwingTechnician, Cards.BlackwingTechnician, Cards.RendBlackhand, Cards.BlackwingCorruptor, Cards.BlackwingCorruptor, Cards.TwilightWhelp, Cards.TwilightWhelp, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.Chillmaw, Cards.WyrmrestAgent, Cards.WyrmrestAgent, Cards.BrannBronzebeard, };
                        deckDictionary.AddOrUpdate(DeckType.DragonPriest, CurrentDeck.Intersect(dragonPriest).Count());
                    }
                    List<Card.Cards> ContrlPriest = new List<Card.Cards> { Cards.WildPyromancer, Cards.WildPyromancer, Cards.CircleofHealing, Cards.CircleofHealing, Cards.Thoughtsteal, Cards.CabalShadowPriest, Cards.CabalShadowPriest, Cards.InjuredBlademaster, Cards.InjuredBlademaster, Cards.PowerWordShield, Cards.PowerWordShield, Cards.ShadowWordDeath, Cards.NorthshireCleric, Cards.NorthshireCleric, Cards.AuchenaiSoulpriest, Cards.AuchenaiSoulpriest, Cards.HolyNova, Cards.ZombieChow, Cards.ZombieChow, Cards.Deathlord, Cards.Deathlord, Cards.LightoftheNaaru, Cards.LightoftheNaaru, Cards.Lightbomb, Cards.Lightbomb, Cards.JusticarTrueheart, Cards.EliseStarseeker, Cards.Entomb, Cards.Entomb, Cards.MuseumCurator, };
                    deckDictionary.AddOrUpdate(DeckType.ControlPriest, CurrentDeck.Intersect(ContrlPriest).Count());
                    if (CurrentDeck.Any(c => c == Cards.InnerFire || c == Cards.ProphetVelen))
                    {
                        List<Card.Cards> ComboPriest = new List<Card.Cards> { Cards.WildPyromancer, Cards.WildPyromancer, Cards.ProphetVelen, Cards.Malygos, Cards.AzureDrake, Cards.ShadowWordPain, Cards.LootHoarder, Cards.LootHoarder, Cards.HolySmite, Cards.HolySmite, Cards.MindBlast, Cards.MindBlast, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.PowerWordShield, Cards.PowerWordShield, Cards.HolyFire, Cards.HolyFire, Cards.BloodmageThalnos, Cards.ShadowWordDeath, Cards.NorthshireCleric, Cards.NorthshireCleric, Cards.HarrisonJones, Cards.HolyNova, Cards.HolyNova, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.VelensChosen, Cards.VelensChosen, Cards.EmperorThaurissan, };
                        deckDictionary.AddOrUpdate(DeckType.ComboPriest, CurrentDeck.Intersect(ComboPriest).Count());
                    }
                    List<Card.Cards> MechPriest = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.CabalShadowPriest, Cards.CabalShadowPriest, Cards.ShadowWordPain, Cards.PowerWordShield, Cards.PowerWordShield, Cards.ShadowMadness, Cards.CairneBloodhoof, Cards.NorthshireCleric, Cards.NorthshireCleric, Cards.HolyNova, Cards.Shrinkmeister, Cards.Shrinkmeister, Cards.VelensChosen, Cards.VelensChosen, Cards.DarkCultist, Cards.DarkCultist, Cards.UpgradedRepairBot, Cards.UpgradedRepairBot, Cards.Voljin, Cards.Mechwarper, Cards.Mechwarper, Cards.SpiderTank, Cards.SpiderTank, Cards.MechanicalYeti, Cards.MechanicalYeti, Cards.PilotedShredder, Cards.PilotedShredder, Cards.Loatheb, Cards.TroggzortheEarthinator, };
                    deckDictionary.AddOrUpdate(DeckType.MechPriest, CurrentDeck.Intersect(MechPriest).Count());
                    if (CurrentDeck.Contains(Cards.Shadowform))
                    {
                        List<Card.Cards> ShadowPriest = new List<Card.Cards> { Cards.WildPyromancer, Cards.WildPyromancer, Cards.Thoughtsteal, Cards.CabalShadowPriest, Cards.CabalShadowPriest, Cards.ProphetVelen, Cards.Alexstrasza, Cards.SenjinShieldmasta, Cards.SenjinShieldmasta, Cards.HolySmite, Cards.MindBlast, Cards.MindBlast, Cards.Shadowform, Cards.Shadowform, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.PowerWordShield, Cards.PowerWordShield, Cards.ShadowMadness, Cards.HolyFire, Cards.ShadowWordDeath, Cards.HolyNova, Cards.ZombieChow, Cards.Deathlord, Cards.Deathlord, Cards.Voljin, Cards.Lightbomb, Cards.Lightbomb, Cards.EmperorThaurissan, Cards.Entomb, };
                        deckDictionary.AddOrUpdate(DeckType.ShadowPriest, CurrentDeck.Intersect(ShadowPriest).Count());
                    }
                    List<Card.Cards> BasicPriest = new List<Card.Cards> { Cards.HolySmite, Cards.HolySmite, Cards.PowerWordShield, Cards.PowerWordShield, Cards.NorthshireCleric, Cards.NorthshireCleric, Cards.DivineSpirit, Cards.ShadowWordPain, Cards.ShadowWordPain, Cards.ShadowWordDeath, Cards.HolyNova, Cards.HolyNova, Cards.MindControl, Cards.VoodooDoctor, Cards.AcidicSwampOoze, Cards.RiverCrocolisk, Cards.RiverCrocolisk, Cards.IronfurGrizzly, Cards.IronfurGrizzly, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.DarkscaleHealer, Cards.DarkscaleHealer, Cards.GurubashiBerserker, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion, };
                    deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(BasicPriest).Count());
                    
                    break;

                #endregion

                #region mage

                case Card.CClass.MAGE:
                    List<Card.Cards> tempoMage = new List<Card.Cards> { Cards.SorcerersApprentice, Cards.SorcerersApprentice, Cards.MirrorImage, Cards.MirrorImage, Cards.Frostbolt, Cards.Frostbolt, Cards.ArchmageAntonidas, Cards.ManaWyrm, Cards.ManaWyrm, Cards.WaterElemental, Cards.WaterElemental, Cards.MindControlTech, Cards.ArcaneIntellect, Cards.ArcaneIntellect, Cards.Fireball, Cards.Fireball, Cards.Counterspell, Cards.MirrorEntity, Cards.ArcaneMissiles, Cards.ArcaneMissiles, Cards.MadScientist, Cards.MadScientist, Cards.UnstablePortal, Cards.UnstablePortal, Cards.Flamecannon, Cards.Flamecannon, Cards.Flamewaker, Cards.Flamewaker, Cards.EtherealConjurer, Cards.EtherealConjurer, };
                    List<Card.Cards> freeze = new List<Card.Cards> { Cards.IceBlock, Cards.IceBlock, Cards.Flamestrike, Cards.FrostNova, Cards.FrostNova, Cards.Frostbolt, Cards.Frostbolt, Cards.IceLance, Cards.IceLance, Cards.ArchmageAntonidas, Cards.Blizzard, Cards.Blizzard, Cards.Alexstrasza, Cards.LootHoarder, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.Doomsayer, Cards.Doomsayer, Cards.ArcaneIntellect, Cards.ArcaneIntellect, Cards.Pyroblast, Cards.Fireball, Cards.Fireball, Cards.BloodmageThalnos, Cards.IceBarrier, Cards.IceBarrier, Cards.MadScientist, Cards.MadScientist, Cards.AntiqueHealbot, Cards.EmperorThaurissan, };
                    List<Card.Cards> faceFreeze = new List<Card.Cards> { Cards.SorcerersApprentice, Cards.SorcerersApprentice, Cards.IceBlock, Cards.IceBlock, Cards.MirrorImage, Cards.FrostNova, Cards.FrostNova, Cards.Frostbolt, Cards.Frostbolt, Cards.IceLance, Cards.IceLance, Cards.ManaWyrm, Cards.ManaWyrm, Cards.AzureDrake, Cards.AzureDrake, Cards.LootHoarder, Cards.LootHoarder, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.ArcaneIntellect, Cards.ArcaneIntellect, Cards.Fireball, Cards.Fireball, Cards.BloodmageThalnos, Cards.MadScientist, Cards.MadScientist, Cards.PilotedShredder, Cards.PilotedShredder, Cards.ForgottenTorch, Cards.ForgottenTorch, };
                    List<Card.Cards> dragonMage = new List<Card.Cards> { Cards.Frostbolt, Cards.Frostbolt, Cards.Duplicate, Cards.IceBarrier, Cards.IceBlock, Cards.Polymorph, Cards.Polymorph, Cards.Flamestrike, Cards.Flamestrike, Cards.ZombieChow, Cards.MadScientist, Cards.MadScientist, Cards.BigGameHunter, Cards.BlackwingTechnician, Cards.BlackwingTechnician, Cards.TwilightDrake, Cards.TwilightGuardian, Cards.TwilightGuardian, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.AzureDrake, Cards.AzureDrake, Cards.BlackwingCorruptor, Cards.BlackwingCorruptor, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.EmperorThaurissan, Cards.DrBoom, Cards.Alexstrasza, Cards.Ysera, };
                    List<Card.Cards> mechMage = new List<Card.Cards> { Cards.Frostbolt, Cards.Frostbolt, Cards.ArchmageAntonidas, Cards.ManaWyrm, Cards.ManaWyrm, Cards.Fireball, Cards.Fireball, Cards.UnstablePortal, Cards.UnstablePortal, Cards.Cogmaster, Cards.Cogmaster, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.DrBoom, Cards.SpiderTank, Cards.SpiderTank, Cards.Mechwarper, Cards.Mechwarper, Cards.PilotedShredder, Cards.PilotedShredder, Cards.GoblinBlastmage, Cards.GoblinBlastmage, Cards.ClockworkGnome, Cards.TinkertownTechnician, Cards.Snowchugger, Cards.Snowchugger, Cards.ClockworkKnight, Cards.ClockworkKnight, Cards.GorillabotA3, Cards.GorillabotA3, };
                    List<Card.Cards> grinderMage = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.Flamestrike, Cards.Flamestrike, Cards.BigGameHunter, Cards.Frostbolt, Cards.Frostbolt, Cards.MindControlTech, Cards.MirrorEntity, Cards.Polymorph, Cards.ZombieChow, Cards.ZombieChow, Cards.Duplicate, Cards.MadScientist, Cards.MadScientist, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.ExplosiveSheep, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.EchoofMedivh, Cards.EmperorThaurissan, Cards.RefreshmentVendor, Cards.JeweledScarab, Cards.JeweledScarab, Cards.BrannBronzebeard, Cards.EtherealConjurer, Cards.EtherealConjurer, };
                    List<Card.Cards> basicMage = new List<Card.Cards> { Cards.Frostbolt, Cards.Frostbolt, Cards.ArcaneIntellect, Cards.ArcaneIntellect, Cards.Fireball, Cards.Fireball, Cards.Polymorph, Cards.Polymorph, Cards.WaterElemental, Cards.WaterElemental, Cards.Flamestrike, Cards.Flamestrike, Cards.AcidicSwampOoze, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.KoboldGeomancer, Cards.RazorfenHunter, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.GnomishInventor, Cards.SenjinShieldmasta, Cards.GurubashiBerserker, Cards.Archmage, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion, };
                    deckDictionary.AddOrUpdate(DeckType.TempoMage, CurrentDeck.Intersect(tempoMage).Count());
                    deckDictionary.AddOrUpdate(DeckType.FreezeMage, CurrentDeck.Intersect(freeze).Count());
                    deckDictionary.AddOrUpdate(DeckType.FaceFreezeMage, CurrentDeck.Intersect(faceFreeze).Count());
                    deckDictionary.AddOrUpdate(DeckType.DragonMage, CurrentDeck.Intersect(dragonMage).Count());
                    deckDictionary.AddOrUpdate(DeckType.MechMage, CurrentDeck.Intersect(mechMage).Count());
                    deckDictionary.AddOrUpdate(DeckType.FatigueMage, CurrentDeck.Intersect(grinderMage).Count());
                    deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(basicMage).Count());
                    break;

                #endregion

                #region paladin

                case Card.CClass.PALADIN:
                    List<Card.Cards> SecretPaladin = new List<Card.Cards> { Cards.AldorPeacekeeper, Cards.BlessingofKings, Cards.NobleSacrifice, Cards.NobleSacrifice, Cards.Consecration, Cards.Consecration, Cards.TruesilverChampion, Cards.TruesilverChampion, Cards.TirionFordring, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.LayonHands, Cards.Avenge, Cards.Avenge, Cards.Loatheb, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.MusterforBattle, Cards.MusterforBattle, Cards.Coghammer, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.Quartermaster, Cards.CompetitiveSpirit, Cards.MysteriousChallenger, };
                    List<Card.Cards> midrangePaladin = new List<Card.Cards> { Cards.AldorPeacekeeper, Cards.AldorPeacekeeper, Cards.BigGameHunter, Cards.Consecration, Cards.Consecration, Cards.TruesilverChampion, Cards.TruesilverChampion, Cards.Equality, Cards.TirionFordring, Cards.KnifeJuggler, Cards.LayonHands, Cards.DefenderofArgus, Cards.ZombieChow, Cards.ZombieChow, Cards.Loatheb, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.MusterforBattle, Cards.MusterforBattle, Cards.Coghammer, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.Quartermaster, Cards.Quartermaster, Cards.JusticarTrueheart, Cards.MurlocKnight, Cards.KeeperofUldaman, };
                    List<Card.Cards> dragonPaladin = new List<Card.Cards> { Cards.AldorPeacekeeper, Cards.AldorPeacekeeper, Cards.BigGameHunter, Cards.Consecration, Cards.Consecration, Cards.TruesilverChampion, Cards.TruesilverChampion, Cards.Alexstrasza, Cards.Equality, Cards.Ysera, Cards.IronbeakOwl, Cards.ZombieChow, Cards.ZombieChow, Cards.SludgeBelcher, Cards.MusterforBattle, Cards.MusterforBattle, Cards.AntiqueHealbot, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.HungryDragon, Cards.HungryDragon, Cards.BlackwingTechnician, Cards.BlackwingTechnician, Cards.BlackwingCorruptor, Cards.VolcanicDrake, Cards.Chromaggus, Cards.DragonConsort, Cards.DragonConsort, Cards.SolemnVigil, Cards.EmperorThaurissan, };
                    List<Card.Cards> AggroPaladin = new List<Card.Cards> { Cards.BlessingofMight, Cards.BlessingofMight, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.Coghammer, Cards.DivineFavor, Cards.DivineFavor, Cards.MusterforBattle, Cards.MusterforBattle, Cards.TruesilverChampion, Cards.BlessingofKings, Cards.BlessingofKings, Cards.KeeperofUldaman, Cards.KeeperofUldaman, Cards.AbusiveSergeant, Cards.AbusiveSergeant, Cards.ArgentSquire, Cards.ArgentSquire, Cards.LeperGnome, Cards.LeperGnome, Cards.SirFinleyMrrgglton, Cards.HauntedCreeper, Cards.HauntedCreeper, Cards.IronbeakOwl, Cards.KnifeJuggler, Cards.KnifeJuggler, Cards.ArcaneGolem, Cards.PilotedShredder, Cards.LeeroyJenkins, Cards.Loatheb, };
                    if (CurrentDeck.Contains(Cards.AnyfinCanHappen))
                    {
                        List<Card.Cards> AnyfinPaladin = new List<Card.Cards> { Cards.AldorPeacekeeper, Cards.AldorPeacekeeper, Cards.CultMaster, Cards.OldMurkEye, Cards.MurlocWarleader, Cards.MurlocWarleader, Cards.Consecration, Cards.Consecration, Cards.BluegillWarrior, Cards.BluegillWarrior, Cards.TruesilverChampion, Cards.KnifeJuggler, Cards.LayonHands, Cards.GrimscaleOracle, Cards.GrimscaleOracle, Cards.ZombieChow, Cards.SludgeBelcher, Cards.DrBoom, Cards.PilotedShredder, Cards.PilotedShredder, Cards.MusterforBattle, Cards.MusterforBattle, Cards.AntiqueHealbot, Cards.AntiqueHealbot, Cards.Coghammer, Cards.ShieldedMinibot, Cards.ShieldedMinibot, Cards.SolemnVigil, Cards.AnyfinCanHappen, Cards.KeeperofUldaman, };
                        deckDictionary.AddOrUpdate(DeckType.AnyfinMurglMurgl, CurrentDeck.Intersect(AnyfinPaladin).Count());
                    }
                    List<Card.Cards> basicPaladin = new List<Card.Cards> { Cards.TruesilverChampion, Cards.TruesilverChampion, Cards.BlessingofKings, Cards.BlessingofKings, Cards.Consecration, Cards.Consecration, Cards.HammerofWrath, Cards.HammerofWrath, Cards.GuardianofKings, Cards.GuardianofKings, Cards.AcidicSwampOoze, Cards.AcidicSwampOoze, Cards.BloodfenRaptor, Cards.BloodfenRaptor, Cards.MurlocTidehunter, Cards.MurlocTidehunter, Cards.RiverCrocolisk, Cards.RiverCrocolisk, Cards.RazorfenHunter, Cards.RazorfenHunter, Cards.ShatteredSunCleric, Cards.ShatteredSunCleric, Cards.ChillwindYeti, Cards.ChillwindYeti, Cards.FrostwolfWarlord, Cards.FrostwolfWarlord, Cards.BoulderfistOgre, Cards.BoulderfistOgre, Cards.StormwindChampion, Cards.StormwindChampion, };
                    deckDictionary.AddOrUpdate(DeckType.SecretPaladin, CurrentDeck.Intersect(SecretPaladin).Count());
                    deckDictionary.AddOrUpdate(DeckType.MidRangePaladin, CurrentDeck.Intersect(midrangePaladin).Count());
                    deckDictionary.AddOrUpdate(DeckType.DragonPaladin, CurrentDeck.Intersect(dragonPaladin).Count());
                    deckDictionary.AddOrUpdate(DeckType.AggroPaladin, CurrentDeck.Intersect(AggroPaladin).Count());
                    
                    deckDictionary.AddOrUpdate(DeckType.Basic, CurrentDeck.Intersect(basicPaladin).Count());
                    break;

                #endregion

                #region warrior

                case Card.CClass.WARRIOR:
                    List<Card.Cards> controlWarriorCards = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.ShieldSlam, Cards.ShieldSlam, Cards.BigGameHunter, Cards.Slam, Cards.Slam, Cards.Execute, Cards.Execute, Cards.Brawl, Cards.Brawl, Cards.Deathwing, Cards.ShieldBlock, Cards.ShieldBlock, Cards.BaronGeddon, Cards.HarrisonJones, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.Armorsmith, Cards.DeathsBite, Cards.DeathsBite, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.Shieldmaiden, Cards.Shieldmaiden, Cards.Revenge, Cards.Revenge, Cards.JusticarTrueheart, Cards.Bash, Cards.JeweledScarab, Cards.JeweledScarab, };
                   
                    if (CurrentDeck.Contains(Cards.Deathlord))
                    {
                        List<Card.Cards> fatigueWarrior = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.ShieldSlam, Cards.ShieldSlam, Cards.BigGameHunter, Cards.Gorehowl, Cards.Slam, Cards.Slam, Cards.Execute, Cards.Execute, Cards.Brawl, Cards.Brawl, Cards.BaronGeddon, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.Revenge, Cards.Bash, Cards.Bash, Cards.BouncingBlade, Cards.DeathsBite, Cards.DeathsBite, Cards.Shieldmaiden, Cards.Shieldmaiden, Cards.Deathlord, Cards.Deathlord, Cards.JusticarTrueheart, Cards.SludgeBelcher, Cards.SludgeBelcher, Cards.UnstableGhoul, Cards.RenoJackson, };
                        deckDictionary.AddOrUpdate(DeckType.FatigueWarrior, CurrentDeck.Intersect(fatigueWarrior).Count());
                    }
                    List<Card.Cards> dragonWarrior = new List<Card.Cards> { Cards.SylvanasWindrunner, Cards.ShieldSlam, Cards.ShieldSlam, Cards.BigGameHunter, Cards.Execute, Cards.Execute, Cards.Brawl, Cards.Alexstrasza, Cards.Deathwing, Cards.Ysera, Cards.BaronGeddon, Cards.HarrisonJones, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.DeathsBite, Cards.DeathsBite, Cards.Shieldmaiden, Cards.EmperorThaurissan, Cards.Nefarian, Cards.Revenge, Cards.Revenge, Cards.BlackwingCorruptor, Cards.BlackwingCorruptor, Cards.JusticarTrueheart, Cards.Chillmaw, Cards.Bash, Cards.Bash, Cards.TwilightGuardian, Cards.TwilightGuardian, };
                    List<Card.Cards> patronWarrior = new List<Card.Cards> { Cards.FrothingBerserker, Cards.KorkronElite, Cards.Whirlwind, Cards.Whirlwind, Cards.Slam, Cards.Slam, Cards.Execute, Cards.Execute, Cards.DreadCorsair, Cards.DreadCorsair, Cards.InnerRage, Cards.InnerRage, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.GrommashHellscream, Cards.Armorsmith, Cards.Armorsmith, Cards.BattleRage, Cards.BattleRage, Cards.DeathsBite, Cards.DeathsBite, Cards.Loatheb, Cards.SludgeBelcher, Cards.UnstableGhoul, Cards.DrBoom, Cards.GrimPatron, Cards.GrimPatron, Cards.ArchThiefRafaam, };
                    List<Card.Cards> worgen = new List<Card.Cards> { Cards.ShieldSlam, Cards.RagingWorgen, Cards.RagingWorgen, Cards.Whirlwind, Cards.Execute, Cards.Execute, Cards.GnomishInventor, Cards.GnomishInventor, Cards.Brawl, Cards.Brawl, Cards.CruelTaskmaster, Cards.CruelTaskmaster, Cards.InnerRage, Cards.InnerRage, Cards.AcolyteofPain, Cards.AcolyteofPain, Cards.NoviceEngineer, Cards.NoviceEngineer, Cards.Rampage, Cards.Rampage, Cards.ShieldBlock, Cards.ShieldBlock, Cards.IronbeakOwl, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.Charge, Cards.Charge, Cards.DeathsBite, Cards.DeathsBite, Cards.AntiqueHealbot, };
                    List<Card.Cards> mechWar = new List<Card.Cards> { Cards.HeroicStrike, Cards.HeroicStrike, Cards.KorkronElite, Cards.KorkronElite, Cards.ArcaniteReaper, Cards.ArcaniteReaper, Cards.MortalStrike, Cards.MortalStrike, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.DeathsBite, Cards.DeathsBite, Cards.Cogmaster, Cards.Cogmaster, Cards.AnnoyoTron, Cards.AnnoyoTron, Cards.SpiderTank, Cards.SpiderTank, Cards.Mechwarper, Cards.Mechwarper, Cards.PilotedShredder, Cards.PilotedShredder, Cards.TinkertownTechnician, Cards.ScrewjankClunker, Cards.ScrewjankClunker, Cards.Warbot, Cards.Warbot, Cards.FelReaver, Cards.FelReaver, Cards.ClockworkKnight, };
                    List<Card.Cards> faceWar = new List<Card.Cards> { Cards.HeroicStrike, Cards.HeroicStrike, Cards.ArcaneGolem, Cards.ArcaneGolem, Cards.SouthseaDeckhand, Cards.SouthseaDeckhand, Cards.KorkronElite, Cards.KorkronElite, Cards.Wolfrider, Cards.DreadCorsair, Cards.DreadCorsair, Cards.MortalStrike, Cards.MortalStrike, Cards.IronbeakOwl, Cards.LeperGnome, Cards.LeperGnome, Cards.FieryWarAxe, Cards.FieryWarAxe, Cards.BloodsailRaider, Cards.BloodsailRaider, Cards.Upgrade, Cards.Upgrade, Cards.DeathsBite, Cards.DeathsBite, Cards.ArgentHorserider, Cards.ArgentHorserider, Cards.Bash, Cards.Bash, Cards.SirFinleyMrrgglton, Cards.CursedBlade, };
                    deckDictionary.AddOrUpdate(DeckType.ControlWarrior, CurrentDeck.Intersect(controlWarriorCards).Count());
                    deckDictionary.AddOrUpdate(DeckType.DragonWarrior, CurrentDeck.Intersect(dragonWarrior).Count());
                    deckDictionary.AddOrUpdate(DeckType.PatronWarrior, CurrentDeck.Intersect(patronWarrior).Count());
                    deckDictionary.AddOrUpdate(DeckType.WorgenOTKWarrior, CurrentDeck.Intersect(worgen).Count());
                    deckDictionary.AddOrUpdate(DeckType.MechWarrior, CurrentDeck.Intersect(mechWar).Count());
                    deckDictionary.AddOrUpdate(DeckType.FaceWarrior, CurrentDeck.Intersect(faceWar).Count());
                    break;

                #endregion

                #region warlock

                case Card.CClass.WARLOCK:
                   
                    break;

                #endregion

                #region hunter

                case Card.CClass.HUNTER:
                   
                    break;

                #endregion

                #region rogue

                case Card.CClass.ROGUE:
                    
                    break;

                #endregion

                #region druid

                case Card.CClass.DRUID:
                   
                    break;

                    #endregion
            }
            try
            {
                var BestDeck = deckDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                info.DeckType = BestDeck;
                info.DeckStyle = DeckStyles[BestDeck];
                Bot.Log(String.Format("{0}|||{1}", info.DeckType, info.DeckStyle));
            }
            catch (Exception e)
            {
                Bot.Log(" Me? " + e.Message);
            }
            Bot.Log(info.DeckType +"||"+ info.DeckStyle);
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
        public List<Card.Cards> DeckList { get; set; }
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

    public enum IdentityMode
    {
        Auto,
        Manual
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
