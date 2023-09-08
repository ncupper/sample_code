using System.Collections.Generic;

using misc.tweens;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Zenject;
namespace misc.managers
{
    public class ScreenSwitcher : FpsCounter
    {
        [SerializeField]
        private EventSystem _eventSystem;
        [SerializeField]
        private Canvas _mainCanvas;
        [SerializeField]
        private Canvas _tooltipCanvas;
        [SerializeField]
        private List<GuiScreen> _screenPrefabs;

        [Space(10), SerializeField, Range(0.01f, 8.0f)]
          private float _timeScale = 1.0f;

        private readonly List<CanvasScaler> _scalers = new List<CanvasScaler>();

        private readonly List<GuiScreen> _screens = new List<GuiScreen>();
        private readonly Stack<GuiScreen> _stack = new Stack<GuiScreen>();
        private Factory _screensFactory;

        public Canvas Canvas => _mainCanvas;
        public Canvas TooltipCanvas => _tooltipCanvas;

        public float TimeScale
        {
            get => _timeScale;
            set => _timeScale = value;
        }

        public bool IsStackEmpty => _stack.Count == 0;

        private GuiScreen CurrentScreen { get; set; }

        private float PortraitScale => Screen.width >= Screen.height ? 1 : 0;

        protected virtual void Update()
        {
            UpdateFps();
            if (_timeScale < 0.5f)
            {
                Time.timeScale = _timeScale;
            }
        }

        [Inject]
        public void Construct(Factory screensFactory)
        {
            _screensFactory = screensFactory;
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            DontDestroyOnLoad(gameObject);
            _scalers.Add(GetComponent<CanvasScaler>());

            _screenPrefabs.RemoveAll(x => x == null);

            _eventSystem.pixelDragThreshold = Mathf.Max(Screen.width, Screen.height) / 50; //2%
        }

        private GuiScreen CreateScreen(string scrName)
        {
            GuiScreen screen = _screensFactory.Create(_screenPrefabs, scrName, _mainCanvas.transform);
            screen.name = scrName;
            screen.Visible = true;
            screen.Switcher = this;
            _screens.Add(screen);
            return screen;
        }

        private GuiScreen GetScreen(string scrName)
        {
            for (var i = 0; i < _screens.Count; ++i)
            {
                if (_screens[i].name == scrName)
                {
                    return _screens[i];
                }
            }
            return CreateScreen(scrName);
        }

        public T GetScreen<T>() where T : GuiScreen
        {
            for (var i = 0; i < _screens.Count; ++i)
            {
                if (_screens[i] is T)
                {
                    return (T)_screens[i];
                }
            }
            return null;
        }

        public bool IsSwitchToOver(string args)
        {
            string[] ar = args.Split(' ');

            if (ar[0] == "_back_")
            {
                return false;
            }
            GuiScreen s = GetScreen(ar[0]);
            if (s == null)
            {
                return false;
            }
            s.Visible = false;
            return s.IsOver;
        }

        public float SwitchScreen(string args)
        {
            string[] ar = args.Split(' ');
            string t = ar.Length > 1 ? ar[1] : "";
            int param = ar.Length > 2 ? Helper.Parse(ar[2]) : 0;

            if (ar[0] == "_back_")
            {
                MoveScreenBack(t);
                return 0;
            }
            GuiScreen s = GetScreen(ar[0]);
            if (s == null)
            {
                return 0;
            }
            if (CurrentScreen != null)
            {
                if (!s.IsOver)
                {
                    CurrentScreen.OnHide();
                }
                _stack.Push(CurrentScreen);
            }

            CurrentScreen = s;
            CurrentScreen.SetShowParams(t, param);
            CurrentScreen.Show();

            if (!CurrentScreen.IsBase && CurrentScreen.PlayShowSound)
            {
                AudioManager.Instance.PlaySound("popup_open");
            }
            OnSwitchScreen();
            return 0;
        }

        protected virtual void OnSwitchScreen()
        {
        }

        public void MoveScreenBack(string toScreen = "")
        {
            if (!CurrentScreen.IsBase)
            {
                AudioManager.Instance.PlaySound("popup_close");
            }

            bool isOver = CurrentScreen.IsOver;
            CurrentScreen.OnHide();
            if (_stack.Count == 0)
            {
                return;
            }

            CurrentScreen = _stack.Pop();
            if (string.IsNullOrEmpty(toScreen) || CurrentScreen.name == toScreen || CurrentScreen.IsBase)
            {
                CurrentScreen.Show(true, isOver);
                OnSwitchScreen();
            }
            else
            {
                MoveScreenBack(toScreen);
            }
        }

        public string GetBackScreenName()
        {
            if (_stack.Count == 0)
            {
                return string.Empty;
            }
            return _stack.Peek().name;
        }

        public bool HaveScreenBack(string screen)
        {
            if (_stack.Count == 0)
            {
                return false;
            }
            foreach (GuiScreen guiScreen in _stack)
            {
                if (guiScreen.name == screen)
                {
                    return true;
                }
            }

            return false;
        }

        protected void AddScaler(CanvasScaler scaler)
        {
            bool landscape = Screen.width >= Screen.height;
            _scalers.Add(scaler);
            scaler.matchWidthOrHeight = landscape ? 1 : PortraitScale;
            OnChangeResolution(landscape);
        }

        protected virtual void OnChangeResolution(bool landscape)
        {
            _scalers[0].matchWidthOrHeight = landscape ? 1 : PortraitScale;

            for (var i = 0; i < _screens.Count; ++i)
            {
                if (_screens[i].Visible)
                {
                    _screens[i].OnChangeOrientation(landscape);
                }
            }

            for (var i = 0; i < _scalers.Count; ++i)
            {
                _scalers[i].matchWidthOrHeight = landscape ? 1 : PortraitScale;
                Vector2 refRes = _scalers[i].referenceResolution;
                float width, height;
                if (landscape)
                {
                    width = Mathf.Max(refRes.x, refRes.y);
                    height = Mathf.Min(refRes.x, refRes.y);
                }
                else
                {
                    width = Mathf.Min(refRes.x, refRes.y);
                    height = Mathf.Max(refRes.x, refRes.y);
                }
                refRes.x = width;
                refRes.y = height;
                _scalers[i].referenceResolution = refRes;
            }
        }

        public class Factory : PlaceholderFactory<List<GuiScreen>, string, Transform, GuiScreen>
        {
            private readonly DiContainer _container;

            public Factory(DiContainer container)
            {
                _container = container;
            }

            public override GuiScreen Create(List<GuiScreen> prefabs, string screenName, Transform root)
            {
                GuiScreen prefab = prefabs.Find(x => x.name == screenName);
                return _container.InstantiatePrefab(prefab, root).GetComponent<GuiScreen>();
            }
        }
    }
}
