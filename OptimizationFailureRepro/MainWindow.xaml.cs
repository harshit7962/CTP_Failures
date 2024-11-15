using Microsoft.Test.Controls;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OptimizationRepro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CustomControl CustomControlWithVsm { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            CustomControlWithVsm = this.customControl;

            if (CustomControlWithVsm != null)
                CustomControlWithVsm.ApplyTemplate();
        }

        private void InitiateTest()
        {
            bool explicitStoryboardCompleted = false;
            bool proxyStoryboardCompletedAfter = false;
            double rectOpacityAtCompleted = 0.0;

            var implementationRoot = (Grid)System.Windows.Media.VisualTreeHelper.GetChild(CustomControlWithVsm, 0);
            var group = GetGroupByName(VisualStateManager.GetVisualStateGroups(implementationRoot), "groupA");
            var transition = (VisualTransition)group.Transitions[0];
            var button = (Button)implementationRoot.Children[0];

            var explicitStoryboard = transition.Storyboard;
            explicitStoryboard.Completed += (s, a) =>
            {
                rectOpacityAtCompleted = button.Opacity;
                explicitStoryboardCompleted = true;
            };

            var storyboard = new Storyboard();
            storyboard.Duration = TimeSpan.FromSeconds(2.0);
            storyboard.Completed += (s, a) =>
            {
                //explicit storyboard should have completed by now.
                if (explicitStoryboardCompleted == true)
                {
                    proxyStoryboardCompletedAfter = true;
                }
            };
            storyboard.Begin();

            VisualStateManager.GoToState(CustomControlWithVsm, "stateA", true);
        }

        protected VisualStateGroup GetGroupByName(System.Collections.IList groups, string name)
        {
            foreach (VisualStateGroup group in groups)
            {
                if (group.Name == name)
                {
                    return group;
                }
            }
            return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InitiateTest();
        }
    }
}

namespace Microsoft.Test.Controls
{
    public class ControlWithAttachedProperty : Control
    {
        public static readonly DependencyProperty CustomAttachedProperty = DependencyProperty.RegisterAttached(
            "CustomAttached", typeof(string), typeof(ControlWithAttachedProperty), null);

        public static void SetCustomAttached(Control target, string value)
        {
            target.SetValue(CustomAttachedProperty, value);
        }
        public static string GetCustomAttached(Control target)
        {
            return target.GetValue(CustomAttachedProperty) as string;
        }
    }

    public class CustomControl : Control
    {
        public static readonly DependencyProperty WorkingTagProperty = DependencyProperty.Register(
            "WorkingTag", typeof(object), typeof(CustomControl), null);

        public static readonly DependencyProperty CustomStringProperty = DependencyProperty.Register(
            "CustomString", typeof(string), typeof(CustomControl), null);

        public static readonly DependencyProperty CustomIntProperty = DependencyProperty.Register(
            "CustomInt", typeof(int), typeof(CustomControl), null);

        public static readonly DependencyProperty CustomSizeProperty = DependencyProperty.Register(
            "CustomSize", typeof(Size), typeof(CustomControl), null);

        public static readonly DependencyProperty CustomThicknessProperty = DependencyProperty.Register(
            "CustomThickness", typeof(Thickness), typeof(CustomControl), null);

        public object WorkingTag
        {
            get { return GetValue(WorkingTagProperty); }
            set { SetValue(WorkingTagProperty, value); }
        }

        public string CustomString
        {
            get { return (string)GetValue(CustomStringProperty); }
            set { SetValue(CustomStringProperty, value); }
        }

        public int CustomInt
        {
            get { return (int)GetValue(CustomIntProperty); }
            set { SetValue(CustomIntProperty, value); }
        }

        public Size CustomSize
        {
            get { return (Size)GetValue(CustomSizeProperty); }
            set { SetValue(CustomSizeProperty, value); }
        }

        public Thickness CustomThickness
        {
            get { return (Thickness)GetValue(CustomThicknessProperty); }
            set { SetValue(CustomThicknessProperty, value); }
        }

        public double NotDp { get; set; }

        public DependencyObject PublicGetTemplateChild(string name)
        {
            return GetTemplateChild(name);
        }
    }
}
