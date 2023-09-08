using System.Collections.Generic;
using System.Linq;

using misc.components;
using misc.managers;
using misc.tweens;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#pragma warning disable 649

namespace misc
{
    public class GuiScreen : ExtMonoBeh
    {
        [SerializeField] protected GameObject[] _nodes;
        [SerializeField] private bool _isOver;
        [SerializeField] private bool _isBase;
        [SerializeField] private bool _playDefSounds = true;

        private readonly List<GuiScreen> _screenNodes = new List<GuiScreen>();
        private List<Animatroller> _animatrollers;

        private bool _waitSwitch;

        protected float OnShowAnimLength;

        public GuiScreen Parent { get; set; }
        public string ShowTag { get; protected set; }
        public int ShowParam { get; protected set; }

        public ScreenSwitcher Switcher { get; set; }

        public bool IsOver => _isOver;
        public bool IsBase => _isBase;

        public GuiScreen CurNode { get; private set; }

        public virtual bool PlayShowSound => true;

        protected override void OnAwake()
        {
            base.OnAwake();

            ShowNodes();
            Button[] btns = GetComponentsInChildren<Button>();
            for (var i = 0; i < btns.Length; ++i)
            {
                btns[i].SetOnClick(OnBtnClick);
            }
            _animatrollers = GetComponentsInChildren<Animatroller>().ToList();

            for (var i = 0; i < _nodes.Length; ++i)
            {
                var screenNode = _nodes[i].GetComponent<GuiScreen>();
                if (screenNode != null)
                {
                    _nodes[i] = null;
                    _screenNodes.Add(screenNode);
                    screenNode.Parent = this;
                }
            }

            OnLoad();
            HideNodes();
        }

        public void SetShowParams(string showTag, int showParam)
        {
            ShowTag = showTag;
            ShowParam = showParam;
        }

        private void OnBtnClick(GameObject sender)
        {
            if (_waitSwitch)
            {
                return;
            }
            if (sender.GetComponent<TouchCatcher>())
            {
                return;
            }

            var action = sender.GetComponent<ClickAction>();
            if (Switcher != null)
            {
                if (action == null || string.IsNullOrEmpty(action.Sound) || action.Sound == "click")
                {
                    AudioManager.Instance.PlaySound("click");
                }
                else if (!string.IsNullOrEmpty(action.Sound))
                {
                    AudioManager.Instance.PlaySound(action.Sound);
                }
            }
            OnBtnClick(sender.name, action);
        }

        protected virtual void OnLoad()
        {
        }

        public virtual bool OnBtnEsc()
        {
            return true;
        }

        protected virtual void OnBtnClick(string btn, ClickAction action)
        {
            if (action != null)
            {
                if (!string.IsNullOrEmpty(action.Action))
                {
                    if (action.Node == "_back_")
                    {
                        DelayedSwitch("_backnode_");
                        if (Parent != null)
                        {
                            Parent.DelayedSwitch(action.Action);
                            return;
                        }
                    }
                    DelayedSwitch(action.Action);
                }
                if (!string.IsNullOrEmpty(action.Node))
                {
                    if (action.Node == "_back_")
                    {
                        DelayedSwitch("_backnode_");
                    }
                    else
                    {
                        if (CurNode != null && CurNode.Visible)
                        {
                            CurNode.DelayedHideNode();
                            CurNode = null;
                        }
                        ShowNode(action.Node);
                    }
                }
            }
        }

        public void ShowNode(string args)
        {
            string[] prms = args.Split(' ');
            string nodeName = prms[0];
            GameObject node = GetNode(nodeName);
            if (node != null)
            {
                node.SetActive(true);
                var scr = node.GetComponent<GuiScreen>();
                if (scr != null)
                {
                    CurNode = scr;
                    string showTag = prms.Length > 1 ? prms[1] : "";
                    int showParam = prms.Length > 2 ? Helper.Parse(prms[2]) : 0;
                    scr.SetShowParams(showTag, showParam);
                    scr.Parent = this;
                    scr.Show();
                    //AnalyticsHelper.SetScreen(name, showTag, showParam, name);
                    if (_playDefSounds)
                    {
                        AudioManager.Instance.PlaySound("popup_open");
                    }
                }
                else
                {
                    var animatroller = node.GetComponent<Animatroller>();
                    if (animatroller != null)
                    {
                        animatroller.OnShow();
                    }
                }
            }
        }

        public void ShowNode(GuiScreen node, string args = "")
        {
            string[] prms = args.Split(' ');
            if (node != null)
            {
                node.Visible = true;
                string showTag = prms.Length > 0 ? prms[0] : "";
                int showParam = prms.Length > 1 ? Helper.Parse(prms[1]) : 0;
                node.SetShowParams(showTag, showParam);
                node.Parent = this;
                node.Show();
                //AnalyticsHelper.SetScreen(name, showTag, showParam, name);
                if (_playDefSounds)
                {
                    AudioManager.Instance.PlaySound("popup_open");
                }
            }
        }

        public void Show(bool back = false, bool isOver = false)
        {
            if (!back)
            {
                HideNodes();
            }
            OnShow();
            if (!isOver)
            {
                PlayOnShowAnims();
            }

            if (ShowTag == null)
            {
                ShowTag = string.Empty;
            }

            OnChangeOrientation(Screen.width > Screen.height);
        }

