// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelixViewport3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A control that contains a <see cref="Viewport3D"/> and a <see cref="CameraController"/> .
    /// </summary>
    /// <example>
    /// The following XAML code shows how to create a 3D view 
    /// <code>
    /// <Window x:Class="..." xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:h="clr-namespace:HelixToolkit;assembly=HelixToolkit.Wpf">
    ///                                                             <h:HelixViewport3D></h:HelixViewport3D>
    ///                                                           </Window>
    ///                                                         </code>
    /// </example>
    [ContentProperty("Children")]
    [TemplatePart(Name = "PART_CameraController", Type = typeof(CameraController))]
    [TemplatePart(Name = "PART_AdornerLayer", Type = typeof(AdornerDecorator))]
    [TemplatePart(Name = "PART_CoordinateView", Type = typeof(Viewport3D))]
    [TemplatePart(Name = "PART_ViewCubeViewport", Type = typeof(Viewport3D))]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public class HelixViewport3D : ItemsControl, IHelixViewport3D
    {
        #region Constants and Fields

        /// <summary>
        ///   The default camera property.
        /// </summary>
        public static readonly DependencyProperty DefaultCameraProperty = DependencyProperty.Register(
            "DefaultCamera", typeof(ProjectionCamera), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        ///   The back view gesture property.
        /// </summary>
        public static readonly DependencyProperty BackViewGestureProperty =
            DependencyProperty.Register(
                "BackViewGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new KeyGesture(Key.B, ModifierKeys.Control)));

        /// <summary>
        ///   The bottom view gesture property.
        /// </summary>
        public static readonly DependencyProperty BottomViewGestureProperty =
            DependencyProperty.Register(
                "BottomViewGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new KeyGesture(Key.D, ModifierKeys.Control)));

        /// <summary>
        ///   The camera changed event.
        /// </summary>
        public static readonly RoutedEvent CameraChangedEvent = EventManager.RegisterRoutedEvent(
            "CameraChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HelixViewport3D));

        /// <summary>
        ///   The camera inertia factor property.
        /// </summary>
        public static readonly DependencyProperty CameraInertiaFactorProperty =
            DependencyProperty.Register(
                "CameraInertiaFactor", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(0.93));

        /// <summary>
        ///   The camera info property.
        /// </summary>
        public static readonly DependencyProperty CameraInfoProperty = DependencyProperty.Register(
            "CameraInfo", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        ///   The camera mode property.
        /// </summary>
        public static readonly DependencyProperty CameraModeProperty = DependencyProperty.Register(
            "CameraMode", typeof(CameraMode), typeof(HelixViewport3D), new UIPropertyMetadata(CameraMode.Inspect));

        /// <summary>
        ///   The camera rotation mode property.
        /// </summary>
        public static readonly DependencyProperty CameraRotationModeProperty =
            DependencyProperty.Register(
                "CameraRotationMode",
                typeof(CameraRotationMode),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(CameraRotationMode.Turntable, CameraRotationModeChanged));

        /// <summary>
        ///   The change fov cursor property.
        /// </summary>
        public static readonly DependencyProperty ChangeFieldOfViewCursorProperty =
            DependencyProperty.Register(
                "ChangeFieldOfViewCursor",
                typeof(Cursor),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(Cursors.ScrollNS));

        /// <summary>
        ///   The change field of view gesture property.
        /// </summary>
        public static readonly DependencyProperty ChangeFieldOfViewGestureProperty =
            DependencyProperty.Register(
                "ChangeFieldOfViewGesture",
                typeof(MouseGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new MouseGesture(MouseAction.RightClick, ModifierKeys.Alt)));

        /// <summary>
        ///   The change field of view gesture property.
        /// </summary>
        public static readonly DependencyProperty ChangeLookAtGestureProperty =
            DependencyProperty.Register(
                "ChangeLookAtGesture",
                typeof(MouseGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new MouseGesture(MouseAction.RightDoubleClick)));

        /// <summary>
        ///   The current position property.
        /// </summary>
        public static readonly DependencyProperty CurrentPositionProperty =
            DependencyProperty.Register(
                "CurrentPosition",
                typeof(Point3D),
                typeof(HelixViewport3D),
                new FrameworkPropertyMetadata(
                    new Point3D(0, 0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        ///   The debug info property.
        /// </summary>
        public static readonly DependencyProperty DebugInfoProperty = DependencyProperty.Register(
            "DebugInfo", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        ///   The enable head light property.
        /// </summary>
        public static readonly DependencyProperty EnableHeadLightProperty =
            DependencyProperty.Register(
                "IsHeadLightEnabled",
                typeof(bool),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(false, HeadlightChanged));

        /// <summary>
        ///   The field of view text property.
        /// </summary>
        public static readonly DependencyProperty FieldOfViewTextProperty =
            DependencyProperty.Register(
                "FieldOfViewText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        ///   The frame rate property.
        /// </summary>
        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register(
            "FrameRate", typeof(int), typeof(HelixViewport3D));

        /// <summary>
        ///   The frame rate text property.
        /// </summary>
        public static readonly DependencyProperty FrameRateTextProperty = DependencyProperty.Register(
            "FrameRateText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        ///   The front view gesture property.
        /// </summary>
        public static readonly DependencyProperty FrontViewGestureProperty =
            DependencyProperty.Register(
                "FrontViewGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new KeyGesture(Key.F, ModifierKeys.Control)));

        /// <summary>
        ///   The infinite spin property.
        /// </summary>
        public static readonly DependencyProperty InfiniteSpinProperty = DependencyProperty.Register(
            "InfiniteSpin", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false));

        /// <summary>
        ///   The info background property.
        /// </summary>
        public static readonly DependencyProperty InfoBackgroundProperty = DependencyProperty.Register(
            "InfoBackground",
            typeof(Brush),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(0x80, 0xff, 0xff, 0xff))));

        /// <summary>
        ///   The info foreground property.
        /// </summary>
        public static readonly DependencyProperty InfoForegroundProperty = DependencyProperty.Register(
            "InfoForeground", typeof(Brush), typeof(HelixViewport3D), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        ///   The is change fov enabled property.
        /// </summary>
        public static readonly DependencyProperty IsChangeFieldOfViewEnabledProperty =
            DependencyProperty.Register(
                "IsChangeFieldOfViewEnabled", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        ///   The is pan enabled property.
        /// </summary>
        public static readonly DependencyProperty IsPanEnabledProperty = DependencyProperty.Register(
            "IsPanEnabled", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        ///   The is rotation enabled property.
        /// </summary>
        public static readonly DependencyProperty IsRotationEnabledProperty =
            DependencyProperty.Register(
                "IsRotationEnabled", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        ///   The is zoom enabled property.
        /// </summary>
        public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register(
            "IsZoomEnabled", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        ///   The left view gesture property.
        /// </summary>
        public static readonly DependencyProperty LeftViewGestureProperty =
            DependencyProperty.Register(
                "LeftViewGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new KeyGesture(Key.L, ModifierKeys.Control)));

        /// <summary>
        ///   The maximum field of view property.
        /// </summary>
        public static readonly DependencyProperty MaximumFieldOfViewProperty =
            DependencyProperty.Register(
                "MaximumFieldOfView", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(140.0));

        /// <summary>
        ///   The minimum field of view property.
        /// </summary>
        public static readonly DependencyProperty MinimumFieldOfViewProperty =
            DependencyProperty.Register(
                "MinimumFieldOfView", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(5.0));

        /// <summary>
        ///   The model up direction property.
        /// </summary>
        public static readonly DependencyProperty ModelUpDirectionProperty =
            DependencyProperty.Register(
                "ModelUpDirection",
                typeof(Vector3D),
                typeof(HelixViewport3D),
                new FrameworkPropertyMetadata(
                    new Vector3D(0, 0, 1), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        ///   The orthographic property.
        /// </summary>
        public static readonly DependencyProperty OrthographicProperty = DependencyProperty.Register(
            "Orthographic", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false, OrthographicChanged));

        /// <summary>
        ///   The orthographic toggle gesture property.
        /// </summary>
        public static readonly DependencyProperty OrthographicToggleGestureProperty =
            DependencyProperty.Register(
                "OrthographicToggleGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift)));

        /// <summary>
        ///   The pan cursor property.
        /// </summary>
        public static readonly DependencyProperty PanCursorProperty = DependencyProperty.Register(
            "PanCursor", typeof(Cursor), typeof(HelixViewport3D), new UIPropertyMetadata(Cursors.Hand));

        /// <summary>
        ///   The pan gesture property.
        /// </summary>
        public static readonly DependencyProperty PanGestureProperty = DependencyProperty.Register(
            "PanGesture",
            typeof(MouseGesture),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(new MouseGesture(MouseAction.RightClick, ModifierKeys.Shift)));

        /// <summary>
        ///   The reset camera gesture property.
        /// </summary>
        public static readonly DependencyProperty ResetCameraGestureProperty =
            DependencyProperty.Register(
                "ResetCameraGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new MouseGesture(MouseAction.MiddleDoubleClick)));

        /// <summary>
        ///   The reset camera key gesture property.
        /// </summary>
        public static readonly DependencyProperty ResetCameraKeyGestureProperty =
            DependencyProperty.Register(
                "ResetCameraKeyGesture",
                typeof(KeyGesture),
                typeof(HelixViewport3D),
                new FrameworkPropertyMetadata(
                    new KeyGesture(Key.Home), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        ///   The right view gesture property.
        /// </summary>
        public static readonly DependencyProperty RightViewGestureProperty =
            DependencyProperty.Register(
                "RightViewGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new KeyGesture(Key.R, ModifierKeys.Control)));

        /// <summary>
        ///   The fixed mouse down point property.
        /// </summary>
        public static readonly DependencyProperty RotateAroundMouseDownPointProperty =
            DependencyProperty.Register(
                "RotateAroundMouseDownPoint", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false));

        /// <summary>
        ///   The rotate cursor property.
        /// </summary>
        public static readonly DependencyProperty RotateCursorProperty = DependencyProperty.Register(
            "RotateCursor", typeof(Cursor), typeof(HelixViewport3D), new UIPropertyMetadata(Cursors.SizeAll));

        /// <summary>
        ///   The rotate gesture property.
        /// </summary>
        public static readonly DependencyProperty RotateGestureProperty = DependencyProperty.Register(
            "RotateGesture",
            typeof(MouseGesture),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(new MouseGesture(MouseAction.RightClick)));

        /// <summary>
        ///   The rotation sensitivity property.
        /// </summary>
        public static readonly DependencyProperty RotationSensitivityProperty =
            DependencyProperty.Register(
                "RotationSensitivity", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(1.0));

        /// <summary>
        ///   The show camera info property.
        /// </summary>
        public static readonly DependencyProperty ShowCameraInfoProperty = DependencyProperty.Register(
            "ShowCameraInfo",
            typeof(bool),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(false, ShowCameraInfoChanged));

        /// <summary>
        ///   The show camera target property.
        /// </summary>
        public static readonly DependencyProperty ShowCameraTargetProperty =
            DependencyProperty.Register(
                "ShowCameraTarget", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        ///   The show coordinate system property.
        /// </summary>
        public static readonly DependencyProperty ShowCoordinateSystemProperty =
            DependencyProperty.Register(
                "ShowCoordinateSystem", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false));

        /// <summary>
        ///   The show field of view property.
        /// </summary>
        public static readonly DependencyProperty ShowFieldOfViewProperty =
            DependencyProperty.Register(
                "ShowFieldOfView",
                typeof(bool),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(false, ShowFieldOfViewChanged));

        /// <summary>
        ///   The show frame rate property.
        /// </summary>
        public static readonly DependencyProperty ShowFrameRateProperty = DependencyProperty.Register(
            "ShowFrameRate",
            typeof(bool),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(false, (d, e) => ((HelixViewport3D)d).OnShowFrameRateChanged()));

        /// <summary>
        ///   The show triangle count info property.
        /// </summary>
        public static readonly DependencyProperty ShowTriangleCountInfoProperty =
            DependencyProperty.Register(
                "ShowTriangleCountInfo",
                typeof(bool),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(false, (d, e) => ((HelixViewport3D)d).OnShowTriangleCountInfoChanged()));

        /// <summary>
        ///   The show view cube property.
        /// </summary>
        public static readonly DependencyProperty ShowViewCubeProperty = DependencyProperty.Register(
            "ShowViewCube", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(true));

        /// <summary>
        ///   The status property.
        /// </summary>
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        ///   The sub title property.
        /// </summary>
        public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register(
            "SubTitle", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        ///   The sub title size property.
        /// </summary>
        public static readonly DependencyProperty SubTitleSizeProperty = DependencyProperty.Register(
            "SubTitleSize", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(12.0));

        /// <summary>
        ///   The text brush property.
        /// </summary>
        public static readonly DependencyProperty TextBrushProperty = DependencyProperty.Register(
            "TextBrush", typeof(Brush), typeof(HelixViewport3D), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        ///   The title background property.
        /// </summary>
        public static readonly DependencyProperty TitleBackgroundProperty =
            DependencyProperty.Register(
                "TitleBackground", typeof(Brush), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        ///   The title font family property.
        /// </summary>
        public static readonly DependencyProperty TitleFontFamilyProperty =
            DependencyProperty.Register(
                "TitleFontFamily", typeof(FontFamily), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        ///   The title property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        ///   The title size property.
        /// </summary>
        public static readonly DependencyProperty TitleSizeProperty = DependencyProperty.Register(
            "TitleSize", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(12.0));

        /// <summary>
        ///   The top view gesture property.
        /// </summary>
        public static readonly DependencyProperty TopViewGestureProperty = DependencyProperty.Register(
            "TopViewGesture",
            typeof(InputGesture),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(new KeyGesture(Key.U, ModifierKeys.Control)));

        /// <summary>
        ///   The triangle count info property.
        /// </summary>
        public static readonly DependencyProperty TriangleCountInfoProperty =
            DependencyProperty.Register(
                "TriangleCountInfo", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata(null));

        /// <summary>
        ///   The view cube back text property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeBackTextProperty =
            DependencyProperty.Register(
                "ViewCubeBackText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata("B"));

        /// <summary>
        ///   The view cube bottom text property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeBottomTextProperty =
            DependencyProperty.Register(
                "ViewCubeBottomText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata("D"));

        /// <summary>
        ///   The view cube front text property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeFrontTextProperty =
            DependencyProperty.Register(
                "ViewCubeFrontText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata("F"));

        /// <summary>
        ///   The view cube left text property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeLeftTextProperty =
            DependencyProperty.Register(
                "ViewCubeLeftText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata("L"));

        /// <summary>
        ///   The view cube opacity property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeOpacityProperty =
            DependencyProperty.Register(
                "ViewCubeOpacity", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(0.5));

        /// <summary>
        ///   The view cube right text property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeRightTextProperty =
            DependencyProperty.Register(
                "ViewCubeRightText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata("R"));

        /// <summary>
        ///   The view cube top text property.
        /// </summary>
        public static readonly DependencyProperty ViewCubeTopTextProperty =
            DependencyProperty.Register(
                "ViewCubeTopText", typeof(string), typeof(HelixViewport3D), new UIPropertyMetadata("U"));

        /// <summary>
        ///   Zoom around mouse down point property.
        /// </summary>
        public static readonly DependencyProperty ZoomAroundMouseDownPointProperty =
            DependencyProperty.Register(
                "ZoomAroundMouseDownPoint", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false));

        /// <summary>
        ///   The zoom cursor property.
        /// </summary>
        public static readonly DependencyProperty ZoomCursorProperty = DependencyProperty.Register(
            "ZoomCursor", typeof(Cursor), typeof(HelixViewport3D), new UIPropertyMetadata(Cursors.SizeNS));

        /// <summary>
        ///   The Zoom extents gesture property.
        /// </summary>
        public static readonly DependencyProperty ZoomExtentsGestureProperty =
            DependencyProperty.Register(
                "ZoomExtentsGesture",
                typeof(InputGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(new KeyGesture(Key.E, ModifierKeys.Control | ModifierKeys.Shift)));

        /// <summary>
        ///   The zoom extents when loaded property.
        /// </summary>
        public static readonly DependencyProperty ZoomExtentsWhenLoadedProperty =
            DependencyProperty.Register(
                "ZoomExtentsWhenLoaded", typeof(bool), typeof(HelixViewport3D), new UIPropertyMetadata(false));

        /// <summary>
        ///   The zoom gesture property.
        /// </summary>
        public static readonly DependencyProperty ZoomGestureProperty = DependencyProperty.Register(
            "ZoomGesture",
            typeof(MouseGesture),
            typeof(HelixViewport3D),
            new UIPropertyMetadata(new MouseGesture(MouseAction.RightClick, ModifierKeys.Control)));

        /// <summary>
        ///   The zoom rectangle cursor property.
        /// </summary>
        public static readonly DependencyProperty ZoomRectangleCursorProperty =
            DependencyProperty.Register(
                "ZoomRectangleCursor", typeof(Cursor), typeof(HelixViewport3D), new UIPropertyMetadata(Cursors.ScrollSE));

        /// <summary>
        ///   The zoom rectangle gesture property.
        /// </summary>
        public static readonly DependencyProperty ZoomRectangleGestureProperty =
            DependencyProperty.Register(
                "ZoomRectangleGesture",
                typeof(MouseGesture),
                typeof(HelixViewport3D),
                new UIPropertyMetadata(
                    new MouseGesture(MouseAction.RightClick, ModifierKeys.Control | ModifierKeys.Shift)));

        /// <summary>
        ///   The zoom sensitivity property.
        /// </summary>
        public static readonly DependencyProperty ZoomSensitivityProperty =
            DependencyProperty.Register(
                "ZoomSensitivity", typeof(double), typeof(HelixViewport3D), new UIPropertyMetadata(1.0));

        /// <summary>
        ///   The adorner layer name.
        /// </summary>
        private const string PartAdornerLayer = "PART_AdornerLayer";

        /// <summary>
        ///   The camera controller name.
        /// </summary>
        private const string PartCameraController = "PART_CameraController";

        /// <summary>
        ///   The coordinate view name.
        /// </summary>
        private const string PartCoordinateView = "PART_CoordinateView";

        /// <summary>
        ///   The view cube name.
        /// </summary>
        private const string PartViewCube = "PART_ViewCube";

        /// <summary>
        ///   The view cube viewport name.
        /// </summary>
        private const string PartViewCubeViewport = "PART_ViewCubeViewport";

        /// <summary>
        ///   The framerate stopwatch.
        /// </summary>
        private readonly Stopwatch fpsWatch = new Stopwatch();

        /// <summary>
        ///   The head light.
        /// </summary>
        private readonly DirectionalLight headLight = new DirectionalLight { Color = Colors.White };

        /// <summary>
        ///   The rendering event listener.
        /// </summary>
        private readonly RenderingEventListener renderingEventListener;

        /// <summary>
        ///   The lights.
        /// </summary>
        private readonly Model3DGroup lights;

        /// <summary>
        ///   The orthographic camera.
        /// </summary>
        private readonly OrthographicCamera orthographicCamera;

        /// <summary>
        ///   The perspective camera.
        /// </summary>
        private readonly PerspectiveCamera perspectiveCamera;

        /// <summary>
        ///   The viewport.
        /// </summary>
        private readonly Viewport3D viewport;

        /// <summary>
        ///   The is subscribed to rendering event.
        /// </summary>
        private bool isSubscribedToRenderingEvent;

        /// <summary>
        ///   The "control has been loaded before" flag.
        /// </summary>
        private bool hasBeenLoadedBefore;

        /// <summary>
        ///   The adorner layer.
        /// </summary>
        private AdornerDecorator adornerLayer;

        /// <summary>
        ///   The camera controller.
        /// </summary>
        private CameraController cameraController;

        /// <summary>
        ///   The coordinate system lights.
        /// </summary>
        private Model3DGroup coordinateSystemLights;

        /// <summary>
        ///   The coordinate view.
        /// </summary>
        private Viewport3D coordinateView;

        /// <summary>
        ///   The current camera.
        /// </summary>
        private Camera currentCamera;

        /// <summary>
        ///   The frame counter.
        /// </summary>
        private int frameCounter;

        /// <summary>
        ///   The frame counter for info field updates.
        /// </summary>
        private int infoFrameCounter;

        /// <summary>
        ///   The view cube.
        /// </summary>
        private ViewCubeVisual3D viewCube;

        /// <summary>
        ///   The view cube lights.
        /// </summary>
        private Model3DGroup viewCubeLights;

        /// <summary>
        ///   The view cube view.
        /// </summary>
        private Viewport3D viewCubeViewport;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref="HelixViewport3D" /> class.
        /// </summary>
        static HelixViewport3D()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(HelixViewport3D), new FrameworkPropertyMetadata(typeof(HelixViewport3D)));
            ClipToBoundsProperty.OverrideMetadata(typeof(HelixViewport3D), new FrameworkPropertyMetadata(true));
            OrthographicToggleCommand = new RoutedCommand();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="HelixViewport3D" /> class.
        /// </summary>
        public HelixViewport3D()
        {
            // The Viewport3D must be created here since the Children collection is attached directly
            this.viewport = new Viewport3D();

            // viewport.SetBinding(UIElement.IsHitTestVisibleProperty, new Binding("IsViewportHitTestVisible") { Source = this });
            // viewport.SetBinding(UIElement.ClipToBoundsProperty, new Binding("ClipToBounds") { Source = this });

            // headlight
            this.lights = new Model3DGroup();
            this.viewport.Children.Add(new ModelVisual3D { Content = this.lights });

            this.perspectiveCamera = new PerspectiveCamera();
            this.orthographicCamera = new OrthographicCamera();
            CameraHelper.Reset(this.perspectiveCamera);
            CameraHelper.Reset(this.orthographicCamera);

            this.Camera = this.Orthographic ? (ProjectionCamera)this.orthographicCamera : this.perspectiveCamera;

            // http://blogs.msdn.com/wpfsdk/archive/2007/01/15/maximizing-wpf-3d-performance-on-tier-2-hardware.aspx
            // RenderOptions.EdgeMode?

            // start a watch for FPS calculations
            this.fpsWatch.Start();

            this.Loaded += this.OnControlLoaded;
            this.Unloaded += this.OnControlUnloaded;

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, this.CopyHandler));
            this.CommandBindings.Add(new CommandBinding(OrthographicToggleCommand, this.OrthographicToggle));
            this.renderingEventListener = new RenderingEventListener(this.CompositionTargetRendering);
        }

#if DEBUG
        /// <summary>
        /// Finalizes an instance of the <see cref="HelixViewport3D"/> class.
        /// </summary>
        ~HelixViewport3D()
        {
            Debug.WriteLine("HelixViewport3D finalized.");
        }
#endif

        #endregion

        #region Public Events

        /// <summary>
        ///   Event when a property has been changed
        /// </summary>
        public event RoutedEventHandler CameraChanged
        {
            add
            {
                this.AddHandler(CameraChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(CameraChangedEvent, value);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the command that toggles between orthographic and perspective camera.
        /// </summary>
        public static RoutedCommand OrthographicToggleCommand { get; private set; }

        /// <summary>
        ///   Gets or sets the back view gesture.
        /// </summary>
        /// <value> The back view gesture. </value>
        public InputGesture BackViewGesture
        {
            get
            {
                return (InputGesture)this.GetValue(BackViewGestureProperty);
            }

            set
            {
                this.SetValue(BackViewGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the bottom view gesture.
        /// </summary>
        /// <value> The bottom view gesture. </value>
        public InputGesture BottomViewGesture
        {
            get
            {
                return (InputGesture)this.GetValue(BottomViewGestureProperty);
            }

            set
            {
                this.SetValue(BottomViewGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the camera.
        /// </summary>
        /// <value> The camera. </value>
        public ProjectionCamera Camera
        {
            get
            {
                return this.Viewport.Camera as ProjectionCamera;
            }

            set
            {
                if (this.currentCamera != null)
                {
                    this.currentCamera.Changed -= this.CameraPropertyChanged;
                }

                this.Viewport.Camera = value;

                this.currentCamera = this.Viewport.Camera;
                this.currentCamera.Changed += this.CameraPropertyChanged;
            }
        }

        /// <summary>
        ///   Gets the camera controller
        /// </summary>
        public CameraController CameraController
        {
            get
            {
                return this.cameraController;
            }
        }

        /// <summary>
        ///   Gets or sets the camera inertia factor.
        /// </summary>
        /// <value> The camera inertia factor. </value>
        public double CameraInertiaFactor
        {
            get
            {
                return (double)this.GetValue(CameraInertiaFactorProperty);
            }

            set
            {
                this.SetValue(CameraInertiaFactorProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the camera info.
        /// </summary>
        /// <value> The camera info. </value>
        public string CameraInfo
        {
            get
            {
                return (string)this.GetValue(CameraInfoProperty);
            }

            set
            {
                this.SetValue(CameraInfoProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the <see cref="CameraMode" />
        /// </summary>
        public CameraMode CameraMode
        {
            get
            {
                return (CameraMode)this.GetValue(CameraModeProperty);
            }

            set
            {
                this.SetValue(CameraModeProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the camera rotation mode.
        /// </summary>
        public CameraRotationMode CameraRotationMode
        {
            get
            {
                return (CameraRotationMode)this.GetValue(CameraRotationModeProperty);
            }

            set
            {
                this.SetValue(CameraRotationModeProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the change fov cursor.
        /// </summary>
        /// <value> The change fov cursor. </value>
        public Cursor ChangeFieldOfViewCursor
        {
            get
            {
                return (Cursor)this.GetValue(ChangeFieldOfViewCursorProperty);
            }

            set
            {
                this.SetValue(ChangeFieldOfViewCursorProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the default camera.
        /// </summary>
        /// <value> The default camera. </value>
        public ProjectionCamera DefaultCamera
        {
            get
            {
                return (ProjectionCamera)this.GetValue(DefaultCameraProperty);
            }

            set
            {
                this.SetValue(DefaultCameraProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the change field of view gesture.
        /// </summary>
        /// <value> The change field of view gesture. </value>
        public MouseGesture ChangeFieldOfViewGesture
        {
            get
            {
                return (MouseGesture)this.GetValue(ChangeFieldOfViewGestureProperty);
            }

            set
            {
                this.SetValue(ChangeFieldOfViewGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the change lookat view gesture.
        /// </summary>
        /// <value> The change lookat gesture. </value>
        public MouseGesture ChangeLookAtGesture
        {
            get
            {
                return (MouseGesture)this.GetValue(ChangeLookAtGestureProperty);
            }

            set
            {
                this.SetValue(ChangeLookAtGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets the children.
        /// </summary>
        /// <value> The children. </value>
        public Visual3DCollection Children
        {
            get
            {
                return this.viewport.Children;
            }
        }

        /// <summary>
        ///   Gets or sets the current position.
        /// </summary>
        /// <value> The current position. </value>
        public Point3D CurrentPosition
        {
            get
            {
                return (Point3D)this.GetValue(CurrentPositionProperty);
            }

            set
            {
                this.SetValue(CurrentPositionProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the debug info text.
        /// </summary>
        /// <value> The debug info text. </value>
        public string DebugInfo
        {
            get
            {
                return (string)this.GetValue(DebugInfoProperty);
            }

            set
            {
                this.SetValue(DebugInfoProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the field of view text.
        /// </summary>
        /// <value> The field of view text. </value>
        public string FieldOfViewText
        {
            get
            {
                return (string)this.GetValue(FieldOfViewTextProperty);
            }

            set
            {
                this.SetValue(FieldOfViewTextProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the frame rate.
        /// </summary>
        /// <value> The frame rate. </value>
        public int FrameRate
        {
            get
            {
                return (int)this.GetValue(FrameRateProperty);
            }

            set
            {
                this.SetValue(FrameRateProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the frame rate text.
        /// </summary>
        /// <value> The frame rate text. </value>
        public string FrameRateText
        {
            get
            {
                return (string)this.GetValue(FrameRateTextProperty);
            }

            set
            {
                this.SetValue(FrameRateTextProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the front view gesture.
        /// </summary>
        /// <value> The front view gesture. </value>
        public InputGesture FrontViewGesture
        {
            get
            {
                return (InputGesture)this.GetValue(FrontViewGestureProperty);
            }

            set
            {
                this.SetValue(FrontViewGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether [infinite spin].
        /// </summary>
        /// <value> <c>true</c> if [infinite spin]; otherwise, <c>false</c> . </value>
        public bool InfiniteSpin
        {
            get
            {
                return (bool)this.GetValue(InfiniteSpinProperty);
            }

            set
            {
                this.SetValue(InfiniteSpinProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the background brush for the CameraInfo and TriangleCount fields.
        /// </summary>
        /// <value> The info background. </value>
        public Brush InfoBackground
        {
            get
            {
                return (Brush)this.GetValue(InfoBackgroundProperty);
            }

            set
            {
                this.SetValue(InfoBackgroundProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the foreground brush for informational text.
        /// </summary>
        /// <value> The foreground brush. </value>
        public Brush InfoForeground
        {
            get
            {
                return (Brush)this.GetValue(InfoForegroundProperty);
            }

            set
            {
                this.SetValue(InfoForegroundProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether change of fov is enabled.
        /// </summary>
        public bool IsChangeFieldOfViewEnabled
        {
            get
            {
                return (bool)this.GetValue(IsChangeFieldOfViewEnabledProperty);
            }

            set
            {
                this.SetValue(IsChangeFieldOfViewEnabledProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this instance is head light enabled.
        /// </summary>
        /// <value> <c>true</c> if this instance is head light enabled; otherwise, <c>false</c> . </value>
        public bool IsHeadLightEnabled
        {
            get
            {
                return (bool)this.GetValue(EnableHeadLightProperty);
            }

            set
            {
                this.SetValue(EnableHeadLightProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether pan is enabled.
        /// </summary>
        public bool IsPanEnabled
        {
            get
            {
                return (bool)this.GetValue(IsPanEnabledProperty);
            }

            set
            {
                this.SetValue(IsPanEnabledProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether rotation is enabled.
        /// </summary>
        public bool IsRotationEnabled
        {
            get
            {
                return (bool)this.GetValue(IsRotationEnabledProperty);
            }

            set
            {
                this.SetValue(IsRotationEnabledProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether zoom is enabled.
        /// </summary>
        public bool IsZoomEnabled
        {
            get
            {
                return (bool)this.GetValue(IsZoomEnabledProperty);
            }

            set
            {
                this.SetValue(IsZoomEnabledProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the left view gesture.
        /// </summary>
        /// <value> The left view gesture. </value>
        public InputGesture LeftViewGesture
        {
            get
            {
                return (InputGesture)this.GetValue(LeftViewGestureProperty);
            }

            set
            {
                this.SetValue(LeftViewGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets the lights.
        /// </summary>
        /// <value> The lights. </value>
        public Model3DGroup Lights
        {
            get
            {
                return this.lights;
            }
        }

        /// <summary>
        ///   Gets or sets the maximum field of view.
        /// </summary>
        /// <value> The maximum field of view. </value>
        public double MaximumFieldOfView
        {
            get
            {
                return (double)this.GetValue(MaximumFieldOfViewProperty);
            }

            set
            {
                this.SetValue(MaximumFieldOfViewProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the minimum field of view.
        /// </summary>
        /// <value> The minimum field of view. </value>
        public double MinimumFieldOfView
        {
            get
            {
                return (double)this.GetValue(MinimumFieldOfViewProperty);
            }

            set
            {
                this.SetValue(MinimumFieldOfViewProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the up direction of the model. This is used by the view cube.
        /// </summary>
        /// <value> The model up direction. </value>
        public Vector3D ModelUpDirection
        {
            get
            {
                return (Vector3D)this.GetValue(ModelUpDirectionProperty);
            }

            set
            {
                this.SetValue(ModelUpDirectionProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this <see cref="HelixViewport3D" /> is orthographic.
        /// </summary>
        /// <value> <c>true</c> if orthographic; otherwise, <c>false</c> . </value>
        public bool Orthographic
        {
            get
            {
                return (bool)this.GetValue(OrthographicProperty);
            }

            set
            {
                this.SetValue(OrthographicProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the orthographic toggle gesture.
        /// </summary>
        /// <value> The orthographic toggle gesture. </value>
        public InputGesture OrthographicToggleGesture
        {
            get
            {
                return (InputGesture)this.GetValue(OrthographicToggleGestureProperty);
            }

            set
            {
                this.SetValue(OrthographicToggleGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the pan cursor.
        /// </summary>
        /// <value> The pan cursor. </value>
        public Cursor PanCursor
        {
            get
            {
                return (Cursor)this.GetValue(PanCursorProperty);
            }

            set
            {
                this.SetValue(PanCursorProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the pan gesture.
        /// </summary>
        /// <value> The pan gesture. </value>
        public MouseGesture PanGesture
        {
            get
            {
                return (MouseGesture)this.GetValue(PanGestureProperty);
            }

            set
            {
                this.SetValue(PanGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the ResetCameraGesture.
        /// </summary>
        public InputGesture ResetCameraGesture
        {
            get
            {
                return (InputGesture)this.GetValue(ResetCameraGestureProperty);
            }

            set
            {
                this.SetValue(ResetCameraGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the reset camera key gesture.
        /// </summary>
        /// <value> The reset camera key gesture. </value>
        public KeyGesture ResetCameraKeyGesture
        {
            get
            {
                return (KeyGesture)this.GetValue(ResetCameraKeyGestureProperty);
            }

            set
            {
                this.SetValue(ResetCameraKeyGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the right view gesture.
        /// </summary>
        /// <value> The right view gesture. </value>
        public InputGesture RightViewGesture
        {
            get
            {
                return (InputGesture)this.GetValue(RightViewGestureProperty);
            }

            set
            {
                this.SetValue(RightViewGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to rotate around the mouse down point.
        /// </summary>
        /// <value> <c>true</c> if rotatation around the mouse down point is enabled; otherwise, <c>false</c> . </value>
        public bool RotateAroundMouseDownPoint
        {
            get
            {
                return (bool)this.GetValue(RotateAroundMouseDownPointProperty);
            }

            set
            {
                this.SetValue(RotateAroundMouseDownPointProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the rotate cursor.
        /// </summary>
        /// <value> The rotate cursor. </value>
        public Cursor RotateCursor
        {
            get
            {
                return (Cursor)this.GetValue(RotateCursorProperty);
            }

            set
            {
                this.SetValue(RotateCursorProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the rotate gesture.
        /// </summary>
        /// <value> The rotate gesture. </value>
        public MouseGesture RotateGesture
        {
            get
            {
                return (MouseGesture)this.GetValue(RotateGestureProperty);
            }

            set
            {
                this.SetValue(RotateGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the rotation sensitivity.
        /// </summary>
        /// <value> The rotation sensitivity. </value>
        public double RotationSensitivity
        {
            get
            {
                return (double)this.GetValue(RotationSensitivityProperty);
            }

            set
            {
                this.SetValue(RotationSensitivityProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to show camera info.
        /// </summary>
        /// <value> <c>true</c> if [show camera info]; otherwise, <c>false</c> . </value>
        public bool ShowCameraInfo
        {
            get
            {
                return (bool)this.GetValue(ShowCameraInfoProperty);
            }

            set
            {
                this.SetValue(ShowCameraInfoProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to show the camera target adorner.
        /// </summary>
        /// <value> <c>true</c> if [show camera target]; otherwise, <c>false</c> . </value>
        public bool ShowCameraTarget
        {
            get
            {
                return (bool)this.GetValue(ShowCameraTargetProperty);
            }

            set
            {
                this.SetValue(ShowCameraTargetProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether [show coordinate system].
        /// </summary>
        /// <value> <c>true</c> if [show coordinate system]; otherwise, <c>false</c> . </value>
        public bool ShowCoordinateSystem
        {
            get
            {
                return (bool)this.GetValue(ShowCoordinateSystemProperty);
            }

            set
            {
                this.SetValue(ShowCoordinateSystemProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to show field of view.
        /// </summary>
        /// <value> <c>true</c> if [show field of view]; otherwise, <c>false</c> . </value>
        public bool ShowFieldOfView
        {
            get
            {
                return (bool)this.GetValue(ShowFieldOfViewProperty);
            }

            set
            {
                this.SetValue(ShowFieldOfViewProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to show frame rate.
        /// </summary>
        /// <value> <c>true</c> if [show frame rate]; otherwise, <c>false</c> . </value>
        public bool ShowFrameRate
        {
            get
            {
                return (bool)this.GetValue(ShowFrameRateProperty);
            }

            set
            {
                this.SetValue(ShowFrameRateProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to show the total number of triangles in the scene.
        /// </summary>
        public bool ShowTriangleCountInfo
        {
            get
            {
                return (bool)this.GetValue(ShowTriangleCountInfoProperty);
            }

            set
            {
                this.SetValue(ShowTriangleCountInfoProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether [show view cube].
        /// </summary>
        /// <value> <c>true</c> if [show view cube]; otherwise, <c>false</c> . </value>
        public bool ShowViewCube
        {
            get
            {
                return (bool)this.GetValue(ShowViewCubeProperty);
            }

            set
            {
                this.SetValue(ShowViewCubeProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the status.
        /// </summary>
        /// <value> The status. </value>
        public string Status
        {
            get
            {
                return (string)this.GetValue(StatusProperty);
            }

            set
            {
                this.SetValue(StatusProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the sub title.
        /// </summary>
        /// <value> The sub title. </value>
        public string SubTitle
        {
            get
            {
                return (string)this.GetValue(SubTitleProperty);
            }

            set
            {
                this.SetValue(SubTitleProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the size of the sub title.
        /// </summary>
        /// <value> The size of the sub title. </value>
        public double SubTitleSize
        {
            get
            {
                return (double)this.GetValue(SubTitleSizeProperty);
            }

            set
            {
                this.SetValue(SubTitleSizeProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the text brush.
        /// </summary>
        /// <value> The text brush. </value>
        public Brush TextBrush
        {
            get
            {
                return (Brush)this.GetValue(TextBrushProperty);
            }

            set
            {
                this.SetValue(TextBrushProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the title.
        /// </summary>
        /// <value> The title. </value>
        public string Title
        {
            get
            {
                return (string)this.GetValue(TitleProperty);
            }

            set
            {
                this.SetValue(TitleProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the title background brush.
        /// </summary>
        /// <value> The title background. </value>
        public Brush TitleBackground
        {
            get
            {
                return (Brush)this.GetValue(TitleBackgroundProperty);
            }

            set
            {
                this.SetValue(TitleBackgroundProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the title font family.
        /// </summary>
        /// <value> The title font family. </value>
        public FontFamily TitleFontFamily
        {
            get
            {
                return (FontFamily)this.GetValue(TitleFontFamilyProperty);
            }

            set
            {
                this.SetValue(TitleFontFamilyProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the size of the title.
        /// </summary>
        /// <value> The size of the title. </value>
        public double TitleSize
        {
            get
            {
                return (double)this.GetValue(TitleSizeProperty);
            }

            set
            {
                this.SetValue(TitleSizeProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the top view gesture.
        /// </summary>
        /// <value> The top view gesture. </value>
        public InputGesture TopViewGesture
        {
            get
            {
                return (InputGesture)this.GetValue(TopViewGestureProperty);
            }

            set
            {
                this.SetValue(TopViewGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets TriangleCountInfo.
        /// </summary>
        public string TriangleCountInfo
        {
            get
            {
                return (string)this.GetValue(TriangleCountInfoProperty);
            }

            set
            {
                this.SetValue(TriangleCountInfoProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the view cube back text.
        /// </summary>
        /// <value> The view cube back text. </value>
        public string ViewCubeBackText
        {
            get
            {
                return (string)this.GetValue(ViewCubeBackTextProperty);
            }

            set
            {
                this.SetValue(ViewCubeBackTextProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the view cube bottom text.
        /// </summary>
        /// <value> The view cube bottom text. </value>
        public string ViewCubeBottomText
        {
            get
            {
                return (string)this.GetValue(ViewCubeBottomTextProperty);
            }

            set
            {
                this.SetValue(ViewCubeBottomTextProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the view cube front text.
        /// </summary>
        /// <value> The view cube front text. </value>
        public string ViewCubeFrontText
        {
            get
            {
                return (string)this.GetValue(ViewCubeFrontTextProperty);
            }

            set
            {
                this.SetValue(ViewCubeFrontTextProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the view cube left text.
        /// </summary>
        /// <value> The view cube left text. </value>
        public string ViewCubeLeftText
        {
            get
            {
                return (string)this.GetValue(ViewCubeLeftTextProperty);
            }

            set
            {
                this.SetValue(ViewCubeLeftTextProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the opacity of the ViewCube when inactive.
        /// </summary>
        public double ViewCubeOpacity
        {
            get
            {
                return (double)this.GetValue(ViewCubeOpacityProperty);
            }

            set
            {
                this.SetValue(ViewCubeOpacityProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the view cube right text.
        /// </summary>
        /// <value> The view cube right text. </value>
        public string ViewCubeRightText
        {
            get
            {
                return (string)this.GetValue(ViewCubeRightTextProperty);
            }

            set
            {
                this.SetValue(ViewCubeRightTextProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the view cube top text.
        /// </summary>
        /// <value> The view cube top text. </value>
        public string ViewCubeTopText
        {
            get
            {
                return (string)this.GetValue(ViewCubeTopTextProperty);
            }

            set
            {
                this.SetValue(ViewCubeTopTextProperty, value);
            }
        }

        /// <summary>
        ///   Gets the viewport.
        /// </summary>
        /// <value> The viewport. </value>
        public Viewport3D Viewport
        {
            get
            {
                return this.viewport;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to zoom around mouse down point.
        /// </summary>
        /// <value> <c>true</c> if zooming around the mouse down point is enabled; otherwise, <c>false</c> . </value>
        public bool ZoomAroundMouseDownPoint
        {
            get
            {
                return (bool)this.GetValue(ZoomAroundMouseDownPointProperty);
            }

            set
            {
                this.SetValue(ZoomAroundMouseDownPointProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the zoom cursor.
        /// </summary>
        /// <value> The zoom cursor. </value>
        public Cursor ZoomCursor
        {
            get
            {
                return (Cursor)this.GetValue(ZoomCursorProperty);
            }

            set
            {
                this.SetValue(ZoomCursorProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets ZoomExtentsGesture.
        /// </summary>
        public InputGesture ZoomExtentsGesture
        {
            get
            {
                return (InputGesture)this.GetValue(ZoomExtentsGestureProperty);
            }

            set
            {
                this.SetValue(ZoomExtentsGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to Zoom extents when the control has loaded.
        /// </summary>
        public bool ZoomExtentsWhenLoaded
        {
            get
            {
                return (bool)this.GetValue(ZoomExtentsWhenLoadedProperty);
            }

            set
            {
                this.SetValue(ZoomExtentsWhenLoadedProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the zoom gesture.
        /// </summary>
        /// <value> The zoom gesture. </value>
        public MouseGesture ZoomGesture
        {
            get
            {
                return (MouseGesture)this.GetValue(ZoomGestureProperty);
            }

            set
            {
                this.SetValue(ZoomGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the zoom rectangle cursor.
        /// </summary>
        /// <value> The zoom rectangle cursor. </value>
        public Cursor ZoomRectangleCursor
        {
            get
            {
                return (Cursor)this.GetValue(ZoomRectangleCursorProperty);
            }

            set
            {
                this.SetValue(ZoomRectangleCursorProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the zoom rectangle gesture.
        /// </summary>
        /// <value> The zoom rectangle gesture. </value>
        public MouseGesture ZoomRectangleGesture
        {
            get
            {
                return (MouseGesture)this.GetValue(ZoomRectangleGestureProperty);
            }

            set
            {
                this.SetValue(ZoomRectangleGestureProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the zoom sensitivity.
        /// </summary>
        /// <value> The zoom sensitivity. </value>
        public double ZoomSensitivity
        {
            get
            {
                return (double)this.GetValue(ZoomSensitivityProperty);
            }

            set
            {
                this.SetValue(ZoomSensitivityProperty, value);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Changes the camera direction.
        /// </summary>
        /// <param name="newDirection">
        /// The new direction. 
        /// </param>
        /// <param name="animationTime">
        /// The animation time. 
        /// </param>
        public void ChangeCameraDirection(Vector3D newDirection, double animationTime = 0)
        {
            if (this.cameraController != null)
            {
                this.cameraController.ChangeDirection(newDirection, animationTime);
            }
        }

        /// <summary>
        /// Copies the view to the clipboard.
        /// </summary>
        public void Copy()
        {
            Viewport3DHelper.Copy(
                this.Viewport, this.Viewport.ActualWidth * 2, this.Viewport.ActualHeight * 2, Brushes.White, 2);
        }

        /// <summary>
        /// Copies the view to the clipboard as xaml.
        /// </summary>
        public void CopyXaml()
        {
            Clipboard.SetText(XamlHelper.GetXaml(this.Viewport.Children));
        }

        /// <summary>
        /// Exports the view to the specified file name.
        /// </summary>
        /// <remarks>
        /// Exporters.Filter contains all supported export file types.
        /// </remarks>
        /// <param name="fileName">
        /// Name of the file. 
        /// </param>
        public void Export(string fileName)
        {
            Viewport3DHelper.Export(this.Viewport, fileName, this.Background);
        }

        /// <summary>
        /// Finds the nearest object.
        /// </summary>
        /// <param name="pt">
        /// The pt. 
        /// </param>
        /// <param name="pos">
        /// The pos. 
        /// </param>
        /// <param name="normal">
        /// The normal. 
        /// </param>
        /// <param name="obj">
        /// The obj. 
        /// </param>
        /// <returns>
        /// The find nearest. 
        /// </returns>
        public bool FindNearest(Point pt, out Point3D pos, out Vector3D normal, out DependencyObject obj)
        {
            return Viewport3DHelper.FindNearest(this.Viewport, pt, out pos, out normal, out obj);
        }

        /// <summary>
        /// Finds the nearest point.
        /// </summary>
        /// <param name="pt">
        /// The pt. 
        /// </param>
        /// <returns>
        /// A point. 
        /// </returns>
        public Point3D? FindNearestPoint(Point pt)
        {
            return Viewport3DHelper.FindNearestPoint(this.Viewport, pt);
        }

        /// <summary>
        /// Finds the nearest visual.
        /// </summary>
        /// <param name="pt">
        /// The pt. 
        /// </param>
        /// <returns>
        /// A visual. 
        /// </returns>
        public Visual3D FindNearestVisual(Point pt)
        {
            return Viewport3DHelper.FindNearestVisual(this.Viewport, pt);
        }

        /// <summary>
        /// Change the camera to look at the specified point.
        /// </summary>
        /// <param name="p">
        /// The point. 
        /// </param>
        public void LookAt(Point3D p)
        {
            this.LookAt(p, 0);
        }

        /// <summary>
        /// Change the camera to look at the specified point.
        /// </summary>
        /// <param name="p">
        /// The point. 
        /// </param>
        /// <param name="animationTime">
        /// The animation time. 
        /// </param>
        public void LookAt(Point3D p, double animationTime)
        {
            CameraHelper.LookAt(this.Camera, p, animationTime);
        }

        /// <summary>
        /// Change the camera to look at the specified point.
        /// </summary>
        /// <param name="p">
        /// The point. 
        /// </param>
        /// <param name="distance">
        /// The distance. 
        /// </param>
        /// <param name="animationTime">
        /// The animation time. 
        /// </param>
        public void LookAt(Point3D p, double distance, double animationTime)
        {
            CameraHelper.LookAt(this.Camera, p, distance, animationTime);
        }

        /// <summary>
        /// Change the camera to look at the specified point.
        /// </summary>
        /// <param name="p">
        /// The point. 
        /// </param>
        /// <param name="direction">
        /// The direction. 
        /// </param>
        /// <param name="animationTime">
        /// The animation time. 
        /// </param>
        public void LookAt(Point3D p, Vector3D direction, double animationTime)
        {
            CameraHelper.LookAt(this.Camera, p, direction, animationTime);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/> .
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (this.adornerLayer == null)
            {
                this.adornerLayer = this.Template.FindName(PartAdornerLayer, this) as AdornerDecorator;
                if (this.adornerLayer != null)
                {
                    this.adornerLayer.Child = this.viewport;
                }
            }

            Debug.Assert(
                this.adornerLayer != null, string.Format("{0} is missing from the template.", PartAdornerLayer));

            if (this.cameraController == null)
            {
                this.cameraController = this.Template.FindName(PartCameraController, this) as CameraController;
                if (this.cameraController != null)
                {
                    this.cameraController.Viewport = this.Viewport;
                }
            }

            Debug.Assert(
                this.cameraController != null, string.Format("{0} is missing from the template.", PartCameraController));

            if (this.coordinateView == null)
            {
                this.coordinateView = this.Template.FindName(PartCoordinateView, this) as Viewport3D;

                this.coordinateSystemLights = new Model3DGroup();

                // coordinateSystemLights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(1, 1, 1)));
                // coordinateSystemLights.Children.Add(new AmbientLight(Colors.DarkGray));
                this.coordinateSystemLights.Children.Add(new AmbientLight(Colors.LightGray));

                if (this.coordinateView != null)
                {
                    this.coordinateView.Camera = new PerspectiveCamera();
                    this.coordinateView.Children.Add(new ModelVisual3D { Content = this.coordinateSystemLights });
                }
            }

            Debug.Assert(
                this.coordinateView != null, string.Format("{0} is missing from the template.", PartCoordinateView));

            if (this.viewCubeViewport == null)
            {
                this.viewCubeViewport = this.Template.FindName(PartViewCubeViewport, this) as Viewport3D;

                this.viewCubeLights = new Model3DGroup();
                this.viewCubeLights.Children.Add(new AmbientLight(Colors.White));
                if (this.viewCubeViewport != null)
                {
                    this.viewCubeViewport.Camera = new PerspectiveCamera();
                    this.viewCubeViewport.Children.Add(new ModelVisual3D { Content = this.viewCubeLights });
                    this.viewCubeViewport.MouseEnter += this.ViewCubeViewportMouseEnter;
                    this.viewCubeViewport.MouseLeave += this.ViewCubeViewportMouseLeave;
                }

                this.viewCube = this.Template.FindName(PartViewCube, this) as ViewCubeVisual3D;
                if (this.viewCube != null)
                {
                    this.viewCube.Viewport = this.Viewport;
                }
            }

            Debug.Assert(
                this.coordinateView != null, string.Format("{0} is missing from the template.", PartCoordinateView));

            // update the coordinateview camera
            this.OnCameraChanged();

            // add the default headlight
            this.OnHeadlightChanged();
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Resets the camera.
        /// </summary>
        public void ResetCamera()
        {
            if (this.cameraController != null)
            {
                this.cameraController.ResetCamera();
            }
        }

        /// <summary>
        /// Change the camera position and directions.
        /// </summary>
        /// <param name="newPosition">
        /// The new camera position. 
        /// </param>
        /// <param name="newDirection">
        /// The new camera look direction. 
        /// </param>
        /// <param name="newUpDirection">
        /// The new camera up direction. 
        /// </param>
        /// <param name="animationTime">
        /// The animation time. 
        /// </param>
        public void SetView(Point3D newPosition, Vector3D newDirection, Vector3D newUpDirection, double animationTime)
        {
            CameraHelper.AnimateTo(this.Camera, newPosition, newDirection, newUpDirection, animationTime);
        }

        /// <summary>
        /// Zooms to the extents of the sceen.
        /// </summary>
        /// <param name="animationTime">
        /// The animation time. 
        /// </param>
        public void ZoomExtents(double animationTime = 0)
        {
            if (this.cameraController != null)
            {
                this.cameraController.ZoomExtents(animationTime);
            }
        }

        /// <summary>
        /// Zooms to the extents of the specified bounding box.
        /// </summary>
        /// <param name="bounds">
        /// The bounding box. 
        /// </param>
        /// <param name="animationTime">
        /// The animation time. 
        /// </param>
        public void ZoomExtents(Rect3D bounds, double animationTime = 0)
        {
            CameraHelper.ZoomExtents(this.Camera, this.Viewport, bounds, animationTime);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when the camera is changed.
        /// </summary>
        protected virtual void OnCameraChanged()
        {
            // update the camera of the coordinate system
            if (this.coordinateView != null)
            {
                CameraHelper.CopyDirectionOnly(this.Camera, this.coordinateView.Camera as PerspectiveCamera, 30);
            }

            // update the camera of the view cube
            if (this.viewCubeViewport != null)
            {
                CameraHelper.CopyDirectionOnly(this.Camera, this.viewCubeViewport.Camera as PerspectiveCamera, 20);
            }

            // update the headlight and coordinate system light
            if (this.Camera != null)
            {
                if (this.headLight != null)
                {
                    this.headLight.Direction = this.Camera.LookDirection;
                }

                if (this.coordinateSystemLights != null)
                {
                    var cshl = this.coordinateSystemLights.Children[0] as DirectionalLight;
                    if (cshl != null)
                    {
                        cshl.Direction = this.Camera.LookDirection;
                    }
                }
            }

            if (this.ShowFieldOfView)
            {
                this.UpdateFieldOfViewInfo();
            }

            if (this.ShowCameraInfo)
            {
                this.UpdateCameraInfo();
            }
        }

        /// <summary>
        /// Called when headlight is changed.
        /// </summary>
        protected void OnHeadlightChanged()
        {
            if (this.lights == null)
            {
                return;
            }

            if (this.IsHeadLightEnabled && !this.lights.Children.Contains(this.headLight))
            {
                this.lights.Children.Add(this.headLight);
            }

            if (!this.IsHeadLightEnabled && this.lights.Children.Contains(this.headLight))
            {
                this.lights.Children.Remove(this.headLight);
            }
        }

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> property changes.
        /// </summary>
        /// <param name="e">
        /// Information about the change. 
        /// </param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Visual3D v in e.NewItems)
                    {
                        this.Children.Add(v);
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Visual3D v in e.OldItems)
                    {
                        this.Children.Remove(v);
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.Children.Clear();
                    break;
            }
        }

        /// <summary>
        /// Invoked when an unhandled MouseMove attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data. 
        /// </param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var pt = e.GetPosition(this);
            var pos = this.FindNearestPoint(pt);
            if (pos != null)
            {
                this.CurrentPosition = pos.Value;
            }
            else
            {
                var p = Viewport3DHelper.UnProject(this.Viewport, pt);
                if (p != null)
                {
                    this.CurrentPosition = p.Value;
                }
            }
        }

        /// <summary>
        /// Raises the camera changed event.
        /// </summary>
        protected virtual void RaiseCameraChangedEvent()
        {
            // e.Handled = true;
            var args = new RoutedEventArgs(CameraChangedEvent);
            this.RaiseEvent(args);
        }

        /// <summary>
        /// The animate opacity.
        /// </summary>
        /// <param name="obj">
        /// The obj. 
        /// </param>
        /// <param name="toOpacity">
        /// The to opacity. 
        /// </param>
        /// <param name="animationTime">
        /// The animation time. 
        /// </param>
        private static void AnimateOpacity(UIElement obj, double toOpacity, double animationTime)
        {
            var a = new DoubleAnimation(toOpacity, new Duration(TimeSpan.FromMilliseconds(animationTime)))
                {
                    AccelerationRatio = 0.3,
                    DecelerationRatio = 0.5
                };
            obj.BeginAnimation(OpacityProperty, a);
        }

        /// <summary>
        /// The camera rotation mode changed.
        /// </summary>
        /// <param name="d">
        /// The d. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private static void CameraRotationModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HelixViewport3D)d).OnCameraRotationModeChanged();
        }

        /// <summary>
        /// The headlight changed.
        /// </summary>
        /// <param name="d">
        /// The d. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private static void HeadlightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HelixViewport3D)d).OnHeadlightChanged();
        }

        /// <summary>
        /// The orthographic changed.
        /// </summary>
        /// <param name="d">
        /// The d. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private static void OrthographicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HelixViewport3D)d).OnOrthographicChanged();
        }

        /// <summary>
        /// Called when the ShowCameraInfo is changed.
        /// </summary>
        /// <param name="d">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data. 
        /// </param>
        private static void ShowCameraInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HelixViewport3D)d).UpdateCameraInfo();
        }

        /// <summary>
        /// Called when ShowFieldOfView is changed.
        /// </summary>
        /// <param name="d">
        /// The d. 
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data. 
        /// </param>
        private static void ShowFieldOfViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HelixViewport3D)d).UpdateFieldOfViewInfo();
        }

        /// <summary>
        /// The on show triangle count info changed.
        /// </summary>
        private void OnShowTriangleCountInfoChanged()
        {
            this.UpdateRenderingEventSubscription();
        }

        /// <summary>
        /// The update rendering event subscription.
        /// </summary>
        private void UpdateRenderingEventSubscription()
        {
            if (this.ShowFrameRate || this.ShowTriangleCountInfo)
            {
                this.SubscribeToRenderingEvent();
            }
            else
            {
                this.UnsubscribeRenderingEvent();
            }
        }

        /// <summary>
        /// The subscribe to rendering event.
        /// </summary>
        private void SubscribeToRenderingEvent()
        {
            if (!this.isSubscribedToRenderingEvent)
            {
                RenderingEventManager.AddListener(this.renderingEventListener);
                this.isSubscribedToRenderingEvent = true;
            }
        }

        /// <summary>
        /// The unsubscribe rendering event.
        /// </summary>
        private void UnsubscribeRenderingEvent()
        {
            if (this.isSubscribedToRenderingEvent)
            {
                RenderingEventManager.RemoveListener(this.renderingEventListener);
                this.isSubscribedToRenderingEvent = false;
            }
        }

        /// <summary>
        /// The on show frame rate changed.
        /// </summary>
        private void OnShowFrameRateChanged()
        {
            this.UpdateRenderingEventSubscription();
        }

        /// <summary>
        /// The camera_ changed.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private void CameraPropertyChanged(object sender, EventArgs e)
        {
            // Raise notification
            this.RaiseCameraChangedEvent();

            // Update the CoordinateView camera and the headlight direction
            this.OnCameraChanged();
        }

        /// <summary>
        /// Handles the Rendering event of the CompositionTarget control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event. 
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data. 
        /// </param>
        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            this.frameCounter++;
            if (this.ShowFrameRate && this.fpsWatch.ElapsedMilliseconds > 500)
            {
                this.FrameRate = (int)(this.frameCounter / (0.001 * this.fpsWatch.ElapsedMilliseconds));
                this.FrameRateText = this.FrameRate + " FPS";
                this.frameCounter = 0;
                this.fpsWatch.Reset();
                this.fpsWatch.Start();
            }

            // update the info fields every 100 frames
            // todo: should only update when the visual model of the Viewport3D changes
            this.infoFrameCounter++;
            if (this.ShowTriangleCountInfo && this.infoFrameCounter > 100)
            {
                int count = Viewport3DHelper.GetTotalNumberOfTriangles(this.viewport);
                this.TriangleCountInfo = string.Format("Triangles: {0}", count);
                this.infoFrameCounter = 0;
            }
        }

        /// <summary>
        /// The copy command handler.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private void CopyHandler(object sender, ExecutedRoutedEventArgs e)
        {
            // var vm = Viewport3DHelper.GetViewMatrix(Camera);
            // double ar = ActualWidth / ActualHeight;
            // var pm = Viewport3DHelper.GetProjectionMatrix(Camera, ar);
            // double w = 2 / pm.M11;
            // pm.OffsetX = -1
            // ;
            // pm.M11 *= 2;
            // pm.M22 *= 2;
            // var mc = new MatrixCamera(vm, pm);
            // viewport.Camera = mc;
            this.Copy();
        }

        /// <summary>
        /// Called when the control is loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            if (!this.hasBeenLoadedBefore)
            {
                if (this.DefaultCamera != null)
                {
                    CameraHelper.Copy(this.DefaultCamera, this.perspectiveCamera);
                    CameraHelper.Copy(this.DefaultCamera, this.orthographicCamera);
                }

                this.hasBeenLoadedBefore = true;
            }

            this.UpdateRenderingEventSubscription();
            if (this.ZoomExtentsWhenLoaded)
            {
                this.ZoomExtents();
            }
        }

        /// <summary>
        /// Called when the control is unloaded.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private void OnControlUnloaded(object sender, RoutedEventArgs e)
        {
            this.UnsubscribeRenderingEvent();
        }

        /// <summary>
        /// The on camera rotation mode changed.
        /// </summary>
        private void OnCameraRotationModeChanged()
        {
            if (this.CameraRotationMode != CameraRotationMode.Trackball && this.cameraController != null)
            {
                this.cameraController.ResetCameraUpDirection();
            }
        }

        /// <summary>
        /// Called when the camera type is changed.
        /// </summary>
        private void OnOrthographicChanged()
        {
            var oldCamera = this.Camera;
            if (this.Orthographic)
            {
                this.Camera = this.orthographicCamera;
            }
            else
            {
                this.Camera = this.perspectiveCamera;
            }

            CameraHelper.Copy(oldCamera, this.Camera);
        }

        /// <summary>
        /// The orthographic toggle.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private void OrthographicToggle(object sender, ExecutedRoutedEventArgs e)
        {
            this.Orthographic = !this.Orthographic;
        }

        /// <summary>
        /// Updates the camera info.
        /// </summary>
        private void UpdateCameraInfo()
        {
            this.CameraInfo = CameraHelper.GetInfo(this.Camera);
        }

        /// <summary>
        /// The update field of view info.
        /// </summary>
        private void UpdateFieldOfViewInfo()
        {
            var pc = this.Camera as PerspectiveCamera;
            this.FieldOfViewText = pc != null ? string.Format("FoV ∠ {0:0}°", pc.FieldOfView) : null;
        }

        /// <summary>
        /// Called when the mouse enters the view cube.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private void ViewCubeViewportMouseEnter(object sender, MouseEventArgs e)
        {
            AnimateOpacity(this.viewCubeViewport, 1.0, 200);
        }

        /// <summary>
        /// Called when the mouse leaves the view cube.
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The event arguments. 
        /// </param>
        private void ViewCubeViewportMouseLeave(object sender, MouseEventArgs e)
        {
            AnimateOpacity(this.viewCubeViewport, this.ViewCubeOpacity, 200);
        }

        #endregion
    }
}