using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;

namespace EnvironmentalSensor.Ipc
{
    /// <summary>
    /// プロセス間通信のクライアント
    /// </summary>
    public class Client
    {
        const string URL = "ipc://EnvironmentSensor/";
        /// <summary>
        /// チャンネル
        /// </summary>
        IpcClientChannel channel = new IpcClientChannel();
        /// <summary>
        /// 排他制御
        /// </summary>
        public Mutex Mutex { get; protected set; } = new Mutex(false, RemoteObject.MutexName);
        /// <summary>
        /// チャンネルを登録
        /// </summary>
        public Client()
        {
            // チャンネルを登録
            ChannelServices.RegisterChannel(channel, true);
        }
        /// <summary>
        /// リモートオブジェクトを取得
        /// </summary>
        public RemoteObject GetRemoteObject()
        {
            return Activator.GetObject(typeof(RemoteObject), URL + nameof(RemoteObject)) as RemoteObject;
        }
    }
}
