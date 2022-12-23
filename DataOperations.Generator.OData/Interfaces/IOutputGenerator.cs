using System.Text;

namespace Generator
{
    public interface IOutputGenerator
    {
        Task<string> GenerateMember(string TemplateName, Dictionary<string, string> replaces = null);
        Task WriteAction(string location, Schema sch, StringBuilder globalActionsSb, Action action);
        Task WriteActionImport(string location, Schema sch, StringBuilder globalActionsSb, ActionImport actionImport);
        Task WriteEntitySetFragment(string location, Schema sch, StringBuilder Sb, EntitySet es, string template);
        Task WriteDTOClassCodeFile(string location, Schema sch, StringBuilder thisClassSb, string className);
        void WriteClassFooter(StringBuilder thisClassSb, string extraFooterLine = "");
        Task WriteClassHeader(StringBuilder thisClassSb, string className, string comment, string template = "ClassHeaderInherits", string InheritsFrom = "BaseDTOWithIDAndETag", string InheritCtorTypeParamName = "", string InheritCtorParamName = "", string Namespace = "Generated");
        Task WriteComplexTypeClass(string location, Schema sch, StringBuilder thisClassSb, ComplexType entity);
        Task WriteEntitySetClass(string location, Schema sch, StringBuilder entitySetSb, EntitySet entSet);
        Task WriteEntityTypeClass(string location, Schema sch, StringBuilder thisClassSb, EntityType entity);
        Task WriteNavigationProperty(StringBuilder thisClassSb, string className, string fqclassname, NavigationProperty prop, Schema sch, NavigationPropertyBinding npb);
        Task WriteProperty(StringBuilder thisClassSb, string className,  Property prop);
        string GetNativePropertyType(string propType, bool Nullable);
        Task WriteCustomCodeClass(string location, Schema sch, StringBuilder globalActionsSb, string className, string comment, string template, string inheritsFrom = "", string inheritCtorTypeParamName = "",  string InjectedObjectType = "", string InjectedObjectParamName = "", string extraFooter = "", string nameSpace = "GeneratedCode", bool staticclass = false, Func<string, Schema, StringBuilder, Task> internalProcessor = null);
        Task WriteTextFragment(StringBuilder diHsb, string v);
        Task WriteProjectFile(string location, Schema sch, string template, string DiskNameSpace);
        Task WriteWebJobsClassCodeFile(string location, Schema sch, StringBuilder thisClassSb, string className);

        string TemplatePath {get;set;}
    }
}