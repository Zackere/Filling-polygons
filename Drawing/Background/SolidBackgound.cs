using System.Drawing;

namespace gk2.Drawing {
    public class SolidBackgound : Background {
        public Color Color { get; set; }
        public SolidBackgound(Color color) {
            Color = color;
        }
        public override Color GetPixel(int x, int y) {
            return Color;
        }

        public override void Dispose() {
            return;
        }
    }
}
