using Generics;

public class CoroutineTicker : SingletonBehaviourGeneric<CoroutineTicker>
{
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}