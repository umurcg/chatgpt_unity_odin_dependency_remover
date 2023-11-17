#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    ///     Base class that may be used by UI Systems. This approach especially prevent race condition that may appear within
    ///     Start or Awake functions.
    ///     Within chain Initialization UI elements are initialized within hierarchy.
    ///     Class is also focused to prevent bugs that may appear due to leaving objects Active or Deactivated in the scene.
    ///     This case especially caused by human error.
    /// </summary>
    public class UIElement : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("UI Objects that will be activated or deactivated according to activation of the UI Element")]
        protected RectTransform[] uiObjects;

        protected MaskableGraphic[] MaskableGraphics;
        
        [SerializeField] protected UIElement[] subUIElements;

        public bool initOnStart;
        public bool activateOnStart;

        [Tooltip(
            "If true, when parent UI elements (if exists) is activate this UI element will be also activated automatically.")]
        public bool activateWithParent = true;

#if ODIN_INSPECTOR
        [SerializeField, ReadOnly] protected bool isInitialized;
        [SerializeField, ReadOnly] protected bool isActive;
#else
        [SerializeField] protected bool isInitialized;
        [SerializeField] protected bool isActive;
#endif
        private LevelManager _levelManager;

        /// <summary>
        ///     UI Manager is source component holding all UI Elements
        /// </summary>
        private UIManager _uiManager;

        protected CoreManager CoreManager;

        /// <summary>
        ///     whether or not UI Element active
        /// </summary>
        public bool IsActive => isActive;


#if ODIN_INSPECTOR 
[FoldoutGroup("Events")] 
#endif
        public UnityEvent onInitialized;
#if ODIN_INSPECTOR 
[FoldoutGroup("Events")] 
#endif
        public UnityEvent onActivated;
#if ODIN_INSPECTOR 
[FoldoutGroup("Events")] 
#endif
        public UnityEvent onDeactivated;
#if ODIN_INSPECTOR 
[FoldoutGroup("Events")] 
#endif
        public UnityEvent<bool> onActivationChanged;

        
        protected virtual void Start()
        {
            if (initOnStart)
                Initialize();


            if (activateOnStart)
                SetActive(true);
        }


        public virtual void Initialize()
        {
            if (isInitialized)
            {
                Debug.LogError(gameObject.name + " is already initialized." +
                               " It is being tried to be initialized more than once which may cause unpredictable issue.");
                return;
            }

            MaskableGraphics = GetComponents<MaskableGraphic>();
            
            isInitialized = true;

            _uiManager = UIManager.Request();
            CoreManager = CoreManager.Request();
            _levelManager = LevelManager.Request();

            //Activate root object of UI element. Because root object must be activated.
            gameObject.SetActive(true);

            //Initialize all children
            foreach (var element in subUIElements)
                element.Initialize();

            //Deactivate all UIObjects because UI elements are deactivated by default
            //This is required for protection. If one activates a ui object from scene this code force it to be deactivated at start
            foreach (var uiObject in uiObjects)
                uiObject.gameObject.SetActive(false);
            
            onInitialized?.Invoke();
        }

#if ODIN_INSPECTOR
        [Button]
#else
        [ContextMenu("Activate")]
#endif
        public virtual void Activate()
        {
            SetActive(true);
        }

#if ODIN_INSPECTOR
        [Button]
#else
        [ContextMenu("Deactivate")]
#endif
        public virtual void Deactivate()
        {
            SetActive(false);
        }

        public virtual void SetActive(bool activate)
        {
            if (isInitialized == false)
            {
                Debug.LogError("UI Element " + gameObject +
                               " couldn't be activated or deactivated because it is not initialized yet.");
                return;
            }

            //Set active all sub ui elements as this element
            foreach (var uiElement in subUIElements)
                if (activate && uiElement.activateWithParent)
                    uiElement.SetActive(true);
                else if (!activate)
                    uiElement.SetActive(false);

            //Switch ui objects
            foreach (var uiObject in uiObjects)
                uiObject.gameObject.SetActive(activate);

            foreach (MaskableGraphic graphic in MaskableGraphics) graphic.enabled = activate;
            isActive = activate;
            
            onActivationChanged?.Invoke(activate);
            if (activate)
                onActivated?.Invoke();
            else
                onDeactivated?.Invoke();
        }

#if UNITY_EDITOR
        
        public virtual void PrepareForBuild()
        {
            gameObject.SetActive(true);
            foreach (var uiObject in uiObjects) uiObject.gameObject.SetActive(false);
            foreach (var element in subUIElements) element.PrepareForBuild();
            var graphics = GetComponents<MaskableGraphic>();
            foreach (MaskableGraphic graphic in graphics) graphic.enabled = false;
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}

