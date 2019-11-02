﻿using gk2.Drawing;
using gk2.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gk2.Algorithm {
    public class MainController {
        private readonly HashSet<Triangle> triangles;
        public MainController(int nSquaresHorizontal, int nSquareVertical,
            DirectBitmap directBitmap) {
            triangles = new HashSet<Triangle>();
            float x_step = (float)(directBitmap.Width - 1) / nSquaresHorizontal;
            float y_step = (float)(directBitmap.Height - 1) / nSquareVertical;
            (float x, float y) cur = (0, 0);
            Vector3[,] points
                = new Vector3[nSquaresHorizontal + 1, nSquareVertical + 1];
            for (int i = 0; i <= nSquaresHorizontal; ++i) {
                cur.y = 0;
                for (int j = 0; j <= nSquareVertical; ++j) {
                    points[i, j] = new Vector3(cur.x, cur.y, 0);
                    cur.y += y_step;
                }
                cur.x += x_step;
            }
            for (int i = 0; i < nSquaresHorizontal; ++i)
                for (int j = 0; j < nSquareVertical; ++j) {
                    triangles.Add(new Triangle(
                        new Vector3(points[i, j]),
                        new Vector3(points[i + 1, j]),
                        new Vector3(points[i, j + 1])));
                    triangles.Add(new Triangle(
                        new Vector3(points[i + 1, j + 1]),
                        new Vector3(points[i + 1, j]),
                        new Vector3(points[i, j + 1])));
                }
        }
        public void OnPaint(DirectBitmap directBitmap, Background bg) {
            foreach (var triangle in triangles)
                triangle.OnPaint(directBitmap, bg);
        }
        public void OnMouseDown(Vector2 mouse_pos) {
            Parallel.ForEach(triangles, triangle =>
                triangle.OnMouseDown(mouse_pos));
        }
        public void OnMouseUp(Vector2 mouse_pos) {
            Parallel.ForEach(triangles, triangle =>
                 triangle.OnMouseUp(mouse_pos));
        }
        public void OnMouseMove(Vector2 mouse_pos) {
            Parallel.ForEach(triangles, triangle =>
                triangle.OnMouseMove(mouse_pos));
        }
        public void OnTimer() {
            Parallel.ForEach(triangles, triangle => triangle.CalculateInsidePixels());
        }
    }
}
