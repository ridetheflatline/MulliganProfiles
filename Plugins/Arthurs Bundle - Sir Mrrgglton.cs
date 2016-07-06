using SmartBot.Plugins.API;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace SmartBot.Plugins
{
    [Serializable]
    public class ABSirFinn : PluginDataContainer
    {
        [DisplayName("[Disclaimer]")]
        public string Disclaimer { get; set; }
        [DisplayName("Druid:\tShapeshift")]
        public int DruidHP { get;  set; }
        [DisplayName("Hunter:\tSteady Shot")]
        public int HunterHP { get;  set; }
        [DisplayName("Mage:\tFire Blast")]
        public int MageHP { get;  set; }
        [DisplayName("Paladin:\tReinforce")]
        public int PaladinHP { get;  set; }
        [DisplayName("Priest:\tLesser Heal")]
        public int PriestHP { get;  set; }
        [DisplayName("Rogue:\tDagger Mastery")]
        public int RogueHP { get;  set; }
        [DisplayName("Shaman:\tTotemic Call")]
        public int ShamanHP { get;  set; }
        [DisplayName("Warlock:\tLife Tap")]
        public int WarlockHP { get;  set; }
        [DisplayName("Warrior:\tArmor Up!")]
        public int WarriorHP { get;  set; }
       

        public ABSirFinn()
        {
            Name = "Arthurs Bundle - Sir Mrrgglton";
            Disclaimer = "Does not affect default smartbot profiles. To utilize this plugin you will need a custom profile.\nConsult Arthur in discord if you are interested. ";
            DruidHP = 10;
            HunterHP = 8;
            MageHP = 7;
            PaladinHP = 0;
            PriestHP = 0;
            RogueHP = 0;
            ShamanHP = 0;
            WarlockHP = 9;
            WarriorHP = 0;
            Enabled = false;
        }
       
    }

    public class ABAccountSwitcher : Plugin
    {
        public override void OnStarted()
        {
            if (!((ABSirFinn)DataContainer).Enabled) return;//leave if it's not enabled
              
             base.OnStarted();
        }
        public override void OnPluginCreated()
        {
            

            base.OnPluginCreated();
        }
    }
   
}