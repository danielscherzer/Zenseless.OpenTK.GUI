using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Zenseless.OpenTK.GUI;

/// <summary>
/// Class that handles rendering of user interfaces with ImGui.
/// </summary>
public class ImGuiRenderer : IDisposable
{
	/// <summary>
	/// Create a new instance.
	/// </summary>
	public ImGuiRenderer()
	{
		ImGuiHelper.AssureContextCreated();

		var io = ImGui.GetIO();
		io.DisplaySize = System.Numerics.Vector2.Zero;
		io.Fonts.AddFontDefault();
		io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
		io.DisplayFramebufferScale = System.Numerics.Vector2.One;

		CreateDeviceResources();
	}

	/// <summary>
	/// Se a new TTF font for rendering the GUI
	/// </summary>
	/// <param name="fontData">TTF file read into a byte array</param>
	/// <param name="sizePixels">Intented size in pixels. Bigger means bigger texture is created.</param>
	public void SetFontTTF(byte[] fontData, float sizePixels)
	{
		var fonts = ImGui.GetIO().Fonts;
		fonts.Clear(); // replace existing fonts
		GCHandle pinnedArray = GCHandle.Alloc(fontData, GCHandleType.Pinned);
		IntPtr pointer = pinnedArray.AddrOfPinnedObject();
		fonts.AddFontFromMemoryTTF(pointer, fontData.Length, sizePixels);
		pinnedArray.Free();
		RecreateFontDeviceTexture();
		//ImGui.PushFont(fnt); // call only after ImGui.NewFrame()
	}

	/// <summary>
	/// Render the user interface
	/// </summary>
	/// <param name="windowResolution">Window resolution in pixels.</param>
	public void Render(Vector2i windowResolution)
	{
		ImGui.GetIO().DisplaySize = new System.Numerics.Vector2(windowResolution.X, windowResolution.Y); // divide by DisplayFramebufferScale if necessary
		ImGui.Render();
		RenderImDrawData(ImGui.GetDrawData());
	}

	/// <summary>
	/// Dispose all OpenGL resources
	/// </summary>
	public void Dispose()
	{
		vertexArray.Dispose();
		GL.DeleteBuffer(_vertexBuffer);
		GL.DeleteBuffer(_indexBuffer);

		fontTexture.Dispose();
		GL.DeleteProgram(_shader);
		GC.SuppressFinalize(this);
	}

	Texture2D fontTexture;

	private int _shader;
	private int _shaderFontTextureLocation;
	private int _shaderProjectionMatrixLocation;

	readonly VertexArray vertexArray = new();
	private int _indexBuffer;
	private int _indexBufferSize;
	private int _vertexBuffer;
	private int _vertexBufferSize;

	private static int CompileShader(string name, ShaderType type, string source)
	{
		int shader = GL.CreateShader(type);

		GL.ShaderSource(shader, source);
		GL.CompileShader(shader);

		GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
		if (success == 0)
		{
			string info = GL.GetShaderInfoLog(shader);
			Debug.WriteLine($"GL.CompileShader for shader '{name}' [{type}] had info log:\n{info}");
		}

		return shader;
	}

	private static int CreateProgram(string name, string vertexSource, string fragmentSoruce)
	{
		int program = GL.CreateProgram();

		int vertex = CompileShader(name, ShaderType.VertexShader, vertexSource);
		int fragment = CompileShader(name, ShaderType.FragmentShader, fragmentSoruce);

		GL.AttachShader(program, vertex);
		GL.AttachShader(program, fragment);

		GL.LinkProgram(program);

		GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
		if (success == 0)
		{
			string info = GL.GetProgramInfoLog(program);
			Debug.WriteLine($"GL.LinkProgram had info log [{name}]:\n{info}");
		}

		GL.DetachShader(program, vertex);
		GL.DetachShader(program, fragment);

		GL.DeleteShader(vertex);
		GL.DeleteShader(fragment);

		return program;
	}

