using Newtonsoft.Json;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using touchtest.Drawing;

namespace touchtest
{
    /// <summary>
    /// Interaction logic for RawEventViewer.xaml
    /// </summary>
    public partial class PaintInputViewer : UserControl
    {
        private static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() },
            Formatting = Formatting.Indented
        };
        private PaintInputProcessor _paintInputProcessor;

        public PaintInputViewer()
        {
            InitializeComponent();
            _paintInputProcessor = new PaintInputProcessor(TouchControl);
            _paintInputProcessor.StrokeStarted += args => Log(args, "StrokeStarted");
            _paintInputProcessor.StrokePointAdded += args => Log(args, "StrokePointAdded");
            _paintInputProcessor.StrokeCompleted += args => Log(args, "StrokeCompleted");
            _paintInputProcessor.StrokeCanceled += args => Log(args, "StrokeCanceled");
            _paintInputProcessor.MultiFingerTap += args => Log(args, "MultiFingerTap");
            _paintInputProcessor.SingleFingerHoldStarted += args => Log(args, "SingleFingerHoldStarted");
            _paintInputProcessor.SingleFingerHoldMoved += args => Log(args, "SingleFingerHoldMoved");
            _paintInputProcessor.SingleFingerHoldEnded += () => Log(null, "SingleFingerHoldEnded");
            _paintInputProcessor.ManipulationStarted += () => Log(null, "ManipulationStarted");
            _paintInputProcessor.ManipulationUpdated += () => Log(null, "ManipulationUpdated");
            _paintInputProcessor.ManipulationEnded += () => Log(null, "ManipulationEnded");
            _paintInputProcessor.DotPlaced += args=> Log(args, "DotPlaced");
        }

        private void Log(object? @event, string? eventName = null)
        {
            Dispatcher.Invoke(() =>
            {
                eventName ??= @event.GetType().Name.Substring(0, @event.GetType().Name.Length - 9);
                EventLog.Items.Insert(0, new TouchEvent(eventName, @event));
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EventLog.Items.Clear();
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

            return v.ToString();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            _paintInputProcessor.DisableFingerPainting = DisableFingerPaintingCheckBox.IsChecked.Value;
        }
    }
}
