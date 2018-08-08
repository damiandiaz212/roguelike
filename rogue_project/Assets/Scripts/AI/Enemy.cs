using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	Seeker seeker;
	AIPath path;
	BoxCollider2D col;
	SpriteRenderer ren;
	Color regColor;
	Animator anim;

	bool alive = true;
	bool canAttack = true;
	bool readyToAttack = false;

	public float health;

	void Awake(){

		ren = GetComponent<SpriteRenderer> ();
		seeker = GetComponent<Seeker> ();
		path = GetComponent<AIPath> ();
		col = GetComponent<BoxCollider2D> ();
		anim = GetComponent<Animator> ();

		path.target = GameObject.FindGameObjectWithTag ("node").transform;
		regColor = ren.material.color;
	}
	void Update(){

		if (alive) {

			//Calculate distance to player
			float distance = Vector2.Distance (new Vector3 (transform.position.x, transform.position.y+ 0.15f, 0), path.target.transform.position);

			//If enemy hasn't reached player, move
			if ((distance < 0.8f || distance > 10)) {
				path.canMove = false;
			}
			else
				path.canMove = true;

			//If enemy has reached player, attack
			if (distance < 1) 
				readyToAttack = true;
			else
				readyToAttack = false;

			handleAttack ();

			//Handle orientation
			if (path.target.transform.position.x > transform.position.x)
				ren.flipX = true;
			else
				ren.flipX = false;

			//Walk animation
			anim.SetBool ("Walk", path.canMove);
		}

		//If enemy dies
		if(health <= 0){
			seeker.enabled = false;
			path.enabled = false;
			col.enabled = false;
			alive = false;
            Destroy(gameObject);
        }
	}

	public void TakeDamage(int damage){
		if (alive) {
			health -= damage;
			StartCoroutine (hit ());
		}

	}

	private void handleAttack(){
		if (canAttack && readyToAttack) {
			StartCoroutine (attackCooldown (2));
			anim.SetTrigger ("Attack");
		}
	}
		

	IEnumerator hit(){
		ren.material.color = Color.red;
		yield return new WaitForSeconds(0.2f);
		ren.material.color = regColor;
	}

	IEnumerator Death(){
		anim.SetBool("Death",true);
		yield return new WaitForSeconds(4);
		Destroy (gameObject);
	}
	IEnumerator attackCooldown(float time){
		canAttack = false;
		yield return new WaitForSeconds(time);
		canAttack = true;
	}
}
