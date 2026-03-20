using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class PlaneTiltController : MonoBehaviour
{
    public LeapProvider leapProvider;
    public float tiltSensitivity = 30f;  // 手势灵敏度
    public float maxTiltAngle = 15f;     // 最大允许倾斜角度

    private static float tiltFx0 = 0f;
    private static float tiltFz0 = 0f;

    private float tiltFx, tiltFz;
    private bool isFrozen = false;

    public void Freeze()
    {
        isFrozen = true;
    }

    public void Unfreeze()
    {
        Debug.Log($"[Unfreeze] 平台即将反向旋转回水平。X方向: {-tiltFx0:F2}°，Z方向: {-tiltFz0:F2}°");
        isFrozen = false;

        Vector3 pivotWorldPosition = new Vector3(3, 0, 3); // 你原来旋转的中心点
        transform.RotateAround(pivotWorldPosition, Vector3.right, -tiltFx0);
        transform.RotateAround(pivotWorldPosition, Vector3.forward, -tiltFz0);

        //重置累计角度
        tiltFx0 = 0f;
        tiltFz0 = 0f;

        MazeGenerator.Instance.GenerateNewMaze();
    }

    void Update()
    {
        Debug.Log("Update running. Frozen? " + isFrozen);
        if (isFrozen) return;

        Frame frame = leapProvider.CurrentFrame;
        Hand rightHand = frame.GetHand(Chirality.Right);

        if (rightHand == null)
        {
            Debug.LogWarning("[Update] No right hand detected.");
            return;
        }

        if (rightHand != null)
        {
            Vector3 palmNormal = rightHand.PalmNormal;
            Vector3 pivotWorldPosition = new Vector3(3, 0, 3); 

            Debug.Log("[Update] Right hand detected. Proceeding to control platform.");

            tiltFx = -palmNormal.z * tiltSensitivity;
            tiltFz = palmNormal.x * tiltSensitivity;

            float tiltFx1 = tiltFx0 + tiltFx;
            float tiltFz1 = tiltFz0 + tiltFz;

            Debug.Log($"[角度追踪] tiltFx0 = {tiltFx0:F2}, tiltFz0 = {tiltFz0:F2}");

            if (tiltFx1 >= maxTiltAngle || tiltFx1 <= -maxTiltAngle ||
                tiltFz1 >= maxTiltAngle || tiltFz1 <= -maxTiltAngle)
            {
                return;
            }

            transform.RotateAround(pivotWorldPosition, Vector3.right, tiltFx);
            transform.RotateAround(pivotWorldPosition, Vector3.forward, tiltFz);

            //只有旋转成功才累计角度
            tiltFx0 = tiltFx1;
            tiltFz0 = tiltFz1;

            tiltFx = 0;
            tiltFz = 0;
        }
    }
}