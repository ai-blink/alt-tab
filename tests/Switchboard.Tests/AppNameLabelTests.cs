using Switchboard.Core.Models;
using Switchboard.Core.Services;

namespace Switchboard.Tests;

public sealed class AppNameLabelTests
{
    [Theory]
    [InlineData("Everything", "*.html - Everything", 6, "Everyt…")]
    [InlineData("explorer", "문서 - 파일 탐색기", 6, "explor…")]
    [InlineData("KakaoTalk", "카카오톡", 5, "Kakao…")]
    [InlineData("KakaoTalk", "카카오톡", 7, "KakaoTa…")]
    [InlineData("cmd", "ComfyUI C-Rig", 6, "cmd")]
    [InlineData("Taskmgr", "작업 관리자", 7, "Taskmgr")]
    public void Label_is_truncated_to_the_selected_length(string appName, string title, int maxLength, string expected)
    {
        Assert.Equal(expected, AppNameLabel.Format(appName, title, maxLength));
    }

    [Theory]
    [InlineData("Hermes", "Hermes")]
    [InlineData("ChatGPT", "ChatGPT")]
    [InlineData("Hermes", "Hermes 비서 챗")]
    [InlineData("chrome", "Chrome - 새 탭")]
    [InlineData("", "Anything")]
    public void Label_is_hidden_when_it_repeats_the_title_start_or_is_empty(string appName, string title)
    {
        Assert.Equal(string.Empty, AppNameLabel.Format(appName, title, 6));
    }

    [Fact]
    public void Label_never_splits_a_surrogate_pair()
    {
        Assert.Equal("앱𝒜𝒜𝒜𝒜𝒜…", AppNameLabel.Format("앱𝒜𝒜𝒜𝒜𝒜𝒜", "Title", 6));
    }

    [Theory]
    [InlineData(AppNameLengthPreset.Five, 5)]
    [InlineData(AppNameLengthPreset.Six, 6)]
    [InlineData(AppNameLengthPreset.Seven, 7)]
    public void Presets_map_to_character_counts(AppNameLengthPreset preset, int expected)
    {
        Assert.Equal(expected, AppNameLabel.ToMaxLength(preset));
    }
}
