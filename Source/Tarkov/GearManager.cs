using System;
using System.Collections.ObjectModel;

namespace eft_dma_radar
{
    public class GearManager
    {
        private static readonly List<string> _skipSlots = new()
        {
            "Scabbard", "SecuredContainer", "Dogtag", "Compass", "Eyewear", "ArmBand"
        };
        /// <summary>
        /// List of equipped items in PMC Inventory Slots.
        /// </summary>
        public ReadOnlyDictionary<string, string> Gear { get; }

        public GearManager(ulong playerBase)
        {
            var inventorycontroller = Memory.ReadPtr(playerBase + Offsets.Player.InventoryController);
            var inventory = Memory.ReadPtr(inventorycontroller + Offsets.InventoryController.Inventory);
            var equipment = Memory.ReadPtr(inventory + Offsets.Inventory.Equipment);
            var slots = Memory.ReadPtr(equipment + Offsets.Equipment.Slots);
            var size = Memory.ReadValue<int>(slots + Offsets.UnityList.Count);
            var slotDict = new Dictionary<string, ulong>();

            for (int slotID = 0; slotID < size; slotID++)
            {
                var slotPtr = Memory.ReadPtr(slots + Offsets.UnityListBase.Start + (uint)slotID * 0x8);
                var namePtr = Memory.ReadPtr(slotPtr + Offsets.Slot.Name);
                var name = Memory.ReadUnityString(namePtr);
                if (_skipSlots.Contains(name, StringComparer.OrdinalIgnoreCase)) continue;
                slotDict.TryAdd(name, slotPtr);
            }
            var gearDict = new Dictionary<string, string>();
            foreach (var slotName in slotDict.Keys)
            {
                if (slotDict.TryGetValue(slotName, out var slot))
                {
                    try
                    {
                        var containedItem = Memory.ReadPtr(slot + Offsets.Slot.ContainedItem);
                        var inventorytemplate = Memory.ReadPtr(containedItem + Offsets.LootItemBase.ItemTemplate);
                        var idPtr = Memory.ReadPtr(inventorytemplate + Offsets.ItemTemplate.BsgId);
                        var id = Memory.ReadUnityString(idPtr);
                        var itemEntry = TarkovMarketManager.AllLoot.FirstOrDefault(x => id == x.bsgId);
                        if (itemEntry is not null)
                        {
                            gearDict.TryAdd(slotName, itemEntry.name);
                        }
                    }
                    catch { } // Skip over empty slots
                }
            }
            Gear = new(gearDict); // update readonly ref
        }
    }
}
