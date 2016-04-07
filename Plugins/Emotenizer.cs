using SmartBot.Plugins.API;
using System;
using System.Collections.Generic;

namespace SmartBot.Plugins
{
	[Serializable]
    public class bPluginDataContainer : PluginDataContainer
    {
        public int OpeningEmoteChance { get; set; }
        public int MaxAnswers { get; set; }
        public int AnsweringChance { get; set; }        
        public int EndingEmoteChance { get; set; }        

		//Init vars
        public bPluginDataContainer()
		{
            Name = "Emotenizer";
            MaxAnswers = 3;
            OpeningEmoteChance = 50;
            EndingEmoteChance = 50;
            AnsweringChance = 50;                        
		}
    }

    public class bPlugin : Plugin
    {        
        private int _round = 1;
        private int _maxAnswers = 3;
        private int _openingEmote = 50;
        private int _endingEmote = 50;
        private int _answering = 50;        
        private bool _openingDone = false;
        private bool _endingDone = false;
        private bool _doOpening = false;
        private bool _doEnding = false;
        private int _emotesSent = 0;
        private int _openingEmoteRound = 1;
        private int _emotesAnswered = 0;        
        private DateTime _lastEmoteSent = DateTime.MinValue;
        private DateTime _lastEmoteReceived = DateTime.MinValue;
        private bool _postedFinalInfo = false;

        private const int BLOCKING_TIME = 15000; // can't send emotes with less than 15 seconds interval between them, HARD LIMIT - DO NOT CHANGE THIS!!!
        private const int TOTAL_EMOTES = 5; // can't send more than 5 emotes per game, HARD LIMIT - DO NOT CHANGE THIS!!!

        private List<KeyValuePair<Bot.EmoteType, DateTime>> _emotesQueue = new List<KeyValuePair<Bot.EmoteType, DateTime>>();
        private List<KeyValuePair<Bot.EmoteType, DateTime>> _emotesSentList = new List<KeyValuePair<Bot.EmoteType, DateTime>>();

		//Constructor
		public override void OnPluginCreated()
		{
		}
		
		//Bot tick event
        public override void OnTick()
        {
            if (!DataContainer.Enabled)
                return;

            DoAllJobs();
            //Bot.Log("[PLUGIN] -> OnTick");
        }
		
		//Bot starting event
        public override void OnStarted()
        {
            if (!DataContainer.Enabled)
                return;
            
            Bot.Log("[PLUGIN] -> Emotenizer : Initialized...");
            Bot.Log("[PLUGIN] -> Emotenizer : Max answers per game : " + ((bPluginDataContainer)DataContainer).MaxAnswers);
            Bot.Log("[PLUGIN] -> Emotenizer : Answering emote chance : " + ((bPluginDataContainer)DataContainer).AnsweringChance + "%");
            Bot.Log("[PLUGIN] -> Emotenizer : Opening emote chance : " + ((bPluginDataContainer)DataContainer).OpeningEmoteChance + "%");
            Bot.Log("[PLUGIN] -> Emotenizer : Ending emote chance : " + ((bPluginDataContainer)DataContainer).EndingEmoteChance + "%");            
        }

		//Bot stopping event
        public override void OnStopped()
        {
            //Bot.Log("[PLUGIN] -> OnStopped");
        }

		//Turn begin event
        public override void OnTurnBegin()
        {
            if (!DataContainer.Enabled)
                return;

            if (_round == _openingEmoteRound)
                OpeningEmote();

            _round++;
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
            if (!DataContainer.Enabled)
                return;

            Init();            
			//Bot.Log("[PLUGIN] -> OnGameBegin");
        }

		//Match end event
        public override void OnGameEnd()
        {
            if (!DataContainer.Enabled || _postedFinalInfo)
                return;
           
            string emotes = "";
            foreach (var entry in _emotesSentList)
                emotes += entry.Key.ToString() + ", ";

            try
            {
                if (emotes.Length > 0)
                    emotes = emotes.Substring(0, emotes.Length - 2);
            }
            catch { }

            Bot.Log("[PLUGIN] -> Emotenizer : All emotes sent in this game: " + emotes);
            _postedFinalInfo = true;
			//Bot.Log("[PLUGIN] -> OnGameEnd");				
			
        }

		//gold balance changed event
        public override void OnGoldAmountChanged()
        {
			//Bot.Log("[PLUGIN] -> OnGoldAmountChanged");
        }

		//arena 12 wins or 3 losses event
        public override void OnArenaEnd()
        {
			//Bot.Log("[PLUGIN] -> OnArenaEnd");
        }
		
		//all quests completed event
        public override void OnAllQuestsCompleted()
        {
			//Bot.Log("[PLUGIN] -> OnAllQuestsCompleted");
        }
		
        public override void OnVictory()
        {
            if (!DataContainer.Enabled)
                return;

            EndingEmote(true);            
        }

        //lethal found event (during a game)
        public override void OnLethal()
        {
            if (!DataContainer.Enabled)
                return;

            EndingEmote(true);
            //Bot.Log("[PLUGIN] -> OnLethal");
        }

        public override void OnDefeat()
        {
            if (!DataContainer.Enabled)
                return;

            EndingEmote(false);
        }

        //concede event
        public override void OnConcede()
        {
            if (!DataContainer.Enabled)
                return;

            EndingEmote(false);
            //Bot.Log("[PLUGIN] -> OnConcede");
        }
		
