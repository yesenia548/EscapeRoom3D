using UnityEngine;

// Estado del juego (llave) + HUD en pantalla (mira, ayuda, mensajes del juego).
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // true mientras hay una ventana abierta (panel de código): bloquea mirar/interactuar.
    public static bool UiBlocked;

    public bool HasKey { get; private set; }

    private string message = "";
    private float messageUntil;
    private GUIStyle titleStyle, textStyle, crossStyle;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => UiBlocked = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ObtainKey()
    {
        HasKey = true;
    }

    // ---------- HUD: los mensajes salen de los Debug.Log del juego ----------

    private void OnEnable() => Application.logMessageReceived += OnLog;
    private void OnDisable() => Application.logMessageReceived -= OnLog;

    private void OnLog(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Log) return;
        message = condition;
        messageUntil = condition.Contains("Escape completado") ? float.MaxValue : Time.time + 3.5f;
    }

    private void Panel(Rect r, Color c)
    {
        var prev = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        GUI.color = prev;
    }

    private void OnGUI()
    {
        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 28, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            textStyle = new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold };
            crossStyle = new GUIStyle(GUI.skin.label) { fontSize = 30, alignment = TextAnchor.MiddleCenter };
        }

        // mira
        if (!UiBlocked)
        {
            crossStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(Screen.width / 2f - 20, Screen.height / 2f - 20, 40, 40), "+", crossStyle);
        }

        // barra superior: controles y llave
        Panel(new Rect(10, 10, 400, 62), new Color(0.1f, 0.05f, 0.25f, 0.75f));
        textStyle.normal.textColor = new Color(0.6f, 0.95f, 1f);
        GUI.Label(new Rect(20, 14, 390, 24), "WASD mover  ·  Mouse mirar  ·  E interactuar", textStyle);
        textStyle.normal.textColor = HasKey ? new Color(1f, 0.85f, 0.2f) : new Color(1f, 0.6f, 0.7f);
        GUI.Label(new Rect(20, 40, 390, 24), HasKey ? "★ Llave dorada: LA TIENES" : "☆ Llave dorada: no la tienes", textStyle);

        // mensaje inferior
        if (Time.time < messageUntil)
        {
            Panel(new Rect(Screen.width / 2f - 360, Screen.height - 130, 720, 56), new Color(0.15f, 0.05f, 0.3f, 0.85f));
            titleStyle.normal.textColor = new Color(0f, 0f, 0f, 0.6f);
            GUI.Label(new Rect(Screen.width / 2f - 359, Screen.height - 129, 720, 56), message, titleStyle);
            titleStyle.normal.textColor = new Color(1f, 0.92f, 0.3f);
            GUI.Label(new Rect(Screen.width / 2f - 360, Screen.height - 130, 720, 56), message, titleStyle);
        }
    }
}
