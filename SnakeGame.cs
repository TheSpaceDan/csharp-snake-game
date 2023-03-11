using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace SnakeGame
{
    public class Position
    {
        public int left;
        public int top;
    }

    class Program
    {
        public static bool _inPlay = true;

        public static int _length = 6;
        public static int _score = 0;

        public static List<Position> points = new List<Position>();

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            DrawScreen();
            while (_inPlay)
            {
                if (AcceptInput() || UpdateGame())
                    DrawScreen();
            }
        }

        public static DateTime nextUpdate = DateTime.MinValue;
        public static Position _foodPosition = null;
        public static Random _rnd = new Random();
        public static bool UpdateGame()
        {
            if (DateTime.Now < nextUpdate) return false;

            if (_foodPosition == null)
            {
                _foodPosition = new Position()
                {
                    left = _rnd.Next(Console.WindowWidth),
                    top = _rnd.Next(Console.WindowHeight)
                };
            }

            if (_lastKey.HasValue)
            {
                Move(_lastKey.Value);
            }

            if (_score <= 3) { nextUpdate = DateTime.Now.AddMilliseconds(100); }
            else if (_score > 3 && _score <= 6) { nextUpdate = DateTime.Now.AddMilliseconds(75); }
            else if (_score > 6 && _score <= 9) { nextUpdate = DateTime.Now.AddMilliseconds(65); }
            else if (_score > 9) { nextUpdate = DateTime.Now.AddMilliseconds(50); }

            return true;
        }

        public static void DrawScreen()
        {
            Console.Clear();
            
            //score
            Console.SetCursorPosition(Console.WindowWidth - 3, Console.WindowHeight - 1);
            Console.Write(_score);

            foreach (var point in points)
            {
                Console.SetCursorPosition(point.left, point.top);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write('O');
            }

            if (_foodPosition != null)
            {
                Console.SetCursorPosition(_foodPosition.left, _foodPosition.top);
                Console.Write('*');
            }
        }

        static ConsoleKeyInfo? _lastKey;
        public static bool AcceptInput()
        {
            if (!Console.KeyAvailable)
                return false;

            _lastKey = Console.ReadKey();

            return true;
        }

        public static void Move(ConsoleKeyInfo key)
        {
            Position currentPos;

            if (points.Count != 0)
                currentPos = new Position() { left = points.Last().left, top = points.Last().top };
            else
                currentPos = GetStartPosition();

            switch (key.Key)
            {
                case ConsoleKey.A:
                    currentPos.left--;
                    break;
                case ConsoleKey.D:
                    currentPos.left++;
                    break;
                case ConsoleKey.W:
                    currentPos.top--;
                    break;
                case ConsoleKey.S:
                    currentPos.top++;
                    break;

            }

            DetectCollision(currentPos);

            points.Add(currentPos);
            CleanUp();
        }

        public static void DetectCollision(Position currentPos)
        {
            // Check if we're off the screen
            if (currentPos.top < 0 || currentPos.top > Console.WindowHeight
                || currentPos.left < 0 || currentPos.left > Console.WindowWidth)
            {
                GameOver();
            }

            // Check if we've crashed into the tail
            if (points.Any(p => p.left == currentPos.left && p.top == currentPos.top))
            {
                GameOver();
            }

            // Check if we've eaten the food
            if (_foodPosition.left == currentPos.left && _foodPosition.top == currentPos.top)
            {
                _length++;
                _score++;
                _foodPosition = null;
            }
        }

        public static void GameOver()
        {
            _inPlay = false;
            Console.Clear();
            Console.WriteLine("Game Over!");
            Console.ReadLine();
        }

        public static Position GetStartPosition()
        {
            return new Position()
            {
                left = 0,
                top = 0
            };
        }

        public static void CleanUp()
        {
            while (points.Count() > _length)
            {
                points.Remove(points.First());
            }
        }

    }
}
