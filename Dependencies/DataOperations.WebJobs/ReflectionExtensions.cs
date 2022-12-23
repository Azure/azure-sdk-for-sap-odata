using System.Reflection;
namespace DataOperations.WebJobs
{
    public static class ReflectionExtensions 
    {
        internal static List<Assembly> GetAllAssemblies(this Assembly mainAsm)
        {
            // USE REFLECTION TO FIND ALL OF THE referenced libs IBaseDTOWithIDAndETag and register them with the DI Container
            List<Assembly> listOfAssemblies = new List<Assembly>();
            listOfAssemblies.Add(mainAsm);
            foreach (var refAsmName in mainAsm.GetReferencedAssemblies())
            {
                listOfAssemblies.Add(Assembly.Load(refAsmName));
            }
            return listOfAssemblies;
        }
        internal static List<Type> GetMatchingTypesForInterface(this Assembly ass, string InterfaceName)
        {
            // Find all types that implement a given interface.
            return ass.GetTypes()
                .Where(x => !x.IsAbstract && x.IsClass && x.GetInterface(InterfaceName) != null)
                .ToList();
        }

    }
}
