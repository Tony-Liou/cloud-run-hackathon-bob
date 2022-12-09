using System.ComponentModel;

namespace CloudRunHackathonCsharp;

public enum Action
{
    /// <summary>
    /// Move forward
    /// </summary>
    [Description("F")]
    F,
    /// <summary>
    /// Turn right
    /// </summary>
    [Description("R")]
    R,
    /// <summary>
    /// Turn left
    /// </summary>
    [Description("L")]
    L,
    /// <summary>
    /// Throw
    /// </summary>
    [Description("T")]
    T
}
