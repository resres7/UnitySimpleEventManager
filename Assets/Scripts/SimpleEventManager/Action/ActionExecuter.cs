using SimpleEventManager.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleEventManager.Action
{
    internal static class ActionExecuter
    {
        private static readonly Dictionary<float, ActionCoroutine> Coroutines = new Dictionary<float, ActionCoroutine>();

        internal static void AttachGameEvent(params GameEvent[] gameEvents)
        {
            foreach (var gameEvent in gameEvents)
            {
                if (!Coroutines.ContainsKey(gameEvent.SecondPerTick))
                    Coroutines.Add(gameEvent.SecondPerTick, CreateCoroutineTick(gameEvent.SecondPerTick));
                Coroutines.TryGetValue(gameEvent.SecondPerTick, out var actionCoroutine);

                gameEvent.GameActions[ActionType.OnStart].Invoke();

                actionCoroutine.IDsInProgress.Add(gameEvent.ID);
            }
        }

        internal static bool RemoveGameEvent(GameEventID id)
        {
            foreach (var actionCoroutines in Coroutines.Values)
                if (actionCoroutines.IDsInProgress.Contains(id))
                    return actionCoroutines.IDsInProgress.Remove(id);
            return false;
        }

        private static ActionCoroutine CreateCoroutineTick(float seconds)
        {
            return new ActionCoroutine(EventManager.CoroutineHolder.StartCoroutine(Tick(seconds)));
        }

        private static IEnumerator Tick(float seconds)
        {
            GameEvent gameEvent;
            List<GameEventID> inProgress;
            while (true)
            {
                yield return new WaitForSeconds(seconds);
                inProgress = new List<GameEventID>(Coroutines[seconds].IDsInProgress);
                foreach (var id in inProgress)
                {
                    gameEvent = EventManager.GameEvents[GameEventStatus.InProgress][id];
                    if (gameEvent.GameActions[ActionType.OnEnd].Invoke()) continue;

                    gameEvent.GameActions.TryGetValue(ActionType.OnEveryTick, out var action);
                    Debug.Log(action);

                    action?.Invoke();
                }
            }
        }
    }
}