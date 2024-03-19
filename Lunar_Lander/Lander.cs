using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Lunar_Lander
{
    public class Lander
    {

        public Vector2 momentum;
        public Vector2 position;
        public Vector2 orientation;
        private double angleRadians;

        public float mass;
        public float fuel;
        private float thrust;
        private float fuelLossRate;
        private float momentOfInertia;

        public float circleRadius;
        public bool isThrusting;
        public bool isDead = false;
        
        public Lander(Vector2 position, double angle, Vector2 momentum, float circleRadius) 
        {
            this.position = position; // x, y position of lander
            this.circleRadius = circleRadius; // Radius of collision circle
            angleRadians = angle;
            this.updateOrientation();
            this.momentum = momentum; // Momentum of lander
            mass = 25f;
            fuel = 100f;
            thrust = 0.75f; // Acceleration applied to lander
            fuelLossRate = 0.01f;
            momentOfInertia = 0.002f; // 1/rotational_inertia of the lander
        }

        public void applyThrust(GameTime gameTime, float value)
        {
            float dt = gameTime.ElapsedGameTime.Milliseconds;
            if (fuel > 0)
            {
                isThrusting = true;
                momentum += orientation * thrust / mass * dt;
                fuel -= dt * fuelLossRate;
            }
            else
            {
                fuel = 0f;
            }
        }

        public void rotateCounterClockwise(GameTime gameTime, float value)
        {

            float dt = gameTime.ElapsedGameTime.Milliseconds;
            angleRadians -= momentOfInertia * dt;
            if (angleRadians < 0) angleRadians += 2 * Math.PI;
            this.updateOrientation();
        }
        public void rotateClockwise(GameTime gameTime, float value)
        {

            float dt = gameTime.ElapsedGameTime.Milliseconds;
            angleRadians += momentOfInertia * dt;
            if (angleRadians >= 2 * Math.PI) angleRadians -= 2 * Math.PI;
            this.updateOrientation();
        }



        // Updates position by the momentum
        public void updatePosition()
        {
            position += momentum / mass;
        }
        // Returns true if the lander orientation is <=5 degrees off (0,-1)
        public bool isStraight()
        {
            if (this.getAngleDegrees() <= 5 || this.getAngleDegrees() >= 355) return true;
            else return false;
        }
        public void updateOrientation()
        {
            orientation.X = Convert.ToSingle(Math.Sin(angleRadians));
            orientation.Y = -1 * Convert.ToSingle(Math.Cos(angleRadians));
        }
        public bool isBelowVerticalSpeed(float speed)
        {
            if (Math.Abs(momentum.Y/mass) < speed) return true;
            else return false;
        }
        public float getAngleRadians()
        {
            return (float)angleRadians;
        }
        public float getAngleDegrees()
        {
            return (float)(angleRadians * 180/Math.PI);
        }

        public bool lineCollision(Line line)
        {
            Vector2 v1 = new Vector2(line.p2.X - line.p1.X, line.p2.Y - line.p1.Y);
            Vector2 v2 = new Vector2(line.p1.X - position.X, line.p1.Y - position.Y);
            double b = -2 * (v1.X * v2.X + v1.Y * v2.Y);
            double c = 2 * (v1.X * v1.X + v1.Y * v1.Y);
            double d = Math.Sqrt(b * b - 2 * c * (v2.X * v2.X + v2.Y * v2.Y - circleRadius * circleRadius));
            if (d == double.NaN)
            {
                return false;
            }
            double u1 = (b - d) / c;
            double u2 = (b + d) / c;
            if (u1 <= 1 && u1 >= 0)
            {
                return true;
            }
            if (u2 <= 1 && u2 >= 0)
            {
                return true;
            }
            return false;
        }
    }
}
