using System.Collections.Generic;

using game.wagon_buildings;

using loader.database;

using misc.managers;
using misc.tweens;

using UnityEngine.Events;
namespace game.colony.works.funcs
{
    public enum FuncState
    {
        None,

        //carrier
        MoveToSource,
        TakeRes,
        MoveToDest,
        PutRes,

        //craft
        Craft,

        Completed,
        Canceled,
        //Errors
        ErrorPivotNotFound,
        ErrorPathNotCompleted
    }

    internal static class TaskFuncUid
    {
        public static int Next = 1;
    }

    internal class WorkFunc<T> : IWorkFunc where T : FuncView
    {

        private readonly List<WorkerView> _perses = new List<WorkerView>();
        private bool _isWait;
        private InteractPivotView _pivot;

        private TweenWait _recheckPivotWaiter;
        private int _tryWalkCount;
        private TweenWait _updatePathWaiter;
        private UnityAction _waitCb;
        private float _waitDuration;
        private float _waitTimer;

        protected T View;
        protected WorkerView Worker;

        protected WorkFunc(ColonyWork owner)
        {
            Uid = TaskFuncUid.Next++;
            Owner = owner;
            State = FuncState.None;
        }

        protected ColonyWork Owner { get; }
        public FuncState State { get; protected set; }

        public int Uid
        {
            get;
        }
        public bool IsCompleted => State == FuncState.Completed;
        public bool IsCanceled => State == FuncState.Canceled;
        public virtual SkillId SkillId => SkillId.None;
        public virtual bool CanAssign => false;
        public virtual bool IsAssigned => Worker != null;
        public virtual float StateProgress { get; protected set; }

        public virtual void DoUpdate(float dt)
        {
            if (_isWait)
            {
                _waitTimer -= dt;
                StateProgress = _waitTimer / _waitDuration;
                if (StateProgress < 0)
                {
                    _isWait = false;
                    Worker.ShowTaskProgress(null);
                    _waitCb?.Invoke();
                }
            }
        }

        public void TryAssign()
        {
            if (_perses.Count > 1)
            {
                for (var i = 0; i < _perses.Count; ++i)
                {
                    SetupPersScore(_perses[i]);
                }
                _perses.Sort((x, y) => y.Scores - x.Scores);
            }
            if (_perses.Count > 0)
            {
                AssignTo(_perses[0]);
                _perses.Clear();
            }
        }

        public void AddPers(WorkerView worker)
        {
            _perses.Add(worker);
        }

        public virtual void UnAssign()
        {
            View.DoFinish();
            View = null;
            Worker.SkipTask(this);
            Worker = null;
        }

        public virtual void DoCancel()
        {
            if (IsAssigned)
            {
                _isWait = false;
                Worker.ShowTaskProgress(null);
                TimersManager.Remove(_recheckPivotWaiter);
                TimersManager.Remove(_updatePathWaiter);
                Worker.WalkStop();
                UnAssign();
            }
            State = FuncState.Canceled;
        }

        protected virtual void SetupPersScore(WorkerView worker)
        {
            worker.Scores = worker.Data.Skill(SkillId);
        }

        protected virtual void AssignTo(WorkerView worker)
        {
            Worker = worker;
            View = Worker.gameObject.AddComponent<T>();
            Worker.ForceTask(this);
        }

        private void FinishWalk(UnityAction cb)
        {
            _tryWalkCount = 0;
            if (_pivot.Pose == PivotPose.Rotate)
            {
                Worker.RotateTo(true, _pivot.Self.rotation);
            }
            cb();
        }

        protected void DoWalkState(WagonBuilding building, InteractPivotType pivotType, FuncState state, UnityAction cb)
        {
            _pivot = building.GetNearestInteractPivot(Worker.Self.position, pivotType);
            if (_pivot == null)
            {
                State = FuncState.ErrorPivotNotFound;
                _recheckPivotWaiter = new TweenWait(1.0f, () =>
                {
                    DoWalkState(building, pivotType, state, cb);
                });
                return;
            }

            _pivot.Locked = Worker;
            State = state;
            StateProgress = 0;
            if ((Worker.Self.position - _pivot.Self.position).sqrMagnitude < 0.5f)
            {
                FinishWalk(cb);
            }
            else
            {
                Worker.WalkTo(_pivot.Self.position, x =>
                {
                    if ((Worker.Self.position - _pivot.Self.position).sqrMagnitude < 0.5f)
                    {
                        FinishWalk(cb);
                    }
                    else
                    {
                        if (_tryWalkCount < 3)
                        {
                            ++_tryWalkCount;
                            DoWalkState(building, pivotType, state, cb);
                        }
                        else
                        {
                            State = FuncState.ErrorPathNotCompleted;
                        }
                    }
                });

                _updatePathWaiter = new TweenWait(1.0f, () => { UpdatePersPath(building, pivotType); });
            }
        }

        private void UpdatePersPath(WagonBuilding building, InteractPivotType pivotType)
        {
            if (!Worker.IsMoving)
            {
                return;
            }
            if (_pivot != null)
            {
                _pivot.Locked = null;
            }
            InteractPivotView newPivot = building.GetNearestInteractPivot(Worker.Self.position, pivotType);
            if (newPivot != null && newPivot != _pivot)
            {
                _pivot = newPivot;
                Worker.UpdatePath(_pivot.Self.position);
            }

            if (_pivot != null)
            {
                _pivot.Locked = Worker;
            }
            _updatePathWaiter = new TweenWait(1.0f, () => { UpdatePersPath(building, pivotType); });
        }

        protected void DoWaitState(float timeOffset, float duration, FuncState state, UnityAction cb)
        {
            _isWait = true;
            _waitCb = cb;
            _waitDuration = duration;
            _waitTimer = _waitDuration - timeOffset;
            State = state;
            StateProgress = 1;
            Worker.ShowTaskProgress(this);
        }
    }
}
