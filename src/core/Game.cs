using System.Numerics;
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
        var logo = m_GameObjectFactory.CreateSpriteObject("TestGameObject", "Content/logo.png");
        logo.Transform.ScaleX = 0.25f;
        logo.Transform.ScaleY = 0.25f;

        var pinkface = m_GameObjectFactory.CreateSpriteObject("TestGameObject", "Content/pink_face.png");
        var yellowface = m_GameObjectFactory.CreateSpriteObject("TestGameObject", "Content/yellow_face.png");
        
        var stress = 10000;
        for(var i = 0; i < stress; i++)
        {
            var blueface = m_GameObjectFactory.CreateSpriteObject("TestGameObject", "Content/blue_face.png");
            var rand = new Random();
            blueface.Transform.X = rand.Next(-1000, 1000);
            blueface.Transform.Y = rand.Next(-1000, 1000);
            defaultScene.RegisterGameObject(blueface);   
        }

        // Here we register our SpriteObjects with our Scene being the container
        defaultScene.RegisterGameObject(logo);
        defaultScene.RegisterGameObject(pinkface);
        defaultScene.RegisterGameObject(yellowface);

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