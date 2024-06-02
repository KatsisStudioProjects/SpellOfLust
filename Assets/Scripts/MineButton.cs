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

        public void ShowMine(bool isGood)
        {
            _interactable = false;
            _image.sprite = _mineSprite;
            _image.color = isGood ? _validatedColor : _flaggedColor;
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
                    _image.color = _isHovered ? _hoverColor : _normalColor;
                    _image.sprite = _normalSprite;
                }
            }
            get => _flagged;
        }

        private bool _isHovered;

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

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_interactable)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    if (!Flagged)
                    {
                        OnLeftClick?.Invoke();
                    }
                }
                else if (eventData.button == PointerEventData.InputButton.Right)
                {
                    OnRightClick?.Invoke();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            if (CanInteract)
            {
                _image.color = _hoverColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            if (CanInteract)
            {
                _image.color = _normalColor;
            }
        }
    }
}
