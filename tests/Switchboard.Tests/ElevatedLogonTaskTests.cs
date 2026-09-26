using System.Xml.Linq;
using Switchboard.Native;

namespace Switchboard.Tests;

public sealed class ElevatedLogonTaskTests
{
    private static readonly XNamespace Task = "http://schemas.microsoft.com/windows/2004/02/mit/task";

    [Fact]
    public void Task_xml_starts_the_app_elevated_at_the_users_sign_in()
    {
        var document = XDocument.Parse(ElevatedLogonTask.BuildTaskXml(@"C:\Tools\Switch & Board\Switchboard.App.exe", @"PC\user"));
        var root = document.Root!;

        Assert.Equal("1.2", (string?)root.Attribute("version"));
        Assert.Equal(@"PC\user", (string?)root.Descendants(Task + "LogonTrigger").Single().Element(Task + "UserId"));
        Assert.Equal("HighestAvailable", (string?)root.Descendants(Task + "RunLevel").Single());
        Assert.Equal(@"C:\Tools\Switch & Board\Switchboard.App.exe", (string?)root.Descendants(Task + "Command").Single());
        Assert.Equal(@"C:\Tools\Switch & Board", (string?)root.Descendants(Task + "WorkingDirectory").Single());
    }

    [Fact]
    public void Task_settings_keep_schema_order_and_normal_priority()
    {
        var settings = XDocument.Parse(ElevatedLogonTask.BuildTaskXml(@"C:\Switchboard.App.exe", "user"))
            .Root!.Element(Task + "Settings")!;

        // Task Scheduler rejects out-of-order <Settings> children; the default priority 7 is below normal.
        Assert.Equal(
            ["MultipleInstancesPolicy", "DisallowStartIfOnBatteries", "StopIfGoingOnBatteries", "ExecutionTimeLimit", "Priority"],
            settings.Elements().Select(element => element.Name.LocalName));
        Assert.Equal("IgnoreNew", (string?)settings.Element(Task + "MultipleInstancesPolicy"));
        Assert.Equal("PT0S", (string?)settings.Element(Task + "ExecutionTimeLimit"));
        Assert.Equal("4", (string?)settings.Element(Task + "Priority"));
    }

    [Fact]
    public void Task_name_is_ascii()
    {
        Assert.All(ElevatedLogonTask.TaskName, character => Assert.True(character < 128));
    }
}
