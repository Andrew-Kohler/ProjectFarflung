using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Handles auto interact effects of looking at respawn auto interactable, which triggers respawn sequence.
/// </summary>
public class RespawnAutoInteractable : MonoBehaviour
{
    [Header("Animation Sequence")]
    [SerializeField, Tooltip("The camera aimed down on the player from above")]
    private CinemachineVirtualCamera _visorCam1;
    [SerializeField, Tooltip("The camera aimed into the visor")]
    private CinemachineVirtualCamera _visorCam2;
    [SerializeField, Tooltip("Handles respawn player model animations.")]
    private Animator _playerAnim;
    [SerializeField, Tooltip("Handles fade to black and vitals display animations")]
    private Animator _cameraAnim;

    private void Awake()
    {
        // prevent player anim from playing until we need it
        _playerAnim.speed = 0;
    }

    public void InteractEffects()
    {
        StartCoroutine(DoInteractEffects());
    }

    public IEnumerator DoInteractEffects()
    {
        // player loses control
        GameManager.Instance.PlayerEnabled = false;

        // pan camera into visor
        CinemachineVirtualCamera mainCam = Camera.main.GetComponent<CinemachineBrain>().
            ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        _visorCam1.gameObject.SetActive(true);
        mainCam.gameObject.SetActive(false);

        // allow player animation to play
        _playerAnim.speed = 1;

        // wait for camera to be for sure above the player
        yield return new WaitUntil(() => Vector3.Distance(Camera.main.transform.position, _visorCam1.transform.position) < 0.1f);

        _visorCam1.gameObject.SetActive(false);
        _visorCam2.gameObject.SetActive(true);
        
        // wait for camera to be for sure within the visor
        yield return new WaitForSeconds(1.5f);
        
        // activate respawn animator
        _cameraAnim.SetTrigger("Activate");

        // scene transition handled through animator event trigger... nothing here        
    }
}
