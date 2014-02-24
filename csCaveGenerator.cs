using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ComponentModel;


namespace maze
{

    /// <summary>
    /// csCaveGenerator - generate a cave system and connect the caves together.
    /// 
    /// For more info on it's use see http://www.evilscience.co.uk/?p=624
    /// </summary>
    class csCaveGenerator
    {

        private Random rnd;

        #region properties

        [Category("Cave Generation"), Description("The number of closed neighbours a cell must have in order to invert it's state")]
        public int Neighbours { get; set; }
        [Category("Cave Generation"), Description("The probability of closing a visited cell")]
        public int CloseCellProb { get; set; } //55 tends to produce 1 cave, 40 few and small caves
        [Category("Cave Generation"), Description("The number of times to visit cells"), DisplayName("Cells to visit")]
        public int Iterations { get; set; }
        [Category("Cave Generation"), Description("The size of the map"), DisplayName("Map Size")]
        public Size MapSize { get; set; }
        


        [Category("Cave Cleaning"), Description("Remove rooms smaller than this value"), DisplayName("Lower Limit")]
        public int LowerLimit { get; set; }
        [Category("Cave Cleaning"), Description("Remove rooms larger than this value"), DisplayName("Upper Limit")]
        public int UpperLimit { get; set; }
        [Category("Cave Cleaning"), DisplayName("Smoothing"), Description("Removes single cells from cave edges: a cell with this number of empty neighbours is removed")]
        public int EmptyNeighbours { get; set; }
        [Category("Cave Cleaning"), DisplayName("Filling"), Description("Fills in holes within caves: an open cell with this number closed neighbours is filled")]
        public int EmptyCellNeighbours { get; set; }


        //corridor properties
        [Category("Corridor"), Description("Minimum corridor length"), DisplayName("Min length")]
        public int Corridor_Min { get; set; }
        [Category("Corridor"), Description("Maximum corridor length"), DisplayName("Max length")]
        public int Corridor_Max { get; set; }
        [Category("Corridor"), Description("Maximum turns"), DisplayName("Max Turns")]
        public int Corridor_MaxTurns { get; set; }
        [Category("Corridor"), Description("The distance a corridor has to be away from a closed cell for it to be built"), DisplayName("Corridor Spacing")]
        public int CorridorSpace { get; set; }
        [Category("Corridor"), Description("When this value is exceeded, stop attempting to connect caves. Prevents the algorithm from getting stuck.")]
        public int BreakOut { get; set; }

        [Category("Generated Map"), Description("Number of caves generated"), DisplayName("Caves")]
        public int CaveNumber { get { return Caves == null ? 0 : Caves.Count; } }


        #endregion

        #region map structures

        /// <summary>
        /// Caves within the map are stored here
        /// </summary>
        private List<List<Point>> Caves;

        /// <summary>
        /// Corridors within the map stored here
        /// </summary>
        private List<Point> Corridors;

        /// <summary>
        /// Contains the map
        /// </summary>
        public int[,] Map;

        #endregion

        #region lookups

        /// <summary>
        /// Generic list of points which contain 4 directions
        /// </summary>
        List<Point> Directions = new List<Point>()
        {
            new Point (0,-1)    //north
            , new Point(0,1)    //south
            , new Point (1,0)   //east
            , new Point (-1,0)  //west
        };

        List<Point> Directions1 = new List<Point>()
        {
            new Point (0,-1)    //north
            , new Point(0,1)    //south
            , new Point (1,0)   //east
            , new Point (-1,0)  //west
            , new Point (1,-1)  //northeast
            , new Point(-1,-1)  //northwest
            , new Point (-1,1)  //southwest
            , new Point (1,1)   //southeast
            , new Point(0,0)    //centre
        };

        #endregion
        #region misc

        /// <summary>
        /// Constructor
        /// </summary>
        public csCaveGenerator()
        {
            rnd = new Random(12345);
            Neighbours = 4;
            Iterations = 50000;
            CloseCellProb = 45;

            LowerLimit = 16;
            UpperLimit = 500;

            MapSize = new Size(100, 100);

            EmptyNeighbours = 3;
            EmptyCellNeighbours = 4;

            CorridorSpace = 2;
            Corridor_MaxTurns = 10;
            Corridor_Min = 2;
            Corridor_Max = 5;

            BreakOut = 100000;
        }


        public int Build()
        {
            BuildCaves();
            GetCaves();
            return Caves.Count();
        }

