using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gk2.Utils {
    public class Triangle {
        private readonly HashSet<Vector3> Verticies;
        private Vector3 ClickedVertex;
        private float RandomKd;
        private float RandomKs;
        private float RandomM;
        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3) {
            Random random = new Random();
            RandomKd = (float)random.NextDouble();
            RandomKs = (float)random.NextDouble();
            RandomM = (float)random.NextDouble() * 100;
            Verticies = new HashSet<Vector3> { v1, v2, v3 };
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
        public void OnPaint(DirectBitmap directBitmap) {
            var verticies_list = Verticies.ToList();
            var from = new Vector2(verticies_list[verticies_list.Count - 1].X,
                verticies_list[verticies_list.Count - 1].Y);
            var to = new Vector2(0, 0);
            for (int i = 0; i < verticies_list.Count; ++i) {
                (to.X, to.Y) = (verticies_list[i].X, verticies_list[i].Y);
                directBitmap.DrawLine(from, to);
                (from.X, from.Y) = (to.X, to.Y);
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
            if (ClickedVertex != null)
                (ClickedVertex.X, ClickedVertex.Y) = (mouse_pos.X, mouse_pos.Y);
        }
    }
}