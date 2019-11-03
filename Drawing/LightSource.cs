using gk2.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gk2.Drawing {
    public class LightSource {
        public Vector3 Pos { get; } = new Vector3(0, 0, 1);
        public Color Color { get; set; }
        private float t = 0;
        public void OnTimer() {
            t += 5;
            (Pos.X, Pos.Y, Pos.Z) = (t, t, 5);
        }
    }
}
