﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using PseudoDynamic.Terraform.Plugin.Protocols.v6;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal static class PluginServerEndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapTerraformPluginServer(this IEndpointRouteBuilder builder)
        {
            builder.MapGrpcService<ProviderAdapter>();
            return builder;
        }
    }
}
