using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drink : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform commandDisplayParent;

    [Header("Key Sprites")]
    [SerializeField] private Sprite spriteW;
    [SerializeField] private Sprite spriteA;
    [SerializeField] private Sprite spriteS;
    [SerializeField] private Sprite spriteD;

    [Header("Drink Manager")]
    [SerializeField] private DrinkManager drinkManager;

    [System.Serializable]
    public class CommandButtonBinding
    {
        public Button button;
        public string commandName;
    }

    [SerializeField] private List<CommandButtonBinding> commandButtons;

    private DrinkData currentDrink;
    private List<KeyCode> currentInput = new List<KeyCode>();
    private bool isListening = false;

    void Start()
    {
        // 버튼에 커맨드 이름 바인딩
        foreach (var binding in commandButtons)
        {
            var captured = binding;
            if (captured.button != null)
            {
                captured.button.onClick.AddListener(() =>
                {
                    StartListening(captured.commandName);
                });
            }
        }
    }

    void Update()
    {
        if (!isListening || currentDrink == null) return;

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.W)) AddInput(KeyCode.W);
            if (Input.GetKeyDown(KeyCode.A)) AddInput(KeyCode.A);
            if (Input.GetKeyDown(KeyCode.S)) AddInput(KeyCode.S);
            if (Input.GetKeyDown(KeyCode.D)) AddInput(KeyCode.D);
        }
    }

    public void StartListening(string commandName)
    {
        Debug.Log($"커맨드 입력 시작: {commandName}");

        currentDrink = drinkManager.GetDrinkByName(commandName);
        if (currentDrink == null)
        {
            Debug.LogWarning("해당 커맨드가 없습니다.");
            return;
        }

        isListening = true;
        currentInput.Clear();

        // 기존 UI 클리어
        foreach (Transform child in commandDisplayParent)
        {
            Destroy(child.gameObject);
        }

        int index = 0;
        foreach (KeyCode key in currentDrink.sequence)
        {
            GameObject go = new GameObject("Command_" + key);
            go.transform.SetParent(commandDisplayParent, false);

            Image img = go.AddComponent<Image>();
            img.sprite = GetSpriteForKey(key);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(100, 100);
            rt.anchoredPosition = new Vector2(100 * index, 0); // ? 다시 오른쪽으로 100씩 이동

            index++;
        }


    }


    private void AddInput(KeyCode key)
    {
        int index = currentInput.Count;

        if (index >= currentDrink.sequence.Count)
        {
            ResetCommand();
            return;
        }

        if (key == currentDrink.sequence[index])
        {
            currentInput.Add(key);
            Debug.Log("현재 입력: " + string.Join(" ", currentInput));

            if (currentInput.Count == currentDrink.sequence.Count)
            {
                Debug.Log($"커맨드 성공: {currentDrink.drinkName}");
                ResetCommand();
            }
        }
        else
        {
            Debug.Log("잘못된 입력. 초기화.");
            ResetCommand();
        }
    }

    private void ResetCommand()
    {
        isListening = false;
        currentInput.Clear();
        currentDrink = null;

        // UI 초기화
        foreach (Transform child in commandDisplayParent)
        {
            Destroy(child.gameObject);
        }
    }

    private Sprite GetSpriteForKey(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W: return spriteW;
            case KeyCode.A: return spriteA;
            case KeyCode.S: return spriteS;
            case KeyCode.D: return spriteD;
            default: return null;
        }
    }
}
