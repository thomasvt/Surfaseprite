namespace Surfaseprite.Input
{
    /// <summary>
    /// Occurs for each point added to a <see cref="Stroke"/>. The new point will always be the last item in the Stroke's Points collection.
    /// </summary>
    /// <param name="Stroke"></param>
    public record StrokeEventArgs(Stroke Stroke)
    {
        public DeviceInfo DeviceInfo => Stroke.DeviceInfo;
    }
}
