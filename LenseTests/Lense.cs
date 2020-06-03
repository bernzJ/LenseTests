using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LenseTests
{
    public partial class Lense : Form
    {
        private static Lense lense;

        public static Lense GetForm
        {
            get
            {
                if (lense == null || lense.IsDisposed)
                    lense = new Lense();
                return lense;
            }
        }
        public int LenseWidth { get; set; } = 317;
        public int LenseHeight { get; set; } = 196;
        public int ZoomFactor { get; set; } = 1;
        public Lense()
        {
            InitializeComponent();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Settings.GetForm.Show();

        }

        private Bitmap getTemp(int x, int y)
        {
            Bitmap temp = new Bitmap(LenseWidth, LenseHeight);
            Graphics g = Graphics.FromImage(temp);
            g.CopyFromScreen(x, y, 0, 0, temp.Size);
            g.Dispose();
            return temp;
        }
        private void setZoomFactor(int dir = 1)
        {
            ZoomFactor = ZoomFactor + dir == 0 ? ZoomFactor : ZoomFactor += dir;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {

            Point position = Cursor.Position;
            Bitmap temp = getTemp(position.X, position.Y);
            Size newSize = new Size(LenseWidth * ZoomFactor, LenseHeight * ZoomFactor);
            Bitmap lens = new Bitmap(temp, newSize);

            pictureBox1.Image = lens;
            Left = position.X + 20;
            Top = position.Y + LenseHeight + 5;
            pictureBox1.Refresh();
        }

        private void Lense_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && e.Modifiers == Keys.Control)
            {
                setZoomFactor();
            }
            else if (e.KeyCode == Keys.Down && e.Modifiers == Keys.Control)
            {
                setZoomFactor(-1);
            }
            Console.WriteLine(ZoomFactor);
        }
    }
}
