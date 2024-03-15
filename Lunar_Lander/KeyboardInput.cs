using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{
    public class KeyboardInput : IInputDevice
    {
        private struct CommandEntry
        {
            public Keys key;
            public bool keyPressOnly;
            public IInputDevice.CommandDelegate callback;
            public CommandEntry(Keys key, bool keyPressOnly, IInputDevice.CommandDelegate callback)
            {
                this.key = key;
                this.keyPressOnly = keyPressOnly;
                this.callback = callback;
            }

        }

        private Dictionary<Keys, CommandEntry> m_commandEntries = new Dictionary<Keys, CommandEntry>();

        public void registerCommand(Keys key, bool keyPressOnly, IInputDevice.CommandDelegate callback)
        {
            if (m_commandEntries.ContainsKey(key))
            { // Removing any existing entry, because it is being replaced
                m_commandEntries.Remove(key);
            }
            m_commandEntries.Add(key, new CommandEntry(key, keyPressOnly, callback));
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            foreach (CommandEntry entry in this.m_commandEntries.Values)
            {
                if (entry.keyPressOnly && keyPressed(entry.key))
                {
                    entry.callback(gameTime, 1.0f);
                }
                else if (!entry.keyPressOnly && state.IsKeyDown(entry.key))
                {
                    entry.callback(gameTime, 1.0f);
                }
            }
            m_statePrevious = state;
        }
        private bool keyPressed(Keys key)
        {
            return (Keyboard.GetState().IsKeyDown(key) && !m_statePrevious.IsKeyDown(key));
        }
        private KeyboardState m_statePrevious;
    }
}
