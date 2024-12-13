using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;

namespace PoolGame.Classes
{
    internal class MainMenu : Screen
    {
        public static Grid grid;


        public MainMenu() { }

        public static void LoadInitialContent(GraphicsDevice graphicsDevice)
        {
            grid = new Grid
            {
                RowSpacing = 8,
                ColumnSpacing = 8,
                ShowGridLines = true,
            };

            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));


            var mainMenuTitle = new Label
            {
                Id = "label",
                Text = "Main Menu"
            };
            grid.Widgets.Add(mainMenuTitle);

            // Button
            var button = new Button
            {
                Content = new Label
                {
                    Text = "Start Match"
                }
            };
            Grid.SetColumn(button, 0);
            Grid.SetRow(button, 1);

            button.Click += (s, a) =>
            {
                Game1._screenState = Game1.ScreenState.Match;
            };

            grid.Widgets.Add(button);
        }

        public override void Update(GraphicsDevice graphicsDevice, GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            Game1._mainMenuDesktop.Render(); // renders Myra UI elements
        }
    }
}
