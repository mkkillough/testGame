using UnityEngine;
using System.Collections;

//requires box collider
[RequireComponent (typeof (BoxCollider2D))]
public class RaycastController : MonoBehaviour {

	public LayerMask collisionMask;
	
	public const float skinWidth = .015f;
	const float dstBetweenRays = .25f;
	
	//how many rays to fire on each side of the box
	[HideInInspector]
	public int horizontalRayCount;
	[HideInInspector]
	public int verticalRayCount;

	//how far apart the rays will be (set in calculateRaySpacing)
	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	//controller
	[HideInInspector]
	public new BoxCollider2D collider;
	public RaycastOrigins raycastOrigins;

	public virtual void Awake() {
		//initialize box collider
		collider = GetComponent<BoxCollider2D> ();
	}

	public virtual void Start() {
		CalculateRaySpacing ();
	}

	public void UpdateRaycastOrigins() {
		//gets boundary of box collider
		Bounds bounds = collider.bounds;
		//shrinks box by skinWidth on all sides
		bounds.Expand (skinWidth * -2);
		
		//set origins
		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}
	
	public void CalculateRaySpacing() {
		//gets boundary of box collider
		Bounds bounds = collider.bounds;
		//shrinks box by skinWidth on all sides
		bounds.Expand (skinWidth * -2);

		float boundsWidth = bounds.size.x;
		float boundsHeight = bounds.size.y;
		
		horizontalRayCount = Mathf.RoundToInt (boundsHeight / dstBetweenRays);
		verticalRayCount = Mathf.RoundToInt (boundsWidth / dstBetweenRays);
		
		//length of side divided by how many rays there are -1 = ray spacing
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
	
	//struct for storing where to shoot the rays from
	public struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}
