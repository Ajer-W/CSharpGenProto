//协议号定义
public class ProtocolId
{
	public const ushort PlayerInfo = 38028;
	public const ushort Head = 57391;
	public const ushort Heart = 39855;
	public const ushort Vector3 = 41135;
	public const ushort Move = 7873;
	public const ushort Jump = 37418;
	public const ushort Run = 28023;
	public const ushort RunStop = 9207;
	public const ushort EnterScene = 48857;
	public const ushort EnterSceneReturn = 52949;
	public const ushort LeaveScene = 29850;
	public const ushort LeaveSceneReturn = 11587;
	public const ushort CAction = 11717;
	public const ushort CActionReturn = 36691;
	public const ushort ReqRoleLogin = 26613;
	public const ushort ResRoleLogin = 44372;
}
//协议号扩展函数
public partial class PlayerInfo
{
	public ushort GetProtocolID() { return ProtocolId.PlayerInfo; }
}
public partial class Head
{
	public ushort GetProtocolID() { return ProtocolId.Head; }
}
public partial class Heart
{
	public ushort GetProtocolID() { return ProtocolId.Heart; }
}
public partial class Vector3
{
	public ushort GetProtocolID() { return ProtocolId.Vector3; }
}
public partial class Move
{
	public ushort GetProtocolID() { return ProtocolId.Move; }
}
public partial class Jump
{
	public ushort GetProtocolID() { return ProtocolId.Jump; }
}
public partial class Run
{
	public ushort GetProtocolID() { return ProtocolId.Run; }
}
public partial class RunStop
{
	public ushort GetProtocolID() { return ProtocolId.RunStop; }
}
public partial class EnterScene
{
	public ushort GetProtocolID() { return ProtocolId.EnterScene; }
}
public partial class EnterSceneReturn
{
	public ushort GetProtocolID() { return ProtocolId.EnterSceneReturn; }
}
public partial class LeaveScene
{
	public ushort GetProtocolID() { return ProtocolId.LeaveScene; }
}
public partial class LeaveSceneReturn
{
	public ushort GetProtocolID() { return ProtocolId.LeaveSceneReturn; }
}
public partial class CAction
{
	public ushort GetProtocolID() { return ProtocolId.CAction; }
}
public partial class CActionReturn
{
	public ushort GetProtocolID() { return ProtocolId.CActionReturn; }
}
public partial class ReqRoleLogin
{
	public ushort GetProtocolID() { return ProtocolId.ReqRoleLogin; }
}
public partial class ResRoleLogin
{
	public ushort GetProtocolID() { return ProtocolId.ResRoleLogin; }
}
//协议辅助函数
public static class ProtocolHelper
{
	public static Google.Protobuf.IMessage Parse(ushort protocolId, byte[] data)
	{
		switch (protocolId)
		{
			case ProtocolId.PlayerInfo: return PlayerInfo.Parser.ParseFrom(data);
			case ProtocolId.Head: return Head.Parser.ParseFrom(data);
			case ProtocolId.Heart: return Heart.Parser.ParseFrom(data);
			case ProtocolId.Vector3: return Vector3.Parser.ParseFrom(data);
			case ProtocolId.Move: return Move.Parser.ParseFrom(data);
			case ProtocolId.Jump: return Jump.Parser.ParseFrom(data);
			case ProtocolId.Run: return Run.Parser.ParseFrom(data);
			case ProtocolId.RunStop: return RunStop.Parser.ParseFrom(data);
			case ProtocolId.EnterScene: return EnterScene.Parser.ParseFrom(data);
			case ProtocolId.EnterSceneReturn: return EnterSceneReturn.Parser.ParseFrom(data);
			case ProtocolId.LeaveScene: return LeaveScene.Parser.ParseFrom(data);
			case ProtocolId.LeaveSceneReturn: return LeaveSceneReturn.Parser.ParseFrom(data);
			case ProtocolId.CAction: return CAction.Parser.ParseFrom(data);
			case ProtocolId.CActionReturn: return CActionReturn.Parser.ParseFrom(data);
			case ProtocolId.ReqRoleLogin: return ReqRoleLogin.Parser.ParseFrom(data);
			case ProtocolId.ResRoleLogin: return ResRoleLogin.Parser.ParseFrom(data);
		}
		return null;
	}
}
