using AOT;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// AR rendering modes used with <see cref="ARFoundationBackgroundRenderer"/>.
    /// </summary>
    public enum ARRenderMode
    {
        /// <summary>
        /// The standard background is rendered according to <c>Camera</c> settings (Skybox, Solid Color, etc.).
        /// </summary>
        StandardBackground,

        /// <summary>
        /// The material associated with <see cref="ARFoundationBackgroundRenderer"/> should be rendered as the background.
        /// </summary>
        MaterialAsBackground
    }

    /// <summary>
    /// Uses command buffers to blit an image to a camera's background.
    /// </summary>
    public partial class ARFoundationBackgroundRenderer
    {
        /// <summary>
        /// A function that can be invoked by
        /// [CommandBuffer.IssuePluginEvent](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.IssuePluginEvent.html).
        /// This function does nothing, but Unity requires a valid function pointer in order to add IssuePluginEvent to a command
        /// buffer. Doing so has the side effect of resetting the OpenGL state.
        /// </summary>
        /// <param name="eventId">The id of the event</param>
        /// <seealso cref="AddOpenGLES3ResetStateCommand(CommandBuffer)"/>
        [MonoPInvokeCallback(typeof(Action<int>))]
        static void ResetGlState(int eventId) {}

        /// <summary>
        /// A delegate representation of <see cref="ResetGlState(int)"/>. This maintains a strong
        /// reference to the delegate, which is converted to an IntPtr by <see cref="s_ResetGlStateFuncPtr"/>.
        /// </summary>
        /// <seealso cref="AddOpenGLES3ResetStateCommand(CommandBuffer)"/>
        static Action<int> s_ResetGlStateDelegate = ResetGlState;

        /// <summary>
        /// A pointer to <see cref="ResetGlState(int)"/> that can be passed to
        /// [CommandBuffer.IssuePluginEvent](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.IssuePluginEvent.html).
        /// </summary>
        /// <seealso cref="AddOpenGLES3ResetStateCommand(CommandBuffer)"/>
        static readonly IntPtr s_ResetGlStateFuncPtr = Marshal.GetFunctionPointerForDelegate(s_ResetGlStateDelegate);

        /// <summary>
        /// The <c>Camera</c> to render the background to.
        /// </summary>
        protected Camera m_Camera;

        /// <summary>
        /// The <c>Material</c> to blit to the camera's background.
        /// </summary>
        protected Material m_BackgroundMaterial;

        /// <summary>
        /// A <c>Texture</c> to use with the material. If no texture is set,
        /// the <see cref="m_BackgroundMaterial"/>'s <c>_MainTex</c> texture is used.
        /// </summary>
        protected Texture m_BackgroundTexture;

        /// <summary>
        /// Invoked when one of the properties of this class changes.
        /// </summary>
        public event Action<ARFoundationBackgroundRendererChangedEventArgs> backgroundRendererChanged;

        /// <summary>
        /// Get or set the <c>Material</c> used during background rendering.
        /// </summary>
        public Material backgroundMaterial
        {
            get
            {
                return m_BackgroundMaterial;
            }
            set
            {
                if (m_BackgroundMaterial == value)
                    return;

                RemoveCommandBuffersIfNeeded();
                m_BackgroundMaterial = value;

                if (backgroundRendererChanged != null)
                    backgroundRendererChanged(new ARFoundationBackgroundRendererChangedEventArgs());

                ReapplyCommandBuffersIfNeeded();
            }
        }

        /// <summary>
        /// The texture to use during background rendering. If no texture is set,
        /// the <see cref="backgroundMaterial"/>'s <c>_MainTex</c> texture is used.
        /// </summary>
        public Texture backgroundTexture
        {
            get
            {
                return m_BackgroundTexture;
            }
            set
            {
                if (m_BackgroundTexture = value)
                    return;

                RemoveCommandBuffersIfNeeded();
                m_BackgroundTexture = value;

                if (backgroundRendererChanged != null)
                    backgroundRendererChanged(new ARFoundationBackgroundRendererChangedEventArgs());

                ReapplyCommandBuffersIfNeeded();
            }
        }

        /// <summary>
        /// The <c>Camera</c> to render the background to.
        /// </summary>
        public Camera camera
        {
            get
            {
                // Return main camera when no Camera has been set
                return (m_Camera != null) ? m_Camera : Camera.main;
            }
            set
            {
                if (m_Camera == value)
                    return;

                RemoveCommandBuffersIfNeeded();
                m_Camera = value;

                if (backgroundRendererChanged != null)
                    backgroundRendererChanged(new ARFoundationBackgroundRendererChangedEventArgs());

                ReapplyCommandBuffersIfNeeded();
            }
        }

        /// <summary>
        /// Get or set the <see cref="ARRenderMode"/>.
        /// </summary>
        public ARRenderMode mode
        {
            get
            {
                return m_RenderMode;
            }
            set
            {
                if (value == m_RenderMode)
                    return;

                m_RenderMode = value;

                switch (m_RenderMode)
                {
                    case ARRenderMode.StandardBackground:
                        DisableARBackgroundRendering();
                        break;
                    case ARRenderMode.MaterialAsBackground:
                        EnableARBackgroundRendering();
                        break;
                    default:
                        throw new Exception("Unhandled render mode.");
                }

                if (backgroundRendererChanged != null)
                    backgroundRendererChanged(new ARFoundationBackgroundRendererChangedEventArgs());
            }
        }

        /// <summary>
        /// When using OpenGLES3, this adds a command to the <paramref name="commandBuffer"/>
        /// which will force Unity to reset the OpenGL state. This is necessary on devices using OpenGLES3.
        /// If OpenGLES3 is not the current graphics device type, this method does nothing. This should be
        /// the first command in the command buffer.
        /// </summary>
        /// <param name="commandBuffer">The [CommandBuffer](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.html)
        /// to add the command to.</param>
        static void AddOpenGLES3ResetStateCommand(CommandBuffer commandBuffer)
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3)
            {
                commandBuffer.IssuePluginEvent(s_ResetGlStateFuncPtr, 0);
            }
        }

        /// <summary>
        /// Invoked to indicate background rendering should begin. Use this to setup
        /// the <see cref="camera"/>'s properties and the command buffers used for background rendering.
        /// </summary>
        /// <returns><c>true</c> if background rendering was enabled.</returns>
        protected virtual bool EnableARBackgroundRendering()
        {
            if (m_BackgroundMaterial == null)
                return false;

            Camera camera;

            if (m_Camera != null)
                camera = m_Camera;
            else
                camera = Camera.main;

            if (camera == null)
                return false;

            // Clear flags
            m_CameraClearFlags = camera.clearFlags;
            camera.clearFlags = CameraClearFlags.Depth;

            // Command buffer setup
            m_CommandBuffer = new CommandBuffer();

            var backgroundTexture = m_BackgroundTexture;
            if (backgroundTexture == null)
            {
                const string kMainTexName = "_MainTex";

                // GetTexture will return null if the texture isn't found, but it also
                // writes an error to the console. We check for existence to silence
                // this error.
                if (m_BackgroundMaterial.HasProperty(kMainTexName))
                    backgroundTexture = m_BackgroundMaterial.GetTexture(kMainTexName);
            }

            AddOpenGLES3ResetStateCommand(m_CommandBuffer);

            m_CommandBuffer.Blit(backgroundTexture, BuiltinRenderTextureType.CameraTarget, m_BackgroundMaterial);
            camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_CommandBuffer);
            camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, m_CommandBuffer);

            return true;
        }

        /// <summary>
        /// Invoked to indicate background rendering should stop.
        /// Removes command buffers and returns the <c>Camera</c> to its previous state.
        /// </summary>
        protected virtual void DisableARBackgroundRendering()
        {
            if (null == m_CommandBuffer)
                return;

            var cam = m_Camera ?? Camera.main;
            if (cam == null)
                return;

            cam.clearFlags = m_CameraClearFlags;

            // Command buffer
            cam.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_CommandBuffer);
            cam.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, m_CommandBuffer);
        }

        bool ReapplyCommandBuffersIfNeeded()
        {
            if (m_RenderMode != ARRenderMode.MaterialAsBackground)
                return false;

            EnableARBackgroundRendering();

            return true;
        }

        bool RemoveCommandBuffersIfNeeded()
        {
            if (m_RenderMode != ARRenderMode.MaterialAsBackground)
                return false;

            DisableARBackgroundRendering();

            return true;
        }

        ARRenderMode m_RenderMode = ARRenderMode.StandardBackground;

        CommandBuffer m_CommandBuffer;

        CameraClearFlags m_CameraClearFlags = CameraClearFlags.Skybox;
    }
}
