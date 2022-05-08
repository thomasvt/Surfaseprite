using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using touchtest.Drawing;

namespace touchtest.Painting
{
    internal class TouchInputReceiver
    {
        private readonly PaintInputProcessor _paintInputProcessor;
        private readonly Panel _hostPanel;
        private readonly FingerCounter _fingerTapCounter;
        private int? _holdFingerId;

        public TouchInputReceiver(PaintInputProcessor paintInputProcessor, Panel hostPanel)
        {
            hostPanel.StylusSystemGesture += HostPanel_StylusSystemGesture;
            hostPanel.StylusDown += HostPanel_StylusDown;
            hostPanel.StylusMove += HostPanel_StylusMove;
            hostPanel.StylusUp += HostPanel_StylusUp;
            hostPanel.ManipulationDelta += HostPanel_ManipulationDelta;
            hostPanel.ManipulationCompleted += HostPanel_ManipulationCompleted;
            _paintInputProcessor = paintInputProcessor;
            _hostPanel = hostPanel;
            _fingerTapCounter = new FingerCounter(150);
            _fingerTapCounter.Tap += _fingerTapCounter_Tap;
        }

        private void HostPanel_ManipulationCompleted(object? sender, ManipulationCompletedEventArgs e)
        {
            if (_paintInputProcessor.IsManipulating)
                _paintInputProcessor.EndManipulation();
        }

        private void HostPanel_ManipulationDelta(object? sender, ManipulationDeltaEventArgs e)
        {
            if (_paintInputProcessor.IsManipulating)
            {
                _paintInputProcessor.UpdateManipulation(e.CumulativeManipulation);
                return;
            }
            // (we detect 2 finger manipulation by checking angle and scale != default value)
            if (_fingerTapCounter.Count == 0 &&
                DisableFingerPainting 
                || e.CumulativeManipulation.Rotation != 0f || e.CumulativeManipulation.Scale.X != 1 || e.CumulativeManipulation.Scale.Y != 1)
            {
                _paintInputProcessor.StartManipulation(e.CumulativeManipulation);
            }
        }

        private void _fingerTapCounter_Tap(IReadOnlyDictionary<DeviceInfo, Point> tappedLocations)
        {
            if (tappedLocations.Count == 1)
            {
                if (!DisableFingerPainting)
                    _paintInputProcessor.PlaceDot(tappedLocations.Keys.First(), tappedLocations.Values.First());
            }
            else
                _paintInputProcessor.TapMultiFinger(tappedLocations.Count);
        }

        private void HostPanel_StylusMove(object sender, StylusEventArgs e)
        {
            if (!e.IsFromTouchDevice())
                return; // we focus on touch events in this Receiver.

            if (_paintInputProcessor.HasStroke(e.StylusDevice.Id))
                _paintInputProcessor.AddStrokePoint(e.StylusDevice.Id, e.GetPosition(_hostPanel));

            e.Handled = true;
        }

        private void HostPanel_StylusDown(object sender, StylusDownEventArgs e)
        {
            if (!e.IsFromTouchDevice())
                return; // we focus on touch events in this Receiver.

            //e.Handled = true; // dont else manipulation doesnt start
        }

        private void HostPanel_StylusUp(object sender, StylusEventArgs e)
        {
            if (!e.IsFromTouchDevice())
                return; // we focus on touch events in this Receiver.

            if (_holdFingerId== e.StylusDevice.Id)
            {
                _paintInputProcessor.EndFingerHold();
                _holdFingerId = null;
            }
            else if (_paintInputProcessor.HasStroke(e.StylusDevice.Id))
            {
                _paintInputProcessor.AddStrokePoint(e.StylusDevice.Id, e.GetPosition(_hostPanel));
                _paintInputProcessor.CompleteStroke(e.StylusDevice.Id);
            }
            e.Handled = true;
        }

        private void HostPanel_StylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
        {
            if (!e.IsFromTouchDevice())
                return; // we focus on toucch events in this Receiver.

            var deviceInfo = new DeviceInfo(e.StylusDevice.Id, DeviceType.Touch);
            var location = e.GetPosition(_hostPanel);
            switch (e.SystemGesture)
            {
                case SystemGesture.Drag:
                    if (DisableFingerPainting || _holdFingerId.HasValue  || _paintInputProcessor.IsManipulating)
                        return;

                    // dont wait for more fingers, start drawing for fast tactile response. We cancel if the gesture
                    // stops being a single-finger-stroke.
                    _paintInputProcessor.StartStroke(deviceInfo);
                    _paintInputProcessor.AddStrokePoint(deviceInfo.Id, location);
                    break;
                case SystemGesture.Tap:
                    if (_holdFingerId.HasValue || _paintInputProcessor.IsManipulating)
                        return; // dont break other gesturing
                    _fingerTapCounter.AddTap(deviceInfo, location);
                    break;
                case SystemGesture.HoldEnter:
                    _holdFingerId = e.StylusDevice.Id;
                    _paintInputProcessor.StartFingerHold(location);
                    break;
            }
            e.Handled = true;
        }

        /// <summary>
        /// If true a finger will never cause stroke events. In that case single finger dragging will cause Manipulation events.
        /// </summary>
        public bool DisableFingerPainting { get; set; }
    }
}
