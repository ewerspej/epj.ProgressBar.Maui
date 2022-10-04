namespace ProgressBarSample;

public partial class MainPage : ContentPage
{
    private const string LowerKey = "lower";
    private const string UpperKey = "upper";

    private readonly Animation _lowerAnimation;
    private readonly Animation _upperAnimation;

    private bool _isAnimating;

    private bool _animate;
    public bool Animate
    {
        get => _animate;
        set
        {
            if (_animate == value)
            {
                return;
            }

            _animate = value;
            OnPropertyChanged();

            ToggleAnimation();
        }
    }

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

        _lowerAnimation = new Animation(v => LowerValue = (float)v, -0.4, 1.0);
        _upperAnimation = new Animation(v => UpperValue = (float)v, 0.0, 1.4);
    }

    private void ToggleAnimation()
    {
        if (_isAnimating)
        {
            //TODO: stop animation here
            this.AbortAnimation(LowerKey);
            this.AbortAnimation(UpperKey);

            LowerValue = 0.0f;
            UpperValue = 0.0f;
        }
        else
        {
            //TODO: start animation here
            _lowerAnimation.Commit(this, LowerKey, length: 1000, easing: Easing.CubicInOut, repeat: () => true);
            _upperAnimation.Commit(this, UpperKey, length: 1000, easing: Easing.CubicInOut, repeat: () => true);
        }

        _isAnimating = !_isAnimating;
    }
}

