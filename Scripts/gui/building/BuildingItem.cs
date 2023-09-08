using gui.craft;

using loader.database;

using misc;
using misc.components;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
namespace gui.building
{
    internal class BuildingItem : ExtMonoBeh
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private BuildingIconPlacer _icon;

        private ExToggle _toggle;

        public UnityAction<BuildingItem> OnSelect = delegate {};

        public DbWagonBuilding Data
        {
            get;
            private set;
        }

        public bool IsOn
        {
            get => _toggle.IsOn;
            set => _toggle.IsOn = value;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _toggle = GetComponent<ExToggle>();
            _toggle.OnChange += ChangeSelection;
        }

        private void ChangeSelection(ExToggle toggle)
        {
            if (toggle.IsOn)
            {
                OnSelect(this);
            }
        }

        public void Setup(DbWagonBuilding data)
        {
            Data = data;
            _name.text = Lang.Get(Data.Name);
            _icon.Setup(Data);
        }
    }
}
