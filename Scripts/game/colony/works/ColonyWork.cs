using System.Collections.Generic;

using game.colony.works.funcs;
using game.wagon_buildings;

using loader;
using loader.database;

using UnityEngine;
namespace game.colony.works
{
    public class ColonyWork
    {
        protected readonly List<IWorkFunc> Funcs = new List<IWorkFunc>();

        protected ColonyWork(Colony colony)
        {
            Colony = colony;
        }

        public Colony Colony { get; }

        public virtual bool IsFinished => Funcs.Count == 0;

        public List<IWorkFunc> GetFuncs<T>() where T : IWorkFunc
        {
            return Funcs.FindAll(x => x is T);
        }

        public virtual void DoUpdate(float dt)
        {
            for (var i = 0; i < Funcs.Count; ++i)
            {
                if (Funcs[i].IsAssigned)
                {
                    Funcs[i].DoUpdate(dt);
                }
                else
                {
                    Funcs[i].TryAssign();
                }
            }

            Funcs.RemoveAll(x => x.IsCompleted);
        }

        public virtual void DoCancel()
        {
            for (var i = 0; i < Funcs.Count; ++i)
            {
                Funcs[i].DoCancel();
            }
            Funcs.RemoveAll(x => x.IsCanceled);
        }

        protected void CreateCarrierFunc(WagonBuilding source, WagonBuilding dest, DbResVal res)
        {
            DbResource resData = DataStorage.Resources[res.Id];
            int carrierCount = res.Count;
            while (carrierCount > 0)
            {
                int count = Mathf.Min(resData.StackSize, carrierCount);
                Funcs.Add(new CarrierFunc(this, source, dest, res.Id, count));
                carrierCount -= count;
            }
        }
    }
}
