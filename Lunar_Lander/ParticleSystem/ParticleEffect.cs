using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{
    public abstract class ParticleEffect
    {
        protected string m_textureName;
        protected Texture2D m_texture;
        protected Dictionary<long, Particle> m_particles = new Dictionary<long, Particle>();
        protected MyRandom m_random = new MyRandom();
        protected int m_sizeMean; // pixels
        protected int m_sizeStdDev;   // pixels
        protected float m_speedMean;  // pixels per millisecond
        protected float m_speedStdDev; // pixles per millisecond
        protected float m_lifetimeMean; // milliseconds
        protected float m_lifetimeStdDev; // milliseconds

        public ParticleEffect(string textureName, int sizeMean, int sizeStdDev, float speedMean, float speedStdDev, float lifetimeMean, float lifetimeStdDev)
        {
            m_textureName = textureName;
            m_sizeMean = sizeMean;
            m_sizeStdDev = sizeStdDev;
            m_speedMean = speedMean;
            m_speedStdDev = speedStdDev;
            m_lifetimeMean = lifetimeMean;
            m_lifetimeStdDev = lifetimeStdDev;
        }

        public void LoadContent(ContentManager content)
        {
            m_texture = content.Load<Texture2D>(m_textureName);
        }

        public virtual Particle Create(Vector2 position, Vector2 direction)
        {
            float size = (float)m_random.nextGaussian(m_sizeMean, m_sizeStdDev);
            var p = new Particle(
                    position,
                    direction,
                    (float)m_random.nextGaussian(m_speedMean, m_speedStdDev),
                    new Vector2(size, size),
                    new TimeSpan(0, 0, 0, 0, (int)m_random.nextGaussian(m_lifetimeMean, m_lifetimeStdDev)));

            return p;
        }
        public abstract void Update(GameTime gameTime);

        public Texture2D GetTexture() { return m_texture; }

        public Dictionary<long, Particle>.ValueCollection GetParticles() { return m_particles.Values; }
    }
}
