﻿namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    /// <summary>
    /// <para>
    /// Represents a resource that has multiple routes. Some routes are separated in
    /// two stages that belong together.
    /// </para>
    /// <para>
    /// The routes to be named are Plan, Create, Update, Delete and Import. The two
    /// stages that belong together are plan stage and apply stage, short: the resource
    /// lifecycle. The routes that follow this resource lifecycle are: Create, Update and
    /// Delete. Even when both stages belong together, they are not dependent on each other.
    /// Both stages could be started separated by Terraform in terms of time and location.
    /// But normally you should only be able to enter the apply stage when you previously
    /// created an execution plan that is normally only generated by the plan stage of one
    /// of the previously mentioned routes. There is another peculiarity: Each stage is run
    /// in its own process. Those stages can cannot share any context to each other within
    /// this application!
    /// </para>
    /// <para>
    /// The plan stage, that also represents 1:1 the Plan route, looks like this:
    /// <br/>- (new process)
    /// <br/>- <see cref="MigrateState"/> (not called in Create route)
    /// <br/>- <see cref="ReviseState"/> (not called in Create route)
    /// <br/>- <see cref="ValidateConfig"/>
    /// <br/>- <see cref="Plan"/>
    /// </para>
    /// <para>
    /// The apply stage consists almost of the same methods as the plan stage:
    /// <br/>- (new process)
    /// <br/>- <see cref="MigrateState"/> (not called in Create route)
    /// <br/>- <see cref="ValidateConfig"/> (not called in Delete route)
    /// <br/>- <see cref="Plan"/> (not called in Delete route)
    /// <br/>- either <see cref="Create"/> or <see cref="Update"/> (not called when no change) or <see cref="Delete"/>
    /// </para>
    /// <para>
    /// The delete route follows the flow of the plan and apply stage, but as the resource is gonna
    /// be deleted, the plan stage is extended by another method, that is called in a new process:
    /// <br/>- (plan stage)
    /// <br/>- (new process)
    /// <br/>- <see cref="MigrateState"/>
    /// </para>
    /// <br/>The Import route is special as it does not follow the flow of the plan or apply
    /// stage at all:
    /// <br/>- (new process)
    /// <br/>- <see cref="ImportState"/>
    /// <br/>- <see cref="ReviseState"/>
    /// </summary>
    /// <typeparam name="Schema"></typeparam>
    public interface IResource<Schema>
    {
        // ISSUE: should be constructor
        //Task Configure();

        /// <summary>
        /// Allows to migrate the pre-existing state of the resource, that is
        /// stored by Terraform, to a possible new schema version.
        /// This method is always called before the state gets read during plan stage or
        /// before validating the config during apply stage.
        /// </summary>
        Task MigrateState();

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
        Task ReviseState();

        /// <summary>
        /// Validates the user-defined resource inputs that has been made in Terraform.
        /// </summary>
        Task ValidateConfig();

        /// <summary>
        /// Plans the possible result of the resource. It is called twice, once during
        /// plan stage and once during apply stage. During plan stage <see cref="Plan"/>
        /// is dependent on <see cref="MigrateState"/>, <see cref="ReviseState"/> and
        /// <see cref="ValidateConfig"/>, in this order. During apply stage <see cref="Plan"/>
        /// is only dependent on <see cref="MigrateState"/> and <see cref="ValidateConfig"/>.
        /// <see cref="ValidateConfig"/>, in this order.
        /// </summary>
        Task Plan();

        /// <summary>
        /// Creates the resource. During apply stage <see cref="Create"/> is dependent
        /// on <see cref="Plan"/>.
        /// </summary>
        Task Create();

        /// <summary>
        /// Updates the resource. During apply stage <see cref="Update"/> is dependent
        /// on <see cref="Plan"/>.
        /// </summary>
        Task Update();

        /// <summary>
        /// Deletes the resource. During apply stage <see cref="Delete"/> is dependent
        /// on <see cref="MigrateState"/>.
        /// </summary>
        Task Delete();
    }
}
