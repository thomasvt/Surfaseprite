using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace touchtest
{
    /// <summary>
    /// Interaction logic for RawEventViewer.xaml
    /// </summary>
    public partial class RawEventViewer : UserControl
    {
        private static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() },
            Formatting = Formatting.Indented
        };

        public RawEventViewer()
        {
            InitializeComponent();
        }

        private void Grid_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (ManipulationFilter.IsChecked.GetValueOrDefault())
                Log(e);
        }

        private void Log(object @event, string? eventName = null)
        {
            eventName ??= @event.GetType().Name.Substring(0, @event.GetType().Name.Length - 9);
            EventLog.Items.Insert(0, new TouchEvent(eventName, @event));
        }

        private void Grid_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (ManipulationFilter.IsChecked.GetValueOrDefault())

                Log(e);
        }

        private void Grid_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (ManipulationFilter.IsChecked.GetValueOrDefault())

                Log(e);
        }

        private void Grid_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            if (ManipulationFilter.IsChecked.GetValueOrDefault())

                Log(e);
        }

        private void Grid_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            if (ManipulationFilter.IsChecked.GetValueOrDefault())

                Log(e);
        }

        private void Grid_StylusButtonDown(object sender, StylusButtonEventArgs e)
        {
            if (!StylusFilter.IsChecked.GetValueOrDefault()) return;
            Log(e, "StylusButtonDown");
        }

        private void Grid_StylusButtonUp(object sender, StylusButtonEventArgs e)
        {
            if (!StylusFilter.IsChecked.GetValueOrDefault()) return;

            Log(e, "StylusButtonUp");
        }

        private void Grid_StylusDown(object sender, StylusDownEventArgs e)
        {
            if (!StylusFilter.IsChecked.GetValueOrDefault()) return;

            Log(e, "StylusDown");
        }

        private void Grid_StylusEnter(object sender, StylusEventArgs e)
        {
            if (!StylusFilter.IsChecked.GetValueOrDefault()) return;

            Log(e, "StylusEnter");
        }

        private void Grid_StylusInAirMove(object sender, StylusEventArgs e)
        {
            if (!StylusFilter.IsChecked.GetValueOrDefault()) return;

            Log(e, "StylesInAirMove");
        }

        private void Grid_StylusInRange(object sender, StylusEventArgs e)
        {
            if (!StylusFilter.IsChecked.GetValueOrDefault()) return;

            Log(e, "StylusInRange");
        }

        private void Grid_StylusLeave(object sender, StylusEventArgs e)
        {
            if (!StylusFilter.IsChecked.GetValueOrDefault()) return;

            Log(e, "StylusLeave");

        }

        private void Grid_StylusMove(object sender, StylusEventArgs e)
        {
            if (!StylusFilter.IsChecked.GetValueOrDefault()) return;

            Log(e, "StylusMove");
        }

        private void Grid_StylusOutOfRange(object sender, StylusEventArgs e)
        {
            if (!StylusFilter.IsChecked.GetValueOrDefault()) return;

            Log(e, "StylusOutOfRange");
        }

        private void Grid_StylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
        {
            if (!StylusFilter.IsChecked.GetValueOrDefault()) return;

            Log(e, "StylusSystemGesture");
        }

        private void Grid_StylusUp(object sender, StylusEventArgs e)
        {
            if (!StylusFilter.IsChecked.GetValueOrDefault()) return;

            Log(e, "StylusUp");
        }

        private void EventLog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                Details.Text = string.Empty;
                return;
            }

            var item = e.AddedItems[0] as TouchEvent;
            var properties = item.EventArgs.GetType().GetProperties()
                .Where(p => p.GetValue(item.EventArgs) == null || p.GetValue(item.EventArgs) is not DependencyObject)
                .ToDictionary(p => p.Name, p => GetValueText(p.GetValue(item.EventArgs)));
            var value = JsonConvert.SerializeObject(properties, JsonSettings);
            Details.Text = $"{item.EventName} {value}";
        }

        private string GetValueText(object? v)
        {
            if (v == null)
                return "<null>";

            if (v is StylusDevice device)
                return FormatDevice(device);
            if (v is ReadOnlyCollection<IManipulator>  || v is ManipulationVelocities || v is ManipulationDelta || v is InertiaTranslationBehavior || v is InertiaRotationBehavior || v is InertiaExpansionBehavior)
                return JsonConvert.SerializeObject(v);
            return v.ToString();
        }

        private string FormatDevice(StylusDevice device)
        {
            var isTouch = device.TabletDevice.SupportedStylusPointProperties.Any(pp => pp.Id.Equals(StylusPointProperties.SystemTouch.Id));
            var type = isTouch ? "Finger" : "Pen";
            return $"{device.Id} ({type})";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EventLog.Items.Clear();
        }

        private void Grid_TouchDown(object sender, TouchEventArgs e)
        {
            if (!TouchFilter.IsChecked.GetValueOrDefault()) return;
            Log(e, "TouchDown");
        }

        private void Grid_TouchEnter(object sender, TouchEventArgs e)
        {
            if (!TouchFilter.IsChecked.GetValueOrDefault()) return;
            Log(e, "TouchEnter");
        }

        private void Grid_TouchLeave(object sender, TouchEventArgs e)
        {
            if (!TouchFilter.IsChecked.GetValueOrDefault()) return;
            Log(e, "TouchLeave");
        }

        private void Grid_TouchMove(object sender, TouchEventArgs e)
        {
            if (!TouchFilter.IsChecked.GetValueOrDefault()) return;
            Log(e, "TouchMove");
        }

        private void Grid_TouchUp(object sender, TouchEventArgs e)
        {
            if (!TouchFilter.IsChecked.GetValueOrDefault()) return;
            Log(e, "TouchUp");
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!MouseFilter.IsChecked.GetValueOrDefault()) return;
            Log(e, "MouseDown");
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!MouseFilter.IsChecked.GetValueOrDefault()) return;
            Log(e, "MouseEnter");
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!MouseFilter.IsChecked.GetValueOrDefault()) return;
            Log(e, "MouseLeave");
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MouseFilter.IsChecked.GetValueOrDefault()) return;
            Log(e, "MouseMove");
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!MouseFilter.IsChecked.GetValueOrDefault()) return;
            Log(e, "MouseUp");
        }
    }
}
