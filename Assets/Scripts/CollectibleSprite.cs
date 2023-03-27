using UnityEngine;

public class CollectibleSprite : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == LevelManager.Instance.PlayerManager.transform)
        {
            Destroy(gameObject);
            SendMessageUpwards("OnCollect", gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }
}
