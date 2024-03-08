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
    internal interface IGameState
    {
        void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics);
        void RegisterCommands();
        void LoadContent(ContentManager contentManager);
        void ProcessInput(GameTime gameTime);
        GameStateEnum Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
