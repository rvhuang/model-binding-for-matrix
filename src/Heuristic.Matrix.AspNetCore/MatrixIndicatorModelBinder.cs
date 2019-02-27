using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace Heuristic.Matrix.AspNetCore
{
    public class MatrixIndicatorModelBinder : IModelBinder
    {
        public static MatrixIndicatorModelBinder Default => new MatrixIndicatorModelBinder();

        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;

            // Try to fetch the value of the argument by name
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None) return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrWhiteSpace(value)) return Task.CompletedTask;

            bindingContext.Model = MatrixIndicator.Parse(value);

            return Task.CompletedTask;
        }
    }
}