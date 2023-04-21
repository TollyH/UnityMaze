using UnityEngine;

public class FlagSprite : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == LevelManager.Instance.MonsterManager.transform.GetChild(1).transform)
        {
            if (Random.value < 0.25)
            {
                Destroy(gameObject);
            }
        }
    }
}
