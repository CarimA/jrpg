﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;
using JRPG.EntityComponent.Components;
using Microsoft.Xna.Framework;

namespace JRPG.Scripting.Commands
{
    public class PlayerControlCommand : Command
    {
        public override string Name => "player_control";

        bool held;

        public PlayerControlCommand(ScriptingManager manager, Engine engine) : base(manager, engine)
        {

        }

        public override object Action(params object[] args)
        {
            bool arg = bool.Parse(args[0].ToString());
            Player.GetComponent<PlayerComponent>().InControl = arg;
            return null;
        }

        public override void Update(GameTime gameTime)
        {
            held = false;
        }

        public override void DrawMask(GameTime gameTime)
        {

        }

        public override void DrawFringe(GameTime gameTime)
        {

        }

        public override void DrawUI(GameTime gameTime)
        {

        }
    }
}