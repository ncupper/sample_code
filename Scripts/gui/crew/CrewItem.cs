using System;

using game;
using game.colony.works.funcs;

using misc;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
namespace gui.crew
{
    [Serializable]
    internal struct TaskIcons
    {
        public FuncState State;
        public Sprite Sprite;
    }

    internal class CrewItem : ExtMonoBeh
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private RawImage _icon;
        [SerializeField] private Image _task;
        [SerializeField] private Image _mood;
        [SerializeField] private Sprite[] _moodIcons;
        [Space(10), SerializeField]
         private TaskIcons[] _taskIcons;

        private WorkerView _view;

        private void Update()
        {
            if (_view != null)
            {
                _name.text = _view.Data?.Name;
                _icon.texture = _view.Data?.Texture;

                Sprite sprite = null;
                FuncView func = _view.FuncView;
                FuncState state = func == null ? FuncState.None : func.State;
                for (var i = 0; i < _taskIcons.Length; ++i)
                {
                    if (_taskIcons[i].State == state)
                    {
                        sprite = _taskIcons[i].Sprite;
                        break;
                    }

                    if (_taskIcons[i].State == FuncState.None)
                    {
                        sprite = _taskIcons[i].Sprite;
                    }
                }

                _task.sprite = sprite;
            }
        }

        public void Setup(WorkerView view)
        {
            _view = view;
        }
    }
}
