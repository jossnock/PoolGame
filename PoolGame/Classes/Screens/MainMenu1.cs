//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using System;
//using PoolGame.Classes;
//using PoolGame.Classes.Screens;
//using Myra;
//using Myra.Graphics2D.UI;
//using System.IO;
//using System.Diagnostics;

//namespace PoolGame.Classes.Screens
//{
//    internal class MainMenu1 : Screen
//    {
//        enum ScreenState
//        {
//            MainMenu,
//            Match
//        }

//        ScreenState _screenState;

//        public GraphicsDeviceManager _graphics;
//        public SpriteBatch _spriteBatch;
//        public Desktop _desktop;

//        public static int windowWidth { get; set; }
//        public static int windowHeight { get; set; }

//        private KeyboardState previousKeyboardState;

//        public MainMenu1()
//        {
            
//        }

//        public override void Initialize()
//        {
//            previousKeyboardState = Keyboard.GetState(); // getting the starting state of the keyboard, so that fullscreen can be used
//        }

//        public override void LoadContent()
//        {
//            var _game = new Game();
//            _spriteBatch = new SpriteBatch(_game.GraphicsDevice);


//            // UI Setup:

//            // initialising Myra and loading the MyraMain.xmmp file:
//            MyraEnvironment.Game = this;

//            var grid = new Grid
//            {
//                RowSpacing = 8,
//                ColumnSpacing = 8,
//                ShowGridLines = true,
//            };

//            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
//            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
//            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
//            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));


//            var mainMenuTitle = new Label
//            {
//                Id = "label",
//                Text = "Main Menu"
//            };
//            grid.Widgets.Add(mainMenuTitle);

//            // Button
//            var button = new Button
//            {
//                Content = new Label
//                {
//                    Text = "Start Match"
//                }
//            };
//            Grid.SetColumn(button, 0);
//            Grid.SetRow(button, 1);

//            button.Click += (s, a) =>
//            {
//                Match1 match = new Match1();
//                match.Run();
//            };

//            grid.Widgets.Add(button);

//            // Add it to the desktop
//            _desktop = new Desktop();
//            _desktop.Root = grid;
//        }
        
//        public override void Update(GameTime gameTime)
//        {
//            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
//            {
//                // [placeholder, put exiting logice here]
//            }

//            // toggle fullscreen with 'F':
//            if (Keyboard.GetState().IsKeyDown(Keys.F))
//            {
//                if (!previousKeyboardState.IsKeyDown(Keys.F))
//                {
//                    _graphics.IsFullScreen = !_graphics.IsFullScreen;
//                    _graphics.ApplyChanges();
//                }
//            }


//            previousKeyboardState = Keyboard.GetState(); // re-assign for the next Update()
//        }

//        public override void Draw(GameTime gameTime)
//        {
//            GraphicsDevice.Clear(Color.Gray); // background colour

//            _desktop.Render(); // renders Myra UI elements
//        }
//    }
//}
