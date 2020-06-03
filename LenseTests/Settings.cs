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
        private static Settings settings;
        public static Settings GetForm
        {
            get
            {
                if (settings == null || settings.IsDisposed)
                    settings = new Settings();
                return settings;
            }
        }
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            var lense = Lense.GetForm;
            textBox1.Text = lense.Height.ToString();
            textBox2.Text = lense.Width.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var lense = Lense.GetForm;
            lense.LenseHeight = int.Parse(textBox1.Text);
            lense.LenseWidth = int.Parse(textBox2.Text);
        }
    }
}
