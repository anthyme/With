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
            var changedProp = ((MemberExpression)selector.Body).Member;
            var type = typeof(T).GetTypeInfo();

            var constructor = type.DeclaredConstructors.Single();
            var newValuePropertyName = changedProp.Name.ToLower();
            var parameters =
                from parameter in constructor.GetParameters()
                let parameterName = parameter.Name.ToLower()
                from property in type.DeclaredProperties
                let propertyName = property.Name.ToLower()
                where propertyName == parameterName
                select propertyName == newValuePropertyName ? newValue : property.GetValue(self);

            return (T)constructor.Invoke(parameters.ToArray());
        }

        public static T WithM<T, P>(this T self, Expression<Func<T, P>> selector, P newValue) where T : new()
        {
            var me = (MemberExpression)selector.Body;
            var changedProp = (System.Reflection.PropertyInfo)me.Member;

            var clone = new T();
            foreach (var prop in typeof(T).GetTypeInfo().DeclaredProperties)
            {
                prop.SetValue(clone, prop.GetValue(self));
            }

            changedProp.SetValue(clone, newValue);
            return clone;
        }
    }
}