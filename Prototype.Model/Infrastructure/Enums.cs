using System;

namespace Prototype.Infrastructure
{
    public enum Access
    {
        None,
        Read,
        ReadWrite
    }

    public static class AccessObject
    {
        public static Guid RestrictedVm = Guid.Parse("D33F8CF6-D0BA-48CD-B9AD-C8EC088CBD15");
    }
}
