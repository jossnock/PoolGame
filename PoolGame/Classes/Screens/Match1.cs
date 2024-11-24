using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PoolGame.Classes;

// SCALE: 1cm = 8px

namespace PoolGame.Classes.Screens
{
    internal class Match1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private KeyboardState previousKeyboardState;

        public static PoolBall[] poolBalls;

        // sizing:
        // using the approx. scale 1cm : 8px
        public static int poolBallRadius;
        public static int pocketRadius;
        public static int tablePocketSpacing;


        // bool balls:

        public CueBall _cueBall;
        public Texture2D cueBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        public ObjectBall _eightBall;
        public Texture2D eightBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        public ObjectBall _solidObjectBall1;
        public ObjectBall _solidObjectBall2;
        public ObjectBall _solidObjectBall3;
        public ObjectBall _solidObjectBall4;
        public ObjectBall _solidObjectBall5;
        public ObjectBall _solidObjectBall6;
        public ObjectBall _solidObjectBall7;
        public Texture2D solidObjectBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        public ObjectBall _stripedObjectBall1;
        public ObjectBall _stripedObjectBall2;
        public ObjectBall _stripedObjectBall3;
        public ObjectBall _stripedObjectBall4;
        public ObjectBall _stripedObjectBall5;
        public ObjectBall _stripedObjectBall6;
        public ObjectBall _stripedObjectBall7;
        public Texture2D stripedObjectBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        public Match1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = MainMenu.windowWidth; // default window width
            _graphics.PreferredBackBufferHeight = MainMenu.windowHeight; // default window height
            _graphics.ApplyChanges();
        }

        public void DoAllPoolBallPoolBallCollisions() // in Match1.cs rather than PoolBall.cs so that all velocities can be changed on the same frame
        {
            for (int i = 0; i < poolBalls.Length - 1; i++)
            {
                for (int j = i + 1; j < poolBalls.Length; j++)
                {
                    if (poolBalls[i] == poolBalls[j]) // no need to check if it collides with itself
                    { continue; }
                    else
                    {
                        if (Vector2.Distance(poolBalls[i].position, poolBalls[j].position) <= poolBalls[i].radius * 2)
                        {
                            Vector2 relativePositionVector = poolBalls[i].position - poolBalls[j].position; // the vector that is normal to the collision surface (aka other ball), pointing towards poolBalls[i]
                            Vector2 unitNormalVector = relativePositionVector / relativePositionVector.Length(); // normalised to have a magnitude of 1

                            Vector2 newVelocity = poolBalls[i].velocity + (Vector2.Dot(poolBalls[j].velocity - poolBalls[i].velocity, unitNormalVector) * unitNormalVector);
                            Vector2 newOtherVelocity = poolBalls[j].velocity + (Vector2.Dot(poolBalls[i].velocity - poolBalls[j].velocity, unitNormalVector) * unitNormalVector);

                            poolBalls[j].velocity = newOtherVelocity;
                            poolBalls[i].velocity = newVelocity;
                        }

                        //// if they are stuck in each other:
                        //else if (Vector2.Distance(poolBalls[i].position, poolBalls[j].position) < poolBalls[i].radius * 2)
                        //{
                        //    Vector2 relativePositionVector = poolBalls[i].position - poolBalls[j].position; // the vector that is normal to the collision surface (aka other ball), pointing towards poolBalls[i]
                        //    Vector2 adjustedRelativePositionVector = ((2 * poolBalls[i].radius) - relativePositionVector.Length()) * (relativePositionVector / relativePositionVector.Length());

                        //    poolBalls[j].position -= adjustedRelativePositionVector;
                        //    poolBalls[i].position += adjustedRelativePositionVector;
                        //}
                    }
                }
            }
        }

        public static bool IsAllStationary()
        {
            foreach (PoolBall poolBall in poolBalls)
            {
                if (poolBall.velocity != Vector2.Zero)
                {
                    return false;
                }
            }
            return true;
        }

        protected override void Initialize()
        {
            previousKeyboardState = Keyboard.GetState(); // getting the starting state of the keyboard so that hotkeys can be used

            // sizing:
            poolBallRadius = 20;
            pocketRadius = 72;
            tablePocketSpacing = 16;


            // objects:

            cueBallTexture = Content.Load<Texture2D>("CueBall_transparent_40x40");
            _cueBall = new CueBall(cueBallTexture, poolBallRadius);

            eightBallTexture = cueBallTexture;
            _eightBall = new ObjectBall(cueBallTexture, poolBallRadius);

            float sqrt3 = (float)Math.Sqrt(3);
            const int spacing = 3; // to avoid frame-1 collisions

            solidObjectBallTexture = Content.Load<Texture2D>("ObjectBall_solid_transparent_40x40");
            stripedObjectBallTexture = Content.Load<Texture2D>("ObjectBall_striped_transparent_40x40");

            // (derivation for exact positions in writeup):

            // first column:
            _solidObjectBall1 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X - (2 * poolBallRadius * sqrt3) - (2 * spacing), _eightBall.position.Y), poolBallRadius, false);

            // second column:
            _solidObjectBall2 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X - (poolBallRadius * sqrt3) - spacing, _eightBall.position.Y + poolBallRadius + spacing), poolBallRadius, false);
            _stripedObjectBall1 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X - (poolBallRadius * sqrt3) - spacing, _eightBall.position.Y - poolBallRadius - spacing), poolBallRadius, false);
            
            // third column:
            _stripedObjectBall2 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X, _eightBall.position.Y + (2 * poolBallRadius) + spacing), poolBallRadius, false);
            // (eight ball goes here)
            _solidObjectBall3 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X, _eightBall.position.Y - (2 * poolBallRadius) - spacing), poolBallRadius, false);

            // fourth column:
            _solidObjectBall4 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt3) + spacing, _eightBall.position.Y + (3 * poolBallRadius) + (2 * spacing)), poolBallRadius, false);
            _stripedObjectBall3 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt3) + spacing, _eightBall.position.Y + poolBallRadius + spacing), poolBallRadius, false);
            _solidObjectBall5 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt3) + spacing, _eightBall.position.Y - poolBallRadius - spacing), poolBallRadius, false);
            _stripedObjectBall4 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt3) + spacing, _eightBall.position.Y - (3 * poolBallRadius) - (2 * spacing)), poolBallRadius, false);

            // fith column:
            _stripedObjectBall5 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y + (4 * poolBallRadius) + (2 * spacing)), poolBallRadius, false);
            _solidObjectBall6 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y + (2 * poolBallRadius) + spacing), poolBallRadius, false);
            _stripedObjectBall6 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y), poolBallRadius, false);
            _stripedObjectBall7 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y - (2 * poolBallRadius) - spacing), poolBallRadius, false);
            _solidObjectBall7 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y - (4 * poolBallRadius) - (2 * spacing)), poolBallRadius, false);

            poolBalls = new PoolBall[] { _cueBall, _eightBall, 
                                         _solidObjectBall1, _solidObjectBall2, _solidObjectBall3, _solidObjectBall4, _solidObjectBall5, _solidObjectBall6, _solidObjectBall7,
                                         _stripedObjectBall1, _stripedObjectBall2, _stripedObjectBall3, _stripedObjectBall4, _stripedObjectBall5, _stripedObjectBall6, _stripedObjectBall7};

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            // exit with 'Esc':
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                Exit();
            }

            // toggle fullscreen with 'F':
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                if (!previousKeyboardState.IsKeyDown(Keys.F))
                {
                    _graphics.IsFullScreen = !_graphics.IsFullScreen;
                    _graphics.ApplyChanges();
                }
            }
            previousKeyboardState = Keyboard.GetState(); // re-assign for the next Update()


            // updating objects:

            DoAllPoolBallPoolBallCollisions();

            _cueBall.Update(gameTime);
            _eightBall.Update(gameTime);

            _solidObjectBall1.Update(gameTime);
            _solidObjectBall2.Update(gameTime);
            _solidObjectBall3.Update(gameTime);
            _solidObjectBall4.Update(gameTime);
            _solidObjectBall5.Update(gameTime);
            _solidObjectBall6.Update(gameTime);
            _solidObjectBall7.Update(gameTime);

            _stripedObjectBall1.Update(gameTime);
            _stripedObjectBall2.Update(gameTime);
            _stripedObjectBall3.Update(gameTime);
            _stripedObjectBall4.Update(gameTime);
            _stripedObjectBall5.Update(gameTime);
            _stripedObjectBall6.Update(gameTime);
            _stripedObjectBall7.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(15,58,43)); // background colour, pool-table green

            // main sprite batch:

            _spriteBatch.Begin();

            _cueBall.Draw(_spriteBatch);

            _eightBall.Draw(_spriteBatch);

            _solidObjectBall1.Draw(_spriteBatch);
            _solidObjectBall2.Draw(_spriteBatch);
            _solidObjectBall3.Draw(_spriteBatch);
            _solidObjectBall4.Draw(_spriteBatch);
            _solidObjectBall5.Draw(_spriteBatch);
            _solidObjectBall6.Draw(_spriteBatch);
            _solidObjectBall7.Draw(_spriteBatch);

            _stripedObjectBall1.Draw(_spriteBatch);
            _stripedObjectBall2.Draw(_spriteBatch);
            _stripedObjectBall3.Draw(_spriteBatch);
            _stripedObjectBall4.Draw(_spriteBatch);
            _stripedObjectBall5.Draw(_spriteBatch);
            _stripedObjectBall6.Draw(_spriteBatch);
            _stripedObjectBall7.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
