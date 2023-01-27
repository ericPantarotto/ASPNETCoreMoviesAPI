using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MoviesAPI.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(propertyName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            try
            {
                T deserializedValue = JsonConvert.DeserializeObject<T>(valueProviderResult.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(deserializedValue);
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(propertyName, $"Value is invalid for type {typeof(T)}");
            }

            return Task.CompletedTask;
        }
    }
}
