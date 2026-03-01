using System;
using System.Collections.Generic;

public class LootTable<T>
{
    private System.Random rand = new System.Random();
    public TableList<T, double> lootTable;

    public double totalTableValue;
    public LootTable()
    {
        lootTable = new TableList<T, double>();
    }

    public LootTable(List<T> itemList, List<double> chanceList)
    {
        lootTable = new TableList<T, double>();

        if (itemList.Count != chanceList.Count)
        {
            throw new Exception("Item List and Chance List of Loot Table must be the same length");
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            Add(itemList[i], chanceList[i]);
        }
    }

    public T Get(int index)
    {
        return lootTable.GetLeft(index);
    }

    public void Add(T item, double chance)
    {
        totalTableValue += chance;
        lootTable.Add(item, chance);
    }

    /// <summary>
    /// "D&D" or other die roll style RPG loot table
    /// 
    /// It adds up all the chances and places them in a range from 0-MAX
    /// 
    /// which you roll a dMAX and get 1 item from the table.
    /// 
    /// For example you might have a table that looks like:
    /// 
    /// Bones: 5
    /// Scales: 5
    /// Tail: 2
    /// Ancient Fairy Dust of Doom: 1
    /// 
    /// Note this can be more or less than 100 as it's not a percentage
    /// but a value relative to a whole so for example
    /// 
    /// Item1 : 5
    /// Item2 : 10
    /// 
    /// In this above case item2 is twice as likely to be chosen
    /// as it has 10 oit of 15 chances to be chosen
    /// 
    /// So for the previous example you'd roll rand.Next(0, 13)
    /// Since adding the bones + scales + etc is 13 total.
    /// 
    /// and then check each subsequent percent. So a 7 would get scales
    /// since the numbers represent item hit "ranges"
    /// 
    /// Bones hit range is 1 - 5 
    /// Scales Hit Range is 5 - 10
    /// Tail Hit Range is 10 - 12
    /// Ancient Fairy Dust of Doom Hit Range is 12-13
    /// 
    /// We calculate this as the "final" value and seeing if it's less than the number
    /// And we choose the first in the linear sequence
    /// 
    /// For example:
    /// 
    /// Bones: 7 > 5
    /// Scales: 7 > 10  <= since this is first in the linear list pick this
    /// Tail: 7 > 12
    /// Ancient Fairy Dust of Doom: 7 > 13
    /// 
    /// If you roll 3 times from this table you in theory could get really
    /// Lucky and roll 3 Fairy Dusts, but would require you getting 3
    /// 13s in a row.
    /// 
    /// Pros of this technique: No chance that you don't draw an item
    /// Cons of this technique: Chance to roll X of the same item many times
    /// 
    /// </summary>
    /// <param name="rollCount">How many times to roll</param>
    /// <returns>A list of items to get from the loot table</returns>
    public List<T> GetRandomWeighted(int rollCount = 1)
    {
        List<T> itemsFromLoot = new List<T>();

        //do we want to throw an error here? 
        if (rollCount <= 0)
        {
            return itemsFromLoot;
        }

        double totalPercents = 0;
        foreach (var lootRoll in lootTable)
        {
            totalPercents += (double)lootRoll.Right;
        }
        //roll once for each loot item we want
        for (int i = 0; i < rollCount; i++)
        {
            //roll for each of the loot we want.
            double roll = rand.NextDouble() * totalPercents;

            //current starts at 0 and adds the lootRoll value
            double currentRoll = 0.0d;
            foreach (var lootRoll in lootTable)
            {
                // 0 < roll < 1
                // 1 < roll < 2 etc.
                if (currentRoll < roll && roll <= (currentRoll + lootRoll.Right))
                {
                    itemsFromLoot.Add(lootRoll.Left);
                    break;
                }
                currentRoll += lootRoll.Right;
            }
        }

        return itemsFromLoot;
    }


    /// <summary>
    /// Xenoblade sytle table where it rolls a percent against each item
    /// and adds it. 
    /// 
    /// For example you might have a table like:
    /// 
    /// Bones: 80
    /// Bones: 50
    /// Bones: 30
    /// Scales: 50
    /// Scales: 20
    /// Tail: 10
    /// Ancient Fairy Dust of Doom: 5
    /// 
    /// In this case it would roll a number against each one
    /// a chance to drop up to a total of 7 items, but more 
    /// likely just 1-3ish
    /// 
    /// In this style of table, the input values should be a
    /// percentage as a 100 or higher would essentially be a guaranteed drop
    /// 
    /// So for example given the drop table above:
    /// 
    /// Bones are an 80% drop chance, you roll a 20, rollValue is less than 80% so you add it 
    /// Bones are an 50% drop chance, you roll a 36, rollValue is less than 50% so you add it 
    /// Bones are an 30% drop chance, you roll a 50, rollValue is more than 30% is more than the  so you don't add it 
    /// Scales are an 80% drop chance, you roll a 45, rollValue is less than 80% so you add it 
    /// Scales are an 50% drop chance, you roll a 75, rollValue is more than 50% is more than the  so you don't add it
    /// Ancient Fairy Dust of Doom are an 5% drop chance, you roll a 6, rollValue is more than 5% so you don't add it 
    /// 
    /// The player would obtain from this:
    /// Bones
    /// Bones
    /// Scales
    /// 
    /// Unlike the linear table, there is no chance to get any row in the table
    /// more than once, unless the item is on the table several times.
    /// So if an item is only listed one, you can only obtain 1 in each list
    /// 
    /// Pros of this technique: Easy to design and provides many items.
    /// Cons of this technique: If you don't add items with 100% chance, there is a chance to get 0 items
    /// 
    /// </summary>
    /// <returns>A list of all the dropped items</returns>
    public List<T> GetRandomFullTable()
    {
        List<T> itemsFromLoot = new List<T>();
        //roll a random value for each element in the loot table
        foreach (var lootRoll in lootTable)
        {
            //In this case it's represented as a percentage
            if ((rand.NextDouble() * 100.0) < lootRoll.Right)
            {
                itemsFromLoot.Add(lootRoll.Left);
            }
        }
        return itemsFromLoot;
    }



