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
            public float Y_min { get; set; }
            public float X_min { get; set; }
            public float Coef { get; set; }
        }
        private class ETArr {
            public LinkedList<EdgeStructure>[] Edges { get; set; }
            public int Y_min { get; set; }
            public int Y_max { get; set; }

        }
        private ETArr GetEt() {
            var edges = new LinkedList<EdgeStructure>();
            for (int i = 0; i < Verticies.Count; ++i) {
                if (Verticies[i].Y != Verticies[(i + 1) % Verticies.Count].Y)
                    edges.AddLast(new EdgeStructure {
                        Y_max = Math.Max(Verticies[i].Y, Verticies[(i + 1) % Verticies.Count].Y),
                        Y_min = Math.Min(Verticies[i].Y, Verticies[(i + 1) % Verticies.Count].Y),
                        X_min = Verticies[i].Y < Verticies[(i + 1) % Verticies.Count].Y ?
                            Verticies[i].X :
                            Verticies[(i + 1) % Verticies.Count].X,
                        Coef = (Verticies[(i + 1) % Verticies.Count].X - Verticies[i].X) /
                            (Verticies[(i + 1) % Verticies.Count].Y - Verticies[i].Y),
                    });
            }
            var ret = new ETArr { Y_max = (int)Verticies[0].Y, Y_min = (int)Verticies[0].Y };
            for (int i = 0; i < Verticies.Count; ++i) {
                var v = Verticies[i];
                if (Verticies[i].Y != Verticies[(i + 1) % Verticies.Count].Y) {
                    if (ret.Y_min > v.Y)
                        ret.Y_min = (int)v.Y;
                    if (ret.Y_max < v.Y)
                        ret.Y_max = (int)v.Y;
                }
            }
            ret.Edges = new LinkedList<EdgeStructure>[ret.Y_max - ret.Y_min + 1];
            for (int i = 0; i < ret.Edges.Length; ++i)
                ret.Edges[i] = new LinkedList<EdgeStructure>();
            foreach (var edge in edges)
                ret.Edges[(int)edge.Y_min - ret.Y_min].AddLast(edge);

            return ret;
        }
        public void CalculateInsidePixels() {
            if (UpToDate)
                return;
            InsidePixels.Clear();

            var Et = GetEt();
            var Aet = new LinkedList<EdgeStructure>();
            var query = from edge in Aet
                        orderby edge.X_min ascending
                        select edge;
            for(int y = (int)Et.Y_min; y <= Et.Y_max;) {
                foreach (var v in Et.Edges[y - Et.Y_min])
                    Aet.AddLast(v);
                EdgeStructure e = null;
                foreach(var edge in query) {
                    if (e == null) {
                        e = edge;
                    }
                    else {
                        for (int x = (int)e.X_min; x < (int)edge.X_min; ++x)
                            InsidePixels.AddLast(new Vector2(x, y));
                        e = null;
                    }
                }

                for (var v = Aet.First; v != Aet.Last;) {
                    var next = v.Next;
                    if ((int)v.Value.Y_max == y)
                        Aet.Remove(v);
                    v = next;
                }
                ++y;
                foreach (var edge in Aet)
                    edge.X_min += edge.Coef;
            }

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