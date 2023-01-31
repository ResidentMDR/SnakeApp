using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SnakeApp2
{
    //28 Rows x 28 Cols in 560x560px
    public class GameState
    {
        public int Rows { get; }
        public int Cols { get; }
        public int SnakeSquareSize { get; } = 20;
        public int GridSquareSize { get; } = 28;
        public GridValue[,]? GridVal { get; }
        public Direction? Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }
        public Grid GameArea { get; set; }

        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows, int cols)
        {
            this.Rows = rows;
            this.Cols = cols;
            GridVal = new GridValue[rows, cols];
            Dir = Direction.Right;
        }

        public void SetupGameArea(int size)
        {
            for (int i = 0; i < size; i++)
            {
                GameArea.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20) });
                GameArea.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20) });
            }
            GameArea.ShowGridLines = true;
        }

        /// <summary>
        /// Make initial snake 3 body part fragment long and add it to snakeParts list and to the Grid in the middle
        /// </summary>
        public void DrawInitialSnake()
        {
            int row = Rows / 2;
            for (int col = 1; col <= 3; col++)
            {
                Rectangle rect = CreateRectangleObject();

                snakePositions.AddFirst(new Position(row, col));
                GridVal[row, col] = GridValue.Snake;    // Causing NullReferenceException
                if (rect != null)
                {
                    Grid.SetRow(rect, row);
                    Grid.SetColumn(rect, col);
                    GameArea.Children.Add(rect);
                }
            }
        }

        private Rectangle CreateRectangleObject()
        {
            Rectangle rect = new Rectangle()
            {
                Width = SnakeSquareSize,
                Height = SnakeSquareSize,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.Green,
            };
            return rect;
        }

        public void AssignEmptyGridPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (GridVal[r, c] != GridValue.Snake &&
                        GridVal[r, c] != GridValue.Food &&
                        GridVal[r, c] != GridValue.Outside)
                    {
                        GridVal[r, c] = GridValue.Empty;
                    }
                }
            }
        }

        public void AddFoodToTheGrid()
        {
            int randomRow = random.Next(GridSquareSize);
            int randomColumn = random.Next(GridSquareSize);

            Rectangle food = CreateRectangleObject();
            food.Fill = Brushes.Red;

            Grid.SetRow(food, randomRow);
            Grid.SetColumn(food, randomColumn);
            GridVal[randomRow, randomColumn] = GridValue.Food;
            GameArea.Children.Add(food);
        }

        private IEnumerable<Position> GetEmptyPositions()
        {
            for (int r = 0; r < GridSquareSize; r++)
            {
                for (int c = 0; c < GridSquareSize; c++)
                {
                    if (GridVal[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        public void MoveSnakeAndDetermineGameOver()
        {
            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }

            Position newHeadPos = SnakeHeadPosition().Translate(Dir);
            var newHeadRow = newHeadPos.Row;
            var newHeadCol = newHeadPos.Col;

            if (WillHit(newHeadPos) == GridValue.Outside || WillHit(newHeadPos) == GridValue.Snake)
            {
                GameOver = true;
                MessageBox.Show("The Game is Over");
            }
            else if (WillHit(newHeadPos) == GridValue.Empty)
            {
                RemoveElementFromTheGrid(SnakeTailPosition());
                AddHead(newHeadPos);
                Rectangle rect = CreateRectangleObject();
                Grid.SetRow(rect, newHeadRow);
                Grid.SetColumn(rect, newHeadCol);
                GameArea.Children.Add(rect);
            }
            else if (WillHit(newHeadPos) == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                GridVal[newHeadRow, newHeadCol] = GridValue.Snake;
                AddFoodToTheGrid();
            }
        }

        private void RemoveElementFromTheGrid(Position pos)
        {
            RemoveTail();
            foreach (UIElement element in GameArea.Children)
            {
                if (Grid.GetRow(element) == pos.Row && Grid.GetColumn(element) == pos.Col)
                {
                    GameArea.Children.Remove(element);
                    break;
                }
            }
        }

        public void ChangeDirection(Direction dir)
        {
            if (CanChangeDirection(dir))
            {
                Dir = dir;
                dirChanges.AddLast(dir);
            }
        }

        private bool CanChangeDirection(Direction newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;
            }

            var lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.OppositeDirection();
        }

        private Direction GetLastDirection()
        {
            if (dirChanges.Count == 0)
            {
                return Dir;
            }
            return dirChanges.Last.Value;
        }

        private IEnumerable<Position> GetSnakePositions()
        {
            return snakePositions;
        }

        private Position SnakeHeadPosition()
        {
            return snakePositions.First.Value;
        }

        private Position SnakeTailPosition()
        {
            return snakePositions.Last.Value;

        }

        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            GridVal[pos.Row, pos.Col] = GridValue.Snake;
        }

        private void RemoveTail()
        {
            Position tail = SnakeTailPosition();

            snakePositions.RemoveLast();
            GridVal[tail.Row, tail.Col] = GridValue.Empty;
        }

        private GridValue WillHit(Position newHeadPos)
        {
            if (IsOutOfGrid(newHeadPos))
            {
                return GridValue.Outside;
            }
            return GridVal[newHeadPos.Row, newHeadPos.Col];
        }

        private bool IsOutOfGrid(Position pos)
        {
            return (pos.Row < 0 || pos.Row >= GridSquareSize || pos.Col < 0 || pos.Col >= GridSquareSize);
        }
    }
}
