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

    public class MainLoop
    {
        
        public static bool inPlay = true;
        public static List<Position> points = new List<Position>();

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            UpdateScreen.DrawScreen();
            while (inPlay)
            {
                if (Input.AcceptInput() || UpdateScreen.UpdateGame())
                    UpdateScreen.DrawScreen();
            }
        }
        public static void GameOver()
        {
            inPlay = false;
            Console.Clear();
            Console.WriteLine("Game Over!");
            Console.ReadLine();
        }
    }
     
    public class UpdateScreen
    {
        public static int length = 6;
        public static int score = 0;
        public static DateTime nextUpdate = DateTime.MinValue;
        public static Position foodPosition = null;
        public static Random rnd = new Random();
        public static bool UpdateGame()
        {
            if (DateTime.Now < nextUpdate) return false;

            if (foodPosition == null)
            {
                foodPosition = new Position()
                {
                    left = rnd.Next(Console.WindowWidth),
                    top = rnd.Next(Console.WindowHeight)
                };
            }

            if (Input.lastKey.HasValue)
            {
                Input.Move(Input.lastKey.Value);
            }

            if (score <= 3) { nextUpdate = DateTime.Now.AddMilliseconds(100); }
            else if (score > 3 && score <= 6) { nextUpdate = DateTime.Now.AddMilliseconds(75); }
            else if (score > 6 && score <= 9) { nextUpdate = DateTime.Now.AddMilliseconds(65); }
            else if (score > 9) { nextUpdate = DateTime.Now.AddMilliseconds(50); }

            return true;
        }

        public static void DrawScreen()
        {
            Console.Clear();

            //score
            Console.SetCursorPosition(Console.WindowWidth - 3, Console.WindowHeight - 1);
            Console.Write(score);

            foreach (var point in MainLoop.points)
            {
                Console.SetCursorPosition(point.left, point.top);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write('O');
            }

            if (foodPosition != null)
            {
                Console.SetCursorPosition(foodPosition.left, foodPosition.top);
                Console.Write('*');
            }
        }
    }
    
    class Input
    {
        public static ConsoleKeyInfo? lastKey;
        public static bool AcceptInput()
        {
            if (!Console.KeyAvailable)
                return false;

            lastKey = Console.ReadKey();

            return true;
        }

        public static void Move(ConsoleKeyInfo key)
        {
            Position currentPos;

            if (MainLoop.points.Count != 0)
                currentPos = new Position() { left = MainLoop.points.Last().left, top = MainLoop.points.Last().top };
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

            CollisionDetection.DetectCollision(currentPos);

            MainLoop.points.Add(currentPos);
            CleanUp();
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
            while (MainLoop.points.Count() > UpdateScreen.length)
            {
                MainLoop.points.Remove(MainLoop.points.First());
            }
        }
    }

    public class CollisionDetection
    {
        MainLoop obj = new MainLoop();
        public static void DetectCollision(Position currentPos)
        {
            
            // Check if we're off the screen
            if (currentPos.top < 0 || currentPos.top > Console.WindowHeight
                || currentPos.left < 0 || currentPos.left > Console.WindowWidth)
            {
                MainLoop.GameOver();
            }
            
            // Check if we've crashed into the tail
            if (MainLoop.points.Any(p => p.left == currentPos.left && p.top == currentPos.top))
            {
                MainLoop.GameOver();
            }

            // Check if we've eaten the food
            if (UpdateScreen.foodPosition.left == currentPos.left && UpdateScreen.foodPosition.top == currentPos.top)
            {
                UpdateScreen.length++;
                UpdateScreen.score++;
                UpdateScreen.foodPosition = null;
            }
        }
    }
}
