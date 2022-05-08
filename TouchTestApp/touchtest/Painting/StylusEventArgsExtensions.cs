using System.Linq;
using System.Windows.Input;

namespace touchtest.Painting
{
    internal static class StylusEventArgsExtensions
    {
        public static bool IsFromTouchDevice(this StylusEventArgs e)
        {
            return e.StylusDevice.TabletDevice.SupportedStylusPointProperties.Any(prop => prop.Id == StylusPointProperties.SystemTouch.Id);
        }
    }
}
