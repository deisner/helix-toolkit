﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TruncatedConeVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows a truncated cone defined by origin, height, normal, base- and top radius.
    /// </summary>
    public class TruncatedConeVisual3D : MeshElement3D
    {
        #region Constants and Fields

        /// <summary>
        /// The base cap property.
        /// </summary>
        public static readonly DependencyProperty BaseCapProperty = DependencyProperty.Register(
            "BaseCap", typeof(bool), typeof(TruncatedConeVisual3D), new UIPropertyMetadata(true, GeometryChanged));

        /// <summary>
        /// The base radius property.
        /// </summary>
        public static readonly DependencyProperty BaseRadiusProperty = DependencyProperty.Register(
            "BaseRadius", typeof(double), typeof(TruncatedConeVisual3D), new PropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// The height property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
            "Height", typeof(double), typeof(TruncatedConeVisual3D), new PropertyMetadata(2.0, GeometryChanged));

        /// <summary>
        /// The normal property.
        /// </summary>
        public static readonly DependencyProperty NormalProperty = DependencyProperty.Register(
            "Normal", 
            typeof(Vector3D), 
            typeof(TruncatedConeVisual3D), 
            new PropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

        /// <summary>
        /// The origin property.
        /// </summary>
        public static readonly DependencyProperty OriginProperty = DependencyProperty.Register(
            "Origin", 
            typeof(Point3D), 
            typeof(TruncatedConeVisual3D), 
            new PropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

        /// <summary>
        /// The theta div property.
        /// </summary>
        public static readonly DependencyProperty ThetaDivProperty = DependencyProperty.Register(
            "ThetaDiv", typeof(int), typeof(TruncatedConeVisual3D), new PropertyMetadata(35, GeometryChanged));

        /// <summary>
        /// The top cap property.
        /// </summary>
        public static readonly DependencyProperty TopCapProperty = DependencyProperty.Register(
            "TopCap", typeof(bool), typeof(TruncatedConeVisual3D), new UIPropertyMetadata(true, GeometryChanged));

        /// <summary>
        /// The top radius property.
        /// </summary>
        public static readonly DependencyProperty TopRadiusProperty = DependencyProperty.Register(
            "TopRadius", typeof(double), typeof(TruncatedConeVisual3D), new PropertyMetadata(0.0, GeometryChanged));

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets a value indicating whether to include a base cap.
        /// </summary>
        public bool BaseCap
        {
            get
            {
                return (bool)this.GetValue(BaseCapProperty);
            }

            set
            {
                this.SetValue(BaseCapProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the base radius.
        /// </summary>
        /// <value>The base radius.</value>
        public double BaseRadius
        {
            get
            {
                return (double)this.GetValue(BaseRadiusProperty);
            }

            set
            {
                this.SetValue(BaseRadiusProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get
            {
                return (double)this.GetValue(HeightProperty);
            }

            set
            {
                this.SetValue(HeightProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the normal.
        /// </summary>
        /// <value>The normal.</value>
        public Vector3D Normal
        {
            get
            {
                return (Vector3D)this.GetValue(NormalProperty);
            }

            set
            {
                this.SetValue(NormalProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the origin.
        /// </summary>
        /// <value>The origin.</value>
        public Point3D Origin
        {
            get
            {
                return (Point3D)this.GetValue(OriginProperty);
            }

            set
            {
                this.SetValue(OriginProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the theta div.
        /// </summary>
        /// <value>The theta div.</value>
        public int ThetaDiv
        {
            get
            {
                return (int)this.GetValue(ThetaDivProperty);
            }

            set
            {
                this.SetValue(ThetaDivProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to include a top cap.
        /// </summary>
        public bool TopCap
        {
            get
            {
                return (bool)this.GetValue(TopCapProperty);
            }

            set
            {
                this.SetValue(TopCapProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the top radius.
        /// </summary>
        /// <value>The top radius.</value>
        public double TopRadius
        {
            get
            {
                return (double)this.GetValue(TopRadiusProperty);
            }

            set
            {
                this.SetValue(TopRadiusProperty, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Do the tesselation and return the <see cref="MeshGeometry3D"/>.
        /// </summary>
        /// <returns>A triangular mesh geometry.</returns>
        protected override MeshGeometry3D Tessellate()
        {
            var builder = new MeshBuilder(false,false);
            builder.AddCone(
                this.Origin, 
                this.Normal, 
                this.BaseRadius, 
                this.TopRadius, 
                this.Height, 
                this.BaseCap, 
                this.TopCap, 
                this.ThetaDiv);
            return builder.ToMesh();
        }

        #endregion
    }
}