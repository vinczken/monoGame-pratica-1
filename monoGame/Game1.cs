using MathNet.Numerics;
using MathNet.Numerics.RootFinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
namespace monoGame
{
    public class Game1 : Game
    {

        Ball ball;
        Field field;

        private MouseState mouseState;
        private Vector2 mouseFinalPosition;

        private float maxSpeed;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont _font;
        private Texture2D _pixel;

        public bool IsOnHitbox(MouseState mouseState, Hitbox hitbox)
        {
            if ((mouseState.X >= hitbox.InitialX && mouseState.X <= hitbox.FinalX) && (mouseState.Y >= hitbox.InitialY && mouseState.Y <= hitbox.FinalY))
                return true;
            else
                return false;
        }

        public List<string> IsInDeadZone(Ball ball, Field field)
        {
            List<string> temp = new List<string>();

            if (ball.Position.X > field.Hitbox.FinalX - ball.Texture.Width / 2)
                temp.Add("X, >");
            else if (ball.Position.X < field.Hitbox.InitialX + ball.Texture.Width / 2)
                temp.Add("X, <");

            if (ball.Position.Y > field.Hitbox.FinalY - ball.Texture.Height / 2)
                temp.Add("Y, >");
            else if (ball.Position.Y < field.Hitbox.InitialY + ball.Texture.Height / 2)
                temp.Add("Y, <");

            return temp;
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            field = new Field();
            ball = new Ball();

            field.Hitbox.SetInitialHitbox(0,0); 
            field.Hitbox.SetFinalHitbox(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            ball.Position = new Vector2(field.Hitbox.FinalX / 2, field.Hitbox.FinalY / 2);
            ball.Speed = 1f;
            ball.Direction = new Vector2(1, 1);

            maxSpeed = 50f;
            base.Initialize();
        }

        protected override void LoadContent()  
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ball.Texture = Content.Load<Texture2D>("ball");
            ball.Origin = new Vector2(ball.Texture.Width / 2, ball.Texture.Height / 2);

            _font = Content.Load<SpriteFont>("font");
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.Black });
        }

        protected override void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float initialX = ball.Position.X - ball.Texture.Width / 2;
            float initialY = ball.Position.Y - ball.Texture.Height / 2;
            float finalX = ball.Position.X + ball.Texture.Width / 2;
            float finalY = ball.Position.Y + ball.Texture.Height / 2;
            
            ball.UpdateHitbox(initialX, initialY, finalX, finalY);

            ball.Speed = MathHelper.Clamp(ball.Speed, 0, maxSpeed);
            float updatedBallSpeed = ball.Speed * deltaTime;

            if(ball.Speed < 0.2f)
            {
                ball.Speed = 0;
            }

            if(ball.Speed == 0)
            {
                ball.IsMoving = false;
            }

            if ((mouseState.LeftButton == ButtonState.Pressed) && (IsOnHitbox(mouseState, ball.Hitbox)))
                ball.IsAiming = true;

            if (ball.IsAiming)
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    mouseFinalPosition = new Vector2(mouseState.X, mouseState.Y);
                    ball.IsMoving = false;
                }
                else
                {
                    ball.IsAiming = false;
                    ball.Direction = ball.Position - mouseFinalPosition;
                    float length = ball.Direction.Length();
                    if(length > 0)
                    {
                        ball.Direction.Normalize();      //precisa estar normalizado
                        ball.Speed = MathHelper.Clamp(length, 0, maxSpeed);
                        ball.IsMoving = true;
                    }
                }
            }

            if (ball.IsMoving)
            {
                Vector2 speedSum = Vector2.Multiply(ball.Direction, updatedBallSpeed);
                ball.Position += speedSum;
                ball.Speed *= 0.99f;
            }


            List<string> isInDeadZone = IsInDeadZone(ball, field);
            if(isInDeadZone.Count > 0)
            {
                Vector2 normal;
                float dotProduct;
                if (isInDeadZone.Contains("X, >") || isInDeadZone.Contains("X, <"))
                {
                    ball.Speed *= 0.9f;
                    normal = new Vector2(1, 0);
                    dotProduct = Vector2.Dot(ball.Direction, normal);

                    if(isInDeadZone.Contains("X, >"))
                    {
                        ball.Position = new Vector2(field.Hitbox.FinalX - ball.Texture.Width / 2, ball.Position.Y);
                    }
                    else
                    {
                        ball.Position = new Vector2(field.Hitbox.InitialX + ball.Texture.Width / 2, ball.Position.Y);
                    }

                    ball.Direction -= Vector2.Multiply(normal, 2 * dotProduct);
                    ball.Direction.Normalize();
                }
                if (isInDeadZone.Contains("Y, >") || isInDeadZone.Contains("Y, <"))
                {
                    ball.Speed *= 0.9f;
                    normal = new Vector2(0, 1);
                    dotProduct = Vector2.Dot(ball.Direction, normal);

                    if (isInDeadZone.Contains("Y, >"))
                    {
                        ball.Position = new Vector2(ball.Position.X, field.Hitbox.FinalY - ball.Texture.Height / 2);
                    }
                    else
                    {
                        ball.Position = new Vector2(ball.Position.X, field.Hitbox.InitialY + ball.Texture.Height / 2);
                    }

                    ball.Direction -= Vector2.Multiply(normal, 2 * dotProduct);
                    ball.Direction.Normalize();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.DrawString(_font, $"({ball.Position.X}, {ball.Position.Y})", new Vector2(0, 0), Color.White);
            _spriteBatch.DrawString(_font, $"({mouseState.X}, {mouseState.Y})", new Vector2(0, 15), Color.White);
            _spriteBatch.DrawString(_font, $"{ball.Speed}", new Vector2(400, 0), Color.White);

            if (ball.IsAiming)
            {
                _spriteBatch.DrawString(_font, $"ta ativo", new Vector2(200, 0), Color.Red);

                Vector2 directionLine = mouseFinalPosition - ball.Position;
                float length = directionLine.Length();
                directionLine.Normalize();

                float rotation = (float)Math.Atan2(directionLine.Y, directionLine.X);

                _spriteBatch.Draw(_pixel, ball.Position, null, Color.Black, rotation, Vector2.Zero, new Vector2(length, 2), SpriteEffects.None, 0);
            }

            if (ball.IsMoving)
            {
                _spriteBatch.DrawString(_font, $"ta ativo", new Vector2(200, 0), Color.Blue);
            }

            _spriteBatch.Draw(ball.Texture, ball.Position, null, Color.White, 0f, ball.Origin, Vector2.One, SpriteEffects.None, 0f);


            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
