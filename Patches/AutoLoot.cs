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
            if (!Get.InGameplay || !KMod.AutoLootActive.Value)
                return;

            LootCoins();
            LootItems();
        }

        static void LootCoins()
        {
            foreach (Coin coin in CurrentEnvironment.environment.coins)
                if (NetVerk.state == OnlineState.offline || NetVerk.state == OnlineState.server)
                {
                    if (NetVerk.state == OnlineState.server)
                        OnlineMessages.Add(OnlineMessageType.coinTaken, (byte)Get.Character.playerNo, (short)coin.uniqueID);

                    int value = coin.collect(Get.Character);

                    if (value > 0)
                    {
                        if (Get.Character.skillsAndTalents.hasTalent("yuppie"))
                            Get.Character.giveExperiance((int)MathHelper.Clamp(value / 10f, 1f, 1E+17f));

                        Get.Character.adjustMoney(value, true);
                    }
                }
                else
                {
                    OnlineMessages.Add(OnlineMessageType.coinTaken, (byte)Get.Character.playerNo, (short)coin.uniqueID);
                }
        }

        static void LootItems()
        {
            InEnviromentItem item = null;

            foreach (InEnviromentItem itemIter in CurrentEnvironment.environment.items)
                if (!Get.Character.inventory.full(itemIter.id))
                {
                    ItemStats itemStats = Stats.GetItemStats(itemIter.id);

                    if (itemStats.type == ItemStats.ItemType.useableItem && itemStats.instantUse && !Get.Character.canUseItem(itemIter.id))
                        Audio.playSound("selectx");
                    else
                        item = itemIter;
                }

            if (item == null)
                return;

            if (NetVerk.state == OnlineState.offline || NetVerk.state == OnlineState.server)
            {
                if (NetVerk.state == OnlineState.server)
                    OnlineMessages.Add(OnlineMessageType.itemTaken, (byte)Get.Character.playerNo, (short)item.uniqueID);

                Get.Character.pickupItem(item);
            }
            else
            {
                OnlineMessages.Add(OnlineMessageType.itemTaken, (byte)Get.Character.playerNo, item);
            }

            Get.Character.showEncumbranceCounter = 180;
        }
    }
}
