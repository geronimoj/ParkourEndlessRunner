using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InputRebinder : MonoBehaviour
{
    private bool waitForInput = false;

    public KeyToRebind keyRebind = KeyToRebind.Jump;

    GameManager _gm = null;

    Text _inputText = null;

    private void Awake()
    {
        GameObject gm = GameObject.FindGameObjectWithTag("GameManager");
        if (gm == null)
            Debug.LogError("Cannot find Game manager. Missing tag or does not exist");
        _gm = gm.GetComponent<GameManager>();
        _inputText = GetComponentInChildren<Text>();
    }

    private void Start()
    {   //Updated the text to be the default input keys. Since we aren't yet saving inputs this is fine.
        switch (keyRebind)
        {
            case KeyToRebind.Jump:
                _inputText.text = "Jump Key: " + KeyCode.Space;
                break;
            case KeyToRebind.Slide:
                _inputText.text = "Slide Key: " + KeyCode.LeftShift;
                break;
            case KeyToRebind.DodgeL:
                _inputText.text = "Dodge Left Key: " + KeyCode.A;
                break;
            case KeyToRebind.DodgeR:
                _inputText.text = "Dodge Right Key: " + KeyCode.D;
                break;
        }
    }

    void Update()
    {
        if (waitForInput)
            if (Input.anyKeyDown)
            {
                waitForInput = false;
                //This isn't a particularly efficient method but its simple and will only be called from a button press
                //So it, at least, won't be called too often
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                    if (Input.GetKeyDown(key))
                    {   //We only need to re-write the text when the input is pressed
                        //We have to read the key back from the player since its possible to hand in an invalid button
                        switch (keyRebind)
                        {
                            case KeyToRebind.Jump:
                                Player.player.JumpKey = key;
                                _inputText.text = "Jump Key: " + Player.player.JumpKey;
                                break;
                            case KeyToRebind.Slide:
                                Player.player.SlideKey = key;
                                _inputText.text = "Slide Key: " + Player.player.SlideKey;
                                break;
                            case KeyToRebind.DodgeL:
                                Player.player.DodgeLeftKey = key;
                                _inputText.text = "Dodge Left Key: " + Player.player.DodgeLeftKey;
                                break;
                            case KeyToRebind.DodgeR:
                                Player.player.DodgeRightKey = key;
                                _inputText.text = "Dodge Right Key: " + Player.player.DodgeRightKey;
                                break;
                        }
                    }
            }
    }

    public void RebindKey()
    {
        waitForInput = true;
        if (_gm != null)
            _gm.RebindingInput();
        //Its the switch statement again but this time, we are going to update it to say its nothing
        //Thus telling the player its waiting for at least an input
        switch (keyRebind)
        {
            case KeyToRebind.Jump:
                _inputText.text = "Jump Key: ";
                break;
            case KeyToRebind.Slide:
                _inputText.text = "Slide Key: ";
                break;
            case KeyToRebind.DodgeL:
                _inputText.text = "Dodge Left Key: ";
                break;
            case KeyToRebind.DodgeR:
                _inputText.text = "Dodge Right Key: ";
                break;
        }
    }
}

[System.Serializable]
public enum KeyToRebind
{
    Jump = 0,
    Slide,
    DodgeL,
    DodgeR
}
