using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompatibilityManager : MonoBehaviour
{
    private RectTransform[] _uiElements;
    private float _scaleFactor = 0.65f;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        Vector3 temp = new Vector3();
        /*foreach (Text text in GameObject.FindObjectsOfType<Text>())
            text.fontSize = 10;*/

        //Get the 4 UI elements for tutorial and scale them down in case of mobile
        _uiElements = new RectTransform[4];
        for (int i = 0; i < _uiElements.Length; i++)
        {
            _uiElements[i] = transform.GetChild(i+2).gameObject.GetComponent<RectTransform>();
            temp.Set(_scaleFactor + 0.15f, _scaleFactor, 1f);
            _uiElements[i].localScale = temp;
            //_uiElements[i].localScale = new Vector3(_scaleFactor+0.15f, _scaleFactor, 1f);
            //Debug.Log(_uiElements[i].localScale);
        }
        /*_uiElements[0].position = new Vector3(_uiElements[0].position.x, _uiElements[0].position.y + 25.0f, 0f);
        _uiElements[1].position = new Vector3(_uiElements[1].position.x, _uiElements[1].position.y - 50.0f, 0f);
        _uiElements[2].position = new Vector3(_uiElements[2].position.x, _uiElements[2].position.y + 25.0f, 0f);
        _uiElements[3].position = new Vector3(_uiElements[3].position.x, _uiElements[3].position.y - 50.0f, 0f);*/
        temp.Set(_uiElements[0].position.x, _uiElements[0].position.y + 25.0f, 0f);
        _uiElements[0].position = temp;
        temp.Set(_uiElements[1].position.x, _uiElements[1].position.y - 50.0f, 0f);
        _uiElements[1].position = temp;
        temp.Set(_uiElements[2].position.x, _uiElements[2].position.y + 25.0f, 0f);
        _uiElements[2].position = temp;
        temp.Set(_uiElements[3].position.x, _uiElements[3].position.y - 50.0f, 0f);
        _uiElements[3].position = temp;
#endif
    }
}
