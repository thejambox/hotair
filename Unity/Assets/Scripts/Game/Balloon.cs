using UnityEngine;
using System.Collections;

public class Balloon : MonoBehaviour
{
    private enum LiftMode
    {
        Hold,
        Ascend,
        Descend
    }

    public Transform tCam;
    public Transform tBalloon;
    public Transform tBasket;
    public Transform tModel;

    public BalloonFire balloonFire;

    public BoxCollider worldConstraint;

    public AudioSource sndMusic;
    public AudioSource sndFire;
    public AudioSource sndThud;

    public float[] altitudes;

    public float groundDistance { get; private set; }

    private Transform cachedTransform;
    private Rigidbody cachedRigidbody;

    private float GROUND_DIST_MAX = 25f;
    private float GROUND_DIST_MIN = 1f;

    private float FORCE_GRAVITY = -9.81f;
    private float FORCE_LIFT = 15f;

    private float ERROR_MARGIN = 0.05f;

    private LiftMode mode;
    private int altitudeLevel;

    private float holdTimer;
    private int holdDirection;

    private float overallForce;

    private float lastScreenWidth = -1f;

    private void Start()
    {
        cachedTransform = transform;
        cachedRigidbody = rigidbody;

        mode = LiftMode.Hold;
        altitudeLevel = 0;

        holdTimer = 0f;
        holdDirection = 0;

        overallForce = 0f;

        Wind.Affect(cachedRigidbody);

        // place the balloon randomly within the bounds
        Bounds worldBounds = worldConstraint.bounds;
        Vector3 rndPoint = Vector3.zero;

        rndPoint.x = Mathf.Lerp(worldBounds.min.x, worldBounds.max.x, Random.Range(0f, 1f));
        rndPoint.y = Mathf.Lerp(15f, 20f, Random.Range(0f, 1f));
        rndPoint.z = Mathf.Lerp(worldBounds.min.z, worldBounds.max.z, Random.Range(0f, 1f));

        cachedTransform.position = rndPoint;

        sndFire.volume = 0f;
    }

    private void Update()
    {
        UpdateGroundDist();
        UpdateCamera();
        UpdateMusic();

        ControlFire();
        ControlTilt();

        if (lastScreenWidth != Screen.width)
            ResetCam();
    }

    private void LateUpdate()
    {
        if (worldConstraint != null && !worldConstraint.bounds.Contains(cachedTransform.position))
            cachedTransform.position = worldConstraint.ClosestPointOnBounds(cachedTransform.position);
    }

    private void OnCollisionEnter()
    {
        sndThud.Play();
    }

    private void FixedUpdate()
    {
        UpdateLift();
    }

    private void ControlFire()
    {
        balloonFire.isLit = overallForce > 0;
    }

    private void ControlTilt()
    {
        float velX = Mathf.Clamp(cachedRigidbody.velocity.x, -8f, 8f);
        float velZ = Mathf.Clamp(cachedRigidbody.velocity.z, -8f, 8f);

        Vector3 tilt = new Vector3(velZ, 0, -velX);

        tModel.localRotation = Quaternion.Lerp(tModel.localRotation, Quaternion.Euler(tilt), Time.deltaTime);
    }

