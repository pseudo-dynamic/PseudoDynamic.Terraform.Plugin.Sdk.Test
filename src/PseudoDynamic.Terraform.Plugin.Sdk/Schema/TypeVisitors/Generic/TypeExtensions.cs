﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic
{
    internal static class TypeExtensions
    {
        public static bool IsCustomStruct(this Type? type)
        {
            if (type is null || !type.IsValueType || type.IsPrimitive || type.IsEnum || type.IsEquivalentTo(typeof(decimal))) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether type is class or user-defined struct.
        /// </summary>
        /// <param name="type"></param>
        public static bool IsComplexType(this Type type) => type.IsClass || type.IsCustomStruct();

        /// <summary>
        /// Determines if type is a annotated by <see cref="SchemaAttribute"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="schemaAttribute"></param>
        public static bool IsSchemaAnnotated(this Type type, [NotNullWhen(true)] out SchemaAttribute? schemaAttribute)
        {
            schemaAttribute = type.GetCustomAttribute<SchemaAttribute>();
            return schemaAttribute is not null;
        }
    }
}
