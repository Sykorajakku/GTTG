using System;
using System.Threading;
using System.Threading.Tasks;

using GTTG.Core.Base;
using SZDC.Editor.Interfaces;
using SZDC.Editor.ModelProviders;
using SZDC.Editor.Selectors;

namespace SZDC.Editor {

    public partial class ApplicationEditor : ObservableObject {

        private readonly IServiceProvider _serviceProvider;

        public InfrastructureSelector InfrastructureSelector { get; }
        public TimeSelector TimeSelector { get; }
        public ModelProvider ModelProvider { get; }
        public DynamicDataProvider DynamicDataProvider { get; }

        public ApplicationEditor(IServiceProvider serviceProvider) {

            var synchronizationContext = serviceProvider.GetService<SynchronizationContext>();
            _serviceProvider = serviceProvider;
            ModelProvider = new ModelProvider();
            DynamicDataProvider = new DynamicDataProvider(serviceProvider.GetService<IStaticDataProvider>(), ModelProvider);
            InfrastructureSelector = new InfrastructureSelector(this);
            TimeSelector = new TimeSelector();
            
            StartDynamicDataProviderUpdates(synchronizationContext);
        }

        private void StartDynamicDataProviderUpdates(SynchronizationContext synchronizationContext) {

            /*
             * Changes content of opened windows in the application periodically
             * by moving hour interval of content
             */
            Task.Factory.StartNew(() => {

                while (true) {

                    Thread.Sleep(10 * 60 * 1000); // 10 minutes
                    synchronizationContext.Send(_ => DynamicDataProvider.TriggerUpdate(), null);
                }
            }, TaskCreationOptions.LongRunning);

            /*
             * Randomly creates update of some selected train event in train schedule
             */
            Task.Factory.StartNew(() => {

                while (true) {

                    Thread.Sleep(800); // 800 ms update for invoking modification on schedule
                    synchronizationContext.Send(_ => DynamicDataProvider.ModifySchedule(), null);
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}
