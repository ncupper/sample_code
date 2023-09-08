//nik 19.07.2020

using UnityEngine;
using UnityEngine.UI;
namespace misc
{
    [ExecuteInEditMode]
    public class ImageAspectHolder : ExtMonoBeh
    {
        private Image _image;
        private Vector4 _MainTex_ST;
        private readonly Vector4 _MainTex_ST0 = new Vector4(1, 1, 0, 0);
        private Material _mat;

        private void Update()
        {
            if (!Self.hasChanged)
            {
                return;
            }

            if (!_image || !_mat)
            {
                _image = GetComponent<Image>();
                _mat = new Material(_image.material);
                _image.material = _mat;
            }

            if (!_image)
            {
                return;
            }

            _MainTex_ST = _MainTex_ST0;
            float aspect = _image.rectTransform.rect.width / _image.rectTransform.rect.height;

            if (aspect < 1)
            {
                _MainTex_ST.x = aspect;
                _MainTex_ST.z = .5f - aspect / 2f;
            }
            else if (aspect > 1)
            {
                _MainTex_ST.y = 1f / aspect;
                _MainTex_ST.w = .5f - 1f / aspect / 2f;
            }

            _mat.SetVector("_MainTex_ST", _MainTex_ST);
        }
    }
}
