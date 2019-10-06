using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SeaBattleV2
{
    public partial class MainWindow : Window
    {
        static GameLogic BotField;
        static GameLogic UserField;
        public static int BtnSize = 30;
        static bool PlayerMove = true;
        static Bot Bot;
        static DateTime Start;
        static DispatcherTimer DTimer;
        static bool FirstClick = false;
        public MainWindow()
        {
            InitializeComponent();

            BotField = new GameLogic("BotField");
            FullDisplayField(BotField, RightField, true);
            UserField = new GameLogic("userField");
            FullDisplayField(UserField, LeftField, false);

            UserField.Enemy = BotField;
            BotField.Enemy = UserField;
            Bot = new Bot(UserField);
        }

        private void OpenSettings (object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings(this);
            settings.Owner = this;
            settings.Show();
        }

        private void Victory(string name)
        {
            string time = Timer.Text;
            DTimer.Stop();
            VictoryMessage vm = new VictoryMessage(name, time, BotField.Score, UserField.Score);
            vm.Owner = this;
            vm.Show();
        }

        private void TimeUp(object v, EventArgs e)
        {
                TimeSpan span = DateTime.Now - Start;
                string timeStr = span.ToString(@"mm\:ss");
                Timer.Text = timeStr;
        }

        public void RestartGame (object sender, RoutedEventArgs e)
        {
            FirstClick = false;
            RightField.Children.Clear();
            LeftField.Children.Clear();
            try { DTimer.Stop(); } catch (Exception) { }//can't stop the timer if it was not started
            Timer.Text = "00:00";
            PlayerMove = true;

            BotField = new GameLogic("BotField");
            FullDisplayField(BotField, RightField, true);
            UserField = new GameLogic("userField");
            FullDisplayField(UserField, LeftField, false);

            UserField.Enemy = BotField;
            BotField.Enemy = UserField;

            Bot = new Bot(UserField);
        }

        private void DisplayScore()
        {
            BotScore.Text = "Бот: " + UserField.Score.ToString();
            UserScore.Text = "Игрок: " + BotField.Score.ToString(); 
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
            if (Bot.Move())
            {
                if (UserField.IsLose())
                {
                    DisplayField(UserField, LeftField);
                    DisplayScore();
                    Victory("Бот");
                    return;
                }
                BotMove();
            }
            else
            {
                PlayerMove = true;
                DisplayField(UserField, LeftField);
                DisplayScore();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        { 
            if (!PlayerMove)
            {
                return;
            }

            if (!FirstClick)
            {
                Start = DateTime.Now;

                DTimer = new DispatcherTimer();
                DTimer.Tick += new EventHandler(TimeUp);
                DTimer.Interval = new TimeSpan(0, 0, 1);
                DTimer.Start();

                FirstClick = true;
            }

            Button btn = (Button)sender;
            string[] splittedTag = btn.Tag.ToString().Split(',');
            int[] pos = new[] { Convert.ToInt32(splittedTag[0]), Convert.ToInt32(splittedTag[1]) };

            if (BotField.CheckShotPosition(pos[0], pos[1]))
            {
                if (PlayerMove = BotField.Move(pos[0], pos[1]))
                {
                    DisplayField(BotField, RightField);
                    DisplayScore();
                }
                else
                {
                    DisplayField(BotField, RightField);
                    DisplayScore();
                    BotMove();
                }

            }

            if (BotField.IsLose())
            {
                PlayerMove = false;
                Victory("Игрок");
            }
        }

        public void ReSizeButtons()
        {
            LeftField.Children.Clear();
            RightField.Children.Clear();
            FullDisplayField(BotField, RightField, true);
            FullDisplayField(UserField, LeftField, false);
        }

        private void FullDisplayField(GameLogic Field, object Side, bool IsUser)
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

                    Image img = null;
                    if(elem.IsFired)
                    {
                        if(elem.IsShip())
                            img = new Image { Source = new BitmapImage(new Uri("RedCross.png", UriKind.Relative)) };
                        else
                            img = new Image { Source = new BitmapImage(new Uri("BlueCircle.png", UriKind.Relative)) };
                    }

                    Button btn = new Button()
                    {
                        Width = BtnSize,
                        Height = BtnSize,
                        FontWeight = FontWeights.UltraBold,
                        Background = new SolidColorBrush(Colors.White),
                        Tag = tag,
                        Content = img
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

        private Button GetButton(int y, int x, StackPanel Side)
        {
            StackPanel row = (StackPanel)Side.Children[y];
            return (Button)row.Children[x];
        }
    }
}
