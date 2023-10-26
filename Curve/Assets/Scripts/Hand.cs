using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Hand
{
    private Transform point, player, bone;
    private float distance, offsetX;

    SplinePath playerTrack;
    Vector3 pointPosition;

    public Hand(Transform point, Transform player, Transform bone, float distance, SplinePath track)
    {
        this.point = point;
        this.player = player;
        this.bone = bone;
        this.distance = distance;
        playerTrack = track;

        offsetX = point.position.x - player.position.x;
    }


    public void Update(float trackPosition)
    {
        if(point.parent == null)
        {
            playerTrack.Evaluate(math.frac(trackPosition - 0.1f), out var pos, out var fwd, out var up);
            pointPosition = pos;
            pointPosition.x += offsetX;
            point.position = pointPosition;
        }
    }

    public bool GetOpportunityToPush()
    {
        bool what = false;
        if (Vector3.Distance(bone.position, point.position) < distance)
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
    }
}
