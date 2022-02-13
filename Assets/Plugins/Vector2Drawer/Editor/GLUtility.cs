using Apxfly.VectorUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Apxfly.Editor.Utils
{
    public static class GLUtility
    {
        private static readonly Material HandleWireMaterial2D =
            (Material) EditorGUIUtility.LoadRequired("SceneView/2DHandleLines.mat");

        public static void DrawSolidCircle(Vector2 center, float radius, int sectors, Color color)
        {
            if (Event.current.type != EventType.Repaint) return;

            var handleWireMaterial = HandleWireMaterial2D;
            handleWireMaterial.SetFloat("_HandleZTest", (float) CompareFunction.Always);
            handleWireMaterial.SetPass(0);

            GL.PushMatrix();

            GL.Begin(GL.TRIANGLES);
            GL.Color(color);

            var angleStep = 360f / sectors;
            var startVector = new Vector2(0, -radius);

            for (var i = 0; i < sectors; i++)
            {
                var angleStart = i * angleStep;
                var angleEnd = (i + 1) * angleStep;

                var v1 = startVector.Rotate(angleStart) + center;
                var v2 = startVector.Rotate(angleEnd) + center;

                GL.Vertex(center);
                GL.Vertex(v1);
                GL.Vertex(v2);
            }

            GL.End();
            GL.PopMatrix();
        }

        public static void DrawWireFrameCircle(Vector2 center, float radius, int sectors, Color color)
        {
            if (Event.current.type != EventType.Repaint) return;

            var handleWireMaterial = HandleWireMaterial2D;
            handleWireMaterial.SetFloat("_HandleZTest", (float) CompareFunction.Always);
            handleWireMaterial.SetPass(0);

            GL.PushMatrix();

            GL.Begin(GL.LINES);
            GL.Color(color);

            var angleStep = 360f / sectors;
            var startVector = new Vector2(0, -radius);
            var lines = sectors * 3;

            for (var i = 0; i < lines; i += 3)
            {
                var angleStart = i * angleStep;
                var angleEnd = (i + 1) * angleStep;

                var v1 = startVector.Rotate(angleStart) + center;
                var v2 = startVector.Rotate(angleEnd) + center;

                GL.Vertex(v1);
                GL.Vertex(v2);
            }

            GL.End();
            GL.PopMatrix();
        }

        public static void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            if (Event.current.type != EventType.Repaint) return;

            var handleWireMaterial = HandleWireMaterial2D;
            handleWireMaterial.SetFloat("_HandleZTest", (float) CompareFunction.Always);
            handleWireMaterial.SetPass(0);

            GL.PushMatrix();

            GL.Begin(GL.LINES);
            GL.Color(color);

            GL.Vertex(start);
            GL.Vertex(end);

            GL.End();
            GL.PopMatrix();
        }
    }
}
