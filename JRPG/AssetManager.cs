using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG
{
    public class AssetManager : GameComponent
    {
        private ContentManager _content;
        private Dictionary<Asset, ReferencedAsset> _assets;
        private float _flushTimer = 10f;

        public AssetManager(Game game, ContentManager content) : base(game)
        {
            game.Components.Add(this);

            _content = content;
            _assets = new Dictionary<Asset, ReferencedAsset>();
        }

        public enum Asset
        {
            DebugPlayer = 0,
            Font,
            InterfacesColourTable,
            InterfacesControls,
            InterfacesAtlas,
            ScriptsTest,
            TextEnglish,
            TilesetsNewTileset,
            TilesetsPocketTileset,
            ShadersPalette
        }

        public static AssetManager LoadAll(Game game, ContentManager content)
        {
            AssetManager am = new AssetManager(game, content);

            // Content/Debug
            am.Add<Texture2D>(Asset.DebugPlayer, "Debug/Player");

            // Content/Fonts
            am.Add<BitmapFont>(Asset.Font, "Fonts/Pixellari");

            // Content/Interfaces
            am.Add<Texture2D>(Asset.InterfacesColourTable, "Interfaces/ColourTable");
            am.Add<Texture2D>(Asset.InterfacesControls, "Interfaces/Controls");
            am.Add<Texture2D>(Asset.InterfacesAtlas, "Interfaces/Atlas");

            // Content/Scripts
            am.Add<string>(Asset.ScriptsTest, "Scripts/Test");

            // Content/Text
            am.Add<string>(Asset.TextEnglish, "Text/English");

            // Content/Tilesets
            am.Add<Texture2D>(Asset.TilesetsNewTileset, "Tilesets/new-tileset");
            am.Add<Texture2D>(Asset.TilesetsPocketTileset, "Tilesets/pocket-tileset");

            // Content
            am.Add<Effect>(Asset.ShadersPalette, "palette");

            return am;
        }

        public void Add<T>(Asset key, string filepath) where T : class
        {
            _assets.Add(key, new ReferencedAsset<T>(_content, filepath));
        }

        public void LoadAssets(params Asset[] assets)
        {
            Flush();
            foreach (var asset in assets)
            {
                _assets[asset].Load();
            }
        }

        public T Get<T>(Asset key) where T : class
        {
            return (_assets[key] as ReferencedAsset<T>).Get();
        }

        public void Flush()
        {
            foreach (var asset in _assets)
            {
                asset.Value.Flush();
            }
        }

        public override void Update(GameTime gameTime)
        {
            _flushTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_flushTimer < 0f)
            {
                Flush();
                _flushTimer = 10f;
            }


            base.Update(gameTime);
        }
    }

    public abstract class ReferencedAsset
    {
        protected ContentManager _content;
        protected string _filepath;
        protected int _lastUsed;

        public ReferencedAsset(ContentManager content, string filepath)
        {
            _content = content;
            _filepath = filepath;
            _lastUsed = 0;
        }

        public abstract void Load();
        public abstract void Unload();

        public void Flush()
        {
            if (_lastUsed < Environment.TickCount - (8 * 1000))
            {
                _lastUsed = 0;
                Unload();
            }
        }
    }

    public class ReferencedAsset<T> : ReferencedAsset where T : class
    {
        private T _asset;

        public ReferencedAsset(ContentManager content, string filepath) : base(content, filepath)
        {
        }

        public override void Load()
        {
            if (_asset != null)
            {
                return;
            }

            if (typeof(T) == typeof(string))
            {
                _asset = File.ReadAllText(_filepath) as T;
            }
            else
            {
                _asset = _content.Load<T>(_filepath);
            }
            Console.WriteLine($"Loaded asset: {_filepath}");
        }

        public override void Unload()
        {
            if (_asset == null)
            {
                return;
            }

            _asset = null;
            Console.WriteLine($"Unloaded asset: {_filepath}");
        }

        public T Get()
        {
            if (_asset == null)
            {
                Load();
            }

            _lastUsed = Environment.TickCount;
            return _asset;
        }
    }
}
