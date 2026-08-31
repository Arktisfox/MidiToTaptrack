using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace TaptrackTools;

internal static class FileDialogs
{
    public static async Task<string?> OpenFile(Window owner, string filterName, params string[] extensions)
    {
        var files = await owner.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = [Filter(filterName, extensions)]
        });
        return files.Count > 0 ? files[0].TryGetLocalPath() : null;
    }

    public static async Task<string?> SaveFile(Window owner, string filterName, string defaultFileName, params string[] extensions)
    {
        var file = await owner.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            SuggestedFileName = defaultFileName,
            DefaultExtension = extensions[0],
            FileTypeChoices = [Filter(filterName, extensions)]
        });
        return file?.TryGetLocalPath();
    }

    private static FilePickerFileType Filter(string name, string[] extensions) =>
        new(name) { Patterns = [.. extensions.Select(e => "*." + e)] };
}
