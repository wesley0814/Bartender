using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using static Drink;

public class Drink : MonoBehaviour
{
    [Header("UI Spawn Point")]
    [SerializeField] private Transform commandSpawnPoint;
    [SerializeField] private Transform drinkSpawnPoint;
    [SerializeField] private Transform customerQueueSpawnPoint;
    [SerializeField] private Transform drinkQueueSpawnPoint;
    [SerializeField] private Transform skillCheckPoint;
    [SerializeField] private Transform wDrinkIndicatorPoint;
    [SerializeField] private Transform aDrinkIndicatorPoint;
    [SerializeField] private Transform sDrinkIndicatorPoint;
    [SerializeField] private Transform dDrinkIndicatorPoint;

    [Header("UI Text")]
    [SerializeField] private Text countdownText;

    [Header("UI Sprites")]
    [SerializeField] private Sprite spriteW;
    [SerializeField] private Sprite spriteA;
    [SerializeField] private Sprite spriteS;
    [SerializeField] private Sprite spriteD;
    [SerializeField] private Sprite speechBubbleSprite;
    [SerializeField] private List<GameObject> drinkSelectionList;
    [SerializeField] private GameObject skillCheckPrefab;
    [SerializeField] private GameObject wIndicatorPrefab;
    [SerializeField] private GameObject aIndicatorPrefab;
    [SerializeField] private GameObject sIndicatorPrefab;
    [SerializeField] private GameObject dIndicatorPrefab;
    [SerializeField] private GameObject countdownUI;


    [Header("Managers")]
    [SerializeField] private DrinkManager drinkManager;
    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private BattleManager battleManager;

    public class DrinkQueue
    {
        public GameObject drinkObject;
        public DrinkData drinkData;

        public DrinkQueue(GameObject obj, DrinkData data)
        {
            drinkObject = obj;
            drinkData = data;
        }
    }
    public class CustomerQueue
    {
        public GameObject customerObject;
        public CustomerData customerData;

        public CustomerQueue(GameObject obj, CustomerData data)
        {
            customerObject = obj;
            customerData = data;
        }
    }
    private List<DrinkQueue> drinkBehaviorList;
    private Queue<DrinkQueue> drinkQueue = new Queue<DrinkQueue>();
    private Queue<CustomerQueue> customerQueue = new Queue<CustomerQueue>();

    private DrinkData currentDrink;
    private List<KeyCode> currentInput = new List<KeyCode>();
    private List<KeyCode> allowedDrinkKeys = new List<KeyCode>();
    private List<GameObject> commandUIObjects = new List<GameObject>();
    private List<GameObject> activeIndicators = new List<GameObject>();
    private GameObject currentCustomerGO;
    private Color originalCustomerColor = Color.white;
    private KeyCode firstKey;

    public float prepareTime = 3f;
    private bool isListening = false;
    private bool isCustomerWaiting = false;

