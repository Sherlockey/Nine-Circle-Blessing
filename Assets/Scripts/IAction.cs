using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    public event EventHandler<bool> OnOnClick;

    public void OnClick();

    public void Execute(CharacterBattle sourceCharacterBattle, CharacterBattle targetCharacterBattle);

    public void PayCost();
}
