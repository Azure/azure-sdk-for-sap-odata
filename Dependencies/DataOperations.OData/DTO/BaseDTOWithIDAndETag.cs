using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using DataOperations.OData.Serialization;

namespace DataOperations.OData
{
    public abstract class BaseDTOWithIDAndETag : IBaseDTOWithIDAndETag, IWorkTracking
    {
        private IOperationsDispatcher dispatcher {get; set;}
        public void AttachDispatcher<T>(IOperationsDispatcher dispatcher) where T : IBaseDTOWithIDAndETag
        {
            this.dispatcher = dispatcher;
            // Console.WriteLine("Attached dispatcher to " + this.GetType().Name); 

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
                    
                // Console.WriteLine("Attached dispatcher to Deferred " + deferredReference.Name);
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

        private Dictionary<string, object> _ChangeLog = new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.  
        protected internal void NotifyPropertyChanged<T>(T oldVal, T newVal, string PropertyName)  
        {  
            // Add it to the change log if we are not reverting changes
            if (!_IsRevertingChanges)
            {
                _ChangeLog.Add(PropertyName, oldVal);
            }
            // We still want to fire INotifyPropertyChanged so that the UI can update even if we are reverting changes
            PropertyChanged?.Invoke(this, new PropertyChangedWithValuesEventArgs<T>(PropertyName, oldVal, newVal));
        }  

        // Key is the property name that changed, value is the old value 
        // (The current value will be reflected in the present value of the property!)
        public Dictionary<string, object> GetChangeLog()
        {
            return _ChangeLog;
        }
        public string GetChangeLogAsJSON()
        {
            return System.Text.Json.JsonSerializer.Serialize<Dictionary<string,object>>(_ChangeLog);
        }
        // This will just nuke the change log
        public void AcceptChanges()
        {
            _ChangeLog = new Dictionary<string, object>();
        }

        bool IWorkTracking.IsChanged => _ChangeLog.Count > 0;

        private bool _IsRevertingChanges = false;

        public void RevertChanges()
        {
            // set a flag to temporarily halt change tracking from adding changes back to the change log 
            // (we don't want to add the changes we are about to make back to the change log)
            _IsRevertingChanges = true;

            // Use reflection to fetch the old values from the table and set them back one by one then clear the change log;
            foreach(var change in _ChangeLog)
            {
                this.GetType().GetProperty(change.Key).SetValue(this, change.Value);
            }

            // unset the flag
            _IsRevertingChanges = false;

        }

        public object UndoLastChange()
        {
            // set a flag to temporarily halt change tracking from adding changes back to the change log 
            // (we don't want to add the changes we are about to make back to the change log)
            _IsRevertingChanges = true;

            // Use reflection to fetch the old value from the table and set it then clear the change log entry;
            KeyValuePair<string, object> lastValue = _ChangeLog.Last();
            object revertedFromValue = this.GetType().GetProperty(lastValue.Key).GetValue(this);
            this.GetType().GetProperty(lastValue.Key).SetValue(this, lastValue.Value);
            _ChangeLog.Remove(lastValue.Key);

            // unset the flag
            _IsRevertingChanges = false;
            // Return the value that was reverted from (i.e. the undone value)
            return revertedFromValue;
        }

        public object UndoLastChange(string PropertyName)
        {
            // set a flag to temporarily halt change tracking from adding changes back to the change log 
            // (we don't want to add the changes we are about to make back to the change log)
            _IsRevertingChanges = true;

            // Use reflection to fetch the old value from the table and set it then clear the change log entry;
            KeyValuePair<string, object> lastValue = _ChangeLog.Where(e => e.Key == PropertyName).FirstOrDefault();
            object revertedFromValue = this.GetType().GetProperty(lastValue.Key).GetValue(this);
            this.GetType().GetProperty(lastValue.Key).SetValue(this, lastValue.Value);
            _ChangeLog.Remove(lastValue.Key);

            // unset the flag
            _IsRevertingChanges = false;
            // Return the value that was reverted from (i.e. the undone value)
            return revertedFromValue;
        }
    }
}
