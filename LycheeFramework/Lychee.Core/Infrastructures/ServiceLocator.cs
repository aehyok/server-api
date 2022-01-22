using System;
using System.Threading;

namespace Lychee.Core.Infrastructures
{
    public class ServiceLocator
    {
        private static readonly AsyncLocal<Func<Type, object>> _resolverFunc = new AsyncLocal<Func<Type, object>>();

        public Func<Type, object> ResolverFunc
        {
            get => _resolverFunc.Value;
            set => _resolverFunc.Value = value;
        }

        public static ServiceLocator Current
        {
            get
            {
                return new ServiceLocator();
            }
        }

        public TService GetService<TService>()
        {
            return (TService)ResolverFunc(typeof(TService));
        }

        public object GetService(Type serviceType)
        {
            return ResolverFunc(serviceType);
        }
    }
}