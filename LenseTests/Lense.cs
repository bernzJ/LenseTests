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
using System.Runtime.InteropServices;

namespace LenseTests
{
    public partial class Lense : Form
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool StretchBlt(IntPtr hdcDest, int nXDest, int nYDest, int nDestWidth, int nDestHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int nSrcWidth, int nSrcHeight, TernaryRasterOperations dwRop);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern bool DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern bool DeleteObject(IntPtr hObject);
        public enum TernaryRasterOperations
        {
            SRCCOPY = 0x00CC0020,
            SRCPAINT = 0x00EE0086,
            SRCAND = 0x008800C6,
            SRCINVERT = 0x00660046,
            SRCERASE = 0x00440328,
            NOTSRCCOPY = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            MERGECOPY = 0x00C000CA,
            MERGEPAINT = 0x00BB0226,
            PATCOPY = 0x00F00021,
            PATPAINT = 0x00FB0A09,
            PATINVERT = 0x005A0049,
            DSTINVERT = 0x00550009,
            BLACKNESS = 0x00000042,
            WHITENESS = 0x00FF0062
        };
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr ptr);
        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

        private System.Threading.Timer timer = null;
        private static Lense lense;
        private volatile int lenseWidth = 317;
        private volatile int lenseHeight = 196;
        private volatile int zoomFactor = 1;
        private Bitmap bmp { get; set; }
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
        public void setZoomFactor(decimal factor)
        {
            ZoomFactor = decimal.ToInt32(factor);
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Settings.GetForm.Show();
        }
        private void setZoomFactor(int dir = 1)
        {
            ZoomFactor = ZoomFactor + dir == 0 ? ZoomFactor : ZoomFactor += dir;
        }
        private void timer_Tick(object state)
        {
            // Get cursor position
            Point position = Cursor.Position;
            // Get desktop handle
            IntPtr hDesk = GetDesktopWindow();
            // Convert the handle pointer to an actual handle.
            IntPtr hSrce = GetWindowDC(hDesk);
            // Create a bitmap compatible DC
            IntPtr hDest = CreateCompatibleDC(hSrce);
            // Create the bitmap
            IntPtr hBmp = CreateCompatibleBitmap(hSrce, LenseWidth, LenseHeight);
            // Select our bitmap
            IntPtr hOldBmp = SelectObject(hDest, hBmp);
            // Stretch magic
            bool res = StretchBlt(hDest, 0, 0, LenseWidth * ZoomFactor, LenseHeight * ZoomFactor,
               hSrce, position.X, position.Y, LenseWidth, LenseHeight, TernaryRasterOperations.SRCCOPY);
            // If we success in getting our image (this should never fail but never say never)
            if (res)
            {
                // Dispose of previous bitmap if we have one.
                bmp?.Dispose();
                // Create our bitmap from the handle
                bmp = Image.FromHbitmap(hBmp);
            }
            // Select the current memory to which we mapped our handles
            SelectObject(hDest, hOldBmp);
            // Clean up everything
            DeleteObject(hBmp);
            DeleteDC(hDest);
            ReleaseDC(hDesk, hSrce);
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    pictureBox1.Image = bmp;
                    Left = position.X + 20;
                    Top = position.Y + LenseHeight + 5;
                    //pictureBox1.Refresh();
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
