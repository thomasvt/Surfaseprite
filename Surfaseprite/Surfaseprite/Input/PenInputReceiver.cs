using System.Windows.Controls;
using System.Windows.Input;

namespace Surfaseprite.Input
{
    internal class PenInputReceiver
    {
        private readonly PaintInputProcessor _paintInputProcessor;
        private readonly Panel _hostPanel;

        public PenInputReceiver(PaintInputProcessor paintInputProcessor, Panel hostPanel)
        {
            hostPanel.StylusSystemGesture += HostPanel_StylusSystemGesture;
            hostPanel.StylusDown += HostPanel_StylusDown;
            hostPanel.StylusMove += HostPanel_StylusMove;
            hostPanel.StylusUp += HostPanel_StylusUp;
            _paintInputProcessor = paintInputProcessor;
            _hostPanel = hostPanel;
        }

        private void HostPanel_StylusMove(object sender, StylusEventArgs e)
        {
            if (e.IsFromTouchDevice())
                return; // we focus on pen events in this Receiver.

            if (_paintInputProcessor.HasStroke(e.StylusDevice.Id))
                _paintInputProcessor.AddStrokePoint(e.StylusDevice.Id, e.GetPosition(_hostPanel));

            e.Handled = true;
        }

        private void HostPanel_StylusDown(object sender, StylusDownEventArgs e)
        {
            if (e.IsFromTouchDevice())
                return; // not a pen, we focus on pen events in this Receiver.

            var deviceInfo = new DeviceInfo(e.StylusDevice.Id, DeviceType.Pen);
            _paintInputProcessor.StartStroke(deviceInfo);
            _paintInputProcessor.AddStrokePoint(deviceInfo.Id, e.GetPosition(_hostPanel));

            e.Handled = true;
        }

        private void HostPanel_StylusUp(object sender, StylusEventArgs e)
        {
            if (e.IsFromTouchDevice())
                return; // we focus on pen events in this Receiver.

            if (_paintInputProcessor.HasStroke(e.StylusDevice.Id))
            {
                _paintInputProcessor.AddStrokePoint(e.StylusDevice.Id, e.GetPosition(_hostPanel));
                _paintInputProcessor.CompleteStroke(e.StylusDevice.Id);
                e.Handled = true;
            }
        }

        private void HostPanel_StylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
        {
            if (e.IsFromTouchDevice())
                return; // we focus on pen events in this Receiver.

            var deviceInfo = new DeviceInfo(e.StylusDevice.Id, DeviceType.Pen);
            switch (e.SystemGesture)
            {
                case SystemGesture.Drag:
                    _paintInputProcessor.StartStroke(deviceInfo);
                    _paintInputProcessor.AddStrokePoint(deviceInfo.Id, e.GetPosition(_hostPanel));
                    break;
                case SystemGesture.Tap:
                    _paintInputProcessor.PlaceDot(deviceInfo, e.GetPosition(_hostPanel));
                    break;
            }
            e.Handled = true;
        }
    }
}
