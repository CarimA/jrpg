using JRPG.EntityComponent;
using JRPG.EntityComponent.Components;
using JRPG.ServiceLocator.Services;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.ServiceLocator
{
    public class Locator
    {
        public static MainGame GameInstance { get; private set; }
        public static EntityService Entity { get; private set; }
        public static IAudio Audio { get; private set; }
        public static IGraphics Graphics { get; private set; }
        public static IInput Input { get; private set; }
        public static ILogger Logger { get; private set; }
        public static IRandom Random { get; private set; }
        public static IScripting Scripting { get; private set; }
        public static IUtility Utility { get; private set; }
        public static IData Data { get; private set; }
        public static IText Text { get; private set; }
        
        public static void ProvideGameInstance(MainGame service) => GameInstance = service;

        public static void ProvideAudio(IAudio service)
        {
            Audio = service;
            Audio.Initialise();
        }

        public static void ProvideGraphics(IGraphics service)
        {
            Graphics = service;
            Graphics.Initialise();
        }

        public static void ProvideInput(IInput service)
        {
            Input = service;
            Input.Initialise();
        }

        public static void ProvideLogger(ILogger service)
        {
            Logger = service;
            Logger.Initialise();
        }

        public static void ProvideRandom(IRandom service)
        {
            Random = service;
            Random.Initialise();
        }

        public static void ProvideScripting(IScripting service)
        {
            Scripting = service;
            Scripting.Initialise();
        }

        public static void ProvideUtility(IUtility service)
        {
            Utility = service;
            Utility.Initialise();
        }

        public static void ProvideData(IData service)
        {
            Data = service;
            Data.Initialise();
        }

        public static void ProvideText(IText service)
        {
            Text = service;
            Text.Initialise();
        }

        public static void ProvideEC(EntityManager service)
        {
            Entity = new EntityService();
            Entity.Service = service;
        }
    }
}
