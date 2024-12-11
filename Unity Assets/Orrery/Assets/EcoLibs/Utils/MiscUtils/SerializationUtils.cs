// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class SerializationUtils
    {
        private class SafeSerializationBinder : DefaultSerializationBinder
        {
            private static readonly string DeclaringAssemblyName = typeof(SafeSerializationBinder).Assembly.GetName().Name;

            internal static readonly SafeSerializationBinder Instance = new SafeSerializationBinder();

            public override Type BindToType(string assemblyName, string typeName)
            {
                if (assemblyName != DeclaringAssemblyName)
                    throw new JsonSerializationException($"Trying to deserialize unsafe type: {typeName}, {assemblyName}");
                return base.BindToType(assemblyName, typeName);
            }
        }

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, SerializationBinder = SafeSerializationBinder.Instance };

        public static string Serialize(object obj) => JsonConvert.SerializeObject(obj, SerializerSettings);
        public static T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, SerializerSettings);
        public static T Deserialize<T>(string json, params JsonConverter[] converters) => JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { SerializationBinder = SafeSerializationBinder.Instance, Converters = converters });
    }

    /// <summary>
    /// This is a copy of the ExpandableObjectContractResolver class from Eco.Core, which couldnt be shared
    /// through Eco.Shared because it uses Newtonsoft.Json, and the client uses a differnet conflicting version of it.
    /// So we have to duplicate it so we can deserialize server types on client-side like GameSettings which need to ignore the 'expandable object' conversion attribute.
    /// </summary>
    public class ExpandableObjectContractResolver : DefaultContractResolver
    {
        protected virtual Type[] IgnoredAttributes() => new[] { typeof(Eco.Shared.Serialization.JsonIgnoreAttribute) };

        bool isWriting;
        bool serializeReadOnlyProperties;

        public ExpandableObjectContractResolver(bool isWriting = false, bool serializeReadOnlyProperties = false)
        {
            this.isWriting = isWriting;
            this.serializeReadOnlyProperties = serializeReadOnlyProperties;
        }

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            return base.GetSerializableMembers(objectType).Where(member =>
            {
                if (this.IgnoredAttributes().Any(x => member.GetCustomAttribute(x) != null))    return false;
                if (member is PropertyInfo info)
                {
                    if (this.serializeReadOnlyProperties) return info.CanRead;
                    else return info.CanWrite && info.CanRead;
                }
                else
                    return true;
            }).ToList();
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            if (objectType.IsValueType && !objectType.IsPrimitive && !objectType.IsEnum)
            {
                var contract = this.CreateObjectContract(objectType);
                contract.IsReference = false;
                return contract;
            }
            if (TypeDescriptor.GetAttributes(objectType).Contains(new TypeConverterAttribute(typeof(ExpandableObjectConverter))))
            {
                return this.CreateObjectContract(objectType);
            }

            return base.CreateContract(objectType);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            // skip if the property is not a DateTime
            if (property.PropertyType != typeof(Type))
                return property;
            return property;
        }
    }

}
