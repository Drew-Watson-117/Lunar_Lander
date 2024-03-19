using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{

    [DataContract(Name = "Score")]
    public class Score : IComparable<Score>
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
            Fuel = fuel;
            Level = level;
            TimeStamp = DateTime.Now;

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
        public Dictionary<int, string> keys = new Dictionary<int, string>();

        public int CompareTo(Score other)
        {
            if (other == null) return 1;
            if (this == other) return 0;
            // If the other score is from a higher level, it is a better score
            if (Level < other.Level) return -1;
            // If this score is from a higher level, it is a better score
            else if (Level > other.Level) return 1;
            // If this score has more fuel, it is a better score
            else if (Fuel > other.Fuel) return 1;
            else if (Fuel == other.Fuel) return 0;
            else return -1;
        }

        // Define the is greater than operator.
        public static bool operator >(Score operand1, Score operand2)
        {
            return operand1.CompareTo(operand2) > 0;
        }

        // Define the is less than operator.
        public static bool operator <(Score operand1, Score operand2)
        {
            return operand1.CompareTo(operand2) < 0;
        }
    }
}