		public override void OnReceivedEmote(Bot.EmoteType emoteType)
		{
            if ((DateTime.Now - _lastEmoteReceived).TotalMilliseconds <= 2000)
                return;
            
            _lastEmoteReceived = DateTime.Now;
            Bot.Log("[PLUGIN] -> Emotenizer : Received emote: " + emoteType.ToString());
            if (_emotesAnswered >= _maxAnswers) // check for max answers
                return;

            Random rnd = new Random();
            int answering = rnd.Next(1, 101);
            if (answering > _answering) // check if should answer with some probability
                return;

            rnd = new Random();
            int type;
            switch (emoteType)
            {
                case Bot.EmoteType.Greetings:
                    type = rnd.Next(1, 3);
                    if (type == 1)
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Greetings, DateTime.Now));
                    else
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Threaten, DateTime.Now));
                    break;
                case Bot.EmoteType.Oops:
                    type = rnd.Next(1, 4);
                    if (type == 1)
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Sorry, DateTime.Now));
                    else if (type == 2)
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Thanks, DateTime.Now));
                    else
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Threaten, DateTime.Now));
                    break;
                case Bot.EmoteType.Sorry:
                    type = rnd.Next(1, 4);
                    if (type == 1)
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Thanks, DateTime.Now));
                    else if (type == 2)
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.WellPlayed, DateTime.Now));
                    else
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Threaten, DateTime.Now));
                    break;
                case Bot.EmoteType.Thanks:
                    type = rnd.Next(1, 4);
                    if (type == 1)
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Oops, DateTime.Now));
                    else if (type == 2)
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Greetings, DateTime.Now));
                    else
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Threaten, DateTime.Now));                    
                    break;
                case Bot.EmoteType.Threaten:
                    type = rnd.Next(1, 4);
                    if (type == 1)
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Sorry, DateTime.Now));
                    else if (type == 2)
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Greetings, DateTime.Now));
                    else
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Threaten, DateTime.Now));                    
                    break;
                case Bot.EmoteType.WellPlayed:
                    type = rnd.Next(1, 4);
                    if (type == 1)
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.WellPlayed, DateTime.Now));
                    else if (type == 2)
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Thanks, DateTime.Now));
                    else
                        _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Threaten, DateTime.Now));                    
                    break;
            }

            _openingDone = true; // if answered reset the opening emote anyway
            _emotesAnswered++;
		}				

        private void Init()
        {
            _maxAnswers = ((bPluginDataContainer)DataContainer).MaxAnswers;
            _openingEmote = ((bPluginDataContainer)DataContainer).OpeningEmoteChance;
            _endingEmote = ((bPluginDataContainer)DataContainer).EndingEmoteChance;
            _answering = ((bPluginDataContainer)DataContainer).AnsweringChance;

            _emotesQueue.Clear();
            _emotesSentList.Clear();
            _lastEmoteSent = DateTime.MinValue;
            _lastEmoteReceived = DateTime.MinValue;
                        
            _round = 1;
            _openingEmoteRound = 1;               
            _openingDone = false;
            _endingDone = false;            

            _emotesSent = 0;
            _emotesAnswered = 0;
            _postedFinalInfo = false;            

            Random rnd = new Random();
            int opening = rnd.Next(1, 101);
            if (opening <= _openingEmote)
            {
                _doOpening = true;
                _openingEmoteRound = rnd.Next(1, 3); // first or second round
            }
            else
            { 
                _doOpening = false;
            }

            int ending = rnd.Next(1, 101);
            if (ending <= _endingEmote)
                _doEnding = true;
            else
                _doEnding = false;
        }
	
        private void OpeningEmote()
        {
            if (!_doOpening || _openingDone || _round > _openingEmoteRound)
                return;

            Random rnd = new Random();
            int type = rnd.Next(1, 3);
            
            if (type == 1)
                _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Greetings, DateTime.Now));
            else
                _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Threaten, DateTime.Now));

            _openingDone = true;          
        }

        private void EndingEmote(bool won)
        {
            if (!_doEnding || _endingDone)
                return;            

            Random rnd = new Random();
            int type;           

            if (won)
                type = rnd.Next(1, 6); 
            else
                type = rnd.Next(1, 3); 
            
            switch (type)
            {
                case 1:
                    _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.WellPlayed, DateTime.Now));
                    break;
                case 2:
                    _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Threaten, DateTime.Now));                    
                    break;
                case 3:
                    _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Greetings, DateTime.Now));
                    break;
                case 4:
                    _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Sorry, DateTime.Now));
                    break;
                case 5:
                    _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.Thanks, DateTime.Now));
                    break;
                default:
                    _emotesQueue.Add(new KeyValuePair<Bot.EmoteType, DateTime>(Bot.EmoteType.WellPlayed, DateTime.Now));
                    break;
            }			                            

            _endingDone = true;          
        }

        private void DoAllJobs()
        {
            if (((DateTime.Now - _lastEmoteSent).TotalMilliseconds <= BLOCKING_TIME) || _emotesSent >= TOTAL_EMOTES)
                _emotesQueue.Clear();

            try
            {                
                if(_emotesQueue.Count > 0)
                {
                    var entry = _emotesQueue[0];                    
                    Random rnd = new Random();
                    double randomInterval;
                    
                    randomInterval = (double)rnd.Next(1000, 5001);                                        
                    if ((DateTime.Now - entry.Value).TotalMilliseconds >= randomInterval)
                    {
                        Bot.SendEmote(entry.Key);
                        _lastEmoteSent = DateTime.Now;
                        _emotesSentList.Add(entry);
                        Bot.Log("[PLUGIN] -> Emotenizer : Sent emote: " + entry.Key.ToString());
                        _emotesSent++;
                        _emotesQueue.Clear();
                    }                    
                }                
            }
            catch (Exception ex)
            {
                Bot.Log("[PLUGIN] -> Emotenizer error: " + ex.Message);
            }
        }
    }
}
