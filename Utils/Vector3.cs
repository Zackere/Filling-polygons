using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gk2.Utils {
    public class Vector3 {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public Vector3(float x, float y, float z) {
            (X, Y, Z) = (x, y, z);
        }
        public static Vector3 operator +(Vector3 v1, Vector3 v2) {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public override int GetHashCode() {
            return (X,Y,Z).GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj is Vector3) {
                var vec = obj as Vector3;
                return (vec.X, vec.Y, vec.Z) == (X, Y, Z);
            }
            return false;
        }
    }
}
