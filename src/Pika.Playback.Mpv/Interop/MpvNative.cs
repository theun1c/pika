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
    
    [LibraryImport("libmpv.so.2",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int mpv_set_option_string(nint ctx, string option, string value);
    
    [LibraryImport("libmpv.so.2",
        StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int mpv_command_string(nint ctx, string args);
    
    [LibraryImport("libmpv.so.2")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint mpv_error_string(int error);
    
}