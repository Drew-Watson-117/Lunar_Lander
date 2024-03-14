using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Lunar_Lander
{

    public enum GameStateEnum
    {
        Menu,
        Level1,
        Level2,
        Credits,
        HighScores,
        Controls
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private Dictionary<GameStateEnum, IGameState> stateDict;
        private GameStateEnum currentState;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            stateDict = new Dictionary<GameStateEnum, IGameState>() 
            {
                {GameStateEnum.Level1, new Level1View(GameStateEnum.Level1) },

            };

        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 1080;
            // Initialize all states and set initial state
            currentState = GameStateEnum.Level1; // TODO: Switch this to menu
            foreach (IGameState state in stateDict.Values)
            {
                state.Initialize(GraphicsDevice, _graphics);
            }

            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Load Content for all States
            foreach (IGameState state in stateDict.Values)
            {
                state.LoadContent(Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Run update for current game state, get next game state
            currentState = stateDict[currentState].Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            stateDict[currentState].Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}