﻿namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public class PluginServerTests
    {
        //[Fact]
        //public async Task Test()
        //{
        //    using var host = await new HostBuilder()
        //        .ConfigureWebHostDefaults(builder => builder
        //            .UseUrls("http://127.0.0.1:0")
        //            .UseKestrel(options => options.ConfigureEndpointDefaults(endpoints => endpoints.Protocols = HttpProtocols.Http2))
        //            .ConfigureServices(services => services.AddTerraformPlugin())
        //            .Configure(app =>
        //            {
        //                app.UseRouting();
        //                app.UseEndpoints(endpoints => endpoints.MapTerraformPluginServer());
        //            }))
        //        .StartAsync();

        //    var pluginServer = host.Services.GetRequiredService<IPluginServer>();
        //    var serverAddress = $"{pluginServer.ServerAddress.Host}:{pluginServer.ServerAddress.Port}";
        //    var providerName = "registry.terraform.io/pseudo-dynamic/debug";

        //    var terraformCommand = new TerraformCommand()
        //    {
        //        WorkingDirectory = "TerraformProjects/init",
        //        TerraformReattachProviders = {
        //            { providerName, new TerraformReattachProvider(new TerraformReattachProviderAddress(serverAddress)) }
        //        }
        //    };

        //    var init = terraformCommand.Init();
        //    var apply = terraformCommand.Apply();
        //}
    }
}