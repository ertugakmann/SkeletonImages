

using System;
using System.IO;

namespace AQA_Graphics_CS
{
    class Program
    {
        const string EMPTY_STRING = "";
        const int MAX_WIDTH = 100;
        const int MAX_HEIGHT = 100;

        struct FileHeader
        {
            public string Title;
            public int Width;
            public int Height;
            public string FileType;
        }

        private static void SetHeader(ref FileHeader header)
        {
            header.Title = EMPTY_STRING;
            header.Width = MAX_WIDTH;
            header.Height = MAX_HEIGHT;
            header.FileType = EMPTY_STRING;
        }

        private static void DisplayError(string errorMessage)
        {
            Console.WriteLine("Error: " + errorMessage);
        }

        private static void PrintHeading(string heading)
        {
            Console.WriteLine(heading);
            {
                for (int position = 1; position <= heading.Length; position++)
                {
                    Console.Write("=");
                }
            }
            Console.WriteLine();
        }

        private static void DisplayImage(string[,] grid, FileHeader header)
        {
            Console.WriteLine();
            PrintHeading(header.Title);
            for (int thisRow = 0; thisRow < header.Height; thisRow++)
            {
                for (int thisColumn = 0; thisColumn < header.Width; thisColumn++)
                {
                    Console.Write(grid[thisRow, thisColumn]);
                }
                Console.WriteLine();
            }
        }

        private static void SaveImage(string[,] grid, FileHeader header)
        {
            string answer, fileName;
            Console.WriteLine("The current title of your image is: " + header.Title);
            Console.Write("Do you want to use this as your filename? (Y/N) ");
            answer = Console.ReadLine();
            if (answer == "N" || answer == "n")
            {
                Console.WriteLine("Enter a new filename: ");
                fileName = Console.ReadLine();
            }
            else
            {
                fileName = header.Title;
            }
            StreamWriter fileOut = new StreamWriter(fileName + ".txt");
            fileOut.WriteLine(header.Title);
            for (int row = 0; row < header.Height; row++)
            {
                for (int column = 0; column < header.Width; column++)
                {
                    fileOut.Write(grid[row, column]);
                }
                fileOut.WriteLine();
            }
            fileOut.Close();
        }

        private static void EditImage(string[,] grid, FileHeader header)
        {
            string symbol, newSymbol;
            DisplayImage(grid, header);
            string answer = EMPTY_STRING;
            while (answer != "N")
            {
                symbol = EMPTY_STRING;
                newSymbol = EMPTY_STRING;
                while (symbol.Length != 1)
                {
                    Console.Write("Enter the symbol you want to replace: ");
                    symbol = Console.ReadLine();
                }
                while (newSymbol.Length != 1)
                {
                    Console.Write("Enter the new symbol: ");
                    newSymbol = Console.ReadLine();
                }
                for (int thisRow = 0; thisRow < header.Height; thisRow++)
                {
                    for (int thisColumn = 0; thisColumn < header.Width; thisColumn++)
                    {
                        if (grid[thisRow, thisColumn] == symbol)
                        {
                            grid[thisRow, thisColumn] = newSymbol;
                        }
                    }
                }
                DisplayImage(grid, header);
                Console.Write("Do you want to make any further changes? (Y/N) ");
                answer = Console.ReadLine();
            }
        }

        private static string ConvertChar(int pixelValue)
        {
            string asciiChar = "";
            if (pixelValue <= 32)
            {
                asciiChar = "#";
            }
            else if (pixelValue <= 64)
            {
                asciiChar = "&";
            }
            else if (pixelValue <= 96)
            {
                asciiChar = "+";
            }
            else if (pixelValue <= 128)
            {
                asciiChar = ";";
            }
            else if (pixelValue <= 160)
            {
                asciiChar = ":";
            }
            else if (pixelValue <= 192)
            {
                asciiChar = ",";
            }
            else if (pixelValue <= 224)
            {
                asciiChar = ".";
            }
            else
            {
                asciiChar = " ";
            }
            return asciiChar;
        }

        private static void LoadGreyScaleImage(StreamReader fileIn, string[,] grid, FileHeader header)
        {
            string nextPixel;
            int pixelValue;
            try
            {
                for (int row = 0; row < header.Height; row++)
                {
                    for (int column = 0; column < header.Width; column++)
                    {
                        nextPixel = fileIn.ReadLine();
                        pixelValue = Convert.ToInt32(nextPixel);
                        grid[row, column] = ConvertChar(pixelValue);
                    }
                }
            }
            catch (Exception)
            {
                DisplayError("Image data error");
            }
        }

        private static void LoadAsciiImage(StreamReader fileIn, string[,] grid, FileHeader header)
        {
            string imageData = fileIn.ReadLine();
            int nextChar = 0;
            try
            {
                for (int row = 0; row < header.Height; row++)
                {
                    for (int column = 0; column < header.Width; column++)
                    {
                        grid[row, column] = imageData[nextChar].ToString();
                        nextChar += 1;
                    }
                }
            }
            catch (Exception)
            {
                DisplayError("Image data error");
            }
        }

