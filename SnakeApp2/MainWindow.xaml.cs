using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;
using System.Threading.Tasks;

namespace SnakeApp2
{

    public partial class MainWindow : Window
    {
        GameState newGame;

        public MainWindow()
        {
            InitializeComponent();
            newGame = new GameState(28, 28);
            newGame.GameArea = GameArea;
            newGame.SetupGameArea(28);
            newGame.DrawInitialSnake();
            newGame.AddFoodToTheGrid();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async Task GameLoop()
        {
            while (!newGame.GameOver)
            {
                await Task.Delay(70);
                newGame.MoveSnakeAndDetermineGameOver();
                UpdateScore();
            }
        }

        private void UpdateScore()
        {
            ScoreText.Text = $"Score: {newGame.Score}";
        }

        //  Window_KeyDown
        private void MoveSnake(object sender, KeyEventArgs e)
        {
            if (newGame.GameOver)
            {
                return;
            }

            if (e.Key == Key.W)
            {
                newGame.ChangeDirection(Direction.Up);
            }
            else if (e.Key == Key.S)
            {
                newGame.ChangeDirection(Direction.Down);
            }
            else if (e.Key == Key.A)
            {
                newGame.ChangeDirection(Direction.Left);
            }
            else if (e.Key == Key.D)
            {
                newGame.ChangeDirection(Direction.Right);
            }
        }
    }
}
