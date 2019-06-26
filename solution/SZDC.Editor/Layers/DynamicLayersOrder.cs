using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using GTTG.Core.Drawing.Layers;
using SZDC.Model.Layers;

namespace SZDC.Editor.Layers {

    public class DynamicLayersOrder : IRegisteredLayersOrder {

        public ImmutableList<Type> DrawingLayerTypeList { get; }

        public DynamicLayersOrder() {

            var order = new List<Type> {
                typeof(BackgroundLayer),
                typeof(InfrastructureLayer),
                typeof(TrafficLayer),
                typeof(SelectedTrainLayer),
                typeof(CurrentTimeLayer)
            };

            DrawingLayerTypeList = ImmutableList.CreateRange(order);
        }
    }
}
