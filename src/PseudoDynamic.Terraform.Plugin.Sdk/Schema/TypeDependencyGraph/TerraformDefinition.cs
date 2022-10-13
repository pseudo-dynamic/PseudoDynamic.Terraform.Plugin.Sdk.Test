﻿using System.Runtime.CompilerServices;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    internal abstract record class TerraformDefinition
    {
        /// <summary>
        /// Prevents: "RCS1132:Remove redundant overriding member." from Roslynator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal static T PreventRCS1036<T>(T input) => input;

        public abstract TerraformDefinitionType DefinitionType { get; }

        protected internal abstract void Visit(TerraformDefinitionVisitor visitor);
    }
}