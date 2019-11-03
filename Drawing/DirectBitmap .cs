using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace gk2.Drawing {
    public class DirectBitmap : IDisposable {
        public Bitmap Bitmap { get; private set; }
        public Int32[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height) {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, Color colour) {
            int index = x + (y * Width);
            int col = colour.ToArgb();
            if (index > 0 && index < Bits.Length)
                Bits[index] = col;
        }

        public Color GetPixel(int x, int y) {
            int index = x + (y * Width);
            if (!(index > 0 && index < Bits.Length))
                return Color.Black;
            int col = Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        public void DrawLine((int x, int y) from, (int x, int y) to) {
            Pen p = new Pen(Color.Black);
            using Graphics g = Graphics.FromImage(Bitmap);
            g.DrawLine(p, from.x, from.y, to.x, to.y);
        }

        public void Clear(Color c) {
            using Graphics g = Graphics.FromImage(Bitmap);
            g.Clear(c);
        }
        public void Dispose() {
            if (Disposed)
                return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }
}
