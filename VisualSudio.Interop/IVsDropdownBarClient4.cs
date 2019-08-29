// IVsDropdownBarClient4 - bis es offiziel angeboten wird

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.Imaging.Interop;

// ReSharper disable once CheckNamespace
namespace Microsoft.VisualStudio.TextManager.Interop {

    [Guid("38002213-5C24-4970-BD9D-C45491879A75")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IVsDropdownBarClient4 {

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: ComAliasName("ImageParameters140.ImageMoniker")]
        ImageMoniker GetEntryImage([In] int iCombo, [In] int iIndex);

    }

}