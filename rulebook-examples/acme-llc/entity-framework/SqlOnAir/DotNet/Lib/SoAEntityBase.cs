using System;
using System.ComponentModel.DataAnnotations;

namespace SqlOnAir.DotNet.Lib
{
    /// <summary>
    /// Simple base class for testing data classes without full Entity Framework context
    /// </summary>
    public abstract class SoAEntityBase
    {
        /// <summary>
        /// Mock context property for testing
        /// </summary>
        public virtual object? Context { get; set; }

        /// <summary>
        /// Typed context property for Entity Framework operations
        /// </summary>
        protected SqlOnAir.DotNet.Lib.DataClasses.SoAEFContext? TypedContext => Context as SqlOnAir.DotNet.Lib.DataClasses.SoAEFContext;

        /// <summary>
        /// Set the context for this entity
        /// </summary>
        /// <param name="context"></param>
        public virtual void SetContext(object context)
        {
            Context = context;
        }

        /// <summary>
        /// Override in derived classes to implement lazy loading
        /// </summary>
        protected virtual void LazyLoadProperties()
        {
            // Base implementation does nothing
        }

        /// <summary>
        /// Override in derived classes to provide string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString() ?? string.Empty;
        }
    }
}
