using DataStats;

using DPII;
using DPII.InGame;
using DPII.Networking;

using HarmonyLib;

using Microsoft.Xna.Framework;

using Reaper.Utils;

namespace Reaper.Patches
{
    [HarmonyPatch(typeof(Game1), "Update", typeof(GameTime))]
    internal class AutoLoot
    {
        public static void Postfix()
        {
            if (!Get.InGameplay || !Reaper.AutoLootActive.Value)
                return;

            Character character = Characters.characters[Characters.getLowestLocalPlayer()];

            LootCoins(character);
            LootItems(character);
        }

        static void LootCoins(Character character)
        {
            foreach (Coin coin in CurrentEnvironment.environment.coins)
                if (NetVerk.state == OnlineState.offline || NetVerk.state == OnlineState.server)
                {
                    if (NetVerk.state == OnlineState.server)
                        OnlineMessages.Add(OnlineMessageType.coinTaken, (byte)character.playerNo, (short)coin.uniqueID);

                    int value = coin.collect(character);

                    if (value > 0)
                    {
                        if (character.skillsAndTalents.hasTalent("yuppie"))
                            character.giveExperiance((int)MathHelper.Clamp(value / 10f, 1f, 1E+17f));

                        character.adjustMoney(value, true);
                    }
                }
                else
                {
                    OnlineMessages.Add(OnlineMessageType.coinTaken, (byte)character.playerNo, (short)coin.uniqueID);
                }
        }

        static void LootItems(Character character)
        {
            InEnviromentItem item = null;

            foreach (InEnviromentItem itemIter in CurrentEnvironment.environment.items)
                if (!character.inventory.full(itemIter.id))
                {
                    ItemStats itemStats = Stats.GetItemStats(itemIter.id);

                    if (itemStats.type == ItemStats.ItemType.useableItem && itemStats.instantUse && !character.canUseItem(itemIter.id))
                        Audio.playSound("selectx");
                    else
                        item = itemIter;
                }

            if (item == null)
                return;

            if (NetVerk.state == OnlineState.offline || NetVerk.state == OnlineState.server)
            {
                if (NetVerk.state == OnlineState.server)
                    OnlineMessages.Add(OnlineMessageType.itemTaken, (byte)character.playerNo, (short)item.uniqueID);

                character.pickupItem(item);
            }
            else
            {
                OnlineMessages.Add(OnlineMessageType.itemTaken, (byte)character.playerNo, item);
            }

            character.showEncumbranceCounter = 180;
        }
    }
}
