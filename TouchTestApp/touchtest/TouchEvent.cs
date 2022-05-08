using System;

namespace touchtest
{
    internal record class TouchEvent(string EventName, object EventArgs)
    {
        private DateTime _time = DateTime.Now;

        public override string ToString()
        {
            return $"{_time:HH:mm:ss fff} {EventName}";
        }
    }
}
