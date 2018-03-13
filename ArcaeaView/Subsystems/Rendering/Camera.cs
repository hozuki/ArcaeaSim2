using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moe.Mottomo.ArcaeaSim.Core;

namespace Moe.Mottomo.ArcaeaSim.Subsystems.Rendering {
    /// <summary>
    /// A simple FPS-like perspective camera.
    /// </summary>
    public sealed class Camera {

        /// <summary>
        /// Creates a new <see cref="Camera"/> instance.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public Camera([NotNull] GraphicsDevice graphicsDevice) {
            _graphicsDevice = graphicsDevice;

            Update();
        }

        /// <summary>
        /// Gets/sets the position of the camera.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets the view matrix of the camera.
        /// </summary>
        public Matrix ViewMatrix { get; private set; }

        /// <summary>
        /// Gets the projection matrix of the camera.
        /// </summary>
        public Matrix ProjectionMatrix { get; private set; }

        /// <summary>
        /// Gets/sets the target to look at.
        /// </summary>
        public Vector3 LookAtTarget {
            get => _lookAtTarget;
            set {
                _lookAtTarget = value;
                _forward = value - Position;
                _right = -Vector3.Normalize(Vector3.Cross(_up, _forward));
            }
        }

        /// <summary>
        /// Gets/sets the up vector.
        /// </summary>
        public Vector3 Up {
            get => _up;
            set {
                _up = value;
                _forward = _lookAtTarget - Position;
                _right = -Vector3.Normalize(Vector3.Cross(value, _forward));
            }
        }

        /// <summary>
        /// Gets/sets the field of view.
        /// </summary>
        public float FieldOfView {
            get => _fieldOfView;
            set => _fieldOfView = MathF.ClampLower(value, 0.01f);
        }

        /// <summary>
        /// Gets/sets the near clip distance.
        /// </summary>
        public float NearClip {
            get => _nearClip;
            set => _nearClip = MathF.ClampLower(value, 0.0001f);
        }

        /// <summary>
        /// Gets/sets the far clip distance.
        /// </summary>
        public float FarClip {
            get => _farClip;
            set => _farClip = MathF.ClampLower(value, 0.1f);
        }

        /// <summary>
        /// Gets the right vector.
        /// </summary>
        public Vector3 Right {
            get => _right;
            private set => _right = value;
        }

        /// <summary>
        /// Gets the forward vector.
        /// </summary>
        public Vector3 Forward {
            get => _forward;
            private set => _forward = value;
        }

        /// <summary>
        /// Strafe left or right.
        /// </summary>
        /// <param name="rightDistance">Strafe distance. Use a positive value to strafe right, and negative value to strafe left.</param>
        public void Strafe(float rightDistance) {
            _right.Normalize();

            var delta = _right * rightDistance;
            Position += delta;
            _lookAtTarget += delta;
        }

        /// <summary>
        /// Walk forward or backwards.
        /// </summary>
        /// <param name="frontDistance">Walk distance. Use a positive value to walk forward, and negative value to walk backwards.</param>
        public void Walk(float frontDistance) {
            _forward.Normalize();

            var delta = _forward * frontDistance;
            Position += delta;
            _lookAtTarget += delta;
        }

        /// <summary>
        /// Rotate X.
        /// </summary>
        /// <param name="amount">The delta rotation angle, in radians. Use a positive value to rotate down, and a negative value to rotate up.</param>
        public void Pitch(float amount) {
            _pitch += amount;

            _forward.Normalize();

            var left = Vector3.Cross(_up, _forward);
            left.Normalize();

            var length = (_lookAtTarget - Position).Length();

            _right = -left;
            _forward = Vector3.Transform(_forward, Matrix.CreateFromAxisAngle(left, amount));
            _up = Vector3.Transform(_up, Matrix.CreateFromAxisAngle(left, amount));

            _lookAtTarget = Position + _forward * length;
        }

        /// <summary>
        /// Rotate Z.
        /// </summary>
        /// <param name="amount">The delta rotation angle, in radians. Use a positive value to rotate left, and a negative value to rotate right.</param>
        public void Yaw(float amount) {
            _yaw += amount;

            _forward.Normalize();

            var length = (_lookAtTarget - Position).Length();

            _forward = Vector3.Transform(_forward, Matrix.CreateFromAxisAngle(_up, amount));

            _lookAtTarget = Position + _forward * length;
        }

        /// <summary>
        /// Rotate Y.
        /// </summary>
        /// <param name="amount">The delta rotation angle, in radians. Use a positive value to rotate counterclockwise, and a negative value to rotate clockwise.</param>
        public void Roll(float amount) {
            _roll += amount;

            _up.Normalize();
            var left = Vector3.Cross(_up, _forward);
            left.Normalize();

            _up = Vector3.Transform(_up, Matrix.CreateFromAxisAngle(_forward, amount));
        }

        /// <summary>
        /// Zoom in or zoom out.
        /// </summary>
        /// <param name="deltaScale">The delta scale. Affects FOV.</param>
        public void Zoom(float deltaScale) {
            var newFov = MathHelper.Clamp(FieldOfView + deltaScale, 0.1f, MathHelper.Pi);

            FieldOfView = newFov;
        }

        /// <summary>
        /// Update the view matrix and the projection matrix of the camera.
        /// </summary>
        public void Update() {
            var view = Matrix.CreateLookAt(Position, LookAtTarget, Up);

            var aspectRatio = _graphicsDevice.Viewport.AspectRatio;
            var projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, aspectRatio, NearClip, FarClip);

            ViewMatrix = view;
            ProjectionMatrix = projection;
        }

        /// <summary>
        /// Reset camera parameters to initial state (not default state).
        /// </summary>
        public void Reset() {
            Position = Vector3.UnitZ * ViewerHeight;
            LookAtTarget = new Vector3(0, FarClip * 0.2f, 0);
            FieldOfView = MathHelper.ToRadians(60);

            _forward = _lookAtTarget - Position;
            _forward.Normalize();
            _right = Vector3.UnitX;
            _up = Vector3.Cross(_right, _forward);

            _pitch = 0;
            _yaw = 0;
            _roll = 0;
        }

        private const float ViewerHeight = 10.5f;

        private Vector3 _up = Vector3.UnitZ;
        private Vector3 _lookAtTarget;
        private Vector3 _forward = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;

        private float _fieldOfView = MathHelper.PiOver2;

        private float _nearClip = 0.5f;
        private float _farClip = 120f;

        private float _pitch;
        private float _yaw;
        private float _roll;

        private readonly GraphicsDevice _graphicsDevice;

    }
}
