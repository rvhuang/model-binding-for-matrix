using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Heuristic.Matrix.AspNetCore
{
    /// <summary>
    ///  Creates <see cref="MatrixIndicatorModelBinder"/> instances.
    /// </summary>
    public class MatrixIndicatorModelBinderFactory : IModelBinderProvider
    {
        /// <summary>
        /// Gets the default instance.
        /// </summary>
        public static IModelBinderProvider Default => new MatrixIndicatorModelBinderFactory();

        IModelBinder IModelBinderProvider.GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.Metadata.ModelType == typeof(MatrixIndicator))
            {
                return MatrixIndicatorModelBinder.Default;
            }
            return null;
        }
    }
}