using SmartBot.Plugins.API;
using System;
using System.Collections.Generic;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.Linq;
using SmartBot.Database;

namespace SmartBot.Plugins
{
	[Serializable]
    public class qPluginDataContainer : PluginDataContainer
    {        
        [ItemsSource(typeof(ProfileStringSource))]
        public string ArenaProfile { get; set; }
        [ItemsSource(typeof(MulliganStringSource))]
        public string ArenaMulligan { get; set; }        
        [ItemsSource(typeof(ProfileStringSource))]
        public string Deck1Profile { get; set; }
        [ItemsSource(typeof(MulliganStringSource))]
        public string Deck1Mulligan { get; set; }
        [ItemsSource(typeof(DeckStringSource))]
        public string Deck1Deck { get; set; }        
        [ItemsSource(typeof(ProfileStringSource))]
        public string Deck2Profile { get; set; }
        [ItemsSource(typeof(MulliganStringSource))]
        public string Deck2Mulligan { get; set; }
        [ItemsSource(typeof(DeckStringSource))]
        public string Deck2Deck { get; set; }        
        [ItemsSource(typeof(ProfileStringSource))]
        public string Deck3Profile { get; set; }
        [ItemsSource(typeof(MulliganStringSource))]
        public string Deck3Mulligan { get; set; }
        [ItemsSource(typeof(DeckStringSource))]
        public string Deck3Deck { get; set; }        
        [ItemsSource(typeof(ProfileStringSource))]
        public string Deck4Profile { get; set; }
        [ItemsSource(typeof(MulliganStringSource))]
        public string Deck4Mulligan { get; set; }
        [ItemsSource(typeof(DeckStringSource))]
        public string Deck4Deck { get; set; }        
        [ItemsSource(typeof(ProfileStringSource))]
        public string Deck5Profile { get; set; }
        [ItemsSource(typeof(MulliganStringSource))]
        public string Deck5Mulligan { get; set; }
        [ItemsSource(typeof(DeckStringSource))]
        public string Deck5Deck { get; set; }
        [ItemsSource(typeof(ProfileStringSource))]
        public string Deck6Profile { get; set; }
        [ItemsSource(typeof(MulliganStringSource))]
        public string Deck6Mulligan { get; set; }
        [ItemsSource(typeof(DeckStringSource))]
        public string Deck6Deck { get; set; }
        [ItemsSource(typeof(ProfileStringSource))]
        public string Deck7Profile { get; set; }
        [ItemsSource(typeof(MulliganStringSource))]
        public string Deck7Mulligan { get; set; }
        [ItemsSource(typeof(DeckStringSource))]
        public string Deck7Deck { get; set; }
        [ItemsSource(typeof(ProfileStringSource))]
        public string Deck8Profile { get; set; }
        [ItemsSource(typeof(MulliganStringSource))]
        public string Deck8Mulligan { get; set; }
        [ItemsSource(typeof(DeckStringSource))]
        public string Deck8Deck { get; set; }
        [ItemsSource(typeof(ProfileStringSource))]
        public string Deck9Profile { get; set; }
        [ItemsSource(typeof(MulliganStringSource))]
        public string Deck9Mulligan { get; set; }
        [ItemsSource(typeof(DeckStringSource))]
        public string Deck9Deck { get; set; }

        //public bool UseAutoArenaProfileSwitcher { get; set; }
		
		//Init vars
        public qPluginDataContainer()
		{
			Name = "DeckSettings";
		}
    }    

    public class bPlugin : Plugin
    {
        private class DeckSetting
        {
            public string Deck;      
            public string Profile;
            public string Mulligan;                  
        }

        private string _arenaProfile;
        private string _arenaMulligan;
        private bool _autoArenaSwitcher = false;

        private DateTime _lastCheckDateTime = DateTime.MinValue;
        private DateTime _lastReadDateTime = DateTime.MinValue;        
        private double _lastAverageCost = 0;
        private const int CHECK_EVERY_X_SECONDS = 5;

