using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Resolve
{
    /// <summary>
    ///  Represents a physical body.
    /// </summary>
    public interface IPolygon
    {
        Vector2 Origin { get; set; }
        List<Vector2> Points { get; }
        List<Vector2> Edges { get; }
        RectangleF BoundingBox { get; }
        RectangleF InflatedBoundingBox { get; }

        List<string> Tags { get; set; }
        object Data { get; set; }
        Action<IPolygon, IPolygon> OnCollide { get; }

        Vector2 Center { get; }
        bool IsTangible { get; set; }

        void Move<T>(List<T> polygons, Vector2 vector) where T : IPolygon;
        CollisionResult Simulate<T>(T polygon, Vector2 vector) where T : IPolygon;

        void AddTags(params string[] tags);
        void AddTag(string tag);
        void RemoveTags(params string[] tags);
        void RemoveTag(string tag);
        bool HasTag(string tag);

        void SetData(object data);
        object GetData();

        void SetCallback(Action<IPolygon, IPolygon> callback);

        void Draw(Action<Vector2, Vector2, Color> drawLine, Action<string, Vector2> drawString);
        string ToString();
    }
}
