using UnityEngine;
using System.Collections;

//require controller
[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {
    public int jumpsAllowed = 1;
    int jumpsPerformed = 0;

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
    public float fallHurtVelocity = 65;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;

    public float moveSpeed = 6;
	public float sprintFactor = 2;
    [HideInInspector]
    public bool isSprinting = false;
	float originalMoveSpeed;
    [HideInInspector]
    public int lastTouchedDirectionX = 1;

    public int lungesAllowed = 1;
    int lungesPerformed = 0;
    public float lungeFactor = 5;
    public float lungeHop = 8;
    public float minimumLunge = 5;
    public float maximumLunge = 15;

    [HideInInspector]
    public Vector3 velocity;
	float velocityXSmoothing;

	//reference to controller
	Controller2D controller;

    [HideInInspector]
	public Vector2 directionalInput;

	public bool wallSliding;
    [HideInInspector]
	public int wallDirX;

    Vector3 ogScale;

	void Start() {
		//initialize controller
		controller = GetComponent<Controller2D> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
		originalMoveSpeed = moveSpeed;

	}

	void Update() {

        CalculateVelocity ();
		HandleWallSliding ();

		controller.Move (velocity * Time.deltaTime, directionalInput);

		if (controller.collisions.above || controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			} else {
				velocity.y = 0;
			}
            if (controller.collisions.below){
                jumpsPerformed = 0;
                lungesPerformed = 0;
            }else{
            }
		}
      

    }

	public void SetDirectionalInput (Vector2 input) {
		directionalInput = input;
        if (input.x > 0){
            lastTouchedDirectionX = 1;
        }else if(input.x < 0){
            lastTouchedDirectionX = -1;
        }

	}

	public void OnJumpInputDown() {
        if (wallSliding && jumpsPerformed < jumpsAllowed) {
            jumpsPerformed ++;
            if (wallDirX == directionalInput.x) {
				velocity.x = -wallDirX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			}
			else if (directionalInput.x == 0) {
				velocity.x = -wallDirX * wallJumpOff.x;
				velocity.y = wallJumpOff.y;
			}
			else {
				velocity.x = -wallDirX * wallLeap.x;
				velocity.y = wallLeap.y;
			}
        }else if (controller.collisions.below) {
            jumpsPerformed = 0;
            lungesPerformed = 0;
            if (controller.collisions.slidingDownMaxSlope && jumpsPerformed < jumpsAllowed) {
				if (directionalInput.x != -Mathf.Sign (controller.collisions.slopeNormal.x)) { // not jumping against max slope
                    jumpsPerformed++;
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
            } else if (jumpsPerformed < jumpsAllowed){
                jumpsPerformed++;
                velocity.y = maxJumpVelocity;
			}
        }else if(jumpsPerformed < jumpsAllowed){
            jumpsPerformed++;
            velocity.y = maxJumpVelocity;
        }
    
	}

	public void OnJumpInputUp() {
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}
		
	public void OnSprintInputDown(){
        if (!controller.collisions.below && lungesPerformed < lungesAllowed){
            lungesPerformed++;
            velocity.y = lungeHop;

            float thisVelocityX = Mathf.Abs(velocity.x);
            if (thisVelocityX < minimumLunge){
                thisVelocityX = minimumLunge;
            }
            if (thisVelocityX > maximumLunge){
                thisVelocityX = maximumLunge;
            }
            velocity.x = thisVelocityX * lungeFactor * lastTouchedDirectionX;
        }
		moveSpeed *= sprintFactor;
        isSprinting = true;
	}

	public void OnSprintInputUp(){
		moveSpeed = originalMoveSpeed;
        isSprinting = false;
	}


    void HandleWallSliding() {
		wallDirX = (controller.collisions.left) ? -1 : 1;
		wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;
            jumpsPerformed = 0;
            lungesPerformed = 0;

			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (directionalInput.x != wallDirX && directionalInput.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				}
				else {
					timeToWallUnstick = wallStickTime;
				}
			}
			else {
				timeToWallUnstick = wallStickTime;
			}

		}

	}

	void CalculateVelocity() {
		float targetVelocityX = directionalInput.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;
	}
}
