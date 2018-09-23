using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//require controller
public class Wolf_AnimationController : MonoBehaviour {
    public float timeScale = 1f;
    float ogTimeScale;
    //reference to player
    Player player;

    private Animator anim;

    Controller2D controller;
    Vector3 ogScale;
    Vector3 ogPosition;
    Vector3 currentAngles;

    public float jumpAngleSteepness = 1.5f;
    public float jumpAngleLimit = -1;
    public float slopeAngle;
    public int hitBehind = -1;
    float fallHurtVelocity;
    float hurtSpinFactor = .001f;
    float ogHurtSpinFactor;

    public bool climbingSlope;


    public float lastNonZeroSlopeAngle;
    // Use this for initialization
    void Start () {

        //initialize player
        player = GetComponentInParent<Player>();
        anim = GetComponent<Animator>();
        controller = GetComponentInParent<Controller2D>();
        ogScale = transform.localScale;
        ogPosition = transform.localPosition;
        currentAngles = transform.eulerAngles;
        fallHurtVelocity = player.fallHurtVelocity;
        ogHurtSpinFactor = hurtSpinFactor;
        ogTimeScale = timeScale;

    }
    void Update()
    {
        climbingSlope = player.controller.collisions.climbingSlope;
        timeScale = ogTimeScale;
        if (player.controller.collisions.behind){
            hitBehind = 1;
            }else{
            hitBehind = -1;
            }
        slopeAngle = player.controller.collisions.slopeAngle;
        if (slopeAngle > 0){
            lastNonZeroSlopeAngle = slopeAngle;
        }
        Time.timeScale = timeScale;
        if (Mathf.Abs(player.velocity.y) < fallHurtVelocity)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            transform.localPosition = ogPosition;
        }
        currentAngles = transform.eulerAngles;
        if (player.wallSliding)
        {
            anim.SetTrigger("wallSlide");
            if (player.lastTouchedDirectionX == player.wallDirX){

                transform.Rotate(Vector3.forward * 70 * player.lastTouchedDirectionX);
            }



            Vector3 newPosition = ogPosition;
            newPosition.x = .17f * player.lastTouchedDirectionX;
            transform.localPosition = newPosition;
        }
        else if (!controller.collisions.below)
        {
            //Time.timeScale = timeScale * .33f;
            anim.SetTrigger("jump");
            float velocityMultiplier = player.velocity.y * player.lastTouchedDirectionX * jumpAngleSteepness;
            if (jumpAngleLimit != -1 && Mathf.Abs(player.velocity.y) < fallHurtVelocity)
            {
                if (Mathf.Abs(velocityMultiplier) > jumpAngleLimit )
                {
                    velocityMultiplier = jumpAngleLimit * Mathf.Sign(player.velocity.y) * player.lastTouchedDirectionX;
                }
            }

            if ( Mathf.Abs(player.velocity.y) < fallHurtVelocity){
                transform.Rotate(Vector3.forward * velocityMultiplier);
            }
            else{
                transform.Rotate(Vector3.forward, velocityMultiplier * hurtSpinFactor);
                if (hurtSpinFactor < ogHurtSpinFactor * 10){
                    hurtSpinFactor += ogHurtSpinFactor * .1f;
                }

            }


        }
        else
        {
            hurtSpinFactor = ogHurtSpinFactor;
            if (Mathf.Abs(player.velocity.x) > .25f)
            {
                if (player.isSprinting)
                {
                    anim.SetTrigger("sprint");
                }
                else
                {
                    anim.SetTrigger("walk");
                }

            }
            else
            {
                anim.SetTrigger("idle");
            }
            //if (Mathf.Abs(player.controller.collisions.slopeAngle) > 1)
            //{  

            if(hitBehind == 1){
                transform.Rotate(Vector3.back * lastNonZeroSlopeAngle * player.lastTouchedDirectionX);
                Vector3 newPosition = ogPosition;
                newPosition.x = -.33f * player.lastTouchedDirectionX;
                newPosition.y = -.05f;
                transform.localPosition = newPosition;
            }
            if (climbingSlope){
                transform.Rotate(Vector3.forward * lastNonZeroSlopeAngle * player.lastTouchedDirectionX);
                Vector3 newPosition = ogPosition;
                newPosition.x = .33f * player.lastTouchedDirectionX;
                newPosition.y = -.05f;
                transform.localPosition = newPosition;
            }
                
            //}

        }

