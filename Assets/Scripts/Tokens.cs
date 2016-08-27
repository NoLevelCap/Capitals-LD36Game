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
	

public abstract class AEffect : ScriptableObject {

	private bool firstTrigger;

	public float mod, cost;

	public AEffect(){
		this.firstTrigger = true;
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

	public Token(Type type, string name, Morality morals, AEffect effect, int lifespan, int upPoints, int tokenID, string desc){
		children = new List<Token> ();
		Type = type;
		Name = name;
		morality = morals;
		UpPoints = upPoints;
		Lifespan = lifespan;
		Desc = desc;
		Effect = effect;
		TokenID = tokenID;
		Unlocked = false;
	}

	public void setParent(Token parent){
		Debug.Log ("Parent Set: " + parent.Name + " to " + Name);
		this.parent = parent;
		this.parent.children.Add (this);
	}
		
}

public class PopulationIncreaseEffect : AEffect
{
	private static float mod = 0.02f;

	CityGenerator[] blocksAround;

	public override void PreEffect (CityGenerator block)
	{

		int x = block.mx;
		int y = block.my;
		blocksAround = new CityGenerator[4];
		if(x-1 >= 0){
			blocksAround[0] = GameManager.GetBlock(x-1, y);
		}

		if(y-1 >= 0){
			blocksAround[1] = GameManager.GetBlock(x, y-1);
		}

		if(y+1 < GameManager.height*2){
			blocksAround[2] = GameManager.GetBlock(x, y+1);
		}

		if(x+1 < GameManager.width*2){
			blocksAround[3] = GameManager.GetBlock(x+1, y);
		}

		MainEffect (block);

	}
	public override void MainEffect (CityGenerator block)
	{
		block.CBD.SpefPopulation (0.04f, 2f);
		foreach (CityGenerator blocks in blocksAround) {
			if(blocks != null){
				blocks.CBD.SpefPopulation (0.02f, 1f);
			}
		}
	}

	#region implemented abstract members of AEffect

	public override void OnDestoy (CityGenerator block)
	{
		GameObject.Destroy (this);
	}

	#endregion
}

public class LocalTaxesIncreaseEffect : AEffect
{
	CityGenerator[] blocksAround;

	public override void PreEffect (CityGenerator block)
	{

		int x = block.mx;
		int y = block.my;
		blocksAround = new CityGenerator[4];
		if(x-1 >= 0){
			blocksAround[0] = GameManager.GetBlock(x-1, y);
		}

		if(y-1 >= 0){
			blocksAround[1] = GameManager.GetBlock(x, y-1);
		}

		if(y+1 < GameManager.height*2){
			blocksAround[2] = GameManager.GetBlock(x, y+1);
		}

		if(x+1 < GameManager.width*2){
			blocksAround[3] = GameManager.GetBlock(x+1, y);
		}

		block.CBD.TaxPP += 0.02f;
		foreach (CityGenerator blocks in blocksAround) {
			if(blocks != null){
				block.CBD.TaxPP += 0.01f;
			}
		}

		MainEffect (block);

	}
	public override void MainEffect (CityGenerator block)
	{
		block.CBD.IncreasePopulation (-0.04f);

		foreach (CityGenerator blocks in blocksAround) {
			if(blocks != null){
				blocks.CBD.IncreasePopulation (-0.02f);
			}
		}
	}

	#region implemented abstract members of AEffect

	public override void OnDestoy (CityGenerator block)
	{
		GameObject.Destroy (this);
	}

	#endregion
}