        private List<DeckSetting> _deckSettings = new List<DeckSetting>();                       
		
		//Bot tick event
        public override void OnTick()
        {
            if (!DataContainer.Enabled)
                return;

            ReadSettings();
            CheckSettings();            
        }
		
		//Bot starting event
        public override void OnStarted()
        {
            if (!DataContainer.Enabled)
                return;

            ReadSettings();
            CheckSettings();

            _lastCheckDateTime = DateTime.MinValue;
            _lastReadDateTime = DateTime.MinValue;
            _lastAverageCost = 0;

            string deckSettings = "";
            for (int i = 0; i < _deckSettings.Count; i++)
                deckSettings += "Deck " + (i + 1) + ": " + _deckSettings[i].Deck + ", Profile " + (i + 1) + ": " + _deckSettings[i].Profile + ", Mulligan " + (i + 1) + ": " + _deckSettings[i].Mulligan + "; ";

            Bot.Log("[PLUGIN] -> DeckSettings : Arena Profile: " + _arenaProfile + ", Arena Mulligan: " + _arenaMulligan + "; " + deckSettings);            
        }

        private void ReadSettings()
        {
            if (_lastReadDateTime.AddSeconds(CHECK_EVERY_X_SECONDS) > DateTime.Now) return;
            _lastReadDateTime = DateTime.Now;

            _deckSettings.Clear();            

            string pluginProfile = "", pluginMulligan = "", pluginDeck = "";            

            try
            {
                for (int i = 1; i <= 9; i++)
                {
                    DeckSetting setting = new DeckSetting();

                    switch (i)
                    {
                        case 1:                            
                            pluginProfile = ((qPluginDataContainer)DataContainer).Deck1Profile;
                            pluginMulligan = ((qPluginDataContainer)DataContainer).Deck1Mulligan;
                            pluginDeck = ((qPluginDataContainer)DataContainer).Deck1Deck;
                            break;

                        case 2:                            
                            pluginProfile = ((qPluginDataContainer)DataContainer).Deck2Profile;
                            pluginMulligan = ((qPluginDataContainer)DataContainer).Deck2Mulligan;
                            pluginDeck = ((qPluginDataContainer)DataContainer).Deck2Deck;
                            break;

                        case 3:                            
                            pluginProfile = ((qPluginDataContainer)DataContainer).Deck3Profile;
                            pluginMulligan = ((qPluginDataContainer)DataContainer).Deck3Mulligan;
                            pluginDeck = ((qPluginDataContainer)DataContainer).Deck3Deck;
                            break;

                        case 4:                            
                            pluginProfile = ((qPluginDataContainer)DataContainer).Deck4Profile;
                            pluginMulligan = ((qPluginDataContainer)DataContainer).Deck4Mulligan;
                            pluginDeck = ((qPluginDataContainer)DataContainer).Deck4Deck;
                            break;

                        case 5:                            
                            pluginProfile = ((qPluginDataContainer)DataContainer).Deck5Profile;
                            pluginMulligan = ((qPluginDataContainer)DataContainer).Deck5Mulligan;
                            pluginDeck = ((qPluginDataContainer)DataContainer).Deck5Deck;
                            break;

                        case 6:
                            pluginProfile = ((qPluginDataContainer)DataContainer).Deck6Profile;
                            pluginMulligan = ((qPluginDataContainer)DataContainer).Deck6Mulligan;
                            pluginDeck = ((qPluginDataContainer)DataContainer).Deck6Deck;
                            break;

                        case 7:
                            pluginProfile = ((qPluginDataContainer)DataContainer).Deck7Profile;
                            pluginMulligan = ((qPluginDataContainer)DataContainer).Deck7Mulligan;
                            pluginDeck = ((qPluginDataContainer)DataContainer).Deck7Deck;
                            break;

                        case 8:
                            pluginProfile = ((qPluginDataContainer)DataContainer).Deck8Profile;
                            pluginMulligan = ((qPluginDataContainer)DataContainer).Deck8Mulligan;
                            pluginDeck = ((qPluginDataContainer)DataContainer).Deck8Deck;
                            break;

                        case 9:
                            pluginProfile = ((qPluginDataContainer)DataContainer).Deck9Profile;
                            pluginMulligan = ((qPluginDataContainer)DataContainer).Deck9Mulligan;
                            pluginDeck = ((qPluginDataContainer)DataContainer).Deck9Deck;
                            break;
                    }

                    if (string.IsNullOrWhiteSpace(pluginDeck) || string.IsNullOrWhiteSpace(pluginProfile) || string.IsNullOrWhiteSpace(pluginMulligan))
                        continue;

                    setting.Deck = pluginDeck;
                    setting.Profile = pluginProfile;
                    setting.Mulligan = pluginMulligan;

                    _deckSettings.Add(setting);                    
                }

                _arenaProfile = ((qPluginDataContainer)DataContainer).ArenaProfile;
                _arenaMulligan = ((qPluginDataContainer)DataContainer).ArenaMulligan;
                //_autoArenaSwitcher = ((qPluginDataContainer)DataContainer).UseAutoArenaProfileSwitcher;                
            }
            catch (Exception ex)
            {
                Bot.Log("[PLUGIN] -> DeckSettings : Error: " + ex.Message);
            }
        }

