using Android.Graphics;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Android;

public class LayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
{
    public LayoutListener(View rootView)
    {
        RootView = rootView ?? throw new ArgumentNullException(nameof(rootView));
    }
    public View RootView { get; private set; }

    public event Action<double>? KeyboardHeightChanged;
    public void OnGlobalLayout()
    {
        Rect r = new Rect();

        RootView.GetWindowVisibleDisplayFrame(r);

        int screenHeight = RootView?.RootView?.Height ?? 0;
        int keyboardHeight = screenHeight - (r.Bottom);

        KeyboardHeightChanged?.Invoke(keyboardHeight);
    }
}
