using System;
using System.IO;
using System.Linq;
using System.Text;
using C5;
using Mono.Cecil;
using Thinklouder.Testability.Metrics.Asm;

namespace Thinklouder.Testability.Metrics
{
    public class ClrClassRepository : IClassRepository
    {
        //private static Dictionary<string, ClassInfo> classes;
        private IDictionary<string, ClassInfo> classes = new HashDictionary<string, ClassInfo>();
        private string assemblyPath;
        private AssemblyDefinition assemblyDefinition;

        public ClrClassRepository()
        {
        }

        // TODO, extract to a path parser
        public ClrClassRepository(string assemblyPath)
        {
            this.assemblyPath = assemblyPath;
        }

        // Get ClassInfo according to the full name from IDictionary<string, ClassInfo> classes;
        // the key looks like: System.Object
        public ClassInfo GetClass(string className)
        {
            string name = className.Replace("+", "/");
            if (string.IsNullOrEmpty(assemblyPath))
            {
                // IsolatedAppDomainHost: D:

                var friendlyName = AppDomain.CurrentDomain.FriendlyName;
                var start = friendlyName.IndexOf(": ");
                var path = friendlyName.Substring(start + 2, friendlyName.Length - start - 2);
                assemblyPath = path;
            }

            ClassInfo clazz;
            if (classes.Count > 0 && classes.Find(name, out clazz))
            {
                return clazz;
            }

            AssemblyDefinition asm;
            try
            {
                asm = LoadAssembly(this.assemblyPath);
            }
            catch (Exception ex)
            {
                throw new LoadAssemblyException(assemblyPath, ex);
            }

            if (!TryGetClass(asm.MainModule, name, out clazz))
            {
                throw new ClassNotFoundException(name);
            }
            return clazz;
        }

        private bool TryGetClass(ModuleDefinition module, string className, out ClassInfo clazz)
        {
            var visitor = new ModuleInfoVisitor(this, className);
            module.Accept(visitor);
            clazz = visitor.GetClassInfo();
            return clazz != null;
        }

        public void AddClass(ClassInfo classInfo)
        {
            if (!classes.Contains(classInfo.Name))
            {
                classes.Add(classInfo.Name, classInfo);
            }
        }

        public bool ExistClass(string className)
        {
            return classes.Contains(className);
        }

        public string ScopeName { get; private set; }

        private AssemblyDefinition LoadAssembly(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Assembly not found : ", path);
            }

            var stream = File.OpenRead(path);
            var asm = AssemblyFactory.GetAssembly(stream);
            this.ScopeName = asm.MainModule.Name;
            var references = asm.MainModule.AssemblyReferences;
            return asm;
        }

        //private ClassInfo ParseAssembly(Stream assemblyBytes)
        //{
        //    try
        //    {
        //        //AssemblyDefinition assemblyDefinition = new AssemblyDefinition()
        //        //StructureReader
        //        AssemblyDefinition assemblyDefinition = AssemblyFactory.GetAssembly(assemblyBytes);

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
    }
}
