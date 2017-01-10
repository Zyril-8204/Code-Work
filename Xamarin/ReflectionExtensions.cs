using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XamarinApp.LocalDb.Helpers
{
    /// <summary>
    /// A write up of MS ReflectionExtension plus some additions/overloads
    /// used to get around xamarin's reflections.permission flag where 
    /// it seals away half the refelction library.
    /// </summary>
    public static class ReflectionExtensions
    {
        public static IEnumerable<Type> GetTypes(this Assembly assembly)
        {
            return assembly.DefinedTypes.Select(t => t.AsType());
        }

        public static EventInfo GetEvent(this Type type, string name)
        {
            return type.GetRuntimeEvent(name);
        }

        public static IEnumerable<Type> GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces;
        }

        public static bool IsAssignableFrom(this Type type, Type otherType)
        {
            return type.GetTypeInfo().IsAssignableFrom(otherType.GetTypeInfo());
        }

        public static Attribute[] GetCustomAttributes(this Type type, Type attributeType, bool inherit)
        {
            return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
        }

        public static Attribute[] GetCustomAttributes(this Type type, bool inherit)
        {
            return type.GetTypeInfo().GetCustomAttributes(inherit).ToArray();
        }

        public static IEnumerable<ConstructorInfo> GetConstructors(this Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors.Where(c => c.IsPublic);
        }

        public static bool IsInstanceOfType(this Type type, object obj)
        {

            return type.IsAssignableFrom(obj.GetType());
        }

        public static MethodInfo GetAddMethod(this EventInfo eventInfo, bool nonPublic = false)
        {
            if (eventInfo.AddMethod == null || (!nonPublic && !eventInfo.AddMethod.IsPublic))
            {
                return null;
            }

            return eventInfo.AddMethod;
        }

        public static MethodInfo GetRemoveMethod(this EventInfo eventInfo, bool nonPublic = false)
        {
            if (eventInfo.RemoveMethod == null || (!nonPublic && !eventInfo.RemoveMethod.IsPublic))
            {
                return null;
            }

            return eventInfo.RemoveMethod;
        }

        public static MethodInfo GetGetMethod(this PropertyInfo property, bool nonPublic = false)
        {
            if (property.GetMethod == null || (!nonPublic && !property.GetMethod.IsPublic))
            {
                return null;
            }

            return property.GetMethod;
        }



        public static MethodInfo GetSetMethod(this PropertyInfo property, bool nonPublic = false)
        {
            if (property.SetMethod == null || (!nonPublic && !property.SetMethod.IsPublic))
            {
                return null;
            }

            return property.SetMethod;
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type type)
        {
            return GetProperties(type, BindingFlags.FlattenHierarchy | BindingFlags.Public);
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type type, BindingFlags flags)
        {
            var properties = type.GetTypeInfo().DeclaredProperties;
            if ((flags & BindingFlags.FlattenHierarchy) == BindingFlags.FlattenHierarchy)
            {
                properties = type.GetRuntimeProperties();
            }

            return from property in properties
                   let getMethod = property.GetMethod
                   where getMethod != null
                   where (flags & BindingFlags.Public) != BindingFlags.Public || getMethod.IsPublic
                   where (flags & BindingFlags.Instance) != BindingFlags.Instance || !getMethod.IsStatic
                   where (flags & BindingFlags.Static) != BindingFlags.Static || getMethod.IsStatic
                   select property;
        }

        public static IEnumerable<PropertyInfo> GetProperties<T>(this T obj, BindingFlags flags) where T : class, new()
        {
            var properties = typeof(T).GetTypeInfo().DeclaredProperties;
            if ((flags & BindingFlags.FlattenHierarchy) == BindingFlags.FlattenHierarchy)
            {
                properties = typeof(T).GetRuntimeProperties();
            }

            return from property in properties
                   let getMethod = property.GetMethod
                   where getMethod != null
                   where (flags & BindingFlags.Public) != BindingFlags.Public || getMethod.IsPublic
                   where (flags & BindingFlags.Instance) != BindingFlags.Instance || !getMethod.IsStatic
                   where (flags & BindingFlags.Static) != BindingFlags.Static || getMethod.IsStatic
                   select property;
        }

        public static PropertyInfo GetProperty(this Type type, string name, BindingFlags flags)
        {
            if ((flags & BindingFlags.IgnoreCase) == BindingFlags.IgnoreCase)
                return GetProperties(type, flags).FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.CurrentCultureIgnoreCase));
            return GetProperties(type, flags).FirstOrDefault(p => p.Name == name);
        }

        //public static PropertyInfo GetProperty(this NotificationKind type, string name)
        //{
        //    return GetProperties(type, BindingFlags.Public | BindingFlags.FlattenHierarchy).FirstOrDefault(p => p.Name == name);
        //}

        internal static object GetProperty<T>(this T obj, string name) where T : class, new()
        {
            return GetProperties(obj, BindingFlags.Public | BindingFlags.FlattenHierarchy).FirstOrDefault(p => p.Name == name);
        }

        public static IEnumerable<MethodInfo> GetMethods(this Type type)
        {
            return GetMethods(type, BindingFlags.FlattenHierarchy | BindingFlags.Public);
        }

        public static IEnumerable<MethodInfo> GetMethods(this Type type, BindingFlags flags)
        {
            var properties = type.GetTypeInfo().DeclaredMethods;
            if ((flags & BindingFlags.FlattenHierarchy) == BindingFlags.FlattenHierarchy)
            {
                properties = type.GetRuntimeMethods();
            }

            return properties
                .Where(m => (flags & BindingFlags.Public) != BindingFlags.Public || m.IsPublic)
                .Where(m => (flags & BindingFlags.Instance) != BindingFlags.Instance || !m.IsStatic)
                .Where(m => (flags & BindingFlags.Static) != BindingFlags.Static || m.IsStatic);
        }

        public static MethodInfo GetMethod(this Type type, string name, BindingFlags flags)
        {
            return GetMethods(type, flags).FirstOrDefault(m => m.Name == name);
        }

        public static MethodInfo GetMethod(this Type type, string name)
        {
            return GetMethods(type, BindingFlags.Public | BindingFlags.FlattenHierarchy)
                   .FirstOrDefault(m => m.Name == name);
        }

        public static IEnumerable<ConstructorInfo> GetConstructors(this Type type, BindingFlags flags)
        {
            return type.GetConstructors()
                .Where(m => (flags & BindingFlags.Public) != BindingFlags.Public || m.IsPublic)
                .Where(m => (flags & BindingFlags.Instance) != BindingFlags.Instance || !m.IsStatic)
                .Where(m => (flags & BindingFlags.Static) != BindingFlags.Static || m.IsStatic);
        }

        public static IEnumerable<FieldInfo> GetFields(this Type type)
        {
            return GetFields(type, BindingFlags.Public | BindingFlags.FlattenHierarchy);
        }

        public static IEnumerable<FieldInfo> GetFields(this Type type, BindingFlags flags)
        {
            var fields = type.GetTypeInfo().DeclaredFields;
            if ((flags & BindingFlags.FlattenHierarchy) == BindingFlags.FlattenHierarchy)
            {
                fields = type.GetRuntimeFields();
            }

            return fields;
        }

        public static IEnumerable<FieldInfo> GetBaseFields(this Type type, BindingFlags flags)
        {
            var fields = type.GetTypeInfo().BaseType.GetRuntimeFields();

            return fields;
        }

        public static IEnumerable<FieldInfo> GetFields<T>(this T obj, BindingFlags flags) where T : class, new()
        {
            var fields = typeof(T).GetTypeInfo().DeclaredFields;
            if ((flags & BindingFlags.FlattenHierarchy) == BindingFlags.FlattenHierarchy)
            {
                fields = typeof(T).GetRuntimeFields();
            }
            return fields
                .Where(f => (flags & BindingFlags.Public) != BindingFlags.Public || f.IsPublic)
                .Where(f => (flags & BindingFlags.Instance) != BindingFlags.Instance || !f.IsStatic)
                .Where(f => (flags & BindingFlags.Static) != BindingFlags.Static || f.IsStatic);
        }

        /// <summary>
        /// Gets the class field by name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static FieldInfo GetField(this Type type, string name, BindingFlags flags)
        {
            return GetFields(type, flags).FirstOrDefault(p => p.Name == name);
        }

        public static object GetField<T>(this T obj, string name, BindingFlags flags) where T : class, new()
        {
            var fields = GetFields(typeof(T), flags);

            if (fields != null)
            {
                foreach (var f in fields)
                {
                    if (f.Name == name)
                    {
                        return f;
                    }
                }
                return null;
            }
            else
            {
                return null;
            } 
        }

        public static FieldInfo GetBaseField<T>(this T obj, string name, BindingFlags flags) where T : class, new()
        {
            var fields = GetBaseFields(typeof(T), flags).FirstOrDefault(p => p.Name == name);
            if (fields != null)
            {
                return fields;
            }
            else
            {
                var customFields = GetCustomAttributes(typeof(T), true).FirstOrDefault();
                return null;
            }

        }

        public static object GetBaseFieldValue<T>(this T obj, string name, BindingFlags flags) where T : class, new()
        {
            var fields = GetBaseFields(typeof(T), flags).FirstOrDefault(p => p.Name == name);
            if (fields != null)
            {
                try
                {
                    var value = fields.GetValue(fields);
                    return value;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                
            }
            else
            {
                var customFields = GetCustomAttributes(typeof(T), true).FirstOrDefault();
                return null;
            }
        }

        public static T GetFieldValue<T>(this T obj, string fieldName) where T : class, new()
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            var field = GetBaseField(obj, fieldName, BindingFlags.Public |
                                                          BindingFlags.NonPublic |
                                                          BindingFlags.Instance);

            if (field == null)
                throw new ArgumentException("fieldName", "No such field was found.");

            return (T)field.GetValue(obj);
        }

        public static FieldInfo GetField(this Type type, string name)
        {
            var fields = GetFields(type, BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (fields != null)
            {
                foreach (var f in fields)
                {
                    if (f.Name == name)
                    {
                        return f;
                    }
                }
                return null; // Not found or fields is empty
            }
            return null; // fields are null;
            
        }

        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GenericTypeArguments;
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            T attribute = null;
            var attributes = (T[])type.GetTypeInfo().GetCustomAttributes(typeof(T), true);
            if (attributes.Length > 0)
            {
                attribute = attributes[0];
            }
            return attribute;
        }

        public static T GetAttribute<T>(this PropertyInfo property) where T : Attribute
        {
            T attribute = null;
            var attributes = (T[])property.GetCustomAttributes(typeof(T), true);
            if (attributes.Length > 0)
            {
                attribute = attributes[0];
            }
            return attribute;
        }
    }
}
