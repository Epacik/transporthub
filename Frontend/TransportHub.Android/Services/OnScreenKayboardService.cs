using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportHub.Services;

namespace TransportHub.Android.Services
{
    internal class OnScreenKayboardService : IOnScreenKeyboardService
    {
        private LayoutListener _listener;

        public OnScreenKayboardService(LayoutListener listener)
        {
            _listener = listener;
            _listener.KeyboardHeightChanged += Listener_KeyboardHeightChanged;
        }

        private void Listener_KeyboardHeightChanged(double height)
        {
            CurrentHeight = height;
            HeightChanged?.Invoke(height);
        }

        public double CurrentHeight { get; private set; }

        public event Action<double>? HeightChanged;
    }
}
