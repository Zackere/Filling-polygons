using System;

namespace gk2.Utils {
    public class Vector3 {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Len {
            get {
                return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }
        public Vector3(float x, float y, float z) {
            (X, Y, Z) = (x, y, z);
        }
        public Vector3(Vector3 v) {
            (X, Y, Z) = (v.X, v.Y, v.Z);
        }
        public Vector3 UnitVector() {
            return this * (1 / Len);
        }
        public static Vector3 operator +(Vector3 v1, Vector3 v2) {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public static Vector3 operator -(Vector3 v1, Vector3 v2) {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        public static Vector3 operator *(Vector3 v1, float f) {
            return new Vector3(v1.X * f, v1.Y * f, v1.Z * f);
        }
        public static Vector3 operator *(float f, Vector3 v1) {
            return new Vector3(v1.X * f, v1.Y * f, v1.Z * f);
        }
        public override int GetHashCode() {
            return (X, Y, Z).GetHashCode();
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
