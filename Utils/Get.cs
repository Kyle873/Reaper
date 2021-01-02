using DPII;
using DPII.InGame;

namespace Reaper.Utils
{
    public static class Get
    {
        public static int PlayerNumber = Characters.getLowestLocalPlayer();

        public static   bool      InGameplay => Game1.CurrentState is Gameplay && Character.inGameMenu == null;
        internal static Character Character  => Characters.characters[PlayerNumber];
    }
}
