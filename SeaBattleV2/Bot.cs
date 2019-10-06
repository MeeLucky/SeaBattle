using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaBattleV2
{
    class Bot
    {
        private GameLogic UserField;
        private List<FieldsElement> Elems = new List<FieldsElement>();
        private Goal Goal = null;

        public Bot(GameLogic UserField)
        {
            this.UserField = UserField;
            foreach (FieldsElement item in UserField.Field)
            {
                this.Elems.Add(item);
            }
        }
        public bool Move()
        {
            if(Goal == null)
                return RandomShot();

            int[] pos = Goal.GetNextShotPosition();
            if (pos[0] == -1)
            {
                Goal = null;
                return RandomShot();
            }

            FieldsElement elem = UserField.Field[pos[0], pos[1]];
            if (elem.IsShip())
                Goal.Elems.Add(elem);

            return UserField.Move(pos[0], pos[1]);
        }

        private bool RandomShot()
        {
            if (Elems.Count() == 0)
                return false;//this = 0 when all cells are used
                
            Random rnd = new Random();
            int n = rnd.Next(Elems.Count());
            FieldsElement elem = Elems[n];
            Elems.RemoveAt(n);
            bool ret = UserField.Move(elem.GetY(), elem.GetX());

            if(ret)
                Goal = new Goal(elem, UserField);

            List<FieldsElement> state = UserField.NewState;
            foreach (FieldsElement item in state)
            {
                Elems.Remove(item);
            }

            return ret;
        }
    }
}
