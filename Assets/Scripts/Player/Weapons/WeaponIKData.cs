using UnityEngine;

[CreateAssetMenu(fileName = "WeaponIKData", menuName = "Scriptable Objects/WeaponIKData")]
public class WeaponIKData : ScriptableObject
{
    [Header("Right Hand IK")]
    public Vector3 rightHandControllerPosition;
    public Vector3 rightHandControllerRotation;
    [Header("Left Hand IK")]
    public Vector3 leftHandControllerPosition;
    public Vector3 leftHandControllerRotation;
    public Vector3 leftHandHintPosition;
    public Vector3 leftHandHintRotation;
    [Header("IK Weights")]
    public float leftHandIKWeight;
    public float rightHandIKWeight;

}
