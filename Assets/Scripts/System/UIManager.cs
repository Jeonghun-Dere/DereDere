using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.DEREDERE.System {
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance {get; private set;}
        [SerializeField]
        Texture2D crosshair;
        [SerializeField]
        Text weight, kill;
        [SerializeField]
        Slider hp;

        void Awake() {
            Instance = this;

            CrossHair();
        }

        void Update() {
            if (Player.Local != null) {
                weight.text = Player.Local.weight.ToString();
                kill.text = Player.Local.kill.ToString();

                hp.value = (float)Player.Local.health / Player.Local.maxHealth;
            }
        }

        void CrossHair() {
            Cursor.SetCursor(crosshair, Vector2.zero, CursorMode.Auto);
        }

        public Texture2D ScaleTexture( Texture2D source, float _scaleFactor)
        {
            if (_scaleFactor == 1f)
            {
                return source;
            }
            else if (_scaleFactor == 0f)
            {
                return Texture2D.blackTexture;
            }

            int _newWidth = Mathf.RoundToInt(source.width * _scaleFactor);
            int _newHeight = Mathf.RoundToInt(source.height * _scaleFactor);


            
            Color[] _scaledTexPixels = new Color[_newWidth * _newHeight];

            for (int _yCord = 0; _yCord < _newHeight; _yCord++)
            {
                float _vCord = _yCord / (_newHeight * 1f);
                int _scanLineIndex = _yCord * _newWidth;

                for (int _xCord = 0; _xCord < _newWidth; _xCord++)
                {
                    float _uCord = _xCord / (_newWidth * 1f);

                    _scaledTexPixels[_scanLineIndex + _xCord] = source.GetPixelBilinear(_uCord, _vCord);
                }
            }

            // Create Scaled Texture
            Texture2D result = new Texture2D(_newWidth, _newHeight, source.format, false);
            result.SetPixels(_scaledTexPixels, 0);
            result.Apply();

            return result;
        }
    }
}
