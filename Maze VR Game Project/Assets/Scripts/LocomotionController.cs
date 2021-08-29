using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


// �ڷ���Ʈ �׻� Ȱ��ȭ�� ���� �ʵ��� �����ϴ� ��ũ��Ʈ.
public class LocomotionController : MonoBehaviour
{
    public XRController leftTeleportRay;
    public XRController rightTeleportRay;
    public InputHelpers.Button teleportActivationButton;
    public float activationTheshold = 0.1f;

    // ������ �������� �ڷ���Ʈ�� ���� �ʵ��� �ϴ°�.
    public bool EnableLeftTeleport { get; set; } = true;
    public bool EnableRightTeleport { get; set; }  = true;


    // Update is called once per frame
    void Update()
    {
        if (leftTeleportRay)
        {
            leftTeleportRay.gameObject.SetActive(EnableLeftTeleport && CheckIfActivated(leftTeleportRay));
        }

        if (rightTeleportRay)
        {
            rightTeleportRay.gameObject.SetActive(EnableRightTeleport && CheckIfActivated(rightTeleportRay));
        }

    }
    public bool CheckIfActivated(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, activationTheshold);
        return isActivated;
    }
}
