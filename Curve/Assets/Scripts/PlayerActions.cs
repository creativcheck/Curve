using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PlayerActions
{
    SplinePath playerTrack;
    PlayerInput playerInput;
    float mainFallSpeed, fallIncreaser, mouseSpeedToFall, pushDistance;
    Transform playerTransform, debug;
    Hand leftHand, rightHand;

    float currentFallSpeed, currentFallPosition;
    bool deathFalling, leftHandLock, rightHandLock;

    public PlayerActions(PlayerInput input, SplinePath path, float fSpeed, Transform player, Transform debug, 
        float maxMouseSpeed, float increaser, float fallPosition, float pushDistance,
        Hand leftHand, Hand rightHand)
    {
        playerInput = input;
        playerTrack = path;
        mainFallSpeed = fSpeed;
        playerTransform = player;
        this.debug = debug;
        mouseSpeedToFall = maxMouseSpeed;
        fallIncreaser = increaser;
        currentFallPosition = fallPosition;
        this.leftHand = leftHand;
        this.rightHand = rightHand;
        this.pushDistance = pushDistance;


        Bind();
    }

    void Bind()
    {
        playerInput.Enable();

        playerInput.Map.HeadRotating.performed += CheckHeadRotation;
        playerInput.Map.PutLeftHand.performed += PutLeftHand;
        playerInput.Map.PutLeftHand.canceled += RelaxLeftHand;
        playerInput.Map.PutRightHand.performed += PutRightHand;
        playerInput.Map.PutRightHand.canceled += RelaxRightHand;
        playerInput.Map.Push.performed += PushHand;
    }

    public void Expose()
    {
        playerInput.Disable();

        playerInput.Map.HeadRotating.performed -= CheckHeadRotation;
        playerInput.Map.PutLeftHand.performed -= PutLeftHand;
        playerInput.Map.PutLeftHand.canceled -= RelaxLeftHand;
        playerInput.Map.PutRightHand.performed -= PutRightHand;
        playerInput.Map.PutRightHand.canceled -= RelaxRightHand;
        playerInput.Map.Push.performed -= PushHand;
    }

    void PushHand(InputAction.CallbackContext context)
    {
        if (leftHandLock && leftHand.GetOpportunityToPush())
            currentFallPosition -= pushDistance;
        else if(rightHandLock && rightHand.GetOpportunityToPush())
            currentFallPosition -= pushDistance;
    }

    void PutLeftHand(InputAction.CallbackContext context)
    {
        if(!deathFalling)
        {
            leftHand.ToWorld();
            leftHandLock = true;
            UnityEngine.Debug.Log("”перлась левой");
        }
    }

    void RelaxLeftHand(InputAction.CallbackContext context)
    {
        leftHandLock = false;
        leftHand.ToPlayer();
        UnityEngine.Debug.Log("ќтпустила левую");
    }

    void PutRightHand(InputAction.CallbackContext context)
    {
        if (!deathFalling)
        {
            rightHand.ToWorld();
            rightHandLock = true;
            UnityEngine.Debug.Log("”перлась правой");
        }
    }

    void RelaxRightHand(InputAction.CallbackContext context)
    {
        rightHandLock = false;
        rightHand.ToPlayer();
        UnityEngine.Debug.Log("ќтпустила правую");
    }

    void CheckHeadRotation(InputAction.CallbackContext context)
    {
        float rot = context.ReadValue<Vector2>().x + context.ReadValue<Vector2>().y;
        if (rot > mouseSpeedToFall)
        {
            deathFalling = true;
            leftHand.ToPlayer();
            rightHand.ToPlayer();
        }
        else
        {
            //currentFallSpeed = 0;
        }
    }

    public void Update()
    {
        UpdateFalling();
        leftHand.Update(currentFallPosition);
        rightHand.Update(currentFallPosition);
    }

    private void UpdateFalling()
    {
        if(deathFalling)
        {
            currentFallPosition += mainFallSpeed * Time.deltaTime;
            mainFallSpeed += fallIncreaser;
        }

        playerTrack.Evaluate(math.frac(currentFallPosition), out var pos, out var right, out var up);
        //Vector3 forward = Vector3.Cross(right, up);
        playerTransform.position = pos;
        pos += up;

        debug.position = pos;
        playerTransform.LookAt(debug);

        if(currentFallPosition > 0.999)
        {
            deathFalling = false;
            currentFallPosition = 0.9991f;
        }
    }
}
