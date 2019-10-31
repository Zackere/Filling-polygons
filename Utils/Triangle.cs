using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gk2.Utils {
    public class Triangle {
        private readonly HashSet<Vector3> Verticies;
        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3) {
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
            for(int i = 0; i < verticies_list.Count; ++i) {
                (to.X, to.Y) = (verticies_list[i].X, verticies_list[i].Y);
                directBitmap.DrawLine(from, to);
                (from.X, from.Y) = (to.X, to.Y);
            }
        }
    }
}