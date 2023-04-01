using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace RogueLikeMapBuilder
{
    /// <summary>
    /// This class demonstrates a simple map builder for a roguelike game. For a detailed
    /// look at using it, go here http://www.evilscience.co.uk/?p=553
    /// </summary>
    internal class csMapbuilder
    {
        public int[,] map;

        /// <summary>
        /// Built rooms stored here
        /// </summary>
        private List<Rectangle> rctBuiltRooms;

        /// <summary>
        /// Built corridors stored here
        /// </summary>
        private List<Point> lBuilltCorridors;

        /// <summary>
        /// Corridor to be built stored here
        /// </summary>
        private List<Point> lPotentialCorridor;

        /// <summary>
        /// Room to be built stored here
        /// </summary>
        private Rectangle rctCurrentRoom;


        #region builder public properties

        //room properties
        [Category("Room"), Description("Minimum Size"), DisplayName("Minimum Size")]
        public Size Room_Min { get; set; }
        [Category("Room"), Description("Max Size"), DisplayName("Maximum Size")]
        public Size Room_Max { get; set; }
        [Category("Room"), Description("Total number"), DisplayName("Rooms to build")]
        public int MaxRooms { get; set; }
        [Category("Room"), Description("Minimum distance between rooms"), DisplayName("Distance from other rooms")]
        public int RoomDistance { get; set; }
        [Category("Room"), Description("Minimum distance of room from existing corridors"), DisplayName("Corridor Distance")]
        public int CorridorDistance { get; set; }

        //corridor properties
        [Category("Corridor"), Description("Minimum corridor length"), DisplayName("Minimum length")]
        public int Corridor_Min { get; set; }
        [Category("Corridor"), Description("Maximum corridor length"), DisplayName("Maximum length")]
        public int Corridor_Max { get; set; }
        [Category("Corridor"), Description("Maximum turns"), DisplayName("Maximum turns")]
        public int Corridor_MaxTurns { get; set; }
        [Category("Corridor"), Description("The distance a corridor has to be away from a closed cell for it to be built"), DisplayName("Corridor Spacing")]
        public int CorridorSpace { get; set; }


        [Category("Probability"), Description("Probability of building a corridor from a room or corridor. Greater than value = room"), DisplayName("Select room")]
        public int BuildProb { get; set; }

        [Category("Map"), DisplayName("Map Size")]
        public Size Map_Size { get; set; }
        [Category("Map"), DisplayName("Break Out"), Description("")]
        public int BreakOut { get; set; }



        #endregion

        /// <summary>
        /// describes the outcome of the corridor building operation
        /// </summary>
        private enum CorridorItemHit
        {

            invalid //invalid point generated
            ,
            self  //corridor hit self
                ,
            existingcorridor //hit a built corridor
                ,
            originroom //corridor hit origin room 
                ,
            existingroom //corridor hit existing room
                ,
            completed //corridor built without problem    
                ,
            tooclose
                , OK //point OK
        }

        Point[] directions_straight = new Point[]{
                                            new Point(0, -1) //n
                                            , new Point(0, 1)//s
                                            , new Point(1, 0)//w
                                            , new Point(-1, 0)//e
                                        };

        private int filledcell = 1;
        private int emptycell = 0;

        Random rnd = new();

        public csMapbuilder(int x, int y)
        {
            Map_Size = new Size(x, y);
            map = new int[Map_Size.Width, Map_Size.Height];
            Corridor_MaxTurns = 5;
            Room_Min = new Size(3, 3);
            Room_Max = new Size(15, 15);
            Corridor_Min = 3;
            Corridor_Max = 15;
            MaxRooms = 15;
            Map_Size = new Size(150, 150);

            RoomDistance = 5;
            CorridorDistance = 2;
            CorridorSpace = 2;

            BuildProb = 50;
            BreakOut = 250;
        }

        /// <summary>
        /// Initialise everything
        /// </summary>
        private void Clear()
        {
            lPotentialCorridor = new List<Point>();
            rctBuiltRooms = new List<Rectangle>();
            lBuilltCorridors = new List<Point>();

            map = new int[Map_Size.Width, Map_Size.Height];
            for (int x = 0; x < Map_Size.Width; x++)
                for (int y = 0; y < Map_Size.Width; y++)
                    map[x, y] = filledcell;
        }

        #region build methods()

        /// <summary>
        /// Randomly choose a room and attempt to build a corridor terminated by
        /// a room off it, and repeat until MaxRooms has been reached. The map
        /// is started of by placing a room in approximately the centre of the map
        /// using the method PlaceStartRoom()
        /// </summary>
        /// <returns>Bool indicating if the map was built, i.e. the property BreakOut was not
        /// exceed</returns>
        public bool Build_OneStartRoom()
        {
            int loopctr = 0;

            CorridorItemHit CorBuildOutcome;
            Point Location = new Point();
            Point Direction = new Point();

            Clear();

            PlaceStartRoom();

            //attempt to build the required number of rooms
            while (rctBuiltRooms.Count() < MaxRooms)
            {

                if (loopctr++ > BreakOut)//bail out if this value is exceeded
                    return false;

                if (Corridor_GetStart(out Location, out Direction))
                {

                    CorBuildOutcome = CorridorMake_Straight(ref Location, ref Direction, rnd.Next(1, Corridor_MaxTurns)
                        , rnd.Next(0, 100) > 50 ? true : false);

                    switch (CorBuildOutcome)
                    {
                        case CorridorItemHit.existingroom:
                        case CorridorItemHit.existingcorridor:
                        case CorridorItemHit.self:
                            Corridor_Build();
                            break;

                        case CorridorItemHit.completed:
                            if (Room_AttemptBuildOnCorridor(Direction))
                            {
                                Corridor_Build();
                                Room_Build();
                            }
                            break;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Randomly choose a room and attempt to build a corridor terminated by
        /// a room off it, and repeat until MaxRooms has been reached. The map
        /// is started of by placing two rooms on opposite sides of the map and joins
        /// them with a long corridor, using the method PlaceStartRooms()
        /// </summary>
        /// <returns>Bool indicating if the map was built, i.e. the property BreakOut was not
        /// exceed</returns>
        public bool Build_ConnectedStartRooms()
        {
            int loopctr = 0;

            CorridorItemHit CorBuildOutcome;
            Point Location = new Point();
            Point Direction = new Point();

            Clear();

            PlaceStartRooms();

            //attempt to build the required number of rooms
            while (rctBuiltRooms.Count() < MaxRooms)
            {

                if (loopctr++ > BreakOut)//bail out if this value is exceeded
                    return false;

                if (Corridor_GetStart(out Location, out Direction))
                {

                    CorBuildOutcome = CorridorMake_Straight(ref Location, ref Direction, rnd.Next(1, Corridor_MaxTurns)
                        , rnd.Next(0, 100) > 50 ? true : false);

                    switch (CorBuildOutcome)
                    {
                        case CorridorItemHit.existingroom:
                        case CorridorItemHit.existingcorridor:
                        case CorridorItemHit.self:
                            Corridor_Build();
                            break;

                        case CorridorItemHit.completed:
                            if (Room_AttemptBuildOnCorridor(Direction))
                            {
                                Corridor_Build();
                                Room_Build();
                            }
                            break;
                    }
                }
            }

            return true;

        }

        #endregion


        #region room utilities

        /// <summary>
        /// Place a random sized room in the middle of the map
        /// </summary>
        private void PlaceStartRoom()
        {
            rctCurrentRoom = new Rectangle()
            {
                Width = rnd.Next(Room_Min.Width, Room_Max.Width)
                ,
                Height = rnd.Next(Room_Min.Height, Room_Max.Height)
            };
            rctCurrentRoom.X = Map_Size.Width / 2;
            rctCurrentRoom.Y = Map_Size.Height / 2;
            Room_Build();
        }


        /// <summary>
        /// Place a start room anywhere on the map
        /// </summary>
        private void PlaceStartRooms()
        {

            Point startdirection;
            bool connection = false;
            Point Location = new Point();
            Point Direction = new Point();
            CorridorItemHit CorBuildOutcome;

            while (!connection)
            {

                Clear();
                startdirection = Direction_Get(new Point());

                //place a room on the top and bottom
                if (startdirection.X == 0)
                {

                    //room at the top of the map
                    rctCurrentRoom = new Rectangle()
                    {
                        Width = rnd.Next(Room_Min.Width, Room_Max.Width)
                                ,
                        Height = rnd.Next(Room_Min.Height, Room_Max.Height)
                    };
                    rctCurrentRoom.X = rnd.Next(0, Map_Size.Width - rctCurrentRoom.Width);
                    rctCurrentRoom.Y = 1;
                    Room_Build();

                    //at the bottom of the map
                    rctCurrentRoom = new Rectangle();
                    rctCurrentRoom.Width = rnd.Next(Room_Min.Width, Room_Max.Width);
                    rctCurrentRoom.Height = rnd.Next(Room_Min.Height, Room_Max.Height);
                    rctCurrentRoom.X = rnd.Next(0, Map_Size.Width - rctCurrentRoom.Width);
                    rctCurrentRoom.Y = Map_Size.Height - rctCurrentRoom.Height - 1;
                    Room_Build();


                }
                else//place a room on the east and west side
                {
                    //west side of room
                    rctCurrentRoom = new Rectangle();
                    rctCurrentRoom.Width = rnd.Next(Room_Min.Width, Room_Max.Width);
                    rctCurrentRoom.Height = rnd.Next(Room_Min.Height, Room_Max.Height);
                    rctCurrentRoom.Y = rnd.Next(0, Map_Size.Height - rctCurrentRoom.Height);
                    rctCurrentRoom.X = 1;
                    Room_Build();

                    rctCurrentRoom = new Rectangle();
                    rctCurrentRoom.Width = rnd.Next(Room_Min.Width, Room_Max.Width);
                    rctCurrentRoom.Height = rnd.Next(Room_Min.Height, Room_Max.Height);
                    rctCurrentRoom.Y = rnd.Next(0, Map_Size.Height - rctCurrentRoom.Height);
                    rctCurrentRoom.X = Map_Size.Width - rctCurrentRoom.Width - 2;
                    Room_Build();

                }



                if (Corridor_GetStart(out Location, out Direction))
                {



                    CorBuildOutcome = CorridorMake_Straight(ref Location, ref Direction, 100, true);

                    switch (CorBuildOutcome)
                    {
                        case CorridorItemHit.existingroom:
                            Corridor_Build();
                            connection = true;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Make a room off the last point of Corridor, using
        /// CorridorDirection as an indicator of how to offset the room.
        /// The potential room is stored in Room.
        /// </summary>
        private bool Room_AttemptBuildOnCorridor(Point pDirection)
        {
            rctCurrentRoom = new Rectangle()
            {
                Width = rnd.Next(Room_Min.Width, Room_Max.Width)
                    ,
                Height = rnd.Next(Room_Min.Height, Room_Max.Height)
            };

            //startbuilding room from this point
            Point lc = lPotentialCorridor.Last();

            if (pDirection.X == 0) //north/south direction
            {
                rctCurrentRoom.X = rnd.Next(lc.X - rctCurrentRoom.Width + 1, lc.X);

                if (pDirection.Y == 1)
                    rctCurrentRoom.Y = lc.Y + 1;//south
                else
                    rctCurrentRoom.Y = lc.Y - rctCurrentRoom.Height - 1;//north


            }
            else if (pDirection.Y == 0)//east / west direction
            {
                rctCurrentRoom.Y = rnd.Next(lc.Y - rctCurrentRoom.Height + 1, lc.Y);

                if (pDirection.X == -1)//west
                    rctCurrentRoom.X = lc.X - rctCurrentRoom.Width;
                else
                    rctCurrentRoom.X = lc.X + 1;//east
            }

            return Room_Verify();
        }


        /// <summary>
        /// Randomly get a point on the edge of a randomly selected room
        /// </summary>
        /// <param name="Location">Out: Location of point on room edge</param>
        /// <param name="Location">Out: Direction of point</param>
        /// <returns>If Location is legal</returns>
        private void Room_GetEdge(out Point pLocation, out Point pDirection)
        {

            rctCurrentRoom = rctBuiltRooms[rnd.Next(0, rctBuiltRooms.Count())];

            //pick a random point within a room
            //the +1 / -1 on the values are to stop a corner from being chosen
            pLocation = new Point(rnd.Next(rctCurrentRoom.Left + 1, rctCurrentRoom.Right - 1)
                                  , rnd.Next(rctCurrentRoom.Top + 1, rctCurrentRoom.Bottom - 1));


            //get a random direction
            pDirection = directions_straight[rnd.Next(0, directions_straight.GetLength(0))];

            do
            {
                //move in that direction
                pLocation.Offset(pDirection);

                if (!Point_Valid(pLocation.X, pLocation.Y))
                    return;

                //until we meet an empty cell
            } while (Point_Get(pLocation.X, pLocation.Y) != filledcell);

        }

        #endregion

        #region corridor utitlies

        /// <summary>
        /// Randomly get a point on an existing corridor
        /// </summary>
        /// <param name="Location">Out: location of point</param>
        /// <returns>Bool indicating success</returns>
        private void Corridor_GetEdge(out Point pLocation, out Point pDirection)
        {
            List<Point> validdirections = new List<Point>();

            do
            {
                //the modifiers below prevent the first of last point being chosen
                pLocation = lBuilltCorridors[rnd.Next(1, lBuilltCorridors.Count - 1)];

                //attempt to locate all the empy map points around the location
                //using the directions to offset the randomly chosen point
                foreach (Point p in directions_straight)
                    if (Point_Valid(pLocation.X + p.X, pLocation.Y + p.Y))
                        if (Point_Get(pLocation.X + p.X, pLocation.Y + p.Y) == filledcell)
                            validdirections.Add(p);


            } while (validdirections.Count == 0);

            pDirection = validdirections[rnd.Next(0, validdirections.Count)];
            pLocation.Offset(pDirection);

        }

        /// <summary>
        /// Build the contents of lPotentialCorridor, adding it's points to the builtCorridors
        /// list then empty
        /// </summary>
        private void Corridor_Build()
        {
            foreach (Point p in lPotentialCorridor)
            {
                Point_Set(p.X, p.Y, emptycell);
                lBuilltCorridors.Add(p);
            }

            lPotentialCorridor.Clear();
        }

        /// <summary>
        /// Get a starting point for a corridor, randomly choosing between a room and a corridor.
        /// </summary>
        /// <param name="Location">Out: pLocation of point</param>
        /// <param name="Location">Out: pDirection of point</param>
        /// <returns>Bool indicating if location found is OK</returns>
        private bool Corridor_GetStart(out Point pLocation, out Point pDirection)
        {
            rctCurrentRoom = new Rectangle();
            lPotentialCorridor = new List<Point>();

            if (lBuilltCorridors.Count > 0)
            {
                if (rnd.Next(0, 100) >= BuildProb)
                    Room_GetEdge(out pLocation, out pDirection);
                else
                    Corridor_GetEdge(out pLocation, out pDirection);
            }
            else//no corridors present, so build off a room
                Room_GetEdge(out pLocation, out pDirection);

            //finally check the point we've found
            return Corridor_PointTest(pLocation, pDirection) == CorridorItemHit.OK;

        }

        /// <summary>
        /// Attempt to make a corridor, storing it in the lPotentialCorridor list
        /// </summary>
        /// <param name="pStart">Start point of corridor</param>
        /// <param name="pTurns">Number of turns to make</param>
        private CorridorItemHit CorridorMake_Straight(ref Point pStart, ref Point pDirection, int pTurns, bool pPreventBackTracking)
        {

            lPotentialCorridor = new List<Point>();
            lPotentialCorridor.Add(pStart);

            int corridorlength;
            Point startdirection = new Point(pDirection.X, pDirection.Y);
            CorridorItemHit outcome;

            while (pTurns > 0)
            {
                pTurns--;

                corridorlength = rnd.Next(Corridor_Min, Corridor_Max);
                //build corridor
                while (corridorlength > 0)
                {
                    corridorlength--;

                    //make a point and offset it
                    pStart.Offset(pDirection);

                    outcome = Corridor_PointTest(pStart, pDirection);
                    if (outcome != CorridorItemHit.OK)
                        return outcome;
                    else
                        lPotentialCorridor.Add(pStart);
                }

                if (pTurns > 1)
                    if (!pPreventBackTracking)
                        pDirection = Direction_Get(pDirection);
                    else
                        pDirection = Direction_Get(pDirection, startdirection);
            }

            return CorridorItemHit.completed;
        }

        /// <summary>
        /// Test the provided point to see if it has empty cells on either side
        /// of it. This is to stop corridors being built adjacent to a room.
        /// </summary>
        /// <param name="pPoint">Point to test</param>
        /// <param name="pDirection">Direction it is moving in</param>
        /// <returns></returns>
        private CorridorItemHit Corridor_PointTest(Point pPoint, Point pDirection)
        {

            if (!Point_Valid(pPoint.X, pPoint.Y))//invalid point hit, exit
                return CorridorItemHit.invalid;
            else if (lBuilltCorridors.Contains(pPoint))//in an existing corridor
                return CorridorItemHit.existingcorridor;
            else if (lPotentialCorridor.Contains(pPoint))//hit self
                return CorridorItemHit.self;
            else if (rctCurrentRoom != null && rctCurrentRoom.Contains(pPoint))//the corridors origin room has been reached, exit
                return CorridorItemHit.originroom;
            else
            {
                //is point in a room
                foreach (Rectangle r in rctBuiltRooms)
                    if (r.Contains(pPoint))
                        return CorridorItemHit.existingroom;
            }


            //using the property corridor space, check that number of cells on
            //either side of the point are empty
            foreach (int r in Enumerable.Range(-CorridorSpace, 2 * CorridorSpace + 1).ToList())
            {
                if (pDirection.X == 0)//north or south
                {
                    if (Point_Valid(pPoint.X + r, pPoint.Y))
                        if (Point_Get(pPoint.X + r, pPoint.Y) != filledcell)
                            return CorridorItemHit.tooclose;
                }
                else if (pDirection.Y == 0)//east west
                {
                    if (Point_Valid(pPoint.X, pPoint.Y + r))
                        if (Point_Get(pPoint.X, pPoint.Y + r) != filledcell)
                            return CorridorItemHit.tooclose;
                }

            }

            return CorridorItemHit.OK;
        }


        #endregion

        #region direction methods

        /// <summary>
        /// Get a random direction, excluding the opposite of the provided direction to
        /// prevent a corridor going back on it's Build
        /// </summary>
        /// <param name="dir">Current direction</param>
        /// <returns></returns>
        private Point Direction_Get(Point pDir)
        {
            Point NewDir;
            do
            {
                NewDir = directions_straight[rnd.Next(0, directions_straight.GetLength(0))];
            } while (Direction_Reverse(NewDir) == pDir);

            return NewDir;
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
                NewDir = directions_straight[rnd.Next(0, directions_straight.GetLength(0))];
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

        #region room test

        /// <summary>
        /// Check if rctCurrentRoom can be built
        /// </summary>
        /// <returns>Bool indicating success</returns>
        private bool Room_Verify()
        {
            //make it one bigger to ensure that testing gives it a border
            rctCurrentRoom.Inflate(RoomDistance, RoomDistance);

            //check it occupies legal, empty coordinates
            for (int x = rctCurrentRoom.Left; x <= rctCurrentRoom.Right; x++)
                for (int y = rctCurrentRoom.Top; y <= rctCurrentRoom.Bottom; y++)
                    if (!Point_Valid(x, y) || Point_Get(x, y) != filledcell)
                        return false;

            //check it doesn't encroach onto existing rooms
            foreach (Rectangle r in rctBuiltRooms)
                if (r.IntersectsWith(rctCurrentRoom))
                    return false;

            rctCurrentRoom.Inflate(-RoomDistance, -RoomDistance);

            //check the room is the specified distance away from corridors
            rctCurrentRoom.Inflate(CorridorDistance, CorridorDistance);

            foreach (Point p in lBuilltCorridors)
                if (rctCurrentRoom.Contains(p))
                    return false;

            rctCurrentRoom.Inflate(-CorridorDistance, -CorridorDistance);

            return true;
        }

        /// <summary>
        /// Add the global Room to the rooms collection and draw it on the map
        /// </summary>
        private void Room_Build()
        {
            rctBuiltRooms.Add(rctCurrentRoom);

            for (int x = rctCurrentRoom.Left; x <= rctCurrentRoom.Right; x++)
                for (int y = rctCurrentRoom.Top; y <= rctCurrentRoom.Bottom; y++)
                    map[x, y] = emptycell;

        }

        #endregion

        #region Map Utilities

        /// <summary>
        /// Check if the point falls within the map array range
        /// </summary>
        /// <param name="x">x to test</param>
        /// <param name="y">y to test</param>
        /// <returns>Is point with map array?</returns>
        private Boolean Point_Valid(int x, int y)
        {
            return x >= 0 & x < map.GetLength(0) & y >= 0 & y < map.GetLength(1);
        }

        /// <summary>
        /// Set array point to specified value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="val"></param>
        private void Point_Set(int x, int y, int val)
        {
            map[x, y] = val;
        }

        /// <summary>
        /// Get the value of the specified point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int Point_Get(int x, int y)
        {
            return map[x, y];
        }

        #endregion

        public delegate void moveDelegate();
        public event moveDelegate playerMoved;

    }
}
