using System.Collections.Generic;
using UnityEngine;

public class InputManager : Single<InputManager>
{
    private class KeyInfo
    {
        public string keyName;
        public KeyCode keyCode;

        public KeyInfo(string keyName, KeyCode keyCode)
        {
            this.keyName = keyName;
            this.keyCode = keyCode;
        }
    }

    private struct KeyCommandPair
    {
        public ICommand command;
        public KeyInfo keyInfo;
    }

    private Dictionary<string, KeyInfo> _keyInfos;

    private List<KeyCommandPair> _commands;

    public InputManager() : base()
    {
        _keyInfos = new Dictionary<string, KeyInfo>();

        _commands = new List<KeyCommandPair>();

        _keyInfos.Add("move_up", new KeyInfo("Move Up", KeyCode.W));
        _keyInfos.Add("move_left", new KeyInfo("Move Left", KeyCode.A));
        _keyInfos.Add("move_down", new KeyInfo("Move Down", KeyCode.S));
        _keyInfos.Add("move_right", new KeyInfo("Move Right", KeyCode.D));
        _keyInfos.Add("jump", new KeyInfo("Jump", KeyCode.Space));
        _keyInfos.Add("sprint", new KeyInfo("Sprint", KeyCode.LeftShift));
        _keyInfos.Add("drop_item", new KeyInfo("Drop Item", KeyCode.T));
        _keyInfos.Add("inventory", new KeyInfo("Inventory", KeyCode.I));
    }

    public void AddCommand(string key, ICommand command)
    {
        try
        {
            KeyInfo keyInfo = _keyInfos[key];

            KeyCommandPair pair = new KeyCommandPair();

            pair.command = command;
            pair.keyInfo = keyInfo;

            _commands.Add(pair);
        }

        catch (KeyNotFoundException exception)
        {
            Debug.LogError(exception.Message);
        }
    }

    public void RemoveCommand(ICommand command)
    {
        int index = _commands.FindIndex(delegate (KeyCommandPair pair) { return pair.command == command; });

        if (index < 0)
            return;

        _commands.RemoveAt(index);
    }

    public void Update(float deltaTime)
    {
        for (int index = 0; index < _commands.Count; index++)
        {
            ICommand command = _commands[index].command;

            KeyCode key = _commands[index].keyInfo.keyCode;

            if (Input.GetKey(key))
                command.OnKey();

            if (Input.GetKeyDown(key))
                command.OnKeyDown();

            if (Input.GetKeyUp(key))
                command.OnKeyUp();
        }
    }
}

public interface ICommand
{
    void OnKey();
    void OnKeyDown();
    void OnKeyUp();
}

public class CommandCallback : ICommand
{
    private System.Action _callback;

    public CommandCallback(System.Action callback)
    {
        _callback = callback;
    }

    public void OnKey()
    {

    }

    public void OnKeyDown()
    {
        _callback();
    }

    public void OnKeyUp()
    {

    }
}