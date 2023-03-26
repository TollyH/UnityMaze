using UnityEngine;

public class TriggerSprite : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            SendMessageUpwards("OnSpriteTrigger", gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }
}
