using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{
    internal class Terrain
    {
        private List<Line> m_terrainLines;
        Random random;
        public int yBottom;
        private int m_maxHeight;
        public Terrain(Coordinate start, Coordinate end, float s, int initialPartitions, int landingZones, int yBottom, int maxHeight, int depth) 
        {
            Line initialLine = new Line(start, end);
            this.m_terrainLines = new List<Line>();
            this.random = new Random();
            this.yBottom = yBottom;
            m_maxHeight = maxHeight;

            for (float i = 0; i < initialPartitions; i++)
            {
                float initialPartitionX1 = i/initialPartitions * initialLine.dx;
                float initialPartitionY1 = initialLine.y(initialPartitionX1);
                float initialPartitionX2 = (i + 1) / initialPartitions * initialLine.dx;
                float initialPartitionY2 = initialLine.y(initialPartitionX2);
                Line initialPartition = new Line(new Coordinate(initialPartitionX1, initialPartitionY1), new Coordinate(initialPartitionX2, initialPartitionY2));
                m_terrainLines.Add(initialPartition);
            }
            // Label initial landing zones
            int labeledZones = 0;
            while (labeledZones < landingZones)
            {
                // Generate randome index that is not touching an edge
                int zoneIndex = random.Next(1,initialPartitions-1);
                Line zonePartition = m_terrainLines[zoneIndex];
                if (!zonePartition.isLandingZone)
                {
                    zonePartition.isLandingZone = true;
                    labeledZones++;
                }
            }
            m_terrainLines = createTerrain(m_terrainLines, s, depth);
        }

        private List<Line> createTerrain(List<Line> initialLines, float s, int depth)
        {
            if (depth <= 0) return initialLines;
            List<Line> terrainLines = new List<Line>();
            Queue<Line> lineQueue = new Queue<Line>(initialLines);
            while (lineQueue.Count > 0)
            {
                Line line = lineQueue.Dequeue();
                if (!line.isLandingZone)
                {

                    if (random.NextDouble() < 0.25f) // There is a chance we don't do the midpoint splitting so the surface looks more non-uniform
                    {
                        terrainLines.Add(line);
                    }
                    else
                    {
                        Coordinate midpoint = line.midpoint;
                        float newY = Math.Max(midpoint.Y + s * GaussianRandomNumber(0f, 1.0f) * Math.Abs(line.dx),m_maxHeight);
                        newY = Math.Min(newY, yBottom);
                        Line leftLine = new Line(line.p1, new Coordinate(midpoint.X, newY));
                        Line rightLine = new Line(new Coordinate(midpoint.X, newY), line.p2);
                        terrainLines.Add(leftLine);
                        terrainLines.Add(rightLine);
                    }
                }
                else
                {
                    terrainLines.Add(line);
                }
            }
            return createTerrain(terrainLines, 2f/3f*s, depth - 1);
        }

        private float GaussianRandomNumber(float mean, float std)
        {
            // From https://stackoverflow.com/questions/218060/random-gaussian-variables
            double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + std * randStdNormal; //random normal(mean,stdDev^2)
            return (float)randNormal;
        }

        public List<Line> GetLines()
        {
            return m_terrainLines;
        }
    }

    internal class Line
    {
        public Coordinate p1;
        public Coordinate p2;
        public float slope;
        public float dx;
        public float dy;
        public Coordinate midpoint;
        public bool isLandingZone;
        public Line(Coordinate p1, Coordinate p2)
        {
            this.p1 = p1;
            this.p2 = p2;
            slope = (p2.Y - p1.Y) / (p2.X - p1.X);
            this.isLandingZone = false;

            dx = p2.X - p1.X;
            dy = y(dx);
            midpoint = new Coordinate(p1.X + dx/2, y(p1.X + dx));
        }

        public float y(float x) 
        {
            return slope * (x - p1.X) + p1.Y;
        }

    }

    internal class Coordinate
    {
        public float X, Y;
        public Coordinate(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
