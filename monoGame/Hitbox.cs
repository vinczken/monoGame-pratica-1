using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace monoGame
{
    public class Hitbox
    {
        private float initialX;
        public float InitialX {  get { return initialX; } }
        private float initialY;
        public float InitialY { get { return initialY; } }

        private float finalX;
        public float FinalX {  get { return finalX; } }
        private float finalY;
        public float FinalY { get { return finalY; } }

        public void SetInitialHitbox(float x, float y)
        {
            this.initialX = x;
            this.initialY = y;
        }

        public void SetFinalHitbox(float x, float y)
        {
            this.finalX = x;
            this.finalY = y;
        }
    }
}
