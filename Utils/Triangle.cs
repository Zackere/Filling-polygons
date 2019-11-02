using gk2.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gk2.Utils {
    public class Triangle {
        private readonly HashSet<Vector3> Verticies;
        private Vector3 ClickedVertex;
        private readonly float RandomKd;
        private readonly float RandomKs;
        private readonly float RandomM;
        private readonly LinkedList<Vector2> InsidePixels;
        private LinkedListNode<Vector2> LastNode;
        private bool UpToDate = false;
        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3) {
            Random random = new Random();
            RandomKd = (float)random.NextDouble();
            RandomKs = (float)random.NextDouble();
            RandomM = (float)random.NextDouble() * 100;
            Verticies = new HashSet<Vector3> { v1, v2, v3 };
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

        public override bool Equals(object obj) {
            if (obj is Triangle) {
                var triangle = obj as Triangle;
                return Verticies.SetEquals(triangle.Verticies);
            }
            return false;
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
                        ((int)Verticies.Skip(0).First().X, (int)Verticies.Skip(0).First().Y),
                         ((int)Verticies.Skip(1).First().X, (int)Verticies.Skip(1).First().Y),
                         ((int)Verticies.Skip(2).First().X, (int)Verticies.Skip(2).First().Y))) {
                        if (LastNode != null) {
                            (LastNode.Value.X, LastNode.Value.Y) = (i, j);
                            LastNode = LastNode.Next;
                        } else {
                            InsidePixels.AddLast(new Vector2(i, j));
                        }
                    }
                }
            UpToDate = true;
        }
        public void OnPaint(DirectBitmap directBitmap, Background bg) {
            var verticies_list = Verticies.ToList();
            var from = ((int)verticies_list[verticies_list.Count - 1].X,
                (int)verticies_list[verticies_list.Count - 1].Y);
            for (var node = InsidePixels.First; node != LastNode; node = node.Next)
                directBitmap.SetPixel((int)node.Value.X, (int)node.Value.Y,
                    bg.GetPixel((int)node.Value.X, (int)node.Value.Y));
            for (int i = 0; i < verticies_list.Count; ++i) {
                directBitmap.DrawLine(from, ((int)verticies_list[i].X, (int)verticies_list[i].Y));
                from = ((int)verticies_list[i].X, (int)verticies_list[i].Y);
            }
        }
        public void OnMouseDown(Vector2 mouse_pos) {
            Vector3 mouse_pos_3 = new Vector3(mouse_pos.X, mouse_pos.Y, 0);
            foreach (var vertex in Verticies)
                if ((vertex - mouse_pos_3).Len < 5) {
                    ClickedVertex = vertex;
                    return;
                }
        }
        public void OnMouseUp(Vector2 mouse_pos) {
            ClickedVertex = null;
        }
        public void OnMouseMove(Vector2 mouse_pos) {
            if (ClickedVertex != null) {
                if ((ClickedVertex.X, ClickedVertex.Y) != (mouse_pos.X, mouse_pos.Y) &&
                    (ClickedVertex - new Vector3(mouse_pos.X, mouse_pos.Y, 0)).Len > 5) {
                    (ClickedVertex.X, ClickedVertex.Y) = (mouse_pos.X, mouse_pos.Y);
                    UpToDate = false;
                }
            }
        }
    }
}