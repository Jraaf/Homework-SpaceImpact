using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceImpact
{
    internal class Game
    {
        int x = 10;
        int y = 10;
        public void Start()
        {
            SetWindowSettings();

            DrawShip(x, y);
            while (true)
            {
                Action();
            }
            Console.ReadLine();
        }
        void DrawShip(int x, int y)
        {
            //Console.Clear();
            Console.SetCursorPosition(x, y);
            Console.Write("        "); Console.SetCursorPosition(x, ++y);
            Console.Write("  ┌┐    "); Console.SetCursorPosition(x, ++y);
            Console.Write(" ├┼╪═══ "); Console.SetCursorPosition(x, ++y);
            Console.Write("  └┘    "); Console.SetCursorPosition(x, ++y);
            Console.Write("        ");
        }
        void Action()
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.A:
                    if (x == 0) x = 1;
                    DrawShip(--x, y);
                    break;
                case ConsoleKey.W:
                    if (y == 0) y = 1;
                    DrawShip(x, --y);
                    break;
                case ConsoleKey.S:
                    if (y == Console.BufferHeight - 5) y--;
                    DrawShip(x, ++y);
                    break;
                case ConsoleKey.D:
                    if (x == Console.BufferWidth - 8) x--;
                    DrawShip(++x, y);
                    break;
                case ConsoleKey.Enter:
                    int x1 = x;
                    int y1 = y;
                    ThreadPool.GetAvailableThreads(out int workerThreads, out int comThreads);
                    if (workerThreads > 0)
                    {
                        ThreadPool.QueueUserWorkItem((o) =>
                        {
                            Thread Shooter = new Thread(() => Shoot(x1, y1));
                            Shooter.Start();
                        });
                    }
                    break;
            }
        }
        //public Thread Shooter = new Thread(() => Shoot());
        void SetWindowSettings()
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(100, 25);
            Console.SetBufferSize(100, 25);
        }

        void Shoot(int x, int y)
        {

            x += 6;
            y += 2;
            for (; x < Console.BufferWidth - 8; x++)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(" ═>");
                Thread.Sleep(25);
                Console.SetCursorPosition(x, y);
                Console.Write("     ");
            }
        }

    }
}
