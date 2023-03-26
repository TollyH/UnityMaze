using UnityEngine;

public class CollectibleSprite : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            Destroy(gameObject);
            SendMessageUpwards("OnCollect", gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }
}
