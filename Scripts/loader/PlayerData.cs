using System;

using loader.database;

using misc;

using UnityEngine;
namespace loader
{
    [Serializable]
    internal class PlayerData : IWantSave
    {
        public DbPlayer Data;

        public PlayerData()
        {
            WanterName = "player";
        }

        public override void Load(JSONNode data)
        {
            if (data["pdata"] != null)
            {
                Data = new DbPlayer();
                Data.Load(data["pdata"]);
            }
            else
            {
                string json = data.GetKey("data");
                if (string.IsNullOrEmpty(json))
                {
                    Data = new DbPlayer();
                    Data.NewPlayer();
                }
                else
                {
                    Data = JsonUtility.FromJson<DbPlayer>(json);
                }
            }
        }

        public override void Save(JSONNode data)
        {
            var p = new JSONClass();
            Data.Save(p);
            data.Add("pdata", p);
            //data.Add("data", JsonUtility.ToJson(Data));
            //Data.Save(data);
        }
    }
}
