# Sandboxable
Enables your project to utilize functionality provided by other (Microsoft) libraries that normally are not available in a Partial Trust environment like the Microsoft Dynamics CRM sandbox process.

**Build status**  
[![Build status](https://ci.appveyor.com/api/projects/status/4l2yfcexv066van5/branch/master?svg=true)](https://ci.appveyor.com/project/eNeRGy164/sandboxable/branch/master)

## Sandboxing 
Sandboxing is the practice of running code in a restricted security environment, which limits the access permissions 
granted to the code. For example, if you have a managed library from a source you do not completely trust, you should not 
run it as fully trusted. Instead, you should place the code in a sandbox that limits its permissions to those that you expect it 
to need.

You can read more on this in the article [How to: Run Partially Trusted Code in a Sandbox][1]
If you encounter a .NET sandbox today chances are it's running with [Security-Transparent Code, Level 2][2]

### Examples of software using sandboxes 
* Microsoft Dynamics CRM (Online) Plug-ins and custom workflow activities ([Plug-in Trusts][3])

## The problem 
As developers we use a lot of library code like NuGet packages because we don't want to reinvent the wheel. The 
downside is that most of these libraries are not written with a sandbox process in mind. 

When we embed these libraries to our code in the sandbox we encounter 2 common issues

1.  The code contains security critical code and will fail to load with a `TypeLoadException` or will throw an `SecurityException` at runtime
2.  The package references another package that contains security critical code and even though the code might not even be used it will trigger one of the exceptions mentioned above 

### Problematic constructs 
*  Calling native code

    ```csharp
    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CryptDestroyHash(IntPtr hashHandle);
    ```
*  Override `SecurityCritical` properties of an object like `Exception`

    ```csharp
    public override void GetObjectData(SerializationInfo info, StreamingContext context) { 
        ... 
    } 
    ```
    Where `Exception` has the following attributes on this method
     
    ```csharp
    [System.Security.SecurityCritical]
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context) 
    {
        ...
    }
    ```

*  Serialize non-public classes, fields or properties

## The solution
When we encounter a NuGet package that fails to load or execute in the sandbox and it's source is available we make a *Sandboxable* copy of it.  
This is done by eliminating the offending code in a way that is the least obtrusive and publish this version to NuGet.

The base rules are:

*  Keep the code changes as small as possible
*  Prefix all namespaces with Sandboxable
*  Eliminate offending NuGet dependencies
*  If a new dependency is needed it will on a sandbox friendly NuGet package
 
## Sandbox unfriendly NuGet packages
These packages need modification

A checked box means there is a Sandboxable alternative available in this project
- [x] Hyak.Common
  * Removed dependencies on Microsoft.Bcl, Microsoft.Bcl.Async, Microsoft.Bcl.Build and Microsoft.Net.Http
- [x] Windows.Azure.Common
  * Changed dependency from Hyak.Common to Sandboxable.Hyak.Common
  * Removed dependency on Microsoft.WindowsAzure.Common.Dependencies
- [ ] Microsoft.Azure.KeyVault 
  * Changed dependency from Windows.Azure.Common to Sandboxable.Windows.Azure.Common
  * **At this moment severely limited to retrieving secrets only**
- [x] Microsoft.Azure.Management.KeyVault
  * Changed dependency from Windows.Azure.Common to Sandboxable.Windows.Azure.Common
- [x] Microsoft.IdentityModel.Clients.ActiveDirectory
  * Changed certificate management to BouncyCastle.Crypto.dll, slightly changing the public API
- [x] Microsoft.WindowsAzure.Storage
  * Removed using native MD5 methods

## Sandbox friendly NuGet packages
In our experience these packages don't need to be modified to be used in a sandbox
- [x] BouncyCastle.Crypto.dll
- [x] Microsoft.Azure.KeyVault.Core
- [x] Microsoft.Data.Edm
- [x] Microsoft.Data.OData
- [x] Microsoft.Data.Services.Client
- [x] Newtonsoft.Json
- [x] System.Spatial

[1]: https://msdn.microsoft.com/en-us/library/bb763046(v=vs.110).aspx
[2]: https://msdn.microsoft.com/en-us/library/dd233102(v=vs.110).aspx
[3]: https://msdn.microsoft.com/en-us/library/gg334752.aspx#Anchor_0 
