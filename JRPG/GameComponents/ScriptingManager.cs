using Jint;
using Jint.Runtime;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public List<IEnumerator> _currentCoroutines;
        public List<IEnumerator> _nextCoroutines;

        public ScriptingManager(Game game) : base(game)
        {
            game.Components.Add(this);

            _currentCoroutines = new List<IEnumerator>();
            _nextCoroutines = new List<IEnumerator>();

            LoadEngine();
            LoadScripts("content/scripts");
        }

        private void LoadEngine()
        {
            // create a new jint instance
            _engine = new Engine();
            _engine.Execute("'use strict';");

            _engine.SetValue("print", new Action<string>(Console.WriteLine));
            _engine.SetValue("print_error", new Action<string>(Game.Console.WriteError));
            _engine.SetValue("print_warning", new Action<string>(Game.Console.WriteWarning));
            _engine.SetValue("print_success", new Action<string>(Game.Console.WriteSuccess));
            _engine.SetValue("text", Game.EnglishText);

            /*
             * new WaitCommand(this, engine);
            new ShowTextCommand(this, engine);
            new PlayerControlCommand(this, engine);
            new GetTextInputCommand(this, engine);
            
             */
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
            LoadScript(e.FullPath);
        }

        private void LoadScript(string file)
        {
            Console.WriteLine("Loading script: " + file);
            string script = File.ReadAllText(file);

            try
            {
                Execute(script);
            }
            catch (ApplicationException e)
            {
                Game.Console.WriteError(e.ToString());
            }
        }

        public void Execute(string script)
        {
            try
            {
                _engine.Execute(script);
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
        }

        public override void Update(GameTime gameTime)
        {
            foreach (IEnumerator coroutine in _currentCoroutines)
            {
                if (!coroutine.MoveNext())
                {
                    // this coroutine has finished.
                    continue;
                }

                if (coroutine.Current == null)
                {
                    // the coroutine yielded null, so run it next frame.
                    _nextCoroutines.Add(coroutine);
                    continue;
                }
            }
            _currentCoroutines = _nextCoroutines;
            base.Update(gameTime);
        }
    }
}
