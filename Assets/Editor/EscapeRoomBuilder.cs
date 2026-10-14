#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// Genera MainScene (habitación colorida), prefabs (Player, Chest, Door), materiales y animaciones.
/// Menú: Escape Room > Construir escena y prefabs.
/// Se ejecuta solo una vez si Assets/Scenes/MainScene.unity no existe.
/// </summary>
public static class EscapeRoomBuilder
{
    private const string ScenePath = "Assets/Scenes/MainScene.unity";

    [InitializeOnLoadMethod]
    private static void AutoBuild()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;
            if (File.Exists(ScenePath)) return;
            Build();
        };
    }

    [MenuItem("Escape Room/Construir escena y prefabs")]
    public static void Build()
    {
        foreach (var f in new[] { "Assets/Scenes", "Assets/Prefabs", "Assets/Materials", "Assets/Animations" })
            if (!AssetDatabase.IsValidFolder(f))
                AssetDatabase.CreateFolder("Assets", Path.GetFileName(f));

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ---------- Paleta ----------
        Material tileA = Mat("TileAmber", Hex("FFC857"));
        Material tileB = Mat("TileTeal", Hex("2EC4B6"));
        Material floorBase = Mat("FloorBase", Hex("3D2C8D"));
        Material wallSouth = Mat("WallCoral", Hex("FF6B6B"));
        Material wallWest = Mat("WallLavender", Hex("A78BFA"));
        Material wallEast = Mat("WallMint", Hex("5EEAD4"));
        Material wallNorth = Mat("WallSky", Hex("60A5FA"));
        Material ceiling = Mat("Ceiling", Hex("FFF4D6"));
        Material trim = Mat("Trim", Hex("1E1B4B"));
        Material wood = Mat("Wood", Hex("8B4A1F"));
        Material darkWood = Mat("DarkWood", Hex("4A2511"));
        Material gold = Mat("Gold", Hex("FFD60A"), true, 0.6f);
        Material doorM = Mat("Door", Hex("2563EB"));
        Material frameM = Mat("DoorFrame", Hex("F472B6"));
        Material padM = Mat("CodePad", Hex("22C55E"), true, 1.2f);
        Material paperM = Mat("Paper", Hex("FEF08A"), true, 0.4f);
        Material exitM = Mat("ExitFloor", Hex("4ADE80"));
        Material exitGlow = Mat("ExitGlow", Hex("86EFAC"), true, 2f);
        Material rugRed = Mat("RugRed", Hex("E11D48"));
        Material rugOrange = Mat("RugOrange", Hex("FB923C"));
        Material rugPink = Mat("RugPink", Hex("EC4899"));
        Material potM = Mat("Pot", Hex("C2410C"));
        Material leafM = Mat("Leaf", Hex("16A34A"));
        Material lampM = Mat("LampGlow", Hex("FDE68A"), true, 2f);
        Material metal = Mat("Metal", Hex("94A3B8"));
        Color[] bookColors = { Hex("EF4444"), Hex("F59E0B"), Hex("10B981"), Hex("3B82F6"), Hex("8B5CF6"), Hex("EC4899"), Hex("FACC15") };

        // ---------- Habitación ----------
        var room = new GameObject("Room").transform;
        Box("FloorBase", room, new Vector3(0, -0.05f, 0), new Vector3(8.4f, 0.1f, 8.4f), floorBase);
        Box("Ceiling", room, new Vector3(0, 3.05f, 0), new Vector3(8.4f, 0.1f, 8.4f), ceiling);
        Box("Wall_South", room, new Vector3(0, 1.5f, -4.1f), new Vector3(8.4f, 3f, 0.2f), wallSouth);
        Box("Wall_West", room, new Vector3(-4.1f, 1.5f, 0), new Vector3(0.2f, 3f, 8.4f), wallWest);
        Box("Wall_East", room, new Vector3(4.1f, 1.5f, 0), new Vector3(0.2f, 3f, 8.4f), wallEast);
        // pared norte con hueco de puerta (1.4 m x 2.2 m)
        Box("Wall_North_Left", room, new Vector3(-2.4f, 1.5f, 4.1f), new Vector3(3.4f, 3f, 0.2f), wallNorth);
        Box("Wall_North_Right", room, new Vector3(2.4f, 1.5f, 4.1f), new Vector3(3.4f, 3f, 0.2f), wallNorth);
        Box("Wall_North_Top", room, new Vector3(0, 2.6f, 4.1f), new Vector3(1.4f, 0.8f, 0.2f), wallNorth);

        // piso de ajedrez (decorativo, sin colliders)
        var tiles = new GameObject("FloorTiles").transform;
        tiles.SetParent(room, false);
        for (int x = 0; x < 8; x++)
            for (int z = 0; z < 8; z++)
                Box($"Tile_{x}_{z}", tiles, new Vector3(-3.5f + x, 0.01f, -3.5f + z), new Vector3(1f, 0.02f, 1f),
                    (x + z) % 2 == 0 ? tileA : tileB, false);

        // zócalos oscuros
        var trims = new GameObject("Trim").transform;
        trims.SetParent(room, false);
        Box("Trim_South", trims, new Vector3(0, 0.1f, -3.97f), new Vector3(7.8f, 0.2f, 0.06f), trim, false);
        Box("Trim_West", trims, new Vector3(-3.97f, 0.1f, 0), new Vector3(0.06f, 0.2f, 7.8f), trim, false);
        Box("Trim_East", trims, new Vector3(3.97f, 0.1f, 0), new Vector3(0.06f, 0.2f, 7.8f), trim, false);

        // marco rosa de la puerta
        Box("Frame_Left", trims, new Vector3(-0.76f, 1.15f, 4f), new Vector3(0.12f, 2.3f, 0.34f), frameM, false);
        Box("Frame_Right", trims, new Vector3(0.76f, 1.15f, 4f), new Vector3(0.12f, 2.3f, 0.34f), frameM, false);
        Box("Frame_Top", trims, new Vector3(0, 2.26f, 4f), new Vector3(1.64f, 0.12f, 0.34f), frameM, false);

        // ---------- Decoración ----------
        var deco = new GameObject("Decoration").transform;

        // alfombra redonda de 3 colores
        Cyl("RugOuter", deco, new Vector3(0, 0.03f, 0), new Vector3(3.2f, 0.01f, 3.2f), rugRed);
        Cyl("RugMiddle", deco, new Vector3(0, 0.04f, 0), new Vector3(2.3f, 0.01f, 2.3f), rugOrange);
        Cyl("RugInner", deco, new Vector3(0, 0.05f, 0), new Vector3(1.4f, 0.01f, 1.4f), rugPink);

        // librero con libros de colores (pared oeste)
        var shelf = new GameObject("Bookshelf").transform;
        shelf.SetParent(deco, false);
        shelf.position = new Vector3(-3.65f, 0, 2.2f);
        Box("Back", shelf, new Vector3(0.2f, 1.1f, 0), new Vector3(0.05f, 2.2f, 1.8f), darkWood, false);
        for (int level = 0; level < 4; level++)
            Box("Plank" + level, shelf, new Vector3(0, 0.05f + level * 0.7f, 0), new Vector3(0.45f, 0.06f, 1.8f), wood, false);
        Box("Side_A", shelf, new Vector3(0, 1.1f, -0.9f), new Vector3(0.45f, 2.2f, 0.06f), wood, false);
        Box("Side_B", shelf, new Vector3(0, 1.1f, 0.9f), new Vector3(0.45f, 2.2f, 0.06f), wood, false);
        var rnd = new System.Random(7);
        for (int level = 0; level < 3; level++)
            for (int b = 0; b < 9; b++)
            {
                float h = 0.4f + (float)rnd.NextDouble() * 0.2f;
                var book = Box($"Book_{level}_{b}", shelf, new Vector3(0, 0.08f + level * 0.7f + h / 2f, -0.76f + b * 0.17f),
                    new Vector3(0.3f, h, 0.12f), Mat("Book" + ((level * 9 + b) % bookColors.Length), bookColors[(level * 9 + b) % bookColors.Length]), false);
            }
        var shelfCol = shelf.gameObject.AddComponent<BoxCollider>();
        shelfCol.center = new Vector3(0, 1.1f, 0);
        shelfCol.size = new Vector3(0.5f, 2.2f, 1.8f);

        Plant(deco, new Vector3(-3.4f, 0, -3.4f), potM, leafM);
        Plant(deco, new Vector3(3.4f, 0, 3.3f), potM, leafM);

        // lámpara de pie
        var lamp = new GameObject("FloorLamp").transform;
        lamp.SetParent(deco, false);
        lamp.position = new Vector3(-3.4f, 0, 3.3f);
        Cyl("Pole", lamp, new Vector3(0, 0.8f, 0), new Vector3(0.06f, 0.8f, 0.06f), metal, false);
        Cyl("Base", lamp, new Vector3(0, 0.03f, 0), new Vector3(0.4f, 0.03f, 0.4f), metal, false);
        var bulb = Sphere("Bulb", lamp, new Vector3(0, 1.75f, 0), Vector3.one * 0.4f, lampM);
        Object.DestroyImmediate(bulb.GetComponent<Collider>());

        // zona de salida
        var exit = new GameObject("ExitArea").transform;
        Box("ExitFloor", exit, new Vector3(0, -0.05f, 6.4f), new Vector3(4f, 0.1f, 4.6f), exitM);
        Box("ExitWall_Back", exit, new Vector3(0, 1.5f, 8.8f), new Vector3(4.4f, 3f, 0.2f), wallMint(wallEast));
        Box("ExitWall_Left", exit, new Vector3(-2.1f, 1.5f, 6.4f), new Vector3(0.2f, 3f, 4.6f), wallEast);
        Box("ExitWall_Right", exit, new Vector3(2.1f, 1.5f, 6.4f), new Vector3(0.2f, 3f, 4.6f), wallEast);
        Box("ExitCeiling", exit, new Vector3(0, 3.05f, 6.4f), new Vector3(4.4f, 0.1f, 4.6f), ceiling);
        Box("ExitGlow", exit, new Vector3(0, 0.02f, 7f), new Vector3(2f, 0.02f, 2f), exitGlow, false);
        for (int i = 0; i < 6; i++)   // "confeti" de esferas de colores
        {
            var s = Sphere("Confetti" + i, exit, new Vector3(-1.5f + i * 0.6f, 2.3f + (i % 2) * 0.3f, 8.6f), Vector3.one * 0.25f,
                Mat("Book" + i, bookColors[i]));
            Object.DestroyImmediate(s.GetComponent<Collider>());
        }

        // ---------- Luces de colores ----------
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.5f, 0.48f, 0.58f);
        AddLight("Light_Warm", new Vector3(-1.5f, 2.6f, -1.5f), Hex("FFE9C2"));
        AddLight("Light_Pink", new Vector3(1.5f, 2.6f, 1.5f), Hex("FFB3E6"));
        AddLight("Light_Cyan", new Vector3(-2.5f, 2.2f, 2.5f), Hex("9BE7FF"));
        AddLight("Light_Exit", new Vector3(0, 2.6f, 6.4f), Hex("B9FFC9"));

        // ---------- Sistemas ----------
        new GameObject("GameManager").AddComponent<GameManager>();

        // ---------- Player ----------
        GameObject player = BuildPlayer();
        player.transform.position = new Vector3(0, 0.95f, -3f);
        Prefab(player, "Assets/Prefabs/Player.prefab");

        // ---------- Cofre ----------
        GameObject chest = BuildChest(wood, gold);
        chest.transform.position = new Vector3(2.7f, 0, -2.6f);
        Prefab(chest, "Assets/Prefabs/Chest.prefab");   // OJO: 'chest' sigue siendo la instancia de la escena

        // ---------- Puerta ----------
        GameObject door = BuildDoor(doorM, gold);
        door.transform.position = new Vector3(-0.7f, 0, 4f);
        Prefab(door, "Assets/Prefabs/Door.prefab");

        // ---------- Puzzle: panel con código + nota de pista ----------
        var pad = Box("CodePad", null, new Vector3(3.9f, 1.4f, 0.5f), new Vector3(0.15f, 0.7f, 0.5f), padM);
        var pm = pad.AddComponent<PuzzleManager>();
        SetRef(pm, "chest", chest.GetComponent<EscapeObject>());

        var note = Box("HintNote", null, new Vector3(-3.93f, 1.6f, -1f), new Vector3(0.05f, 0.6f, 0.8f), paperM);
        var noteObj = note.AddComponent<EscapeObject>();
        SetEnum(noteObj, "kind", (int)EscapeKind.Note);

        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[EscapeRoom] Escena y prefabs generados. Abre Assets/Scenes/MainScene.unity y pulsa Play. Código del cofre: 1234.");
    }

    private static Material wallMint(Material m) => m;

    // ======================= Objetos =======================

    private static GameObject BuildPlayer()
    {
        var p = new GameObject("Player");
        var rb = p.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        var cap = p.AddComponent<CapsuleCollider>();
        cap.height = 1.8f;
        cap.radius = 0.35f;

        var pivot = new GameObject("CameraPivot").transform;
        pivot.SetParent(p.transform, false);
        pivot.localPosition = new Vector3(0, 0.7f, 0);

        var camGo = new GameObject("PlayerCamera");
        camGo.tag = "MainCamera";
        camGo.transform.SetParent(pivot, false);
        var cam = camGo.AddComponent<Camera>();
        cam.nearClipPlane = 0.05f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Hex("1E1B4B");
        camGo.AddComponent<AudioListener>();

        var pc = p.AddComponent<PlayerController>();
        SetRef(pc, "cameraPivot", pivot);
        var inter = p.AddComponent<PlayerInteraction>();
        SetRef(inter, "playerCamera", cam);
        return p;
    }

    private static GameObject BuildChest(Material wood, Material gold)
    {
        var chest = new GameObject("Chest");
        var b = Box("Base", chest.transform, new Vector3(0, 0.25f, 0), new Vector3(0.9f, 0.5f, 0.6f), wood, false);
        Box("Band_L", b.transform, new Vector3(-0.32f, 0, 0), new Vector3(0.1f, 1.02f, 1.04f), gold, false);
        Box("Band_R", b.transform, new Vector3(0.32f, 0, 0), new Vector3(0.1f, 1.02f, 1.04f), gold, false);
        Box("Lock", chest.transform, new Vector3(0, 0.4f, -0.31f), new Vector3(0.14f, 0.16f, 0.04f), gold, false);

        // Tapa con bisagra en el borde trasero superior
        var lid = new GameObject("Lid").transform;
        lid.SetParent(chest.transform, false);
        lid.localPosition = new Vector3(0, 0.5f, 0.3f);
        var lm = Box("LidMesh", lid, new Vector3(0, 0.06f, -0.3f), new Vector3(0.94f, 0.12f, 0.64f), wood, false);
        Box("LidBand_L", lm.transform, new Vector3(-0.32f, 0.5f, 0), new Vector3(0.1f, 1.1f, 1.04f), gold, false);
        Box("LidBand_R", lm.transform, new Vector3(0.32f, 0.5f, 0), new Vector3(0.1f, 1.1f, 1.04f), gold, false);

        // Llave dorada: oculta hasta que la animación "Open" la activa
        var key = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        key.name = "Key";
        key.transform.SetParent(chest.transform, false);
        key.transform.localPosition = new Vector3(0, 0.6f, 0);
        key.transform.localScale = Vector3.one * 0.2f;
        key.GetComponent<Renderer>().sharedMaterial = gold;
        var keyObj = key.AddComponent<EscapeObject>();
        SetEnum(keyObj, "kind", (int)EscapeKind.Key);
        key.SetActive(false);

        // IInteractable se busca en el objeto del collider -> collider en la raíz
        var col = chest.AddComponent<BoxCollider>();
        col.center = new Vector3(0, 0.25f, 0);
        col.size = new Vector3(0.9f, 0.5f, 0.6f);

        var animator = chest.AddComponent<Animator>();
        var open = new AnimationClip { name = "Chest_Open" };
        open.SetCurve("Lid", typeof(Transform), "localEulerAnglesRaw.x", AnimationCurve.EaseInOut(0, 0, 0.8f, -110f));
        open.SetCurve("Key", typeof(GameObject), "m_IsActive",
            new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.399f, 0), new Keyframe(0.4f, 1), new Keyframe(0.8f, 1)));
        animator.runtimeAnimatorController = Controller("Chest", open);

        var eo = chest.AddComponent<EscapeObject>();
        SetEnum(eo, "kind", (int)EscapeKind.Chest);
        SetRef(eo, "animator", animator);
        return chest;
    }

    private static GameObject BuildDoor(Material doorMat, Material gold)
    {
        var pivot = new GameObject("Door");
        var panel = Box("DoorPanel", pivot.transform, new Vector3(0.7f, 1.1f, 0), new Vector3(1.4f, 2.2f, 0.12f), doorMat, false);
        var knob = Sphere("Knob", pivot.transform, new Vector3(1.2f, 1.05f, -0.1f), Vector3.one * 0.14f, gold);
        Object.DestroyImmediate(knob.GetComponent<Collider>());
        Box("Window", panel.transform, new Vector3(0, 0.25f, -0.55f), new Vector3(0.5f, 0.25f, 0.2f), gold, false);

        var col = pivot.AddComponent<BoxCollider>();
        col.center = new Vector3(0.7f, 1.1f, 0);
        col.size = new Vector3(1.4f, 2.2f, 0.3f);

        var animator = pivot.AddComponent<Animator>();
        var open = new AnimationClip { name = "Door_Open" };
        open.SetCurve("", typeof(Transform), "localEulerAnglesRaw.y", AnimationCurve.EaseInOut(0, 0, 1f, -100f));
        animator.runtimeAnimatorController = Controller("Door", open);

        var eo = pivot.AddComponent<EscapeObject>();
        SetEnum(eo, "kind", (int)EscapeKind.Door);
        SetRef(eo, "animator", animator);
        return pivot;
    }

    private static void Plant(Transform parent, Vector3 pos, Material pot, Material leaf)
    {
        var p = new GameObject("Plant").transform;
        p.SetParent(parent, false);
        p.position = pos;
        Cyl("Pot", p, new Vector3(0, 0.25f, 0), new Vector3(0.5f, 0.25f, 0.5f), pot, false);
        Sphere("Leaves", p, new Vector3(0, 0.85f, 0), new Vector3(0.8f, 0.7f, 0.8f), leaf);
        Sphere("Leaves2", p, new Vector3(0.15f, 1.2f, 0.05f), new Vector3(0.5f, 0.5f, 0.5f), leaf);
        foreach (var c in p.GetComponentsInChildren<Collider>()) Object.DestroyImmediate(c);
    }

    // ======================= Utilidades =======================

    private static AnimatorController Controller(string name, AnimationClip openClip)
    {
        AssetDatabase.DeleteAsset($"Assets/Animations/{name}_Open.anim");
        AssetDatabase.DeleteAsset($"Assets/Animations/{name}.controller");
        AssetDatabase.CreateAsset(openClip, $"Assets/Animations/{name}_Open.anim");
        var ctrl = AnimatorController.CreateAnimatorControllerAtPath($"Assets/Animations/{name}.controller");
        ctrl.AddParameter("Open", AnimatorControllerParameterType.Trigger);
        var sm = ctrl.layers[0].stateMachine;
        var closed = sm.AddState("Closed");
        var opened = sm.AddState("Open");
        opened.motion = openClip;
        sm.defaultState = closed;
        var t = closed.AddTransition(opened);
        t.AddCondition(AnimatorConditionMode.If, 0, "Open");
        t.hasExitTime = false;
        t.duration = 0f;
        return ctrl;
    }

    // Guarda el prefab y deja la instancia de la escena conectada a él.
    // (SaveAsPrefabAssetAndConnect devuelve el asset del prefab, NO la instancia: por eso no se usa su resultado.)
    private static void Prefab(GameObject sceneObject, string path)
    {
        AssetDatabase.DeleteAsset(path);
        PrefabUtility.SaveAsPrefabAssetAndConnect(sceneObject, path, InteractionMode.AutomatedAction);
    }

    private static GameObject Prim(PrimitiveType type, string name, Transform parent, Vector3 pos, Vector3 scale, Material mat, bool collider)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = name;
        if (parent != null) go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        if (!collider) Object.DestroyImmediate(go.GetComponent<Collider>());
        return go;
    }
    private static GameObject Box(string n, Transform p, Vector3 pos, Vector3 s, Material m, bool col = true) => Prim(PrimitiveType.Cube, n, p, pos, s, m, col);
    private static GameObject Cyl(string n, Transform p, Vector3 pos, Vector3 s, Material m, bool col = false) => Prim(PrimitiveType.Cylinder, n, p, pos, s, m, col);
    private static GameObject Sphere(string n, Transform p, Vector3 pos, Vector3 s, Material m) => Prim(PrimitiveType.Sphere, n, p, pos, s, m, true);

    private static void AddLight(string name, Vector3 pos, Color color)
    {
        var go = new GameObject(name);
        go.transform.position = pos;
        var l = go.AddComponent<Light>();
        l.type = LightType.Point;
        l.range = 9f;
        l.intensity = GraphicsSettings.defaultRenderPipeline != null ? 10f : 1.1f;
        l.color = color;
    }

    private static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out var c);
        return c;
    }

    private static Material Mat(string name, Color color, bool emissive = false, float intensity = 1f)
    {
        string path = $"Assets/Materials/{name}.mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;

        Shader sh = null;
        if (GraphicsSettings.defaultRenderPipeline != null) sh = Shader.Find("Universal Render Pipeline/Lit");
        if (sh == null) sh = Shader.Find("Standard");
        if (sh == null) sh = Shader.Find("Sprites/Default");
        var m = new Material(sh) { color = color };
        if (emissive && m.HasProperty("_EmissionColor"))
        {
            m.EnableKeyword("_EMISSION");
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            m.SetColor("_EmissionColor", color * intensity);
        }
        AssetDatabase.CreateAsset(m, path);
        return m;
    }

    private static void SetRef(Object target, string field, Object value)
    {
        var so = new SerializedObject(target);
        var p = so.FindProperty(field);
        if (p == null) { Debug.LogError($"[EscapeRoom] Campo no encontrado: {target.GetType().Name}.{field}"); return; }
        p.objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetEnum(Object target, string field, int value)
    {
        var so = new SerializedObject(target);
        var p = so.FindProperty(field);
        if (p == null) { Debug.LogError($"[EscapeRoom] Campo no encontrado: {target.GetType().Name}.{field}"); return; }
        p.enumValueIndex = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
#endif
