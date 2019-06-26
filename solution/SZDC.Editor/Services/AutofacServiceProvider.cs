using System;
using Autofac;

namespace SZDC.Editor.Services {

    public class AutofacServiceProvider : IServiceProvider {

        private readonly ILifetimeScope _scope;

        public AutofacServiceProvider(ILifetimeScope scope) {
            _scope = scope;
        }

        object IServiceProvider.GetService(Type serviceType) {
            return _scope.Resolve(serviceType);
        }
    }
}
