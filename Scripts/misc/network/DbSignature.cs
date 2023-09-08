using System;

using UnityEngine;
namespace misc.network
{
    [Serializable]
    internal class DbSignature
    {
        public string Signature;

        public void CheckSignature()
        {
            string signature = Signature;

            Signature = "check";
            string json = JsonUtility.ToJson(this);
            if (signature != Net.Md5Sum(json + 17032015))
            {
#if UNITY_EDITOR
                Helper.LogError("Bad signature [" + Net.Md5Sum(json + 17032015) + "]  " + GetType());
#endif
                OnBadSignature();
            }
            Signature = signature;
        }

        public virtual void OnBadSignature()
        {
        }
    }
}
