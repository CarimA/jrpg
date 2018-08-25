using Jint;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Debugger;
using Jint.Runtime.Interop;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public List<IEnumerator> _nextToAddCoroutines;
        public List<IEnumerator> _currentCoroutines;
        public List<IEnumerator> _nextCoroutines;

        public ScriptingManager(Game game) : base(game)
        {
            game.Components.Add(this);

            _nextToAddCoroutines = new List<IEnumerator>();
            _currentCoroutines = new List<IEnumerator>();
            _nextCoroutines = new List<IEnumerator>();

            LoadEngine();
            LoadScripts("content/scripts");
        }
        
        private void LoadEngine()
        {
            // create a new jint instance
            _engine = new Engine(new Action<Options>((options) =>
            {
                options.DebugMode();
            }));
            _engine.Execute("'use strict';");

            _engine.SetValue("print", new Action<string>(Console.WriteLine));
            _engine.SetValue("print_error", new Action<string>(Game.Console.WriteError));
            _engine.SetValue("print_warning", new Action<string>(Game.Console.WriteWarning));
            _engine.SetValue("print_success", new Action<string>(Game.Console.WriteSuccess));
            _engine.SetValue("text", Game.EnglishText);

            //_engine.SetValue("wait", new Func<float, IEnumerator>(StartCoroutine(WaitTest)));
            //_engine.SetValue("wait", new Action<float>(async (float input) => {
            //    await StartCoroutine(WaitTest(input));
            //}));
            
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

        public JsValue Execute(string script)
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
        }

        private void StartCoroutine(IEnumerator source)
        {
            _currentCoroutines.Add(source);
            source.MoveNext();
            /*var result = Task.Run(() =>
            {
                while (source.Current == null) ;
                Console.WriteLine("done");
            });*/

            /*while (!result.MoveNext())
            {
                yield return null;
            }*/
        }

        public StepMode WaitTest2(float seconds)
        {
            return StepMode.None;
        }
        
        public IEnumerator WaitTest(float seconds)
        {
            float waitTime = seconds;
            while (true)
            {
                waitTime -= deltaTime;
                if (waitTime <= 0f)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
        }

        float deltaTime;
        public override void Update(GameTime gameTime)
        {
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
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
            _currentCoroutines = _nextCoroutines.ToList();
            _nextCoroutines.Clear();
            _currentCoroutines.AddRange(_nextToAddCoroutines);

            base.Update(gameTime);
        }
    }
}
