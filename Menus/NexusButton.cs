using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace NexusModsButton.Menus
{
    internal class NexusButton
    {
        private readonly ClickableTextureComponent nexusButton;
        private bool wasUpdateButtonHovered;

        internal bool ShowUpdateButton { get; set; }

        private Point mousePosition = Point.Zero;

        private IModHelper modHelper = null;

        public NexusButton(IModHelper helper)
        {
            modHelper = helper;
            Texture2D buttonTexture = helper.ModContent.Load<Texture2D>("assets/nexusButton.png");

            this.nexusButton = new ClickableTextureComponent(
                new Rectangle(36, Game1.viewport.Height - getWindowYPos() - 48, 81, 75), buttonTexture, new Rectangle(0, 0, 27, 25),
                3, false);

            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Input.CursorMoved += this.OnCursorMoved;
            helper.Events.Display.WindowResized += this.OnWindowResized;
        }

        internal int getWindowYPos()
        {
            int position = 50;
            // GenericModConfigMenu adds a button in the position we want to use. 
            if (modHelper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
            {
                position = 150;
            }

            // ModUpdateMenu adds a button in the position we use if GMCM is loaded, so move up again.
            if (modHelper.ModRegistry.IsLoaded("cat.modupdatemenu") && modHelper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
            {
                position = 250;
            }

            return position;
        }

        /// <summary>Raised after tha game window is resized.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguements.</param>
        private void OnWindowResized(object sender, WindowResizedEventArgs e)
        {
            this.nexusButton.bounds.Y = Game1.viewport.Height - getWindowYPos() - 48;
        }

        /// <summary>Raised after the game state is updated (60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!this.ShowUpdateButton)
                return;

            if (this.mousePosition != Point.Zero)
                this.nexusButton.tryHover(this.mousePosition.X, this.mousePosition.Y, 0.25f);
        }

        /// <summary>Raised after the player moves the in-game cursor.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCursorMoved(object sender, CursorMovedEventArgs e)
        {
            if (!this.ShowUpdateButton)
                return;

            this.mousePosition = new Point((int)e.NewPosition.ScreenPixels.X, (int)e.NewPosition.ScreenPixels.Y);
            bool isUpdateButtonHovered = this.nexusButton.containsPoint(this.mousePosition.X, this.mousePosition.Y);
            if (isUpdateButtonHovered != this.wasUpdateButtonHovered)
            {
                this.nexusButton.sourceRect.X += this.wasUpdateButtonHovered ? -27 : 27;
                if (!this.wasUpdateButtonHovered)
                    Game1.playSound("Cowboy_Footstep");
            }
            this.wasUpdateButtonHovered = isUpdateButtonHovered;
        }

        internal bool PointContainsButton(Vector2 p)
        {
            return this.nexusButton.containsPoint((int)p.X, (int)p.Y);
        }

        public void Draw(SpriteBatch b)
        {
            if (!this.ShowUpdateButton)
                return;
            this.nexusButton.draw(Game1.spriteBatch);

            if (Game1.activeClickableMenu is TitleMenu titleMenu)
                titleMenu.drawMouse(b);
        }

    }
}
