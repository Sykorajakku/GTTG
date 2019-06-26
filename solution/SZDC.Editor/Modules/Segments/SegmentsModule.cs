using Autofac;

using GTTG.Core.Strategies.Implementations;
using GTTG.Core.Strategies.Interfaces;
using GTTG.Model.Model.Infrastructure;
using GTTG.Model.Strategies.Types;

namespace SZDC.Editor.Modules.Segments {

    public class SegmentsModule : Module {

        protected override void Load(ContainerBuilder builder) {

            builder
                .RegisterType<SegmentRegistry<SegmentType<Track>, MeasureableSegment>>()
                .As<ISegmentRegistry<SegmentType<Track>, MeasureableSegment>>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<SegmentRegistry<SegmentType<Station>, MeasureableSegment>>()
                .As<ISegmentRegistry<SegmentType<Station>, MeasureableSegment>>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<SegmentRegistry<LineType, MeasureableSegment>>()
                .As<ISegmentRegistry<LineType, MeasureableSegment>>()
                .InstancePerLifetimeScope();
        }
    }
}
