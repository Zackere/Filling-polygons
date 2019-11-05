using gk2.Drawing;
using gk2.Drawing.NormalMap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace gk2.Utils {
    public class Triangle {
        private readonly List<Vector3> Verticies;
        private Vector3? ClickedVertex;
        private readonly float RandomKd;
        private readonly float RandomKs;
        private readonly float RandomM;
        private readonly LinkedList<Vector2> InsidePixels;
        private bool UpToDate = false;
        private Vector3 LightDirection = new Vector3(0, 0, 1);
        private Vector3 V = new Vector3(0, 0, 1);
        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3) {
            Random random = new Random(DateTime.Now.Millisecond);
            RandomKd = (float)random.NextDouble();
            RandomKs = (float)random.NextDouble();
            RandomM = (float)random.NextDouble() * 10;
            Verticies = new List<Vector3> {
                new Vector3(v1.X, v1.Y, v1.Z),
                new Vector3(v2.X, v2.Y, v2.Z),
                new Vector3(v3.X, v3.Y, v3.Z) };
            InsidePixels = new LinkedList<Vector2>();
            CalculateInsidePixels();
            UpToDate = true;
        }
        private class EdgeStructure {
            public float Y_max { get; set; }
            public float X_min { get; set; }
            public float Coef { get; set; }
            public EdgeStructure Next { get; set; }
        }
        private class ETArr {
            EdgeStructure[] Edges { get; set; }

        }
        private EdgeStructure[] GetEt() {
            var ret = new LinkedList<EdgeStructure>();
            for (int i = 0; i < Verticies.Count; ++i) {
                if (Verticies[i].Y != Verticies[(i + 1) % Verticies.Count].Y)
                    ret.AddLast(new EdgeStructure {
                        Y_max = Math.Max(Verticies[i].Y, Verticies[(i + 1) % Verticies.Count].Y),
                        X_min = Math.Min(Verticies[i].X, Verticies[(i + 1) % Verticies.Count].X),
                        Coef = (Verticies[i].X - Verticies[(i + 1) % Verticies.Count].X) / Verticies[i].Y - Verticies[(i + 1) % Verticies.Count].Y,
                        Next = null,
                    });
            }




            return ret_arr;
        }
        public void CalculateInsidePixels() {
            if (UpToDate)
                return;
            InsidePixels.Clear();

            var Et = GetEt();



            UpToDate = true;
        }
        private Color CalculateColor(
            Color LightColor,
            Color BgColor,
            Vector3 LightDirection,
            Vector3 NormalVector,
            (float kd, float ks, float m)? par) {
            float kd = par == null ? RandomKd : par.Value.kd;
            float ks = par == null ? RandomKs : par.Value.ks;
            float m = par == null ? RandomM : par.Value.m;
            var BgColorVector = new Vector3(BgColor.R, BgColor.G, BgColor.B) / byte.MaxValue;
            var LightColorVector = new Vector3(LightColor.R, LightColor.G, LightColor.B) / byte.MaxValue;
            var dp1 = Math.Max(Vector3.Dot(
                Vector3.Normalize(NormalVector),
                Vector3.Normalize(LightDirection)), 0);
            var R = Vector3.Reflect(-LightDirection, NormalVector); // 2 * <N,L> * N - L
            var dp2 = Math.Pow(Math.Max(Vector3.Dot(V, Vector3.Normalize(R)), 0), m);
            var v = (float)(kd * dp1 + ks * dp2) * LightColorVector * BgColorVector;
            v = Vector3.Clamp(v, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            v *= byte.MaxValue;
            return Color.FromArgb(
                (byte)(Math.Max(v.X, 0.0)),
                (byte)(Math.Max(v.Y, 0.0)),
                (byte)(Math.Max(v.Z, 0.0)));
        }
        public void OnPaint(
            DirectBitmap directBitmap,
            Background bg,
            NormalMap nm,
            (float kd, float ks, float m)? par,
            LightSource ls) {
            var from = ((int)Verticies[Verticies.Count - 1].X,
                (int)Verticies[Verticies.Count - 1].Y);
            Parallel.ForEach(InsidePixels, node =>
            directBitmap.SetPixel((int)node.X, (int)node.Y,
                    CalculateColor(
                        ls.Color,
                        bg.GetPixel((int)node.X, (int)node.Y),
                        Vector3.Normalize(ls.Pos - new Vector3(node.X, node.Y, 0)),
                        nm.GetVector((int)node.X, (int)node.Y),
                        par)));
            for (int i = 0; i < Verticies.Count; ++i) {
                directBitmap.DrawLine(from, ((int)Verticies[i].X, (int)Verticies[i].Y));
                from = ((int)Verticies[i].X, (int)Verticies[i].Y);
            }
        }
        public void OnMouseDown(Vector2 mouse_pos) {
            Vector3 mouse_pos_3 = new Vector3(mouse_pos.X, mouse_pos.Y, 0);
            foreach (var vertex in Verticies)
                if ((vertex - mouse_pos_3).Length() < 5) {
                    ClickedVertex = vertex;
                    return;
                }
        }
        public void OnMouseUp(Vector2 mouse_pos) {
            ClickedVertex = null;
        }
        public void OnMouseMove(Vector2 mouse_pos) {
            if (ClickedVertex.HasValue) {
                if ((ClickedVertex.Value.X, ClickedVertex.Value.Y) != (mouse_pos.X, mouse_pos.Y) &&
                    (ClickedVertex - new Vector3(mouse_pos, 0)).Value.Length() > 5) {
                    for (int i = 0; i < Verticies.Count; ++i) {
                        if (Verticies[i].Equals(ClickedVertex)) {
                            ClickedVertex = Verticies[i] = new Vector3(mouse_pos, 0);
                            break;
                        }
                    }
                    UpToDate = false;
                }
            }
        }
    }
}