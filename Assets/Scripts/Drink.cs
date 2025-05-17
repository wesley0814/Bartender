using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drink : MonoBehaviour
{
    [Header("UI Spawn Point")]
    [SerializeField] private Transform commandSpawnPoint;
    [SerializeField] private Transform drinkSpawnPoint;
    [SerializeField] private Transform customerQueueSpawnPoint;
    [SerializeField] private Transform drinkQueueSpawnPoint;

    [Header("UI Text")]
    [SerializeField] private Text earningsText;

    [Header("UI Sprites")]
    [SerializeField] private Sprite spriteW;
    [SerializeField] private Sprite spriteA;
    [SerializeField] private Sprite spriteS;
    [SerializeField] private Sprite spriteD;
    [SerializeField] private Sprite speechBubbleSprite;

    [Header("Managers")]
    [SerializeField] private DrinkManager drinkManager;
    [SerializeField] private CustomerManager customerManager;

    [System.Serializable]
    public class CommandButtonBinding
    {
        public Button button;
        public string commandName;
    }

    [SerializeField] private List<CommandButtonBinding> commandButtons;

    // Make drink and put in queue
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
    // Make customer and put in queue
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
    private Queue<DrinkQueue> drinkQueue = new Queue<DrinkQueue>();
    private Queue<CustomerQueue> customerQueue = new Queue<CustomerQueue>();

    private DrinkData currentDrink;
    private List<KeyCode> currentInput = new List<KeyCode>();
    private List<GameObject> commandUIObjects = new List<GameObject>();
    private bool isListening = false;
    private float totalEarnings = 0f;

    void Start()
    {
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
        if (!isListening || currentDrink == null)
        {
            CheckServeInput();
            return;
        }

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.W)) AddInput(KeyCode.W);
            if (Input.GetKeyDown(KeyCode.A)) AddInput(KeyCode.A);
            if (Input.GetKeyDown(KeyCode.S)) AddInput(KeyCode.S);
            if (Input.GetKeyDown(KeyCode.D)) AddInput(KeyCode.D);
        }

        CheckServeInput();
    }

    private void CheckServeInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ServeDrink(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ServeDrink(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ServeDrink(3);
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
        if (currentDrink.drinkSprite != null)
        {
            GameObject drinkGO = new GameObject("DrinkSprite");
            drinkGO.transform.SetParent(drinkSpawnPoint, false);

            Image img = drinkGO.AddComponent<Image>();
            img.sprite = currentDrink.drinkSprite;

            RectTransform rt = img.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
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
                DrinkQueue queue = new DrinkQueue(null, currentDrink);
                drinkQueue.Enqueue(queue);

                UpdateDrinkQueueUI();
                ResetCommand();
            }
        }
        else
        {
            Debug.Log("Failed");
            ResetCommand();
        }
    }

    /*private void UpdateCommandUI()
    {
        foreach (Transform child in commandSpawnPoint)
        {
            Destroy(child.gameObject);
        }
        commandUIObjects.Clear();

        int remainingCount = currentDrink.sequence.Count - currentInput.Count;
        for (int i = 0; i < remainingCount; i++)
        {
            KeyCode key = currentDrink.sequence[currentInput.Count + i];

            GameObject go = new GameObject("Command_" + key + "_" + i);
            go.transform.SetParent(commandSpawnPoint, false);

            Image img = go.AddComponent<Image>();
            img.sprite = GetSpriteForKey(key);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(80, 80);
            rt.anchoredPosition = new Vector2(80 * i, 0);

            commandUIObjects.Add(go);

            if (i == 0)
            {
                img.color = Color.yellow;
            }
            else
            {
                img.color = Color.white;
            }
        }
    }*/

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
                rt.sizeDelta = new Vector2(80, 80);
                rt.anchoredPosition = new Vector2(80 * i, 0);

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
            GameObject drinkGO = new GameObject("Drink_" + queue.drinkData.drinkName);
            drinkGO.transform.SetParent(drinkQueueSpawnPoint, false);

            Image img = drinkGO.AddComponent<Image>();
            img.sprite = queue.drinkData.drinkSprite;

            RectTransform rt = drinkGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);
            rt.anchoredPosition = new Vector2(index * 80f, 0);

            queue.drinkObject = drinkGO;
            index++;
        }
    }


    private void ServeDrink(int queueNumber)
    {
        if (drinkQueue.Count == 0)
            return;

        DrinkQueue queue = null;

        switch (queueNumber)
        {
            // 1. Sell
            case 1:
                queue = drinkQueue.Dequeue();
                Debug.Log($"SELL '{queue.drinkObject.name}'");
                totalEarnings += queue.drinkData.drinkPrice;
                Destroy(queue.drinkObject);
                UpdateEarningsUI();
                UpdateDrinkQueueUI();
                break;
            // 2. Give guard
            case 2:
                queue = drinkQueue.Dequeue();
                Debug.Log($"GIVE GUARD '{queue.drinkObject.name}'");
                Destroy(queue.drinkObject);
                UpdateEarningsUI();
                UpdateDrinkQueueUI();
                break;
            // 3. Sell to customer
            case 3:
                if (customerQueue.Count > 0)
                {
                    queue = drinkQueue.Peek();
                    CustomerQueue customer = customerQueue.Peek();
                    
                    if (queue.drinkData.drinkName.Equals(customer.customerData.order))
                    {
                        drinkQueue.Dequeue();
                        customerQueue.Dequeue();

                        Debug.Log($"SELL TO CUSTOMER '{queue.drinkObject.name}'");
                        totalEarnings += queue.drinkData.drinkPrice;

                        Destroy(queue.drinkObject);
                        Destroy(customer.customerObject);

                        UpdateEarningsUI();
                        UpdateDrinkQueueUI();
                        UpdateCustomerQueueUI();
                    }
                    else
                    {
                        Debug.Log("Don't order this drink");
                    }
                }
                break;
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
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in drinkSpawnPoint)
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
    private void UpdateEarningsUI()
    {
        earningsText.text = $"💰 {Mathf.FloorToInt(totalEarnings)} $";
    }

    public void SpawnCustomer()
    {
        CustomerData randomCustomer = customerManager.GetRandomCustomer();
        CustomerQueue queue = new CustomerQueue(null, randomCustomer);
        customerQueue.Enqueue(queue);

        UpdateCustomerQueueUI();
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

            DrinkData orderedDrink = drinkManager.GetDrinkByName(queue.customerData.order);
            if (orderedDrink != null && orderedDrink.drinkSprite != null && speechBubbleSprite != null)
            {
                GameObject bubbleGO = new GameObject("SpeechBubble");
                bubbleGO.transform.SetParent(customerGO.transform, false);

                Image bubbleImg = bubbleGO.AddComponent<Image>();
                bubbleImg.sprite = speechBubbleSprite;

                RectTransform bubbleRT = bubbleGO.GetComponent<RectTransform>();
                bubbleRT.sizeDelta = new Vector2(80, 80);
                bubbleRT.anchoredPosition = new Vector2(0, 70);

                GameObject drinkIcon = new GameObject("DrinkOrderIcon");
                drinkIcon.transform.SetParent(bubbleGO.transform, false);

                Image drinkImg = drinkIcon.AddComponent<Image>();
                drinkImg.sprite = orderedDrink.drinkSprite;

                RectTransform drinkRT = drinkIcon.GetComponent<RectTransform>();
                drinkRT.sizeDelta = new Vector2(30, 30);
                drinkRT.anchoredPosition = Vector2.zero;
            }

            index++;
        }
    }

}
