using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private CheckpointData checkPointData = null;
    [SerializeField] private LayerMask targetLayerMask = default;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (0 != (targetLayerMask.value & 1 << other.gameObject.layer))
        {
            Teleport(other.transform);
        }
    }

    private void Teleport(Transform target)
    {
        target.transform.position = checkPointData.InitialValue;
        CameraController.Instance.ChangeCamera(checkPointData.CameraIndex);
    }
}
