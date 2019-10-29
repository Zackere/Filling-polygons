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
    public partial class Form1 : Form {
        private DirectBitmap Db;
        private Timer timer = new Timer();

        public Form1() {
            InitializeComponent();
            Db = new DirectBitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = Db.Bitmap;
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
            for (int i = 0; i < Db.Width; ++i)
                for (int j = 0; j < Db.Height; ++j)
                    Db.SetPixel(i, j, 
                        Color.FromArgb((i | j) % 256, (i & j) % 256, (i ^ j) % 256));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            Db.Dispose();
        }

        private void Form1_ResizeBegin(object sender, EventArgs e) {
            Invalidate(false);
        }
    }
}
