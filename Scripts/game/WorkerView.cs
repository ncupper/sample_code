using System;
using System.Collections.Generic;

using EPOOutline;

using game.colony.works.funcs;
using game.helpers;

using gui.pers;

using loader;
using loader.database;

using misc;
using misc.tweens;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
namespace game
{
    public class WorkerView : ExtMonoBeh
    {
        [SerializeField] private Transform _boxPivot;
        [SerializeField] private PersHud _hud;
        private readonly List<IWorkFunc> _works = new List<IWorkFunc>();
        private NavMeshAgent _agent;

        private Animator _anims;
        private BoxResView _carry;
        private Collider[] _colliders;
        private IWorkFunc _curWork;

        private bool _doRotateTo;

        private UnityAction<WorkerView> _finishWalkCb = delegate {};

        private bool _isMoving;
        private NavMeshObstacle _obstacle;
        private Outlinable _outline;
        private Quaternion _rotateTo;
        private int _statePropId;

        public Transform BoxPivot => _boxPivot;

        public bool IsMoving => _isMoving && _agent.remainingDistance > 0.3f;

        //for colony task priority sorting
        public int Scores { get; set; }

        public DbWorker Data
        {
            get;
            private set;
        }

        public FuncView FuncView { get; private set; }

        private void Update()
        {
            if (_isMoving && _agent.remainingDistance < 0.301f)
            {
                _isMoving = false;
                new TweenWait(0.1f, () =>
                {
                    _agent.isStopped = true;
                    _finishWalkCb(this);
                }).DoAlive();
                IdleState();
            }
            else if (_doRotateTo)
            {
                Self.rotation = Quaternion.Lerp(Self.rotation, _rotateTo, 5 * Time.deltaTime);
            }
            else
            {
                CheckTasks();
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _agent = GetComponent<NavMeshAgent>();
            _anims = GetComponent<Animator>();
            _outline = GetComponent<Outlinable>();
            _colliders = GetComponentsInChildren<Collider>();

            for (var i = 0; i < _anims.parameters.Length; ++i)
            {
                if (_anims.parameters[i].name == "state")
                {
                    _statePropId = _anims.parameters[i].nameHash;
                }
            }

            SetSelect(false);
        }

        public void Setup(DbWorker data)
        {
            Data = data;
            _hud.Setup(Data);
        }

        private void IdleState()
        {
            _anims.SetInteger(_statePropId, _carry == null ? (int)AnimState.Idle : (int)AnimState.IdleCarry);
        }

        public void DoItemUse(bool use)
        {
            if (use)
            {
                if (_curWork != null && _curWork.SkillId == SkillId.Constructor)
                {
                    _anims.SetInteger(_statePropId, (int)AnimState.Construct);
                }
                else
                {
                    _anims.SetInteger(_statePropId, (int)AnimState.ItemUse);
                }
            }
            else
            {
                IdleState();
            }
        }

        public void TakeBox(BoxResView box)
        {
            if (_carry != null)
            {
                _carry.Visible = false;
            }
            _carry = box;
            IdleState();
        }

        public void WalkTo(Vector3 pos, UnityAction<WorkerView> cb)
        {
            _finishWalkCb = cb;
            _anims.SetInteger(_statePropId, (int)AnimState.Walk);

            new TweenWait(0.0f, () =>
            {
                _agent.isStopped = false;
                _agent.destination = pos;
                if (_agent.path.status != NavMeshPathStatus.PathComplete)
                {
                    Debug.LogError("agent.path.status = " + _agent.path.status);
                }
            }).DoAlive();
            new TweenWait(0.3f, () => { _isMoving = true; }).DoAlive();
        }

        public void UpdatePath(Vector3 pos)
        {
            if (!_isMoving)
            {
                return;
            }
            if ((_agent.destination - pos).magnitude > 0.3f)
            {
                _agent.destination = pos;
                if (_agent.path.status != NavMeshPathStatus.PathComplete)
                {
                    Debug.LogError("agent.path.status = " + _agent.path.status);
                }
            }
        }

        public void WalkStop()
        {
            _finishWalkCb = null;
            IdleState();

            _agent.isStopped = true;
            _isMoving = false;
        }

        public void ShowTaskProgress(IWorkFunc work)
        {
            _hud.ShowTask(work);
        }

        public float GetTakeResTime()
        {
            float tm = DataStorage.Constants.TakeResTime;
            tm = HelperRpg.CalcSkill(tm, SkillId.Carrier, Data);
            return tm;
        }

        public float GetCraftResTime()
        {
            float tm = DataStorage.Constants.CraftResTime;
            tm = HelperRpg.CalcSkill(tm, SkillId.Technic, Data);
            return tm;
        }

        public float GetConstructTime()
        {
            float tm = DataStorage.Constants.ConstructBaseTime;
            tm = HelperRpg.CalcSkill(tm, SkillId.Constructor, Data);
            return tm;
        }

        public float GetBuildTime()
        {
            return DataStorage.Constants.BuildTime;
        }

        public void RotateTo(bool doIt, Quaternion rotate)
        {
            _doRotateTo = doIt;
            _rotateTo = rotate;
            if (doIt)
            {
                new TweenWait(0.5f, () => { _doRotateTo = false; }).DoAlive();
            }
        }

        public void AddWorkFunc(IWorkFunc work)
        {
            _works.Add(work);
            _works.Sort((x, y) => Data.Skill(y.SkillId) - Data.Skill(x.SkillId));
        }

        public void RemoveTask(IWorkFunc work)
        {
            SkipTask(work);
            _works.Remove(work);
        }

        public void ForceTask(IWorkFunc work)
        {
            if (!_works.Contains(work))
            {
                _works.Add(work);
            }
            _curWork = work;
            FuncView = GetComponent<FuncView>();
        }

        public void SkipTask(IWorkFunc work)
        {
            if (_curWork == work)
            {
                _curWork = null;
            }
        }

        private void CheckTasks()
        {
            _works.RemoveAll(x => x.IsCompleted);
            _works.RemoveAll(x => x.IsCanceled);
            if (_curWork != null)
            {
                if (!_curWork.IsCompleted)
                {
                    return;
                }
                _curWork = null;
            }

            for (var i = 0; i < _works.Count; ++i)
            {
                if (!_works[i].IsAssigned && _works[i].CanAssign)
                {
                    _works[i].AddPers(this);
                    break;
                }
            }
        }

        public bool IsIntersect(RaycastHit[] hits, int count)
        {
            for (var i = 0; i < count; ++i)
            {
                if (Array.IndexOf(_colliders, hits[i].collider) != -1)
                {
                    return true;
                }
            }

            return false;
        }

        public void SetSelect(bool val)
        {
            if (_outline != null)
            {
                _outline.enabled = val;
            }
        }

        private enum AnimState
        {
            Idle = 0,
            Walk = 1,
            IdleCarry = 2,
            ItemUse = 3,
            Construct
        }
    }
}
