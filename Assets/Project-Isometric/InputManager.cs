using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Single<InputManager>
{
    private Dictionary<string, KeyInfo> _keyInfos;

    public InputManager() : base()
    {
        _keyInfos = new Dictionary<string, KeyInfo>();

        _keyInfos.Add("move_up", new KeyInfo("Move Up", KeyCode.W));
        _keyInfos.Add("move_left", new KeyInfo("Move Left", KeyCode.A));
        _keyInfos.Add("move_down", new KeyInfo("Move Down", KeyCode.S));
        _keyInfos.Add("move_right", new KeyInfo("Move Right", KeyCode.D));
        _keyInfos.Add("jump", new KeyInfo("Jump", KeyCode.Space));
        _keyInfos.Add("sprint", new KeyInfo("Sprint", KeyCode.LeftShift));
        _keyInfos.Add("drop_item", new KeyInfo("Drop Item", KeyCode.T));
        _keyInfos.Add("inventory", new KeyInfo("Inventory", KeyCode.I));
    }

    public KeyInfo GetKeyInfo(string key)
    {
        return _keyInfos[key];
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
    private Action _callback;

    public CommandCallback(Action callback)
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

public class CommandDelegate
{
    private List<KeyCommandPair> _commands;

    public CommandDelegate()
    {
        _commands = new List<KeyCommandPair>();
    }

    public CommandDelegate(KeyCommandPair[] commands)
    {
        _commands = new List<KeyCommandPair>(commands);
    }

    public void Add(string key, ICommand command)
    {
        KeyCommandPair pair = new KeyCommandPair();

        pair.command = command;
        pair.keyInfo = InputManager.Instance.GetKeyInfo(key);

        _commands.Add(pair);
    }

    public KeyCommandPair[] ToArray()
    {
        return _commands.ToArray();
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

public struct KeyCommandPair
{
    public ICommand command;

    public KeyInfo keyInfo;
}

public class KeyInfo
{
    public string keyName;

    public KeyCode keyCode;

    public KeyInfo(string keyName, KeyCode keyCode)
    {
        this.keyName = keyName;
        this.keyCode = keyCode;
    }
}