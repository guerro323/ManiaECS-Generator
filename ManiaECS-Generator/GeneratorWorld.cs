using System;
using System.Collections.Generic;
using System.Text;

namespace ManiaECS_Generator
{
    public class GeneratorWorld
    {
        private readonly Dictionary<Type, SystemBase> allSystems;
        public readonly IReadOnlyDictionary<Type, SystemBase> systems;

        public GeneratorWorld()
        {
            allSystems = new Dictionary<Type, SystemBase>();
            systems = allSystems;
        }

        /// <summary>
        /// Get an already existing system.
        /// </summary>
        /// <typeparam name="T">The system type to get</typeparam>
        /// <returns>The system</returns>
        public T GetSystem<T>()
            where T : SystemBase
        {
            return (T)allSystems[typeof(T)];
        }

        /// <summary>
        /// Add a new system
        /// </summary>
        /// <typeparam name="T">The system type</typeparam>
        /// <returns>The current world</returns>
        public GeneratorWorld AddSystem<T>()
               where T : SystemBase, new()
        {
            return SetSystem(new T());
        }

        /// <summary>
        /// Set an already existing system to the world
        /// </summary>
        /// <typeparam name="T">The system type</typeparam>
        /// <param name="system">The existing system</param>
        /// <returns>The current world</returns>
        public GeneratorWorld SetSystem<T>(T system)
            where T : SystemBase
        {
            allSystems[typeof(T)] = system;

            system.InternalOnGeneratorPass();

            return this;
        }
    }
}
