using game.wagon_buildings;

using loader;
using loader.database;

using UnityEngine;
namespace game.colony.works.funcs
{
    [RequireComponent(typeof(WorkerView))]
    internal class CraftFuncView : FuncView
    {
        public int Uid;
        public WagonBuildingView Dest;
        public ResId ResId;
        public int Count;

        private CraftFunc _func;

        private void Update()
        {
            State = _func.State;
        }

        public void Setup(CraftFunc func)
        {
            _func = func;
            Uid = _func.Uid;
            Dest = _func.Dest.View as WagonBuildingView;
            ResId = _func.ResId;
            Count = _func.Count;
            State = _func.State;
        }
    }

    internal class CraftFunc : WorkFunc<CraftFuncView>
    {

        private readonly DbCraftWork _craftWork;
        public readonly WagonBuilding Dest;
        public readonly ResId ResId;
        private int _crafted;
        public int Count;

        public CraftFunc(ColonyWork owner, WagonBuilding dest, DbCraftWork craftWork, int craftCount) : base(owner)
        {
            Dest = dest;
            _craftWork = craftWork;
            ResId = (ResId)_craftWork.ResId;
            Count = craftCount;
        }

        public override SkillId SkillId => SkillId.Technic;

        public override bool CanAssign
        {
            get
            {
                if (!HaveIngs)
                {
                    return false;
                }

                if (Dest.GetNearestInteractPivot(Vector3.zero, InteractPivotType.General) == null)
                {
                    return false;
                }

                return true;
            }
        }

        private bool HaveIngs
        {
            get
            {
                DbResource resData = DataStorage.Resources[ResId];
                for (var i = 0; i < resData.Craft.Ings.Length; ++i)
                {
                    DbResVal ing = resData.Craft.Ings[i];
                    if (!DbWarehouse.HaveRes(Dest.PlayerData.Storage.Items, ing.Id, ing.Count))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private void MarkIngsReserved()
        {
            DbResource resData = DataStorage.Resources[ResId];
            for (var i = 0; i < resData.Craft.Ings.Length; ++i)
            {
                DbResVal ing = resData.Craft.Ings[i];
                DbWarehouse.GetRes(Dest.PlayerData.Storage.Items, ing.Id).Reserved += ing.Count;
            }
        }

        protected override void SetupPersScore(WorkerView worker)
        {
            worker.Scores = worker.Data.Skill(SkillId) * 15
                + Mathf.RoundToInt((worker.Self.position - Dest.View.Self.position).magnitude);
        }

        protected override void AssignTo(WorkerView worker)
        {
            base.AssignTo(worker);
            View.Setup(this);

            MarkIngsReserved();
            DoWalkState(Dest, InteractPivotType.General, FuncState.MoveToDest, DoCraft);
        }

        public override void UnAssign()
        {
            Dest.UnlockPivots(Worker);

            base.UnAssign();
        }

        public override void DoCancel()
        {
            if (IsAssigned)
            {
                Dest.SwitchOnOff(false);
                Worker.DoItemUse(false);
                DbResource resData = DataStorage.Resources[ResId];
                for (var i = 0; i < resData.Craft.Ings.Length; ++i)
                {
                    DbResVal ing = resData.Craft.Ings[i];
                    DbWarehouse.GetRes(Dest.PlayerData.Storage.Items, ing.Id).Reserved -= ing.Count;
                }
            }
            base.DoCancel();
        }

        private void CreateCarrier()
        {
            var work = new CarrierWork(Owner.Colony);
            work.Setup(Dest, new DbResVal
            {
                Id = (int)ResId, Count = _crafted
            });
            Owner.Colony.Works.Add(work);
            _crafted = 0;
        }

        private void DoCraft()
        {
            Dest.SwitchOnOff(true);
            Worker.DoItemUse(true);
            DoWaitState(0, Worker.GetCraftResTime() * Dest.Data.CraftTmCoef, FuncState.Craft, () =>
            {
                DbResource resData = DataStorage.Resources[ResId];
                Dest.PlayerData.Storage.GainRes((int)ResId, resData.Craft.Count);
                Count -= resData.Craft.Count;
                _crafted += resData.Craft.Count;
                for (var i = 0; i < resData.Craft.Ings.Length; ++i)
                {
                    DbResVal ing = resData.Craft.Ings[i];
                    Dest.PlayerData.Storage.PayRes(ing.Id, ing.Count);
                }
                if (Count > 0 && HaveIngs)
                {
                    if (_crafted + resData.Craft.Count > resData.StackSize)
                    {
                        CreateCarrier();
                    }
                    MarkIngsReserved();
                    DoCraft();
                }
                else
                {
                    CreateCarrier();
                    Dest.SwitchOnOff(false);
                    Worker.DoItemUse(false);
                    UnAssign();
                    if (Count <= 0)
                    {
                        if (_craftWork.Condition == CraftCondition.None)
                        {
                            Dest.PlayerData.Queue.Remove(_craftWork);
                        }
                        State = FuncState.Completed;
                    }
                }
            });
        }
    }
}
