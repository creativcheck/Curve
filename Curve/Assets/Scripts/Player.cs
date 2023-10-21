using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class Player : MonoBehaviour
{
    [SerializeField] Transform rightPoint, leftPoint;
    [SerializeField] float fallSpeed = 0.02f;
    [SerializeField] SplineContainer splineContainer;
    [SerializeField] Transform debug;

    SplinePath playerTrack;

    private Vector3 rightPointPos, leftPointPos;
    private Quaternion rightPointRot, leftPointRot;
    private bool firstUpdate = true;

    void Start()
    {
        rightPointPos = rightPoint.position;
        leftPointPos = leftPoint.position;
        rightPointRot = rightPoint.rotation;
        leftPointRot = leftPoint.rotation;

        playerTrack = new SplinePath(new[]
        {
                new SplineSlice<Spline>(splineContainer.Splines[0], new SplineRange(0, 6),
                    splineContainer.transform.localToWorldMatrix)
        });
    }


    void Update()
    {
        if(firstUpdate)
            UpdateIKPoints();

        UpdateFalling();
    }

    private void UpdateFalling()
    {
        playerTrack.Evaluate(math.frac(fallSpeed * Time.time), out var pos, out var right, out var up);
        //Vector3 forward = Vector3.Cross(right, up);
        transform.position = pos;
        pos += up;

        debug.position = pos;
        transform.LookAt(debug);
    }

    private void UpdateIKPoints()
    {
        rightPoint.rotation = Quaternion.Euler(-7, -75, -55);
        leftPoint.rotation = Quaternion.Euler(-7, 75, 55);
        firstUpdate = false;
    }
}
