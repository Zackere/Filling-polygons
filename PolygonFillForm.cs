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


            directBitmap =
                new DirectBitmap(
                    pictureBox1.Width - pictureBox1.Padding.Horizontal,
                    pictureBox1.Height - pictureBox1.Padding.Vertical);
            directBitmap.BackgoundStyle = DirectBitmap.BkgStyle.SOLID;
            directBitmap.BgColor = Color.Red;
            directBitmap.NormalVectorStyle = DirectBitmap.NVStyle.CONSTANT;
            directBitmap.ConstantNormalVector = new Utils.Vector3(0, 0, 1);

            pictureBox1.Image = directBitmap.Bitmap;
            (nSquaresHorizontal, nSquaresVertical) = (10, 7);
            mainController =
                new MainController(
                    nSquaresHorizontal, nSquaresVertical, directBitmap);

            timer = new Timer();
            SetTimer();

            radioButton2.Checked = radioButton3.Checked = true;
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
            KtrackBar2.Enabled = KtrackBar1.Enabled
                 = MtrackBar1.Enabled = !checkBox1.Checked;
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

        private void button1_Click(object sender, EventArgs e) {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
                directBitmap.BgColor = cd.Color;
        }

        private void button2_Click(object sender, EventArgs e) {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK) {
                Bitmap bmp = new Bitmap(open.FileName);
                directBitmap.BgImage = new Bitmap(bmp,
                    new Size(directBitmap.Width, directBitmap.Height));
                bmp.Dispose();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) {
            if (radioButton1.Checked) {
                if (directBitmap.BgImage == null) {
                    MessageBox.Show("Select image first");
                    radioButton2.Checked = true;
                    return;
                }
                directBitmap.BackgoundStyle = DirectBitmap.BkgStyle.IMAGE;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e) {
            if (radioButton2.Checked)
                directBitmap.BackgoundStyle = DirectBitmap.BkgStyle.SOLID;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e) {
            if (radioButton3.Checked)
                directBitmap.NormalVectorStyle = DirectBitmap.NVStyle.CONSTANT;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e) {
            if (radioButton4.Checked) {
                if (directBitmap.NormalMap == null) {
                    MessageBox.Show("Select image first");
                    radioButton3.Checked = true;
                    return;
                }
                directBitmap.NormalVectorStyle =
                    DirectBitmap.NVStyle.FROM_IMAGE;
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK) {
                Bitmap bmp = new Bitmap(open.FileName);
                directBitmap.NormalMap = new Bitmap(bmp,
                    new Size(directBitmap.Width, directBitmap.Height));
                bmp.Dispose();
            }
        }
    }
}
