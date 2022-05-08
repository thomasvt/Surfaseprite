using System.Collections.Generic;
using System.Windows;

namespace touchtest.Drawing
{
    public class Stroke
    {
        private List<Point> _points;

        public Stroke(DeviceInfo deviceInfo)
        {
            DeviceInfo = deviceInfo;
            _points = new List<Point>();
        }

        public void AddPoint(Point point)
        {
            _points.Add(point);
        }

        public DeviceInfo DeviceInfo { get; }
        public IReadOnlyList<Point> Points => _points;
    }
}
