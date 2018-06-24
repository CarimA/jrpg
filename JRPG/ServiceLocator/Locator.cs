using JRPG.ServiceLocator.Services;
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

        public static IAudio Audio { get; private set; }
        public static IGraphics Graphics { get; private set; }
        public static IInput Input { get; private set; }
        public static ILogger Logger { get; private set; }
        public static IRandom Random { get; private set; }

        public static void ProvideGameInstance(MainGame service) => GameInstance = service;
        public static void ProvideAudio(IAudio service) => Audio = service;
        public static void ProvideGraphics(IGraphics service) => Graphics = service;
        public static void ProvideInput(IInput service) => Input = service;
        public static void ProvideLogger(ILogger service) => Logger = service;
        public static void ProvideRandom(IRandom service) => Random = service;

    }
}
