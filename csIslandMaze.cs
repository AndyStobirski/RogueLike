/// <summary>
    /// IslandMaze class - generates simple islands and mazes.
    /// 
    /// For more info on it's use see http://www.evilscience.co.uk/?p=53
    /// </summary>
    class csIslandMaze
    {

        private Random r = new Random(System.DateTime.Now.Millisecond);

        public int Neighbours {get ; set;}
        public int Iterations {get ; set;}
        public int MapX {get ; set;}
        public int MapY {get ; set;}
        public int CloseCellProb {get ; set;}
        public bool ProbExceeded {get ; set;}

        public int [,] Map;


        public csIslandMaze()
        {
            Neighbours = 4;
            Iterations = 50000;
            ProbExceeded = true;
            MapX = 99;
            MapY = 99;
            CloseCellProb = 45;
            
        }



        /// <summary>
        /// Build a Map
        /// </summary>
        /// <param name="closeCellProb">Probability of closing a cell</param>
        /// <param name="neighbours">The number of cells required to trigger</param>
        /// <param name="iterations">Number of iterations</param>
        /// <param name="Map">Map array to opearate on</param>
        /// <param name="reset">Clear the Map before operation</param>
        /// <param name="probExceeded">probability exceeded</param>
        /// <param name="invert"></param>
        /// <returns></returns>
        public void  go()
        {

            Map = new int[MapX, MapY];

            
            //go through each cell and use the specified probability to determine if it's open
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    if (r.Next(0, 100) < CloseCellProb)
                    {
                        Map[x, y] = 1;
                    }
                }
            }

            //pick some cells at random
            for (int x = 0; x <= Iterations; x++)
            {
                int rX = r.Next(0, Map.GetLength(0));
                int rY = r.Next(0, Map.GetLength(1));

                if (ProbExceeded == true)
                {
                    if (examineNeighbours(rX, rY) > Neighbours)
                    {
                        Map[rX, rY] = 1;
                    }
                    else
                    {
                        Map[rX, rY] = 0;
                    }
                }
                else
                {
                    if (examineNeighbours(rX, rY) > Neighbours)
                    {
                        Map[rX, rY] = 0;
                    }
                    else
                    {
                        Map[rX, rY] = 1;
                    }
                }


            }
        }

        /// <summary>
        /// Count all the closed cells around the specified cell and return that number
        /// </summary>
        /// <param name="xVal">cell X value</param>
        /// <param name="yVal">cell Y value</param>
        /// <returns>Number of surrounding cells</returns>
        private int examineNeighbours(int xVal, int yVal)
        {
            int count = 0;

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (checkCell(xVal + x, yVal + y) == true)
                        count += 1;
                }
            }

            return count;
        }

        /// <summary>
        /// Check the examined cell is legal and closed
        /// </summary>
        /// <param name="x">cell X value</param>
        /// <param name="y">cell Y value</param>
        /// <returns>Cell state - true if closed, false if open or illegal</returns>
        private Boolean checkCell(int x, int y)
        {
            if (x >= 0 & x < Map.GetLength(0) &
                y >= 0 & y < Map.GetLength(1))
            {
                if (Map[x, y] > 0)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
    }
