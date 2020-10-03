using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    // Класс настроек программы
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        public int lengthSquare = 3;

        // Загрузка предыдущих настроек
        private void SetPrevSettings_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = lengthSquare;
        }

        // Изменение настроек программы
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            lengthSquare = (int)numericUpDown1.Value;
        }


    }
}
