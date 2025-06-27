using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace SSO.SharedKernel.Utilities.Library.Helpers;

public static class AttributeHelper
{
    public static (string Name, string Schema) GetTableAttributeInfo(Type type)
    {
        var tableAttribute = type.GetCustomAttributes<TableAttribute>(false).FirstOrDefault();

        if (tableAttribute == null)
        {
            throw new InvalidOperationException("The specified type does not have a TableAttribute.");
        }

        return (tableAttribute.Name, tableAttribute.Schema);
    }
}
