using System;

namespace SqlOnAir.DotNet.Lib.DataClasses
{
    public abstract class SoAEntityBase
    {
        public SoAEFContext? SoAContext { get; set; }

        public void SetContext(SoAEFContext context)
        {
            if (SoAContext != null && SoAContext != context)
            {
                throw new InvalidOperationException("Cannot change the context of an entity once it has been set.");
            }
            SoAContext = context;
            this.LazyLoadProperties();
        }

        protected abstract void LazyLoadProperties();
    }
}