        private static void LoadFile(string[,] grid, ref FileHeader header)
        {
            bool fileFound = false;
            bool fileTypeOK = false;
            string fileName, headerLine;
            Console.Write("Enter filename to load: ");
            fileName = Console.ReadLine();
            try
            {
                StreamReader fileIn = new StreamReader(fileName + ".txt");
                fileFound = true;
                headerLine = fileIn.ReadLine();
                string[] fields = headerLine.Split(',');
                header.Title = fields[0];
                header.Width = Convert.ToInt32(fields[1]);
                header.Height = Convert.ToInt32(fields[2]);
                header.FileType = fields[3];
                header.FileType = header.FileType[0].ToString();
                if (header.FileType == "A")
                {
                    LoadAsciiImage(fileIn, grid, header);
                    fileTypeOK = true;
                }
                else if (header.FileType == "G")
                {
                    LoadGreyScaleImage(fileIn, grid, header);
                    fileTypeOK = true;
                }
                fileIn.Close();
                if (!fileTypeOK)
                {
                    DisplayError("Unknown file type");
                }
                else
                {
                    DisplayImage(grid, header);
                }
            }
            catch (Exception)
            {
                if (!fileFound)
                {
                    DisplayError("File not found");
                }
                else
                {
                    DisplayError("Unknown error");
                }
            }
        }

        private static void SaveFile(string[,] grid, FileHeader header)
        {
            string fileName;
            Console.Write("Enter filename: ");
            fileName = Console.ReadLine();
            StreamWriter fileOut = new StreamWriter(fileName + ".txt");
            fileOut.WriteLine(header.Title + "," + header.Width + "," + header.Height + "," + "A");
            for (int row = 0; row < header.Height; row++)
            {
                for (int column = 0; column < header.Width; column++)
                {
                    fileOut.Write(grid[row, column]);
                }
            }
            fileOut.Close();
        }

        private static void ClearGrid(string[,] grid)
        {
            for (int row = 0; row < MAX_HEIGHT; row++)
            {
                for (int column = 0; column < MAX_WIDTH; column++)
                {
                    grid[row, column] = ".";
                }
            }
        }

        private static void DisplayMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Main Menu");
            Console.WriteLine("=========");
            Console.WriteLine("L - Load graphics file");
            Console.WriteLine("D - Display image");
            Console.WriteLine("E - Edit image");
            Console.WriteLine("R - Resize image");
            Console.WriteLine("S - Save image");
            Console.WriteLine("X - Exit program");
            Console.WriteLine();
        }

        private static char GetMenuOption()
        {
            string menuOption = EMPTY_STRING;
            while (menuOption.Length != 1)
            {
                Console.Write("Enter your choice: ");
                menuOption = Console.ReadLine();
            }
            return menuOption[0];
        }

        private static void Graphics()
        {
            string[,] grid = new string[MAX_HEIGHT, MAX_WIDTH];
            ClearGrid(grid);
            FileHeader header = new FileHeader();
            SetHeader(ref header);
            bool programEnd = false;
            char menuOption;
            char answer;
            while (!programEnd)
            {
                DisplayMenu();
                menuOption = GetMenuOption();
                if (menuOption == 'L')
                {
                    LoadFile(grid, ref header);
                }
                else if (menuOption == 'D')
                {
                    DisplayImage(grid, header);
                }
                else if (menuOption == 'E')
                {
                    EditImage(grid, header);
                }
                else if (menuOption == 'S')
                {
                    SaveImage(grid, header);
                }
                else if (menuOption == 'R')
                {
                    ResizeImage(grid, header);
                }
                else if (menuOption == 'X')
                {
                    programEnd = true;
                }
                else
                {
                    Console.WriteLine("You did not choose a valid menu option. Try again");
                }
            }
            Console.WriteLine("You have chosen to exit the program");
            Console.Write("Do you want to save the image as a graphics file? (Y/N) ");
            answer = Convert.ToChar(Console.ReadLine());
            if (answer == 'Y' || answer == 'y')
            {
                SaveFile(grid, header);
            }
        }

        private static void ResizeImage(string[,] grid, FileHeader header)
        {
            int scaleRow = 0;
            int scaleCol = 0;
            int scale = 2;

            string[,] newGrid = new string[header.Height * scale, header.Width * scale];

            for (int thisRow = 0; thisRow < header.Height * scale; thisRow++)
            {
                for (int thisCol = 0; thisCol < header.Width * scale; thisCol++)
                {
                    scaleRow = thisRow / 2;
                    scaleCol = thisCol / 2;

                    newGrid[thisRow, thisCol] = grid[scaleRow, scaleCol];
                }
            }

            for (int thisRow = 0; thisRow < header.Height * scale; thisRow++)
            {
                for (int thisColumn = 0; thisColumn < header.Width * scale; thisColumn++)
                {
                    Console.Write(newGrid[thisRow, thisColumn]);
                }
                Console.WriteLine();
            }
        }


        static void Main(string[] args)
        {
            Graphics();
        }
    }
}
