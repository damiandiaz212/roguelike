using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	public float speed;
	public float swingDistance;
	public float attackCooldown;
	public int health;
	public int strength;

	Vector2 vel;
	Rigidbody2D rb;
	Animator anim;
	SpriteRenderer ren;

	//raycast properties
	Vector2 dir;
	Vector3 pos;

	float x;
	float y;

	bool canAttack = true;
	float timer;


	void Start ()
	{
		rb = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		ren = GetComponent<SpriteRenderer> ();
	}

	void FixedUpdate ()
	{

		x = Input.GetAxis ("Horizontal");
		y = Input.GetAxis ("Vertical");

		vel = new Vector2 (x, y);

		rb.MovePosition (rb.position + vel * Time.deltaTime * speed);

	}

	void Update ()
	{
		bool moving = false;

		if (x != 0 || y != 0) {
			anim.SetBool ("Walk", true);
			moving = true;
		} else {
			anim.SetBool ("Walk", false);
			moving = false;
		}
			
		if ((y > 0) && moving) {
			dir = Vector2.up;
			pos = new Vector3 (transform.position.x, transform.position.y+ 0.3f, transform.position.z);

		} else if ((y < 0) && moving) {
			dir = Vector2.down;
			pos = new Vector3 (transform.position.x, transform.position.y + 0.3f, transform.position.z);
		}
		if (x > 0) {
			ren.flipX = false;
			dir = Vector2.right;
			pos = new Vector3 (transform.position.x, transform.position.y + 0.5f, transform.position.z);
		}
		if (x < 0) {
			ren.flipX = true;
			dir = Vector2.left;
			pos = new Vector3 (transform.position.x, transform.position.y + 0.5f, transform.position.z);
		}

		if (!moving) {

			if (ren.flipX == false)
				dir = Vector2.right;
			else
				dir = Vector2.left;

			pos = new Vector3 (transform.position.x, transform.position.y + 0.5f, transform.position.z);
		}

		if (timer >= attackCooldown) {
			canAttack = true;
		} else {
			timer += Time.deltaTime;
		}

		if (Input.GetKeyDown (KeyCode.P) && canAttack) {
			canAttack = false;
			timer = 0;
			anim.SetTrigger ("Attack");
            Attack();
		}


		Debug.DrawRay (pos, dir * swingDistance);
	}

	void Attack ()
	{

		RaycastHit2D hit;

		hit = Physics2D.Raycast (pos, dir, swingDistance);
	
		if (hit.transform != null) {
			if (hit.collider.tag == "wall") {
				hit.transform.SendMessage ("DamageWall", strength);
			} else if (hit.collider.tag == "Enemy") {
				hit.transform.SendMessage ("TakeDamage", strength);
			}
		}

	}

	public int Health {
		get {
			return health;
		}
		set {
			health = value;
		}
	}
}