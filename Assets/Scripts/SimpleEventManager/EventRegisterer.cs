using SimpleEventManager.Action;
using SimpleEventManager.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleEventManager
{
    internal static class EventRegisterer
    {
        private static Coroutine coroutine;

        private const int capacity = 512;
        private static Queue<uint> freeIDs;

        private static uint increasedCount;

        static EventRegisterer()
        {
            freeIDs = new Queue<uint>(capacity);
            IncreaseSize();
        }

        private static void IncreaseSize()
        {
            increasedCount++;
            for (uint i = capacity * (increasedCount - 1); i < capacity * increasedCount; i++)
                freeIDs.Enqueue(i);
        }

        private static uint GetID()
        {
            if (freeIDs.Count == 0)
                IncreaseSize();
            return freeIDs.Dequeue();
        }

        internal static void ReturnID(uint id) => freeIDs.Enqueue(id);

        internal static GameEventID Registration(GameEvent gameEvent)
        {
            gameEvent.ID = new GameEventID(GetID());
            var status = gameEvent.StartTime <= EventManager.CurrentTime
                ? GameEventStatus.InProgress
                : GameEventStatus.WaitToRegistration;

            AddGameEvent(gameEvent, status);

            if (status == GameEventStatus.WaitToRegistration)
                StartWaitingToRegistration();

            return gameEvent.ID;
        }

        private static void AddGameEvent(GameEvent gameEvent, GameEventStatus status)
        {
            if (!EventManager.GameEvents.ContainsKey(status))
                EventManager.GameEvents.Add(status, new Dictionary<GameEventID, GameEvent>());
            if (EventManager.GameEvents[status].ContainsKey(gameEvent.ID)) return;
            EventManager.GameEvents[status].Add(gameEvent.ID, gameEvent);

            if (status == GameEventStatus.InProgress)
                ActionExecuter.AttachGameEvent(gameEvent);
        }

        private static void StartWaitingToRegistration()
        {
            if (coroutine == null)
                coroutine = EventManager.CoroutineHolder.StartCoroutine(
                        StarvationRegistration(EventManager.starvationRegistrationTick));
        }

        private static IEnumerator StarvationRegistration(float seconds)
        {
            while (true)
            {
                yield return new WaitForSeconds(seconds);
                CheckStartTime();
            }
        }

        internal static void CheckStartTime()
        {
            var waitersToRegistration = EventManager.GameEvents[GameEventStatus.WaitToRegistration].Values;
            foreach (var gameEvent in new List<GameEvent>(waitersToRegistration))
                if (gameEvent.StartTime <= EventManager.CurrentTime)
                    EventManager.ChangeStatus(gameEvent.ID, GameEventStatus.InProgress);
        }
    }
}