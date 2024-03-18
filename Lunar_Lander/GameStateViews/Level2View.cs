using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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

    public class Level2View : GameStateView
    {
        #region Class Member Variables
        private KeyboardInput m_keyboard;
        private GameStateEnum m_nextState;
        private Keys m_thrustKey, m_leftKey, m_rightKey;

        // Game data
        public Vector2 m_gravity;
        private float m_metersPerUnit;
        private float verticalSpeedThreshold = 2f;
        private Timer m_thrustSoundTimer;
        private int landerWidth = 50;
        private int landerHeight = 50;
        private float collisionRadius = 20f;

        // Pause menu variables
        private (string, GameStateEnum)[] m_menuArray = new (string, GameStateEnum)[]{
                ("Continue", GameStateEnum.Level2),
                ("Main Menu", GameStateEnum.Menu),
                ("Quit", GameStateEnum.Exit)
            };
        private int m_selectedIndex = 0;

        // Update and draw function delegates
        public delegate GameStateEnum UpdateFunction(GameTime gameTime);
        UpdateFunction m_updateFunction;
        public delegate void DrawFunction(GameTime gameTime);
        DrawFunction m_drawFunction;


        // Game entities and renderers
        public Lander m_lander;
        public Terrain m_terrain;
        public TerrainRenderer m_terrainRenderer;
        public ParticleSystem m_particleSystem;
        public ParticleSystemRenderer m_particleSystemRenderer;

        // Game assets
        Texture2D landerTexture;
        Texture2D backgroundTexture;
        Texture2D rectangleTexture;
        SpriteFont roboto;
        SoundEffect thrustSound;
        SoundEffect explosionSound;
        SoundEffect landingSound;

        // High score persister
        ScorePersister m_persister = new ScorePersister("HighScores.json");
        List<Score> m_highScores;

        #endregion
        public Level2View(GameStateEnum myState, Keys thrustKey, Keys leftKey, Keys rightKey) : base(myState)
        {
            m_thrustKey = thrustKey;
            m_leftKey = leftKey;
            m_rightKey = rightKey;
        }
        public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {

            m_persister.Load();

            m_gravity = new Vector2(0, 0.01f);
            m_metersPerUnit = 5f;

            Vector2 initialPos = new Vector2(400, 400);
            double initialAngle = 0;
            Vector2 initialMomentum = new Vector2(0, 0);
            m_lander = new Lander(initialPos, initialAngle, initialMomentum, collisionRadius);

            float surfaceRoughness = 3f;
            int recursionDepth = 4;
            int numLandingZones = 1;
            int initialPartitions = 12;
            m_terrain = new Terrain(
                new Coordinate(0, 2f / 3f * graphics.PreferredBackBufferHeight),
                new Coordinate(graphics.PreferredBackBufferWidth, 2f / 3f * graphics.PreferredBackBufferHeight),
                surfaceRoughness, initialPartitions, numLandingZones, graphics.PreferredBackBufferHeight, (int)initialPos.Y + 50, recursionDepth);

            m_particleSystem = new ParticleSystem(new ParticleEffect[] {
                new PropolsionEffect(m_lander, "propolsion", 10, 5, 0.30f, 0.05f, 500, 100),
                new ExplosionEffect(m_lander, "explosion", 10, 5, 0.12f, 0.05f, 1000, 200) });
            m_particleSystemRenderer = new ParticleSystemRenderer(m_particleSystem);

            m_updateFunction = MainUpdate;
            m_drawFunction = MainDraw;
            m_nextState = m_myState;
            m_keyboard = new KeyboardInput();
            RegisterCommands();

            m_thrustSoundTimer = new Timer(0);

            base.Initialize(graphicsDevice, graphics);
        }

        public override void RegisterCommands()
        {
            m_keyboard.registerCommand(m_thrustKey, false, m_lander.applyThrust);
            m_keyboard.registerCommand(m_leftKey, false, m_lander.rotateCounterClockwise);
            m_keyboard.registerCommand(m_rightKey, false, m_lander.rotateClockwise);
            m_keyboard.registerCommand(Keys.Escape, true, OnPause);
        }

        #region Pause Input Handler Functions

        private void OnPause(GameTime gameTime, float value)
        {
            m_updateFunction = PauseUpdate;
            m_drawFunction = PauseDraw;
        }
        private void MenuUp(GameTime gameTime, float value)
        {
            m_selectedIndex--;
            if (m_selectedIndex < 0) m_selectedIndex = m_menuArray.Length - 1;
        }

        private void MenuDown(GameTime gameTime, float value)
        {
            m_selectedIndex++;
            if (m_selectedIndex >= m_menuArray.Length) m_selectedIndex = 0;
        }

        private void MenuSelect(GameTime gameTime, float value)
        {
            GameStateEnum state = m_menuArray[m_selectedIndex].Item2;
            if (state == m_myState) { m_updateFunction = MainUpdate; m_drawFunction = MainDraw; }
            else { m_nextState = state; }
        }
        #endregion
        public override void ReregisterCommands(Keys thrustKey, Keys leftKey, Keys rightKey)
        {
            m_keyboard.registerCommand(m_thrustKey, false, (gameTime, value) => { });
            m_keyboard.registerCommand(m_leftKey, false, (gameTime, value) => { });
            m_keyboard.registerCommand(m_rightKey, false, (gameTime, value) => { });
            m_thrustKey = thrustKey;
            m_leftKey = leftKey;
            m_rightKey = rightKey;
            m_keyboard.registerCommand(m_thrustKey, false, m_lander.applyThrust);
            m_keyboard.registerCommand(m_leftKey, false, m_lander.rotateCounterClockwise);
            m_keyboard.registerCommand(m_rightKey, false, m_lander.rotateClockwise);
        }


        public override void LoadContent(ContentManager contentManager)
        {
            m_terrainRenderer = new TerrainRenderer(m_terrain, Color.Orange, m_graphics);
            landerTexture = contentManager.Load<Texture2D>("Lander2");
            roboto = contentManager.Load<SpriteFont>("roboto");
            backgroundTexture = contentManager.Load<Texture2D>("space");
            rectangleTexture = contentManager.Load<Texture2D>("whiteRectangle");
            landingSound = contentManager.Load<SoundEffect>("landingSoundCropped");
            explosionSound = contentManager.Load<SoundEffect>("explosionSound");
            thrustSound = contentManager.Load<SoundEffect>("thrustSound");
            m_particleSystemRenderer.LoadContent(contentManager);

        }

        public override void ProcessInput(GameTime gameTime)
        {
            m_keyboard.Update(gameTime);
        }

        public override GameStateEnum Update(GameTime gameTime)
        {
            // Check if game is paused
            if (m_updateFunction == PauseUpdate)
            {
                m_keyboard.registerCommand(m_thrustKey, false, (GameTime gameTime, float value) => { });
                m_keyboard.registerCommand(m_leftKey, false, (GameTime gameTime, float value) => { });
                m_keyboard.registerCommand(m_rightKey, false, (GameTime gameTime, float value) => { });
                m_keyboard.registerCommand(Keys.Up, true, MenuUp);
                m_keyboard.registerCommand(Keys.Down, true, MenuDown);
                m_keyboard.registerCommand(Keys.Enter, true, MenuSelect);
                m_keyboard.registerCommand(Keys.Escape, true, (GameTime gameTime, float value) => { m_updateFunction = MainUpdate; m_drawFunction = MainDraw; });

            }
            return m_updateFunction(gameTime);
        }


        #region Update Functions

        public GameStateEnum MainUpdate(GameTime gameTime)
        {
            // Set next state to my state at the beginning of each update
            m_nextState = m_myState;
            ProcessInput(gameTime);

            // Add gravity to the lander's momentum
            m_lander.momentum += m_gravity * m_lander.mass;
            // Update lander's position and orientation
            m_lander.updatePosition();
            m_lander.updateOrientation();

            //Update particle system
            m_particleSystem.Update(gameTime);

            // If the ship is thrusting and timer expired, play thrust sound effect
            if (m_lander.isThrusting)
            {
                m_thrustSoundTimer.Update(gameTime);
                if (m_thrustSoundTimer.HasExpired())
                {
                    thrustSound.Play();
                    m_thrustSoundTimer = new Timer(250);
                }
            }

            // Check if the lander has collided with terrain
            foreach (Line line in m_terrain.GetLines())
            {
                if (m_lander.lineCollision(line))
                {
                    if (line.isLandingZone && m_lander.isBelowVerticalSpeed(metersToGameUnits(verticalSpeedThreshold)) && m_lander.isStraight())
                    {
                        // Unregister lander controls
                        m_keyboard.registerCommand(m_thrustKey, false, (gameTime, value) => { });
                        m_keyboard.registerCommand(m_leftKey, false, (gameTime, value) => { });
                        m_keyboard.registerCommand(m_rightKey, false, (gameTime, value) => { });

                        landingSound.Play();

                        m_updateFunction = WonUpdate;
                        m_drawFunction = WonDraw;
                    }
                    else
                    {
                        // Add one time loss logic
                        m_lander.isDead = true;
                        m_updateFunction = LostUpdate;
                        m_drawFunction = LostDraw;
                        // Play explosion sound effect
                        explosionSound.Play();
                    }
                }
            }

            // Reset the thrust flag for lander
            m_lander.isThrusting = false;

            // Return the next state
            return m_nextState;
        }

        public GameStateEnum WonUpdate(GameTime gameTime)
        {
            // Write to high scores
            Score score = new Score(m_lander.fuel, 2);
            m_highScores = m_persister.getHighScores();
            if (m_highScores != null)
            {
                if (m_highScores.Count >= 5)
                {
                    for (int i = 0; i < m_highScores.Count; i++)
                    {
                        if (score > m_highScores[i])
                        {
                            m_highScores[i] = score;
                            break;
                        }
                    }
                }
                else
                {
                    m_highScores.Add(score);
                }
                m_highScores.Sort();
            }
            else
            {
                m_highScores = new List<Score>() { score };
            }
            m_persister.Save(m_highScores);

            ProcessInput(gameTime);

            return m_nextState;
        }

        public GameStateEnum LostUpdate(GameTime gameTime)
        {
            ProcessInput(gameTime);
            m_particleSystem.Update(gameTime);
            return m_nextState;
        }

        public GameStateEnum PauseUpdate(GameTime gameTime)
        {
            m_nextState = m_myState;
            ProcessInput(gameTime);
            // If game is unpaused, reregister control inputs
            if (m_updateFunction == MainUpdate)
            {
                this.RegisterCommands();
            }
            return m_nextState;
        }


        #endregion
        public override void Draw(GameTime gameTime)
        {
            // Render background
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), null, Color.White, 0, new Vector2(), SpriteEffects.None, 0);
            m_spriteBatch.End();
            // Call draw function
            m_drawFunction(gameTime);

        }

        #region Draw Methods

        private void MainDraw(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            // Render Particle Effects
            m_particleSystemRenderer.Draw(m_spriteBatch);

            // Draw Lander
            Rectangle landerRect = new Rectangle((int)m_lander.position.X, (int)m_lander.position.Y, landerWidth, landerHeight);
            m_spriteBatch.Draw(landerTexture, landerRect, null, Color.White, m_lander.getAngleRadians(), new Vector2(landerTexture.Width / 2, landerTexture.Height / 2), SpriteEffects.None, 0);

            //Draw Controls
            DrawControls(new Vector2(m_graphics.PreferredBackBufferWidth - 310, 100), Color.Green, Color.White);
            m_spriteBatch.End();

            // Render terrain after spriteBatch.End()
            m_terrainRenderer.Draw();

        }

        private void WonDraw(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            // Draw Lander
            Rectangle landerRect = new Rectangle((int)m_lander.position.X, (int)m_lander.position.Y, landerWidth, landerHeight);
            m_spriteBatch.Draw(landerTexture, landerRect, null, Color.White, m_lander.getAngleRadians(), new Vector2(landerTexture.Width / 2, landerTexture.Height / 2), SpriteEffects.None, 0);

            //Draw Controls
            DrawControls(new Vector2(m_graphics.PreferredBackBufferWidth - 310, 100), Color.Green, Color.White);
            m_spriteBatch.End();

            // Render terrain after spriteBatch.End()
            m_terrainRenderer.Draw();

            // Draw Timer in front of terrain
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(rectangleTexture, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), new Color(Color.Black, 0.5f));
            m_spriteBatch.DrawString(roboto, "You Win!", new Vector2(m_graphics.PreferredBackBufferWidth / 2, m_graphics.PreferredBackBufferHeight / 2), Color.White, 0, new Vector2(), 3f, SpriteEffects.None, 0);
            m_spriteBatch.End();

        }
        private void LostDraw(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            m_particleSystemRenderer.Draw(m_spriteBatch);
            m_spriteBatch.End();
            // Render terrain after spriteBatch.End()
            m_terrainRenderer.Draw();
        }

        private void PauseDraw(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            // Render Particle Effects
            m_particleSystemRenderer.Draw(m_spriteBatch);

            // Draw Lander
            if (!m_lander.isDead)
            {
                Rectangle landerRect = new Rectangle((int)m_lander.position.X, (int)m_lander.position.Y, landerWidth, landerHeight);
                m_spriteBatch.Draw(landerTexture, landerRect, null, Color.White, m_lander.getAngleRadians(), new Vector2(landerTexture.Width / 2, landerTexture.Height / 2), SpriteEffects.None, 0);
            }

            //Draw Controls
            DrawControls(new Vector2(m_graphics.PreferredBackBufferWidth - 310, 100), Color.Green, Color.White);
            m_spriteBatch.End();

            // Render terrain after spriteBatch.End()
            m_terrainRenderer.Draw();

            // Draw Menu in front of terrain
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(rectangleTexture, new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight), new Color(Color.Black, 0.5f));
            m_spriteBatch.DrawString(roboto, "Game Paused", new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 100), Color.Orange, 0f, new Vector2(), 2f, SpriteEffects.None, 0);
            for (int i = 0; i < m_menuArray.Length; i++)
            {
                Color textColor = Color.White;
                if (i == m_selectedIndex) textColor = Color.OrangeRed;
                m_spriteBatch.DrawString(roboto, m_menuArray[i].Item1, new Vector2(m_graphics.PreferredBackBufferWidth / 2 - 100, 150 + i * 50), textColor);
            }
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

            // Draw Background Rectangle
            m_spriteBatch.Draw(rectangleTexture, new Rectangle((int)pos.X - 10, (int)pos.Y - 10, 300, 100), new Color(Color.Black, 0.75f));
            // Draw Text
            m_spriteBatch.DrawString(roboto, "Ship Status:", new Vector2(pos.X, pos.Y - 5), Color.White, 0f, new Vector2(), 1.5f, SpriteEffects.None, 0);
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

        #endregion
        private float gameUnitsToMeters(float gameUnits)
        {
            return m_metersPerUnit * gameUnits;
        }

        private float metersToGameUnits(float meters)
        {
            return meters / m_metersPerUnit;
        }

    }
}
