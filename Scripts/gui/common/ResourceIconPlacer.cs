using gui.tool_tips;

using loader.database;

using misc;

using UnityEngine;
using UnityEngine.UI;
namespace gui.craft
{
    [RequireComponent(typeof(RawImage))]
    public class ResourceIconPlacer : ExtMonoBeh, IResourceHolder
    {
        private RawImage _icon;

        public int ResId { get; private set; }

        public void Setup(DbResource resData)
        {
            ResId = resData.Id;
            if (_icon == null)
            {
                _icon = GetComponent<RawImage>();
            }
            _icon.texture = resData.Texture;
        }
    }
}
