﻿namespace Mocha.Common;

/// <summary>
/// A NetworkId is a wrapper around a 64-bit unsigned integer value used to identify an entity.
/// The first bit of the value is used in order to determine whether the value is networked or local.
/// 
/// For example, take an entity with the ID 12345678:
/// Local
/// `00000000000000000000000000000000000000000000000000000000010111101`
/// 
/// Networked
/// `10000000000000000000000000000000000000000000000000000000010111101`
/// 
/// Note that the first bit is set to 1 in the binary representation of the networked entity.
/// The IDs used within this should not reflect the native engine handle for the entity - NetworkIds
/// are unique to a managed context.Note that the same ID (eg. 12345678) can be used twice - once locally, and once networked - to refer to two distinct entities.
/// </summary>
public class NetworkId
{
	internal ulong Value { get; private set; }
	internal NetworkId( ulong value )
	{
		Value = value;
	}
	public bool IsNetworked()
	{
		// If first bit of the value is set, it's a networked entity
		return (Value & 0x8000000000000000) != 0;
	}
	public bool IsLocal()
	{
		// If first bit of the value is not set, it's a local entity
		return (Value & 0x8000000000000000) == 0;
	}

	public ulong GetValue()
	{
		// Returns the value without the first bit
		return Value & 0x7FFFFFFFFFFFFFFF;
	}

	public static NetworkId CreateLocal()
	{
		// Create a local entity by setting the first bit to 0
		// Use EntityRegistry.Instance to get the next available local id
		return new( (uint)EntityRegistry.Instance.Count() << 1 );
	}

	public static NetworkId CreateNetworked()
	{
		// Create a networked entity by setting the first bit to 1
		// Use EntityRegistry.Instance to get the next available local id
		return new( (uint)EntityRegistry.Instance.Count() | 0x8000000000000000 );
	}

	public static implicit operator ulong( NetworkId id ) => id.GetValue();
	public static implicit operator NetworkId( ulong value ) => new( value );

	public override string ToString()
	{
		return $"{(IsNetworked() ? "Networked" : "Local")}: {GetValue()} ({Value})";
	}
}