﻿namespace PseudoDynamic.Terraform.Plugin.Schema
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class NameAttribute : Attribute
    {
        public string Name { get; }

        public NameAttribute(string name) => Name = name;
    }
}
