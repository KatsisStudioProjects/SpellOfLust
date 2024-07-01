using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SpellOfLust
{
    public class MineButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private Color _normalColor, _hoverColor, _disabledColor, _flaggedColor, _validatedColor;
        [SerializeField]
        private Sprite _disabledSprite, _flagSprite, _mineSprite;
        private Sprite _normalSprite;
        public UnityEvent OnLeftClick { private set; get; } = new();
        public UnityEvent OnRightClick { private set; get; } = new();

        private Image _image;

        private bool _interactable = true;
        private bool _flagged;
        public void Disable()
        {
            _interactable = false;
            _image.color = _disabledColor;
            _image.sprite = _disabledSprite;
        }

        public void ShowMine()
        {
            _interactable = false;
            _image.sprite = _mineSprite;
            _image.color = _flaggedColor;
        }

        private bool CanInteract => _interactable && !Flagged;

        public bool Flagged
        {
            set
            {
                _flagged = value;
                if (value)
                {
                    _image.color = _flaggedColor;
                    _image.sprite = _flagSprite;
                }
                else
                {
                    _image.color = IsHovered ? _hoverColor : _normalColor;
                    _image.sprite = _normalSprite;
                }
            }
            get => _flagged;
        }

        public bool IsHovered { private set; get; }

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.color = _normalColor;
            _normalSprite = _image.sprite;
        }

        public void Validate()
        {
            _image.color = _validatedColor;
        }

        public void MainClick()
        {
            if (!Flagged)
            {
                OnLeftClick?.Invoke();
            }
        }

        public void AltClick()
        {
            OnRightClick?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_interactable)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    MainClick();
                }
                else if (eventData.button == PointerEventData.InputButton.Right)
                {
                    AltClick();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHovered = true;
            if (CanInteract)
            {
                _image.color = _hoverColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsHovered = false;
            if (CanInteract)
            {
                _image.color = _normalColor;
            }
        }
    }
}
