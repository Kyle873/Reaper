using DPII;

using Microsoft.Xna.Framework.Input;

namespace Reaper.Utils
{
    public static class Input
    {
        public static bool KeyDown(Keys key)
            => Players.Stats.currentKeyboard.IsKeyDown(key);

        public static bool KeyPressed(Keys key)
            => Players.Stats.currentKeyboard.IsKeyDown(key) && Players.Stats.previousKeyboard.IsKeyUp(key);
    }
}
