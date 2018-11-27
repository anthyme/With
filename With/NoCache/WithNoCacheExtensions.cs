using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace With.NoCache
{
    public static class WithNoCacheExtensions
    {
        public static T WithNoCache<T, P>(this T self, Expression<Func<T, P>> selector, P newValue)
        {
            var type = typeof(T).GetTypeInfo();
            var constructor = type.DeclaredConstructors.Single();
            var parameterInfos = constructor.GetParameters();
            var changedProp = (PropertyInfo)((MemberExpression)selector.Body).Member;

            return parameterInfos.Any()
                ? ImmutableWith(self, changedProp, newValue, type, constructor, parameterInfos)
                : MutableWith(self, changedProp, newValue, type);
        }

        public static T WithNoCache<T>(this T self, params (Expression<Func<T, object>> Selector, object NewValue)[] changes)
        {
            var type = typeof(T).GetTypeInfo();
            var constructor = type.DeclaredConstructors.Single();
            var parameterInfos = constructor.GetParameters();
            var changedProps = changes.Select(x => (GetProperty(x.Selector), x.NewValue)).ToArray();

            return parameterInfos.Any()
                ? ImmutableWith(self, changedProps, type, constructor, parameterInfos)
                : MutableWith(self, changedProps, type);
        }

        private static T MutableWith<T>(T self, (PropertyInfo Property, object NewValue)[] changedProps, TypeInfo type)
        {
            var clone = Activator.CreateInstance<T>();

            foreach (var prop in type.DeclaredProperties)
            {
                prop.SetValue(clone, prop.GetValue(self));
            }
            foreach (var (changedProp, newValue) in changedProps)
            {
                changedProp.SetValue(clone, newValue);
            }
            return clone;
        }

        private static T ImmutableWith<T>(T self, (PropertyInfo Property, object NewValue)[] changedProps, TypeInfo type, ConstructorInfo constructor, ParameterInfo[] parameterInfos)
        {
            var newValueProperties = changedProps.ToDictionary(x => x.Property.Name.ToLower(), x => x.NewValue);
            var parameters =
                from parameter in parameterInfos
                let parameterName = parameter.Name.ToLower()
                from property in type.DeclaredProperties
                let propertyName = property.Name.ToLower()
                where propertyName == parameterName
                select newValueProperties.TryGetValue(propertyName, out var newValue)
                    ? newValue : property.GetValue(self);

            return (T)constructor.Invoke(parameters.ToArray());
        }

        private static T ImmutableWith<T, P>(T self, PropertyInfo changedProp, P newValue, TypeInfo type,
            ConstructorInfo constructor, ParameterInfo[] parameterInfos)
        {
            var newValuePropertyName = changedProp.Name.ToLower();
            var parameters =
                from parameter in parameterInfos
                let parameterName = parameter.Name.ToLower()
                from property in type.DeclaredProperties
                let propertyName = property.Name.ToLower()
                where propertyName == parameterName
                select propertyName == newValuePropertyName ? newValue : property.GetValue(self);

            return (T)constructor.Invoke(parameters.ToArray());
        }

        private static T MutableWith<T, P>(T self, PropertyInfo changedProp, P newValue, TypeInfo type)
        {
            var clone = Activator.CreateInstance<T>();
            foreach (var prop in type.DeclaredProperties)
            {
                prop.SetValue(clone, prop.GetValue(self));
            }
            changedProp.SetValue(clone, newValue);
            return clone;
        }

        private static PropertyInfo GetProperty<T>(Expression<Func<T, object>> expression)
        {
            if (expression.Body is MemberExpression)
            {
                return (PropertyInfo)((MemberExpression)expression.Body).Member;
            }
            else
            {
                return (PropertyInfo)((MemberExpression)((UnaryExpression)expression.Body).Operand).Member;
            }
        }
    }
}