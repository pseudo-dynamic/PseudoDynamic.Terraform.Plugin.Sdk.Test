﻿using Microsoft.Extensions.DependencyInjection;
using PseudoDynamic.Terraform.Plugin.Protocols;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public abstract record PluginServerSpecificationBase<TServerSpecification, TProviderFeatures> : IPluginServerSpecification
        where TServerSpecification : PluginServerSpecificationBase<TServerSpecification, TProviderFeatures>
        where TProviderFeatures : ProviderFeaturesBase
    {
        /// <summary>
        /// The intended plugin protocol version to be used by the gRPC server.
        /// </summary>
        public abstract PluginProtocol Protocol { get; }

        /// <summary>
        /// By default, HTTPS is enabled, which is required by Terraform in production environment.
        /// However, if this value is set to <see langword="true"/>, HTTP is used instead and the
        /// instructions for Terraform using this debug instance are displayed in the console output.
        /// </summary>
        public bool IsDebuggable { get; init; }

        internal (string ProviderName, List<Action<TProviderFeatures>> Delegates)? ProviderConfigurations { get; private set; }

        internal PluginServerSpecificationBase()
        {
        }

        public TServerSpecification ConfigureProvider(string providerName, Action<TProviderFeatures> configure)
        {
            ProviderConfigurations = (providerName, ProviderConfigurations?.Delegates ?? new List<Action<TProviderFeatures>>());
            ProviderConfigurations.Value.Delegates.Add(configure);
            return (TServerSpecification)this;
        }
    }

    public interface IPluginServerSpecification
    {
        public static ProtocolV5 NewProtocolV5() => new ProtocolV5();
        public static ProtocolV6 NewProtocolV6() => new ProtocolV6();

        /// <summary>
        /// The intended plugin protocol version to be used by the gRPC server.
        /// </summary>
        PluginProtocol Protocol { get; }

        /// <summary>
        /// By default, HTTPS is enabled, which is required by Terraform in production environment.
        /// However, if this value is set to <see langword="true"/>, HTTP is used instead and the
        /// instructions for Terraform using this debug instance are displayed in the console output.
        /// </summary>
        bool IsDebuggable { get; }

        public sealed record ProtocolV5 : PluginServerSpecificationBase<ProtocolV5, ProtocolV5.ProviderFeatures>
        {
            public override PluginProtocol Protocol => PluginProtocol.V5;

            public class ProviderFeatures : ProviderFeaturesBase, IProviderFeature.IProvisionerFeature
            {
                IProviderFeature IProviderFeature.IProvisionerFeature.ProviderFeature => this;

                public ProviderFeatures(IServiceCollection services) : base(services)
                {
                }
            }
        }

        public sealed record ProtocolV6 : PluginServerSpecificationBase<ProtocolV6, ProtocolV6.ProviderFeatures>
        {
            public override PluginProtocol Protocol => PluginProtocol.V5;

            public class ProviderFeatures : ProviderFeaturesBase
            {
                public ProviderFeatures(IServiceCollection services) : base(services)
                {
                }
            }
        }
    }

    public abstract class ProviderFeaturesBase : IProviderFeature, IProviderFeature.IResourceFeature, IProviderFeature.IDataSourceFeature
    {
        public IServiceCollection Services { get; }

        IProviderFeature IProviderFeature.IResourceFeature.ProviderFeature => this;

        IProviderFeature IProviderFeature.IDataSourceFeature.ProviderFeature => this;

        internal ProviderFeaturesBase(IServiceCollection services) =>
            Services = services ?? throw new ArgumentNullException(nameof(services));
    }
}