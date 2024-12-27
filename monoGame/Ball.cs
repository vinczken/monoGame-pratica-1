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
    public class Ball
    {
        private Texture2D texture;
        public Texture2D Texture { get { return texture; } set { texture = value; } }
        private Vector2 origin;
        public Vector2 Origin { get { return origin; } set { origin = value; } }
        private Vector2 position;
        public Vector2 Position { get { return position; } set { position = value; } }
        private float speed;
        public float Speed {  get { return speed; } set { speed = value; } }
        private Vector2 direction;
        public Vector2 Direction { get { return direction; } set { direction = value; } }
        private Hitbox hitbox;
        public Hitbox Hitbox {  get { return hitbox; } }
        private bool isAiming;
        public bool IsAiming {  get { return isAiming; } set { isAiming = value; } }
        private bool isMoving;
        public bool IsMoving { get { return isMoving; } set { isMoving = value; } }

        public Ball()
        {
            hitbox = new Hitbox();
        }

        public void UpdateHitbox(float initialX, float initialY, float finalX, float finalY)
        {
            this.hitbox.SetInitialHitbox(initialX, initialY);
            this.hitbox.SetFinalHitbox(finalX, finalY);
        }
    }
}
