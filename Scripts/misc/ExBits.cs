using System.Collections.Generic;
namespace misc
{
    internal class ExBits
    {
        private List<int> _data = new List<int>();

        public bool this[int idx]
        {
            get => Get(idx);
            set => Set(idx, value);
        }

        public void Save(string name, JSONNode node)
        {
            node.Add(name, _data);
        }

        public void Load(string name, JSONNode node)
        {
            _data = node.GetIntList(name, 0, 0);
        }

        private bool Get(int idx)
        {
            if (idx < 0 || idx >= _data.Count * 32)
            {
                return false;
            }

            int bit = 1 << idx % 32;
            return (_data[idx / 32] & bit) != 0;
        }

        private void Set(int idx, bool val)
        {
            if (idx < 0)
            {
                return;
            }

            int dataIdx = idx / 32;
            if (dataIdx > 10)
            {
                return;
            }

            while (dataIdx >= _data.Count)
            {
                _data.Add(0);
            }

            int bit = 1 << idx % 32;
            if (val)
            {
                _data[dataIdx] |= bit;
            }
            else
            {
                _data[dataIdx] &= ~bit;
            }
        }
    }
}
