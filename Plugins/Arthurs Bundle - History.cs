using SmartBot.Plugins.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;


namespace SmartBot.Plugins
{
    public static class Extension
    {
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }

        public static bool IsArena(this Bot.Mode mode)
        {
            return mode == Bot.Mode.Arena || mode == Bot.Mode.ArenaAuto;
        }
    }
    [Serializable]
    public class Arthurs_Bundle___History : PluginDataContainer
    {
        [DisplayName("Games to Analyze")]
        public int GTA { get; set; }
        [DisplayName("Record Game Time")]
        public bool RGT { get; set; }
        [DisplayName("Print Summary on start")]
        public bool print { get; set; }
        [DisplayName("Detailed Summary")]
        public Details details { get; set; }
        
        public Arthurs_Bundle___History()
        {
            Name = "Arthurs Bundle - History";
            GTA = 50;
            RGT = true;
            print = true;
            details = Details.Classes;
            Enabled = true;
        }
    }
    public class HistoryDisplay : Plugin
    {

    }
    public enum Details
    {
        Classes,
        DeckTypes,
        Both,
    }
}

