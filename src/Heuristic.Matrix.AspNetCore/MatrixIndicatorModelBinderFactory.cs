using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Heuristic.Matrix.AspNetCore
{
    public class MatrixIndicatorModelBinderFactory : IModelBinderProvider
    {
        public static MatrixIndicatorModelBinderFactory Default => new MatrixIndicatorModelBinderFactory();

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