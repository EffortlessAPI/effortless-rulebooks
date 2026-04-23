// In-memory replacement for the EF Core SoAEFContext. The generated
// dataclasses call Context.<Plural>.Find(Guid), Context.<Plural>.Where(...),
// Context.Attach(...), Context.AttachRange(...). We satisfy that surface
// with plain lists — no database, no EF Core — so calculated/lookup/
// aggregation property getters can resolve cross-entity references purely
// in memory.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SqlOnAir.DotNet.Lib.DataClasses
{
    public sealed class InMemorySet<T> : IEnumerable<T> where T : class
    {
        private readonly List<T> _items = new();
        private readonly PropertyInfo _keyProp;

        public InMemorySet()
        {
            _keyProp = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .First(p => p.PropertyType == typeof(Guid)
                            && p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true).Any());
        }

        public void Add(T item) => _items.Add(item);
        public IReadOnlyList<T> AsReadOnly() => _items;

        public T? Find(Guid id)
        {
            foreach (var item in _items)
            {
                if ((Guid)_keyProp.GetValue(item)! == id) return item;
            }
            return null;
        }

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }

    public class SoAEFContext
    {
        public static bool ThrowErrorOnContextMissing { get; set; } = false;

        public InMemorySet<Customer> Customers { get; } = new();
        public InMemorySet<Employee> Employees { get; } = new();
        public InMemorySet<Project> Projects { get; } = new();
        public InMemorySet<Role> Roles { get; } = new();
        public InMemorySet<TypesOfProject> TypesOfProjects { get; } = new();

        public void Attach(object _) { /* no-op: entities are already in-memory */ }
        public void AttachRange(IEnumerable<object> _) { /* no-op */ }
    }
}
