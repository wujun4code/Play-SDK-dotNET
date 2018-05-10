using LeanCloud;

namespace PlayDocSample
{
    /// <summary>
    /// 注册回调的类型，必须继承自 PlayMonoBehaviour
    /// </summary>
    public class SampleConnect : PlayMonoBehaviour
    {

        /// <summary>
        /// 并且一定要定无参数的构造函数，而且一定要调用父类的构造函数
        /// </summary>
        public SampleConnect() : base()
        {

        }

        void start()
        {
            // 打开调试日志
            Play.ToggleLog(true);
            // 设置 UserID
            Play.UserID = "hjiang";
            // 声明游戏版本，不同游戏版本的玩家会被分配在不同的隔离区域中
            Play.Connect("0.0.1");
        }


        [PlayEvent]
        public override void OnAuthenticated()
        {
            Play.Log("OnAuthenticated");
        }


    }
}