        protected virtual void OnShow()
        {
            Visible = true;
            transform.SetAsLastSibling();
            OnChangeOrientation(Screen.width >= Screen.height);
        }
        public void PlayOnShowAnims()
        {
            OnShowAnimLength = 0;
            if (_animatrollers == null)
            {
                return;
            }

            Animatroller[] trollers = GetComponentsInChildren<Animatroller>();
            for (var i = 0; i < trollers.Length; ++i)
            {
                if (_animatrollers.IndexOf(trollers[i]) == -1)
                {
                    _animatrollers.Add(trollers[i]);
                }
            }
            for (var i = 0; i < _animatrollers.Count; ++i)
            {
                if (_animatrollers[i].Visible)
                {
                    float len = _animatrollers[i].OnShow();
                    OnShowAnimLength = Mathf.Max(OnShowAnimLength, len);
                }
            }
        }

        public float DelayedMoveBack(string backScreen = "", UnityAction cb = null)
        {
            if (string.IsNullOrEmpty(backScreen))
            {
                return DelayedSwitch("_back_", 0.1f, cb);
            }

            return DelayedSwitch("_back_ " + backScreen, 0.1f, cb);
        }

        public float DelayedHideNode()
        {
            return DelayedSwitch("_backnode_");
        }

        public float SwitchScreen(string args)
        {
            if (Switcher.IsSwitchToOver(args))
            {
                return Switcher.SwitchScreen(args);
            }
            return DelayedSwitch(args);
        }

        protected virtual float GetOnHideDelay(float minDelay)
        {
            float delay = minDelay;
            for (var i = 0; i < _animatrollers.Count; ++i)
            {
                float d = _animatrollers[i].OnHide();
                delay = Mathf.Max(d, delay);
            }

            for (var n = 0; n < _screenNodes.Count; ++n)
            {
                if (_screenNodes[n] != null && _screenNodes[n].Visible)
                {
                    GuiScreen screen = _screenNodes[n];
                    for (var i = 0; i < screen._animatrollers.Count; ++i)
                    {
                        float d = screen._animatrollers[i].OnHide();
                        delay = Mathf.Max(d, delay);
                    }
                }
            }

            return delay;
        }

        protected float DelayedSwitch(string args, float minDelay = 0.1f, UnityAction cb = null)
        {
            OnPreHide();
            _waitSwitch = true;
            float delay = GetOnHideDelay(minDelay);

            if (args == "_backnode_")
            {
                if (_playDefSounds)
                {
                    AudioManager.Instance.PlaySound("popup_close");
                }
            }
            else if (IsOver && args == "_back_")
            {
                if (_playDefSounds)
                {
                    AudioManager.Instance.PlaySound("popup_close");
                }
            }

            new TweenWait(delay, x =>
            {
                _waitSwitch = false;
                if (args == "_backnode_")
                {
                    Visible = false;
                }
                else
                {
                    Switcher.SwitchScreen(args);
                }

                if (cb != null)
                {
                    cb();
                }
            }).DoAlive();

            return delay;
        }

        protected virtual void OnPreHide()
        {
        }

        public virtual void OnHide()
        {
            Visible = false;
        }

        public void ShowNodes()
        {
            if (_nodes != null)
            {
                for (var i = 0; i < _nodes.Length; ++i)
                {
                    if (_nodes[i] != null)
                    {
                        _nodes[i].SetActive(_nodes[i].GetComponent<GuiScreen>() == null);
                    }
                }
            }
        }

        public void HideNodes()
        {
            if (_nodes != null)
            {
                for (var i = 0; i < _nodes.Length; ++i)
                {
                    if (_nodes[i] != null)
                    {
                        _nodes[i].SetActive(false);
                    }
                }
                for (var i = 0; _screenNodes != null && i < _screenNodes.Count; ++i)
                {
                    if (_screenNodes[i] != null)
                    {
                        _screenNodes[i].Visible = false;
                    }
                }
            }
        }

        public GameObject GetNode(string n)
        {
            if (_nodes != null)
            {
                for (var i = 0; i < _nodes.Length; ++i)
                {
                    if (_nodes[i] != null && _nodes[i].name == n)
                    {
                        return _nodes[i];
                    }
                }
                for (var i = 0; i < _screenNodes.Count; ++i)
                {
                    if (_screenNodes[i] != null && _screenNodes[i].name == n)
                    {
                        return _screenNodes[i].gameObject;
                    }
                }
            }
            return null;
        }

        public T GetNode<T>() where T : GuiScreen
        {
            for (var i = 0; i < _screenNodes.Count; ++i)
            {
                if (_screenNodes[i] != null)
                {
                    var t = _screenNodes[i] as T;
                    if (t != null)
                    {
                        return t;
                    }
                }
            }
            return null;
        }

        public GuiScreen GetScreenNode(string nodeName)
        {
            for (var i = 0; i < _screenNodes.Count; ++i)
            {
                if (_screenNodes[i] != null && _screenNodes[i].name == nodeName)
                {
                    return _screenNodes[i];
                }
            }
            return null;
        }

        public virtual void OnChangeOrientation(bool landscape)
        {
            if (CurNode != null)
            {
                CurNode.OnChangeOrientation(landscape);
            }
        }
    }
}
