using System.Text.Json.Serialization;
using DataOperations.OData.Serialization;

namespace DataOperations.OData
{
    public abstract class BaseDTOWithIDAndETag : IBaseDTOWithIDAndETag
    {
        private IOperationsDispatcher dispatcher {get; set;}
        public void AttachDispatcher<T>(IOperationsDispatcher dispatcher) where T : IBaseDTOWithIDAndETag
        {
            this.dispatcher = dispatcher;
            Console.WriteLine("Attached dispatcher to " + this.GetType().Name); 

            // Get instances of all properties that are of type DeferredReference<T> using reflection
            // We have to attach a reference to the dispatcher to each of these properties so that they can fetch the data from the server
            // to execute the deferred calls.

            var deferredReferences = this.GetType().GetProperties()
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Deferred<>));
                
            // Now get the instances of the deferred references and attach the dispatcher to them.
            foreach(var deferredReference in deferredReferences)
            {
                // Get the deferred property setter
                var def = deferredReference.GetMethod.Invoke(this, null);

                // Create the instance of the deferred enum property 
                // if it is null, then create a new instance of the deferred property.
                // otherwise after deserialization it will be null, which is bad :(

                def.GetType()
                    .GetProperties()
                    .Where(e => e.Name == "Result")
                    .First()
                    .SetValue(def, Activator.CreateInstance(deferredReference.PropertyType.GetGenericArguments()[0]));

                def.GetType()
                    .GetProperties()
                    .Where(e => e.Name == "Dispatcher")
                    .First()
                    .SetValue(def, dispatcher);
                    
                Console.WriteLine("Attached dispatcher to Deferred " + deferredReference.Name);
            }

            // Now do the same for the DeferredEnumerable<T> properties
            var deferredEnumerables = this.GetType().GetProperties()
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DeferredEnumerable<>));

            // Now get the instances of the deferred references and attach the dispatcher to them.
            foreach(var deferredEnumerable in deferredEnumerables)
            {

                // Get the deferred property setter
                var def = deferredEnumerable.GetMethod.Invoke(this, null);              
                
                // Create the instance of the deferred enum property
                var thisList = Activator.CreateInstance
                (
                    typeof(List<>)
                    .MakeGenericType(
                        new Type[] {
                            deferredEnumerable.PropertyType.GetGenericArguments()[0]
                        }
                    )
                );

                def.GetType()
                    .GetProperties()
                    .Where(e => e.Name == "Results")
                    .First()
                    .SetValue(def, thisList);

                def.GetType()
                    .GetProperties()
                    .Where(e => e.Name == "Dispatcher")
                    .First()
                    .SetValue(def, dispatcher);
                Console.WriteLine("Attached dispatcher to DeferredEnumerable " + deferredEnumerable.Name);
            }

        }

        [JsonPropertyName("__metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Metadata Metadata;
        protected virtual Dictionary<string, List<string>> ReferenceKeys { get { return new Dictionary<string, List<string>>();}}
        public virtual Dictionary<string, object> GetPrimaryKeyValues()
        {
            return GetReferenceValues("PrimaryKey");
        }
        public virtual IEnumerable<string> GetPrimaryKey()
        {
            return GetReferenceValues("PrimaryKey").Select(e => e.Key);
        }
        [JsonIgnore()]
        public string eTag { get; set; } = ""; 
        public virtual List<string> GetReferenceKeys(string ReferenceName)
        {
            return ReferenceKeys[ReferenceName];
        }
        public virtual Dictionary<string, object> GetReferenceValues(string ReferenceName)
        {
            var _ = new Dictionary<string, object>();

            foreach(string tup in GetReferenceKeys(ReferenceName))
            {
                if(tup.Contains("|"))
                {
                    string[] MappedProperty = tup.Split('|');
                    // Use reflection to get the value of the **mapped** property at runtime but add it to the collection 
                    // with the alternate name
                    _.Add(MappedProperty[0], this.GetType().GetProperty(MappedProperty[1]).GetValue(this));
                }
                else
                {
                    // Use reflection to get the value of the normal property at runtime
                    _.Add(tup, this.GetType().GetProperty(tup).GetValue(this));
                }
            }
            return _;
        }
    }
}
