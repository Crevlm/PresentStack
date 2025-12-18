using UnityEngine;
using FMODUnity;
using FMOD.Studio;

[RequireComponent(typeof(Rigidbody))]
public class PresentImpactAudio : MonoBehaviour
{
    [Header("FMOD")] //set FMOD event and parameter names in inspector
    [SerializeField] private EventReference impactEvent;
    [SerializeField] private string intensityParameter = "ImpactIntensity";

    [Header("Ground Detection")] //set what is ground in inspector
    [SerializeField] private LayerMask groundLayers;

    [Header("Intensity Mapping")]// set min/max fall times and intensities in inspector
    [SerializeField] private float minFallTime = 0.1f;
    [SerializeField] private float maxFallTime = 2.5f;
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 1f;

    private Rigidbody rb;
    private bool isGrounded = true;
    private bool hasImpacted = false;// to prevent multiple impact sounds on one landing
    private float fallStartTime;// time when the present started falling to increase impact intensity

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    { // Ignore collisions that are not with ground layers
        if (!IsInGroundLayer(collision.gameObject.layer))
            return;
        // If we've already fired an impact for this fall, do nothing
        if (hasImpacted)
            return;

        isGrounded = true;
        hasImpacted = true;
        // Calculate how long the present was falling
        float fallDuration = Time.time - fallStartTime;
        // Normalize fall duration into a 0–1 range
        float normalized = Mathf.InverseLerp(minFallTime, maxFallTime, fallDuration);
        // Map normalized value to intensity range
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, normalized);
        // Play the FMOD impact sound with the calculated intensity
        PlayImpact(intensity);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!IsInGroundLayer(collision.gameObject.layer))
            return;
        //reset fall tracking variables
        isGrounded = false;
        hasImpacted = false;
        fallStartTime = Time.time;
    }

    private bool IsInGroundLayer(int layer)
    {
        return (groundLayers.value & (1 << layer)) != 0;
    }

    private void PlayImpact(float intensity)
    {
        if (impactEvent.IsNull)
            return;

        EventInstance instance = RuntimeManager.CreateInstance(impactEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        instance.setParameterByName(intensityParameter, intensity);
        instance.start();
        instance.release();
    }
   
    /// Called when the player releases the present.
    /// Resets fall timing so held airtime does not count.
    public void NotifyDropped()
    {
        hasImpacted = false;
        isGrounded = false;
        fallStartTime = Time.time;
    }

}
