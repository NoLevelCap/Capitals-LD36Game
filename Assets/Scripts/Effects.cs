using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PopulationIncreaseEffect : AEffect
{

	private int cost;
	private float mod, bigmod;
	private float[] data;
	private List<string> output;

	CityGenerator[] blocksAround;

	public override void PreEffect (CityGenerator block)
	{
		output = new List<string> ();
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
		if(!firstTrigger){
			output.Clear ();
		}
		block.CBD.IncreasePopulation (bigmod);
		output.Add ("<color=green>"+"Pop +" + (bigmod*100f)+"%"+"</color>");
		foreach (CityGenerator blocks in blocksAround) {
			if(blocks != null){
				blocks.CBD.IncreasePopulation (mod);
			}
		}

		output.Add ("<color=green>"+blocksAround.Length + " Pops +" + (mod*100f)+"%"+"</color>");
	}

	public override void OnDestoy (CityGenerator block)
	{
		GameObject.Destroy (this);
	}


	public override int[] GetIconTypes ()
	{
		return new int[]{6, 10};
	}


	public override string[] getOutput ()
	{
		return output.ToArray();
	}


	public override int getCost ()
	{
		return cost;
	}

	public override float[] getData ()
	{
		return data;
	}

	public override void setData (float[] data)
	{
		this.data = data;
		cost = Mathf.RoundToInt(data [0]);
		this.mod = data [1];
		this.bigmod = data [2];
	}

	public override int getType ()
	{
		return 0;
	}
}

public class LocalTaxesIncreaseEffect : AEffect
{
	CityGenerator[] blocksAround;

	private float[] data;

	float mod, bigmod, tax;
	private List<string> output;

	public override void PreEffect (CityGenerator block)
	{
		output = new List<string> ();
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

		block.CBD.TaxPP += tax*2f;
		foreach (CityGenerator blocks in blocksAround) {
			if(blocks != null){
				block.CBD.TaxPP += tax;
			}
		}

		output.Add ("<color=green>"+"Taxes +" + (tax*2f*100f)+"%"+"</color>");
		output.Add ("<color=green>"+blocksAround.Length + " Taxes -" + (tax*100f)+"%"+"</color>");

		MainEffect (block);

	}
	public override void MainEffect (CityGenerator block)
	{
		if(!firstTrigger){
			output.Clear ();
		}

		block.CBD.IncreasePopulation (-bigmod);
		output.Add ("<color=red>"+"Pop -" + (bigmod*100f)+"%"+"</color>");
		foreach (CityGenerator blocks in blocksAround) {
			if(blocks != null){
				blocks.CBD.IncreasePopulation (-mod);
			}
		}
		output.Add ("<color=red>"+blocksAround.Length + " Pops -" + (mod*100f)+"%"+"</color>");
	}

	#region implemented abstract members of AEffect

	public override void OnDestoy (CityGenerator block)
	{
		GameObject.Destroy (this);
	}

	#endregion

	public override int[] GetIconTypes ()
	{
		return new int[]{8, 9, 10};
	}

	public override int getCost ()
	{
		return 0;
	}

	public override void setData (float[] data)
	{
		this.data = data;
		mod = data [0];
		bigmod = data [1];
		tax = data [2];
	}
	public override float[] getData ()
	{
		return data;
	}

	public override int getType ()
	{
		return 0;
	}
	public override string[] getOutput ()
	{
		return output.ToArray();
	}

}

public class AddCardsEffect:AEffect
{

	private float[] data;
	private int cost, amount;
	private List<string> output;

	#region implemented abstract members of AEffect
	public override void PreEffect (CityGenerator block)
	{
		output = new List<string> ();
		GameManager.instance.FillHand (amount);
	}
	public override void MainEffect (CityGenerator block)
	{
		
	}
	public override void OnDestoy (CityGenerator block)
	{
		
	}
	public override int[] GetIconTypes ()
	{
		return new int[]{2, 4};
	}
	public override int getCost ()
	{
		return cost;
	}
	public override void setData (float[] data)
	{
		this.data = data;
		cost = Mathf.RoundToInt(data [0]);
		amount = Mathf.RoundToInt(data [1]);
	}
	public override float[] getData ()
	{
		return data;
	}

	public override int getType ()
	{
		return 0;
	}
	#endregion
	public override string[] getOutput ()
	{
		return output.ToArray();
	}

}

public class IncreasePopEffect:AEffect
{

	private float[] data;
	private int cost, radi;
	private float amount;
	private List<string> output;

