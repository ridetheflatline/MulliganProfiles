using System;
using System.Collections.Generic;
using System.Linq;
using SmartBot.Plugins.API;

/* Explanation on profiles :
 * 
 * All the values defined in profiles are percentage modifiers, it means that it will affect base profile's default values.
 * 
 * Modifiers values can be set within the range (-1000 - 1000)  (negative modifier has the opposite effect)
 * You can specify targets for the non-global modifiers, these target specific modifers will be added on top of global modifier + modifier for the card (without target)
 * 
 * parameters.GlobalSpellsModifier ---> Modifier applied to all spells no matter what they are. The higher is the modifier, the less likely the AI will be to play the spell
 * parameters.GlobalMinionsModifier ---> Modifier applied to all minions no matter what they are. The higher is the modifier, the less likely the AI will be to play the minion
 * 
 * parameters.GlobalAggroModifier ---> Modifier applied to enemy's health value, the higher it is, the more aggressive will be the AI
 * parameters.GlobalDefenseModifier ---> Modifier applied to friendly's health value, the higher it is, the more hp conservative will be the AI
 * 
 * parameters.SpellsModifiers ---> You can set individual modifiers to each spells there, those are ADDED to the GLOBAL modifiers !!
 * parameters.MinionsModifiers ---> You can set individual modifiers to each minions there, those are ADDED to the GLOBAL modifiers !!
 * 
 * parameters.GlobalDrawModifier ---> Modifier applied to card draw value
 * parameters.GlobalWeaponsModifier ---> Modifier applied to the value of weapons attacks
 * 
 */

namespace SmartBotProfiles
{
    [Serializable]
    public class DruidCombo : Profile
    {
        //Cards definitions
        private const Card.Cards SteadyShot = Card.Cards.DS1h_292;
        private const Card.Cards Shapeshift = Card.Cards.CS2_017;
        private const Card.Cards LifeTap = Card.Cards.CS2_056;
        private const Card.Cards Fireblast = Card.Cards.CS2_034;
        private const Card.Cards Reinforce = Card.Cards.CS2_101;
        private const Card.Cards ArmorUp = Card.Cards.CS2_102;
        private const Card.Cards LesserHeal = Card.Cards.CS1h_001;
        private const Card.Cards DaggerMastery = Card.Cards.CS2_083b;

        private readonly Dictionary<Card.Cards, int> _heroPowersPriorityTable = new Dictionary<Card.Cards, int>
        {
            {SteadyShot, 8},
            {Shapeshift, 7},
            {LifeTap, 6},
            {Fireblast, 5},
            {Reinforce, 4},
            {ArmorUp, 3},
            {LesserHeal, 2},
            {DaggerMastery, 1}
        };

        private readonly Dictionary<Card.Cards, int> _spellDamagesTable = new Dictionary<Card.Cards, int>
        {
            {Cards.Swipe, 4},
            {Cards.LivingRoots, 2}
        };

