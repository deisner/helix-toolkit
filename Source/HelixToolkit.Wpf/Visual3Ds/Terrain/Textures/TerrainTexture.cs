﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TerrainTexture.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A terrain texture base class.
    /// </summary>
    public abstract class TerrainTexture
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TerrainTexture" /> class.
        /// </summary>
        public TerrainTexture()
        {
            this.Material = Materials.Green;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the material.
        /// </summary>
        /// <value>The material.</value>
        public Material Material { get; set; }

        /// <summary>
        ///   Gets or sets the texture coordinates.
        /// </summary>
        /// <value>The texture coordinates.</value>
        public PointCollection TextureCoordinates { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculates the texture of the specified model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="mesh">
        /// The mesh.
        /// </param>
        public virtual void Calculate(TerrainModel model, MeshGeometry3D mesh)
        {
        }

        #endregion
    }
}