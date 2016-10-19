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
            var type = typeof(T).GetTypeInfo();
            var constructor = type.DeclaredConstructors.Single();
            var parameterInfos = constructor.GetParameters();
            var changedProp = (PropertyInfo)((MemberExpression)selector.Body).Member;

            return parameterInfos.Any() 
                ? ImmutableWith(self, changedProp, newValue, type,constructor, parameterInfos) 
                : MutableWith(self, changedProp, newValue, type);
        }

        private static T ImmutableWith<T, P>(T self, PropertyInfo changedProp, P newValue, TypeInfo type, ConstructorInfo constructor, ParameterInfo[] parameterInfos)
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
    }
}