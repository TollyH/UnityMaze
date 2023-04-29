using UnityEngine;

public class CollectibleSprite : MonoBehaviour
{
    public LevelManager levelManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == levelManager.PlayerManager.transform)
        {
            Destroy(gameObject);
            SendMessageUpwards("OnCollect", gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }
}
