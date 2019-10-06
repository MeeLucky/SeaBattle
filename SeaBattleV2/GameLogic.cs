 using System;
using System.Collections.Generic;

namespace SeaBattleV2
{
    class GameLogic
    {
        static Random rnd = new Random(unchecked((int)(DateTime.Now.Ticks)));//он здесь чтобы рандом был рандомным...
        public readonly FieldsElement[,] Field = new FieldsElement[10, 10];
        private readonly List<Ship> ShipsList = new List<Ship>();
        public  List<FieldsElement> NewState = new List<FieldsElement>();
        private int ShipID = 1;
        public string propName;
        public int hp = 10;//10 ships
        public int Score = 0;
        public GameLogic Enemy;
        public GameLogic(string propName)
        {
            this.propName = propName;
            for (int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    Field[i, j] = new FieldsElement(i, j);
                }
            }

            AddShipInRandomPosition(4);

            AddShipInRandomPosition(3);
            AddShipInRandomPosition(3);

            AddShipInRandomPosition(2);
            AddShipInRandomPosition(2);
            AddShipInRandomPosition(2);

            AddShipInRandomPosition(1);
            AddShipInRandomPosition(1);
            AddShipInRandomPosition(1);
            AddShipInRandomPosition(1);
        }

        public bool IsLose()
        {
            return hp == 0;
        }

        public bool CheckShotPosition(int y, int x)
        {
            if ( !(y >= 0 && y <= 10 && x >= 0 && x <= 10) )
                return false;

            if (this.Field[y, x].IsFired)
                return false;
            else
                return true;
        }

        public bool Move(int y, int x)
        {
            FieldsElement elem = this.Field[y, x];
            elem.IsFired = true;
            NewState.Add(elem);
            if (elem.IsShip())
            {
                Score += 5;
                Enemy.Score -= 1;

                elem.ShipRef.Decks -= 1;//hit ship
                if (elem.ShipRef.IsDead())
                    Explosion(elem.ShipRef);

                return true;
            }
            else
            {
                Score--;
                return false;
            }
        }

        private void Explosion(Ship ship)
        {
            Score += 10;
            Enemy.Score -= 5;
            hp--;

            int[] start = ship.GetStartPosition();
            bool dir = ship.Direction;
            int decks = ship.DecksCount;

            int y = start[0];
            int x = start[1];

            for (int i = 0; i < decks; i++)
            {
                //explosion
                bool right = x + 1 < 10;
                bool left = x - 1 >= 0;
                bool up = y - 1 >= 0;
                bool down = y + 1 < 10;

                //each
                if (dir)
                {
                    if (up) Move(y - 1, x);
                    if (down) Move(y + 1, x);
                }
                else
                {
                    if (left) Move(y, x - 1);
                    if (right) Move(y, x + 1);
                }

                //first
                if (i == 0)
                {
                    if (dir)
                    {
                        if (left) Move(y, x - 1);
                        if (left && up) Move(y - 1, x - 1);
                        if (left && down) Move(y + 1, x - 1);
                    }
                    else
                    {
                        if (up) Move(y - 1, x);
                        if (up && left) Move(y - 1, x - 1);
                        if (up && right) Move(y - 1, x + 1);
                    }
                }

                //last
                if (i == decks - 1)
                {
                    if (dir)
                    {
                        if (right) Move(y, x + 1);
                        if (right && up) Move(y - 1, x + 1);
                        if (right && down) Move(y + 1, x + 1);
                    }
                    else
                    {
                        if (down) Move(y + 1, x);
                        if (down && left) Move(y + 1, x - 1);
                        if (down && right) Move(y + 1, x + 1);
                    }
                }

                if (dir) x++;
                else y++;
            }
        }

        public void ClearState()
        {
            NewState = new List<FieldsElement>();
        }

        private void AddShipInRandomPosition(int deckCount)
        {
            bool loop = false;
            while (!loop)
            {
                bool dir = rnd.NextDouble() < 0.5;
                int x = rnd.Next(0, 9);
                int y = rnd.Next(0, 9);
                loop = AddShip(y, x, deckCount, dir);
            }
        }

        private bool AddShip(int y, int x, int deckCount, bool dir)
        {
            int y2 = dir ? y : y + deckCount - 1;
            int x2 = dir ? x + deckCount - 1 : x;

            if (CheckCells(y, x, y2, x2, deckCount, dir))
                return false;

            Ship TheShip = new Ship(ShipID, y, x, y2, x2, dir, deckCount);
            ShipsList.Add(TheShip);
            //ссылка на корабль из листа хранится в каждой корабельной клетке поля
            ShipID++;

            int outY = y, outX = x;
            for (int i = 0; i < deckCount; i++)
            {
                if (dir)
                {
                    outX = x + i;
                }
                else
                {
                    outY = y + i;
                }
                    this.Field[outY, outX].Ship = deckCount;
                    this.Field[outY, outX].ShipRef = TheShip;
            }
            
            return true;
        }

        private bool CheckCells(int y1, int x1, int y2, int x2, int decks, bool direction)//return true if finding ship
        {
            if (x1 < 0 || y1 < 0 || x2 >= 10 || y2 >= 10)
                return true;//выход за границы поля
            
            bool up = true, down = true, right = true, left = true;
            if (x1 == 0) left = false;
            if (y1 == 0) up = false;
            if (x2 == 9) right = false;
            if (y2 == 9) down = false;

            //на самом корабле
            if (direction)
            {
                for (int i = 0; i < decks; i++)
                    if (this.Field[y1, x1 + i].IsShip())
                        return true;
            }
            else
            {
                for (int i = 0; i < decks; i++)
                    if (this.Field[y1 + i, x1].IsShip())
                        return true;
            }
            //вокруг коробля
            if (direction)
            {
                for (int i = 0; i < decks; i++)
                {
                    if (up && this.Field[y1 - 1, x1 + i].IsShip())
                        return true;

                    if (down && this.Field[y1 + 1, x1 + i].IsShip())
                        return true;
                }
                if (right && this.Field[y2, x2 + 1].IsShip())
                    return true;
                if (left && this.Field[y1, x1 - 1].IsShip())
                    return true;
                //углы вокруг коробля
                if (up && right && this.Field[y1 - 1, x2 + 1].IsShip())
                    return true;
                if (down && right && this.Field[y1 + 1, x2 + 1].IsShip())
                    return true;
                if (up && left && this.Field[y1 - 1, x1 - 1].IsShip())
                    return true;
                if (down && left && this.Field[y1 + 1, x1 - 1].IsShip())
                    return true;
            }
            else
            {
                for (int i = 0; i < decks; i++)
                {
                    if (right && this.Field[y1 + i, x1 + 1].IsShip())
                        return true;

                    if (left && this.Field[y1 + i, x1 - 1].IsShip())
                        return true;
                }
                if (up && this.Field[y1 - 1, x2].IsShip())
                    return true;
                if (down && this.Field[y2 + 1, x1].IsShip())
                    return true;
                //углы вокруг коробля
                if (up && right && this.Field[y1 - 1, x1 + 1].IsShip())
                    return true;
                if (down && right && this.Field[y2 + 1, x2 + 1].IsShip())
                    return true;
                if (up && left && this.Field[y1 - 1, x1 - 1].IsShip())
                    return true;
                if (down && left && this.Field[y2 + 1, x1 - 1].IsShip())
                    return true;
            }
            //в углах вокруг коробля
            return false;
        }
    }
}
