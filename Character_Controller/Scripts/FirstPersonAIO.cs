//Modified By Fungus-Light
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Black_Rabbit
{
    public enum InvertMouseInput { None, X, Y, Both }

    [RequireComponent(typeof(CapsuleCollider)), RequireComponent(typeof(Rigidbody)), AddComponentMenu("First Person Controller")]
    public class FirstPersonAIO : MonoBehaviour
    {


        #region Variables

        #region Input Settings

        #endregion

        #region Look Settings
        public bool enableCameraMovement = true;
        
        public InvertMouseInput mouseInputInversion = InvertMouseInput.None;
        public float verticalRotationRange = 170;
        public float mouseSensitivity = 10;
        public float mouseSensitivityInternal;
        public float fOVToMouseSensitivity = 1;
        public float cameraSmoothing = 5f;
        public bool lockAndHideCursor = false;
        public Camera playerCamera;
        public bool enableCameraShake = false;
        internal Vector3 cameraStartingPosition;
        float baseCamFOV;


        public bool autoCrosshair = false;
        public bool drawStaminaMeter = true;
        float smoothRef;
        Image StaminaMeter;
        Image StaminaMeterBG;
        public Sprite Crosshair;
        public Vector3 targetAngles;
        public Vector3 followAngles;
        public Vector3 followVelocity;
        public Vector3 originalRotation;
        #endregion

        #region Movement Settings

        public bool playerCanMove = true;
        public bool walkByDefault = true;
        public float walkSpeed = 4f;
        public KeyCode sprintKey = KeyCode.LeftShift;
        public float sprintSpeed = 8f;
        public float jumpPower = 5f;
        public bool canJump = true;
        public bool canHoldJump;
        bool didJump;
        public bool useStamina = true;
        public float staminaDepletionSpeed = 5f;
        public float staminaLevel = 50;
        public float speed;
        public float staminaInternal;
        internal float walkSpeedInternal;
        internal float sprintSpeedInternal;
        internal float jumpPowerInternal;

        [System.Serializable]
        public class CrouchModifiers
        {
            public bool useCrouch = true;
            public bool toggleCrouch = false;
            public KeyCode crouchKey = KeyCode.LeftControl;
            public float crouchWalkSpeedMultiplier = 0.5f;
            public float crouchJumpPowerMultiplier = 0f;
            public bool crouchOverride;
            internal float colliderHeight;

        }
        public CrouchModifiers _crouchModifiers = new CrouchModifiers();
        [System.Serializable]
        public class FOV_Kick
        {
            public bool useFOVKick = false;
            public float FOVKickAmount = 4;
            public float changeTime = 0.1f;
            public AnimationCurve KickCurve = new AnimationCurve();
            public float fovStart;
        }
        public FOV_Kick fOVKick = new FOV_Kick();
        [System.Serializable]
        public class AdvancedSettings
        {
            public float gravityMultiplier = 1.0f;
            public PhysicMaterial zeroFrictionMaterial;
            public PhysicMaterial highFrictionMaterial;
            public float _maxSlopeAngle = 70;
            public float maxStepHeight = 0.2f;
            internal bool stairMiniHop = false;
            public RaycastHit surfaceAngleCheck;
            public float lastKnownSlopeAngle;
        }
        public AdvancedSettings advanced = new AdvancedSettings();
        private CapsuleCollider capsule;
        private const float jumpRayLength = 0.7f;
        public bool IsGrounded { get; private set; }
        Vector2 inputXY;
        public bool isCrouching;

        bool isSprinting = false;

        public Rigidbody fps_Rigidbody;

        #endregion

        #region Headbobbing Settings
        public bool useHeadbob = true;
        public Transform head = null;
        public bool snapHeadjointToCapsul = true;
        public float headbobFrequency = 1.5f;
        public float headbobSwayAngle = 5f;
        public float headbobHeight = 3f;
        public float headbobSideMovement = 5f;
        public float jumpLandIntensity = 3f;
        private Vector3 originalLocalPosition;
        private float nextStepTime = 0.5f;
        private float headbobCycle = 0.0f;
        private float headbobFade = 0.0f;
        private float springPosition = 0.0f;
        private float springVelocity = 0.0f;
        private float springElastic = 1.1f;
        private float springDampen = 0.8f;
        private float springVelocityThreshold = 0.05f;
        private float springPositionThreshold = 0.05f;
        Vector3 previousPosition;
        Vector3 previousVelocity = Vector3.zero;
        Vector3 miscRefVel;
        bool previousGrounded;
        AudioSource audioSource;

        #endregion

        #region Audio Settings

        public float Volume = 5f;
        public AudioClip jumpSound = null;
        public AudioClip landSound = null;
        public List<AudioClip> footStepSounds = null;
        public enum FSMode { Static, Dynamic }
        public FSMode fsmode;

        [System.Serializable]
        public class DynamicFootStep
        {
            public enum matMode { physicMaterial, Material };
            public matMode materialMode;
            public List<PhysicMaterial> woodPhysMat;
            public List<PhysicMaterial> metalAndGlassPhysMat;
            public List<PhysicMaterial> grassPhysMat;
            public List<PhysicMaterial> dirtAndGravelPhysMat;
            public List<PhysicMaterial> rockAndConcretePhysMat;
            public List<PhysicMaterial> mudPhysMat;
            public List<PhysicMaterial> customPhysMat;

            public List<Material> woodMat;
            public List<Material> metalAndGlassMat;
            public List<Material> grassMat;
            public List<Material> dirtAndGravelMat;
            public List<Material> rockAndConcreteMat;
            public List<Material> mudMat;
            public List<Material> customMat;
            public List<AudioClip> currentClipSet;

            public List<AudioClip> woodClipSet;
            public List<AudioClip> metalAndGlassClipSet;
            public List<AudioClip> grassClipSet;
            public List<AudioClip> dirtAndGravelClipSet;
            public List<AudioClip> rockAndConcreteClipSet;
            public List<AudioClip> mudClipSet;
            public List<AudioClip> customClipSet;
        }
        public DynamicFootStep dynamicFootstep = new DynamicFootStep();

        #endregion

        #region BETA Settings
        /*
         [System.Serializable]
    public class BETA_SETTINGS{

    }

                [Space(15)]
        [Tooltip("Settings in this feild are currently in beta testing and can prove to be unstable.")]
        [Space(5)]
        public BETA_SETTINGS betaSettings = new BETA_SETTINGS();
         */

        #endregion

        #endregion

        private void Awake()
        {
            #region Look Settings - Awake
            originalRotation = transform.localRotation.eulerAngles;
            //Debug.Log(originalLocalPosition);
            #endregion

            #region Movement Settings - Awake
            walkSpeedInternal = walkSpeed;
            sprintSpeedInternal = sprintSpeed;
            jumpPowerInternal = jumpPower;
            capsule = GetComponent<CapsuleCollider>();
            IsGrounded = true;
            isCrouching = false;
            fps_Rigidbody = GetComponent<Rigidbody>();
            _crouchModifiers.colliderHeight = capsule.height;
            #endregion

            #region Headbobbing Settings - Awake

            #endregion

            #region BETA_SETTINGS - Awake

            #endregion

        }

        private void Start()
        {

            //fuck
            followAngles = Vector3.zero;
            originalRotation = Vector3.zero;
            targetAngles = Vector3.zero;


            #region Look Settings - Start

            if (autoCrosshair || drawStaminaMeter)
            {
                Canvas canvas = new GameObject("AutoCrosshair").AddComponent<Canvas>();
                canvas.gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.pixelPerfect = true;
                canvas.transform.SetParent(playerCamera.transform);
                canvas.transform.position = Vector3.zero;

                if (autoCrosshair)
                {
                    Image crossHair = new GameObject("Crosshair").AddComponent<Image>();
                    crossHair.sprite = Crosshair;
                    crossHair.rectTransform.sizeDelta = new Vector2(25, 25);
                    crossHair.transform.SetParent(canvas.transform);
                    crossHair.transform.position = Vector3.zero;
                }

                if (drawStaminaMeter)
                {
                    StaminaMeterBG = new GameObject("StaminaMeter").AddComponent<Image>();
                    StaminaMeter = new GameObject("Meter").AddComponent<Image>();
                    StaminaMeter.transform.SetParent(StaminaMeterBG.transform);
                    StaminaMeterBG.transform.SetParent(canvas.transform);
                    StaminaMeterBG.transform.position = Vector3.zero;
                    StaminaMeterBG.rectTransform.anchorMax = new Vector2(0.5f, 0);
                    StaminaMeterBG.rectTransform.anchorMin = new Vector2(0.5f, 0);
                    StaminaMeterBG.rectTransform.anchoredPosition = new Vector2(0, 15);
                    StaminaMeterBG.rectTransform.sizeDelta = new Vector2(250, 6);
                    StaminaMeterBG.color = new Color(0, 0, 0, 0);
                    StaminaMeter.rectTransform.sizeDelta = new Vector2(250, 6);
                    StaminaMeter.color = new Color(0, 0, 0, 0);
                }
            }
            mouseSensitivityInternal = mouseSensitivity;
            cameraStartingPosition = playerCamera.transform.localPosition;
            //Debug.Log(cameraStartingPosition);
            if (lockAndHideCursor) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
            baseCamFOV = playerCamera.fieldOfView;
            #endregion

            #region Movement Settings - Start  
            staminaInternal = staminaLevel;
            advanced.zeroFrictionMaterial = new PhysicMaterial("Zero_Friction");
            advanced.zeroFrictionMaterial.dynamicFriction = 0;
            advanced.zeroFrictionMaterial.staticFriction = 0;
            advanced.zeroFrictionMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
            advanced.zeroFrictionMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
            advanced.highFrictionMaterial = new PhysicMaterial("Max_Friction");
            advanced.highFrictionMaterial.dynamicFriction = 1;
            advanced.highFrictionMaterial.staticFriction = 1;
            advanced.highFrictionMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
            advanced.highFrictionMaterial.bounceCombine = PhysicMaterialCombine.Average;
            #endregion

            #region Headbobbing Settings - Start

            originalLocalPosition = snapHeadjointToCapsul ? new Vector3(head.localPosition.x, (capsule.height / 2) * head.localScale.y, head.localPosition.z) : head.localPosition;
            if (GetComponent<AudioSource>() == null) { gameObject.AddComponent<AudioSource>(); }

            previousPosition = fps_Rigidbody.position;
            audioSource = GetComponent<AudioSource>();
            #endregion

            #region BETA_SETTINGS - Start
            fOVKick.fovStart = playerCamera.fieldOfView;
            #endregion
        }

        private void Update()
        {
            #region Look Settings - Update

            if (enableCameraMovement)
            {
                float mouseYInput;
                float mouseXInput;
                float camFOV = playerCamera.fieldOfView;
                mouseYInput = mouseInputInversion == InvertMouseInput.None || mouseInputInversion == InvertMouseInput.X ? Input.GetAxis("Mouse Y") : -Input.GetAxis("Mouse Y");
                mouseXInput = mouseInputInversion == InvertMouseInput.None || mouseInputInversion == InvertMouseInput.Y ? Input.GetAxis("Mouse X") : -Input.GetAxis("Mouse X");
                //Debug.Log(mouseXInput);
                //Debug.Log(mouseYInput);
                if (targetAngles.y > 180) { targetAngles.y -= 360; followAngles.y -= 360; } else if (targetAngles.y < -180) { targetAngles.y += 360; followAngles.y += 360; }
                if (targetAngles.x > 180) { targetAngles.x -= 360; followAngles.x -= 360; } else if (targetAngles.x < -180) { targetAngles.x += 360; followAngles.x += 360; }
                targetAngles.y += mouseXInput * (mouseSensitivityInternal - ((baseCamFOV - camFOV) * fOVToMouseSensitivity) / 6f);
                targetAngles.x += mouseYInput * (mouseSensitivityInternal - ((baseCamFOV - camFOV) * fOVToMouseSensitivity) / 6f);
                targetAngles.y = Mathf.Clamp(targetAngles.y, -0.5f * Mathf.Infinity, 0.5f * Mathf.Infinity);
                targetAngles.x = Mathf.Clamp(targetAngles.x, -0.5f * verticalRotationRange, 0.5f * verticalRotationRange);
                //Debug.Log(targetAngles);
                followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, (cameraSmoothing) / 100);
                playerCamera.transform.localRotation = Quaternion.Euler(-followAngles.x + originalRotation.x, 0, 0);
                transform.localRotation = Quaternion.Euler(0, followAngles.y + originalRotation.y, 0);
                //Debug.Log(playerCamera.transform.localRotation);
                //Debug.Log(transform.localRotation);
            }

            #endregion

            #region  Input Settings - Update
            didJump = canHoldJump ? Input.GetButton("Jump") : Input.GetButtonDown("Jump");

            if (_crouchModifiers.useCrouch)
            {
                if (!_crouchModifiers.toggleCrouch) { isCrouching = _crouchModifiers.crouchOverride || Input.GetKey(_crouchModifiers.crouchKey); }
                else { if (Input.GetKeyDown(_crouchModifiers.crouchKey)) { isCrouching = !isCrouching || _crouchModifiers.crouchOverride; } }
            }
            #endregion

            #region Movement Settings - Update

            #endregion

            #region Headbobbing Settings - Update

            #endregion

            #region BETA_SETTINGS - Update

            #endregion
        }

        private void FixedUpdate()
        {
            #region Look Settings - FixedUpdate

            #endregion

            #region Movement Settings - FixedUpdate

            bool wasWalking = !isSprinting;
            if (useStamina)
            {
                isSprinting = Input.GetKey(sprintKey) && !isCrouching && staminaInternal > 0 && (Mathf.Abs(fps_Rigidbody.velocity.x) > 0.01f || Mathf.Abs(fps_Rigidbody.velocity.x) > 0.01f);
                if (isSprinting)
                {
                    staminaInternal -= (staminaDepletionSpeed * 2) * Time.deltaTime;
                    if (drawStaminaMeter)
                    {
                        StaminaMeterBG.color = Vector4.MoveTowards(StaminaMeterBG.color, new Vector4(0, 0, 0, 0.5f), 0.15f);
                        StaminaMeter.color = Vector4.MoveTowards(StaminaMeter.color, new Vector4(1, 1, 1, 1), 0.15f);
                    }
                }
                else if ((!Input.GetKey(sprintKey) || Mathf.Abs(fps_Rigidbody.velocity.x) < 0.01f || Mathf.Abs(fps_Rigidbody.velocity.x) < 0.01f || isCrouching) && staminaInternal < staminaLevel)
                {
                    staminaInternal += staminaDepletionSpeed * Time.deltaTime;
                }
                if (drawStaminaMeter && staminaInternal == staminaLevel)
                {
                    StaminaMeterBG.color = Vector4.MoveTowards(StaminaMeterBG.color, new Vector4(0, 0, 0, 0), 0.15f);
                    StaminaMeter.color = Vector4.MoveTowards(StaminaMeter.color, new Vector4(1, 1, 1, 0), 0.15f);
                }
                staminaInternal = Mathf.Clamp(staminaInternal, 0, staminaLevel);
                float x = Mathf.Clamp(Mathf.SmoothDamp(StaminaMeter.transform.localScale.x, (staminaInternal / staminaLevel) * StaminaMeterBG.transform.localScale.x, ref smoothRef, (1) * Time.deltaTime, 1), 0.001f, StaminaMeterBG.transform.localScale.x);
                StaminaMeter.transform.localScale = new Vector3(x, 1, 1);
            }
            else { isSprinting = Input.GetKey(sprintKey); }

            Vector3 dMove = Vector3.zero;
            speed = walkByDefault ? isCrouching ? walkSpeedInternal : (isSprinting ? sprintSpeedInternal : walkSpeedInternal) : (isSprinting ? walkSpeedInternal : sprintSpeedInternal);
            if (IsGrounded || fps_Rigidbody.velocity.y < 0.1)
            {
                RaycastHit[] hits = Physics.SphereCastAll(transform.position - new Vector3(0, ((capsule.height / 2) * transform.localScale.y) - 0.01f, 0), capsule.radius, Vector3.down, 0, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                float nearest = float.PositiveInfinity;
                IsGrounded = false;
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].distance < nearest && hits[i].collider != capsule)
                    {
                        IsGrounded = true;
                        advanced.stairMiniHop = false;
                        nearest = hits[i].distance;
                    }
                }
            }




            if (advanced._maxSlopeAngle > 0 && Physics.Raycast(transform.position - new Vector3(0, ((capsule.height / 2) * transform.localScale.y) - capsule.radius, 0), new Vector3(dMove.x, -1.5f, dMove.z), out advanced.surfaceAngleCheck, 1.5f))
            {
                dMove = (transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeedInternal) * SlopeCheck();
                if (SlopeCheck() <= 0) { didJump = false; }
            }
            else
            {
                dMove = transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeedInternal;
            }

            RaycastHit WT;


            if (IsGrounded && advanced.maxStepHeight > 0 && Physics.Raycast(transform.position - new Vector3(0, ((capsule.height / 2) * transform.localScale.y) - 0.01f, 0), dMove, out WT, capsule.radius + 0.15f) && Vector3.Angle(WT.normal, Vector3.up) > 88)
            {
                RaycastHit ST;
                if (!Physics.Raycast(transform.position - new Vector3(0, ((capsule.height / 2) * transform.localScale.y) - (advanced.maxStepHeight), 0), dMove, out ST, capsule.radius + 0.25f))
                {
                    advanced.stairMiniHop = true;
                    transform.position += new Vector3(0, advanced.maxStepHeight * 1.2f, 0);
                }
            }
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            inputXY = new Vector2(horizontalInput, verticalInput);
            if (inputXY.magnitude > 1) { inputXY.Normalize(); }

            float yv = fps_Rigidbody.velocity.y;

            if (!canJump) didJump = false;

            if (IsGrounded && didJump && jumpPowerInternal > 0)
            {
                yv += jumpPowerInternal;
                IsGrounded = false;
                didJump = false;
            }

            if (playerCanMove)
            {
                fps_Rigidbody.velocity = dMove + (Vector3.up * yv);
            }
            else { fps_Rigidbody.velocity = Vector3.zero; }

            if (dMove.magnitude > 0 || !IsGrounded)
            {
                capsule.sharedMaterial = advanced.zeroFrictionMaterial;
            }
            else { capsule.sharedMaterial = advanced.highFrictionMaterial; }

            fps_Rigidbody.AddForce(Physics.gravity * (advanced.gravityMultiplier - 1));
            /* if(fOVKick.useFOVKick && wasWalking == isSprinting && fps_Rigidbody.velocity.magnitude > 0.1f && !isCrouching){
                StopAllCoroutines();
                StartCoroutine(wasWalking ? FOVKickOut() : FOVKickIn());
            } */

            if (_crouchModifiers.useCrouch)
            {

                if (isCrouching)
                {
                    capsule.height = Mathf.MoveTowards(capsule.height, _crouchModifiers.colliderHeight / 1.5f, 5 * Time.deltaTime);
                    walkSpeedInternal = walkSpeed * _crouchModifiers.crouchWalkSpeedMultiplier;
                    jumpPowerInternal = jumpPower * _crouchModifiers.crouchJumpPowerMultiplier;

                }
                else
                {
                    capsule.height = Mathf.MoveTowards(capsule.height, _crouchModifiers.colliderHeight, 5 * Time.deltaTime);
                    walkSpeedInternal = walkSpeed;
                    sprintSpeedInternal = sprintSpeed;
                    jumpPowerInternal = jumpPower;
                }
            }

            #endregion

            #region BETA_SETTINGS - FixedUpdate

            #endregion

            #region Headbobbing Settings - FixedUpdate
            float yPos = 0;
            float xPos = 0;
            float zTilt = 0;
            float xTilt = 0;
            float bobSwayFactor = 0;
            float bobFactor = 0;
            float strideLangthen = 0;
            float flatVel = 0;

            //calculate headbob freq
            if (useHeadbob == true || fsmode == FSMode.Dynamic)
            {
                Vector3 vel = (fps_Rigidbody.position - previousPosition) / Time.deltaTime;
                Vector3 velChange = vel - previousVelocity;
                previousPosition = fps_Rigidbody.position;
                previousVelocity = vel;
                springVelocity -= velChange.y;
                springVelocity -= springPosition * springElastic;
                springVelocity *= springDampen;
                springPosition += springVelocity * Time.deltaTime;
                springPosition = Mathf.Clamp(springPosition, -0.3f, 0.3f);

                if (Mathf.Abs(springVelocity) < springVelocityThreshold && Mathf.Abs(springPosition) < springPositionThreshold) { springPosition = 0; springVelocity = 0; }
                flatVel = new Vector3(vel.x, 0.0f, vel.z).magnitude;
                strideLangthen = 1 + (flatVel * ((headbobFrequency * 2) / 10));
                headbobCycle += (flatVel / strideLangthen) * (Time.deltaTime / headbobFrequency);
                bobFactor = Mathf.Sin(headbobCycle * Mathf.PI * 2);
                bobSwayFactor = Mathf.Sin(Mathf.PI * (2 * headbobCycle + 0.5f));
                bobFactor = 1 - (bobFactor * 0.5f + 1);
                bobFactor *= bobFactor;

                yPos = 0;
                xPos = 0;
                zTilt = 0;
                if (jumpLandIntensity > 0 && !advanced.stairMiniHop) { xTilt = -springPosition * (jumpLandIntensity * 5.5f); }
                else if (!advanced.stairMiniHop) { xTilt = -springPosition; }

                if (IsGrounded)
                {
                    if (new Vector3(vel.x, 0.0f, vel.z).magnitude < 0.1f) { headbobFade = Mathf.MoveTowards(headbobFade, 0.0f, 0.5f); } else { headbobFade = Mathf.MoveTowards(headbobFade, 1.0f, Time.deltaTime); }
                    float speedHeightFactor = 1 + (flatVel * 0.3f);
                    xPos = -(headbobSideMovement / 10) * headbobFade * bobSwayFactor;
                    yPos = springPosition * (jumpLandIntensity / 10) + bobFactor * (headbobHeight / 10) * headbobFade * speedHeightFactor;
                    zTilt = bobSwayFactor * (headbobSwayAngle / 10) * headbobFade;
                }
            }
            //apply headbob position
            if (useHeadbob == true)
            {
                if (fps_Rigidbody.velocity.magnitude > 0.1f)
                {
                    head.localPosition = Vector3.MoveTowards(head.localPosition, snapHeadjointToCapsul ? (new Vector3(originalLocalPosition.x, (capsule.height / 2) * head.localScale.y, originalLocalPosition.z) + new Vector3(xPos, yPos, 0)) : originalLocalPosition + new Vector3(xPos, yPos, 0), 0.5f);
                }
                else
                {
                    head.localPosition = Vector3.SmoothDamp(head.localPosition, snapHeadjointToCapsul ? (new Vector3(originalLocalPosition.x, (capsule.height / 2) * head.localScale.y, originalLocalPosition.z) + new Vector3(xPos, yPos, 0)) : originalLocalPosition + new Vector3(xPos, yPos, 0), ref miscRefVel, 0.15f);
                }
                head.localRotation = Quaternion.Euler(xTilt, 0, zTilt);


            }
            #endregion

            #region Dynamic Footsteps
            if (fsmode == FSMode.Dynamic)
            {
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(transform.position, Vector3.down, out hit))
                {

                    if (dynamicFootstep.materialMode == DynamicFootStep.matMode.physicMaterial)
                    {
                        dynamicFootstep.currentClipSet = (dynamicFootstep.woodPhysMat.Any() && dynamicFootstep.woodPhysMat.Contains(hit.collider.sharedMaterial) && dynamicFootstep.woodClipSet.Any()) ? // If standing on Wood
                        dynamicFootstep.woodClipSet : ((dynamicFootstep.grassPhysMat.Any() && dynamicFootstep.grassPhysMat.Contains(hit.collider.sharedMaterial) && dynamicFootstep.grassClipSet.Any()) ? // If standing on Grass
                        dynamicFootstep.grassClipSet : ((dynamicFootstep.metalAndGlassPhysMat.Any() && dynamicFootstep.metalAndGlassPhysMat.Contains(hit.collider.sharedMaterial) && dynamicFootstep.metalAndGlassClipSet.Any()) ? // If standing on Metal/Glass
                        dynamicFootstep.metalAndGlassClipSet : ((dynamicFootstep.rockAndConcretePhysMat.Any() && dynamicFootstep.rockAndConcretePhysMat.Contains(hit.collider.sharedMaterial) && dynamicFootstep.rockAndConcreteClipSet.Any()) ? // If standing on Rock/Concrete
                        dynamicFootstep.rockAndConcreteClipSet : ((dynamicFootstep.dirtAndGravelPhysMat.Any() && dynamicFootstep.dirtAndGravelPhysMat.Contains(hit.collider.sharedMaterial) && dynamicFootstep.dirtAndGravelClipSet.Any()) ? // If standing on Dirt/Gravle
                        dynamicFootstep.dirtAndGravelClipSet : ((dynamicFootstep.mudPhysMat.Any() && dynamicFootstep.mudPhysMat.Contains(hit.collider.sharedMaterial) && dynamicFootstep.mudClipSet.Any()) ? // If standing on Mud
                        dynamicFootstep.mudClipSet : ((dynamicFootstep.customPhysMat.Any() && dynamicFootstep.customPhysMat.Contains(hit.collider.sharedMaterial) && dynamicFootstep.customClipSet.Any()) ? // If standing on the custom material 
                        dynamicFootstep.customClipSet : footStepSounds)))))); // If material is unknown, fall back
                    }
                    else if (hit.collider.GetComponent<MeshRenderer>())
                    {
                        dynamicFootstep.currentClipSet = (dynamicFootstep.woodMat.Any() && dynamicFootstep.woodMat.Contains(hit.collider.GetComponent<MeshRenderer>().sharedMaterial) && dynamicFootstep.woodClipSet.Any()) ? // If standing on Wood
                        dynamicFootstep.woodClipSet : ((dynamicFootstep.grassMat.Any() && dynamicFootstep.grassMat.Contains(hit.collider.GetComponent<MeshRenderer>().sharedMaterial) && dynamicFootstep.grassClipSet.Any()) ? // If standing on Grass
                        dynamicFootstep.grassClipSet : ((dynamicFootstep.metalAndGlassMat.Any() && dynamicFootstep.metalAndGlassMat.Contains(hit.collider.GetComponent<MeshRenderer>().sharedMaterial) && dynamicFootstep.metalAndGlassClipSet.Any()) ? // If standing on Metal/Glass
                        dynamicFootstep.metalAndGlassClipSet : ((dynamicFootstep.rockAndConcreteMat.Any() && dynamicFootstep.rockAndConcreteMat.Contains(hit.collider.GetComponent<MeshRenderer>().sharedMaterial) && dynamicFootstep.rockAndConcreteClipSet.Any()) ? // If standing on Rock/Concrete
                        dynamicFootstep.rockAndConcreteClipSet : ((dynamicFootstep.dirtAndGravelMat.Any() && dynamicFootstep.dirtAndGravelMat.Contains(hit.collider.GetComponent<MeshRenderer>().sharedMaterial) && dynamicFootstep.dirtAndGravelClipSet.Any()) ? // If standing on Dirt/Gravle
                        dynamicFootstep.dirtAndGravelClipSet : ((dynamicFootstep.mudMat.Any() && dynamicFootstep.mudMat.Contains(hit.collider.GetComponent<MeshRenderer>().sharedMaterial) && dynamicFootstep.mudClipSet.Any()) ? // If standing on Mud
                        dynamicFootstep.mudClipSet : ((dynamicFootstep.customMat.Any() && dynamicFootstep.customMat.Contains(hit.collider.GetComponent<MeshRenderer>().sharedMaterial) && dynamicFootstep.customClipSet.Any()) ? // If standing on the custom material 
                        dynamicFootstep.customClipSet : footStepSounds.Any() ? footStepSounds : null)))))); // If material is unknown, fall back
                    }

                    if (IsGrounded)
                    {
                        if (!previousGrounded)
                        {
                            if (dynamicFootstep.currentClipSet.Any()) { audioSource.PlayOneShot(dynamicFootstep.currentClipSet[Random.Range(0, dynamicFootstep.currentClipSet.Count)], Volume / 10); }
                            nextStepTime = headbobCycle + 0.5f;
                        }
                        else
                        {
                            if (headbobCycle > nextStepTime)
                            {
                                nextStepTime = headbobCycle + 0.5f;
                                if (dynamicFootstep.currentClipSet.Any()) { audioSource.PlayOneShot(dynamicFootstep.currentClipSet[Random.Range(0, dynamicFootstep.currentClipSet.Count)], Volume / 10); }
                            }
                        }
                        previousGrounded = true;
                    }
                    else
                    {
                        if (previousGrounded)
                        {
                            if (dynamicFootstep.currentClipSet.Any()) { audioSource.PlayOneShot(dynamicFootstep.currentClipSet[Random.Range(0, dynamicFootstep.currentClipSet.Count)], Volume / 10); }
                        }
                        previousGrounded = false;
                    }

                }
                else
                {
                    dynamicFootstep.currentClipSet = footStepSounds;
                    if (IsGrounded)
                    {
                        if (!previousGrounded)
                        {
                            if (landSound) { audioSource.PlayOneShot(landSound, Volume / 10); }
                            nextStepTime = headbobCycle + 0.5f;
                        }
                        else
                        {
                            if (headbobCycle > nextStepTime)
                            {
                                nextStepTime = headbobCycle + 0.5f;
                                int n = Random.Range(0, footStepSounds.Count);
                                if (footStepSounds.Any()) { audioSource.PlayOneShot(footStepSounds[n], Volume / 10); }
                                footStepSounds[n] = footStepSounds[0];
                            }
                        }
                        previousGrounded = true;
                    }
                    else
                    {
                        if (previousGrounded)
                        {
                            if (jumpSound) { audioSource.PlayOneShot(jumpSound, Volume / 10); }
                        }
                        previousGrounded = false;
                    }
                }

            }
            else
            {
                if (IsGrounded)
                {
                    if (!previousGrounded)
                    {
                        if (landSound) { audioSource.PlayOneShot(landSound, Volume / 10); }
                        nextStepTime = headbobCycle + 0.5f;
                    }
                    else
                    {
                        if (headbobCycle > nextStepTime)
                        {
                            nextStepTime = headbobCycle + 0.5f;
                            int n = Random.Range(0, footStepSounds.Count);
                            if (footStepSounds.Any() && footStepSounds[n] != null) { audioSource.PlayOneShot(footStepSounds[n], Volume / 10); }

                        }
                    }
                    previousGrounded = true;
                }
                else
                {
                    if (previousGrounded)
                    {
                        if (jumpSound) { audioSource.PlayOneShot(jumpSound, Volume / 10); }
                    }
                    previousGrounded = false;
                }
            }


            #endregion

        }

        /*     public IEnumerator FOVKickOut()
            {
                float t = Mathf.Abs((playerCamera.fieldOfView - fOVKick.fovStart) / fOVKick.FOVKickAmount);
                while(t < fOVKick.changeTime)
                {
                    playerCamera.fieldOfView = fOVKick.fovStart + (fOVKick.KickCurve.Evaluate(t / fOVKick.changeTime) * fOVKick.FOVKickAmount);
                    t += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
            }

            public IEnumerator FOVKickIn()
            {
                float t = Mathf.Abs((playerCamera.fieldOfView - fOVKick.fovStart) / fOVKick.FOVKickAmount);
                while(t > 0)
                {
                    playerCamera.fieldOfView = fOVKick.fovStart + (fOVKick.KickCurve.Evaluate(t / fOVKick.changeTime) * fOVKick.FOVKickAmount);
                    t -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                playerCamera.fieldOfView = fOVKick.fovStart;
            } */

        public IEnumerator CameraShake(float Duration, float Magnitude)
        {
            float elapsed = 0;
            while (elapsed < Duration && enableCameraShake)
            {
                playerCamera.transform.localPosition = Vector3.MoveTowards(playerCamera.transform.localPosition, new Vector3(cameraStartingPosition.x + Random.Range(-1, 1) * Magnitude, cameraStartingPosition.y + Random.Range(-1, 1) * Magnitude, cameraStartingPosition.z), Magnitude * 2);
                yield return new WaitForSecondsRealtime(0.001f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            playerCamera.transform.localPosition = cameraStartingPosition;
        }

        float SlopeCheck()
        {
            advanced.lastKnownSlopeAngle = (Vector3.Angle(advanced.surfaceAngleCheck.normal, Vector3.up));
            return new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(advanced._maxSlopeAngle, 0.0f), new Keyframe(90, 0.0f)) { preWrapMode = WrapMode.Clamp, postWrapMode = WrapMode.ClampForever }.Evaluate(advanced.lastKnownSlopeAngle);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FirstPersonAIO)), InitializeOnLoadAttribute]
    public class FPAIO_Editor : Editor
    {
        FirstPersonAIO t;
        SerializedObject SerT;
        static bool showCrouchMods = false;
        static bool showFOVKickSet = false;
        static bool showAdvanced = false;
        static bool showStaticFS = false;
        SerializedProperty staticFS;

        static bool showWoodFS = false;
        SerializedProperty woodFS;
        SerializedProperty woodMat;
        SerializedProperty woodPhysMat;

        static bool showMetalFS = false;
        SerializedProperty metalFS;
        SerializedProperty metalAndGlassMat;
        SerializedProperty metalAndGlassPhysMat;

        static bool showGrassFS = false;
        SerializedProperty grassFS;
        SerializedProperty grassMat;
        SerializedProperty grassPhysMat;

        static bool showDirtFS = false;
        SerializedProperty dirtFS;
        SerializedProperty dirtAndGravelMat;
        SerializedProperty dirtAndGravelPhysMat;

        static bool showConcreteFS = false;
        SerializedProperty concreteFS;
        SerializedProperty rockAndConcreteMat;
        SerializedProperty rockAndConcretePhysMat;

        static bool showMudFS = false;
        SerializedProperty mudFS;
        SerializedProperty mudMat;
        SerializedProperty mudPhysMat;

        static bool showCustomFS = false;
        SerializedProperty customFS;
        SerializedProperty customMat;
        SerializedProperty customPhysMat;


        void OnEnable()
        {
            t = (FirstPersonAIO)target;
            SerT = new SerializedObject(t);
            staticFS = SerT.FindProperty("footStepSounds");

            woodFS = SerT.FindProperty("dynamicFootstep.woodClipSet");
            woodMat = SerT.FindProperty("dynamicFootstep.woodMat");
            woodPhysMat = SerT.FindProperty("dynamicFootstep.woodPhysMat");

            metalFS = SerT.FindProperty("dynamicFootstep.metalAndGlassClipSet");
            metalAndGlassMat = SerT.FindProperty("dynamicFootstep.metalAndGlassMat");
            metalAndGlassPhysMat = SerT.FindProperty("dynamicFootstep.metalAndGlassPhysMat");

            grassFS = SerT.FindProperty("dynamicFootstep.grassClipSet");
            grassMat = SerT.FindProperty("dynamicFootstep.grassMat");
            grassPhysMat = SerT.FindProperty("dynamicFootstep.grassPhysMat");

            dirtFS = SerT.FindProperty("dynamicFootstep.dirtAndGravelClipSet");
            dirtAndGravelMat = SerT.FindProperty("dynamicFootstep.dirtAndGravelMat");
            dirtAndGravelPhysMat = SerT.FindProperty("dynamicFootstep.dirtAndGravelPhysMat");

            concreteFS = SerT.FindProperty("dynamicFootstep.rockAndConcreteClipSet");
            rockAndConcreteMat = SerT.FindProperty("dynamicFootstep.rockAndConcreteMat");
            rockAndConcretePhysMat = SerT.FindProperty("dynamicFootstep.rockAndConcretePhysMat");

            mudFS = SerT.FindProperty("dynamicFootstep.mudClipSet");
            mudMat = SerT.FindProperty("dynamicFootstep.mudMat");
            mudPhysMat = SerT.FindProperty("dynamicFootstep.mudPhysMat");

            customFS = SerT.FindProperty("dynamicFootstep.customClipSet");
            customMat = SerT.FindProperty("dynamicFootstep.customMat");
            customPhysMat = SerT.FindProperty("dynamicFootstep.customPhysMat");
        }
        public override void OnInspectorGUI()
        {
            SerT.Update();
            EditorGUILayout.Space();

            GUILayout.Label("第一人称控制器", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });

            EditorGUILayout.Space();

            #region Camera Setup
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("相机设置", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            t.enableCameraMovement = EditorGUILayout.ToggleLeft(new GUIContent("启用相机移动", "Determines whether the player can move camera or not."), t.enableCameraMovement);
            EditorGUILayout.Space();
            GUI.enabled = t.enableCameraMovement;

            t.verticalRotationRange = EditorGUILayout.Slider(new GUIContent("垂直旋转范围", "Determines how much range does the camera have to move vertically."), t.verticalRotationRange, 90, 180);

            t.mouseInputInversion = (InvertMouseInput)EditorGUILayout.EnumPopup(new GUIContent("鼠标轴翻转", "Determines if mouse input should be inverted, and along which axes"), t.mouseInputInversion);

            t.mouseSensitivityInternal = t.mouseSensitivity = EditorGUILayout.Slider(new GUIContent("鼠标灵敏度", "Determines how sensitive the mouse is."), t.mouseSensitivity, 1, 15);
            //t.mouseSensitivity = EditorGUILayout.Slider(new GUIContent("Mouse Sensitivity","Determines how sensitive the mouse is."),t.mouseSensitivity, 1,15);
            t.fOVToMouseSensitivity = EditorGUILayout.Slider(new GUIContent("FOV to Mouse Sensitivity", "Determines how much the camera's Field Of View will effect the mouse sensitivity. \n\n0 = no effect, 1 = full effect on sensitivity."), t.fOVToMouseSensitivity, 0, 1);
            t.cameraSmoothing = EditorGUILayout.Slider(new GUIContent("相机平滑", "Determines how smooth the camera movement is."), t.cameraSmoothing, 1, 25);
            t.playerCamera = (Camera)EditorGUILayout.ObjectField(new GUIContent("玩家相机", "Camera attached to this controller"), t.playerCamera, typeof(Camera), true);
            if (!t.playerCamera) { EditorGUILayout.HelpBox("需要指派玩家相机", MessageType.Error); }
            t.enableCameraShake = EditorGUILayout.ToggleLeft(new GUIContent("Enable Camera Shake?", "Call this Coroutine externally with duration ranging from 0.01 to 1, and a magnitude of 0.01 to 0.5."), t.enableCameraShake);
            t.lockAndHideCursor = EditorGUILayout.ToggleLeft(new GUIContent("Lock and Hide Cursor", "For debuging or if You don't plan on having a pause menu or quit button."), t.lockAndHideCursor);
            t.autoCrosshair = EditorGUILayout.ToggleLeft(new GUIContent("Auto Crosshair", "Determines if a basic crosshair will be generated."), t.autoCrosshair);
            if (t.autoCrosshair) { EditorGUI.indentLevel++; EditorGUILayout.BeginHorizontal(); EditorGUILayout.PrefixLabel(new GUIContent("Crosshair", "Sprite to use as a crosshair.")); t.Crosshair = (Sprite)EditorGUILayout.ObjectField(t.Crosshair, typeof(Sprite), false); EditorGUILayout.EndHorizontal(); EditorGUI.indentLevel--; }
            GUI.enabled = true;
            EditorGUILayout.Space();
            #endregion

            #region Movement Setup
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("移动设置", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            t.playerCanMove = EditorGUILayout.ToggleLeft(new GUIContent("允许移动", "Determines if the player is allowed to move."), t.playerCanMove);
            GUI.enabled = t.playerCanMove;
            t.walkByDefault = EditorGUILayout.ToggleLeft(new GUIContent("默认移动方式", "Determines if the default mode of movement is 'Walk' or 'Srpint'."), t.walkByDefault);
            t.walkSpeed = EditorGUILayout.Slider(new GUIContent("走路速度", "Determines how fast the player walks."), t.walkSpeed, 0.1f, 10);
            t.sprintKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Sprint Key", "Determines what key needs to be pressed to enter a sprint"), t.sprintKey);
            t.sprintSpeed = EditorGUILayout.Slider(new GUIContent("Sprint Speed", "Determines how fast the player sprints."), t.sprintSpeed, 0.1f, 20);
            t.canJump = EditorGUILayout.ToggleLeft(new GUIContent("Can Player Jump?", "Determines if the player is allowed to jump."), t.canJump);
            GUI.enabled = t.playerCanMove && t.canJump; EditorGUI.indentLevel++;
            t.jumpPower = EditorGUILayout.Slider(new GUIContent("Jump Power", "Determines how high the player can jump."), t.jumpPower, 0.1f, 15);
            t.canHoldJump = EditorGUILayout.ToggleLeft(new GUIContent("Hold Jump", "Determines if the jump button needs to be pressed down to jump, or if the player can hold the jump button to automaticly jump every time the it hits the ground."), t.canHoldJump);
            EditorGUI.indentLevel--; GUI.enabled = t.playerCanMove;
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            showCrouchMods = EditorGUILayout.Foldout(showCrouchMods, new GUIContent("Crouch Modifiers", "Stat modifiers that will apply when player is crouching."));
            if (showCrouchMods)
            {
                t._crouchModifiers.useCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Enable Coruch", "Determines if the player is allowed to crouch."), t._crouchModifiers.useCrouch);
                GUI.enabled = t.playerCanMove && t._crouchModifiers.useCrouch;
                t._crouchModifiers.crouchKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Crouch Key", "Determines what key needs to be pressed to crouch"), t._crouchModifiers.crouchKey);
                t._crouchModifiers.toggleCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Toggle Crouch?", "Determines if the crouching behaviour is on a toggle or momentary basis."), t._crouchModifiers.toggleCrouch);
                t._crouchModifiers.crouchWalkSpeedMultiplier = EditorGUILayout.Slider(new GUIContent("Crouch Movement Speed Multiplier", "Determines how fast the player can move while crouching."), t._crouchModifiers.crouchWalkSpeedMultiplier, 0.01f, 1.5f);
                t._crouchModifiers.crouchJumpPowerMultiplier = EditorGUILayout.Slider(new GUIContent("Crouching Jump Power Mult.", "Determines how much the player's jumping power is increased or reduced while crouching."), t._crouchModifiers.crouchJumpPowerMultiplier, 0, 1.5f);
                t._crouchModifiers.crouchOverride = EditorGUILayout.ToggleLeft(new GUIContent("Force Crouch Override", "A Toggle that will override the crouch key to force player to crouch."), t._crouchModifiers.crouchOverride);
            }
            GUI.enabled = t.playerCanMove;

            EditorGUILayout.Space();
            showFOVKickSet = EditorGUILayout.Foldout(showFOVKickSet, new GUIContent("FOV Kick Settings", "Settings for FOV Kick"));
            if (showFOVKickSet)
            {
                GUILayout.Label("Under Development", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
                GUI.enabled = false;
                t.fOVKick.useFOVKick = EditorGUILayout.ToggleLeft(new GUIContent("Enable FOV Kick", "Determines if the camera's Field of View will kick when entering a sprint."), t.fOVKick.useFOVKick);
                //GUI.enabled = t.playerCanMove&&t.fOVKick.useFOVKick;
                t.fOVKick.FOVKickAmount = EditorGUILayout.Slider(new GUIContent("Kick Amount", "Determines how much the camera's FOV will kick upon entering a sprint."), t.fOVKick.FOVKickAmount, 0, 10);
                t.fOVKick.changeTime = EditorGUILayout.Slider(new GUIContent("Change Time", "Determines the duration of the FOV kick"), t.fOVKick.changeTime, 0.01f, 5);
                t.fOVKick.KickCurve = EditorGUILayout.CurveField(new GUIContent("Kick Curve", ""), t.fOVKick.KickCurve);
            }
            GUI.enabled = t.playerCanMove;
            EditorGUILayout.Space();
            showAdvanced = EditorGUILayout.Foldout(showAdvanced, new GUIContent("Advanced Movement", "Advanced movenet settings"));
            if (showAdvanced)
            {
                t.useStamina = EditorGUILayout.ToggleLeft(new GUIContent("Enable Stamina", "Determines if spriting will be limited by stamina."), t.useStamina);
                GUI.enabled = t.playerCanMove && t.useStamina; EditorGUI.indentLevel++;
                t.staminaLevel = EditorGUILayout.Slider(new GUIContent("Stamina Level", "Determines how much stamina the player has. if left 0, stamina will not be used."), t.staminaLevel, 0, 100);
                t.staminaDepletionSpeed = EditorGUILayout.Slider(new GUIContent("Stamina Depletion Speed", "Determines how quickly the player's stamina depletes."), t.staminaDepletionSpeed, 0.1f, 9);
                t.drawStaminaMeter = EditorGUILayout.ToggleLeft(new GUIContent("Draw Stamina Meter", "Determines if a basic stamina meter will be generated."), t.drawStaminaMeter);
                GUI.enabled = t.playerCanMove; EditorGUI.indentLevel--;
                EditorGUILayout.Space();
                t.advanced.gravityMultiplier = EditorGUILayout.Slider(new GUIContent("Gravity Multiplier", "Determines how much the physics engine's gravitational force is multiplied."), t.advanced.gravityMultiplier, 0.1f, 5);
                t.advanced._maxSlopeAngle = EditorGUILayout.Slider(new GUIContent("Max Slope Angle", "Determines the maximum angle the player can walk up. If left 0, the slope detection/limiting system will not be used."), t.advanced._maxSlopeAngle, 0, 70);
                t.advanced.maxStepHeight = EditorGUILayout.Slider(new GUIContent("Max Step Height", "EXPERIMENTAL! Determines if a small ledge is a stair by comparing it to this value. Values over 0.5 produces     odd results."), t.advanced.maxStepHeight, 0, 1);
            }

            GUI.enabled = true;
            EditorGUILayout.Space();
            #endregion

            #region Headbobbing Setup
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Headbobbing Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            t.useHeadbob = EditorGUILayout.ToggleLeft(new GUIContent("Enable Headbobbing", "Determines if headbobbing will be used."), t.useHeadbob);
            GUI.enabled = t.useHeadbob;
            t.head = (Transform)EditorGUILayout.ObjectField(new GUIContent("Head Transform", "A transform representing the head. The camera should be a child to this transform."), t.head, typeof(Transform), true);
            if (!t.head) { EditorGUILayout.HelpBox("A Head Transform is required for headbobbing.", MessageType.Error); }
            GUI.enabled = t.useHeadbob && t.head;
            t.snapHeadjointToCapsul = EditorGUILayout.ToggleLeft(new GUIContent("Snap Head to collider", "Recommended. Determines if the head joint will snap to the top on the capsul Collider, It provides better crouch results."), t.snapHeadjointToCapsul);
            t.headbobFrequency = EditorGUILayout.Slider(new GUIContent("Headbob Frequency (Hz)", "Determines how fast the headbobbing cycle is."), t.headbobFrequency, 0.1f, 10);
            t.headbobSwayAngle = EditorGUILayout.Slider(new GUIContent("Tilt Angle", "Determines the angle the head will tilt."), t.headbobSwayAngle, 0, 10);
            t.headbobHeight = EditorGUILayout.Slider(new GUIContent("Headbob Hight", "Determines the highest point the head will reach in the headbob cycle."), t.headbobHeight, 0, 10);
            t.headbobSideMovement = EditorGUILayout.Slider(new GUIContent("Headbob Horizontal Movement", "Determines how much vertical movement will occur in the headbob cycle."), t.headbobSideMovement, 0, 10);
            t.jumpLandIntensity = EditorGUILayout.Slider(new GUIContent("Jump/Land Jerk Intensity", "Determines the Jerk intensity when jumping and landing if any."), t.jumpLandIntensity, 0, 15);
            GUI.enabled = true;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            #endregion

            #region Audio/SFX Setup
            GUILayout.Label("声音设置", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            t.Volume = EditorGUILayout.Slider(new GUIContent("Volume", "Volume to play audio at."), t.Volume, 0, 10);
            EditorGUILayout.Space();
            t.fsmode = (FirstPersonAIO.FSMode)EditorGUILayout.EnumPopup(new GUIContent("Footstep Mode", "Determines the method used to trigger footsetps."), t.fsmode);
            EditorGUILayout.Space();

            #region FS Static
            if (t.fsmode == FirstPersonAIO.FSMode.Static)
            {
                showStaticFS = EditorGUILayout.Foldout(showStaticFS, new GUIContent("Footstep Clips", "Audio clips available as footstep sounds."));
                if (showStaticFS)
                {
                    GUILayout.BeginVertical("box");
                    for (int i = 0; i < staticFS.arraySize; i++)
                    {
                        SerializedProperty LS_ref = staticFS.GetArrayElementAtIndex(i);
                        EditorGUILayout.BeginHorizontal("box");
                        LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("Clip " + (i + 1) + ":", LS_ref.objectReferenceValue, typeof(AudioClip), false);
                        if (GUILayout.Button(new GUIContent("X", "Remove this clip"), GUILayout.MaxWidth(20))) { this.t.footStepSounds.RemoveAt(i); }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button(new GUIContent("Add Clip", "Add new clip entry"))) { this.t.footStepSounds.Add(null); }
                    if (GUILayout.Button(new GUIContent("Remove All Clips", "Remove all clip entries"))) { this.t.footStepSounds.Clear(); }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    DropAreaGUI(t.footStepSounds, GUILayoutUtility.GetLastRect());
                }

                t.jumpSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Jump Clip", "An audio clip that will play when jumping."), t.jumpSound, typeof(AudioClip), false);
                t.landSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Land Clip", "An audio clip that will play when landing."), t.landSound, typeof(AudioClip), false);

            }
            #endregion

            else
            {
                t.dynamicFootstep.materialMode = (FirstPersonAIO.DynamicFootStep.matMode)EditorGUILayout.EnumPopup(new GUIContent("Material Type", "Determines the type of material will trigger footstep audio."), t.dynamicFootstep.materialMode);
                EditorGUILayout.Space();
                #region Wood Section
                showWoodFS = EditorGUILayout.Foldout(showWoodFS, new GUIContent("Wood Clips", "Audio clips available as footsteps when walking on a collider with the Physic Material assigned to 'Wood Physic Material'"));
                if (showWoodFS)
                {
                    GUILayout.BeginVertical("box");
                    if (t.dynamicFootstep.materialMode == FirstPersonAIO.DynamicFootStep.matMode.physicMaterial)
                    {
                        if (!t.dynamicFootstep.woodPhysMat.Any()) { EditorGUILayout.HelpBox("At least one Physic Material must be assigned first.", MessageType.Warning); }
                        EditorGUILayout.LabelField("Wood Physic Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                        for (int i = 0; i < woodPhysMat.arraySize; i++)
                        {
                            SerializedProperty LS_ref = woodPhysMat.GetArrayElementAtIndex(i);
                            EditorGUILayout.BeginHorizontal("box");
                            LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("", LS_ref.objectReferenceValue, typeof(PhysicMaterial), false);
                            if (GUILayout.Button(new GUIContent("X", "Remove this Physic Material"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.woodPhysMat.RemoveAt(i); }
                            EditorGUILayout.EndHorizontal();
                        }


                        if (GUILayout.Button(new GUIContent("Add new Physic Material entry", "Add new Physic Material entry"))) { t.dynamicFootstep.woodPhysMat.Add(null); }
                        GUI.enabled = t.dynamicFootstep.woodPhysMat.Any();
                    }

                    else
                    {
                        if (!t.dynamicFootstep.woodMat.Any()) { EditorGUILayout.HelpBox("At least one Material must be assigned first.", MessageType.Warning); }
                        EditorGUILayout.LabelField("Wood Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                        for (int i = 0; i < woodMat.arraySize; i++)
                        {
                            SerializedProperty LS_ref = woodMat.GetArrayElementAtIndex(i);
                            EditorGUILayout.BeginHorizontal("box");
                            LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("", LS_ref.objectReferenceValue, typeof(Material), false);
                            if (GUILayout.Button(new GUIContent("X", "Remove this Material"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.woodMat.RemoveAt(i); }
                            EditorGUILayout.EndHorizontal();
                        }
                        if (GUILayout.Button(new GUIContent("Add new Material entry", "Add new Material entry"))) { t.dynamicFootstep.woodMat.Add(null); }
                        GUI.enabled = t.dynamicFootstep.woodMat.Any();
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Wood Audio Clips", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                    for (int i = 0; i < woodFS.arraySize; i++)
                    {
                        SerializedProperty LS_ref = woodFS.GetArrayElementAtIndex(i);
                        EditorGUILayout.BeginHorizontal("box");
                        LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("Clip " + (i + 1) + ":", LS_ref.objectReferenceValue, typeof(AudioClip), false);
                        if (GUILayout.Button(new GUIContent("X", "Remove this clip"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.woodClipSet.RemoveAt(i); }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button(new GUIContent("Add Clip", "Add new clip entry"))) { t.dynamicFootstep.woodClipSet.Add(null); }
                    if (GUILayout.Button(new GUIContent("Remove All Clips", "Remove all clip entries"))) { t.dynamicFootstep.woodClipSet.Clear(); }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    DropAreaGUI(t.dynamicFootstep.woodClipSet, GUILayoutUtility.GetLastRect());
                }
                GUI.enabled = true;

                EditorGUILayout.Space();
                #endregion 
                #region Metal Section
                showMetalFS = EditorGUILayout.Foldout(showMetalFS, new GUIContent("Metal & Glass Clips", "Audio clips available as footsteps when walking on a collider with the Physic Material assigned to 'Metal & Glass Physic Material'"));
                if (showMetalFS)
                {
                    GUILayout.BeginVertical("box");

                    if (t.dynamicFootstep.materialMode == FirstPersonAIO.DynamicFootStep.matMode.physicMaterial)
                    {
                        if (!t.dynamicFootstep.metalAndGlassPhysMat.Any()) { EditorGUILayout.HelpBox("At least one Physic Material must be assigned first.", MessageType.Warning); }
                        EditorGUILayout.LabelField("Metal & Glass Physic Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                        for (int i = 0; i < metalAndGlassPhysMat.arraySize; i++)
                        {
                            SerializedProperty LS_ref = metalAndGlassPhysMat.GetArrayElementAtIndex(i);
                            EditorGUILayout.BeginHorizontal("box");
                            LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("", LS_ref.objectReferenceValue, typeof(PhysicMaterial), false);
                            if (GUILayout.Button(new GUIContent("X", "Remove this Physic Material"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.metalAndGlassPhysMat.RemoveAt(i); }
                            EditorGUILayout.EndHorizontal();
                        }


                        if (GUILayout.Button(new GUIContent("Add new Physic Material entry", "Add new Physic Material entry"))) { t.dynamicFootstep.metalAndGlassPhysMat.Add(null); }
                        GUI.enabled = t.dynamicFootstep.metalAndGlassPhysMat.Any();
                    }

                    else
                    {
                        if (!t.dynamicFootstep.metalAndGlassMat.Any()) { EditorGUILayout.HelpBox("At least one Material must be assigned first.", MessageType.Warning); }
                        EditorGUILayout.LabelField("Metal & Glass Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                        for (int i = 0; i < metalAndGlassMat.arraySize; i++)
                        {
                            SerializedProperty LS_ref = metalAndGlassMat.GetArrayElementAtIndex(i);
                            EditorGUILayout.BeginHorizontal("box");
                            LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("", LS_ref.objectReferenceValue, typeof(Material), false);
                            if (GUILayout.Button(new GUIContent("X", "Remove this Material"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.metalAndGlassMat.RemoveAt(i); }
                            EditorGUILayout.EndHorizontal();
                        }
                        if (GUILayout.Button(new GUIContent("Add new Material entry", "Add new Material entry"))) { t.dynamicFootstep.metalAndGlassMat.Add(null); }
                        GUI.enabled = t.dynamicFootstep.metalAndGlassMat.Any();
                    }

                    EditorGUILayout.LabelField("Metal & Glass Audio Clips", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                    for (int i = 0; i < metalFS.arraySize; i++)
                    {
                        SerializedProperty LS_ref = metalFS.GetArrayElementAtIndex(i);
                        EditorGUILayout.BeginHorizontal("box");
                        LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("Clip " + (i + 1) + ":", LS_ref.objectReferenceValue, typeof(AudioClip), false);
                        if (GUILayout.Button(new GUIContent("X", "Remove this clip"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.metalAndGlassClipSet.RemoveAt(i); }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button(new GUIContent("Add Clip", "Add new clip entry"))) { t.dynamicFootstep.metalAndGlassClipSet.Add(null); }
                    if (GUILayout.Button(new GUIContent("Remove All Clips", "Remove all clip entries"))) { t.dynamicFootstep.metalAndGlassClipSet.Clear(); }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    DropAreaGUI(t.dynamicFootstep.metalAndGlassClipSet, GUILayoutUtility.GetLastRect());
                }
                GUI.enabled = true;

                EditorGUILayout.Space();
                #endregion
                #region Grass Section
                showGrassFS = EditorGUILayout.Foldout(showGrassFS, new GUIContent("Grass Clips", "Audio clips available as footsteps when walking on a collider with the Physic Material assigned to 'Grass Physic Material'"));
                if (showGrassFS)
                {
                    GUILayout.BeginVertical("box");

                    if (t.dynamicFootstep.materialMode == FirstPersonAIO.DynamicFootStep.matMode.physicMaterial)
                    {
                        if (!t.dynamicFootstep.grassPhysMat.Any()) { EditorGUILayout.HelpBox("At least one Physic Material must be assigned first.", MessageType.Warning); }
                        EditorGUILayout.LabelField("Grass Physic Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                        for (int i = 0; i < grassPhysMat.arraySize; i++)
                        {
                            SerializedProperty LS_ref = grassPhysMat.GetArrayElementAtIndex(i);
                            EditorGUILayout.BeginHorizontal("box");
                            LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("", LS_ref.objectReferenceValue, typeof(PhysicMaterial), false);
                            if (GUILayout.Button(new GUIContent("X", "Remove this Physic Material"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.grassPhysMat.RemoveAt(i); }
                            EditorGUILayout.EndHorizontal();
                        }


                        if (GUILayout.Button(new GUIContent("Add new Physic Material entry", "Add new Physic Material entry"))) { t.dynamicFootstep.grassPhysMat.Add(null); }
                        GUI.enabled = t.dynamicFootstep.grassPhysMat.Any();
                    }

                    else
                    {
                        if (!t.dynamicFootstep.grassMat.Any()) { EditorGUILayout.HelpBox("At least one Material must be assigned first.", MessageType.Warning); }
                        EditorGUILayout.LabelField("Grass Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                        for (int i = 0; i < grassMat.arraySize; i++)
                        {
                            SerializedProperty LS_ref = grassMat.GetArrayElementAtIndex(i);
                            EditorGUILayout.BeginHorizontal("box");
                            LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("", LS_ref.objectReferenceValue, typeof(Material), false);
                            if (GUILayout.Button(new GUIContent("X", "Remove this Material"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.grassMat.RemoveAt(i); }
                            EditorGUILayout.EndHorizontal();
                        }
                        if (GUILayout.Button(new GUIContent("Add new Material entry", "Add new Material entry"))) { t.dynamicFootstep.grassMat.Add(null); }
                        GUI.enabled = t.dynamicFootstep.grassMat.Any();
                    }

                    EditorGUILayout.LabelField("Grass Audio Clips", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                    for (int i = 0; i < grassFS.arraySize; i++)
                    {
                        SerializedProperty LS_ref = grassFS.GetArrayElementAtIndex(i);
                        EditorGUILayout.BeginHorizontal("box");
                        LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("Clip " + (i + 1) + ":", LS_ref.objectReferenceValue, typeof(AudioClip), false);
                        if (GUILayout.Button(new GUIContent("X", "Remove this clip"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.grassClipSet.RemoveAt(i); }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button(new GUIContent("Add Clip", "Add new clip entry"))) { t.dynamicFootstep.grassClipSet.Add(null); }
                    if (GUILayout.Button(new GUIContent("Remove All Clips", "Remove all clip entries"))) { t.dynamicFootstep.grassClipSet.Clear(); }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    DropAreaGUI(t.dynamicFootstep.grassClipSet, GUILayoutUtility.GetLastRect());
                }
                GUI.enabled = true;

                EditorGUILayout.Space();
                #endregion
                #region Dirt Section
                showDirtFS = EditorGUILayout.Foldout(showDirtFS, new GUIContent("Dirt & Gravel Clips", "Audio clips available as footsteps when walking on a collider with the Physic Material assigned to 'Dirt & Gravel Physic Material'"));
                if (showDirtFS)
                {
                    GUILayout.BeginVertical("box");

                    if (t.dynamicFootstep.materialMode == FirstPersonAIO.DynamicFootStep.matMode.physicMaterial)
                    {
                        if (!t.dynamicFootstep.dirtAndGravelPhysMat.Any()) { EditorGUILayout.HelpBox("At least one Physic Material must be assigned first.", MessageType.Warning); }
                        EditorGUILayout.LabelField("Dirt & Gravel Physic Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                        for (int i = 0; i < dirtAndGravelPhysMat.arraySize; i++)
                        {
                            SerializedProperty LS_ref = dirtAndGravelPhysMat.GetArrayElementAtIndex(i);
                            EditorGUILayout.BeginHorizontal("box");
                            LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("", LS_ref.objectReferenceValue, typeof(PhysicMaterial), false);
                            if (GUILayout.Button(new GUIContent("X", "Remove this Physic Material"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.dirtAndGravelPhysMat.RemoveAt(i); }
                            EditorGUILayout.EndHorizontal();
                        }


                        if (GUILayout.Button(new GUIContent("Add new Physic Material entry", "Add new Physic Material entry"))) { t.dynamicFootstep.dirtAndGravelPhysMat.Add(null); }
                        GUI.enabled = t.dynamicFootstep.dirtAndGravelPhysMat.Any();
                    }

                    else
                    {
                        if (!t.dynamicFootstep.dirtAndGravelMat.Any()) { EditorGUILayout.HelpBox("At least one Material must be assigned first.", MessageType.Warning); }
                        EditorGUILayout.LabelField("Dirt & Gravel Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                        for (int i = 0; i < dirtAndGravelMat.arraySize; i++)
                        {
                            SerializedProperty LS_ref = dirtAndGravelMat.GetArrayElementAtIndex(i);
                            EditorGUILayout.BeginHorizontal("box");
                            LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("", LS_ref.objectReferenceValue, typeof(Material), false);
                            if (GUILayout.Button(new GUIContent("X", "Remove this Material"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.dirtAndGravelMat.RemoveAt(i); }
                            EditorGUILayout.EndHorizontal();
                        }
                        if (GUILayout.Button(new GUIContent("Add new Material entry", "Add new Material entry"))) { t.dynamicFootstep.dirtAndGravelMat.Add(null); }
                        GUI.enabled = t.dynamicFootstep.dirtAndGravelMat.Any();
                    }

                    EditorGUILayout.LabelField("Dirt & Gravel Audio Clips", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                    for (int i = 0; i < dirtFS.arraySize; i++)
                    {
                        SerializedProperty LS_ref = dirtFS.GetArrayElementAtIndex(i);
                        EditorGUILayout.BeginHorizontal("box");
                        LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("Clip " + (i + 1) + ":", LS_ref.objectReferenceValue, typeof(AudioClip), false);
                        if (GUILayout.Button(new GUIContent("X", "Remove this clip"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.dirtAndGravelClipSet.RemoveAt(i); }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button(new GUIContent("Add Clip", "Add new clip entry"))) { t.dynamicFootstep.dirtAndGravelClipSet.Add(null); }
                    if (GUILayout.Button(new GUIContent("Remove All Clips", "Remove all clip entries"))) { t.dynamicFootstep.dirtAndGravelClipSet.Clear(); }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    DropAreaGUI(t.dynamicFootstep.dirtAndGravelClipSet, GUILayoutUtility.GetLastRect());
                }
                GUI.enabled = true;

                EditorGUILayout.Space();
                #endregion
                #region Rock Section
                showConcreteFS = EditorGUILayout.Foldout(showConcreteFS, new GUIContent("Rock & Concrete Clips", "Audio clips available as footsteps when walking on a collider with the Physic Material assigned to 'Rock & Concrete Physic Material'"));
                if (showConcreteFS)
                {
                    GUILayout.BeginVertical("box");

                    if (t.dynamicFootstep.materialMode == FirstPersonAIO.DynamicFootStep.matMode.physicMaterial)
                    {
                        if (!t.dynamicFootstep.rockAndConcretePhysMat.Any()) { EditorGUILayout.HelpBox("At least one Physic Material must be assigned first.", MessageType.Warning); }
                        EditorGUILayout.LabelField("Rock & Concrete Physic Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                        for (int i = 0; i < rockAndConcretePhysMat.arraySize; i++)
                        {
                            SerializedProperty LS_ref = rockAndConcretePhysMat.GetArrayElementAtIndex(i);
                            EditorGUILayout.BeginHorizontal("box");
                            LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("", LS_ref.objectReferenceValue, typeof(PhysicMaterial), false);
                            if (GUILayout.Button(new GUIContent("X", "Remove this Physic Material"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.rockAndConcretePhysMat.RemoveAt(i); }
                            EditorGUILayout.EndHorizontal();
                        }


                        if (GUILayout.Button(new GUIContent("Add new Physic Material entry", "Add new Physic Material entry"))) { t.dynamicFootstep.rockAndConcretePhysMat.Add(null); }
                        GUI.enabled = t.dynamicFootstep.rockAndConcretePhysMat.Any();
                    }

                    else
                    {
                        if (!t.dynamicFootstep.rockAndConcreteMat.Any()) { EditorGUILayout.HelpBox("At least one Material must be assigned first.", MessageType.Warning); }
                        EditorGUILayout.LabelField("Rock & Concrete Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                        for (int i = 0; i < rockAndConcreteMat.arraySize; i++)
                        {
                            SerializedProperty LS_ref = rockAndConcreteMat.GetArrayElementAtIndex(i);
                            EditorGUILayout.BeginHorizontal("box");
                            LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("", LS_ref.objectReferenceValue, typeof(Material), false);
                            if (GUILayout.Button(new GUIContent("X", "Remove this Material"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.rockAndConcreteMat.RemoveAt(i); }
                            EditorGUILayout.EndHorizontal();
                        }
                        if (GUILayout.Button(new GUIContent("Add new Material entry", "Add new Material entry"))) { t.dynamicFootstep.rockAndConcreteMat.Add(null); }
                        GUI.enabled = t.dynamicFootstep.rockAndConcreteMat.Any();
                    }

                    EditorGUILayout.LabelField("Rock & Concrete Audio Clips", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                    for (int i = 0; i < concreteFS.arraySize; i++)
                    {
                        SerializedProperty LS_ref = concreteFS.GetArrayElementAtIndex(i);
                        EditorGUILayout.BeginHorizontal("box");
                        LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("Clip " + (i + 1) + ":", LS_ref.objectReferenceValue, typeof(AudioClip), false);
                        if (GUILayout.Button(new GUIContent("X", "Remove this clip"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.rockAndConcreteClipSet.RemoveAt(i); }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button(new GUIContent("Add Clip", "Add new clip entry"))) { t.dynamicFootstep.rockAndConcreteClipSet.Add(null); }
                    if (GUILayout.Button(new GUIContent("Remove All Clips", "Remove all clip entries"))) { t.dynamicFootstep.rockAndConcreteClipSet.Clear(); }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    DropAreaGUI(t.dynamicFootstep.rockAndConcreteClipSet, GUILayoutUtility.GetLastRect());
                }
                GUI.enabled = true;

                EditorGUILayout.Space();
                #endregion
                #region Mud Section
                showMudFS = EditorGUILayout.Foldout(showMudFS, new GUIContent("Mud Clips", "Audio clips available as footsteps when walking on a collider with the Physic Material assigned to 'Mud Physic Material'"));
                if (showMudFS)
                {
                    GUILayout.BeginVertical("box");

                    if (t.dynamicFootstep.materialMode == FirstPersonAIO.DynamicFootStep.matMode.physicMaterial)
                    {
                        if (!t.dynamicFootstep.mudPhysMat.Any()) { EditorGUILayout.HelpBox("At least one Physic Material must be assigned first.", MessageType.Warning); }
                        EditorGUILayout.LabelField("Mud Physic Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                        for (int i = 0; i < mudPhysMat.arraySize; i++)
                        {
                            SerializedProperty LS_ref = mudPhysMat.GetArrayElementAtIndex(i);
                            EditorGUILayout.BeginHorizontal("box");
                            LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("", LS_ref.objectReferenceValue, typeof(PhysicMaterial), false);
                            if (GUILayout.Button(new GUIContent("X", "Remove this Physic Material"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.mudPhysMat.RemoveAt(i); }
                            EditorGUILayout.EndHorizontal();
                        }


                        if (GUILayout.Button(new GUIContent("Add new Physic Material entry", "Add new Physic Material entry"))) { t.dynamicFootstep.mudPhysMat.Add(null); }
                        GUI.enabled = t.dynamicFootstep.mudPhysMat.Any();
                    }

                    else
                    {
                        if (!t.dynamicFootstep.mudMat.Any()) { EditorGUILayout.HelpBox("At least one Material must be assigned first.", MessageType.Warning); }
                        EditorGUILayout.LabelField("Mud Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                        for (int i = 0; i < mudMat.arraySize; i++)
                        {
                            SerializedProperty LS_ref = mudMat.GetArrayElementAtIndex(i);
                            EditorGUILayout.BeginHorizontal("box");
                            LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("", LS_ref.objectReferenceValue, typeof(Material), false);
                            if (GUILayout.Button(new GUIContent("X", "Remove this Material"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.mudMat.RemoveAt(i); }
                            EditorGUILayout.EndHorizontal();
                        }
                        if (GUILayout.Button(new GUIContent("Add new Material entry", "Add new Material entry"))) { t.dynamicFootstep.mudMat.Add(null); }
                        GUI.enabled = t.dynamicFootstep.mudMat.Any();
                    }

                    EditorGUILayout.LabelField("Mud Audio Clips", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                    for (int i = 0; i < mudFS.arraySize; i++)
                    {
                        SerializedProperty LS_ref = mudFS.GetArrayElementAtIndex(i);
                        EditorGUILayout.BeginHorizontal("box");
                        LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("Clip " + (i + 1) + ":", LS_ref.objectReferenceValue, typeof(AudioClip), false);
                        if (GUILayout.Button(new GUIContent("X", "Remove this clip"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.mudClipSet.RemoveAt(i); }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button(new GUIContent("Add Clip", "Add new clip entry"))) { t.dynamicFootstep.mudClipSet.Add(null); }
                    if (GUILayout.Button(new GUIContent("Remove All Clips", "Remove all clip entries"))) { t.dynamicFootstep.mudClipSet.Clear(); }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    DropAreaGUI(t.dynamicFootstep.mudClipSet, GUILayoutUtility.GetLastRect());
                }
                GUI.enabled = true;

                EditorGUILayout.Space();
                #endregion
                #region Custom Section
                showCustomFS = EditorGUILayout.Foldout(showCustomFS, new GUIContent("Custom Material Clips", "Audio clips available as footsteps when walking on a collider with the Physic Material assigned to 'Custom Physic Material'"));
                if (showCustomFS)
                {
                    GUILayout.BeginVertical("box");

                    if (t.dynamicFootstep.materialMode == FirstPersonAIO.DynamicFootStep.matMode.physicMaterial)
                    {
                        if (!t.dynamicFootstep.customPhysMat.Any()) { EditorGUILayout.HelpBox("At least one Physic Material must be assigned first.", MessageType.Warning); }
                        EditorGUILayout.LabelField("Custom Physic Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                        for (int i = 0; i < customPhysMat.arraySize; i++)
                        {
                            SerializedProperty LS_ref = customPhysMat.GetArrayElementAtIndex(i);
                            EditorGUILayout.BeginHorizontal("box");
                            LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("", LS_ref.objectReferenceValue, typeof(PhysicMaterial), false);
                            if (GUILayout.Button(new GUIContent("X", "Remove this Physic Material"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.customPhysMat.RemoveAt(i); }
                            EditorGUILayout.EndHorizontal();
                        }


                        if (GUILayout.Button(new GUIContent("Add new Physic Material entry", "Add new Physic Material entry"))) { t.dynamicFootstep.customPhysMat.Add(null); }
                        GUI.enabled = t.dynamicFootstep.customPhysMat.Any();
                    }

                    else
                    {
                        if (!t.dynamicFootstep.customMat.Any()) { EditorGUILayout.HelpBox("At least one Material must be assigned first.", MessageType.Warning); }
                        EditorGUILayout.LabelField("Custom Materials", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                        for (int i = 0; i < customMat.arraySize; i++)
                        {
                            SerializedProperty LS_ref = customMat.GetArrayElementAtIndex(i);
                            EditorGUILayout.BeginHorizontal("box");
                            LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("", LS_ref.objectReferenceValue, typeof(Material), false);
                            if (GUILayout.Button(new GUIContent("X", "Remove this Material"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.customMat.RemoveAt(i); }
                            EditorGUILayout.EndHorizontal();
                        }
                        if (GUILayout.Button(new GUIContent("Add new Material entry", "Add new Material entry"))) { t.dynamicFootstep.customMat.Add(null); }
                        GUI.enabled = t.dynamicFootstep.customMat.Any();
                    }

                    EditorGUILayout.LabelField("Custom Audio Clips", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                    for (int i = 0; i < customFS.arraySize; i++)
                    {
                        SerializedProperty LS_ref = customFS.GetArrayElementAtIndex(i);
                        EditorGUILayout.BeginHorizontal("box");
                        LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("Clip " + (i + 1) + ":", LS_ref.objectReferenceValue, typeof(AudioClip), false);
                        if (GUILayout.Button(new GUIContent("X", "Remove this clip"), GUILayout.MaxWidth(20))) { t.dynamicFootstep.customClipSet.RemoveAt(i); }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button(new GUIContent("Add Clip", "Add new clip entry"))) { t.dynamicFootstep.customClipSet.Add(null); }
                    if (GUILayout.Button(new GUIContent("Remove All Clips", "Remove all clip entries"))) { t.dynamicFootstep.customClipSet.Clear(); }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    DropAreaGUI(t.dynamicFootstep.customClipSet, GUILayoutUtility.GetLastRect());
                }
                GUI.enabled = true;

                EditorGUILayout.Space();
                #endregion
                #region Fallback Section
                showStaticFS = EditorGUILayout.Foldout(showStaticFS, new GUIContent("Fallback Footstep Clips", "Audio clips available as footsteps in case a collider with an unrecognized/null Physic Material is walked on."));
                if (showStaticFS)
                {
                    GUILayout.BeginVertical("box");
                    for (int i = 0; i < staticFS.arraySize; i++)
                    {
                        SerializedProperty LS_ref = staticFS.GetArrayElementAtIndex(i);
                        EditorGUILayout.BeginHorizontal("box");
                        LS_ref.objectReferenceValue = EditorGUILayout.ObjectField("Clip " + (i + 1) + ":", LS_ref.objectReferenceValue, typeof(AudioClip), false);
                        if (GUILayout.Button(new GUIContent("X", "Remove this clip"), GUILayout.MaxWidth(20))) { this.t.footStepSounds.RemoveAt(i); }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button(new GUIContent("Add Clip", "Add new clip entry"))) { this.t.footStepSounds.Add(null); }
                    if (GUILayout.Button(new GUIContent("Remove All Clips", "Remove all clip entries"))) { this.t.footStepSounds.Clear(); }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    DropAreaGUI(t.footStepSounds, GUILayoutUtility.GetLastRect());
                }

                #endregion
            }
            #endregion

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(t);
                Undo.RecordObject(t, "FPAIO Change");
                SerT.ApplyModifiedProperties();
            }
        }
        private void DropAreaGUI(List<AudioClip> clipList, Rect dropArea)
        {
            var evt = Event.current;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition)) { break; }
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            var drago = draggedObject as AudioClip;
                            if (!drago) { continue; }
                            clipList.Add(drago);
                        }
                    }
                    Event.current.Use();
                    EditorUtility.SetDirty(t);
                    SerT.ApplyModifiedProperties();
                    break;
            }
        }
    }
#endif





}

