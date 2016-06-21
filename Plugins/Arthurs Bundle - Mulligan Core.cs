using SmartBot.Plugins.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;


namespace SmartBot.Plugins
{
    [Serializable]
    public class ArthursBundleMulliganCore : PluginDataContainer
    {
       
        [DisplayName("Curve Mode")]
        public MulliganMode mode { get; set; }
        [DisplayName("Curve Mode")]
        public string cmode { get; private set; }
        [DisplayName("Maximum 1 Drops")]
        public int Max1Drops { get; set; }
        [DisplayName("Maximum 1 Drops [Coin]")]
        public int Max1DropsCoin { get; set; }
        [DisplayName("Maximum 2 Drops")]
        public int Max2Drops { get; set; }
        [DisplayName("Maximum 2 Drops [Coin]")]
        public int Max2DropsCoin { get; set; }
        [DisplayName("Maximum 3 Drops")]
        public int Max3Drops { get; set; }
        [DisplayName("Maximum 3 Drops [Coin]")]
        public int Max3DropsCoin { get; set; }
        [DisplayName("Maximum 4 Drops")]
        public int Max4Drops { get; set; }
        [DisplayName("Maximum 4 Drops [Coin]")]
        public int Max4DropsCoin { get; set; }
        [DisplayName("Allow 3+ drops without\n1 or 2 drops")]
        public bool control { get; set; }
        [DisplayName("[Automatic Default]") ]
        public bool NoChange { get; set; }
        [DisplayName("[Description]")]
        public string message { get; private set; }
        [DisplayName("Force keep cards")]
        public string wll { get; set; }
        [DisplayName("Force keep cards")]
        public string wllm { get; private set; }
        [DisplayName("Force keep cards")]
        public string api { get; set; }
               
        public ArthursBundleMulliganCore()
        {
            Name = "Arthurs Bundle - Mulligan Core";
            message  = "If [Automatic Default] is ticked, Mulligan Bundle will automatically\nfigure out number of max drops for your deck\nand ignore anything you change below"+
                "\n[Disclaimer] Mulligan Core changes here will only affect Arena and few not well refined decks ";
            cmode = "Standard: default logic, if card is good it keeps it\nStrictCurve: Requires 1 drop to keep a 2 drop, 2 drop to keep a 3 drops, and so on.";
            wllm = "All CARD_ID are separated by '~', for example:\n"
                +"OG_311~CS2_188\nLine above will forcefully keep Abusive sergent {CS2_188} and A Light in the Darkness {OG_311}";
            api = "http://hearthstoneapi.com/cards";
            Max1Drops = 1;
            Max1DropsCoin = 2;
            
            Max2Drops = 2;
            Max2DropsCoin = 3;
            
            Max3Drops = 1;
            Max3DropsCoin = 2;
            
            Max4Drops = 1;
            Max4DropsCoin = 1;

            NoChange = true;
            Enabled = true;
        }

    }

    public class MulliganCore : Plugin
    {
        //TODO: add mor
           
    }
    public enum MulliganMode
    {
        Standard,
        StrictCurve,
    }
}