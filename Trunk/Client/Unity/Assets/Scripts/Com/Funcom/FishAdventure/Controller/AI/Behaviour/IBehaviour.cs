public interface IBehaviour
{
    int GetWeight();

    BehaviourType GetBehaviourType();

    void Start();

    void GenericTweenUpdate(object args);

    void Update();

    void LateUpdate();

    void OnDrawGizmos();

    void Stop();
}