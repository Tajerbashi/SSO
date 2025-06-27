
namespace SSO.SharedKernel.Utilities.Library.Extensions;

public static class TypeExtentsions
{
    public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                return true;
            }
            toCheck = toCheck.BaseType;
        }
        return false;
    }

    public static string GetAttribute(this Type type)
    {
        var tableAtt = type.GetCustomAttributes(typeof(TableAttribute), false)
                                 .Cast<TableAttribute>()
                                 .FirstOrDefault();
        return $"[{tableAtt.Schema}].[{tableAtt.Name}]";
    }
}
