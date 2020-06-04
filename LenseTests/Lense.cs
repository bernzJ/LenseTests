using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace LenseTests
{
    public partial class Lense : Form
    {
        private System.Threading.Timer timer = null;
        private static Lense lense;
        private int lenseWidth = 317;
        private int lenseHeight = 196;
        private int zoomFactor = 1;

        public object SyncLock = new object();
        public static Lense GetForm
        {
            get
            {
                if (lense == null || lense.IsDisposed)
                    lense = new Lense();
                return lense;
            }
            private set
            {
                lense = value;
            }
        }
        public int LenseWidth { get => lenseWidth; set => lenseWidth = value; }
        public int LenseHeight { get => lenseHeight; set => lenseHeight = value; }
        public int ZoomFactor { get => zoomFactor; set => zoomFactor = value; }
        public Lense()
        {
            InitializeComponent();
            GetForm = this;
            Size = new Size(LenseWidth, LenseHeight);
            timer = new System.Threading.Timer(timer_Tick, null, 0, Timeout.Infinite);
        }
        public void setSize(int w, int h)
        {
            LenseWidth = w;
            LenseHeight = h;
            Size = new Size(w, h);
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
        private void timer_Tick(object state)
        {
            Point position = Cursor.Position;
            Bitmap temp = getTemp(position.X, position.Y);
            Size newSize = new Size(LenseWidth * ZoomFactor, LenseHeight * ZoomFactor);
            Bitmap lens = new Bitmap(temp, newSize);

            // Invoke
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    pictureBox1.Image = lens;
                    Left = position.X + 20;
                    Top = position.Y + LenseHeight + 5;
                    pictureBox1.Refresh();
                }));
            }
            timer.Change(1, Timeout.Infinite);
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
