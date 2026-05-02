using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace WoolenSwing.Management
{
    public enum GameState { Menu, Playing, GameOver, Won }
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public GameState CurrentState { get; private set; }
        [Header("Cameras")]
        [SerializeField] private GameObject menuCamera;
        [SerializeField] private GameObject playerCamera;

        [Header("Player and Cat")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform catTransform;

        [Header("Post Processing")]
        [SerializeField] private Volume menuVolume;
        [SerializeField] private float volumeFadeDuration = 2.0f;

        [Header("UI Transition")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private GameObject mainMenuParent;
        [SerializeField] private CanvasGroup winWindowCanvasGroup;
        [SerializeField] private float fadeDuration = 1.0f;

        private Vector3 _startPosition;
        private Vector3 _catStartPosition;


        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (playerTransform) _startPosition = playerTransform.position;
            if (catTransform) _catStartPosition = catTransform.position;
        }

        void Start()
        {
            SetState(GameState.Menu);
            PrepareMenu();
            if (fadeCanvasGroup)
            {
                fadeCanvasGroup.alpha = 1f;
                StartCoroutine(Fade(0f)); // black screen fade out at start
            }
        }

        private void PrepareMenu()
        {
            menuCamera.SetActive(true);
            playerCamera.SetActive(false);
            mainMenuParent.SetActive(true);
            if (winWindowCanvasGroup) winWindowCanvasGroup.alpha = 0;
            if (menuVolume) menuVolume.weight = 1f;
            //hide the mouse 
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        private IEnumerator FadeVolumeWeight(float targetWeight)
        {
            float startWeight = menuVolume.weight;
            float timer = 0;
            while (timer < volumeFadeDuration)
            {
                timer += Time.deltaTime;
                menuVolume.weight = Mathf.Lerp(startWeight, targetWeight, timer / volumeFadeDuration);
                yield return null;
            }
            menuVolume.weight = targetWeight;
        }
        private void SetState(GameState newState)
        {
            CurrentState = newState;
            Debug.Log($"Game State Changed to: {newState}");
        }
        public void StartGame()
        {
            if (CurrentState != GameState.Menu) return;
            SetState(GameState.Playing);
            mainMenuParent.SetActive(false);
            menuCamera.SetActive(false);
            playerCamera.SetActive(true);

            if (menuVolume) StartCoroutine(FadeVolumeWeight(0f));

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void GameOver()
        {
            if (CurrentState != GameState.Playing) return;

            SetState(GameState.GameOver);
            StartCoroutine(RespawnSequence());
        }

        private IEnumerator RespawnSequence() //this is a simple respawn sequence with a fade to black and back
        {
            yield return StartCoroutine(Fade(1f));

            playerTransform.position = _startPosition;
            catTransform.position = _catStartPosition;

            var rb = playerTransform.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(Fade(0f));

            if (menuVolume) menuVolume.weight = 0f;
            SetState(GameState.Playing);
        }

        public void WinGame()
        {
            if (CurrentState != GameState.Playing) return;
            SetState(GameState.Won);
            StartCoroutine(WinSequence());
        }
        public void ReplayGame()
        {
            Time.timeScale = 1f;
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
        private IEnumerator WinSequence()
        {

            Debug.Log("Victory!");

            winWindowCanvasGroup.interactable = true;
            winWindowCanvasGroup.blocksRaycasts = true;

            float timer = 0;
            while (timer < 1.0f)
            {
                timer += Time.deltaTime;
                winWindowCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / 1.0f);
                yield return null;
            }
            winWindowCanvasGroup.alpha = 1;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        private IEnumerator Fade(float targetAlpha)
        {
            if (!fadeCanvasGroup) yield break;
            float startAlpha = fadeCanvasGroup.alpha;
            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = targetAlpha;
        }
    }
}