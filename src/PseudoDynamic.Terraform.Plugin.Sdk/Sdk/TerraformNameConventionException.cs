﻿using System.Runtime.Serialization;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class TerraformNameConventionException : Exception
    {
        public static void EnsureProviderNameConvention(string providerName)
        {
            var lastNamePart = providerName.Split("/").Last();

            if (!string.Equals(lastNamePart, SnakeCaseConvention.Default.Format(lastNamePart), StringComparison.InvariantCulture)
                && !string.Equals(lastNamePart, KebabCaseConvention.Default.Format(lastNamePart), StringComparison.InvariantCulture)) {
                throw new TerraformNameConventionException($"A provider name part \"{lastNamePart}\" must be snake_case or kebab-case");
            }
        }

        public static void EnsureResourceTypeNameConvention(string resourceName)
        {
            if (!string.Equals(resourceName, SnakeCaseConvention.Default.Format(resourceName), StringComparison.InvariantCulture)) {
                throw new TerraformNameConventionException($"A provider name part \"{resourceName}\" must be snake_case");
            }
        }

        public TerraformNameConventionException()
        {
        }

        public TerraformNameConventionException(string? message) : base(message)
        {
        }

        public TerraformNameConventionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TerraformNameConventionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
