using gk2.Drawing;
using gk2.Drawing.NormalMap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace gk2.Utils {
    public class Triangle {
        private readonly List<Vector3> Verticies;
        private Vector3? ClickedVertex;
        private readonly float RandomKd;
        private readonly float RandomKs;
        private readonly float RandomM;
        private readonly LinkedList<Vector2> InsidePixels;
        private LinkedListNode<Vector2> LastNode;
        private bool UpToDate = false;
        private Vector3 LightDirection = new Vector3(0, 0, 1);
        private Vector3 V = new Vector3(0, 0, 1);
        private Vector3 R = new Vector3(0, 0, 0);
        private Vector3 BgColorVector = new Vector3(0, 0, 0);
        private Vector3 LightColorVector = new Vector3(0, 0, 0);
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
        public override int GetHashCode() {
            Vector3 vector = new Vector3(0, 0, 0);
            foreach (var vertex in Verticies)
                vector += vertex;
            return vector.GetHashCode();
        }
        private float Sign((int X, int Y) p1, (int X, int Y) p2, (int X, int Y) p3) {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        private bool PointInTriangle((int X, int Y) pt, (int X, int Y) v1, (int X, int Y) v2, (int X, int Y) v3) {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = Sign(pt, v1, v2);
            d2 = Sign(pt, v2, v3);
            d3 = Sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }
        public void CalculateInsidePixels() {
            if (UpToDate)
                return;
            int min_x = (int)Verticies.First().X, max_x = (int)Verticies.First().Y,
                min_y = (int)Verticies.First().Y, max_y = (int)Verticies.First().Y;
            foreach (var v in Verticies) {
                if (min_x > (int)v.X)
                    min_x = (int)v.X;
                if (max_x < (int)v.X)
                    max_x = (int)v.X;
                if (min_y > (int)v.Y)
                    min_y = (int)v.Y;
                if (max_y < (int)v.Y)
                    max_y = (int)v.Y;
            }
            LastNode = InsidePixels.First;
            for (int i = min_x; i < max_x; ++i)
                for (int j = min_y; j < max_y; ++j) {
                    if (PointInTriangle((i, j),
                        ((int)Verticies[0].X, (int)Verticies[0].Y),
                        ((int)Verticies[1].X, (int)Verticies[1].Y),
                        ((int)Verticies[2].X, (int)Verticies[2].Y))) {
                        if (LastNode != null) {
                            LastNode.Value = new Vector2(i, j);
                            LastNode = LastNode.Next;
                        } else {
                            InsidePixels.AddLast(new Vector2(i, j));
                        }
                    }
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
            (BgColorVector.X, BgColorVector.Y, BgColorVector.Z) =
                (BgColor.R, BgColor.G, BgColor.B);
            BgColorVector /= byte.MaxValue;
            (LightColorVector.X, LightColorVector.Y, LightColorVector.Z) =
                (LightColor.R, LightColor.G, LightColor.B);
            LightColorVector /= byte.MaxValue;
            var dp1 = Vector3.Dot(
                Vector3.Normalize(NormalVector),
                Vector3.Normalize(LightDirection));
            R = Vector3.Reflect(-LightDirection, NormalVector); // 2 * <N,L> * N - L
            var dp2 = Math.Pow(Vector3.Dot(V, Vector3.Normalize(R)), m);
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
            var verticies_list = Verticies.ToList();
            var from = ((int)verticies_list[verticies_list.Count - 1].X,
                (int)verticies_list[verticies_list.Count - 1].Y);
            for (var node = InsidePixels.First; node != LastNode; node = node.Next)
                directBitmap.SetPixel((int)node.Value.X, (int)node.Value.Y,
                    CalculateColor(
                        ls.Color,
                        bg.GetPixel((int)node.Value.X, (int)node.Value.Y),
                        Vector3.Normalize(ls.Pos - new Vector3(node.Value.X, node.Value.Y, 0)),
                        nm.GetVector((int)node.Value.X, (int)node.Value.Y),
                        par)
                    );
            for (int i = 0; i < verticies_list.Count; ++i) {
                directBitmap.DrawLine(from, ((int)verticies_list[i].X, (int)verticies_list[i].Y));
                from = ((int)verticies_list[i].X, (int)verticies_list[i].Y);
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