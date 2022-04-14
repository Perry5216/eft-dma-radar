using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Numerics;

namespace eft_dma_radar
{
    public class GrenadeManager
    {
        private readonly Stopwatch _sw = new();
        private ulong _grenadeList;
        private ulong? _listBase = null;
        private int GrenadeCount
        {
            get
            {
                var count = Memory.ReadValue<int>(_grenadeList + Offsets.UnityList.Count);
                if (count < 0 || count > 32) throw new ArgumentOutOfRangeException();
                return count;
            }
        }
        /// <summary>
        /// List of "Hot" grenades in Local Game World.
        /// </summary>
        public ReadOnlyCollection<Grenade> Grenades { get; private set; }

        public GrenadeManager(ulong localGameWorld)
        {
            var grenadesPtr = Memory.ReadPtr(localGameWorld + Offsets.LocalGameWorld.Grenades);
            _grenadeList = Memory.ReadPtr(grenadesPtr + Offsets.Grenades.List);
            _sw.Start();
        }

        /// <summary>
        /// Check for "hot" grenades in LocalGameWorld.
        /// </summary>
        public void Refresh()
        {
            try
            {
                if (_sw.ElapsedMilliseconds < 150) return;
                var count = this.GrenadeCount;
                if (count > 0 && _listBase is null) // List addr becomes valid after first grenade(s) thrown
                {
                    _listBase = Memory.ReadPtr(_grenadeList + Offsets.UnityList.Base);
                }
                var list = new List<Grenade>();
                for (uint i = 0; i < count; i++)
                {
                    try
                    {
                        if (_listBase is null) return;
                        var grenadeAddr = Memory.ReadPtr((ulong)_listBase + Offsets.UnityListBase.Start + (i * 0x08));
                        var grenade = new Grenade(grenadeAddr);
                        list.Add(grenade);
                    }
                    catch { }
                }
                Grenades = new(list); // update readonly ref
                _sw.Restart();
            }
            catch { }
        }
    }

    /// <summary>
    /// Represents a 'Hot' grenade in Local Game World.
    /// </summary>
    public class Grenade
    {
        /// <summary>
        /// Unity Position of grenade in Local Game World.
        /// </summary>
        public readonly Vector3 Position = new();
        public Grenade(ulong baseAddr)
        {
            var posAddr = Memory.ReadPtrChain(baseAddr, Offsets.GameObject.To_TransformInternal);
            Position = new Transform(posAddr).GetPosition();
        }
    }
}
