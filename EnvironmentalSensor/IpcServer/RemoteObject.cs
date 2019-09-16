using System;

namespace IpcServer
{
    class RemoteObject : MarshalByRefObject
    {
        public int Counter { get; set; }
    }
}