    void Start()
    {
        StartCoroutine(PrepareAndStartGame());
        StartCoroutine(CustomerSpawnRoutine());
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.anyKeyDown)
            {
                if (!isListening)
                {
                    if (currentInput.Count >= 1 && Input.GetKeyDown(KeyCode.S))
                    {
                        currentInput.Clear();
                        ClearIndicators();
                        return;
                    }
                    if (Input.GetKeyDown(KeyCode.W)) { currentInput.Add(KeyCode.W); AddKeyIndicator(KeyCode.W); }
                    if (Input.GetKeyDown(KeyCode.A)) { currentInput.Add(KeyCode.A); AddKeyIndicator(KeyCode.A); }
                    if (Input.GetKeyDown(KeyCode.S)) { currentInput.Add(KeyCode.S); AddKeyIndicator(KeyCode.S); }
                    if (Input.GetKeyDown(KeyCode.D)) { currentInput.Add(KeyCode.D); AddKeyIndicator(KeyCode.D); }

                    var foundDrink = drinkManager.FindDrinkByKeys(currentInput);
                    Debug.Log("🔸 현재 입력: " + string.Join(", ", currentInput));
                    if (isCustomerWaiting)
                    {
                        if(currentInput.Count >= 2)
                        {
                            if (IsAllowedDrink(foundDrink))
                            {
                                Debug.Log($"✅ 음료 선택됨: {foundDrink.drinkName}");
                                StartListening(foundDrink.drinkName);
                                currentInput.Clear();
                                ClearIndicators();
                            }
                            else if (!IsAllowedDrink(foundDrink))
                            {
                                HighlightCustomerWarning();
                                Debug.Log($"이 음료를 원하지 않습니다.{foundDrink.drinkName}");
                                currentInput.Clear();
                                ClearIndicators();
                            }
                            currentInput.Clear();
                            ClearIndicators();
                        }
                    }
                    else
                    {
                        if (foundDrink != null)
                        {
                            Debug.Log($"✅ 음료 선택됨: {foundDrink.drinkName}");
                            StartListening(foundDrink.drinkName);
                            currentInput.Clear();
                            ClearIndicators();
                        }
                        else if (currentInput.Count >= 2)
                        {
                            Debug.Log("🔸 현재 입력: " + string.Join(", ", currentInput));
                            Debug.Log("❌ 유효하지 않은 조합입니다.");
                            currentInput.Clear();
                            ClearIndicators();
                        }
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.W)) AddInput(KeyCode.W);
                    if (Input.GetKeyDown(KeyCode.A)) AddInput(KeyCode.A);
                    if (Input.GetKeyDown(KeyCode.S)) AddInput(KeyCode.S);
                    if (Input.GetKeyDown(KeyCode.D)) AddInput(KeyCode.D);
                }
            }
        }
    }

    private IEnumerator PrepareAndStartGame()
    {
        Time.timeScale = 0f;
        countdownUI.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSecondsRealtime(1f);
        countdownText.text = "2";
        yield return new WaitForSecondsRealtime(1f);
        countdownText.text = "1";
        yield return new WaitForSecondsRealtime(1f);
        countdownText.text = "Start!";
        yield return new WaitForSecondsRealtime(1f);

        countdownUI.SetActive(false);
        Time.timeScale = 1f;
    }

    private bool IsAllowedDrink(DrinkData drink)
    {
        for (int i = 0; i < 2; i++)
        {
            if (drink.selectKey[i] != allowedDrinkKeys[i])
                return false;
        }

        return true;
    }

    private void AddKeyIndicator(KeyCode key)
    {
        GameObject indicator;
        switch (key)
        {
            case KeyCode.W:
                indicator = Instantiate(wIndicatorPrefab, wDrinkIndicatorPoint);
                break;
            case KeyCode.A:
                indicator = Instantiate(aIndicatorPrefab, aDrinkIndicatorPoint);
                break;
            case KeyCode.S:
                indicator = Instantiate(sIndicatorPrefab, sDrinkIndicatorPoint);
                break;
            case KeyCode.D:
                indicator = Instantiate(dIndicatorPrefab, dDrinkIndicatorPoint);
                break;
            default: return;
        }
        var text = indicator.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (text != null)
        {
            text.text = key.ToString();
        }
        activeIndicators.Add(indicator);
    }


    private void ClearIndicators()
    {
        foreach (var indicator in activeIndicators)
        {
            Destroy(indicator);
        }
        activeIndicators.Clear();
    }

    public void SelectDrink(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W: 
                StartListening("Screwdriver"); 
                break;
            case KeyCode.A:
                StartListening("Gin & Tonic");
                break;
            case KeyCode.S:
                StartListening("Margarita");
                break;
            case KeyCode.D:
                StartListening("Manhattan");
                break;
        }
    }

    public void StartListening(string commandName)
    {
        Debug.Log($"Start '{commandName}'");

        ResetCommand();
        isListening = true;

        currentDrink = drinkManager.GetDrinkByName(commandName);
        if (currentDrink == null)
        {
            Debug.LogWarning("NULL");
            return;
        }

        foreach (Transform child in commandSpawnPoint)
        {
            Destroy(child.gameObject);
        }

        UpdateCommandUI();

        // Print 'drink' image
        if (currentDrink.drinkPrefab != null)
        {
            GameObject drinkGO = Instantiate(currentDrink.drinkPrefab, drinkSpawnPoint);
            RectTransform rt = drinkGO.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = Vector2.zero;
            }
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

            UpdateCommandUI();

            // Finished
            if (currentInput.Count == currentDrink.sequence.Count)
            {
                if (isCustomerWaiting)
                {
                    Debug.Log("🎯 손님이 원하는 음료를 만들었습니다!");

                    allowedDrinkKeys.Clear();
                    customerQueue.Dequeue();
                    UpdateCustomerQueueUI();
                    ResetCommand();

                    if (!allowedDrinkKeys.Any())
                    {
                        isCustomerWaiting = false;
                    }

                    return;
                }

                DrinkQueue queue = new DrinkQueue(null, currentDrink);
                drinkQueue.Enqueue(queue);

                UpdateDrinkQueueUI();
                ResetCommand();

                if (drinkQueue.Count == 3)
                {
                    drinkBehaviorList = new List<DrinkQueue>(drinkQueue);
                    StartCoroutine(SkillCheckCoroutine());
                    drinkQueue.Clear();
                }
            }
        }
        else
        {
            Debug.Log("Failed");
            ResetCommand();
        }
    }

    private void UpdateCommandUI()
    {
        if (commandUIObjects.Count == 0 && currentDrink != null)
        {
            for (int i = 0; i < currentDrink.sequence.Count; i++)
            {
                KeyCode key = currentDrink.sequence[i];

                GameObject go = new GameObject("Command_" + key + "_" + i);
                go.transform.SetParent(commandSpawnPoint, false);

                Image img = go.AddComponent<Image>();
                img.sprite = GetSpriteForKey(key);

                RectTransform rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(60, 60);
                rt.anchoredPosition = new Vector2(60 * i, 0);

                commandUIObjects.Add(go);
            }
        }

        for (int i = 0; i < commandUIObjects.Count; i++)
        {
            Image img = commandUIObjects[i].GetComponent<Image>();

            if (i < currentInput.Count)
            {
                img.color = new Color(1f, 1f, 1f, 0.3f);
            }
            else if (i == currentInput.Count)
            {
                img.color = Color.yellow;
            }
            else
            {
                img.color = Color.white;
            }
        }
    }


    private void UpdateDrinkQueueUI()
    {
        foreach (Transform child in drinkQueueSpawnPoint)
        {
            Destroy(child.gameObject);
        }

        int index = 0;
        foreach (DrinkQueue queue in drinkQueue)
        {
            if (queue.drinkData.drinkPrefab != null)
            {
                GameObject drinkGO = Instantiate(queue.drinkData.drinkPrefab, drinkQueueSpawnPoint);
                RectTransform rt = drinkGO.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchoredPosition = new Vector2(index * 60f, 0);
                }

                queue.drinkObject = drinkGO;
                index++;
            }
        }
    }

    private void ResetCommand()
    {
        isListening = false;
        currentInput.Clear();
        currentDrink = null;

        foreach (var obj in commandUIObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        commandUIObjects.Clear();

        foreach (Transform child in commandSpawnPoint)
            Destroy(child.gameObject);

        foreach (Transform child in drinkSpawnPoint)
            Destroy(child.gameObject);
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

    public void SpawnCustomer()
    {
        CustomerData randomCustomer = customerManager.GetRandomCustomer();
        CustomerQueue queue = new CustomerQueue(null, randomCustomer);
        customerQueue.Enqueue(queue);
        isCustomerWaiting = true;

        DrinkData drink = drinkManager.GetDrinkByName(queue.customerData.order);

        for (int i = 0; i < drink.selectKey.Count; i++)
        {
            allowedDrinkKeys.Add(drink.selectKey[i]);
        }

        UpdateCustomerQueueUI();

    }

    IEnumerator CustomerSpawnRoutine()
    {
        yield return new WaitForSeconds(20f);

        while (true)
        {
            while (isListening)
            {
                yield return null;
            }

            SpawnCustomer();

            yield return new WaitForSeconds(20f);
        }
    }


    private IEnumerator SkillCheckCoroutine()
    {
        bool isSuccess = false;

        while (!isSuccess)
        {
            GameObject barGO = Instantiate(skillCheckPrefab, skillCheckPoint);
            RectTransform barRT = barGO.GetComponent<RectTransform>();
            barRT.anchoredPosition = Vector2.zero;

            RectTransform checkBarRT = barGO.transform.Find("Check Bar").GetComponent<RectTransform>();
            RectTransform successZoneRT = barGO.transform.Find("Success Zone").GetComponent<RectTransform>();
            RectTransform barArea = barGO.GetComponent<RectTransform>();

            float barWidth = barArea.rect.width;
            float handleWidth = checkBarRT.rect.width;

            float speed = 200f;
            float direction = 1f;
            bool keyPressed = false;

            while (!keyPressed)
            {
                float deltaX = direction * speed * Time.deltaTime;
                float nextX = checkBarRT.anchoredPosition.x + deltaX;

                float leftBound = -barWidth / 2f + handleWidth / 2f;
                float rightBound = barWidth / 2f - handleWidth / 2f;

                if (nextX > rightBound)
                {
                    nextX = rightBound;
                    direction = -1f;
                }
                else if (nextX < leftBound)
                {
                    nextX = leftBound;
                    direction = 1f;
                }

                checkBarRT.anchoredPosition = new Vector2(nextX, checkBarRT.anchoredPosition.y);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    keyPressed = true;

                    float handleLeft = checkBarRT.anchoredPosition.x - handleWidth / 2f;
                    float handleRight = checkBarRT.anchoredPosition.x + handleWidth / 2f;

                    float zoneLeft = successZoneRT.anchoredPosition.x - successZoneRT.rect.width / 2f;
                    float zoneRight = successZoneRT.anchoredPosition.x + successZoneRT.rect.width / 2f;

                    if (handleLeft >= zoneLeft && handleRight <= zoneRight)
                    {
                        battleManager.DrinkBehavior(drinkBehaviorList);
                        UpdateDrinkQueueUI();
                        isSuccess = true;
                    }
                    else
                    {
                        Debug.Log("❌ 실패! 다시 시도합니다...");
                    }
                }

                yield return null;
            }

            Destroy(barGO);
            yield return new WaitForSeconds(0.2f); // 약간의 딜레이 후 재시도
        }
    }


    private void HighlightCustomerWarning()
    {
        if (currentCustomerGO != null)
        {
            StartCoroutine(ShakeCustomer(currentCustomerGO.transform));
        }
    }

    private IEnumerator ShakeCustomer(Transform target)
    {
        Vector3 originalPos = target.localPosition;

        float shakeDuration = 0.5f;
        float shakeMagnitude = 5f;
        float shakeSpeed = 40f;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float xOffset = Mathf.Sin(elapsed * shakeSpeed) * shakeMagnitude;
            target.localPosition = originalPos + new Vector3(xOffset, 0f, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localPosition = originalPos;
    }


    private void UpdateCustomerQueueUI()
    {
        foreach (Transform child in customerQueueSpawnPoint)
        {
            Destroy(child.gameObject);
        }

        int index = 0;
        foreach (CustomerQueue queue in customerQueue)
        {
            GameObject customerGO = new GameObject("CustomerSprite");
            customerGO.transform.SetParent(customerQueueSpawnPoint, false);

            Image customerImg = customerGO.AddComponent<Image>();
            customerImg.sprite = queue.customerData.customerSprite;

            RectTransform customerRT = customerGO.GetComponent<RectTransform>();
            customerRT.sizeDelta = new Vector2(100, 100);
            customerRT.anchoredPosition = new Vector2(index * 80f, 0);

            queue.customerObject = customerGO;

            if (index == 0)
            {
                currentCustomerGO = customerGO;
            }

            DrinkData orderedDrink = drinkManager.GetDrinkByName(queue.customerData.order);
            if (orderedDrink != null && orderedDrink.drinkPrefab != null && speechBubbleSprite != null)
            {
                GameObject bubbleGO = new GameObject("SpeechBubble");
                bubbleGO.transform.SetParent(customerGO.transform, false);

                Image bubbleImg = bubbleGO.AddComponent<Image>();
                bubbleImg.sprite = speechBubbleSprite;

                RectTransform bubbleRT = bubbleGO.GetComponent<RectTransform>();
                bubbleRT.sizeDelta = new Vector2(85, 85);
                bubbleRT.anchoredPosition = new Vector2(0, 70);

                GameObject drinkIcon = Instantiate(orderedDrink.drinkPrefab, bubbleGO.transform);
                RectTransform drinkRT = drinkIcon.GetComponent<RectTransform>();
                if (drinkRT != null)
                {
                    drinkRT.anchoredPosition = Vector2.zero;
                }
            }
            index++;
        }

    }

}
