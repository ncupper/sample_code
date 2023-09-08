using game.colony.works.funcs;

using loader.database;

using misc;

using TMPro;

using UnityEngine;
namespace gui.pers
{
    internal class PersHud : ExtMonoBeh
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private ExProgressBar _taskBar;

        private IWorkFunc _work;

        private void Update()
        {
            if (_work != null)
            {
                _taskBar.Value = 100 - Mathf.RoundToInt(_work.StateProgress * 100.0f);
            }
        }
        protected override void OnAwake()
        {
            base.OnAwake();
            _taskBar.Visible = false;
        }

        public void Setup(DbWorker data)
        {
            _name.text = data.Name;
        }

        public void ShowTask(IWorkFunc work)
        {
            _work = work;
            _taskBar.Visible = _work != null;
            _taskBar.Value = 0;
        }
    }
}
