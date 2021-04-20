using UnityEngine;
using SimpleEventManager;
using SimpleEventManager.Event;
using SimpleEventManager.Action;
using System.Collections;

public class SimpleEvent : MonoBehaviour
{
    int i;

    GameEventID lastID;
    GameEventID longGameEventID;
    GameAction createtdWhileProgressGA;


    private void Start()
    {
        //StartCoroutine(StartGameEvents());
        //StartCoroutine(StartLateEvent());

        StartCoroutine(StartAnotherLateEvent());
        StartCoroutine(AddNewActionWhileWorking());
        StartCoroutine(RemoveActionWhileWorking());

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

    private IEnumerator StartAnotherLateEvent()
    {
        yield return new WaitForSeconds(1);
        StartLongEvent();
    }

    private IEnumerator AddNewActionWhileWorking()
    {
        yield return new WaitForSeconds(2);
        test_AddToExist(longGameEventID);
    }

    private IEnumerator RemoveActionWhileWorking()
    {
        yield return new WaitForSeconds(8);
        test_RemoveToExist(longGameEventID);
    }

    private void StartEvent()
    {
        var gameEvent = new GameEvent(EventManager.CurrentTime.AddSeconds(6), EventManager.CurrentTime.AddSeconds(1));
        gameEvent.AddActions(new GameAction(ActionType.OnStart, test_START));
        gameEvent.AddActions(new GameAction(ActionType.OnEnd, test_END));
        gameEvent.AddActions(new GameAction(ActionType.OnEveryTick, test_ES));
        lastID = EventManager.Register(gameEvent);
        Debug.Log($"Created: {lastID}");
    }

    private void test_ES()
    {
        Debug.Log($"Tick :{i}");
        i++;
        Debug.Log($"Remains time: {EventManager.RemainsTime(lastID)}");
    }

    private void test_END()
    {
        Debug.Log($"End {EventManager.CurrentTime}");
    }

    private void test_START()
    {
        Debug.Log($"Start {EventManager.CurrentTime}");
    }

    private void StartLongEvent()
    {
        var gameEvent = new GameEvent(EventManager.CurrentTime.AddSeconds(20), EventManager.CurrentTime.AddSeconds(1));
        gameEvent.AddActions(new GameAction(ActionType.OnEnd, test_END));
        longGameEventID = EventManager.Register(gameEvent);
        Debug.Log($"Created LongEvent: {longGameEventID}");
    }

    private void test_AddToExist(GameEventID id)
    {
        createtdWhileProgressGA = EventManager.AddGameAction(id, ActionType.OnEveryTick, EveryTickLogForLongGameEvent);
        Debug.Log($"New GA was added to GameEvent with ID:{id}");

    }

    private void EveryTickLogForLongGameEvent()
    {
        Debug.Log($"Long event Tick! Remains: {EventManager.RemainsTime(longGameEventID)}");
    }

    private void test_RemoveToExist(GameEventID id)
    {
        EventManager.RemoveGameAction(id, createtdWhileProgressGA);
        Debug.Log($"GA removed from GameEvent with ID:{id}");
    }
}