        public ProfileParameters GetParameters(Board board)
        {
            //Init profile parameter based on rush profile
            var parameters = new ProfileParameters(BaseProfile.Default);

            /* ------------------------------------------------------------------------------ */
            /* ----------------------------   Global cards modifiers ------------------------- */
            /* ------------------------------------------------------------------------------ */

            //The coin
            parameters.SpellsModifiers.AddOrUpdate(Card.Cards.GAME_005, new Modifier(150));

            //Innervate
            parameters.SpellsModifiers.AddOrUpdate(Cards.Innervate, new Modifier(180));

            //Swipe
            parameters.SpellsModifiers.AddOrUpdate(Cards.Swipe, new Modifier(75));

            //Silence specifics
            parameters.MinionsModifiers.AddOrUpdate(Cards.KeeperoftheGrove, new Modifier(30, Cards.TwilightDrake));
            parameters.MinionsModifiers.AddOrUpdate(Cards.KeeperoftheGrove, new Modifier(20, Cards.SludgeBelcher));
            parameters.MinionsModifiers.AddOrUpdate(Cards.KeeperoftheGrove, new Modifier(1, Cards.TunnelTrogg));
            parameters.MinionsModifiers.AddOrUpdate(Cards.KeeperoftheGrove, new Modifier(1, Cards.KnifeJuggler));
            parameters.MinionsModifiers.AddOrUpdate(Cards.KeeperoftheGrove, new Modifier(1, Cards.NorthshireCleric));

            //Dr.Boom
            parameters.MinionsModifiers.AddOrUpdate(Cards.DrBoom, new Modifier(-100));

            //AzureDrake
            parameters.MinionsModifiers.AddOrUpdate(Cards.AzureDrake, new Modifier(0));

            //Thaurissan
            if (HasSimpleComboInHand(board) ||
                board.HasCardInHand(Cards.ForceofNature) && board.Hand.Count > 4 &&
                board.MinionEnemy.Sum(x => x.CurrentAtk) < 5)
                parameters.MinionsModifiers.AddOrUpdate(Cards.EmperorThaurissan, new Modifier(-30));

            //Force of nature
            parameters.SpellsModifiers.AddOrUpdate(Cards.ForceofNature,
                new Modifier(HasDoubleForceOfNature(board) ? 60 : 130));

            //Wild growth end game

            if (board.MaxMana == 7 && HasSimpleComboInHand(board))
                parameters.SpellsModifiers.AddOrUpdate(Cards.WildGrowth, new Modifier(1));
            else if (board.MaxMana >= 10)
                parameters.SpellsModifiers.AddOrUpdate(Cards.WildGrowth, new Modifier(1));

            /* ------------------------------------------------------------------------------ */
            /* ------------------------------------------------------------------------------ */


            /* ------------------------------------------------------------------------------ */
            /* ---------------------       Aggro modifiers       ---------------------------- */
            /* ------------------------------------------------------------------------------ */

            parameters.GlobalAggroModifier = new Modifier(175);

            //If we cant put down enemy's life at topdeck lethal range
            if (HasPotentialLethalNextTurn(board))
            {
                Bot.Log("Detected potential lethal next turn, aggro, aggro, aggro !!!");

                //Bot.Log("Detected potential lethal next turn !!!!");
                parameters.GlobalAggroModifier = new Modifier(500);

                if (board.Hand.Count(x => x.Template.Id == Cards.SavageRoar) == 1)
                    parameters.SpellsModifiers.AddOrUpdate(Cards.SavageRoar, new Modifier(150));
            }

            /* ------------------------------------------------------------------------------ */
            /* ------------------------------------------------------------------------------ */


            /* ------------------------------------------------------------------------------ */
            /* ---------------------        Draw modifiers       ---------------------------- */
            /* ------------------------------------------------------------------------------ */

            if (ShouldDrawCards(board)) //If we need to draw cards
            {
                parameters.MinionsModifiers.AddOrUpdate(Cards.AncientofLore, new Modifier(40));
                parameters.GlobalDrawModifier = new Modifier(110);
            }
            else
            {
                parameters.GlobalDrawModifier = new Modifier(50);
            }

            /* ------------------------------------------------------------------------------ */
            /* ------------------------------------------------------------------------------ */

            if (board.TurnCount < 5)
            {
                //Turn specific handlers
                switch (board.ManaAvailable)
                {
                    case 1:
                        HandleTurnOneSpecifics(board, ref parameters);
                        break;

                    case 2:
                        HandleTurnTwoSpecifics(board, ref parameters);
                        break;

                    case 3:
                        HandleTurnThreeSpecifics(board, ref parameters);
                        break;
                }
            }

            return parameters;
        }

        public Card.Cards SirFinleyChoice(List<Card.Cards> choices)
        {
            var filteredTable = _heroPowersPriorityTable.Where(x => choices.Contains(x.Key)).ToList();
            return filteredTable.First(x => x.Value == filteredTable.Max(y => y.Value)).Key;
        }

        private void HandleTurnOneSpecifics(Board board, ref ProfileParameters parameters)
        {
            //We want shade turn one
            if (Hasinnervate(board) && board.HasCardInHand(Cards.ShadeofNaxxramas))
            {
                parameters.MinionsModifiers.AddOrUpdate(Cards.ShadeofNaxxramas, new Modifier(-600));
                return;
            }

            //We want shredder on turn one if possible
            if (HasCoin(board) && Hasinnervate(board) && board.HasCardInHand(Cards.PilotedShredder))
            {
                parameters.MinionsModifiers.AddOrUpdate(Cards.PilotedShredder, new Modifier(-600));
                return;
            }
            //else we wanna keep innervate
            parameters.SpellsModifiers.AddOrUpdate(Cards.Innervate, new Modifier(500));

            if (HasCoin(board))
            {
                if (HasDarnassus(board) && !HasWildGrowth(board) && board.MinionEnemy.Count > 0 &&
                    board.MinionEnemy.Sum(x => x.CurrentAtk) < 3)
                    parameters.MinionsModifiers.AddOrUpdate(Cards.DarnassusAspirant, new Modifier(-300));

                //We choose to coin out darnassus over wild growth
                if (HasDarnassus(board) && (HasWildGrowth(board) || board.HasCardInHand(Cards.ShadeofNaxxramas)) &&
                    board.MinionEnemy.All(x => x.Template.Id != Cards.NorthshireCleric))
                    parameters.MinionsModifiers.AddOrUpdate(Cards.DarnassusAspirant, new Modifier(-300));

                if (HasWildGrowth(board) && !HasDarnassus(board) && board.HasCardInHand(Cards.ShadeofNaxxramas))
                    parameters.SpellsModifiers.AddOrUpdate(Cards.WildGrowth, new Modifier(-300));

                if (board.Hand.Count(x => x.Template.Id == Cards.WildGrowth) == 2)
                    parameters.SpellsModifiers.AddOrUpdate(Cards.WildGrowth, new Modifier(-100));
            }
        }

