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
        Controls,
        Exit
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private Dictionary<GameStateEnum, IGameState> stateDict;
        private GameStateEnum currentState, nextState;
        public Keys thrustKey, leftKey, rightKey;
        private ControlsPersister m_controlsPersister;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            
            m_controlsPersister = new ControlsPersister("myControls.json");
            m_controlsPersister.Load();
            Controls controls = m_controlsPersister.getControls();
            if (controls == null)
            {
                thrustKey = Keys.Up;
                leftKey = Keys.Left;
                rightKey = Keys.Right;
            }
            else
            {
                thrustKey = controls.ThrustKey;
                leftKey = controls.LeftKey;
                rightKey = controls.RightKey;
            }

            stateDict = new Dictionary<GameStateEnum, IGameState>() 
            {
                { GameStateEnum.Level1, new Level1View(GameStateEnum.Level1, thrustKey, leftKey, rightKey) },
                { GameStateEnum.Menu, new MenuView(GameStateEnum.Menu) },
                { GameStateEnum.Controls, new ControlsView(GameStateEnum.Controls, thrustKey, leftKey, rightKey) },
                { GameStateEnum.HighScores, new HighScoreView(GameStateEnum.HighScores) },
                { GameStateEnum.Level2, new Level2View(GameStateEnum.Level2, thrustKey, leftKey, rightKey) },
                { GameStateEnum.Credits, new CreditsView(GameStateEnum.Credits) },
            };

        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            // Initialize all states and set initial state
            currentState = GameStateEnum.Menu;
            nextState = currentState;
            foreach (IGameState state in stateDict.Values)
            {
                state.Initialize(GraphicsDevice, _graphics);
            }


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
            // Run update for current game state, get next game state
            nextState = stateDict[currentState].Update(gameTime);
            // If next state is exit, quit
            if (nextState == GameStateEnum.Exit)
            {
                this.Exit();
            }
            // If controls have been changed, do the remap
            if (currentState == GameStateEnum.Controls && ((ControlsView)stateDict[currentState]).remap)
            {
                foreach (IGameState state in stateDict.Values)
                {
                    thrustKey = ((ControlsView)stateDict[currentState]).getThrustKey();
                    leftKey = ((ControlsView)stateDict[currentState]).getLeftKey();
                    rightKey = ((ControlsView)stateDict[currentState]).getRightKey();
                    state.ReregisterCommands(thrustKey, leftKey, rightKey);
                }
                ((ControlsView)stateDict[currentState]).remap = false;
                m_controlsPersister.Save(new Controls(thrustKey,leftKey,rightKey));
            }
            // Conduct state change
            if (currentState != nextState && nextState != GameStateEnum.Exit)
            {
                stateDict[nextState].Initialize(GraphicsDevice, _graphics);
                stateDict[nextState].LoadContent(Content);
                currentState = nextState;
            }

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