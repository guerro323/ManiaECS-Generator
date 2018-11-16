using System;
using System.Collections.Generic;
using System.Text;

namespace ManiaECS_Generator
{
    /// <summary>
    /// Offer a base for all systems from a <see cref="GeneratorWorld"/>.
    /// </summary>
    public abstract class SystemBase
    {
        private bool generatorAsAtLeastPassedOnce;

        internal void InternalOnGeneratorPass()
        {
            OnGeneratorPass(generatorAsAtLeastPassedOnce);

            generatorAsAtLeastPassedOnce = true;
        }

        /// <summary>
        /// Did the generator created us?
        /// </summary>
        /// <param name="calledOneTime">Did it he did one time or multiple time?</param>
        protected abstract void OnGeneratorPass(bool calledOneTime);
    }
}
