using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SeaBattleV2
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings(MainWindow main)
        {
            InitializeComponent();
            Size.Text = MainWindow.BtnSize.ToString();
        }

        private void Save (object sender, RoutedEventArgs e)
        {
            string str = Size.Text;
            foreach (char item in str)
            {
                if (!Char.IsNumber(item))
                    return;
            }
            if(MainWindow.BtnSize != Convert.ToInt32(Size.Text))
            {
                MainWindow.BtnSize = Convert.ToInt32(str);
                MainWindow main = (MainWindow)Owner;
                main.ReSizeButtons();
            }
            this.Close();
        }
    }
}
