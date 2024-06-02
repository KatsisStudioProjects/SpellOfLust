using Ink.Runtime;
using SpellOfLust.VN;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sketch.VN
{
    public class VNManager : MonoBehaviour
    {
        public static VNManager Instance { private set; get; }

        [SerializeField]
        private TextDisplay _display;

        [SerializeField]
        private TextAsset _intro;

        [SerializeField]
        private Image _imageContainer;

        [SerializeField]
        private Sprite[] _images;
        private int _imageIndex;

        private Story _story;

        private bool _isSkipEnabled;
        private float _skipTimer;
        private readonly float _skipTimerRef = .1f;

        private void Awake()
        {
            Instance = this;
            ShowStory(_intro);
        }

        private void Update()
        {
            if (_isSkipEnabled)
            {
                _skipTimer -= Time.deltaTime;
                if (_skipTimer < 0)
                {
                    _skipTimer = _skipTimerRef;
                    DisplayNextDialogue();
                }
            }
        }

        public void ShowImage()
        {
            _imageContainer.sprite = _images[_imageIndex];
        }

        private void ResetVN()
        {
            _isSkipEnabled = false;
        }

        public void ShowStory(TextAsset asset)
        {
            _story = new(asset.text);
            ResetVN();
            DisplayStory(_story.Continue());
            ShowImage();
        }

        private void DisplayStory(string text)
        {
            _display.ToDisplay = text;
        }

        public void DisplayNextDialogue()
        {
            if (!_display.IsDisplayDone)
            {
                // We are slowly displaying a text, force the whole display
                _display.ForceDisplay();
            }
            else if (_story.canContinue && // There is text left to write
                !_story.currentChoices.Any()) // We are not currently in a choice
            {
                DisplayStory(_story.Continue());
                _imageIndex++;
                ShowImage();
            }
            else if (!_story.canContinue && !_story.currentChoices.Any())
            {
                SceneManager.LoadScene("Main");
            }
        }

        public void ToggleSkip()
        {
            _isSkipEnabled = !_isSkipEnabled;
        }

        public void OnNextDialogue(InputAction.CallbackContext value)
        {
            if (value.performed && !_isSkipEnabled)
            {
                ResetVN();
                DisplayNextDialogue();
            }
        }

        public void OnSkip(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started)
            {
                _isSkipEnabled = true;
            }
            else if (value.phase == InputActionPhase.Canceled)
            {
                _isSkipEnabled = false;
            }
        }
    }
}