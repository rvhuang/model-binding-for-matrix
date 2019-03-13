using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace Heuristic.Matrix.AspNetCore
{
    /// <summary>
    /// Represents the model binder for <see cref="MatrixIndicator"/>.
    /// </summary>
    public class MatrixIndicatorModelBinder : IModelBinder
    {
        /// <summary>
        /// Gets the default instance.
        /// </summary>
        public static IModelBinder Default => new MatrixIndicatorModelBinder();

        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            // Try to fetch the value of the argument by name
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None) return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var model = MatrixIndicator.Parse(valueProviderResult.FirstValue);
            
            bindingContext.Model = model;
            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}