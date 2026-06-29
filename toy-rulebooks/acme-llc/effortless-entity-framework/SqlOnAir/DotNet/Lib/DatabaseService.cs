using System;
using System.IO;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SqlOnAir.DotNet.Lib.DataClasses;
using DataClassesTest.Services;

namespace DataClassesTest.Services
{
    public class DatabaseService
    {
        public static async Task SetupDatabaseAsync(
            bool recreateDatabase = false,
            bool createIfNotExists = true,
            bool displayMockData = false)
        {
            Console.WriteLine("=== Database Setup Application ===");
            Console.WriteLine($"Options: Recreate={recreateDatabase}, Create={createIfNotExists}");
            Console.WriteLine();

            try
            {
                // Create connection string with unique application name and no pooling to avoid conflicts
                var connectionString = "Data Source=localhost;Database=SidebarSimpleMINI;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;Application Name=DatabaseSetup;Pooling=false;";
                
                // Create DbContext options
                var optionsBuilder = new DbContextOptionsBuilder<SoAEFContext>();
                optionsBuilder.UseSqlServer(connectionString);
                
                var context = new SoAEFContext(optionsBuilder.Options);
                try
                {
                    // Step 1: Handle database creation
                    await HandleDatabaseCreationAsync(context, recreateDatabase, createIfNotExists);
                    
                    // Step 2: Handle data synchronization (only when recreating database)
                    if (recreateDatabase)
                    {
                        await HandleDataSyncAsync(context);
                    }
                    
                    Console.WriteLine("✓ Database setup completed successfully!");
                }
                finally
                {
                    // Explicitly dispose the context and its connection
                    await context.DisposeAsync();
                }
                
                // Force garbage collection to ensure all connections are properly disposed
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error setting up database: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            // Optional: Load and display Airtable mock data for comparison
            if (displayMockData)
            {
                LoadAndDisplayAirtableData();
            }
        }

        private static async Task HandleDatabaseCreationAsync(SoAEFContext context, bool recreateDatabase, bool createIfNotExists)
        {
            if (recreateDatabase)
            {
                Console.WriteLine("Recreating database (delete + create)...");
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
                Console.WriteLine("✓ Database recreated successfully");
            }
            else if (createIfNotExists)
            {
                Console.WriteLine("Ensuring database exists...");
                var created = await context.Database.EnsureCreatedAsync();
                if (created)
                {
                    Console.WriteLine("✓ Database created successfully");
                }
                else
                {
                    Console.WriteLine("✓ Database already exists");
                }
            }
            else
            {
                Console.WriteLine("Skipping database creation");
            }
        }

        private static async Task HandleDataSyncAsync(SoAEFContext context)
        {
            Console.WriteLine("Synchronizing data with Airtable mock data...");
            await SyncAirtableDataAsync(context);
        }
        
        static async Task SyncAirtableDataAsync(SoAEFContext context)
        {
            // Path to the Airtable JSON file (relative to the project directory)
            var airtableJsonPath = Path.Combine("..", "SSoT", "Airtable.json");
            
            if (!File.Exists(airtableJsonPath))
            {
                Console.WriteLine($"Warning: Airtable.json not found at {airtableJsonPath}");
                Console.WriteLine("Skipping Airtable data sync.");
                return;
            }

            Console.WriteLine("=== Starting Data Synchronization ===");
            
            try
            {
                // Phase 1: Disable all foreign key constraints globally
                Console.WriteLine("Phase 1: Disabling foreign key constraints...");
                await DisableForeignKeyConstraintsAsync(context);

                // Phase 2: Sync data without worrying about circular references
                var syncService = new SyncService(context, airtableJsonPath);
                Console.WriteLine("Phase 2: Full sync - Adding missing rows and updating existing rows");
                await syncService.SyncAllDataAsync();

                // Phase 3: Re-enable foreign key constraints
                Console.WriteLine("Phase 3: Re-enabling foreign key constraints...");
                await EnableForeignKeyConstraintsAsync(context);
                
                Console.WriteLine("✓ Data synchronization completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error during data synchronization: {ex.Message}");
                
                // Always try to re-enable constraints even if sync failed
                try
                {
                    Console.WriteLine("Attempting to re-enable foreign key constraints after error...");
                    await EnableForeignKeyConstraintsAsync(context);
                }
                catch (Exception constraintEx)
                {
                    Console.WriteLine($"Warning: Could not re-enable constraints: {constraintEx.Message}");
                }
                
                throw; // Re-throw the original exception
            }
        }

        /// <summary>
        /// Disables all foreign key constraints in the database
        /// </summary>
        private static async Task DisableForeignKeyConstraintsAsync(SoAEFContext context)
        {
            try
            {
                await context.Database.ExecuteSqlRawAsync("EXEC sp_msforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"");
                Console.WriteLine("✓ Foreign key constraints disabled");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not disable foreign key constraints: {ex.Message}");
            }
        }

        /// <summary>
        /// Re-enables all foreign key constraints in the database
        /// </summary>
        private static async Task EnableForeignKeyConstraintsAsync(SoAEFContext context)
        {
            try
            {
                // First, try to check all constraints with trust
                await context.Database.ExecuteSqlRawAsync("EXEC sp_msforeachtable \"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all\"");
                Console.WriteLine("✓ Foreign key constraints re-enabled with validation");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not re-enable constraints with validation: {ex.Message}");
                
                // Fallback: Enable constraints without validation
                try
                {
                    await context.Database.ExecuteSqlRawAsync("EXEC sp_msforeachtable \"ALTER TABLE ? CHECK CONSTRAINT all\"");
                    Console.WriteLine("✓ Foreign key constraints re-enabled without validation");
                }
                catch (Exception fallbackEx)
                {
                    Console.WriteLine($"Error: Could not re-enable constraints: {fallbackEx.Message}");
                    // Don't throw here - we want the sync to continue even if constraints can't be re-enabled
                }
            }
        }

        static void LoadAndDisplayAirtableData()
        {
            Console.WriteLine();
            Console.WriteLine("--- Loading Airtable Mock Data for Comparison ---");
            
            try
            {
                // Path to the Airtable.json file relative to the working directory
                string airtableJsonPath = Path.Combine("..", "SSoT", "Airtable.json");
                
                if (!File.Exists(airtableJsonPath))
                {
                    Console.WriteLine($"✗ Airtable.json file not found at: {airtableJsonPath}");
                    return;
                }

                // Read the JSON file
                string jsonContent = File.ReadAllText(airtableJsonPath);
                Console.WriteLine($"✓ Successfully loaded Airtable.json ({jsonContent.Length} characters)");
                
                // Parse and re-serialize with pretty formatting for display
                using (JsonDocument document = JsonDocument.Parse(jsonContent))
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    
                    string formattedJson = JsonSerializer.Serialize(document.RootElement, options);
                    
                    Console.WriteLine();
                    Console.WriteLine("=== AIRTABLE MOCK DATA (JSON SERIALIZATION) ===");
                    Console.WriteLine(formattedJson);
                    Console.WriteLine("=== END AIRTABLE MOCK DATA ===");
                }
                
                Console.WriteLine();
                Console.WriteLine("✓ Airtable mock data loaded and displayed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error loading Airtable data: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Displays data from all tables in the database
        /// </summary>
        public static async Task DisplayAllDataAsync()
        {
            Console.WriteLine("=== Database Table Data ===");
            Console.WriteLine();

            try
            {
                // Create connection string with unique application name for display
                var connectionString = "Data Source=localhost;Database=SidebarSimpleMINI;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;Application Name=DataDisplay;Pooling=false;";
                
                var optionsBuilder = new DbContextOptionsBuilder<SoAEFContext>();
                optionsBuilder.UseSqlServer(connectionString);
                
                using var context = new SoAEFContext(optionsBuilder.Options);
                
                // Get all DbSet properties using reflection
                var dbSetProperties = typeof(SoAEFContext)
                    .GetProperties()
                    .Where(p => p.PropertyType.IsGenericType && 
                               p.PropertyType.GetGenericTypeDefinition() == typeof(Microsoft.EntityFrameworkCore.DbSet<>))
                    .ToList();

                foreach (var property in dbSetProperties)
                {
                    await DisplayTableDataAsync(context, property);
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error displaying data: {ex.Message}");
            }
        }

        /// <summary>
        /// Displays data from a specific table using reflection
        /// </summary>
        private static async Task DisplayTableDataAsync(SoAEFContext context, System.Reflection.PropertyInfo dbSetProperty)
        {
            try
            {
                var tableName = dbSetProperty.Name;
                Console.WriteLine($"--- {tableName} Table ---");
                
                // Get the DbSet
                var dbSet = dbSetProperty.GetValue(context);
                if (dbSet == null) return;

                // Get the entity type
                var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
                
                // Use reflection to call AsNoTracking().ToListAsync() to avoid lazy loading issues
                var asNoTrackingMethod = typeof(Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions)
                    .GetMethods()
                    .Where(m => m.Name == "AsNoTracking" && m.GetParameters().Length == 1)
                    .FirstOrDefault()
                    ?.MakeGenericMethod(entityType);

                if (asNoTrackingMethod == null) return;

                var noTrackingQuery = asNoTrackingMethod.Invoke(null, new[] { dbSet });

                var toListAsyncMethod = typeof(Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions)
                    .GetMethods()
                    .Where(m => m.Name == "ToListAsync" && m.GetParameters().Length == 2)
                    .FirstOrDefault()
                    ?.MakeGenericMethod(entityType);

                if (toListAsyncMethod == null) return;

                var entities = await (dynamic)toListAsyncMethod.Invoke(null, new[] { noTrackingQuery, CancellationToken.None });
                var entityList = (System.Collections.IList)entities;
                
                Console.WriteLine($"Total {tableName}: {entityList.Count}");
                
                if (entityList.Count == 0)
                {
                    Console.WriteLine("  (No data)");
                    return;
                }

                // Display first few records with their properties
                var displayCount = Math.Min(3, entityList.Count);
                var properties = entityType.GetProperties()
                    .Where(p => p.CanRead && IsSimpleType(p.PropertyType) && !IsNavigationProperty(p))
                    .Take(5) // Limit to first 5 properties to keep output manageable
                    .ToList();

                for (int i = 0; i < displayCount; i++)
                {
                    var entity = entityList[i];
                    Console.WriteLine($"  Record {i + 1}:");
                    
                    foreach (var prop in properties)
                    {
                        try
                        {
                            var value = prop.GetValue(entity);
                            var displayValue = value?.ToString() ?? "null";
                            if (displayValue.Length > 50) displayValue = displayValue.Substring(0, 47) + "...";
                            Console.WriteLine($"    {prop.Name}: {displayValue}");
                        }
                        catch
                        {
                            Console.WriteLine($"    {prop.Name}: (error reading value)");
                        }
                    }
                    Console.WriteLine("    ---");
                }
                
                if (entityList.Count > displayCount)
                {
                    Console.WriteLine($"  ... and {entityList.Count - displayCount} more records");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Error displaying {dbSetProperty.Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a type is a simple type that can be easily displayed
        /// </summary>
        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive ||
                   type == typeof(string) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid) ||
                   type == typeof(decimal) ||
                   Nullable.GetUnderlyingType(type) != null && IsSimpleType(Nullable.GetUnderlyingType(type));
        }

        /// <summary>
        /// Checks if a property is a navigation property that could trigger lazy loading
        /// </summary>
        private static bool IsNavigationProperty(System.Reflection.PropertyInfo property)
        {
            // Skip virtual properties (likely navigation properties)
            if (property.GetMethod?.IsVirtual == true && !property.GetMethod.IsFinal)
                return true;

            // Skip collection types (navigation properties)
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) && 
                property.PropertyType != typeof(string))
                return true;

            // Skip complex entity types (other entities in our namespace)
            if (property.PropertyType.Namespace?.Contains("SqlOnAir.DotNet.Lib.DataClasses") == true)
                return true;

            return false;
        }
    }
}