    /// <summary>
    /// Diminishing return loot table
    /// 
    /// implementation of this one is tricky.
    /// 
    /// this is essentially one of the other types but where you
    /// remove any loot that's already been pulled until
    /// all loot has been pulled from the table
    /// 
    /// For example you might have a table like:
    /// 
    /// Bones: 5
    /// Scales: 5
    /// Tail: 2
    /// Ancient Fairy Dust of Doom: 1
    /// 
    /// If you roll 2 items, and then get scales and bones 
    /// the next time you roll it, it should not be possible
    /// to get scales or bones, so it's now a 66% chance for 
    /// Tail and a 33% chance of AFDOD. 
    /// 
    /// The goal here is to minimize the RNG aspect 
    /// by having the loot eventualy guaranteed to roll a rare 
    /// item if you get many items
    /// 
    /// Pros of this technique: Ensures rolling will eventually get you the rare items
    /// Cons of this technique: More expensive computationally as it requires a second table
    /// 
    /// </summary>
    /// <returns></returns>
    /// 
    public TableList<T, double> diminishingTable;
    public List<T> GetRandomDiminishing(int rollCount)
    {
        //ensure the list isn't null
        if (diminishingTable == null)
        {
            diminishingTable = new TableList<T, double>(lootTable);
        }

        List<T> itemsFromLoot = new List<T>();

        //do we want to throw an error here? 
        if (rollCount <= 0)
        {
            return itemsFromLoot;
        }

        //roll once for each loot item we want
        for (int i = 0; i < rollCount; i++)
        {
            double totalPercents = 0;
            foreach (var lootRoll in diminishingTable)
            {
                totalPercents += (double)lootRoll.Right;
            }

            //roll for each of the loot we want.
            double roll = rand.NextDouble() * totalPercents;

            //current starts at 0 and adds the lootRoll value
            double currentRoll = 0.0d;

            int found = -1;
            for (int j = 0; j < diminishingTable.Count; j++)
            {
                var currentItem = diminishingTable.Get(j);
                if (roll > currentRoll && roll <= (currentRoll + currentItem.Right))
                {
                    itemsFromLoot.Add(currentItem.Left);
                    found = j;
                    break;
                }
                currentRoll += currentItem.Right;
            }
            if (found >= 0)
            {
                diminishingTable.RemoveAt(found);
            }

            //ensure the loot table still has items
            if (diminishingTable.Count == 0)
            {
                //if not fill it back up
                //fill the table back up
                diminishingTable = new TableList<T, double>(lootTable);
            }
        }

        return itemsFromLoot;
    }

    /// <summary>
    /// Not sure if this one makes sense yet...
    /// 
    /// need to think about it more
    /// 
    /// This is like a variant of the Xenoblade table but 
    /// is intended to include some kind of "diminishing" 
    /// feature.
    /// 
    /// The only way I can see it is being like an...
    /// increasing weight?
    /// 
    /// So instead of being a diminishing percent it's like..
    /// every time you roll an item, the percent increases by
    /// some weight... until you get the item
    /// then it resets to it's original
    /// 
    /// So if the weights are
    /// 
    /// Bones: 80
    /// Bones: 50
    /// Bones: 30
    /// Scales: 50
    /// Scales: 20
    /// Tail: 10
    /// Ancient Fairy Dust of Doom: 5
    /// 
    /// and the percent increases by 5 every time you get the item 
    /// in the slot, it ensures you are able to eventually guarantee
    /// every item in the table, even the Ancient Fairy Dust of Doom.
    /// 
    /// As an example After 10 rolls of the full table the AFDOD 
    /// would be a 55% chance if you didn't roll it before then. 
    /// </summary>
    public TableList<T, double> currentFullTable;
    /// <summary>
    /// We may want to do this as a list so each item can 
    /// have a difference increase
    /// </summary>
    private double weightIncreasePerRoll = 5.0d;
    public List<T> GetRandomFullTableWeightIncrease(double weightIncrease = 5.0d)
    {
        if (currentFullTable == null)
        {
            currentFullTable = new TableList<T, double>(lootTable);
        }

        List<T> itemsFromLoot = new List<T>();

        for (int i = 0; i < currentFullTable.Count; i++)
        {
            var item = currentFullTable.Get(i);
            //In this case it's represented as a percentage
            if ((rand.NextDouble() * 100.0) < item.Right)
            {
                itemsFromLoot.Add(item.Left);
                currentFullTable.right[i] = lootTable.right[i];
            }
            else
            {
                currentFullTable.right[i] += weightIncrease;
            }
        }

        //roll a random value for each element in the loot table
        foreach (var lootRoll in currentFullTable)
        {
        }
        return itemsFromLoot;
    }
}