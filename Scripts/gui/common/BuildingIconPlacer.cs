using gui.tool_tips;

using loader.database;

using misc;

using UnityEngine;
using UnityEngine.UI;
namespace gui.craft
{
    [RequireComponent(typeof(RawImage))]
    public class BuildingIconPlacer : ExtMonoBeh, IWagonBuildingHolder
    {
        private RawImage _icon;
        public int WagonBuildingId { get; private set; }

        public void Setup(DbWagonBuilding data)
        {
            WagonBuildingId = data.Id;
            if (_icon == null)
            {
                _icon = GetComponent<RawImage>();
            }
            _icon.texture = data.Texture;
        }
    }
}
