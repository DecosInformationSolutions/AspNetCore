Decos.AspNetCore
================

[![Build status](https://dev.azure.com/decos/Decos%20Core/_apis/build/status/Decos.AspNetCore%20components)](https://dev.azure.com/decos/Decos%20Core/_build/latest?definitionId=229)

This repository contains custom middleware and other additions for ASP.NET Core.

Decos.AspNetCore.Authorization
------------------------------

[![Nuget](https://img.shields.io/nuget/v/Decos.AspNetCore.Authorization.svg?label=Decos.AspNetCore.Authorization)](https://www.nuget.org/packages/Decos.AspNetCore.Authorization/)

Currently, this package provides middleware that uses the Microsoft Graph API to add claims for every Active Directory group an authenticated user is a member of. For this, it relies on the [Microsoft.Identity.Web] code which was not officially released on NuGet. We have provided a NuGet package for this, with some minor changes to reduce the required dependencies.

### Usage

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ...
    services.AddGraphApiClaims(); // <--
    // ...
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseAuthentication();
    app.UseGraphApiClaims(); // <--
    app.UseMvc();
}
```

[Microsoft.Identity.Web]: https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/tree/master/Microsoft.Identity.Web



Decos.AspNetCore.BackgroundTasks
--------------------------------

[![Nuget](https://img.shields.io/nuget/v/Decos.AspNetCore.BackgroundTasks.svg?label=Decos.AspNetCore.Authorization)](https://www.nuget.org/packages/Decos.AspNetCore.BackgroundTasks/)

This package provides a QueueBackgroundWorkItem implementation as provided in [aspnet/Extensions issue #805](https://github.com/aspnet/Extensions/issues/805).

Use the **IServiceCollection.AddBackgroundTasks** extension to provide an **IBackgroundTaskQueue** for queueing work items and a hosted service that runs the queue.
