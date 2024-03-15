using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{
    public class PropolsionEffect : ParticleEffect
    {

        private Lander m_lander;
        public PropolsionEffect(Lander lander, string textureName, int sizeMean, int sizeStdDev, float speedMean, float speedStdDev, int lifetimeMean, int lifetimeStdDev) : base(textureName, sizeMean, sizeStdDev, speedMean, speedStdDev, lifetimeMean, lifetimeStdDev)
        {
            m_lander = lander;
        }

        public override void Update(GameTime gameTime)
        {
            // Update existing particles
            List<long> removeMe = new List<long>();
            foreach (Particle p in m_particles.Values)
            {
                if (!p.update(gameTime))
                {
                    removeMe.Add(p.name);
                }
            }

            // Remove dead particles
            foreach (long key in removeMe)
            {
                m_particles.Remove(key);
            }
            if (m_lander.isThrusting && !m_lander.isDead)
            {
                for (int i = 0; i < 8; i++)
                {
                    var particle = Create(m_lander.position, -1 * m_lander.orientation);
                    m_particles.Add(particle.name, particle);
                }
            }
        }
    }
}
