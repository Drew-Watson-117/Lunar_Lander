using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{

    [DataContract(Name = "Controls")]
    internal class Controls
    {
        public Controls()
        {
        }

        /// <summary>
        /// Overloaded constructor used to create an object for long term storage
        /// </summary>
        /// <param name="thrustKey"></param>
        /// <param name="leftKey"></param>
        /// <param name="rightKey"></param>
        public Controls(Keys thrustKey, Keys leftKey, Keys rightKey)
        {
            this.ThrustKey = thrustKey;
            this.LeftKey = leftKey;
            this.RightKey = rightKey;

            //keys.Add(1, "one");
            //keys.Add(2, "two");
        }

        [DataMember()]
        public Keys ThrustKey { get; set; }
        [DataMember()]
        public Keys LeftKey { get; set; }
        [DataMember()]
        public Keys RightKey { get; set; }

        //[DataMember()]
        //public Dictionary<int, String> keys = new Dictionary<int, string>();

    }
}
