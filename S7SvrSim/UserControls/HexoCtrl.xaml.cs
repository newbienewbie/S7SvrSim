using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace S7Svr.Simulator.UserControls
{
    /// <summary>
    /// Interaction logic for HexoCtrl.xaml
    /// </summary>
    public partial class HexoCtrl : UserControl
    {
        public HexoCtrl()
        {
            InitializeComponent();
        }

        #region


        public int DBNumber
        {
            get { return (int)GetValue(DBNumberProperty); }
            set { SetValue(DBNumberProperty, value); }
        }
        public static readonly DependencyProperty DBNumberProperty = DependencyProperty.Register(nameof(DBNumber), typeof(int), typeof(HexoCtrl), new PropertyMetadata(0));
        #endregion



        #region 
        public int DBSize
        {
            get { return (int)GetValue(DBSizeProperty); }
            set { SetValue(DBSizeProperty, value); }
        }
        public static readonly DependencyProperty DBSizeProperty = DependencyProperty.Register(nameof(DBSize), typeof(int), typeof(HexoCtrl), new PropertyMetadata(0));
        #endregion





        #region
        public IList<byte> Bytes
        {
            get { return (byte[])GetValue(BytesProperty); }
            set { SetValue(BytesProperty, value); }
        }

        public static readonly DependencyProperty BytesProperty = DependencyProperty.Register(nameof(Bytes), typeof(IList<byte>), typeof(HexoCtrl), new PropertyMetadata(default));
        #endregion

    }
}
