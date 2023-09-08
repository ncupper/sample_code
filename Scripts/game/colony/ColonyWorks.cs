using System.Collections.Generic;

using game.colony.works;
using game.colony.works.funcs;
using game.wagon_buildings;

using loader.database;

using UnityEngine;
namespace game.colony
{
    public class ColonyWorks
    {

        private readonly List<DbCraftWork> _addedCrafts = new List<DbCraftWork>();
        private readonly Colony _colony;

        private readonly Dictionary<DbCraftWork, ColonyWork> _crafts = new Dictionary<DbCraftWork, ColonyWork>();
        private readonly List<DbCraftWork> _removedCrafts = new List<DbCraftWork>();

        private readonly List<ColonyWork> _works = new List<ColonyWork>();

        public ColonyWorks(Colony colony)
        {
            _colony = colony;
        }

        public void DoUpdate(float dt)
        {
            for (var i = 0; i < _works.Count; ++i)
            {
                _works[i].DoUpdate(dt);
            }

            _works.RemoveAll(x => x.IsFinished);
        }

        public void DoLateUpdate()
        {
            RemoveCrafts();
            AddCrafts();
        }

        public void AddCraft(DbCraftWork work)
        {
            _addedCrafts.Add(work);
        }

        public void RemoveCraft(DbCraftWork work)
        {
            if (work == null)
            {
                Debug.LogError("remove null work");
            }
            else
            {
                _removedCrafts.Add(work);
            }
        }

        public void Add(ColonyWork work)
        {
            _works.Add(work);
        }

        public void CancelConstruct(WagonBuilding building)
        {
            for (var i = 0; i < _works.Count; ++i)
            {
                if (_works[i] is ConstructWork constructWork && constructWork.Building == building)
                {
                    _works[i].DoCancel();
                    break;
                }
            }
            for (var i = 0; i < _works.Count; ++i)
            {
                if (_works[i] is DeconstructWork deconstructWork && deconstructWork.Building == building)
                {
                    _works[i].DoCancel();
                    break;
                }
            }
        }

        public void CancelAllFor(WagonBuilding building)
        {
            for (var i = 0; i < _works.Count; ++i)
            {
                if (_works[i] is ConstructWork && ((ConstructWork)_works[i]).Building == building)
                {
                    _works[i].DoCancel();
                }
                if (_works[i] is CraftWork craftTask && craftTask.Building == building)
                {
                    RemoveCraft(craftTask.CraftData);
                    building.PlayerData.Queue.Remove(craftTask.CraftData);
                }
            }
        }

        private void RemoveCrafts()
        {
            for (var i = 0; i < _removedCrafts.Count; ++i)
            {
                if (_crafts.ContainsKey(_removedCrafts[i]))
                {
                    ColonyWork cWork = _crafts[_removedCrafts[i]];
                    cWork.DoCancel();
                    _crafts.Remove(_removedCrafts[i]);
                }
            }
            _removedCrafts.Clear();
        }

        private void AddCrafts()
        {
            for (var i = 0; i < _addedCrafts.Count; ++i)
            {
                WagonBuilding building = _colony.Buildings.GetBuilding(_addedCrafts[i].BuildingUid);
                if (building != null)
                {
                    var cTask = new CraftWork(_colony);
                    cTask.Setup(building, _addedCrafts[i]);
                    _works.Add(cTask);
                    _crafts.Add(_addedCrafts[i], cTask);
                }
            }
            _addedCrafts.Clear();
        }

        public int GetCarriedResources(int resId)
        {
            var count = 0;
            foreach (ColonyWork work in _works)
            {
                foreach (IWorkFunc workFunc in work.GetFuncs<CarrierFunc>())
                {
                    var func = (CarrierFunc)workFunc;
                    if (workFunc.IsAssigned && func.State == FuncState.MoveToDest && (int)func.ResId == resId)
                    {
                        count += func.Count;
                    }
                }
            }

            return count;
        }
    }
}