	[MemberNotNull(nameof(fontTexture))]
	private void CreateDeviceResources()
	{
		CreateShader();

		RecreateFontDeviceTexture();

		_vertexBufferSize = 10000;
		_indexBufferSize = 2000;

		vertexArray.Bind();

		_vertexBuffer = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
		GL.BufferData(BufferTarget.ArrayBuffer, _vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

		_indexBuffer = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
		GL.BufferData(BufferTarget.ElementArrayBuffer, _indexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

		void SetAttribute(int index, int size, VertexAttribType type, int relativeoffset, bool normalized = false)
		{
			int stride = Unsafe.SizeOf<ImDrawVert>();
			GL.VertexArrayVertexBuffer(vertexArray.Handle, index, _vertexBuffer, new IntPtr(0), stride);
			GL.VertexArrayAttribBinding(vertexArray.Handle, index, index);
			GL.VertexArrayAttribFormat(vertexArray.Handle, index, size, type, normalized, relativeoffset);
			GL.EnableVertexArrayAttrib(vertexArray.Handle, index);
		}
		SetAttribute(0, 2, VertexAttribType.Float, 0);
		SetAttribute(1, 2, VertexAttribType.Float, 8);
		SetAttribute(2, 4, VertexAttribType.UnsignedByte, 16, true);

		GL.BindVertexArray(0);
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
		GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
	}

	private void CreateShader()
	{
		string VertexSource = @"#version 330 core

uniform mat4 projection_matrix;

layout(location = 0) in vec2 in_position;
layout(location = 1) in vec2 in_texCoord;
layout(location = 2) in vec4 in_color;

out vec4 color;
out vec2 texCoord;

void main()
{
    gl_Position = projection_matrix * vec4(in_position, 0, 1);
    color = in_color;
    texCoord = in_texCoord;
}";
		string FragmentSource = @"#version 330 core

uniform sampler2D in_fontTexture;

in vec4 color;
in vec2 texCoord;

out vec4 outputColor;

void main()
{
    outputColor = color * texture(in_fontTexture, texCoord);
}";

		_shader = CreateProgram("ImGui", VertexSource, FragmentSource);
		_shaderProjectionMatrixLocation = GL.GetUniformLocation(_shader, "projection_matrix");
		_shaderFontTextureLocation = GL.GetUniformLocation(_shader, "in_fontTexture");
	}

	[MemberNotNull(nameof(fontTexture))]
	private void RecreateFontDeviceTexture()
	{
		ImGuiIOPtr io = ImGui.GetIO();
		io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out _);

		fontTexture?.Dispose();
		fontTexture = new(width, height, SizedInternalFormat.Rgba8, 1) { Function = TextureFunction.Repeat, MinFilter = TextureMinFilter.Linear, MagFilter = TextureMagFilter.Linear };
		GL.TextureSubImage2D(fontTexture.Handle, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);

		io.Fonts.SetTexID((IntPtr)fontTexture.Handle.Id);
		io.Fonts.ClearTexData();
	}

	private void RenderImDrawData(ImDrawDataPtr draw_data)
	{
		if (0 == draw_data.CmdListsCount) return;

		vertexArray.Bind();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
		GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
		for (int i = 0; i < draw_data.CmdListsCount; i++)
		{
			ImDrawListPtr cmd_list = draw_data.CmdListsRange[i];

			int vertexSize = cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
			if (vertexSize > _vertexBufferSize)
			{
				int newSize = (int)Math.Max(_vertexBufferSize * 1.5f, vertexSize);

				GL.BufferData(BufferTarget.ArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
				_vertexBufferSize = newSize;
				Debug.WriteLine($"Resized dear imgui vertex buffer to new size {_vertexBufferSize}");
			}

			int indexSize = cmd_list.IdxBuffer.Size * sizeof(ushort);
			if (indexSize > _indexBufferSize)
			{
				int newSize = (int)Math.Max(_indexBufferSize * 1.5f, indexSize);
				GL.BufferData(BufferTarget.ElementArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
				_indexBufferSize = newSize;
				Debug.WriteLine($"Resized dear imgui index buffer to new size {_indexBufferSize}");
			}
		}

		// Setup orthographic projection matrix into our constant buffer
		ImGuiIOPtr io = ImGui.GetIO();
		Matrix4 mvp = Matrix4.CreateOrthographicOffCenter(0.0f, io.DisplaySize.X, io.DisplaySize.Y, 0.0f, -1.0f, 1.0f);
		GL.ProgramUniformMatrix4(_shader, _shaderProjectionMatrixLocation, false, ref mvp);
		GL.ProgramUniform1(_shader, _shaderFontTextureLocation, 0);

		draw_data.ScaleClipRects(io.DisplayFramebufferScale);

		GL.UseProgram(_shader);
		GL.Enable(EnableCap.Blend);
		GL.Enable(EnableCap.ScissorTest);
		GL.BlendEquation(BlendEquationMode.FuncAdd);
		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		GL.Disable(EnableCap.CullFace);
		GL.Disable(EnableCap.DepthTest);

		// Render command lists
		for (int n = 0; n < draw_data.CmdListsCount; n++)
		{
			ImDrawListPtr cmd_list = draw_data.CmdListsRange[n];
			GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);
			GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);

			for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
			{
				ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
				if (pcmd.UserCallback != IntPtr.Zero)
				{
					throw new NotImplementedException();
				}
				else
				{
					GL.BindTextureUnit(0, (int)pcmd.TextureId);

					// We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
					var clip = pcmd.ClipRect;
					GL.Scissor((int)clip.X, (int)io.DisplaySize.Y - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

					if ((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
					{
						GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(pcmd.IdxOffset * sizeof(ushort)), (int)pcmd.VtxOffset);
					}
					else
					{
						GL.DrawElements(BeginMode.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (int)pcmd.IdxOffset * sizeof(ushort));
					}
				}
			}
		}
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
		GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		GL.UseProgram(0);
		GL.BindTextureUnit(0, 0);
		GL.BindVertexArray(0);
		GL.Disable(EnableCap.Blend);
		GL.Disable(EnableCap.ScissorTest);
	}
}
