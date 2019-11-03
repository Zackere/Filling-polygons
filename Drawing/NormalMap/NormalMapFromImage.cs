using System.Drawing;
using System.Numerics;

namespace gk2.Drawing.NormalMap {
    public class NormalMapFromImage : NormalMap {
        DirectBitmap directBitmap;
        public NormalMapFromImage(Bitmap bitmap, int width, int height) {
            using Bitmap bmp = new Bitmap(bitmap, width, height);
            directBitmap = new DirectBitmap(width, height);
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                    directBitmap.SetPixel(i, j, bmp.GetPixel(i, j));
        }
        public override void Dispose() {
            if (directBitmap != null)
                directBitmap.Dispose();
        }

        public override Vector3 GetVector(int x, int y) {
            var col = directBitmap.GetPixel(x, y);
            var ret = new Vector3(
                col.R - byte.MaxValue / 2,
                byte.MaxValue / 2 - col.G,
                col.B - byte.MaxValue / 2);
            return ret / ret.Length();
        }
    }
}
