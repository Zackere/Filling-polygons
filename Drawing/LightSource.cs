using System.Drawing;
using System.Numerics;

namespace gk2.Drawing {
    public class LightSource {
        public Vector3 Pos { get; private set; } = new Vector3(0, 0, 1);
        public Color Color { get; set; }
        private float t = 0;
        public void OnTimer() {
            t += 5f;
            Pos = new Vector3(t, t, 1);
        }
    }
}
