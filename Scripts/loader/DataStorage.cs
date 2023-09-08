using System.Threading.Tasks;

using loader.database;

using misc;
using misc.managers;

using UnityEngine;
namespace loader
{
    internal static class DataStorage
    {
        private static Lang _lang;
        private static PlayerData _playerData;
        private static Saver _saver;
        public static DbConstants Constants;
        public static DbOptions Options;

        public static DbResources Resources;

        public static Options Settings;
        public static DbWagonBuildings WagonBuildings;

        public static bool IsLoaded { get; private set; }

        public static DbPlayer Player => _playerData?.Data;

        private static SystemLanguage LoadSaver()
        {
            _lang = new Lang();
            _lang.Load("Lang - GUI", "GUI");
            _saver = new Saver(false, "options");
            Settings = new Options();
            _saver.AddWanter(Settings);
            AudioManager.Instance.SetOptions(Settings);

            return Settings.CurLang;
        }

        private static void LoadPlayer()
        {
            _playerData = new PlayerData();
            _saver.AddWanter(_playerData);
            _playerData.Data.JobAfterLoad();
        }

        public static async Task Load()
        {
            SystemLanguage lang = LoadSaver();
            await Task.Yield();
            LoadPlayer();
            await Task.Yield();

            _lang.SetLang(SystemLanguage.Russian);
            Resources = JsonUtility.FromJson<DbResources>(ResStorage.GetText("params/resources"));
            await Task.Yield();
            WagonBuildings = JsonUtility.FromJson<DbWagonBuildings>(ResStorage.GetText("params/wagon_buildings"));
            await Task.Yield();
            Constants = JsonUtility.FromJson<DbConstants>(ResStorage.GetText("params/constants"));
            await Task.Yield();
            Options = JsonUtility.FromJson<DbOptions>(ResStorage.GetText("params/options"));
            await Task.Yield();

            Player.DoAfterAllParamsLoading();
            IsLoaded = true;
        }
    }
}
