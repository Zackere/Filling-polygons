using gk2.Algorithm;
using System;
using System.Drawing;
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
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            timer = new Timer();

            MinimumSize = new Size(MinimumSize.Width,
                groupBox1.Height + groupBox2.Height);
            directBitmap =
                new DirectBitmap(
                    pictureBox1.Width - pictureBox1.Padding.Horizontal, 
                    pictureBox1.Height - pictureBox1.Padding.Vertical);
            pictureBox1.Image = directBitmap.Bitmap;
            (nSquaresHorizontal, nSquaresVertical) = (10, 7);
            mainController =
                new MainController(
                    nSquaresHorizontal, nSquaresVertical, directBitmap);
            SetTimer();
        }

        private void SetTimer() {
            timer.Interval = 20;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e) {
            pictureBox1.Invalidate();
            pictureBox1.Refresh();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            directBitmap.Dispose();
        }

        private void CheckBox1_Click(object sender, EventArgs e) {
            KtrackBar1.Enabled = mtrackBar1.Enabled = !checkBox1.Checked;
        }

        private Utils.Vector2 pom_vector = new Utils.Vector2(0, 0);
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
            (pom_vector.X, pom_vector.Y) = 
                (e.X - pictureBox1.Padding.Left, e.Y - pictureBox1.Padding.Top);
            mainController.OnMouseDown(pom_vector);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
            (pom_vector.X, pom_vector.Y) =
                (e.X - pictureBox1.Padding.Left, e.Y - pictureBox1.Padding.Top);
            mainController.OnMouseUp(pom_vector);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {
            (pom_vector.X, pom_vector.Y) =
                (e.X - pictureBox1.Padding.Left, e.Y - pictureBox1.Padding.Top);
            mainController.OnMouseMove(pom_vector);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e) {
            mainController.OnPaint(directBitmap);
        }
    }
}
