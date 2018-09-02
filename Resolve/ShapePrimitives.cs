using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Resolve
{
    public static class ShapePrimitives
    {
        /// <summary>
        /// Creates an approximation of a circle based on the numbed of n-sides.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <param name="sides"></param>
        /// <returns></returns>
        public static Polygon Circle(Vector2 origin, float radius, int sides)
        {
            List<Vector2> points = new List<Vector2>();

            float angle = (float)Math.PI * 2 / (float)sides;

            for (int i = 0; i < sides; i++)
            {
                points.Add(new Vector2(radius * (float)Math.Cos(angle * i), radius * (float)Math.Sin(angle * i)));
            }

            return new Polygon(origin, points);
        }

        public static Polygon BezelRectangle(Vector2 min, Vector2 max, float bezelLength) => BezelRectangle(min.X, min.Y, max.X, max.Y, bezelLength);
        public static Polygon BezelRectangle(float aX, float aY, float bX, float bY, float bezelLength)
        {
            // todo: fix origin assumption flaw
            List<Vector2> points = new List<Vector2>();
            points.Add(new Vector2(bezelLength, 0));
            points.Add(new Vector2(bX - aX - bezelLength, 0));
            points.Add(new Vector2(bX - aX, bezelLength));
            points.Add(new Vector2(bX - aX, bY - aY - bezelLength));
            points.Add(new Vector2(bX - aX - bezelLength, bY - aY));
            points.Add(new Vector2(bezelLength, bY - aY));
            points.Add(new Vector2(0, bY - aY - bezelLength));
            points.Add(new Vector2(0, bezelLength));

            return new Polygon(new Vector2(aX, aY), points);
        }
        public static Polygon BezelRectangle(Vector2 origin, float width, float height, float bezelLength) // blech, I'll figure something robust out later
        {
            List<Vector2> points = new List<Vector2>();
            points.Add(new Vector2(bezelLength, 0));
            points.Add(new Vector2(width - bezelLength, 0));
            points.Add(new Vector2(width, bezelLength));
            points.Add(new Vector2(width, height - bezelLength));
            points.Add(new Vector2(width - bezelLength, height));
            points.Add(new Vector2(bezelLength, height));
            points.Add(new Vector2(0, height - bezelLength));
            points.Add(new Vector2(0, bezelLength));
            return new Polygon(origin, points);
        }

        public static Polygon Rectangle(Vector2 min, Vector2 max) =>  Rectangle(min.X, min.Y, max.X, max.Y);
        public static Polygon Rectangle(float aX, float aY, float bX, float bY)
        {
            List<Vector2> points = new List<Vector2>();
            points.Add(new Vector2(0, 0));
            points.Add(new Vector2(bX - aX, 0));
            points.Add(new Vector2(bX - aX, bY - aY));
            points.Add(new Vector2(0, bY - aY));
            return new Polygon(new Vector2(aX, aY), points);
        }

        public static Polygon Rectangle(Vector2 origin, float width, float height) // blech, I'll figure something robust out later
        {
            List<Vector2> points = new List<Vector2>();
            points.Add(new Vector2(0, 0));
            points.Add(new Vector2(width, 0));
            points.Add(new Vector2(width, height));
            points.Add(new Vector2(0, height));
            return new Polygon(origin, points);
        }
    }
}
