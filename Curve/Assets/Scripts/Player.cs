using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class Player : MonoBehaviour
{
    [SerializeField] Transform rightPoint, leftPoint, rightBone, leftBone;
    [SerializeField] float mainFallSpeed = 0.02f, fallSpeedIncreaser, mouseSpeedToFall, mainFallPosition, pushDistance;
    [SerializeField] SplineContainer splineContainer;
    [SerializeField] Transform debug;
    [SerializeField] float distanceBetweenBoneAndPoint;

    SplinePath playerTrack;
    PlayerActions playerActions;
    PlayerInput playerInput;
    Hand leftHand, rightHand;

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

        leftHand = new Hand(leftPoint, transform, leftBone, distanceBetweenBoneAndPoint, playerTrack);
        rightHand = new Hand(rightPoint, transform, rightBone, distanceBetweenBoneAndPoint, playerTrack);

        playerInput = new PlayerInput();
        playerActions = new PlayerActions(playerInput, playerTrack, mainFallSpeed, transform, debug, 
            mouseSpeedToFall, fallSpeedIncreaser, mainFallPosition, pushDistance, leftHand, rightHand);
    }


    void Update()
    {
        if(firstUpdate)
            UpdateIKPoints();

        playerActions.Update();
    }

    private void OnDestroy()
    {
        playerActions.Expose();
    }

    private void UpdateIKPoints()
    {
        rightPoint.rotation = Quaternion.Euler(-7, -75, -55); // руки в нужном положении на старте
        leftPoint.rotation = Quaternion.Euler(-7, 75, 55);
        firstUpdate = false;
    }
}
