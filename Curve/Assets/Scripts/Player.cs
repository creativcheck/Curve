using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject deathScreen;
    [SerializeField] Transform rightPoint, leftPoint, rightBone, leftBone;
    [SerializeField] float mainFallSpeed = 0.02f, fallSpeedIncreaser, mouseSpeedToFall, mainFallPosition, pushDistance;
    [SerializeField] SplineContainer splineContainer;
    [SerializeField] Transform debug;
    [SerializeField] float distanceBetweenBoneAndPoint;
    [SerializeField] PostProcessManager postProcessManager;
    [SerializeField] float handsDistance;
    [Header("Настройки критической высоты подъема или съезда по стене")]
    [SerializeField][Range(0, 0.5f)] float maxHeight;
    [SerializeField][Range(0.5f, 1)] float minHeight;
    [Header("Настройки усталости")]
    [SerializeField] float fatigueRaseByPush;
    [SerializeField] float fatigueRaseByTime;
    [SerializeField] float relaxFatigueValue;
    [SerializeField] float fatigueLimit;
    [Header("Звуки")]
    [SerializeField] AudioSource leftPushSound;
    [SerializeField] AudioSource rightPushSound;
    [SerializeField] AudioClip[] pushAudios;
    [SerializeField] AudioSource heatbeatSound, earRingSound;
    [SerializeField] AudioSource screamSound;
    [SerializeField] AudioClip[] screamAudios;

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

        leftHand = new Hand(leftPoint, transform, leftBone, distanceBetweenBoneAndPoint, playerTrack, handsDistance);
        rightHand = new Hand(rightPoint, transform, rightBone, distanceBetweenBoneAndPoint, playerTrack, handsDistance);

        playerInput = new PlayerInput();
        playerActions = new PlayerActions(postProcessManager, playerInput, playerTrack, mainFallSpeed, transform, debug, 
            mouseSpeedToFall, fallSpeedIncreaser, mainFallPosition, pushDistance, maxHeight, minHeight, leftHand, rightHand,
            fatigueRaseByPush, fatigueRaseByTime, fatigueLimit, relaxFatigueValue,
            earRingSound, heatbeatSound, leftPushSound, rightPushSound, pushAudios, screamSound, screamAudios,
            deathScreen);
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
        rightPoint.localRotation = Quaternion.Euler(-7, -63, 40); // руки в нужном положении на старте
        leftPoint.localRotation = Quaternion.Euler(-7, 63, -40);
        firstUpdate = false;
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
