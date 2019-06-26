using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Autofac;

using SZDC.Editor;
using SZDC.Editor.Interfaces;
using SZDC.Editor.TrainTimetables;
using SZDC.Wpf.Modules;

namespace SZDC.Wpf.Designer {

    public class DesignerHelper : ApplicationEditor {

        private static readonly string SelectedRailwaySource = "42";

        public static readonly RailwaySegmentBriefDescription SelectedRailwaySegmentDescription =
            new RailwaySegmentBriefDescription {
                StationsInSegment = new List<string> {

                    "Albrechtice u Č.T",
                    "Horní Suchá z",
                    "Havířov-Suchá z",
                    "Havířov střed z",
                    "Havířov",
                    "Šenov z",
                    "Ostrava-Bartovice",
                    "Ostrava-Kunčice",
                    "Ostrava-Vítkovice",
                    "Ostrava-Zábřeh z",
                    "Odb Odra",
                    "Ostrava-Svinov",
                    "Ostrava-Třebovice",
                    "Děhylov",
                    "Jilešovice z",
                    "Háj ve Slezsku"
                }
            };

        public DesignerHelper() : base(new DesignerServiceProvider()) {

            TimeSelector.TimetableType = TrainTimetableType.Static;
            InfrastructureSelector.AvailableRailways = ImmutableArray.Create<string>();
            InfrastructureSelector.AvailableRailways = InfrastructureSelector.AvailableRailways.Add(SelectedRailwaySource);
            InfrastructureSelector.SelectedRailway = SelectedRailwaySource;
            InfrastructureSelector.SelectedRailwaySection = SelectedRailwaySegmentDescription;
        }

        private class DesignerServiceProvider : IServiceProvider {

            private class DesignerStaticDataProvider : IStaticDataProvider {

                public IEnumerable<string> LoadRailwayNumbers() {
                    yield return "301a+307";
                    yield return "301b/305";
                    yield return "302";
                    yield return "304";
                    yield return "306";
                    yield return "322";
                    yield return "501";
                    yield return "504";
                    yield return "516";
                }

                public RailwaySegmentDetailedDescription LoadDetailedSegmentDescription(long railwaySegmentId) {
                    throw new NotImplementedException(); // not impl.
                }

                public IEnumerable<RailwaySegmentBriefDescription> LoadRailwaySegments(string railwayNumber) {
                    yield return SelectedRailwaySegmentDescription;
                }

                public IEnumerable<StaticTrainDescription> LoadTrainsInRailwaySegment(RailwaySegmentDetailedDescription detailedDescription) {
                    throw new NotImplementedException(); // not impl.
                }
            }

            private readonly IServiceProvider _serviceProvider;

            public DesignerServiceProvider() {

                var builder = new ContainerBuilder();

                builder.RegisterModule<CoreModule>();
                builder.RegisterModule<LocatorModule>();
                builder.RegisterType<DesignerStaticDataProvider>().As<IStaticDataProvider>().InstancePerLifetimeScope();
                builder.RegisterInstance(SynchronizationContext.Current).As<SynchronizationContext>();

                var container = builder.Build();
                _serviceProvider = container.Resolve<IServiceProvider>();
            }

            public object GetService(Type serviceType) {
                return _serviceProvider.GetService(serviceType);
            }
        }
    }
}