        /// <summary>
        /// Generate a bitmap from the contents of the map array
        /// </summary>
        /// <returns></returns>
        public Bitmap GetMapImage()
        {
            //adjust to change the pixel size on the image
            Size blocksize = new Size(5, 5);

            Bitmap bmp = new Bitmap(MapSize.Width * blocksize.Width, MapSize.Height * blocksize.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                using (SolidBrush sbBlack = new SolidBrush(Color.Black))
                {
                    for (int x = 0; x < MapSize.Width; x++)
                        for (int y = 0; y < MapSize.Height; y++)
                            if (Map[x, y] == 1)
                                g.FillRectangle(sbBlack, new Rectangle(x * blocksize.Width, y * blocksize.Height, blocksize.Width, blocksize.Height));

                }
            }

            return bmp;
        }

#endregion


        #region cave related

        #region make caves

        /// <summary>
        /// Calling this method will build caves, smooth them off and fill in any holes
        /// </summary>
        private void BuildCaves()
        {

            Map = new int[MapSize.Width, MapSize.Height];


            //go through each map cell and randomly determine whether to close it
            //the +5 offsets are to leave an empty border round the edge of the map
            for (int x = 0; x < MapSize.Width; x++)
                for (int y = 0; y < MapSize.Height; y++)
                    if (rnd.Next(0, 100) < CloseCellProb)
                        Map[x, y] = 1;

            Point cell;

            //Pick cells at random
            for (int x = 0; x <= Iterations; x++)
            {
                cell = new Point(rnd.Next(0, MapSize.Width), rnd.Next(0, MapSize.Height));

                //if the randomly selected cell has more closed neighbours than the property Neighbours
                //set it closed, else open it
                if (Neighbours_Get1(cell).Where(n => Point_Get(n) == 1).Count() > Neighbours)
                    Point_Set(cell, 1);
                else
                    Point_Set(cell, 0);
            }



            //
            //  Smooth of the rough cave edges and any single blocks by making several 
            //  passes on the map and removing any cells with 3 or more empty neighbours
            //
            for (int ctr = 0; ctr < 5; ctr++)
            {
                //examine each cell individually
                for (int x = 0; x < MapSize.Width; x++)
                    for (int y = 0; y < MapSize.Height; y++)
                    {
                        cell = new Point(x, y);

                        if (
                                Point_Get(cell) > 0
                                && Neighbours_Get(cell).Where(n => Point_Get(n) == 0).Count() >= EmptyNeighbours
                            )
                            Point_Set(cell, 0);
                    }
            }

            //
            //  fill in any empty cells that have 4 full neighbours
            //  to get rid of any holes in an cave
            //
            for (int x = 0; x < MapSize.Width; x++)
                for (int y = 0; y < MapSize.Height; y++)
                {
                    cell = new Point(x, y);

                    if (
                            Point_Get(cell) == 0
                            && Neighbours_Get(cell).Where(n => Point_Get(n) == 1).Count() >= EmptyCellNeighbours
                        )
                        Point_Set(cell, 1);
                }
        }

        #endregion

        #region locate caves
        /// <summary>
        /// Locate the edge of the specified cave
        /// </summary>
        /// <param name="pCaveNumber">Cave to examine</param>
        /// <param name="pCavePoint">Point on the edge of the cave</param>
        /// <param name="pDirection">Direction to start formting the tunnel</param>
        /// <returns>Boolean indicating if an edge was found</returns>
        private void Cave_GetEdge(List<Point> pCave, ref Point pCavePoint, ref Point pDirection)
        {
            do
            {

                //random point in cave
                pCavePoint = pCave.ToList()[rnd.Next(0, pCave.Count())];

                pDirection = Direction_Get(pDirection);

                do
                {
                    pCavePoint.Offset(pDirection);

                    if (!Point_Check(pCavePoint))
                        break;
                    else if (Point_Get(pCavePoint) == 0)
                        return;

                } while (true);



            } while (true);
        }

        /// <summary>
        /// Locate all the caves within the map and place each one into the generic list Caves
        /// </summary>
        private void GetCaves()
        {
            Caves = new List<List<Point>>();

            List<Point> Cave;
            Point cell;

            //examine each cell in the map...
            for (int x = 0; x < MapSize.Width; x++)
                for (int y = 0; y < MapSize.Height; y++)
                {
                    cell = new Point(x, y);
                    //if the cell is closed, and that cell doesn't occur in the list of caves..
                    if (Point_Get(cell) > 0 && Caves.Count(s => s.Contains(cell)) == 0)
                    {
                        Cave = new List<Point>();

                        //launch the recursive
                        LocateCave(cell, Cave);

                        //check that cave falls with the specified property range size...
                        if (Cave.Count() <= LowerLimit | Cave.Count() > UpperLimit)
                        {
                            //it does, so bin it
                            foreach (Point p in Cave)
                                Point_Set(p, 0);
                        }
                        else
                            Caves.Add(Cave);
                    }
                }

        }

