#region License
/*
MIT License
Copyright © 2006 The Mono.Xna Team

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System;
using Microsoft.Xna.Framework;

/* This is a modified Monogame Rectangle class to be used with floats instead of ints */

namespace Resolve
{
    public struct RectangleF : IEquatable<RectangleF>
    {
        #region Private Fields

        private static RectangleF emptyRectangle = new RectangleF();

        #endregion Private Fields


        #region Public Fields

        public float X;
        public float Y;
        public float Width;
        public float Height;

        #endregion Public Fields


        #region Public Properties

        public static RectangleF Empty
        {
            get { return emptyRectangle; }
        }

        public float Left
        {
            get { return this.X; }
        }

        public float Right
        {
            get { return (this.X + this.Width); }
        }

        public float Top
        {
            get { return this.Y; }
        }

        public float Bottom
        {
            get { return (this.Y + this.Height); }
        }

        public Vector2 TopLeft { get { return new Vector2(Left, Top); } }
        public Vector2 TopRight { get { return new Vector2(Right, Top); } }
        public Vector2 BottomLeft { get { return new Vector2(Left, Bottom); } }
        public Vector2 BottomRight { get { return new Vector2(Right, Bottom); } }

        #endregion Public Properties


        #region Constructors

        public RectangleF(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        #endregion Constructors


        #region Public Methods

        public static bool operator ==(RectangleF a, RectangleF b)
        {
            return ((a.X == b.X) && (a.Y == b.Y) && (a.Width == b.Width) && (a.Height == b.Height));
        }

        public bool Contains(float x, float y)
        {
            return ((((this.X <= x) && (x < (this.X + this.Width))) && (this.Y <= y)) && (y < (this.Y + this.Height)));
        }

        public bool Contains(Vector2 value)
        {
            return ((((this.X <= value.X) && (value.X < (this.X + this.Width))) && (this.Y <= value.Y)) && (value.Y < (this.Y + this.Height)));
        }

        public bool Contains(Point value)
        {
            return ((((this.X <= value.X) && (value.X < (this.X + this.Width))) && (this.Y <= value.Y)) && (value.Y < (this.Y + this.Height)));
        }

        public bool Contains(RectangleF value)
        {
            return ((((this.X <= value.X) && ((value.X + value.Width) <= (this.X + this.Width))) && (this.Y <= value.Y)) && ((value.Y + value.Height) <= (this.Y + this.Height)));
        }

        public static bool operator !=(RectangleF a, RectangleF b)
        {
            return !(a == b);
        }

        public void Offset(Vector2 offset)
        {
            X += offset.X;
            Y += offset.Y;
        }

        public void Offset(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public void Set(Vector2 position)
        {
            X = position.X;
            Y = position.Y;
        }

        public Vector2 Center
        {
            get
            {
                return new Vector2((this.X + this.Width) / 2f, (this.Y + this.Height) / 2f);
            }
        }
        
        public void Inflate(float horizontalValue, float verticalValue)
        {
            X -= horizontalValue;
            Y -= verticalValue;
            Width += horizontalValue * 2;
            Height += verticalValue * 2;
        }

        public bool IsEmpty
        {
            get
            {
                return ((((this.Width == 0) && (this.Height == 0)) && (this.X == 0)) && (this.Y == 0));
            }
        }

        public bool Equals(RectangleF other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return (obj is RectangleF) ? this == ((RectangleF)obj) : false;
        }

        public override string ToString()
        {
            return string.Format("{{X:{0} Y:{1} Width:{2} Height:{3}}}", X, Y, Width, Height);
        }

        public override int GetHashCode()
        {
            // todo: determine a good hash code
            return (int)(this.X * this.Y * this.Width * this.Height) * ((int)this.X ^ (int)this.Y ^ (int)this.Width ^ (int)this.Height);
        }

        public bool Intersects(RectangleF r2)
        {
            return !(r2.Left > Right
                     || r2.Right < Left
                     || r2.Top > Bottom
                     || r2.Bottom < Top
                    );

        }


        public void Intersects(ref RectangleF value, out bool result)
        {
            result = !(value.Left > Right
                     || value.Right < Left
                     || value.Top > Bottom
                     || value.Bottom < Top
                    );

        }

        #endregion Public Methods
    }
}