using System.ComponentModel;

namespace Nxt.NET.Env
{
    public enum ServerStatus
    {
        [Description("Loading Database")]
        BEFORE_DATABASE,

        [Description("Loading Resources")]
        AFTER_DATABASE,

        [Description("Online")]
        STARTED
    }
}