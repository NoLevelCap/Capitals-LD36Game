using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Morality 
{
	Good, Evil, Neutral
}

public enum Type
{
	Buisness, Science
}

public class Effect {
	public float MPPIncrease;

	public float MoneyIncrease;

	public float NextTechIncrease;

	public float GenTechIncrease;

	public float PopIncrease;

	public Effect(float PopIncrease, float MPPIncrease, float MoneyIncrease, float NextTechIncrease, float GenTechIncrease){
		this.PopIncrease = PopIncrease;
		this.MPPIncrease = MPPIncrease;
		this.MoneyIncrease = MoneyIncrease;
		this.NextTechIncrease = NextTechIncrease;
		this.GenTechIncrease = GenTechIncrease;
	}
}

public class Token {
	public Token parent;
	public List<Token> children;

	public string Name;

	public string Desc;

	public int UpPoints;

	public int Progress = 0;

	public Effect Effect;

	private Morality morality;

	public Type Type;

	public bool Unlocked;

	public Token(Type type, string name, Morality morals, Effect effect, int upPoints, string desc){
		children = new List<Token> ();
		Type = type;
		Name = name;
		morality = morals;
		UpPoints = upPoints;
		Desc = desc;
		Effect = effect;
		Unlocked = false;
	}

	public void setEffect(float PopIncrease, float MPPIncrease, float MoneyIncrease, float NextTechIncrease, float GenTechIncrease){
		this.Effect = new Effect (PopIncrease, MPPIncrease, MoneyIncrease, NextTechIncrease, GenTechIncrease);
	}

	public void setParent(Token parent){
		this.parent = parent;
		this.parent.children.Add (this);
	}
		
}
