using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Morality 
{
	Good, Evil, Neutral
}

public enum Type
{
	Buisness, Science, Neutral
}
	

public abstract class AEffect : ScriptableObject {

	private bool firstTrigger;
	public Token parent;

	public AEffect(){
		this.firstTrigger = true;
	}

	public void SetParent(Token Parent){
		this.parent = Parent;
	}

	public void OnDown(CityGenerator block){
	}

	public void TriggerEffect(CityGenerator block){
		if (!firstTrigger) {
			MainEffect (block);
		} else {
			FirstTrigger (block);
		}
	}



	public void FirstTrigger(CityGenerator block){
		PreEffect (block);
		firstTrigger = false;
	}

	public abstract void PreEffect (CityGenerator block);
	public abstract void MainEffect (CityGenerator block);

	public abstract void OnDestoy (CityGenerator block);
	public abstract int[] GetIconTypes();
	public abstract int getCost ();
	public abstract string[] getOutput ();
	public abstract int getType ();
	public abstract float[] getData ();
	public abstract void setData (float[] data);
}

public class Token {
	public Token parent;
	public List<Token> children;

	public string Name;

	public string Desc;

	public int TokenID;

	public int UpPoints;

	public int Lifespan;

	public int Progress = 0;

	public AEffect Effect;

	private Morality morality;

	public Type Type;

	public bool Unlocked;

	public float Rarity;

	public Token(Type type, string name, Morality morals, AEffect effect, int lifespan, int upPoints, int tokenID, float rarity, string desc){
		children = new List<Token> ();
		Type = type;
		Name = name;
		morality = morals;
		UpPoints = upPoints;
		Lifespan = lifespan-1;
		Desc = desc;
		Effect = effect;
		Effect.SetParent (this);
		TokenID = tokenID;
		Rarity = rarity;
		Unlocked = false;
	}

	public void setParent(Token parent){
		Debug.Log ("Parent Set: " + parent.Name + " to " + Name);
		this.parent = parent;
		this.parent.children.Add (this);
	}
		

	public int IncreaseChildTech(int amount){
		int amountleft = amount;
		bool exaustedChildren = false;
		foreach (Token child in children) {
			if (!child.Unlocked && child.Progress < child.UpPoints) {
				child.Progress += 1;
				amountleft -= 1;
			}

			if(amountleft == 0){
				return amountleft;
			}
		}

		foreach (Token child in children) {
			if (child.Unlocked || child.Progress >= child.UpPoints) {
				amountleft = child.IncreaseChildTech (amountleft);
			}

			if(amountleft == 0){
				return amountleft;
			}
		}
		return amountleft;
	}

	public bool ShouldBeLocked(){
		foreach (Token child in children) {
			if(Progress < UpPoints){
				return true;
			}
		}
		return false;
	}

	public void CheckForUnlock(){
		if (Progress >= UpPoints) {
			parent.Unlocked = ShouldBeLocked ();
			Unlocked = true;
		} else {
			Unlocked = false;
		}
	}
		
}


