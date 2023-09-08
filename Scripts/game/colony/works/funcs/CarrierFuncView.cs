using game.wagon_buildings;

using loader.database;

using misc.managers;
using misc.tweens;

using UnityEngine;
namespace game.colony.works.funcs
{
    [RequireComponent(typeof(WorkerView))]
    internal class CarrierFuncView : FuncView
    {
        public int _uid;
        public WagonBuildingView _source;
        public WagonBuildingView _dest;
        public ResId _resId;
        public int _count;

        private CarrierFunc _func;

        private void Update()
        {
            State = _func.State;
        }

        public void Setup(CarrierFunc func)
        {
            _uid = func.Uid;
            _func = func;
            _source = _func.Source.View as WagonBuildingView;
            _dest = _func.Dest.View as WagonBuildingView;
            _resId = _func.ResId;
            _count = _func.Count;
        }
    }

    internal class CarrierFunc : WorkFunc<CarrierFuncView>
    {
        private readonly DbResStorage _sourceStorage;
        public readonly int Count;
        public readonly ResId ResId;
        public readonly WagonBuilding Source;
        private DbResStorage _destStorage;

        private TweenWait _waiter;
        public WagonBuilding Dest;

        public CarrierFunc(ColonyWork owner, WagonBuilding source, WagonBuilding dest, int resId, int count) : base(owner)
        {
            Source = source;
            Dest = dest;
            ResId = (ResId)resId;
            Count = count;
            _sourceStorage = Source.PlayerData.Storage;
            _destStorage = Dest.PlayerData.Storage;
        }

        public override SkillId SkillId => SkillId.Carrier;

        public override bool CanAssign
        {
            get
            {
                if (!DbWarehouse.HaveRes(_sourceStorage.Items, (int)ResId, Count))
                {
                    return false;
                }
                if (DbWarehouse.BoxCount(_destStorage.Items) == DbWarehouse.BoxCount(_destStorage.Items, (int)ResId, Count))
                {
                    return true;
                }
                if (DbWarehouse.GetRes(_destStorage.Items, (int)ResId).Count <= 0)
                {
                    return true;
                }
                if (DbWarehouse.BoxCount(Dest.PlayerData.Storage.Items, (int)ResId, Count) > Dest.StorageLimit)
                {
                    return false;
                }
                return true;
            }
        }

        public override void UnAssign()
        {
            Dest.UnlockPivots(Worker);
            Source.UnlockPivots(Worker);

            base.UnAssign();
        }

        protected override void SetupPersScore(WorkerView worker)
        {
            worker.Scores = worker.Data.Skill(SkillId) * 3
                + Mathf.RoundToInt((worker.Self.position - Source.View.Self.position).magnitude);
        }

        protected override void AssignTo(WorkerView worker)
        {
            base.AssignTo(worker);
            View.Setup(this);

            DbWarehouse.GetRes(_sourceStorage.Items, (int)ResId).Reserved += Count;

            DoWalkState(Source, InteractPivotType.LoadUnload, FuncState.MoveToSource, DoTakeResFromSource);
        }

        private float GetPenalty(int boxCount, int limit)
        {
            if (boxCount < limit * 0.1f)
            {
                return 0.5f;
            }
            if (boxCount > limit)
            {
                return 1.5f;
            }
            return 1.0f;
        }

        private void DoTakeResFromSource()
        {
            float penalty = GetPenalty(DbWarehouse.BoxCount(_sourceStorage.Items), Source.StorageLimit);
            DoWaitState(0, Worker.GetTakeResTime() * penalty, FuncState.TakeRes, CompleteTakeRes);
        }

        private void CompleteTakeRes()
        {
            Source.UnlockPivots(Worker);

            var box = Source.BoxStorage.Get<BoxResView>((int)ResId, Worker.BoxPivot);
            Worker.TakeBox(box);
            box.Self.localPosition = Vector3.zero;

            _waiter = new TweenWait(0.3f, () =>
            {
                _sourceStorage.PayRes((int)ResId, Count);
                DoWalkState(Dest, InteractPivotType.LoadUnload, FuncState.MoveToDest, DoPutResToDest);
            });
        }

        private void DoPutResToDest()
        {
            float penalty = GetPenalty(DbWarehouse.BoxCount(_destStorage.Items), Dest.StorageLimit);
            DoWaitState(0, Worker.GetTakeResTime() * penalty, FuncState.PutRes, CompletePutRes);
        }

        private void CompletePutRes()
        {
            Dest.UnlockPivots(Worker);
            Worker.TakeBox(null);

            _waiter = new TweenWait(0.1f, () =>
            {
                _destStorage.GainRes((int)ResId, Count);
                View.DoFinish();
                State = FuncState.Completed;
            });
        }

        public override void DoCancel()
        {
            if (IsAssigned)
            {
                Dest.UnlockPivots(Worker);
                if ((int)State == (int)FuncState.TakeRes || (int)State == (int)FuncState.MoveToSource)
                {
                    Source.UnlockPivots(Worker);
                    DbWarehouse.GetRes(_sourceStorage.Items, (int)ResId).Reserved -= Count;
                    Worker.TakeBox(null);
                }
                if ((int)State == (int)FuncState.MoveToDest || (int)State == (int)FuncState.PutRes)
                {
                    TimersManager.Remove(_waiter);
                    WorkerView pers = Worker;
                    base.DoCancel();

                    State = FuncState.MoveToDest;
                    base.AssignTo(pers);
                    View.Setup(this);

                    pers.ForceTask(this);
                    var box = Source.BoxStorage.Get<BoxResView>((int)ResId, Worker.BoxPivot);
                    Worker.TakeBox(box);
                    box.Self.localPosition = Vector3.zero;
                    new TweenWait(0.3f, () =>
                    {
                        Dest = Source;
                        _destStorage = _sourceStorage;
                        DoWalkState(Dest, InteractPivotType.LoadUnload, FuncState.MoveToDest, DoPutResToDest);
                    }).DoAlive();
                }
                else
                {
                    base.DoCancel();
                }
            }
            else
            {
                base.DoCancel();
            }
        }
    }
}
