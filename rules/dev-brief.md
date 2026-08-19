# Dev Brief

- Product: Switchboard, a Windows 11 window switcher utility.
- Audience: general Windows users who want clearer window switching.
- Stack: `.NET 10`, WPF, CommunityToolkit.Mvvm, Microsoft.Extensions.DependencyInjection.
- Current UX direction: thumbnail-first Grid/Compact/List overlay, owner-modal sidebar settings, and direct-drag position persistence.
- Core quality bar: stable layout, readable thumbnails/titles, obvious selected state, keyboard-first flow.
- Current baseline: native Win32 enumeration and DWM previews are active; `v0.3.1` Portable is published with 57 passing tests, with post-release UI acceptance still pending.
