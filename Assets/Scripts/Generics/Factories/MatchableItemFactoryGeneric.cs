using ObjectPools;
using UnityEngine;

namespace Factories
{
    public class MatchableItemFactoryGeneric<T> where T : struct
    {
        public IMatchableItem<T> ConstructItem(string path) => ConstructItem(path, Vector2.zero, default);
        
        public IMatchableItem<T> ConstructItem(string path, Vector2 position, T criteria)
        {
            var objectFromPool = GameObjectPool.GetObjectFromPool(path);
            objectFromPool.transform.position = position;

            var iMatchableItem = objectFromPool.GetComponent<IMatchableItem<T>>();
            iMatchableItem.UpdateData(criteria);
            iMatchableItem.CollisionHandler.SwitchPhysicsSimulation(false);

            return iMatchableItem;
        }
    }
}