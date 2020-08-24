using Generics;

public class CoroutineTicker : SingletonBehaviourPersistentGeneric<CoroutineTicker>
{
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}