using UnityEngine;

[System.Serializable]
struct Check
{
	public Vector2 offset;
	public Vector2 size;

	public Check(Vector2 offset, Vector2 size)
	{
		this.offset = offset;
		this.size = size;
	}
}

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyWalkingMovement : MonoBehaviour
{
	Rigidbody2D rb;

	[SerializeField] Transform graphicsTransform;

	[Space(5)]
	public float speed = 200f;
	float xDir = 1f;
	public float stopWaitTime = 1f;
	float _stopWaitTimer = 0;

	[Header("Checks")]
	[SerializeField] LayerMask groundLayer;
	[SerializeField] bool checkForGround = true;
	[SerializeField] Check groundAheadCheck = new Check(new Vector2(1f, -1f), new Vector2(0.25f, 0.5f));
	[SerializeField] Check wallAheadCheck = new Check(new Vector2(0.65f, 0.1f), new Vector2(0.25f, 1.75f));

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		_stopWaitTimer -= Time.fixedDeltaTime;
		if (_stopWaitTimer < 0)
		{
			if ((!GroundCheck() && checkForGround) || WallCheck())
			{
				_stopWaitTimer = stopWaitTime;
				xDir = -xDir;
				rb.velocity = Vector2.zero;
				return;
			}
			Move();
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireCube(
			(Vector2)transform.position + groundAheadCheck.offset * new Vector2(xDir, Mathf.Sign(-Physics2D.gravity.y)),
			groundAheadCheck.size);
		Gizmos.DrawWireCube(
			(Vector2)transform.position + wallAheadCheck.offset * new Vector2(xDir, Mathf.Sign(-Physics2D.gravity.y)),
			wallAheadCheck.size);
	}



	void Move()
	{
		rb.velocity = new Vector2(xDir * speed * Time.fixedDeltaTime, rb.velocity.y);
		graphicsTransform.lossyScale.Set(graphicsTransform.lossyScale.x, graphicsTransform.lossyScale.y * xDir, 1f);
	}

	bool GroundCheck()
	{
		return Physics2D.OverlapBox(
			(Vector2)transform.position + groundAheadCheck.offset * new Vector2(xDir, Mathf.Sign(-Physics2D.gravity.y)),
			groundAheadCheck.size, 0, groundLayer) != null;
	}

	bool WallCheck()
	{
		return Physics2D.OverlapBox(
			(Vector2)transform.position + wallAheadCheck.offset * new Vector2(xDir, Mathf.Sign(-Physics2D.gravity.y)),
			wallAheadCheck.size, 0, groundLayer) != null;
	}
}
