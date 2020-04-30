using System;
using System.Threading;

namespace GOL
{
    //Main game of life object
    /// <summary>
    /// Game of life object
    /// </summary>
    class GOL
    {
        //Game of life attributes
        public int width; //table size
        public int height; 
        private char _alive = '▒';  //▫▪■█●░▒▓
        private char _dead = ' ';
        public int generation;

        //Table of creatures
        public char[,] life;
        private char[,] _next;
        
        //Table of neighbors
        private byte[,] _neighbors;

        //Create game of life object - class constructor
        /// <summary>
        /// Create new game of life with desired board size
        /// </summary>
        /// <param name="boardWidth">Board width</param>
        /// <param name="boardHeight">Board height</param>
        public GOL(int boardWidth, int boardHeight)
        {
            width = boardWidth;
            height = boardHeight;
            generation = 1;

            //current = new bool[width, height];
            life = new char[width, height];
            _next = new char[width, height];
            _neighbors = new byte[width, height];

            //Initialize current generation with dead creatures
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    life[x, y] = _dead;
        }

        //Fill neighbors table
        private void FillNeighbors()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    _neighbors[x, y] = GetNeighbors(x, y);
        }

        //Count neighbors for given creature
        private byte GetNeighbors(int x, int y)
        {
            //Reset neighbors counter
            byte _n = 0;

            //Check all neighbors for input creature
            if (life[(x - 1 + width) % width, (y - 1 + height) % height] == _alive) _n++;
            if (life[(x - 1 + width) % width, y                        ] == _alive) _n++;
            if (life[(x - 1 + width) % width, (y + 1) % height         ] == _alive) _n++;
            if (life[x,                       (y - 1 + height) % height] == _alive) _n++;
            if (life[x,                       (y + 1) % height         ] == _alive) _n++;
            if (life[(x + 1) % width,         (y - 1 + height) % height] == _alive) _n++;
            if (life[(x + 1) % width,         y                        ] == _alive) _n++;
            if (life[(x + 1) % width,         (y + 1) % height         ] == _alive) _n++;
            
            return _n;
        }

        //Create next generation of creatures
        public void MakeLife()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    byte _n = _neighbors[x, y];
                    char _l = life[x, y];

                    if (_l == _alive)
                        if (_n < 2 || _n > 3) //kill - over/under populated
                            _next[x, y] = _dead;
                        else
                            _next[x, y] = _alive; //keep alive
                    else
                        if (_n == 3)
                            _next[x, y] = _alive; //procreate
                }

            //Save created generation as current and fill in neighbors table
            life = _next;
            FillNeighbors();
            generation++;
        }

        //Birht-giving methods - add creatures to the board at desired position
        private void addCreature(int x, int y)
        {
            life[x % width, y % height] = _alive;
        }
        //BLINKER
        public void addLife_Blinker(int x, int y)
        {
           addLife_Generic(new string[] { " * ",
                                          " * ",
                                          " * "}, x, y);
        }
        //FROG
        public void addLife_Frog(int x, int y)
        {
            addLife_Generic(new string[] {" ** ",
                                          "*   ",
                                          "   *",
                                          " ** "}, x, y);
        }
        //GLIDER
        public void addLife_Glider(int x, int y)
        {
            addLife_Generic(new string[] { " * ",
                                           "  *",
                                           "***"}, x, y);
        }
        //DAKOTA
        public void addLife_Dakota(int x, int y)
        {
            addLife_Generic(new string[] { " *  *",
                                           "*    ",
                                           "*   *",
                                           "**** "}, x, y);
        }
        //DIEHARD
        public void addLife_Diehard(int x, int y)
        {
            addLife_Generic(new string[] { "      * ",
                                           "**      ",
                                           " *   ***"}, x, y);
        }
        //R-PENTOMINO
        /// <summary>
        /// Create R-pentomino creature
        /// </summary>
        /// <param name="x">Initial x-position of top-left corner</param>
        /// <param name="y">Initiali y-position of top-left corner</param>
        public void addLife_Pentomino(int x, int y)
        {
            addLife_Generic(new string[] { " **",
                                           "** ",
                                           " * "}, x, y);
        }
        //ETERNAL GROWTH
        /// <summary>
        /// Constantly growing structure
        /// </summary>
        /// <param name="x">Initial x-position of top-left corner</param>
        /// <param name="y">Initiali y-position of top-left corner</param>
        public void addLife_EternalGrowth(int x, int y)
        {
            addLife_Generic(new string[] { "*** *",
                                           "*    ",
                                           "   **",
                                           " ** *",
                                           "* * *"}, x, y);
        }
        //GENERIC ROUTINE
        /// <summary>
        /// Add generic pattern to life table
        /// </summary>
        /// <param name="life">Table of strings (rows) representing life table. Every non-space character means cell is alive</param>
        private void addLife_Generic(string[] life, int x, int y)
        {
            int posy = y;

            foreach (var s in life)
            {
                for (int posx = 0; posx < s.Length; posx++)
                {
                    if (s.Substring(posx,1) != " ")
                    addCreature(x + posx, posy);
                }

                posy++;
            }

            FillNeighbors();
        }
        /// <summary>
        /// Add random pattern of creatures to life table
        /// </summary>
        //RANDOM
        public void addLife_Random()
        {
            Random _rnd = new Random();

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (_rnd.Next(3) > 1) addCreature(x, y);

            FillNeighbors();
        }
    }

    //Main program class
    class Program
    {
        static void Main(string[] args)
        {
            //Create a population
            GOL gol = new GOL(150, 60);
            gol.addLife_Blinker(0, 0);
            //gol.addLife_Frog(10, 10);
            gol.addLife_Glider(15, 15);
            //gol.addLife_Dakota(0, 5);
            //gol.addLife_Dakota(80, 15);
            //gol.addLife_Diehard(20, 10);
            //gol.addLife_Diehard(60, 50);
            //gol.addLife_Pentomino(30, 10);
            //gol.addLife_EternalGrowth(110, 55);
            //gol.addLife_Random();

            //Setup concole output
            Console.CursorVisible = false;
            //if (gol.width > Console.LargestWindowWidth) gol.width = Console.LargestWindowWidth;
            //if (gol.height > Console.LargestWindowHeight - 1) gol.height = Console.LargestWindowHeight - 1;
            Console.SetWindowSize(gol.width, gol.height);
            Console.SetBufferSize(gol.width, gol.height);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            //Infinite loop of life
            while (true)
            {
                //if (gol.generation % 150000000 == 0)
                {
                    DrawLife(gol);
                    Thread.Sleep(150);
                }
                gol.MakeLife();
            }
        }

        //Draw creatures from GOL object with desired character
        static void DrawLife(GOL gol)
        {
            //Buffer of bytes to be written on screen
            byte[] buf = new byte[gol.width * gol.height];
            char[] geninfo = ("Generation: " + gol.generation).ToCharArray();

            int pos = 0;
            var conout = Console.OpenStandardOutput(buf.Length);

            //Copy 2-dim life table to 1-dim buffer
            for (int y = 0; y < gol.height; y++)
                for (int x = 0; x < gol.width; x++)
                    buf[pos++] = (byte)gol.life[x, y];

            //Add generation number info to last line of screen buffer
            for (int i = 0; i < geninfo.Length; i++)
            {
                buf[(gol.width * gol.height) - gol.width + 1 + i] = (byte)geninfo[i];
            }

            conout.Write(buf, 0, buf.Length);
            conout.Flush();
        }
    }
}