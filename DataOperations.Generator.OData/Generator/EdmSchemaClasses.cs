using System.Collections.Generic;
using System.Xml.Serialization;
public static class Utilities
{
	public static string ScrubNameSpace(this Schema sch, string input)
    {
        return input.Replace(sch.Namespace + ".", "");
    }
	
	public static EntitySet GetEntitySet(this EntityType et, Schema sch)
	{
		return sch.EntityContainer.EntitySets.Where(e => e.EntityType == sch.ScrubNameSpace(et.Name)).FirstOrDefault();
	}
	public static EntityType GetEntityType(this EntitySet es, Schema sch)
    {
        return sch.EntityTypes.Where(e => e.Name == sch.ScrubNameSpace(es.EntityType)).FirstOrDefault();
    }
	public static IEnumerable<string> GetKeyProperties(this EntityType et, Schema sch, string quote = "\"")
	{
		// Linq Clause -> Where et.properties.name matches the key property reference names for the entity type
		return et.Key.PropertyRef.Select(e => quote + e.Name + quote);
	}
	public static IEnumerable<NavigationProperty> FindNavigationProperties(this EntityType et, Schema sch)
	{
		return sch.EntityTypes.Where(e => e.Name == sch.ScrubNameSpace(et.Name)).FirstOrDefault().NavigationProperties;
	}
	public static NavigationPropertyBinding FindReferencedNavPropBinding(this NavigationProperty np, Schema sch)
	{
	
		return sch.EntityContainer.EntitySets.Where(e => e.NavigationPropertyBindings.Where(f => f.Path == np.Name).Any()).FirstOrDefault().NavigationPropertyBindings.Where(g => g.Path == np.Name).FirstOrDefault();

	}
	// public static EntitySet FindReferencedEntitySet(this NavigationPropertyBinding np, Schema sch)
	// {
	// 	// firstly get the EntitySet that holds the type reference
	// 	return sch.EntityContainer.EntitySets.Where(e => sch.ScrubNameSpace(e.Name) == np.Target).FirstOrDefault();
	// }
	// public static EntityType FindReferencedEntityType(this NavigationPropertyBinding np, Schema sch)
	// {
	// 	// firstly get the EntitySet that holds the type reference
	// 	var es = np.FindReferencedEntitySet(sch);

