using System.ComponentModel;
using System.Runtime.CompilerServices;
using AdmiraltySimulatorGUI.Annotations;

namespace AdmiraltySimulatorGUI
{
    public class BaseVm : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetProperty<T>(ref T oldVal, T newVal, string propertyName)
        {
            if (oldVal == null && newVal == null)
                return;

            if (oldVal != null && oldVal.Equals(newVal))
                return;

            oldVal = newVal;
            OnPropertyChanged(propertyName);
        }
    }
}