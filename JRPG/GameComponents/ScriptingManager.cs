using Jint;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Debugger;
using Jint.Runtime.Interop;
using JRPG.EntityComponent.Components;
using JRPG.GameComponents.ScriptCommands;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JRPG.GameComponents
{
    // https://stackoverflow.com/questions/12932306/how-does-startcoroutine-yield-return-pattern-really-work-in-unity
    // a static couroutine manager that this makes use of might make more sense
    // just worth thinking about
    public class ScriptingManager : GameComponent
    {
        public new MainGame Game => (MainGame)base.Game;
        
        private Engine _engine;

        private readonly List<Command> _subscriptions;
        private readonly List<Command> _subscriptionsToAdd;
        private readonly List<Command> _subscriptionsToRemove;


        public ScriptingManager(Game game) : base(game)
        {
            game.Components.Add(this);

            _subscriptions = new List<Command>();
            _subscriptionsToAdd = new List<Command>();
            _subscriptionsToRemove = new List<Command>();

            LoadEngine();
            LoadScripts("content/scripts");
        }

        private void LoadEngine()
        {
            // create a new jint instance
            _engine = new Engine();

            _engine.Execute("'use strict';");

            // set accessible functions
            _engine.SetValue("Console", Game.Console);
            _engine.SetValue("warp_to_map", new Action<string, int, int>(WarpToMap));
            _engine.SetValue("warp_to", new Action<int, int>(WarpTo));

            // set accessible types
            _engine.SetValue("Color", TypeReference.CreateTypeReference(_engine, typeof(Color)));
            _engine.SetValue("Vector2", TypeReference.CreateTypeReference(_engine, typeof(Vector2)));

            // set accessible (global) variables
            _engine.SetValue("text", Game.EnglishText);

            // set custom "async" commands
            new WaitCommand(this, _engine);
        }

        private void WarpToMap(string map, int x, int y)
        {
            Game.MapManager.Set(map);
            var pos = Game.Player.GetComponent<PositionComponent>();
            pos.Set(x, y);
        }
        private void WarpTo(int x, int y)
        {
            var pos = Game.Player.GetComponent<PositionComponent>();
            pos.Set(x, y);
        }

        private void LoadScripts(string directory)
        {
            // load every script and run it in jint to
            // set it up for global state
            string[] scripts = Directory.GetFiles(directory, "*.js");
            foreach (var script in scripts)
            {
                LoadScript(script);
            }

            // create a file watcher for dynamic reload
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = directory;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.js";
            watcher.Changed += Watcher_Changed;
            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Reloaded script: " + e.FullPath);
            LoadScript(e.FullPath);
        }

        private async void LoadScript(string file)
        {
            Console.WriteLine("Loading script: " + file);
            string script = File.ReadAllText(file);

            try
            {
                await Execute(script);
            }
            catch (ApplicationException e)
            {
                Game.Console.WriteError(e.ToString());
            }
        }

        public Task<JsValue> Execute(string script)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    _engine.Execute(script);
                    return _engine.GetCompletionValue();
                }
                catch (JavaScriptException e)
                {
                    var location = _engine.GetLastSyntaxNode().Location.Start;
                    throw new ApplicationException(
                      String.Format("{0} (Line {1}, Column {2})",
                        e.Error,
                        location.Line,
                        location.Column
                        ), e);
                }
            });
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_subscriptionsToAdd.Count() > 0)
            {
                _subscriptions.AddRange(_subscriptionsToAdd);
                _subscriptionsToAdd.Clear();
            }
            if (_subscriptionsToRemove.Count() > 0)
            {
                foreach (var com in _subscriptionsToRemove)
                    _subscriptions.Remove(com);
                _subscriptionsToRemove.Clear();
            }

            foreach (var subscriptions in _subscriptions)
            {
                subscriptions.Update(gameTime);
            }
        }

        public void Subscribe(Command update)
        {
            _subscriptionsToAdd.Add(update);
        }

        public void Unsubscribe(Command update)
        {
            _subscriptionsToRemove.Add(update);
        }
    }
}
