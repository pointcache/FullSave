namespace pointcache.FullSave.Internal
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Temporary data storage for saving
    /// </summary>
    public class SerializationData
    {
        public List<GameObjectData> gameObjects = new List<GameObjectData>();
        public List<SavedComponent> components = new List<SavedComponent>();
    } 
}