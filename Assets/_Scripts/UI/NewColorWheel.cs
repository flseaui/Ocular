using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game;
using Player;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class NewColorWheel : MonoBehaviour
{
    public static bool Turning;

    private bool _disabled;
    private bool _bufferLeft;
    private bool _bufferRight;

    [SerializeField] private GameObject _gameManager;
    private GlassesController _glassesController;

    [SerializeField] private List<Sprite> _wheelSprites;

    private Image _wheelImage;

    [ShowInInspector]
    private int _spriteIndex;
    
    private void Awake()
    {
        _glassesController = _gameManager.GetComponent<GlassesController>();
        _wheelImage = GetComponent<Image>();
    }

    void Update()
    {
        var left = Input.GetKeyDown(KeyCode.Q);
        var right = Input.GetKeyDown(KeyCode.E);
        
        if (Turning && Input.GetKeyDown(KeyCode.Q))
        {
            _bufferLeft = true;
            _bufferRight = false;
        }

        if (Turning && Input.GetKeyDown(KeyCode.E))
        {
            _bufferRight = true;
            _bufferLeft = false;
        }

        if (left && right)
        {
            left = false;
            right = false;
        }

        if (Pathfinder.Navigating || Player.Player.Falling)
        {
            if (!_disabled)
            {
                GetComponent<Image>().DOColor(new Color(130, 130, 130, 114), .1f);
                _disabled = true;
            }
        }
        else
        {
            if (_disabled)
            {
                GetComponent<Image>().DOColor(new Color(255, 255, 255, 255), .1f);
                _disabled = false;
            }
            if (!Turning)
            {
                if (left || _bufferLeft)
                {
                    Turning = true;
                    _bufferLeft = false;
                    _glassesController.index++;
                    _glassesController.UpdateOcularState();
                    _spriteIndex++;
                    if (_spriteIndex > _wheelSprites.Count - 1)
                        _spriteIndex = 0;
                    _wheelImage.sprite = _wheelSprites[_spriteIndex];
                    var rotate = transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, 0, 60),
                        .3f);
                    rotate.onComplete += () => Turning = false;
                }

                if (right || _bufferRight)
                {
                    Turning = true;
                    _bufferRight = false;
                    _glassesController.index--;
                    _glassesController.UpdateOcularState();
                    _spriteIndex--;
                    if (_spriteIndex < 0)
                        _spriteIndex = _wheelSprites.Count - 1;
                    _wheelImage.sprite = _wheelSprites[_spriteIndex];
                    var rotate = transform.DOLocalRotate(transform.localRotation.eulerAngles + new Vector3(0, 0, -60),
                        .3f);
                    rotate.onComplete += () => Turning = false;
                }
            }
        }
    }
}
