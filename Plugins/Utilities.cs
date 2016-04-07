using SmartBot.Plugins.API;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace SmartBot.Plugins
{
	[Serializable]
    public class bPluginDataContainer : PluginDataContainer
    {        
        public string HearthTrackerPath { get; set; }
        public string MisplayReporterPath { get; set; }
        public bool HearthTrackerActivated { get; set; }      
        public bool Logger { get; set; } 
        public bool ResetStatsAtMidnight { get; set; }
        public bool KillBattleNetClient { get; set; }
        public bool PlaySoundOnFriendRequest { get; set; }
        public bool SuspendBotOnHighLatency { get; set; }
        public int SuspendMaxAverageLatency { get; set; }
        public bool SuspendBotFullyOnHighLatency { get; set; }
	    

	    //Init vars
        public bPluginDataContainer()
		{
			Name = "Utilities";         
		}
    }

    public class bPlugin : Plugin
    {        
        private bool _useHearthTracker = true;
        private string _hearthTrackerPath;
        private bool _resetStatsAtMidnight = true;
        private bool _killBNet = true;
        private bool _gameStarted = false;
        private bool _logger = true;
        private bool _playSoundOnFriendRequest = false;
        private int _startingDay;
        private DateTime _lastTick = DateTime.MinValue;
        private List<FriendRequest> _friendRequests = new List<FriendRequest>();
        private bool _suspendBotHighLatency = false;
        private int _maxLatency = 1000;
        private bool _finishGame = false;
        private bool _fullySuspend = false;
        private bool _arenaActive = false;
        private int _totalWins = 0;
        private int _totalLosses = 0;
        private Bot.Mode _currentMode;
        private DateTime _arenaBegan = DateTime.MinValue;                

		//Constructor
		public override void OnPluginCreated()
		{
		}
        		
		//Bot tick event
        public override void OnTick()
        {
            //Bot.Log("[PLUGIN] -> OnTick");

            
            if (_lastTick.AddMilliseconds(10000) > DateTime.Now) return;
            _lastTick = DateTime.Now;

            if (_playSoundOnFriendRequest)
            {
                foreach (FriendRequest request in Bot.GetFriendRequests())
                {
                    if (!_friendRequests.Contains(request))
                    {
                        _friendRequests.Add(request);
                        System.Media.SystemSounds.Exclamation.Play();
                    }
                }
            }

            if (_suspendBotHighLatency)
            {
                if (Bot.GetAverageLatency() > _maxLatency)
                {
                    FinishGame();
                    Bot.SetHoverRoutineEnabled(false);
                    Bot.SetThinkingRoutineEnabled(false);
                }
                else
                {
                    ResumeGame();
                    Bot.SetHoverRoutineEnabled(true);
                    Bot.SetThinkingRoutineEnabled(true);
                }
            }            
        }
		
		//Bot starting event
        public override void OnStarted()
        {
            //Bot.Log("[PLUGIN] -> OnStarted");
			
			if(!DataContainer.Enabled)
				return;
			
			Init();
        }

		//Bot stopping event
        public override void OnStopped()
        {
            //Bot.Log("[PLUGIN] -> OnStopped");
        }

		//Turn begin event
        public override void OnTurnBegin()
        {            
			//Bot.Log("[PLUGIN] -> OnTurnBegin");
        }

		//Turn end event
        public override void OnTurnEnd()
        {            
			//Bot.Log("[PLUGIN] -> OnTurnEnd");
        }

		//Simulation event (AI calculation)
        public override void OnSimulation()
        {
			//Bot.Log("[PLUGIN] -> OnSimulation");
        }

		//Match begin event
        public override void OnGameBegin()
        {
			//Bot.Log("[PLUGIN] -> OnGameBegin");            
            if (_killBNet)
            {
                try
                {
                    Process[] proc = Process.GetProcessesByName("Battle.net");
                    proc[0].Kill();
                    Bot.Log("[PLUGIN] -> Utilities : Killed Battle.Net client for more reliable relogging ...");
                }
                catch { }
            }

            _gameStarted = true;            

            if ((Bot.CurrentMode() == Bot.Mode.Arena || Bot.CurrentMode() == Bot.Mode.ArenaAuto) && !_arenaActive)            
            {
                _arenaActive = true;
                _totalWins = Statistics.Wins;
                _totalLosses = Statistics.Losses;                
                _arenaBegan = DateTime.Now;

                if (Bot.CurrentBoard.HeroFriend != null)
                {
                    Bot.Log("[PLUGIN] -> Utilities -> New arena begins, Hero: " + Bot.CurrentBoard.HeroFriend);                    
                }
                else
                {
                    Bot.Log("[PLUGIN] -> Utilities -> New arena begins!");
                }
            }
        }

		//Match end event
        public override void OnGameEnd()
        {
			//Bot.Log("[PLUGIN] -> OnGameEnd");            
			if(!DataContainer.Enabled)
				return;

            double winRatio = ((double)SmartBot.Plugins.API.Statistics.Wins / (double)(SmartBot.Plugins.API.Statistics.Wins + SmartBot.Plugins.API.Statistics.Losses)) * 100;
            winRatio = Math.Round(winRatio, 2);

            double winsPerHour = (double)SmartBot.Plugins.API.Statistics.Wins / (double)SmartBot.Plugins.API.Statistics.ElapsedTime.TotalHours;
            winsPerHour = Math.Round(winsPerHour, 2);

            if (_gameStarted)
                Bot.Log("[PLUGIN] -> Utilities -> Won: " + SmartBot.Plugins.API.Statistics.Wins + ", Lost: " + SmartBot.Plugins.API.Statistics.Losses + ", Win Ratio: " + winRatio + "%, Wins per hour: " + winsPerHour + ", Elapsed Time: " + SmartBot.Plugins.API.Statistics.ElapsedTime);

            if (_arenaActive && _gameStarted)            
                LogArenaStats("Arena stats");                            

            ResetStats();

            if (_finishGame)
                Bot.SuspendBot();

            _gameStarted = false;
        }

		//gold balance changed event
        public override void OnGoldAmountChanged()
        {
			//Bot.Log("[PLUGIN] -> OnGoldAmountChanged");
        }

		//arena 12 wins or 3 losses event
        public override void OnArenaEnd()
        {
            if (_arenaActive)
                LogArenaStats("Arena ends");

            _arenaActive = false;            
			//Bot.Log("[PLUGIN] -> OnArenaEnd");
        }

		//lethal found event (during a game)
        public override void OnLethal()
        {
			//Bot.Log("[PLUGIN] -> OnLethal");
        }

		//all quests completed event
        public override void OnAllQuestsCompleted()
        {
			//Bot.Log("[PLUGIN] -> OnAllQuestsCompleted");
        }

		//concede event
        public override void OnConcede()
        {
			Bot.Log("[PLUGIN] -> OnConcede event fired");
        }

        public override void OnDecklistUpdate()
        {
            //Bot.Log("[PLUGIN] -> OnDecklistUpdate event fired");
        }


        /* --------------- Utilities Methods -------------- */
		        
		private void Init()
		{
			if(!DataContainer.Enabled)
				return;

            _useHearthTracker = ((bPluginDataContainer)DataContainer).HearthTrackerActivated;
            _hearthTrackerPath = ((bPluginDataContainer)DataContainer).HearthTrackerPath;
            _resetStatsAtMidnight = ((bPluginDataContainer)DataContainer).ResetStatsAtMidnight;
            _killBNet = ((bPluginDataContainer)DataContainer).KillBattleNetClient;
            _logger = ((bPluginDataContainer)DataContainer).Logger;
            _playSoundOnFriendRequest = ((bPluginDataContainer)DataContainer).PlaySoundOnFriendRequest;
            _suspendBotHighLatency = ((bPluginDataContainer)DataContainer).SuspendBotOnHighLatency;
            _maxLatency = ((bPluginDataContainer)DataContainer).SuspendMaxAverageLatency;
            _fullySuspend = ((bPluginDataContainer)DataContainer).SuspendBotFullyOnHighLatency;

            _currentMode = Bot.CurrentMode();

            Bot.Log("[PLUGIN] -> Utilities : Initialized...");
            Bot.Log("[PLUGIN] -> Utilities : Use HearthTracker : " + _useHearthTracker.ToString());
            Bot.Log("[PLUGIN] -> Utilities : HearthTracker path : " + _hearthTrackerPath);
            Bot.Log("[PLUGIN] -> Utilities : Reset stats after midnight : " + _resetStatsAtMidnight.ToString());
            Bot.Log("[PLUGIN] -> Utilities : Kill Battle.Net client for reliable relogging : " + _killBNet.ToString());
            Bot.Log("[PLUGIN] -> Utilities : Logging enabled : " + _logger.ToString());
            Bot.Log("[PLUGIN] -> Utilities : Play a sound on friend request : " + _playSoundOnFriendRequest.ToString());
            Bot.Log("[PLUGIN] -> Utilities : Suspend bot on high latency : " + _suspendBotHighLatency.ToString());
            Bot.Log("[PLUGIN] -> Utilities : Max average latency before suspension : " + _maxLatency.ToString());
            Bot.Log("[PLUGIN] -> Utilities : Full suspension on high latency : " + _fullySuspend.ToString());
            Bot.SetLatencySamplingRate(60000);

            _gameStarted = false;
            _finishGame = false;            

            if (_useHearthTracker)
            {
                try 
                {
                    string trackerFileName = Path.GetFileName(_hearthTrackerPath);
                    Process[] proc = Process.GetProcessesByName(trackerFileName.Remove(trackerFileName.Length - 4));
                    if (proc.Length == 0)
                    {
                        Process.Start(_hearthTrackerPath);
                        Bot.Log("[PLUGIN] -> Utilities : Starting HearthTracker ...");
                    }
                }
                catch { }
            }

            _startingDay = DateTime.Now.Day;                        
		}
				
        private void ResetStats()
        {
            int nowDay = DateTime.Now.Day;
            if (_resetStatsAtMidnight && (nowDay - _startingDay != 0))
            {                
                SmartBot.Plugins.API.Statistics.Reset();
                _startingDay = DateTime.Now.Day;
                Bot.Log("[PLUGIN] -> Utilities : Stats resetted at " + DateTime.Now.ToString());
            }
        }

        private void FinishGame()
        {
            if (_fullySuspend)
                Bot.SuspendBot();

            _finishGame = true;
        }

        private void ResumeGame()
        {
            if (_finishGame)
                Bot.ResumeBot();

            _finishGame = false;                        
        }

        private void LogArenaStats(string partOfTheLog)
        {
            try
            {
                int wins = (SmartBot.Plugins.API.Statistics.Wins - _totalWins);
                int losses = (SmartBot.Plugins.API.Statistics.Losses - _totalLosses);
                double winRatio = ((double)wins / (double)(wins + losses)) * 100;
                winRatio = Math.Round(winRatio, 2);
                TimeSpan elapsedTime = DateTime.Now - _arenaBegan;
                double winsPerHour = (double)wins / (double)elapsedTime.TotalHours;
                winsPerHour = Math.Round(winsPerHour, 2);

                string hero = "";
                if (Bot.CurrentBoard.HeroFriend != null)
                    hero = "Hero: " + Bot.CurrentBoard.HeroFriend + ", ";

                Bot.Log("[PLUGIN] -> Utilities -> " + partOfTheLog + " : " + hero + "Won: " + wins + ", Lost: " + losses + ", Win Ratio: " + winRatio + "%, Wins per hour: " + winsPerHour + ", Elapsed Time: " + elapsedTime);
            }
            catch (Exception ex)
            {
                Bot.Log("[PLUGIN] -> Utilities error -> " + ex);
            }
        }        
    }
}
