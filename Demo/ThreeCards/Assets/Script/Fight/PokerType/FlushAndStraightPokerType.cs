using System.Text;

// 同花顺牌型
public class FlushAndStraightPokerType: PokerType {
    public FlushAndStraightPokerType(Poker[] pokers): base(pokers) {
        base.pokerType = FLUSH_AND_STRAIGHT;
        base.score = 7855 + GetMinNum(pokers);
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.Append("Flush and straight: ");
        for (int i = 0; i < base.pokers.Length; i++) {
            Poker poker = base.pokers[i];
            sb.AppendFormat("{0}-{1} ", poker.Flower, poker.Num);
        }
        return sb.ToString();
    }
}