﻿using System;
using System.Security.Cryptography;

namespace game {
    internal class Program
    {
        private const int ScreenWidth = 150;
        private const int ScreenHeight = 50;

        private const int MapWidth = 20;
        private const int MapHeight = 10;

        private static double _playerX;
        private static double _playerY;
        private static double _playerA = 0;

        private const double Depth = 16; // Константа для глубины видимости.
        private const double Fov = Math.PI / 3;

        private static string _map = "";
        private static int _collectedCoins;
        private const int CoinsToWin = 10;

        private static readonly char[] Screen = new char[ScreenHeight * ScreenWidth]; 
        static void Main(string[] args)
        {
            Console.SetWindowSize(ScreenWidth, ScreenHeight); // Устанавливаем размер окна консоли
            Console.SetBufferSize(ScreenWidth, ScreenHeight); // Устанавливаем размер буфера консоли
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false; // Скрываем курсор

            _map += "####################";
            _map += "#..$......P.$....$.#";
            _map += "###...#####....#####";
            _map += "#.....$.#..........#";
            _map += "#...#####.....#..$.#";
            _map += "#..$..#.......######";
            _map += "#########..........#";
            _map += "#....$.........#.$.#";
            _map += "#.$........$...#...#";
            _map += "####################";

            InitializePlayerPosition();


            DateTime dateTimeFrom = DateTime.Now;

            char c = ' ';
            while (true)
            {
                DateTime dateTimeTo = DateTime.Now;
                double elapsedTime = (dateTimeTo - dateTimeFrom).TotalSeconds;// Рассчитываем время, прошедшее с предыдущего кадра
                dateTimeFrom = DateTime.Now; // Обновляем время начала текущего кадра

                if (Console.KeyAvailable)
                {
                    var ConsoleKey = Console.ReadKey(true).Key;

                    switch (ConsoleKey)
                    {
                        case ConsoleKey.LeftArrow:
                            _playerA += 30 * elapsedTime;
                            break;
                        case ConsoleKey.RightArrow:
                            _playerA -= 30 * elapsedTime;
                            break;

                        case ConsoleKey.D:
                            MovePlayer(-Math.Cos(_playerA) * 50 * elapsedTime, Math.Sin(_playerA) * 50 * elapsedTime);
                            break;

                        case ConsoleKey.A:
                            MovePlayer(Math.Cos(_playerA) * 50 * elapsedTime, -Math.Sin(_playerA) * 50 * elapsedTime);
                            break;

                        case ConsoleKey.W:
                            MovePlayer(Math.Sin(_playerA) * 70 * elapsedTime, Math.Cos(_playerA) * 70 * elapsedTime);
                            break;

                        case ConsoleKey.S:
                            MovePlayer(-Math.Sin(_playerA) * 70 * elapsedTime, -Math.Cos(_playerA) * 70 * elapsedTime);
                            break;
                    }
                }
                RenderFrame();
                DisplayMiniMap();
            }
        }
        private static void InitializePlayerPosition()
        {
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    if (_map[y * MapWidth + x] == 'P')
                    {
                        _playerX = x;
                        _playerY = y;
                        return;
                    }
                }
            }
        }

        private static void MovePlayer(double deltaX, double deltaY)
        {
            double newX = _playerX + deltaX;
            double newY = _playerY + deltaY;
            if (newX >= 0 && newX < MapWidth && newY >= 0 && newY < MapHeight) { 

                if (_map[(int)newY * MapWidth + (int)newX] != '#')
                {
                    if (_map[(int)newY * MapWidth + (int)newX] == '$')
                    {
                        _collectedCoins++;
                        _map = _map.Remove((int)newY * MapWidth + (int)newX, 1).Insert((int)newY * MapWidth + (int)newX, " ");

                        if (_collectedCoins >= CoinsToWin)
                        {
                            Console.WriteLine("Поздравляем! Вы собрали достаточно монет для победы!");
                            Console.WriteLine("Нажмите любую клавишу для выхода...");
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                    }

                    _playerX = newX;
                    _playerY = newY;
                }
            }
        }

        private static void RenderFrame()
        {
            for (int x = 0; x < ScreenWidth; x++)
            {
                double rayAngel = _playerA + Fov / 2 - x * Fov / ScreenWidth;

                double rayX = Math.Sin(rayAngel);
                double rayY = Math.Cos(rayAngel);

                double distanceToWall = 0;
                bool hitWall = false;

                while (!hitWall && distanceToWall < Depth)
                {
                    distanceToWall += 0.1;

                    int testX = (int)(_playerX + rayX * distanceToWall);
                    int testY = (int)(_playerY + rayY * distanceToWall);

                    if (testX < 0 || testX >= MapWidth || testY >= MapHeight)
                    {
                        hitWall = true;
                        distanceToWall = Depth;
                    } else
                    {
                        char testCell = _map[testY * MapWidth + testX];

                        if (testCell == '#')
                        {
                            hitWall = true;
                        }
                    }
                }
                // Определяем разделение на потолок и пол
                int ceiling = (int)(ScreenHeight / 2d - ScreenHeight * Fov / distanceToWall);
                int floor = ScreenHeight - ceiling;

                char wallshade;

                if (distanceToWall < Depth / 4d) 
                {
                    wallshade = '\u2588';
                } else if (distanceToWall < Depth / 3d)
                {
                    wallshade = '\u2593';
                } else if (distanceToWall < Depth / 2d)
                {
                    wallshade = '\u2592';
                }
                else if (distanceToWall < Depth)
                {
                    wallshade = '\u2591';
                }
                else
                {
                    wallshade = ' ';
                }

                for (int y = 0; y < ScreenHeight; y++)
                {
                    if (y <= ceiling)
                    {
                        Screen[y * ScreenWidth + x] = ' ';
                    } else if (y > ceiling && y <= floor)
                    {
                        Screen[y * ScreenWidth + x] = wallshade; 
                    } else
                    {
                        Screen[y * ScreenWidth + x] = '.';
                    }
                }
                if (_map[(int)_playerY * MapWidth + (int)_playerX] == '$')
                {
                    Screen[(int)_playerY * ScreenWidth + (int)_playerX] = '$';
                }
            }
            Console.SetCursorPosition(0, 0);
            Console.Write(Screen);
        }
        private static void DisplayMiniMap()
        {
            Console.SetCursorPosition(0, ScreenHeight - MapHeight - 1); // Устанавливаем курсор перед мини-картой

            Console.WriteLine("Миникарта:");

            for (int y = 0; y < MapHeight; y++) // Проходим по высоте карты
            {
                for (int x = 0; x < MapWidth; x++) // Проходим по ширине карты
                {
                    if (x >= 0 && x < MapWidth && y >= 0 && y < MapHeight) // Проверка на пределы карты
                    {
                        char cell = _map[y * MapWidth + x]; // Получаем символ из карты по текущей позиции

                        if (x == (int)_playerX && y == (int)_playerY)
                        {
                            Console.Write("P ");
                        }
                        else
                        {
                            Console.Write($"{cell} ");
                        }
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine("Собрано монет: " + _collectedCoins);
        }
    }
}
