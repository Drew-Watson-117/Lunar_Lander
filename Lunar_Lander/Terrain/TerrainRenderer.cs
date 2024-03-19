using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{
    public class TerrainRenderer
    {
        private Terrain m_terrain;
        private VertexPositionColor[] m_triangleVertices;
        private int[] m_triangleVertexIndices;
        private VertexPositionColor[] m_lineVertices;
        private int[] m_lineVertexIndices;
        private GraphicsDeviceManager m_graphics;
        private BasicEffect m_effect;
        public TerrainRenderer(Terrain terrain, Color color, GraphicsDeviceManager graphics)
        {
            m_terrain = terrain;

            // Set up graphics RasterizerState
            m_graphics = graphics;
            m_graphics.GraphicsDevice.RasterizerState = new RasterizerState
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.CullCounterClockwiseFace,   // CullMode.None If you want to not worry about triangle winding order
                MultiSampleAntiAlias = true,
            };

            m_effect = new BasicEffect(m_graphics.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up),

                Projection = Matrix.CreateOrthographicOffCenter(
                    0, m_graphics.GraphicsDevice.Viewport.Width,
                    m_graphics.GraphicsDevice.Viewport.Height, 0,   // doing this to get it to match the default of upper left of (0, 0)
                    0.1f, 2)
            };

            // Get terrain in a drawable format
            // There are two triangles per line. One which has (x1,yBottom), (x1,y1), (x2,y2), and one which has (x1,yBottom), (x2,yBottom), (x2,y2)

            m_triangleVertices = new VertexPositionColor[2 * 3 * terrain.GetLines().Count];
            m_triangleVertexIndices = new int[2 * 3 * terrain.GetLines().Count];
            for (int i = 0; i < terrain.GetLines().Count; i++)
            {
                int baseIndex = i * 6;
                Line line = terrain.GetLines()[i];
                // First triangle for the line
                m_triangleVertices[baseIndex].Position = new Vector3(line.p1.X, terrain.yBottom, 0);
                m_triangleVertices[baseIndex].Color = color;
                m_triangleVertices[baseIndex + 1].Position = new Vector3(line.p1.X, line.p1.Y, 0);
                m_triangleVertices[baseIndex + 1].Color = color;
                m_triangleVertices[baseIndex + 2].Position = new Vector3(line.p2.X, line.p2.Y, 0);
                m_triangleVertices[baseIndex + 2].Color = color;
                //Second triangle for the line
                m_triangleVertices[baseIndex + 3].Position = new Vector3(line.p1.X, terrain.yBottom, 0);
                m_triangleVertices[baseIndex + 3].Color = color;
                m_triangleVertices[baseIndex + 4].Position = new Vector3(line.p2.X, line.p2.Y, 0);
                m_triangleVertices[baseIndex + 4].Color = color;
                m_triangleVertices[baseIndex + 5].Position = new Vector3(line.p2.X, terrain.yBottom, 0);
                m_triangleVertices[baseIndex + 5].Color = color;

                m_triangleVertexIndices[baseIndex] = baseIndex;
                m_triangleVertexIndices[baseIndex + 1] = baseIndex + 1;
                m_triangleVertexIndices[baseIndex + 2] = baseIndex + 2;
                m_triangleVertexIndices[baseIndex + 3] = baseIndex + 3;
                m_triangleVertexIndices[baseIndex + 4] = baseIndex + 4;
                m_triangleVertexIndices[baseIndex + 5] = baseIndex + 5;
            }

            // Define line list for terrain outline
            m_lineVertices = new VertexPositionColor[4 * terrain.GetLines().Count];
            m_lineVertexIndices = new int[4 * terrain.GetLines().Count];
            int lineOffset = 1;
            Color lineColor = Color.OrangeRed;
            for (int i = 0; i < terrain.GetLines().Count; i++)
            {
                int baseIndex = i * 4;
                Line line = terrain.GetLines()[i];
                // Vertex 1
                m_lineVertices[baseIndex].Position = new Vector3(line.p1.X, line.p1.Y, 0);
                m_lineVertices[baseIndex].Color = lineColor;
                // Vertex 2
                m_lineVertices[baseIndex + 1].Position = new Vector3(line.p2.X,line.p2.Y, 0);
                m_lineVertices[baseIndex + 1].Color = lineColor;

                // Create another line 2px above the first to make the line appear thicker
                m_lineVertices[baseIndex + 2].Position = new Vector3(line.p1.X, line.p1.Y - lineOffset, 0);
                m_lineVertices[baseIndex + 2].Color = lineColor;
                m_lineVertices[baseIndex + 3].Position = new Vector3(line.p2.X, line.p2.Y - lineOffset, 0);
                m_lineVertices[baseIndex + 3].Color = lineColor;
                // Indices
                m_lineVertexIndices[baseIndex] = baseIndex;
                m_lineVertexIndices[baseIndex + 1] = baseIndex + 1;
                m_lineVertexIndices[baseIndex + 2] = baseIndex + 2;
                m_lineVertexIndices[baseIndex + 3] = baseIndex + 3;
            }
        }

        public void Draw()
        {
            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                // Draw Triangles
                m_graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    m_triangleVertices, 0, m_triangleVertices.Length,
                    m_triangleVertexIndices, 0, m_triangleVertexIndices.Length / 3);
                // Draw Lines
                m_graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    m_lineVertices, 0, m_lineVertices.Length,
                    m_lineVertexIndices, 0, m_lineVertexIndices.Length / 2);
            }
        }
    }
}
