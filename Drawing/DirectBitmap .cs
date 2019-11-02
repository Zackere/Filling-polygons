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
        public Color BgColor { get; set; }
        public Bitmap BgImage {
            get { return bgImage; }
            set {
                if (bgImage != null)
                    bgImage.Dispose();
                bgImage = value;
            }
        }
        public enum BkgStyle {
            SOLID,
            IMAGE,
        };
        public BkgStyle BackgoundStyle { get; set; }
        public Vector3 ConstantNormalVector { get; set; }
        public Bitmap NormalMap {
            get { return normalMap; }
            set {
                if (normalMap != null)
                    normalMap.Dispose();
                normalMap = value;
            }
        }
        public enum NVStyle {
            CONSTANT,
            FROM_IMAGE,
        };
        public NVStyle NormalVectorStyle { get; set; }


        protected GCHandle BitsHandle { get; private set; }

        private Bitmap bgImage = null;
        private Bitmap normalMap = null;

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
        public Color GetBgPixel(int x, int y) {
            return BackgoundStyle == BkgStyle.SOLID ?
                BgColor : bgImage.GetPixel(x, y);
        }
        public Vector3 GetNormalVector(int x, int y) {
            if (NormalVectorStyle == NVStyle.CONSTANT) {
                return ConstantNormalVector;
            } else {
                Color c = NormalMap.GetPixel(x, y);
                return 
                    new Vector3(c.R - 127, c.G - 127, c.B - 127).UnitVector();
            }
        }
        public void DrawLine(Vector2 from, Vector2 to) {
            Pen blackPen = new Pen(Color.Black, 1);
            using (var graphics = Graphics.FromImage(Bitmap)) {
                graphics.SmoothingMode =
                    System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                graphics.DrawLine(blackPen, from.X, from.Y, to.X, to.Y);
            }
        }
        public void Clear() {
            using (var graphics = Graphics.FromImage(Bitmap)) {
                if (BackgoundStyle == BkgStyle.IMAGE)
                    graphics.DrawImage(bgImage, new PointF(0, 0));
                else if (BackgoundStyle == BkgStyle.SOLID)
                    graphics.Clear(BgColor);
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
