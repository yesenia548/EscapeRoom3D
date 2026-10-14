# Cómo abrir y trabajar (Unity 6000.5.5f1 + Visual Studio Code)

## 1. Abrir en Unity
1. Unity Hub > Add > Add project from disk > carpeta `EscapeRoom3D` (editor **6000.5.5f1**).
2. Al importar, un script genera solo la escena colorida `Assets/Scenes/MainScene.unity`, los prefabs
   `Player`, `Chest`, `Door`, los materiales y las animaciones.
   Si no ocurre: menú **Escape Room > Construir escena y prefabs**.
3. Abrir MainScene y pulsar Play. Código del panel: **1234**.

## 2. Configurar Visual Studio Code como editor de Unity
1. Unity > Edit > Preferences > External Tools > **External Script Editor = Visual Studio Code**.
2. Abrir con VS Code la carpeta raíz `EscapeRoom3D` (o `EscapeRoom3D.code-workspace`).
3. Instalar las extensiones recomendadas (ventana emergente; están en `.vscode/extensions.json`):
   Unity (vstuc), C#, C# Dev Kit, GitLens.
4. Todo el Git de la guía se hace desde **Source Control** de VS Code (rama actual en la barra inferior).
   `.vscode/` está en el `.gitignore` de la guía, así que esta configuración es local de cada integrante.

## 3. Scripts (solo 5 + 1 de editor)
| Script | Contenido |
|---|---|
| `Player/PlayerController.cs` | Movimiento WASD (código de la guía) + mirada con el mouse |
| `Interaction/PlayerInteraction.cs` | Raycast con E + interfaz `IInteractable` |
| `Environment/EscapeObject.cs` | Cofre, llave, puerta y nota (un script con `EscapeKind`) |
| `Puzzle/PuzzleManager.cs` | Validación del código + ventana del panel numérico |
| `Systems/GameManager.cs` | Estado de la llave + HUD en pantalla |
| `Editor/EscapeRoomBuilder.cs` | Genera escena/prefabs/materiales (solo editor) |

## 4. Ramas sugeridas
| Rama | Archivos |
|---|---|
| Feature_PlayerMovement | PlayerController.cs, Prefabs/Player.prefab |
| Feature_Interaction | PlayerInteraction.cs, GameManager.cs, EscapeObject.cs |
| Feature_Animation | Animations/*, Prefabs/Chest.prefab, Door.prefab |
| Feature_Puzzle | PuzzleManager.cs |

Los scripts se necesitan entre sí (PlayerController usa GameManager, EscapeObject usa GameManager,
PuzzleManager usa EscapeObject y IInteractable). Para que compile en cada rama, traigan primero lo mínimo
o integren en Dev antes de probar. Como el proyecto ya viene completo, copien cada archivo al caso que
corresponda en vez de commitear todo junto.

## Notas
- Input Manager clásico (`Input.GetAxis`) como pide la guía: no agregar el paquete Input System.
- Los `.meta` los crea Unity y se versionan.
- No probado en Unity desde aquí: si hay algún error de consola, revisarlo y avisar.

## Si ya tenías generada una versión anterior (v2)
1. Reemplaza `Assets/Scripts/Player/PlayerController.cs` y `Assets/Editor/EscapeRoomBuilder.cs` por los nuevos.
2. En Unity ejecuta **Escape Room > Construir escena y prefabs** (regenera MainScene, prefabs y animaciones).
3. Prueba: código 1234 > E en el cofre > E en la llave > E en la puerta. Sin llave, la puerta debe bloquear.
