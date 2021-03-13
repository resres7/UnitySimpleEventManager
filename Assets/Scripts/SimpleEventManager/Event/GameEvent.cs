using SimpleEventManager.Action;
using SimpleEventManager.Interface;
using System;
using System.Collections.Generic;
using static SimpleEventManager.EventManager;

namespace SimpleEventManager.Event
{
    public class GameEvent : IGameEvent
    {
        // When created ID is default. Need Register event to aquire the GameEventID.
        internal GameEventID ID;
        public Dictionary<ActionType, GameAction> GameActions { get; } = new Dictionary<ActionType, GameAction>();

        public DateTime EndTime { get; private set; } = DateTime.MaxValue;
        public DateTime StartTime { get; private set; } = DateTime.MinValue;

        public float SecondPerTick { get; private set; } = 1f;
        public GameEventStatus Status { get; internal set; } = GameEventStatus.Idle;

        public GameEvent(DateTime endTime, DateTime startTime = default, float secondPerTick = 1f)
        {
            EndTime = endTime;
            StartTime = startTime;
            SecondPerTick = secondPerTick;

            AddActions(new GameAction(ActionType.OnEnd, new List<ConditionWithNoArgs>() { CheckTimeExpired }, Dispose));

        }

        public void AddActions(GameAction action)
        {
            if (GameActions.TryGetValue(action.ActionType, out var gameAction))
                gameAction += action;
            else
                GameActions.Add(action.ActionType, action);
        }

        public bool RemoveActions(GameAction action)
        {
            if (GameActions.TryGetValue(action.ActionType, out var gameAction))
            {
                gameAction -= action;
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            if (GameActions.Count != 0)
            {
                Delete(ID);
                ActionExecuter.RemoveGameEvent(ID);
                foreach (var gameAction in GameActions.Values)
                    gameAction.Dispose();
                GameActions.Clear();
            }
        }

        private bool CheckTimeExpired() => CurrentTime >= EndTime;

        public void ForceCheckEndActions()
        {

        }
    }
}
