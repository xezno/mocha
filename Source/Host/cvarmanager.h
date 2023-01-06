#pragma once
#include <any>
#include <fstream>
#include <globalvars.h>
#include <memory>
#include <nlohmann/json.hpp>
#include <spdlog/spdlog.h>
#include <string>
#include <subsystem.h>
#include <unordered_map>

enum CVarFlags
{
	None = 0,

	// Save this convar to cvars.json
	Archive = 1 << 0,

	// TODO
	Cheat = 1 << 1,

	// TODO
	Temp = 1 << 2
};

struct CVarEntry
{
	std::string m_name;
	std::string m_description;

	CVarFlags m_flags;

	std::any m_value;
};

class CVarParameter
{
protected:
	std::string m_name;

public:
	friend class CVarSystem;
};

class CVarManager : ISubSystem
{
public:
	void Startup() override;
	void Shutdown() override;
};

class CVarSystem
{
private:
	std::unordered_map<std::size_t, CVarEntry> m_cvarEntries;
	size_t GetHash( std::string string );

	template <typename T>
	void Register( std::string name, T value, CVarFlags flags, std::string description );

	template <typename T>
	T Get( std::string name );

	template <typename T>
	void Set( std::string name, T value );

public:
	friend class StringCVar;

	//
	// CVarSystem is a singleton because it needs creating *as soon as* it's referenced
	// and not after.
	//
	static CVarSystem& Instance()
	{
		static CVarSystem instance;
		return instance;
	}

	void Startup()
	{
		// Load all archive cvars from disk
		nlohmann::json cvarArchive;

		std::ifstream cvarFile( "cvars.json" );
		cvarFile >> cvarArchive;

		for ( auto& [name, value] : cvarArchive.items() )
		{
			assert( Exists( name ) ); // Doesn't exist! Register it first

			size_t hash = GetHash( name );
			CVarEntry& entry = m_cvarEntries[hash];

			if ( entry.m_flags & CVarFlags::Archive )
				FromString( name, value );
		}
	}

	void Shutdown()
	{
		// Save all archive cvars to disk
		nlohmann::json cvarArchive;

		for ( auto& entry : m_cvarEntries )
		{
			if ( entry.second.m_flags & CVarFlags::Archive )
				cvarArchive[entry.second.m_name] = ToString( entry.second.m_name );
		}

		std::ofstream cvarFile( "cvars.json" );
		cvarFile << std::setw( 4 ) << cvarArchive << std::endl;
	}

	void RegisterString( std::string name, std::string value, CVarFlags flags, std::string description )
	{
		Register<std::string>( name, value, flags, description );
	}

	void RegisterFloat( std::string name, float value, CVarFlags flags, std::string description )
	{
		Register<float>( name, value, flags, description );
	}

	void RegisterBool( std::string name, bool value, CVarFlags flags, std::string description )
	{
		Register<bool>( name, value, flags, description );
	}

	std::string GetString( std::string name ) { return Get<std::string>( name ); }
	float GetFloat( std::string name ) { return Get<float>( name ); }
	bool GetBool( std::string name ) { return Get<bool>( name ); }

	void FromString( std::string name, std::string valueStr );
	std::string ToString( std::string name );

	void SetString( std::string name, std::string value ) { Set<std::string>( name, value ); }
	void SetFloat( std::string name, float value ) { Set<float>( name, value ); }
	void SetBool( std::string name, bool value ) { Set<bool>( name, value ); }

	void ForEach( std::function<void( CVarEntry& entry )> func );
	void ForEach( std::string filter, std::function<void( CVarEntry& entry )> func );

	bool Exists( std::string name ) { return m_cvarEntries.find( GetHash( name ) ) != m_cvarEntries.end(); }
};

class StringCVar : CVarParameter
{
public:
	StringCVar( std::string name, std::string value, CVarFlags flags, std::string description )
	{
		m_name = name;

		CVarSystem::Instance().RegisterString( name, value, flags, description );
	}

	std::string GetValue() { return CVarSystem::Instance().GetString( m_name ); }
	void SetValue( std::string value ) { CVarSystem::Instance().SetString( m_name, value ); }

	operator std::string() { return GetValue(); }
};

class FloatCVar : CVarParameter
{
public:
	FloatCVar( std::string name, float value, CVarFlags flags, std::string description )
	{
		m_name = name;

		CVarSystem::Instance().RegisterFloat( name, value, flags, description );
	}

	float GetValue() { return CVarSystem::Instance().GetFloat( m_name ); }
	void SetValue( float value ) { CVarSystem::Instance().SetFloat( m_name, value ); }
	
	operator float() { return GetValue(); }
};

class BoolCVar : CVarParameter
{
public:
	BoolCVar( std::string name, bool value, CVarFlags flags, std::string description )
	{
		m_name = name;

		CVarSystem::Instance().RegisterBool( name, value, flags, description );
	}

	bool GetValue() { return CVarSystem::Instance().GetBool( m_name ); }
	void SetValue( bool value ) { CVarSystem::Instance().SetBool( m_name, value ); }

	operator bool() { return GetValue(); };
};

template <typename T>
inline void CVarSystem::Register( std::string name, T value, CVarFlags flags, std::string description )
{
	CVarEntry entry = {};
	entry.m_name = name;
	entry.m_description = description;
	entry.m_flags = flags;
	entry.m_value = value;

	size_t hash = GetHash( name );
	m_cvarEntries[hash] = entry;
}

template <typename T>
inline T CVarSystem::Get( std::string name )
{
	assert( Exists( name ) ); // Doesn't exist! Register it first

	size_t hash = GetHash( name );
	CVarEntry& entry = m_cvarEntries[hash];

	return std::any_cast<T>( entry.m_value );
}

template <typename T>
inline void CVarSystem::Set( std::string name, T value )
{
	assert( Exists( name ) ); // Doesn't exist! Register it first

	size_t hash = GetHash( name );
	CVarEntry& entry = m_cvarEntries[hash];

	entry.m_value = value;

	spdlog::info( "{} was set to '{}'.", entry.m_name, value );
}