
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    namespace Net.CmdLine.ThreadStudy.反射
    {
        public interface IGetValue
        {
            object Get(object target);
        }
        public interface ISetValue
        {
            void Set(object target, object val);
        }

        public static class GetterSetterFactory
        {
            private static readonly Hashtable s_getterDict = Hashtable.Synchronized(new Hashtable(10240));
            private static readonly Hashtable s_setterDict = Hashtable.Synchronized(new Hashtable(10240));

            internal static IGetValue GetPropertyGetterWrapper(PropertyInfo propertyInfo)
            {
                IGetValue property = (IGetValue)s_getterDict[propertyInfo];
                if (property == null)
                {
                    property = CreatePropertyGetterWrapper(propertyInfo);
                    s_getterDict[propertyInfo] = property;
                }
                return property;
            }

            internal static ISetValue GetPropertySetterWrapper(PropertyInfo propertyInfo)
            {
                ISetValue property = (ISetValue)s_setterDict[propertyInfo];
                if (property == null)
                {
                    property = CreatePropertySetterWrapper(propertyInfo);
                    s_setterDict[propertyInfo] = property;
                }
                return property;
            }
            public static ISetValue CreatePropertySetterWrapper(PropertyInfo propertyInfo)
            {
                if (propertyInfo == null)
                    throw new ArgumentNullException("propertyInfo");
                if (propertyInfo.CanWrite == false)
                    throw new NotSupportedException("属性不支持写操作。");

                MethodInfo mi = propertyInfo.GetSetMethod(true);

                if (mi.GetParameters().Length > 1)
                    throw new NotSupportedException("不支持构造索引器属性的委托。");

                Type instanceType = typeof(SetterWrapper<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType);
                return (ISetValue)Activator.CreateInstance(instanceType, propertyInfo);
            }

            public static IGetValue CreatePropertyGetterWrapper(PropertyInfo propertyInfo)
            {
                if (propertyInfo == null)
                    throw new ArgumentNullException("propertyInfo");
                if (propertyInfo.CanRead == false)
                    throw new NotSupportedException("属性不支持读操作。");

                MethodInfo mi = propertyInfo.GetGetMethod(true);

                if (mi.GetParameters().Length > 1)
                    throw new NotSupportedException("不支持构造索引器属性的委托。");

                Type instanceType = typeof(GetterWrapper<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType);
                return (IGetValue)Activator.CreateInstance(instanceType, propertyInfo);
            }
        }

        public static class PropertyExtensions
        {
            public static object FastGetValue(this PropertyInfo propertyInfo, object obj)
            {
                if (propertyInfo == null)
                    throw new ArgumentNullException("propertyInfo");

                return GetterSetterFactory.GetPropertyGetterWrapper(propertyInfo).Get(obj);
            }

            public static void FastSetValue(this PropertyInfo propertyInfo, object obj, object value)
            {
                if (propertyInfo == null)
                    throw new ArgumentNullException("propertyInfo");

                GetterSetterFactory.GetPropertySetterWrapper(propertyInfo).Set(obj, value);
            }
        }
        public class GetterWrapper<TTarget, TValue> : IGetValue
        {
            private Func<TTarget, TValue> _getter;

            public GetterWrapper(PropertyInfo propertyInfo)
            {
                if (propertyInfo == null)
                    throw new ArgumentNullException("propertyInfo");

                if (propertyInfo.CanRead == false)
                    throw new InvalidOperationException("属性不支持读操作。");

                MethodInfo m = propertyInfo.GetGetMethod(true);
                _getter = (Func<TTarget, TValue>)Delegate.CreateDelegate(typeof(Func<TTarget, TValue>), null, m);
            }

            public TValue GetValue(TTarget target)
            {
                return _getter(target);
            }
            object IGetValue.Get(object target)
            {
                return GetValue((TTarget)target);
            }
        }
        public class SetterWrapper<TTarget, TValue> : ISetValue
        {
            private Action<TTarget, TValue> _setter;

            public SetterWrapper(PropertyInfo propertyInfo)
            {
                if (propertyInfo == null)
                    throw new ArgumentNullException("propertyInfo");

                if (propertyInfo.CanWrite == false)
                    throw new NotSupportedException("属性不支持写操作。");

                MethodInfo m = propertyInfo.GetSetMethod(true);
                _setter = (Action<TTarget, TValue>)Delegate.CreateDelegate(typeof(Action<TTarget, TValue>), null, m);
            }

            public void SetValue(TTarget target, TValue val)
            {
                _setter(target, val);
            }
            void ISetValue.Set(object target, object val)
            {
                SetValue((TTarget)target, (TValue)val);
            }
        }



    }


