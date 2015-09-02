using Base.DAL;
using Ninject;
using Ninject.Syntax;

namespace WebUI.Helpers
{
    public static class NinjectExtensions
    {
        public static IBindingWithOrOnSyntax<TImplementation> WithDefaultUOW<TImplementation>(this IBindingWhenInNamedWithOrOnSyntax<TImplementation> binding)
        {
            return binding
                .WithConstructorArgument(typeof(IUnitOfWork), x => binding.Kernel.Get<IUnitOfWork>());
        }
    }
}