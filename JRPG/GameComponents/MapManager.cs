using JRPG.EntityComponent;
using JRPG.EntityComponent.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JRPG.EntityComponent.Components.PositionComponent;

namespace JRPG
{
    public class MapManager : GameComponent
    {
        public new MainGame Game => (MainGame)base.Game;

        public List<Map> Maps;
        public TiledMapRenderer TmxRenderer;

        public Map CurrentMap;
        public Entity Player;

        public MapManager(Game game, Entity player) : base(game)
        {
            game.Components.Add(this);
            Maps = new List<Map>();
            Player = player;
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        public void LoadMaps(string directory)
        {
            // get all files in a directory
            string[] maps = Directory.GetFiles(directory);
            directory = directory.ToLower().Replace("content/", "");

            // load all maps
            foreach (string map in maps)
            {
                string id = Path.GetFileNameWithoutExtension(map);
                // IF THERE'S AN ERROR HERE ABOUT ID ALREADY FOUND, YOU HAVE TWO LAYERS WITH THE SAME NAME
                var t = Game.Content.Load<TiledMap>(directory + "/" + id);
                Map m = new Map(this, t, id);
                Maps.Add(m);

                // generate a buffer of each map
                m.Buffer((Game as MainGame).SpriteBatch, TmxRenderer);
            }

            // iterate and build adjacency
            foreach (Map map in Maps)
            {
                string north = map.GetProperty("_bound-north");
                string south = map.GetProperty("_bound-south");
                string east = map.GetProperty("_bound-east");
                string west = map.GetProperty("_bound-west");

                if (north != null)
                    map.MapNorth = Maps.First((m) => m.ID == north);
                if (south != null)
                    map.MapSouth = Maps.First((m) => m.ID == south);
                if (east != null)
                    map.MapEast = Maps.First((m) => m.ID == east);
                if (west != null)
                    map.MapWest = Maps.First((m) => m.ID == west);
            }
        }

        public void Move(Direction direction)
        {
            PositionComponent position = Player.GetComponent<PositionComponent>();
            switch (direction)
            {
                case Direction.Up:
                    if (CurrentMap.MapNorth != null)
                    {
                        position.Set(position.Position.X, CurrentMap.MapNorth.PixelHeight - 1);
                        //position.SubTileY = 1;
                        Set(CurrentMap.MapNorth.ID);
                    }
                    break;
                case Direction.Down:
                    if (CurrentMap.MapSouth != null)
                    {
                        position.Set(position.Position.X, 0);
                        //position.SubTileY = -1;
                        Set(CurrentMap.MapSouth.ID);
                    }
                    break;
                case Direction.Left:
                    if (CurrentMap.MapWest != null)
                    {
                        position.Set(CurrentMap.MapWest.PixelWidth - 1, position.Position.Y);
                        //position.SubTileX = 1;
                        Set(CurrentMap.MapWest.ID);
                    }
                    break;
                case Direction.Right:
                    if (CurrentMap.MapEast != null)
                    {
                        position.Set(0, position.Position.Y);
                        //position.SubTileX = -1;
                        Set(CurrentMap.MapEast.ID);
                    }
                    break;
            }
        }

        public void Set(string id)
        {

            // also todo: HANDLE EXCEPTIONS SERIOUSLY.

            CurrentMap = Maps.First((m) => m.ID == id);
            Game.Audio.PlayBGM(CurrentMap.GetProperty("music"));
            // Game.SuppressDraw();

        }

        bool init = false;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!init)
            {
                TmxRenderer = new TiledMapRenderer(Game.GraphicsDevice);
                LoadMaps("content/maps");

                Set("town1");
                init = true;
            }

            /*PositionComponent position = Player.GetComponent<PositionComponent>();
            if (position.TileY < 0)
            {
                Move(Direction.Up);
            }
            if (position.TileX < 0)
            {
                Move(Direction.Left);
            }
            if (position.TileY >= CurrentMap.MapHeight)
            {
                Move(Direction.Down);
            }
            if (position.TileX >= CurrentMap.MapWidth)
            {
                Move(Direction.Right);
            }*/


        }

        public void DrawMask(SpriteBatch spriteBatch)
        {
            if (CurrentMap != null)
                CurrentMap.Draw(spriteBatch, Vector2.Zero, true, true);
        }

        public void DrawFringe(SpriteBatch spriteBatch)
        {
            if (CurrentMap != null)
                CurrentMap.Draw(spriteBatch, Vector2.Zero, false, true);
        }
    }
}
