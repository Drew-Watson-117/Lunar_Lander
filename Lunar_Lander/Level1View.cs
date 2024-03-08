using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{

    public enum WinState
    {
        None,
        Won,
        Lost
    }
    internal class Level1View : GameStateView
    {
        #region Class Member Variables
        private KeyboardInput m_keyboard;
        private GameStateEnum m_nextState;
        
        // Game metadata
        public Vector2 m_gravity;
        private float m_metersPerUnit;
        private float verticalSpeedThreshold = 5f;
        private WinState winState = WinState.None;

        // Game entities and renderers
        public Lander m_lander;
        public Terrain m_terrain;
        public TerrainRenderer m_terrainRenderer;
        public ExplosionSystem m_explosionParticleSystem;
        public PropolsionSystem m_propolsionParticleSystem;
        public ParticleSystemRenderer m_explosionParticleSystemRenderer;
        public ParticleSystemRenderer m_propolsionParticleSystemRenderer;

        Texture2D landerTexture;
        SpriteFont roboto;
        #endregion
        public Level1View(GameStateEnum myState) : base(myState)
        {
        }
        public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_gravity = new Vector2(0, 0.01f);
            m_metersPerUnit = 5f;

            Vector2 initialPos = new Vector2(400, 400);
            double initialAngle = 0;
            Vector2 initialMomentum = new Vector2(0, 0);
            m_lander = new Lander(initialPos,initialAngle, initialMomentum, 40f);

            float surfaceRoughness = 2f;
            int recursionDepth = 5;
            m_terrain = new Terrain(
                new Coordinate(0, 2f / 3f * graphics.PreferredBackBufferHeight),
                new Coordinate(graphics.PreferredBackBufferWidth, 2f / 3f * graphics.PreferredBackBufferHeight),
                surfaceRoughness, 20, 2, graphics.PreferredBackBufferHeight, recursionDepth);

            m_explosionParticleSystem = new ExplosionSystem(m_lander, 10, 5, 0.12f, 0.05f, 1000, 200);
            m_propolsionParticleSystem = new PropolsionSystem(m_lander, 10, 5, 0.30f, 0.05f, 500, 100);
            m_explosionParticleSystemRenderer = new ParticleSystemRenderer("explosion");
            m_propolsionParticleSystemRenderer = new ParticleSystemRenderer("propolsion");

            m_nextState = m_myState;
            m_keyboard = new KeyboardInput();
            this.RegisterCommands();

            base.Initialize(graphicsDevice, graphics);
        }

        public override void RegisterCommands()
        {
            m_keyboard.registerCommand(Keys.Up, false, m_lander.applyThrust);
            m_keyboard.registerCommand(Keys.Left, false, m_lander.rotateCounterClockwise);
            m_keyboard.registerCommand(Keys.Right, false, m_lander.rotateClockwise);
        }

        #region Input Handler Functions

        #endregion
        public override void LoadContent(ContentManager contentManager)
        {
            m_terrainRenderer = new TerrainRenderer(m_terrain, Color.Orange, m_graphics);
            //landerTexture = contentManager.Load<Texture2D>("Lander");
            landerTexture = contentManager.Load<Texture2D>("Lander2");
            roboto = contentManager.Load<SpriteFont>("roboto");
            m_explosionParticleSystemRenderer.LoadContent(contentManager);
            m_propolsionParticleSystemRenderer.LoadContent(contentManager);
        }

        public override void ProcessInput(GameTime gameTime)
        {
            m_keyboard.Update(gameTime);

        }

        public override GameStateEnum Update(GameTime gameTime)
        {
            // Set next state to my state at the beginning of each update
            m_nextState = m_myState;
            this.ProcessInput(gameTime);

            // Update lander if there is no win or loss
            if (winState == WinState.None)
            {
                // Add gravity to the lander's momentum
                m_lander.momentum += m_gravity * m_lander.mass;
                // Update lander's position and orientation
                m_lander.updatePosition();
                m_lander.updateOrientation();
            }
            else // Unregister lander inputs
            {
                m_keyboard.registerCommand(Keys.Up, false, (GameTime gameTime, float value) => { });
                m_keyboard.registerCommand(Keys.Left, false, (GameTime gameTime, float value) => { });
                m_keyboard.registerCommand(Keys.Right, false, (GameTime gameTime, float value) => { });
            }

            //Update particle system
            m_propolsionParticleSystem.update(gameTime);
            m_explosionParticleSystem.update(gameTime, winState);

            // Check if the lander has collided with terrain
            foreach (Line line in m_terrain.GetLines())
            {
                if (m_lander.lineCollision(line))
                {
                    if (line.isLandingZone && m_lander.isBelowVerticalSpeed(verticalSpeedThreshold) && m_lander.isStraight())
                    {
                        winState = WinState.Won;
                        m_lander.hasLanded = true;
                    }
                    else
                    {
                        winState = WinState.Lost;
                    }
                }
            }

            // Reset the thrust flag for lander
            m_lander.isThrusting = false;

            // Return the next state
            return m_nextState;
        }

        public override void Draw(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            // Render Particle Effects
            m_propolsionParticleSystemRenderer.draw(m_spriteBatch, m_propolsionParticleSystem);
            m_explosionParticleSystemRenderer.draw(m_spriteBatch, m_explosionParticleSystem);

            // Draw Lander
            if (winState != WinState.Lost)
            {
                int landerWidth = 100;
                int landerHeight = 100;
                Rectangle landerRect = new Rectangle((int)m_lander.position.X, (int)m_lander.position.Y, landerWidth, landerHeight);
                m_spriteBatch.Draw(landerTexture, landerRect, null, Color.White, m_lander.getAngleRadians(), new Vector2(landerTexture.Width / 2, landerTexture.Height / 2), SpriteEffects.None, 0);
            }

            //Draw Controls
            this.DrawControls(new Vector2(800, 100), Color.Green, Color.White);
            

            if (winState == WinState.Lost)
            {
                m_spriteBatch.DrawString(roboto, "You Lose", new Vector2(200, 200), Color.Black);
            }
            if (winState == WinState.Won)
            {
                m_spriteBatch.DrawString(roboto, "You Win!", new Vector2(200, 220), Color.Black);
            }


            // Render Terrain
            m_terrainRenderer.Draw();



            m_spriteBatch.End();
        }

        
        private void DrawControls(Vector2 pos, Color goodColor, Color badColor)
        {
            int offset = 20;
            Dictionary<string, bool> controls = new Dictionary<string, bool>()
            {
                { "Angle (deg): " + m_lander.getAngleDegrees().ToString(), m_lander.isStraight()},
                {"Fuel: " + m_lander.fuel.ToString() + "%", m_lander.fuel > 0f},
                { "Vertical Speed: " + gameUnitsToMeters(m_lander.momentum.Y/m_lander.mass).ToString() + "m/s", m_lander.isBelowVerticalSpeed(metersToGameUnits(verticalSpeedThreshold))}
            };
            m_spriteBatch.DrawString(roboto, "Ship Status:", new Vector2(pos.X, pos.Y), Microsoft.Xna.Framework.Color.White);
            int i = 0;
            foreach (string key in controls.Keys)
            {
                if (controls[key] == true)
                {
                    m_spriteBatch.DrawString(roboto, key, new Vector2(pos.X, pos.Y + (i + 1) * offset), goodColor);
                }
                else
                {
                    m_spriteBatch.DrawString(roboto, key, new Vector2(pos.X, pos.Y + (i + 1) * offset), badColor);
                }
                i++;
            }
        }

        private float gameUnitsToMeters(float gameUnits)
        {
            return m_metersPerUnit* gameUnits;
        }

        private float metersToGameUnits(float meters)
        {
            return meters / m_metersPerUnit;
        }

    }
}
