namespace Offsets
{
    public struct UnityList
    {
        public const uint Base = 0x10; // to UnityListBase
        public const uint Count = 0x18; // int32
    }
    public struct UnityListBase
    {
        public const uint Start = 0x20; // start of list +(i * 0x8)
    }
    public struct UnityString
    {
        public const uint Length = 0x10; // int32
        public const uint Value = 0x14; // string,unicode
    }
    public struct ModuleBase
    {
        public const uint GameObjectManager = 0x17F8D28; // to eft_dma_radar.GameObjectManager
    }
    public struct GameObject
    {
        public static readonly uint[] To_TransformInternal = new uint[] { 0x10, 0x30, 0x30, 0x8, 0x28, 0x10 }; // to TransformInternal
        public const uint ObjectClass = 0x30;
        public const uint ObjectName = 0x60; // string,default (null terminated)
    }
    public struct GameWorld
    {
        public static readonly uint[] To_LocalGameWorld = new uint[] { GameObject.ObjectClass, 0x18, 0x28 };
    }
    public struct LocalGameWorld
    {
        public const uint ExfilController = 0x18; // to ExfilController
        public const uint LootList = 0x60; // to UnityList
        public const uint RegisteredPlayers = 0x80; // to RegisteredPlayers
        public const uint Grenades = 0xF0; // to Grenades
    }
    public struct ExfilController
    {
        public const uint ExfilCount = 0x18; // int32
        public const uint ExfilList = 0x20; // to UnityListBase
    }
    public struct Exfil
    {
        public const uint Status = 0xA8; // int32
    }
    public struct Grenades
    {
        public const uint List = 0x18; // to UnityList
    }
    public struct Player
    {
        public static readonly uint[] To_TransformInternal = new uint[] { 0xA8, 0x28, 0x28, Offsets.UnityList.Base, Offsets.UnityListBase.Start + 0x0, 0x10 }; // to TransformInternal
        public const uint MovementContext = 0x40; // to MovementContext
        public const uint Corpse = 0x318; // ulong (0 if alive)
        public const uint Profile = 0x4C0; // to Profile
        public const uint HealthController = 0x4F8; // to HealthController
        public const uint InventoryController = 0x508; // to InventoryController
        public const uint IsLocalPlayer = 0x7FF; // bool
    }
    public struct Profile
    {
        public const uint PlayerID = 0x10; // unity string
        public const uint PlayerInfo = 0x28; // to PlayerInfo
    }
    public struct PlayerInfo
    {
        public const uint PlayerName = 0x10; // unity string
        public const uint GroupID = 0x18; // ptr to UnityString (0/null if solo or bot)
        public const uint Settings = 0x38; // to PlayerSettings
        public const uint Experience = 0x74; // int32
        public const uint PlayerSide = 0x58; // int32
        public const uint RegDate = 0x5C; // int32
    }
    public struct PlayerSettings
    {
        public const uint Role = 0x10; // int32
    }
    public struct MovementContext
    {
        public const uint Rotation = 0x22C; // vector2
    }
    public struct InventoryController
    {
        public const uint Inventory = 0x118; // to Inventory
    }
    public struct Inventory
    {
        public const uint Equipment = 0x10; // to Equipment
    }
    public struct Equipment
    {
        public const uint Slots = 0x78; // to UnityList
    }
    public struct Slot
    {
        public const uint Name = 0x10; // string,unity
        public const uint ContainedItem = 0x38; // to LootItemBase
    }
    public struct HealthController
    {
        public static readonly uint[] To_HealthEntries = { 0x50 , 0x18}; // to HealthEntries
    }
    public struct HealthEntries
    {
        public const uint HealthEntries_Start = 0x30; // Each body part 0x18 , to HealthEntry
    }
    public struct HealthEntry
    {
        public const uint Value = 0x10; // to HealthValue
    }
    public struct HealthValue
    {
        public const uint Current = 0x0; // float
        public const uint Maximum = 0x4; // float
        public const uint Minimum = 0x8; // float
    }
    public struct LootListItem
    {
        public const uint LootUnknownPtr = 0x10; // to LootUnknownPtr
    }

    public struct LootUnknownPtr
    {
        public const uint LootInteractiveClass = 0x28; // to LootInteractiveClass
    }
    public struct LootInteractiveClass
    {
        public const uint LootBaseObject = 0x10; // to LootBaseObject
        public const uint LootItemBase = 0x50; // to LootItemBase
        public const uint ContainerItemOwner = 0x108; // to ContainerItemOwner
    }
    public struct LootItemBase //EFT.InventoryLogic.Item
    {
        public const uint ItemTemplate = 0x40; // to ItemTemplate
        public const uint Grids = 0x68; // to Grids
    }
    public struct ItemTemplate //EFT.InventoryLogic.ItemTemplate
    {
        public const uint BsgId = 0x50; // string,unity
        public const uint IsQuestItem = 0x94; // bool
    }
    public struct LootBaseObject
    {
        public const uint GameObject = 0x30; // to GameObject
    }
    public struct LootGameObjectClass
    {
        public static readonly uint[] To_TransformInternal = new uint[] { 0x8, 0x28, 0x10 };
    }
    public struct ContainerItemOwner
    {
        public const uint LootItemBase = 0xA0; // to LootItemBase
    }
    public struct Grids
    {
        public const uint GridsEnumerableClass = 0x40;
    }
    public struct TransformInternal
    {
        public const uint Hierarchy = 0x38; // to TransformHierarchy
        public const uint HierarchyIndex = 0x40; // int32
    }
    public struct TransformHierarchy
    {
        public const uint Vertices = 0x18; // List<Vector128<float>>
        public const uint Indices = 0x20; // List<int>
    }
}