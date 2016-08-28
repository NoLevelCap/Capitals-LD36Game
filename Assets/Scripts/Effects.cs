using System.Collections;
using UnityEngine;

public class PopulationIncreaseEffect : AEffect
{

	private int cost;
	private float mod, bigmod;
	private float[] data;

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
		block.CBD.IncreasePopulation (bigmod);
		foreach (CityGenerator blocks in blocksAround) {
			if(blocks != null){
				blocks.CBD.IncreasePopulation (mod);
			}
		}
	}

	public override void OnDestoy (CityGenerator block)
	{
		GameObject.Destroy (this);
	}


	public override int[] GetIconTypes ()
	{
		return new int[]{0, 1};
	}


	public override string[] getOutput ()
	{
		return new string[]{"Example", "ABCUS"};
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

		block.CBD.TaxPP += tax*2f;
		foreach (CityGenerator blocks in blocksAround) {
			if(blocks != null){
				block.CBD.TaxPP += tax;
			}
		}

		MainEffect (block);

	}
	public override void MainEffect (CityGenerator block)
	{
		block.CBD.IncreasePopulation (-bigmod);

		foreach (CityGenerator blocks in blocksAround) {
			if(blocks != null){
				blocks.CBD.IncreasePopulation (-mod);
			}
		}
	}

	#region implemented abstract members of AEffect

	public override void OnDestoy (CityGenerator block)
	{
		GameObject.Destroy (this);
	}

	#endregion

	public override int[] GetIconTypes ()
	{
		return new int[]{1};
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
		return new string[]{};
	}

}

public class AddCardsEffect:AEffect
{

	private float[] data;
	private int cost, amount;

	#region implemented abstract members of AEffect
	public override void PreEffect (CityGenerator block)
	{
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
		return new int[]{2};
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
		return new string[]{};
	}

}

public class IncreasePopEffect:AEffect
{

	private float[] data;
	private int cost, radi;
	private float amount;

	#region implemented abstract members of AEffect
	public override void PreEffect (CityGenerator block)
	{
		if(block.CBD.Pop < amount){
			block.CBD.SetPopData(amount);
		}
	}
	public override void MainEffect (CityGenerator block)
	{

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
		return new string[]{};
	}

}



public class IncreaseMoney:AEffect
{

	private float[] data;
	private int  amount;

	#region implemented abstract members of AEffect
	public override void PreEffect (CityGenerator block)
	{
		GameManager.instance.ChargeACost ("Illicit Funding", -amount);
	}
	public override void MainEffect (CityGenerator block)
	{

	}
	public override void OnDestoy (CityGenerator block)
	{

	}
	public override int[] GetIconTypes ()
	{
		return new int[]{1};
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
		return new string[]{};
	}

}

public class ClearTilesEffect:AEffect
{

	private float[] data;
	private int cost, radi;

	#region implemented abstract members of AEffect
	public override void PreEffect (CityGenerator block)
	{
	}
	public override void MainEffect (CityGenerator block)
	{

	}
	public override void OnDestoy (CityGenerator block)
	{

	}
	public override int[] GetIconTypes ()
	{
		return new int[]{5};
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
		return new string[]{};
	}

}

public class BasicScienceIncreaserEffect:AEffect
{

	private float[] data;
	private int amount;

	#region implemented abstract members of AEffect
	public override void PreEffect (CityGenerator block)
	{
		parent.IncreaseChildTech (amount);
	}
	public override void MainEffect (CityGenerator block)
	{
		parent.IncreaseChildTech (amount);
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
		return new string[]{"EXAMPLE"};
	}

}

public class BasicPrefaceEffect:AEffect
{

	private float[] data;
	private int cost, amount;

	#region implemented abstract members of AEffect
	public override void PreEffect (CityGenerator block)
	{

	}
	public override void MainEffect (CityGenerator block)
	{

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
		return new string[]{};
	}

}

