using Interop.UIAutomationClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TransparentWindow))]
public class flower : MonoBehaviour
{
    private TransparentWindow _transparentWindow;
    private string _fullPath;
    [SerializeField] private RectTransform _flower;

    private void Awake()
    {
        _transparentWindow = GetComponent<TransparentWindow>();
        InvokeRepeating(nameof(UpdateFlowerPos),60,0.1f);
    }
    private void UpdateFlowerPos()
    {
        var elementArray = _transparentWindow.GetTaskbarElementArray();
        IUIAutomationElement element = null;
        if (elementArray != null)
        {
            int nNbItems = elementArray.Length;
            for (int nItem = 0; nItem <= nNbItems - 1; nItem++)
            {
                var foundElement = elementArray.GetElement(nItem);
                if (foundElement.CurrentName.Contains("flower"))
                {
                    element = foundElement;
                    break;
                }
            }
        }

        var automation = new CUIAutomation();
        var mainWndElement = automation.ElementFromHandle(_transparentWindow.TaskbarHandle); // Replace with actual handle






        if (element != null)
        {
            var sName = element.CurrentName;
            var sAutomationId = element.CurrentAutomationId;
            var rect = element.CurrentBoundingRectangle;
            UnityEngine.Debug.Log(String.Format("\tName : {0} - AutomationId : {1}  - Rect({2}, {3}, {4}, {5})", sName, sAutomationId, rect.left, rect.top, rect.right, rect.bottom));
            _flower.position = new Vector3(rect.left, Mathf.Clamp(Screen.height - rect.top, 0, Screen.height), 0);
            _flower.sizeDelta = new Vector2(rect.right - rect.left, rect.bottom - rect.top);
        }
    }
}