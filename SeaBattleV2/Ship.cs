using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleV2
{
    class Ship
    {
        public int ShipId;
        public int DecksCount;
        public int Decks;
        public bool Direction;
        int x1, y1, x2, y2;

        public Ship(int id, int y1, int x1, int y2, int x2, bool dir, int decksCount)
        {
            this.ShipId = id;
            this.y1 = y1;
            this.x1 = x1;
            this.y2 = y2;
            this.x2 = x2;
            this.Direction = dir;
            this.Decks = decksCount;
            this.DecksCount = decksCount;
        }

        public int[] GetStartPosition() { return new int[] { this.y1, this.x1 }; }

        public bool IsDead() { return Decks == 0; }
    }
}
