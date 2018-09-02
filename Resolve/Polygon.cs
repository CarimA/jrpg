using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Resolve
{
    public class Polygon : IPolygon
    {
        const int INFLATE = 10;

        public Vector2 Position { get { return Origin; } set { Origin = value; } }
        public Vector2 Origin { get; set; }

        public List<Vector2> Points { get; private set; }
        public List<Vector2> Edges { get; private set; }
        public RectangleF BoundingBox { get; private set; }
        public RectangleF InflatedBoundingBox { get; private set; }

        public List<string> Tags { get; set; }
        public object Data { get; set; }
        public Action<IPolygon, IPolygon> OnCollide { get; private set; }

        private Vector2 center;
        public Vector2 Center { get { return Origin + center; } private set { center = value; } }

        public bool IsTangible { get; set; }

        public Polygon(Vector2 origin, List<Vector2> points)
        {
            Origin = origin;
            Points = points;
            Edges = new List<Vector2>();
            Tags = new List<string>();
            IsTangible = true;

            // build edges
            Vector2 p1, p2;
            for (int i = 0; i < Points.Count; i++)
            {
                p1 = Points[i];
                if (i + 1 >= Points.Count)
                {
                    p2 = Points[0];
                }
                else
                {
                    p2 = Points[i + 1];
                }
                Edges.Add(p2 - p1);
            }

            // iterate over points and find lowest + highest
            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;

            foreach (Vector2 point in Points)
            {
                if (point.X < minX)
                {
                    minX = point.X;
                }

                if (point.Y < minY)
                {
                    minY = point.Y;
                }

                if (point.X > maxX)
                {
                    maxX = point.X;
                }

                if (point.Y > maxY)
                {
                    maxY = point.Y;
                }
            }

            BoundingBox = new RectangleF(minX, minY, maxX - minX, maxY - minY);

            InflatedBoundingBox = new RectangleF(BoundingBox.X - INFLATE, BoundingBox.Y - INFLATE, 
                BoundingBox.Width + (INFLATE * 2), BoundingBox.Height + (INFLATE * 2));

            // calculate center
            float tX = 0, tY = 0;
            foreach (Vector2 point in Points)
            {
                tX += point.X;
                tY += point.Y;
            }

            Center = new Vector2(tX / (float)Points.Count, tY / (float)Points.Count);
        }

        public void Move<T>(List<T> polygons, Vector2 velocity) where T: IPolygon
        {
            Vector2 translation = velocity;
            List<Vector2> minimumTranslations = new List<Vector2>();

            InflatedBoundingBox.Set(new Vector2(Origin.X - INFLATE, Origin.Y - INFLATE));

            foreach (T polygon in polygons)
            {
                // don't do work that doesn't need to be done.
                if (this.InflatedBoundingBox.Intersects(polygon.InflatedBoundingBox))
                {
                    CollisionResult result = Simulate(polygon, velocity);

                    if (result.WillIntersect && this.IsTangible && polygon.IsTangible)
                    {
                        minimumTranslations.Add(result.MinimumTranslation);
                        //translation += result.MinimumTranslation;
                        //break;
                    }

                    if (result.AreIntersecting)
                    {
                        this.OnCollide?.Invoke(this, polygon);
                        polygon.OnCollide?.Invoke(polygon, this);
                    }
                }
            }

            if (minimumTranslations.Count > 0)
            {
                if (minimumTranslations.Count == 1)
                {
                    translation += minimumTranslations.First();
                }
                else
                {
                    //Vector2 average = new Vector2(minimumTranslations.Min(x => x.X), minimumTranslations.Min(x => x.Y));
                    Vector2 average = GetFurthestFromOrigin(minimumTranslations);
                    translation += average;
                    //translation += (new Vector2(Math.Sign(average.X), Math.Sign(average.Y)) * 0.1f);
                }
            }
            Position += translation;
        }

        private Vector2 GetFurthestFromOrigin(IEnumerable<Vector2> points)
        {
            float x = 0;
            float y = 0;
            foreach (Vector2 point in points)
            {
                if (Math.Abs(point.X) > Math.Abs(x))
                {
                    x = point.X;
                }
                if (Math.Abs(point.Y) > Math.Abs(y))
                {
                    y = point.Y;
                }
            }
            return new Vector2(x, y);
        }
        
        public CollisionResult Simulate<T>(T polygon, Vector2 velocity) where T : IPolygon
        {
            CollisionResult result = new CollisionResult();
            result.AreIntersecting = true;
            result.WillIntersect = true;

            int edgeCountA = this.Edges.Count;
            int edgeCountB = polygon.Edges.Count;
            float minimumInterval = float.PositiveInfinity;

            Vector2 translationAxis = Vector2.Zero;
            Vector2 edge = Vector2.Zero;

            // loop through edges of both polygons
            for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++)
            {
                if (edgeIndex < edgeCountA)
                {
                    edge = this.Edges[edgeIndex];
                }
                else
                {
                    edge = polygon.Edges[edgeIndex - edgeCountA];
                }

                // find if the polygons are intersecting.
                // find the axis perpendicular to the current edge
                Vector2 axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // find the projection of the axis
                float minA = 0, minB = 0, maxA = 0, maxB = 0;
                projectPolygon(axis, this, ref minA, ref maxA);
                projectPolygon(axis, polygon, ref minB, ref maxB);

                // check if projections are intersecting
                if (intervalDistance(minA, maxA, minB, maxB) > 0)
                {
                    result.AreIntersecting = false;
                }
               
                // find if polygons _will_ intersect
                float velocityProjection = Vector2.Dot(axis, velocity);

                // get the projection of this poly during movement
                if (velocityProjection < 0)
                {
                    minA += velocityProjection;
                }
                else
                {
                    maxA += velocityProjection;
                }

                // same test for new projection
                float interval = intervalDistance(minA, maxA, minB, maxB);
                if (interval > 0)
                {
                    result.WillIntersect = false;
                }

                if (!result.AreIntersecting && !result.WillIntersect)
                {
                    break;
                }

                // check if the current interval is the minimum one
                interval = Math.Abs(interval);
                if (interval < minimumInterval)
                {
                    minimumInterval = interval;
                    translationAxis = axis;

                    Vector2 d = this.Center - polygon.Center;
                    if (Vector2.Dot(d, translationAxis) < 0)
                    {
                        translationAxis = -translationAxis;
                    }
                }
            }

            if (result.WillIntersect)
            {
                result.MinimumTranslation = translationAxis * minimumInterval;
            }

            return result;
        }

        // Calculate the projection of a polygon on an axis and returns it as a [min, max] interval
        private void projectPolygon(Vector2 axis, IPolygon polygon, ref float min, ref float max)
        {
            // To project a point on an axis use the dot product
            float d = Vector2.Dot(polygon.Origin + polygon.Points[0], axis);
            min = d;
            max = d;
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                d = Vector2.Dot(polygon.Origin + polygon.Points[i], axis);
                if (d < min)
                {
                    min = d;
                }
                else
                {
                    if (d > max)
                    {
                        max = d;
                    }
                }
            }
        }

        // Calculate the distance between [minA, maxA] and [minB, maxB]
        // The distance will be negative if the intervals overlap
        private float intervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
            {
                return minB - maxA;
            }
            else
            {
                return minA - maxB;
            }
        }

        public void AddTag(string tag) => Tags.Add(tag);
        public void AddTags(params string[] tags) => Tags.AddRange(tags);
        public bool HasTag(string tag) => Tags.Contains(tag);
        public void RemoveTag(string tag) => Tags.Remove(tag);
        public void RemoveTags(params string[] tags) => tags.ToList().ForEach(x => RemoveTag(x));

        public void SetData(object data) => Data = data;
        public object GetData() => Data;

        public void SetCallback(Action<IPolygon, IPolygon> callback)
        {
            OnCollide = callback;
        }

        public void Draw(Action<Vector2, Vector2, Color> drawLine, Action<string, Vector2> drawString)
        {
            // draw box
            drawLine(Origin + InflatedBoundingBox.TopLeft, Origin + InflatedBoundingBox.TopRight, Color.Yellow * 0.5f);
            drawLine(Origin + InflatedBoundingBox.TopRight, Origin + InflatedBoundingBox.BottomRight, Color.Yellow * 0.5f);
            drawLine(Origin + InflatedBoundingBox.BottomRight, Origin + InflatedBoundingBox.BottomLeft, Color.Yellow * 0.5f);
            drawLine(Origin + InflatedBoundingBox.TopLeft, Origin + InflatedBoundingBox.BottomLeft, Color.Yellow * 0.5f);

            drawLine(Origin + BoundingBox.TopLeft, Origin + BoundingBox.TopRight, Color.Red * 0.5f);
            drawLine(Origin + BoundingBox.TopRight, Origin + BoundingBox.BottomRight, Color.Red * 0.5f);
            drawLine(Origin + BoundingBox.BottomRight, Origin + BoundingBox.BottomLeft, Color.Red * 0.5f);
            drawLine(Origin + BoundingBox.TopLeft, Origin + BoundingBox.BottomLeft, Color.Red * 0.5f);

            // draw polygon
            Vector2 p1, p2;

            Color c = this.HasTag("debug") ? Color.White : Color.Blue;

            for (int i = 0; i < Points.Count; i++)
            {
                p1 = Points[i];
                if (i + 1 >= Points.Count)
                {
                    p2 = Points[0];
                }
                else
                {
                    p2 = Points[i + 1];
                }
                drawLine(Origin + p1, Origin + p2, c);
            }

            drawLine(Center - new Vector2(5), Center + new Vector2(5), Color.Yellow);
            drawLine(Center - new Vector2(5, -5), Center + new Vector2(5, -5), Color.Yellow);

            drawString("poly", Center);
        }

        public override string ToString()
        {
            string result = "";
            foreach (Vector2 point in Points)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += " ";
                }
                result += point.ToString();
            }
            return result;
        }
    }
}
