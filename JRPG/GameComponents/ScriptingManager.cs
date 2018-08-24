using Jint;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
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

            // create a new jint instance
            _engine = new Engine();
            _engine.Execute("'use strict';");

            // load every script and run it in jint to
            // set it up for global state
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
