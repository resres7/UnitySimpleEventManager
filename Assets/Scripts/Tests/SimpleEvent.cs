using UnityEngine;
using SimpleEventManager;
using SimpleEventManager.Event;
using SimpleEventManager.Action;
using System.Collections;

public class SimpleEvent : MonoBehaviour
{
    int i;

    private void Start()
    {
        StartCoroutine(StartGameEvents());
        StartCoroutine(StartLateEvent());
    }

    private IEnumerator StartGameEvents()
    {
        for (int i = 0; i < 12; i++)
        {
            StartEvent();
            yield return new WaitForSeconds(.01f);
        }
    }

    private IEnumerator StartLateEvent()
    {
        yield return new WaitForSeconds(10);
        StartEvent();
    }

    private void StartEvent()
    {
        var gameEvent = new GameEvent(EventManager.CurrentTime.AddSeconds(3), EventManager.CurrentTime.AddSeconds(1));
        gameEvent.AddActions(new GameAction(ActionType.OnStart, test_START));
        gameEvent.AddActions(new GameAction(ActionType.OnEnd, test_END));
        gameEvent.AddActions(new GameAction(ActionType.OnEveryTick, test_ES));
        var id = EventManager.Register(gameEvent);
        Debug.Log($"Created: {id}");

    }

    private void test_ES()
    {
        Debug.Log($"Tick :{i}");
        i++;
    }

    private void test_END()
    {
        Debug.Log($"End {EventManager.CurrentTime}");
    }

    private void test_START()
    {
        Debug.Log($"Start {EventManager.CurrentTime}");
    }
}
