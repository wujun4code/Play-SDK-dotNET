using System.Text;

// 炸弹牌型
public class BombPokerType: PokerType {
    public BombPokerType(Poker[] pokers): base(pokers) {
        base.pokerType = BOMB;
        base.score = 7867 + base.pokers[0].Num;
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.Append("Bomb: ");
        for (int i = 0; i < base.pokers.Length; i++) {
            Poker poker = base.pokers[i];
            sb.AppendFormat("{0}-{1} ", poker.Flower, poker.Num);
        }
        return sb.ToString();
    }
}