	#region implemented abstract members of AEffect
	public override void PreEffect (CityGenerator block)
	{

		output = new List<string> ();
		if (block.CBD.Pop < amount) {
			block.CBD.SetPopData (amount);
			output.Add ("<color=green>"+"Pop set to " + (amount));
		} else {
			output.Add ("<color=orange>"+"Pop already at " + amount+"</color>");
		}

	}
	public override void MainEffect (CityGenerator block)
	{
		output.Clear ();
	}
	public override void OnDestoy (CityGenerator block)
	{

	}
	public override int[] GetIconTypes ()
	{
		return new int[]{7, 4};
	}
	public override int getCost ()
	{
		return cost;
	}
	public override void setData (float[] data)
	{
		this.data = data;
		cost = Mathf.RoundToInt(data [0]);
		amount = data [1];
		radi =  Mathf.RoundToInt(data [2]);
	}
	public override float[] getData ()
	{
		return data;
	}

	public override int getType ()
	{
		return 0;
	}
	#endregion
	public override string[] getOutput ()
	{
		return output.ToArray();
	}

}



public class IncreaseMoney:AEffect
{

	private float[] data;
	private int  amount;
	private List<string> output;

	#region implemented abstract members of AEffect
	public override void PreEffect (CityGenerator block)
	{
		output = new List<string> ();
		GameManager.instance.ChargeACost ("Illicit Funding", -amount);
		output.Add ("<color=green>"+"Money +£" + (amount)+"</color>");

	}
	public override void MainEffect (CityGenerator block)
	{
		output.Clear ();
	}
	public override void OnDestoy (CityGenerator block)
	{

	}
	public override int[] GetIconTypes ()
	{
		return new int[]{1, 4};
	}
	public override int getCost ()
	{
		return 0;
	}
	public override void setData (float[] data)
	{
		this.data = data;
		amount = Mathf.RoundToInt(data [0]);
	}
	public override float[] getData ()
	{
		return data;
	}

	public override int getType ()
	{
		return 0;
	}
	#endregion
	public override string[] getOutput ()
	{
		return output.ToArray();
	}

}

public class ClearTilesEffect:AEffect
{

	private float[] data;
	private int cost, radi;
	private List<string> output;

	#region implemented abstract members of AEffect
	public override void PreEffect (CityGenerator block)
	{
		output = new List<string> ();
	}
	public override void MainEffect (CityGenerator block)
	{

	}
	public override void OnDestoy (CityGenerator block)
	{

	}
	public override int[] GetIconTypes ()
	{
		return new int[]{5, 1};
	}
	public override int getCost ()
	{
		return cost;
	}
	public override void setData (float[] data)
	{
		this.data = data;
		cost = Mathf.RoundToInt(data [0]);
		radi = Mathf.RoundToInt(data [1]);
	}
	public override float[] getData ()
	{
		return data;
	}
	#endregion

	#region implemented abstract members of AEffect

	public override int getType ()
	{
		return 1;
	}

	#endregion
	public override string[] getOutput ()
	{
		return output.ToArray();
	}

}

public class BasicScienceIncreaserEffect:AEffect
{

	private float[] data;
	private int amount;
	private List<string> output;

	#region implemented abstract members of AEffect
	public override void PreEffect (CityGenerator block)
	{
		output = new List<string> ();
		MainEffect (block);
	}
	public override void MainEffect (CityGenerator block)
	{
		if(!firstTrigger){
			output.Clear ();
		}
		parent.IncreaseChildTech (amount);
		output.Add ("<color=cyan>"+"Science +" + (amount)+"</color>");
	}
	public override void OnDestoy (CityGenerator block)
	{

	}
	public override int[] GetIconTypes ()
	{
		return new int[]{3};
	}
	public override int getCost ()
	{
		return 0;
	}
	public override void setData (float[] data)
	{
		this.data = data;
		amount = Mathf.RoundToInt(data [0]);
	}
	public override float[] getData ()
	{
		return data;
	}

	public override int getType ()
	{
		return 0;
	}
	#endregion
	public override string[] getOutput ()
	{
		return output.ToArray();
	}

}

public class BasicPrefaceEffect:AEffect
{

	private float[] data;
	private int cost, amount;
	private List<string> output;

	#region implemented abstract members of AEffect
	public override void PreEffect (CityGenerator block)
	{
		output = new List<string> ();
	}
	public override void MainEffect (CityGenerator block)
	{
		if(!firstTrigger){
			output.Clear ();
		}
	}
	public override void OnDestoy (CityGenerator block)
	{

	}
	public override int[] GetIconTypes ()
	{
		return new int[]{};
	}
	public override int getCost ()
	{
		return cost;
	}
	public override void setData (float[] data)
	{
		this.data = data;
		cost = Mathf.RoundToInt(data [0]);
		amount = Mathf.RoundToInt(data [1]);
	}
	public override float[] getData ()
	{
		return data;
	}

	public override int getType ()
	{
		return 0;
	}
	#endregion
	public override string[] getOutput ()
	{
		return output.ToArray();
	}

}

