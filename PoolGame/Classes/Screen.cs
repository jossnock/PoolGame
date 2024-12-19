using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PoolGame.Classes
{
    /// <summary>
    /// The superclass for all in-game screens, managed by an instance of <see cref="Game1.ScreenState"./>
    /// </summary>
    public abstract class Screen
    {
        public Color backgroundColor;

        /// <summary>
        /// Loads the initial content for the current screen (determined by <see cref="Game1.ScreenState"/>).
        /// </summary>
        /// <remarks>All overrides are called by <see cref="Game1.LoadContent"/> (1 time on startup) and whenever the screen state.</remarks>
        public abstract void LoadInitialContent(GraphicsDevice graphicsDevice);

        /// <summary>
        /// Updates the game by (when specified) recieving input, altering attributes, and calling methods.
        /// </summary>
        /// <remarks>Called by <see cref="Game1.Update"/> (1 time every frame) when this is the active screen.</remarks>
        public abstract void Update(GraphicsDevice graphicsDevice, GameTime gameTime);

        /// <summary>
        /// Draws all active sprites.
        /// </summary>
        /// <remarks>Called by <see cref="Game1.Draw"/> (1 time every frame) when this is the active screen.</remarks>
        public abstract void Draw(GameTime gameTime);
    }
}