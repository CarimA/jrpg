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
                Map m = new Map(Game.Content.Load<TiledMap>(directory + "/" + id), id);
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
                    position.Set(position.TileX, CurrentMap.MapNorth.MapHeight - 1);
                    position.SubTileY = 1;
                    Set(CurrentMap.MapNorth.ID);
                    break;
                case Direction.Down:
                    position.Set(position.TileX, 0);
                    position.SubTileY = -1;
                    Set(CurrentMap.MapSouth.ID);
                    break;
                case Direction.Left:
                    position.Set(CurrentMap.MapEast.MapWidth - 1, position.TileY);
                    position.SubTileX = 1;
                    Set(CurrentMap.MapEast.ID);
                    break;
                case Direction.Right:
                    position.Set(0, position.TileY);
                    position.SubTileX = -1;
                    Set(CurrentMap.MapWest.ID);
                    break;
            }
        }

        public void Set(string id)
        {
            // todo: play music
            // or send some event to do that idk

            // also todo: HANDLE EXCEPTIONS SERIOUSLY.

            CurrentMap = Maps.First((m) => m.ID == id);
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

                Set("pocket");
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
