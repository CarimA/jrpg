using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG
{
    public class Map
    {
        private MapManager mapManager;

        public string ID;
        public string Name;

        public TiledMap MapData;

        public bool[,] Collision;
        public List<string>[,] Scripts;

        public Map MapNorth;
        public Map MapSouth;
        public Map MapEast;
        public Map MapWest;

        public Texture2D Mask;
        public Texture2D Fringe;

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
            Collision = new bool[MapWidth, MapHeight];
            Scripts = new List<string>[MapWidth, MapHeight];
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
                                Collision[x, y] = true;
                            }
                        }

                    }

                    if (obj is TiledMapRectangleObject && obj.Properties.Where(x => x.Key == "script").Count() > 0)
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
                                if (Scripts[x, y] == null)
                                {
                                    Scripts[x, y] = new List<string>();
                                }
                                Scripts[x, y].Add(obj.Properties["script"]);
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

            if (Scripts[x, y] != null)
            {
                foreach (var c in Scripts[x, y])
                {
                    mapManager.Game.ScriptingManager.RunScript(c);
                }
            }
        }

        public bool SolidAt(int x, int y)
        {
            // if it's out of bounds, find the according map and try to place it there
            // if it's still out of bounds or a map doesn't exist there, just return true

            if (x < 0)
            {
                return MapWest == null ? true : MapWest.SolidAt(MapWest.MapWidth + x - 1, y);
            }
            if (x >= MapWidth)
            {
                return MapEast == null ? true : MapEast.SolidAt(MapWidth - x, y);
            }
            if (y < 0)
            {
                return MapNorth == null ? true : MapNorth.SolidAt(x, MapNorth.MapHeight + y - 1);
            }
            if (y >= MapHeight)
            {
                return MapSouth == null ? true : MapSouth.SolidAt(x, MapHeight - y);
            }
            

            return Collision[x, y]; 

            /*
            return MapData.TileLayers.All((layer) =>
            {
                TiledMapTile? tile;
                if (layer.TryGetTile(x, y,, out tile))
                {
                    if (tile.HasValue)
                    {
                        // todo: go through tile properties and find if "c" is set
                        tile.Value
                        MapData.Tilesets[0].
                    }
                }

                
            }*/
        }
    }
}
