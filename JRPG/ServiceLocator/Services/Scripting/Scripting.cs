using Jint;
using Jint.Runtime;
using JRPG.EntityComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.ServiceLocator.Services.Scripting
{
    public class Scripting : IScripting
    {
        Engine engine;

        public Scripting()
        {
        }

        public void Initialise()
        {
            engine = new Engine();
            engine.SetValue("log", new Action<string>(Console.WriteLine));

            engine.SetValue("Audio", Locator.Audio);
            engine.SetValue("Graphics", Locator.Graphics);
            engine.SetValue("Input", Locator.Input);
            engine.SetValue("Logger", Locator.Logger);
            engine.SetValue("Random", Locator.Random);
            engine.SetValue("Utility", Locator.Utility);
            engine.SetValue("Scripting", Locator.Scripting);
            engine.SetValue("Data", Locator.Data);
            engine.SetValue("Text", Locator.Text);
            engine.SetValue("EntityFactory", Locator.Entity);
            

            engine.SetValue("Globals", Locator.Data.PlayerData);

            engine.Execute("'use strict';");
        }

        public void Execute(string script)
        {
            //Initialise();]
            

            System.Threading.Tasks.Task.Factory.StartNew(() =>
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
    }
}
