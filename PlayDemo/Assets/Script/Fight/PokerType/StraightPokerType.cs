using System.Text;

// 顺子牌型
public class StraightPokerType: PokerType {
    public StraightPokerType(Poker[] pokers): base(pokers) {
        base.pokerType = STRAIGHT;
        int min = GetMinNum(pokers);
		base.score = 4040 + min;
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.Append("Straight: ");
        for (int i = 0; i < base.pokers.Length; i++) {
            Poker poker = base.pokers[i];
            sb.AppendFormat("{0}-{1} ", poker.Flower, poker.Num);
        }
        return sb.ToString();
    }
}