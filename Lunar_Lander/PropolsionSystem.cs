using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{
    internal class PropolsionSystem : ParticleSystem
    {
        Lander m_lander;
        public PropolsionSystem(Lander lander, int sizeMean, int sizeStdDev, float speedMean, float speedStdDev, int lifetimeMean, int lifetimeStdDev) : base(lander.position, sizeMean, sizeStdDev, speedMean, speedStdDev, lifetimeMean, lifetimeStdDev)
        {
            m_lander = lander;
        }

        protected override Particle create()
        {
            float size = (float)m_random.nextGaussian(m_sizeMean, m_sizeStdDev);
            // Create particle that goes down
            var p = new Particle(
                    m_lander.position,
                    -1 * m_lander.orientation,
                    (float)m_random.nextGaussian(m_speedMean, m_speedStDev),
                    new Vector2(size, size),
                    new System.TimeSpan(0, 0, 0, 0, (int)(m_random.nextGaussian(m_lifetimeMean, m_lifetimeStdDev)))); ;

            return p;
        }

        public override void update(GameTime gameTime)
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

            // Generate some new particles IF LANDER HAS THRUST
            if (m_lander.isThrusting)
            {
                for (int i = 0; i < 8; i++)
                {
                    var particle = create();
                    m_particles.Add(particle.name, particle);
                }
            }
        }
    }
}
