# PersistenceCacheStack
PersistenceCacheStack is a C# library that implements a two levels cache allowing at the same time high performance and persistence.
PersistenceCacheStack uses the **MemoryCache** as the first layer and **Redis** as the second layer.

### What can it be used for?
PersistenceCacheStack can be used in all contexts that require fast access to the cache without sacrificing persistence.
For example, a service that should not lose the cached data after a reboot or an update.

### How it works
PersistenceCacheStack uses two different cache layers, a first layer in-memory using MemoryCache and a second layer on Redis for persistence.
All operations performed in cache will take place on the first in-memory cache layer and will be synchronized with the fire-and-forget pattern, with Redis. 
This logic allows the data to be obtained immediately without waiting for any network latency. On the other hand, the system changes from a strong consistency to eventually consistency in a multi-node context.
In fact, a cached data may not be updated to the version of the other nodes but eventually it will be aligned.

### Architecture
![Alt text](/wiki/img/Architecture.png?raw=true)

### Redis
PersistenceCacheStack uses the **StackExchange.Redis** and **StackExchange.Redis.Extensions** to wrap the CRUD operations towars Redis.
You can find all the references to the projects in this file.

### Installation

To install the **PersistenceCacheStack** library just add the dll reference and its dependencies into your project.

### How to configure it

To configure the library just set access to Redis for persistence.

Here the example of an App.config file

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="redisCacheClient" type="StackExchange.Redis.Extensions.LegacyConfiguration.RedisCachingSectionHandler, StackExchange.Redis.Extensions.LegacyConfiguration" />
  </configSections>
  <redisCacheClient allowAdmin="true" ssl="false" connectTimeout="3000" database="24">
    <serverEnumerationStrategy mode="Single" targetRole="PreferSlave" unreachableServerAction="IgnoreIfOtherAvailable" />
    <hosts>
      <add host="127.0.0.1" cachePort="6379" />
    </hosts>
  </redisCacheClient>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-11.0.0.0" newVersion="11.0.0.0" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>
```

### Serialization
In order to store a class into Redis, that class must be serializable. 
PersistenceCacheStack uses the **PersistenceCacheStackEntity** object to incapsulate the <key, value> couple and checks if the object is actually serializable into the constructor.

### Usage

To use the PersistenceCacheStack library, just instantiate a **PersistenceCacheStackClient** object specifying the type of object it will work on.

```csharp
var persistenceCacheStack = new PersistenceCacheStackClient<T>();
```
at this point the CRUD operations can be performed.
For example, below how to add an object of type string inside the cache with no expiration:

```csharp
var persistenceCacheStack = new PersistenceCacheStackClient<string>();
var addResult = persistenceCacheStack.AddItem("Key", "AddItemString", null);
```

To synchronize the in-memory cache with Redis use the following instruction:

```csharp
persistenceCacheStack.SynchFromRedis();
```

in this way all the persistent objects on Redis will be copied in the first cache layer.

### StackExchange.Redis Reference
https://github.com/StackExchange/StackExchange.Redis

### StackExchange.Redis.Extensions Reference
https://github.com/imperugo/StackExchange.Redis.Extensions

### Contributing
Contributions welcome! Please contact me to join the project.

**Getting started with Git and GitHub**

 * [Setting up Git for Windows and connecting to GitHub](http://help.github.com/win-set-up-git/)
 * [Forking a GitHub repository](http://help.github.com/fork-a-repo/)
 * [The simple guide to GIT guide](http://rogerdudler.github.com/git-guide/)
 * [Open an issue](https://github.com/engineering87/PersistenceCacheStack/issues) if you encounter a bug or have a suggestion for improvements/features

### Licensee
PersistenceCacheStack source code is available under GNU General Public License v3.0, see license in the source.

### Contact
Please contact at francesco.delre.87[at]gmail.com for any details.
