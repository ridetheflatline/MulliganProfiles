using System;
using System.Collections.Generic;
using System.Linq;
using SmartBot.Database;
using SmartBot.Plugins;
using SmartBot.Plugins.API;

namespace SmartBot.PluginsRA
{
    [Serializable]
    public class RenoContainer : PluginDataContainer
    {
        public RenoContainer()
        {
            Name = "RenoAlert";
        }

        public bool ShowDrawnCards { get; set; }
    }

    public class qPlugin : Plugin
    {
        private const string BrannBronzebeard = "LOE_077";
        private const string IronJuggernaut = "GVG_056";
        private const string BurrowingMine = "GVG_056t";
        private const string BeneathTheGround = "AT_035";
        private const string Ambush = "AT_035t";
        private static bool _gameBegan;
        private static bool _renoFlag;
        private static List<string> _secrets;

        private static List<string> oldDuplicateList;
        private readonly List<string> _brannJugg = new List<string> { BrannBronzebeard, IronJuggernaut };

        private bool _beneathTheGrownd;
        private int _currentNumberOfCards;
        private int _deckCounter;
        private List<string> _duplicatesList;
        private List<string> _myDeckList;
        private List<string> _myHand;
        private int _screenHeight;
        private int _screenWidth;
        private bool _sdc;

        private int PercToPixWidth(int percent)
        {
            return (int)((_screenWidth / 100.0f) * percent);
        }

        private int PercToPixHeight(int percent)
        {
            return (int)((_screenHeight / 100.0f) * percent);
        }

        private float RgbToSrgb(int rgb)
        {
            return (rgb / (255.0f));
        }

        public override void OnTurnBegin()
        {
            try
            {
                _gameBegan = true;
                var combo = Bot.CurrentBoard.MinionEnemy.Select(c => c.Template.Id.ToString()).ToList();

                if (!_brannJugg.Except(combo.Select(c => c.ToString()).ToList()).Any())
                {
                    _duplicatesList.Add(BurrowingMine);
                    oldDuplicateList.Add(BurrowingMine);
                }
                var graveyard =
                    Bot.CurrentBoard.EnemyGraveyard.Select(q => CardTemplate.LoadFromId(q).Id.ToString()).ToList();
                if (graveyard.Contains(BeneathTheGround) && !_beneathTheGrownd)
                {
                    _beneathTheGrownd = true;
                    oldDuplicateList.Add(Ambush);
                    _duplicatesList.Add(Ambush);
                }
                _currentNumberOfCards = Bot.CurrentBoard.Hand.Count;
                _myHand = Bot.CurrentBoard.Hand.Select(q => q.Template.Id.ToString()).ToList();
                foreach (var q in _duplicatesList.Where(q => _myHand.Contains(q)))
                {
                    _duplicatesList.Remove(q);
                    if (_duplicatesList.Count == 0) _renoFlag = true;
                }
            }
            catch (Exception e)
            {
            }
        }

        public override void OnTurnEnd()
        {
            _deckCounter = Bot.CurrentBoard.FriendDeckCount;
        }

