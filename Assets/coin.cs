using UnityEngine;

public class Coin : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(1); //使用 GameManager 加分

            MazeGenerator.Instance.SpawnOneCoin(); //保持原逻辑

            Destroy(gameObject);
        }
    }
}