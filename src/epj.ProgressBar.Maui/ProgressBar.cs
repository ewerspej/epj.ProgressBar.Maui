using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace epj.ProgressBar.Maui;

public class ProgressBar : SKCanvasView
{
    private SKCanvas _canvas;
    private SKRect _drawRect;
    private SKImageInfo _info;

    private bool _isEnableSegment = false;

    public float Progress
    {
        get => (float)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public Color ProgressColor
    {
        get => (Color)GetValue(ProgressColorProperty);
        set => SetValue(ProgressColorProperty, value);
    }

    public Color GradientColor
    {
        get => (Color)GetValue(GradientColorProperty);
        set => SetValue(GradientColorProperty, value);
    }

    public Color BaseColor
    {
        get => (Color)GetValue(BaseColorProperty);
        set => SetValue(BaseColorProperty, value);
    }

    public bool UseRange
    {
        get => (bool)GetValue(UseRangeProperty);
        set => SetValue(UseRangeProperty, value);
    }

    public float LowerRangeValue
    {
        get => (float)GetValue(LowerRangeValueProperty);
        set => SetValue(LowerRangeValueProperty, value);
    }

    public float UpperRangeValue
    {
        get => (float)GetValue(UpperRangeValueProperty);
        set => SetValue(UpperRangeValueProperty, value);
    }

    public bool UseGradient
    {
        get => (bool)GetValue(UseGradientProperty);
        set => SetValue(UseGradientProperty, value);
    }

    public bool RoundCaps
    {
        get => (bool)GetValue(RoundCapsProperty);
        set => SetValue(RoundCapsProperty, value);
    }

    public bool UseSegment
    {
        get => (bool)GetValue(UseSegmentProperty);
        set => SetValue(UseSegmentProperty, value);
    }

    public float SegmentSpacing
    {
        get => (float)GetValue(SegmentSpacingProperty);
        set => SetValue(SegmentSpacingProperty, value);
    }

    public int SegmentCount
    {
        get => (int)GetValue(SegmentCountProperty);
        set => SetValue(SegmentCountProperty, value);
    }

    public int ProgressSegment
    {
        get => (int)GetValue(ProgressSegmentProperty);
        set => SetValue(ProgressSegmentProperty, value);
    }

    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(nameof(Progress), typeof(float), typeof(ProgressBar), 0.0f, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty ProgressColorProperty = BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(ProgressBar), Colors.BlueViolet, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty GradientColorProperty = BindableProperty.Create(nameof(GradientColor), typeof(Color), typeof(ProgressBar), Colors.BlueViolet, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty BaseColorProperty = BindableProperty.Create(nameof(BaseColor), typeof(Color), typeof(ProgressBar), Colors.LightGray, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty UseRangeProperty = BindableProperty.Create(nameof(UseRange), typeof(bool), typeof(ProgressBar), false, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty LowerRangeValueProperty = BindableProperty.Create(nameof(LowerRangeValue), typeof(float), typeof(ProgressBar), 0.0f, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty UpperRangeValueProperty = BindableProperty.Create(nameof(UpperRangeValue), typeof(float), typeof(ProgressBar), 0.0f, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty UseGradientProperty = BindableProperty.Create(nameof(UseGradient), typeof(bool), typeof(ProgressBar), false, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty RoundCapsProperty = BindableProperty.Create(nameof(RoundCaps), typeof(bool), typeof(ProgressBar), false, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty UseSegmentProperty = BindableProperty.Create(nameof(UseSegment), typeof(bool), typeof(ProgressBar), false, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty SegmentSpacingProperty = BindableProperty.Create(nameof(SegmentSpacing), typeof(float), typeof(ProgressBar), 0.0f, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty SegmentCountProperty = BindableProperty.Create(nameof(SegmentCount), typeof(int), typeof(ProgressBar), 1, propertyChanged: OnBindablePropertyChanged);
    public static readonly BindableProperty ProgressSegmentProperty = BindableProperty.Create(nameof(ProgressSegment), typeof(int), typeof(ProgressBar), 0, propertyChanged: OnBindablePropertyChanged);


    private static void OnBindablePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((ProgressBar)bindable).InvalidateSurface();
    }

    public ProgressBar()
    {
        IgnorePixelScaling = false;
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);

        _canvas = e.Surface.Canvas;
        _canvas.Clear();

        _info = e.Info;

        _drawRect = new SKRect(0, 0, _info.Width, _info.Height);

        DrawBase();
        DrawProgress();
    }

    private void DrawBase()
    {
        using var basePath = new SKPath();

        if (RoundCaps)
        {
            basePath.AddRoundRect(_drawRect, _drawRect.Height / 2, _drawRect.Height / 2);
        }
        else
        {
            basePath.AddRect(_drawRect);
        }

        _canvas.ClipPath(basePath);
        _canvas.DrawPath(basePath, new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = UseSegment && SegmentCount > 0 ? Colors.Transparent.ToSKColor() : BaseColor.ToSKColor(),
            IsAntialias = true
        });
    }

    private void DrawProgress()
    {
        using var progressPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        if (UseGradient)
        {
            progressPaint.Shader = SKShader.CreateLinearGradient(
                new SKPoint(_drawRect.Left, _drawRect.MidY),
                new SKPoint(_drawRect.Right, _drawRect.MidY),
                new[] { GradientColor.ToSKColor(), ProgressColor.ToSKColor() },
                new[] { 0.0f, 1.0f },
                SKShaderTileMode.Clamp
            );
        }
        else
        {
            progressPaint.Color = ProgressColor.ToSKColor();
        }

        if (UseSegment && SegmentCount > 0 && _isEnableSegment)
        {
            using SKPaint basePaint = GenerateSegment(progressPaint);
        }
        else
        {
            using SKPath progressPath = GenerateProgress(progressPaint);
        }
    }

    private SKPath GenerateProgress(SKPaint progressPaint)
    {
        var progressPath = new SKPath();

        var progressRect = UseRange
            ? new SKRect(_info.Width * LowerRangeValue, 0, _info.Width * UpperRangeValue, _info.Height)
            : new SKRect(0, 0, _info.Width * Progress, _info.Height);

        if (RoundCaps)
        {
            progressPath.AddRoundRect(progressRect, progressRect.Height / 2, progressRect.Height / 2);
        }
        else
        {
            progressPath.AddRect(progressRect);
        }

        _canvas.DrawPath(progressPath, progressPaint);
        return progressPath;
    }

    private SKPaint GenerateSegment(SKPaint progressPaint)
    {
        var basePaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            Color = BaseColor.ToSKColor()
        };

        float segmentWidth = (_info.Width - (SegmentSpacing * (SegmentCount - 1))) / SegmentCount;
        for (int i = 0; i < SegmentCount; i++)
        {
            var segmentRect = new SKRect(
                i * (segmentWidth + SegmentSpacing),
                0,
                i * (segmentWidth + SegmentSpacing) + segmentWidth,
                _info.Height
            );

            using var segmentPath = new SKPath();
            if (RoundCaps)
            {
                segmentPath.AddRoundRect(segmentRect, segmentRect.Height / 2, segmentRect.Height / 2);
            }
            else
            {
                segmentPath.AddRect(segmentRect);
            }

            if (ProgressSegment < i + 1)
                _canvas.DrawPath(segmentPath, basePaint);
            else
                _canvas.DrawPath(segmentPath, progressPaint);
        }

        return basePaint;
    }
}
