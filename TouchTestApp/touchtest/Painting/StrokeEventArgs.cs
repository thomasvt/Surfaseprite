namespace touchtest.Drawing
{
    public record StrokeEventArgs(Stroke Stroke)
    {
        public DeviceInfo DeviceInfo => Stroke.DeviceInfo;
    }
}
