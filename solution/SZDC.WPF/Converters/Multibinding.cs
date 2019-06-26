using System.Windows.Data;

namespace SZDC.Wpf.Converters {

    public class MultiBinding : System.Windows.Data.MultiBinding {

        public MultiBinding(BindingBase b1, BindingBase b2) {
            Bindings.Add(b1);
            Bindings.Add(b2);
        }
    }
}
