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
    public class ParticleSystem
    {

        private List<ParticleEffect> m_particleEffects;

        public ParticleSystem(ParticleEffect[] particleEffects)
        {
            m_particleEffects = new List<ParticleEffect>(particleEffects);
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (ParticleEffect particleEffect in m_particleEffects)
            {
                particleEffect.Update(gameTime);
            }
        }

        public List<ParticleEffect> GetParticleEffects() { return m_particleEffects; }
    }
}
