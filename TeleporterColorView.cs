﻿using Kitchen;
using KitchenMods;
using MessagePack;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenColoredTeleporters
{
    public class TeleporterColorView : UpdatableObjectView<TeleporterColorView.ViewData>
    {
        const float DEFAULT_HUE = 0.5738632f;
        const float SURFACE_SATURATION = 0.2508833f;
        const float SURFACE_VALUE = 0.6792f;
        const float ARROWS_SATURATION = 0.5905837f;
        const float ARROWS_VALUE = 0.7264f;

        public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            EntityQuery Views;
            protected override void Initialise()
            {
                base.Initialise();
                Views = GetEntityQuery(typeof(CConveyTeleport), typeof(CLinkedView));
            }

            protected override void OnUpdate()
            {
                using NativeArray<CConveyTeleport> teleports = Views.ToComponentDataArray<CConveyTeleport>(Allocator.Temp);
                using NativeArray<CLinkedView> views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);

                for (int i = 0; i < teleports.Length; i++)
                {
                    CConveyTeleport teleport = teleports[i];
                    CLinkedView view = views[i];

                    SendUpdate(view, new ViewData()
                    {
                        Index = teleport.GroupID
                    });

                    if (teleport.GroupID < 0 && teleport.Target != default && Require(teleport.Target, out CLinkedView targetLinkedView))
                    {
                        SendUpdate(targetLinkedView, new ViewData()
                        {
                            Index = teleport.GroupID
                        });
                    }
                }
            }
        }

        [MessagePackObject(false)]
        public class ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public int Index;
            [Key(1)] public float Hue;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<TeleporterColorView>();

            public bool IsChangedFrom(ViewData check)
            {
                return Index != check.Index ||
                    Hue != check.Hue;
            }
        }

        public int GroupID = 0;
        float Hue = DEFAULT_HUE;
        public MeshRenderer SurfaceRenderer;
        public MeshRenderer ArrowsRenderer;

        public void Update()
        {
            Hue = GetHue(Mathf.Abs(GroupID));
            Color surfaceColor = Color.HSVToRGB(Hue, SURFACE_SATURATION, SURFACE_VALUE);
            Color arrowsColor = Color.HSVToRGB(Hue, ARROWS_SATURATION, ARROWS_VALUE);

            UpdateMaterialColor(SurfaceRenderer, surfaceColor);
            UpdateMaterialColor(ArrowsRenderer, arrowsColor);
        }

        protected override void UpdateData(ViewData data)
        {
            GroupID = data.Index;
        }

        private void UpdateMaterialColor(Renderer renderer, Color color)
        {
            if (renderer?.material != null)
            {
                if (renderer.material.name != "New Material")
                {
                    renderer.material = new Material(renderer.material);
                    renderer.material.name = "New Material";
                }
                renderer.material.SetColor("_Color0", color);
            }
        }

        private float GetHue(int index)
        {
            return (DEFAULT_HUE + (1f / Main.PrefMananger.Get<int>(Main.COLOR_CYCLE_LENGTH_ID) * index * Main.PrefMananger.Get<int>(Main.COLOR_STAGGER_ID))) % 1;
        }
    }
}
