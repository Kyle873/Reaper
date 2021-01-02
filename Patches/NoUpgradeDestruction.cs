using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using DPII.InGame.SubMenu;

using HarmonyLib;

namespace Reaper.Patches
{
    [HarmonyPatch(typeof(UpgradeWeaponSubMenu), nameof(UpgradeWeaponSubMenu.update))]
    public class NoUpgradeDestruction
    {
        /*
            IL_043b: ldarg.0      // this
            IL_043c: ldfld        class DPII.InGame.InGameMenu DPII.InGame.InGameMenu/InGameMenuSubMenu::menu
            IL_0441: ldfld        class DPII.InGame.Character DPII.InGame.InGameMenu::character
            IL_0446: ldfld        class DPII.InGame.TraderNPC DPII.InGame.Character::store
            IL_044b: ldstr        "Sorry. I broke it"
            IL_0450: callvirt     instance void DPII.InGame.TraderNPC::speak(string)
            IL_0455: ldarg.0      // this
            IL_0456: ldfld        class DPII.InGame.InGameMenu DPII.InGame.InGameMenu/InGameMenuSubMenu::menu
            IL_045b: ldfld        class DPII.InGame.Character DPII.InGame.InGameMenu::character
            IL_0460: ldfld        class DPII.InGame.Inventory DPII.InGame.Character::inventory
            IL_0465: ldarg.0      // this
            IL_0466: ldfld        string DPII.InGame.SubMenu.UpgradeWeaponSubMenu::selectedItemID
            IL_046b: ldc.i4.1
            IL_046c: callvirt     instance void DPII.InGame.Inventory::removeItem(string, int32)
            IL_0471: ldarg.0      // this
            IL_0472: ldfld        class DPII.InGame.InGameMenu DPII.InGame.InGameMenu/InGameMenuSubMenu::menu
            IL_0477: ldfld        class DPII.InGame.Character DPII.InGame.InGameMenu::character
            IL_047c: callvirt     instance void DPII.InGame.Character::replaceAndEquipeWeapons()
         */
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction instruction = codes[i];

                if (instruction.opcode == OpCodes.Ldstr && (string)instruction.operand == "Sorry. I broke it")
                {
                    codes.RemoveRange(i + 2, 2 + 10);

                    return codes.AsEnumerable();
                }
            }

            throw new EntryPointNotFoundException("Could not find the necessary Ldstr!");
        }
    }
}
