﻿using SimpleEventManager.Action;
using SimpleEventManager.Event;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleEventManager
{
    public static class EventManager
    {
        #region Settings
        public static readonly float starvationRegistrationTick = .25f;
        #endregion

        public static DateTime CurrentTime => DateTime.UtcNow;

        internal static readonly Dictionary<GameEventStatus, Dictionary<GameEventID, GameEvent>> GameEvents =
            new Dictionary<GameEventStatus, Dictionary<GameEventID, GameEvent>>();

        public delegate bool ConditionWithNoArgs();

        /// <summary>
        /// Corotuine holder is MonoBehaviour which use for starting and contains the coroutines.
        /// </summary>
        #region Corotuine holder
        private static GameObject baseGameObjet;
        private static MonoBehaviour _coroutineHolder;
        internal static MonoBehaviour CoroutineHolder
        {
            get
            {
                if (_coroutineHolder == null)
                {
                    baseGameObjet = new GameObject("EventManagerCoroutineContainer");
                    _coroutineHolder = baseGameObjet.AddComponent<CoroutineHolder>();
                    GameObject.DontDestroyOnLoad(baseGameObjet);
                }
                return _coroutineHolder;
            }
        }
        #endregion

        /// <summary>
        /// Registers GameEvent.
        /// </summary>
        /// <param name="gameEvent"></param>
        /// <returns> Return ID of registered GameEvent </returns>
        public static GameEventID Register(GameEvent gameEvent) => EventRegisterer.Registration(gameEvent);

        /// <summary>
        /// Abort the exists GameEvent
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Return true if abort the GameEvent was accomplished. </returns>
        public static bool Abort(GameEventID id) => ChangeStatus(id, GameEventStatus.WaitToEnd);

        /// <summary>
        /// Check if GameEventID exists
        /// </summary>
        /// <param name="id"></param> 
        /// <returns> Return true if GameEventID exists </returns>
        public static bool Containts(GameEventID id)
        {
            bool result = false;
            foreach (var byStatuses in GameEvents.Values)
                foreach (var gameEvent in byStatuses.Values)
                    if (gameEvent.ID.Equals(id))
                        return true;
            return result;
        }


        public static void CheckAllEvents()
        {
            if (GameEvents[GameEventStatus.WaitToEnd].Count != 0)
            {
                foreach (var gameEvent in GameEvents[GameEventStatus.WaitToEnd].Values)
                {
                    gameEvent.GameActions[ActionType.OnEnd].Invoke();
                    gameEvent.ID.Dispose();
                    gameEvent.Dispose();
                }
                GameEvents[GameEventStatus.WaitToEnd].Clear();
            }

            EventRegisterer.CheckStartTime();
        }


        internal static bool ChangeStatus(GameEventID id, GameEventStatus status)
        {
            GameEventStatus statusBuffer = default;
            GameEvent gameEventBuffer = null;
            foreach (var byStatus in GameEvents.Keys)
            {
                foreach (var gameEvent in GameEvents[byStatus].Values)
                    if (gameEvent.ID.Equals(id))
                    {
                        gameEventBuffer = gameEvent;
                        statusBuffer = byStatus;
                        break;
                    }
                if (gameEventBuffer != null) break;
            }

            if (gameEventBuffer == null && statusBuffer != status) return false;

            if (!GameEvents.ContainsKey(status))
                GameEvents.Add(status, new Dictionary<GameEventID, GameEvent>());
            GameEvents[status].Add(id, gameEventBuffer);
            GameEvents[statusBuffer].Remove(id);

            if (status == GameEventStatus.InProgress)
                ActionExecuter.AttachGameEvent(gameEventBuffer);

            return true;
        }

        internal static bool Delete(GameEventID id)
        {
            foreach (var byStatuses in GameEvents.Values)
                foreach (var gameEvent in byStatuses.Values)
                    if (gameEvent.ID.Equals(id))
                    {
                        byStatuses.Remove(id);
                        return true;
                    }
            return false;
        }

        internal static bool Dispose()
        {
            foreach (var dict in GameEvents.Values)
            {
                dict.Clear();
                CoroutineHolder.StopAllCoroutines();
                GameObject.Destroy(baseGameObjet);
            }
            return true;
        }
    }
}