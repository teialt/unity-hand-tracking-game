using UnityEngine;

public class CenterBlock : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 球消失
            Destroy(other.gameObject);

            //使用 GameManager 加分
            GameManager.Instance.AddScore(10);
        }
    }
}