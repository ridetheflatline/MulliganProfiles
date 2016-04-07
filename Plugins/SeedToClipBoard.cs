using System;
using System.Windows.Forms;
using SmartBot.Plugins.API;

namespace SmartBot.Plugins
{
    [Serializable]
    public class bPluginDataContainer : PluginDataContainer
    {
        public bPluginDataContainer()
        {
            Name = "SeedToClipboard";
        }
    }

    public class LethalAlertPlugin : Plugin
    {
        private GuiElementButton buttonCatcher;
        private string LastSeed = string.Empty;
        private bool Started;

        public override void OnStarted()
        {
            Started = true;
            buttonCatcher = new GuiElementButton("Catch Seed", delegate
            {
                if (LastSeed != string.Empty)
                {
                    Clipboard.SetText(LastSeed);
                }
            }, 10, 100, 80, 30);
        }

        public override void OnStopped()
        {
            base.OnStopped();
            Started = false;
            if (buttonCatcher != null)
                GUI.RemoveElement(buttonCatcher);
        }

        public override void OnSimulation()
        {
            base.OnSimulation();

            if (Bot.CurrentBoard != null)
            {
                Bot.Log("[PLUGIN] SeedToClipBoard -> Seed saved to clipboard");
                LastSeed = Bot.CurrentBoard.ToSeed();
            }
        }

        public override void OnTick()
        {
            if (buttonCatcher != null)
            {
                GUI.RemoveElement(buttonCatcher);

                if (Started)
                    GUI.AddElement(buttonCatcher);
            }
        }
    }
}