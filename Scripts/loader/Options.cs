using misc;
using misc.managers;

using UnityEngine;
namespace loader
{
    internal class Options : IWantSave, ISoundOptions
    {
        public SystemLanguage CurLang;

        public Options()
        {
            WanterName = "options";
        }
        public bool NotifsOn { get; set; }
        public int Orientation { get; set; }
        public bool Sound { get; set; }
        public bool Music { get; set; }

        public override void Load(JSONNode data)
        {
            SystemLanguage sysLang = Application.systemLanguage;
            if (sysLang == SystemLanguage.ChineseSimplified || sysLang == SystemLanguage.ChineseTraditional)
            {
                sysLang = SystemLanguage.Chinese;
            }
            if (sysLang == SystemLanguage.Ukrainian || sysLang == SystemLanguage.Belarusian)
            {
                sysLang = SystemLanguage.Russian;
            }

            if (sysLang != SystemLanguage.French && sysLang != SystemLanguage.Japanese && sysLang != SystemLanguage.Russian &&
                sysLang != SystemLanguage.English && sysLang != SystemLanguage.Chinese && sysLang != SystemLanguage.Italian &&
                sysLang != SystemLanguage.German && sysLang != SystemLanguage.Polish && sysLang != SystemLanguage.Portuguese &&
                sysLang != SystemLanguage.Korean && sysLang != SystemLanguage.Spanish)
            {
                sysLang = SystemLanguage.English;
            }

            CurLang = data.GetEnum("lang", sysLang);
            Sound = data.GetBool("sound", true);
            Music = data.GetBool("music", true);
            NotifsOn = data.GetBool("notif", true);

            Orientation = data.GetInt("orient", 2);

            RefreshOrientation();

            //Lang.Instance.SetLang(CurLang);
        }

        public override void Save(JSONNode data)
        {
            data.Add("lang", CurLang.ToString());
            data.Add("sound", Sound);
            data.Add("music", Music);
            data.Add("notif", NotifsOn);
            data.Add("orient", Orientation);
        }

        public void RefreshOrientation()
        {
            if (Orientation == 0) // && Screen.orientation != ScreenOrientation.Portrait)
            {
                Screen.orientation = ScreenOrientation.Portrait;
                //new TweenWait(0, () =>
                //{
                Screen.orientation = ScreenOrientation.AutoRotation;
                Screen.autorotateToPortrait = true;
                Screen.autorotateToPortraitUpsideDown = true;
                Screen.autorotateToLandscapeLeft = false;
                Screen.autorotateToLandscapeRight = false;
                //}).DoAlive();
            }
            if (Orientation == 1) // && Screen.orientation != ScreenOrientation.Landscape)
            {
                Screen.orientation = ScreenOrientation.Landscape;
                //new TweenWait(0, () =>
                //{
                Screen.orientation = ScreenOrientation.AutoRotation;
                Screen.autorotateToPortrait = false;
                Screen.autorotateToPortraitUpsideDown = false;
                Screen.autorotateToLandscapeLeft = true;
                Screen.autorotateToLandscapeRight = true;
                //}).DoAlive();
            }
            if (Orientation == 2) // && Screen.orientation != ScreenOrientation.AutoRotation)
            {
                Screen.orientation = ScreenOrientation.AutoRotation;
                Screen.autorotateToPortrait = true;
                Screen.autorotateToPortraitUpsideDown = true;
                Screen.autorotateToLandscapeLeft = true;
                Screen.autorotateToLandscapeRight = true;
            }
        }
    }
}
