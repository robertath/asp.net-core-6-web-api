using System.Dynamic;
using System.Reflection;

namespace CourseLibrary.API.Helpers
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Implementing DataShaping, to return an object; The fields to return are set dynamically;
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="fields"></param>
        /// <returns>ExpandoObject</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static ExpandoObject ShapeData<TSource>(
            this TSource source,
            string? fields)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var dataShapedObject = new ExpandoObject();

            var propertyInfoList = new List<PropertyInfo>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource)
                    .GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                foreach(var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);

                    ((IDictionary<string, object?>)dataShapedObject)
                        .Add(propertyInfo.Name, propertyValue);
                }

                return dataShapedObject;
            }
            else
            {
                var fieldsAfterSplit = fields.Split(',');

                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();

                    // use reflection to get the property on the source object
                    // we need to include public and instance, b/c specifying a 
                    // binding flag overwrites the already-existing binding flags.
                    var propertyInfo = typeof(TSource)
                        .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new ArgumentException($"Property {propertyName} wans't found on "
                            + $"{typeof(TSource)}");
                    }
                    var propertyValue = propertyInfo.GetValue(source);

                    ((IDictionary<string, object?>)dataShapedObject).Add(propertyName, propertyValue);
                }

                //return shaped object
                return dataShapedObject;
            }
        }
    }
}
