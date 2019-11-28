# Proxy adapter for Visual Studio (2012, 2013, 2015, 2017 and 2019)

![ProxyAdapterInstaller](Images/syringe_160.png)

This is HTTP proxy adapter for Visual Studio. It has ability for applying HTTP proxy settings retreive from the environment variable `HTTP_PROXY` or likes.
The manner is likely unix-style, so your unix based tools (Git, MinGW and also) better fitting usages.

It'll resident onto task tray, and you can quickly install only double clicking the icon when you'll each update Visual Studio :)

Still under construction...

## How to use

This installation steps are manually. Current version installation is only double clicking on task tray icon (Skip to next section.)

![Task tray double clicking installation](Images/tasktray-menu.png)

### Manually installation steps

1. Place ProxyAdapter.dll into your Visual Studio IDE environment folder like:
`C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\`

2. Edit devenv.exe.config. it's very large xml file, you can see the element `<system.net>` at the bottom of xml:

```xml
    <system.net>
      <settings>
        <ipv6 enabled="true"/>
      </settings>
    </system.net>
```

Insert proxy entries:

```xml
    <system.net>
      <!-- Insert below -->
      <defaultProxy enabled="true" useDefaultCredentials="false">
        <module type="ProxyAdapter.FromEnvironment, ProxyAdapter"/>
      </defaultProxy>
      <!-- Insert above -->
      <settings>
        <ipv6 enabled="true"/>
      </settings>
    </system.net>
```

### Set environment variables after installed

Ready to use, you have to add environemnt variables be able to named `HTTPS_PROXY`, `HTTP_PROXY` and/or `ALL_PROXY` (Will fallback these names). You may know these're standard unix-like proxy assignment solution.

Format example: `http://<proxyUser>:<proxyPassword>@proxy.example.com:8080`

![Windows environment dialog](Images/environment-dialog.png)

## License

Apache-v2
