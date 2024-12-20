using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class FPSController : MonoBehaviour
{

	private Transform firstPerson_View;
	private Transform firstPerson_Camera;
	private Vector3 firstPerson_View_Rotation = Vector3.zero;

	public float walkSpeed = 6.75f;
	public float runSpeed = 10f;
	public float crouchSpeed = 4f;
	public float jumpSpeed = 8f;
	public float gravity = 20f;

	private float speed;
	private bool is_Moving, is_Grounded, is_Crouching;

	
	private float inputX, inputY;
	private float inputX_Set, inputY_Set;
	private float inputModifyFactor;

	private bool limitDiagonalSpeed = true;
	private float antiBumpFactor = 0.75f; // smooth movement of the character if we bump on the wall

	private CharacterController charController;
	private Vector3 moveDirection = Vector3.zero;

	public LayerMask groundLayer;
	private float rayDistance;
	private float default_ControllerHeight;
	private Vector3 default_CamPos;
	private float camHeight;

	private FPSPlayerAnimations playerAnimation;

	/*[SerializeField]
	private WeaponManager weapon_Manager;
	private FPSWeapon current_Weapon;*/

	private float fireRate = 15f;
	private float nextTimeToFire = 0f;

	[SerializeField]
	private WeaponManager handsWeapon_Manager;
	private FPSHandsWeapon current_Hands_Weapon;


	public GameObject playerHolder, weaponsHolder;
	public GameObject[] weapons_FPS;
	private Camera mainCam;
	public FPSMouseLook[] mouseLook;


	void Start()
	{
		firstPerson_View = transform.Find("FPS View").transform;
		charController = GetComponent<CharacterController>();
		speed = walkSpeed;
		is_Moving = false;

		rayDistance = charController.height * 0.5f + charController.radius;
		default_ControllerHeight = charController.height;
		default_CamPos = firstPerson_View.localPosition;

		playerAnimation = GetComponent<FPSPlayerAnimations>();

        handsWeapon_Manager.weapons[0].SetActive(true);
		current_Hands_Weapon = handsWeapon_Manager.weapons[0].GetComponent<FPSHandsWeapon>();
	}


		void Update()
	    {

			PlayerMovement();
			SelectWeapon();
		}

		void PlayerMovement()
		{
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
			{
				if (Input.GetKey(KeyCode.W))
				{
					inputY_Set = 1f;
				}
				else
				{
					inputY_Set = -1f;
				}
			}
			else
			{
				inputY_Set = 0f;
			}

			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
			{
				if (Input.GetKey(KeyCode.A))
				{
					inputX_Set = -1f;
				}
				else
				{
					inputX_Set = 1f;
				}
			}
			else
			{
				inputX_Set = 0f;
			}

			inputY = Mathf.Lerp(inputY, inputY_Set, Time.deltaTime * 19f); 
			inputX = Mathf.Lerp(inputX, inputX_Set, Time.deltaTime * 19f);

			inputModifyFactor = Mathf.Lerp(inputModifyFactor,
				(inputY_Set != 0 && inputX_Set != 0 && limitDiagonalSpeed) ? 0.75f : 1.0f,
				Time.deltaTime * 19f);

			firstPerson_View_Rotation = Vector3.Lerp(firstPerson_View_Rotation,
				Vector3.zero, Time.deltaTime * 5f);
			firstPerson_View.localEulerAngles = firstPerson_View_Rotation; 

			if (is_Grounded)
			{

				
				PlayerCrouchingAndSprinting();

				moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor,
					inputY * inputModifyFactor);

				moveDirection = transform.TransformDirection(moveDirection) * speed;

				
				PlayerJump();
			}

			moveDirection.y -= gravity * Time.deltaTime;

			is_Grounded = (charController.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;

			is_Moving = charController.velocity.magnitude > 0.15f;

			HandleAnimations();

		}

		void PlayerCrouchingAndSprinting()
		{
			if (Input.GetKeyDown(KeyCode.C))
			{

				if (!is_Crouching)
				{
					is_Crouching = true;
				}
				else
				{
					if (CanGetUp())
					{
						is_Crouching = false;
					}
				}

				StopCoroutine(MoveCameraCrouch());
				StartCoroutine(MoveCameraCrouch());
			}

			if (is_Crouching)
			{
				speed = crouchSpeed;
			}
			else
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					speed = runSpeed;
				}
				else
				{
					speed = walkSpeed;
				}
			}

			//playerAnimation.PlayerCrouch(is_Crouching);

		}

		bool CanGetUp()
		{
			Ray groundRay = new Ray(transform.position, transform.up);
			RaycastHit groundHit;

			if (Physics.SphereCast(groundRay, charController.radius + 0.05f,
				out groundHit, rayDistance, groundLayer))
			{

				if (Vector3.Distance(transform.position, groundHit.point) < 2.3f)
				{
					return false;
				}
			}

			return true;
		}

		IEnumerator MoveCameraCrouch()
		{
			charController.height = is_Crouching ? default_ControllerHeight / 1.5f : default_ControllerHeight;
			charController.center = new Vector3(0f, charController.height / 2f, 0f);

			camHeight = is_Crouching ? default_CamPos.y / 1.5f : default_CamPos.y;

			while (Mathf.Abs(camHeight - firstPerson_View.localPosition.y) > 0.01f)
			{

				firstPerson_View.localPosition = Vector3.Lerp(firstPerson_View.localPosition,
					new Vector3(default_CamPos.x, camHeight, default_CamPos.z),
					Time.deltaTime * 11f);

				yield return null;
			}
		}

		void PlayerJump()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{

				if (is_Crouching)
				{

					if (CanGetUp())
					{
						is_Crouching = false;

						//playerAnimation.PlayerCrouch(is_Crouching);

						StopCoroutine(MoveCameraCrouch());
						StartCoroutine(MoveCameraCrouch());
					}

				}
				else
				{
					moveDirection.y = jumpSpeed;
				}
			}
		}

		void HandleAnimations()
		{
			/*playerAnimation.Movement(charController.velocity.magnitude);
			playerAnimation.PlayerJump(charController.velocity.y);

			if (is_Crouching && charController.velocity.magnitude > 0f)
			{
				playerAnimation.PlayerCrouchWalk(charController.velocity.magnitude);
			}

			// SHOOTING
			*/if (Input.GetMouseButtonDown(0) && Time.time > nextTimeToFire)
			{
				nextTimeToFire = Time.time + 1f / fireRate;

				/*if (is_Crouching)
				{
					playerAnimation.Shoot(false);
				}
				else
				{
					playerAnimation.Shoot(true);
				}*/

			     //current_Weapon.Shoot()
				 current_Hands_Weapon.Shoot();
		    }

			/*if (Input.GetKeyDown(KeyCode.R))
			{
				playerAnimation.ReloadGun();
				current_Hands_Weapon.Reload();
			}*/

		}

		void SelectWeapon()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{

				if (!handsWeapon_Manager.weapons[0].activeInHierarchy)
				{
					for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++)
					{
						handsWeapon_Manager.weapons[i].SetActive(false);
					}

					current_Hands_Weapon = null;

					handsWeapon_Manager.weapons[0].SetActive(true);
					current_Hands_Weapon = handsWeapon_Manager.weapons[0].GetComponent<FPSHandsWeapon>();
			     }
			}

			if (Input.GetKeyDown(KeyCode.Alpha2))
			{

				if (!handsWeapon_Manager.weapons[1].activeInHierarchy)
				{
					for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++)
					{
						handsWeapon_Manager.weapons[i].SetActive(false);
					}

					current_Hands_Weapon = null;

					handsWeapon_Manager.weapons[1].SetActive(true);
					current_Hands_Weapon = handsWeapon_Manager.weapons[1].GetComponent<FPSHandsWeapon>();
			    }
			}

			if (Input.GetKeyDown(KeyCode.Alpha3))
			{

				if (!handsWeapon_Manager.weapons[2].activeInHierarchy)
				{
					for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++)
					{
						handsWeapon_Manager.weapons[i].SetActive(false);
					}

					current_Hands_Weapon = null;

					handsWeapon_Manager.weapons[2].SetActive(true);
					current_Hands_Weapon = handsWeapon_Manager.weapons[2].GetComponent<FPSHandsWeapon>();
			     }
			}

		}


}
// class
