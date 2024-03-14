using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{

    [DataContract(Name = "Score")]
    public class Score
    {
        /// <summary>
        /// Have to have a default constructor for the XmlSerializer.Deserialize method
        /// </summary>
        public Score()
        {
        }

        /// <summary>
        /// Overloaded constructor used to create an object for long term storage
        /// </summary>
        /// <param name="fuel"></param>
        /// <param name="level"></param>
        public Score(float fuel, ushort level)
        {
            this.Fuel = fuel;
            this.Level = level;
            this.TimeStamp = DateTime.Now;

            keys.Add(1, "one");
            keys.Add(2, "two");
        }

        [DataMember()]
        public float Fuel { get; set; }
        [DataMember()]
        public ushort Level { get; set; }
        [DataMember()]
        public DateTime TimeStamp { get; set; }
        [DataMember()]
        public Dictionary<int, String> keys = new Dictionary<int, string>();
    }
}
