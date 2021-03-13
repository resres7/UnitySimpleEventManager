using SimpleEventManager.Event;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleEventManager.Action
{
    internal struct ActionCoroutine
    {
        internal Coroutine Coroutine;
        internal HashSet<GameEventID> IDsInProgress;

        internal ActionCoroutine(Coroutine coroutine)
        {
            Coroutine = coroutine;
            IDsInProgress = new HashSet<GameEventID>();
        }
    }
}