using System;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Runtime.InteropServices;

namespace eft_dma_radar
{
    public class LootManager
    {
        /// <summary>
        /// All tracked loot/corpses in Local Game World.
        /// </summary>
        public ReadOnlyCollection<LootItem> Loot { get; }

        #region Constructor
        public LootManager(ulong localGameWorld)
        {
            var loot = new List<LootItem>();
            var lootlistPtr = Memory.ReadPtr(localGameWorld + Offsets.LocalGameWorld.LootList);
            var lootListEntity = Memory.ReadPtr(lootlistPtr + Offsets.UnityList.Base);
            var countLootListObjects = Memory.ReadValue<int>(lootListEntity + Offsets.UnityList.Count);

            Program.Log("Parsing loot...");
            for (int i = 0; i < countLootListObjects; i++)
            {
                try
                {
                    //Get Loot Item
                    var lootObjectsEntity = Memory.ReadPtr(lootListEntity + Offsets.UnityListBase.Start + (ulong)(0x8 * i));
                    var unkownPtr = Memory.ReadPtr(lootObjectsEntity + Offsets.LootListItem.LootUnknownPtr);
                    var interactiveClass = Memory.ReadPtr(unkownPtr + Offsets.LootUnknownPtr.LootInteractiveClass);
                    var baseObject = Memory.ReadPtr(interactiveClass + Offsets.LootInteractiveClass.LootBaseObject);
                    var gameObject = Memory.ReadPtr(baseObject + Offsets.LootBaseObject.GameObject);
                    var pGameObjectName = Memory.ReadPtr(gameObject + Offsets.GameObject.ObjectName);
                    var name = Memory.ReadString(pGameObjectName, 64); //this represents the BSG name, it's not clean text though

                    if (name.Contains("script", StringComparison.OrdinalIgnoreCase))
                    {

                        //skip these. These are scripts which I think are things like landmines but not sure
                    }
                    else if (name.Contains("lootcorpse_playersuperior", StringComparison.OrdinalIgnoreCase))
                    {
                        var objectClass = Memory.ReadPtr(gameObject + Offsets.GameObject.ObjectClass);
                        var transformInternal = Memory.ReadPtrChain(objectClass, Offsets.LootGameObjectClass.To_TransformInternal);
                        var pos = new Transform(transformInternal).GetPosition();
                        loot.Add(new LootItem
                        {
                            Position = pos,
                            Label = "Corpse"
                        });
                    }
                    else
                    {
                        //Get Position
                        var objectClass = Memory.ReadPtr(gameObject + Offsets.GameObject.ObjectClass);
                        var transformInternal = Memory.ReadPtrChain(objectClass, Offsets.LootGameObjectClass.To_TransformInternal);
                        var pos = new Transform(transformInternal).GetPosition();

                        //the WORST method to figure out if an item is a container...but no better solution now
                        bool container = Misc.CONTAINERS.Any(x => name.Contains(x, StringComparison.OrdinalIgnoreCase));

                        //If the item is a Static Container like weapon boxes, barrels, caches, safes, airdrops etc
                        if (container)
                        {
                            //Grid Logic for static containers so that we can see what's inside
                            try
                            {
                                if (name.Contains("container_crete_04_COLLIDER(1)", StringComparison.OrdinalIgnoreCase))
                                {
                                    loot.Add(new LootItem
                                    {
                                        Position = pos,
                                        Label = "!!Airdrop",
                                        Important = true
                                    });
                                    continue;
                                }
                                var itemOwner = Memory.ReadPtr(interactiveClass + Offsets.LootInteractiveClass.ContainerItemOwner);
                                var itemBase = Memory.ReadPtr(itemOwner + Offsets.ContainerItemOwner.LootItemBase);
                                var grids = Memory.ReadPtr(itemBase + Offsets.LootItemBase.Grids);
                                GetItemsInGrid(grids, "ignore", pos, loot);
                            }
                            catch
                            {
                            }
                        }
                        //If the item is NOT a Static Container
                        else
                        {
                            var item = Memory.ReadPtr(interactiveClass + Offsets.LootInteractiveClass.LootItemBase); //EFT.InventoryLogic.Item
                            var itemTemplate = Memory.ReadPtr(item + Offsets.LootItemBase.ItemTemplate); //EFT.InventoryLogic.ItemTemplate
                            bool questItem = Memory.ReadValue<bool>(itemTemplate + Offsets.ItemTemplate.IsQuestItem);

                            //If NOT a quest item. Quest items are like the quest related things you need to find like the pocket watch or Jaeger's Letter etc. We want to ignore these quest items.
                            if (!questItem)
                            {
                                var BSGIdPtr = Memory.ReadPtr(itemTemplate + Offsets.ItemTemplate.BsgId);
                                var id = Memory.ReadUnityString(BSGIdPtr);

                                //If the item is a corpose
                                if (id.Equals("55d7217a4bdc2d86028b456d")) // Corpse
                                {
                                    loot.Add(new LootItem
                                    {
                                        Position = pos,
                                        Label = "Corpse"
                                    });
                                }
                                //Finally we must have found a loose loot item, eg a keycard, backpack, gun, salewa. Anything not in a container or corpse.
                                else
                                {

                                    //Grid Logic for loose loot because some loose loot have items inside, eg a backpack or docs case. We want to check those items too. But not all loose loot have items inside, so we have a try-catch below
                                    try
                                    {
                                        var grids = Memory.ReadPtr(item + Offsets.LootItemBase.Grids);
                                        GetItemsInGrid(grids, id, pos, loot);
                                    }
                                    catch
                                    {
                                        //The loot item we found does not have any grids so it's basically like a keycard or a ledx etc. Therefore add it to our loot dictionary.
                                        if (TarkovMarketManager.ItemFilter.TryGetValue(id, out var filter))
                                        {
                                            loot.Add(new LootItem
                                            {
                                                Label = filter.Label,
                                                Important = filter.Important,
                                                Position = pos
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }
            Loot = new(loot); // update readonly ref
            Program.Log("Loot parsing completed");
        }
        #endregion

        #region Methods
        ///This method recursively searches grids. Grids work as follows:
        ///Take a Groundcache which holds a Blackrock which holds a pistol.
        ///The Groundcache will have 1 grid array, this method searches for whats inside that grid.
        ///Then it finds a Blackrock. This method then invokes itself recursively for the Blackrock.
        ///The Blackrock has 11 grid arrays (not to be confused with slots!! - a grid array contains slots. Look at the blackrock and you'll see it has 20 slots but 11 grids).
        ///In one of those grid arrays is a pistol. This method would recursively search through each item it finds
        ///To Do: add slot logic, so we can recursively search through the pistols slots...maybe it has a high value scope or something.
        private void GetItemsInGrid(ulong gridsArrayPtr, string id, Vector3 pos, List<LootItem> loot)
        {
            var gridsArray = new MemArray(gridsArrayPtr);

            if (TarkovMarketManager.ItemFilter.TryGetValue(id, out var filter))
            {
                loot.Add(new LootItem
                {
                    Label = filter.Label,
                    Important = filter.Important,
                    Position = pos
                });
            }

            // Check all sections of the container
            foreach (var grid in gridsArray.Data)
            {

                var gridEnumerableClass = Memory.ReadPtr(grid + Offsets.Grids.GridsEnumerableClass); // -.GClass178A->gClass1797_0x40 // Offset: 0x0040 (Type: -.GClass1797)

                var itemListPtr = Memory.ReadPtr(gridEnumerableClass + 0x18); // -.GClass1797->list_0x18 // Offset: 0x0018 (Type: System.Collections.Generic.List<Item>)
                var itemList = new MemList(itemListPtr);

                foreach (var childItem in itemList.Data)
                {
                    try
                    {
                        var childItemTemplate = Memory.ReadPtr(childItem + Offsets.LootItemBase.ItemTemplate); // EFT.InventoryLogic.Item->_template // Offset: 0x0038 (Type: EFT.InventoryLogic.ItemTemplate)
                        var childItemIdPtr = Memory.ReadPtr(childItemTemplate + Offsets.ItemTemplate.BsgId);
                        var childItemIdStr = Memory.ReadUnityString(childItemIdPtr).Replace("\\0", "");

                        // Check to see if the child item has children
                        var childGridsArrayPtr = Memory.ReadPtr(childItem + Offsets.LootItemBase.Grids);   // -.GClassXXXX->Grids // Offset: 0x0068 (Type: -.GClass1497[])
                        GetItemsInGrid(childGridsArrayPtr, childItemIdStr, pos, loot);        // Recursively add children to the entity
                    }
                    catch (Exception ee) { }
                }

            }
        }
        #endregion
    }

    #region Classes
    //Helper class or struct
    public class MemArray
    {
        public ulong Address { get; }
        public int Count { get; }
        public ulong[] Data { get; }

        public MemArray(ulong address)
        {
            var type = typeof(ulong);

            Address = address;
            Count = Memory.ReadValue<int>(address + Offsets.UnityList.Count);
            var arrayBase = address + Offsets.UnityListBase.Start;
            var tSize = (uint)Marshal.SizeOf(type);

            // Rudimentary sanity check
            if (Count > 4096 || Count < 0)
                Count = 0;

            var retArray = new ulong[Count];

            for (uint i = 0; i < Count; i++)
            {
                retArray[i] = Memory.ReadPtr(arrayBase + i * tSize);
            }

            Data = retArray;
        }
    }


    //Helper class or struct
    public class MemList
    {
        public ulong Address { get; }

        public int Count { get; }

        public List<ulong> Data { get; }

        public MemList(ulong address)
        {
            var type = typeof(ulong);

            Address = address;
            Count = Memory.ReadValue<int>(address + Offsets.UnityList.Count);

            if (Count > 4096 || Count < 0)
                Count = 0;

            var arrayBase = Memory.ReadPtr(address + Offsets.UnityList.Base) + Offsets.UnityListBase.Start;
            var tSize = (uint)Marshal.SizeOf(type);
            var retList = new List<ulong>(Count);

            for (uint i = 0; i < Count; i++)
            {
                retList.Add(Memory.ReadPtr(arrayBase + i * tSize));
            }

            Data = retList;
        }
    }
    public class LootItem
    {
        public string Label { get; set; }
        public bool Important { get; set; }
        public Vector3 Position { get; set; }

    }
    #endregion
}