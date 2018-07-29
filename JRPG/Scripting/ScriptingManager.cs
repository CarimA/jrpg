using Jint;
using Jint.Runtime;
using JRPG.Scripting.Commands;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.Scripting
{
    public class ScriptingManager : GameComponent
    {
        public new MainGame Game => (MainGame)base.Game;

        private Engine engine;

        private List<Command> activeCommands;
        public Dictionary<string, string> Scripts;

        public ScriptingManager(MainGame game) : base(game)
        {
            game.Components.Add(this);

            Scripts = new Dictionary<string, string>();
            activeCommands = new List<Command>();

            LoadScripts("content/scripts");
        }

        public void LoadScripts(string directory)
        {
            string[] scripts = Directory.GetFiles(directory);
            directory = directory.ToLower().Replace("content/", "");
            foreach (string script in scripts)
            {
                string id = Path.GetFileNameWithoutExtension(script);
                string text = File.ReadAllText("content/" + directory + "/" + id + ".js");
                Scripts.Add(id, text);
            }
        }
        public void RunScript(string script)
        {
            Execute(Scripts[script]);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        private void LoadCommands()
        {
            //engine.SetValue("Globals", Locator.Data.PlayerData);
            engine.SetValue("log", new Action<string>(Console.WriteLine));
            engine.SetValue("text", Game.EnglishText);

            //engine.SetValue("get_input", new Func<string, string>());

            /*var types = Assembly.GetAssembly(typeof(Command)).GetTypes()
                .Where(t => t.Namespace == "JRPG.Scripting.Commands")
                .ToArray();

            foreach (var type in types)
            {
                string name = type.GetProperty("Name").GetValue(null).ToString();
                var method = type.GetMethod("Action");
                var input = Expression.Parameter(typeof(object), "Action");
                var func2 = Expression.Lambda< Func<object[], object> > (
                    Expression.Call(Expression.Convert(input, method.GetType()), method), input).Compile();
                var func = Delegate.CreateDelegate(typeof(Func<>), method);
                engine.SetValue(name, func);
            }*/

            new WaitCommand(this, engine);
            new ShowTextCommand(this, engine);
            new PlayerControlCommand(this, engine);
            new GetTextInputCommand(this, engine);
        }

        private void Execute(string script)
        {
            engine = new Engine();
            engine.Execute("'use strict';");

            LoadCommands();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    engine.Execute(script);
                }
                catch (JavaScriptException exc)
                {
                    var location = engine.GetLastSyntaxNode().Location.Start;
                    throw new ApplicationException(
                      String.Format("{0} (Line {1}, Column {2})",
                        exc.Error,
                        location.Line,
                        location.Column
                        ), exc);
                }
            });
        }

        List<Command> commandsToAdd = new List<Command>();
        public void AddActiveCommand(Command command)
        {
            commandsToAdd.Add(command);
        }

        List<Command> commandsToRemove = new List<Command>();
        public void RemoveActiveCommand(Command command)
        {
            commandsToRemove.Add(command);
        }
        
        public override void Update(GameTime gameTime)
        {
            if (commandsToAdd.Count() > 0)
            {
                activeCommands.AddRange(commandsToAdd);
                commandsToAdd.Clear();
            }

            if (commandsToRemove.Count() > 0)
            {
                foreach (var com in commandsToRemove)
                    activeCommands.Remove(com);
                commandsToRemove.Clear();
            }

            foreach (var com in activeCommands)
            {
                if (com != null)
                {
                    com.Update(gameTime);
                }
            }

            base.Update(gameTime);
        }

        public void DrawFringe(GameTime gameTime)
        {
            foreach (var com in activeCommands)
            {
                if (com != null)
                {
                    com.DrawFringe(gameTime);
                }
            }
        }

        public void DrawMask(GameTime gameTime)
        {
            foreach (var com in activeCommands)
            {
                if (com != null)
                {
                    com.DrawMask(gameTime);
                }
            }
        }

        public void DrawUI(GameTime gameTime)
        {
            foreach (var com in activeCommands)
            {
                if (com != null)
                {
                    com.DrawUI(gameTime);
                }
            }
        }
    }
}
