namespace Standard
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media.Animation;

    internal enum AnimationType
    {
        Automatic,
        FromTo,
        FromBy,
        From,
        To,
        By,
    }

    /// <summary>
    /// Animates the value of a CornerRadius property using linear interpolation
    /// between two values.  The values are determined by the combination of
    /// From, To, or By values that are set on the animation.
    /// </summary>
    internal partial class CornerRadiusAnimation : CornerRadiusAnimationBase
    {
        #region Data

        /// <summary>
        /// This is used if the user has specified From, To, and/or By values.
        /// </summary>
        private CornerRadius[] _keyValues;

        private AnimationType _animationType;        
        private bool _isAnimationFunctionValid;

        #endregion

        #region Constructors

        /// <summary>
        /// Static ctor for CornerRadiusAnimation establishes
        /// dependency properties, using as much shared data as possible.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static CornerRadiusAnimation()
        {
            Type typeofProp = typeof(CornerRadius?);
            Type typeofThis = typeof(CornerRadiusAnimation);
            PropertyChangedCallback propCallback = new PropertyChangedCallback(AnimationFunction_Changed);
            ValidateValueCallback validateCallback = new ValidateValueCallback(ValidateFromToOrByValue);

            FromProperty = DependencyProperty.Register(
                "From",
                typeofProp,
                typeofThis,
                new PropertyMetadata((CornerRadius?)null, propCallback),
                validateCallback);

            ToProperty = DependencyProperty.Register(
                "To",
                typeofProp,
                typeofThis,
                new PropertyMetadata((CornerRadius?)null, propCallback),
                validateCallback);

            ByProperty = DependencyProperty.Register(
                "By",
                typeofProp,
                typeofThis,
                new PropertyMetadata((CornerRadius?)null, propCallback),
                validateCallback);

        }


        /// <summary>
        /// Creates a new CornerRadiusAnimation with all properties set to
        /// their default values.
        /// </summary>
        public CornerRadiusAnimation()
            : base()
        {
        }

        /// <summary>
        /// Creates a new CornerRadiusAnimation that will animate a
        /// CornerRadius property from its base value to the value specified
        /// by the "toValue" parameter of this constructor.
        /// </summary>
        public CornerRadiusAnimation(CornerRadius toValue, Duration duration)
            : this()
        {
            To = toValue;
            Duration = duration;
        }

        /// <summary>
        /// Creates a new CornerRadiusAnimation that will animate a
        /// CornerRadius property from its base value to the value specified
        /// by the "toValue" parameter of this constructor.
        /// </summary>
        public CornerRadiusAnimation(CornerRadius toValue, Duration duration, FillBehavior fillBehavior)
            : this()
        {
            To = toValue;
            Duration = duration;
            FillBehavior = fillBehavior;
        }

        /// <summary>
        /// Creates a new CornerRadiusAnimation that will animate a
        /// CornerRadius property from the "fromValue" parameter of this constructor
        /// to the "toValue" parameter.
        /// </summary>
        public CornerRadiusAnimation(CornerRadius fromValue, CornerRadius toValue, Duration duration)
            : this()
        {
            From = fromValue;
            To = toValue;
            Duration = duration;
        }

        /// <summary>
        /// Creates a new CornerRadiusAnimation that will animate a
        /// CornerRadius property from the "fromValue" parameter of this constructor
        /// to the "toValue" parameter.
        /// </summary>
        public CornerRadiusAnimation(CornerRadius fromValue, CornerRadius toValue, Duration duration, FillBehavior fillBehavior)
            : this()
        {
            From = fromValue;
            To = toValue;
            Duration = duration;
            FillBehavior = fillBehavior;
        }

        #endregion

        #region Freezable

        /// <summary>
        /// Creates a copy of this CornerRadiusAnimation
        /// </summary>
        /// <returns>The copy</returns>
        public new CornerRadiusAnimation Clone()
        {
            return (CornerRadiusAnimation)base.Clone();
        }

        //
        // Note that we don't override the Clone virtuals (CloneCore, CloneCurrentValueCore,
        // GetAsFrozenCore, and GetCurrentValueAsFrozenCore) even though this class has state
        // not stored in a DP.
        // 
        // We don't need to clone _animationType and _keyValues because they are the the cached 
        // results of animation function validation, which can be recomputed.  The other remaining
        // field, isAnimationFunctionValid, defaults to false, which causes this recomputation to happen.
        //

        /// <summary>
        /// Implementation of <see cref="System.Windows.Freezable.CreateInstanceCore">Freezable.CreateInstanceCore</see>.
        /// </summary>
        /// <returns>The new Freezable.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new CornerRadiusAnimation();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Calculates the value this animation believes should be the current value for the property.
        /// </summary>
        /// <param name="defaultOriginValue">
        /// This value is the suggested origin value provided to the animation
        /// to be used if the animation does not have its own concept of a
        /// start value. If this animation is the first in a composition chain
        /// this value will be the snapshot value if one is available or the
        /// base property value if it is not; otherise this value will be the 
        /// value returned by the previous animation in the chain with an 
        /// animationClock that is not Stopped.
        /// </param>
        /// <param name="defaultDestinationValue">
        /// This value is the suggested destination value provided to the animation
        /// to be used if the animation does not have its own concept of an
        /// end value. This value will be the base value if the animation is
        /// in the first composition layer of animations on a property; 
        /// otherwise this value will be the output value from the previous 
        /// composition layer of animations for the property.
        /// </param>
        /// <param name="animationClock">
        /// This is the animationClock which can generate the CurrentTime or
        /// CurrentProgress value to be used by the animation to generate its
        /// output value.
        /// </param>
        /// <returns>
        /// The value this animation believes should be the current value for the property.
        /// </returns>
        protected override CornerRadius GetCurrentValueCore(CornerRadius defaultOriginValue, CornerRadius defaultDestinationValue, AnimationClock animationClock)
        {
            Debug.Assert(animationClock.CurrentState != ClockState.Stopped);

            if (!_isAnimationFunctionValid)
            {
                ValidateAnimationFunction();
            }

            double progress = animationClock.CurrentProgress.Value;

            CornerRadius   from        = new CornerRadius();
            CornerRadius   to          = new CornerRadius();
            CornerRadius   accumulated = new CornerRadius();
            CornerRadius   foundation  = new CornerRadius();

            // need to validate the default origin and destination values if 
            // the animation uses them as the from, to, or foundation values
            bool validateOrigin = false;
            bool validateDestination = false;

            switch(_animationType)
            {
                case AnimationType.Automatic:

                    from    = defaultOriginValue;
                    to      = defaultDestinationValue;

                    validateOrigin = true;
                    validateDestination = true;

                    break;

                case AnimationType.From:

                    from    = _keyValues[0];
                    to      = defaultDestinationValue;

                    validateDestination = true;

                    break;

                case AnimationType.To:

                    from = defaultOriginValue;
                    to = _keyValues[0];

                    validateOrigin = true;

                    break;

                case AnimationType.By:

                    // According to the SMIL specification, a By animation is
                    // always additive.  But we don't force this so that a
                    // user can re-use a By animation and have it replace the
                    // animations that precede it in the list without having
                    // to manually set the From value to the base value.

                    to          = _keyValues[0];
                    foundation  = defaultOriginValue;

                    validateOrigin = true;

                    break;

                case AnimationType.FromTo:

                    from    = _keyValues[0];
                    to      = _keyValues[1];

                    if (IsAdditive)
                    {
                        foundation = defaultOriginValue;
                        validateOrigin = true;
                    }

                    break;

                case AnimationType.FromBy:

                    from    = _keyValues[0];
                    to      = _AddCornerRadius(_keyValues[0], _keyValues[1]);

                    if (IsAdditive)
                    {
                        foundation = defaultOriginValue;
                        validateOrigin = true;
                    }

                    break;

                default:

                    Debug.Fail("Unknown animation type.");

                    break;
            }

            if (validateOrigin 
                && !_IsValidAnimationValueCornerRadius(defaultOriginValue))
            {
                throw new InvalidOperationException();
            }

            if (validateDestination 
                && !_IsValidAnimationValueCornerRadius(defaultDestinationValue))
            {
                throw new InvalidOperationException();
            }


            if (IsCumulative)
            {
                double currentRepeat = (double)(animationClock.CurrentIteration - 1);

                if (currentRepeat > 0.0)
                {
                    CornerRadius accumulator = _SubtractCornerRadius(to, from);

                    accumulated = _ScaleCornerRadius(accumulator, currentRepeat);
                }
            }

            // return foundation + accumulated + from + ((to - from) * progress)

            return _AddCornerRadius(
                foundation, 
                _AddCornerRadius(
                    accumulated,
                    _InterpolateCornerRadius(from, to, progress)));
        }

        private CornerRadius _InterpolateCornerRadius(CornerRadius from, CornerRadius to, double progress)
        {
            return this._ScaleCornerRadius(this._SubtractCornerRadius(to, from), progress);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private CornerRadius _ScaleCornerRadius(CornerRadius first, double currentRepeat)
        {
            return new CornerRadius(
                first.TopLeft * currentRepeat,
                first.TopRight * currentRepeat,
                first.BottomRight * currentRepeat,
                first.BottomLeft * currentRepeat);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private CornerRadius _SubtractCornerRadius(CornerRadius first, CornerRadius second)
        {
            return new CornerRadius(
                first.TopLeft - second.TopLeft,
                first.TopRight - second.TopRight,
                first.BottomRight - second.BottomRight,
                first.BottomLeft - second.BottomLeft);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "defaultOriginValue")]
        private static bool _IsValidAnimationValueCornerRadius(CornerRadius defaultOriginValue)
        {
            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private CornerRadius _AddCornerRadius(CornerRadius first, CornerRadius second)
        {
            return new CornerRadius(
                first.TopLeft + second.TopLeft, 
                first.TopRight + second.TopRight, 
                first.BottomRight + second.BottomRight, 
                first.BottomLeft + second.BottomLeft);
        }

        private void ValidateAnimationFunction()
        {
            _animationType = AnimationType.Automatic;
            _keyValues = null;

            if (From.HasValue)
            {
                if (To.HasValue)
                {
                    _animationType = AnimationType.FromTo;
                    _keyValues = new CornerRadius[2];
                    _keyValues[0] = From.Value;
                    _keyValues[1] = To.Value;
                }
                else if (By.HasValue)
                {
                    _animationType = AnimationType.FromBy;
                    _keyValues = new CornerRadius[2];
                    _keyValues[0] = From.Value;
                    _keyValues[1] = By.Value;
                }
                else
                {
                    _animationType = AnimationType.From;
                    _keyValues = new CornerRadius[1];
                    _keyValues[0] = From.Value;
                }
            }
            else if (To.HasValue)
            {
                _animationType = AnimationType.To;
                _keyValues = new CornerRadius[1];
                _keyValues[0] = To.Value;
            }
            else if (By.HasValue)
            {
                _animationType = AnimationType.By;
                _keyValues = new CornerRadius[1];
                _keyValues[0] = By.Value;
            }

            _isAnimationFunctionValid = true;
        }

        #endregion

        #region Properties

        private static void AnimationFunction_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CornerRadiusAnimation a = (CornerRadiusAnimation)d;

            a._isAnimationFunctionValid = false;
            //a.PropertyChanged(e.Property);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dependencyProperty"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void PropertyChanged(DependencyProperty dependencyProperty)
        {
            //throw new NotImplementedException();
        }

        private static bool ValidateFromToOrByValue(object value)
        {
            CornerRadius? typedValue = (CornerRadius?)value;

            if (typedValue.HasValue)
            {
                return _IsValidAnimationValueCornerRadius(typedValue.Value);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// FromProperty
        /// </summary>                                 
        public static readonly DependencyProperty FromProperty;

        /// <summary>
        /// From
        /// </summary>
        public CornerRadius? From                
        {
            get
            {
                return (CornerRadius?)GetValue(FromProperty);
            }
            set
            {
                SetValue(FromProperty, value);
            }
        }

        /// <summary>
        /// ToProperty
        /// </summary>
        public static readonly DependencyProperty ToProperty;

        /// <summary>
        /// To
        /// </summary>
        public CornerRadius? To                
        {
            get
            {
                return (CornerRadius?)GetValue(ToProperty);
            }
            set
            {
                SetValue(ToProperty, value);
            }
        }

        /// <summary>
        /// ByProperty
        /// </summary>
        public static readonly DependencyProperty ByProperty;

        /// <summary>
        /// By
        /// </summary>
        public CornerRadius? By                
        {
            get
            {
                return (CornerRadius?)GetValue(ByProperty);
            }
            set
            {
                SetValue(ByProperty, value);
            }
        }


        /// <summary>
        /// If this property is set to true the animation will add its value to
        /// the base value instead of replacing it entirely.
        /// </summary>
        public bool IsAdditive         
        { 
            get
            {
                return (bool)GetValue(IsAdditiveProperty);
            }
            set
            {
                SetValue(IsAdditiveProperty, value);
            }
        }

        /// <summary>
        /// It this property is set to true, the animation will accumulate its
        /// value over repeats.  For instance if you have a From value of 0.0 and
        /// a To value of 1.0, the animation return values from 1.0 to 2.0 over
        /// the second reteat cycle, and 2.0 to 3.0 over the third, etc.
        /// </summary>
        public bool IsCumulative      
        { 
            get
            {
                return (bool)GetValue(IsCumulativeProperty);
            }
            set
            {
                SetValue(IsCumulativeProperty, value);
            }
        }

        #endregion
    }
}
