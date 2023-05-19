using System.Dynamic;
using System.Reflection;

namespace CourseLibrary.API.Helpers;

public static class IEnumerableExtensions
{

    /// <summary>
    /// Implementing DataShaping Collection Resources, to return a collection of data, 
    /// The fields to return are set dynamically
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="fields"></param>
    /// <returns>List<ExpandoObject></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static IEnumerable<ExpandoObject> ShapeData<TSource>(
        this IEnumerable<TSource> source,
        string? fields)
    {
        if(source == null) 
            throw new ArgumentNullException(nameof(source));

        var expandoObjectList = new List<ExpandoObject>();

        var propertyInfoList= new List<PropertyInfo>();

        if (string.IsNullOrWhiteSpace(fields))
        {
            var propertyInfos = typeof(TSource)
            .GetProperties(BindingFlags.IgnoreCase
            | BindingFlags.Public | BindingFlags.Instance);

            propertyInfoList.AddRange(propertyInfos);
        }
        else
        {
            var fieldsAfterSplit = fields.Split(',');

            foreach(var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();

                var propertyInfo = typeof(TSource)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase
                    | BindingFlags.Public | BindingFlags.Instance);


                if(propertyInfo == null )
                {
                    throw new ArgumentException($"Property {propertyName} wans't found on "
                        + $"{typeof(TSource)}");
                }
                propertyInfoList.Add( propertyInfo );
            }
        }

        //run through the sourceobjects
        foreach(TSource sourceObject in source)
        {
            //create an ExpandoObject that will hold the selected propertiies & values
            var dataShapedObject = new ExpandoObject();

            //get the value of each property we have to return.
            foreach(var propertyInfo in propertyInfoList)
            {
                var propertyValue = propertyInfo.GetValue(sourceObject);

                //add field to ExpandoObject
                ((IDictionary<string, object?>)dataShapedObject)
                    .Add(propertyInfo.Name, propertyValue);
            }

            expandoObjectList.Add(dataShapedObject);
        }

        return expandoObjectList;
    }

}
