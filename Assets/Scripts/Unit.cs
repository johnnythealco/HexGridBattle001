using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{

	public float movement = 3;
	public float moveTime = 0.1f;
	public int health = 100;
	public int damage = 50;
	public float attackRange = 1;
	public Animator animator; 
//	public Affilation type;

	
	private float inverseMoveTime;

	protected virtual void Start ()
	{
		inverseMoveTime = 1 / moveTime;
		animator = GetComponent<Animator> ();

	}

	public void SetPosition (Vector3 newPosition)
	{
		transform.position = newPosition;
	}

	public Vector3 GetPosition ()
	{
		return transform.position;
	}

	public void Move (List<Vector3> waypoints)
	{

		StartCoroutine (SmoothMovement (waypoints));
	}


	protected IEnumerator SmoothMovement (List<Vector3> waypoints)
	{
		animator.SetBool ("Walking", true);
		foreach (var waypoint in waypoints)
		{
			float sqrRemainingDistance = (transform.position - waypoint).sqrMagnitude; //sqrMagnitude is cheaper on the CPU than Magnitude

			while (sqrRemainingDistance > float.Epsilon) //Epsion is the smallest value that a float can have different from zero.
			{
				Vector3 newPosition = Vector3.MoveTowards (transform.position, waypoint, inverseMoveTime * Time.deltaTime);
				transform.position = newPosition;
				sqrRemainingDistance = (transform.position - waypoint).sqrMagnitude;

				yield return null;

			}
		}
		animator.SetBool ("Walking", false);

	}

	public void AttackAnnimation()
	{
		animator.SetTrigger("Striking");
	}

//	public enum Affilation {Enemy, Player} ;



}


