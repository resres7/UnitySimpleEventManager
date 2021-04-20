using SimpleEventManager.Interface;
using System.Collections.Generic;
using UnityEngine.Events;
using static SimpleEventManager.EventManager;

namespace SimpleEventManager.Action
{
    public class GameAction : IGameAction
    {
        private ActionType actionType;
        public ActionType ActionType => actionType;

        private List<ConditionWithNoArgs> conditions;
        public List<ConditionWithNoArgs> Conditions => conditions;

        internal UnityAction action;

        public GameAction(ActionType actionType, params UnityAction[] actions)
        {
            GameActionInitialize(actionType, actions);
        }

        public GameAction(
            ActionType actionType, List<ConditionWithNoArgs> conditions = null, params UnityAction[] actions)
        {
            GameActionInitialize(actionType, actions);
            if (conditions?.Count != 0)
                this.conditions = conditions;
            else
                this.conditions = new List<ConditionWithNoArgs>();
        }

        private void GameActionInitialize(ActionType actionType, params UnityAction[] actions)
        {
            this.actionType = actionType;
            for (int i = 0; i < actions.Length; i++)
                action += actions[i];
        }

        public bool CheckConditions()
        {
            if (Conditions == null) return true;
            foreach (var condition in Conditions)
                if (!condition()) return false;
            return true;
        }

        public static GameAction operator +(GameAction gameAction, GameAction newAction)
        {
            gameAction.action += newAction.action;
            return gameAction;
        }

        public static GameAction operator -(GameAction gameAction, GameAction newAction)
        {
            gameAction.action -= newAction.action;
            return gameAction;
        }

        public static GameAction operator +(GameAction gameAction, UnityAction newAction)
        {
            gameAction.action += newAction;
            return gameAction;
        }

        public static GameAction operator -(GameAction gameAction, UnityAction newAction)
        {
            gameAction.action -= newAction;
            return gameAction;
        }

        public bool Invoke()
        {
            if (!CheckConditions()) return false;
            action?.Invoke();
            return true;
        }

        public void Dispose()
        {
            if (action != null) action = null;
            if (conditions != null) conditions = null;
        }
    }
}