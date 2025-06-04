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

    [Header("UI Text")]
    [SerializeField] private Text earningsText;

    [Header("UI Sprites")]
    [SerializeField] private Sprite spriteW;
    [SerializeField] private Sprite spriteA;
    [SerializeField] private Sprite spriteS;
    [SerializeField] private Sprite spriteD;
    [SerializeField] private Sprite speechBubbleSprite;
    [SerializeField] private List<GameObject> drinkSelectionList;

    [Header("Managers")]
    [SerializeField] private DrinkManager drinkManager;
    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private BattleManager battleManager;

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
    private List<KeyCode> allowedDrinkKeys = new List<KeyCode>();
    private GameObject currentCustomerGO;
    private Color originalCustomerColor = Color.white;

    private bool isListening = false;
    private bool isCustomerWaiting = false;
    private float totalEarnings = 0f;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            for (var i = 0; i < allowedDrinkKeys.Count; i++)
            {
                Console.WriteLine(allowedDrinkKeys[i]);
            }
            if (!isListening)
            {
                CheckKey(KeyCode.W);
                CheckKey(KeyCode.A);
                CheckKey(KeyCode.S);
                CheckKey(KeyCode.D);
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

    private void CheckKey(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            if (!allowedDrinkKeys.Any() ||  allowedDrinkKeys[0] == key)
            {
                SelectDrink(key);
            }
            else
            {
                Debug.Log($"❌ 이 음료는 손님이 원하지 않습니다! ({key})");
                HighlightCustomerWarning();
            }
        }
    }


    public void SelectDrink(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W: 
                StartListening("Americano"); 
                break;
            case KeyCode.A:
                StartListening("Lemonade");
                break;
            case KeyCode.S:
                StartListening("Porridge");
                break;
            case KeyCode.D:
                StartListening("Wine");
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
                if (isCustomerWaiting)
                {
                    Debug.Log("🎯 손님이 원하는 음료를 만들었습니다!");

                    allowedDrinkKeys.RemoveAt(0);
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
                    List<DrinkQueue> drinkList = new List<DrinkQueue>(drinkQueue);
                    drinkQueue.Clear();
                    battleManager.DrinkBehavior(drinkList);
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
    private void UpdateEarningsUI()
    {
        earningsText.text = $"💰 {Mathf.FloorToInt(totalEarnings)} $";
    }

    public void SpawnCustomer()
    {
        CustomerData randomCustomer = customerManager.GetRandomCustomer();
        CustomerQueue queue = new CustomerQueue(null, randomCustomer);
        customerQueue.Enqueue(queue);
        isCustomerWaiting = true;

        switch (queue.customerData.order)
        {
            case "Americano":
                allowedDrinkKeys.Add(KeyCode.W);
                break;
            case "Lemonade":
                allowedDrinkKeys.Add(KeyCode.A);
                break;
            case "Porridge":
                allowedDrinkKeys.Add(KeyCode.S);
                break;
            case "Wine":
                allowedDrinkKeys.Add(KeyCode.D);
                break;
        }

        UpdateCustomerQueueUI();

    }

    private void HighlightCustomerWarning()
    {
        if (currentCustomerGO != null)
        {
            Image img = currentCustomerGO.GetComponent<Image>();
            if (img != null)
            {
                img.color = Color.red;
                CancelInvoke(nameof(ResetCustomerHighlight));
                Invoke(nameof(ResetCustomerHighlight), 0.5f);
            }
        }
    }

    private void ResetCustomerHighlight()
    {
        if (currentCustomerGO != null)
        {
            Image img = currentCustomerGO.GetComponent<Image>();
            if (img != null)
            {
                img.color = originalCustomerColor;
            }
        }
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
