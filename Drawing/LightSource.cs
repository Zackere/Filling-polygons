using System;
using System.Drawing;
using System.Numerics;

namespace gk2.Drawing {
    public class LightSource {
        public Vector3 Pos { get; private set; } = new Vector3(0, 0, 1);
        public Color Color { get; set; }
        private float t = 0;
        public int ScreenW { get; set; }
        public int ScreenH { get; set; }
        public void OnTimer() {
            t += 0.1f;
            t %= (float)Math.PI * 2;
            Pos = new Vector3(
                (float)(ScreenW * (1 + Math.Cos(t))/ 2),
                (float)(ScreenH * (1 + Math.Sin(2 * t)) / 2), 
                20 * (float)(1 + Math.Sin(Math.PI * t)));
        }
    }
}
