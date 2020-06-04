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
    public partial class Settings : Form
    {
        private Lense lense { get => Lense.GetForm; }
        private static Settings settings;
        public static Settings GetForm
        {
            get
            {
                if (settings == null || settings.IsDisposed)
                    settings = new Settings();
                return settings;
            }
            set
            {
                settings = value;
            }
        }
        public Settings()
        {
            InitializeComponent();
            GetForm = this;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            textBox1.Text = lense.Height.ToString();
            textBox2.Text = lense.Width.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lense.setSize(int.Parse(textBox2.Text), int.Parse(textBox1.Text));
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            decimal factor = numericUpDown1.Value;
            lense.setZoomFactor(factor);
        }
    }
}
