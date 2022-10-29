using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SXDZD.Tools
{
    public static class ReflectionTools
    {
        #region Field

        public static FieldInfo GetFieldInfo(this object instance, string fieldName)
        {
            if (instance == null) return null;
            FieldInfo info = instance.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return info;
        }

        public static T GetFieldValue<T>(this object instance, string fieldName)
        {
            if (instance == null) return default(T);
            FieldInfo info = instance.GetFieldInfo(fieldName);
            return (T)info.GetValue(instance);
        }

        public static void SetFieldValue<T>(this object instance, string fieldName, T value)
        {
            if (instance == null) return;
            FieldInfo info = instance.GetFieldInfo(fieldName);
            info.SetValue(instance, value);
        }


        public static T GetStaticFieldValue<T>(this Type type, string fieldName)
        {
            FieldInfo info = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic);

            if (info == null)
            {
                return default(T);
            }

            return (T)info.GetValue(null);
        }

        public static void SetStaticFieldValue<T>(this Type type, string fieldName, T value)
        {
            FieldInfo info = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic);
            info.SetValue(null, value);
        }

        #endregion

        #region Property

        public static PropertyInfo GetPropertyInfo(this object instance, string fieldName)
        {
            if (instance == null) return null;
            PropertyInfo info = instance.GetType().GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (info == null)
            {
                info = instance.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
            }
            return info;
        }

        public static T GetPropertyValue<T>(this object instance, string proName)
        {
            if (instance == null) return default(T);
            PropertyInfo info = instance.GetPropertyInfo(proName);
            if (info == null)
            {
                return default(T);
            }
            return (T)info.GetValue(instance);
        }


        public static void SetPropertyValue<T>(this object instance, string fieldName, T value)
        {
            if (instance == null) return;
            PropertyInfo info = instance.GetType().GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Public);
            info.SetValue(instance, value);
        }


        public static T GetStaticPropertyValue<T>(this Type type, string fieldName)
        {
            PropertyInfo info = type.GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Public);

            if (info == null)
            {
                return default(T);
            }

            return (T)info.GetValue(null);
        }

        public static void SetStaticPropertyValue<T>(this Type type, string fieldName, T value)
        {
            PropertyInfo info = type.GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Public);
            info.SetValue(null, value);
        }


        #endregion

        #region Method

        public static object ExcuteMethod(this object instance, string methodName, params object[] args)
        {
            if (instance == null) return null;

            MethodInfo info = instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (info == null)
            {
                info = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            }

            object result = info.Invoke(instance, args);
            return result;
        }

        public static object ExcuteStaticMethod(this Type type, string methodName, params object[] args)
        {
            MethodInfo info = type.GetMethod(methodName, BindingFlags.NonPublic);
            if (info == null)
            {
                info = type.GetMethod(methodName, BindingFlags.Public);
            }
            if (info == null)
            {
                return null;
            }
            object result = info.Invoke(null, args);
            return result;
        }
        #endregion

    }
}
