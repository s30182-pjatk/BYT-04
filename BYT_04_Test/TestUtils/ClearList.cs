using System.Collections;
using System.Reflection;

namespace BYT_04_Test.TestUtils;

public static class ClearList
{
    /// <summary>
    /// Clears a static list field or property in a class using reflection.
    /// Used in tests to reset extent collections between test runs.
    /// </summary>
    /// <typeparam name="T">The type containing the static list</typeparam>
    /// <param name="fieldName">The name of the static field or property to clear</param>
    public static void ClearStaticList<T>(string fieldName)
    {
        var type = typeof(T);
        var field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        var prop = type.GetProperty(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        object? listObject = null;

        if (field != null) listObject = field.GetValue(null);
        else if (prop != null) listObject = prop.GetValue(null);

        if (listObject != null && listObject is IList list)
        {
            list.Clear();
        }
    }
}

