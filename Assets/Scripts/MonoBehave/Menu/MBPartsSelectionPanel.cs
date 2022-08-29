using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBPartsSelectionPanel : MonoBehaviour
{
    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private GameObject rowTemplate;
    private float RowHeight => rowTemplate.GetComponent<RectTransform>().sizeDelta.y;

    private readonly List<GameObject> rows = new();

    void Start() {
        rows.Add(rowTemplate);
        List<CoreUnitBase> cores = SaveManager.Get().CoreUnitDatabase.Parts;

        int numRows = cores.Count / 3;
        numRows += (cores.Count % 3 > 0) ? 1 : 0;

        SetContentHeight(numRows);

        if (numRows > 1) {
            AddRows(numRows - 1);
        }
        for (int row = 0; row < numRows; row++) {
            for (int col = 0; col < 3; col++) {
                int coreIndex = row * 3 + col;
                // Get the part button
                if (coreIndex < cores.Count) {
                    // Set the text
                    // Enable the button
                } else {
                    // Disable the button
                }

            }
        }
    }

    private void AddRows(int numRows) {
        rowTemplate.name = "Row1";
        for (int i = 0; i < numRows; i++) {
            int rowIndex = i + 1;
            GameObject newRow = InstantiateRow(rowIndex, content.transform);
            rows.Add(newRow);
            newRow.name = "Row" + (i + 2);
        }
    }

    private GameObject InstantiateRow(int rowIndex, Transform parent) {
        GameObject newRow = Instantiate(rowTemplate, parent);
        newRow.transform.SetParent(parent);
        RectTransform newRowRect = newRow.GetComponent<RectTransform>();
        float yPos = RowHeight * rowIndex;
        newRowRect.localPosition = new Vector3(0, -yPos, 0);
        return newRow;
    }

    private void SetContentHeight(int rows) {
        SetContentHeight(content, rows * RowHeight);
    }

    private static void SetContentHeight(RectTransform rect, float height) {
        rect.sizeDelta = new Vector2(0, height);
    }
}
