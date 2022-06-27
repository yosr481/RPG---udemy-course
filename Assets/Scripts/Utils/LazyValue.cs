namespace RPG.Utils
{
    /// <summary>
    /// Container class that wraps a value and ensures initialisation is 
    /// called just before first use.
    /// </summary>
    public class LazyValue<T>
    {
        private T value;
        private bool initialized;
        private readonly InitializerDelegate initializer;

        public delegate T InitializerDelegate();

        /// <summary>
        /// Setup the container but don't initialise the value yet.
        /// </summary>
        /// <param name="initializer"> 
        /// The initialiser delegate to call when first used. 
        /// </param>
        public LazyValue(InitializerDelegate initializer)
        {
            this.initializer = initializer;
        }

        /// <summary>
        /// Get or set the contents of this container.
        /// </summary>
        /// <remarks>
        /// Note that setting the value before initialisation will initialise 
        /// the class.
        /// </remarks>
        public T Value
        {
            get
            {
                // Ensure we init before returning a value.
                ForceInit();
                return value;
            }
            set
            {
                // Don't use default init anymore.
                initialized = true;
                this.value = value;
            }
        }

        /// <summary>
        /// Force the initialisation of the value via the delegate.
        /// </summary>
        public void ForceInit()
        {
            if (!initialized)
            {
                value = initializer();
                initialized = true;
            }
        }
    }
}