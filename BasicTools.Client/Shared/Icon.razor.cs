using System;
using System.ComponentModel;

namespace BasicTools.Client.Shared
{
    partial class Icon
    {

    }

    public enum IconStyles
    {
        Filled,
        Outlined,
        Rounded,
        Sharp,
        TwoTone
    }

    public enum IconSizes : byte
    {
        Small = 18,
        Medium = 24,
        Large = 36,
        XLarge = 48
    }
}
