﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuadVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that displays a quadrilateral polygon.
    /// </summary>
    /// <remarks>
    /// See http://en.wikipedia.org/wiki/Quadrilateral
    /// </remarks>
    public class QuadVisual3D : MeshElement3D
    {
        // A quadrilateral defined by the four corner points.
        // Point4          Point3
        // +---------------+
        // |               |
        // |               |
        // +---------------+
        // Point1          Point2
        // The texture coordinates are
        // (0,0)           (1,0)
        // +---------------+
        // |               |
        // |               |
        // +---------------+
        // (0,1)          (1,1)
        #region Constants and Fields

        /// <summary>
        /// The point 1 property.
        /// </summary>
        public static readonly DependencyProperty Point1Property = DependencyProperty.Register(
            "Point1", 
            typeof(Point3D), 
            typeof(QuadVisual3D), 
            new UIPropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

        /// <summary>
        /// The point 2 property.
        /// </summary>
        public static readonly DependencyProperty Point2Property = DependencyProperty.Register(
            "Point2", 
            typeof(Point3D), 
            typeof(QuadVisual3D), 
            new UIPropertyMetadata(new Point3D(1, 0, 0), GeometryChanged));

        /// <summary>
        /// The point 3 property.
        /// </summary>
        public static readonly DependencyProperty Point3Property = DependencyProperty.Register(
            "Point3", 
            typeof(Point3D), 
            typeof(QuadVisual3D), 
            new UIPropertyMetadata(new Point3D(1, 1, 0), GeometryChanged));

        /// <summary>
        /// The point 4 property.
        /// </summary>
        public static readonly DependencyProperty Point4Property = DependencyProperty.Register(
            "Point4", 
            typeof(Point3D), 
            typeof(QuadVisual3D), 
            new UIPropertyMetadata(new Point3D(0, 1, 0), GeometryChanged));

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the first point.
        /// </summary>
        /// <value>The point1.</value>
        public Point3D Point1
        {
            get
            {
                return (Point3D)this.GetValue(Point1Property);
            }

            set
            {
                this.SetValue(Point1Property, value);
            }
        }

        /// <summary>
        ///   Gets or sets the second point.
        /// </summary>
        /// <value>The point2.</value>
        public Point3D Point2
        {
            get
            {
                return (Point3D)this.GetValue(Point2Property);
            }

            set
            {
                this.SetValue(Point2Property, value);
            }
        }

        /// <summary>
        ///   Gets or sets the third point.
        /// </summary>
        /// <value>The point3.</value>
        public Point3D Point3
        {
            get
            {
                return (Point3D)this.GetValue(Point3Property);
            }

            set
            {
                this.SetValue(Point3Property, value);
            }
        }

        /// <summary>
        ///   Gets or sets the fourth point.
        /// </summary>
        /// <value>The point4.</value>
        public Point3D Point4
        {
            get
            {
                return (Point3D)this.GetValue(Point4Property);
            }

            set
            {
                this.SetValue(Point4Property, value);
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
            var builder = new MeshBuilder(false,true);
            builder.AddQuad(
                this.Point1, 
                this.Point2, 
                this.Point3, 
                this.Point4, 
                new Point(0, 1), 
                new Point(1, 1), 
                new Point(1, 0), 
                new Point(0, 0));
            return builder.ToMesh();
        }

        #endregion
    }
}