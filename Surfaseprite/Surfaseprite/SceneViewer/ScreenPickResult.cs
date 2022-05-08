using System.Numerics;

namespace Surfaseprite.SceneViewer
{
    public record ScreenPickResult(SceneItem SceneItem, Vector2 HitTexelCoords);
}
