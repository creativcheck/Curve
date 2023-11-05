using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Hand
{
    private Transform point, player, bone;
    private float distancePointBone, offsetX, distanceToPlayer;

    SplinePath playerTrack;
    Vector3 pointPosition;
    float _trackPosition;

    public Hand(Transform point, Transform player, Transform bone, float distance, SplinePath track, float distanceToPlayer)
    {
        this.point = point;
        this.player = player;
        this.bone = bone;
        this.distancePointBone = distance;
        playerTrack = track;
        this.distanceToPlayer = distanceToPlayer;

        offsetX = point.position.x - player.position.x;
    }


    public void Update(float trackPosition)
    {
        /*if(point.parent == null)
        {
            playerTrack.Evaluate(math.frac(trackPosition - 0.1f), out var pos, out var fwd, out var up);
            pointPosition = pos;
            pointPosition.x += offsetX;
            point.position = pointPosition;
        }*/
        _trackPosition = trackPosition;
    }

    public bool GetOpportunityToPush()
    {
        bool what = false;
        if (Vector3.Distance(bone.position, point.position) < distancePointBone)
        {
            what = true;
        }
        
        return what;
    }

    public void ToWorld()
    {
        point.parent = null;
        
    }

    public void ToPlayer()
    {
        point.parent = player;
        //if (Vector3.Distance(bone.position, point.position) > distance)
        //{
            playerTrack.Evaluate(math.frac(Mathf.Clamp(_trackPosition - distanceToPlayer, 0, 1)), out var pos, out var fwd, out var up);
            pointPosition = pos;
            pointPosition.x += offsetX;
            point.position = pointPosition;
        //}
    }
}
