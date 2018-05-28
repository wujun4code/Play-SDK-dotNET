using System.Text;

// 同花牌型
public class FlushPokerType: PokerType {
    public FlushPokerType(Poker[] pokers): base(pokers) {
        base.pokerType = FLUSH;
        base.score = 4052 + CalcNormalValue(pokers);
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.Append("Flush: ");
        for (int i = 0; i < base.pokers.Length; i++) {
            Poker poker = base.pokers[i];
            sb.AppendFormat("{0}-{1} ", poker.Flower, poker.Num);
        }
        return sb.ToString();
    }
}