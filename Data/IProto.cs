using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IProto : Google.Protobuf.IMessage
{
    /// <summary>
    /// 协议编号
    /// </summary>
    ushort ProtoId { get; }

    /// <summary>
    /// 协议编码
    /// </summary>
    string ProtoEnName { get; }
}

