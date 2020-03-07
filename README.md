# Introduction

Asp.net Core Settings is an open-source library designed to quickly access and save settings using latest .net core technology.

# Install

```ps
Install-Package CodeCityCrew.Settings -Version 1.0.1-alpha
```

### Package Manager Console
```ps
Update-Database -Context SettingDbContext
```

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

```csharp

public class MyService : IMyService
{
    private readonly ISettingService _settingsService;
    
    public MyService(ISettingService settingService)
    {
        _settingsService = settingService;
    }
    
    public string GetApplicationName()
    {
        return _settingsService.As<MySetting>().ApplicationName;
    }
    
    public void SetApplicationName(string applicationName)
    {
        var mySetting = _settingsService.As<MySetting>();
        
        mySetting.ApplicationName = applicationName;
        
        _settingsService.Save(mySetting);
    }
}

```



