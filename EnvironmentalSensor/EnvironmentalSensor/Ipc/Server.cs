using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;

namespace EnvironmentalSensor.Ipc
{
    /// <summary>
    /// プロセス間通信のサーバー
    /// </summary>
    public class Server
    {
        /// <summary>
        /// サーバーチャンネル
        /// </summary>
        public IpcServerChannel Channel { get; } = new IpcServerChannel("EnvironmentSensor");
        #region リモートオブジェクト
        /// <summary>
        /// リモートオブジェクト
        /// </summary>
        public RemoteObject RemoteObject { get; } = new RemoteObject();
        /// <summary>
        /// 排他制御
        /// </summary>
        public Mutex Mutex { get; protected set; } = new Mutex(false, RemoteObject.MutexName);
        #endregion リモートオブジェクト
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Server()
        {
            // チャンネルを登録
            ChannelServices.RegisterChannel(Channel, true);
            // 公開
            RemotingServices.Marshal(RemoteObject, nameof(RemoteObject), typeof(RemoteObject));
        }
    }
}
