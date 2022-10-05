namespace ProgressBarSample;

public partial class MainPage : ContentPage
{
    private const string LowerKey = "lower";
    private const string UpperKey = "upper";

    private float _lowerValue;
    public float LowerValue
    {
        get => _lowerValue;
        set
        {
            if (Math.Abs(_lowerValue - value) < 0.1f)
            {
                return;
            }

            _lowerValue = value;
            OnPropertyChanged();
        }
    }

    private float _upperValue;
    public float UpperValue
    {
        get => _upperValue;
        set
        {
            if (Math.Abs(_upperValue - value) < 0.1f)
            {
                return;
            }

            _upperValue = value;
            OnPropertyChanged();
        }
    }

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;

        var lowerAnimation = new Animation(v => AnimatedProgressBar.LowerRangeValue = (float)v, -0.4, 1.0);
        var upperAnimation = new Animation(v => AnimatedProgressBar.UpperRangeValue = (float)v, 0.0, 1.4);

        lowerAnimation.Commit(this, LowerKey, length: 1000, easing: Easing.CubicInOut, repeat: () => true);
        upperAnimation.Commit(this, UpperKey, length: 1000, easing: Easing.CubicInOut, repeat: () => true);
    }
}
