using System;

namespace FxcCompilerGui.Utils
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T value)
        {
            Value = value;
        }

        public T Value;
    }

    public class EventArgs<T, U> : EventArgs<T>
    {
        public EventArgs(T value1, U value2)
            : base(value1)
        {
            Value2 = value2;
        }

        public U Value2;
    }
}