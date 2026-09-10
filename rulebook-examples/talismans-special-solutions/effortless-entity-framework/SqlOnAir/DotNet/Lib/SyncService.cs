using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace DataClassesTest.Services
{
    public class SyncService
    {
        private readonly SoAEFContext _context;
        private readonly string _airtableJsonPath;
        private readonly Dictionary<string, List<JsonElement>> _originalJsonData = new Dictionary<string, List<JsonElement>>();
        private readonly SyncConfiguration _configuration;
        private readonly Dictionary<string, string> _airtableIdToMD5Map = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _recordSignatureToAirtableId = new Dictionary<string, string>();
        private string _currentTableName = string.Empty;
        private int _currentRecordIndex = 0;

        public SyncService(SoAEFContext context, string airtableJsonPath, string? dataSchemaPath = null)
        {
            _context = context;
            _airtableJsonPath = airtableJsonPath;
            
            // Load configuration from DataSchema.json if provided, otherwise use default path
            var schemaPath = dataSchemaPath ?? Path.Combine(Path.GetDirectoryName(_airtableJsonPath) ?? "", "..", "docs", "DataSchema.json");
            if (File.Exists(schemaPath))
            {
                _configuration = SyncConfiguration.LoadFromDataSchema(schemaPath);
                Console.WriteLine($"Loaded sync configuration from {schemaPath}");
                Console.WriteLine($"Found {_configuration.Entities.Count} entities with schema-driven configuration");
            }
            else
            {
                Console.WriteLine($"Warning: DataSchema.json not found at {schemaPath}, using fallback configuration");
                _configuration = CreateFallbackConfiguration();
            }
        }

        public async Task SyncAllDataAsync()
        {
            Console.WriteLine("Starting Airtable data sync...");

            // Read and parse the Airtable JSON file
            var jsonContent = await File.ReadAllTextAsync(_airtableJsonPath);
            var airtableData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            if (!airtableData.TryGetProperty("Airtable", out var airtableRoot))
            {
                throw new InvalidOperationException("Invalid Airtable JSON format - missing 'Airtable' root property");
            }

            // Disable foreign key constraints
            await DisableConstraintsAsync();

            try
            {
                // PHASE 0: Build Airtable ID to MD5 mapping dictionary
                Console.WriteLine("=== PHASE 0: Building Airtable ID to MD5 mapping ===");
                await BuildAirtableIdMappingAsync(airtableRoot);

                // Get all DbSet properties from the context
                var dbSetProperties = GetDbSetProperties();

                // PHASE 1: Load all entities into memory without saving (to handle circular references)
                // Console.WriteLine("=== PHASE 1: Loading all entities into memory ===");
                
                // Process tables in dependency order (parents before children)
                var tableProcessingOrder = _configuration.GetProcessingOrder();
                // Console.WriteLine($"Processing tables in dependency order: {string.Join(", ", tableProcessingOrder)}");
                
                foreach (var tableName in tableProcessingOrder)
                {
                    if (airtableRoot.TryGetProperty(tableName, out var tableData))
                    {
                        // Find matching DbSet property (case-insensitive)
                        var dbSetProperty = dbSetProperties.FirstOrDefault(p => 
                            string.Equals(p.Name, tableName, StringComparison.OrdinalIgnoreCase));

                        if (dbSetProperty != null)
                        {
                            // Console.WriteLine($"Loading table into memory: {tableName}");
                            await LoadTableIntoMemoryAsync(dbSetProperty, tableData);
                        }
                        else
                        {
                            Console.WriteLine($"Warning: No matching DbSet found for table '{tableName}'");
                        }
                    }
                }
                
                // Process any remaining tables not in the predefined order
                foreach (var tableProperty in airtableRoot.EnumerateObject())
                {
                    var tableName = tableProperty.Name;
                    if (!tableProcessingOrder.Contains(tableName))
                    {
                        var tableData = tableProperty.Value;
                        var dbSetProperty = dbSetProperties.FirstOrDefault(p => 
                            string.Equals(p.Name, tableName, StringComparison.OrdinalIgnoreCase));

                        if (dbSetProperty != null)
                        {
                            // Console.WriteLine($"Loading remaining table into memory: {tableName}");
                            await LoadTableIntoMemoryAsync(dbSetProperty, tableData);
                        }
                        else
                        {
                            Console.WriteLine($"Warning: No matching DbSet found for table '{tableName}'");
                        }
                    }
                }

                // PHASE 2: Save all entities at once (constraints are already disabled)
                Console.WriteLine("=== PHASE 2: Saving all entities to database ===");
                await SaveAllEntitiesAsync();

                // PHASE 3: Update entities with formula field values from JSON
                // Console.WriteLine("=== PHASE 3: Updating formula fields with actual JSON values ===");
                await UpdateFormulaFieldsAsync();
            }
            finally
            {
                // Re-enable foreign key constraints
                await EnableConstraintsAsync();
            }

            Console.WriteLine("Airtable data sync completed successfully!");
        }

        private async Task BuildAirtableIdMappingAsync(JsonElement airtableRoot)
        {
            Console.WriteLine("Scanning all records to build Airtable ID to MD5 mapping...");
            
            // First pass: collect all Airtable record IDs and build reverse mapping
            var allAirtableIds = new HashSet<string>();
            var foreignKeyReferences = new Dictionary<string, List<(string tableName, JsonElement record)>>();
            
            foreach (var tableProperty in airtableRoot.EnumerateObject())
            {
                var tableName = tableProperty.Name;
                var tableData = tableProperty.Value;
                
                if (tableData.ValueKind == JsonValueKind.Array)
                {
                    foreach (var recordElement in tableData.EnumerateArray())
                    {
                        // Look for any field that contains an Airtable record ID
                        foreach (var recordProperty in recordElement.EnumerateObject())
                        {
                            if (recordProperty.Value.ValueKind == JsonValueKind.String)
                            {
                                var stringValue = recordProperty.Value.GetString();
                                if (!string.IsNullOrEmpty(stringValue) && stringValue.StartsWith("rec"))
                                {
                                    allAirtableIds.Add(stringValue);
                                    
                                    // Track which records reference this Airtable ID
                                    if (!foreignKeyReferences.ContainsKey(stringValue))
                                        foreignKeyReferences[stringValue] = new List<(string, JsonElement)>();
                                    foreignKeyReferences[stringValue].Add((tableName, recordElement));
                                }
                            }
                            else if (recordProperty.Value.ValueKind == JsonValueKind.Array)
                            {
                                // Handle arrays of Airtable IDs (like Contacts field in Clients)
                                foreach (var arrayElement in recordProperty.Value.EnumerateArray())
                                {
                                    if (arrayElement.ValueKind == JsonValueKind.String)
                                    {
                                        var stringValue = arrayElement.GetString();
                                        if (!string.IsNullOrEmpty(stringValue) && stringValue.StartsWith("rec"))
                                        {
                                            allAirtableIds.Add(stringValue);
                                            
                                            // Track which records reference this Airtable ID
                                            if (!foreignKeyReferences.ContainsKey(stringValue))
                                                foreignKeyReferences[stringValue] = new List<(string, JsonElement)>();
                                            foreignKeyReferences[stringValue].Add((tableName, recordElement));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            // Second pass: create MD5 hash mapping for all found Airtable IDs
            foreach (var airtableId in allAirtableIds)
            {
                var md5Hash = ComputeMD5Hash(airtableId);
                _airtableIdToMD5Map[airtableId] = md5Hash;
                Console.WriteLine($"Mapped Airtable ID: {airtableId} -> {md5Hash}");
            }
            
            // Third pass: Build reverse mapping to identify which record owns which Airtable ID
            // This is critical for records that don't have their own ID explicitly in their data
            await BuildRecordOwnershipMappingAsync(airtableRoot, foreignKeyReferences);
            
            Console.WriteLine($"✓ Built mapping for {_airtableIdToMD5Map.Count} Airtable record IDs");
        }

        private async Task BuildRecordOwnershipMappingAsync(JsonElement airtableRoot, Dictionary<string, List<(string tableName, JsonElement record)>> foreignKeyReferences)
        {
            Console.WriteLine("Building record ownership mapping...");
            
            // For each table, try to infer which Airtable ID belongs to which record
            foreach (var tableProperty in airtableRoot.EnumerateObject())
            {
                var tableName = tableProperty.Name;
                var tableData = tableProperty.Value;
                
                if (tableData.ValueKind == JsonValueKind.Array)
                {
                    var recordIndex = 0;
                    foreach (var recordElement in tableData.EnumerateArray())
                    {
                        var recordSignature = $"{tableName}[{recordIndex}]";
                        
                        // Try to infer the Airtable ID for this record by looking at foreign key references
                        var inferredAirtableId = InferAirtableIdForRecord(tableName, recordElement, foreignKeyReferences);
                        
                        if (!string.IsNullOrEmpty(inferredAirtableId))
                        {
                            _recordSignatureToAirtableId[recordSignature] = inferredAirtableId;
                            Console.WriteLine($"Inferred: {recordSignature} -> {inferredAirtableId}");
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Could not infer Airtable ID for {recordSignature}");
                        }
                        
                        recordIndex++;
                    }
                }
            }
        }

        private string InferAirtableIdForRecord(string tableName, JsonElement recordElement, Dictionary<string, List<(string tableName, JsonElement record)>> foreignKeyReferences)
        {
            // Strategy 1: First check if the record itself contains an Airtable ID in any of its fields
            // This handles cases like DocEditings with DocEditingId field or Colors with ColorId field
            foreach (var property in recordElement.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.String)
                {
                    var stringValue = property.Value.GetString();
                    if (!string.IsNullOrEmpty(stringValue) && stringValue.StartsWith("rec"))
                    {
                        // Check if this field name suggests it's the primary ID for this table
                        // Common patterns: TableNameId, or just any field ending with "Id" that contains a rec value
                        var fieldName = property.Name.ToLowerInvariant();
                        var tableNameLower = tableName.ToLowerInvariant();
                        
                        // Check for patterns like "DocEditingId" for "DocEditings" table, or "ColorId" for "Colors" table
                        if (fieldName.EndsWith("id") && (fieldName.StartsWith(tableNameLower.TrimEnd('s')) || fieldName.Contains(tableNameLower.TrimEnd('s'))))
                        {
                            Console.WriteLine($"Found direct Airtable ID in field '{property.Name}': {stringValue}");
                            return stringValue;
                        }
                    }
                }
            }
            
            // Strategy 2: Look for Airtable IDs that are referenced by other records pointing back to this record
            // For example, if a Client record has "Plan": "rec123", then the Plan record with matching data should have ID "rec123"
            
            foreach (var kvp in foreignKeyReferences)
            {
                var airtableId = kvp.Key;
                var referencingRecords = kvp.Value;
                
                // Check if any of the referencing records point to this table
                foreach (var (refTableName, refRecord) in referencingRecords)
                {
                    // Look for fields in the referencing record that point to our table
                    foreach (var refProperty in refRecord.EnumerateObject())
                    {
                        // Check if this field name suggests it's a reference to our table
                        if (IsFieldReferenceToTable(refProperty.Name, tableName))
                        {
                            if (refProperty.Value.ValueKind == JsonValueKind.String)
                            {
                                var refValue = refProperty.Value.GetString();
                                if (refValue == airtableId)
                                {
                                    // This referencing record points to our table with this Airtable ID
                                    // Now check if our record could be the target by comparing some identifying data
                                    if (CouldRecordMatchReference(recordElement, refRecord, refProperty.Name))
                                    {
                                        return airtableId;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            return string.Empty;
        }

        private bool IsFieldReferenceToTable(string fieldName, string tableName)
        {
            // Simple heuristic: field name matches table name (singular/plural variations)
            var normalizedFieldName = fieldName.ToLowerInvariant();
            var normalizedTableName = tableName.ToLowerInvariant();
            
            // Direct match
            if (normalizedFieldName == normalizedTableName) return true;
            
            // Singular/plural variations
            if (normalizedFieldName == normalizedTableName.TrimEnd('s')) return true;
            if (normalizedFieldName + "s" == normalizedTableName) return true;
            
            return false;
        }

        private bool CouldRecordMatchReference(JsonElement targetRecord, JsonElement referencingRecord, string referenceFieldName)
        {
            // For now, we'll use a simple heuristic - if we can't find a better match, assume it could match
            // In a more sophisticated implementation, we might compare identifying fields
            return true;
        }

        private List<PropertyInfo> GetDbSetProperties()
        {
            return _context.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType && 
                           p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .ToList();
        }

        private async Task SyncTableAsync(PropertyInfo dbSetProperty, JsonElement tableData)
        {
            // Get the entity type from the DbSet<T>
            var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
            var dbSet = dbSetProperty.GetValue(_context);

            if (dbSet == null) return;

            // Console.WriteLine($"Processing {entityType.Name} entities...");

            // Get the DbSet as IQueryable for generic operations
            var queryableMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "AsQueryable" && m.GetParameters().Length == 1)
                .MakeGenericMethod(entityType);
            var queryable = queryableMethod.Invoke(null, new[] { dbSet });

            // Get existing entities as a list
            var toListMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "ToList" && m.GetParameters().Length == 1)
                .MakeGenericMethod(entityType);
            var existingEntities = (System.Collections.IList)toListMethod.Invoke(null, new[] { queryable });

            // Create a dictionary for fast lookup by primary key
            var existingEntitiesDict = new Dictionary<string, object>();
            var primaryKeyProperty = GetPrimaryKeyProperty(entityType);

            foreach (var entity in existingEntities)
            {
                var pkValue = primaryKeyProperty.GetValue(entity)?.ToString();
                if (!string.IsNullOrEmpty(pkValue))
                {
                    existingEntitiesDict[pkValue] = entity;
                }
            }

            var recordCount = 0;
            // Process each record in the Airtable data
            if (tableData.ValueKind == JsonValueKind.Array)
            {
                foreach (var recordElement in tableData.EnumerateArray())
                {
                    try
                    {
                        await ProcessRecordAsync(entityType, dbSet, recordElement, existingEntitiesDict, primaryKeyProperty);
                        recordCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing record in {entityType.Name}: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"  Inner exception: {ex.InnerException.Message}");
                        }
                        Console.WriteLine($"  Stack trace: {ex.StackTrace}");
                        // Continue with next record
                    }
                }
            }

            Console.WriteLine($"Processed {recordCount} records for {entityType.Name}");

            // Save changes for this table
            try
            {
                // Disable change tracking and lazy loading to avoid formula property evaluation
                var originalAutoDetectChanges = _context.ChangeTracker.AutoDetectChangesEnabled;
                var originalLazyLoading = _context.ChangeTracker.LazyLoadingEnabled;
                var originalQueryTracking = _context.ChangeTracker.QueryTrackingBehavior;
                
                _context.ChangeTracker.AutoDetectChangesEnabled = false;
                _context.ChangeTracker.LazyLoadingEnabled = false;
                _context.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
                
                // For entities with navigation properties, ensure they don't trigger lazy loading
                foreach (var entry in _context.ChangeTracker.Entries())
                {
                    if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Added || 
                        entry.State == Microsoft.EntityFrameworkCore.EntityState.Modified)
                    {
                        // Prevent navigation property access during save
                        entry.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    }
                }
                
                await _context.SaveChangesAsync();
                
                // Restore original settings
                _context.ChangeTracker.AutoDetectChangesEnabled = originalAutoDetectChanges;
                _context.ChangeTracker.LazyLoadingEnabled = originalLazyLoading;
                _context.ChangeTracker.QueryTrackingBehavior = originalQueryTracking;
                
                Console.WriteLine($"✓ Successfully saved {entityType.Name} entities");
            }
            catch (Exception ex)
            {
                // Restore original settings in case of error
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
                _context.ChangeTracker.LazyLoadingEnabled = true;
                _context.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.TrackAll;
                
                Console.WriteLine($"✗ Error saving {entityType.Name} entities: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                // Don't throw for now, continue with other tables
                // throw;
            }
        }

        private async Task ProcessRecordAsync(Type entityType, object dbSet, JsonElement recordElement, 
            Dictionary<string, object> existingEntitiesDict, PropertyInfo primaryKeyProperty)
        {
            // Generate stable primary key from Airtable record ID (if present) or from record content
            var stablePrimaryKey = GenerateStablePrimaryKey(recordElement);

            // Check if entity already exists
            object? entity;
            bool isNewEntity = false;

            if (existingEntitiesDict.TryGetValue(stablePrimaryKey, out var existingEntity))
            {
                entity = existingEntity;
            }
            else
            {
                // Create new entity instance
                try
                {
                    entity = Activator.CreateInstance(entityType);
                    if (entity == null)
                    {
                        Console.WriteLine($"Failed to create instance of {entityType.Name} - Activator returned null");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to create instance of {entityType.Name}: {ex.Message}");
                    return;
                }
                
                isNewEntity = true;

                // Set the primary key
                try
                {
                    primaryKeyProperty.SetValue(entity, stablePrimaryKey);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to set primary key for {entityType.Name}: {ex.Message}");
                    return;
                }
            }

            // Map properties from JSON to entity
            if (entity != null)
            {
                MapJsonToEntity(recordElement, entity, entityType);
            }
            else
            {
                Console.WriteLine($"Entity is null before mapping for {entityType.Name} (PK: {stablePrimaryKey})");
            }

            // Add to DbSet if it's a new entity
            if (isNewEntity && entity != null)
            {
                try
                {
                    var addMethod = dbSet.GetType().GetMethod("Add");
                    if (addMethod != null)
                    {
                        // Double-check entity is not null before invoking
                        if (entity != null)
                        {
                            addMethod.Invoke(dbSet, new[] { entity });
                            existingEntitiesDict[stablePrimaryKey] = entity;
                        }
                        else
                        {
                            Console.WriteLine($"Entity is null when trying to add {entityType.Name} to DbSet");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Could not find Add method for DbSet of {entityType.Name}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to add {entityType.Name} to DbSet: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"  Inner exception: {ex.InnerException.Message}");
                    }
                    // Add debugging info
                    Console.WriteLine($"  Entity type: {entityType.Name}");
                    Console.WriteLine($"  Entity is null: {entity == null}");
                    Console.WriteLine($"  Primary key: {stablePrimaryKey}");
                }
            }
            else if (isNewEntity && entity == null)
            {
                Console.WriteLine($"Skipping add for {entityType.Name} because entity is null (PK: {stablePrimaryKey})");
            }
        }

        private string GenerateStablePrimaryKey(JsonElement recordElement)
        {
            // First, try to find the record signature in our mapping
            // We need to determine which record this is by its position in the processing order
            var recordSignature = GetRecordSignature(recordElement);
            if (!string.IsNullOrEmpty(recordSignature) && _recordSignatureToAirtableId.ContainsKey(recordSignature))
            {
                var airtableId = _recordSignatureToAirtableId[recordSignature];
                var hash = _airtableIdToMD5Map[airtableId];
                Console.WriteLine($"Generated PK from record signature '{recordSignature}' -> Airtable ID '{airtableId}': {hash}");
                return hash;
            }

            // Fallback: Look for any Airtable record ID in this record that we've mapped
            foreach (var property in recordElement.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.String)
                {
                    var stringValue = property.Value.GetString();
                    if (!string.IsNullOrEmpty(stringValue) && stringValue.StartsWith("rec"))
                    {
                        // Check if this Airtable ID is in our mapping (meaning it's a primary key for some record)
                        if (_airtableIdToMD5Map.ContainsKey(stringValue))
                        {
                            // This could be either a PK or FK reference
                            // We need to determine if this is the primary key for THIS record
                            // For now, let's use the first one we find that's not a known FK field
                            var excludedForeignKeyFields = GetExcludedForeignKeyFields();
                            if (!excludedForeignKeyFields.Contains(property.Name, StringComparer.OrdinalIgnoreCase))
                            {
                                var hash = _airtableIdToMD5Map[stringValue];
                                Console.WriteLine($"Generated PK from mapped Airtable ID '{stringValue}' (field: {property.Name}): {hash}");
                                return hash;
                            }
                        }
                    }
                }
            }

            throw new InvalidOperationException($"Could not generate stable primary key for record: {recordElement.GetRawText()}. No Airtable record ID found in mapping.");
        }

        private string GetRecordSignature(JsonElement recordElement)
        {
            // Generate a signature based on current table and record index
            return $"{_currentTableName}[{_currentRecordIndex}]";
        }

        private string InferAirtableIdFromReferences(JsonElement recordElement)
        {
            // This method tries to infer the Airtable record ID for a record
            // by looking at how other records reference it
            // For now, we'll need a more sophisticated approach
            // This is a placeholder - in practice, we might need to build a reverse mapping
            return string.Empty;
        }

        private string ComputeMD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes).ToLowerInvariant();
            }
        }

        private PropertyInfo GetPrimaryKeyProperty(Type entityType)
        {
            // Look for property with [Key] attribute
            var keyProperty = entityType.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true).Any());

            if (keyProperty != null)
                return keyProperty;

            // Fallback: look for property ending with "Id"
            var idProperty = entityType.GetProperties()
                .FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));

            if (idProperty != null)
                return idProperty;

            throw new InvalidOperationException($"Could not find primary key property for entity type {entityType.Name}");
        }

        private void MapJsonToEntity(JsonElement jsonElement, object entity, Type entityType)
        {
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && !IsFormulaProperty(p))
                .ToList();

            foreach (var jsonProperty in jsonElement.EnumerateObject())
            {
                var propertyName = jsonProperty.Name;
                var jsonValue = jsonProperty.Value;

                PropertyInfo? entityProperty = null;
                
                // Handle special cases where Airtable field names need to be mapped to foreign key properties
                // This needs to happen FIRST for RecID values to prioritize foreign keys over navigation properties
                if (jsonValue.ValueKind == JsonValueKind.String)
                {
                    var stringValue = jsonValue.GetString();
                    if (!string.IsNullOrEmpty(stringValue) && stringValue.StartsWith("rec"))
                    {
                        // This looks like an Airtable record reference, try to map to foreign key using configuration first
                        var foreignKeyProperty = FindForeignKeyPropertyFromConfiguration(entityType, propertyName, properties);
                        
                        if (foreignKeyProperty != null)
                        {
                            entityProperty = foreignKeyProperty;
                            Console.WriteLine($"  Mapped {propertyName} -> {foreignKeyProperty.Name} for foreign key via configuration");
                        }
                        else
                        {
                            // Try convention-based mapping for foreign keys
                            entityProperty = FindPropertyByConvention(properties, propertyName);
                            if (entityProperty != null)
                            {
                                Console.WriteLine($"  Mapped {propertyName} -> {entityProperty.Name} for foreign key via convention");
                            }
                        }
                    }
                }
                
                // If no foreign key mapping found, try direct property match (but skip if it's a navigation property for RecID values)
                if (entityProperty == null)
                {
                    var directMatch = properties.FirstOrDefault(p => 
                        string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));
                    
                    // For RecID values, skip navigation properties and continue looking for foreign key properties
                    if (directMatch != null && 
                        jsonValue.ValueKind == JsonValueKind.String && 
                        jsonValue.GetString()?.StartsWith("rec") == true &&
                        IsNavigationProperty(directMatch))
                    {
                        // Skip navigation property for RecID values, we want the foreign key property instead
                        Console.WriteLine($"  Skipping navigation property {directMatch.Name} for RecID value, looking for foreign key property");
                    }
                    else
                    {
                        entityProperty = directMatch;
                    }
                }
                
                if (entityProperty == null)
                {
                    // Try to find using configuration-based mapping
                    entityProperty = FindForeignKeyPropertyFromConfiguration(entityType, propertyName, properties);
                    
                    if (entityProperty != null)
                    {
                        Console.WriteLine($"  Found configuration-based mapping: {propertyName} -> {entityProperty.Name}");
                    }
                    else
                    {
                        // Try generic convention-based mapping as fallback
                        entityProperty = FindPropertyByConvention(properties, propertyName);
                        
                        if (entityProperty != null)
                        {
                            Console.WriteLine($"  Found convention-based mapping: {propertyName} -> {entityProperty.Name}");
                        }
                        else
                        {
                            // Debug: Show what we're trying to map
                            if (jsonValue.ValueKind == JsonValueKind.String && jsonValue.GetString()?.StartsWith("rec") == true)
                            {
                                Console.WriteLine($"  WARNING: Could not find property for foreign key field '{propertyName}' with value '{jsonValue.GetString()}' in {entityType.Name}");
                                Console.WriteLine($"    Available properties: {string.Join(", ", properties.Select(p => p.Name))}");
                            }
                        }
                    }
                }

                if (entityProperty != null && !IsNavigationProperty(entityProperty) && !IsFormulaProperty(entityProperty))
                {
                    try
                    {
                        // Handle foreign key references (Airtable record IDs)
                        if (IsForeignKeyProperty(entityProperty) && jsonValue.ValueKind == JsonValueKind.String)
                        {
                            var airtableRecordId = jsonValue.GetString();
                            if (!string.IsNullOrEmpty(airtableRecordId) && airtableRecordId.StartsWith("rec"))
                            {
                                // Use the pre-built mapping dictionary to get the MD5 hash
                                if (_airtableIdToMD5Map.TryGetValue(airtableRecordId, out var foreignKeyValue))
                                {
                                    Console.WriteLine($"Mapping FK {propertyName}: {airtableRecordId} -> {foreignKeyValue}");
                                    entityProperty.SetValue(entity, foreignKeyValue);
                                    continue;
                                }
                                else
                                {
                                    Console.WriteLine($"Warning: Airtable ID '{airtableRecordId}' not found in mapping dictionary for FK {propertyName}");
                                }
                            }
                        }

                        var convertedValue = ConvertJsonValueToPropertyType(jsonValue, entityProperty.PropertyType);
                        entityProperty.SetValue(entity, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Could not set property '{propertyName}' on {entityType.Name}: {ex.Message}");
                    }
                }
            }
            
            // Ensure required foreign key properties have default values if not set
            EnsureRequiredForeignKeysHaveValues(entity, entityType);
        }

        private bool IsForeignKeyProperty(PropertyInfo property)
        {
            // Check if this is a foreign key property (ends with "Id")
            return property.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase) && 
                   property.PropertyType == typeof(string);
        }

        private bool IsFormulaProperty(PropertyInfo property)
        {
            // First check the configuration for this property
            var entityType = property.DeclaringType;
            if (entityType != null)
            {
                var entityConfig = _configuration.GetEntityByName(entityType.Name);
                if (entityConfig != null)
                {
                    var fieldConfig = entityConfig.Fields.FirstOrDefault(f => 
                        string.Equals(f.Name, property.Name, StringComparison.OrdinalIgnoreCase));
                    if (fieldConfig != null)
                    {
                        return fieldConfig.IsFormula;
                    }
                }
            }
            
            // Fallback to reflection-based detection
            return IsFormulaPropertyByReflection(property);
        }

        private bool IsNavigationProperty(PropertyInfo property)
        {
            // Skip navigation properties (collections and complex objects)
            var propertyType = property.PropertyType;
            
            // Skip collections
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string))
                return true;

            // Skip complex objects (entities)
            if (propertyType.IsClass && propertyType != typeof(string) && !propertyType.IsPrimitive && !propertyType.IsValueType)
                return true;

            return false;
        }

        private object ConvertJsonValueToPropertyType(JsonElement jsonValue, Type targetType)
        {
            // Handle nullable types
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (jsonValue.ValueKind == JsonValueKind.Null)
                return null;

            if (underlyingType == typeof(string))
                return jsonValue.GetString();

            if (underlyingType == typeof(int))
                return jsonValue.GetInt32();

            if (underlyingType == typeof(long))
                return jsonValue.GetInt64();

            if (underlyingType == typeof(decimal))
                return jsonValue.GetDecimal();

            if (underlyingType == typeof(double))
                return jsonValue.GetDouble();

            if (underlyingType == typeof(float))
                return jsonValue.GetSingle();

            if (underlyingType == typeof(bool))
            {
                if (jsonValue.ValueKind == JsonValueKind.String)
                {
                    var stringValue = jsonValue.GetString();
                    return string.Equals(stringValue, "true", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(stringValue, "TRUE", StringComparison.OrdinalIgnoreCase);
                }
                return jsonValue.GetBoolean();
            }

            if (underlyingType == typeof(DateTime))
            {
                if (jsonValue.ValueKind == JsonValueKind.String)
                {
                    var dateString = jsonValue.GetString();
                    if (DateTime.TryParse(dateString, out var dateTime))
                        return dateTime;
                }
                return jsonValue.GetDateTime();
            }

            if (underlyingType == typeof(Guid))
            {
                var guidString = jsonValue.GetString();
                return Guid.Parse(guidString);
            }

            // For other types, try to convert from string
            if (jsonValue.ValueKind == JsonValueKind.String)
            {
                var stringValue = jsonValue.GetString();
                return Convert.ChangeType(stringValue, underlyingType);
            }

            throw new NotSupportedException($"Cannot convert JSON value to type {targetType.Name}");
        }

        private async Task DisableConstraintsAsync()
        {
            Console.WriteLine("Disabling foreign key constraints...");
            
            // For SQL Server
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
            }
            // Add support for other database providers as needed
        }

        private async Task EnableConstraintsAsync()
        {
            Console.WriteLine("Re-enabling foreign key constraints...");
            
            // For SQL Server
            if (_context.Database.IsSqlServer())
            {
                try
                {
                    return;
                    // First, try to check all constraints with trust
                    await _context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'");
                    Console.WriteLine("✓ Foreign key constraints re-enabled with validation");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not re-enable constraints with validation: {ex.Message}");
                    
                    // Fallback: Enable constraints without validation
                    try
                    {
                        return;
                        await _context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL'");
                        Console.WriteLine("✓ Foreign key constraints re-enabled without validation");
                    }
                    catch (Exception fallbackEx)
                    {
                        Console.WriteLine($"Error: Could not re-enable constraints: {fallbackEx.Message}");
                        throw;
                    }
                }
            }
            // Add support for other database providers as needed
        }

        private void EnsureRequiredForeignKeysHaveValues(object entity, Type entityType)
        {
            // Ensure foreign key properties have valid values to prevent null reference issues
            var foreignKeyProperties = entityType.GetProperties()
                .Where(p => IsForeignKeyProperty(p) && p.CanWrite)
                .ToList();

            foreach (var fkProperty in foreignKeyProperties)
            {
                var currentValue = fkProperty.GetValue(entity) as string;
                if (currentValue == null)
                {
                    // Set a default empty string to prevent null issues
                    fkProperty.SetValue(entity, string.Empty);
                    // Console.WriteLine($"Set default empty value for FK property: {fkProperty.Name} on {entityType.Name}");
                }
            }
        }

        private async Task LoadTableIntoMemoryAsync(PropertyInfo dbSetProperty, JsonElement tableData)
        {
            // Get the entity type from the DbSet<T>
            var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
            var dbSet = dbSetProperty.GetValue(_context);

            if (dbSet == null) return;

            // Set current table name for record signature generation
            _currentTableName = dbSetProperty.Name;

            // Console.WriteLine($"Loading {entityType.Name} entities into memory...");

            // Store original JSON data for later use in PHASE 3
            var jsonRecords = new List<JsonElement>();
            if (tableData.ValueKind == JsonValueKind.Array)
            {
                foreach (var recordElement in tableData.EnumerateArray())
                {
                    jsonRecords.Add(recordElement);
                }
            }
            _originalJsonData[entityType.Name] = jsonRecords;

            // Get the DbSet as IQueryable for generic operations
            var queryableMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "AsQueryable" && m.GetParameters().Length == 1)
                .MakeGenericMethod(entityType);
            var queryable = queryableMethod.Invoke(null, new[] { dbSet });

            // Get existing entities as a list
            var toListMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "ToList" && m.GetParameters().Length == 1)
                .MakeGenericMethod(entityType);
            var existingEntities = (System.Collections.IList)toListMethod.Invoke(null, new[] { queryable });

            // Create a dictionary for fast lookup by primary key
            var existingEntitiesDict = new Dictionary<string, object>();
            var primaryKeyProperty = GetPrimaryKeyProperty(entityType);

            foreach (var entity in existingEntities)
            {
                var pkValue = primaryKeyProperty.GetValue(entity)?.ToString();
                if (!string.IsNullOrEmpty(pkValue))
                {
                    existingEntitiesDict[pkValue] = entity;
                }
            }

            var recordCount = 0;
            // Process each record in the Airtable data
            if (tableData.ValueKind == JsonValueKind.Array)
            {
                _currentRecordIndex = 0;
                foreach (var recordElement in tableData.EnumerateArray())
                {
                    try
                    {
                        await LoadRecordIntoMemoryAsync(entityType, dbSet, recordElement, existingEntitiesDict, primaryKeyProperty);
                        recordCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading record in {entityType.Name}: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"  Inner exception: {ex.InnerException.Message}");
                        }
                        // Continue with next record
                    }
                    _currentRecordIndex++;
                }
            }

            // Console.WriteLine($"Loaded {recordCount} {entityType.Name} entities into memory");
        }

        private async Task LoadRecordIntoMemoryAsync(Type entityType, object dbSet, JsonElement recordElement, 
            Dictionary<string, object> existingEntitiesDict, PropertyInfo primaryKeyProperty)
        {
            // Generate stable primary key from Airtable record ID (if present) or from record content
            var stablePrimaryKey = GenerateStablePrimaryKey(recordElement);

            // Check if entity already exists
            object? entity;
            bool isNewEntity = false;

            if (existingEntitiesDict.TryGetValue(stablePrimaryKey, out var existingEntity))
            {
                entity = existingEntity;
            }
            else
            {
                // Create new entity instance
                try
                {
                    entity = Activator.CreateInstance(entityType);
                    if (entity == null)
                    {
                        Console.WriteLine($"Failed to create instance of {entityType.Name} - Activator returned null");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to create instance of {entityType.Name}: {ex.Message}");
                    return;
                }
                
                isNewEntity = true;

                // Set the primary key
                try
                {
                    primaryKeyProperty.SetValue(entity, stablePrimaryKey);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to set primary key for {entityType.Name}: {ex.Message}");
                    return;
                }
            }

            // Map properties from JSON to entity
            if (entity != null)
            {
                MapJsonToEntity(recordElement, entity, entityType);
            }
            else
            {
                Console.WriteLine($"Entity is null before mapping for {entityType.Name} (PK: {stablePrimaryKey})");
            }

            // Add to DbSet if it's a new entity (but don't save yet)
            if (isNewEntity && entity != null)
            {
                try
                {
                    // Use the generic DbSet.Add method directly instead of reflection
                    var addMethod = typeof(Microsoft.EntityFrameworkCore.DbSet<>)
                        .MakeGenericType(entityType)
                        .GetMethod("Add", new[] { entityType });
                    
                    if (addMethod != null)
                    {
                        addMethod.Invoke(dbSet, new[] { entity });
                        existingEntitiesDict[stablePrimaryKey] = entity;
                        Console.WriteLine($"Successfully added {entityType.Name} to DbSet (PK: {stablePrimaryKey})");
                    }
                    else
                    {
                        Console.WriteLine($"Could not find Add method for DbSet of {entityType.Name}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to add {entityType.Name} to DbSet: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"  Inner exception: {ex.InnerException.Message}");
                    }
                }
            }
            else if (isNewEntity && entity == null)
            {
                Console.WriteLine($"Skipping add for {entityType.Name} because entity is null (PK: {stablePrimaryKey})");
            }
            else if (!isNewEntity)
            {
                Console.WriteLine($"Entity {entityType.Name} already exists (PK: {stablePrimaryKey})");
            }
        }

        private async Task SaveAllEntitiesAsync()
        {
            try
            {
                // Disable change tracking and lazy loading to avoid formula property evaluation
                var originalAutoDetectChanges = _context.ChangeTracker.AutoDetectChangesEnabled;
                var originalLazyLoading = _context.ChangeTracker.LazyLoadingEnabled;
                var originalQueryTracking = _context.ChangeTracker.QueryTrackingBehavior;
                
                _context.ChangeTracker.AutoDetectChangesEnabled = false;
                _context.ChangeTracker.LazyLoadingEnabled = false;
                _context.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
                
                Console.WriteLine("Saving all entities to database...");
                
                // Get count of entities to be saved
                var addedEntities = _context.ChangeTracker.Entries()
                    .Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Added)
                    .ToList();
                
                Console.WriteLine($"Found {addedEntities.Count} entities to save");
                
                // Debug: Show what entities are being saved
                foreach (var entry in addedEntities)
                {
                    Console.WriteLine($"Entity to save: {entry.Entity.GetType().Name} (State: {entry.State})");
                }
                
                // Debug: Check if Plans and Clients are missing from the save list
                var allEntries = _context.ChangeTracker.Entries().ToList();
                var planEntries = allEntries.Where(e => e.Entity.GetType().Name == "Plan").ToList();
                var clientEntries = allEntries.Where(e => e.Entity.GetType().Name == "Client").ToList();
                
                Console.WriteLine($"Total Plan entities in ChangeTracker: {planEntries.Count}");
                foreach (var plan in planEntries)
                {
                    Console.WriteLine($"  Plan entity state: {plan.State}");
                }
                
                Console.WriteLine($"Total Client entities in ChangeTracker: {clientEntries.Count}");
                foreach (var client in clientEntries)
                {
                    Console.WriteLine($"  Client entity state: {client.State}");
                }
                
                // Force Plans and Clients back to Added state if they were detached
                foreach (var plan in planEntries)
                {
                    if (plan.State != Microsoft.EntityFrameworkCore.EntityState.Added)
                    {
                        Console.WriteLine($"Forcing Plan entity back to Added state (was {plan.State})");
                        plan.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    }
                }
                
                foreach (var client in clientEntries)
                {
                    if (client.State != Microsoft.EntityFrameworkCore.EntityState.Added)
                    {
                        Console.WriteLine($"Forcing Client entity back to Added state (was {client.State})");
                        client.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    }
                }
                
                // Refresh the addedEntities list after fixing states
                addedEntities = _context.ChangeTracker.Entries()
                    .Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Added)
                    .ToList();
                
                Console.WriteLine($"Updated count: Found {addedEntities.Count} entities to save after state correction");
                
                // CRITICAL: Configure EF to ignore formula properties during save
                foreach (var entry in addedEntities)
                {
                    var entityType = entry.Entity.GetType();
                    
                    // Get all formula properties for this entity type
                    var formulaProperties = entityType.GetProperties()
                        .Where(p => IsFormulaProperty(p))
                        .ToList();
                    
                    // Console.WriteLine($"Processing {entityType.Name} with {formulaProperties.Count} formula properties");
                    
                    foreach (var formulaProp in formulaProperties)
                    {
                        try
                        {
                            // Find the EF property metadata
                            var efProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == formulaProp.Name);
                            if (efProperty != null)
                            {
                                // Mark as not modified and set a safe default value
                                efProperty.IsModified = false;
                                
                                // Set default values based on property type
                                if (formulaProp.PropertyType == typeof(bool?) || formulaProp.PropertyType == typeof(bool))
                                {
                                    efProperty.CurrentValue = false;
                                }
                                else if (formulaProp.PropertyType == typeof(string))
                                {
                                    efProperty.CurrentValue = string.Empty;
                                }
                                
                                // Console.WriteLine($"  Configured formula property: {formulaProp.Name}");
                            }
                            else
                            {
                                // Try to configure the property to be ignored at the model level
                                var entityTypeBuilder = _context.Model.FindEntityType(entityType);
                                if (entityTypeBuilder != null)
                                {
                                    var property = entityTypeBuilder.FindProperty(formulaProp.Name);
                                    if (property != null)
                                    {
                                        // This property exists in the model, we need to exclude it from the save
                                        Console.WriteLine($"  Found model property to ignore: {formulaProp.Name}");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Warning: Could not configure formula property {formulaProp.Name}: {ex.Message}");
                        }
                    }
                }
                
                // Additional step: Force all entities to be in Added state
                foreach (var entry in addedEntities)
                {
                    entry.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                }
                
                Console.WriteLine("Attempting to save all entities to database...");
                
                // Try a different approach: Use raw SQL to insert data and bypass EF property evaluation
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) when (ex.Message.Contains("Value cannot be null. (Parameter 'entity')"))
                {
                    Console.WriteLine("EF SaveChanges failed due to formula property evaluation. Trying raw SQL approach...");
                    
                    // Fall back to raw SQL insertion for problematic entities
                    await SaveEntitiesWithRawSqlAsync(addedEntities);
                }
                
                // Restore original settings
                _context.ChangeTracker.AutoDetectChangesEnabled = originalAutoDetectChanges;
                _context.ChangeTracker.LazyLoadingEnabled = originalLazyLoading;
                _context.ChangeTracker.QueryTrackingBehavior = originalQueryTracking;
                
                Console.WriteLine($"✓ Successfully saved all {addedEntities.Count} entities to database");
            }
            catch (Exception ex)
            {
                // Restore original settings in case of error
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
                _context.ChangeTracker.LazyLoadingEnabled = true;
                _context.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.TrackAll;
                
                Console.WriteLine($"✗ Error saving entities to database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        private async Task SaveEntitiesWithRawSqlAsync(List<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> addedEntities)
        {
            Console.WriteLine("Using raw SQL to insert entities and bypass formula property evaluation...");
            
            // Group entities by type for batch processing
            var entitiesByType = addedEntities.GroupBy(e => e.Entity.GetType()).ToList();
            
            foreach (var group in entitiesByType)
            {
                var entityType = group.Key;
                var entities = group.Select(e => e.Entity).ToList();
                
                Console.WriteLine($"Inserting {entities.Count} {entityType.Name} entities using raw SQL...");
                
                try
                {
                    await InsertEntitiesWithRawSqlAsync(entityType, entities);
                    Console.WriteLine($"✓ Successfully inserted {entities.Count} {entityType.Name} entities");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Failed to insert {entityType.Name} entities: {ex.Message}");
                    throw;
                }
            }
        }

        private async Task InsertEntitiesWithRawSqlAsync(Type entityType, List<object> entities)
        {
            if (entities.Count == 0) return;
            
            // Get the table name from EF model
            var entityTypeInfo = _context.Model.FindEntityType(entityType);
            if (entityTypeInfo == null)
            {
                throw new InvalidOperationException($"Could not find entity type info for {entityType.Name}");
            }
            
            var tableName = entityTypeInfo.GetTableName();
            
            // Get all non-formula properties that should be inserted
            var properties = entityType.GetProperties()
                .Where(p => p.CanRead && !IsFormulaProperty(p) && !IsNavigationProperty(p))
                .ToList();
            
            if (properties.Count == 0) return;
            
            // Build the INSERT statement
            var columnNames = string.Join(", ", properties.Select(p => $"[{p.Name}]"));
            
            var sql = $"INSERT INTO [{tableName}] ({columnNames}) VALUES ";
            var valuesClauses = new List<string>();
            var parameters = new List<Microsoft.Data.SqlClient.SqlParameter>();
            
            for (int entityIndex = 0; entityIndex < entities.Count; entityIndex++)
            {
                var entity = entities[entityIndex];
                var entityValueParams = new List<string>();
                
                for (int propIndex = 0; propIndex < properties.Count; propIndex++)
                {
                    var property = properties[propIndex];
                    var paramName = $"@{property.Name}_{entityIndex}";
                    entityValueParams.Add(paramName);
                    
                    var value = property.GetValue(entity);
                    parameters.Add(new Microsoft.Data.SqlClient.SqlParameter(paramName, value ?? DBNull.Value));
                }
                
                valuesClauses.Add($"({string.Join(", ", entityValueParams)})");
            }
            
            sql += string.Join(", ", valuesClauses);
            
            // Execute the raw SQL
            await _context.Database.ExecuteSqlRawAsync(sql, parameters.ToArray());
        }

        private async Task UpdateFormulaFieldsAsync()
        {
            // Console.WriteLine("Updating formula fields with actual JSON values...");
            
            foreach (var kvp in _originalJsonData)
            {
                var entityTypeName = kvp.Key;
                var jsonRecords = kvp.Value;
                
                if (jsonRecords.Count == 0) continue;
                
                // Console.WriteLine($"Updating formula fields for {entityTypeName} ({jsonRecords.Count} records)");
                
                // Get the entity type
                var entityType = GetEntityTypeByName(entityTypeName);
                if (entityType == null)
                {
                    Console.WriteLine($"Warning: Could not find entity type for {entityTypeName}");
                    continue;
                }
                
                // Get formula properties for this entity type
                var formulaProperties = entityType.GetProperties()
                    .Where(p => IsFormulaProperty(p))
                    .ToList();
                
                if (formulaProperties.Count == 0)
                {
                    // Console.WriteLine($"No formula properties found for {entityTypeName}");
                    continue;
                }
                
                // Console.WriteLine($"Found {formulaProperties.Count} formula properties: {string.Join(", ", formulaProperties.Select(p => p.Name))}");
                
                // Process each JSON record
                foreach (var jsonRecord in jsonRecords)
                {
                    await UpdateEntityFormulaFieldsAsync(entityType, jsonRecord, formulaProperties);
                }
            }
            
            Console.WriteLine("✓ Formula fields update completed");
        }

        private async Task UpdateEntityFormulaFieldsAsync(Type entityType, JsonElement jsonRecord, List<PropertyInfo> formulaProperties)
        {
            try
            {
                // Find the entity in the database by computing its primary key from the JSON
                var primaryKey = ComputePrimaryKeyFromJson(entityType, jsonRecord);
                if (string.IsNullOrEmpty(primaryKey))
                {
                    Console.WriteLine($"Warning: Could not compute primary key for {entityType.Name} record");
                    return;
                }
                
                // Build UPDATE SQL for formula fields
                var tableName = _context.Model.FindEntityType(entityType)?.GetTableName();
                if (string.IsNullOrEmpty(tableName))
                {
                    Console.WriteLine($"Warning: Could not find table name for {entityType.Name}");
                    return;
                }
                
                var updateClauses = new List<string>();
                var parameters = new List<Microsoft.Data.SqlClient.SqlParameter>();
                
                foreach (var formulaProp in formulaProperties)
                {
                    // Check if this formula field exists in the JSON
                    if (jsonRecord.TryGetProperty(formulaProp.Name, out var jsonValue))
                    {
                        var convertedValue = ConvertJsonValueToPropertyType(jsonValue, formulaProp.PropertyType);
                        
                        updateClauses.Add($"[{formulaProp.Name}] = @{formulaProp.Name}");
                        parameters.Add(new Microsoft.Data.SqlClient.SqlParameter($"@{formulaProp.Name}", convertedValue ?? DBNull.Value));
                    }
                }
                
                if (updateClauses.Count > 0)
                {
                    var primaryKeyProperty = GetPrimaryKeyProperty(entityType);
                    var sql = $"UPDATE [{tableName}] SET {string.Join(", ", updateClauses)} WHERE [{primaryKeyProperty.Name}] = @PrimaryKey";
                    parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PrimaryKey", primaryKey));
                    
                    await _context.Database.ExecuteSqlRawAsync(sql, parameters.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not update formula fields for {entityType.Name}: {ex.Message}");
            }
        }

        private Type? GetEntityTypeByName(string entityTypeName)
        {
            // Get all entity types from the context
            var entityTypes = _context.Model.GetEntityTypes()
                .Select(et => et.ClrType)
                .Where(t => t.Name == entityTypeName)
                .FirstOrDefault();
                
            return entityTypes;
        }

        private string ComputePrimaryKeyFromJson(Type entityType, JsonElement jsonRecord)
        {
            // Use the same logic as GenerateStablePrimaryKey to compute the primary key
            // This ensures we can find the entity that was created in PHASE 1
            return GenerateStablePrimaryKey(jsonRecord);
        }
        
        private PropertyInfo? FindForeignKeyPropertyFromConfiguration(Type entityType, string airtableFieldName, List<PropertyInfo> properties)
        {
            var entityConfig = _configuration.GetEntityByName(entityType.Name);
            if (entityConfig == null) return null;
            
            var fieldConfig = entityConfig.FindFieldByAirtableName(airtableFieldName);
            if (fieldConfig != null && fieldConfig.IsForeignKey)
            {
                var configProperty = properties.FirstOrDefault(p => 
                    string.Equals(p.Name, fieldConfig.Name, StringComparison.OrdinalIgnoreCase));
                
                // If the config property is a navigation property, look for the corresponding foreign key property
                if (configProperty != null && IsNavigationProperty(configProperty))
                {
                    // Try to find the foreign key property (e.g., Plan -> PlanId)
                    var foreignKeyProperty = properties.FirstOrDefault(p => 
                        string.Equals(p.Name, $"{fieldConfig.Name}Id", StringComparison.OrdinalIgnoreCase));
                    
                    if (foreignKeyProperty != null)
                    {
                        return foreignKeyProperty;
                    }
                }
                
                return configProperty;
            }
            
            // Try to find by foreign key reference mapping
            foreach (var field in entityConfig.Fields)
            {
                foreach (var fkRef in field.ForeignKeyReferences)
                {
                    if (string.Equals(fkRef.SymmetricProperty, airtableFieldName, StringComparison.OrdinalIgnoreCase))
                    {
                        var configProperty = properties.FirstOrDefault(p => 
                            string.Equals(p.Name, field.Name, StringComparison.OrdinalIgnoreCase));
                        
                        // If the config property is a navigation property, look for the corresponding foreign key property
                        if (configProperty != null && IsNavigationProperty(configProperty))
                        {
                            // Try to find the foreign key property (e.g., Plan -> PlanId)
                            var foreignKeyProperty = properties.FirstOrDefault(p => 
                                string.Equals(p.Name, $"{field.Name}Id", StringComparison.OrdinalIgnoreCase));
                            
                            if (foreignKeyProperty != null)
                            {
                                return foreignKeyProperty;
                            }
                        }
                        
                        return configProperty;
                    }
                }
            }
            
            return null;
        }
        
        private string[] GetExcludedForeignKeyFields()
        {
            var excludedFields = new List<string>();
            
            // Get all foreign key field names from configuration
            foreach (var entity in _configuration.Entities)
            {
                foreach (var field in entity.Fields)
                {
                    foreach (var fkRef in field.ForeignKeyReferences)
                    {
                        if (!excludedFields.Contains(fkRef.SymmetricProperty))
                        {
                            excludedFields.Add(fkRef.SymmetricProperty);
                        }
                    }
                }
            }
            
            return excludedFields.ToArray();
        }
        
        private string[] GetIdentifyingFieldPriority()
        {
            // Build priority list dynamically from schema
            var priorityFields = new List<string>();
            
            // Add common identifying field patterns
            var commonPatterns = new[] { "Email", "Name", "Title", "Code", "Id" };
            
            foreach (var entity in _configuration.Entities)
            {
                foreach (var field in entity.Fields)
                {
                    if (!field.IsFormula && !field.IsForeignKey)
                    {
                        foreach (var pattern in commonPatterns)
                        {
                            if (field.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase) && 
                                !priorityFields.Contains(field.Name))
                            {
                                priorityFields.Add(field.Name);
                            }
                        }
                    }
                }
            }
            
            // Add foreign key fields at the end (lower priority for primary key generation)
            foreach (var entity in _configuration.Entities)
            {
                foreach (var field in entity.Fields)
                {
                    if (field.IsForeignKey && !priorityFields.Contains(field.Name))
                    {
                        priorityFields.Add(field.Name);
                    }
                }
            }
            
            return priorityFields.ToArray();
        }
        
        private SyncConfiguration CreateFallbackConfiguration()
        {
            // Create a minimal fallback configuration for backward compatibility
            var config = new SyncConfiguration
            {
                Name = "Fallback Configuration",
                BaseId = "unknown"
            };
            
            // Add basic entity configurations based on DbSet properties
            var dbSetProperties = GetDbSetProperties();
            foreach (var dbSetProperty in dbSetProperties)
            {
                var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
                var entityConfig = new EntityConfiguration
                {
                    Name = entityType.Name,
                    AirtableName = dbSetProperty.Name,
                    PluralName = dbSetProperty.Name,
                    SingularName = entityType.Name
                };
                
                // Add basic field configurations
                var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in properties)
                {
                    var fieldConfig = new FieldConfiguration
                    {
                        Name = prop.Name,
                        OriginalName = prop.Name,
                        DataType = prop.PropertyType.Name,
                        IsPrimaryKey = prop.Name.EndsWith("Id") && prop.Name == $"{entityType.Name}Id",
                        IsFormula = IsFormulaPropertyByReflection(prop),
                        IsReadOnly = false,
                        IsNullable = Nullable.GetUnderlyingType(prop.PropertyType) != null
                    };
                    
                    entityConfig.Fields.Add(fieldConfig);
                }
                
                config.Entities.Add(entityConfig);
            }
            
            config.BuildDependencyGraph();
            return config;
        }
        
        private bool IsFormulaPropertyByReflection(PropertyInfo property)
        {
            var getter = property.GetGetMethod();
            var setter = property.GetSetMethod();
            
            if (getter == null || setter == null) return false;
            
            // Check if setter has empty body (common pattern for formula properties)
            try
            {
                var setterBody = setter.GetMethodBody();
                if (setterBody != null)
                {
                    var ilBytes = setterBody.GetILAsByteArray();
                    // Empty setter typically has very few IL bytes (just return)
                    if (ilBytes != null && ilBytes.Length <= 2)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // If we can't analyze the method body, fall back to naming patterns
            }
            
            // Check for common formula property naming patterns
            var formulaPatterns = new[] { "Full", "Display", "Calculated", "Computed", "Is", "Has", "Can" };
            return formulaPatterns.Any(pattern => property.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }
        
        private PropertyInfo? FindPropertyByConvention(List<PropertyInfo> properties, string airtableFieldName)
        {
            // Try common foreign key conventions
            var conventionMappings = new[]
            {
                $"{airtableFieldName}Id",       // Try this first - most common pattern (Plan -> PlanId)
                $"{airtableFieldName}IdValue",  // Alternative pattern used in some versions
                $"{airtableFieldName}ID",
                $"{airtableFieldName}_Id",
                $"{airtableFieldName}_ID"
            };
            
            foreach (var mapping in conventionMappings)
            {
                var property = properties.FirstOrDefault(p => 
                    string.Equals(p.Name, mapping, StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    return property;
                }
            }
            
            return null;
        }
    }
}
