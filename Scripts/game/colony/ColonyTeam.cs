using System.Collections.Generic;

using game.colony.works.funcs;

using gui;

using loader.database;

using misc;

using UnityEngine;
namespace game.colony
{
    public class ColonyTeam
    {
        private readonly IconsCamera _iconsCamera;
        private readonly List<WorkerView> _workers = new List<WorkerView>();

        public ColonyTeam(IconsCamera iconsCamera)
        {
            _iconsCamera = iconsCamera;
        }

        public int Count => _workers.Count;

        public void Setup()
        {
            _workers.Add(Helper.GetRootObject<WorkerView>("male_01"));
            _workers.Add(Helper.GetRootObject<WorkerView>("woman_01"));
            if (_workers[0] != null)
            {
                DbWorker data = DbWorker.Random(SkillId.Technic);
                data.Name = "Бендер";
                _iconsCamera.Setup(_workers[0], data);
                _workers[0].Setup(data);
            }

            if (_workers[1] != null)
            {
                DbWorker data = DbWorker.Random(SkillId.Carrier);
                data.Name = "Глаша";
                _iconsCamera.Setup(_workers[1], data);
                _workers[1].Setup(data);
            }

            for (var i = 2; i < _workers.Count; ++i)
            {
                DbWorker data = DbWorker.Random();
                data.Name = "Random";
                _iconsCamera.Setup(_workers[i], data);
                _workers[i].Setup(data);
            }
        }

        public void PushWork(List<IWorkFunc> funcs, int fromIdx = 0)
        {
            for (var i = 0; i < _workers.Count; ++i)
            {
                for (int f = fromIdx; f < funcs.Count; ++f)
                {
                    _workers[i].AddWorkFunc(funcs[f]);
                }
            }
        }

        public WorkerView GetIntersect(RaycastHit[] hits, int hitCount)
        {
            for (var i = 0; i < _workers.Count; ++i)
            {
                if (_workers[i].IsIntersect(hits, hitCount))
                {
                    return _workers[i];
                }
            }
            return null;
        }

        public WorkerView GetWorker(int index)
        {
            return _workers[index];
        }
    }
}
