using System.Drawing;

namespace gk2.Drawing {
    public class ImageBackground : Background {
        private DirectBitmap directBitmap;
        public ImageBackground(Bitmap bitmap, int width, int height) {
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

        public override Color GetPixel(int x, int y) {
            return directBitmap.GetPixel(x, y);
        }
    }
}