        private void CheckSettings()
        {          
            if (_lastCheckDateTime.AddSeconds(CHECK_EVERY_X_SECONDS) > DateTime.Now) return;
            _lastCheckDateTime = DateTime.Now;

            if (Bot.CurrentMode() == Bot.Mode.Arena || Bot.CurrentMode() == Bot.Mode.ArenaAuto)
            {
                if (_autoArenaSwitcher)
                {                    
                    if (Bot.CurrentDeck().Cards.Count <= 0) return;

                    double averageDeckCost = Bot.CurrentDeck().Cards.Average(x => CardTemplate.LoadFromId(x).Cost);

                    if (_lastAverageCost != averageDeckCost)
                    {
                        _lastAverageCost = averageDeckCost;

                        string newProfile = (averageDeckCost < 3.5) ? "Rush" : "Default";
                        
                        Bot.Log("[PLUGIN] -> DeckSettings : Switching to Arena Profile: " + newProfile + ", average deck cost: " + averageDeckCost);
                        Bot.ChangeProfile(newProfile);
                    }
                }
                else
                {
                    if (Bot.CurrentProfile() != _arenaProfile)
                    {
                        Bot.ChangeProfile(_arenaProfile);
                        Bot.Log("[PLUGIN] -> DeckSettings : Switching to Arena Profile: " + _arenaProfile);
                    }                    
                }

                if (Bot.CurrentMulligan() != _arenaMulligan)
                {
                    Bot.ChangeMulligan(_arenaMulligan);
                    Bot.Log("[PLUGIN] -> DeckSettings : Switching to Arena Mulligan: " + _arenaMulligan);
                }

                return;
            }

            string currentDeck = "";
            if (Bot.CurrentDeck() != null)
            {
                currentDeck = Bot.CurrentDeck().Name;                                    
            }
            else if (Bot.GetSelectedDecks().FirstOrDefault() != null)
            {
                currentDeck = Bot.GetSelectedDecks().FirstOrDefault().Name;
            }
            else
            {
                return;
            }

            foreach (DeckSetting d in _deckSettings)
            {
                if (currentDeck == d.Deck)
                {
                    if (Bot.CurrentProfile() != d.Profile)
                    {
                        Bot.ChangeProfile(d.Profile);
                        Bot.Log("[PLUGIN] -> DeckSettings : Switching to Deck Profile: " + d.Profile);
                    }

                    if (Bot.CurrentMulligan() != d.Mulligan)
                    {
                        Bot.ChangeMulligan(d.Mulligan);
                        Bot.Log("[PLUGIN] -> DeckSettings : Switching to Deck Mulligan: " + d.Mulligan);
                    }
                }
            }
        }
    }
}
