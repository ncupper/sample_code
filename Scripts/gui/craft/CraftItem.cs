using loader.database;

using misc;
using misc.components;

using TMPro;

using UnityEngine;
namespace gui.craft
{
    internal class CraftItem : ExtMonoBeh
    {
        [SerializeField] private ResourceIconPlacer _icon;
        [SerializeField] private TextMeshProUGUI _name0;

        private ExToggle _toogle;

        public bool IsOn
        {
            get => _toogle != null && _toogle.IsOn;
            set
            {
                if (_toogle != null)
                {
                    _toogle.IsOn = value;
                }
            }
        }

        public DbResource Data { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            _toogle = GetComponent<ExToggle>();
        }

        public void Setup(DbResource data)
        {
            Data = data;
            _icon.Setup(Data);
            if (_name0 != null)
            {
                _name0.text = Lang.Get(Data.Name);
            }
        }
    }
}