        private void HandleTurnTwoSpecifics(Board board, ref ProfileParameters parameters)
        {
            //Please shredder
            if (board.ManaAvailable == 2 && Hasinnervate(board) && board.HasCardInHand(Cards.PilotedShredder))
            {
                parameters.MinionsModifiers.AddOrUpdate(Cards.PilotedShredder, new Modifier(-150));
                return;
            }

            if (board.ManaAvailable == 3 && HasCoin(board) && board.HasCardInHand(Cards.PilotedShredder))
            {
                parameters.MinionsModifiers.AddOrUpdate(Cards.PilotedShredder, new Modifier(-600));
                return;
            }

            //We choose darnassus over wild growth
            if (HasWildGrowth(board) && HasDarnassus(board))
            {
                if (board.EnemyClass != Card.CClass.WARRIOR &&
                    (board.HasWeapon(false) ? board.WeaponEnemy.CurrentAtk < 3 : board.WeaponEnemy == null))
                    parameters.MinionsModifiers.AddOrUpdate(Cards.DarnassusAspirant, new Modifier(-100));
                else
                {
                    parameters.SpellsModifiers.AddOrUpdate(Cards.WildGrowth, new Modifier(10));
                }
            }
        }

        private void HandleTurnThreeSpecifics(Board board, ref ProfileParameters parameters)
        {
            //Please shredder
            if (board.ManaAvailable == 3 && HasCoin(board) && board.HasCardInHand(Cards.PilotedShredder))
            {
                parameters.MinionsModifiers.AddOrUpdate(Cards.PilotedShredder, new Modifier(-600));
            }

            //DruidoftheClaw
            if (board.ManaAvailable == 3 && Hasinnervate(board) && board.HasCardInHand(Cards.DruidoftheClaw))
            {
                parameters.MinionsModifiers.AddOrUpdate(Cards.DruidoftheClaw, new Modifier(-600));
            }
        }

        private bool Hasinnervate(Board board)
        {
            return board.HasCardInHand(Cards.Innervate);
        }

        private bool HasCoin(Board board)
        {
            return board.HasCardInHand(Card.Cards.GAME_005);
        }

        private bool HasDarnassus(Board board)
        {
            return board.HasCardInHand(Cards.DarnassusAspirant);
        }

        private bool HasWildGrowth(Board board)
        {
            return board.HasCardInHand(Cards.WildGrowth);
        }

        private bool HasEnemyTauntOnBoard(Board board)
        {
            return board.MinionEnemy.Any(x => x.IsTaunt && !x.IsStealth);
        }

        private bool HasAncientOfLoreInHand(Board board)
        {
            return board.HasCardInHand(Cards.AncientofLore);
        }

        private bool HasSimpleComboInHand(Board board)
        {
            return board.HasCardInHand(Cards.ForceofNature) && board.HasCardInHand(Cards.SavageRoar);
        }

        private bool HasDoubleForceOfNature(Board board)
        {
            return board.Hand.Count(x => x.Template.Id == Cards.ForceofNature) >= 2;
        }

        private bool HasPotentialLethalNextTurn(Board board)
        {
            if (HasEnemyTauntOnBoard(board)) return false;

            if (CanPlaySimpleComboNextTurn(board))
            {
                if (GetEnemyHealthAndArmor(board) - GetPotentialMinionAttackThisTurn(board) <=
                    14 + board.Hand.Count(x => x.Template.Id == Cards.ShadeofNaxxramas && x.IsStealth)*2 +
                    GetPotentialAttackNextTurn(board) + GetPotentialAttackersCountNextTurn(board)*2 +
                    GetPlayableMinionsThisTurn(board)*2)
                {
                    return true;
                }
            }
            return GetRemainingBlastDamagesAfterSequence(board) >= GetSecondTurnLethalRange(board);
        }

        private int GetEnemyHealthAndArmor(Board board)
        {
            return board.HeroEnemy.CurrentHealth + board.HeroEnemy.CurrentArmor;
        }

        private int GetPlayableSpellSequenceDamages(Board board)
        {
            return GetSpellSequenceDamages(GetPlayableSpellSequence(board, CanPlaySimpleCombo(board)), board);
        }