	// 	// now lookup the correct type
	// 	return sch.EntityTypes.Where(e => sch.ScrubNameSpace(e.Name) == sch.ScrubNameSpace(es.EntityType)).FirstOrDefault();
	// }
	// public static NavigationProperty GetNavigationData(this NavigationPropertyBinding np, EntitySet ParentEntitySet, Schema sch)
	// {
	// 	// firstly get the EntitySet that holds the type reference
	// 	// var es = np.FindReferencedEntitySet(sch);
	//  // return sch.EntityTypes.Where(e => e.NavigationProperties.Where(n => n.Name == sch.ScrubNameSpace(ParentEntitySet.EntityType)).Any()).FirstOrDefault();
	//  // Relationshipdata --> Helper method to fetch / NavigationProperties[0]/ReferentialConstraint/.Property (Mapped to ReferencedProperty)
	// }
}
[XmlRoot(ElementName="PropertyRef", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class PropertyRef { 

	[XmlAttribute(AttributeName="Name", Namespace="")] 
	public string Name { get; set; } 
}

[XmlRoot(ElementName="Key", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class Key { 

	[XmlElement(ElementName="PropertyRef", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public List<PropertyRef> PropertyRef { get; set; } 
}

[XmlRoot(ElementName="Property", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class Property { 

	[XmlAttribute(AttributeName="Name", Namespace="")] 
	public string Name { get; set; } 

	[XmlAttribute(AttributeName="Type", Namespace="")] 
	public string Type { get; set; } 

	[XmlAttribute(AttributeName="Nullable", Namespace="")] 
	public bool Nullable { get; set; } 

	[XmlElement(ElementName="Annotation", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public List<Annotation> Annotation { get; set; } 

	[XmlAttribute(AttributeName="MaxLength", Namespace="")] 
	public int MaxLength { get; set; } 

	[XmlAttribute(AttributeName="Precision", Namespace="")] 
	public int Precision { get; set; } 

	[XmlAttribute(AttributeName="Scale", Namespace="")] 
	public int Scale { get; set; } 
}

[XmlRoot(ElementName="Annotation", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class Annotation { 

	[XmlAttribute(AttributeName="Term", Namespace="")] 
	public string Term { get; set; } 

	[XmlAttribute(AttributeName="String", Namespace="")] 
	public string String { get; set; } 

	[XmlAttribute(AttributeName="Path", Namespace="")] 
	public string Path { get; set; } 

	[XmlElement(ElementName="Record", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public Record Record { get; set; } 

	[XmlText] 
	public string Text { get; set; } 

	[XmlAttribute(AttributeName="Bool", Namespace="")] 
	public bool Bool { get; set; } 
}

[XmlRoot(ElementName="NavigationProperty", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class NavigationProperty { 
	// Navigation Properties should be relationships, so ideally we want a convenience method
	// to create a relationship from a navigation property.
	public bool IsCollection() {
		return Type.Contains("Collection");
	}
	public bool IsNullable()
	{
		return Nullable;
	}
	

	[XmlAttribute(AttributeName="Name", Namespace="")] 
	public string Name { get; set; } 

	[XmlAttribute(AttributeName="Type", Namespace="")] 
	public string Type { get; set; } 

	[XmlAttribute(AttributeName="Partner", Namespace="")] 
	public string Partner { get; set; } 

	[XmlElement(ElementName="ReferentialConstraint", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public ReferentialConstraint ReferentialConstraint { get; set; } 

	[XmlAttribute(AttributeName="Nullable", Namespace="")] 
	public bool Nullable { get; set; } 
}

[XmlRoot(ElementName="EntityType", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class EntityType { 

	[XmlElement(ElementName="Key", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public Key Key { get; set; } 

	[XmlElement(ElementName="Property", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public List<Property> Properties { get; set; } 

	[XmlElement(ElementName="NavigationProperty", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public List<NavigationProperty> NavigationProperties { get; set; } 

	[XmlAttribute(AttributeName="Name", Namespace="")] 
	public string Name { get; set; } 
}

[XmlRoot(ElementName="ReferentialConstraint", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class ReferentialConstraint { 

	[XmlAttribute(AttributeName="Property", Namespace="")] 
	public string Property { get; set; } 

	[XmlAttribute(AttributeName="ReferencedProperty", Namespace="")] 
	public string ReferencedProperty { get; set; } 
}

[XmlRoot(ElementName="ComplexType", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class ComplexType { 

	[XmlElement(ElementName="Property", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public List<Property> Properties { get; set; } 

	[XmlAttribute(AttributeName="Name", Namespace="")] 
	public string Name { get; set; } 
}

[XmlRoot(ElementName="NavigationPropertyBinding", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class NavigationPropertyBinding { 

	[XmlAttribute(AttributeName="Target", Namespace="")] 
	public string Target { get; set; } 

	[XmlAttribute(AttributeName="Path", Namespace="")] 
	public string Path { get; set; } 
}

[XmlRoot(ElementName="Collection", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class CollectionOfPropertyPaths { 

	[XmlElement(ElementName="PropertyPath", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public List<string> PropertyPaths { get; set; } 
}

[XmlRoot(ElementName="PropertyValue", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class PropertyValue { 

	[XmlElement(ElementName="Collection", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public CollectionOfPropertyPaths Collection { get; set; } 

	[XmlAttribute(AttributeName="Property", Namespace="")] 
	public string Property { get; set; } 

	[XmlText] 
	public string Text { get; set; } 

	[XmlAttribute(AttributeName="Bool", Namespace="")] 
	public bool Bool { get; set; } 
}

[XmlRoot(ElementName="Record", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class Record { 

	[XmlElement(ElementName="PropertyValue", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public PropertyValue PropertyValue { get; set; } 
}

[XmlRoot(ElementName="EntitySet", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class EntitySet { 

	[XmlElement(ElementName="NavigationPropertyBinding", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public List<NavigationPropertyBinding> NavigationPropertyBindings { get; set; } 

	[XmlElement(ElementName="Annotation", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public List<Annotation> Annotations { get; set; } 

	[XmlAttribute(AttributeName="Name", Namespace="")] 
	public string Name { get; set; } 

	[XmlAttribute(AttributeName="EntityType", Namespace="")] 
	public string EntityType { get; set; } 

	[XmlText] 
	public string Text { get; set; } 
}

[XmlRoot(ElementName="ActionImport", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class ActionImport { 

	[XmlAttribute(AttributeName="Name", Namespace="")] 
	public string Name { get; set; } 

	[XmlAttribute(AttributeName="Action", Namespace="")] 
	public string Action { get; set; } 

	[XmlElement(ElementName="Annotation", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public Annotation Annotation { get; set; } 

	[XmlAttribute(AttributeName="EntitySet", Namespace="")] 
	public string EntitySet { get; set; } 
}

[XmlRoot(ElementName="EntityContainer", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class EntityContainer { 

	[XmlElement(ElementName="EntitySet", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public List<EntitySet> EntitySets { get; set; } 

	[XmlElement(ElementName="ActionImport", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public List<ActionImport> ActionImports { get; set; } 

	[XmlAttribute(AttributeName="Name", Namespace="")] 
	public string Name { get; set; } 

	[XmlText] 
	public string Text { get; set; } 
}

[XmlRoot(ElementName="Parameter", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class Parameter { 

	[XmlAttribute(AttributeName="Nullable", Namespace="")] 
	public bool Nullable { get; set; } 

	[XmlAttribute(AttributeName="Name", Namespace="")] 
	public string Name { get; set; } 

	[XmlAttribute(AttributeName="Type", Namespace="")] 
	public string Type { get; set; } 

	[XmlAttribute(AttributeName="MaxLength", Namespace="")] 
	public int MaxLength { get; set; } 
}

[XmlRoot(ElementName="ReturnType", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class ReturnType { 

	[XmlAttribute(AttributeName="Type", Namespace="")] 
	public string Type { get; set; } 

	[XmlAttribute(AttributeName="Nullable", Namespace="")] 
	public bool Nullable { get; set; } 
}

[XmlRoot(ElementName="Action", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class Action { 

	[XmlElement(ElementName="Parameter", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public Parameter Parameter { get; set; } 

	[XmlElement(ElementName="ReturnType", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public ReturnType ReturnType { get; set; } 

	[XmlAttribute(AttributeName="Name", Namespace="")] 
	public string Name { get; set; } 
}

[XmlRoot(ElementName="Schema", Namespace="http://docs.oasis-open.org/odata/ns/edm")]
public class Schema { 

	[XmlElement(ElementName="EntityType", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public List<EntityType> EntityTypes { get; set; } 

	[XmlElement(ElementName="ComplexType", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public List<ComplexType> ComplexTypes { get; set; } 

	[XmlElement(ElementName="EntityContainer", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public EntityContainer EntityContainer { get; set; } 

	[XmlElement(ElementName="Action", Namespace="http://docs.oasis-open.org/odata/ns/edm")] 
	public List<Action> Actions { get; set; } 

	[XmlAttribute(AttributeName="Namespace", Namespace="")] 
	public string Namespace { get; set; } 

	[XmlAttribute(AttributeName="xmlns", Namespace="")] 
	public string Xmlns { get; set; } 

	[XmlText] 
	public string Text { get; set; } 
}
