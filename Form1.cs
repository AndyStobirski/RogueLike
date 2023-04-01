using map;
using maze;
using RogueLikeMapBuilder;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace RogueLike
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }




        SolidBrush sbBlue = new SolidBrush(Color.Blue);
        SolidBrush sbLightGray = new SolidBrush(Color.LightGray);
        SolidBrush sbGray = new SolidBrush(Color.FromArgb(32, Color.Gray));


        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.KeyPreview = true;

            FOVInit();
            InitMapBuilder();
            InitCave();
            InitIslandGenerator();
        }

        #region csCaveGenerator.cs

        csCaveGenerator caveGenerator;

        private void InitCave()
        {
            caveGenerator = new csCaveGenerator();
            propertyGrid2.SelectedObject = caveGenerator;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            caveGenerator.Build();
            pictureBox2.Invalidate();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            caveGenerator.Build();

            pictureBox2.Invalidate();
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            if (caveGenerator.Map == null) return;


            for (int x = 0; x < caveGenerator.Map.GetLength(0); x++)
            {
                for (int y = 0; y < caveGenerator.Map.GetLength(1); y++)
                {
                    if (caveGenerator.Map[x, y] != 0)
                    {
                        e.Graphics.FillRectangle(sbBlue, new Rectangle(x * mapBSize.Width, y * mapBSize.Height, mapBSize.Width, mapBSize.Height));
                    }
                }
            }
        }


        #endregion

        #region csMapBuilder.cs

        csMapbuilder mapb;
        Size mapBSize = new Size(4, 4);
        private void InitMapBuilder()
        {
            mapb = new csMapbuilder(100, 100);
            propertyGrid1.SelectedObject = mapb;

        }
        private void pbMapBuilder_Paint(object sender, PaintEventArgs e)
        {
            for (int x = 0; x < mapb.map.GetLength(0); x++)
            {
                for (int y = 0; y < mapb.map.GetLength(1); y++)
                {
                    if (mapb.map[x, y] != 0)
                    {
                        e.Graphics.FillRectangle(sbBlue, new Rectangle(x * mapBSize.Width, y * mapBSize.Height, mapBSize.Width, mapBSize.Height));
                    }
                }
            }
        }

        private void llResetMap_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            InitMapBuilder();

            pbMapBuilder.Invalidate();
        }

        private void llBuildMap_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var built = mapb.Build_OneStartRoom();
            pbMapBuilder.Invalidate();
        }

        #endregion

        #region FOVRecurse.cs

        Size cellSize;
        bool mouseLDown = false;
        bool mouseRDown = false;

        int[,] map;

        FOVRecurse fov;

        private void FOVInit()
        {
            map = new int[50, 50];
            cellSize = new(10, 10);

            for (int x = 0; x < 7; x++)
            {
                map[22 + x, 24] = 1;
                map[22 + x, 26] = 1;

                map[16 + x, 27] = 1;
                map[28 + x, 27] = 1;

                map[16 + x, 23] = 1;
                map[28 + x, 23] = 1;
            }


            fov = new FOVRecurse(map);
            fov.Player = new Point(25, 25);
            fov.VisualRange = 8;

            pictureBox1.Width = map.GetLength(0) * cellSize.Width;
            pictureBox1.Height = map.GetLength(1) * cellSize.Height;

            fov.playerMoved += Fov_playerMoved;

            fov.movePlayer(0, 0);
        }

        private void Fov_playerMoved()
        {
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) mouseLDown = true;
            if (e.Button == MouseButtons.Right) mouseRDown = true;
        }


        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) mouseLDown = false;
            if (e.Button == MouseButtons.Right) mouseRDown = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            int x = e.X / cellSize.Width;
            int y = e.Y / cellSize.Height;

            if (x < 0 || y < 0) return;
            if (x >= map.GetLength(0)) return;
            if (y >= map.GetLength(1)) return;

            label1.Text = string.Format("Mouse: {0},{1}", x, y);

            if (!mouseLDown & !mouseRDown)
                return;

            int val = 0;
            if (mouseLDown)
                val = 1;
            else if (mouseRDown)
                val = 0;

            map[x, y] = val;

            pictureBox1.Invalidate();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Debug.WriteLine(e.KeyValue + ": " + e.KeyValue.ToString());

            switch (e.KeyValue)
            {
                case 81://Q
                    fov.movePlayer(-1, -1);
                    break;

                case 87://W
                    fov.movePlayer(0, -1);
                    break;

                case 69://Q
                    fov.movePlayer(1, -1);
                    break;

                case 65://Q
                    fov.movePlayer(-1, 0);
                    break;

                case 68://D
                    fov.movePlayer(1, 0);
                    break;

                case 90://Z
                    fov.movePlayer(-1, 1);
                    break;

                case 88://X
                    fov.movePlayer(0, 1);
                    break;

                case 67://V
                    fov.movePlayer(1, 1);
                    break;
            }

            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(sbBlue, new Rectangle(fov.Player.X * cellSize.Width, fov.Player.Y * cellSize.Height, cellSize.Width, cellSize.Height));

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y] != 0)
                    {
                        e.Graphics.FillRectangle(sbLightGray, new Rectangle(x * cellSize.Width, y * cellSize.Height, cellSize.Width, cellSize.Height));
                    }
                }
            }


            foreach (var p in fov.VisiblePoints)
            {
                e.Graphics.FillRectangle(sbGray, new Rectangle(p.X * cellSize.Width, p.Y * cellSize.Height, cellSize.Width, cellSize.Height));
            }


        }

        private void lblReset_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int[,] map = new int[100, 100];

        }

        #endregion

        #region Island Generator

        csIslandMaze islandMaze;

        public void InitIslandGenerator()
        {
            islandMaze = new csIslandMaze();
        }

        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            if (islandMaze.Map == null) return;


            for (int x = 0; x < islandMaze.Map.GetLength(0); x++)
            {
                for (int y = 0; y < islandMaze.Map.GetLength(1); y++)
                {
                    if (islandMaze.Map[x, y] != 0)
                    {
                        e.Graphics.FillRectangle(sbBlue, new Rectangle(x * mapBSize.Width, y * mapBSize.Height, mapBSize.Width, mapBSize.Height));
                    }
                }
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //reset
            InitIslandGenerator();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //build
            islandMaze.go();
            pictureBox3.Invalidate();
        }

        #endregion
    }
}