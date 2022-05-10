using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Path : MonoBehaviour
{
    public static Path instance;
    [SerializeField] private List<Bend> _bendsList = new();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _bendsList.ForEach(Construct);
    }

    private void Construct(Bend bend)
    {
        Vector3 bendQuitPoint = bend.bendQuitPoint.position;
        Vector3 bendEnterPoint = bend.bendEnterPoint.position;
        Vector3 bendCenterPoint = new Vector3(
            bendQuitPoint.x,
            bendQuitPoint.y,
            bendEnterPoint.z
        );
        bend.BendCenterPoint = bendCenterPoint;
        bend.R = Vector2.Distance(new Vector2(bendCenterPoint.x, bendCenterPoint.z), new Vector2(bendQuitPoint.x, bendQuitPoint.z));

    }

    public Bend GetBend(Collider bendEnterPointCollider)
    {
        Bend result = _bendsList.First(bend => bend.bendEnterPoint.GetComponent<Collider>() == bendEnterPointCollider);
        return result;
    }




}

[Serializable]
public class Bend
{
    public Transform bendEnterPoint; 
    public Transform bendQuitPoint;

    private float _r;
    private Vector3 _bendCenterPoint;

    public float R
    {
        get => _r;
        set => _r = value;
    }
    public Vector3 BendCenterPoint
    {
        get => _bendCenterPoint;
        set => _bendCenterPoint = value;
    }
    public float CalculateBendTime()
    {
        PlayerController player = PlayerController.instance;
        Vector3 playerPosition = player.transform.position;
        float playerSpeed = player.GetForwardSpeed;
        float distance = Vector2.Distance(new Vector2(BendCenterPoint.x, BendCenterPoint.z), new Vector2(playerPosition.x, playerPosition.z));
        float x = (2 * Mathf.PI * distance) / 4;
        //x = v * t;
        return x / playerSpeed;
    }


}