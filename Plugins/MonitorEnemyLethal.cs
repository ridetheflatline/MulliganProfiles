using System;
using System.Linq;
using SmartBot.Database;
using SmartBot.Plugins.API;

namespace SmartBot.Plugins
{
    [Serializable]
    public class bPluginDataContainerMonitorEnemyLethal : PluginDataContainer
    {
        public bPluginDataContainerMonitorEnemyLethal()
        {
            Name = "MonitorEnemyLethal";
        }
    }

    public class MonitorEnemyLethal : Plugin
    {
        private DateTime _LastConcedeTime = DateTime.MinValue;
        private DateTime _LastTickTime = DateTime.MinValue;
        private bool IsOwnTurn = false;

        public override void OnTurnBegin()
        {
            IsOwnTurn = true;
            base.OnTurnBegin();
        }

        public override void OnTurnEnd()
        {
            IsOwnTurn = false;
            base.OnTurnEnd();
        }

        public override void OnTick()
        {
            if (Bot.CurrentBoard != null && _LastConcedeTime.AddMinutes(2) < DateTime.Now &&
                _LastTickTime.AddSeconds(1) < DateTime.Now)
            {
                if (Bot.CurrentScene() == Bot.Scene.GAMEPLAY && EnemyHasLethal(Bot.CurrentBoard) &&
                    Bot.CurrentBoard.IsOwnTurn == false && IsOwnTurn == false)
                {
                    Bot.Log("[PLUGIN] MonitorEnemyLethal -> Enemy has already lethal on board, we can concede now ...");
                    Bot.Concede();
                    _LastConcedeTime = DateTime.Now;
                }

                _LastTickTime = DateTime.Now;
            }
        }
        private bool EnemyHasLethal(Board board)
        {
            if (board.MinionFriend.Any(x => x.IsTaunt)) return false;
            
            
            Bot.Log(string.Format("[PLUGIN] {0} debug message", board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor <=
                board.MinionEnemy.FindAll(x => x.CanAttack && (x.IsCharge || x.NumTurnsInPlay != 0) && x.Id != 388).Sum(x => x.CurrentAtk) +
                   (board.HasWeapon(false) && board.HeroEnemy.CountAttack == 0 ? board.WeaponEnemy.CurrentAtk : 0)));
            return board.HeroFriend.CurrentHealth + board.HeroFriend.CurrentArmor <=
                board.MinionEnemy.FindAll(x => x.CanAttack && (x.IsCharge || x.NumTurnsInPlay != 0)).Sum(x => x.CurrentAtk) +
                   (board.HasWeapon(false) && board.HeroEnemy.CountAttack == 0 ? board.WeaponEnemy.CurrentAtk : 0);
        }
    }
}