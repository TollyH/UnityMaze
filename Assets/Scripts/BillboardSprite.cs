using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.localRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
    }
}
