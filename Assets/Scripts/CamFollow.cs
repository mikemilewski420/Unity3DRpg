﻿#pragma warning disable 0649
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField]
    private Transform Knight, ShadowPriest, Toadstool;

    [SerializeField]
    private Vector3 Offset;

    private Vector3 NewPosition;

    [SerializeField]
    private float SmoothFactor, offsetY, offsetZ;

    private Quaternion CamTurn;

    private void LateUpdate()
    {
        if(Input.GetMouseButton(1))
        {
            CamTurn = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * 5, Vector3.up);

            Offset = CamTurn * Offset;
        }

        if(CheckKnightActive())
        {
            KnightPos();

            transform.LookAt(Knight);
        }
        if(CheckShadowPriestActive())
        {
            ShadowPriestPos();

            transform.LookAt(ShadowPriest);
        }
        if (CheckToadstoolActive())
        {
            ToadstoolPos();

            transform.LookAt(Toadstool);
        }
    }

    private bool CheckKnightActive()
    {
        return Knight.gameObject.activeInHierarchy;
    }

    private bool CheckShadowPriestActive()
    {
        return ShadowPriest.gameObject.activeInHierarchy;
    }

    private bool CheckToadstoolActive()
    {
        return Toadstool.gameObject.activeInHierarchy;
    }

    private void KnightPos()
    {
        NewPosition = Knight.position + Offset;

        transform.position = Vector3.Slerp(transform.position, NewPosition, SmoothFactor);
    }

    private void ShadowPriestPos()
    {
        NewPosition = ShadowPriest.position + Offset;

        transform.position = Vector3.Slerp(transform.position, NewPosition, SmoothFactor);
    }

    private void ToadstoolPos()
    {
        NewPosition = Toadstool.position + Offset;

        transform.position = Vector3.Slerp(transform.position, NewPosition, SmoothFactor);
    }
}
