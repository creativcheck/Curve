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
    PostProcessManager postProcessManager;

    float currentFallSpeed, currentFallPosition, minHeight, maxHeight;
    float currentFatigueLeft, currentFatigueRight, fatigueByPush, fatigueByTime, fatigueLimit, fatigueRelax;
    bool deathFalling, leftHandLock, rightHandLock;

    public PlayerActions(PostProcessManager posProcessMng, PlayerInput input, SplinePath path, float fSpeed, Transform player, Transform debug, 
        float maxMouseSpeed, float increaser, float fallPosition, float pushDistance, float maxHeight, float minHeight,
        Hand leftHand, Hand rightHand,
        float fatiguePush, float fatigueTime, float limit, float relax)
    {
        this.postProcessManager = posProcessMng;
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
        this.maxHeight = maxHeight;
        this.minHeight = minHeight;
        fatigueByPush = fatiguePush;
        fatigueByTime = fatigueTime;
        fatigueLimit = limit;
        fatigueRelax = relax;

        //leftHand.ToPlayer();
        //rightHand.ToPlayer();

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
            Pushing();
        else if(rightHandLock && rightHand.GetOpportunityToPush())
            Pushing();
    }

    void PutLeftHand(InputAction.CallbackContext context)
    {
        if(!deathFalling)
        {
            leftHand.ToWorld();
            leftHandLock = true;
            UnityEngine.Debug.Log("Уперлась левой");
        }
    }

    void RelaxLeftHand(InputAction.CallbackContext context)
    {
        leftHandLock = false;
        leftHand.ToPlayer();
        UnityEngine.Debug.Log("Отпустила левую");
    }

    void PutRightHand(InputAction.CallbackContext context)
    {
        if (!deathFalling)
        {
            rightHand.ToWorld();
            rightHandLock = true;
            UnityEngine.Debug.Log("Уперлась правой");
        }
    }

    void RelaxRightHand(InputAction.CallbackContext context)
    {
        rightHandLock = false;
        rightHand.ToPlayer();
        UnityEngine.Debug.Log("Отпустила правую");
    }

    void CheckHeadRotation(InputAction.CallbackContext context)
    {
        float rot = context.ReadValue<Vector2>().x + context.ReadValue<Vector2>().y;
        if (rot > mouseSpeedToFall)
        {
            StartFalling();
        }
    }

    void Pushing()
    {
        currentFallPosition -= pushDistance;
        if (leftHandLock)
        {
            currentFatigueLeft += fatigueByPush;
        }
        if (rightHandLock)
        {
            currentFatigueRight += fatigueByPush;
        }
    }

    void StartFalling()
    {
        deathFalling = true;
        leftHand.ToPlayer();
        rightHand.ToPlayer();
        Expose();
    }

    public void Update()
    {
        UpdateFalling();
        UpdateFatigue();
        leftHand.Update(currentFallPosition);
        rightHand.Update(currentFallPosition);
    }

    void UpdateFatigue()
    {
        if(leftHandLock)
        {
            currentFatigueLeft += Time.deltaTime * fatigueByTime;

            if(currentFatigueLeft > fatigueLimit)
            {
                StartFalling();
            }
        }
        else if (currentFatigueLeft > 0)
        {
            currentFatigueLeft -= Time.deltaTime * fatigueRelax;
        }

        if (rightHandLock)
        {
            currentFatigueRight += Time.deltaTime * fatigueByTime;

            if (currentFatigueRight > fatigueLimit)
            {
                StartFalling();
            }
        }
        else if (currentFatigueRight > 0)
        {
            currentFatigueRight -= Time.deltaTime * fatigueRelax;
        }

        // обновляем эффекты в зависимости от усталости
        if (currentFatigueLeft > currentFatigueRight)
        {
            postProcessManager.SetMultiplier(currentFatigueLeft / fatigueLimit);
        }
        else
        {
            postProcessManager.SetMultiplier(currentFatigueRight / fatigueLimit);
        }
    }

    private void UpdateFalling()
    {
        if(currentFallPosition < maxHeight /*|| currentFallPosition > minHeight*/)
        {
            if(!deathFalling)
                StartFalling();
        }

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

        if(currentFallPosition > 0.98)
        {
            deathFalling = false;
        }
    }
}
