using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    class MapGenerator
    {
        public int MapWidth;
        public int MapHeight;
        public int MinRoomSize;
        public int MaxRoomSize;
        public int MaxNumberOfRooms;
        public int[,] Map;

        public List<Room> Rooms;

        public MapGenerator(int mapWidth, int mapHeight, int minRoomSize, int maxRoomSize, int maxNumberOfRooms)
        {
            Map = new int[mapHeight, mapWidth];
            Rooms = new List<Room>();
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            MinRoomSize = minRoomSize;
            MaxRoomSize = maxRoomSize;
            MaxNumberOfRooms = maxNumberOfRooms;
        }

        public void PlaceRooms()
        {
            for (int I = 0; I < MaxNumberOfRooms; I++)
            {
                int Width = MinRoomSize + new Random(MaxRoomSize - MinRoomSize + 1).Next();
                int Height = MinRoomSize + new Random(MaxRoomSize - MinRoomSize + 1).Next();
                int X = new Random(MapWidth - Width - 1).Next() + 1;
                int Y = new Random(MapHeight - Height - 1).Next() + 1;

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
                }
            }

        }

        private int[] fillArray()
        {
            foreach (Room room in Rooms)
            {
                for (int I = room.Y1; I <= room.Y2)
            }
            return null;
        }
    }
}
