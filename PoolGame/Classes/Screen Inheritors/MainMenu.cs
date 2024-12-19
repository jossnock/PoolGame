using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.Graphics2D.Brushes;

namespace PoolGame.Classes
{
    /// <summary>
    /// The screen that sets up everything for the main menu and renders the user interface.
    /// </summary>
    public class MainMenu : Screen
    {
        public static Desktop desktop;
        public VerticalStackPanel panelStack;

        public Color textColor;

        public MainMenu() 
        {
            backgroundColor = Color.Gray;
        }

        public override void LoadInitialContent(GraphicsDevice graphicsDevice)
        {
            // setting up the UI: [work in progress]

            textColor = Color.Black;
            IBrush widgetBackground = new SolidBrush(Color.White);

            // group of Widgets, on top of each other, positioned in the centre of the screen:
            panelStack = new VerticalStackPanel
            {
                Spacing = 10,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };


            // Widgets:

            var screenTitle = new Label
            {
                Text = "Main Menu",
                TextColor = textColor,
                Width = Game1.windowWidth / 10,
                Height = Game1.windowHeight / 20,
                TextAlign = (FontStashSharp.RichText.TextHorizontalAlignment)HorizontalAlignment.Center,
                Background = widgetBackground,
                Padding = new Thickness(10),
                BorderThickness = new Thickness(2),

            };
            panelStack.Widgets.Add(screenTitle);

            var mainMenubutton = new Button
            {
                Content = new Label
                {
                    Text = "Start Match",
                    TextColor = textColor,
                    Width = Game1.windowWidth / 10,
                    Height = Game1.windowHeight / 20,
                    TextAlign = (FontStashSharp.RichText.TextHorizontalAlignment)HorizontalAlignment.Center,
                    Background = widgetBackground,
                    Padding = new Thickness(10),
                    BorderThickness = new Thickness(2),
                }
            };
            void OnMainMenuButtonClick(object sender, EventArgs args) // event handler; note: the documentation uses a lambda expression, but I think this is clearer
            {
                Game1._screenState = Game1.ScreenState.Match;

                var match = new Match(); // since Match.LoadInitialContent() isn't static, an instance is needed
                match.LoadInitialContent(graphicsDevice);
            }
            mainMenubutton.Click += OnMainMenuButtonClick; // attaching the event handler to the Click event
            panelStack.Widgets.Add(mainMenubutton);


            var exitButton = new Button
            {
                Content = new Label
                {
                    Text = "Exit to Desktop",
                    TextColor = textColor,
                    Width = Game1.windowWidth / 10,
                    Height = Game1.windowHeight / 20,
                    TextAlign = (FontStashSharp.RichText.TextHorizontalAlignment)HorizontalAlignment.Center,
                    Background = widgetBackground,
                    Padding = new Thickness(10),
                    BorderThickness = new Thickness(2),
                }
            };
            void OnExitButtonClick(object sender, EventArgs args) // event handler; note: the documentation uses a lambda expression, but I think this is clearer
            {
                Game1.instance.Exit();
            }
            exitButton.Click += OnExitButtonClick; // attaching the event handler to the Click event
            panelStack.Widgets.Add(exitButton);



            // Adding it to desktop so it can be rendered by Draw()
            desktop = new Desktop();
            desktop.Root = panelStack;

        }

        public override void Update(GraphicsDevice graphicsDevice, GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            Game1.instance.GraphicsDevice.Clear(backgroundColor); // background colour

            desktop.Render(); // renders Myra UI elements
        }
    }
}
