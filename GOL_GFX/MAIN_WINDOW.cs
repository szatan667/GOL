using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GOL_GFX
{
    public partial class MAIN_WINDOW : Form
    {
        GOL Gol { get; set; }

        //Bitmap next;
        private readonly Graphics gfx;

        public MAIN_WINDOW()
        {
            InitializeComponent();
            Gol = new(lifeBox.Width / 10, lifeBox.Height / 10);
            gfx = lifeBox.CreateGraphics();

            comboPreset.SelectedIndex = 0;
        }
        private void TimerLife_Tick(object s, EventArgs e)
        {
            //Display current life state
            for (int x = 0; x < Gol.Width; x++)
                for (int y = 0; y < Gol.Height; y++)
                    //if (Gol.Current.Cols[x].Cells[y].IsAlive)
                    if (Gol.GetLife(x,y))
                        SetPixel(x, y, Brushes.Black);
                    else
                        SetPixel(x, y, Brushes.White);
            
            //Create next generation
            Gol.MakeLife();
        }

        private void SetPixel(int x, int y, Brush Brush) => 
            gfx.FillRectangle(Brush, x * 10, y * 10, 8, 8);

        private void ButtonStartStop_Click(object s, EventArgs e)
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

        private void ButtonStep_Click(object s, EventArgs e) => 
            TimerLife_Tick(s, e);

        private void ButtonReset_Click(object s, EventArgs e)
        {
            TimerLife.Enabled = false;
            Gol.InitializeLife(comboPreset.SelectedItem);
            TimerLife.Enabled = true;
        }
    }
    public class GRID
    {
        //List of columns
        public List<COL> Cols { get; set; }
        public GRID(int NoOfCols, int NoOfRows)
        {
            //Allocate set of columns
            Cols = new(NoOfCols);

            //dd actual colums to list
            for (int i = 0; i < NoOfCols; i++)
                Cols.Add(new(NoOfCols));

            //Add number of cells to each columns
            foreach (COL row in Cols)
                for (int i = 0; i < NoOfRows; i++)
                    row.Cells.Add(new());
        }
    }
    public class COL
    {
        public List<CELL> Cells { get; set; }
        public COL(int NoOfCells) =>
            Cells = new(NoOfCells);
    }
    public class CELL
    {
        public bool IsAlive { get; set; }
        public CELL() =>
            IsAlive = false;
    }

    //Main game of life object
    public class GOL
    {
        //Game of life attributes
        //Table size
        public int Width { get; set; }
        public int Height { get; set; }
        private readonly byte[,] _neighbors;

        //Life boards
        private GRID current; //current generation
        private readonly GRID next; //temporary next generation

        //Create game of life object - class constructor
        public GOL(int GameWidth, int GameHeight)
        {
            Width = GameWidth;
            Height = GameHeight;

            _neighbors = new byte[Width, Height];

            current = new(Width, Height);
            next = new(Width, Height);

            InitializeLife(-1);
        }
        
        //Initial state of life board
        public void InitializeLife(object preset)
        {
            //Clear life boards
            foreach (COL col in current.Cols)
                foreach (CELL cell in col.Cells)
                    cell.IsAlive = false;
            foreach (COL col in next.Cols)
                foreach (CELL cell in col.Cells)
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
        
        //Get life status of single cell
        public bool GetLife(int x, int y)
        {
            if (current.Cols[x].Cells[y].IsAlive)
                return true;
            return false;
        }

        //Fill neighbors table
        private void FillNeighbors()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    _neighbors[x, y] = GetNeighbors(x, y);
        }

        //Count neighbors for given creature
        private byte GetNeighbors(int x, int y)
        {
            //Reset neighbors counter
            byte _n = 0;

            if (current.Cols[(x - 1 + Width) % Width].Cells[(y - 1 + Height) % Height].IsAlive) _n++;
            if (current.Cols[(x - 1 + Width) % Width].Cells[y].IsAlive) _n++;
            if (current.Cols[(x - 1 + Width) % Width].Cells[(y + 1) % Height].IsAlive) _n++;
            if (current.Cols[x].Cells[(y - 1 + Height) % Height].IsAlive) _n++;
            if (current.Cols[x].Cells[(y + 1) % Height].IsAlive) _n++;
            if (current.Cols[(x + 1) % Width].Cells[(y - 1 + Height) % Height].IsAlive) _n++;
            if (current.Cols[(x + 1) % Width].Cells[y].IsAlive) _n++;
            if (current.Cols[(x + 1) % Width].Cells[(y + 1) % Height].IsAlive) _n++;

            return _n;
        }

        //Create next generation of creatures
        internal void MakeLife()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
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
            Random _rnd = new();
            foreach (var row in g.Cols)
                foreach (var cell in row.Cells)
                {
                    if (_rnd.Next(3) < 1) cell.IsAlive = false;
                    else cell.IsAlive = true;
                }
        }
    }
}