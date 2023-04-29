using UnityEngine;

public class TriggerSprite : MonoBehaviour
{
    public LevelManager levelManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == levelManager.PlayerManager.transform)
        {
            SendMessageUpwards("OnSpriteTrigger", gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }
}
