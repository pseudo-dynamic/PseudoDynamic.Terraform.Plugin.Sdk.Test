﻿using System.Data;
using Microsoft.Extensions.Options;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class Provider : IProvider
    {
        public IReadOnlyDictionary<string, ResourceDefinition> ResourceDefinitions => _resourceDefinitions;
        public string FullyQualifiedProviderName { get; }

        /// <summary>
        /// The provider name. Represents the last "/"-separated part of <see cref="FullyQualifiedProviderName"/>.
        /// </summary>
        public string ProviderName { get; }

        /// <summary>
        /// Same as <see cref="ProviderName"/> but snake_case formatted to comply with Terraform naming convention.
        /// </summary>
        public string ResourceProviderName { get; }

        private Dictionary<string, ResourceDefinition> _resourceDefinitions;

        public Provider(IOptionsSnapshot<ProviderOptions> providerOptions)
        {
            var unwrappedProviderOptions = providerOptions.Value ?? throw new ArgumentNullException(nameof(providerOptions));
            _resourceDefinitions = new Dictionary<string, ResourceDefinition>();

            var fullyQualifiedProviderName = unwrappedProviderOptions.FullyQualifiedProviderName;
            FullyQualifiedProviderName = fullyQualifiedProviderName;
            var providerName = fullyQualifiedProviderName.Split("/").Last();
            var snakeCaseProviderName = SnakeCaseConvention.Default.Format(providerName);
            ProviderName = providerName;
            ResourceProviderName = snakeCaseProviderName;

            AddResourceDefinitions(unwrappedProviderOptions.ResourceDefinitions, snakeCaseProviderName);
        }

        private void AddResourceDefinitions(IEnumerable<ResourceDefinition> resourceDescriptors, string providerName)
        {
            foreach (var resourceDefinition in resourceDescriptors) {
                var resourceTypeName = resourceDefinition.ResourceTypeName;
                var fullResourceTypeName = $"{providerName}_{resourceTypeName}";

                if (_resourceDefinitions.ContainsKey(fullResourceTypeName)) {
                    throw new DuplicateNameException($"The resource type name \"{resourceTypeName}\" is already taken");
                }

                var updatedResourceDefinition = resourceDefinition with { ResourceTypeName = fullResourceTypeName };
                _resourceDefinitions.Add(fullResourceTypeName, updatedResourceDefinition);
            }
        }
    }
}
