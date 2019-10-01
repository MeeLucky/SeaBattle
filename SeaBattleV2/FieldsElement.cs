using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleV2
{
    class FieldsElement
    {
        private int y { get; }
        private int x { get; }
        public int Ship;
        public Ship ShipRef;
        public bool IsFired;

        public int[] GetPosition() { return new int[] { this.y, this.x }; }
        public int GetY () { return this.y; }
        public int GetX () { return this.x; }

        public FieldsElement (int y, int x, int ship = 0, Ship shipRef = null)
        {
            this.y = y;
            this.x = x;
            this.Ship = ship;
            this.ShipRef = shipRef;
            this.IsFired = false;
        }

        public bool IsShip() { return this.Ship > 0; }//return true if is ship
    }
}
