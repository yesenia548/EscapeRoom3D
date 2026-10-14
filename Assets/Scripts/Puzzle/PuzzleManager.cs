using UnityEngine;

// Puzzle del código + panel numérico. Va en el objeto "CodePad" (el panel verde de la pared).
// ValidateCode() es el código original de la guía; el resto es la ventana para escribir el código.
public class PuzzleManager : MonoBehaviour, IInteractable
{
    [SerializeField] private string correctCode = "1234";
    [SerializeField] private EscapeObject chest;

    private bool panelOpen;
    private bool solved;
    private string input = "";
    private string feedback = "";

    public bool ValidateCode(string code)
    {
        bool valid = code == correctCode;

        if (valid)
        {
            Debug.Log("Código correcto.");
            if (chest != null)
            {
                chest.Unlock();
            }
        }
        else
        {
            Debug.Log("Código incorrecto.");
        }

        return valid;
    }

    // ---------- panel numérico ----------

    public void Interact()
    {
        if (solved)
        {
            Debug.Log("El panel ya está desbloqueado.");
            return;
        }
        panelOpen = true;
        input = "";
        feedback = "";
        GameManager.UiBlocked = true;
    }

    private void ClosePanel()
    {
        panelOpen = false;
        GameManager.UiBlocked = false;
    }

    private void Update()
    {
        if (panelOpen && Input.GetKeyDown(KeyCode.Escape)) ClosePanel();
    }

    private void OnGUI()
    {
        if (!panelOpen) return;

        var box = new Rect(Screen.width / 2f - 170, Screen.height / 2f - 100, 340, 200);
        var prev = GUI.color;
        GUI.color = new Color(0.1f, 0.8f, 0.4f, 1f);
        GUI.Box(box, "PANEL DE CÓDIGO");
        GUI.color = prev;

        Event e = Event.current;
        if (e.type == EventType.KeyDown && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter))
        {
            Submit();
            e.Use();
            return;
        }

        var field = new GUIStyle(GUI.skin.textField) { fontSize = 26, alignment = TextAnchor.MiddleCenter };
        GUI.SetNextControlName("code");
        string typed = GUI.TextField(new Rect(box.x + 40, box.y + 45, 260, 44), input, 4, field);
        input = System.Text.RegularExpressions.Regex.Replace(typed, "[^0-9]", "");
        GUI.FocusControl("code");

        var msg = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 16 };
        msg.normal.textColor = new Color(1f, 0.45f, 0.45f);
        GUI.Label(new Rect(box.x + 20, box.y + 95, 300, 26), feedback, msg);

        if (GUI.Button(new Rect(box.x + 30, box.y + 140, 130, 40), "Probar")) Submit();
        if (GUI.Button(new Rect(box.x + 180, box.y + 140, 130, 40), "Cerrar (Esc)")) ClosePanel();
    }

    private void Submit()
    {
        if (ValidateCode(input))
        {
            solved = true;
            ClosePanel();
        }
        else
        {
            feedback = "Código incorrecto";
            input = "";
        }
    }
}
