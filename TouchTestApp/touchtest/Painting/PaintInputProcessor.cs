using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using touchtest.Painting;

namespace touchtest.Drawing
{
    /// <summary>
    /// Converts lowlevel mouse, pen and touch inputs from WPF into painting and document manipulation gestures for a typical painting program.
    /// </summary>
    public class PaintInputProcessor
    {
        private Dictionary<int, Stroke> _strokesPerStylusId;
        private MouseInputReceiver _mouseInputReceiver;
        private PenInputReceiver _penInputReceiver;
        private TouchInputReceiver _touchInputReceiver;
        private bool _isManipulating;

        public PaintInputProcessor(Panel hostPanel)
        {
            if (!hostPanel.IsManipulationEnabled)
                hostPanel.IsManipulationEnabled = true; // enable touch manipulation events
            if (hostPanel.Background == null)
                hostPanel.Background = Brushes.Transparent; // make interaction events arrive

            HostControl = hostPanel;
            _strokesPerStylusId = new Dictionary<int, Stroke>();
            _mouseInputReceiver = new MouseInputReceiver(this, hostPanel);
            _penInputReceiver = new PenInputReceiver(this, hostPanel);
            _touchInputReceiver = new TouchInputReceiver(this, hostPanel);
        }

        internal Stroke GetStroke(int id)
        {
            return _strokesPerStylusId[id];
        }

        internal void TapMultiFinger(int fingerCount)
        {
            MultiFingerTap?.Invoke(fingerCount);
        }

        internal void UpdateManipulation(ManipulationDelta cumulativeManipulation)
        {
            ManipulationUpdated?.Invoke();
        }

        internal void StartManipulation(ManipulationDelta amount)
        {
            CancelAllStrokes();
            _isManipulating = true;
            ManipulationStarted?.Invoke();
        }

        internal bool IsManipulating => _isManipulating;

        internal void EndManipulation()
        {
            _isManipulating = false;
            ManipulationEnded?.Invoke();
        }

        private void CancelAllStrokes()
        {
            foreach (var stroke in _strokesPerStylusId.Values)
            {
                StrokeCanceled?.Invoke(new StrokeEventArgs(stroke));
            }
            _strokesPerStylusId.Clear();
        }

        internal void StartStroke(in DeviceInfo stylus)
        {
            if (_strokesPerStylusId.ContainsKey(stylus.Id))
                CancelStroke(stylus.Id);

            var stroke = new Stroke(stylus);
            _strokesPerStylusId.Add(stylus.Id, stroke);
            StrokeStarted?.Invoke(new StrokeEventArgs(stroke));
        }

        internal bool HasStroke(int stylusId)
        {
            return _strokesPerStylusId.ContainsKey(stylusId);
        }

        internal void CompleteStroke(int stylusId)
        {
            if (!_strokesPerStylusId.TryGetValue(stylusId, out var stroke))
                throw new Exception("That stylus doesn't have an active stroke.");

            StrokeCompleted?.Invoke(new StrokeEventArgs(stroke));
            _strokesPerStylusId.Remove(stylusId);
        }

        internal void AddStrokePoint(int stylusId, Point point)
        {
            if (!_strokesPerStylusId.TryGetValue(stylusId, out var stroke))
                throw new Exception("That stylus doesn't have an active stroke.");

            stroke.AddPoint(point);
            StrokePointAdded?.Invoke(new StrokeEventArgs(stroke));
        }

        internal void CancelStroke(int stylusId)
        {
            if (!_strokesPerStylusId.TryGetValue(stylusId, out var stroke))
                throw new Exception("That stylus doesn't have an active stroke.");

            StrokeCanceled?.Invoke(new StrokeEventArgs(stroke));
            _strokesPerStylusId.Remove(stroke.DeviceInfo.Id);
        }

        internal void PlaceDot(DeviceInfo deviceInfo, Point location)
        {
            DotPlaced?.Invoke(new DotEventArgs(deviceInfo, location));
        }

        internal void StartFingerHold(Point location)
        {
            SingleFingerHoldStarted?.Invoke(location);
        }

        internal void EndFingerHold()
        {
            SingleFingerHoldEnded?.Invoke();
        }

        public UIElement HostControl { get; }

        public event Action<StrokeEventArgs>? StrokeCanceled;
        public event Action<StrokeEventArgs>? StrokePointAdded;
        public event Action<StrokeEventArgs>? StrokeCompleted;
        public event Action<StrokeEventArgs>? StrokeStarted;
        public event Action<DotEventArgs>? DotPlaced;
        public event Action<int>? MultiFingerTap;
        public event Action<Point>? SingleFingerHoldStarted;
        public event Action<Point>? SingleFingerHoldMoved;
        public event Action? SingleFingerHoldEnded;
        public event Action ManipulationStarted;
        public event Action ManipulationUpdated;
        public event Action ManipulationEnded;

        /// <summary>
        /// If true a finger will never cause Stroke events. In that case single finger dragging will cause Manipulation events instead.
        /// </summary>
        public bool DisableFingerPainting {
            get => _touchInputReceiver.DisableFingerPainting;
            set => _touchInputReceiver.DisableFingerPainting = value;
        }
    }
}
