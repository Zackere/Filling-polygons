using System.Drawing;
using System.Numerics;

namespace gk2.Drawing.NormalMap {
    public class NormalMapFromImage : NormalMap {
        private Vector3[,] normals;
        public NormalMapFromImage(Bitmap bitmap, int width, int height) {
            using Bitmap bmp = new Bitmap(bitmap, width, height);
            normals = new Vector3[width, height];
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j) {
                    var pixel = bmp.GetPixel(i, j);
                    normals[i, j] = Vector3.Normalize(new Vector3(
                                pixel.R - byte.MaxValue / 2,
                                byte.MaxValue / 2 - pixel.G,
                                pixel.B - byte.MaxValue / 2
                        ));
                }
        }

        public override Vector3 GetVector(int x, int y) {
            return normals[x, y];
        }
    }
}
