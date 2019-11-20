# Proxy adapter for Visual Studio (2012, 2013, 2015, 2017 and 2019)

Still under construction...

## How to use

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

3. Ready to use, you have to add shell environemnts named both `HTTPS_PROXY`, `HTTP_PROXY` and  `ALL_PROXY` (Will fallback these names). You may know these're standard unix-like proxy assignment solution.

Format examples: `http://<proxyUser>:<proxyPassword>@proxy.example.com:8080`

![Windows environment dialog](Images/environment-dialog.png)

## License

Apache-v2
