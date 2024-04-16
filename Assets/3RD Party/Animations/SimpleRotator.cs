using UnityEngine;

public enum eUpdateType
{
    Update,
    FixedUpdate,
    LateUpdate
}

public class SimpleRotator : MonoBehaviour
{
    public eUpdateType UpdateType;
    public float Speed;
    public Vector3 RotationVector;

    void Update()
    {
        if(UpdateType == eUpdateType.Update)
            Rotate();
    }

    private void FixedUpdate()
    {
        if (UpdateType == eUpdateType.FixedUpdate)
            Rotate();
    }

    private void LateUpdate()
    {
        if (UpdateType == eUpdateType.LateUpdate)
            Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(RotationVector * Speed * Time.deltaTime);
    }
}