public abstract class BaseManager : BaseBehaviour
{
    public bool isOnUpdate = false;
    public abstract void Initialize();
    public abstract void OnUpdate(float deltaTime);
    public abstract void OnDispose();
}