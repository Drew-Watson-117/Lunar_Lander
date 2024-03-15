using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{
    public class ParticleSystemRenderer
    {
        private ParticleSystem m_system;

        public ParticleSystemRenderer(ParticleSystem system)
        {
            m_system = system;
        }

        public void LoadContent(ContentManager content)
        {
            foreach (ParticleEffect particleEffect in m_system.GetParticleEffects())
            {
                particleEffect.LoadContent(content);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle r = new Rectangle(0, 0, 0, 0);
            foreach (ParticleEffect particleEffect in m_system.GetParticleEffects())
            {
                Texture2D effectTexture = particleEffect.GetTexture();
                Vector2 centerTexture = new Vector2(effectTexture.Width / 2, effectTexture.Height / 2);

                foreach (Particle particle in particleEffect.GetParticles())
                {
                    r.X = (int)particle.center.X;
                    r.Y = (int)particle.center.Y;
                    r.Width = (int)particle.size.X;
                    r.Height = (int)particle.size.Y;
                    spriteBatch.Draw(
                        effectTexture,
                        r,
                        null,
                        Color.White,
                        particle.rotation,
                        centerTexture,
                        SpriteEffects.None,
                        0);
                }
            }
        }
    }
}
