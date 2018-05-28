using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 牌型类：用于算分和比较
public class PokerType {
    public const int BOMB = 5;
    public const int FLUSH_AND_STRAIGHT = 4;
    public const int FLUSH = 3;
    public const int STRAIGHT = 2;
    public const int DOUBLE = 1;
    public const int NORMAL = 0;

    protected int pokerType = 0;

    protected Poker[] pokers = null;

    protected int score = 0;

    public PokerType(Poker[] pokers) {
        this.pokers = pokers;
    }

    public int Score {
        get { return this.score; }
    }

	public static bool operator < (PokerType x, PokerType y) {
        return x.Score < y.Score;
	}

	public static bool operator > (PokerType x, PokerType y) {
		return x.Score > y.Score;
	}

    public static PokerType CalcPokerType(Poker[] pokers) {
        PokerType pokerType = null;
        if (IsBomb(pokers)) {
			// 炸弹
			// Debug.Log("Bomb--------------------------------------------------");
            pokerType = new BombPokerType(pokers);
		} else if (IsFlush(pokers)) {
			// 同花
			if (IsStraight(pokers)) {
				// 同花顺
				// Debug.Log("Flush and straight--------------------------------------------------");
                pokerType = new FlushAndStraightPokerType(pokers);
			} else {
				// Debug.Log("Flush--------------------------------------------------");
                pokerType = new FlushPokerType(pokers);
			}
		} else if (IsStraight(pokers)) {
			// 顺子
			// Debug.Log("Straight--------------------------------------------------");
			pokerType = new StraightPokerType(pokers);
		} else if (IsDouble(pokers)) {
			// 对子
			// Debug.Log("Double--------------------------------------------------");
			pokerType = new DoublePokerType(pokers);
		} else {
			// 普通牌
			// Debug.Log("Normal--------------------------------------------------");
            pokerType = new NormalPokerType(pokers);
		}
        return pokerType;
    }

    protected static int CalcNormalValue(Poker[] pokers) {
		int[] nums = new int[3];
		for (int i = 0; i < 3; i++) {
			nums[i] = pokers[i].Num;
		}
		Array.Sort(nums);
		return nums[0] + nums[1] * 16 + nums[2] * 16 * 16;
	}

	protected static int GetMinNum(Poker[] pokers) {
		return Math.Min(Math.Min(pokers[0].Num, pokers[1].Num), pokers[2].Num);
	}

	// 是否是炸弹
	private static bool IsBomb(Poker[] pokers) {
		return pokers[0].Num == pokers[1].Num
			&& pokers[0].Num == pokers[2].Num;
	}

	// 是否同花
	private static bool IsFlush(Poker[] pokers) {
		return pokers[0].Flower == pokers[1].Flower 
			&& pokers[0].Flower == pokers[2].Flower;
	}

	// 是否是顺子
	private static bool IsStraight(Poker[] pokers) {
		int[] nums = new int[3];
		for (int i = 0; i < 3; i++) {
			nums[i] = pokers[i].Num;
		}
		Array.Sort(nums);
		return nums[0] + 1 == nums[1]
			&& nums[1] + 1 == nums[2];
	}

	// 是否是对牌
	private static bool IsDouble(Poker[] pokers) {
		return pokers[0].Num == pokers[1].Num
			|| pokers[0].Num == pokers[2].Num
			|| pokers[1].Num == pokers[2].Num;
	}
}