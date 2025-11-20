using Microsoft.Win32;

namespace AdmiraltySimulatorGUI
{
    public interface IFileDialogService
    {
        string OpenFile();
    }

    public class FileDialogService : IFileDialogService
    {
        public string OpenFile()
        {
            var dialog = new OpenFileDialog();
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}