using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionOnlyLoadInDifferentAppdomain
{
    public class SeperateAppDomainAssemblyLoader
    {

        AppDomain workingAppDomain;

        public SeperateAppDomainAssemblyLoader()
        {
            workingAppDomain = BuildChildDomain(AppDomain.CurrentDomain);
            AssemblyResolver.Register(workingAppDomain);
        }

        public string LoadAssemblies(FileInfo assemblyLocation)
        {
            try
            {
                Type loaderType = typeof(AssemblyLoader);
                if (loaderType.Assembly != null)
                {
                    var loader = (AssemblyLoader)workingAppDomain.CreateInstanceFrom(loaderType.Assembly.Location
                                                                                , loaderType.FullName).Unwrap();
                    loader.LoadAssembly(assemblyLocation);
                }
            }
            catch
            {

            }
            return AssemblyName.GetAssemblyName(assemblyLocation.FullName).FullName;
        }

        public List<String> GetNameSpaces(string assemblyName)
        {
            List<String> ns = new List<string>();
            try
            {
                Type loaderType = typeof(AssemblyLoader);
                if (loaderType.Assembly != null)
                {
                    var loader = (AssemblyLoader)workingAppDomain.CreateInstanceFrom(loaderType.Assembly.Location
                                                                                , loaderType.FullName).Unwrap();
                    ns = loader.GetNameSpaces(assemblyName);
                }
            }
            catch
            {

            }
            return ns;
        }

        public List<String> GetClassNames(string nameSpace, string assemblyName)
        {
            List<String> classes = new List<string>();
            try
            {
                Type loaderType = typeof(AssemblyLoader);
                if (loaderType.Assembly != null)
                {
                    var loader = (AssemblyLoader)workingAppDomain.CreateInstanceFrom(loaderType.Assembly.Location
                                                                                , loaderType.FullName).Unwrap();
                    classes = loader.GetClassNames(nameSpace, assemblyName);
                }
            }
            catch
            {

            }
            return classes;
        }

        public List<Type> GetTypes(string nameSpace, string assemblyName)
        {
            List<Type> types = new List<Type>();
            try
            {
                Type loaderType = typeof(AssemblyLoader);
                if (loaderType.Assembly != null)
                {
                    var loader = (AssemblyLoader)workingAppDomain.CreateInstanceFrom(loaderType.Assembly.Location
                                                                                , loaderType.FullName).Unwrap();
                    types = loader.GetTypes(nameSpace, assemblyName);
                }
            }
            catch
            {

            }
            return types;
        }

        private AppDomain BuildChildDomain(AppDomain parentDomain)
        {
            Evidence evidence = new Evidence(parentDomain.Evidence);
            AppDomainSetup setup = parentDomain.SetupInformation;
            return AppDomain.CreateDomain("DiscoveryRegion",
                evidence, setup);
        }

        public void Dispose()
        {
            AppDomain.Unload(workingAppDomain);
        }

        class AssemblyLoader : MarshalByRefObject
        {

            public AssemblyLoader()
            {

            }

            /// <summary>
            /// ReflectionOnlyLoad of single Assembly based on 
            /// the assemblyPath parameter
            /// </summary>
            /// <param name="assemblyPath">The path to the Assembly</param>
            [SuppressMessage("Microsoft.Performance",
                "CA1822:MarkMembersAsStatic")]
            internal List<String> LoadAssemblies(List<FileInfo> assemblyLocations)
            {
                List<String> namespaces = new List<String>();
                try
                {
                    foreach (FileInfo assemblyLocation in assemblyLocations)
                    {
                        Assembly.ReflectionOnlyLoadFrom(assemblyLocation.FullName);
                        //Assembly.LoadFrom(assemblyLocation.FullName);
                    }

                    foreach (Assembly reflectionOnlyAssembly in AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies())
                    {
                        foreach (Type type in reflectionOnlyAssembly.GetTypes())
                        {
                            if (!namespaces.Contains(
                                type.Namespace))
                                namespaces.Add(
                                    type.Namespace);
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    /* Continue loading assemblies even if an assembly
                     * can not be loaded in the new AppDomain. */

                }
                return namespaces;
            }


            [SuppressMessage("Microsoft.Performance",
 "CA1822:MarkMembersAsStatic")]
            internal void LoadAssembly(FileInfo assemblyLocation)
            {
                try
                {
                    Assembly.ReflectionOnlyLoadFrom(assemblyLocation.FullName);
                }
                catch (FileNotFoundException)
                {
                    /* Continue loading assemblies even if an assembly
                     * can not be loaded in the new AppDomain. */

                }

            }

            [SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic")]
            internal List<String> GetNameSpaces(string assemblyName)
            {
                List<String> namespaces = new List<String>();
                try
                {

                    foreach (Assembly reflectionOnlyAssembly in AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies())
                    {
                        if (reflectionOnlyAssembly.FullName.Equals(assemblyName))
                        {
                            foreach (Type type in reflectionOnlyAssembly.GetTypes())
                            {
                                if (!namespaces.Contains(
                                    type.Namespace))
                                    namespaces.Add(
                                        type.Namespace);
                            }
                            break;
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    /* Continue loading assemblies even if an assembly
                     * can not be loaded in the new AppDomain. */

                }
                return namespaces;
            }

            [SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic")]
            internal List<String> GetClassNames(string nameSpace, string assemblyName)
            {
                List<String> classes = new List<String>();
                try
                {

                    foreach (Assembly reflectionOnlyAssembly in AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies())
                    {
                        if (reflectionOnlyAssembly.FullName.Equals(assemblyName))
                        {
                            foreach (Type type in reflectionOnlyAssembly.GetTypes())
                            {
                                if (type.Namespace.Equals(nameSpace))
                                {
                                    classes.Add(type.FullName);
                                }
                            }
                            break;
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    /* Continue loading assemblies even if an assembly
                     * can not be loaded in the new AppDomain. */

                }
                return classes;
            }

            [SuppressMessage("Microsoft.Performance",
        "CA1822:MarkMembersAsStatic")]
            internal List<Type> GetTypes(string nameSpace, string assemblyName)
            {
                List<Type> classes = new List<Type>();
                try
                {

                    foreach (Assembly reflectionOnlyAssembly in AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies())
                    {
                        if (reflectionOnlyAssembly.FullName.Equals(assemblyName))
                        {
                            foreach (Type type in reflectionOnlyAssembly.GetTypes())
                            {
                                if (type.Namespace.Equals(nameSpace))
                                {
                                    classes.Add(type);
                                }
                            }
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    /* Continue loading assemblies even if an assembly
                     * can not be loaded in the new AppDomain. */

                }
                return classes;
            }

        }

        internal class AssemblyResolver : MarshalByRefObject
        {
            static internal void Register(AppDomain domain)
            {
                AssemblyResolver resolver =
                    domain.CreateInstanceFromAndUnwrap(
                      Assembly.GetExecutingAssembly().Location,
                      typeof(AssemblyResolver).FullName) as AssemblyResolver;

                resolver.RegisterDomain(domain);
            }

            private void RegisterDomain(AppDomain domain)
            {
                domain.AssemblyResolve += ResolveAssembly;
                domain.ReflectionOnlyAssemblyResolve += ResolveAssembly;
                //domain.AssemblyLoad += LoadAssembly;
            }

            private Assembly ResolveAssembly(object sender, ResolveEventArgs args)
            {
                Assembly requestedAssembly = null;

                string referenceDirectory = Path.GetDirectoryName(args.RequestingAssembly.Location);
                string[] assemblyFiles = Directory.GetFiles(referenceDirectory, "*.dll");
                for (int i = 0; i < assemblyFiles.Length; i++)
                {
                    AssemblyName currentAssemblyName = AssemblyName.GetAssemblyName(assemblyFiles[i]);
                    if (currentAssemblyName.FullName.Equals(args.Name))
                    {
                        requestedAssembly = Assembly.ReflectionOnlyLoadFrom(assemblyFiles[i]);
                        break;
                    }
                }
                return requestedAssembly;
            }

            private void LoadAssembly(object sender, AssemblyLoadEventArgs args)
            {
                // implement assembly loading here

                //Assembly.ReflectionOnlyLoad(args.LoadedAssembly.Location);
            }
        }

    }
}
