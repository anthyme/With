using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace With
{
    public static class WithExtensions
    {
        public static T With<T, P>(this T self, Expression<Func<T, P>> selector, P newValue)
        {
            return OptimizedWith<T, P>.With(self, selector, newValue);
        }

        private static class OptimizedWith<T, P>
        {
            private static Func<T, P, T> create;

            public static T With(T self, Expression<Func<T, P>> selector, P newValue)
            {
                if (create == null)
                {
                    var changedProp = (PropertyInfo) ((MemberExpression) selector.Body).Member;
                    var type = typeof(T);
                    var constructor = type.GetTypeInfo().DeclaredConstructors.Single();
                    var parameterInfos = constructor.GetParameters();

                    create = parameterInfos.Any()
                        ? ImmutableWith(changedProp, type, constructor, parameterInfos)
                        : MutableWith(changedProp, type);
                }
                return create(self, newValue);
            }

            private static Func<T, P, T> ImmutableWith(PropertyInfo changedProp, Type type,
                ConstructorInfo constructor, ParameterInfo[] parameterInfos)
            {
                var typeInfo = type.GetTypeInfo();
                var changedPropName = changedProp.Name.ToLower();
                var propertyAccessors =
                (from parameter in parameterInfos
                    let parameterName = parameter.Name.ToLower()
                    from property in typeInfo.DeclaredProperties
                    let propertyName = property.Name.ToLower()
                    where propertyName == parameterName
                    select propertyName == changedPropName ? null : GetPropertyGetter(property)).ToArray();

                return (self, newValue) => (T)constructor.Invoke(propertyAccessors.Select(x => x == null ? newValue : x(self)).ToArray());
            }

            private static Func<T, P, T> MutableWith(PropertyInfo changedProp, Type type)
            {
                var typeInfo = type.GetTypeInfo();
                var properties = typeInfo.DeclaredProperties;
                var copies = properties.Select(p => CopyExpression(p, type)).ToList();
                var copyValue = GetValueSetter(changedProp);

                return (self, newValue) =>
                {
                    var clone = Activator.CreateInstance<T>();
                    foreach (var copy in copies)
                    {
                        copy(self, clone);
                    }
                    copyValue(clone, newValue);
                    return clone;
                };
            }

            private static Func<object, object> GetPropertyGetter(PropertyInfo propertyInfo)
            {
                var instance = Expression.Parameter(typeof(object), "x");
                var convertToType = Expression.TypeAs(instance, propertyInfo.DeclaringType);
                var property = Expression.Property(convertToType, propertyInfo);
                var convertToObject = Expression.Convert(property, typeof(object));
                return (Func<object, object>)Expression.Lambda(convertToObject, instance).Compile();
            }

            private static Action<object, object> GetValueSetter(PropertyInfo propertyInfo)
            {
                var instance = Expression.Parameter(typeof(object), "x");
                var arg = Expression.Parameter(typeof(object), "a");
                var convertToType = Expression.TypeAs(instance, propertyInfo.DeclaringType);
                var convertToObject = Expression.Convert(arg, propertyInfo.PropertyType);
                var setter = Expression.Call(convertToType, propertyInfo.SetMethod, convertToObject);
                return (Action<object, object>)Expression.Lambda(setter, instance, arg).Compile();
            }

            private static Action<T, T> CopyExpression(PropertyInfo changedProp, Type type)
            {
                var getter = GetPropertyGetter( changedProp);
                var setter = GetValueSetter( changedProp);
                return (source, destination) => setter(destination, getter(source));
            }
        }
    }
}