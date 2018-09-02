using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrrKlang;

namespace JRPG.GameComponents
{
    public class AudioManager : GameComponent
    {
        ISoundEngine sfxEngine = new ISoundEngine();
        ISoundEngine bgmEngine = new ISoundEngine();

        private Queue<string> BGM;

        public AudioManager(Game game) : base(game)
        {
            BGM = new Queue<string>();
            game.Components.Add(this);
            bgmEngine.SoundVolume = 0;
        }

        public void PlaySFX(string sound, Vector2 position)
        {
            Vector2 source = (Game as MainGame).Camera.GetTargetPosition() - position;
            bgmEngine.Play3D($"content/audio/sfx/{sound}.ogg", source.X, 0, source.Y, false);
        }

        public void PlayBGM(string song)
        {
            BGM.Enqueue(song);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (BGM.Count > 0)
            {
                bgmEngine.SoundVolume -= ((float)gameTime.ElapsedGameTime.TotalSeconds / 2);

                if (bgmEngine.SoundVolume <= 0)
                {
                    string song = BGM.Dequeue();
                    bgmEngine.StopAllSounds();
                    bgmEngine.Play2D($"content/audio/{song}.ogg", true);
                }
            }
            else
            {
                if (bgmEngine.SoundVolume < 1)
                {
                    bgmEngine.SoundVolume += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }
    }
}
