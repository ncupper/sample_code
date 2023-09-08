using UnityEngine;
namespace misc.network
{
#pragma warning disable 649

    internal class NetCommand
    {
        private readonly bool _postRequest;
        private JSONNode _answer;
        private int _repeatCount;
        private float _wait = -1;

        protected bool NeedLock;
        protected JSONNode Payload;
        protected bool Repeat;

        protected bool Success;

        protected NetCommand(bool post)
        {
            _postRequest = post;
        }

        public bool IsComplete
        {
            get;
            private set;
        }
        public bool IsFailed
        {
            get;
            private set;
        }

        public virtual void DoIt()
        {
            Net.AddCommand(this);
        }

        public virtual void SendRequest()
        {
            Net.Request(GetUrl(), OnCompleteCmd, GetParams(), OnErrorCmd, GetPostData());
        }

        private void OnErrorCmd()
        {
            if (Repeat)
            {
                ++_repeatCount;
                _wait = Mathf.Min(7, _repeatCount);
            }
            else
            {
                IsFailed = true;
                OnError();
            }
        }

        public void DoUpdate()
        {
            if (_wait > -0.5f)
            {
                _wait -= Time.deltaTime;
                if (_wait < 0)
                {
                    _wait = -1;
                    SendRequest();
                }
            }
        }

        private void OnCompleteCmd(JSONNode r)
        {
            _answer = r;
            long time = _answer.GetLong("tm");
            if (time > 0)
            {
                Helper.ServerDelta = (uint)time;
            }

            Payload = _answer;
            Success = Payload != null;

            OnComplete();
        }

        protected virtual string GetUrl()
        {
            return string.Empty;
        }

        protected virtual string GetParams()
        {
            return string.Empty;
        }

        public void Break()
        {
            IsFailed = true;
            Unlock();
        }

        protected virtual void OnError()
        {
            Helper.LogError("SyncError (" + GetUrl() + ")(" + _answer + ")");
            IsFailed = true;
            Unlock();
        }

        protected virtual void OnComplete()
        {
            IsComplete = true;
            Unlock();
        }

        protected virtual string GetPostData()
        {
            return _postRequest ? string.Empty : null;
        }

        private void Unlock()
        {
        }
    }

    internal class NetGetCommand : NetCommand
    {
        public NetGetCommand() : base(false)
        {
        }
    }

    internal class NetPostCommand : NetCommand
    {
        public NetPostCommand() : base(true)
        {
        }
    }
}
