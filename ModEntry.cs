using NexusModsButton.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Diagnostics;

namespace NexusModsButton
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        /*********
        ** Fields
        *********/
        /// <summary>The update button.</summary>
        private NexusButton button;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {            
            this.button = new NexusButton(helper);

            // helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Display.Rendered += this.OnRendered;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game state is updated</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The events args.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            this.button.ShowUpdateButton = this.ShouldDrawUpdateButton();
        }

        /// <summary>Raised after the game draws to the sprite patch in a draw tick, just before the final sprite batch is rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRendered(object sender, RenderedEventArgs e)
        {
            if (Game1.activeClickableMenu is TitleMenu && this.ShouldDrawUpdateButton())
            {
                this.button.Draw(e.SpriteBatch);
            }
        }


        /// <summary>If the update button should be drawn</summary>
        /// <returns>If the update button should be drawn</returns>
        private bool ShouldDrawUpdateButton()
        {
            if (!(Game1.activeClickableMenu is TitleMenu titleMenu))
            {
                return false;
            }

            bool showButton = TitleMenu.subMenu == null && !this.GetPrivateBool(titleMenu, "isTransitioningButtons") &&
                (this.GetPrivateBool(titleMenu, "titleInPosition") && !this.GetPrivateBool(titleMenu, "transitioningCharacterCreationMenu"));

            return showButton;
        }

        /// <summary>Gets a private boolean using reflection.</summary>
        /// <param name="obj">The object to get the boolean from.</param>
        /// <param name="name">The name of the boolean.</param>
        /// <returns>The boolean value.</returns>
        private bool GetPrivateBool(object obj, string name)
        {
            return this.Helper.Reflection.GetField<bool>(obj, name).GetValue();
        }


        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Check for left mouse button
            if (e.Button != SButton.MouseLeft)
            {
                return;
            }

            if (this.button.PointContainsButton(e.Cursor.ScreenPixels) && Game1.activeClickableMenu is TitleMenu && TitleMenu.subMenu == null)
            {
                // TitleMenu.subMenu = this.menu;
                // this.menu.Activated();
                try
                {
                    var ps = new ProcessStartInfo("https://nexusmods.com/stardewvalley")
                    {
                        UseShellExecute = true,
                        Verb = "open"
                    };
                    Process.Start(ps);
                    Game1.playSound("bigSelect");

                }
                catch
                {
                    Game1.playSound("toyPiano");
                }
            }
        }
    }
}
