using TMPro;

using UnityEngine;
#pragma warning disable 649

namespace misc.components
{
    internal class Localized : MonoBehaviour
    {
        public string Key;
        private int _curLang = -1;

        private void Awake()
        {
            OnLangChanged();
        }

        private void Update()
        {
            if (Lang.Instance != null && _curLang != (int)Lang.Instance.CurLang)
            {
                OnLangChanged();
            }
        }

        private void OnLangChanged()
        {
            if (Lang.Instance == null)
            {
                return;
            }
            _curLang = (int)Lang.Instance.CurLang;
            var txt = GetComponent<TextMeshProUGUI>();
            if (txt != null)
            {
                txt.text = Lang.Get(Key);
            }
        }
    }
}
