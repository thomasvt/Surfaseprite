using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using touchtest.Drawing;

namespace touchtest.Painting
{
    /// <summary>
    /// Combines taps from multiple fingers within a short timewindow into multifinger tap event.
    /// </summary>
    internal class FingerCounter
    {
        private Timer _timer;
        private Dictionary<DeviceInfo, Point> _fingerLocations;

        public FingerCounter(double timeout)
        {
            _fingerLocations = new Dictionary<DeviceInfo, Point>(3);
            _timer = new Timer();
            _timer.Elapsed += _timer_Elapsed;
            _timer.Interval = timeout;
            _timer.AutoReset = false;
        }

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        { 
            _timer.Stop();
            Tap?.Invoke(_fingerLocations);
            _fingerLocations.Clear();
        }

        public int Count => _fingerLocations.Count;

        public void AddTap(DeviceInfo fingerInfo, Point location)
        {
            _fingerLocations.Add(fingerInfo, location);
            _timer.Start();
        }

        public event Action<IReadOnlyDictionary<DeviceInfo, Point>>? Tap;
    }
}
