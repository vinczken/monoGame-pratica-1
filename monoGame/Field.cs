using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monoGame
{
    public class Field
    {
        private Hitbox hitbox;
        public Hitbox Hitbox {  get { return hitbox; } }

        public Field()
        {
            hitbox = new Hitbox();
        }
    }
}
