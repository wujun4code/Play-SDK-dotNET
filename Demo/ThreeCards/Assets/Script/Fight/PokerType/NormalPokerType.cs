using System;
using System.Text;

// 普通牌型
public class NormalPokerType: PokerType {
    public NormalPokerType(Poker[] pokers): base(pokers) {
        base.pokerType = NORMAL;
        base.score = CalcNormalValue(pokers);
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.Append("Normal: ");
        for (int i = 0; i < base.pokers.Length; i++) {
            Poker poker = base.pokers[i];
            sb.AppendFormat("{0}-{1} ", poker.Flower, poker.Num);
        }
        return sb.ToString();
    }
}