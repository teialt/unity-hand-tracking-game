using UnityEngine;

public class BallFreezer : MonoBehaviour
{
    public Rigidbody[] balls;  // 拖入所有球的 Rigidbody 组件

    public void FreezeBalls()
    {
        foreach (Rigidbody rb in balls)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true; // 冻结物理
            Debug.Log("球体已冻结！");
        }
    }
}
