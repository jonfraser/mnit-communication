using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Builder;
using Autofac.Core;

namespace MNIT_Communication.Services
{
    public static class ServiceLocator
    {
        private static readonly ContainerBuilder builder = new ContainerBuilder();
        private static IContainer container;

        public static IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType<T>()
        {
            return builder.RegisterType<T>();
        }

        public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterInstance<T>(T instance) where T: class
        {
            return builder.RegisterInstance(instance);
        } 
        
        public static T Resolve<T>()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                return scope.Resolve<T>();
            }
        }
        
        public static IContainer Container
        {
            private get
            {
                if (container == null)
                {
                    container = builder.Build();
                }

                return container;
            }

            set { container = value; }
        }

       

    }
}
