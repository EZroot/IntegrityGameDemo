using System.Numerics;
using Integrity.Core;
using Integrity.Interface;
using Integrity.Rendering;
using Integrity.Scenes;
using Silk.NET.SDL;

public class Game : IGame
{
    private readonly IEngineSettings m_Settings;
    private readonly IAssetManager m_AssetManager;
    private readonly IInputManager m_InputManager;
    private readonly IWindowPipeline m_WindowPipe;
    private readonly IRenderPipeline m_RenderPipe;
    private readonly IImGuiPipeline m_ImGuiPipe;
    private readonly ISceneManager m_SceneManager;
    private readonly IAudioManager m_AudioManager;
    private readonly ICameraManager m_CameraManager;
    private readonly IGameObjectFactory m_GameObjectFactory;
    private readonly IProfiler m_Profiler;


    const float cameraSpeed = 300.0f;

    public Game()
    {
        m_Settings = Service.Get<IEngineSettings>() ?? throw new Exception("Engine Settings service not found.");
        m_AssetManager = Service.Get<IAssetManager>() ?? throw new Exception("Asset Manager service not found.");
        m_InputManager = Service.Get<IInputManager>() ?? throw new Exception("Input Manager service not found.");
        m_WindowPipe = Service.Get<IWindowPipeline>() ?? throw new Exception("Window Pipeline service not found.");
        m_RenderPipe = Service.Get<IRenderPipeline>() ?? throw new Exception("Render Pipeline service not found.");
        m_ImGuiPipe = Service.Get<IImGuiPipeline>() ?? throw new Exception("ImGui Pipeline service not found.");
        m_SceneManager = Service.Get<ISceneManager>() ?? throw new Exception("Scene Manager service not found.");
        m_AudioManager = Service.Get<IAudioManager>() ?? throw new Exception("Audio Manager service not found.");
        m_CameraManager = Service.Get<ICameraManager>() ?? throw new Exception("Camera Manager service not found.");
        m_GameObjectFactory = Service.Get<IGameObjectFactory>() ?? throw new Exception("GameObjectFactory service not found.");
        m_Profiler = Service.Get<IProfiler>() ?? throw new Exception("Profiler service not found.");
    }
    
    public void Initialize()
    {
        Scene defaultScene = new Scene("DefaultScene");
        
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

        defaultScene.RegisterGameObject(logo);
        defaultScene.RegisterGameObject(pinkface);
        defaultScene.RegisterGameObject(yellowface);

        m_SceneManager.AddScene(defaultScene);
        m_SceneManager.LoadScene(defaultScene);

        Camera2D mainCamera = new Camera2D("MainCamera", m_Settings.Data.WindowWidth, m_Settings.Data.WindowHeight);
        m_CameraManager.RegisterCamera(mainCamera);
    }

    public void Update(float deltaTime)
    {
        if (m_InputManager.IsKeyDown(Scancode.ScancodeW))
            m_CameraManager.MainCamera!.Position += new Vector2(0, -cameraSpeed * deltaTime);
        if (m_InputManager.IsKeyDown(Scancode.ScancodeS))
            m_CameraManager.MainCamera!.Position += new Vector2(0, cameraSpeed * deltaTime);
        if (m_InputManager.IsKeyDown(Scancode.ScancodeA))
            m_CameraManager.MainCamera!.Position += new Vector2(-cameraSpeed * deltaTime, 0);
        if (m_InputManager.IsKeyDown(Scancode.ScancodeD))
            m_CameraManager.MainCamera!.Position += new Vector2(cameraSpeed * deltaTime, 0);

    }

    public void Render()
    {
        // Not needed atm
    }

    public void Cleanup()
    {
    }
}