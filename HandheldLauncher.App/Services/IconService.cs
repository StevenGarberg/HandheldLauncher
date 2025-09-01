using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace HandheldLauncher.App.Services;

public static class IconService
{
    /// <summary>
    /// Returns a data URL (image/png;base64,…) for the executable’s associated icon.
    /// Returns null if path invalid or extraction fails.
    /// </summary>
    public static string GetExeIconDataUrl(string exePath, int size = 64)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath)) return null;

            using var ico = Icon.ExtractAssociatedIcon(exePath);
            if (ico == null) return null;

            using var bmp = new Bitmap(ico.ToBitmap(), new Size(size, size));
            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            var b64 = Convert.ToBase64String(ms.ToArray());
            return $"data:image/png;base64,{b64}";
        }
        catch
        {
            return null;
        }
    }
}

public static class StockIconHelper
{
    public enum SHSTOCKICONID : uint
    {
        SIID_FOLDER = 3,
        SIID_DRIVEFIXED = 8,
        SIID_DRIVEREMOVE = 9,
        SIID_DRIVECD = 11,
        SIID_DRIVENET = 10,
        SIID_DRIVEUNKNOWN = 12
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct SHSTOCKICONINFO
    {
        public uint cbSize;
        public IntPtr hIcon;
        public int iSysIconIndex;
        public int iIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szPath;
    }

    [DllImport("shell32.dll", SetLastError = false)]
    private static extern int SHGetStockIconInfo(SHSTOCKICONID siid, uint uFlags, ref SHSTOCKICONINFO psii);

    private const uint SHGSI_ICON = 0x000000100;
    private const uint SHGSI_LARGEICON = 0x000000000; // or SHGSI_SMALLICON for 16px

    public static string? GetStockIconBase64(SHSTOCKICONID id, int size = 64)
    {
        var info = new SHSTOCKICONINFO { cbSize = (uint)Marshal.SizeOf(typeof(SHSTOCKICONINFO)) };
        int hr = SHGetStockIconInfo(id, SHGSI_ICON | SHGSI_LARGEICON, ref info);
        if (hr != 0) return null;

        using var ico = Icon.FromHandle(info.hIcon);
        using var bmp = new Bitmap(ico.ToBitmap(), new Size(size, size));
        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);
        return $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
    }
}