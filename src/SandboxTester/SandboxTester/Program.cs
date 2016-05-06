using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace SandboxTester
{
    // The Sandboxer class needs to derive from MarshalByRefObject so that we can create it in another AppDomain and refer to it from the default AppDomain.
    public class Sandboxer : MarshalByRefObject
    {
        private const string PathToUntrusted = @"..\..\..\..\SandboxTesterUntrusted\bin\Debug";
        private const string UntrustedAssembly = "SandboxTesterUntrusted";
        private const string UntrustedClass = "SandboxTesterUntrusted.TestCode";
        private const string EntryPoint = "Test";

        public static void Main()
        {
            // Setting the AppDomainSetup. It is very important to set the ApplicationBase to a folder other than the one in which the sandboxer resides.
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = Path.GetFullPath(PathToUntrusted)
            };

            // Setting the permissions for the AppDomain. We give the permission to execute and to read/discover the location where the untrusted code is loaded.
            var permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            // Microsoft Dynamics CRM allows outbound calls to a limited pattern of hostnames, source: https://msdn.microsoft.com/en-us/library/gg334752.aspx#Anchor_0
            permSet.AddPermission(new WebPermission(NetworkAccess.Connect, new Regex(@"^http[s]?://(?!((localhost[:/])|(\[.*\])|([0-9]+[:/])|(0x[0-9a-f]+[:/])|(((([0-9]+)|(0x[0-9A-F]+))\.){3}(([0-9]+)|(0x[0-9A-F]+))[:/]))).+", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant)));
            permSet.Assert();

            // We want the sandboxer assembly's strong name, so that we can add it to the full trust list.
            var fullTrustAssembly = typeof(Sandboxer).Assembly.Evidence.GetHostEvidence<StrongName>();

            // Now we have everything we need to create the AppDomain, so let's create it.
            var newDomain = AppDomain.CreateDomain("Sandbox", null, appDomainSetup, permSet, fullTrustAssembly);

            // Use CreateInstanceFrom to load an instance of the Sandboxer class into the new AppDomain. 
            var handle = Activator.CreateInstanceFrom(newDomain, typeof(Sandboxer).Assembly.ManifestModule.FullyQualifiedName, typeof(Sandboxer).FullName);

            // Unwrap the new domain instance into a reference in this domain and use it to execute the untrusted code.
            var newDomainInstance = (Sandboxer)handle.Unwrap();
            newDomainInstance.ExecuteUntrustedCode(UntrustedAssembly, UntrustedClass, EntryPoint);
        }

        public void ExecuteUntrustedCode(string assemblyName, string typeName, string entryPoint)
        {
            // Load the MethodInfo for a method in the new Assembly. This might be a method you know, or you can use Assembly.EntryPoint to get to the main function in an executable.
            var target = Assembly.Load(assemblyName).GetType(typeName).GetMethod(entryPoint);

            try
            {
                // Now invoke the method.
                var retVal = (bool)target.Invoke(null, new object[] {});
                Console.WriteLine($"Succesful run, result: {retVal}");
                Console.ReadLine();
            }
            catch (Exception exception)
            {
                // When we print informations from a SecurityException extra information can be printed if we are calling it with a full-trust stack.
                new PermissionSet(PermissionState.Unrestricted).Assert();
                Console.WriteLine("SecurityException caught:");
                Console.WriteLine(exception.ToString());
                CodeAccessPermission.RevertAssert();
                Console.ReadLine();
            }
        }
    }
}
