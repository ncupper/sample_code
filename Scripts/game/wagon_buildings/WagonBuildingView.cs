using System;

using EPOOutline;

using misc;

using UnityEngine;
namespace game.wagon_buildings
{
    public class WagonBuildingView : ExtMonoBeh
    {
        public GameObject[] BuildStages;
        [Space(10)]
        public bool IsOn;
        public GameObject[] OnObjects;
        public Animation[] OnAnims;
        [NonSerialized] public Collider[] Colliders;
        [NonSerialized] public InteractPivotView[] InteractPivots;

        [NonSerialized] public Outlinable Outline;

        protected override void OnAwake()
        {
            base.OnAwake();

            Outline = GetComponentInChildren<Outlinable>();
            InteractPivots = GetComponentsInChildren<InteractPivotView>();
            Colliders = GetComponentsInChildren<Collider>();
        }
    }
}
