using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using GTTG.Core.Drawing.Layers;
using SZDC.Model.Layers;

namespace SZDC.Editor.Layers {

    public class StaticLayersOrder : IRegisteredLayersOrder {

        public ImmutableList<Type> DrawingLayerTypeList { get; }

        public StaticLayersOrder() {

            var order = new List<Type> {
                typeof(BackgroundLayer),
                typeof(InfrastructureLayer),
                typeof(TrafficLayer),
                typeof(SelectedTrainLayer),
            };

            DrawingLayerTypeList = ImmutableList.CreateRange(order);
        }
    }
}
