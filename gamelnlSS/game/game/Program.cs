using System;

namespace game {
    internal class Program
    {
        // Константы для размеров экрана и карты
        private const int ScreenWidth = 150;
        private const int ScreenHeight = 50;

        private const int MapWidth = 32;
        private const int MapHeight = 32;

        // Переменные для позиции и угла поворота игрока
        private static double _playerX = 1.5;
        private static double _playerY = 1.5;
        private static double _playerA = 0;

        // Константы для глубины отрисовки и поля зрения
        private const double Depth = 16;
        private const double Fov = Math.PI / 3;

        // Строка, представляющая карту
        private static string _map = "";

        // Массив символов для буфера экрана
        private static readonly char[] Screen = new char[ScreenHeight * ScreenWidth];
        static void Main(string[] args)
        {
            // Настройка консоли
            Console.SetWindowSize(ScreenWidth, ScreenHeight);
            Console.SetBufferSize(ScreenWidth, ScreenHeight);
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            _map += "################################";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "################################";

            while (true)
            {
                RenderFrame();
                Thread.Sleep(1000);
            }
        }

        private static void RenderFrame()
        {
            // Цикл по всем столбцам экрана
            for (int x = 0; x < ScreenWidth; x++)
            {
                // Вычисление угла луча
                double rayAngel = _playerA + Fov / 2 - x * Fov / ScreenWidth;

                // Вычисление направления луча
                double rayX = Math.Sin(rayAngel);
                double rayY = Math.Cos(rayAngel);

                // Инициализация расстояния до стены и признака попадания
                double distanceToWall = 0;
                bool hitWall = false;

                // Цикл прохождения луча до тех пор, пока не будет встречена стена или достигнут предел глубины
                while (!hitWall && distanceToWall < Depth)
                {
                    distanceToWall += 0.1;

                    // Вычисляем координаты точки, в которую попадает луч
                    int testX = (int)(_playerX + rayX * distanceToWall);
                    int testY = (int)(_playerY + rayY * distanceToWall);

                    // Проверка, не вышла ли точка за пределы карты
                    if (testX < 0 || testX >= MapWidth || testY >= MapHeight)
                    {
                        hitWall = true;
                        distanceToWall = Depth;
                    } else
                    {
                        // Получаем символ в текущей клетке карты
                        char testCell = _map[testY * MapWidth + testX];

                        // Проверяем, является ли клетка стеной
                        if (testCell == '#')
                        {
                            hitWall = true;
                        }
                    }
                }
                // Вычисляем высоту стены на экране
                int ceiling = (int)(ScreenHeight / 2d - ScreenHeight * Fov / distanceToWall);
                int floor = ScreenHeight - ceiling;

                // Выбираем символ для стены
                char wallshade;

                //Проверяет, насколько близко расстояние до стены, по сравнению с максимальной глубиной 
                if (distanceToWall < Depth / 4d) //Если расстояние до стены меньше четверти глубины
                {
                    wallshade = '\u2588'; // Полная закрашенная клетка
                } else if (distanceToWall < Depth / 3d)
                {
                    wallshade = '\u2593'; // Темная клетка
                } else if (distanceToWall < Depth / 2d)
                {
                    wallshade = '\u2592'; // Полузакрашенная клетка
                }
                else if (distanceToWall < Depth)
                {
                    wallshade = '\u2591'; // Светлая клетка
                }
                else
                {
                    wallshade = ' ';
                }

                for (int y = 0; y < ScreenHeight; y++)
                {
                    if (y <= ceiling) //Если текущая строка меньше или равна высоте потолка
                    {
                        Screen[y * ScreenWidth + x] = ' ';
                    } else if (y > ceiling && y <= floor) //Если текущая строка находится между потолком и полом
                    {
                        Screen[y * ScreenWidth + x] = wallshade; 
                    } else //Если текущая строка ниже пола
                    {
                        Screen[y * ScreenWidth + x] = '.';
                    }
                }
            }

            Console.SetCursorPosition(0, 0); //Устанавливает курсор в верхний левый угол консоли, чтобы перерисовать экран
            Console.Write(Screen); //Выводит содержимое массива Screen в консоль
        }
    }
}
