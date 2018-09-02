using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resolve
{
    public class Quadtree<T> where T : IPolygon
    {
        protected enum Corner : int
        {
            BottomLeft = 0,
            BottomRight,
            TopLeft,
            TopRight,
            Left,
            Right,
            Top, 
            Bottom,
            All
        }

        const int MAX_OBJECTS = 8;
        const int MAX_LEVELS = 6;

        private int level;
        private List<T> objects;
        private RectangleF boundaries;

        private RectangleF topLeftBounds;
        private RectangleF topRightBounds;
        private RectangleF bottomLeftBounds;
        private RectangleF bottomRightBounds;
        private RectangleF leftBounds;
        private RectangleF rightBounds;
        private RectangleF topBounds;
        private RectangleF bottomBounds;

        private Quadtree<T>[] nodes;

        public Quadtree(RectangleF worldBounds)
        {
            level = 1;
            objects = new List<T>();
            boundaries = worldBounds;
            setCornerBounderies();
            nodes = new Quadtree<T>[4];
            split();
        }

        private Quadtree(int nodeLevel, RectangleF nodeBounds)
        {
            level = nodeLevel;
            objects = new List<T>();
            boundaries = nodeBounds;
            setCornerBounderies();
            nodes = new Quadtree<T>[4];
        }

        private void setCornerBounderies()
        {
            float halfWidth = boundaries.Width / 2;
            float halfHeight = boundaries.Height / 2;
            topLeftBounds = new RectangleF(boundaries.Left, boundaries.Top, halfWidth, halfHeight);
            topRightBounds = new RectangleF(boundaries.Left + halfWidth, boundaries.Top, halfWidth, halfHeight);
            bottomLeftBounds = new RectangleF(boundaries.Left, boundaries.Top + halfHeight, halfWidth, halfHeight);
            bottomRightBounds = new RectangleF(boundaries.Left + halfWidth, boundaries.Top + halfHeight, halfWidth, halfHeight);

            topBounds = new RectangleF(boundaries.Left, boundaries.Top, boundaries.Width, halfHeight);
            bottomBounds = new RectangleF(bottomBounds.Left, bottomBounds.Top + halfHeight, boundaries.Width, halfHeight);
            leftBounds = new RectangleF(boundaries.Left, boundaries.Top, halfWidth, boundaries.Height);
            rightBounds = new RectangleF(boundaries.Left + halfWidth, boundaries.Top, halfWidth, boundaries.Height);
        }

        public void Clear()
        {
            objects.Clear();
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].Clear();
                nodes[i] = null;
            }
        }

        private void split() {
            float x = boundaries.X;
            float y = boundaries.Y;
            float subWidth = boundaries.Width / 2;
            float subHeight = boundaries.Height / 2;
            int nextLevel = level + 1;

            nodes[(int)Corner.TopRight] = new Quadtree<T>(nextLevel, new RectangleF(x + subWidth, y, subWidth, subHeight));
            nodes[(int)Corner.TopLeft] = new Quadtree<T>(nextLevel, new RectangleF(x, y, subWidth, subHeight));
            nodes[(int)Corner.BottomLeft] = new Quadtree<T>(nextLevel, new RectangleF(x, y + subHeight, subWidth, subHeight));
            nodes[(int)Corner.BottomRight] = new Quadtree<T>(nextLevel, new RectangleF(x + subWidth, y + subHeight, subWidth, subHeight));
        }

        private Corner getIndex(T polygon)
        {
            return getIndex(new RectangleF(polygon.Origin.X + polygon.BoundingBox.Left, polygon.Origin.Y + polygon.BoundingBox.Top, polygon.BoundingBox.Width, polygon.BoundingBox.Height));
        }

        private Corner getIndex(RectangleF bound, bool allowEdges = false)
        {
            // determines if an object fits into a node completely
            // returns none if it cannot completely fit and is part of the parent
            Corner index = Corner.All;

            if (topLeftBounds.Contains(bound))
            {
                index = Corner.TopLeft;
            }
            else if (topRightBounds.Contains(bound))
            {
                index = Corner.TopRight;
            }
            else if (bottomLeftBounds.Contains(bound))
            {
                index = Corner.BottomLeft;
            }
            else if (bottomRightBounds.Contains(bound))
            {
                index = Corner.BottomRight;
            }

            if (allowEdges && index == Corner.All)
            {
                if (topBounds.Contains(bound))
                {
                    index = Corner.Top;
                }
                else if (bottomBounds.Contains(bound))
                {
                    index = Corner.Bottom;
                }
                else if (leftBounds.Contains(bound))
                {
                    index = Corner.Left;
                }
                else if (rightBounds.Contains(bound))
                {
                    index = Corner.Right;
                }
            }

            return index;
        }

        public void Insert(params T[] polygons)
        {
            foreach (T polygon in polygons)
            {
                Insert(polygon);
            }
        }

        public void Insert(T polygon)
        {
            if (nodes[0] != null)
            {
                Corner index = getIndex(polygon);
                if (index != Corner.All)
                {
                    nodes[(int)index].Insert(polygon);
                    return;
                }
            }

            objects.Add(polygon);

            if (objects.Count > MAX_OBJECTS - 1 && level < MAX_LEVELS)
            {
                if (nodes[0] == null)
                {
                    split();
                }

                for (int i = objects.Count - 1; i >= 0; i--)
                {
                    Corner index = getIndex(objects[i]);
                    if (index != Corner.All)
                    {
                        // add to child and flag for removal
                        nodes[(int)index].Insert(objects[i]);
                        objects.RemoveAt(i);
                    }
                }
            }
        }

        public List<T> Retrieve(RectangleF bounds)
        {
            List<T> items = new List<T>();

            Corner index = getIndex(bounds, true);
            if ((index != Corner.All && index != Corner.Top && index != Corner.Bottom && index != Corner.Left && index != Corner.Right) && nodes[0] != null)
            {
                items.AddRange(nodes[(int)index].Retrieve(bounds));
            } 
            // todo: optimise for checking if you just want the two left quadrants or two right quadrants etc
            // instead of returning EVERYTHING
            else 
            {
                switch (index)
                {
                    case Corner.All:
                        for (int i = 0; i < 4; i++)
                        {
                            if (nodes[i] != null)
                            {
                                items.AddRange(nodes[i].Retrieve(bounds));
                            }
                        }
                        break;
                    case Corner.Top:
                        items.AddRangeIgnoreNull(nodes[(int)Corner.TopLeft]?.Retrieve(bounds));
                        items.AddRangeIgnoreNull(nodes[(int)Corner.TopRight]?.Retrieve(bounds));
                        break;
                    case Corner.Bottom:
                        items.AddRangeIgnoreNull(nodes[(int)Corner.BottomLeft]?.Retrieve(bounds));
                        items.AddRangeIgnoreNull(nodes[(int)Corner.BottomRight]?.Retrieve(bounds));
                        break;
                    case Corner.Left:
                        items.AddRangeIgnoreNull(nodes[(int)Corner.TopLeft]?.Retrieve(bounds));
                        items.AddRangeIgnoreNull(nodes[(int)Corner.BottomLeft]?.Retrieve(bounds));
                        break;
                    case Corner.Right:
                        items.AddRangeIgnoreNull(nodes[(int)Corner.BottomRight]?.Retrieve(bounds));
                        items.AddRangeIgnoreNull(nodes[(int)Corner.TopRight]?.Retrieve(bounds));
                        break;

                }                
            }

            items.AddRange(objects);
            return items;
        }

        public void Draw(Action<Vector2, Vector2, Color> drawLine, Action<string, Vector2> drawString)
        {
            float subWidth = boundaries.Width / 2;
            float subHeight = boundaries.Height / 2;
            float halfX = boundaries.X + subWidth;
            float halfY = boundaries.Y + subHeight;

            int? total = nodes[0]?.objects.Count + nodes[1]?.objects.Count + nodes[2]?.objects.Count + nodes[3]?.objects.Count;
            if (total.HasValue && total.Value > 0)
            {
                drawLine(new Vector2(halfX, boundaries.Y), new Vector2(halfX, boundaries.Bottom), Color.Red);
                drawLine(new Vector2(boundaries.X, halfY), new Vector2(boundaries.Right, halfY), Color.Red);
            }

            nodes[0]?.Draw(drawLine, drawString);
            nodes[1]?.Draw(drawLine, drawString);
            nodes[2]?.Draw(drawLine, drawString);
            nodes[3]?.Draw(drawLine, drawString);

            if (level > 3)
            {
                while (true)
                {
                    break;
                }
            }

            foreach (T polygon in objects)
            {
                polygon.Draw(drawLine, drawString);
            }
        }

        public void ClearDebugTag()
        {
            foreach (T poly in objects)
            {
                poly.RemoveTag("debug");
            }

            nodes[0]?.ClearDebugTag();
            nodes[1]?.ClearDebugTag();
            nodes[2]?.ClearDebugTag();
            nodes[3]?.ClearDebugTag();
        }
    }
}
