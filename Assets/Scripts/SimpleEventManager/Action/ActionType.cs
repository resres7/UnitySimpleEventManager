namespace SimpleEventManager.Action
{
    public enum ActionType : uint
    {
        OnStart = 0,
        OnEveryTick,
        OnEnd,
        OnCancel
    }
}