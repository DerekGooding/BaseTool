using UnityEngine;
using UnityEngine.UIElements;

namespace BaseTool.Core.Consoles
{
    [RequireComponent(typeof(UIDocument))]
    public class ConsoleManager : MonoBehaviour
    {
        private UIDocument _uiDocument;

        private ScrollView _scrollView;
        private TextField _textField;

        private bool _displayed = false;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            _uiDocument.panelSettings = Resources.Load<PanelSettings>("ConsolePanelSettings");
            _uiDocument.visualTreeAsset = Resources.Load<VisualTreeAsset>("ConsoleView");
            _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        }

        private void OnEnable()
        {
            _scrollView = _uiDocument.rootVisualElement.Q<ScrollView>("ConsoleScroll");
            
            _textField = _uiDocument.rootVisualElement.Q<TextField>("ConsoleField");
            _textField.RegisterCallback<KeyDownEvent>(OnConsoleFieldKeyDown, TrickleDown.TrickleDown);
            _textField.RegisterCallback<NavigationSubmitEvent>(e =>
            {
                e.StopImmediatePropagation();
                _textField.ElementAt(0).Focus();
            }, TrickleDown.TrickleDown);
        }

        private void Update()
        {
            Console.ConsoleUpdate();
            
            if (!Input.GetKeyDown(KeyCode.F4)) return;
            Toggle();
        }

        private void Toggle()
        {
            _displayed = !_displayed;
            _uiDocument.rootVisualElement.style.display = 
                _displayed 
                    ? DisplayStyle.Flex 
                    : DisplayStyle.None;
            
            if(_displayed) _textField.ElementAt(0).Focus();
        }

        public void WriteLine(string txt)
        {
            _scrollView.Add(new Label(txt));
        }

        private void OnConsoleFieldKeyDown(KeyDownEvent evt)
        {
            evt.StopImmediatePropagation();
            if (evt.keyCode == KeyCode.Tab)
            {
                var completion = Console.TabComplete(_textField.text).TrimEnd();
                _textField.SetValueWithoutNotify(completion);
                _textField.cursorIndex = completion.Length;
                _textField.selectIndex = completion.Length;
            }
            
            if (evt.keyCode != KeyCode.Return) return;

            Console.EnqueueCommand(_textField.text);
            _textField.SetValueWithoutNotify(null);
            _uiDocument.rootVisualElement.schedule.Execute(_ => _textField.ElementAt(0).Focus()).ExecuteLater(50);
        }
    }
}