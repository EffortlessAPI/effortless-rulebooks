using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataClassesTest.Services
{
    public class SyncConfiguration
    {
        public string Name { get; set; } = string.Empty;
        public string BaseId { get; set; } = string.Empty;
        public List<EntityConfiguration> Entities { get; set; } = new();
        public Dictionary<string, List<string>> EntityDependencies { get; set; } = new();
        
        public static SyncConfiguration LoadFromDataSchema(string dataSchemaPath)
        {
            var jsonContent = File.ReadAllText(dataSchemaPath);
            var dataSchema = JsonSerializer.Deserialize<DataSchemaRoot>(jsonContent);
            
            if (dataSchema?.Ontology?.OntologyGroups?.OntologyGroup == null)
                throw new InvalidOperationException("Invalid DataSchema format");
                
            var config = new SyncConfiguration
            {
                Name = dataSchema.Ontology.Name,
                BaseId = dataSchema.Ontology.BaseId
            };
            
            var ontologyGroup = dataSchema.Ontology.OntologyGroups.OntologyGroup.FirstOrDefault();
            if (ontologyGroup?.ObjectDefs?.ObjectDef == null)
                return config;
                
            // Process each entity
            foreach (var objectDef in ontologyGroup.ObjectDefs.ObjectDef)
            {
                var entityConfig = new EntityConfiguration
                {
                    Name = objectDef.Name,
                    AirtableName = objectDef.AirtableName,
                    PluralName = objectDef.PluralName,
                    SingularName = objectDef.SingularName
                };
                
                if (objectDef.PropertyDefs?.PropertyDef != null)
                {
                    foreach (var propDef in objectDef.PropertyDefs.PropertyDef)
                    {
                        var fieldConfig = new FieldConfiguration
                        {
                            Name = propDef.Name,
                            OriginalName = propDef.OriginalName,
                            DataType = propDef.DataType,
                            IsPrimaryKey = propDef.IsPrimaryKey == "1",
                            IsFormula = propDef.IsFormula == "1",
                            IsReadOnly = propDef.IsReadOnly == "1",
                            IsNullable = propDef.IsNullable == "1",
                            Formula = propDef.Formula,
                            Dependencies = propDef.Dependencies
                        };
                        
                        // Check for foreign key relationships
                        if (propDef.Relationships?.Relationship != null)
                        {
                            foreach (var rel in propDef.Relationships.Relationship)
                            {
                                fieldConfig.ForeignKeyReferences.Add(new ForeignKeyReference
                                {
                                    ReferencedEntity = rel.ReferencedObjectDef,
                                    ReferencedProperty = rel.ReferencedPropertyDef,
                                    SymmetricProperty = rel.SymmetricPropertyDef
                                });
                            }
                        }
                        
                        entityConfig.Fields.Add(fieldConfig);
                    }
                }
                
                config.Entities.Add(entityConfig);
            }
            
            // Build dependency graph
            config.BuildDependencyGraph();
            
            return config;
        }
        
        public void BuildDependencyGraph()
        {
            EntityDependencies.Clear();
            
            foreach (var entity in Entities)
            {
                var dependencies = new List<string>();
                
                foreach (var field in entity.Fields)
                {
                    foreach (var fkRef in field.ForeignKeyReferences)
                    {
                        if (!dependencies.Contains(fkRef.ReferencedEntity))
                        {
                            dependencies.Add(fkRef.ReferencedEntity);
                        }
                    }
                }
                
                EntityDependencies[entity.Name] = dependencies;
            }
        }
        
        public List<string> GetProcessingOrder()
        {
            var visited = new HashSet<string>();
            var visiting = new HashSet<string>();
            var result = new List<string>();
            
            foreach (var entity in Entities)
            {
                if (!visited.Contains(entity.Name))
                {
                    TopologicalSort(entity.Name, visited, visiting, result);
                }
            }
            
            return result;
        }
        
        private void TopologicalSort(string entityName, HashSet<string> visited, HashSet<string> visiting, List<string> result)
        {
            if (visiting.Contains(entityName))
            {
                // Circular dependency detected - add to result anyway to avoid infinite loop
                Console.WriteLine($"Warning: Circular dependency detected involving {entityName}");
                return;
            }
            
            if (visited.Contains(entityName))
                return;
                
            visiting.Add(entityName);
            
            if (EntityDependencies.TryGetValue(entityName, out var dependencies))
            {
                foreach (var dependency in dependencies)
                {
                    TopologicalSort(dependency, visited, visiting, result);
                }
            }
            
            visiting.Remove(entityName);
            visited.Add(entityName);
            result.Add(entityName);
        }
        
        public EntityConfiguration? GetEntityByName(string name)
        {
            return Entities.FirstOrDefault(e => 
                string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(e.AirtableName, name, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(e.PluralName, name, StringComparison.OrdinalIgnoreCase));
        }
    }
    
    public class EntityConfiguration
    {
        public string Name { get; set; } = string.Empty;
        public string AirtableName { get; set; } = string.Empty;
        public string PluralName { get; set; } = string.Empty;
        public string SingularName { get; set; } = string.Empty;
        public List<FieldConfiguration> Fields { get; set; } = new();
        
        public List<FieldConfiguration> GetFormulaFields()
        {
            return Fields.Where(f => f.IsFormula).ToList();
        }
        
        public List<FieldConfiguration> GetForeignKeyFields()
        {
            return Fields.Where(f => f.ForeignKeyReferences.Any()).ToList();
        }
        
        public FieldConfiguration? GetPrimaryKeyField()
        {
            return Fields.FirstOrDefault(f => f.IsPrimaryKey);
        }
        
        public FieldConfiguration? FindFieldByAirtableName(string airtableName)
        {
            // First try direct match
            var directMatch = Fields.FirstOrDefault(f => 
                string.Equals(f.Name, airtableName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(f.OriginalName, airtableName, StringComparison.OrdinalIgnoreCase));
            
            if (directMatch != null) return directMatch;
            
            // Try to find foreign key mapping
            foreach (var field in Fields)
            {
                foreach (var fkRef in field.ForeignKeyReferences)
                {
                    if (string.Equals(fkRef.SymmetricProperty, airtableName, StringComparison.OrdinalIgnoreCase))
                    {
                        return field;
                    }
                }
            }
            
            return null;
        }
    }
    
    public class FieldConfiguration
    {
        public string Name { get; set; } = string.Empty;
        public string OriginalName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public bool IsPrimaryKey { get; set; }
        public bool IsFormula { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsNullable { get; set; }
        public string? Formula { get; set; }
        public string? Dependencies { get; set; }
        public List<ForeignKeyReference> ForeignKeyReferences { get; set; } = new();
        
        public bool IsForeignKey => ForeignKeyReferences.Any();
    }
    
    public class ForeignKeyReference
    {
        public string ReferencedEntity { get; set; } = string.Empty;
        public string ReferencedProperty { get; set; } = string.Empty;
        public string SymmetricProperty { get; set; } = string.Empty;
    }
    
    // Data schema JSON structure classes
    public class DataSchemaRoot
    {
        [JsonPropertyName("Ontology")]
        public OntologyRoot? Ontology { get; set; }
    }
    
    public class OntologyRoot
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("BaseId")]
        public string BaseId { get; set; } = string.Empty;
        
        [JsonPropertyName("OntologyGroups")]
        public OntologyGroups? OntologyGroups { get; set; }
    }
    
    public class OntologyGroups
    {
        [JsonPropertyName("OntologyGroup")]
        public List<OntologyGroup>? OntologyGroup { get; set; }
    }
    
    public class OntologyGroup
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("ObjectDefs")]
        public ObjectDefs? ObjectDefs { get; set; }
    }
    
    public class ObjectDefs
    {
        [JsonPropertyName("ObjectDef")]
        public List<ObjectDef>? ObjectDef { get; set; }
    }
    
    public class ObjectDef
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("AirtableName")]
        public string AirtableName { get; set; } = string.Empty;
        
        [JsonPropertyName("PluralName")]
        public string PluralName { get; set; } = string.Empty;
        
        [JsonPropertyName("SingularName")]
        public string SingularName { get; set; } = string.Empty;
        
        [JsonPropertyName("PropertyDefs")]
        public PropertyDefs? PropertyDefs { get; set; }
    }
    
    public class PropertyDefs
    {
        [JsonPropertyName("PropertyDef")]
        public List<PropertyDef>? PropertyDef { get; set; }
    }
    
    public class PropertyDef
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("OriginalName")]
        public string OriginalName { get; set; } = string.Empty;
        
        [JsonPropertyName("DataType")]
        public string DataType { get; set; } = string.Empty;
        
        [JsonPropertyName("IsPrimaryKey")]
        public string IsPrimaryKey { get; set; } = "0";
        
        [JsonPropertyName("IsFormula")]
        public string IsFormula { get; set; } = "0";
        
        [JsonPropertyName("IsReadOnly")]
        public string IsReadOnly { get; set; } = "0";
        
        [JsonPropertyName("IsNullable")]
        public string IsNullable { get; set; } = "1";
        
        [JsonPropertyName("Formula")]
        public string? Formula { get; set; }
        
        [JsonPropertyName("Dependencies")]
        public string? Dependencies { get; set; }
        
        [JsonPropertyName("Relationships")]
        public Relationships? Relationships { get; set; }
    }
    
    public class Relationships
    {
        [JsonPropertyName("Relationship")]
        public List<Relationship>? Relationship { get; set; }
    }
    
    public class Relationship
    {
        [JsonPropertyName("ReferencedObjectDef")]
        public string ReferencedObjectDef { get; set; } = string.Empty;
        
        [JsonPropertyName("ReferencedPropertyDef")]
        public string ReferencedPropertyDef { get; set; } = string.Empty;
        
        [JsonPropertyName("SymmetricPropertyDef")]
        public string SymmetricPropertyDef { get; set; } = string.Empty;
    }
}
