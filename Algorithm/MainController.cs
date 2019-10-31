using gk2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gk2.Algorithm {
    public class MainController {
        public HashSet<Triangle> triangles { get; private set; }
        public MainController(int nSquaresHorizontal, int nSquareVertical,
            DirectBitmap directBitmap) {
            triangles = new HashSet<Triangle>();
            triangles.Add(new Triangle(
                new Vector3(0, 0, 0),
                new Vector3(0, directBitmap.Height - 1, 0),
                new Vector3(directBitmap.Width - 1, 0, 0)));
            triangles.Add(new Triangle(
                new Vector3(directBitmap.Width - 1, directBitmap.Height - 1, 0),
                new Vector3(0, directBitmap.Height - 1, 0),
                new Vector3(directBitmap.Width - 1, 0, 0)));
        }
        public void OnPaint(DirectBitmap directBitmap) {
            foreach (var triangle in triangles)
                triangle.OnPaint(directBitmap);
        }
    }
}
