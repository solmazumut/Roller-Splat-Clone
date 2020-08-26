using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BallControl : MonoBehaviour
{

    public Rigidbody rb;
    public float speed = 15;

    private bool isTraveling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;

    private Vector2 swipePositionLastFrame;
    private Vector2 swipePositionCurrentFrame;
    private Vector2 currentSwipe;

    public int minSwipeRecognition = 1000;

    private Color solveColor;

    private void Start()
    {
        solveColor = Random.ColorHSV(0.5f, 1);
        GetComponent<MeshRenderer>().material.color = solveColor;
    }

    private void FixedUpdate()
    {
        if (isTraveling)
        {
            rb.velocity = speed * travelDirection;
        }

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position - (Vector3.up/2), 0.08f);
        Debug.Log("uzunluk: "+hitColliders.Length + "vektör:" + (Vector3.up)+ "\n");
        int i = 0;
        while(i < hitColliders.Length)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>();
            Debug.Log("ilk if\n");
            if (ground && !ground.isColored)
            {
                Debug.Log("ikinci if\n");
                ground.ChangeColor(solveColor);
            }
            i++;
        }

        if(nextCollisionPosition != Vector3.zero)
        {
            if(Vector3.Distance(transform.position, nextCollisionPosition) < 1)
            {
                isTraveling = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;
            }
        }
             
        if(isTraveling)
        {
            return;
        }

        if(Input.GetMouseButton(0))
        {
            swipePositionCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if(swipePositionLastFrame != Vector2.zero)
            {
                currentSwipe = swipePositionCurrentFrame - swipePositionLastFrame;

                if(currentSwipe.sqrMagnitude < minSwipeRecognition)
                {
                    return;
                }

                currentSwipe.Normalize();

                //up-down
                if(currentSwipe.x > -0.5f && currentSwipe.x < 0.5)
                {
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
                }

                //left - right
                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5)
                {
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }
            }

            swipePositionLastFrame = swipePositionCurrentFrame;
        }
    
        if(Input.GetMouseButtonUp(0))
        {
            swipePositionLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;

        }
    }

    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;

        RaycastHit hit;
        if(Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }

        isTraveling = true;
    }
}