        public override void OnTick()
        {
            try
            {
                GUI.ClearUI();
                if (!_gameBegan) return;

                if (Bot.CurrentScene() != Bot.Scene.GAMEPLAY) return;
                var boxWidth = 265;
                var boxHeight = 30;
                var leftBorder = (_screenWidth - boxWidth) / 32;


                //GUI.AddElement(new GuiElementBitmap(new Bitmap(), leftBorder, PercToPixHeight(60), boxWidth, boxHeight));
                GUI.AddElement(_renoFlag
                    ? new GuiElementText("Will we be rich? \n   Yup :)", leftBorder, PercToPixHeight(40), boxWidth,
                        boxHeight,
                        16, 255, 215, 0)
                    : new GuiElementText("Will we be rich? \n   NO :(", leftBorder, PercToPixHeight(40), boxWidth,
                        boxHeight,
                        16));
                if (_renoFlag) return;
                GUI.AddElement(new GuiElementText("Checklist to be Rich", leftBorder, PercToPixHeight(45), boxWidth,
                    boxHeight));
                var extra = 5;
                foreach (var q in _sdc ? oldDuplicateList : _duplicatesList)
                {
                    GUI.AddElement(new GuiElementText(CardTemplate.LoadFromId(q).Name, leftBorder,
                        PercToPixHeight(45 + extra), boxWidth, boxHeight, 14,
                        _duplicatesList.Contains(q) ? 255 : 0, _duplicatesList.Contains(q) ? 0 : 255, 0));
                    extra += 2;
                }
                if (DateTime.Now.Second % 5 != 0) return; //some next level Thread.Sleep(5000) function 
                CheckHandAndSecrets();
                if (_beneathTheGrownd)
                {
                    var graveyard =
                        Bot.CurrentBoard.FriendGraveyard.Select(q => CardTemplate.LoadFromId(q).Id.ToString())
                            .ToList()
                            .GroupBy(a => a)
                            .SelectMany(ab => ab.Skip(1).Take(1))
                            .ToList();
                    ;
                    if (graveyard.Contains(Ambush))
                    {
                        _duplicatesList.Remove(Ambush);
                    }
                }
                if (Bot.CurrentBoard.Hand.Count == _currentNumberOfCards) return;
                if (Bot.CurrentBoard.Hand.Count < _currentNumberOfCards)
                {
                    _currentNumberOfCards = Bot.CurrentBoard.Hand.Count;
                    return;
                }
                if (Bot.CurrentBoard.Hand.Count > _currentNumberOfCards)
                    CheckWhatWasDrawn(Bot.CurrentBoard.Hand.Count - _currentNumberOfCards);
            }
            catch (Exception)
            {
            }
        }

        private void CheckHandAndSecrets()
        {
            try
            {
                _myHand = Bot.CurrentBoard.Hand.Select(q => q.Template.Id.ToString()).ToList();
                var ownGrave = Bot.CurrentBoard.FriendGraveyard.Select(q => q.ToString()).ToList();
                _secrets = Bot.CurrentBoard.Secret.Select(q => q.ToString()).ToList();

                foreach (var q in _duplicatesList.Where(q => _myHand.Contains(q)))
                {
                    _duplicatesList.Remove(q);
                    if (_duplicatesList.Count == 0) _renoFlag = true;
                }
                if (_secrets == null) return;
                foreach (var q in _duplicatesList.Where(q => _secrets.Contains(q)))
                {
                    _duplicatesList.Remove(q);
                    if (_duplicatesList.Count == 0) _renoFlag = true;
                }
                foreach (var q in _duplicatesList.Distinct().Where(q => ownGrave.Contains(q)))
                {
                    _duplicatesList.Remove(q);
                    if (_duplicatesList.Count == 0) _renoFlag = true;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void CheckWhatWasDrawn(int numCards)
        {
            try
            {
                for (var i = Bot.CurrentBoard.Hand.Count; i > Bot.CurrentBoard.Hand.Count - numCards; i--)
                {
                    if (_duplicatesList.Contains(Bot.CurrentBoard.Hand[i - 1].Id.ToString()))
                    {
                        _duplicatesList.Remove(Bot.CurrentBoard.Hand[i - 1].Id.ToString());
                        if (_duplicatesList.Count == 0) _renoFlag = true;
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        public override void OnStarted()
        {
            try
            {
                if (Bot.CurrentScene() != Bot.Scene.GAMEPLAY) return;
                Initialize();
            }
            catch (Exception e)
            {
            }
        }

        private void Initialize()
        {
            try
            {
                _sdc = ((RenoContainer)DataContainer).ShowDrawnCards;
                _renoFlag = false;
                _myDeckList = Bot.CurrentDeck().Cards;
                _duplicatesList = _myDeckList.GroupBy(a => a).SelectMany(ab => ab.Skip(1).Take(1)).ToList();
                oldDuplicateList = _myDeckList.GroupBy(a => a).SelectMany(ab => ab.Skip(1).Take(1)).ToList();
                foreach (var q in _myHand.Where(q => _duplicatesList.Contains(q)))
                {
                    _duplicatesList.Remove(q);
                }
                _secrets = Bot.CurrentBoard.Secret.Select(q => q.ToString()).ToList();
            }
            catch (NullReferenceException)
            {
            }
        }

        public override void OnGameBegin()
        {
            try
            {
                _gameBegan = true;
                Initialize();
            }
            catch (Exception e)
            {
            }
        }

        public override void OnGameResolutionUpdate(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;
        }
    }
}