    private void UpdateGroundDist()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(tBasket.position, Vector3.down, out hitInfo, GROUND_DIST_MAX, Layers.MaskAllBut(Layers.Player)))
        {
            groundDistance = hitInfo.distance;
        }
        else
        {
            groundDistance = GROUND_DIST_MAX;
        }
    }

    private void UpdateCamera()
    {
        Vector3 pos = Vector3.zero;

        // come close to the balloon depending on how close to the ground it is
        float zoom = Mathf.Clamp(groundDistance - GROUND_DIST_MIN, 0f, GROUND_DIST_MAX) / GROUND_DIST_MAX;

        pos.z = Mathf.Lerp(-75f, -325f, zoom);

        float pan = 0f;

        if (groundDistance >= GROUND_DIST_MAX)
        {
            // move up or down depending on opposite of the y velocity. (move down if the velocity is positive)
            pan = cachedRigidbody.velocity.y;

            // we only care up until moving 20mps in each direction
            pan = Mathf.Clamp(pan, -20f, 20f) / 20f;

            // normalize between 0..1, and flip it so going down is looking down
            pan = 1f - ((pan + 1f) / 2f);

            pan = Mathf.Lerp(-15f, 15f, pan);
        }
        else
        {
            pan = 15f;
        }

        // the y of the basket is important, we don't want the camera going below that ever, because 
        pos.y = pan;

        // always look at the center of the balloon
        tCam.localPosition = Vector3.Slerp(tCam.localPosition, pos, Time.deltaTime * 2f);
        tCam.forward = cachedTransform.position - tCam.position;
    }

    float desiredVolume;
    private void UpdateMusic()
    {
        desiredVolume = 0f;

        if (groundDistance < GROUND_DIST_MIN)
        {
            if (sndMusic.isPlaying)
                sndMusic.Pause();

            sndMusic.volume = 0f;
        }
        else if (groundDistance < GROUND_DIST_MAX)
        {
            if (!sndMusic.isPlaying && overallForce > 0)
                sndMusic.Play();

            desiredVolume = Mathf.Max(0, ((groundDistance - GROUND_DIST_MIN) / (GROUND_DIST_MAX - GROUND_DIST_MIN)));
        }
        else
        {
            desiredVolume = 1f;
        }

        desiredVolume *= 0.5f; // max volume

        sndMusic.volume = Mathf.Lerp(sndMusic.volume, desiredVolume, Time.deltaTime * 0.25f);

        // fire
        sndFire.volume = balloonFire.isLit ? Mathf.Lerp(sndFire.volume, 0.25f, Time.deltaTime * 4f) : Mathf.Lerp(sndFire.volume, 0f, Time.deltaTime * 8f);
    }

    private void UpdateLift()
    {
        float desiredHeight = altitudes[altitudeLevel];
        float height = cachedTransform.position.y;
        float distance = desiredHeight - height;
        bool belowRange = height < (1f - ERROR_MARGIN) * desiredHeight;
        bool aboveRange = height > (1f + ERROR_MARGIN) * desiredHeight;
        bool withinRange = !belowRange && !aboveRange;

        overallForce = FORCE_GRAVITY;
        
        if (mode == LiftMode.Hold)
        {
            if (altitudeLevel == 0 && groundDistance <= 2f)
            {
                // do nothing. 
            }
            else if (withinRange)
            {
                if (holdTimer <= 0f)
                {
                    holdTimer = Random.Range(0.25f, 1.25f);
                    holdDirection = Random.Range(0f, 1f) <= 0.5f ? -1 : 1;
                }

                holdTimer -= Time.deltaTime;

                overallForce = 0.1f * -FORCE_GRAVITY * holdDirection;
            }
            else if (belowRange)
            {
                overallForce += Mathf.Clamp01(distance / 3) * FORCE_LIFT;
            }
            else if (aboveRange)
            {
                overallForce += -FORCE_GRAVITY * 0.5f;
            }
        }
        else if (mode == LiftMode.Ascend)
        {
            if (withinRange)
            {
                mode = LiftMode.Hold;
            }
            else
            {
                overallForce += Mathf.Clamp01(distance/5) * FORCE_LIFT;
            }
        }
        else if (mode == LiftMode.Descend)
        {
            if (withinRange)
            {
                mode = LiftMode.Hold;
            }
        }

        cachedRigidbody.AddForce(Vector3.up * overallForce);
    }

    public void Lift()
    {
        if (altitudeLevel == altitudes.Length - 1)
            return;

        ++altitudeLevel;

        mode = LiftMode.Ascend;

        holdTimer = 0f;
    }

    public void Descend()
    {
        if (altitudeLevel == 0)
            return;

        --altitudeLevel;

        mode = LiftMode.Descend;

        holdTimer = 0f;
    }

    private void ResetCam()
    {
        Rect guiRect = tCam.camera.rect;

        guiRect.width = (96f / Screen.width);

        tCam.camera.rect = guiRect;
    }
}
