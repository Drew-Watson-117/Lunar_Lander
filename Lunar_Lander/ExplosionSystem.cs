using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{
    internal class ExplosionSystem : ParticleSystem
    {
        Lander m_lander;
        bool m_hasExploded;
        
        public ExplosionSystem(Lander lander, int sizeMean, int sizeStdDev, float speedMean, float speedStdDev, int lifetimeMean, int lifetimeStdDev) : base(lander.position, sizeMean, sizeStdDev, speedMean, speedStdDev, lifetimeMean, lifetimeStdDev)
        {
            m_lander = lander;
            m_hasExploded = false;
        }

        protected override Particle create()
        {
            float size = (float)m_random.nextGaussian(m_sizeMean, m_sizeStdDev);
            var p = new Particle(
                    m_lander.position,
                    m_random.nextCircleVector(),
                    (float)m_random.nextGaussian(m_speedMean, m_speedStDev),
                    new Vector2(size, size),
                    new System.TimeSpan(0, 0, 0, 0, (int)(m_random.nextGaussian(m_lifetimeMean, m_lifetimeStdDev)))); ;

            return p;
        }
        public void update(GameTime gameTime, WinState winState)
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

            // Generate some new particles
            if (winState == WinState.Lost && !m_hasExploded)
            {
                for (int i = 0; i < 100; i++)
                {
                    var particle = create();
                    m_particles.Add(particle.name, particle);
                }
                m_hasExploded = true;
            }
        }
    }
}
