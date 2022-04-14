using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Numerics;

namespace eft_dma_radar
{
    public class ExfilManager
    {
        private readonly Stopwatch _sw = new();
        /// <summary>
        /// List of PMC Exfils in Local Game World and their position/status.
        /// </summary>
        public ReadOnlyCollection<Exfil> Exfils { get; }

        public ExfilManager(ulong localGameWorld)
        {
            var exfilController = Memory.ReadPtr(localGameWorld + Offsets.LocalGameWorld.ExfilController);
            var exfilPoints = Memory.ReadPtr(exfilController + Offsets.ExfilController.ExfilList);
            var count = Memory.ReadValue<int>(exfilPoints + Offsets.ExfilController.ExfilCount);
            if (count < 1 || count > 24) throw new ArgumentOutOfRangeException();
            var list = new List<Exfil>();
            for (uint i = 0; i < count; i++)
            {
                var exfilAddr = Memory.ReadPtr(exfilPoints + Offsets.UnityListBase.Start + (i * 0x08));
                var exfil = new Exfil(exfilAddr);
                list.Add(exfil);
            }
            Exfils = new(list); // update readonly ref
            Refresh();
            _sw.Start();
        }

        /// <summary>
        /// Refresh exfils to update status.
        /// </summary>
        public void Refresh()
        {
            try
            {
                if (_sw.IsRunning && _sw.Elapsed.TotalSeconds < 5) return;
                var map = new ScatterReadMap();
                var round1 = map.AddRound();
                for (int i = 0; i < Exfils.Count; i++)
                {
                    round1.AddEntry(i, 0, Exfils[i].BaseAddr + Offsets.Exfil.Status, typeof(int));
                }
                map.Execute(Exfils.Count);
                for (int i = 0; i < Exfils.Count; i++)
                {
                    try
                    {
                        var status = (int)map.Results[i][0].Result;
                        Exfils[i].UpdateStatus(status);
                    }
                    catch { }
                }
            } catch { }
            _sw.Restart();
        }
    }

    #region Classes_Enums
    public class Exfil
    {
        public ulong BaseAddr { get; }
        public Vector3 Position { get; }
        public ExfilStatus Status { get; private set; } = ExfilStatus.Closed;

        public Exfil(ulong baseAddr)
        {
            this.BaseAddr = baseAddr;
            var transform_internal = Memory.ReadPtrChain(baseAddr, Offsets.GameObject.To_TransformInternal);
            Position = new Transform(transform_internal).GetPosition();
        }

        /// <summary>
        /// Update status of exfil.
        /// </summary>
        public void UpdateStatus(int status)
        {
            switch (status)
            {
                case 1: // NotOpen
                    this.Status = ExfilStatus.Closed;
                    break;
                case 2: // IncompleteRequirement
                    this.Status = ExfilStatus.Pending;
                    break;
                case 3: // Countdown
                    this.Status = ExfilStatus.Open;
                    break;
                case 4: // Open
                    this.Status = ExfilStatus.Open;
                    break;
                case 5: // Pending
                    this.Status = ExfilStatus.Pending;
                    break;
                case 6: // AwaitActivation
                    this.Status = ExfilStatus.Pending;
                    break;
                default:
                    break;
            }
        }
    }

    public enum ExfilStatus
    {
        Open,
        Pending,
        Closed
    }
    #endregion
}