        /// <summary>
        /// Recursive method to locate the cells comprising a cave, 
        /// based on flood fill algorithm
        /// </summary>
        /// <param name="cell">Cell being examined</param>
        /// <param name="current">List containing all the cells in the cave</param>
        private void LocateCave(Point pCell, List<Point> pCave)
        {
            foreach (Point p in Neighbours_Get(pCell).Where(n => Point_Get(n) > 0))
            {
                if (!pCave.Contains(p))
                {
                    pCave.Add(p);
                    LocateCave(p, pCave);
                }
            }
        }

        #endregion

        #region connect caves

        /// <summary>
        /// Attempt to connect the caves together
        /// </summary>
        public bool ConnectCaves()
        {


            if (Caves.Count() == 0)
                return false;



            List<Point> currentcave;
            List<List<Point>> ConnectedCaves = new List<List<Point>>();
            Point cor_point = new Point();
            Point cor_direction = new Point();
            List<Point> potentialcorridor = new List<Point>();
            int breakoutctr = 0;

            Corridors = new List<Point>(); //corridors built stored here

            //get started by randomly selecting a cave..
            currentcave = Caves[rnd.Next(0, Caves.Count())];
            ConnectedCaves.Add(currentcave);
            Caves.Remove(currentcave);



            //starting builder
            do
            {

                //no corridors are present, sp build off a cave
                if (Corridors.Count() == 0)
                {
                    currentcave = ConnectedCaves[rnd.Next(0, ConnectedCaves.Count())];
                    Cave_GetEdge(currentcave, ref cor_point, ref cor_direction);
                }
                else
                    //corridors are presnt, so randomly chose whether a get a start
                    //point from a corridor or cave
                    if (rnd.Next(0, 100) > 50)
                    {
                        currentcave = ConnectedCaves[rnd.Next(0, ConnectedCaves.Count())];
                        Cave_GetEdge(currentcave, ref cor_point, ref cor_direction);
                    }
                    else
                    {
                        currentcave = null;
                        Corridor_GetEdge(ref cor_point, ref cor_direction);
                    }



                //using the points we've determined above attempt to build a corridor off it
                potentialcorridor = Corridor_Attempt(cor_point
                                                , cor_direction
                                                , true);


                //if not null, a solid object has been hit
                if (potentialcorridor != null)
                {

                    //examine all the caves
                    for (int ctr = 0; ctr < Caves.Count(); ctr++)
                    {

                        //check if the last point in the corridor list is in a cave
                        if (Caves[ctr].Contains(potentialcorridor.Last()))
                        {
                            if (
                                    currentcave == null //we've built of a corridor
                                    | currentcave != Caves[ctr] //or built of a room
                                )
                            {
                                //the last corridor point intrudes on the room, so remove it
                                potentialcorridor.Remove(potentialcorridor.Last());
                                //add the corridor to the corridor collection
                                Corridors.AddRange(potentialcorridor);
                                //write it to the map
                                foreach (Point p in potentialcorridor)
                                    Point_Set(p, 1);


                                //the room reached is added to the connected list...
                                ConnectedCaves.Add(Caves[ctr]);
                                //...and removed from the Caves list
                                Caves.RemoveAt(ctr);

                                break;

                            }
                        }
                    }
                }

                //breakout
                if (breakoutctr++ > BreakOut)
                    return false;

            } while (Caves.Count() > 0);

            Caves.AddRange(ConnectedCaves);
            ConnectedCaves.Clear();
            return true;
        }

        #endregion

        #endregion

        #region corridor related

        /// <summary>
        /// Randomly get a point on an existing corridor
        /// </summary>
        /// <param name="Location">Out: location of point</param>
        /// <returns>Bool indicating success</returns>
        private void Corridor_GetEdge(ref Point pLocation, ref Point pDirection)
        {
            List<Point> validdirections = new List<Point>();

            do
            {
                //the modifiers below prevent the first of last point being chosen
                pLocation = Corridors[rnd.Next(1, Corridors.Count - 1)];

                //attempt to locate all the empy map points around the location
                //using the directions to offset the randomly chosen point
                foreach (Point p in Directions)
                    if (Point_Check(new Point(pLocation.X + p.X, pLocation.Y + p.Y)))
                        if (Point_Get(new Point(pLocation.X + p.X, pLocation.Y + p.Y)) == 0)
                            validdirections.Add(p);


            } while (validdirections.Count == 0);

            pDirection = validdirections[rnd.Next(0, validdirections.Count)];
            pLocation.Offset(pDirection);

        }

