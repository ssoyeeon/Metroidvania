using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed = 2.0f;            //ī�޶� ĳ���͸� ���󰡴� ���ǵ�
    public Transform Target;                    //Ÿ�� Ʈ������

    private Transform camTransfrom;             //ī�޶��� Ʈ������

    public float shakeDruation = 0.0f;
    public float shakeAmount = 0.1f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;                        //���� ��ġ 

    private void OnEnable()
    {
        //originalPos = camTransfrom.localPosition;
    }
    void Update()
    {
        Vector3 newPosition = Target.position;
        newPosition.z = -10;
        transform.position = Vector3.Slerp(transform.position, newPosition, FollowSpeed * Time.deltaTime);
    }
}
