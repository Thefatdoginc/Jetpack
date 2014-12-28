using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour {
	public float jetpackForce=75.0f;
	public float forwardMovementSpeed=3.0f;
	public Transform groundCheckTransform;
	private bool grounded;
	public LayerMask groundCheckLayerMask;
	public ParticleSystem jetpack;

	private bool dead=false;

	private uint coins=0;

	public Texture2D coinIconTexture;


	
	public GUIStyle restartButtonStyle;

	public AudioClip coinCollectSound;
	public AudioSource jetpackAudio;
	public AudioSource footstepsAudio;

	public ParallaxScroll parallax;

	Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate()
	{
		bool jetpackActive = Input.GetButton ("Fire1");

		jetpackActive = jetpackActive && !dead;

		if (jetpackActive) 
		{
			rigidbody2D.AddForce (new Vector2 (0, jetpackForce));
		}
		if (!dead) 
		{
		Vector2 newVelocity = rigidbody2D.velocity;
		newVelocity.x = forwardMovementSpeed;
		rigidbody2D.velocity = newVelocity;
		}

		updateGroundStatus ();
		AdjustJetpack (jetpackActive);

		AdjustFootsepsAndJetpackSound (jetpackActive);

		parallax.offset = transform.position.x;
	}

	void updateGroundStatus()
	{
		grounded = Physics2D.OverlapCircle (groundCheckTransform.position, 0.1f, groundCheckLayerMask);

		animator.SetBool ("grounded", grounded);
	}


	void AdjustJetpack(bool jetpackActive)
	{
		jetpack.enableEmission = ! grounded;
		jetpack.emissionRate = jetpackActive ? 300.0f : 75.0f;
	}

	void OnTriggerEnter2D(Collider2D collider)
	{

		if (collider.gameObject.CompareTag ("Coins")) 

			CollectCoin (collider);
			

		else
		HitByLaser (collider);
	}

	void HitByLaser(Collider2D laserCollider)
	{
		if (!dead) 
		{
			laserCollider.gameObject.audio.Play();	
		}

		dead = true;
		animator.SetBool ("dead", true);


	}

	void CollectCoin(Collider2D  coinCollider)

	{
		coins++;
		Destroy (coinCollider.gameObject);
		AudioSource.PlayClipAtPoint (coinCollectSound, transform.position);
	}


	void DisplayCoinsCount()
	{
		Rect coinIconRect = new Rect (10, 10, 32, 32);
		GUI.DrawTexture (coinIconRect, coinIconTexture);

		GUIStyle style = new GUIStyle ();
		style.fontSize = 30;
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.yellow;

		Rect labelRect = new Rect (coinIconRect.xMax, coinIconRect.y, 60, 32);

		GUI.Label (labelRect, coins.ToString (), style);
	}

	void OnGUI()
	{
		DisplayCoinsCount ();
		DisplayRestartButton ();
	}


	void DisplayRestartButton()
	{

		


		if (dead && grounded) 
		{
			Rect buttonRect=new Rect(Screen.width*0.35f,Screen.height*0.45f, Screen.width*0.30f,Screen.height*0.1f);

			if(GUI.Button(buttonRect, "Restart"))
		
			{
				Application.LoadLevel(Application.loadedLevel);
			}
		}
	}

	void AdjustFootsepsAndJetpackSound(bool jetpackActive)
	{
		footstepsAudio.enabled = !dead && grounded;

		jetpackAudio.enabled = !dead && !grounded;
	jetpackAudio.volume = jetpackActive ? 1.0f : 0.5f;
	}

}
