using gk2.Algorithm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gk2 {
    public partial class PolygonFillForm : Form {
        public DirectBitmap directBitmap { get; private set; }
        public Timer timer { get; private set; }
        public int nSquaresHorizontal { get; private set; }
        public int nSquaresVertical { get; private set; }
        public MainController mainController { get; private set; }

        public PolygonFillForm() {
            InitializeComponent();
            timer = new Timer();

            MinimumSize = new Size(MinimumSize.Width,
                groupBox1.Height + groupBox2.Height);
            directBitmap =
                new DirectBitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = directBitmap.Bitmap;
            (nSquaresHorizontal, nSquaresVertical) = (10, 10);
            mainController =
                new MainController(
                    nSquaresHorizontal, nSquaresVertical, directBitmap);
            SetTimer();
        }

        private void SetTimer() {
            timer.Interval = 100 / 6;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e) {
            Invalidate(false);
        }

        private void Form1_Paint(object sender, PaintEventArgs e) {
            // for (int i = 0; i < directBitmap.Width; ++i)
            //     for (int j = 0; j < directBitmap.Height; ++j)
            //         directBitmap.SetPixel(i, j,
            //             Color.FromArgb((i | j) % 256,
            //             (i & j) % 256, (i ^ j) % 256));
            mainController.OnPaint(directBitmap);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            directBitmap.Dispose();
        }

        private void Form1_ResizeBegin(object sender, EventArgs e) {
            Invalidate(false);
        }

        private void CheckBox1_Click(object sender, EventArgs e) {
            KtrackBar1.Enabled = mtrackBar1.Enabled = !checkBox1.Checked;
        }
    }
}
