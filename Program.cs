// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static LinkedList<string> protoNameList = new LinkedList<string>();
    private static Dictionary<string, bool> protoDict = new Dictionary<string, bool>();
    private static string CSProtoFilePath = string.Empty;
    private static string LuaProtoFileName = string.Empty;
    private static string ProtoToCSPath = string.Empty;
    private static string protogen = "protoc.exe";

    static void Main(string[] args)
    {
        string protoPath = @args[0];
        CSProtoFilePath = @args[1];
        LuaProtoFileName = @args[2];
        ProtoToCSPath = @args[3];

        Console.WriteLine("Proto文件路径:" + protoPath);
        Console.WriteLine("CSharp路径:" + CSProtoFilePath);
        Console.WriteLine("Lua文件路径:" + LuaProtoFileName);

        List<string> cmds = new List<string>();
        DirectoryInfo directory = new DirectoryInfo(protoPath);
        FileInfo[] arrFiles = directory.GetFiles("*.proto", SearchOption.AllDirectories);
        foreach (FileInfo file in arrFiles)
        {
            FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(fs, Encoding.UTF8);

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine() ?? "";
                if (line.IndexOf("message") != -1)
                {
                    string s = Regex.Replace(line, @"\s+", "").Replace("{", "");//去掉空格
                    string protoName = s.Replace("message", "");
                    protoNameList.AddLast(protoName);
                    protoDict.Add(protoName, true);
                    //Console.WriteLine(protoName);
                }
            }
            sr.Close();
            fs.Close();

            string cmd = protogen + " --csharp_out=" + ProtoToCSPath + " -I " + protoPath + " " + file.FullName;
            cmds.Add(cmd);
        }

        Cmd(cmds);
        GetCSFileContent();
        CreateLuaProtoIdFile();

        ChangeProtoContent();
        Console.ReadLine();
    }

    static byte[] CRC16(byte[] data)
    {
        int len = data.Length;
        if (len > 0)
        {
            ushort crc = 0xFFFF;

            for (int i = 0; i < len; i++)
            {
                crc = (ushort)(crc ^ (data[i]));
                for (int j = 0; j < 8; j++)
                {
                    crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                }
            }
            byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
            byte lo = (byte)(crc & 0x00FF);         //低位置

            return new byte[] { lo, hi };
        }
        return new byte[] { 0, 0 };
    }
    public static ushort StringToCRC(byte[] str)
    {
        return BitConverter.ToUInt16(CRC16(str), 0);
    }

    private static void GetCSFileContent()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("//协议号定义");
        sb.AppendLine("public class ProtocolId");
        sb.AppendLine("{");
        var curr = protoNameList.First;
        while (curr != null)
        {
            int protoId = StringToCRC(Encoding.ASCII.GetBytes(curr.Value));
            sb.AppendLine($"\tpublic const ushort {curr.Value} = {protoId};");
            curr = curr.Next;
        }
        sb.AppendLine("}");

        sb.AppendLine("//协议号扩展函数");
        curr = protoNameList.First;
        while (curr != null)
        {
            sb.AppendLine($"public partial class {curr.Value}");
            sb.AppendLine("{");
            sb.AppendLine("\tpublic ushort GetProtocolID() { return ProtocolId." + curr.Value + "; }");
            sb.AppendLine("}");
            curr = curr.Next;
        }

        sb.AppendLine("//协议辅助函数");
        sb.AppendLine("public static class ProtocolHelper");
        sb.AppendLine("{");
        sb.AppendLine("\tpublic static Google.Protobuf.IMessage Parse(ushort protocolId, byte[] data)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tswitch (protocolId)");
        sb.AppendLine("\t\t{");
        curr = protoNameList.First;
        while (curr != null)
        {
            sb.AppendLine($"\t\t\tcase ProtocolId.{curr.Value}: return {curr.Value}.Parser.ParseFrom(data);");
            curr = curr.Next;
        }
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t\treturn null;");
        sb.AppendLine("\t}");
        sb.AppendLine("}");
        CreateProtoIdFile(CSProtoFilePath, sb.ToString());
    }

    private static void CreateLuaProtoIdFile()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("local ProtoIdDefine = {");
        var curr = protoNameList.First;
        while (curr != null)
        {
            int protoId = StringToCRC(Encoding.ASCII.GetBytes(curr.Value));
            //sb.AppendLine("\t['" + curr.Value + "'] = " + protoId + ",");
            sb.AppendLine("\t" + curr.Value + " = " + protoId + ",");
            sb.AppendLine("\t[" + protoId + "] = '" + curr.Value + "',");
            curr = curr.Next;
        }
        sb.AppendLine("}");
        sb.AppendLine("return ProtoIdDefine;");
        CreateProtoIdFile(LuaProtoFileName, sb.ToString());
    }

    private static void CreateProtoIdFile(string filePath, string content)
    {
        //DeleteFile(filePath);
        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(content.ToString());
                sw.Close();
            }
            fs.Close();
        }
    }

    public static void Cmd(List<string> cmds)
    {
        Process process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.WorkingDirectory = ".";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardError = true;
        process.OutputDataReceived += OutputHandler;
        process.ErrorDataReceived += ErrorDataHandler;
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        for (int i = 0; i < cmds.Count; i++)
        {
            process.StandardInput.WriteLine(cmds[i]);
        }
        process.StandardInput.WriteLine("exit");
        process.WaitForExit();

    }

    private static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (!string.IsNullOrEmpty(outLine.Data))
        {
            Console.WriteLine(outLine.Data);
        }
    }
    private static void ErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        if (!string.IsNullOrEmpty(outLine.Data))
        {
            Console.WriteLine(outLine.Data);
        }
    }

    private static void ChangeProtoContent()
    {
        Console.WriteLine("========================ChangeProtoContent==========================");
        DirectoryInfo directory = new DirectoryInfo(ProtoToCSPath);
        FileInfo[] arrFiles = directory.GetFiles("*.cs", SearchOption.AllDirectories);
        foreach (FileInfo file in arrFiles)
        {
            FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(fs, Encoding.UTF8);

            StringBuilder sb = new StringBuilder();
            int index = 0;
            bool isMath = false;
            string protoName = string.Empty;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine() ?? "";
                string s = "public sealed partial class";
                if (Regex.IsMatch(line, s))
                {
                    var curr = protoNameList.First;
                    while (curr != null)
                    {
                        string s2 = $"public sealed partial class {curr.Value} : pb::IMessage<{curr.Value}>";
                        if (Regex.IsMatch(line, s2))
                        {
                            index = 0;
                            isMath = true;
                            string changeLine = $"public sealed partial class {curr.Value} :IProto , pb::IMessage<{curr.Value}>";
                            protoName = curr.Value;
                            sb.AppendLine(changeLine);
                            //Console.WriteLine(line);
                            break;
                        }
                        curr = curr.Next;
                    }
                }
                else
                {
                    if (isMath)
                    {
                        index++;
                        if (index == 5)
                        {
                            isMath = false;
                            sb.AppendLine($"  public ushort ProtoId => ProtocolId.{protoName};");
                            sb.AppendLine($"  public string ProtoEnName => \"{protoName}\";");
                        }
                    }
                    sb.AppendLine(line);
                }
            }
            sr.Close();
            fs.Close();

            File.Delete(file.FullName);

            using (FileStream fs2 = new FileStream(file.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs2))
                {
                    sw.Write(sb.ToString());
                    sw.Close();
                }
                fs.Close();
            }
        }

        Console.WriteLine("Gen Proto Finish!");
    }

    public static void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}