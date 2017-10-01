/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;

namespace VOX{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerMovement : Photon.MonoBehaviour{

		public float inputLerpSpeed = 10f;
		public bool airControl = true;

		[Header("Speeds")]
		public float walkingSpeed = 6f;
		public float runningSpeed = 11f;
		public float crouchingSpeed = 3f;
		public float aimingSpeed = 4f;
		public float crouchAimingSpeed = 2f;

		[Header("Physics")]
		public float jumpSpeed = 8f;
		public float gravity = 20f;
		public float fallingDamageThreshold = 10f;
		
		[Header("Crouching")]
		public float crouchCamLerpSpeed = 10f;
		public float normalHeight = 1.85f;
		public float normalCameraHeight = 1.55f;
		public float crouchingHeight = 1.35f;
		public float crouchingCameraHeight = 1.15f;

		[Header("Others")]
		public float antiBumpFactor = 0.75f;
		public Headbob headbob;
		public ViewmodelSway sway;

		[Header("Headbob Templates")]
		public HeadbobTemplate walkingHeadbob;
		public HeadbobTemplate runningHeadbob;
		public HeadbobTemplate crouchingHeadbob;
		public HeadbobTemplate aimingHeadbob;
		public HeadbobTemplate crouchAimingHeadbob;

		[Header("Sway Templates")]
		public SwayTemplate walkingSway;
		public SwayTemplate aimingSway;

		Player player;

		public bool grounded{
			get{ return controller.isGrounded; }
		}

		Vector3 moveDirection = Vector3.zero;
		float speed;

		float fallStartLevel;

		public CharacterController controller{
			get{
				if(_controller == null)
					_controller = GetComponent<CharacterController>();

				return _controller;
			}
		}

		CharacterController _controller;
		
		float inputZ;
		float inputX;

		bool lastCrouching;
		bool requestJump;

		void Awake(){
			player = GetComponent<Player>();
			speed = walkingSpeed;
		}

		void Update(){
			if(photonView.isMine){
				headbob.animate = Moving();

				bool crouching = Crouching();
				Transform cam = player.mouseLook.mouseLookRoot;
				cam.localPosition = Vector3.Lerp(cam.localPosition, Vector3.up * (crouching ? crouchingCameraHeight : normalCameraHeight), crouchCamLerpSpeed * Time.deltaTime);

				if(crouching != lastCrouching){
					lastCrouching = crouching;
				
					controller.height = crouching ? crouchingHeight : normalHeight;
					controller.center = Vector3.up * controller.height / 2f;
				}

				if(Input.GetKeyDown(KeyCode.Space) && !GameController.IsPaused() && grounded)
					requestJump = true;
			}
		}

		void FixedUpdate(){
			if(photonView.isMine){
				if(!GameController.IsPaused()){
					inputZ = Mathf.Lerp(inputZ, GetAxisFromKeys(KeyCode.W, KeyCode.S), inputLerpSpeed * Time.deltaTime);
					inputX = Mathf.Lerp(inputX, GetAxisFromKeys(KeyCode.D, KeyCode.A), inputLerpSpeed * Time.deltaTime);
				}else{
					inputZ = Mathf.Lerp(inputZ, 0f, inputLerpSpeed * Time.deltaTime);
					inputX = Mathf.Lerp(inputX, 0f, inputLerpSpeed * Time.deltaTime);
				}

				if(grounded){
					if(fallStartLevel != float.MinValue){
						if(transform.position.y < fallStartLevel - fallingDamageThreshold)
							FallingDamage(fallStartLevel - transform.position.y);
					}

					bool crouch = Input.GetKey(KeyCode.C) && grounded;

					if(!GameController.IsPaused() && Input.GetKey(KeyCode.LeftShift) && !crouch && !Crouching() && Moving() && !player.reloading){
						//Running
						speed = runningSpeed;
						headbob.template = runningHeadbob;
					}else{
						sway.template = !player.aiming ? walkingSway : aimingSway;

						if(crouch && !GameController.IsPaused()){
							//Crouching or CrouchAiming
							speed = !player.aiming ? crouchingSpeed : crouchAimingSpeed;
							headbob.template = !player.aiming ? crouchingHeadbob : crouchAimingHeadbob;
						}else{
							//Walking or Aiming
							speed = !player.aiming ? walkingSpeed : aimingSpeed;
							headbob.template = !player.aiming ? walkingHeadbob : aimingHeadbob;
						}
					}

					moveDirection = new Vector3(inputX, -antiBumpFactor, inputZ);
					moveDirection = transform.TransformDirection(moveDirection) * speed;

					if(requestJump){
						requestJump = false;
						moveDirection.y = jumpSpeed;
					}
				}else{
					if(fallStartLevel == float.MinValue){
						fallStartLevel = transform.position.y;
					}

					if(airControl){
						moveDirection.x = inputX * speed;
						moveDirection.z = inputZ * speed;
						moveDirection = transform.TransformDirection(moveDirection);
					}
				}

				float y = moveDirection.y;
				moveDirection.y = 0f;
				moveDirection = Vector3.ClampMagnitude(moveDirection, speed);
				moveDirection.y = y;

				moveDirection.y -= gravity * Time.deltaTime;

				controller.Move(moveDirection * Time.deltaTime);
			}
		}

		void FallingDamage(float fallDistance){
			print("Fall damage! - " + fallDistance);

			fallStartLevel = float.MinValue;
		}

		public bool Running(){
			if(photonView.isMine){
				return speed == runningSpeed && Moving();
			}

			return player.running;
		}

		public bool Moving(){
			if(photonView.isMine)
				return !GameController.IsPaused() && (
					Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) ||
					Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)
				);

			return player.walking;
		}

		public bool Crouching(){
			if(photonView.isMine)
				return (speed == crouchingSpeed || speed == crouchAimingSpeed);
			
			return player.crouching;
		}

		public bool CrouchAiming(){
			if(photonView.isMine)
				return speed == crouchAimingSpeed;
			
			return player.crouchAiming;
		}

		public float GetAxisFromKeys(KeyCode positive, KeyCode negative){
			if(Input.GetKey(positive) && !Input.GetKey(negative)){
				return 1f;
			}else if(!Input.GetKey(positive) && Input.GetKey(negative)){
				return -1f;
			}else{
				return 0f;
			}
		}
	}
}