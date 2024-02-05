using System.Reflection;

namespace CourseLibrary.API.Services;

public class PropertyCheckerService : IPropertyCheckerService
{
    public bool TypeHasProperties<T>(string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields))
        {
            return true;
        }

        // the field are separated by ",", so we split it.
        var fieldsAfterSplit = fields.Split(",");

        // check if the requested fields exist on source
        foreach (var field in fieldsAfterSplit)
        {
            // trim each field, as it might contain leading
            // or trailing spaces. Can not trim the var in foreach,
            // so use another var.
            var propertyName = field.Trim();

            // use reflection to check if property can be
            // found on T.
            var propertyInfo = typeof(T)
                .GetProperty(propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public
                | BindingFlags.Instance);

            // it can not be found, return false
            if (propertyInfo is null)
            {
                return false;
            }
        }

        //all checks out, return true
        return true;
    }
}
