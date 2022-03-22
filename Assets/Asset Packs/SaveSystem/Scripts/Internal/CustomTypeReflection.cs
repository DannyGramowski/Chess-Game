using Hson.Serializer;
using System;
using System.Collections.Generic;
using System.Reflection;

public class CustomTypeReflection
{
    public static List<ICustomSerializeType> GetCustomTypes()
    {
        Type thisType = typeof(CustomTypeReflection);
        Type baseType = typeof(ICustomSerializeType);

        Assembly currentAssembly = Assembly.GetAssembly(thisType);
        Type[] typesInCurrentAssembly = currentAssembly.GetTypes();

        List<ICustomSerializeType> types = new List<ICustomSerializeType>();
        foreach(Type type in typesInCurrentAssembly)
        {
            if (type.GetInterface("ICustomSerializeType") != null)
                types.Add((ICustomSerializeType)Activator.CreateInstance(type));
        }
        return types;
    }
}