        if (player.lastTouchedDirectionX > 0)
        {
            if (transform.localScale != ogScale)
            {
                transform.localScale = ogScale;
            }
        }
        else if (player.lastTouchedDirectionX < 0)
        {
            Vector3 newScale = ogScale;
            newScale.x *= -1;
            if (transform.localScale != newScale)
            {
                transform.localScale = newScale;
            }
        }
        //transform.Rotate(Vector3.forward, Time.deltaTime);
       
    }

}
//instant
//transform.localRotation = Quaternion.Euler(0f, 0f, 70f*player.lastTouchedDirectionX);


//smooth but weird
//transform.localRotation = Quaternion.Lerp(Quaternion.EulerAngles(0f,0f,0f), Quaternion.EulerAngles(0f, 0f, 70f * player.lastTouchedDirectionX), 1f);

//Vector3 targetAngle = new Vector3(0f, 0f, 70f*player.lastTouchedDirectionX);
//Vector3 currentAngle = currentAngle = transform.eulerAngles;


//currentAngle = new Vector3(
// Mathf.LerpAngle(currentAngle.x, targetAngle.x, 1f),
// Mathf.LerpAngle(currentAngle.y, targetAngle.y, 1f),
// Mathf.LerpAngle(currentAngle.z, targetAngle.z, 1f));

//transform.eulerAngles = currentAngle;

//rotationFactor = velocity.y /rotationFactorMultiplier;
//if (rotationFactor > rotationLimit){
//    rotationFactor = rotationLimit;
//}
//if (rotationFactor < rotationLimit * -1){
//    rotationFactor = rotationLimit * -1;
//}
//Mathf.Clamp(rotationFactor, -45, 45);
//transform.Rotate(0, 0, -rotationFactor, Space.Self);
//transform.localRotation.z = transform.localRotation.z * velocity.y;
// Update is called once per frame
//void FixedUpdate () {
//    currentAngles = transform.eulerAngles;
//    transform.localRotation = Quaternion.Euler(0, 0, 0);
//    transform.localPosition = ogPosition;
//    if (player.wallSliding){   
//        anim.SetTrigger("wallSlide");

//        if (Mathf.Abs(currentAngles.z)<70f){
//            transform.Rotate(Vector3.forward * 70 * player.lastTouchedDirectionX);
//            //transform.Rotate(Vector3.forward, Time.deltaTime);
//        }

//        Vector3 newPosition = ogPosition;
//        newPosition.x = .17f * player.lastTouchedDirectionX;
//        transform.localPosition = newPosition;
//        // Rotate the object around its local z axis at 1 degree per second
//        //transform.Rotate(Vector3.forward, Time.deltaTime);
//    }else if (!controller.collisions.below){
//        anim.SetTrigger("jump");
//        float velocityMultiplier = player.velocity.y * player.lastTouchedDirectionX * jumpAngleSteepness;
//        if (jumpAngleLimit != -1){
//            if (Mathf.Abs(velocityMultiplier) > jumpAngleLimit )
//            {
//                velocityMultiplier = jumpAngleLimit * Mathf.Sign(player.velocity.y) * player.lastTouchedDirectionX;
//            }
//        }

//        transform.Rotate(Vector3.forward * velocityMultiplier);

//    }else{
//        if (Mathf.Abs(player.velocity.x)>.25f){
//            if (player.isSprinting){
//                anim.SetTrigger("sprint");
//            }else{
//                anim.SetTrigger("walk");
//            }
//        } else{
//            anim.SetTrigger("idle");
//        }

//    }

//    //if (!player.wallSliding){
//        if (player.directionalInput.x > 0)
//        {
//            if (transform.localScale != ogScale)
//            {
//                transform.localScale = ogScale;
//            }
//        }
//        else if (player.directionalInput.x < 0)
//        {
//            Vector3 newScale = ogScale;
//            newScale.x *= -1;
//            if (transform.localScale != newScale)
//            {
//                transform.localScale = newScale;
//            }
//        }

//    }
////}