using System.Linq;
using System.Windows;
using System.Windows.Input;
using touchtest.Drawing;

namespace touchtest.Painting
{
    internal class MouseInputReceiver
    {
        private static DeviceInfo DeviceInfo = new(-1, DeviceType.Mouse);
        private readonly PaintInputProcessor _paintInputProcessor;
        private readonly UIElement _hostControl;

        public MouseInputReceiver(PaintInputProcessor paintInputProcessor, UIElement hostControl)
        {
            _paintInputProcessor = paintInputProcessor;
            _hostControl = hostControl;

            hostControl.MouseDown += HostControl_MouseDown;
            hostControl.MouseMove += HostControl_MouseMove;
            hostControl.MouseUp += HostControl_MouseUp;
        }

        private void HostControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
                return; // ignore mouse emulation from stylus

            if (e.ChangedButton == MouseButton.Left && _paintInputProcessor.HasStroke(DeviceInfo.Id))
            {
                var location = e.GetPosition(_hostControl);
                
                var stroke = _paintInputProcessor.GetStroke(DeviceInfo.Id);
                var startLocation = stroke.Points.First();

                if ((location - startLocation).Length <= 3)
                {
                    _paintInputProcessor.PlaceDot(DeviceInfo, startLocation);
                }
                else
                {
                    _paintInputProcessor.AddStrokePoint(DeviceInfo.Id, location);
                    _paintInputProcessor.CompleteStroke(DeviceInfo.Id);
                }
                _hostControl.ReleaseMouseCapture();
            }
        }

        private void HostControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice != null)
                return; // ignore mouse emulation from stylus

            if (!_paintInputProcessor.HasStroke(DeviceInfo.Id))
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
                _paintInputProcessor.AddStrokePoint(DeviceInfo.Id, e.GetPosition(_hostControl));
            else

            {
                _paintInputProcessor.CancelStroke(DeviceInfo.Id);
                _hostControl.ReleaseMouseCapture();
            }
        }

        private void HostControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
                return; // ignore mouse emulation from stylus

            if (e.ChangedButton == MouseButton.Left)
            {
                _hostControl.CaptureMouse();
                _paintInputProcessor.StartStroke(DeviceInfo);
                _paintInputProcessor.AddStrokePoint(DeviceInfo.Id, e.GetPosition(_hostControl));
            }
        }
    }
}
