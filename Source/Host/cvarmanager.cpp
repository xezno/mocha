#include "cvarmanager.h"

#include <sstream>

size_t CVarSystem::GetHash( std::string string )
{
	return std::hash<std::string>{}( string );
}

void CVarSystem::ForEach( std::function<void( CVarEntry& entry )> func )
{
	for ( auto& entry : m_cvarEntries )
	{
		func( entry.second );
	}
}

void CVarSystem::ForEach( std::string filter, std::function<void( CVarEntry& entry )> func )
{
	std::vector<CVarEntry> matchingEntries = {};

	for ( auto& item : m_cvarEntries )
	{
		if ( item.second.m_name.find( filter ) == std::string::npos )
			continue;

		matchingEntries.push_back( item.second );
	}

	for ( auto& entry : matchingEntries )
	{
		func( entry );
	}
}

void CVarSystem::FromString( std::string name, std::string valueStr )
{
	assert( Exists( name ) ); // Doesn't exist! Register it first

	std::stringstream valueStream( valueStr );
	size_t hash = GetHash( name );
	CVarEntry& entry = m_cvarEntries[hash];

	auto& type = entry.m_value.type();

	if ( type == typeid( float ) )
	{
		float value;
		valueStream >> value;

		Set<float>( entry.m_name, value );
	}
	else if ( type == typeid( bool ) )
	{
		bool value;

		if ( valueStr == "true" || valueStr == "1" || valueStr == "yes" )
			value = true;
		else if ( valueStr == "false" || valueStr == "0" || valueStr == "no" )
			value = false;
		else
			assert( false ); // Invalid bool value

		Set<bool>( entry.m_name, value );
	}
	else if ( type == typeid( std::string ) )
	{
		Set<std::string>( entry.m_name, valueStr );
	}
}

std::string CVarSystem::ToString( std::string name )
{
	assert( Exists( name ) ); // Doesn't exist! Register it first

	size_t hash = GetHash( name );
	CVarEntry& entry = m_cvarEntries[hash];

	const std::type_info& type = entry.m_value.type();
	std::string valueStr;

	if ( type == typeid( std::string ) )
		valueStr = std::any_cast<std::string>( entry.m_value );
	else if ( type == typeid( float ) )
		valueStr = std::to_string( std::any_cast<float>( entry.m_value ) );
	else if ( type == typeid( bool ) )
		valueStr = std::any_cast<bool>( entry.m_value ) ? "true" : "false";

	return valueStr;
}

void CVarManager::Startup()
{
	CVarSystem::Instance().Startup();
}

void CVarManager::Shutdown()
{
	CVarSystem::Instance().Shutdown();
}