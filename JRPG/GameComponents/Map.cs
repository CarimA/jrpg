using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using Resolve;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG
{
    public class ScriptData
    {
        public string Script;
    }

    public class MapObject<T>
    {
        public int X;
        public int Y;
        public T Data;

        public static List<MapObject<T>> Find(List<MapObject<T>> mapObjects, int X, int Y)
        {
            return mapObjects.FindAll(m => m.X == X && m.Y == Y);
        }
    }

    public class Map
    {
        private MapManager mapManager;

        public string ID;
        public string Name;

        public TiledMap MapData;
        
        public List<MapObject<ScriptData>> InteractScripts;
        public List<MapObject<ScriptData>> WalkoverScripts;

        public Map MapNorth;
        public Map MapSouth;
        public Map MapEast;
        public Map MapWest;

        public Texture2D Mask;
        public Texture2D Fringe;

        public Quadtree<Polygon> World;

        public int PixelHeight => MapData.HeightInPixels;
        public int PixelWidth => MapData.WidthInPixels;

        public int MapHeight => MapData.Height;
        public int MapWidth => MapData.Width;

        public Map(MapManager manager, TiledMap map, string id)
        {
            mapManager = manager;
            MapData = map;
            ID = id;
            Name = GetProperty("name");
            InteractScripts = new List<MapObject<ScriptData>>();
            WalkoverScripts = new List<MapObject<ScriptData>>();

            World = new Quadtree<Polygon>(new Resolve.RectangleF(0, 0, PixelWidth, PixelHeight));

            LoadBoundaryData();
        }

        public void LoadBoundaryData()
        {
            foreach (var objectLayer in MapData.ObjectLayers)
            {
                foreach (var obj in objectLayer.Objects)
                {
                    if (objectLayer.Name.Contains("collision") && obj is TiledMapRectangleObject)
                    {
                        if (obj is TiledMapPolygonObject)
                        {
                            Point2[] points = (obj as TiledMapPolygonObject).Points;

                            List<Vector2> newPoints = points.Select(point => new Vector2(point.X, point.Y)).ToList();
                            World.Insert(new Polygon(obj.Position, newPoints));
                        }
                        else if (obj is TiledMapRectangleObject)
                        {
                            TiledMapRectangleObject rect = obj as TiledMapRectangleObject;
                            Polygon p = ShapePrimitives.Rectangle(obj.Position, (obj as TiledMapRectangleObject).Size.Width, (obj as TiledMapRectangleObject).Size.Height);
                            World.Insert(p);
                        }
                        else
                        {
                            throw new Exception("uhh something's missing :>");
                        }
                    }

                    if (objectLayer.Name.Contains("scripts") && obj is TiledMapRectangleObject)
                    {

                        Vector2 pos = (obj as TiledMapRectangleObject).Position;
                        Size2 size = (obj as TiledMapRectangleObject).Size;

                        Rectangle rect = new Rectangle(
                            (int)(pos.X / MapData.TileWidth),
                            (int)(pos.Y / MapData.TileHeight),
                            (int)(size.Width / MapData.TileWidth),
                            (int)(size.Height / MapData.TileHeight));

                        for (int x = rect.Left; x < rect.Right; x++)
                        {
                            for (int y = rect.Top; y < rect.Bottom; y++)
                            {
                                InteractScripts.Add(new MapObject<ScriptData>()
                                {
                                    X = x,
                                    Y = y,
                                    Data = new ScriptData()
                                    {
                                        Script = obj.Name
                                    }
                                });
                            }
                        }
                    }

                    // todo: handle scripts, warps, etc etc
                }
            }
        }

        public void Buffer(SpriteBatch spriteBatch, TiledMapRenderer tmxRenderer)
        {
            Mask = BufferToTexture(spriteBatch, tmxRenderer, "mask");
            Fringe = BufferToTexture(spriteBatch, tmxRenderer, "fringe");
        }

        private Texture2D BufferToTexture(SpriteBatch spriteBatch, TiledMapRenderer tmxRenderer, string requiredLayers)
        {
            // make each layer visible
            foreach (var layer in MapData.Layers)
            {
                layer.IsVisible = true;
            }

            // check what should be displayed (if any changes are needed at all)
            if (!string.IsNullOrEmpty(requiredLayers))
            {
                // disable layers that don't match in name
                foreach (var layer in MapData.Layers)
                {
                    if (!layer.Name.Contains(requiredLayers))
                        layer.IsVisible = false;
                }
            }

            // create a framebuffer matching the needed size...
            RenderTarget2D renderTarget = new RenderTarget2D(spriteBatch.GraphicsDevice, MapWidth * MainGame.TILE_SIZE, MapHeight * MainGame.TILE_SIZE);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);

            spriteBatch.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp);
            tmxRenderer.Draw(MapData);
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);

            // send the texture back
            return (Texture2D)renderTarget;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, bool mask, bool recurse = false)
        {
            Texture2D tex = mask ? Mask : Fringe;
            spriteBatch.Draw(tex, position, Color.White);

            if (recurse)
            {
                if (MapNorth != null)
                {
                    MapNorth.Draw(spriteBatch, new Vector2(0, -MapNorth.PixelHeight), mask);

                    if (MapNorth.MapEast != null)
                    {
                        MapNorth.MapEast.Draw(spriteBatch, new Vector2(PixelWidth, -MapNorth.MapEast.PixelHeight), mask);
                    }

                    if (MapNorth.MapWest != null)
                    {
                        MapNorth.MapWest.Draw(spriteBatch, new Vector2(-MapNorth.MapWest.PixelWidth, -MapNorth.MapWest.PixelHeight), mask);
                    }
                }

                if (MapSouth != null)
                {
                    MapSouth.Draw(spriteBatch, new Vector2(0, PixelHeight), mask);

                    if (MapSouth.MapEast != null)
                    {
                        MapSouth.MapEast.Draw(spriteBatch, new Vector2(PixelWidth, PixelHeight), mask);
                    }

                    if (MapSouth.MapWest != null)
                    {
                        MapSouth.MapWest.Draw(spriteBatch, new Vector2(-MapSouth.MapWest.PixelWidth, PixelHeight), mask);
                    }
                }

                if (MapEast != null)
                {
                    MapEast.Draw(spriteBatch, new Vector2(PixelWidth, 0), mask);
                }

                if (MapWest != null)
                {
                    MapWest.Draw(spriteBatch, new Vector2(-MapWest.PixelWidth, 0), mask);
                }
            }
        }

        public string GetProperty(string id) => MapData.Properties.ContainsKey(id) ? MapData.Properties[id] : null;

        public void Interact(int x, int y)
        {
            // if it's out of bounds, find the according map and try to place it there
            // if it's still out of bounds or a map doesn't exist there, just return true

            if (x < 0)
            {
                MapWest?.Interact(MapWest.MapWidth + x - 1, y);
                return;
            }
            if (x >= MapWidth)
            {
                MapEast?.Interact(MapWidth - x, y);
                return;
            }
            if (y < 0)
            {
                MapNorth?.Interact(x, MapNorth.MapHeight + y - 1);
                return;
            }
            if (y >= MapHeight)
            {
                MapSouth?.Interact(x, MapHeight - y);
                return;
            }

            foreach (var c in MapObject<ScriptData>.Find(InteractScripts as List<MapObject<ScriptData>>, x, y))
            {
                // todo
                //mapManager.Game.ScriptingManager.RunScript(c.Data.Script);
            }
        }
    }
}
