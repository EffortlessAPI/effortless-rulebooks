using System;

namespace SqlOnAir.DotNet.Lib.DataClasses
{
    public abstract class SoAEntityBase
    {
        public SoAEFContext? Context { get; set; }

        public void SetContext(SoAEFContext context)
        {
            if (Context != null && Context != context)
            {
                throw new InvalidOperationException("Cannot change the context of an entity once it has been set.");
            }
            Context = context;
            this.LazyLoadProperties();
        }

        protected abstract void LazyLoadProperties();
    }
}
