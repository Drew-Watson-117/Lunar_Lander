using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{
    internal abstract class GameStateView : IGameState
    {
        protected GraphicsDeviceManager m_graphics;
        protected SpriteBatch m_spriteBatch;
        protected GameStateEnum m_myState;

        public GameStateView(GameStateEnum myState)
        {
            m_myState = myState;
        }
        public virtual void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);

        }
        public abstract void RegisterCommands();

        public virtual void ReregisterCommands(Keys thrustKey, Keys leftKey, Keys rightKey)
        {

        }
        public abstract void LoadContent(ContentManager contentManager);
        public abstract void ProcessInput(GameTime gameTime);
        public abstract GameStateEnum Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
    }
}
