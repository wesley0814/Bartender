using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drink : MonoBehaviour
{
    [SerializeField] private Button drinkButton;

    private List<KeyCode> commandSequence = new List<KeyCode> {
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.W, KeyCode.W, KeyCode.D
    };

    private List<KeyCode> currentInput = new List<KeyCode>();

    private bool isListening = false;

    void Start()
    {
        // ��ư�� �Լ� ����
        if (drinkButton != null)
        {
            drinkButton.onClick.AddListener(StartListening);
        }
    }

    void Update()
    {
        InputCommand();
    }

    public void InputCommand()
    {
        if (!isListening) return;

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.A)) AddInput(KeyCode.A);
            if (Input.GetKeyDown(KeyCode.S)) AddInput(KeyCode.S);
            if (Input.GetKeyDown(KeyCode.D)) AddInput(KeyCode.D);
            if (Input.GetKeyDown(KeyCode.W)) AddInput(KeyCode.W);
        }
    }

    public void StartListening()
    {
        Debug.Log("Ŀ�ǵ� �Է� ����!");
        isListening = true;
        currentInput.Clear();
    }

    private void AddInput(KeyCode key)
    {
        // ���� �Է��� �������� �ش� ��ġ�� ��ġ�ϴ��� Ȯ��
        int nextIndex = currentInput.Count;

        // �Է� �ʰ� ����
        if (nextIndex >= commandSequence.Count)
        {
            ResetCommand();
            return;
        }

        if (key == commandSequence[nextIndex])
        {
            currentInput.Add(key);

            // ���� �Է� ���
            string inputSoFar = string.Join(" ", currentInput);
            Debug.Log("���� �Է�: " + inputSoFar);

            // ���� ����
            if (currentInput.Count == commandSequence.Count)
            {
                Debug.Log("Ŀ�ǵ� ����!");
                ResetCommand();
            }
        }
        else
        {
            Debug.Log("�߸��� �Է�. �ʱ�ȭ.");
            ResetCommand();
        }
    }

    private void ResetCommand()
    {
        currentInput.Clear();
    }
}
