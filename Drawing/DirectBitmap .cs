using gk2.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace gk2 {
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
            Bitmap = new Bitmap(width, height, width * 4,
                PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, Color colour) {
            int index = x + (y * Width);
            int col = colour.ToArgb();

            Bits[index] = col;
        }
        public void DrawLine(Vector2 from, Vector2 to) {
            Pen blackPen = new Pen(Color.Black, 1);
            using (var graphics = Graphics.FromImage(Bitmap)) {
                graphics.SmoothingMode = 
                    System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                graphics.DrawLine(blackPen, from.X, from.Y, to.X, to.Y);
            }
        }

        public Color GetPixel(int x, int y) {
            int index = x + (y * Width);
            int col = Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        public void Dispose() {
            if (Disposed)
                return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }

        ~DirectBitmap() {
            Dispose();
        }
    }
}
