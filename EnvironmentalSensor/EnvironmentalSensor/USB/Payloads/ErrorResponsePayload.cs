﻿using System.Text;

namespace EnvironmentalSensor.USB.Payloads
{
    /// <summary>
    /// エラー返答
    /// マニュアル:4.3.5 Payload frame format [Error Response from 2JCIE-BU01] 
    /// </summary>
    public class ErrorResponsePayload : ResponsePayload
    {
        /// <summary>
        /// Error 内容
        /// </summary>
        public ErrorCode Code { get; private set; }
        /// <summary>
        /// 指定されたバッファから初期化
        /// </summary>
        /// <param name="buffer">Payloadの範囲のバッファ</param>
        public ErrorResponsePayload(byte[] buffer, int index) : base(buffer, index)
        {
            Code = (ErrorCode)Data[0];
        }
        public override string ToString()
        {
            var text = new StringBuilder();
            text.AppendLine("{");
            text.AppendLine($"{nameof(Command)}={Command}");
            text.AppendLine($"{nameof(Address)}={Address}");
            text.AppendLine($"{nameof(Code)}={Code}");
            text.AppendLine("}");
            return text.ToString();
        }
    }
}
