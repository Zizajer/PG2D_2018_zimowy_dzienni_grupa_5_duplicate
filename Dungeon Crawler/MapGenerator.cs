using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    //Source: https://gamedevelopment.tutsplus.com/tutorials/create-a-procedurally-generated-dungeon-cave-system--gamedev-10099 (The main idea of algorithm)
    class MapGenerator
    {
        public int MapWidth;
        public int MapHeight;
        public int MinRoomSize;
        public int MaxRoomSize;
        public int MaxNumberOfRooms;

        // ByteMap representation:
        // - 2: `Cell` is transparent and walkable - currently unused
        // - 1: `Cell` is walkable (but not transparent)
        // - 0: `Cell` is transparent (but not walkable)
        // - 3: `Cell` is not transparent or walkable - currently unused
        public byte[,] ByteMap;
        
        public List<Room> Rooms;

        public MapGenerator(int mapWidth, int mapHeight, int minRoomSize, int maxRoomSize, int maxNumberOfRooms)
        {
            ByteMap = new byte[mapHeight, mapWidth];
            Rooms = new List<Room>();
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            MinRoomSize = minRoomSize;
            MaxRoomSize = maxRoomSize;
            MaxNumberOfRooms = maxNumberOfRooms;
        }

        public byte[,] Generate()
        {
            Random Random = new Random();
            for (int I = 0; I < MaxNumberOfRooms; I++)
            {
                int Width = MinRoomSize + Random.Next(MaxRoomSize - MinRoomSize + 1);
                int Height = MinRoomSize + Random.Next(MaxRoomSize - MinRoomSize + 1);
                int X = Random.Next(MapWidth - Width - 1) + 1;
                int Y = Random.Next(MapHeight - Height - 1) + 1;

                Room NewRoom = new Room(X, Y, Width, Height);
                bool Failed = false;
                foreach (Room Room in Rooms)
                {
                    if (NewRoom.Intersects(Room))
                    {
                        Failed = true;
                        break;
                    }
                }
                if (!Failed)
                {
                    Rooms.Add(NewRoom);

                    //Draw room
                    AddToArray(NewRoom);

                    if(Rooms.Count >= 2)
                    {
                        Point PrevCenter = Rooms[Rooms.Count - 2].Center;
                        Point CurrentCenter = Rooms[Rooms.Count - 1].Center;

                        //Draw corridor
                        AddToArray(PrevCenter, CurrentCenter);

                    }
                }
            }

            return ByteMap;
        }

        private void AddToArray(Room room)
        {
            for (int I = room.Y1; I <= room.Y2; I++)
            {
                for (int J = room.X1; J<= room.X2; J++)
                {
                    ByteMap[I, J] = 1;
                }
            }
        }

        private void AddToArray(Point room1Center, Point room2Center)
        {
            for (int I = Math.Min(room1Center.X, room2Center.X); I <= Math.Max(room1Center.X, room2Center.X); I++)
            {
                ByteMap[room1Center.Y, I] = 1;
            }
            for (int I = Math.Min(room1Center.Y, room2Center.Y); I <= Math.Max(room1Center.Y, room2Center.Y); I++)
            {
                ByteMap[I, room2Center.X] = 1;
            }
        }

        //StringMap representation (to preserve compatibility with RogueSharp)
        // - `.`: `Cell` is transparent and walkable - currently unused
        // - `s`: `Cell` is walkable (but not transparent)
        // - `o`: `Cell` is transparent (but not walkable)
        // - `#`: `Cell` is not transparent or walkable - currently unused

        private String getStringRepresentation()
        {
            StringBuilder StringBuilder = new StringBuilder();
            for (int I = 0; I <= ByteMap.GetUpperBound(0); I++)
            {
                for (int J = 0; J <= ByteMap.GetUpperBound(1); J++)
                {
                    switch (ByteMap[I, J])
                    {
                        case 0:
                            StringBuilder.Append('o');
                            break;
                        case 1:
                            StringBuilder.Append('s');
                            break;
                        case 2:
                            StringBuilder.Append('.');
                            break;
                        case 3:
                            StringBuilder.Append('#');
                            break;
                    }
                }
                StringBuilder.Append('\n');
            }
            return StringBuilder.ToString();
        }

        private byte[,] getByteArrayRepresentation()
        {
            return ByteMap;
        }
    }
}
