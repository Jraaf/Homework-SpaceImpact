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
        //start position pf our ship
        static int x = 10;
        static int y = 10;
        static Random random = new Random();
        //maximum number of enemies on screen
        static int MaxEnemyCount = 7;

        static List<int[]>EnemyCoords=new List<int[]>();
        static List<int[]>BulletCoords=new List<int[]>();

        #region Models

        static string[] Ship ={
            "        " ,
            "  ┌┐    " ,
            " ├┼╪═══ " ,
            "  └┘    ",
            "        "};

        static string[] Enemy ={
            "  <═╗   ",
            "(═══╬─┤ ",
            "  <═╝   "};

        static string[] ClearEnemy ={
            "       ",
            "       ",
            "       "};
        static string[] Bullet = {
            " =>" };

        static string[] ClearBullet ={
            "   " };

        #endregion

        public void Start()
        {
            for (int i = 0; i < MaxEnemyCount; i++)
            {
                EnemyCoords.Add(new int[]
                    {
                        Console.BufferWidth - 5,
                        random.Next(1, Console.BufferHeight - 6)
                    });
            }
            SetWindowSettings();
            BattleThread.Start();
            DrawModel(x, y, Ship);
            while (true)
            {
                Action();
            }
        }
        static void DrawModel(int x, int y, string[] Model)
        {            
            Console.SetCursorPosition(x, y);
            foreach (var line in Model)
            {
                Console.Write(line);
                Console.SetCursorPosition(x, ++y);
            }
        }
        void Action()
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.A:
                    if (x == 0) x = 1;
                    DrawModel(--x, y, Ship);
                    break;
                case ConsoleKey.W:
                    if (y == 0) y = 1;
                    DrawModel(x, --y, Ship);
                    break;
                case ConsoleKey.S:
                    if (y == Console.BufferHeight - 6) y--;
                    DrawModel(x, ++y, Ship);
                    break;
                case ConsoleKey.D:
                    if (x == Console.BufferWidth - 8) x--;
                    DrawModel(++x, y, Ship);
                    break;
                case ConsoleKey.Enter:
                    var shooting = Task.Factory.StartNew(() =>
                    {
                        Shoot(x, y+1);
                    });
                    //Task.WaitAll(shooting);
                    break;
            }
        }
        void SetWindowSettings()
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(100, 25);
            Console.SetBufferSize(100, 25);
        }
        void Shoot(int x, int y)
        {
            BulletCoords.Add(new int[]{ x + 6, y + 1 });
        }

        Thread BattleThread=new Thread(() =>
        {
            while (true)
            {
                BulletsAndEnemiesInteraction();
                Thread.Sleep(25);
            }
        });
        static void BulletsAndEnemiesInteraction()
        {
            MoveBullets();
            MoveEnenemies();
            HitCheck();
            LooseCheck();
        }
        static void MoveBullets()
        {
            List<int> BulletsOutOfScreenBuffer = new List<int>();
            for (int i = 0; i < BulletCoords.Count; i++)
            {
                try
                {
                    BulletCoords[i][0]++;
                    if (BulletCoords[i][0]>Console.BufferWidth-3)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    DrawModel(BulletCoords[i][0], BulletCoords[i][1], Bullet);
                }
                catch (ArgumentOutOfRangeException) 
                {
                    BulletsOutOfScreenBuffer.Add(i);
                }                
            }
            foreach (var BulletPos in BulletsOutOfScreenBuffer)
            {
                DrawModel(BulletCoords[BulletPos][0], BulletCoords[BulletPos][1], ClearBullet);
                BulletCoords.RemoveAt(BulletPos);
            }
        }
        static void MoveEnenemies()
        {
            for (int i = 0; i < EnemyCoords.Count; i++)
            {
                try
                {
                    EnemyCoords[i][0]--;
                    if (EnemyCoords[i][0] < 0)
                    {
                        DrawModel(EnemyCoords[i][0]+1, EnemyCoords[i][1], ClearEnemy);
                        throw new ArgumentOutOfRangeException();
                    }
                    DrawModel(EnemyCoords[i][0], EnemyCoords[i][1], Enemy);
                }
                catch (ArgumentOutOfRangeException)
                {                   
                    EnemyCoords[i] = new int[]
                    {
                        Console.BufferWidth - 5,
                        random.Next(1, Console.BufferHeight - 6)
                    };
                }
            }
        }
        static void HitCheck()
        {
            List<int> BulletsThatHit = new List<int>();
            List<int> DestroyedEnemies = new List<int>();

            for (int i = 0; i < EnemyCoords.Count; i++)
            {
                for (int j = 0; j < BulletCoords.Count; j++)
                {
                    if (EnemyCoords[i][1] == BulletCoords[j][1] ||
                        EnemyCoords[i][1] + 1 == BulletCoords[j][1] ||
                        EnemyCoords[i][1] + 2 == BulletCoords[j][1])
                    {
                        if (EnemyCoords[i][0] - 4 == BulletCoords[j][0] + 3)
                        {
                            BulletsThatHit.Add(j);
                            DestroyedEnemies.Add(i);
                            DrawModel(EnemyCoords[i][0], EnemyCoords[i][1], ClearEnemy);
                            DrawModel(BulletCoords[j][0], BulletCoords[j][1], ClearBullet);
                        }
                    }
                }
            }
            foreach (var BulletNumber in BulletsThatHit)
            {
                try
                {
                    BulletCoords.RemoveAt(BulletNumber);
                }
                catch (ArgumentOutOfRangeException) { };
            }
            foreach (var EnemyNumber in DestroyedEnemies)
            {
                EnemyCoords[EnemyNumber] = new int[]
                {
                    Console.BufferWidth - 5,
                    random.Next(1, Console.BufferHeight - 6)
                };
            }
        }
        static void LooseCheck()
        {
            foreach (var EnemyCoord in EnemyCoords)
            {
                if (x == EnemyCoord[0])
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (y + 1+i == EnemyCoord[1]     ||
                            y + 1+i == EnemyCoord[1] + 1 ||
                            y + 1+i == EnemyCoord[1] + 2 )
                        {
                            Environment.Exit(0);
                        }
                    }

                }
            }
        }
    }
}
