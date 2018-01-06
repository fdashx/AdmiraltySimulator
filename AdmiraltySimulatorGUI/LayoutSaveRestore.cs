using System.IO;
using DevExpress.Mvvm;

namespace AdmiraltySimulatorGUI
{
    public abstract class LayoutSaveRestore : BaseVm, ISupportServices
    {
        private ILayoutSerializationService _layoutSerializationService;
        private IServiceContainer _serviceContainer;

        private IServiceContainer ServiceContainer =>
            _serviceContainer ?? (_serviceContainer = new ServiceContainer(this));

        private ILayoutSerializationService LayoutSerializationService =>
            _layoutSerializationService ??
            (_layoutSerializationService = ServiceContainer.GetService<ILayoutSerializationService>());

        IServiceContainer ISupportServices.ServiceContainer => ServiceContainer;

        protected void SaveLayout(string filePath)
        {
            File.WriteAllText(filePath, LayoutSerializationService.Serialize());
        }

        protected void LoadLayout(string filePath)
        {
            LayoutSerializationService.Deserialize(File.ReadAllText(filePath));
        }
    }
}