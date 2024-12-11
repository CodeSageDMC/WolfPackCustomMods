// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Eco.Shared.Localization;
    using Eco.Shared.Text;
    using Eco.Shared.Utils;
    using JetBrains.Annotations;

    /// <summary>
    /// This context class may be used for mismatches detection.
    /// It configured with <see cref="detectors"/> which may be used to detect mismatches for different types using custom logic.
    /// Detectors implements <see cref="IMismatchDetector"/> interface and receives call to <see cref="IMismatchDetector.DetectMismatches"/> method with context parameter which
    /// then may be used for recursive mismatch detection (for properties or for child objects).
    /// You can also set <see cref="OneName"/> and <see cref="OtherName"/> for better logging if you're looking for mismatches between some specific objects.
    /// </summary>
    public class MismatchDetectionContext
    {
        private const int maxDepth = 20;

        private readonly HashSet<object>                     visited = new HashSet<object>();
        private readonly Dictionary<Type, IMismatchDetector> detectors = new Dictionary<Type, IMismatchDetector>();

        private int depth; // mismatch detection depth, used to avoid StackOverflowException

        public readonly List<IMemberMismatchResolver> MemberMismatchResolvers = new List<IMemberMismatchResolver>();

        public string                        OneName   { get; set; } = "one";
        public string                        OtherName { get; set; } = "other";

        /// <summary>
        /// Adds mapping between <paramref name="detectorType"/> and <paramref name="detector"/>.
        /// It will be used for all instances of this type and if no direct mapping exists for derived type.
        /// </summary>
        [PublicAPI] public void AddDetector(Type detectorType, IMismatchDetector detector) => this.detectors[detectorType] = detector;
        /// Syntax sugar for <see cref="AddDetector(Type, IMismatchDetector)"/>
        [PublicAPI] public void AddDetector<T>(IMismatchDetector<T> detector) => this.detectors[typeof(T)] = detector;
        /// Syntax sugar for <see cref="AddDetector(Type, IMismatchDetector)"/>
        [PublicAPI] public void AddDetector<T>(IMismatchDetector detector) => this.detectors[typeof(T)] = detector;
        /// <summary> Adds mapping between <typeparamref name="T"/> and <see cref="SkipMismatchDetector"/> which effectively skips any mismatches for instances of this type (and not explicitly mapped instances assignable from this type). </summary>
        [PublicAPI] public void SkipMismatches<T>() => this.detectors[typeof(T)] = SkipMismatchDetector.Instance;

        /// <summary> Detects mismatches between <paramref name="one"/> and <paramref name="other"/> using standard Equals comparison and registered custom <see cref="detectors"/>. </summary>
        public InfoBuilder DetectMismatches(object one, object other)
        {
            if (Equals(one, other))
                return null;

            if (one == null)
                return new InfoBuilder().AppendLineLoc($"{this.OneName} is null, but {this.OtherName} is not null");
            if (other == null)
                return new InfoBuilder().AppendLineLoc($"{this.OneName} is not null, but {this.OtherName} is null");

            var type = one.GetType();
            var isTrivialObject = type.IsPrimitive || type == typeof(string) || type == typeof(Guid);
            // add to visited (if not yet) and skip already visited (check only non-trivial objects)
            if (!(isTrivialObject || type.IsValueType) && !this.visited.Add(one))
                return null;

            if (type != other.GetType())
                return new InfoBuilder().AppendLineLocStr($"type mismatch, {this.OneName}: {one.GetType()} {this.OtherName}: {other.GetType()}");

            // skip trivial objects from advanced check (value types and strings)
            return isTrivialObject ? this.ReportValueMismatch(new InfoBuilder(), one, other) : this.DetectValueMismatches(type, one, other);
        }


        /// <summary> Detects mismatches between two values of same <paramref name="type"/>. </summary>
        private InfoBuilder DetectValueMismatches(Type type, object one, object other)
        {
            try
            {
                if (++this.depth > maxDepth)
                    return new InfoBuilder().AppendLineLoc($"mismatch checking is too deep ({this.depth}), skip in depth check.");

                // try to get registered detector for this type and use it for mismatches detection
                if (this.TryGetMismatchDetector(type, out var detector))
                    return detector.DetectMismatches(one, other, this);

                // if value is IEnumerable then use special handling
                if (one is IEnumerable enumerable)
                    return this.DetectMismatches(enumerable, (IEnumerable)other);

                // compare object members
                return this.DetectMembersMismatches(one, other, typeof(object));
            }
            finally
            {
                this.depth--;
            }
        }

        /// <summary> Reports value mismatch. </summary>
        private InfoBuilder ReportValueMismatch(InfoBuilder infoBuilder, object one, object other)
        {
            try
            {
                return infoBuilder.AppendLineLocStr($"value mismatch, {this.OneName}: <{one}> {this.OtherName}: <{other}>");
            }
            catch (Exception ex)
            {
                return infoBuilder.AppendLineLocStr($"value mismatch, failed to stringify values due to an exception: {ex.Message}");
            }
        }

        /// <summary> Recursively for <paramref name="type"/> and all of it base types checks for registered detector and returns it if found. </summary>
        private bool TryGetMismatchDetector(Type type, out IMismatchDetector detector)
        {
            // check if type is valid
            if (type == null || type == typeof(object))
            {
                detector = null;
                return false;
            }

            // try to get registered detector for this type or one of base types and use it for mismatches detection
            return this.detectors.TryGetValue(type, out detector) || this.TryGetMismatchDetector(type.BaseType, out detector);
        }

        /// <summary> Detect mismatches between two <see cref="IEnumerable"/> values. </summary>
        private InfoBuilder DetectMismatches(IEnumerable one, IEnumerable other)
        {
            var  infoBuilder        = new InfoBuilder();
            var  instanceEnumerator = one.GetEnumerator();
            var  prefabEnumerator   = other.GetEnumerator();
            bool instanceHasNext, prefabHasNext;
            var  index = 0;
            // try to move next both enumerators, used & to ensure both MoveNext calls executed
            while ((instanceHasNext = instanceEnumerator.MoveNext()) & (prefabHasNext = prefabEnumerator.MoveNext()))
                infoBuilder.AddSection(Localizer.NotLocalized($"[{index}]"), this.DetectMismatches(instanceEnumerator.Current, prefabEnumerator.Current));

            // instance collection wasn't fully consumed, report mismatch
            if (instanceHasNext)
                return infoBuilder.AppendLineLoc($"collection size mismatch, {this.OneName} has more items than {this.OtherName}");

            // prefab collection wasn't fully consumed, report mismatch
            if (prefabHasNext)
                return infoBuilder.AppendLineLoc($"collection size mismatch, {this.OtherName} has more items than {this.OneName}");

            // both collections fully consumed, nothing to report
            return infoBuilder;
        }

        /// <summary> Detects members mismatches until optional base type. If <paramref name="untilBaseType"/> not specified then it will members for whole hierarchy. </summary>
        public InfoBuilder DetectMembersMismatches(object one, object other, Type untilBaseType = null)
        {
            var infoBuilder = new InfoBuilder();
            // compare instance and prefab components member by member (only interested in properties and fields)
            foreach (var member in one.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                // only check members before MonoBehavior
                if ((untilBaseType == null || untilBaseType.IsAssignableFrom(member.DeclaringType) && member.DeclaringType != untilBaseType) && !this.ShouldIgnoreMember(one, member))
                    this.AddMemberMismatchInfo(infoBuilder, member, one, other);
            }

            return infoBuilder;
        }

        /// <summary> Detects field with name <paramref name="fieldName"/> mismatches with <see cref="DetectMemberMismatches"/> and adds it as section to <paramref name="infoBuilder"/>. </summary>
        [PublicAPI] public void AddFieldMismatchInfo(InfoBuilder infoBuilder, string fieldName, object one, object other) =>
            this.AddMemberMismatchInfo(infoBuilder, one.GetType().GetField(fieldName), one, other);

        /// <summary> Detects property with name <paramref name="propertyName"/> mismatches with <see cref="DetectMemberMismatches"/> and adds it as section to <paramref name="infoBuilder"/>. </summary>
        [PublicAPI] public void AddPropertyMismatchInfo(InfoBuilder infoBuilder, string propertyName, object one, object other) =>
            this.AddMemberMismatchInfo(infoBuilder, one.GetType().GetProperty(propertyName), one, other);

        /// <summary> Detects <paramref name="member"/> mismatches with <see cref="DetectMemberMismatches"/> and adds it as section to <paramref name="infoBuilder"/>. </summary>
        public void AddMemberMismatchInfo(InfoBuilder infoBuilder, MemberInfo member, object one, object other) =>
            infoBuilder.AddSection(Localizer.NotLocalized($"{(member as FieldInfo)?.FieldType ?? (member as PropertyInfo)?.PropertyType} {member.Name}"), this.DetectMemberMismatches(member, one, other));

        /// <summary> Detects mismatches for <paramref name="member"/> values for <paramref name="one"/> and <paramref name="other"/>. </summary>
        private InfoBuilder DetectMemberMismatches(MemberInfo member, object one, object other)
        {
            // get prefab value (or exception)
            var (prefabValue, prefabException)     = this.GetValueOrException(member, other);
            if (other is IMemberMismatchDetectionAware resolver && resolver.ShouldIgnoreMemberMismatch(member, prefabValue, prefabException))
                return new InfoBuilder();

            // get instance value (or exception)
            var (instanceValue, instanceException) = this.GetValueOrException(member, one);

            // if both values was get without exception then just compare them
            if (instanceException == null && prefabException == null)
                return this.DetectMismatches(instanceValue, prefabValue);
            var infoBuilder = new InfoBuilder();
            // if both thrown exception and exception is same then assume they're equal
            if (instanceException != null && prefabException != null && instanceException.GetType() == prefabException.GetType())
                return infoBuilder;
            if (prefabException == null)
                return infoBuilder.AppendLineLoc($"{this.OneName} thrown exception when {this.OtherName} returned value.");
            if (instanceException == null)
                return infoBuilder.AppendLineLoc($"{this.OtherName} thrown exception when {this.OneName} returned value.");
            return infoBuilder.AppendLineLoc($"Mismatch between type of {this.OneName}  exception and {this.OtherName}  exception for value: {instanceException.GetType()} != {prefabException.GetType()}.");
        }

        /// <summary> Checks if member should be ignored taking into account both standard checks and <see cref="MemberMismatchResolvers"/>. </summary>
        private bool ShouldIgnoreMember(object instance, MemberInfo member)
        {
            // only check mismatches for writable fields and properties
            if (member is FieldInfo fieldInfo && (fieldInfo.IsInitOnly || fieldInfo.IsLiteral || fieldInfo.IsBackingField()))
                return true;

            if (member is PropertyInfo propertyInfo && !propertyInfo.CanWrite)
                return true;

            // skip members which asked to be ignored by pool issue resolver
            return this.MemberMismatchResolvers.Any(x => x.ShouldIgnoreMember(instance, member));
        }

        /// <summary> Returns either (value, null) for member or if <see cref="Exception"/> thrown when get member then returns (null, exception). </summary>
        private (object, Exception) GetValueOrException(MemberInfo member, object obj)
        {
            try
            {
                object value;
                switch (member)
                {
                    case PropertyInfo propertyInfo:
                        value = propertyInfo.GetValue(obj);
                        break;
                    case FieldInfo fieldInfo:
                        value = fieldInfo.GetValue(obj);
                        break;
                    default:
                        value = null;
                        break;
                }

                // nullify missing/unassigned Unity object, because if you compare it with null as `object` then it won't use Unity Object special check.
                if (value is UnityEngine.Object unityObject && unityObject == null)
                    value = null;

                return (value, null);
            }
            catch (Exception ex)
            {
                return (null, ex);
            }
        }

        /// <summary> Resets context to initial state which them may be used again. </summary>
        public void Reset() => this.visited.Clear();
    }
}