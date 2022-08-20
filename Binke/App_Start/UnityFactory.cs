using Doctyme.Repository;
using Doctyme.Repository.Interface;
using System;

namespace Binke
{

    /// <summary>
    /// Creates an IMyDataContext instance
    /// </summary>
    public static class UnityFactory
    {
        /// <summary>
        /// The factory used to create an instance
        /// </summary>

        public static Func<IErrorLogService> Factory;
        /// <summary>
        /// Initializes the specified creation factory.
        /// </summary>
        /// <param name="creationFactory">The creation factory.</param>
        public static void SetFactory(Func<IErrorLogService> creationFactory)
        {
            Factory = creationFactory;
        }

        /// <summary>
        /// Creates a new IMyDataContext instance.
        /// </summary>
        /// <returns>Returns an instance of an IMyDataContext </returns>
        public static IErrorLogService CreateContext()
        {
            if (Factory == null) throw new InvalidOperationException("You can not create a context without first building the factory.");

            return Factory();
        }

    }

}
