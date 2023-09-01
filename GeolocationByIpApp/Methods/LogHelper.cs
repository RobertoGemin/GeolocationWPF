namespace GeolocationApp.Methods
{
    public class LogHelper
    {
        public static string GetPropertyReader<T>(T instance)
        {
            var type = typeof(T);
            var properties = type.GetProperties();

            var result = $"Class name: {type.Name} n\"";

            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(instance);

                result += $"Property name: {propertyName}, Property value: {propertyValue} n\"";
            }

            return result;
        }
    }
}