﻿#nullable disable

using System;
using System.IO;

namespace SkiaSharp
{
	public unsafe class SKPicture : SKObject, ISKReferenceCounted
	{
		internal SKPicture (IntPtr h, bool owns)
			: base (h, owns)
		{
		}

		protected override void Dispose (bool disposing) =>
			base.Dispose (disposing);

		public uint UniqueId =>
			SkiaApi.sk_picture_get_unique_id (Handle);

		public SKRect CullRect {
			get {
				SKRect rect;
				SkiaApi.sk_picture_get_cull_rect (Handle, &rect);
				return rect;
			}
		}

		public int ApproximateBytesUsed =>
			(int)SkiaApi.sk_picture_approximate_bytes_used (Handle);

		public int ApproximateOperationCount =>
			GetApproximateOperationCount (false);

		public int GetApproximateOperationCount(bool includeNested) =>
			SkiaApi.sk_picture_approximate_op_count (Handle, includeNested);

		// Serialize

		public SKData Serialize () =>
			SKData.GetObject (SkiaApi.sk_picture_serialize_to_data (Handle));

		public void Serialize (Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException (nameof (stream));

			using var managed = new SKManagedWStream (stream);
			Serialize (managed);
		}

		public void Serialize (SKWStream stream)
		{
			if (stream == null)
				throw new ArgumentNullException (nameof (stream));

			SkiaApi.sk_picture_serialize_to_stream (Handle, stream.Handle);
		}

		// Playback

		public void Playback (SKCanvas canvas)
		{
			if (canvas is null)
				throw new ArgumentNullException (nameof (canvas));

			SkiaApi.sk_picture_playback (Handle, canvas.Handle);
		}

		// ToShader

		public SKShader ToShader () =>
			ToShader (SKShaderTileMode.Clamp, SKShaderTileMode.Clamp, SKFilterMode.Nearest, null, null);

		public SKShader ToShader (SKShaderTileMode tmx, SKShaderTileMode tmy) =>
			ToShader (tmx, tmy, SKFilterMode.Nearest, null, null);

		public SKShader ToShader (SKShaderTileMode tmx, SKShaderTileMode tmy, SKFilterMode filterMode) =>
			ToShader (tmx, tmy, filterMode, null, null);

		public SKShader ToShader (SKShaderTileMode tmx, SKShaderTileMode tmy, SKRect tile) =>
			ToShader (tmx, tmy, SKFilterMode.Nearest, null, &tile);

		public SKShader ToShader (SKShaderTileMode tmx, SKShaderTileMode tmy, SKFilterMode filterMode, SKRect tile) =>
			ToShader (tmx, tmy, filterMode, null, &tile);

		public SKShader ToShader (SKShaderTileMode tmx, SKShaderTileMode tmy, SKMatrix localMatrix, SKRect tile) =>
			ToShader (tmx, tmy, SKFilterMode.Nearest, &localMatrix, &tile);

		public SKShader ToShader (SKShaderTileMode tmx, SKShaderTileMode tmy, SKFilterMode filterMode, SKMatrix localMatrix, SKRect tile) =>
			ToShader (tmx, tmy, filterMode, &localMatrix, &tile);

		private SKShader ToShader (SKShaderTileMode tmx, SKShaderTileMode tmy, SKFilterMode filterMode, SKMatrix* localMatrix, SKRect* tile) =>
			SKShader.GetObject (SkiaApi.sk_picture_make_shader (Handle, tmx, tmy, filterMode, localMatrix, tile));

		// Deserialize

		public static SKPicture Deserialize (IntPtr data, int length)
		{
			if (data == IntPtr.Zero)
				throw new ArgumentNullException (nameof (data));

			if (length == 0)
				return null;

			return GetObject (SkiaApi.sk_picture_deserialize_from_memory ((void*)data, (IntPtr)length));
		}

		public static SKPicture Deserialize (ReadOnlySpan<byte> data)
		{
			if (data.Length == 0)
				return null;

			fixed (void* ptr = data) {
				return GetObject (SkiaApi.sk_picture_deserialize_from_memory (ptr, (IntPtr)data.Length));
			}
		}

		public static SKPicture Deserialize (SKData data)
		{
			if (data == null)
				throw new ArgumentNullException (nameof (data));

			return GetObject (SkiaApi.sk_picture_deserialize_from_data (data.Handle));
		}

		public static SKPicture Deserialize (Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException (nameof (stream));

			using var managed = new SKManagedStream (stream);
			return Deserialize (managed);
		}

		public static SKPicture Deserialize (SKStream stream)
		{
			if (stream == null)
				throw new ArgumentNullException (nameof (stream));

			return GetObject (SkiaApi.sk_picture_deserialize_from_stream (stream.Handle));
		}

		//

		internal static SKPicture GetObject (IntPtr handle, bool owns = true, bool unrefExisting = true) =>
			GetOrAddObject (handle, owns, unrefExisting, (h, o) => new SKPicture (h, o));
	}
}
