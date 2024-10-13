using System.Runtime.InteropServices;
using System.Windows.Input;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PromptBrancher : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;

    [SerializeField] private string _originalText;

    [SerializeField] private float timeBetwenLetters = 0.1f;

    private float _timeAtLastLeter = 0f;

    private int _currentLetter = 0;

    [SerializeField] private GameObject Buttons;

    [SerializeField] private GameObject BranchAPrefab;
    [SerializeField] private GameObject BranchBPrefab;

    //[DllImport("User32.Dll")]
    //public static extern long SetCursorPos(int x, int y);

    private void Awake()
    {
        _textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        _originalText = _textMeshProUGUI.text;
        _textMeshProUGUI.text = "";
        Buttons.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (_currentLetter < _originalText.Length)
        {
            if (Time.time - _timeAtLastLeter > timeBetwenLetters)
            {
                _textMeshProUGUI.text += _originalText[_currentLetter];
                _currentLetter++;
                _timeAtLastLeter = Time.time;
            }
        }
        else
        {
            Buttons.SetActive(true);
        }
        //Debug.Log(Input.mousePosition);
    }

    public void BranchA()
    {
        Instantiate(BranchAPrefab, transform.parent);
        Destroy(gameObject);
    }

    public void BranchB()
    {
        Instantiate(BranchBPrefab, transform.parent);
        Destroy(gameObject);
    }

    public void Finish()
    {
        FindFirstObjectByType<TransparentWindow>().SetClickthrough(true);
        Destroy(transform.parent.gameObject);

    }
    public void ExitProgram()
    {
        Application.Quit();
    }
}
