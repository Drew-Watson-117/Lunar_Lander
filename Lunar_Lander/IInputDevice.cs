using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{
    public interface IInputDevice
    {
        // Command Delegate is a reference to callback function for the desired command
        public delegate void CommandDelegate(GameTime gameTime, float value);
        public delegate void CommandDelegatePosition(GameTime GameTime, int x, int y);
        void Update(GameTime gameTime);
    }
}
