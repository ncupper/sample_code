using System;

using game;
namespace loader.database
{
    internal enum WorkType
    {
        None,
        Craft
    }

    [Serializable]
    internal class DbWork
    {
        public WorkType Type;
        public int Where; //Building.Uid

        [NonSerialized] public WorkerView Worker;
    }
}
