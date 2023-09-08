using UnityEngine;
namespace misc.components
{
    public class DayTimeSettings : MonoBehaviour
    {
        public static DayTimeSettings[] DayTimes = new DayTimeSettings[4];
        public static int CurDayTime = 12;

        public int Clock;

        public Color LightColor;
        public float Light = 0.78f;

        public Color SkyColor;
        public Color EquatorColor;
        public Color GroundColor;
        public float Ambient = 0.85f;

        private DayTimeSettings _cache;
        private float _dist = -1;

        private Light _light;

        private void Awake()
        {
            DayTimes[Clock / 6] = this;
        }

        private Color LerpColor(Color from, Color to, float dist)
        {
            return new Color(
                Mathf.Lerp(from.r, to.r, dist),
                Mathf.Lerp(from.g, to.g, dist),
                Mathf.Lerp(from.b, to.b, dist),
                Mathf.Lerp(from.a, to.a, dist)
            );
        }
        public void Apply(DayTimeSettings to, float dist, bool force)
        {
            if (to == null || !force && _cache == to && Helper.IsEqual(_dist, dist))
            {
                return;
            }

            _dist = dist;
            _cache = to;

            RenderSettings.ambientIntensity = Mathf.Lerp(Ambient, to.Ambient, dist);
            RenderSettings.ambientGroundColor = LerpColor(GroundColor, to.GroundColor, dist);
            RenderSettings.ambientEquatorColor = LerpColor(EquatorColor, to.EquatorColor, dist);
            RenderSettings.ambientSkyColor = LerpColor(SkyColor, to.SkyColor, dist);

            if (_light == null)
            {
                _light = Helper.GetRootObject<Light>();
            }
            if (_light != null)
            {
                _light.color = LerpColor(LightColor, to.LightColor, dist);
                _light.intensity = Mathf.Lerp(Light, to.Light, dist);
            }
        }

        public void Apply(float dist, bool force)
        {
            if (!force && _cache == this && Helper.IsEqual(_dist, dist))
            {
                return;
            }

            _dist = dist;
            _cache = this;

            RenderSettings.ambientIntensity = Mathf.Lerp(Ambient, Ambient, dist);
            RenderSettings.ambientGroundColor = LerpColor(GroundColor, GroundColor, dist);
            RenderSettings.ambientEquatorColor = LerpColor(EquatorColor, EquatorColor, dist);
            RenderSettings.ambientSkyColor = LerpColor(SkyColor, SkyColor, dist);

            if (_light == null)
            {
                _light = Helper.GetRootObject<Light>();
            }
            _light.color = LerpColor(LightColor, LightColor, dist);
            _light.intensity = Mathf.Lerp(Light, Light, dist);
        }

        public static void ApplyClock(float tm)
        {
            DayTimeSettings from = DayTimes[0];
            DayTimeSettings to = null;
            for (var i = 0; i < DayTimes.Length; ++i)
            {
                if (DayTimes[i].Clock > tm)
                {
                    to = DayTimes[i];
                    break;
                }
                from = DayTimes[i];
            }

            if (to == null)
            {
                to = DayTimes[0];
            }
            from.Apply(to, (tm - from.Clock) / 6.0f, false);
            CurDayTime = Mathf.RoundToInt(tm);
            //SkyView.SetTime(tm);
        }
    }
}
