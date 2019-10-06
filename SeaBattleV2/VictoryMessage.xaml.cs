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
    /// Логика взаимодействия для VictoryMessage.xaml
    /// </summary>
    public partial class VictoryMessage : Window
    {
        public VictoryMessage(string name, string time, int botScore, int userScore)
        {
            InitializeComponent();

            TBName.Text = $"{name} победил!";
            TBTime.Text = $"Длительность игры {time}";
            TBUserScore.Text = $"Счёт игрока: {userScore}";
            TBBotScore.Text = $"Счёт бота: {botScore}";
        }

        private void RestartGame (object sender, RoutedEventArgs e)
        {
            MainWindow main = (MainWindow)Owner;
            main.RestartGame(null, null);
            this.Close();
        }

        private void Exit (object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
