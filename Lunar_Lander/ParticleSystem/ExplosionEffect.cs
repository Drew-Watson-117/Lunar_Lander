using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{
    public class ExplosionEffect : ParticleEffect
    {
        private Lander m_lander;
        private bool hasExploded = false;
        public ExplosionEffect(Lander lander, string textureName, int sizeMean, int sizeStdDev, float speedMean, float speedStdDev, float lifetimeMean, float lifetimeStdDev) : base(textureName, sizeMean, sizeStdDev, speedMean, speedStdDev, lifetimeMean, lifetimeStdDev)
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

            if (m_lander.isDead && !hasExploded)
            {
                hasExploded = true;
                for (int i = 0; i < 100; i++)
                {
                    var particle = Create(m_lander.position, m_random.nextCircleVector());
                    m_particles.Add(particle.name, particle);
                }
            }
        }
    }
}
