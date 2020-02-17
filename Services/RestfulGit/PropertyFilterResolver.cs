using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EW.Navigator.SCM.RestfulGit.Sync
{
    public class PropertyFilterResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, IEnumerable<string>> _ignorePropertiesMap = new Dictionary<Type, IEnumerable<string>>();
        private readonly Dictionary<Type, IEnumerable<string>> _includePropertiesMap = new Dictionary<Type, IEnumerable<string>>();
        public PropertyFilterResolver SetIgnoredProperties<T>(params Expression<Func<T, object>>[] propertyAccessors)
        {
            if (propertyAccessors == null) return this;

            if (_includePropertiesMap.ContainsKey(typeof(T))) throw new ArgumentException(Properties.Resources.IncludeError);

            var properties = propertyAccessors.Select(GetPropertyName);
            _ignorePropertiesMap[typeof(T)] = properties.ToArray();
            return this;
        }

        public PropertyFilterResolver SetIncludedProperties<T>(params Expression<Func<T, object>>[] propertyAccessors)
        {
            if (propertyAccessors == null)
                return this;

            if (_ignorePropertiesMap.ContainsKey(typeof(T))) throw new ArgumentException(Properties.Resources.IncludeError);

            var properties = propertyAccessors.Select(GetPropertyName);
            _includePropertiesMap[typeof(T)] = properties.ToArray();
            return this;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            var isIgnoreList = _ignorePropertiesMap.TryGetValue(type, out var map);
            if (!isIgnoreList && !_includePropertiesMap.TryGetValue(type, out map))
                return properties;

            bool Predicate(JsonProperty jp) => map.Contains(jp.PropertyName) == !isIgnoreList;
            return properties.Where(Predicate).ToArray();
        }

        private static string GetPropertyName<TSource, TProperty>(
        Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (!(propertyLambda.Body is MemberExpression member))
                throw new ArgumentException(string.Format(Properties.Resources.MethodReferenceError, propertyLambda));

            if (!(member.Member is PropertyInfo propInfo))
                throw new ArgumentException(string.Format(Properties.Resources.FieldReferenceError, propertyLambda));

            var type = typeof(TSource);
            var baseType = type.BaseType;

            var typeInfo = propInfo.DeclaringType.GetTypeInfo();
            if (!type.GetTypeInfo().IsAssignableFrom(typeInfo) && !baseType.GetTypeInfo().IsAssignableFrom(typeInfo))
                throw new ArgumentException(string.Format(Properties.Resources.TypeError, propertyLambda, type));

            var customAttr = propInfo.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault();
            if (customAttr == null) return string.Empty;
            var castAttribute = (JsonPropertyAttribute)customAttr;
            var property = castAttribute.PropertyName;

            //return propInfo.Name;
            return property;
        }
    }
}

