using UnityEngine;

public class FlagSprite : MonoBehaviour
{
    public LevelManager levelManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == levelManager.MonsterManager.transform.GetChild(1).transform)
        {
            if (Random.value < 0.25)
            {
                Destroy(gameObject);
            }
        }
    }
}