        private int GetSecondTurnLethalRange(Board board)
        {
            return GetEnemyHealthAndArmor(board) - GetPotentialFaceDamages(board);
        }

        private List<Card> GetPotentialAttackersNextTurn(Board board)
        {
            var potentialAttackers = board.MinionFriend.ToList();
            potentialAttackers.OrderByDescending(x => x.CurrentAtk);

            foreach (var card in board.MinionEnemy)
            {
                foreach (var potentialAttacker in potentialAttackers)
                {
                    if (card.CurrentAtk >= potentialAttacker.CurrentHealth && potentialAttacker.IsDivineShield == false)
                    {
                        if (potentialAttacker.Template.Id != Cards.PilotedShredder)
                            potentialAttackers.Remove(potentialAttacker);
                        break;
                    }
                }
            }

            return potentialAttackers;
        }

        private int GetPotentialAttackNextTurn(Board board)
        {
            return GetPotentialAttackersNextTurn(board).Sum(x => x.CurrentAtk);
        }

        private int GetPotentialAttackersCountNextTurn(Board board)
        {
            return GetPotentialAttackersNextTurn(board).Count;
        }

        private int GetPotentialMinionAttackThisTurn(Board board)
        {
            if (HasEnemyTauntOnBoard(board)) return 0;
            return board.MinionFriend.FindAll(x => x.CanAttack).Sum(x => x.CurrentAtk);
        }

        private int GetSpellSequenceDamages(List<Card.Cards> sequence, Board board)
        {
            return
                sequence.FindAll(x => _spellDamagesTable.ContainsKey(x))
                    .Sum(x => _spellDamagesTable[x] + board.GetSpellPower());
        }

        private List<Card.Cards> GetPlayableSpellSequence(Board board, bool altogetherwithcombo)
        {
            var ret = new List<Card.Cards>();
            var manaAvailable = altogetherwithcombo ? GetRemainingManaAfterCombo(board) : board.ManaAvailable;

            foreach (var card in board.Hand)
            {
                if (_spellDamagesTable.ContainsKey(card.Template.Id) == false) continue;
                if (manaAvailable < card.CurrentCost) continue;

                ret.Add(card.Template.Id);
                manaAvailable -= card.CurrentCost;
            }

            return ret;
        }

        private int GetPotentialFaceDamages(Board board)
        {
            return GetPotentialMinionAttackThisTurn(board) + GetPlayableSpellSequenceDamages(board);
        }

        private int GetRemainingBlastDamagesAfterSequence(Board board)
        {
            return GetTotalBlastDamagesInHand(board) -
                   GetPlayableSpellSequenceDamages(board);
        }

        private int GetTotalBlastDamagesInHand(Board board)
        {
            return
                board.Hand.FindAll(x => _spellDamagesTable.ContainsKey(x.Template.Id))
                    .Sum(x => _spellDamagesTable[x.Template.Id] + board.GetSpellPower());
        }

        private bool CanPlaySimpleCombo(Board board)
        {
            if (HasSimpleComboInHand(board) == false) return false;
            return GetRemainingManaAfterCombo(board) >= 0;
        }

        private bool CanPlaySimpleComboNextTurn(Board board)
        {
            if (HasSimpleComboInHand(board) == false) return false;
            return board.MaxMana + 1 - GetSimpleComboCost(board) >= 0;
        }

        private int GetRemainingManaAfterCombo(Board board)
        {
            return board.ManaAvailable - GetSimpleComboCost(board);
        }

        private int GetSimpleComboCost(Board board)
        {
            if (HasSimpleComboInHand(board) == false) return 0;
            return board.Hand.FindAll(x => x.Template.Id == Cards.ForceofNature).Min(x => x.CurrentCost) +
                   board.Hand.FindAll(x => x.Template.Id == Cards.SavageRoar).Min(x => x.CurrentCost);
        }

        private bool ShouldDrawCards(Board board)
        {
            if (HasAncientOfLoreInHand(board) && board.Hand.Count < 7) return true;
            if (board.Hand.Count(x => x.Type == Card.CType.MINION) < 2 && board.ManaAvailable > 2 &&
                board.Ability.Template.Id == LifeTap)
            {
                return true;
            }

            return false;
        }

        private int GetPlayableMinionsThisTurn(Board board)
        {
            var ret = 0;
            var manaAvailable = board.ManaAvailable;

            foreach (var card in board.Hand)
            {
                if (card.CurrentCost <= manaAvailable && card.Type == Card.CType.MINION)
                {
                    ret++;
                    manaAvailable -= card.CurrentCost;
                }
            }

            return ret;
        }
    }
}