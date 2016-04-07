using System;
using SmartBot.Plugins.API;

namespace SmartBot.Plugins
{
    [Serializable]
    public class BreakAfterXTimeDataContainer : PluginDataContainer
    {
        //Init vars
        public BreakAfterXTimeDataContainer()
        {
            Name = "BreakAfterXTime";
        }

        public int PlayDurationMin { get; set; } //in seconds
        public int PlayDurationMax { get; set; } //in seconds

        public bool LongBreakEnabled { get; set; }
        public bool LongBreakCloseHS { get; set; }

        public int LongBreakDurationMin { get; set; } //in seconds
        public int LongBreakDurationMax { get; set; } //in seconds

        public bool ShortBreakEnabled { get; set; }
        public bool ShortBreakCloseHS { get; set; }

        public int ShortBreakDurationMin { get; set; } //in seconds
        public int ShortBreakDurationMax { get; set; } //in seconds
    }

    public class BreakAfterXTimePlugin : Plugin
    {
        private readonly Random rnd = new Random();
        private bool _isSuspended;
        private bool _isWaitingLongBreak, _isWaitingShortBreak;

        /* --------------- BreakAfterX Methods -------------- */

        private DateTime _startTime = DateTime.Now;
        private DateTime _stopTime;
        private int _timerSuspend, _timerLongResume, _timerShortResume;


        public override void OnTick()
        {
            if (_isSuspended && ShouldResume())
                Resume();
        }

        public override void OnStarted()
        {
            Init();
        }
		
		public override void OnStopped()
		{
			Init();
		}

        public override void OnGameEnd()
        {
            if (!_isSuspended && ShouldSuspendLongBreak())
                SuspendLongBreak();
            else if (!_isSuspended && ShouldSuspendShortBreak())
                SuspendShortBreak();
        }


        public override void OnWhisperReceived(Friend friend, string message) {}

        private void Init()
        {
            if (!DataContainer.Enabled)
                return;

            Bot.Log("[PLUGIN] -> BreakAfterXTime : Initialized...");

            InitLongBreak();
            InitShortBreak();
        }

        private void InitLongBreak()
        {
            _timerSuspend = rnd.Next(((BreakAfterXTimeDataContainer) DataContainer).PlayDurationMin,
                ((BreakAfterXTimeDataContainer) DataContainer).PlayDurationMax);
            _timerLongResume = rnd.Next(((BreakAfterXTimeDataContainer) DataContainer).LongBreakDurationMin,
                ((BreakAfterXTimeDataContainer) DataContainer).LongBreakDurationMax);

            Bot.Log("[PLUGIN] -> BreakAfterXTime : Play duration " + _timerSuspend + "s");
            Bot.Log("[PLUGIN] -> BreakAfterXTime : Long Break duration " + _timerLongResume + "s");

            _isSuspended = false;
        }

        private void InitShortBreak()
        {
            _timerShortResume = rnd.Next(((BreakAfterXTimeDataContainer) DataContainer).ShortBreakDurationMin,
                ((BreakAfterXTimeDataContainer) DataContainer).ShortBreakDurationMax);

            Bot.Log("[PLUGIN] -> BreakAfterXTime : Short Break duration " + _timerShortResume + "s");

            _isSuspended = false;
        }

        private bool ShouldSuspendLongBreak()
        {
            return (_startTime.AddSeconds(_timerSuspend) < DateTime.Now) &&
                   ((BreakAfterXTimeDataContainer) DataContainer).LongBreakEnabled;
        }

        private bool ShouldSuspendShortBreak()
        {
            return ((BreakAfterXTimeDataContainer) DataContainer).ShortBreakEnabled;
        }

        private bool ShouldResume()
        {
            if (_isWaitingLongBreak)
                return ShouldResumeLongBreak();
            return ShoudResumeShortBreak();
        }

        private bool ShouldResumeLongBreak()
        {
            return (_stopTime.AddSeconds(_timerLongResume) < DateTime.Now);
        }

        private bool ShoudResumeShortBreak()
        {
            return (_stopTime.AddSeconds(_timerShortResume) < DateTime.Now);
        }

        private void SuspendLongBreak()
        {
            Suspend();

            _isWaitingLongBreak = true;
            _isWaitingShortBreak = false;

            if (((BreakAfterXTimeDataContainer) DataContainer).LongBreakCloseHS)
                Bot.CloseHs();
            Bot.Log("[PLUGIN] -> BreakAfterXTime : Long break suspending for " + _timerLongResume +"...");
        }

        private void SuspendShortBreak()
        {
            Suspend();
            _isWaitingLongBreak = false;
            _isWaitingShortBreak = true;
            if (((BreakAfterXTimeDataContainer) DataContainer).ShortBreakCloseHS)
                Bot.CloseHs();
            Bot.Log("[PLUGIN] -> BreakAfterXTime : Short break suspending for " + _timerShortResume +"...");
        }

        private void Suspend()
        {
            Bot.StopRelogger();
            Bot.SuspendBot();

            _stopTime = DateTime.Now;
            _isSuspended = true;
        }

        private void Resume()
        {
            if (_isWaitingLongBreak)
            {
                Bot.Log("[PLUGIN] -> BreakAfterXTime : Resuming after long break...");
                _startTime = DateTime.Now;

                InitLongBreak();
            }
            else
            {
                Bot.Log("[PLUGIN] -> BreakAfterXTime : Resuming after short break...");

                InitShortBreak();
            }

            Bot.ResumeBot();
            Bot.StartRelogger();
        }
    }
}