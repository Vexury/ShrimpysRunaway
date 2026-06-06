using UnityEngine;

public class PlayerMovementAudio : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;

    [Header("Footstep Settings")]
    [SerializeField] private float footstepVolume = 2.0f;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.3f;
    [SerializeField] private float pitchVariationMin = 0.95f;
    [SerializeField] private float pitchVariationMax = 1.05f;

    [Header("Default Audio")]
    [SerializeField] private AudioClip[] defaultFootsteps;
    [SerializeField] private AudioClip[] defaultJumps;
    [SerializeField] private AudioClip[] defaultLands;

    [Header("Tag-Based Audio")]
    [SerializeField] private TagAudioSet[] tagAudio;

    private float stepTimer;
    private bool wasGrounded = true;
    private float minLandingVelocity = -2f;

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    [System.Serializable]
    public class TagAudioSet
    {
        public string tag = "Untagged";
        public AudioClip[] footsteps;
        public AudioClip[] jumps;
        public AudioClip[] lands;
        [Range(0f, 25f)] public float footstepVolume = 1f;
        [Range(0f, 5f)] public float jumpVolume = 1f;
        [Range(0f, 5f)] public float landVolume = 1f;
    }

    private void Update()
    {
        HandleFootsteps();
        HandleLanding();
    }

    private void HandleFootsteps()
    {
        bool isMoving = characterController.velocity.sqrMagnitude > 0.1f;
        bool isGrounded = characterController.isGrounded;

        if (isMoving && isGrounded)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                PlayFootstepSound();

                float speed = characterController.velocity.magnitude;
                stepTimer = speed > 6f ? sprintStepInterval : walkStepInterval;
            }
        }
        else
        {
            stepTimer = 0;
        }
    }

    private void HandleLanding()
    {
        bool isGrounded = characterController.isGrounded;

        if (isGrounded && !wasGrounded && characterController.velocity.y < minLandingVelocity)
        {
            PlayLandSound();
        }

        wasGrounded = isGrounded;
    }

    public void PlayJumpSound()
    {
        (AudioClip[] clips, float volumeMult) = GetAudioClipsWithVolume(ClipType.Jump);
        PlayRandomClip(clips, volumeMult);
    }

    private void PlayFootstepSound()
    {
        (AudioClip[] clips, float volumeMult) = GetAudioClipsWithVolume(ClipType.Footstep);
        PlayRandomClip(clips, footstepVolume * volumeMult);
    }

    private void PlayLandSound()
    {
        (AudioClip[] clips, float volumeMult) = GetAudioClipsWithVolume(ClipType.Land);
        PlayRandomClip(clips, volumeMult);
    }

    private enum ClipType { Footstep, Jump, Land }

    private (AudioClip[], float) GetAudioClipsWithVolume(ClipType clipType)
    {
        string groundTag = DetectGroundTag();

        // Try to find matching tag audio
        foreach (var audioSet in tagAudio)
        {
            if (audioSet.tag == groundTag)
            {
                AudioClip[] clips = clipType switch
                {
                    ClipType.Footstep => audioSet.footsteps,
                    ClipType.Jump => audioSet.jumps,
                    ClipType.Land => audioSet.lands,
                    _ => null
                };

                if (clips != null && clips.Length > 0)
                {
                    float volumeMult = clipType switch
                    {
                        ClipType.Footstep => audioSet.footstepVolume,
                        ClipType.Jump => audioSet.jumpVolume,
                        ClipType.Land => audioSet.landVolume,
                        _ => 1f
                    };
                    return (clips, volumeMult);
                }
            }
        }

        // Return defaults with multiplier of 1
        AudioClip[] defaultClips = clipType switch
        {
            ClipType.Footstep => defaultFootsteps,
            ClipType.Jump => defaultJumps,
            ClipType.Land => defaultLands,
            _ => defaultFootsteps
        };

        return (defaultClips, 1f);
    }

    private string DetectGroundTag()
    {
        // Raycast down from player position
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            return hit.collider.tag;
        }

        return "Untagged";
    }

    private void PlayRandomClip(AudioClip[] clips, float volumeScale = 1f)
    {
        if (clips != null && clips.Length > 0)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            AudioManager.Instance.PlaySFXWithPitchVariation(clip, pitchVariationMin, pitchVariationMax, volumeScale);
        }
    }
}