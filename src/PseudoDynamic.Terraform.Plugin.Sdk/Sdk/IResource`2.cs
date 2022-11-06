﻿using static PseudoDynamic.Terraform.Plugin.Sdk.Resource;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// <para>
    /// Represents a resource that has multiple routes. Some routes are separated in
    /// two stages that belong together. The next paragraph explains it more detailed.
    /// </para>
    /// <para>
    /// The routes to be named are Validate, Plan, Create, Update, Delete and Import. The
    /// two stages that belong together are plan stage and apply stage, short: the resource
    /// lifecycle. The routes that follow this resource lifecycle are: Create, Update and
    /// Delete. Even when both stages belong together, they are not dependent on each other.
    /// Both stages could be started separated by Terraform in terms of time and location.
    /// But normally you should only be able to enter the apply stage when you previously
    /// created an execution plan that is normally only generated by the plan stage of one
    /// of the previously mentioned routes. There is another peculiarity: Every stage runs
    /// in its own process, and some stages in some routes may also run in more than one
    /// process. So, please keep in mind, that you always loose the context between new
    /// spawning processes.
    /// </para>
    /// <para>
    /// The <b>plan stage</b> looks like this:
    /// <br/>- [routes: Validate, Plan, Create, Update, Delete]
    /// <br/>1. (new process)
    /// <br/>2. <see cref="ValidateConfig"/>
    /// <br/>- [routes: Plan, Create, Update, Delete] (depends on prior steps)
    /// <br/>3. (new process)
    /// <br/>4. <see cref="MigrateState"/> except for [routes: Create]
    /// <br/>5. <see cref="ReviseState"/> except for [routes: Create]
    /// <br/>6. <see cref="Plan"/>
    /// <br/>- [routes: Delete] (depends on prior steps)
    /// <br/>7. (new process)
    /// <br/>8. <see cref="MigrateState"/>
    /// </para>
    /// <para>
    /// The <b>apply stage</b> consists almost of the same methods as the plan stage:
    /// <br/>- [routes: Create, Update, Delete]
    /// <br/>1. (new process)
    /// <br/>2. <see cref="MigrateState"/> except for [routes: Create]
    /// <br/>3. <see cref="ValidateConfig"/> except for [routes: Delete]
    /// <br/>4. <see cref="Plan"/> except for [routes: Delete]
    /// <br/>- [routes: Create] (depends on prior steps)
    /// <br/>5. <see cref="Apply"/>
    /// <br/>- [routes: Update] (depends on prior steps)
    /// <br/>6. <see cref="Apply"/> (may be skipped if TF sees no change)
    /// <br/>- [routes: Delete] (depends on prior steps)
    /// <br/>7. <see cref="Apply"/>
    /// </para>
    /// <br/>The <b>Import route</b> is special as it does not follow the flow of the plan
    /// or apply stage at all:
    /// <br/>- (new process)
    /// <br/>1. <see cref="ImportState"/>
    /// <br/>2. <see cref="ReviseState"/>
    /// </summary>
    /// <typeparam name="Schema"></typeparam>
    /// <typeparam name="ProviderMetaSchema"></typeparam>
    public interface IResource<Schema, ProviderMetaSchema> : IResource
        where Schema : class
        where ProviderMetaSchema : class
    {
        internal static new readonly ProviderAdapter.ResourceGenericAdapter<Schema, ProviderMetaSchema> ResourceAdapter;

        ProviderAdapter.IResourceAdapter IResource.ResourceAdapter => ResourceAdapter;

        /// <summary>
        /// <para>
        /// This type name that is going to be appended to the provider name to comply with the resource name
        /// convention of Terraform. (e.g. <![CDATA["<provider-name>_<type-name>"]]>
        /// </para>
        /// <para>
        /// Do not prepend the provider name by yourself! The name remain unformatted, so please ensure snake_case!
        /// </para>
        /// </summary>
        string TypeName { get; }

        string INameProvider.Name => TypeName;

        /// <summary>
        /// Allows to migrate the pre-existing state of the resource, that is
        /// stored by Terraform, to a possible new schema version.
        /// This method is always called before the state gets read during plan stage or
        /// before validating the config during apply stage.
        /// </summary>
        Task MigrateState(IMigrateStateContext context);

        /// <summary>
        /// This method enables importing a resource. It is then always followed by
        /// <see cref="ReviseState"/>.
        /// </summary>
        Task ImportState();

        /// <summary>
        /// Revises the pre-existing state after it may have been migrated right before
        /// or revises the state after Terraform has just surpassed <see cref="ImportState"/>
        /// for this resource.
        /// <br/>In the first case, this method is only called during plan stage but not
        /// during apply stage. In other words: When Terraform visualized the execution
        /// plan where <see cref="Plan"/> got called the first time during
        /// plan stage, and you then accept the execution plan for applying, this method
        /// won't be called again before the "second" <see cref="Plan"/> during apply stage.
        /// </summary>
        Task ReviseState(IReviseStateContext<Schema, ProviderMetaSchema> context);

        /// <summary>
        /// Validates the user-defined resource inputs that has been made in Terraform.
        /// </summary>
        Task ValidateConfig(IValidateConfigContext<Schema> context);

        /// <summary>
        /// Plans the possible result of the resource. It is called twice, once during
        /// plan stage and once during apply stage. During plan stage <see cref="Plan"/>
        /// is dependent on <see cref="MigrateState"/>, <see cref="ReviseState"/> and
        /// <see cref="ValidateConfig"/>, in this order. During apply stage <see cref="Plan"/>
        /// is only dependent on <see cref="MigrateState"/> and <see cref="ValidateConfig"/>.
        /// <see cref="ValidateConfig"/>, in this order.
        /// </summary>
        Task Plan(IPlanContext<Schema, ProviderMetaSchema> context);

        Task Apply(IApplyContext<Schema, ProviderMetaSchema> context);
    }
}
