using UnityEngine;

public class TriggerSprite : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == LevelManager.Instance.PlayerManager.transform)
        {
            SendMessageUpwards("OnSpriteTrigger", gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }
}
