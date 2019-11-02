using System;
using System.Drawing;

namespace gk2.Drawing {
    public abstract class Background : IDisposable {
        public abstract void Dispose();
        public abstract Color GetPixel(int x, int y);
    }
}
