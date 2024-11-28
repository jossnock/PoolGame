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
        //private GraphicsDeviceManager _graphics;
        //private SpriteBatch _spriteBatch;

        //private KeyboardState previousKeyboardState;

        //// sizing:
        //// using the approx. scale 1cm : 8px
        //public static int poolBallRadius;
        //public static int pocketRadius;
        //public static int tablePocketSpacing;


        //// pool balls:

        //public CueBall _cueBall;
        //public Texture2D cueBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        //public ObjectBall _eightBall;
        //public Texture2D eightBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        //public ObjectBall _solidObjectBall1;
        //public ObjectBall _solidObjectBall2;
        //public ObjectBall _solidObjectBall3;
        //public ObjectBall _solidObjectBall4;
        //public ObjectBall _solidObjectBall5;
        //public ObjectBall _solidObjectBall6;
        //public ObjectBall _solidObjectBall7;
        //public Texture2D solidObjectBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        //public ObjectBall _stripedObjectBall1;
        //public ObjectBall _stripedObjectBall2;
        //public ObjectBall _stripedObjectBall3;
        //public ObjectBall _stripedObjectBall4;
        //public ObjectBall _stripedObjectBall5;
        //public ObjectBall _stripedObjectBall6;
        //public ObjectBall _stripedObjectBall7;
        //public Texture2D stripedObjectBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        //public static List<PoolBall> poolBalls; // List (not array) so that PoolBalls can be removed


        //// pockets:
        //public Pocket _pocketTopLeft;
        //public Pocket _pocketTopMiddle;
        //public Pocket _pocketTopRight;
        //public Pocket _pocketBottomLeft;
        //public Pocket _pocketBottomMiddle;
        //public Pocket _pocketBottomRight;
        //public Texture2D pocketTexture;
        //public static Pocket[] pockets; // Array (not list) because Pockets don't need to be removed

        //public Match1()
        //{
        //    _graphics = new GraphicsDeviceManager(this);
        //    Content.RootDirectory = "Content";
        //    IsMouseVisible = true;

        //    _graphics.PreferredBackBufferWidth = MainMenu.windowWidth; // default window width
        //    _graphics.PreferredBackBufferHeight = MainMenu.windowHeight; // default window height
        //    _graphics.ApplyChanges();
        //}

        public static void DoAllPoolBallPoolBallCollisions() // in Match1.cs rather than PoolBall.cs so that all velocities can be changed on the same frame
        {
            for (int i = 0; i < MainMenu.poolBalls.Count - 1; i++)
            {
                for (int j = i + 1; j < MainMenu.poolBalls.Count; j++)
                {
                    if (MainMenu.poolBalls[i] == MainMenu.poolBalls[j]) // no need to check if it collides with itself
                    { continue; }
                    else
                    {
                        if (Vector2.Distance(MainMenu.poolBalls[i].position, MainMenu.poolBalls[j].position) <= MainMenu.poolBallRadius * 2)
                        {
                            Vector2 relativePositionVector = MainMenu.poolBalls[i].position - MainMenu.poolBalls[j].position; // the vector that is normal to the collision surface (aka other ball), pointing towards poolBalls[i]
                            Vector2 unitNormalVector = relativePositionVector / relativePositionVector.Length(); // normalised to have a magnitude of 1

                            Vector2 newVelocity = MainMenu.poolBalls[i].velocity + (Vector2.Dot(MainMenu.poolBalls[j].velocity - MainMenu.poolBalls[i].velocity, unitNormalVector) * unitNormalVector);
                            Vector2 newOtherVelocity = MainMenu.poolBalls[j].velocity + (Vector2.Dot(MainMenu.poolBalls[i].velocity - MainMenu.poolBalls[j].velocity, unitNormalVector) * unitNormalVector);

                            MainMenu.poolBalls[j].velocity = newOtherVelocity;
                            MainMenu.poolBalls[i].velocity = newVelocity;
                        }
                    }
                }
            }
        }

        public static bool IsAllStationary() // to prevent CueBall from being shot again when things are moving
        {
            foreach (PoolBall poolBall in MainMenu.poolBalls)
            {
                if (poolBall.velocity != Vector2.Zero)
                {
                    return false;
                }
            }
            return true;
        }

        //protected override void Initialize()
        //{
        //    previousKeyboardState = Keyboard.GetState(); // getting the starting state of the keyboard so that hotkeys can be used

        //    // sizing:
        //    poolBallRadius = 20;
        //    pocketRadius = 36;
        //    tablePocketSpacing = 16;


        //    // objects:

        //    // PoolBalls:

        //    cueBallTexture = Content.Load<Texture2D>("CueBall_transparent_40x40");
        //    _cueBall = new CueBall(cueBallTexture, poolBallRadius);

        //    eightBallTexture = cueBallTexture;
        //    _eightBall = new ObjectBall(cueBallTexture, poolBallRadius);

        //    float sqrt3 = (float)Math.Sqrt(3); // to not need to repeatedly use Math.Sqrt(3)
        //    const int spacing = 3; // to avoid frame-1 collisions

        //    solidObjectBallTexture = Content.Load<Texture2D>("ObjectBall_solid_transparent_40x40");
        //    stripedObjectBallTexture = Content.Load<Texture2D>("ObjectBall_striped_transparent_40x40");

        //    // (derivation for exact positions in writeup):

        //    // first column:
        //    _solidObjectBall1 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X - (2 * poolBallRadius * sqrt3) - (2 * spacing), _eightBall.position.Y), poolBallRadius, false);

        //    // second column:
        //    _solidObjectBall2 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X - (poolBallRadius * sqrt3) - spacing, _eightBall.position.Y + poolBallRadius + spacing), poolBallRadius, false);
        //    _stripedObjectBall1 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X - (poolBallRadius * sqrt3) - spacing, _eightBall.position.Y - poolBallRadius - spacing), poolBallRadius, false);
            
        //    // third column:
        //    _stripedObjectBall2 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X, _eightBall.position.Y + (2 * poolBallRadius) + spacing), poolBallRadius, false);
        //    // (eight ball goes here)
        //    _solidObjectBall3 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X, _eightBall.position.Y - (2 * poolBallRadius) - spacing), poolBallRadius, false);

        //    // fourth column:
        //    _solidObjectBall4 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt3) + spacing, _eightBall.position.Y + (3 * poolBallRadius) + (2 * spacing)), poolBallRadius, false);
        //    _stripedObjectBall3 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt3) + spacing, _eightBall.position.Y + poolBallRadius + spacing), poolBallRadius, false);
        //    _solidObjectBall5 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt3) + spacing, _eightBall.position.Y - poolBallRadius - spacing), poolBallRadius, false);
        //    _stripedObjectBall4 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt3) + spacing, _eightBall.position.Y - (3 * poolBallRadius) - (2 * spacing)), poolBallRadius, false);

        //    // fith column:
        //    _stripedObjectBall5 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y + (4 * poolBallRadius) + (2 * spacing)), poolBallRadius, false);
        //    _solidObjectBall6 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y + (2 * poolBallRadius) + spacing), poolBallRadius, false);
        //    _stripedObjectBall6 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y), poolBallRadius, false);
        //    _stripedObjectBall7 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y - (2 * poolBallRadius) - spacing), poolBallRadius, false);
        //    _solidObjectBall7 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y - (4 * poolBallRadius) - (2 * spacing)), poolBallRadius, false);

        //    poolBalls = new List<PoolBall> { _cueBall, _eightBall, 
        //                                 _solidObjectBall1, _solidObjectBall2, _solidObjectBall3, _solidObjectBall4, _solidObjectBall5, _solidObjectBall6, _solidObjectBall7,
        //                                 _stripedObjectBall1, _stripedObjectBall2, _stripedObjectBall3, _stripedObjectBall4, _stripedObjectBall5, _stripedObjectBall6, _stripedObjectBall7};


        //    // Pockets:

        //    //pocketRadius
        //    //tablePocketSpacing

        //    pocketTexture = Content.Load<Texture2D>("CueBall_transparent_72x72"); // [placeholder]
        //    _pocketTopLeft = new Pocket(pocketTexture, new Vector2(pocketRadius + tablePocketSpacing, pocketRadius + tablePocketSpacing), pocketRadius);
        //    _pocketTopMiddle = new Pocket(pocketTexture, new Vector2(MainMenu.windowWidth / 2, pocketRadius + tablePocketSpacing), pocketRadius);
        //    _pocketTopRight = new Pocket(pocketTexture, new Vector2(MainMenu.windowWidth - pocketRadius - tablePocketSpacing, pocketRadius + tablePocketSpacing), pocketRadius);
        //    _pocketBottomLeft = new Pocket(pocketTexture, new Vector2(pocketRadius + tablePocketSpacing, MainMenu.windowHeight - pocketRadius - tablePocketSpacing), pocketRadius);
        //    _pocketBottomMiddle = new Pocket(pocketTexture, new Vector2(MainMenu.windowWidth / 2, MainMenu.windowHeight - pocketRadius - tablePocketSpacing), pocketRadius);
        //    _pocketBottomRight = new Pocket(pocketTexture, new Vector2(MainMenu.windowWidth - pocketRadius - tablePocketSpacing, MainMenu.windowHeight - pocketRadius - tablePocketSpacing), pocketRadius);

        //    pockets = new Pocket[6] { _pocketTopLeft,    _pocketTopMiddle,    _pocketTopRight,
        //                              _pocketBottomLeft, _pocketBottomMiddle, _pocketBottomRight };

        //    base.Initialize();
        //}

        //protected override void LoadContent()
        //{
        //    _spriteBatch = new SpriteBatch(GraphicsDevice);
        //}

        //protected override void Update(GameTime gameTime)
        //{
        //    // exit with 'Esc':
        //    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
        //    {
        //        Exit();
        //    }

        //    // toggle fullscreen with 'F':
        //    if (Keyboard.GetState().IsKeyDown(Keys.F))
        //    {
        //        if (!previousKeyboardState.IsKeyDown(Keys.F))
        //        {
        //            _graphics.IsFullScreen = !_graphics.IsFullScreen;
        //            _graphics.ApplyChanges();
        //        }
        //    }
        //    previousKeyboardState = Keyboard.GetState(); // re-assign for the next Update()


        //    // updating objects:

        //    // Pocket-PoolBall collisions before PoolBall-PoolBall collisions because deleting is less intensive than calculating all collisions
        //    // and, if a PoolBall is deleted, less collision work needs to be done
        //    foreach (Pocket _pocket in pockets)
        //    {
        //        _pocket.Update(gameTime);
        //    }

        //    DoAllPoolBallPoolBallCollisions();

        //    // foreach means that PoolBalls removed from the array aren't updated:
        //    foreach (PoolBall _poolBall in poolBalls)
        //    {
        //        _poolBall.Update(gameTime);
        //    }
        //}

        //protected override void Draw(GameTime gameTime)
        //{
        //    GraphicsDevice.Clear(new Color(15,58,43)); // background colour, pool-table green

        //    // main sprite batch:

        //    _spriteBatch.Begin();

        //    foreach (Pocket _pocket in pockets)
        //    {
        //        _pocket.Draw(_spriteBatch);
        //    }

        //    // foreach means that PoolBalls removed from the array aren't drawn
        //    foreach (PoolBall _poolBall in poolBalls)
        //    {
        //        _poolBall.Draw(_spriteBatch);
        //    }

        //    _spriteBatch.End();

        //    base.Draw(gameTime);
        //}
    }
}
