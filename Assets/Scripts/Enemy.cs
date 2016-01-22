using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{

	public float movement = 6;
	public float moveTime = 0.001f;
	public int health = 100;
	public int damage = 50;


	private float inverseMoveTime;

	protected virtual void Start ()
	{
		inverseMoveTime = 1 / moveTime;
	}

	public void SetPosition (Vector3 newPosition)
	{
		transform.position = newPosition;
	}

	public void Move (Vector3 newPosition)
	{
		StartCoroutine (SmoothMovement (newPosition));
	}

	public Vector3 GetPosition ()
	{
		return transform.position;
	}

	protected IEnumerator SmoothMovement (Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude; //sqrMagnitude is cheaper on the CPU than Magnitude

		while (sqrRemainingDistance > float.Epsilon) //Epsion is the smallest value that a float can have different from zero.
		{
			Vector3 newPosition = Vector3.MoveTowards (transform.position, end, inverseMoveTime * Time.deltaTime);
			transform.position = newPosition;
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;

			yield return null;

		}

	}

	public void TakeDamage (int damage)
	{
		health = health - damage;
		if (health <= 0)
		{
			Destroy (this.gameObject);
		}
	}
}
