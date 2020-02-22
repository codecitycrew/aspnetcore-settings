# Introduction

Asp.net Core Settings is an open-source library designed to quickly access and save settings using latest .net core technlogy.

# Get Started

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSettings();
    
    var mySetting = services.Setting<MySetting>();
    
    string applicationName = mySetting.ApplicationName;
}
```

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    var myConfigSetting = app.Setting<MyConfigSetting>();
}
```
