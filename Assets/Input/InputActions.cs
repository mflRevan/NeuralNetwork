//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/Input/InputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""ce31afac-8c12-415b-a209-4c907c0a90f3"",
            ""actions"": [
                {
                    ""name"": ""Cannon"",
                    ""type"": ""Button"",
                    ""id"": ""827a05be-8b0a-4be5-98d1-e6d0123ec5ec"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Stomper"",
                    ""type"": ""Button"",
                    ""id"": ""fef5ed22-2055-4147-ac0d-5f04d92d655e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Mortar"",
                    ""type"": ""Button"",
                    ""id"": ""4210edfd-1371-46a9-a312-2d5146f61d73"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Resource"",
                    ""type"": ""Button"",
                    ""id"": ""40c27727-d526-4f68-843b-b2c7ceeacdbc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Blank"",
                    ""type"": ""Button"",
                    ""id"": ""f4645366-ca6a-44f6-9d39-a4a0a6bdfcf6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Clear"",
                    ""type"": ""Button"",
                    ""id"": ""54aa817b-a32d-4516-a40b-2451ab796e3c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d4583348-de94-4247-8481-6fc356abe100"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cannon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""34338dff-5294-4748-9dbf-5e254efed59a"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mortar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5ac1c524-4f6c-4050-8ac6-62c12a062a58"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Resource"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8c366e94-5f3c-4f88-b975-990709bf5048"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Blank"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ada7ef3c-90fb-47b8-a662-d25ba25610b9"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Clear"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""426404ab-c4fe-4656-ad63-74f62a64d9c2"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Stomper"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Driver"",
            ""id"": ""7ca419c2-d9f2-4151-a66c-6a0f9de21d38"",
            ""actions"": [
                {
                    ""name"": ""DriveInput"",
                    ""type"": ""PassThrough"",
                    ""id"": ""5fc52f0c-bf08-4f67-9bc0-41b5cb6fb0df"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": ""Hold"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Brake"",
                    ""type"": ""PassThrough"",
                    ""id"": ""1b95d4e7-6d03-4a5e-9810-3bb44a674d8f"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": ""Hold"",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""b4615dbb-e866-4c02-b223-a9176635ae08"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DriveInput"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""744f60f1-d94f-4004-9839-b977c7290e03"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DriveInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""64283432-e986-41a9-9b7d-43c7210bf7dc"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DriveInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""4156d2e7-be3f-47eb-bfaf-f143d7cd350c"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DriveInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""acaf77d8-a65f-4ed8-b5d1-0d11ca8b3123"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DriveInput"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""883465a2-739e-4ba3-9c2c-ff1739069b63"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Brake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Cannon = m_Player.FindAction("Cannon", throwIfNotFound: true);
        m_Player_Stomper = m_Player.FindAction("Stomper", throwIfNotFound: true);
        m_Player_Mortar = m_Player.FindAction("Mortar", throwIfNotFound: true);
        m_Player_Resource = m_Player.FindAction("Resource", throwIfNotFound: true);
        m_Player_Blank = m_Player.FindAction("Blank", throwIfNotFound: true);
        m_Player_Clear = m_Player.FindAction("Clear", throwIfNotFound: true);
        // Driver
        m_Driver = asset.FindActionMap("Driver", throwIfNotFound: true);
        m_Driver_DriveInput = m_Driver.FindAction("DriveInput", throwIfNotFound: true);
        m_Driver_Brake = m_Driver.FindAction("Brake", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Cannon;
    private readonly InputAction m_Player_Stomper;
    private readonly InputAction m_Player_Mortar;
    private readonly InputAction m_Player_Resource;
    private readonly InputAction m_Player_Blank;
    private readonly InputAction m_Player_Clear;
    public struct PlayerActions
    {
        private @InputActions m_Wrapper;
        public PlayerActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Cannon => m_Wrapper.m_Player_Cannon;
        public InputAction @Stomper => m_Wrapper.m_Player_Stomper;
        public InputAction @Mortar => m_Wrapper.m_Player_Mortar;
        public InputAction @Resource => m_Wrapper.m_Player_Resource;
        public InputAction @Blank => m_Wrapper.m_Player_Blank;
        public InputAction @Clear => m_Wrapper.m_Player_Clear;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Cannon.started += instance.OnCannon;
            @Cannon.performed += instance.OnCannon;
            @Cannon.canceled += instance.OnCannon;
            @Stomper.started += instance.OnStomper;
            @Stomper.performed += instance.OnStomper;
            @Stomper.canceled += instance.OnStomper;
            @Mortar.started += instance.OnMortar;
            @Mortar.performed += instance.OnMortar;
            @Mortar.canceled += instance.OnMortar;
            @Resource.started += instance.OnResource;
            @Resource.performed += instance.OnResource;
            @Resource.canceled += instance.OnResource;
            @Blank.started += instance.OnBlank;
            @Blank.performed += instance.OnBlank;
            @Blank.canceled += instance.OnBlank;
            @Clear.started += instance.OnClear;
            @Clear.performed += instance.OnClear;
            @Clear.canceled += instance.OnClear;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Cannon.started -= instance.OnCannon;
            @Cannon.performed -= instance.OnCannon;
            @Cannon.canceled -= instance.OnCannon;
            @Stomper.started -= instance.OnStomper;
            @Stomper.performed -= instance.OnStomper;
            @Stomper.canceled -= instance.OnStomper;
            @Mortar.started -= instance.OnMortar;
            @Mortar.performed -= instance.OnMortar;
            @Mortar.canceled -= instance.OnMortar;
            @Resource.started -= instance.OnResource;
            @Resource.performed -= instance.OnResource;
            @Resource.canceled -= instance.OnResource;
            @Blank.started -= instance.OnBlank;
            @Blank.performed -= instance.OnBlank;
            @Blank.canceled -= instance.OnBlank;
            @Clear.started -= instance.OnClear;
            @Clear.performed -= instance.OnClear;
            @Clear.canceled -= instance.OnClear;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Driver
    private readonly InputActionMap m_Driver;
    private List<IDriverActions> m_DriverActionsCallbackInterfaces = new List<IDriverActions>();
    private readonly InputAction m_Driver_DriveInput;
    private readonly InputAction m_Driver_Brake;
    public struct DriverActions
    {
        private @InputActions m_Wrapper;
        public DriverActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @DriveInput => m_Wrapper.m_Driver_DriveInput;
        public InputAction @Brake => m_Wrapper.m_Driver_Brake;
        public InputActionMap Get() { return m_Wrapper.m_Driver; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DriverActions set) { return set.Get(); }
        public void AddCallbacks(IDriverActions instance)
        {
            if (instance == null || m_Wrapper.m_DriverActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_DriverActionsCallbackInterfaces.Add(instance);
            @DriveInput.started += instance.OnDriveInput;
            @DriveInput.performed += instance.OnDriveInput;
            @DriveInput.canceled += instance.OnDriveInput;
            @Brake.started += instance.OnBrake;
            @Brake.performed += instance.OnBrake;
            @Brake.canceled += instance.OnBrake;
        }

        private void UnregisterCallbacks(IDriverActions instance)
        {
            @DriveInput.started -= instance.OnDriveInput;
            @DriveInput.performed -= instance.OnDriveInput;
            @DriveInput.canceled -= instance.OnDriveInput;
            @Brake.started -= instance.OnBrake;
            @Brake.performed -= instance.OnBrake;
            @Brake.canceled -= instance.OnBrake;
        }

        public void RemoveCallbacks(IDriverActions instance)
        {
            if (m_Wrapper.m_DriverActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IDriverActions instance)
        {
            foreach (var item in m_Wrapper.m_DriverActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_DriverActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public DriverActions @Driver => new DriverActions(this);
    public interface IPlayerActions
    {
        void OnCannon(InputAction.CallbackContext context);
        void OnStomper(InputAction.CallbackContext context);
        void OnMortar(InputAction.CallbackContext context);
        void OnResource(InputAction.CallbackContext context);
        void OnBlank(InputAction.CallbackContext context);
        void OnClear(InputAction.CallbackContext context);
    }
    public interface IDriverActions
    {
        void OnDriveInput(InputAction.CallbackContext context);
        void OnBrake(InputAction.CallbackContext context);
    }
}
