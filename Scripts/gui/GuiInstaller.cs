using System.Collections.Generic;

using gui.tool_tips;

using misc;
using misc.managers;

using UnityEngine;

using Zenject;
namespace gui
{
    public class GuiInstaller : ScriptableObjectInstaller<GuiInstaller>
    {
        public ScreenSwitcher UiPrefab;

        public override void InstallBindings()
        {
            Container.Bind<ScreenSwitcher>()
                     .FromComponentInNewPrefab(UiPrefab)
                     .WithGameObjectName(UiPrefab.name)
                     .AsSingle();
            Container.BindFactory<List<GuiScreen>, string, Transform, GuiScreen, ScreenSwitcher.Factory>()
                     .FromFactory<PlaceholderFactory<List<GuiScreen>, string, Transform, GuiScreen>>();
            Container.BindFactory<TooltipView, Transform, TooltipView, TooltipView.Factory>()
                     .FromFactory<PlaceholderFactory<TooltipView, Transform, TooltipView>>();
            Container.BindFactory<GameObject, Transform, GameObject, Helper.Factory>()
                     .FromFactory<PlaceholderFactory<GameObject, Transform, GameObject>>();
        }
    }
}
