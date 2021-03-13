using SimpleEventManager.Action;
using System;
using System.Collections.Generic;
using static SimpleEventManager.EventManager;

namespace SimpleEventManager.Interface
{
    public interface IGameAction : IDisposable
    {
        ActionType ActionType { get; }
        List<ConditionWithNoArgs> Conditions { get; }
        bool CheckConditions();
        bool Invoke();
    }
}