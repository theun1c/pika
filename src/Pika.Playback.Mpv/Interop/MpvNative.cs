using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Pika.Playback.Mpv.Interop;

internal static partial class MpvNative
{
    [LibraryImport("libmpv.so.2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint mpv_create();

    [LibraryImport("libmpv.so.2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int mpv_initialize(nint ctx);
    
    [LibraryImport("libmpv.so.2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void mpv_destroy(nint ctx);
}