using System.Numerics;
using Integrity.Components;
using Integrity.Core;
using Integrity.Interface;
using Integrity.Rendering;
using Integrity.Scenes;
using Silk.NET.SDL;

/// <summary>
/// A minimum example of how to use Integrity2D for a game
/// 
/// This Github:    https://github.com/EZroot/IntegrityGameDemo
/// Engine Github:  https://github.com/EZroot/Integrity2D
/// Docs:           https://ezroot.github.io/Integrity2D/
/// 
/// </summary>
public class Game : IGame
{
    private readonly IEngineSettings m_Settings;
    private readonly IInputManager m_InputManager;
    private readonly ISceneManager m_SceneManager;
    private readonly ICameraManager m_CameraManager;
    private readonly IGameObjectFactory m_GameObjectFactory;

    const float m_CameraSpeed = 300.0f;

    public Game()
    {
        // Using our service locator, we can grab the managers convieniently and store their reference
        // Docs: https://ezroot.github.io/Integrity2D/api/Integrity.Interface.html

        m_Settings = Service.Get<IEngineSettings>() ?? throw new Exception("Engine Settings service not found.");
        m_InputManager = Service.Get<IInputManager>() ?? throw new Exception("Input Manager service not found.");
        m_SceneManager = Service.Get<ISceneManager>() ?? throw new Exception("Scene Manager service not found.");
        m_CameraManager = Service.Get<ICameraManager>() ?? throw new Exception("Camera Manager service not found.");
        m_GameObjectFactory = Service.Get<IGameObjectFactory>() ?? throw new Exception("GameObjectFactory service not found.");
    }

    public void Initialize()
    {
        // We create our scene that will serve as a base to store and render our SpriteObjects
        Scene defaultScene = new Scene("DefaultScene");

        // Using GameObjectFactory we can create GameObjects (A Base Game Object class that only has a Transform Component)
        // In this instance, we use SpriteObject, which will default with a Transform Component and a SpriteComponent
        // You can build a SpriteObject yourself by adding any component you want
        // But SpriteObject works best with our engine Renderer
        // Objects: https://ezroot.github.io/Integrity2D/api/Integrity.Objects.html
        // Components: https://ezroot.github.io/Integrity2D/api/Integrity.Components.html

        // Create a simple object with a sprite
        var pinkface = m_GameObjectFactory.CreateSpriteObject("TestGameObject", "Content/pink_face.png");
        // Here we register our SpriteObjects with our Scene being the container
        defaultScene.RegisterGameObject(pinkface);

        var rand = new Random();
        var stress = 10000;
        for (var i = 0; i < stress; i++)
        {
            // Example of using an atlas, with animation
            var yellowface = m_GameObjectFactory.CreateSpriteObject("TestGameObject", "Content/atlas.png");
            yellowface.Transform.X = rand.Next(-stress / 5, stress / 5);
            yellowface.Transform.Y = rand.Next(-stress / 5, stress / 5);

            var spriteComponent = yellowface.GetComponent<SpriteComponent>();

            // Set the intial atlas UV position of our sprite
            spriteComponent.SourceRect = new Integrity.Utils.Rect(0, 0, 32, 32);

            // Create an animation component to store our frames
            var animationComponent = new Integrity.Components.AnimationComponent();

            // Using an array of our UV rect positions on the atlas for each frame along with the frame time
            var animationIdleFrames = new Integrity.Components.AnimationFrame[4];
            animationIdleFrames[0] = new Integrity.Components.AnimationFrame(new Integrity.Utils.Rect(0, 0, 32, 32), 0.25f);
            animationIdleFrames[1] = new Integrity.Components.AnimationFrame(new Integrity.Utils.Rect(32, 0, 32, 32), 0.25f);
            animationIdleFrames[2] = new Integrity.Components.AnimationFrame(new Integrity.Utils.Rect(64, 0, 32, 32), 0.25f);
            animationIdleFrames[3] = new Integrity.Components.AnimationFrame(new Integrity.Utils.Rect(98, 0, 32, 32), 0.25f);

            // Add our animation frames to our animation component
            animationComponent.AddAnimation("Idle", animationIdleFrames);

            // Add our component to our sprite game object
            yellowface.AddComponent(animationComponent);
            defaultScene.RegisterGameObject(yellowface);
        }

        // Add our scene to our scene manager map
        m_SceneManager.AddScene(defaultScene);

        // Loading a scene will just set it as the m_SceneManager.CurrentScene
        // Which is used internally in the Engine for current rendering
        m_SceneManager.LoadScene(defaultScene);

        // We NEED a default camera or the engine will abort since there is no point to render
        // Multiple cameras are not supported
        // Doc: https://ezroot.github.io/Integrity2D/api/Integrity.Rendering.html

        Camera2D mainCamera = new Camera2D("MainCamera", m_Settings.Data.WindowWidth, m_Settings.Data.WindowHeight);

        // Register the Camera to our Camera Manager
        // Similar to SceneManager it will just set the m_CameraManager.MainCamera to the registered camera
        // Doc: https://ezroot.github.io/Integrity2D/api/Integrity.Rendering.html
        m_CameraManager.RegisterCamera(mainCamera);
    }

    public void Update(float deltaTime)
    {
        if (m_InputManager.IsKeyDown(Scancode.ScancodeW))
            m_CameraManager.MainCamera!.Position += new Vector2(0, -m_CameraSpeed * deltaTime);
        if (m_InputManager.IsKeyDown(Scancode.ScancodeS))
            m_CameraManager.MainCamera!.Position += new Vector2(0, m_CameraSpeed * deltaTime);
        if (m_InputManager.IsKeyDown(Scancode.ScancodeA))
            m_CameraManager.MainCamera!.Position += new Vector2(-m_CameraSpeed * deltaTime, 0);
        if (m_InputManager.IsKeyDown(Scancode.ScancodeD))
            m_CameraManager.MainCamera!.Position += new Vector2(m_CameraSpeed * deltaTime, 0);
    }

    public void Render()
    {
        // Not needed for our usecase
    }

    public void Cleanup()
    {
    }
}