using System;

namespace SimpleEventManager.Event
{
    public struct GameEventID : IDisposable, IEquatable<uint>
    {
        public readonly uint Value;

        public GameEventID(uint id)
        {
            Value = id;
        }

        public void Dispose()
        {
            if (Value == 0) return;
            EventRegisterer.ReturnID(Value);
        }

        public bool Equals(uint other)
        {
            return other == Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}