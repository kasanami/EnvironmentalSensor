using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

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
