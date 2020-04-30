using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GOL_GFX
{
    public partial class MAIN_WINDOW : Form
    {
        readonly GOL gol;

        //Bitmap next;
        readonly Graphics gfx;

        public MAIN_WINDOW()
        {
            InitializeComponent();
            gol = new GOL(lifeBox.Width/10, lifeBox.Height/10);
            gfx = lifeBox.CreateGraphics();

            comboPreset.SelectedIndex = 0;
        }
        private void TimerLife_Tick(object sender, EventArgs e)
        {
            //Display current life state
            for (int x = 0; x < gol.width; x++)
                for (int y = 0; y < gol.height; y++)
                    if (gol.current.Cols[x].Cells[y].IsAlive)
                        SetPixel(x, y, Brushes.Black);
                    else
                        SetPixel(x, y, Brushes.White);
            
            //Create next generation
            gol.MakeLife();
        }

        private void SetPixel(int x, int y, Brush brush)
        {
            gfx.FillRectangle(brush, x*10, y*10, 8, 8);
        }

        private void ButtonStartStop_Click(object sender, EventArgs e)
        {
            if (TimerLife.Enabled == true)
            {
                TimerLife.Enabled = false;
                buttonStartStop.Text = "GO";
                buttonStep.Enabled = true;
            }
            else
            {
                TimerLife.Enabled = true;
                buttonStartStop.Text = "Pause";
                buttonStep.Enabled = false;
            }
        }

        private void ButtonStep_Click(object sender, EventArgs e)
        {
            TimerLife_Tick(sender, e);
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            TimerLife.Enabled = false;
            gol.InitializeLife(comboPreset.SelectedItem);
            TimerLife.Enabled = true;
        }
    }
    public class GRID
    {
        //List of columns
        public List<COL> Cols;
        public GRID(int NoOfCols, int NoOfRows)
        {
            //Allocate set of columns
            Cols = new List<COL>(NoOfCols);

            //Add actual colums to list
            for (int i = 0; i < NoOfCols; i++)
                Cols.Add(new COL(NoOfCols));

            //Add number of cells to each columns
            foreach (var row in Cols)
                for (int i = 0; i < NoOfRows; i++)
                    row.Cells.Add(new CELL());
        }
    }
    public class COL
    {
        //List of cells (row items in x-y pattern)
        public List<CELL> Cells;
        public COL(int NoOfCells)
        {
            //Allocate list of cells
            Cells = new List<CELL>(NoOfCells);
        }
    }
    public class CELL
    {
        public bool IsAlive = false;
    }

    //Main game of life object
    public class GOL
    {
        //Game of life attributes
        //Table size
        public int width;
        public int height;
        private readonly byte[,] _neighbors;

        //Life boards
        public GRID current; //current generation to display
        private readonly GRID next;   //temporary next generation

        //Create game of life object - class constructor
        public GOL(int w, int h)
        {
            width = w;
            height = h;

            _neighbors = new byte[width, height];

            current = new GRID(width, height);
            next = new GRID(width, height);

            InitializeLife(-1);
        }
        
        //Initial state of life board
        public void InitializeLife(object preset)
        {
            //Clear life boards
            foreach (var col in current.Cols)
                foreach (var cell in col.Cells)
                    cell.IsAlive = false;
            foreach (var col in next.Cols)
                foreach (var cell in col.Cells)
                    cell.IsAlive = false;

            //Draw some initial creatures on life board
            switch (preset)
            {
                case "Blinker":
                    {
                        DrawBlinker(current, 5, 5);
                        break;
                    }
                case "Glider":
                    {
                        DrawGlider(current, 10, 10);
                        break;
                    }
                case "Dakota":
                    {
                        DrawDakota(current, 20, 20);
                        break;
                    }
                case "Random":
                    {
                        DrawRandom(current);
                        break;
                    }
                default:
                    break;
            }

            //Copy initial board to current and reset neighbor table
            FillNeighbors();
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

            if (current.Cols[(x - 1 + width) % width].Cells[(y - 1 + height) % height].IsAlive) _n++;
            if (current.Cols[(x - 1 + width) % width].Cells[y].IsAlive) _n++;
            if (current.Cols[(x - 1 + width) % width].Cells[(y + 1) % height].IsAlive) _n++;
            if (current.Cols[x].Cells[(y - 1 + height) % height].IsAlive) _n++;
            if (current.Cols[x].Cells[(y + 1) % height].IsAlive) _n++;
            if (current.Cols[(x + 1) % width].Cells[(y - 1 + height) % height].IsAlive) _n++;
            if (current.Cols[(x + 1) % width].Cells[y].IsAlive) _n++;
            if (current.Cols[(x + 1) % width].Cells[(y + 1) % height].IsAlive) _n++;

            return _n;
        }

        //Create next generation of creatures
        internal void MakeLife()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    byte _n;
                    _n = _neighbors[x, y];

                    //Kill cell if under- or over-populated
                    if (current.Cols[x].Cells[y].IsAlive && (_n < 2 || _n > 3))
                        next.Cols[x].Cells[y].IsAlive = false;
                    //Kepp cell alive
                    if (current.Cols[x].Cells[y].IsAlive && (_n == 2 || _n == 3))
                        next.Cols[x].Cells[y].IsAlive = true;
                    //Resurect dead cell
                    if (!current.Cols[x].Cells[y].IsAlive && _n == 3)
                        next.Cols[x].Cells[y].IsAlive = true;
                }

            //Save created generation as current and fill in neighbors table
            current = next;
            FillNeighbors();
        }

        //Drawing methods
        //Blinker
        void DrawBlinker(GRID g, int x, int y)
        {
            g.Cols[x    ].Cells[y + 1].IsAlive = true;
            g.Cols[x + 1].Cells[y + 1].IsAlive = true;
            g.Cols[x + 2].Cells[y + 1].IsAlive = true;
        }
        //Glider
        void DrawGlider(GRID g, int x, int y)
        {
            g.Cols[x    ].Cells[y    ].IsAlive = true;
            g.Cols[x + 1].Cells[y    ].IsAlive = true;
            g.Cols[x + 2].Cells[y    ].IsAlive = true;
            g.Cols[x    ].Cells[y + 1].IsAlive = true;
            g.Cols[x + 1].Cells[y + 2].IsAlive = true;
        }
        //Dakota
        void DrawDakota(GRID g, int x, int y)
        {
            g.Cols[x + 1].Cells[y    ].IsAlive = true;
            g.Cols[x + 4].Cells[y    ].IsAlive = true;
            g.Cols[x    ].Cells[y + 1].IsAlive = true;
            g.Cols[x    ].Cells[y + 2].IsAlive = true;
            g.Cols[x + 4].Cells[y + 2].IsAlive = true;
            g.Cols[x    ].Cells[y + 3].IsAlive = true;
            g.Cols[x + 1].Cells[y + 3].IsAlive = true;
            g.Cols[x + 2].Cells[y + 3].IsAlive = true;
            g.Cols[x + 3].Cells[y + 3].IsAlive = true;
        }
        //Random board
        void DrawRandom(GRID g)
        {
            Random _rnd = new Random();
            foreach (var row in g.Cols)
                foreach (var cell in row.Cells)
                {
                    int _val = _rnd.Next(3);
                    if (_val < 1) cell.IsAlive = false;
                    else cell.IsAlive = true;
                }
        }
    }
}