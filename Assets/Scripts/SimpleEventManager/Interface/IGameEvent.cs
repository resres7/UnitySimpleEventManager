using SimpleEventManager.Action;
using SimpleEventManager.Event;
using System;
using System.Collections.Generic;

namespace SimpleEventManager.Interface
{
    public interface IGameEvent : IDisposable
    {
        Dictionary<ActionType, GameAction> GameActions { get; }
        GameEventStatus Status { get; }
        DateTime EndTime { get; }
        DateTime StartTime { get; }
        float SecondPerTick { get; }

        void AddActions(GameAction action);
        bool RemoveActions(GameAction action);
    }
}
