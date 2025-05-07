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
        // 버튼에 함수 연결
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
        Debug.Log("커맨드 입력 시작!");
        isListening = true;
        currentInput.Clear();
    }

    private void AddInput(KeyCode key)
    {
        // 다음 입력이 시퀀스의 해당 위치와 일치하는지 확인
        int nextIndex = currentInput.Count;

        // 입력 초과 방지
        if (nextIndex >= commandSequence.Count)
        {
            ResetCommand();
            return;
        }

        if (key == commandSequence[nextIndex])
        {
            currentInput.Add(key);

            // 현재 입력 출력
            string inputSoFar = string.Join(" ", currentInput);
            Debug.Log("현재 입력: " + inputSoFar);

            // 성공 판정
            if (currentInput.Count == commandSequence.Count)
            {
                Debug.Log("커맨드 성공!");
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
        currentInput.Clear();
    }
}
