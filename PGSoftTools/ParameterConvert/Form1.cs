using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParameterConvert
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            string strSource = txtSource.Text;
            if (string.IsNullOrEmpty(strSource))
                return;

            Random random         = new Random((int) DateTime.Now.Millisecond);
            int   changePosition  = random.Next(1, 20);

            string  strResult       = changePosition.ToString("00");
            var     charArray       = strSource.ToCharArray();
            var sb = new StringBuilder(strResult);

            for (int i = 0; i < charArray.Length; i++)
            {
                if (charArray[i] == '.')
                    sb.Append('=');
                else if (charArray[i] >= 0x61 && charArray[i] <= 0x7A)
                    sb.Append((char) ((charArray[i] - 0x61 + changePosition) % 26 + 0x61));
                else if (charArray[i] >= 0x41 && charArray[i] <= 0x5A)
                    sb.Append((char)((charArray[i] - 0x41 + changePosition) % 26 + 0x41));
                else
                    sb.Append(charArray[i]);
            }
            txtEncrypted.Text = Uri.EscapeDataString(sb.ToString());
        }
    }
}
