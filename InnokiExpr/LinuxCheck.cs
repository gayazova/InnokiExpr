using Mono.Unix;

namespace InnokiExpr
{
    public class LinuxCheck
    {
        public static void Check(string filePath)
        {
            var unixFileInfo = new UnixFileInfo(filePath);
            Console.WriteLine($"Current user: { Environment.UserName}");
            Console.WriteLine($"Current user group {new UnixUserInfo(Environment.UserName).GroupName}");
            Console.WriteLine($"File Owner User: {unixFileInfo.OwnerUser.UserName}");
            Console.WriteLine($"File Owner Group: {unixFileInfo.OwnerGroup.GroupName}");
            Console.WriteLine($"Permissions: {unixFileInfo.FileAccessPermissions}");

            var userGroupInfo = new UnixUserInfo(Environment.UserName).Group;
            var userGroupMembers = userGroupInfo.GetMembers();
            var text1 = userGroupMembers == default || userGroupMembers.Count() == 0 ? "No memebers (" : $"{string.Join(",", userGroupMembers.Select(x => x.UserName))}";
            Console.WriteLine($"User Group Memeber Names {text1}");
            var groupInfo = unixFileInfo.OwnerGroup;
            var members = groupInfo.GetMemberNames();
            Console.WriteLine($"File Group Memebers: {string.Join(";", members)}");
            var memebers = groupInfo.GetMembers();
            var text = members == default || memebers.Count() == 0 ? "No members (" : $"{string.Join(",", memebers.Select(x => x.UserName))}" ;
            Console.WriteLine($"{text}");
            Console.WriteLine($"File Attributes: {unixFileInfo.FileSpecialAttributes}");
        }
    }
}
