using MathNet.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
namespace monoGame
{
    public class Game1 : Game
    {

        private Texture2D ballTexture;
        private Vector2 ballPosition;
        private float ballSpeed;
        private Vector2 direction;

        private MouseState mouseState;
        private Vector2 mouseFinalPosition;

        private float maxSpeed;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont _font;
        private Texture2D _pixel;

        private bool isAiming = false;
        private bool isMoving = false;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //                                  width da resolução atual do jogo        height da resolução atual do jogo
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            ballSpeed = 1f;
            maxSpeed = 800f;
            mouseState = Mouse.GetState();

            base.Initialize();
        }


        protected override void LoadContent()  //Só é chamado uma vez no Initialize()
        {
            // TODO: use this.Content to load your game content here
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ballTexture = Content.Load<Texture2D>("ball");
            direction = new Vector2(1, 1);

            _font = Content.Load<SpriteFont>("font");
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.Black });

            mouseFinalPosition = new Vector2(mouseState.X, mouseState.Y);
        }

        protected override void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            // TODO: Add your update logic here
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // faz com que a velocidade da bola seja constante independente de quanto o update seja chamado!


            float[] ballHitboxX = { ballPosition.X - ballTexture.Width / 2, ballPosition.X + ballTexture.Width / 2 };
            float[] ballHitboxY = { ballPosition.Y - ballTexture.Height / 2, ballPosition.Y + ballTexture.Height / 2 };
            
            ballSpeed = MathHelper.Clamp(ballSpeed, 0.1f, maxSpeed);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float updatedBallSpeed = ballSpeed * deltaTime;
            if(ballSpeed < 10f)
            {
                ballSpeed = 0;
            }
            if(ballSpeed == 0)
            {
                isMoving = false;
            }

            //movimento
            if ((mouseState.LeftButton == ButtonState.Pressed) && (mouseState.X >= ballHitboxX[0] && mouseState.X <= ballHitboxX[1]) && (mouseState.Y >= ballHitboxY[0] && mouseState.Y <= ballHitboxY[1]))
            {
                isAiming = true;
            }

            if (isAiming)
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    mouseFinalPosition = new Vector2(mouseState.X, mouseState.Y);
                    isMoving = false;
                }
                else
                {
                    isAiming = false;
                    direction = ballPosition - mouseFinalPosition;
                    float length = direction.Length();
                    if(length > 0)
                    {
                        direction.Normalize();      //precisa estar normalizado
                        ballSpeed = MathHelper.Clamp(length * 2, 0, maxSpeed);
                        isMoving = true;
                    }


                }
            }


            if (isMoving)
            {
                Vector2 speedSum = Vector2.Multiply(direction, updatedBallSpeed);
                ballPosition += speedSum;
                ballSpeed *= 0.99f;
            }


            //limite
            if ((ballPosition.X > _graphics.PreferredBackBufferWidth - ballTexture.Width / 2) || (ballPosition.X < ballTexture.Width / 2))
            {
                ballSpeed *= 0.9f;
                Vector2 normal = new Vector2(1, 0);
                float dotProduct = Vector2.Dot(direction, normal);
                direction = direction - Vector2.Multiply(normal, 2 * dotProduct);
                direction.Normalize();
                if(ballPosition.X > _graphics.PreferredBackBufferWidth - ballTexture.Width / 2)
                {
                    ballPosition.X = _graphics.PreferredBackBufferWidth - ballTexture.Width / 2;
                }
                else
                {
                    ballPosition.X = ballTexture.Width / 2;
                }
            }
            if((ballPosition.Y > _graphics.PreferredBackBufferHeight - ballTexture.Height / 2) || (ballPosition.Y < ballTexture.Height / 2))
            {
                ballSpeed *= 0.9f;
                Vector2 normal = new Vector2(0, 1);
                float dotProduct = Vector2.Dot(direction, normal);
                direction = direction - Vector2.Multiply(normal, 2 * dotProduct);
                direction.Normalize();
                if (ballPosition.Y > _graphics.PreferredBackBufferHeight - ballTexture.Height / 2)
                {
                    ballPosition.Y = _graphics.PreferredBackBufferHeight - ballTexture.Height / 2;
                }
                else
                {
                    ballPosition.Y = ballTexture.Height / 2;
                }
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.DrawString(_font, $"({ballPosition.X}, {ballPosition.Y})", new Vector2(0, 0), Color.White);
            _spriteBatch.DrawString(_font, $"({mouseState.X}, {mouseState.Y})", new Vector2(0, 15), Color.White);
            _spriteBatch.DrawString(_font, $"{ballSpeed}", new Vector2(400, 0), Color.White);

            if (isAiming)
            {
                _spriteBatch.DrawString(_font, $"ta ativo", new Vector2(200, 0), Color.Red);

                Vector2 directionLine = mouseFinalPosition - ballPosition;
                float length = directionLine.Length();
                directionLine.Normalize();      //precisa estar normalizado

                float rotation = (float)Math.Atan2(directionLine.Y, directionLine.X); //formula para achar o angulo desses pontos cotg(y/x)

                //              textura,   pos inicial,   *,   cor,       rotacao,  origem (0,0) dessa linha,     escala **,  sem efeito,    profundidade  
                // * é pq nenhuma outra textura sera usada           escala**  (x,y) x - comprimento  y - espessura da linha
                _spriteBatch.Draw(_pixel, ballPosition, null, Color.Black, rotation, Vector2.Zero, new Vector2(length, 2), SpriteEffects.None, 0);
            }

            if (isMoving)
            {
                _spriteBatch.DrawString(_font, $"ta ativo", new Vector2(200, 0), Color.Blue);
            }

            //                  textura,     posição,   sourceRectangle, cor,  rotação,                     origem,                        escala,      efeito,             layerdepth
            _spriteBatch.Draw(ballTexture, ballPosition, null, Color.White, 0f, new Vector2(ballTexture.Width / 2, ballTexture.Height / 2), Vector2.One, SpriteEffects.None, 0f);


            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
