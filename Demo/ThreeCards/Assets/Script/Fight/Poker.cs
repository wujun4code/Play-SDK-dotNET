public class Poker {
    // 花色
    // 黑桃
    public const int FLOWER_SPADE = 3;
    // 红心
    public const int FLOWER_HEART = 2;
    // 梅花
    public const int FLOWER_CLUB = 1;
    // 方块
    public const int FLOWER_DIAMOND = 0;

    // 牌值
    // 最大为A
    public const int NUM_A = 14;
    public const int NUM_K = 13;
    public const int NUM_Q = 12;
    public const int NUM_J = 11;
    public const int NUM_10 = 10;
    public const int NUM_9 = 9;
    public const int NUM_8 = 8;
    public const int NUM_7 = 7;
    public const int NUM_6 = 6;
    public const int NUM_5 = 5;
    public const int NUM_4 = 4;
    public const int NUM_3 = 3;
    // 最小为2
    public const int NUM_2 = 2;

    public Poker() {
        Flower = -1;
        Num = -1;
    }

    public Poker(int flower, int num) {
        Flower = flower;
        Num = num;
    }

    public int Flower {
        get; set;
    }

    public int Num {
        get; set;
    }

    public override string ToString() {
        return string.Format("flower: {0}, num: {1}", Flower, Num);
    }
}