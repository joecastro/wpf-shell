/**************************************************************************\
    Copyright Microsoft Corporation. All Rights Reserved.
\**************************************************************************/
#if !DOT_NET_4

namespace Microsoft.Windows.Shell
{
    public class JumpPath : JumpItem
    {
        public JumpPath()
        {}

        public string Path { get; set; }
    }
}

#endif