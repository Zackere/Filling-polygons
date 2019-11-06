using gk2.Algorithm;
using gk2.Drawing;
using gk2.Drawing.NormalMap;
using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace gk2 {
    public partial class PolygonFillForm : Form {
        public DirectBitmap DirectBitmap { get; private set; }
        public Timer Timer { get; private set; }
        public int NSquaresHorizontal { get; private set; }
        public int NSquaresVertical { get; private set; }
        public MainController MainController { get; private set; }
        public Background Background { get; private set; }
        public NormalMap NormalMap { get; private set; }
        public LightSource MainLight { get; set; }

        private Color BgColor = Color.Red;
        private Bitmap BgImage = null;

        private Vector3 ConstantNormalVector = new Vector3(0, 0, 1);
        private Bitmap NormalMapImage;

        public PolygonFillForm() {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;


            DirectBitmap =
                new DirectBitmap(
                    pictureBox1.Width - pictureBox1.Padding.Horizontal,
                    pictureBox1.Height - pictureBox1.Padding.Vertical);
            pictureBox1.Image = DirectBitmap.Bitmap;
            (NSquaresHorizontal, NSquaresVertical) = (1, 1);
            MainController =
                new MainController(
                    NSquaresHorizontal, NSquaresVertical, DirectBitmap);
            Background = new SolidBackgound(BgColor);
            MainLight = new LightSource();
            MainLight.Color = Color.White;
            (MainLight.ScreenW, MainLight.ScreenH) =
                (DirectBitmap.Width, DirectBitmap.Height);

            Timer = new Timer();
            SetTimer();

            Exact.Checked = BgFromColorRadioButton.Checked = ConstNormalVectorRadioButton.Checked = true;
        }

        private void SetTimer() {
            Timer.Interval = 16;
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e) {
            if (!StopLightCheckbox.Checked)
                MainLight.OnTimer();
            MainController.OnTimer();
            pictureBox1.Invalidate();
            pictureBox1.Refresh();
        }

        private void PolygonFillForm_FormClosing(object sender, FormClosingEventArgs e) {
            DirectBitmap.Dispose();
        }

        private void RandomParamsCheckbox_Click(object sender, EventArgs e) {
            KtrackBar2.Enabled = KtrackBar1.Enabled
                 = MtrackBar1.Enabled = !RandomParamsCheckbox.Checked;
        }

        private Vector2 pom_vector = new Vector2(0, 0);
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
            (pom_vector.X, pom_vector.Y) =
                (e.X - pictureBox1.Padding.Left, e.Y - pictureBox1.Padding.Top);
            MainController.OnMouseDown(pom_vector);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
            (pom_vector.X, pom_vector.Y) =
                (e.X - pictureBox1.Padding.Left, e.Y - pictureBox1.Padding.Top);
            MainController.OnMouseUp(pom_vector);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {
            (pom_vector.X, pom_vector.Y) =
                (e.X - pictureBox1.Padding.Left, e.Y - pictureBox1.Padding.Top);
            MainController.OnMouseMove(pom_vector);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e) {
            if (RandomParamsCheckbox.Checked)
                MainController.OnPaint(DirectBitmap, Background, NormalMap,
               null, MainLight);
            else
                MainController.OnPaint(DirectBitmap, Background, NormalMap,
                    (KtrackBar1.Value / 10.0f,
                    KtrackBar2.Value / 10.0f, MtrackBar1.Value),
                    MainLight);
        }

        private void SelectColorButton_Click(object sender, EventArgs e) {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK) {
                BgColor = cd.Color;
                Background = new SolidBackgound(BgColor);
                BgFromColorRadioButton.Checked = true;
            }
        }

        private void SelectImageButton_Click(object sender, EventArgs e) {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
            if (open.ShowDialog() == DialogResult.OK) {
                if (BgImage != null)
                    BgImage.Dispose();
                BgImage = new Bitmap(open.FileName);
                if (BgFromImageRadioButton.Checked) {
                    if (Background != null)
                        Background.Dispose();
                    Background = new ImageBackground(BgImage,
                        DirectBitmap.Width, DirectBitmap.Height);
                } else {
                    BgFromImageRadioButton.Checked = true;
                }
            }
        }

        private void BgFromImageRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (BgFromImageRadioButton.Checked) {
                if (BgImage == null) {
                    MessageBox.Show("Select image first");
                    BgFromColorRadioButton.Checked = true;
                    return;
                }
                if (Background != null)
                    Background.Dispose();
                Background = new ImageBackground(BgImage,
                    DirectBitmap.Width, DirectBitmap.Height);
            }
        }

        private void BgFromColorRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (BgFromColorRadioButton.Checked) {
                if (Background != null)
                    Background.Dispose();
                Background = new SolidBackgound(BgColor);
            }
        }

        private void NormalVectorFromBitmapRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (ConstNormalVectorRadioButton.Checked) {
                NormalMap = new ConstantNormalVector(ConstantNormalVector);
            }
        }

        private void ConstNormalVectorRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (NormalVectorFromBitmapRadioButton.Checked) {
                if (NormalMapImage == null) {
                    MessageBox.Show("Select image first");
                    ConstNormalVectorRadioButton.Checked = true;
                    return;
                }
                NormalMap = new NormalMapFromImage(NormalMapImage,
                    DirectBitmap.Width, DirectBitmap.Height);
            }
        }

        private void SelectNormalMapButton_Click(object sender, EventArgs e) {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
            if (open.ShowDialog() == DialogResult.OK) {
                if (NormalMapImage != null)
                    NormalMapImage.Dispose();
                NormalMapImage = new Bitmap(open.FileName);
                if (NormalVectorFromBitmapRadioButton.Checked) {
                    NormalMap = new NormalMapFromImage(NormalMapImage,
                        DirectBitmap.Width, DirectBitmap.Height);
                } else {
                    NormalVectorFromBitmapRadioButton.Checked = true;
                }
            }
        }

        private void ShowMeshCheckbox_CheckedChanged(object sender, EventArgs e) {
            MainController.ShowMesh(ShowMeshCheckbox.Checked);
        }

        private void Exact_CheckedChanged(object sender, EventArgs e) {
            MainController.SetExactMethod();
        }

        private void Aproximate_CheckedChanged(object sender, EventArgs e) {
            MainController.SetAproximateMethod();
        }

        private void Combined_CheckedChanged(object sender, EventArgs e) {
            MainController.SetCombinedMethod();
        }

        private void AddTrianglesButton_Click(object sender, EventArgs e) {
            ++NSquaresHorizontal;
            ++NSquaresVertical;
            MainController =
                new MainController(
                    NSquaresHorizontal, NSquaresVertical, DirectBitmap);
            if (Exact.Checked)
                MainController.SetExactMethod();
            else if (Aproximate.Checked)
                MainController.SetAproximateMethod();
            else if (Combined.Checked)
                MainController.SetCombinedMethod();
            MainController.ShowMesh(ShowMeshCheckbox.Checked);
        }

        private void RemoveTrianglesButton_Click(object sender, EventArgs e) {
            if (NSquaresHorizontal == 1 || NSquaresVertical == 1)
                return;
            --NSquaresHorizontal;
            --NSquaresVertical;
            MainController =
                new MainController(
                    NSquaresHorizontal, NSquaresVertical, DirectBitmap);
            if (Exact.Checked)
                MainController.SetExactMethod();
            else if (Aproximate.Checked)
                MainController.SetAproximateMethod();
            else if (Combined.Checked)
                MainController.SetCombinedMethod();
            MainController.ShowMesh(ShowMeshCheckbox.Checked);
        }
    }
}
