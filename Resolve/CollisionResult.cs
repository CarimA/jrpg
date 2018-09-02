using Microsoft.Xna.Framework;

namespace Resolve
{
    public struct CollisionResult
    {
        public bool WillIntersect;
        public bool AreIntersecting;
        public Vector2 MinimumTranslation;
    }
}
