using System.Text;

// 对子牌型
public class DoublePokerType: PokerType {
    public DoublePokerType(Poker[] pokers): base(pokers) {
        base.pokerType = DOUBLE;
        int d = getDoubleNum();
        int s = getSingleNum();
        base.score = 3803 + d * 16 + s;
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.Append("Double: ");
        for (int i = 0; i < base.pokers.Length; i++) {
            Poker poker = base.pokers[i];
            sb.AppendFormat("{0}-{1} ", poker.Flower, poker.Num);
        }
        return sb.ToString();
    }

    private int getDoubleNum() {
        if (pokers[0].Num == pokers[1].Num) {
            return pokers[0].Num;
        } else if (pokers[0].Num == pokers[2].Num) {
            return pokers[0].Num;
        } else {
            return pokers[1].Num;
        }
    }

    private int getSingleNum() {
        if (pokers[0].Num == pokers[1].Num) {
            return pokers[2].Num;
        } else if (pokers[0].Num == pokers[2].Num) {
            return pokers[1].Num;
        } else {
            return pokers[0].Num;
        }
    }
}