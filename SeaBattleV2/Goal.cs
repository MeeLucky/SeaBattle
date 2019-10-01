using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleV2
{
    class Goal
    {
        public GameLogic UserField;
        public List<FieldsElement> Elems = new List<FieldsElement>();
        public bool ShipDirection;
        public int GoalDirection = 1;

        public Goal (FieldsElement elem, GameLogic userField)
        {
            this.Elems.Add(elem);
            this.UserField = userField;
        }

        public int[] GetNextShot()
        {
            if(Elems.Count() == 1)
            {
                FieldsElement elem = Elems[0];
                int y = elem.GetY();
                int x = elem.GetX();
                for(int i = 0; i < 4; i++)
                {
                    if(CheckEdge(y, x))
                    {
                        return cfd(y, x);
                    }
                    else
                    {
                        ChangeGoalDirection();
                    }
                }

                //ship destoyed
                return new int[] { -1, -1 };
            }
            else
            {
                int minY = 9, minX = 9;
                int maxY = 0, maxX = 0;
                foreach (FieldsElement item in Elems)
                {
                    int y = item.GetY();
                    int x = item.GetX();
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                }

                FieldsElement elem1 = UserField.Field[minY, minX];//начало открытого коробля
                FieldsElement elem2 = UserField.Field[maxY, maxX];//конец открытого коробля
                ShipDirection = Elems[0].GetY() == Elems[1].GetY() ? true : false;

                for(int i = 0; i < 2; i++)
                {
                    if (CheckEdge(elem1, elem2))
                    {
                        //return cfd for goalDir and shipDir
                        if (GoalDirection == 1 || GoalDirection == 2)
                            return cfd(elem2.GetY(), elem2.GetX());
                        else
                            return cfd(elem1.GetY(), elem1.GetX());
                    }
                    else
                    {
                        ChangeGoalDirection(true);
                    }
                }
                //ship destoyed
                return new int[] { -1, -1 };
            }
        }

        private bool CheckEdge(FieldsElement elem1, FieldsElement elem2)
        {
            switch(GoalDirection)
            {
                case 1:
                    return CheckEdge(elem2.GetY(), elem2.GetX());
                case 2:
                    return CheckEdge(elem2.GetY(), elem2.GetX());
                case 3:
                    return CheckEdge(elem1.GetY(), elem1.GetX());
                case 4:
                    return CheckEdge(elem1.GetY(), elem1.GetX());
                default:
                    return false;
            }
        }

        private bool CheckEdge(int y, int x)
        {
            int[] pos = cfd(y, x);
            if (pos == null)
                return false;
            FieldsElement elem = UserField.Field[pos[0], pos[1]];
            return !elem.IsFired;
        }

        public void AddElem(FieldsElement elem)
        {
            this.Elems.Add(elem);
        }
        private int[] cfd(int y, int x)//correction for direction
        {
            switch(GoalDirection)
            {//1- right, 2 - down, etc
                case 1:
                    x = x + 1;
                    break;
                case 2:
                    y = y + 1;
                    break;
                case 3:
                    x = x - 1;
                    break;
                case 4:
                    y = y - 1;
                    break;
            }
            if (y >= 0 && y < 10 && x >= 0 && x < 10)
                return new int[] { y, x };
            else
                return null;
        }

        private void DetectShipDirection()
        {
            if (Elems.Count() != 0)
                ShipDirection = Elems[0].GetY() == Elems[1].GetY() ? true : false;
        }

        private void ChangeGoalDirection(bool ShipDirOnOff = false)
        {
            if(ShipDirOnOff)
            {
                if (ShipDirection)
                    GoalDirection = GoalDirection == 1 ? 3 : 1;
                else
                    GoalDirection = GoalDirection == 2 ? 4 : 2;

                return;
            }

            if (GoalDirection == 4)
                GoalDirection = 1;
            else
                GoalDirection++;
        }
    }
}
