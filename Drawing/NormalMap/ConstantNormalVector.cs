using gk2.Utils;

namespace gk2.Drawing.NormalMap {
    public class ConstantNormalVector : NormalMap {
        public Vector3 Vector { get; set; }
        public ConstantNormalVector(Vector3 vector) {
            Vector = vector;
        }
        public override void Dispose() {
            return;
        }

        public override Vector3 GetVector(int x, int y) {
            return Vector;
        }
    }
}
