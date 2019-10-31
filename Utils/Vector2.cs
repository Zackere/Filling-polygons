using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gk2.Utils {
    public class Vector2 {
        public float X { get; set; }
        public float Y { get; set; }
        public Vector2(float x, float y) {
            (X, Y) = (x, y);
        }
        public static Vector2 operator +(Vector2 v1, Vector2 v2) {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }
        public override int GetHashCode() {
            return (X, Y).GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj is Vector3) {
                var vec = obj as Vector3;
                return (vec.X, vec.Y) == (X, Y);
            }
            return false;
        }
    }
}
