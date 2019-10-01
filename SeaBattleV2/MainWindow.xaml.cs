using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SeaBattleV2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static GameLogic botField;
        static GameLogic userField;
        static int BtnSize = 30;
        static bool PlayerMove = true;
        static Bot bot;
        static DateTime start;
        static DispatcherTimer timer;
        static bool firstClick = false;
        public MainWindow()
        {
            InitializeComponent();

            botField = new GameLogic("BotField");
            FirstDisplayField(botField, RightField, true);
            userField = new GameLogic("userField");
            FirstDisplayField(userField, LeftField, false);

            bot = new Bot(userField);
        }

        private void Victory(string name)
        {
            string time = Timer.Text;
            timer.Stop();
            MessageBox.Show($"{name} победил! \nИгра длилась {time}");
        }

        private void timeUp(object v, EventArgs e)
        {
                TimeSpan span;
                span = DateTime.Now - start;
                string timeStr = span.ToString(@"mm\:ss");
                Timer.Text = timeStr;
        }

        private void RestartGame (object sender, RoutedEventArgs e)
        {
            firstClick = false;
            RightField.Children.Clear();
            LeftField.Children.Clear();
            try { timer.Stop(); } catch (Exception) { }//can't stop the timer if it was not started
            Timer.Text = "00:00";
            PlayerMove = true;

            botField = new GameLogic("BotField");
            FirstDisplayField(botField, RightField, true);
            userField = new GameLogic("userField");
            FirstDisplayField(userField, LeftField, false);

            bot = new Bot(userField);
        }

        private void DisplayField(GameLogic Field, object Side)
        {
            if (Field.NewState.Count == 0)
                return;

            StackPanel side = (StackPanel)Side;
            List<FieldsElement> State = Field.NewState;
            Field.ClearState();
            foreach (FieldsElement item in State)
            {
                Button btn = GetButton(item.GetY(), item.GetX(), side);
                if(item.IsShip())
                {
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri("RedCross.png", UriKind.Relative));
                    btn.Content = img;
                }
                else
                {
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri("BlueCircle.png", UriKind.Relative));
                    btn.Content = img;
                }
            }
        }

        private void BotMove()
        {
            if (bot.Move())
            {
                BotMove();
                if (userField.IsLose())
                {
                    Victory("Бот");
                }
            }
            else
            {
                PlayerMove = true;
                DisplayField(userField, LeftField);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!PlayerMove)
            {
                return;
            }

            if (!firstClick)
            {
                start = DateTime.Now;

                timer = new DispatcherTimer();
                timer.Tick += new EventHandler(timeUp);
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();

                firstClick = true;
            }

            Button btn = (Button)sender;
            string[] splittedTag = btn.Tag.ToString().Split(',');
            int[] pos = new[] { Convert.ToInt32(splittedTag[0]), Convert.ToInt32(splittedTag[1]) };

            if (botField.CheckShotPosition(pos[0], pos[1]))
            {
                if (PlayerMove = botField.Move(pos[0], pos[1]))
                    DisplayField(botField, RightField);
                else
                {
                    DisplayField(botField, RightField);
                    BotMove();
                }

            }

            if (botField.IsLose())
            {
                PlayerMove = false;
                Victory("Игрок");
            }
        }

        private void FirstDisplayField(GameLogic Field, object Side, bool IsUser)
        {
            StackPanel panel = (StackPanel)Side;

            for(int i = 0; i < 10; i++)
            {
                StackPanel row = new StackPanel()
                {
                    Orientation = Orientation.Horizontal
                };
                panel.Children.Add(row);
                for(int j = 0; j < 10; j++)
                {
                    FieldsElement elem = Field.Field[i, j];
                    string tag = $"{elem.GetY()},{elem.GetX()},{elem.Ship}";
                    Button btn = new Button()
                    {
                        Width = BtnSize,
                        Height = BtnSize,
                        FontWeight = FontWeights.UltraBold,
                        Background = new SolidColorBrush(Colors.White),
                        Tag = tag,
                        Content = ""//elem.Ship > 0 ? elem.Ship.ToString() : ""//for debug
                    };
                    if(!IsUser)
                    {
                        if (elem.IsShip())
                        {
                            btn.BorderBrush = new SolidColorBrush(Colors.Blue);
                            btn.BorderThickness = new Thickness(2);
                            btn.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#f0f8ff"));
                        }
                    }
                    else
                        btn.Click += Button_Click;

                    row.Children.Add(btn);
                }
            }
        }

        private Button GetButton(int y, int x, object Side)
        {
            StackPanel side = (StackPanel)Side;
            return GetButton(y, x, side);
        }
        private Button GetButton(int y, int x, StackPanel Side)
        {
            StackPanel row = (StackPanel)Side.Children[y];
            return (Button)row.Children[x];
        }
    }
}
