using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
		io.Fonts.AddFontDefault();
		io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
		io.DisplayFramebufferScale = System.Numerics.Vector2.One;

		CreateDeviceResources();
	}

	/// <summary>
	/// Call after each window resize to update window width and height
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	public static void WindowResized(int width, int height)
	{
		ImGuiIOPtr io = ImGui.GetIO();
		io.DisplaySize = new System.Numerics.Vector2(width, height); // divide by DisplayFramebufferScale if necessary
	}

	/// <summary>
	/// Render the user interface
	/// </summary>
	public void Render()
	{
		ImGui.Render();
		RenderImDrawData(ImGui.GetDrawData());
	}

	/// <summary>
	/// Dispose all OpenGL resources
	/// </summary>
	public void Dispose()
	{
		GL.DeleteVertexArray(_vertexArray);
		GL.DeleteBuffer(_vertexBuffer);
		GL.DeleteBuffer(_indexBuffer);

		GL.DeleteTexture(_fontTexture);
		GL.DeleteProgram(_shader);
		GC.SuppressFinalize(this);
	}

	private int _fontTexture;
	private int _shader;
	private int _shaderFontTextureLocation;
	private int _shaderProjectionMatrixLocation;

	private int _indexBuffer;
	private int _indexBufferSize;
	private int _vertexArray;
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

	private void CreateDeviceResources()
	{
		_vertexBufferSize = 10000;
		_indexBufferSize = 2000;

		_vertexArray = GL.GenVertexArray();
		GL.BindVertexArray(_vertexArray);

		_vertexBuffer = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
		GL.BufferData(BufferTarget.ArrayBuffer, _vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

		_indexBuffer = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
		GL.BufferData(BufferTarget.ElementArrayBuffer, _indexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

		RecreateFontDeviceTexture();

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

		int stride = Unsafe.SizeOf<ImDrawVert>();
		GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
		GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 8);
		GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, stride, 16);

		GL.EnableVertexAttribArray(0);
		GL.EnableVertexAttribArray(1);
		GL.EnableVertexAttribArray(2);
	}

	private void RecreateFontDeviceTexture()
	{
		ImGuiIOPtr io = ImGui.GetIO();
		io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out _);

		int mips = (int)Math.Floor(Math.Log(Math.Max(width, height), 2));

		_fontTexture = GL.GenTexture();
		GL.ActiveTexture(TextureUnit.Texture0);
		GL.BindTexture(TextureTarget.Texture2D, _fontTexture);
		GL.TexStorage2D(TextureTarget2d.Texture2D, mips, SizedInternalFormat.Rgba8, width, height);

		GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);

		GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, mips - 1);

		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

		io.Fonts.SetTexID((IntPtr)_fontTexture);

		io.Fonts.ClearTexData();
	}

	private void RenderImDrawData(ImDrawDataPtr draw_data)
	{
		if (0 == draw_data.CmdListsCount) return;

		// Bind the element buffer (thru the VAO) so that we can resize it.
		GL.BindVertexArray(_vertexArray);
		// Bind the vertex buffer so that we can resize it.
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

		GL.UseProgram(_shader);
		GL.UniformMatrix4(_shaderProjectionMatrixLocation, false, ref mvp);
		GL.Uniform1(_shaderFontTextureLocation, 0);

		GL.BindVertexArray(_vertexArray);

		draw_data.ScaleClipRects(io.DisplayFramebufferScale);

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
					GL.ActiveTexture(TextureUnit.Texture0);
					GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);

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
		GL.Disable(EnableCap.Blend);
		GL.Disable(EnableCap.ScissorTest);
	}
}
