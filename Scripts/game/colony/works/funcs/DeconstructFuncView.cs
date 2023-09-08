using game.wagon_buildings;

using loader.database;

using UnityEngine;
namespace game.colony.works.funcs
{
    [RequireComponent(typeof(WorkerView))]
    internal class DeconstructFuncView : FuncView
    {
        public int Uid;
        public WagonBuildingView Dest;

        private DeconstructFunc _func;

        private void Update()
        {
            State = _func.State;
        }

        public void Setup(DeconstructFunc func)
        {
            _func = func;
            Uid = _func.Uid;
            Dest = _func.Dest.View as WagonBuildingView;
            State = _func.State;
        }
    }

    internal class DeconstructFunc : WorkFunc<DeconstructFuncView>
    {
        public readonly WagonBuilding Dest;
        private float _buildProgress;

        private float _buildTime = -1;

        private bool _isCanceled;

        public DeconstructFunc(ColonyWork owner, WagonBuilding dest) : base(owner)
        {
            Dest = dest;
            Dest.Constructing.AssignWork(this);
        }

        public override SkillId SkillId => SkillId.Constructor;

        public override bool CanAssign
        {
            get
            {
                if (!DbWarehouse.IsEmpty(Dest.PlayerData.Storage.Items)
                    || Dest.GetNearestInteractPivot(Vector3.zero, InteractPivotType.LoadUnload) == null)
                {
                    return false;
                }
                return true;
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

            _buildTime = -1;

            DoWalkState(Dest, InteractPivotType.LoadUnload, FuncState.MoveToDest, DoDeconstruct);
        }

        public override void UnAssign()
        {
            Dest.UnlockPivots(Worker);
            Dest.Constructing.AssignWork(null);

            base.UnAssign();
        }

        public override void DoCancel()
        {
            if (IsAssigned && State == FuncState.Craft)
            {
                _isCanceled = !_isCanceled;
                _buildProgress = _buildTime - _buildProgress;

                if (_isCanceled)
                {
                    DoWaitState(_buildTime - _buildProgress, _buildTime, FuncState.Craft, CompleteConstruct);
                }
                else
                {
                    DoWaitState(_buildTime - _buildProgress, _buildTime, FuncState.Craft, CompleteDeconstruct);
                }

                Dest.Constructing.SwitchConstructDeconstruct();
            }
            else
            {
                Dest.Constructing.AssignWork(null);
                base.DoCancel();
            }
        }

        private void DoDeconstruct()
        {
            Worker.DoItemUse(true);

            float count = 0;
            DbResVal[] ings = Dest.Data.Ings;
            for (var i = 0; i < ings.Length; ++i)
            {
                count += ings[i].Count / 100.0f;
            }

            _buildTime = Worker.GetConstructTime() * count;
            _buildProgress = _buildTime;
            DoWaitState(0, _buildTime, FuncState.Craft, CompleteDeconstruct);
        }

        private void CompleteDeconstruct()
        {
            _buildTime = -1;
            DbResVal[] ings = Dest.Data.Ings;
            for (var i = 0; i < ings.Length; ++i)
            {
                DbResVal ing = ings[i];
                Dest.PlayerData.Storage.GainRes(ing.Id, ing.Count);
            }
            Worker.DoItemUse(false);
            UnAssign();
            State = FuncState.Completed;
        }

        private void CompleteConstruct()
        {
            _buildTime = -1;
            Dest.Constructing.FinalCostruct();
            Worker.DoItemUse(false);
            UnAssign();
            State = FuncState.Completed;
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            if (_buildTime > 0)
            {
                _buildProgress -= dt;
                if (_isCanceled)
                {
                    Dest.PlayerData.LeftToBuildPercents = Mathf.Max(Mathf.RoundToInt(100.0f * _buildProgress / _buildTime), 1);
                }
                else
                {
                    Dest.PlayerData.LeftToBuildPercents =
                        Mathf.Max(Mathf.RoundToInt(100.0f * (_buildTime - _buildProgress) / _buildTime), 1);
                }

                var progress = (int)((100 - Dest.PlayerData.LeftToBuildPercents) / 100.0f * Dest.Constructing.BuildingStages);
                Dest.Constructing.SetBuildingStage(progress);
            }
        }
    }
}
