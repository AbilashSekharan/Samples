using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionOnlyLoadInDifferentAppdomain
{
    class Program
    {
        static void Main(string[] args)
        {
            SeperateAppDomainAssemblyLoader
               appDomainAssemblyLoader =
               new SeperateAppDomainAssemblyLoader();

            string bo = @"D:\NeogenRootRepository\Neogen\Solution\Release\Sagitec.BusinessObjects.dll";

            FileInfo assemblyFile = new FileInfo(bo);
            string assemblyName = appDomainAssemblyLoader.LoadAssemblies(assemblyFile);

            Console.WriteLine("Assembly Name:-");
            Console.WriteLine(assemblyName);
            Console.WriteLine("----------------------");
            Console.WriteLine("--> Namespaces");
            List<string> ns = appDomainAssemblyLoader.GetNameSpaces(assemblyName);
            for (int i = 0; i < ns.Count; i++)
            {
                Console.WriteLine("* {0}", ns[i]);
                Console.WriteLine("----> Classes");
                List<string> className = appDomainAssemblyLoader.GetClassNames(ns[i], assemblyName);
                for (int j = 0; j < className.Count; j++)
                {
                    Console.WriteLine("# {0}", className[j]);
                }
            }

            appDomainAssemblyLoader.Dispose();
            Console.ReadLine();
        }
    }
}
