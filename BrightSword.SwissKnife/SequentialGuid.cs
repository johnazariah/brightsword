using System;
using System.Diagnostics.CodeAnalysis;
using BrightSword.SwissKnife.Properties;

namespace BrightSword.SwissKnife;

public static class SequentialGuid
{
	private static int _a;

	private static short _b;

	private static short _c;

	static SequentialGuid()
	{
		InitializeFromSettings();
	}

	public static Guid NewSequentialGuid(int a = -1, short b = -1, short c = -1)
	{
		return new Guid((a == -1) ? _a : a, (b == -1) ? _b : b, (c == -1) ? _c : c, UniqueSequenceGenerator.NextAscendingUniqueValue.GetReversedBytes());
	}

	public static Guid NewReverseSequentialGuid(int a = -1, short b = -1, short c = -1)
	{
		return new Guid((a == -1) ? _a : a, (b == -1) ? _b : b, (c == -1) ? _c : c, UniqueSequenceGenerator.NextDescendingUniqueValue.GetReversedBytes());
	}

	private static void SaveSettings()
	{
		Settings.Default.Realm_UniqueID = (uint)_a;
		Settings.Default.Server_UniqueID = (ushort)_b;
		Settings.Default.Application_UniqueID = (ushort)_c;
		Settings.Default.Save();
	}

	[ExcludeFromCodeCoverage]
	private static void InitializeFromSettings()
	{
		Random random = new Random();
		for (_a = (int)Settings.Default.Realm_UniqueID; _a == 0; _a = random.Next())
		{
		}
		for (_b = (short)Settings.Default.Server_UniqueID; _b == 0; _b = (short)random.Next(32767))
		{
		}
		for (_c = (short)Settings.Default.Application_UniqueID; _c == 0; _c = (short)random.Next(32767))
		{
		}
		SaveSettings();
	}

	[ExcludeFromCodeCoverage]
	public static void Initialize(uint a, ushort b, ushort c)
	{
		_a = (int)a;
		_b = (short)b;
		_c = (short)c;
		SaveSettings();
	}
}