        /// <summary>
        /// Attempt to build a corridor
        /// </summary>
        /// <param name="pStart"></param>
        /// <param name="pDirection"></param>
        /// <param name="pPreventBackTracking"></param>
        /// <returns></returns>
        private List<Point> Corridor_Attempt(Point pStart, Point pDirection, bool pPreventBackTracking)
        {

            List<Point> lPotentialCorridor = new List<Point>();
            lPotentialCorridor.Add(pStart);

            int corridorlength;
            Point startdirection = new Point(pDirection.X, pDirection.Y);

            int pTurns = Corridor_MaxTurns;

            while (pTurns >= 0)
            {
                pTurns--;

                corridorlength = rnd.Next(Corridor_Min, Corridor_Max);
                //build corridor
                while (corridorlength > 0)
                {
                    corridorlength--;

                    //make a point and offset it
                    pStart.Offset(pDirection);

                    if (Point_Check(pStart) && Point_Get(pStart) == 1)
                    {
                        lPotentialCorridor.Add(pStart);
                        return lPotentialCorridor;
                    }

                    if (!Point_Check(pStart))
                        return null;
                    else if (!Corridor_PointTest(pStart, pDirection))
                        return null;

                    lPotentialCorridor.Add(pStart);

                }

                if (pTurns > 1)
                    if (!pPreventBackTracking)
                        pDirection = Direction_Get(pDirection);
                    else
                        pDirection = Direction_Get(pDirection, startdirection);
            }

            return null;
        }

        private bool Corridor_PointTest(Point pPoint, Point pDirection)
        {

            //using the property corridor space, check that number of cells on
            //either side of the point are empty
            foreach (int r in Enumerable.Range(-CorridorSpace, 2 * CorridorSpace + 1).ToList())
            {
                if (pDirection.X == 0)//north or south
                {
                    if (Point_Check(new Point(pPoint.X + r, pPoint.Y)))
                        if (Point_Get(new Point(pPoint.X + r, pPoint.Y)) != 0)
                            return false;
                }
                else if (pDirection.Y == 0)//east west
                {
                    if (Point_Check(new Point(pPoint.X, pPoint.Y + r)))
                        if (Point_Get(new Point(pPoint.X, pPoint.Y + r)) != 0)
                            return false;
                }

            }

            return true;
        }

        #endregion

        #region direction related

        /// <summary>
        /// Return a list of the valid neighbouring cells of the provided point
        /// using only north, south, east and west
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private List<Point> Neighbours_Get(Point p)
        {
            return Directions.Select(d => new Point(p.X + d.X, p.Y + d.Y))
                    .Where(d => Point_Check(d)).ToList();
        }

        /// <summary>
        /// Return a list of the valid neighbouring cells of the provided point
        /// using north, south, east, ne,nw,se,sw
        private List<Point> Neighbours_Get1(Point p)
        {
            return Directions1.Select(d => new Point(p.X + d.X, p.Y + d.Y))
                    .Where(d => Point_Check(d)).ToList();
        }

        /// <summary>
        /// Get a random direction, provided it isn't equal to the opposite one provided
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Point Direction_Get(Point p)
        {
            Point newdir;
            do
            {
                newdir = Directions[rnd.Next(0, Directions.Count())];

            } while (newdir.X != -p.X & newdir.Y != -p.Y);

            return newdir;
        }

        /// <summary>
        /// Get a random direction, excluding the provided directions and the opposite of 
        /// the provided direction to prevent a corridor going back on it's self.
        /// 
        /// The parameter pDirExclude is the first direction chosen for a corridor, and
        /// to prevent it from being used will prevent a corridor from going back on 
        /// it'self
        /// </summary>
        /// <param name="dir">Current direction</param>
        /// <param name="pDirectionList">Direction to exclude</param>
        /// <param name="pDirExclude">Direction to exclude</param>
        /// <returns></returns>
        private Point Direction_Get(Point pDir, Point pDirExclude)
        {
            Point NewDir;
            do
            {
                NewDir = Directions[rnd.Next(0, Directions.Count())];
            } while (
                        Direction_Reverse(NewDir) == pDir
                         | Direction_Reverse(NewDir) == pDirExclude
                    );


            return NewDir;
        }

        private Point Direction_Reverse(Point pDir)
        {
            return new Point(-pDir.X, -pDir.Y);
        }

        #endregion

        #region cell related

        /// <summary>
        /// Check if the provided point is valid
        /// </summary>
        /// <param name="p">Point to check</param>
        /// <returns></returns>
        private bool Point_Check(Point p)
        {
            return p.X >= 0 & p.X < MapSize.Width & p.Y >= 0 & p.Y < MapSize.Height;
        }

        /// <summary>
        /// Set the map cell to the specified value
        /// </summary>
        /// <param name="p"></param>
        /// <param name="val"></param>
        private void Point_Set(Point p, int val)
        {
            Map[p.X, p.Y] = val;
        }

        /// <summary>
        /// Get the value of the provided point
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private int Point_Get(Point p)
        {
            return Map[p.X, p.Y];
        }

        #endregion



    }
}


